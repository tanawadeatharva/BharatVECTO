using System.Xml;
using Ninject;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Factory;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Reader.Impl
{
	public class XMLADASReaderV10 : AbstractComponentReader, IXMLADASReader
	{
		public const string NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V10;

		[Inject]
		public IDeclarationInjectFactory Factory { protected get; set; }

		protected IXMLDeclarationVehicleData Vehicle;
		private IAdvancedDriverAssistantSystemDeclarationInputData _adas;


		public XMLADASReaderV10(IXMLDeclarationVehicleData vehicle, XmlNode vehicleNode, bool verifyXML) : base(
			vehicle, vehicleNode, verifyXML)
		{
			Vehicle = vehicle;
		}

		#region Implementation of IXMLADASReader

		public virtual IAdvancedDriverAssistantSystemDeclarationInputData ADASInputData
		{
			get { return _adas ?? (_adas = CreateComponent(XMLNames.Vehicle_ADAS, ADASCreator)); }
		}

		#endregion

		protected virtual IAdvancedDriverAssistantSystemDeclarationInputData ADASCreator(
			string version, XmlNode componentNode, string sourceFile)
		{
			return Factory.CreateADASData(version, Vehicle, componentNode, sourceFile);
		}
	}

	// ---------------------------------------------------------------------------------------

	public class XMLADASReaderV20 : XMLADASReaderV10
	{
		public new const string NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V20;

		public XMLADASReaderV20(IXMLDeclarationVehicleData vehicle, XmlNode vehicleNode, bool verifyXML) : base(vehicle, vehicleNode, verifyXML) { }
	}
}
