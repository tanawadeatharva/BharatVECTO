using System.Xml;
using TUGraz.IVT.VectoXML;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider
{
	public class XMLDeclarationTorqueConverterDataProviderV10 : AbstractCommonComponentType,
		IXMLTorqueConverterDeclarationInputData
	{
		public const string NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V10;

		public XMLDeclarationTorqueConverterDataProviderV10(
			IXMLDeclarationVehicleData vehicle, XmlNode componentNode, string sourceFile) :
			base(componentNode, sourceFile)
		{
			SourceType = DataSourceType.XMLFile;
		}

		#region Implementation of ITorqueconverterDeclarationInputData

		public TableData TCData
		{
			get {
				return ReadTableData(
					XMLNames.TorqueConverter_Characteristics, XMLNames.TorqueConverter_Characteristics_Entry,
					AttributeMappings.TorqueConverterDataMapping);
			}
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
