using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration
{
	// ReSharper disable once InconsistentNaming
	public class XMLDeclarationJobInputDataProvider : AbstractDeclarationXMLComponentDataProvider, IDeclarationJobInputData
	{
		public XMLDeclarationJobInputDataProvider(XMLDeclarationInputDataProvider xmlInputDataProvider) : base(xmlInputDataProvider)
		{
			XBasePath = VehiclePath;
		}


		public IVehicleDeclarationInputData Vehicle
		{
			get { return InputData.VehicleInputData; }
		}

		public string JobName
		{
			get { return GetAttributeValue("", XMLNames.Component_ID_Attr); }
		}
	}
}