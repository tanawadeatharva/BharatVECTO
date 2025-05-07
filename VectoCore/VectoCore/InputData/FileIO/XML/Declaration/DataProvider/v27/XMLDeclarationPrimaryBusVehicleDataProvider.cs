using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider.v27
{
	public abstract class AbstractXMLDeclarationPrimaryBusVehicleDataProviderV27 : AbstractXMLVehicleDataProviderV27
	{
        public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V27;

        public AbstractXMLDeclarationPrimaryBusVehicleDataProviderV27(
			IXMLDeclarationJobInputData jobData,
			XmlNode xmlNode,
			string sourceFile)
			: base(jobData, xmlNode, sourceFile)
		{
			SourceType = DataSourceType.XMLEmbedded;
		}

		#region Overrides of XMLDeclarationVehicleDataProviderV10

		public override bool? SleeperCab => false;

		public override CubicMeter CargoVolume => null;

		public override XmlElement PTONode => null;

		public override IPTOTransmissionInputData PTOTransmissionInputData => null;

		public override bool Articulated => GetBool(XMLNames.Vehicle_Articulated);

		public override Kilogram CurbMassChassis => null;

		public override Kilogram GrossVehicleMassRating => GetDouble(XMLNames.Vehicle_TPMLM).SI<Kilogram>();

		public override Meter EntranceHeight => null;

		public override bool VocationalVehicle => false;
		
		#endregion

		#region Overrides of XMLDeclarationVehicleDataProviderV20

		public override bool ZeroEmissionVehicle => GetBool(XMLNames.Vehicle_ZeroEmissionVehicle);

		#endregion

		#region Overrides of AbstractXMLResource

		protected override XNamespace SchemaNamespace => NAMESPACE_URI;

		protected override DataSourceType SourceType { get; }

		#endregion
	}

	public class XMLDeclaration_FCHV_PrimaryBus_DataProviderV27 : AbstractXMLDeclarationPrimaryBusVehicleDataProviderV27
	{
		public new const string XSD_TYPE = "Vehicle_FCHV_Fx_PrimaryBusDeclarationType"; 
		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		public XMLDeclaration_FCHV_PrimaryBus_DataProviderV27(IXMLDeclarationJobInputData jobData, XmlNode xmlNode, string sourceFile)
			: base(jobData, xmlNode, sourceFile) { }

        public override string PowertrainPositionPrefix => "F";

        public override IList<ITorqueLimitInputData> TorqueLimits => null;

		public override VectoSimulationJobType VehicleType => VectoSimulationJobType.FCHV;

		public override bool HybridElectricHDV => true;

        public override bool OVC => GetBool("OVC");

        public override bool BatteryOnlyMode => GetBool("BatteryOnlyMode");

        public override DynamicChargingTechnology DynamicChargingTechnology => DynamicChargingTechnologyHelper.Parse(GetString("DynamicChargingTechnology"));
    }

	public class XMLDeclaration_FCHV_IEPC_PrimaryBus_DataProviderV27 : XMLDeclaration_FCHV_PrimaryBus_DataProviderV27
    {
		public new const string XSD_TYPE = "Vehicle_FCHV_IEPC_PrimaryBusDeclarationType";
		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		public XMLDeclaration_FCHV_IEPC_PrimaryBus_DataProviderV27(IXMLDeclarationJobInputData jobData, XmlNode xmlNode, string sourceFile)
			: base(jobData, xmlNode, sourceFile) { }

		public override VectoSimulationJobType VehicleType => VectoSimulationJobType.FCHV_IEPC;
	}

}
