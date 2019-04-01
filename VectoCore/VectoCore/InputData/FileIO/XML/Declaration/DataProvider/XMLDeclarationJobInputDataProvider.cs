using System.Xml;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Reader;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider
{
	public class XMLDeclarationJobInputDataProviderV10 : AbstractXMLResource, IXMLDeclarationJobInputData
	{
		private IVehicleDeclarationInputData _vehicle;

		public const string NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V10;

		public XMLDeclarationJobInputDataProviderV10(XmlNode node, IXMLDeclarationInputData inputProvider, string fileName) :
			base(node, fileName)
		{
			InputData = inputProvider;
		}

		#region Overrides of AbstractXMLResource

		protected override string SchemaNamespace
		{
			get { return NAMESPACE_URI; }
		}

		protected override DataSourceType SourceType
		{
			get { return DataSourceType.XMLFile; }
		}

		#endregion

		#region Implementation of IDeclarationJobInputData

		public bool SavedInDeclarationMode
		{
			get { return true; }
		}

		public IVehicleDeclarationInputData Vehicle
		{
			get { return _vehicle ?? (_vehicle = Reader.CreateVehicle); }
		}

		public string JobName { get { return Vehicle.Identifier; } }

		#endregion

		#region Implementation of IXMLDeclarationJobInputData

		public IXMLJobDataReader Reader { protected get; set; }
		public IXMLDeclarationInputData InputData { get; }

		#endregion
	}
}
