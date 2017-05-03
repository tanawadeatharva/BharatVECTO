using System.Xml;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.InputData.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Resources;

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

		public IStartStopDeclarationInputData StartStop
		{
			get {
				//var node =
				//	Navigator.SelectSingleNode(Helper.Query(VehiclePath,
				//		XMLNames.Vehicle_AdvancedDriverAssist,
				//		XMLNames.Vehicle_AdvancedDriverAssist_EngineStartStop,
				//		XMLNames.Vehicle_AdvancedDriverAssist_EngineStartStop_Enabled), Manager);
				return new StartStopInputData() {
					Enabled = false
				};
			}
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