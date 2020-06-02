/*
* This file is part of VECTO.
*
* Copyright © 2012-2019 European Union
*
* Developed by Graz University of Technology,
*              Institute of Internal Combustion Engines and Thermodynamics,
*              Institute of Technical Informatics
*
* VECTO is licensed under the EUPL, Version 1.1 or - as soon they will be approved
* by the European Commission - subsequent versions of the EUPL (the "Licence");
* You may not use VECTO except in compliance with the Licence.
* You may obtain a copy of the Licence at:
*
* https://joinup.ec.europa.eu/community/eupl/og_page/eupl
*
* Unless required by applicable law or agreed to in writing, VECTO
* distributed under the Licence is distributed on an "AS IS" basis,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the Licence for the specific language governing permissions and
* limitations under the Licence.
*
* Authors:
*   Stefan Hausberger, hausberger@ivt.tugraz.at, IVT, Graz University of Technology
*   Christian Kreiner, christian.kreiner@tugraz.at, ITI, Graz University of Technology
*   Michael Krisper, michael.krisper@tugraz.at, ITI, Graz University of Technology
*   Raphael Luz, luz@ivt.tugraz.at, IVT, Graz University of Technology
*   Markus Quaritsch, markus.quaritsch@tugraz.at, IVT, Graz University of Technology
*   Martin Rexeis, rexeis@ivt.tugraz.at, IVT, Graz University of Technology
*/

using System;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.BusAuxiliaries;
using TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
	public class BusAuxiliariesAdapter : LoggingObject, IAuxInProvider, IAuxPort
	{
		protected readonly IDataBus DataBus;
		protected internal BusAuxState CurrentState;
		protected internal BusAuxState PreviousState;

		protected internal IAuxPort AdditionalAux;

		protected IBusAuxiliaries Auxiliaries;

		private double EngineStopStartUtilityFactor;
		private bool SmartElectricSystem;
		private IAuxiliaryConfig AuxCfg;

		//private readonly FuelConsumptionAdapter _fcMapAdapter;

		public BusAuxiliariesAdapter(
			IVehicleContainer container, IAuxiliaryConfig auxiliaryConfig, IAuxPort additionalAux = null)
		{

			EngineStopStartUtilityFactor = container.RunData?.DriverData?.EngineStopStart?.UtilityFactor ?? double.NaN;

			CurrentState = new BusAuxState();
			PreviousState = new BusAuxState { AngularSpeed = container.EngineIdleSpeed };

			AdditionalAux = additionalAux;
			AuxCfg = auxiliaryConfig;
			DataBus = container;

			var tmpAux = new BusAuxiliaries.BusAuxiliaries(container.ModalData);

			//'Set Signals
			tmpAux.Signals.EngineIdleSpeed = container.EngineIdleSpeed;
			tmpAux.Initialise(auxiliaryConfig);

			SmartElectricSystem = auxiliaryConfig.ElectricalUserInputsConfig.SmartElectrical;

			Auxiliaries = tmpAux;
		}

		
		public IAuxPort Port()
		{
			return this;
		}

		public NewtonMeter Initialize(NewtonMeter torque, PerSecond angularSpeed)
		{
			//PreviousState.TotalFuelConsumption = 0.SI<Kilogram>();
			PreviousState.AngularSpeed = angularSpeed;
			CurrentState.AngularSpeed = angularSpeed;
			if (AdditionalAux != null) {
				AdditionalAux.Initialize(torque, angularSpeed);
			}
			PreviousState.PowerDemand = GetBusAuxPowerDemand(0.SI<Second>(), 1.SI<Second>(), torque, angularSpeed);
			return PreviousState.PowerDemand / angularSpeed;
		}


		public NewtonMeter TorqueDemand(Second absTime, Second dt, NewtonMeter torquePowerTrain, PerSecond angularSpeed, bool dryRun = false)
		{
			CurrentState.AngularSpeed = angularSpeed;
			CurrentState.dt = dt;

			var signals = Auxiliaries.Signals;
			// trick bus auxiliaries that ice is on - all auxiliaries are considered. ESS is corrected in post-processing
			signals.EngineStopped = false; 
			signals.VehicleStopped = false; 

			CurrentState.PowerDemand = GetBusAuxPowerDemand(absTime, dt, torquePowerTrain, angularSpeed, dryRun);

			var avgAngularSpeed = (CurrentState.AngularSpeed + PreviousState.AngularSpeed) / 2.0;
			return CurrentState.PowerDemand / avgAngularSpeed;
		}

		public Watt PowerDemandEngineOn(Second time, Second simulationInterval, PerSecond engineSpeed)
		{
			var signals = Auxiliaries.Signals;
			signals.EngineStopped = false; 
			signals.VehicleStopped = false;
			var retVal =  GetBusAuxPowerDemand(time, simulationInterval, 0.SI<NewtonMeter>(), engineSpeed, true);

			if (!SmartElectricSystem) {
				return retVal;
			}

			var batteryPwr = Auxiliaries.BatterySOC * AuxCfg.ElectricalUserInputsConfig.ElectricStorageCapacity / simulationInterval;
			var esSum = Auxiliaries.ElectricPowerConsumerSum;
			//if (batteryPwr < esSum) {
				retVal += (esSum ) / AuxCfg.ElectricalUserInputsConfig.AlternatorGearEfficiency /
						AuxCfg.ElectricalUserInputsConfig.AlternatorMap.GetEfficiency(0.RPMtoRad(), 0.SI<Ampere>());
			//}

			return retVal;
		}

		public Watt PowerDemandEngineOff(Second absTime, Second dt)
		{
			var conventionalAux = AdditionalAux;
			AdditionalAux = null;
			CurrentState.AngularSpeed = DataBus.EngineIdleSpeed;
			CurrentState.dt = dt;

			var signals = Auxiliaries.Signals;

			// set internal state of power demand as if ICE is on - multiplied by (1-ESS_UF) 
			signals.EngineStopped = false;
			signals.VehicleStopped = false;

			var busAuxPowerDemand  = GetBusAuxPowerDemand(
				absTime, dt, 0.SI<NewtonMeter>(), DataBus.EngineIdleSpeed);
			AdditionalAux = conventionalAux;

			CurrentState.PowerDemand = ((AdditionalAux?.PowerDemandEngineOn(absTime, dt, DataBus.EngineIdleSpeed) ?? 0.SI<Watt>()) +
										busAuxPowerDemand) * (1 - EngineStopStartUtilityFactor);
			//CurrentState.ESPowerGeneratedICE_On = Auxiliaries.ElectricPowerGenerated;
			//CurrentState.ESPowerMech = Auxiliaries.ElectricPowerDemandMech;
			// 
			signals.EngineStopped = !DataBus.CombustionEngineOn;
			signals.VehicleStopped = DataBus.VehicleStopped;

			busAuxPowerDemand = GetBusAuxPowerDemand(
				absTime, dt, 0.SI<NewtonMeter>(), DataBus.EngineIdleSpeed);
			AdditionalAux = conventionalAux;

			return EngineStopStartUtilityFactor * (busAuxPowerDemand + AdditionalAux?.PowerDemandEngineOff(absTime, dt));
		}


		protected internal virtual void DoWriteModalResults(Second absTime, Second dt, IModalDataContainer container)
		{
			var essUtilityFactor = 1.0;
			if (!DataBus.CombustionEngineOn) {
				essUtilityFactor = 1 - EngineStopStartUtilityFactor;
			}

			//var signals = Auxiliaries.Signals;
			//signals.EngineStopped = !DataBus.IgnitionOn;
			//signals.VehicleStopped = DataBus.VehicleStopped;

			// cycleStep has to be called here and not in DoCommit, write is called before Commit!
			var oldSOC = Auxiliaries.BatterySOC;
			Auxiliaries.CycleStep(CurrentState.dt, DataBus.CombustionEngineOn ? 1.0 : EngineStopStartUtilityFactor);
			var newSOC = Auxiliaries.BatterySOC;

			//CurrentState.TotalFuelConsumption = Auxiliaries.TotalFuel;
			container[ModalResultField.P_aux_mech] = CurrentState.PowerDemand;

			container[ModalResultField.P_busAux_ES_HVAC] = essUtilityFactor * Auxiliaries.HVACElectricalPowerConsumer;
			container[ModalResultField.P_busAux_ES_other] = essUtilityFactor * Auxiliaries.ElectricPowerConsumer;
			container[ModalResultField.P_busAux_ES_consumer_sum] = essUtilityFactor * Auxiliaries.ElectricPowerConsumerSum;
			container[ModalResultField.P_busAux_ES_sum_mech] = essUtilityFactor * Auxiliaries.ElectricPowerDemandMech;
			container[ModalResultField.P_busAux_ES_generated] = essUtilityFactor * Auxiliaries.ElectricPowerGenerated;

			if (SmartElectricSystem) {
				var batteryPwr = (oldSOC - newSOC) * AuxCfg.ElectricalUserInputsConfig.ElectricStorageCapacity / dt;

				container[ModalResultField.BatterySOC] = Auxiliaries.BatterySOC * 100.0;
				
				container[ModalResultField.P_busAux_ES_generated] = essUtilityFactor * (DataBus.VehicleStopped && !DataBus.CombustionEngineOn ? Auxiliaries.ElectricPowerConsumerSum : Auxiliaries.ElectricPowerGenerated);
				container[ModalResultField.P_busAux_ES_sum_mech] = essUtilityFactor * (Auxiliaries.ElectricPowerConsumerSum - batteryPwr) /
																	AuxCfg.ElectricalUserInputsConfig.AlternatorGearEfficiency /
																	AuxCfg.ElectricalUserInputsConfig.AlternatorMap.GetEfficiency(0.RPMtoRad(), 0.SI<Ampere>());
				
				if (batteryPwr.IsSmaller(Auxiliaries.ElectricPowerConsumerSum * EngineStopStartUtilityFactor)) {
					// add to P_aux_ES
				}
			}

			container[ModalResultField.Nl_busAux_PS_consumer] = Auxiliaries.PSDemandConsumer;
			container[ModalResultField.Nl_busAux_PS_generated] = essUtilityFactor * Auxiliaries.PSAirGenerated;
			container[ModalResultField.Nl_busAux_PS_generated_alwaysOn] = essUtilityFactor * Auxiliaries.PSAirGeneratedAlwaysOn;
			//container[ModalResultField.Nl_busAux_PS_generated_dragOnly] = Auxiliaries.PSAirGeneratedDrag;
			container[ModalResultField.P_busAux_PS_generated] = essUtilityFactor * Auxiliaries.PSPowerDemandAirGenerated;
			container[ModalResultField.P_busAux_PS_generated_alwaysOn] = essUtilityFactor * Auxiliaries.PSPowerCompressorAlwaysOn;
			container[ModalResultField.P_busAux_PS_generated_dragOnly] = essUtilityFactor * Auxiliaries.PSPowerCompressorDragOnly;

			container[ModalResultField.P_busAux_HVACmech_consumer] = essUtilityFactor * Auxiliaries.HVACMechanicalPowerConsumer;
			container[ModalResultField.P_busAux_HVACmech_gen] = essUtilityFactor *  Auxiliaries.HVACMechanicalPowerGenerated;
		}

		protected internal virtual void DoCommitSimulationStep()
		{
			PreviousState = CurrentState;
			CurrentState = new BusAuxState();
		}

		protected virtual Watt GetBusAuxPowerDemand(Second absTime, Second dt, NewtonMeter torquePowerTrain, PerSecond angularSpeed, bool dryRun = false)
		{
			Auxiliaries.ResetCalculations();

			var signals = Auxiliaries.Signals;

			signals.SimulationInterval = dt;
			signals.ClutchEngaged = DataBus.ClutchClosed(absTime);
			signals.EngineDrivelineTorque = torquePowerTrain;
			
			signals.EngineSpeed = angularSpeed;
			var avgAngularSpeed = (PreviousState.AngularSpeed + CurrentState.AngularSpeed) / 2;


			signals.PreExistingAuxPower = AdditionalAux != null
				? AdditionalAux.TorqueDemand(absTime, dt, torquePowerTrain, angularSpeed, dryRun) * avgAngularSpeed
				: 0.SI<Watt>();

			var drivetrainPower = torquePowerTrain * avgAngularSpeed;
			if (!dryRun && DataBus.DrivingAction == DrivingAction.Brake && CurrentState.ExcessiveDragPower.IsEqual(0)) {
				CurrentState.ExcessiveDragPower = drivetrainPower -
												(DataBus.EngineDragPower(avgAngularSpeed) - signals.PreExistingAuxPower) - DataBus.BrakePower;
			}
			if (!dryRun && DataBus.DrivingAction != DrivingAction.Brake) {
				CurrentState.ExcessiveDragPower = 0.SI<Watt>();
			}

			
			signals.ExcessiveDragPower = CurrentState.ExcessiveDragPower;
			signals.Idle = DataBus.VehicleStopped;
			signals.InNeutral = DataBus.Gear == 0;

			

			return Auxiliaries.AuxiliaryPowerAtCrankWatts + signals.PreExistingAuxPower;
		}

		public class BusAuxState
		{
			public Second dt;
			public PerSecond AngularSpeed;
			public Watt PowerDemand;
			
			public Watt ExcessiveDragPower = 0.SI<Watt>();
		}
	}
}
