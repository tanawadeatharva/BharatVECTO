/*
* This file is part of VECTO.
*
* Copyright © 2012-2016 European Union
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
using System.IO;
using System.Windows.Forms.VisualStyles;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Engine;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.Utils;
using VectoAuxiliaries;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
	public class BusAuxiliariesAdapter : StatefulVectoSimulationComponent<BusAuxiliariesAdapter.BusAuxState>,
		IEngineAuxInProvider, IEngineAuxPort
	{
		protected IAdvancedAuxiliaries Auxiliaries;
		private readonly FuelConsumptionAdapter _fcMapAdapter;


		public BusAuxiliariesAdapter(IVehicleContainer container, string aauxFile, string cycleName, Kilogram vehicleWeight,
			FuelConsumptionMap fcMap,
			PerSecond engineIdleSpeed) : base(container)
		{
			//	mAAUX_Global.advancedAuxModel.Signals.DeclarationMode = Cfg.DeclMode
			//	mAAUX_Global.advancedAuxModel.Signals.WHTC = Declaration.WHTCcorrFactor

			var tmpAux = new AdvancedAuxiliaries();

			// 'Set Statics
			tmpAux.VectoInputs.Cycle = DetermineCycle(cycleName, tmpAux.Signals);
			tmpAux.VectoInputs.VehicleWeightKG = (float)vehicleWeight.Value();
			_fcMapAdapter = new FuelConsumptionAdapter() { FcMap = fcMap };
			tmpAux.VectoInputs.FuelMap = _fcMapAdapter;
			tmpAux.VectoInputs.FuelDensity = Physics.FuelDensity.Value();

			//'Set Signals
			tmpAux.Signals.EngineIdleSpeed = (float)(engineIdleSpeed.Value() / Constants.RPMToRad);
			tmpAux.Initialise(Path.GetFileName(aauxFile), Path.GetDirectoryName(Path.GetFullPath(aauxFile)) + @"\");

			Auxiliaries = tmpAux;
		}

		private static string DetermineCycle(string cycleName, ISignals aauxsignals)
		{
			var cycle = cycleName.ToLower();

			// cycle time is hard coded based on previous simulations
			if (cycle.Contains("bus")) {
				if (cycle.Contains("heavy_urban")) {
					aauxsignals.TotalCycleTimeSeconds = 8912;
					return "Heavy urban";
				}
				if (cycle.Contains("suburban")) {
					aauxsignals.TotalCycleTimeSeconds = 3283;
					return "Suburban";
				}
				if (cycle.Contains("interurban")) {
					aauxsignals.TotalCycleTimeSeconds = 12962;
					return "Interurban";
				}
				if (cycle.Contains("urban")) {
					aauxsignals.TotalCycleTimeSeconds = 8149;
					return "Urban";
				}
			}
			if (cycle.Contains("coach")) {
				aauxsignals.TotalCycleTimeSeconds = 15086;
				return "Coach";
			}
			Logger<BusAuxiliariesAdapter>()
				.Warn("UnServiced Cycle Name '{0}' in Pneumatics Actuations Map 0 Actuations returned", cycleName);
			aauxsignals.TotalCycleTimeSeconds = 1;
			return "UnknownCycleName";
		}

		public IEngineAuxPort Port()
		{
			return this;
		}

		public NewtonMeter Initialize(NewtonMeter torque, PerSecond angularSpeed)
		{
			PreviousState.AngularSpeed = angularSpeed;
			PreviousState.PowerDemand = GetBusAuxPowerDemand(0.SI<Second>(), 1.SI<Second>(), torque, torque, angularSpeed);
			return PreviousState.PowerDemand / angularSpeed;
		}


		public NewtonMeter PowerDemand(Second absTime, Second dt, NewtonMeter torquePowerTrain, NewtonMeter torqueEngine,
			PerSecond angularSpeed, bool dryRun = false)
		{
			CurrentState.AngularSpeed = angularSpeed;
			CurrentState.dt = dt;
			CurrentState.PowerDemand = GetBusAuxPowerDemand(absTime, dt, torquePowerTrain, torqueEngine, angularSpeed, dryRun);

			var avgAngularSpeed = (CurrentState.AngularSpeed + PreviousState.AngularSpeed) / 2.0;
			return CurrentState.PowerDemand / avgAngularSpeed;
		}


		protected override void DoWriteModalResults(IModalDataContainer container)
		{
			_fcMapAdapter.AllowExtrapolation = true;
			// cycleStep has to be called here and not in DoCommit, write is called before Commit!
			var message = String.Empty;
			Auxiliaries.CycleStep(CurrentState.dt.Value(), ref message);
			Log.Warn(message);

			container[ModalResultField.P_aux] = CurrentState.PowerDemand;

			container[ModalResultField.AA_NonSmartAlternatorsEfficiency] = Auxiliaries.AA_NonSmartAlternatorsEfficiency;
			if (Auxiliaries.AA_SmartIdleCurrent_Amps != null) {
				container[ModalResultField.AA_SmartIdleCurrent_Amps] = Auxiliaries.AA_SmartIdleCurrent_Amps.Value.SI<Ampere>();
			}
			container[ModalResultField.AA_SmartIdleAlternatorsEfficiency] = Auxiliaries.AA_SmartIdleAlternatorsEfficiency;
			if (Auxiliaries.AA_SmartTractionCurrent_Amps != null) {
				container[ModalResultField.AA_SmartTractionCurrent_Amps] =
					Auxiliaries.AA_SmartTractionCurrent_Amps.Value.SI<Ampere>();
			}
			container[ModalResultField.AA_SmartTractionAlternatorEfficiency] = Auxiliaries.AA_SmartTractionAlternatorEfficiency;
			if (Auxiliaries.AA_SmartOverrunCurrent_Amps != null) {
				container[ModalResultField.AA_SmartOverrunCurrent_Amps] = Auxiliaries.AA_SmartOverrunCurrent_Amps.Value.SI<Ampere>();
			}
			container[ModalResultField.AA_SmartOverrunAlternatorEfficiency] = Auxiliaries.AA_SmartOverrunAlternatorEfficiency;
			if (Auxiliaries.AA_CompressorFlowRate_LitrePerSec != null) {
				container[ModalResultField.AA_CompressorFlowRate_LitrePerSec] =
					new SI(Auxiliaries.AA_CompressorFlowRate_LitrePerSec.Value);
			}
			container[ModalResultField.AA_OverrunFlag] = Auxiliaries.AA_OverrunFlag;
			container[ModalResultField.AA_EngineIdleFlag] = Auxiliaries.AA_EngineIdleFlag;
			container[ModalResultField.AA_CompressorFlag] = Auxiliaries.AA_CompressorFlag;
			if (Auxiliaries.AA_TotalCycleFC_Grams != null) {
				container[ModalResultField.AA_TotalCycleFC_Grams] = new SI(Auxiliaries.AA_TotalCycleFC_Grams.Value);
			}
			if (Auxiliaries.AA_TotalCycleFC_Litres != null) {
				container[ModalResultField.AA_TotalCycleFC_Litres] = new SI(Auxiliaries.AA_TotalCycleFC_Litres.Value);
			}
			if (Auxiliaries.AA_AveragePowerDemandCrankHVACMechanicals != null) {
				container[ModalResultField.AA_AveragePowerDemandCrankHVACMechanicals] =
					new SI(Auxiliaries.AA_AveragePowerDemandCrankHVACMechanicals.Value);
			}
			if (Auxiliaries.AA_AveragePowerDemandCrankHVACElectricals != null) {
				container[ModalResultField.AA_AveragePowerDemandCrankHVACElectricals] =
					new SI(Auxiliaries.AA_AveragePowerDemandCrankHVACElectricals.Value);
			}
			if (Auxiliaries.AA_AveragePowerDemandCrankElectrics != null) {
				container[ModalResultField.AA_AveragePowerDemandCrankElectrics] =
					new SI(Auxiliaries.AA_AveragePowerDemandCrankElectrics.Value);
			}
			if (Auxiliaries.AA_AveragePowerDemandCrankPneumatics != null) {
				container[ModalResultField.AA_AveragePowerDemandCrankPneumatics] =
					new SI(Auxiliaries.AA_AveragePowerDemandCrankPneumatics.Value);
			}
			if (Auxiliaries.AA_TotalCycleFuelConsumptionCompressorOff != null) {
				container[ModalResultField.AA_TotalCycleFuelConsumptionCompressorOff] =
					new SI(Auxiliaries.AA_TotalCycleFuelConsumptionCompressorOff.Value);
			}
			container[ModalResultField.AA_TotalCycleFuelConsumptionCompressorOn] =
				new SI(Auxiliaries.AA_TotalCycleFuelConsumptionCompressorOn.Value);
		}

		protected override void DoCommitSimulationStep()
		{
			AdvanceState();
		}


		private Watt GetBusAuxPowerDemand(Second absTime, Second dt, NewtonMeter torquePowerTrain, NewtonMeter torqueEngine,
			PerSecond angularSpeed, bool dryRun = false)
		{
			_fcMapAdapter.AllowExtrapolation = true;

			Auxiliaries.Signals.ClutchEngaged = DataBus.ClutchClosed(absTime);
			Auxiliaries.Signals.EngineDrivelinePower = (float)(torquePowerTrain * angularSpeed / 1000).Value();
			Auxiliaries.Signals.EngineDrivelineTorque = (float)torquePowerTrain.Value();
			Auxiliaries.Signals.Internal_Engine_Power =
				(float)((torqueEngine * angularSpeed - DataBus.BrakePower) / 1000).Value();
			if (DataBus.DriverBehavior == DrivingBehavior.Coasting) {
				// make sure smart aux are _not_ enabled for now
				// set internal_engine_power a little bit lower so there is no excessive power for smart aux
				Auxiliaries.Signals.Internal_Engine_Power =
					(float)((0.9 * torqueEngine * angularSpeed /*- DataBus.BrakePower*/) / 1000).Value();
				// if smart aux should be on during coasting use the following line
				// set internal_engine_power to a large value (*10) so that there's excessive power for smart aux (alreadin during search operating point)
				//(float)DataBus.EngineDragPower(angularSpeed).Value() / 100;
			} else {
				if (DataBus.DriverBehavior != DrivingBehavior.Braking) {
					Auxiliaries.Signals.Internal_Engine_Power = 0;
					//(float)((0.9 * torqueEngine * angularSpeed - DataBus.BrakePower) / 1000).Value();
				} else {
					// smart aux should be on during braking
				}
			}
			Auxiliaries.Signals.EngineMotoringPower = (float)(-DataBus.EngineDragPower(angularSpeed).Value() / 1000);
			Auxiliaries.Signals.EngineSpeed = angularSpeed.Value() / Constants.RPMToRad;
			Auxiliaries.Signals.PreExistingAuxPower = 0; //mAAUX_Global.PreExistingAuxPower;
			Auxiliaries.Signals.Idle = DataBus.VehicleStopped;
			Auxiliaries.Signals.InNeutral = DataBus.Gear == 0;
			Auxiliaries.Signals.RunningCalc = true;

			//mAAUX_Global.Internal_Engine_Power;
			//'Power coming out of Advanced Model is in Watts.

			return ((double)Auxiliaries.AuxiliaryPowerAtCrankWatts).SI<Watt>();
		}

		protected class FuelConsumptionAdapter : IFuelConsumptionMap
		{
			protected internal FuelConsumptionMap FcMap;

			public bool AllowExtrapolation { get; set; }

			public double GetFuelConsumption(double torque, double angularVelocity)
			{
				return FcMap.GetFuelConsumption(torque.SI<NewtonMeter>(), angularVelocity.RPMtoRad(), AllowExtrapolation).Value() *
						1000 * 3600;
			}
		}

		public class BusAuxState
		{
			public Second dt;
			public PerSecond AngularSpeed;
			public Watt PowerDemand;
		}
	}
}