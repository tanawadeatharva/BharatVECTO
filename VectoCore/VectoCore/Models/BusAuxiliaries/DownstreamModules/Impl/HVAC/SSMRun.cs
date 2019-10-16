using System;
using System.Text;
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


		public double HVACOperation
		{
			get {
				// =IF(C43>C25,3,IF(C43<C24,1,2))
				// C43 = EC_Enviromental Temperature
				// C25 = BC_CoolingBoundary Temperature
				// C24 = BC_HeatingBoundaryTemperature

				var gen = ssmTOOL.SSMInputs;

				return gen.EnvironmentalConditions.EnviromentalTemperature > gen.BoundaryConditions.CoolingBoundaryTemperature
					? 3
					: gen.EnvironmentalConditions.EnviromentalTemperature < gen.BoundaryConditions.HeatingBoundaryTemperature
						? 1
						: 2;
			}
		}
		public Kelvin TCalc
		{
			get {

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
					? (gen.EnvironmentalConditions.EnviromentalTemperature - gen.BoundaryConditions.CoolingBoundaryTemperature) <
					gen.BoundaryConditions.MaxTemperatureDeltaForLowFloorBusses
						? gen.BoundaryConditions.CoolingBoundaryTemperature
						: gen.EnvironmentalConditions.EnviromentalTemperature - 3.SI<Kelvin>()
					: gen.BoundaryConditions.CoolingBoundaryTemperature;
			}
		}
		public Kelvin TemperatureDelta
		{
			get {
				// =C43-F79/F80
				// C43 = EC_Enviromental Temperature
				// F79/80 = Me.TCalc

				var gen = ssmTOOL.SSMInputs.EnvironmentalConditions;
				return gen.EnviromentalTemperature - TCalc;
			}
		}
		public Watt QWall
		{
			get {
				// =I79*D8*C23  or '=I80*D8*C23
				// Translated to
				// =I79*C8*C23  or '=I80*C8*C23

				// C23 = BC_UValues
				// C8  = BP_BusSurfaceArea
				// I78/I80 = Me.TemperatureDelta

				var gen = ssmTOOL.SSMInputs;

				return TemperatureDelta * gen.BusParameters.BusSurfaceArea * gen.BoundaryConditions.UValue;
			}
		}
		public Watt WattsPerPass
		{
			get {

				// =IF(D5="",C22,IF(E5="",IF(C22<D5,C22,D5),E5))*C17
				// Translated to
				// =IF(IF(C22<C5,C22,C5))*C17
				// Simplified to
				// Max( C22,C5 )

				// C5   = BP_NumberOfPassengers
				// C22  = BC_Calculated Passenger Number
				// C17  = BC_Heat Per Passenger into cabin


				var gen = ssmTOOL.SSMInputs;

				return Math.Min(gen.BusParameters.NumberOfPassengers, gen.BusParameters.CalculatedPassengerNumber) *
						gen.BoundaryConditions.HeatPerPassengerIntoCabin;
			}
		}
		public Watt Solar
		{
			get {
				// =C44*D9*C15*C16*0.25
				// Translated to 
				// =C44*C9*C15*C16*0.25

				// C44 = EC_Solar
				// C9  = BP_BusWindowSurfaceArea
				// C15 = BC_GFactor
				// C16 = BC_SolarClouding

				var gen = ssmTOOL.SSMInputs;


				return gen.EnvironmentalConditions.Solar * gen.BusParameters.BusWindowSurface *
						gen.BoundaryConditions.GFactor * gen.BoundaryConditions.SolarClouding * 0.25;
			}
		}
		public Watt TotalW
		{
			get {

				// =SUM(J79:L79) or =SUM(J80:L80)             
				// Tanslated to 
				// =Sum ( Me.Qwall	,Me.WattsPerPass,Me.Solar )

				return QWall + WattsPerPass + Solar;
			}
		}
		//public Watt TotalKW
		//{
		//	get {
		//		// =M79 or =M80  / (1000)

		//		return TotalW / 1000;
		//	}
		//}

		public Watt FuelW
		{
			get {
				// =IF(AND(N79<0,N79<(C60*-1)),N79-(C60*-1),0)*1000

				var gen = ssmTOOL.SSMInputs.AuxHeater;

				// Dim N79  as Double =  TotalKW
				// Dim C60  As Double = gen.AH_EngineWasteHeatkW

				return (TotalW < 0 && TotalW < (gen.EngineWasteHeatkW * -1))
					? TotalW - (gen.EngineWasteHeatkW * -1)
					: 0.SI<Watt>();
			}
		}
		public Watt TechListAmendedFuelW
		{
			get {
				// =IF(IF(AND((N79*(1-$J$89))<0,(N79*(1-$J$89))<(C60*-1)),(N79*(1-$J$89))-(C60*-1),0)*1000<0,IF(AND((N79*(1-$J$89))<0,(N79*(1-$J$89))<(C60*-1)),(N79*(1-$J$89))-(C60*-1),0)*1000,0)

				var gen = ssmTOOL.SSMInputs.AuxHeater;
				var TLFFH = ssmTOOL.Calculate.TechListAdjustedHeatingW_FuelFiredHeating;
				// Dim C60 As Double = gen.AH_EngineWasteHeatkW
				// Dim N79 As Double = Me.TotalKW
				//Return IF(IF(((TotalKW * (1 - TLFFH)) < 0 AndAlso(TotalKW * (1 - TLFFH)) < (gen.AH_EngineWasteHeatkW * -1)), _
				//	(TotalKW * (1 - TLFFH)) - (gen.AH_EngineWasteHeatkW * -1), 0)*1000 < 0, _
				//IF(((TotalKW * (1 - TLFFH)) < 0 AndAlso(TotalKW * (1 - TLFFH)) < (gen.AH_EngineWasteHeatkW * -1)),(TotalKW * (1 - TLFFH)) - (gen.AH_EngineWasteHeatkW * -1),0)*1000,0)

				return (TotalW * (1 - TLFFH) < 0 && TotalW * (1 - TLFFH) < gen.EngineWasteHeatkW * -1?
							TotalW * (1 - TLFFH) - gen.EngineWasteHeatkW * -1: 0.SI<Watt>()) < 0
					? (TotalW * (1 - TLFFH) < 0 && TotalW * (1 - TLFFH) < gen.EngineWasteHeatkW * -1
							? TotalW * (1 - TLFFH) - gen.EngineWasteHeatkW * -1
							: 0.SI<Watt>())
					: 0.SI<Watt>();
			}
		}

		// Provides Diagnostic Information
		// To be utilised by the User.
		public override string ToString()
		{
			var sb = new StringBuilder();

			var  vbTab = "\t";
			sb.AppendLine(string.Format("Run : {0}", runNumber));
			sb.AppendLine(string.Format("************************************"));
			sb.AppendLine(string.Format("HVAC OP         " + vbTab + ": {0}", HVACOperation));
			sb.AppendLine(string.Format("TCALC           " + vbTab + ": {0}", TCalc));
			sb.AppendLine(string.Format("Tempurature D   " + vbTab + ": {0}", TemperatureDelta));
			sb.AppendLine(string.Format("QWall           " + vbTab + ": {0}", QWall));
			sb.AppendLine(string.Format("WattsPerPass    " + vbTab + ": {0}", WattsPerPass));
			sb.AppendLine(string.Format("Solar           " + vbTab + ": {0}", Solar));
			sb.AppendLine(string.Format("TotalW          " + vbTab + ": {0}", TotalW));
			sb.AppendLine(string.Format("TotalKW         " + vbTab + ": {0}", TotalW.Value() * 1000));
			sb.AppendLine(string.Format("Fuel W          " + vbTab + ": {0}", FuelW));
			sb.AppendLine(string.Format("Fuel Tech Adj   " + vbTab + ": {0}", TechListAmendedFuelW));


			return sb.ToString();
		}
	}
}
