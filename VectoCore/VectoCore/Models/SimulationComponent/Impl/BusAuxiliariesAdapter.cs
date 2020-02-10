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

		protected internal readonly IAuxPort AdditionalAux;

		protected IBusAuxiliaries Auxiliaries;

		//private readonly FuelConsumptionAdapter _fcMapAdapter;

		public BusAuxiliariesAdapter(
			IVehicleContainer container, IAuxiliaryConfig auxiliaryConfig, IAuxPort additionalAux = null)
		{
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

		public Watt PowerDemandEngineOn(PerSecond engineSpeed)
		{
			throw new NotImplementedException();
		}

		public Watt PowerDemandEngineOff()
		{
			throw new NotImplementedException();
		}


		protected internal void DoWriteModalResults(IModalDataContainer container)
		{
			//_fcMapAdapter.AllowExtrapolation = true;
			// cycleStep has to be called here and not in DoCommit, write is called before Commit!
			var message = String.Empty;
			Auxiliaries.CycleStep(CurrentState.dt);
			Log.Warn(message);

			//CurrentState.TotalFuelConsumption = Auxiliaries.TotalFuel;
			container[ModalResultField.P_aux] = CurrentState.PowerDemand;

			container[ModalResultField.P_busAux_ES_HVAC] = Auxiliaries.HVACElectricalPowerConsumer;
			container[ModalResultField.P_busAux_ES_other] = Auxiliaries.ElectricPowerConsumer;
			container[ModalResultField.P_busAux_ES_consumer_sum] = Auxiliaries.ElectricPowerConsumerSum;
			container[ModalResultField.P_busAux_ES_generated] = Auxiliaries.ElectricPowerGenerated;
			container[ModalResultField.P_busAux_ES_sum_mech] = Auxiliaries.ElectricPowerDemandMech;

			container[ModalResultField.BatterySOC] = Auxiliaries.BatterySOC * 100.0;

			container[ModalResultField.Nl_busAux_PS_consumer] = Auxiliaries.PSDemandConsumer;
			container[ModalResultField.Nl_busAux_PS_generated] = Auxiliaries.PSAirGenerated;
			container[ModalResultField.Nl_busAux_PS_generated_alwaysOn] = Auxiliaries.PSAirGeneratedAlwaysOn;
			//container[ModalResultField.Nl_busAux_PS_generated_dragOnly] = Auxiliaries.PSAirGeneratedDrag;
			container[ModalResultField.P_busAux_PS_generated] = Auxiliaries.PSPowerDemandAirGenerated;
			container[ModalResultField.P_busAux_PS_generated_alwaysOn] = Auxiliaries.PSPowerCompressorAlwaysOn;
			container[ModalResultField.P_busAux_PS_generated_dragOnly] = Auxiliaries.PSPowerCompressorDragOnly;

			container[ModalResultField.P_busAux_HVACmech_consumer] = Auxiliaries.HVACMechanicalPowerConsumer;
			container[ModalResultField.P_busAux_HVACmech_gen] = Auxiliaries.HVACMechanicalPowerGenerated;
		}

		protected internal void DoCommitSimulationStep()
		{
			PreviousState = CurrentState;
			CurrentState = new BusAuxState();
		}

		//protected internal KilogramPerSecond AAuxFuelConsumption
		//{
		//    get { return (CurrentState.TotalFuelConsumption - PreviousState.TotalFuelConsumption) / CurrentState.dt; }
		//}

		private Watt GetBusAuxPowerDemand(
			Second absTime, Second dt, NewtonMeter torquePowerTrain, NewtonMeter torqueEngine,
			PerSecond angularSpeed, bool dryRun = false)
		{
			Auxiliaries.ResetCalculations();

			//_fcMapAdapter.AllowExtrapolation = true;

			var signals = Auxiliaries.Signals;

			signals.SimulationInterval = dt;
			signals.ClutchEngaged = DataBus.ClutchClosed(absTime);
			signals.EngineDrivelineTorque = torquePowerTrain;
			//Auxiliaries.Signals.EngineDrivelinePower = torquePowerTrain * angularSpeed;
			//Auxiliaries.Signals.InternalEnginePower = torqueEngine * angularSpeed - DataBus.BrakePower;
			//if (DataBus.DriverBehavior == DrivingBehavior.Coasting) {
			// make sure smart aux are _not_ enabled for now
			// set internal_engine_power a little bit lower so there is no excessive power for smart aux
			//Auxiliaries.Signals.InternalEnginePower = 3 * torqueEngine * angularSpeed /*- DataBus.BrakePower*/;

			// if smart aux should be on during coasting use the following line
			// set internal_engine_power to a large value (*10) so that there's excessive power for smart aux (alreadin during search operating point)
			//(float)DataBus.EngineDragPower(angularSpeed).Value() / 100;
			//} else {
			// Toodo: change to driveraction 
			//if (DataBus.DriverBehavior != DrivingBehavior.Braking) {
			//	Auxiliaries.Signals.InternalEnginePower = 0.SI<Watt>();

			//	//(float)((0.9 * torqueEngine * angularSpeed - DataBus.BrakePower) / 1000).Value();
			//} else {
			//	// smart aux should be on during braking
			//}
			//}
			//Auxiliaries.Signals.EngineMotoringPower = -DataBus.EngineDragPower(angularSpeed);

			signals.EngineSpeed = angularSpeed;
			var avgAngularSpeed = (PreviousState.AngularSpeed + CurrentState.AngularSpeed) / 2;
			signals.PreExistingAuxPower = AdditionalAux != null
				? AdditionalAux.TorqueDemand(absTime, dt, torquePowerTrain, torqueEngine, angularSpeed, dryRun) * avgAngularSpeed
				: 0.SI<Watt>();

			//var avgEngineSpeed = (DataBus.EngineSpeed + angularSpeed) / 2.0;
			var drivetrainPower = torquePowerTrain * avgAngularSpeed;

			signals.ExcessiveDragPower = DataBus.DrivingAction == DrivingAction.Brake ?
				drivetrainPower - (DataBus.EngineDragPower(avgAngularSpeed) - signals.PreExistingAuxPower) - DataBus.BrakePower:
				0.SI<Watt>();

			

			//mAAUX_Global.PreExistingAuxPower;
			signals.Idle = DataBus.VehicleStopped;
			signals.InNeutral = DataBus.Gear == 0;

			//Auxiliaries.Signals.RunningCalc = true;

			//mAAUX_Global.Internal_Engine_Power;
			//'Power coming out of Advanced Model is in Watts.

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
			public Kilogram TotalFuelConsumption = 0.SI<Kilogram>();
		}
	}
}
