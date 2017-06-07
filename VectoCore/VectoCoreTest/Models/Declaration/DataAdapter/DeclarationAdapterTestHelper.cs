using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.InputData.Reader.Impl;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data;

namespace TUGraz.VectoCore.Tests.Models.Declaration.DataAdapter
{
	public class DeclarationAdapterTestHelper
	{
		public static VectoRunData[] CreateVectoRunData(string file)
		{
			var inputData = (IDeclarationInputDataProvider)JSONInputDataFactory.ReadJsonJob(file);
			var dataReader = new DeclarationModeVectoRunDataFactory(inputData, null);
			var runData = dataReader.NextRun().ToArray();
			return runData;
		}

		public static void AssertVehicleData(VehicleData vehicleData, AirdragData airdragData, VehicleCategory vehicleCategory,
			VehicleClass vehicleClass, AxleConfiguration axleConfiguration, double wheelsInertia, double totalVehicleWeight,
			double totalRollResistance, double aerodynamicDragArea)
		{
			Assert.AreEqual(vehicleCategory, vehicleData.VehicleCategory, "VehicleCategory");
			Assert.AreEqual(vehicleClass, vehicleData.VehicleClass, "VehicleClass");
			Assert.AreEqual(axleConfiguration, vehicleData.AxleConfiguration, "AxleConfiguration");
			Assert.AreEqual(totalVehicleWeight, vehicleData.TotalVehicleWeight.Value(), 1e-3, "TotalVehicleWeight");
			Assert.AreEqual(wheelsInertia, vehicleData.WheelsInertia.Value(), 1e-6, "WheelsInertia");
			Assert.AreEqual(totalRollResistance, vehicleData.TotalRollResistanceCoefficient, 1e-6, "TotalRollResistance");

			Assert.AreEqual(aerodynamicDragArea, airdragData.CrossWindCorrectionCurve.AirDragArea.Value(), 1e-6, "Cd x A");
		}
	}
}