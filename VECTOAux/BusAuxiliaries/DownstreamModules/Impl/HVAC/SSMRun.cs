using System;
using System.Text;
using TUGraz.VectoCore.BusAuxiliaries.Interfaces.DownstreamModules.HVAC;

namespace TUGraz.VectoCore.BusAuxiliaries.DownstreamModules.Impl.HVAC
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

				var gen = ssmTOOL.GenInputs;

				return gen.EC_EnviromentalTemperature > gen.BC_CoolingBoundaryTemperature ? 3 : gen.EC_EnviromentalTemperature < gen.BC_HeatingBoundaryTemperature ? 1 : 2;
			}
		}
		public double TCalc
		{
			get {

				// C24 = BC_HeatingBoundaryTemperature
				// C25 = BC_CoolingBoundary Temperature
				// C6  = BP_BusFloorType
				// C43 = EC_Enviromental Temperature
				// C39 = BC_FontAndRearWindowArea

				var gen = ssmTOOL.GenInputs;
				double returnVal;

				if (runNumber == 1)
					returnVal = gen.BC_HeatingBoundaryTemperature;
				else
					returnVal = gen.BP_BusFloorType == "low floor" ? (gen.EC_EnviromentalTemperature - gen.BC_CoolingBoundaryTemperature) < gen.BC_FrontRearWindowArea ? gen.BC_CoolingBoundaryTemperature : gen.EC_EnviromentalTemperature - 3 : gen.BC_CoolingBoundaryTemperature;


				return returnVal;
			}
		}
		public double TemperatureDelta
		{
			get {
				// =C43-F79/F80
				// C43 = EC_Enviromental Temperature
				// F79/80 = Me.TCalc

				var gen = ssmTOOL.GenInputs;
				return gen.EC_EnviromentalTemperature - TCalc;
			}
		}
		public double QWall
		{
			get {
				// =I79*D8*C23  or '=I80*D8*C23
				// Translated to
				// =I79*C8*C23  or '=I80*C8*C23

				// C23 = BC_UValues
				// C8  = BP_BusSurfaceAreaM2
				// I78/I80 = Me.TemperatureDelta

				var gen = ssmTOOL.GenInputs;

				return TemperatureDelta * gen.BP_BusSurfaceAreaM2 * gen.BC_UValues;
			}
		}
		public double WattsPerPass
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


				var gen = ssmTOOL.GenInputs;

				return Math.Min(gen.BP_NumberOfPassengers, gen.BC_CalculatedPassengerNumber) * gen.BC_HeatPerPassengerIntoCabinW;
			}
		}
		public double Solar
		{
			get {
				// =C44*D9*C15*C16*0.25
				// Translated to 
				// =C44*C9*C15*C16*0.25

				// C44 = EC_Solar
				// C9  = BP_BusWindowSurfaceArea
				// C15 = BC_GFactor
				// C16 = BC_SolarClouding

				var gen = ssmTOOL.GenInputs;


				return gen.EC_Solar * gen.BP_BusWindowSurface * gen.BC_GFactor * gen.BC_SolarClouding * 0.25;
			}
		}
		public double TotalW
		{
			get {

				// =SUM(J79:L79) or =SUM(J80:L80)             
				// Tanslated to 
				// =Sum ( Me.Qwall	,Me.WattsPerPass,Me.Solar )

				return QWall + WattsPerPass + Solar;
			}
		}
		public double TotalKW
		{
			get {
				// =M79 or =M80  / (1000)

				return TotalW / 1000;
			}
		}
		public double FuelW
		{
			get {
				// =IF(AND(N79<0,N79<(C60*-1)),N79-(C60*-1),0)*1000

				var gen = ssmTOOL.GenInputs;

				// Dim N79  as Double =  TotalKW
				// Dim C60  As Double = gen.AH_EngineWasteHeatkW

				return (TotalKW < 0 && TotalKW < (gen.AH_EngineWasteHeatkW * -1)) ? TotalKW - (gen.AH_EngineWasteHeatkW * -1) : 0
								* 1000;
			}
		}
		public double TechListAmendedFuelW
		{
			get {
				// =IF(IF(AND((N79*(1-$J$89))<0,(N79*(1-$J$89))<(C60*-1)),(N79*(1-$J$89))-(C60*-1),0)*1000<0,IF(AND((N79*(1-$J$89))<0,(N79*(1-$J$89))<(C60*-1)),(N79*(1-$J$89))-(C60*-1),0)*1000,0)

				var gen = ssmTOOL.GenInputs;
				var TLFFH = ssmTOOL.Calculate.TechListAdjustedHeatingW_FuelFiredHeating;
				// Dim C60 As Double = gen.AH_EngineWasteHeatkW
				// Dim N79 As Double = Me.TotalKW

				return ((TotalKW * (1 - TLFFH)) < 0 && (TotalKW * (1 - TLFFH)) < (gen.AH_EngineWasteHeatkW * -1)) ? (TotalKW * (1 - TLFFH)) - (gen.AH_EngineWasteHeatkW * -1) : 0 * 1000 < 0 ? ((TotalKW * (1 - TLFFH)) < 0 && (TotalKW * (1 - TLFFH)) < (gen.AH_EngineWasteHeatkW * -1)) ? (TotalKW * (1 - TLFFH)) - (gen.AH_EngineWasteHeatkW * -1) : 0 * 1000 : 0;
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
			sb.AppendLine(string.Format("TotalKW         " + vbTab + ": {0}", TotalKW));
			sb.AppendLine(string.Format("Fuel W          " + vbTab + ": {0}", FuelW));
			sb.AppendLine(string.Format("Fuel Tech Adj   " + vbTab + ": {0}", TechListAmendedFuelW));


			return sb.ToString();
		}
	}
}
