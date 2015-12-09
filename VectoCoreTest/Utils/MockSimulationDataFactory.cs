using TUGraz.VectoCore.InputData;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.InputData.FileIO.Reader.DataObjectAdaper;
using TUGraz.VectoCore.Models.SimulationComponent.Data;

namespace TUGraz.VectoCore.Tests.Utils
{
	public class MockSimulationDataFactory
	{
		/// <summary>
		/// Create gearboxdata instance directly from a file
		/// </summary>
		/// <param name="gearBoxFile"></param>
		/// <param name="engineFile"></param>
		/// <returns>GearboxData instance</returns>
		public static GearboxData CreateGearboxDataFromFile(string gearBoxFile, string engineFile)
		{
			var gearboxInput = JSONInputDataFactory.ReadGearbox(gearBoxFile);
			var engineInput = JSONInputDataFactory.ReadEngine(engineFile);
			var dao = new DeclarationDataAdapter();
			var engineData = dao.CreateEngineData(engineInput);
			return dao.CreateGearboxData(gearboxInput, engineData);
		}

		public static AxleGearData CreateAxleGearDataFromFile(string axleGearFile)
		{
			var dao = new DeclarationDataAdapter();
			var axleGearInput = JSONInputDataFactory.ReadGearbox(axleGearFile);
			return dao.CreateAxleGearData((IAxleGearInputData)axleGearInput);
		}

		public static CombustionEngineData CreateEngineDataFromFile(string engineFile)
		{
			var dao = new EngineeringDataAdapter();
			var engineInput = JSONInputDataFactory.ReadEngine(engineFile);
			return dao.CreateEngineData(engineInput);
		}

		public static VehicleData CreateVehicleDataFromFile(string vehicleDataFile)
		{
			var dao = new EngineeringDataAdapter();
			var vehicleInput = JSONInputDataFactory.ReadJsonVehicle(vehicleDataFile);
			return dao.CreateVehicleData(vehicleInput);
		}

		public static DriverData CreateDriverDataFromFile(string driverDataFile)
		{
			var jobInput = JSONInputDataFactory.ReadJsonJob(driverDataFile);
			var dao = new EngineeringDataAdapter();
			return dao.CreateDriverData(jobInput.DriverInputData);
		}
	}
}