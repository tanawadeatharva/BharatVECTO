using System;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Engine;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.Utils;
using VectoAuxiliaries;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
	public class BusAuxiliariesAdapter : StatefulVectoSimulationComponent<BusAuxiliariesAdapter.BusAuxState>,
		IEngineAuxPort
	{
		protected IAdvancedAuxiliaries Auxiliaries;


		public BusAuxiliariesAdapter(IVehicleContainer container, string aauxFile, string cycleName, Kilogram vehicleWeight,
			FuelConsumptionMap fcMap,
			PerSecond engineIdleSpeed) : base(container)
		{
			//	mAAUX_Global.advancedAuxModel.Signals.DeclarationMode = Cfg.DeclMode
			//	mAAUX_Global.advancedAuxModel.Signals.WHTC = Declaration.WHTCcorrFactor

			// 'Set Statics
			Auxiliaries.VectoInputs.Cycle = DetermineCycle(cycleName);
			Auxiliaries.VectoInputs.VehicleWeightKG = (float)vehicleWeight.Value();
			Auxiliaries.VectoInputs.FuelMap = new FuelConsumptionAdapter() { FcMap = fcMap };
			Auxiliaries.VectoInputs.FuelDensity = Physics.FuelDensity.Value();

			//'Set Signals
			Auxiliaries.Signals.TotalCycleTimeSeconds = 3600; // TODO MQ: get cycle time somehow!
			Auxiliaries.Signals.EngineIdleSpeed = (float)engineIdleSpeed.Value();
			Auxiliaries.RunStart(aauxFile, "");
		}

		private static string DetermineCycle(string cycleName)
		{
			return "Coach";
			//			Public Function DetermineCycleNameFromCurrentFile() As String

			//	'Get DriveFile without path and without extension
			//	Dim driveFile As String = fFILE(CurrentCycleFile, False)

			//	Select Case (True)

			//		'DJN - update to make contains test case insensitive
			//		Case driveFile.ToLower().Contains("heavy_urban") AndAlso driveFile.ToLower().Contains("bus")
			//			Return "Heavy urban"

			//		Case driveFile.ToLower().Contains("suburban") AndAlso driveFile.ToLower().Contains("bus")
			//			Return "Suburban"

			//		Case driveFile.ToLower().Contains("urban") AndAlso driveFile.ToLower().Contains("bus")
			//			Return "Urban"

			//		Case driveFile.ToLower().Contains("interurban") AndAlso driveFile.ToLower().Contains("bus")
			//			Return "Interurban"

			//		Case driveFile.ToLower().Contains("coach")
			//			Return "Coach"

			//		Case Else
			//			WorkerMsg(tMsgID.Warn,
			//					String.Format("UnServiced Cycle Name '{0}' in Pneumatics Actuations Map 0 Actuations returned", driveFile),
			//					"Advanced Auxiliaries")
			//			Return "UnknownCycleName"

			//	End Select


			//	Return "Urban"
			//End Function
		}

		public NewtonMeter Initialize(NewtonMeter torque, PerSecond angularSpeed)
		{
			PreviousState.AngularSpeed = angularSpeed;
			PreviousState.PowerDemand = GetBusAuxPowerDemand(0.SI<Second>(), 1.SI<Second>(), torque, torque, angularSpeed);
			return CurrentState.PowerDemand / angularSpeed;
		}


		public NewtonMeter PowerDemand(Second absTime, Second dt, NewtonMeter torquePowerTrain, NewtonMeter torqueEngine,
			PerSecond angularSpeed,
			bool dryRun = false)
		{
			CurrentState.AngularSpeed = angularSpeed;

			CurrentState.PowerDemand = GetBusAuxPowerDemand(absTime, dt, torquePowerTrain, torqueEngine, angularSpeed);

			var avgAngularSpeed = (CurrentState.AngularSpeed + PreviousState.AngularSpeed) / 2.0;
			return CurrentState.PowerDemand / avgAngularSpeed;
		}


		protected override void DoWriteModalResults(IModalDataContainer container)
		{
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
			container[ModalResultField.AA_CompressorFlowRate_LitrePerSec] = Auxiliaries.AA_CompressorFlowRate_LitrePerSec;
			container[ModalResultField.AA_OverrunFlag] = Auxiliaries.AA_OverrunFlag;
			container[ModalResultField.AA_EngineIdleFlag] = Auxiliaries.AA_EngineIdleFlag;
			container[ModalResultField.AA_CompressorFlag] = Auxiliaries.AA_CompressorFlag;
			container[ModalResultField.AA_TotalCycleFC_Grams] = Auxiliaries.AA_TotalCycleFC_Grams;
			container[ModalResultField.AA_TotalCycleFC_Litres] = Auxiliaries.AA_TotalCycleFC_Litres;
			container[ModalResultField.AA_AveragePowerDemandCrankHVACMechanicals] =
				Auxiliaries.AA_AveragePowerDemandCrankHVACMechanicals;
			container[ModalResultField.AA_AveragePowerDemandCrankHVACElectricals] =
				Auxiliaries.AA_AveragePowerDemandCrankHVACElectricals;
			container[ModalResultField.AA_AveragePowerDemandCrankElectrics] = Auxiliaries.AA_AveragePowerDemandCrankElectrics;
			container[ModalResultField.AA_AveragePowerDemandCrankPneumatics] = Auxiliaries.AA_AveragePowerDemandCrankPneumatics;
			container[ModalResultField.AA_TotalCycleFuelConsumptionCompressorOff] =
				Auxiliaries.AA_TotalCycleFuelConsumptionCompressorOff;
			container[ModalResultField.AA_TotalCycleFuelConsumptionCompressorOn] =
				Auxiliaries.AA_TotalCycleFuelConsumptionCompressorOn;
		}

		protected override void DoCommitSimulationStep()
		{
			var message = String.Empty;
			Auxiliaries.CycleStep(CurrentState.dt.Value(), ref message);
			Log.Warn(message);
			AdvanceState();
		}


		private Watt GetBusAuxPowerDemand(Second absTime, Second dt, NewtonMeter torquePowerTrain, NewtonMeter torqueEngine,
			PerSecond angularSpeed)
		{
			Auxiliaries.Signals.ClutchEngaged = DataBus.ClutchClosed(absTime);
			Auxiliaries.Signals.EngineDrivelinePower = (float)(torquePowerTrain * angularSpeed).Value();
			Auxiliaries.Signals.EngineDrivelineTorque = (float)torquePowerTrain.Value();
			Auxiliaries.Signals.EngineMotoringPower = (float)DataBus.EngineDragPower(angularSpeed).Value();
			Auxiliaries.Signals.EngineSpeed = (int)(angularSpeed.Value() / Constants.RPMToRad);
			Auxiliaries.Signals.PreExistingAuxPower = 0; //mAAUX_Global.PreExistingAuxPower;
			Auxiliaries.Signals.Idle = DataBus.VehicleStopped;
			Auxiliaries.Signals.InNeutral = DataBus.Gear == 0;
			Auxiliaries.Signals.RunningCalc = true;
			Auxiliaries.Signals.Internal_Engine_Power = (float)(torqueEngine * angularSpeed).Value();
			//mAAUX_Global.Internal_Engine_Power;
			//'Power coming out of Advanced Model is in Watts.

			return ((double)Auxiliaries.AuxiliaryPowerAtCrankWatts).SI<Watt>();
		}

		protected class FuelConsumptionAdapter : IFuelConsumptionMap
		{
			protected internal FuelConsumptionMap FcMap;

			public double GetFuelConsumption(double torque, double angularVelocity)
			{
				return FcMap.GetFuelConsumption(torque.SI<NewtonMeter>(), angularVelocity.SI<PerSecond>()).Value();
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