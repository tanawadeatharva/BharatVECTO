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

		public virtual CubicMeter Displacement
		{
			get { return GetDouble(XMLNames.Engine_Displacement).SI(Unit.SI.Cubic.Centi.Meter).Cast<CubicMeter>(); }
		}

		public virtual PerSecond IdleSpeed
		{
			get { return GetDouble(XMLNames.Engine_IdlingSpeed).RPMtoRad(); }
		}

		public virtual FuelType FuelType
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

		public virtual TableData FuelConsumptionMap
		{
			get {
				return ReadTableData(XMLNames.Engine_FuelConsumptionMap, XMLNames.Engine_FuelConsumptionMap_Entry, AttributeMappings.FuelConsumptionMapMapping);
			}
		}

		public virtual TableData FullLoadCurve
		{
			get {
				return ReadTableData(XMLNames.Engine_FullLoadAndDragCurve, XMLNames.Engine_FullLoadCurve_Entry, AttributeMappings.EngineFullLoadCurveMapping);
			}
		}

		public virtual Watt RatedPowerDeclared
		{
			get { return GetDouble(XMLNames.Engine_RatedPower).SI<Watt>(); }
		}

		public virtual PerSecond RatedSpeedDeclared
		{
			get { return GetDouble(XMLNames.Engine_RatedSpeed).RPMtoRad(); }
		}

		public virtual NewtonMeter MaxTorqueDeclared
		{
			get { return GetDouble(XMLNames.Engine_MaxTorque).SI<NewtonMeter>(); }
		}

		public virtual double WHTCMotorway
		{
			get { return GetDouble(XMLNames.Engine_WHTCMotorway); }
		}

		public virtual double WHTCRural
		{
			get { return GetDouble(XMLNames.Engine_WHTCRural); }
		}

		public virtual double WHTCUrban
		{
			get { return GetDouble(XMLNames.Engine_WHTCUrban); }
		}

		public virtual double ColdHotBalancingFactor
		{
			get { return GetDouble(XMLNames.Engine_ColdHotBalancingFactor); }
		}

		public virtual double CorrectionFactorRegPer
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

	// ---------------------------------------------------------------------------------------

	public class XMLDeclarationEngineDataProviderV20 : XMLDeclarationEngineDataProviderV10
	{
		public new const string NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V20;

		public XMLDeclarationEngineDataProviderV20(IXMLDeclarationVehicleData vehicle, XmlNode componentNode, string sourceFile) : base(vehicle, componentNode, sourceFile) { }

		protected override string SchemaNamespace
		{
			get { return NAMESPACE_URI; }
		}
	}
}
