using Microsoft.VisualStudio.TestTools.UnitTesting;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.SimulationComponent.Data;

namespace TUGraz.VectoCore.Tests.Models.Declaration.DataAdapter
{
	public class DeclarationAdapterTestHelper
	{
		public static void AssertVehicleData(VehicleData vehicleData, VehicleCategory vehicleCategory,
			VehicleClass vehicleClass,
			AxleConfiguration axleConfiguration, double wheelsInertia, double totalVehicleWeight, double totalRollResistance)
		{
			Assert.AreEqual(vehicleCategory, vehicleData.VehicleCategory, "VehicleCategory");
			Assert.AreEqual(vehicleClass, vehicleData.VehicleClass, "VehicleClass");
			Assert.AreEqual(axleConfiguration, vehicleData.AxleConfiguration, "AxleConfiguration");
			Assert.AreEqual(totalVehicleWeight, vehicleData.TotalVehicleWeight.Value(), 1e-3, "TotalVehicleWeight");
			Assert.AreEqual(wheelsInertia, vehicleData.WheelsInertia.Value(), 1e-6, "WheelsInertia");
			Assert.AreEqual(totalRollResistance, vehicleData.TotalRollResistanceCoefficient, 1e-6, "TotalRollResistance");
		}
	}
}