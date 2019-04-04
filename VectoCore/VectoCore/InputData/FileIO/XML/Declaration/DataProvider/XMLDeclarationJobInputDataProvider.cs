using System.Xml;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Reader;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider
{
	public class XMLDeclarationJobInputDataProviderV10 : AbstractXMLResource, IXMLDeclarationJobInputData
	{
		protected IVehicleDeclarationInputData _vehicle;

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

		public virtual bool SavedInDeclarationMode
		{
			get { return true; }
		}

		public virtual IVehicleDeclarationInputData Vehicle
		{
			get { return _vehicle ?? (_vehicle = Reader.CreateVehicle); }
		}

		public virtual string JobName { get { return Vehicle.Identifier; } }

		#endregion

		#region Implementation of IXMLDeclarationJobInputData

		public virtual IXMLJobDataReader Reader { protected get; set; }
		public virtual IXMLDeclarationInputData InputData { get; }

		#endregion
	}
}
