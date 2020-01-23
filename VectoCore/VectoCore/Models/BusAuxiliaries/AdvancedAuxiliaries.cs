// Copyright 2017 European Union.
// Licensed under the EUPL (the 'Licence');
// 
// * You may not use this work except in compliance with the Licence.
// * You may obtain a copy of the Licence at: http://ec.europa.eu/idabc/eupl
// * Unless required by applicable law or agreed to in writing,
// software distributed under the Licence is distributed on an "AS IS" basis,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// 
// See the LICENSE.txt for the specific language governing permissions and limitations.

using System;
using System.Collections.Generic;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl;
using TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.Electrics;
using TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.HVAC;
using TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.Pneumatics;
using TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces;
using TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules;
using TUGraz.VectoCore.Models.BusAuxiliaries.Util;

namespace TUGraz.VectoCore.Models.BusAuxiliaries {
	/// <summary>

	/// ''' Main entry point for the advanced auxiliary module. 

	/// ''' This class represents slide number 17 titled Calculations of Cycle FC accounting for Smart Auxiliaries.

	/// ''' </summary>

	/// ''' <remarks></remarks>
	public class AdvancedAuxiliaries : IAdvancedAuxiliaries
	{
		protected internal IAuxiliaryConfig auxConfig;

		// Supporting classes which may generate event messages
		//private ICompressorMap compressorMap;

		private SSMTOOL ssmTool;
		private SSMTOOL ssmToolModule14;

		//private IAlternatorMap alternatorMap;

		private IFuelConsumptionMap fuelMap;

		// Classes which compose the model.
		private IM0_NonSmart_AlternatorsSetEfficiency M0;
		private IM0_5_SmartAlternatorSetEfficiency M0_5;
		private IM1_AverageHVACLoadDemand M1;
		private IM2_AverageElectricalLoadDemand M2;
		private IM3_AveragePneumaticLoadDemand M3;
		private IM4_AirCompressor M4;
		private IM5_SmartAlternatorSetGeneration M5;
		private IM6 M6;
		private IM7 M7;
		private IM8 M8;
		private IM9 M9;
		private IM10 M10;
		private IM11 M11;
		private IM12 M12;
		private IM13 M13;
		private IM14 M14;

		
		public void VectoEventHandler(ref object sender, string message, AdvancedAuxiliaryMessageType messageType)
		{
			//if (Signals.AuxiliaryEventReportingLevel <= messageType) {
				AuxiliaryEvent?.Invoke(ref sender, message, messageType);
			//}
		}

		// Constructor
		public AdvancedAuxiliaries()
		{
			//VectoInputs = new VectoInputs();
			Signals = new Signals();
		}

	

		// Initialise Model
		public void Initialise(IAuxiliaryConfig auxCfg, IFuelProperties fuelProperties /*string IAuxPath, string vectoFilePath*/)
		{
			Signals.CurrentCycleTimeInSeconds = 0;
			auxConfig = auxCfg; //new AuxiliaryConfig(auxPath);

			// Pass some signals from config to Signals. ( These are stored in the configuration but shared in the signal distribution around modules )
			Signals.SmartElectrics = auxConfig.ElectricalUserInputsConfig.SmartElectrical;
			//Signals.StoredEnergyEfficiency = auxConfig.ElectricalUserInputsConfig.StoredEnergyEfficiency;
			Signals.SmartPneumatics = auxConfig.PneumaticUserInputsConfig.SmartAirCompression;
			//Signals.PneumaticOverrunUtilisation = auxConfig.PneumaticAuxillariesConfig.OverrunUtilisationForCompressionFraction;

			var alternatorMap = auxConfig.ElectricalUserInputsConfig.AlternatorMap;
			var compressorMap = auxConfig.PneumaticUserInputsConfig.CompressorMap;
			
			// fuelMap = New cMAP()
			// fuelMap.FilePath = FilePathUtils.ResolveFilePath(vectoDirectory, VectoInputs.FuelMap)
			// If Not fuelMap.ReadFile() Then
			// MessageBox.Show("Unable to read fuel map, aborting.")
			// Return
			// End If
			// fuelMap.Triangulate()
			fuelMap = auxCfg.FuelMap;

			
			// SSM HVAC
			//var ssmPath = FilePathUtils.ResolveFilePath(vectoDirectory, auxConfig.HvacUserInputsConfig.SSMFilePath);
			//var BusDatabase = FilePathUtils.ResolveFilePath(vectoDirectory, auxConfig.HvacUserInputsConfig.BusDatabasePath);
			ssmTool = new SSMTOOL(auxConfig.SSMInputs);

			// This duplicate SSM is being created for use in M14 as its properties will be dynamically changed at that point
			// to honour EngineWaste Heat Usage in Fueling calculations.
			ssmToolModule14 = new SSMTOOL(auxCfg.SSMInputs);
			
			//if ((ssmTool.Load(ssmPath) == false || ssmToolModule14.Load(ssmPath) == false))
				//throw new Exception(string.Format("Unable to load the ssmTOOL with file {0}", ssmPath));
			
			
			M0 = new M00Impl(auxConfig.ElectricalUserInputsConfig, Signals, ssmTool);


			var M0_5tmp = new M0_5Impl(
				M0, auxConfig.ElectricalUserInputsConfig, Signals);
			M0_5 = M0_5tmp;

			M1 = new M01Impl(
				M0, auxConfig.ElectricalUserInputsConfig.AlternatorGearEfficiency,
				auxConfig.PneumaticUserInputsConfig.CompressorGearEfficiency, ssmTool);


			M2 = new M02Impl(M0, auxConfig.ElectricalUserInputsConfig);

			
			M3 = new M03Impl(auxConfig, compressorMap, auxCfg.Actuations, Signals);

			M4 = new M04Impl(compressorMap, auxConfig.PneumaticUserInputsConfig.CompressorGearRatio, auxConfig.PneumaticUserInputsConfig.CompressorGearEfficiency, Signals);
			M5 = new M05Impl(M0_5tmp, auxConfig.ElectricalUserInputsConfig.PowerNetVoltage, auxConfig.ElectricalUserInputsConfig.AlternatorGearEfficiency);
			M6 = new M06Impl(M1, M2, M3, M4, M5, Signals);
			M7 = new M07Impl(M5, M6, Signals);
			M8 = new M08Impl(M1, M6, M7, Signals);
			M9 = new M09Impl(M1, M4, M6, M8, fuelMap, auxConfig.PneumaticAuxillariesConfig, Signals);
			M10 = new M10Impl(M3, M9);
			M11 = new M11Impl(M1, M3, M6, M8, fuelMap, Signals);
			M12 = new M12Impl(M10, M11);
			M13 = new M13Impl(M10, M11, M12, Signals);
			M14 = new M14Impl(M13, ssmToolModule14, fuelProperties, Signals);

			//compressorMap.AuxiliaryEvent += VectoEventHandler;
			//alternatorMap.AuxiliaryEvent += VectoEventHandler;
			//ssmTool.Message += VectoEventHandler;
			//ssmToolModule14.Message += VectoEventHandler;
		}

		
		public ISignals Signals { get; set; }
		//public IVectoInputs VectoInputs { get; set; }

		public event AuxiliaryEventEventHandler AuxiliaryEvent;

		//public delegate void AuxiliaryEventEventHandler(ref object sender, string message, AdvancedAuxiliaryMessageType messageType);

		//public bool Configure(string filePath, string vectoFilePath)
		//{
		//	try {
		//		frmAuxiliaryConfig frmAuxiliaryConfig = new frmAuxiliaryConfig(filePath, vectoFilePath);

		//		frmAuxiliaryConfig.Show();

		//		if (frmAuxiliaryConfig.DialogResult != DialogResult.OK) {
		//			return true;
		//		}

		//		return false;
		//	} catch (Exception ex) {
		//		return false;
		//	}
		//}

		public bool CycleStep(Second seconds, ref string message)
		{
			try {
				M9.CycleStep(seconds);
				M10.CycleStep(seconds);
				M11.CycleStep(seconds);

				Signals.CurrentCycleTimeInSeconds += seconds.Value();
			} catch (Exception ex) {
				//MessageBox.Show("Exception: " + ex.Message + " Stack Trace: " + ex.StackTrace);
				throw ex;
			}


			return true;
		}

		public bool Running
		{
			get {
				throw new NotImplementedException();
			}
		}

		public bool RunStart(IAuxiliaryConfig auxCfg, IFuelProperties fuelProperties)
		{
			try {
				Initialise(auxCfg, fuelProperties);
			} catch (Exception ) {
				return false;
			}

			return true;
		}

		public bool RunStop(ref string message)
		{
			throw new NotImplementedException();
		}

		public void ResetCalculations()
		{
			var modules = new List<IAbstractModule>() { M0, M0_5, M1, M2, M3, M4, M5, M6, M7, M8, M9, M10, M11, M12, M13, M14 };
			foreach (var moduel in modules)
				moduel.ResetCalculations();
		}

		public Kilogram TotalFuel
		{
			get {
				if (M13 != null)
					return M14.TotalCycleFC;
				else
					return 0.SI<Kilogram>();
			}
		}

		//public Liter TotalFuelLITRES
		//{
		//	get {
		//		if (M14 != null)
		//			return M14.TotalCycleFCLitres;
		//		else
		//			return 0.SI<Liter>();
		//	}
		//}

		public string AuxiliaryName
		{
			get {
				return "BusAuxiliaries";
			}
		}

		public string AuxiliaryVersion
		{
			get {
				return "Version 1.0 Beta";
			}
		}



		// Helpers
		

		public bool ValidateAAUXFile(string filePath, ref string message)
		{
			var validResult = FilePathUtils.ValidateFilePath(filePath, ".aaux", ref message);

			return validResult;
		}

		// Diagnostics outputs for testing purposes in Vecto.
		// Eventually this can be removed or rendered non effective to reduce calculation load on the model.
		public double AA_NonSmartAlternatorsEfficiency
		{
			get {
				return M0.AlternatorsEfficiency;
			}
		}

		public Ampere AA_SmartIdleCurrent_Amps
		{
			get {
				return M0_5.SmartIdleCurrent;
			}
		}

		public double AA_SmartIdleAlternatorsEfficiency
		{
			get {
				return M0_5.AlternatorsEfficiencyIdleResultCard;
			}
		}

		public Ampere AA_SmartTractionCurrent_Amps
		{
			get {
				return M0_5.SmartTractionCurrent;
			}
		}

		public double AA_SmartTractionAlternatorEfficiency
		{
			get {
				return M0_5.AlternatorsEfficiencyTractionOnResultCard;
			}
		}

		public Ampere AA_SmartOverrunCurrent_Amps
		{
			get {
				return M0_5.SmartOverrunCurrent;
			}
		}

		public double AA_SmartOverrunAlternatorEfficiency
		{
			get {
				return M0_5.AlternatorsEfficiencyOverrunResultCard;
			}
		}

		public NormLiterPerSecond AA_CompressorFlowRate_LitrePerSec
		{
			get {
				return M4.GetFlowRate();
			}
		}

		public bool AA_OverrunFlag
		{
			get {
				return M6.OverrunFlag;
			}
		}

		public int? AA_EngineIdleFlag
		{
			get {
				return Signals.EngineSpeed <= Signals.EngineIdleSpeed && (!Signals.ClutchEngaged || Signals.InNeutral) ? 1 : 0;
			}
		}

		public bool AA_CompressorFlag
		{
			get {
				return M8.CompressorFlag;
			}
		}

		public Kilogram AA_TotalCycleFC_Grams
		{
			get {
				return M14.TotalCycleFC;
			}
		}

		//public Liter AA_TotalCycleFC_Litres
		//{
		//	get {
		//		return M14.TotalCycleFCLitres;
		//	}
		//}

		public Watt AuxiliaryPowerAtCrankWatts
		{
			get {
				return M8.AuxPowerAtCrankFromElectricalHVACAndPneumaticsAncillaries;
			}
		}

		public Watt AA_AveragePowerDemandCrankHVACMechanicals
		{
			get {
				return M1.AveragePowerDemandAtCrankFromHVACMechanicals();
			}
		}

		public Watt AA_AveragePowerDemandCrankHVACElectricals
		{
			get {
				return M1.AveragePowerDemandAtCrankFromHVACElectrics();
			}
		}

		public Watt AA_AveragePowerDemandCrankElectrics
		{
			get {
				return M2.GetAveragePowerAtCrankFromElectrics();
			}
		}

		public Watt AA_AveragePowerDemandCrankPneumatics
		{
			get {
				return M3.GetAveragePowerDemandAtCrankFromPneumatics();
			}
		}

		public Kilogram AA_TotalCycleFuelConsumptionCompressorOff
		{
			get {
				return M9.TotalCycleFuelConsumptionCompressorOffContinuously;
			}
		}

		public Kilogram AA_TotalCycleFuelConsumptionCompressorOn
		{
			get {
				return M9.TotalCycleFuelConsumptionCompressorOnContinuously;
			}
		}

		}
}
