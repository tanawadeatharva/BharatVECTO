using System.Xml;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.InputData.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Data;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration
{
	public class XMLDeclarationDriverDataProvider : AbstractDeclarationXMLComponentDataProvider,
		IDriverDeclarationInputData
	{
		public XMLDeclarationDriverDataProvider(XMLDeclarationInputDataProvider xmlInputDataProvider)
			: base(xmlInputDataProvider)
		{
			XBasePath = VehiclePath;
		}

		public IOverSpeedEcoRollDeclarationInputData OverSpeedEcoRoll
		{
			get {
				//var node =
				//	Navigator.SelectSingleNode(Helper.Query(VehiclePath,
				//		XMLNames.Vehicle_AdvancedDriverAssist,
				//		XMLNames.DriverModel_Overspeed,
				//		XMLNames.DriverModel_Overspeed_Mode), Manager);
				return new OverSpeedEcoRollInputData() {
					Mode = DriverMode.Overspeed
				};
			}
		}
	}
}