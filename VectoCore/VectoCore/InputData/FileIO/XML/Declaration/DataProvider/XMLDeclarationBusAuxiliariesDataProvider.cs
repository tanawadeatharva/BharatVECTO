using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.InputData.FileIO.XML.Common;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider {
	public class XMLDeclarationBusAuxiliariesDataProviderV26 : AbstractXMLType, IXMLBusAuxiliariesDeclarationData
	{
		public static XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V26;

		public const string XSD_TYPE = "PrimaryVehicleAuxiliaryDataDeclarationType";

		public static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		public XMLDeclarationBusAuxiliariesDataProviderV26(
			IXMLDeclarationVehicleData vehicle, XmlNode componentNode, string sourceFile) : base(componentNode) { }

		#region Implementation of IBusAuxiliariesDeclarationData

		public string FanTechnology
		{
			get { return GetNode(new[] { "Fan", XMLNames.Auxiliaries_Auxiliary_Technology }).InnerText; }
		}

		public IList<string> SteeringPumpTechnology
		{
			get { return GetNodes(new[] { "SteeringPump", XMLNames.Auxiliaries_Auxiliary_Technology }).Cast <XmlNode>().Select(x => x.InnerText).ToList(); }
		}

		public IElectricSupplyDeclarationData ElectricSupply { get; }
		public IElectricConsumersDeclarationData ElectricConsumers { get; }
		public IPneumaticSupplyDeclarationData PneumaticSupply { get; }
		public IPneumaticConsumersDeclarationData PneumaticConsumers { get; }
		public IHVACBusAuxiliariesDeclarationData HVACAux { get; }

		#endregion
	}
}