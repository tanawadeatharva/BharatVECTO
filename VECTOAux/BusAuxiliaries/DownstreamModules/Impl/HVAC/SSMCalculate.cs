using System;
using System.Text;
using TUGraz.VectoCore.BusAuxiliaries.Interfaces.DownstreamModules.HVAC;

namespace TUGraz.VectoCore.BusAuxiliaries.DownstreamModules.Impl.HVAC
{
	// Modeling SSHVAC V07
	public class SSMCalculate : ISSMCalculate
	{
		private ISSMTOOL ssmTOOL;

		private ISSMRun Run1; // { get; set; }
		private ISSMRun Run2 { get; set; }

		// Constructor
		public SSMCalculate(ISSMTOOL ssmTool)
		{
			ssmTOOL = ssmTool;
			Run1 = new SSMRun(this.ssmTOOL, 1);
			Run2 = new SSMRun(this.ssmTOOL, 2);
		}


		// BASE RESULTS
		public double ElectricalWBase
		{
			get {
				var ElectricalWBaseWeightedAverage = 0.0;
				var gen = ssmTOOL.GenInputs;
				var EC_EnviromentalTemperatureBefore = gen.EC_EnviromentalTemperature;
				var EC_SolarBefore = gen.EC_Solar;

				// If batch mode is disabled use the EC_EnviromentalTemperature and EC_Solar variables. 
				// Else if batch is enable calculate the ElectricalWBase for each input in the AENV file and then calculate the weighted average
				if (!gen.EC_EnviromentalConditions_BatchEnabled)
					ElectricalWBaseWeightedAverage = CalculateElectricalWBase(gen, gen.EC_EnviromentalTemperature, gen.EC_Solar, 1);
				else {
					foreach (var envCondition in gen.EC_EnvironmentalConditionsMap.GetEnvironmentalConditions())
						ElectricalWBaseWeightedAverage += CalculateElectricalWBase(gen, envCondition.GetTemperature(), envCondition.GetSolar(), envCondition.GetNormalisedWeighting(gen.EC_EnvironmentalConditionsMap.GetEnvironmentalConditions()));
					gen.EC_EnviromentalTemperature = EC_EnviromentalTemperatureBefore;
					gen.EC_Solar = EC_SolarBefore;
				}

				return ElectricalWBaseWeightedAverage;
			}
		}

		public double MechanicalWBase
		{
			get {
				var MechanicalWBaseWeightedAverage = 0.0;
				var gen = ssmTOOL.GenInputs;
				var EC_EnviromentalTemperatureBefore = gen.EC_EnviromentalTemperature;
				var EC_SolarBefore = gen.EC_Solar;

				// If batch mode is disabled use the EC_EnviromentalTemperature and EC_Solar variables. 
				// Else if batch is enable calculate the MechanicalWBase for each input in the AENV file and then calculate the weighted average
				if (!gen.EC_EnviromentalConditions_BatchEnabled)
					MechanicalWBaseWeightedAverage = CalculateMechanicalWBase(gen, gen.EC_EnviromentalTemperature, gen.EC_Solar, 1);
				else {
					foreach (var envCondition in gen.EC_EnvironmentalConditionsMap.GetEnvironmentalConditions())
						MechanicalWBaseWeightedAverage += CalculateMechanicalWBase(gen, envCondition.GetTemperature(), envCondition.GetSolar(), envCondition.GetNormalisedWeighting(gen.EC_EnvironmentalConditionsMap.GetEnvironmentalConditions()));
					gen.EC_EnviromentalTemperature = EC_EnviromentalTemperatureBefore;
					gen.EC_Solar = EC_SolarBefore;
				}

				return MechanicalWBaseWeightedAverage;
			}
		}

		public double FuelPerHBase
		{
			get {
				var FuelLPerHBaseWeightedAverage = 0.0;
				var gen = ssmTOOL.GenInputs;
				var EC_EnviromentalTemperatureBefore = gen.EC_EnviromentalTemperature;
				var EC_SolarBefore = gen.EC_Solar;

				// If batch mode is disabled use the EC_EnviromentalTemperature and EC_Solar variables. 
				// Else if batch is enable calculate the FuelLPerHBase for each input in the AENV file and then calculate the weighted average
				if (!gen.EC_EnviromentalConditions_BatchEnabled)
					FuelLPerHBaseWeightedAverage = CalculateFuelLPerHBase(gen, gen.EC_EnviromentalTemperature, gen.EC_Solar, 1);
				else {
					foreach (var envCondition in gen.EC_EnvironmentalConditionsMap.GetEnvironmentalConditions())
						FuelLPerHBaseWeightedAverage += CalculateFuelLPerHBase(gen, envCondition.GetTemperature(), envCondition.GetSolar(), envCondition.GetNormalisedWeighting(gen.EC_EnvironmentalConditionsMap.GetEnvironmentalConditions()));
					gen.EC_EnviromentalTemperature = EC_EnviromentalTemperatureBefore;
					gen.EC_Solar = EC_SolarBefore;
				}

				return FuelLPerHBaseWeightedAverage;
			}
		}

		// ADJUSTED RESULTS
		public double ElectricalWAdjusted
		{
			get {
				var ElectricalWAdjustedAverage = 0.0;
				var gen = ssmTOOL.GenInputs;
				var tl = ssmTOOL.TechList;
				var EC_EnviromentalTemperatureBefore = gen.EC_EnviromentalTemperature;
				var EC_SolarBefore = gen.EC_Solar;

				// If batch mode is disabled use the EC_EnviromentalTemperature and EC_Solar variables. 
				// Else if batch is enable calculate the ElectricalWAdjusted for each input in the AENV file and then calculate the weighted average
				if (!gen.EC_EnviromentalConditions_BatchEnabled)
					ElectricalWAdjustedAverage = CalculateElectricalWAdjusted(gen, tl, gen.EC_EnviromentalTemperature, gen.EC_Solar, 1);
				else {
					foreach (var envCondition in gen.EC_EnvironmentalConditionsMap.GetEnvironmentalConditions())
						ElectricalWAdjustedAverage += CalculateElectricalWAdjusted(gen, tl, envCondition.GetTemperature(), envCondition.GetSolar(), envCondition.GetNormalisedWeighting(gen.EC_EnvironmentalConditionsMap.GetEnvironmentalConditions()));
					gen.EC_EnviromentalTemperature = EC_EnviromentalTemperatureBefore;
					gen.EC_Solar = EC_SolarBefore;
				}

				return ElectricalWAdjustedAverage;
			}
		}

		public double MechanicalWBaseAdjusted
		{
			get {
				var MechanicalWBaseAdjustedAverage = 0.0;
				var gen = ssmTOOL.GenInputs;
				var tl = ssmTOOL.TechList;
				var EC_EnviromentalTemperatureBefore = gen.EC_EnviromentalTemperature;
				var EC_SolarBefore = gen.EC_Solar;

				// If batch mode is disabled use the EC_EnviromentalTemperature and EC_Solar variables. 
				// Else if batch is enable calculate the MechanicalWBaseAdjusted for each input in the AENV file and then calculate the weighted average
				if (!gen.EC_EnviromentalConditions_BatchEnabled)
					MechanicalWBaseAdjustedAverage = CalculateMechanicalWBaseAdjusted(gen, tl, gen.EC_EnviromentalTemperature, gen.EC_Solar, 1);
				else {
					foreach (var envCondition in gen.EC_EnvironmentalConditionsMap.GetEnvironmentalConditions())
						MechanicalWBaseAdjustedAverage += CalculateMechanicalWBaseAdjusted(gen, tl, envCondition.GetTemperature(), envCondition.GetSolar(), envCondition.GetNormalisedWeighting(gen.EC_EnvironmentalConditionsMap.GetEnvironmentalConditions()));
					gen.EC_EnviromentalTemperature = EC_EnviromentalTemperatureBefore;
					gen.EC_Solar = EC_SolarBefore;
				}

				return MechanicalWBaseAdjustedAverage;
			}
		}

		public double FuelPerHBaseAdjusted
		{
			get {
				var FuelLPerHBaseAdjustedAverage = 0.0;
				var gen = ssmTOOL.GenInputs;
				var tl = ssmTOOL.TechList;
				var EC_EnviromentalTemperatureBefore = gen.EC_EnviromentalTemperature;
				var EC_SolarBefore = gen.EC_Solar;

				// If batch mode is disabled use the EC_EnviromentalTemperature and EC_Solar variables. 
				// Else if batch is enable calculate the FuelLPerHBaseAdjusted for each input in the AENV file and then calculate the weighted average
				if (!gen.EC_EnviromentalConditions_BatchEnabled)
					FuelLPerHBaseAdjustedAverage = CalculateFuelLPerHBaseAdjusted(gen, tl, gen.EC_EnviromentalTemperature, gen.EC_Solar, 1);
				else {
					foreach (var envCondition in gen.EC_EnvironmentalConditionsMap.GetEnvironmentalConditions())
						FuelLPerHBaseAdjustedAverage += CalculateFuelLPerHBaseAdjusted(gen, tl, envCondition.GetTemperature(), envCondition.GetSolar(), envCondition.GetNormalisedWeighting(gen.EC_EnvironmentalConditionsMap.GetEnvironmentalConditions()));
					gen.EC_EnviromentalTemperature = EC_EnviromentalTemperatureBefore;
					gen.EC_Solar = EC_SolarBefore;
				}

				return FuelLPerHBaseAdjustedAverage;
			}
		}




		// Base Values
		public double BaseHeatingW_Mechanical
		{
			get {
				return default(Double);
			}
		}

		public double BaseHeatingW_ElectricalCoolingHeating
		{
			get {
				return default(Double);
			}
		}

		public double BaseHeatingW_ElectricalVentilation
		{
			get {
				// =IF(AND(M89<0,M90<0),IF(AND(C62="yes",C66="high"),C33,IF(AND(C62="yes",C66="low"),C34,0)),0)

				var gen = ssmTOOL.GenInputs;

				// Dim C33 = gen.BC_HighVentPowerW
				// Dim C34 = gen.BC_LowVentPowerW
				// Dim C62 = gen.VEN_VentilationONDuringHeating
				// Dim C66 = gen.VEN_VentilationDuringHeating
				// Dim M89 = Me.Run1.TotalW
				// Dim M90 = Me.Run2.TotalW

				double res;

				res = Run1.TotalW < 0 && Run2.TotalW < 0 ? gen.VEN_VentilationOnDuringHeating && gen.VEN_VentilationDuringHeating.ToLower() == "high" ? gen.BC_HighVentPowerW : gen.VEN_VentilationOnDuringHeating && gen.VEN_VentilationDuringHeating.ToLower() == "low" ? gen.BC_LowVentPowerW : 0 : 0;


				return res;
			}
		}

		public double BaseHeatingW_FuelFiredHeating
		{
			get {
				// =IF(AND(M89<0,M90<0),VLOOKUP(MAX(M89:M90),M89:O90,3),0)

				// Dim M89 = Me.Run1.TotalW
				// Dim M90 = Me.Run2.TotalW
				// VLOOKUP(MAX(M89:M90),M89:O90  => VLOOKUP ( lookupValue, tableArray, colIndex, rangeLookup )

				// If both Run TotalW values are >=0 then return FuelW from Run with largest TotalW value, else return 0
				if ((Run1.TotalW < 0 && Run2.TotalW < 0))
					return Run1.TotalW > Run2.TotalW ? Run1.FuelW : Run2.FuelW;
				else
					return 0;
			}
		}

		public double BaseCoolingW_Mechanical
		{
			get {
				// =IF(C46<C28,0,IF(C53="electrical", 0, IF(AND(M89>0,M90>0),MIN(M89:M90),0)))

				var gen = ssmTOOL.GenInputs;

				// Dim C46 = gen.EC_EnviromentalTemperature
				// Dim C28 = gen.BC_TemperatureCoolingTurnsOff
				// Dim C53 = gen.AC_CompressorTypeDerived
				// Dim M89 = Run1.TotalW
				// Dim M90 = Run2.TotalW

				return gen.EC_EnviromentalTemperature < gen.BC_TemperatureCoolingTurnsOff ? 0 : gen.AC_CompressorTypeDerived.ToLower() == "electrical" ? 0 : Run1.TotalW > 0 && Run2.TotalW > 0 ? Math.Min(Run1.TotalW, Run2.TotalW) : 0;
			}
		}

		public double BaseCoolingW_ElectricalCoolingHeating
		{
			get {
				// =IF(C46<C28,0,IF(C53="electrical",IF(AND(M89>0,M90>0),MIN(M89:M90),0),0))

				var gen = ssmTOOL.GenInputs;

				// Dim C46 = gen.EC_EnviromentalTemperature
				// Dim C28 = gen.BC_TemperatureCoolingTurnsOff
				// Dim C53 = gen.AC_CompressorTypeDerived
				// Dim M89 = Run1.TotalW
				// Dim M90 = Run2.TotalW

				return gen.EC_EnviromentalTemperature < gen.BC_TemperatureCoolingTurnsOff ? 0 : gen.AC_CompressorTypeDerived.ToLower() == "electrical" ? Run1.TotalW > 0 && Run2.TotalW > 0 ? Math.Min(Run1.TotalW, Run2.TotalW) : 0 : 0;
			}
		}

		public double BaseCoolingW_ElectricalVentilation
		{
			get {
				// =IF(AND(C46>=C28,M89>0,M90>0),IF(AND(C64="yes",C67="high"),C33,IF(AND(C64="yes",C67="low"),C34,0)),0)

				var gen = ssmTOOL.GenInputs;

				// Dim C46 = gen.EC_EnviromentalTemperature
				// Dim C28 = gen.BC_TemperatureCoolingTurnsOff
				// Dim M89 = Run1.TotalW
				// Dim M90 = Run2.TotalW
				// Dim C64 = gen.VEN_VentilationDuringAC
				// Dim C67 = gen.VEN_VentilationDuringCooling
				// Dim C33 = gen.BC_HighVentPowerW
				// Dim C34 = gen.BC_LowVentPowerW

				return gen.EC_EnviromentalTemperature >= gen.BC_TemperatureCoolingTurnsOff && Run1.TotalW > 0 && Run2.TotalW > 0 ? gen.VEN_VentilationDuringAC && gen.VEN_VentilationDuringCooling.ToLower() == "high" ? gen.BC_HighVentPowerW : gen.VEN_VentilationDuringAC && gen.VEN_VentilationDuringCooling.ToLower() == "low" ? gen.BC_LowVentPowerW : 0 : 0;
			}
		}

		public double BaseCoolingW_FuelFiredHeating
		{
			get {
				return 0;
			}
		}

		public double BaseVentilationW_Mechanical
		{
			get {
				return default(Double);
			}
		}

		public double BaseVentilationW_ElectricalCoolingHeating
		{
			get {
				return default(Double);
			}
		}

		public double BaseVentilationW_ElectricalVentilation
		{
			get {
				// =IF(OR(AND(C46<C28,M89>0,M90>0),AND(M89>0,M90<0)),IF(AND(C63="yes",C65="high"),C33,IF(AND(C63="yes",C65="low"),C34,0)),0)

				var gen = ssmTOOL.GenInputs;

				// Dim C46 = gen.EC_EnviromentalTemperature
				// Dim C28 = gen.BC_TemperatureCoolingTurnsOff
				// Dim M89 = Run1.TotalW
				// Dim M90 = Run2.TotalW
				// Dim C63 = gen.VEN_VentilationWhenBothHeatingAndACInactive
				// Dim C65 = gen.VEN_VentilationFlowSettingWhenHeatingAndACInactive
				// Dim C33 = gen.BC_HighVentPowerW
				// Dim C34 = gen.BC_LowVentPowerW

				return (gen.EC_EnviromentalTemperature < gen.BC_TemperatureCoolingTurnsOff && Run1.TotalW > 0 && Run2.TotalW > 0) || (Run1.TotalW > 0 && Run2.TotalW < 0) ? gen.VEN_VentilationWhenBothHeatingAndACInactive && gen.VEN_VentilationFlowSettingWhenHeatingAndACInactive.ToLower() == "high" ? gen.BC_HighVentPowerW : gen.VEN_VentilationWhenBothHeatingAndACInactive && gen.VEN_VentilationFlowSettingWhenHeatingAndACInactive.ToLower() == "low" ? gen.BC_LowVentPowerW : 0 : 0;
			}
		}

		public double BaseVentilationW_FuelFiredHeating
		{
			get {
				return 0;
			}
		}

		// Adjusted Values
		public double TechListAdjustedHeatingW_Mechanical
		{
			get {
				return default(Double);
			}
		}

		public double TechListAdjustedHeatingW_ElectricalCoolingHeating
		{
			get {
				return default(Double);
			}
		}

		public double TechListAdjustedHeatingW_ElectricalVentilation
		{
			get {
				// =IF('TECH LIST INPUT'!O92>0,MIN('TECH LIST INPUT'!O92,C43),MAX('TECH LIST INPUT'!O92,-C43))
				var gen = ssmTOOL.GenInputs;
				var tl = ssmTOOL.TechList;

				// TECH LIST INPUT'!O92
				// Dim C43 As Double   =  gen.BC_MaxPossibleBenefitFromTechnologyList
				// Dim TLO92 As Double = tl.VHValueVariation


				return tl.VHValueVariation > 0 ? Math.Min(tl.VHValueVariation, gen.BC_MaxPossibleBenefitFromTechnologyList) : Math.Max(tl.VHValueVariation, -gen.BC_MaxPossibleBenefitFromTechnologyList);
			}
		}

		public double TechListAdjustedHeatingW_FuelFiredHeating
		{
			get {
				// =IF('TECH LIST INPUT'!N92>0,MIN('TECH LIST INPUT'!N92,C43),MAX('TECH LIST INPUT'!N92,-C43))

				var gen = ssmTOOL.GenInputs;
				var tl = ssmTOOL.TechList;

				// TECH LIST INPUT'!N92
				// Dim C43 As Double   =  gen.BC_MaxPossibleBenefitFromTechnologyList
				// Dim TLN92 As Double =  tl.HValueVariation


				return tl.HValueVariation > 0 ? Math.Min(tl.HValueVariation, gen.BC_MaxPossibleBenefitFromTechnologyList) : Math.Max(tl.HValueVariation, -gen.BC_MaxPossibleBenefitFromTechnologyList);
			}
		}

		public double TechListAdjustedCoolingW_Mechanical
		{
			get {
				// =IF(IF(C53="mechanical",'TECH LIST INPUT'!R92,0)>0,MIN(IF(C53="mechanical",'TECH LIST INPUT'!R92,0),C43),MAX(IF(C53="mechanical",'TECH LIST INPUT'!R92,0),-C43))

				var gen = ssmTOOL.GenInputs;
				var tl = ssmTOOL.TechList;
				double result;
				// Dim TLR92 As Double =  tl.CValueVariation 'TECH LIST INPUT'!R92
				// Dim C43 As Double   =  gen.BC_MaxPossibleBenefitFromTechnologyList
				// Dim C53 As string   =  gen.AC_CompressorType

				//result = If(If(gen.AC_CompressorType.ToLower() = "mechanical", tl.CValueVariation, 0) > 0,
				//			Math.Min(If(gen.AC_CompressorType.ToLower() = "mechanical", tl.CValueVariation, 0),
				//					gen.BC_MaxPossibleBenefitFromTechnologyList),
				//			Math.Max(If(gen.AC_CompressorType.ToLower() = "mechanical", tl.CValueVariation, 0),
				//					-gen.BC_MaxPossibleBenefitFromTechnologyList))

				result = (gen.AC_CompressorType.ToLower() == "mechanical"
					? tl.CValueVariation
					: 0) > 0
						? Math.Min(
							gen.AC_CompressorType.ToLower() == "mechanical" ? tl.CValueVariation : 0,
							gen.BC_MaxPossibleBenefitFromTechnologyList)
						: Math.Max(
							gen.AC_CompressorType.ToLower() == "mechanical" ? tl.CValueVariation : 0,
							-gen.BC_MaxPossibleBenefitFromTechnologyList);

				return result;
			}
		}

		public double TechListAdjustedCoolingW_ElectricalCoolingHeating
		{
			get {
				// =IF(IF(C53="mechanical",0,'TECH LIST INPUT'!R92)>0,MIN(IF(C53="mechanical",0,'TECH LIST INPUT'!R92),C43),MAX(IF(C53="mechanical",0,'TECH LIST INPUT'!R92),-C43))

				var gen = ssmTOOL.GenInputs;
				var tl = ssmTOOL.TechList;
				double result;

				// Dim TLR92 As Double =  tl.CValueVariation 'TECH LIST INPUT'!R92
				// Dim C43 As Double   =  gen.BC_MaxPossibleBenefitFromTechnologyList
				// Dim C53 As string   =  gen.AC_CompressorType

				result = gen.AC_CompressorType.ToLower() == "mechanical" ? 0 : tl.CValueVariation > 0 ? Math.Min(gen.AC_CompressorType.ToLower() == "mechanical" ? 0 : tl.CValueVariation, gen.BC_MaxPossibleBenefitFromTechnologyList) : Math.Max(gen.AC_CompressorType.ToLower() == "mechanical" ? 0 : tl.CValueVariation, -gen.BC_MaxPossibleBenefitFromTechnologyList);

				return result;
			}
		}

		public double TechListAdjustedCoolingW_ElectricalVentilation
		{
			get {
				// =IF('TECH LIST INPUT'!Q92>0,MIN('TECH LIST INPUT'!Q92,C43),MAX('TECH LIST INPUT'!Q92,-C43))

				var gen = ssmTOOL.GenInputs;
				var tl = ssmTOOL.TechList;

				// Dim TLQ92 As Double =  tl.VCValueVariation'TECH LIST INPUT'!Q92
				// Dim C43 As Double   =  gen.BC_MaxPossibleBenefitFromTechnologyList

				return tl.VCValueVariation > 0 ? Math.Min(tl.VCValueVariation, gen.BC_MaxPossibleBenefitFromTechnologyList) : Math.Max(tl.VCValueVariation, -gen.BC_MaxPossibleBenefitFromTechnologyList);
			}
		}

		public double TechListAdjustedCoolingW_FuelFiredHeating
		{
			get {
				return 0;
			}
		}

		public double TechListAdjustedVentilationW_Mechanical
		{
			get {
				return default(Double);
			}
		}

		public double TechListAdjustedVentilationW_ElectricalCoolingHeating
		{
			get {
				return default(Double);
			}
		}

		public double TechListAdjustedVentilationW_ElectricalVentilation
		{
			get {
				// =IF('TECH LIST INPUT'!P92>0,MIN('TECH LIST INPUT'!P92,C43),MAX('TECH LIST INPUT'!P92,-C43))

				var gen = ssmTOOL.GenInputs;
				var tl = ssmTOOL.TechList;

				// Dim TLP92 As Double =  tl.VVValueVariation  'TECH LIST INPUT'!P92
				// Dim C43 As Double   =  gen.BC_MaxPossibleBenefitFromTechnologyList


				return tl.VVValueVariation > 0 ? Math.Min(tl.VVValueVariation, gen.BC_MaxPossibleBenefitFromTechnologyList) : Math.Max(tl.VVValueVariation, -gen.BC_MaxPossibleBenefitFromTechnologyList);
			}
		}

		public double TechListAdjustedVentilationW_FuelFiredHeating
		{
			get {
				return 0;
			}
		}


		// Provides Diagnostic Information for the user which can be displayed on the form.
		// Based on the inputs generated, can be used to cross reference the Excel Model with the
		// Outputs generated here.
		public override string ToString()
		{
			var sb = new StringBuilder();
			var vbTab = "\t";

			sb.AppendLine("");
			sb.AppendLine("TechList Detail");
			sb.AppendLine("***********************");

			var nameLength = 40;
			var catLength = 15;
			var unitLength = 15;
			var firstValuePos = nameLength + catLength + unitLength + 2;
			string cat;
			string name;
			string units;

			sb.AppendLine(string.Format(new string(' ', firstValuePos) + "H{0}VH{0}VV{0}VC{0}C{0}", vbTab));


			foreach (var line in ssmTOOL.TechList.TechLines) {
				{
					var withBlock = line;
					int extraNameSpaces, extraCatSpaces, extraUnitSpaces;

					extraNameSpaces = nameLength - withBlock.BenefitName.Length;
					extraCatSpaces = catLength - withBlock.Category.Length;
					extraUnitSpaces = unitLength - withBlock.Units.Length;

					cat = line.Category.Substring(0, Math.Min(line.Category.Length, catLength)) + new string(' ', extraCatSpaces < 0 ? 0 : extraCatSpaces).Replace(" ", ".");
					name = line.BenefitName.Substring(0, Math.Min(line.BenefitName.Length, nameLength)) + new string(' ', extraNameSpaces < 0 ? 0 : extraNameSpaces).Replace(" ", ".");
					units = line.Units.Substring(0, Math.Min(line.Units.Length, unitLength)) + new string(' ', extraUnitSpaces < 0 ? 0 : extraUnitSpaces).Replace(" ", ".");

					sb.AppendLine(string.Format(units + cat + name + " {0}{1}{0}{2}{0}{3}{0}{4}{0}{5}", vbTab, withBlock.H.ToString("0.000"), withBlock.VH.ToString("0.000"), withBlock.VV.ToString("0.000"), withBlock.VC.ToString("0.000"), withBlock.C.ToString("0.000")));
				}
			}

			sb.AppendLine("");
			sb.AppendLine("TechList Totals");
			sb.AppendLine("***********************");

			{
				var withBlock = ssmTOOL.TechList;
				sb.AppendLine(vbTab + vbTab + "H" + vbTab + "VH" + vbTab + "VV" + vbTab + "VC" + vbTab + "C");
				sb.AppendLine(string.Format("Base Var %   {0}{1}{0}{2}{0}{3}{0}{4}{0}{5}", vbTab, withBlock.HValueVariation.ToString("0.000"), withBlock.VHValueVariation.ToString("0.000"), withBlock.VVValueVariation.ToString("0.000"), withBlock.VCValueVariation.ToString("0.000"), withBlock.CValueVariation.ToString("0.000")));
				sb.AppendLine(string.Format("Base Var KW  {0}{1}{0}{2}{0}{3}{0}{4}{0}{5}", vbTab, withBlock.HValueVariationKW.ToString("0.000"), withBlock.VHValueVariationKW.ToString("0.000"), withBlock.VVValueVariationKW.ToString("0.000"), withBlock.VCValueVariationKW.ToString("0.000"), withBlock.CValueVariationKW.ToString("0.000")));
			}


			// Runs
			sb.AppendLine(Run1.ToString());
			sb.AppendLine(Run2.ToString());

			// Staging Calcs
			sb.AppendLine("Staging Base Values");
			sb.AppendLine("*******************");
			sb.AppendLine(vbTab + vbTab + vbTab + "Mechanical" + vbTab + "Elec Cool/Heat" + vbTab + "Elec Vent" + vbTab + "Fuel Fired Heating");

			sb.AppendLine(string.Format("Heating   {0}{1}{0}{2}{0}{3}{0}{4}", vbTab + vbTab, BaseHeatingW_Mechanical.ToString("0.00"), BaseHeatingW_ElectricalCoolingHeating.ToString("0.00"), BaseHeatingW_ElectricalVentilation.ToString("0.00"), BaseHeatingW_FuelFiredHeating.ToString("0.00")));
			sb.AppendLine(string.Format("Cooling   {0}{1}{0}{2}{0}{3}{0}{4}", vbTab + vbTab, BaseCoolingW_Mechanical.ToString("0.00"), BaseCoolingW_ElectricalCoolingHeating.ToString("0.00"), BaseCoolingW_ElectricalVentilation.ToString("0.00"), BaseCoolingW_FuelFiredHeating.ToString("0.00")));
			sb.AppendLine(string.Format("Ventilate {0}{1}{0}{2}{0}{3}{0}{4}", vbTab + vbTab, BaseVentilationW_Mechanical.ToString("0.00"), BaseVentilationW_ElectricalCoolingHeating.ToString("0.00"), BaseVentilationW_ElectricalVentilation.ToString("0.00"), BaseVentilationW_FuelFiredHeating.ToString("0.00")));

			sb.AppendLine("");
			sb.AppendLine("Staging Adjusted Values");
			sb.AppendLine("***********************");

			sb.AppendLine(string.Format("Heating   {0}{1}{0}{2}{0}{3}{0}{4}", vbTab + vbTab, TechListAdjustedHeatingW_Mechanical.ToString("0.00"), TechListAdjustedHeatingW_ElectricalCoolingHeating.ToString("0.00"), TechListAdjustedHeatingW_ElectricalVentilation.ToString("0.00"), TechListAdjustedHeatingW_FuelFiredHeating.ToString("0.00")));
			sb.AppendLine(string.Format("Cooling   {0}{1}{0}{2}{0}{3}{0}{4}", vbTab + vbTab, TechListAdjustedCoolingW_Mechanical.ToString("0.00"), TechListAdjustedCoolingW_ElectricalCoolingHeating.ToString("0.00"), TechListAdjustedCoolingW_ElectricalVentilation.ToString("0.00"), TechListAdjustedCoolingW_FuelFiredHeating.ToString("0.00")));
			sb.AppendLine(string.Format("Ventilate {0}{1}{0}{2}{0}{3}{0}{4}", vbTab + vbTab, TechListAdjustedVentilationW_Mechanical.ToString("0.00"), TechListAdjustedVentilationW_ElectricalCoolingHeating.ToString("0.00"), TechListAdjustedVentilationW_ElectricalVentilation.ToString("0.00"), TechListAdjustedVentilationW_FuelFiredHeating.ToString("0.00")));


			return sb.ToString();
		}

		private double CalculateElectricalWBase(ISSMGenInputs genInputs, double EnviromentalTemperature, double Solar, double Weight)
		{

			// MIN(SUM(H94),C54*1000)/C59+SUM(I93:I95)

			genInputs.EC_EnviromentalTemperature = EnviromentalTemperature;
			genInputs.EC_Solar = Solar;

			// Dim H94 = BaseCoolingW_ElectricalCoolingHeating
			// Dim C54 = genInputs.AC_CompressorCapacitykW
			// Dim C59 = genInputs.AC_COP
			// Dim I93 = BaseHeatingW_ElectricalVentilation
			// Dim I94 = BaseCoolingW_ElectricalVentilation
			// Dim I95 = BaseVentilationW_ElectricalVentilation

			var ElectricalWBaseCurrentResult = Math.Min(BaseCoolingW_ElectricalCoolingHeating, genInputs.AC_CompressorCapacitykW * 1000) / (double)genInputs.AC_COP + BaseHeatingW_ElectricalVentilation + BaseCoolingW_ElectricalVentilation + BaseVentilationW_ElectricalVentilation;

			return ElectricalWBaseCurrentResult * Weight;
		}

		private double CalculateMechanicalWBase(ISSMGenInputs genInputs, double EnviromentalTemperature, double Solar, double Weight)
		{

			// =MIN(F94,C54*1000)/C59

			genInputs.EC_EnviromentalTemperature = EnviromentalTemperature;
			genInputs.EC_Solar = Solar;

			// Dim F94 = BaseCoolingW_Mechanical
			// Dim C54 = genInputs.AC_CompressorCapacitykW
			// Dim C59 = genInputs.AC_COP 

			var MechanicalWBaseCurrentResult = Math.Min(BaseCoolingW_Mechanical, genInputs.AC_CompressorCapacitykW * 1000) / (double)genInputs.AC_COP;

			return MechanicalWBaseCurrentResult * Weight;
		}

		private double CalculateFuelLPerHBase(ISSMGenInputs genInputs, double EnviromentalTemperature, double Solar, double Weight)
		{

			// =(MIN(ABS(J93/1000),C71)/C37)*(1/(C39*C38))

			genInputs.EC_EnviromentalTemperature = EnviromentalTemperature;
			genInputs.EC_Solar = Solar;

			// Dim J93 = BaseHeatingW_FuelFiredHeating
			// Dim C71 = genInputs.AH_FuelFiredHeaterkW
			// Dim C37 = genInputs.BC_AuxHeaterEfficiency
			// Dim C39 = ssmTOOL.HVACConstants.FuelDensity
			// Dim C38 = genInputs.BC_GCVDieselOrHeatingOil

			var FuelLPerHBaseCurrentResult = (Math.Min(Math.Abs(BaseHeatingW_FuelFiredHeating / 1000), genInputs.AH_FuelFiredHeaterkW) / (double)genInputs.BC_AuxHeaterEfficiency) * (1 / (double)(genInputs.BC_GCVDieselOrHeatingOil * ssmTOOL.HVACConstants.FuelDensityAsGramPerLiter));

			return FuelLPerHBaseCurrentResult * Weight;
		}

		private double CalculateElectricalWAdjusted(ISSMGenInputs genInputs, ISSMTechList tecList, double EnviromentalTemperature, double Solar, double Weight)
		{

			// =(MIN((H94*(1-H100)),C54*1000)/C59)+(I93*(1-I99))+(I94*(1-I100))+(I95*(1-I101))

			genInputs.EC_EnviromentalTemperature = EnviromentalTemperature;
			genInputs.EC_Solar = Solar;

			var H94 = BaseCoolingW_ElectricalCoolingHeating;
			var H100 = TechListAdjustedCoolingW_ElectricalCoolingHeating;
			var C54 = genInputs.AC_CompressorCapacitykW;
			var C59 = genInputs.AC_COP;

			var I93 = BaseHeatingW_ElectricalVentilation;
			var I94 = BaseCoolingW_ElectricalVentilation;
			var I95 = BaseVentilationW_ElectricalVentilation;
			var I99 = TechListAdjustedHeatingW_ElectricalVentilation;
			var I100 = TechListAdjustedCoolingW_ElectricalVentilation;
			var I101 = TechListAdjustedVentilationW_ElectricalVentilation;

			var ElectricalWAdjusted = (Math.Min((H94 * (1 - H100)), C54 * 1000) / C59) + (I93 * (1 - I99)) + (I94 * (1 - I100)) + (I95 * (1 - I101));

			return ElectricalWAdjusted * Weight;
		}

		private double CalculateMechanicalWBaseAdjusted(ISSMGenInputs genInputs, ISSMTechList tecList, double EnviromentalTemperature, double Solar, double Weight)
		{

			// =(MIN((F94*(1-F100)),C54*1000)/C59)

			genInputs.EC_EnviromentalTemperature = EnviromentalTemperature;
			genInputs.EC_Solar = Solar;

			var F94 = BaseCoolingW_Mechanical;
			var F100 = TechListAdjustedCoolingW_Mechanical;
			var C54 = genInputs.AC_CompressorCapacitykW;
			var C59 = genInputs.AC_COP;

			var MechanicalWBaseAdjusted = (Math.Min((F94 * (1 - F100)), C54 * 1000) / C59);

			return MechanicalWBaseAdjusted * Weight;
		}

		private double CalculateFuelLPerHBaseAdjusted(ISSMGenInputs genInputs, ISSMTechList tecList, double EnviromentalTemperature, double Solar, double Weight)
		{

			// =MIN(ABS(IF(AND(M89<0,M90<0),VLOOKUP(MAX(M89:M90),M89:P90,4),0)/1000),C71)/C37*(1/(C39*C38))

			genInputs.EC_EnviromentalTemperature = EnviromentalTemperature;
			genInputs.EC_Solar = Solar;

			// Dim M89 = Run1.TotalW
			// Dim M90 = genInputs.BC_GCVDieselOrHeatingOil
			// Dim C71 = genInputs.AH_FuelFiredHeaterkW
			// Dim C37 = genInputs.BC_AuxHeaterEfficiency
			// Dim C38 = genInputs.BC_GCVDieselOrHeatingOil
			// Dim C39 = ssmTOOL.HVACConstants.FuelDensity

			double result = 0;

			if (Run1.TotalW < 0 && Run2.TotalW < 0)
				result = Math.Abs(Run1.TotalW > Run2.TotalW ? Run1.TechListAmendedFuelW : Run2.TechListAmendedFuelW / (double)1000);

			var FuelLPerHBaseAdjusted = Math.Min(result, genInputs.AH_FuelFiredHeaterkW) / (double)genInputs.BC_AuxHeaterEfficiency * (1 / (double)(genInputs.BC_GCVDieselOrHeatingOil * ssmTOOL.HVACConstants.FuelDensityAsGramPerLiter));

			return FuelLPerHBaseAdjusted * Weight;
		}
	}
}
