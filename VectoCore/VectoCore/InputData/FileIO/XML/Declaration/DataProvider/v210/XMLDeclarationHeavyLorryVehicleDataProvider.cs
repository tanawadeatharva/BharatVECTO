using System;
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

	public class XMLDeclarationConventionalHeavyLorryDataProviderV210 : AbstractXMLVehicleDataProviderV210
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V24;
		public new const string XSD_TYPE = "Vehicle_Conventional_HeavyLorryDeclarationType";
		public new static readonly string QUALIFIED_XSD_TYPE =
			XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		public XMLDeclarationConventionalHeavyLorryDataProviderV210(
			IXMLDeclarationJobInputData jobData, XmlNode xmlNode, string sourceFile)
			: base(jobData, xmlNode, sourceFile) { }

		public override VehicleCategory VehicleCategory =>
			VehicleCategoryHelper.Parse(GetString(XMLNames.ChassisConfiguration));

		public override Kilogram CurbMassChassis => GetDouble(XMLNames.CorrectedActualMass).SI<Kilogram>();

		public override Kilogram GrossVehicleMassRating => GetDouble(XMLNames.TPMLM).SI<Kilogram>();

		#region Overrides of XMLDeclarationVehicleDataProviderV10

		public override IAdvancedDriverAssistantSystemDeclarationInputData ADAS => ADASReader.ADASInputData;

		public override IList<ITorqueLimitInputData> TorqueLimits =>
			ElementExists(XMLNames.Vehicle_TorqueLimits) ? base.TorqueLimits : null;

		public override TankSystem? TankSystem =>
			ElementExists(XMLNames.Vehicle_NgTankSystem)
				? EnumHelper.ParseEnum<TankSystem>(GetString(XMLNames.Vehicle_NgTankSystem))
				: (TankSystem?)null;

		#endregion
	}

	// ---------------------------------------------------------------------------------------

	public class XMLDeclarationHEVPxHeavyLorryDataProviderV210 : AbstractXMLVehicleDataProviderV210
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V24;
		public new const string XSD_TYPE = "Vehicle_HEV-Px_HeavyLorryDeclarationType";
		public new static readonly string QUALIFIED_XSD_TYPE =
			XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		#region Overrides of VehicleDataProviderHelper

		public override string PowertrainPositionPrefix => "P";

		#endregion

		public XMLDeclarationHEVPxHeavyLorryDataProviderV210(
			IXMLDeclarationJobInputData jobData, XmlNode xmlNode, string sourceFile)
			: base(jobData, xmlNode, sourceFile) { }

		#region Overrides of XMLDeclarationVehicleDataProviderV20

		public override TankSystem? TankSystem =>
			ElementExists(XMLNames.Vehicle_NgTankSystem)
				? EnumHelper.ParseEnum<TankSystem>(GetString(XMLNames.Vehicle_NgTankSystem))
				: (TankSystem?)null;

		#endregion


		#region Overrides of XMLDeclarationVehicleDataProviderV10

		public override IList<ITorqueLimitInputData> TorqueLimits =>
			ElementExists(XMLNames.Vehicle_TorqueLimits) ? base.TorqueLimits : null;


		public override TableData BoostingLimitations
			=> ElementExists(XMLNames.Vehicle_BoostingLimitation)
				? ReadTableData(XMLNames.Vehicle_BoostingLimitation, XMLNames.BoostingLimitation_Entry,
					new Dictionary<string, string> {
						{XMLNames.BoostingLimitation_RotationalSpeed, XMLNames.BoostingLimitation_RotationalSpeed},
						{XMLNames.BoostingLimitation_BoostingTorque, XMLNames.BoostingLimitation_BoostingTorque}
					})
				: null;

		#endregion
	}

	// ---------------------------------------------------------------------------------------

	public class XMLDeclarationHEVSxHeavyLorryDataProviderV210 : AbstractXMLVehicleDataProviderV210
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V24;
		public new const string XSD_TYPE = "Vehicle_HEV-Sx_HeavyLorryDeclarationType";
		public new static readonly string QUALIFIED_XSD_TYPE =
			XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		#region Overrides of XMLDeclarationHEVPxHeavyLorryDataProviderV210

		public override string PowertrainPositionPrefix => "E";

		#endregion

		public XMLDeclarationHEVSxHeavyLorryDataProviderV210(IXMLDeclarationJobInputData jobData, XmlNode xmlNode, string sourceFile)
			: base(jobData, xmlNode, sourceFile) { }

		#region Overrides of XMLDeclarationHEVPxHeavyLorryDataProviderV210

		public override TableData BoostingLimitations => null;

		#endregion
	}

	// ---------------------------------------------------------------------------------------

	public class XMLDeclarationPEVHeavyLorryE2DataProviderV210 : AbstractXMLVehicleDataProviderV210
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V24;
		public new const string XSD_TYPE = "Vehicle_PEV_HeavyLorryDeclarationType";
		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		#region Overrides of XMLDeclarationHEVPxHeavyLorryDataProviderV210

		public override string PowertrainPositionPrefix => "E";

		#endregion

		public XMLDeclarationPEVHeavyLorryE2DataProviderV210(IXMLDeclarationJobInputData jobData, XmlNode xmlNode, string sourceFile)
			: base(jobData, xmlNode, sourceFile) { }


		#region Overrides of XMLDeclarationHEVPxHeavyLorryDataProviderV210

		public override TableData BoostingLimitations => null;

		#endregion

		#region Overrides of XMLDeclarationVehicleDataProviderV10

		public override CubicMeter CargoVolume => null;

		#endregion
	}

	// ---------------------------------------------------------------------------------------

	public class XMLDeclarationIEPCHeavyLorryDataProviderV210 : AbstractXMLVehicleDataProviderV210
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V24;
		public new const string XSD_TYPE = "Vehicle_IEPC_HeavyLorryDeclarationType";
		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		public XMLDeclarationIEPCHeavyLorryDataProviderV210(IXMLDeclarationJobInputData jobData, XmlNode xmlNode, string sourceFile)
			: base(jobData, xmlNode, sourceFile) { }


		#region Overrides of XMLDeclarationVehicleDataProviderV10

		public override CubicMeter CargoVolume => null;

		public override IList<ITorqueLimitInputData> TorqueLimits => null;

		
		

		#endregion


	}

	// ---------------------------------------------------------------------------------------

	public class XMLDeclarationHEVIEPCSHeavyLorryDataProviderV210 : AbstractXMLVehicleDataProviderV210
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V24;
		public new const string XSD_TYPE = "Vehicle_HEV-IEPC-S_HeavyLorryDeclarationType";
		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		#region Overrides of VehicleDataProviderHelper

		public override string PowertrainPositionPrefix => "E";

		#endregion

		public XMLDeclarationHEVIEPCSHeavyLorryDataProviderV210(IXMLDeclarationJobInputData jobData, XmlNode xmlNode, string sourceFile)
			: base(jobData, xmlNode, sourceFile) { }

		#region Overrides of XMLDeclarationVehicleDataProviderV20

		public override TankSystem? TankSystem =>
			ElementExists(XMLNames.Vehicle_NgTankSystem)
				? EnumHelper.ParseEnum<TankSystem>(GetString(XMLNames.Vehicle_NgTankSystem))
				: (TankSystem?)null;

		#endregion

		#region Overrides of XMLDeclarationVehicleDataProviderV10

		public override CubicMeter CargoVolume => null;

		public override IList<ITorqueLimitInputData> TorqueLimits => null;

		public override Dictionary<PowertrainPosition, List<Tuple<int, TableData>>> ElectricMotorTorqueLimits => null;
		public override TableData BoostingLimitations => null;

		#endregion
	}

	// ---------------------------------------------------------------------------------------

}