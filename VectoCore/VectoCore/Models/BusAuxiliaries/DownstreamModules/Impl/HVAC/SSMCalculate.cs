using System;
using System.Text;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules.HVAC;

namespace TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.HVAC
{
	// Modeling SSHVAC V07
	public class SSMCalculate : ISSMCalculate
	{
		private ISSMTOOL ssmTOOL;

		private ISSMRun Run1; // { get; set; }
		private ISSMRun Run2; // { get; set; }

		// Constructor
		public SSMCalculate(ISSMTOOL ssmTool)
		{
			ssmTOOL = ssmTool;
			Run1 = new SSMRun(ssmTOOL, 1);
			Run2 = new SSMRun(ssmTOOL, 2);
		}


		// BASE RESULTS
		public Watt ElectricalWBase
		{
			get {
				var ElectricalWBaseWeightedAverage = 0.SI<Watt>();
				var ec = ssmTOOL.SSMInputs.EnvironmentalConditions;
				

				// If batch mode is disabled use the EC_EnviromentalTemperature and EC_Solar variables. 
				// Else if batch is enable calculate the ElectricalWBase for each input in the AENV file and then calculate the weighted average
				if (!ec.BatchMode)
					ElectricalWBaseWeightedAverage = CalculateElectricalWBase(
						ssmTOOL.SSMInputs, ec.DefaultConditions);
				else {
					foreach (var envCondition in ec.EnvironmentalConditionsMap.GetEnvironmentalConditions())
						ElectricalWBaseWeightedAverage += CalculateElectricalWBase(
							ssmTOOL.SSMInputs, envCondition);

					
				}

				return ElectricalWBaseWeightedAverage;
			}
		}

		public Watt MechanicalWBase
		{
			get {
				var MechanicalWBaseWeightedAverage = 0.SI<Watt>();
				var ec = ssmTOOL.SSMInputs.EnvironmentalConditions;
				

				// If batch mode is disabled use the EC_EnviromentalTemperature and EC_Solar variables. 
				// Else if batch is enable calculate the MechanicalWBase for each input in the AENV file and then calculate the weighted average
				if (!ec.BatchMode)
					MechanicalWBaseWeightedAverage = CalculateMechanicalWBase(
						ssmTOOL.SSMInputs, ec.DefaultConditions);
				else {
					foreach (var envCondition in ec.EnvironmentalConditionsMap.GetEnvironmentalConditions())
						MechanicalWBaseWeightedAverage += CalculateMechanicalWBase(
							ssmTOOL.SSMInputs, envCondition);

					
				}

				return MechanicalWBaseWeightedAverage;
			}
		}

		public KilogramPerSecond FuelPerHBase
		{
			get {
				var FuelLPerHBaseWeightedAverage = 0.0.SI<KilogramPerSecond>();
				var ec = ssmTOOL.SSMInputs.EnvironmentalConditions;
				

				// If batch mode is disabled use the EC_EnviromentalTemperature and EC_Solar variables. 
				// Else if batch is enable calculate the FuelLPerHBase for each input in the AENV file and then calculate the weighted average
				if (!ec.BatchMode)
					FuelLPerHBaseWeightedAverage = CalculateFuelLPerHBase(ssmTOOL.SSMInputs, ec.DefaultConditions);
				else {
					foreach (var envCondition in ec.EnvironmentalConditionsMap.GetEnvironmentalConditions())
						FuelLPerHBaseWeightedAverage += CalculateFuelLPerHBase(ssmTOOL.SSMInputs, envCondition);

					
				}

				return FuelLPerHBaseWeightedAverage;
			}
		}

		// ADJUSTED RESULTS
		public Watt ElectricalWAdjusted
		{
			get {
				var ElectricalWAdjustedAverage = 0.0.SI<Watt>();
				var ec = ssmTOOL.SSMInputs.EnvironmentalConditions;
				var tl = ssmTOOL.TechList;
				

				// If batch mode is disabled use the EC_EnviromentalTemperature and EC_Solar variables. 
				// Else if batch is enable calculate the ElectricalWAdjusted for each input in the AENV file and then calculate the weighted average
				if (!ec.BatchMode)
					ElectricalWAdjustedAverage = CalculateElectricalWAdjusted(
						ssmTOOL.SSMInputs, tl, ec.DefaultConditions);
				else {
					foreach (var envCondition in ec.EnvironmentalConditionsMap.GetEnvironmentalConditions())
						ElectricalWAdjustedAverage += CalculateElectricalWAdjusted(ssmTOOL.SSMInputs, tl, envCondition);

					
				}

				return ElectricalWAdjustedAverage;
			}
		}

		public Watt MechanicalWBaseAdjusted
		{
			get {
				var MechanicalWBaseAdjustedAverage = 0.0.SI<Watt>();
				var ec = ssmTOOL.SSMInputs.EnvironmentalConditions;
				var tl = ssmTOOL.TechList;
				

				// If batch mode is disabled use the EC_EnviromentalTemperature and EC_Solar variables. 
				// Else if batch is enable calculate the MechanicalWBaseAdjusted for each input in the AENV file and then calculate the weighted average
				if (!ec.BatchMode)
					MechanicalWBaseAdjustedAverage = CalculateMechanicalWBaseAdjusted(
						ssmTOOL.SSMInputs, tl, ec.DefaultConditions);
				else {
					foreach (var envCondition in ec.EnvironmentalConditionsMap.GetEnvironmentalConditions())
						MechanicalWBaseAdjustedAverage += CalculateMechanicalWBaseAdjusted(ssmTOOL.SSMInputs, tl, envCondition);

					
				}

				return MechanicalWBaseAdjustedAverage;
			}
		}

		public KilogramPerSecond FuelPerHBaseAdjusted
		{
			get {
				var FuelLPerHBaseAdjustedAverage = 0.0.SI<KilogramPerSecond>();
				var gen = ssmTOOL.SSMInputs.EnvironmentalConditions;
				var tl = ssmTOOL.TechList;
				

				// If batch mode is disabled use the EC_EnviromentalTemperature and EC_Solar variables. 
				// Else if batch is enable calculate the FuelLPerHBaseAdjusted for each input in the AENV file and then calculate the weighted average
				if (!gen.BatchMode)
					FuelLPerHBaseAdjustedAverage = CalculateFuelLPerHBaseAdjusted(
						ssmTOOL.SSMInputs, tl, gen.DefaultConditions);
				else {
					foreach (var envCondition in gen.EnvironmentalConditionsMap.GetEnvironmentalConditions())
						FuelLPerHBaseAdjustedAverage += CalculateFuelLPerHBaseAdjusted(
							ssmTOOL.SSMInputs, tl, envCondition);

					
				}

				return FuelLPerHBaseAdjustedAverage;
			}
		}


		// Base Values
		
		public Watt BaseHeatingW_ElectricalVentilation(Kelvin environmentalTemperature, WattPerSquareMeter solar)
		{
			
				// =IF(AND(M89<0,M90<0),IF(AND(C62="yes",C66="high"),C33,IF(AND(C62="yes",C66="low"),C34,0)),0)

				var ventilation = ssmTOOL.SSMInputs.Ventilation;
				var bc = ssmTOOL.SSMInputs.BoundaryConditions;

				// Dim C33 = gen.BC_HighVentPower
				// Dim C34 = gen.BC_LowVentPower
				// Dim C62 = gen.VEN_VentilationONDuringHeating
				// Dim C66 = gen.VEN_VentilationDuringHeating
				// Dim M89 = Me.Run1.TotalW
				// Dim M90 = Me.Run2.TotalW

				var run1TotalW = Run1.TotalW(environmentalTemperature, solar);
				var run2TotalW = Run2.TotalW(environmentalTemperature, solar);

				var res = run1TotalW < 0 && run2TotalW < 0
					? ventilation.VentilationOnDuringHeating && ventilation.VentilationDuringHeating == VentilationLevel.High
						? bc.HighVentPower
						: ventilation.VentilationOnDuringHeating && ventilation.VentilationDuringHeating ==VentilationLevel.Low
							? bc.LowVentPower
							: 0.SI<Watt>()
					: 0.SI<Watt>();

				return res;
			
		}

		public Watt BaseHeatingW_FuelFiredHeating(Kelvin environmentalTemperature, WattPerSquareMeter solar)
		{
			
				// =IF(AND(M89<0,M90<0),VLOOKUP(MAX(M89:M90),M89:O90,3),0)

				// Dim M89 = Me.Run1.TotalW
				// Dim M90 = Me.Run2.TotalW
				// VLOOKUP(MAX(M89:M90),M89:O90  => VLOOKUP ( lookupValue, tableArray, colIndex, rangeLookup )

				// If both Run TotalW values are >=0 then return FuelW from Run with largest TotalW value, else return 0
				var run1TotalW = Run1.TotalW(environmentalTemperature, solar);
				var run2TotalW = Run2.TotalW(environmentalTemperature, solar);

				if ((run1TotalW < 0 && run2TotalW < 0)) {
					return run1TotalW > run2TotalW ? Run1.FuelW(environmentalTemperature, solar) : Run2.FuelW(environmentalTemperature, solar);
				}

				return 0.SI<Watt>();
			
		}

		protected Watt BaseCoolingW_Mechanical(Kelvin environmentalTemperature, WattPerSquareMeter solar)
		{
			// =IF(C46<C28,0,IF(C53="electrical", 0, IF(AND(M89>0,M90>0),MIN(M89:M90),0)))

			var gen = ssmTOOL.SSMInputs;

			// Dim C46 = gen.EC_EnviromentalTemperature
			// Dim C28 = gen.BC_TemperatureCoolingTurnsOff
			// Dim C53 = gen.AC_CompressorTypeDerived
			// Dim M89 = Run1.TotalW
			// Dim M90 = Run2.TotalW

			var run1TotalW = Run1.TotalW(environmentalTemperature, solar);
			var run2TotalW = Run2.TotalW(environmentalTemperature, solar);

			return environmentalTemperature < gen.BoundaryConditions.TemperatureCoolingTurnsOff
				? 0.SI<Watt>()
				: gen.ACSystem.CompressorType.IsElectrical()
					? 0.SI<Watt>()
					: run1TotalW > 0 && run2TotalW > 0
						? VectoMath.Min(run1TotalW, run2TotalW)
						: 0.SI<Watt>();
		}

		protected Watt BaseCoolingW_ElectricalCoolingHeating(Kelvin environmentalTemperature, WattPerSquareMeter solar)
		{
			// =IF(C46<C28,0,IF(C53="electrical",IF(AND(M89>0,M90>0),MIN(M89:M90),0),0))

			var gen = ssmTOOL.SSMInputs;

			// Dim C46 = gen.EC_EnviromentalTemperature
			// Dim C28 = gen.BC_TemperatureCoolingTurnsOff
			// Dim C53 = gen.AC_CompressorTypeDerived
			// Dim M89 = Run1.TotalW
			// Dim M90 = Run2.TotalW

			var run1TotalW = Run1.TotalW(environmentalTemperature, solar);
			var run2TotalW = Run2.TotalW(environmentalTemperature, solar);
			return environmentalTemperature < gen.BoundaryConditions.TemperatureCoolingTurnsOff
				? 0.SI<Watt>()
				: gen.ACSystem.CompressorType.IsElectrical()
					? run1TotalW > 0 && run2TotalW > 0
						? VectoMath.Min(run1TotalW, run2TotalW)
						: 0.SI<Watt>()
					: 0.SI<Watt>();
		}

		protected Watt BaseCoolingW_ElectricalVentilation(Kelvin environmentalTemperature, WattPerSquareMeter solar)
		{
			// =IF(AND(C46>=C28,M89>0,M90>0),IF(AND(C64="yes",C67="high"),C33,IF(AND(C64="yes",C67="low"),C34,0)),0)

			var gen = ssmTOOL.SSMInputs;

			// Dim C46 = gen.EC_EnviromentalTemperature
			// Dim C28 = gen.BC_TemperatureCoolingTurnsOff
			// Dim M89 = Run1.TotalW
			// Dim M90 = Run2.TotalW
			// Dim C64 = gen.VEN_VentilationDuringAC
			// Dim C67 = gen.VEN_VentilationDuringCooling
			// Dim C33 = gen.BC_HighVentPower
			// Dim C34 = gen.BC_LowVentPower

			var run1TotalW = Run1.TotalW(environmentalTemperature, solar);
			var run2TotalW = Run2.TotalW(environmentalTemperature, solar);

			return environmentalTemperature >= gen.BoundaryConditions.TemperatureCoolingTurnsOff && run1TotalW > 0 &&
					run2TotalW > 0
				? gen.Ventilation.VentilationDuringAC && gen.Ventilation.VentilationDuringCooling == VentilationLevel.High
					? gen.BoundaryConditions.HighVentPower
					: gen.Ventilation.VentilationDuringAC && gen.Ventilation.VentilationDuringCooling == VentilationLevel.Low
						? gen.BoundaryConditions.LowVentPower
						: 0.SI<Watt>()
				: 0.SI<Watt>();
		}

		public Watt BaseVentilationW_ElectricalVentilation(Kelvin environmentalTemperature, WattPerSquareMeter solar)
		{
			
				// =IF(OR(AND(C46<C28,M89>0,M90>0),AND(M89>0,M90<0)),IF(AND(C63="yes",C65="high"),C33,IF(AND(C63="yes",C65="low"),C34,0)),0)

				var gen = ssmTOOL.SSMInputs;

				// Dim C46 = gen.EC_EnviromentalTemperature
				// Dim C28 = gen.BC_TemperatureCoolingTurnsOff
				// Dim M89 = Run1.TotalW
				// Dim M90 = Run2.TotalW
				// Dim C63 = gen.VEN_VentilationWhenBothHeatingAndACInactive
				// Dim C65 = gen.VEN_VentilationFlowSettingWhenHeatingAndACInactive
				// Dim C33 = gen.BC_HighVentPower
				// Dim C34 = gen.BC_LowVentPower

				var run1TotalW = Run1.TotalW(environmentalTemperature, solar);
				var run2TotalW = Run2.TotalW(environmentalTemperature, solar);

				return (environmentalTemperature < gen.BoundaryConditions.TemperatureCoolingTurnsOff &&
						run1TotalW > 0 && run2TotalW > 0) ||
						(run1TotalW > 0 && run2TotalW < 0)
					? gen.Ventilation.VentilationWhenBothHeatingAndACInactive &&
					gen.Ventilation.VentilationFlowSettingWhenHeatingAndACInactive == VentilationLevel.High
						? gen.BoundaryConditions.HighVentPower
						: gen.Ventilation.VentilationWhenBothHeatingAndACInactive &&
						gen.Ventilation.VentilationFlowSettingWhenHeatingAndACInactive == VentilationLevel.Low
							? gen.BoundaryConditions.LowVentPower
							: 0.SI<Watt>()
					: 0.SI<Watt>();
			
		}

		
		// Adjusted Values
		
		public double TechListAdjustedHeatingW_ElectricalVentilation
		{
			get {
				// =IF('TECH LIST INPUT'!O92>0,MIN('TECH LIST INPUT'!O92,C43),MAX('TECH LIST INPUT'!O92,-C43))
				var bc = ssmTOOL.SSMInputs.BoundaryConditions;
				var tl = ssmTOOL.TechList;

				// TECH LIST INPUT'!O92
				// Dim C43 As Double   =  gen.BC_MaxPossibleBenefitFromTechnologyList
				// Dim TLO92 As Double = tl.VHValueVariation

				return tl.VHValueVariation > 0
					? Math.Min(tl.VHValueVariation, bc.MaxPossibleBenefitFromTechnologyList)
					: Math.Max(tl.VHValueVariation, -bc.MaxPossibleBenefitFromTechnologyList);
			}
		}

		public double TechListAdjustedHeatingW_FuelFiredHeating
		{
			get {
				// =IF('TECH LIST INPUT'!N92>0,MIN('TECH LIST INPUT'!N92,C43),MAX('TECH LIST INPUT'!N92,-C43))

				var bc = ssmTOOL.SSMInputs.BoundaryConditions;
				var tl = ssmTOOL.TechList;

				// TECH LIST INPUT'!N92
				// Dim C43 As Double   =  gen.BC_MaxPossibleBenefitFromTechnologyList
				// Dim TLN92 As Double =  tl.HValueVariation

				return tl.HValueVariation > 0
					? Math.Min(tl.HValueVariation, bc.MaxPossibleBenefitFromTechnologyList)
					: Math.Max(tl.HValueVariation, -bc.MaxPossibleBenefitFromTechnologyList);
			}
		}

		public double TechListAdjustedCoolingW_Mechanical
		{
			get {
				// =IF(IF(C53="mechanical",'TECH LIST INPUT'!R92,0)>0,MIN(IF(C53="mechanical",'TECH LIST INPUT'!R92,0),C43),MAX(IF(C53="mechanical",'TECH LIST INPUT'!R92,0),-C43))

				var gen = ssmTOOL.SSMInputs;
				var tl = ssmTOOL.TechList;
				
				// Dim TLR92 As Double =  tl.CValueVariation 'TECH LIST INPUT'!R92
				// Dim C43 As Double   =  gen.BC_MaxPossibleBenefitFromTechnologyList
				// Dim C53 As string   =  gen.AC_CompressorType

				//result = If(If(gen.AC_CompressorType.ToLower() = "mechanical", tl.CValueVariation, 0) > 0,
				//			Math.Min(If(gen.AC_CompressorType.ToLower() = "mechanical", tl.CValueVariation, 0),
				//					gen.BC_MaxPossibleBenefitFromTechnologyList),
				//			Math.Max(If(gen.AC_CompressorType.ToLower() = "mechanical", tl.CValueVariation, 0),
				//					-gen.BC_MaxPossibleBenefitFromTechnologyList))

				if (gen.ACSystem.CompressorType.IsElectrical()) {
					return 0;
				}

				return tl.CValueVariation.LimitTo(
					-gen.BoundaryConditions.MaxPossibleBenefitFromTechnologyList,
					gen.BoundaryConditions.MaxPossibleBenefitFromTechnologyList);
			}
		}

		public double TechListAdjustedCoolingW_ElectricalCoolingHeating
		{
			get {
				// =IF(IF(C53="mechanical",0,'TECH LIST INPUT'!R92)>0,MIN(IF(C53="mechanical",0,'TECH LIST INPUT'!R92),C43),MAX(IF(C53="mechanical",0,'TECH LIST INPUT'!R92),-C43))

				var gen = ssmTOOL.SSMInputs;
				var tl = ssmTOOL.TechList;
				
				// Dim TLR92 As Double =  tl.CValueVariation 'TECH LIST INPUT'!R92
				// Dim C43 As Double   =  gen.BC_MaxPossibleBenefitFromTechnologyList
				// Dim C53 As string   =  gen.AC_CompressorType

				if (gen.ACSystem.CompressorType.IsMechanical()) {
					return 0;
				}

				return tl.CValueVariation.LimitTo(
					-gen.BoundaryConditions.MaxPossibleBenefitFromTechnologyList,
					gen.BoundaryConditions.MaxPossibleBenefitFromTechnologyList);
			}
		}

		public double TechListAdjustedCoolingW_ElectricalVentilation
		{
			get {
				// =IF('TECH LIST INPUT'!Q92>0,MIN('TECH LIST INPUT'!Q92,C43),MAX('TECH LIST INPUT'!Q92,-C43))

				var gen = ssmTOOL.SSMInputs.BoundaryConditions;
				var tl = ssmTOOL.TechList;

				// Dim TLQ92 As Double =  tl.VCValueVariation'TECH LIST INPUT'!Q92
				// Dim C43 As Double   =  gen.BC_MaxPossibleBenefitFromTechnologyList

				return tl.VCValueVariation > 0
					? Math.Min(tl.VCValueVariation, gen.MaxPossibleBenefitFromTechnologyList)
					: Math.Max(tl.VCValueVariation, -gen.MaxPossibleBenefitFromTechnologyList);
			}
		}

		

		public double TechListAdjustedVentilationW_ElectricalVentilation
		{
			get {
				// =IF('TECH LIST INPUT'!P92>0,MIN('TECH LIST INPUT'!P92,C43),MAX('TECH LIST INPUT'!P92,-C43))

				var gen = ssmTOOL.SSMInputs.BoundaryConditions;
				var tl = ssmTOOL.TechList;

				// Dim TLP92 As Double =  tl.VVValueVariation  'TECH LIST INPUT'!P92
				// Dim C43 As Double   =  gen.BC_MaxPossibleBenefitFromTechnologyList

				return tl.VVValueVariation > 0
					? Math.Min(tl.VVValueVariation, gen.MaxPossibleBenefitFromTechnologyList)
					: Math.Max(tl.VVValueVariation, -gen.MaxPossibleBenefitFromTechnologyList);
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

			sb.AppendLine(string.Format(new string(' ', firstValuePos) + "H{0}VH{0}VV{0}VC{0}C{0}", vbTab));

			foreach (var line in ssmTOOL.TechList.TechLines) {
				{
					var withBlock = line;

					var extraNameSpaces = nameLength - withBlock.BenefitName.Length;
					var extraCatSpaces = catLength - withBlock.Category.Length;

					cat = line.Category.Substring(0, Math.Min(line.Category.Length, catLength)) +
						new string(' ', extraCatSpaces < 0 ? 0 : extraCatSpaces).Replace(" ", ".");
					name = line.BenefitName.Substring(0, Math.Min(line.BenefitName.Length, nameLength)) +
							new string(' ', extraNameSpaces < 0 ? 0 : extraNameSpaces).Replace(" ", ".");

					sb.AppendLine(
						string.Format(
							cat + name + " {0}{1}{0}{2}{0}{3}{0}{4}{0}{5}", vbTab, withBlock.H.ToString("0.000"),
							withBlock.VH.ToString("0.000"), withBlock.VV.ToString("0.000"), withBlock.VC.ToString("0.000"),
							withBlock.C.ToString("0.000")));
				}
			}

			sb.AppendLine("");
			sb.AppendLine("TechList Totals");
			sb.AppendLine("***********************");

			{
				var withBlock = ssmTOOL.TechList;
				sb.AppendLine(vbTab + vbTab + "H" + vbTab + "VH" + vbTab + "VV" + vbTab + "VC" + vbTab + "C");
				sb.AppendLine(
					string.Format(
						"Base Var %   {0}{1}{0}{2}{0}{3}{0}{4}{0}{5}", vbTab, withBlock.HValueVariation.ToString("0.000"),
						withBlock.VHValueVariation.ToString("0.000"), withBlock.VVValueVariation.ToString("0.000"),
						withBlock.VCValueVariation.ToString("0.000"), withBlock.CValueVariation.ToString("0.000")));
			}

			// Runs
			sb.AppendLine(Run1.ToString());
			sb.AppendLine(Run2.ToString());

			
			return sb.ToString();
		}

		private Watt CalculateElectricalWBase(ISSMInputs genInputs, IEnvironmentalConditionsMapEntry env)
		{
			// MIN(SUM(H94),C54*1000)/C59+SUM(I93:I95)

			

			// Dim H94 = BaseCoolingW_ElectricalCoolingHeating
			// Dim C54 = genInputs.AC_CompressorCapacitykW
			// Dim C59 = genInputs.AC_COP
			// Dim I93 = BaseHeatingW_ElectricalVentilation
			// Dim I94 = BaseCoolingW_ElectricalVentilation
			// Dim I95 = BaseVentilationW_ElectricalVentilation

			

			var electricalWBaseCurrentResult =
				VectoMath.Min(
					BaseCoolingW_ElectricalCoolingHeating(env.Temperature, env.Solar), genInputs.ACSystem.CompressorCapacity) /
				genInputs.ACSystem.COP + BaseHeatingW_ElectricalVentilation(env.Temperature, env.Solar) +
				BaseCoolingW_ElectricalVentilation(env.Temperature, env.Solar) +
				BaseVentilationW_ElectricalVentilation(env.Temperature, env.Solar);

			return electricalWBaseCurrentResult * env.Weighting;
		}

		private Watt CalculateMechanicalWBase(ISSMInputs genInputs, IEnvironmentalConditionsMapEntry env)
		{
			// =MIN(F94,C54*1000)/C59



			// Dim F94 = BaseCoolingW_Mechanical
			// Dim C54 = genInputs.AC_CompressorCapacitykW
			// Dim C59 = genInputs.AC_COP 

			var MechanicalWBaseCurrentResult = VectoMath.Min(
													BaseCoolingW_Mechanical(env.Temperature, env.Solar), genInputs.ACSystem.CompressorCapacity) /
												genInputs.ACSystem.COP;

			return MechanicalWBaseCurrentResult * env.Weighting;
		}

		private KilogramPerSecond CalculateFuelLPerHBase(ISSMInputs genInputs, IEnvironmentalConditionsMapEntry env)
		{
			// =(MIN(ABS(J93/1000),C71)/C37)*(1/(C39*C38))

			// Dim J93 = BaseHeatingW_FuelFiredHeating
			// Dim C71 = genInputs.AH_FuelFiredHeaterkW
			// Dim C37 = genInputs.BC_AuxHeaterEfficiency
			// Dim C39 = ssmTOOL.HVACConstants.FuelDensity
			// Dim C38 = genInputs.BC_GCVDieselOrHeatingOil

			var fuelLPerHBaseCurrentResult =
				VectoMath.Min(
					VectoMath.Abs(BaseHeatingW_FuelFiredHeating(env.Temperature, env.Solar)).Value().SI<Watt>(), genInputs.AuxHeater.FuelFiredHeaterPower) /
				genInputs.BoundaryConditions.AuxHeaterEfficiency /
				(genInputs.BoundaryConditions.GCVDieselOrHeatingOil /* * ssmTOOL.HVACConstants.FuelDensity */);

			return fuelLPerHBaseCurrentResult * env.Weighting;
		}

		private Watt CalculateElectricalWAdjusted(
			ISSMInputs genInputs, ISSMTechList tecList, IEnvironmentalConditionsMapEntry env)
		{
			// =(MIN((H94*(1-H100)),C54*1000)/C59)+(I93*(1-I99))+(I94*(1-I100))+(I95*(1-I101))

			var H94 = BaseCoolingW_ElectricalCoolingHeating(env.Temperature, env.Solar);
			var H100 = TechListAdjustedCoolingW_ElectricalCoolingHeating;
			var C54 = genInputs.ACSystem.CompressorCapacity;
			var C59 = genInputs.ACSystem.COP;

			var I93 = BaseHeatingW_ElectricalVentilation(env.Temperature, env.Solar);
			var I94 = BaseCoolingW_ElectricalVentilation(env.Temperature, env.Solar);
			var I95 = BaseVentilationW_ElectricalVentilation(env.Temperature, env.Solar);
			var I99 = TechListAdjustedHeatingW_ElectricalVentilation;
			var I100 = TechListAdjustedCoolingW_ElectricalVentilation;
			var I101 = TechListAdjustedVentilationW_ElectricalVentilation;

			var ElectricalWAdjusted = (VectoMath.Min((H94 * (1 - H100)), C54) / C59) + (I93 * (1 - I99)) + (I94 * (1 - I100)) +
									(I95 * (1 - I101));

			return ElectricalWAdjusted * env.Weighting;
		}

		private Watt CalculateMechanicalWBaseAdjusted(ISSMInputs genInputs, ISSMTechList tecList, IEnvironmentalConditionsMapEntry env)
		{
			// =(MIN((F94*(1-F100)),C54*1000)/C59)

			var F94 = BaseCoolingW_Mechanical(env.Temperature, env.Solar);
			var F100 = TechListAdjustedCoolingW_Mechanical;
			var C54 = genInputs.ACSystem.CompressorCapacity;
			var C59 = genInputs.ACSystem.COP;

			var MechanicalWBaseAdjusted = (VectoMath.Min((F94 * (1 - F100)), C54) / C59);

			return MechanicalWBaseAdjusted * env.Weighting;
		}

		private KilogramPerSecond CalculateFuelLPerHBaseAdjusted(
			ISSMInputs genInputs, ISSMTechList tecList, IEnvironmentalConditionsMapEntry env)
		{
			// =MIN(ABS(IF(AND(M89<0,M90<0),VLOOKUP(MAX(M89:M90),M89:P90,4),0)/1000),C71)/C37*(1/(C39*C38))

			// Dim M89 = Run1.TotalW
			// Dim M90 = genInputs.BC_GCVDieselOrHeatingOil
			// Dim C71 = genInputs.AH_FuelFiredHeaterkW
			// Dim C37 = genInputs.BC_AuxHeaterEfficiency
			// Dim C38 = genInputs.BC_GCVDieselOrHeatingOil
			// Dim C39 = ssmTOOL.HVACConstants.FuelDensity


			var run1TotalW = Run1.TotalW(env.Temperature, env.Solar);
			var run2TotalW = Run2.TotalW(env.Temperature, env.Solar);

			var result = 0.SI<Watt>();

			if (run1TotalW < 0 && run2TotalW < 0) {
				result = VectoMath
					.Abs(
						run1TotalW > run2TotalW
							? Run1.TechListAmendedFuelW(env.Temperature, env.Solar)
							: Run2.TechListAmendedFuelW(env.Temperature, env.Solar)).Value().SI<Watt>();
			}

			var fuelLPerHBaseAdjusted = VectoMath.Min(result, genInputs.AuxHeater.FuelFiredHeaterPower) /
										genInputs.BoundaryConditions.AuxHeaterEfficiency /
										(genInputs.BoundaryConditions.GCVDieselOrHeatingOil /* * ssmTOOL.HVACConstants.FuelDensity*/);

			return fuelLPerHBaseAdjusted * env.Weighting;
		}
	}
}
