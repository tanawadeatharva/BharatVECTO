using System.Xml;
using TUGraz.IVT.VectoXML;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider
{
	public class XMLDeclarationRetarderDataProviderV10 : AbstractCommonComponentType, IXMLRetarderInputData
	{
		public const string NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V10;

		protected IXMLDeclarationVehicleData Vehicle;

		public XMLDeclarationRetarderDataProviderV10(IXMLDeclarationVehicleData vehicle, XmlNode componentNode, string sourceFile) :
			base(componentNode, sourceFile)
		{
			SourceType = DataSourceType.XMLFile;
			Vehicle = vehicle;
		}

		#region Implementation of IRetarderInputData

		public virtual RetarderType Type
		{
			get { return Vehicle.RetarderType; }
		}

		public virtual double Ratio
		{
			get { return Vehicle.RetarderRatio; }
		}

		public virtual TableData LossMap
		{
			get {
				return ReadTableData(XMLNames.Retarder_RetarderLossMap, XMLNames.Retarder_RetarderLossMap_Entry, AttributeMappings.RetarderLossmapMapping);
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
