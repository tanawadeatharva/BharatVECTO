using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider.v210
{

	public abstract class AbstractXMLDeclarationPrimaryBusVehicleDataProviderV210 : AbstractXMLVehicleDataProviderV210
	{

		public AbstractXMLDeclarationPrimaryBusVehicleDataProviderV210(
			IXMLDeclarationJobInputData jobData, XmlNode xmlNode, string sourceFile) : base(jobData, xmlNode, sourceFile)
		{
			SourceType = DataSourceType.XMLEmbedded;

		}

		#region Overrides of XMLDeclarationVehicleDataProviderV10

		public override bool SleeperCab => false;

		public override IAdvancedDriverAssistantSystemDeclarationInputData ADAS => ADASReader.ADASInputData;

		public override XmlElement PTONode => null;

		public override IPTOTransmissionInputData PTOTransmissionInputData => null;

		public override LegislativeClass? LegislativeClass => VectoCommon.Models.LegislativeClass.M3;

		public override VehicleCategory VehicleCategory => VehicleCategory.HeavyBusPrimaryVehicle;

		public override bool Articulated => GetBool(XMLNames.Vehicle_Articulated);

		public override Kilogram CurbMassChassis => null;

		public override Kilogram GrossVehicleMassRating => GetDouble(XMLNames.Vehicle_TPMLM).SI<Kilogram>();

		public override Meter EntranceHeight => null;

		public override IList<ITorqueLimitInputData> TorqueLimits =>
			ElementExists(XMLNames.Vehicle_TorqueLimits) ? base.TorqueLimits : null;

		#region Overrides of XMLDeclarationVehicleDataProviderV20

		public override bool ZeroEmissionVehicle => GetBool(XMLNames.Vehicle_ZeroEmissionVehicle);

		#endregion

		#endregion

		#region Overrides of AbstractXMLResource

		protected override XNamespace SchemaNamespace => NAMESPACE_URI;

		protected override DataSourceType SourceType { get; }

		#endregion
	}

	// ---------------------------------------------------------------------------------------

	public class XMLDeclarationConventionalPrimaryBusVehicleDataProviderV210 : AbstractXMLDeclarationPrimaryBusVehicleDataProviderV210
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V210_JOBS;

		public new const string XSD_TYPE = "Vehicle_Conventional_PrimaryBusDeclarationType";

		public new static readonly string QUALIFIED_XSD_TYPE =
			XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		public XMLDeclarationConventionalPrimaryBusVehicleDataProviderV210(IXMLDeclarationJobInputData jobData, XmlNode xmlNode, string sourceFile) : base(jobData, xmlNode, sourceFile) { }

		#region Overrides of AbstractXMLResource

		protected override XNamespace SchemaNamespace => NAMESPACE_URI;

		protected override DataSourceType SourceType { get; }

		#endregion
	}

	// ---------------------------------------------------------------------------------------

	public class XMLDeclarationHEVPxPrimaryBusDataProviderV210 : AbstractXMLDeclarationPrimaryBusVehicleDataProviderV210
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V210_JOBS;
		public new const string XSD_TYPE = "Vehicle_HEV-Px_PrimaryBusDeclarationType";
		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		#region Overrides of XMLDeclarationHEVPxMediumLorryDataProviderV210

		public override string PowertrainPositionPrefix => "P";

		#endregion

		public XMLDeclarationHEVPxPrimaryBusDataProviderV210(IXMLDeclarationJobInputData jobData, XmlNode xmlNode, string sourceFile)
			: base(jobData, xmlNode, sourceFile) { }
	}

	// ---------------------------------------------------------------------------------------

	public class XMLDeclarationHEVSxPrimaryBusDataProviderV210 : AbstractXMLDeclarationPrimaryBusVehicleDataProviderV210
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V210_JOBS;
		public new const string XSD_TYPE = "Vehicle_HEV-Sx_PrimaryBusDeclarationType";
		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		#region Overrides of XMLDeclarationHEVPxMediumLorryDataProviderV210

		public override string PowertrainPositionPrefix => "E";

		#endregion

		public XMLDeclarationHEVSxPrimaryBusDataProviderV210(IXMLDeclarationJobInputData jobData, XmlNode xmlNode, string sourceFile)
			: base(jobData, xmlNode, sourceFile) { }

	}

	// ---------------------------------------------------------------------------------------

	public class XMLDeclarationPEVPrimaryBusDataProviderV210 : AbstractXMLDeclarationPrimaryBusVehicleDataProviderV210
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V210_JOBS;
		public new const string XSD_TYPE = "Vehicle_PEV_PrimaryBusDeclarationType";
		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		#region Overrides of XMLDeclarationPEVMediumLorryExDataProviderV210

		public override string PowertrainPositionPrefix => "E";

		#endregion

		public XMLDeclarationPEVPrimaryBusDataProviderV210(IXMLDeclarationJobInputData jobData, XmlNode xmlNode, string sourceFile)
			: base(jobData, xmlNode, sourceFile) { }

		#region Overrides of XMLDeclarationVehicleDataProviderV10

		public override CubicMeter CargoVolume => null;

		#endregion
	}


	// ---------------------------------------------------------------------------------------

	public class XMLDeclarationIEPCPrimaryBusDataProviderV210 : AbstractXMLDeclarationPrimaryBusVehicleDataProviderV210
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V210_JOBS;
		public new const string XSD_TYPE = "Vehicle_IEPC_PrimaryBusDeclarationType";
		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		public XMLDeclarationIEPCPrimaryBusDataProviderV210(IXMLDeclarationJobInputData jobData, XmlNode xmlNode, string sourceFile)
			: base(jobData, xmlNode, sourceFile) { }


		#region Overrides of XMLDeclarationVehicleDataProviderV10

		public override CubicMeter CargoVolume => null;

		public override bool Articulated => GetBool(XMLNames.Vehicle_Articulated);

		#endregion
	}

	// ---------------------------------------------------------------------------------------

	public class XMLDeclarationHEVIEPCSPrimaryBusDataProviderV210 : AbstractXMLDeclarationPrimaryBusVehicleDataProviderV210
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V210_JOBS;
		public new const string XSD_TYPE = "Vehicle_HEV-IEPC-S_PrimaryBusDeclarationType";
		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		#region Overrides of XMLDeclarationHEVIEPCSHeavyLorryDataProviderV210

		public override string PowertrainPositionPrefix => "E";

		#endregion

		public XMLDeclarationHEVIEPCSPrimaryBusDataProviderV210(IXMLDeclarationJobInputData jobData, XmlNode xmlNode, string sourceFile)
			: base(jobData, xmlNode, sourceFile) { }

		#region Overrides of XMLDeclarationVehicleDataProviderV10

		public override IPTOTransmissionInputData PTOTransmissionInputData => null;

		public override XmlElement PTONode => null;

		#endregion
	}


	// ---------------------------------------------------------------------------------------

}