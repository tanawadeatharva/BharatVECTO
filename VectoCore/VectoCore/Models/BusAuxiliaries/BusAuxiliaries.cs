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
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl;
using TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.Electrics;
using TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.HVAC;
using TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.Pneumatics;
using TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces;
using TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules;
using TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules.Electrics;
using TUGraz.VectoCore.Models.BusAuxiliaries.Util;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.VectoCore.Models.BusAuxiliaries
{
	/// <summary>
	/// ''' Main entry point for the advanced auxiliary module. 
	/// ''' This class represents slide number 17 titled Calculations of Cycle FC accounting for Smart Auxiliaries.
	/// ''' </summary>
	/// ''' <remarks></remarks>
	public class BusAuxiliaries : IBusAuxiliaries
	{
		protected internal IAuxiliaryConfig auxConfig;

		// Supporting classes which may generate event messages
		//private ICompressorMap compressorMap;

		//private SSMTOOL ssmTool;
		//private SSMTOOL ssmToolModule14;

		//private IAlternatorMap alternatorMap;

		//private IFuelConsumptionMap fuelMap;

		// Classes which compose the model.
		private IM0_NonSmart_AlternatorsSetEfficiency M0;

		//private IM0_5_SmartAlternatorSetEfficiency M0_5;
		private IM1_AverageHVACLoadDemand M1;
		private IM2_AverageElectricalLoadDemand M2;
		private IM3_AveragePneumaticLoadDemand M3;
		private IM4_AirCompressor M4;
		private IM5_SmartAlternatorSetGeneration M5;
		private IM6 M6;
		private IM7 M7;
		private IM8 M8;

		private ISimpleBattery ElectricStorage;

		//private IM9 M9;
		//private IM10 M10;
		//private IM11 M11;
		//private IM12 M12;
		//private IM13 M13;
		//private IM14 M14;


		public BusAuxiliaries(IModalDataContainer modDataContainer)
		{
			Signals = new Signals();
			if (modDataContainer != null) {
				modDataContainer.AuxHeaterDemandCalc = AuxHeaterDemandCalculation;
			}
		}

		protected virtual Joule AuxHeaterDemandCalculation(Second cycleTime, Joule engineWasteHeatTotal)
		{
			if (auxConfig == null) {
				throw new VectoException("Auxiliary configuration missing!");
			}

			var ssmTool = new SSMTOOL(auxConfig.SSMInputs);
			var M14 = new M14aImpl(ssmTool);
			return M14.AuxHeaterDemand(cycleTime, engineWasteHeatTotal);
		}

		public void Initialise(IAuxiliaryConfig auxCfg)
		{
			Signals.CurrentCycleTimeInSeconds = 0;
			auxConfig = auxCfg; //new AuxiliaryConfig(auxPath);

			var compressorMap = auxConfig.PneumaticUserInputsConfig.CompressorMap;

			// SSM HVAC
			//var ssmPath = FilePathUtils.ResolveFilePath(vectoDirectory, auxConfig.HvacUserInputsConfig.SSMFilePath);
			//var BusDatabase = FilePathUtils.ResolveFilePath(vectoDirectory, auxConfig.HvacUserInputsConfig.BusDatabasePath);
			var ssmTool = new SSMTOOL(auxConfig.SSMInputs);

			ElectricStorage = new SimpleBattery(
				auxCfg.ElectricalUserInputsConfig.SmartElectrical
					? auxCfg.ElectricalUserInputsConfig.ElectricStorageCapacity
					: 0.SI<WattSecond>());

			M0 = new M00Impl(auxConfig.ElectricalUserInputsConfig, Signals, ssmTool.ElectricalWAdjusted);

			//M0_5 = new M0_5Impl(
			//	M0, auxConfig.ElectricalUserInputsConfig, Signals);

			M1 = new M01Impl(
				M0, auxConfig.ElectricalUserInputsConfig.AlternatorGearEfficiency,
				auxConfig.PneumaticUserInputsConfig.CompressorGearEfficiency, ssmTool.ElectricalWAdjusted,
				ssmTool.MechanicalWBaseAdjusted);

			M2 = new M02Impl(M0, auxConfig.ElectricalUserInputsConfig, Signals);

			M3 = new M03Impl(auxConfig, compressorMap, auxCfg.Actuations, Signals);

			M4 = new M04Impl(
				compressorMap, auxConfig.PneumaticUserInputsConfig.CompressorGearRatio,
				auxConfig.PneumaticUserInputsConfig.CompressorGearEfficiency, Signals);

			//M5 = new M05Impl(
			//	M0_5, auxConfig.ElectricalUserInputsConfig.PowerNetVoltage,
			//	auxConfig.ElectricalUserInputsConfig.AlternatorGearEfficiency);
			M5 = new M05Impl_P0(M0, M1, M2, ElectricStorage, auxCfg.ElectricalUserInputsConfig, Signals);
			M6 = new M06Impl(auxCfg.ElectricalUserInputsConfig, M1, M2, M3, M4, M5, Signals);
			M7 = new M07Impl(
				M0, M1, M2, M5, M6, ElectricStorage, auxCfg.ElectricalUserInputsConfig.AlternatorGearEfficiency, Signals);
			M8 = new M08Impl(auxConfig, M1, M6, M7, Signals);

			//M9 = new M09Impl(M1, M4, M6, M8, fuelMap, auxConfig.PneumaticAuxillariesConfig, Signals);
			//M10 = new M10Impl(M3, M9);
			//M11 = new M11Impl(M1, M3, M6, M8, fuelMap, Signals);
			//M12 = new M12Impl(M10, M11);
			//M13 = new M13Impl(auxCfg, M10, M11, M12, Signals);

			// This duplicate SSM is being created for use in M14 as its properties will be dynamically changed at that point
			// to honour EngineWaste Heat Usage in Fueling calculations.
			//var ssmToolModule14 = new SSMTOOL(auxCfg.SSMInputs);

			//M14 = new M14Impl(M13, ssmToolModule14, fuelProperties, Signals);
		}


		public ISignals Signals { get; set; }

		public Watt ElectricPowerConsumer
		{
			get { return M2.AveragePowerDemandAtAlternatorFromElectrics; }
		}

		public Watt HVACElectricalPowerConsumer
		{
			get { return M1.AveragePowerDemandAtAlternatorFromHVACElectrics; }
		}


		public Watt ElectricPowerConsumerSum
		{
			get { return M1.AveragePowerDemandAtAlternatorFromHVACElectrics + M2.AveragePowerDemandAtAlternatorFromElectrics; }
		}

		public Watt ElectricPowerDemandMech
		{
			get {
				if (auxConfig.ElectricalUserInputsConfig.SmartElectrical) {
					return auxConfig.PneumaticUserInputsConfig.SmartAirCompression
						? M7.SmartElectricalAndPneumaticAuxAltPowerGenAtCrank
						: M7.SmartElectricalOnlyAuxAltPowerGenAtCrank;
				}

				return M2.GetAveragePowerAtCrankFromElectrics() + M1.AveragePowerDemandAtCrankFromHVACElectrics;
			}
		}

		public Watt ElectricPowerGenerated
		{
			get {
				if (auxConfig.ElectricalUserInputsConfig.SmartElectrical) {
					var retVal = auxConfig.PneumaticUserInputsConfig.SmartAirCompression
						? M7.SmartElectricalAndPneumaticAuxAltPowerGenAtCrank
						: M7.SmartElectricalOnlyAuxAltPowerGenAtCrank;
					return retVal * M0.AlternatorsEfficiency * auxConfig.ElectricalUserInputsConfig.AlternatorGearEfficiency;
				}

				return M1.AveragePowerDemandAtAlternatorFromHVACElectrics + M2.AveragePowerDemandAtAlternatorFromElectrics;
			}
		}

		public NormLiter PSDemandConsumer
		{
			get { return M3.AverageAirConsumed * Signals.SimulationInterval; }
		}

		public NormLiter PSAirGenerated
		{
			get {
				if (auxConfig.PneumaticUserInputsConfig.SmartAirCompression && M6.OverrunFlag && Signals.ClutchEngaged &&
					!Signals.InNeutral) {
					if (M8.CompressorFlag) {
						return M4.GetFlowRate() *
								auxConfig.PneumaticAuxillariesConfig.OverrunUtilisationForCompressionFraction
								* Signals.SimulationInterval;
					}

					return 0.SI<NormLiter>();
				}

				return PSDemandConsumer;
			}
		}

		public NormLiter PSAirGeneratedAlwaysOn
		{
			get { return M4.GetFlowRate() * Signals.SimulationInterval; }
		}


		public Watt PSPowerDemandAirGenerated
		{
			get {
				if (auxConfig.PneumaticUserInputsConfig.SmartAirCompression) {
					return auxConfig.ElectricalUserInputsConfig.SmartElectrical
						? M7.SmartElectricalAndPneumaticAuxAirCompPowerGenAtCrank
						: M7.SmartPneumaticOnlyAuxAirCompPowerGenAtCrank;
				}

				return M3.GetAveragePowerDemandAtCrankFromPneumatics();
			}
		}

		public Watt PSPowerCompressorAlwaysOn
		{
			get { return M4.GetPowerCompressorOn(); }
		}

		public Watt PSPowerCompressorDragOnly
		{
			get { return M4.GetPowerCompressorOff(); }
		}

		public Watt HVACMechanicalPowerConsumer
		{
			get { return M1.AveragePowerDemandAtCrankFromHVACMechanicals; }
		}

		public Watt HVACMechanicalPowerGenerated
		{
			get { return M1.AveragePowerDemandAtCrankFromHVACMechanicals; }
		}

		public double BatterySOC
		{
			get { return ElectricStorage.SOC; }
		}


		public string AuxiliaryName
		{
			get { return "BusAuxiliaries"; }
		}

		public string AuxiliaryVersion
		{
			get { return "Version 2.0 DEV"; }
		}

		public Watt AuxiliaryPowerAtCrankWatts
		{
			get { return M8.AuxPowerAtCrankFromElectricalHVACAndPneumaticsAncillaries; }
		}


		public void CycleStep(Second seconds, double essFactor)
		{
			try {
				//M9.CycleStep(seconds);
				//M10.CycleStep(seconds);
				//M11.CycleStep(seconds);
				if (auxConfig.ElectricalUserInputsConfig.SmartElectrical) {
					var generatedElPower =
						(auxConfig.PneumaticUserInputsConfig.SmartAirCompression
							? M7.SmartElectricalAndPneumaticAuxAltPowerGenAtCrank
							: M7.SmartElectricalOnlyAuxAltPowerGenAtCrank) * M0.AlternatorsEfficiency *
						auxConfig.ElectricalUserInputsConfig.AlternatorGearEfficiency;
					var energy = (generatedElPower - ElectricPowerConsumerSum) * essFactor * seconds;
					var maxCharge = (ElectricStorage.SOC - 1) * ElectricStorage.Capacity;
					var maxDischarge = ElectricStorage.SOC * ElectricStorage.Capacity;
					var batEnergy = energy.LimitTo(-maxDischarge, -maxCharge);
					ElectricStorage.Request(batEnergy);
				}
				Signals.CurrentCycleTimeInSeconds += seconds.Value();
			} catch (Exception ex) {
				//MessageBox.Show("Exception: " + ex.Message + " Stack Trace: " + ex.StackTrace);
				throw ex;
			}
		}

		public void ResetCalculations()
		{
			var modules = new List<IAbstractModule>() { M0, M1, M2, M3, M4, M5, M6, M7, M8 };
			foreach (var moduel in modules)
				moduel.ResetCalculations();
		}
	}
}
