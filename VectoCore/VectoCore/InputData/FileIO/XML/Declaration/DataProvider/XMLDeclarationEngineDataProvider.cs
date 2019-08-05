using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using TUGraz.IVT.VectoXML;
using TUGraz.VectoCommon.Exceptions;
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
		public static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V10;

		public const string XSD_TYPE = "EngineDataDeclarationType";

		public static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		protected List<IEngineModeDeclarationInputData> _engineModes;

		public XMLDeclarationEngineDataProviderV10(
			IXMLDeclarationVehicleData vehicle, XmlNode componentNode, string sourceFile) :
			base(componentNode, sourceFile)
		{
			SourceType = DataSourceType.XMLFile;
		}

		#region Implementation of IEngineDeclarationInputData

		public virtual CubicMeter Displacement
		{
			get { return GetDouble(XMLNames.Engine_Displacement).SI(Unit.SI.Cubic.Centi.Meter).Cast<CubicMeter>(); }
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

		public virtual IList<IEngineModeDeclarationInputData> EngineModes
		{
			get {
				return _engineModes ??
						(_engineModes = new List<IEngineModeDeclarationInputData>() { new XMLSingleFuelEngineMode(BaseNode) });
			}
		}
		

		public class XMLSingleFuelEngineMode : AbstractXMLType, IEngineModeDeclarationInputData
		{
			protected IList<IEngineFuelDelcarationInputData> FuelsList;

			public XMLSingleFuelEngineMode(XmlNode baseNode) : base(baseNode)
			{
			}

			public virtual PerSecond IdleSpeed
			{
				get { return GetDouble(XMLNames.Engine_IdlingSpeed).RPMtoRad(); }
			}

			public virtual TableData FullLoadCurve
			{
				get {
					return ReadTableData(
						XMLNames.Engine_FullLoadAndDragCurve, XMLNames.Engine_FullLoadCurve_Entry,
						AttributeMappings.EngineFullLoadCurveMapping);
				}
			}

			public virtual IList<IEngineFuelDelcarationInputData> Fuels {
				get {
					return FuelsList ?? (FuelsList = new List<IEngineFuelDelcarationInputData>() { new XMLSingleFuelEngineFuel(BaseNode) });
				}
			
			}

			public virtual IWHRData WasteHeatRecoveryData { get { return null; } }
		}

		public class XMLSingleFuelEngineFuel : AbstractXMLType, IEngineFuelDelcarationInputData
		{
			public XMLSingleFuelEngineFuel(XmlNode baseNode) : base(baseNode) { }

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
					return ReadTableData(
						XMLNames.Engine_FuelConsumptionMap, XMLNames.Engine_FuelConsumptionMap_Entry,
						AttributeMappings.FuelConsumptionMapMapping);
				}
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
		}

		
		#endregion

		#region Overrides of AbstractXMLResource

		protected override XNamespace SchemaNamespace
		{
			get { return NAMESPACE_URI; }
		}

		protected override DataSourceType SourceType { get; }

		#endregion
	}


	// ---------------------------------------------------------------------------------------

	public class XMLDeclarationEngineDataProviderV20 : XMLDeclarationEngineDataProviderV10
	{
		//public new static readonly XNamespace
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V20;

		//public new const string XSD_TYPE = "EngineComponentDeclarationType";

		public new static readonly string QUALIFIED_XSD_TYPE =
			XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		public XMLDeclarationEngineDataProviderV20(
			IXMLDeclarationVehicleData vehicle, XmlNode componentNode, string sourceFile) : base(
			vehicle, componentNode, sourceFile) { }

		protected override XNamespace SchemaNamespace
		{
			get { return NAMESPACE_URI; }
		}
	}

	// ---------------------------------------------------------------------------------------

	public class XMLDeclarationEngineDataProviderV21 : XMLDeclarationEngineDataProviderV20
	{
		/*
		 * harmonize fuel-type paramenter in Regulation 2019/318 (amendment of 2017/2400)
		 */

		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V21;

		//public new const string XSD_TYPE = "EngineComponentDeclarationType";

		public new static readonly string QUALIFIED_XSD_TYPE =
			XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		public XMLDeclarationEngineDataProviderV21(
			IXMLDeclarationVehicleData vehicle, XmlNode componentNode, string sourceFile) : base(
			vehicle, componentNode, sourceFile) { }

		protected override XNamespace SchemaNamespace
		{
			get { return NAMESPACE_URI; }
		}
	}

	// ---------------------------------------------------------------------------------------

	public class XMLDeclarationEngineDataProviderV23 : XMLDeclarationEngineDataProviderV20
	{
		/*
		 * Support for dual-fuel engines (in different operating modes - either single fuel or dual fuel)
		 */

		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V23;

		public new static readonly string QUALIFIED_XSD_TYPE =
			XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		public XMLDeclarationEngineDataProviderV23(
			IXMLDeclarationVehicleData vehicle, XmlNode componentNode, string sourceFile) : base(
			vehicle, componentNode, sourceFile) { }

		protected override XNamespace SchemaNamespace
		{
			get { return NAMESPACE_URI; }
		}

		#region Overrides of XMLDeclarationEngineDataProviderV10

		public override IList<IEngineModeDeclarationInputData> EngineModes
		{
			get {
				return _engineModes ?? (_engineModes = GetNodes(XMLNames.Engine_FuelModes)
							.Cast<XmlNode>().Select(x => new XMLDualFuelEngineMode(x)).Cast<IEngineModeDeclarationInputData>().ToList());
			}
		}

		#endregion

		public class XMLDualFuelEngineMode : XMLSingleFuelEngineMode
		{
			protected IWHRData WHRData;

			public XMLDualFuelEngineMode(XmlNode baseNode) : base(baseNode) { }

			#region Overrides of XMLSingleFuelEngineMode

			public override IList<IEngineFuelDelcarationInputData> Fuels
			{
				get {
					return FuelsList ?? (FuelsList = GetNodes(XMLNames.Engine_FuelModes_Fuel).Cast<XmlNode>().Select(x => new XMLDualFuelEngineFuel(x))
																.Cast<IEngineFuelDelcarationInputData>().ToList());
				}
			}

			public override IWHRData WasteHeatRecoveryData
			{
				get { return WHRData ?? (WHRData = ReadWHRData()); }
			}

			#endregion

			protected virtual IWHRData ReadWHRData()
			{
				var correctionFactorNodes = GetNodes(new[] { XMLNames.Engine_FuelModes_Fuel, XMLNames.Engine_WHRCorrectionFactors });
				if (correctionFactorNodes.Count == 0) {
					return new XMLWHRData();
				}
				if (correctionFactorNodes.Count > 1) {
					throw new VectoException("WHRData can only be defined for one fuel!");
				}

				if (GetNodes(new[] { XMLNames.Engine_FuelModes_Fuel, XMLNames.Engine_FuelConsumptionMap, XMLNames.Engine_FuelConsumptionMap_Entry })
					.Cast<XmlNode>().All(x => x.Attributes?[XMLNames.Engine_FuelConsumptionMap_WHRElPower_Attr] == null)) {
					return new XMLWHRData();
				}

				var fuelNodes = GetNodes(XMLNames.Engine_FuelModes_Fuel);
				XmlNode whrFuelNode = null;
				if (fuelNodes.Count > 1) {
					for (var i = 0; i < fuelNodes.Count; i++) {
						var fuel = fuelNodes[i];
						if (GetNodes("Entry", fuel).Cast<XmlNode>().Any(x => x.Attributes?[XMLNames.Engine_FuelConsumptionMap_WHRElPower_Attr] != null)) {
							if (whrFuelNode != null) {
								throw new VectoException("WHRData can only be defined for one fuel!");
							}

							whrFuelNode = fuel;
						}
					}
				} else {
					whrFuelNode = fuelNodes[0];
				}

				if (correctionFactorNodes[0].ParentNode != whrFuelNode) {
					throw new VectoException("Correction Factors and WHR-Map have to be defined for the same fuel!");
				}

				return new XMLWHRData(whrFuelNode);
			}

		}

		public class XMLDualFuelEngineFuel : XMLSingleFuelEngineFuel
		{
			public XMLDualFuelEngineFuel(XmlNode baseNode) : base(baseNode) { }

			#region Overrides of XMLSingleFuelEngineFuel

			public override FuelType FuelType
			{
				get { return GetAttribute(BaseNode, "type").ParseEnum<FuelType>(); }
			}

			#endregion
		}

		public class XMLWHRData : AbstractXMLType, IWHRData
		{
			protected TableData WHRPower;

			public XMLWHRData(XmlNode whrFuelNode) : base(whrFuelNode)
			{
			}

			public XMLWHRData() :base(null)
			{
			}

			#region Implementation of IWHRData

			public double UrbanCorrectionFactor { get { return GetDouble(new[] { XMLNames.Engine_WHRCorrectionFactors, XMLNames.Engine_WHRCorrectionFactors_Urban}, 1); } }
			public double RuralCorrectionFactor { get { return GetDouble(new[] { XMLNames.Engine_WHRCorrectionFactors, XMLNames.Engine_WHRCorrectionFactors_Rural}, 1); } }
			public double MotorwayCorrectionFactor { get { return GetDouble(new[] { XMLNames.Engine_WHRCorrectionFactors, XMLNames.Engine_WHRCorrectionFactors_Motorway}, 1); } }
			public double BFColdHot { get { return GetDouble(new[] { XMLNames.Engine_WHRCorrectionFactors, XMLNames.Engine_WHRCorrectionFactors_BFColdHot}, 1); } }
			public double CFRegPer { get { return GetDouble(new[] { XMLNames.Engine_WHRCorrectionFactors, XMLNames.Engine_WHRCorrectionFactors_CFRegPer}, 1); } }

			public TableData GeneratedElectricPower
			{
				get { return WHRPower ?? (WHRPower = ReadTableData(XMLNames.Engine_FuelConsumptionMap, XMLNames.Engine_FuelConsumptionMap_Entry, AttributeMappings.WHRPowerMapMapping)); }
			}

			#endregion
		}
	}

	
}
