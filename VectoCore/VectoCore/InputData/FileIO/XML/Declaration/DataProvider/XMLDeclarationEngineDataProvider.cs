using System;
using System.Xml;
using TUGraz.IVT.VectoXML;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider
{
	public class XMLDeclarationEngineDataProviderV10 : AbstractCommonComponentType, IXMLEngineDeclarationInputData
	{
		public const string NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V10;

		public XMLDeclarationEngineDataProviderV10(IXMLDeclarationVehicleData vehicle, XmlNode componentNode, string sourceFile) :
			base(componentNode, sourceFile)
		{
			SourceType = DataSourceType.XMLFile;
		}

		#region Implementation of IEngineDeclarationInputData

		public CubicMeter Displacement
		{
			get { return GetDouble(XMLNames.Engine_Displacement).SI(Unit.SI.Cubic.Centi.Meter).Cast<CubicMeter>(); }
		}

		public PerSecond IdleSpeed
		{
			get { return GetDouble(XMLNames.Engine_IdlingSpeed).RPMtoRad(); }
		}

		public FuelType FuelType
		{
			get {
				var value = GetString(XMLNames.Engine_FuelType);
				if ("LPG".Equals(value, StringComparison.InvariantCultureIgnoreCase)) {
					return FuelType.LPGPI;
				}
				if ("NG".Equals(value, StringComparison.InvariantCultureIgnoreCase)) {
					return FuelType.NGPI;
				}

				return value.ParseEnum<FuelType>();
			}
		}

		public TableData FuelConsumptionMap
		{
			get {
				return ReadTableData(XMLNames.Engine_FuelConsumptionMap, XMLNames.Engine_FuelConsumptionMap_Entry, AttributeMappings.FuelConsumptionMapMapping);
			}
		}

		public TableData FullLoadCurve
		{
			get {
				return ReadTableData(XMLNames.Engine_FullLoadAndDragCurve, XMLNames.Engine_FullLoadCurve_Entry, AttributeMappings.EngineFullLoadCurveMapping);
			}
		}

		public Watt RatedPowerDeclared
		{
			get { return GetDouble(XMLNames.Engine_RatedPower).SI<Watt>(); }
		}

		public PerSecond RatedSpeedDeclared
		{
			get { return GetDouble(XMLNames.Engine_RatedSpeed).RPMtoRad(); }
		}

		public NewtonMeter MaxTorqueDeclared
		{
			get { return GetDouble(XMLNames.Engine_MaxTorque).SI<NewtonMeter>(); }
		}

		public double WHTCMotorway
		{
			get { return GetDouble(XMLNames.Engine_WHTCMotorway); }
		}

		public double WHTCRural
		{
			get { return GetDouble(XMLNames.Engine_WHTCRural); }
		}

		public double WHTCUrban
		{
			get { return GetDouble(XMLNames.Engine_WHTCUrban); }
		}

		public double ColdHotBalancingFactor
		{
			get { return GetDouble(XMLNames.Engine_ColdHotBalancingFactor); }
		}

		public double CorrectionFactorRegPer
		{
			get { return GetDouble(XMLNames.Engine_CorrectionFactor_RegPer); }
		}

		#endregion

		#region Overrides of AbstractXMLResource

		protected override string SchemaNamespace
		{
			get { return NAMESPACE_URI; }
		}

		protected override DataSourceType SourceType { get; }

		#endregion
	}
}
