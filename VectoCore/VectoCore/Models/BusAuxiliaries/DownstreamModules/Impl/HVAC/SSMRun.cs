using System;
using System.Text;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules.HVAC;
using TUGraz.VectoCore.Models.Declaration;

namespace TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.HVAC
{

	// Used By SSMHVAC Class
	public class SSMRun : ISSMRun
	{
		private ISSMTOOL ssmTOOL;
		private int runNumber;


		public SSMRun(ISSMTOOL ssm, int runNbr)
		{
			if (runNbr != 1 && runNbr != 2)
				throw new ArgumentException("Run number must be either 1 or 2");

			runNumber = runNbr;
			ssmTOOL = ssm;
		}


		
		public Kelvin TCalc(Kelvin enviromentalTemperature)
		{
			

				// C24 = BC_HeatingBoundaryTemperature
				// C25 = BC_CoolingBoundary Temperature
				// C6  = BP_BusFloorType
				// C43 = EC_Enviromental Temperature
				// C39 = BC_FontAndRearWindowArea

				var gen = ssmTOOL.SSMInputs;
				
				if (runNumber == 1) {
					return gen.BoundaryConditions.HeatingBoundaryTemperature;
				}

				return gen.BusParameters.BusFloorType == FloorType.LowFloor
					? (enviromentalTemperature - gen.BoundaryConditions.CoolingBoundaryTemperature) <
					gen.BoundaryConditions.MaxTemperatureDeltaForLowFloorBusses
						? gen.BoundaryConditions.CoolingBoundaryTemperature
						: enviromentalTemperature - 3.SI<Kelvin>()
					: gen.BoundaryConditions.CoolingBoundaryTemperature;
			
		}
		
		
		public Watt TotalW(Kelvin enviromentalTemperature, WattPerSquareMeter solarFactor)
		{

			// =SUM(J79:L79) or =SUM(J80:L80)             
			// Tanslated to 
			// =Sum ( Me.Qwall	,Me.WattsPerPass,Me.Solar )


			var gen = ssmTOOL.SSMInputs;

			var qWall = (enviromentalTemperature - TCalc(enviromentalTemperature)) * gen.BusParameters.BusSurfaceArea *
						gen.BoundaryConditions.UValue;
			var wattsPerPass = Math.Min(gen.BusParameters.NumberOfPassengers, gen.BusParameters.CalculatedPassengerNumber) *
					gen.BoundaryConditions.HeatPerPassengerIntoCabin(enviromentalTemperature);
			var solar = solarFactor * gen.BusParameters.BusWindowSurface *
						gen.BoundaryConditions.GFactor * gen.BoundaryConditions.SolarClouding(enviromentalTemperature) * 0.25;

			return qWall + wattsPerPass + solar;
			
		}
		

		public Watt FuelW(Kelvin enviromentalTemperature, WattPerSquareMeter solarFactor)
		{
			
				// =IF(AND(N79<0,N79<(C60*-1)),N79-(C60*-1),0)*1000

				var gen = ssmTOOL.SSMInputs.AuxHeater;

				// Dim N79  as Double =  TotalKW
				// Dim C60  As Double = gen.AH_EngineWasteHeatkW

				var totalW = TotalW(enviromentalTemperature, solarFactor);
				return (totalW < 0 && totalW < (ssmTOOL.EngineWasteHeatkW * -1))
					? totalW - (ssmTOOL.EngineWasteHeatkW * -1)
					: 0.SI<Watt>();

		}
		public Watt TechListAmendedFuelW(Kelvin enviromentalTemperature, WattPerSquareMeter solarFactor)
		{
			
				// =IF(IF(AND((N79*(1-$J$89))<0,(N79*(1-$J$89))<(C60*-1)),(N79*(1-$J$89))-(C60*-1),0)*1000<0,IF(AND((N79*(1-$J$89))<0,(N79*(1-$J$89))<(C60*-1)),(N79*(1-$J$89))-(C60*-1),0)*1000,0)

				var gen = ssmTOOL.SSMInputs.AuxHeater;
				var TLFFH = ssmTOOL.Calculate.TechListAdjustedHeatingW_FuelFiredHeating;
				// Dim C60 As Double = gen.AH_EngineWasteHeatkW
				// Dim N79 As Double = Me.TotalKW
				//Return IF(IF(((TotalKW * (1 - TLFFH)) < 0 AndAlso(TotalKW * (1 - TLFFH)) < (gen.AH_EngineWasteHeatkW * -1)), _
				//	(TotalKW * (1 - TLFFH)) - (gen.AH_EngineWasteHeatkW * -1), 0)*1000 < 0, _
				//IF(((TotalKW * (1 - TLFFH)) < 0 AndAlso(TotalKW * (1 - TLFFH)) < (gen.AH_EngineWasteHeatkW * -1)),(TotalKW * (1 - TLFFH)) - (gen.AH_EngineWasteHeatkW * -1),0)*1000,0)

				var totalW = TotalW(enviromentalTemperature, solarFactor);
				return (totalW * (1 - TLFFH) < 0 && totalW * (1 - TLFFH) < ssmTOOL.EngineWasteHeatkW * -1?
							totalW * (1 - TLFFH) - ssmTOOL.EngineWasteHeatkW * -1: 0.SI<Watt>()) < 0
					? (totalW * (1 - TLFFH) < 0 && totalW * (1 - TLFFH) < ssmTOOL.EngineWasteHeatkW * -1
							? totalW * (1 - TLFFH) - ssmTOOL.EngineWasteHeatkW * -1
							: 0.SI<Watt>())
					: 0.SI<Watt>();
			
		}

		// Provides Diagnostic Information
		// To be utilised by the User.
		//public override string ToString()
		//{
		//	var sb = new StringBuilder();

		//	var  vbTab = "\t";
		//	var temp = ssmTOOL.SSMInputs.EnvironmentalConditions.EnviromentalTemperature;
		//	sb.AppendLine(string.Format("Run : {0}", runNumber));
		//	sb.AppendLine(string.Format("************************************"));
		//	sb.AppendLine(string.Format("HVAC OP         " + vbTab + ": {0}", HVACOperation(temp)));
		//	sb.AppendLine(string.Format("TCALC           " + vbTab + ": {0}", TCalc(temp)));
		//	sb.AppendLine(string.Format("Tempurature D   " + vbTab + ": {0}", (temp - TCalc(temp))));
		//	sb.AppendLine(string.Format("QWall           " + vbTab + ": {0}", QWall(temp)));
		//	sb.AppendLine(string.Format("WattsPerPass    " + vbTab + ": {0}", WattsPerPass));
		//	sb.AppendLine(string.Format("Solar           " + vbTab + ": {0}", Solar));
		//	sb.AppendLine(string.Format("TotalW          " + vbTab + ": {0}", TotalW(temp)));
		//	sb.AppendLine(string.Format("TotalKW         " + vbTab + ": {0}", TotalW(temp).Value() * 1000));
		//	sb.AppendLine(string.Format("Fuel W          " + vbTab + ": {0}", FuelW(temp)));
		//	sb.AppendLine(string.Format("Fuel Tech Adj   " + vbTab + ": {0}", TechListAmendedFuelW(temp)));


		//	return sb.ToString();
		//}
	}
}
