using System.Linq;
using NUnit.Framework;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.InputData.Reader.Impl;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace TUGraz.VectoCore.Tests.Models.Declaration.DataAdapter
{
	[TestFixture]
	public class DeclarationDataAdapterTest_Class9
	{
		public const string Class9RigidTruckNoEMS_PTOJob =
			@"TestData\Integration\DeclarationMode\Class9_RigidTruck_6x2\Class9_RigidTruck_DECL.vecto";

		[TestCase(Class9RigidTruckNoEMS_PTOJob, 0)]
		public void TestClass9_Vehicle_LongHaul_LowLoad(string file, int runIdx)
		{
			var inputData = (IDeclarationInputDataProvider)JSONInputDataFactory.ReadJsonJob(file);
			var dataReader = new DeclarationModeVectoRunDataFactory(inputData, null);
			var runData = dataReader.NextRun().ToArray();

			Assert.AreEqual(6, runData.Length);

			// long haul, min load
			DeclarationAdapterTestHelper.AssertVehicleData(runData[runIdx].VehicleData,
				vehicleCategory: VehicleCategory.RigidTruck,
				vehicleClass: VehicleClass.Class9,
				axleConfiguration: AxleConfiguration.AxleConfig_6x2,
				wheelsInertia: 196,
				totalVehicleWeight: 9300 + 2200 + 5400 + 2600,
				totalRollResistance: 0.0059426);
		}

		[TestCase(Class9RigidTruckNoEMS_PTOJob, 1)]
		public void TestClass9_Vehicle_LongHaul_RefLoad(string file, int runIdx)
		{
			var inputData = (IDeclarationInputDataProvider)JSONInputDataFactory.ReadJsonJob(file);
			var dataReader = new DeclarationModeVectoRunDataFactory(inputData, null);
			var runData = dataReader.NextRun().ToArray();

			// long haul, ref load
			DeclarationAdapterTestHelper.AssertVehicleData(runData[runIdx].VehicleData,
				vehicleCategory: VehicleCategory.RigidTruck,
				vehicleClass: VehicleClass.Class9,
				axleConfiguration: AxleConfiguration.AxleConfig_6x2,
				wheelsInertia: 196,
				totalVehicleWeight: 9300 + 2200 + 5400 + 19300,
				totalRollResistance: 0.00558611);
		}


		[TestCase(Class9RigidTruckNoEMS_PTOJob, 2)]
		public void TestClass9_Vehicle_RegionalDel_LowLoad(string file, int runIdx)
		{
			var inputData = (IDeclarationInputDataProvider)JSONInputDataFactory.ReadJsonJob(file);
			var dataReader = new DeclarationModeVectoRunDataFactory(inputData, null);
			var runData = dataReader.NextRun().ToArray();

			// regional del., min load
			DeclarationAdapterTestHelper.AssertVehicleData(runData[runIdx].VehicleData,
				vehicleCategory: VehicleCategory.RigidTruck,
				vehicleClass: VehicleClass.Class9,
				axleConfiguration: AxleConfiguration.AxleConfig_6x2,
				wheelsInertia: 119.2,
				totalVehicleWeight: 9300 + 2200 + 1400,
				totalRollResistance: 0.0059109);
		}

		[TestCase(Class9RigidTruckNoEMS_PTOJob, 3)]
		public void TestClass9_Vehicle_RegionalDel_RefLoad(string file, int runIdx)
		{
			var inputData = (IDeclarationInputDataProvider)JSONInputDataFactory.ReadJsonJob(file);
			var dataReader = new DeclarationModeVectoRunDataFactory(inputData, null);
			var runData = dataReader.NextRun().ToArray();

			// regional del., ref load
			DeclarationAdapterTestHelper.AssertVehicleData(runData[runIdx].VehicleData,
				vehicleCategory: VehicleCategory.RigidTruck,
				vehicleClass: VehicleClass.Class9,
				axleConfiguration: AxleConfiguration.AxleConfig_6x2,
				wheelsInertia: 119.2,
				totalVehicleWeight: 9300 + 2200 + 7100,
				totalRollResistance: 0.0056986);
		}


		[TestCase(Class9RigidTruckNoEMS_PTOJob, 4)]
		public void TestClass9_Vehicle_Municipal_LowLoad(string file, int runIdx)
		{
			var inputData = (IDeclarationInputDataProvider)JSONInputDataFactory.ReadJsonJob(file);
			var dataReader = new DeclarationModeVectoRunDataFactory(inputData, null);
			var runData = dataReader.NextRun().ToArray();

			// municipal, min load
			DeclarationAdapterTestHelper.AssertVehicleData(runData[runIdx].VehicleData,
				vehicleCategory: VehicleCategory.RigidTruck,
				vehicleClass: VehicleClass.Class9,
				axleConfiguration: AxleConfiguration.AxleConfig_6x2,
				wheelsInertia: 119.2,
				totalVehicleWeight: 9300 + 6750 + 1200,
				totalRollResistance: 0.0057417);
		}

		[TestCase(Class9RigidTruckNoEMS_PTOJob, 5)]
		public void TestClass9_Vehicle_Municipal_RefLoad(string file, int runIdx)
		{
			var inputData = (IDeclarationInputDataProvider)JSONInputDataFactory.ReadJsonJob(file);
			var dataReader = new DeclarationModeVectoRunDataFactory(inputData, null);
			var runData = dataReader.NextRun().ToArray();

			// municipal, min load
			DeclarationAdapterTestHelper.AssertVehicleData(runData[runIdx].VehicleData,
				vehicleCategory: VehicleCategory.RigidTruck,
				vehicleClass: VehicleClass.Class9,
				axleConfiguration: AxleConfiguration.AxleConfig_6x2,
				wheelsInertia: 119.2,
				totalVehicleWeight: 9300 + 6750 + 6000,
				totalRollResistance: 0.005602);
		}
	}
}