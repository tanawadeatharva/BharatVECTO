using System;
using System.IO;
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
		private readonly FuelConsumptionAdapter _fcMapAdapter;


		public BusAuxiliariesAdapter(IVehicleContainer container, string aauxFile, string cycleName, Kilogram vehicleWeight,
			FuelConsumptionMap fcMap,
			PerSecond engineIdleSpeed) : base(container)
		{
			//	mAAUX_Global.advancedAuxModel.Signals.DeclarationMode = Cfg.DeclMode
			//	mAAUX_Global.advancedAuxModel.Signals.WHTC = Declaration.WHTCcorrFactor

			var tmpAux = new AdvancedAuxiliaries();

			// 'Set Statics
			tmpAux.VectoInputs.Cycle = DetermineCycle(cycleName);
			tmpAux.VectoInputs.VehicleWeightKG = (float)vehicleWeight.Value();
			_fcMapAdapter = new FuelConsumptionAdapter() { FcMap = fcMap };
			tmpAux.VectoInputs.FuelMap = _fcMapAdapter;
			tmpAux.VectoInputs.FuelDensity = Physics.FuelDensity.Value();

			//'Set Signals
			tmpAux.Signals.TotalCycleTimeSeconds = 15000; // TODO MQ: get cycle time somehow!
			tmpAux.Signals.EngineIdleSpeed = (float)engineIdleSpeed.Value();
			tmpAux.Initialise(Path.GetFileName(aauxFile), Path.GetDirectoryName(Path.GetFullPath(aauxFile)) + @"\");

			Auxiliaries = tmpAux;
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
			return PreviousState.PowerDemand / angularSpeed;
		}


		public NewtonMeter PowerDemand(Second absTime, Second dt, NewtonMeter torquePowerTrain, NewtonMeter torqueEngine,
			PerSecond angularSpeed, bool dryRun = false)
		{
			CurrentState.AngularSpeed = angularSpeed;
			CurrentState.dt = dt;
			CurrentState.PowerDemand = GetBusAuxPowerDemand(absTime, dt, torquePowerTrain, torqueEngine, angularSpeed);

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
			PerSecond angularSpeed)
		{
			_fcMapAdapter.AllowExtrapolation = true;

			Auxiliaries.Signals.ClutchEngaged = DataBus.ClutchClosed(absTime);
			Auxiliaries.Signals.EngineDrivelinePower = (float)(torquePowerTrain * angularSpeed / 1000).Value();
			Auxiliaries.Signals.EngineDrivelineTorque = (float)torquePowerTrain.Value();
			Auxiliaries.Signals.EngineMotoringPower = -(float)DataBus.EngineDragPower(angularSpeed).Value() / 1000;
			Auxiliaries.Signals.EngineSpeed = (int)(angularSpeed.Value() / Constants.RPMToRad);
			Auxiliaries.Signals.PreExistingAuxPower = 0; //mAAUX_Global.PreExistingAuxPower;
			Auxiliaries.Signals.Idle = DataBus.VehicleStopped;
			Auxiliaries.Signals.InNeutral = DataBus.Gear == 0;
			Auxiliaries.Signals.RunningCalc = true;
			Auxiliaries.Signals.Internal_Engine_Power = (float)(torqueEngine * angularSpeed / 1000).Value();
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