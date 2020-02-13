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
using TUGraz.VectoCore.Models.SimulationComponent.Data.Engine;
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

		//private readonly FuelConsumptionAdapter _fcMapAdapter;

		public BusAuxiliariesAdapter(
			IVehicleContainer container, IAuxiliaryConfig auxiliaryConfig, IAuxPort additionalAux = null)
		{

			EngineStopStartUtilityFactor = container.RunData?.DriverData?.EngineStopStart?.UtilityFactor ?? double.NaN;

			CurrentState = new BusAuxState();
			PreviousState = new BusAuxState { AngularSpeed = container.EngineIdleSpeed };

			AdditionalAux = additionalAux;

			DataBus = container;

			var tmpAux = new BusAuxiliaries.BusAuxiliaries();

			//'Set Signals
			tmpAux.Signals.EngineIdleSpeed = container.EngineIdleSpeed;
			tmpAux.Initialise(auxiliaryConfig);

			Auxiliaries = tmpAux;
		}

		private static string DetermineCycle(string cycleName)
		{
			var cycle = cycleName.ToLower();

			if (cycle.Contains("bus")) {
				if (cycle.Contains("heavy_urban")) {
					return "Heavy urban";
				}
				if (cycle.Contains("suburban")) {
					return "Suburban";
				}
				if (cycle.Contains("interurban")) {
					return "Interurban";
				}
				if (cycle.Contains("urban")) {
					return "Urban";
				}
			}

			if (cycle.Contains("coach")) {
				return "Coach";
			}

			Logger<BusAuxiliariesAdapter>()
				.Warn("UnServiced Cycle Name '{0}' in Pneumatics Actuations Map 0 Actuations returned", cycleName);
			return cycleName;
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
			PreviousState.PowerDemand = GetBusAuxPowerDemand(0.SI<Second>(), 1.SI<Second>(), torque, torque, angularSpeed);
			return PreviousState.PowerDemand / angularSpeed;
		}


		public NewtonMeter TorqueDemand(
			Second absTime, Second dt, NewtonMeter torquePowerTrain, NewtonMeter torqueEngine,
			PerSecond angularSpeed, bool dryRun = false)
		{
			CurrentState.AngularSpeed = angularSpeed;
			CurrentState.dt = dt;
			CurrentState.PowerDemand = GetBusAuxPowerDemand(absTime, dt, torquePowerTrain, torqueEngine, angularSpeed, dryRun);

			var avgAngularSpeed = (CurrentState.AngularSpeed + PreviousState.AngularSpeed) / 2.0;
			return CurrentState.PowerDemand / avgAngularSpeed;
		}

		public Watt PowerDemandEngineOn(Second time, Second simulationInterval, PerSecond engineSpeed)
		{
			return GetBusAuxPowerDemand(time, simulationInterval, 0.SI<NewtonMeter>(), 0.SI<NewtonMeter>(), engineSpeed, true);
		}

		public Watt PowerDemandEngineOff(Second absTime, Second dt)
		{
			var conventionalAux = AdditionalAux;
			AdditionalAux = null;
			CurrentState.AngularSpeed = DataBus.EngineIdleSpeed;
			CurrentState.dt = dt;
			var busAuxPowerDemand  = GetBusAuxPowerDemand(
				absTime, dt, 0.SI<NewtonMeter>(), 0.SI<NewtonMeter>(), DataBus.EngineIdleSpeed);
			AdditionalAux = conventionalAux;

			CurrentState.PowerDemand = ((AdditionalAux?.PowerDemandEngineOn(absTime, dt, DataBus.EngineIdleSpeed) ?? 0.SI<Watt>()) +
										busAuxPowerDemand) * (1 - EngineStopStartUtilityFactor);

			return EngineStopStartUtilityFactor * busAuxPowerDemand + AdditionalAux?.PowerDemandEngineOff(absTime, dt);
		}


		protected internal virtual void DoWriteModalResults(IModalDataContainer container)
		{
			
			// cycleStep has to be called here and not in DoCommit, write is called before Commit!
			Auxiliaries.CycleStep(CurrentState.dt);

			var essUtilityFactor = 1.0;
			if (!DataBus.IgnitionOn) {
				essUtilityFactor = 1 - EngineStopStartUtilityFactor;
			}

			//CurrentState.TotalFuelConsumption = Auxiliaries.TotalFuel;
			container[ModalResultField.P_aux] = CurrentState.PowerDemand;

			container[ModalResultField.P_busAux_ES_HVAC] = Auxiliaries.HVACElectricalPowerConsumer;
			container[ModalResultField.P_busAux_ES_other] = Auxiliaries.ElectricPowerConsumer;
			container[ModalResultField.P_busAux_ES_consumer_sum] = Auxiliaries.ElectricPowerConsumerSum;
			container[ModalResultField.P_busAux_ES_sum_mech] = essUtilityFactor * Auxiliaries.ElectricPowerDemandMech;
			container[ModalResultField.P_busAux_ES_generated] = essUtilityFactor * Auxiliaries.ElectricPowerGenerated;

			container[ModalResultField.BatterySOC] = Auxiliaries.BatterySOC * 100.0;

			container[ModalResultField.Nl_busAux_PS_consumer] = Auxiliaries.PSDemandConsumer;
			container[ModalResultField.Nl_busAux_PS_generated] = essUtilityFactor * Auxiliaries.PSAirGenerated;
			container[ModalResultField.Nl_busAux_PS_generated_alwaysOn] = essUtilityFactor * Auxiliaries.PSAirGeneratedAlwaysOn;
			//container[ModalResultField.Nl_busAux_PS_generated_dragOnly] = Auxiliaries.PSAirGeneratedDrag;
			container[ModalResultField.P_busAux_PS_generated] = essUtilityFactor * Auxiliaries.PSPowerDemandAirGenerated;
			container[ModalResultField.P_busAux_PS_generated_alwaysOn] = essUtilityFactor * Auxiliaries.PSPowerCompressorAlwaysOn;
			container[ModalResultField.P_busAux_PS_generated_dragOnly] = essUtilityFactor * Auxiliaries.PSPowerCompressorDragOnly;

			container[ModalResultField.P_busAux_HVACmech_consumer] = Auxiliaries.HVACMechanicalPowerConsumer;
			container[ModalResultField.P_busAux_HVACmech_gen] = essUtilityFactor *  Auxiliaries.HVACMechanicalPowerGenerated;
		}

		protected internal virtual void DoCommitSimulationStep()
		{
			PreviousState = CurrentState;
			CurrentState = new BusAuxState();
		}

		protected virtual Watt GetBusAuxPowerDemand(
			Second absTime, Second dt, NewtonMeter torquePowerTrain, NewtonMeter torqueEngine,
			PerSecond angularSpeed, bool dryRun = false)
		{
			Auxiliaries.ResetCalculations();

			var signals = Auxiliaries.Signals;

			signals.SimulationInterval = dt;
			signals.ClutchEngaged = DataBus.ClutchClosed(absTime);
			signals.EngineDrivelineTorque = torquePowerTrain;
			
			signals.EngineSpeed = angularSpeed;
			var avgAngularSpeed = (PreviousState.AngularSpeed + CurrentState.AngularSpeed) / 2;


			signals.PreExistingAuxPower = AdditionalAux != null
				? AdditionalAux.TorqueDemand(absTime, dt, torquePowerTrain, torqueEngine, angularSpeed, dryRun) * avgAngularSpeed
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

		protected class FuelConsumptionAdapter : IFuelConsumptionMap
		{
			protected internal FuelConsumptionMap FcMap;

			public bool AllowExtrapolation { get; set; }

			public KilogramPerSecond GetFuelConsumptionValue(NewtonMeter torque, PerSecond angularVelocity)
			{
				return FcMap.GetFuelConsumption(torque, angularVelocity, AllowExtrapolation).Value;
			}
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
