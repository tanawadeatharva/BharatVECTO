using System.Xml;
using Ninject;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Factory;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Reader.Impl
{
	public class XMLPTOReaderV10 : AbstractComponentReader, IXMLPTOReader
	{
		public const string NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V10;

		[Inject]
		public IDeclarationInjectFactory Factory { protected get; set; }

		protected IXMLDeclarationVehicleData Vehicle;
		private IPTOTransmissionInputData _ptoInputData;


		public XMLPTOReaderV10(IXMLDeclarationVehicleData vehicle, XmlNode componentNode, bool verifyXML) : base(
			vehicle, componentNode, verifyXML)
		{
			Vehicle = vehicle;
		}

		#region Implementation of IXMLPTOReader

		public IPTOTransmissionInputData PTOInputData
		{
			get { return _ptoInputData ?? (_ptoInputData = CreateComponent(XMLNames.Vehicle_PTO, PTOCreator)); }
		}

		#endregion

		protected IPTOTransmissionInputData PTOCreator(string version, XmlNode componentNode, string sourceFile)
		{
			return Factory.CreatePTOData(version, Vehicle, componentNode, sourceFile);
		}
	}
}
