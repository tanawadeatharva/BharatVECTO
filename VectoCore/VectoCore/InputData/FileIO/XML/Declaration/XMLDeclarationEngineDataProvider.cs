using TUGraz.IVT.VectoXML;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Resources;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration
{
	public class XMLDeclarationEngineDataProvider : AbstractDeclarationXMLComponentDataProvider,
		IEngineDeclarationInputData
	{
		public XMLDeclarationEngineDataProvider(XMLDeclarationInputDataProvider xmlInputDataProvider)
			: base(xmlInputDataProvider)
		{
			XBasePath = Helper.Query(VehiclePath,
				XMLNames.Vehicle_Components,
				XMLNames.Component_Engine,
				XMLNames.ComponentDataWrapper);
		}

		public CubicMeter Displacement
		{
			get { return GetDoubleElementValue(XMLNames.Engine_Displacement).SI().Cubic.Centi.Meter.Cast<CubicMeter>(); }
		}

		public PerSecond IdleSpeed
		{
			get { return GetDoubleElementValue(XMLNames.Engine_IdlingSpeed).RPMtoRad(); }
		}

		public FuelType FuelType
		{
			get { return GetElementValue(XMLNames.Engine_FuelType).ParseEnum<FuelType>(); }
		}

		public TableData FuelConsumptionMap
		{
			get {
				return ReadTableData(AttributeMappings.FuelConsumptionMapMapping,
					Helper.Query(XMLNames.Engine_FuelConsumptionMap, XMLNames.Engine_FuelConsumptionMap_Entry));
			}
		}

		public TableData FullLoadCurve
		{
			get {
				return ReadTableData(AttributeMappings.EngineFullLoadCurveMapping,
					Helper.Query(XMLNames.Engine_FullLoadAndDragCurve, XMLNames.Engine_FullLoadCurve_Entry));
			}
		}

		public Watt RatedPowerDeclared
		{
			get { return GetDoubleElementValue(XMLNames.Engine_RatedPower).SI<Watt>(); }
		}

		public PerSecond RatedSpeedDeclared
		{
			get { return GetDoubleElementValue(XMLNames.Engine_RatedSpeed).RPMtoRad(); }
		}

		public NewtonMeter MaxTorqueDeclared
		{
			get { return GetDoubleElementValue(XMLNames.Engine_MaxTorque).SI<NewtonMeter>(); }
		}

		public double WHTCMotorway
		{
			get { return GetDoubleElementValue(XMLNames.Engine_WHTCMotorway); }
		}

		public double WHTCRural
		{
			get { return GetDoubleElementValue(XMLNames.Engine_WHTCRural); }
		}

		public double WHTCUrban
		{
			get { return GetDoubleElementValue(XMLNames.Engine_WHTCUrban); }
		}

		public double ColdHotBalancingFactor
		{
			get { return GetDoubleElementValue(XMLNames.Engine_ColdHotBalancingFactor); }
		}

		public double CorrectionFactorRegPer
		{
			get { return GetDoubleElementValue(XMLNames.Engine_CorrectionFactor_RegPer); }
		}

		public double CorrectionFactorNCV
		{
			get { return GetDoubleElementValue(XMLNames.Engine_CorrecionFactor_NCV); }
		}
	}
}