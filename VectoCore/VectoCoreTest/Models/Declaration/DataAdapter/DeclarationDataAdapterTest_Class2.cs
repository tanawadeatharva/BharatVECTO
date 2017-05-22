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
	public class DeclarationDataAdapterTest_Class2
	{
		public const string Class2RigidTruckNoEMSJob =
			@"TestData\Integration\DeclarationMode\Class2_RigidTruck_4x2\Class2_RigidTruck_DECL.vecto";

		public const int CurbWeight = 4670;

		[TestCase(Class2RigidTruckNoEMSJob, 0)]
		public void TestClass2_Vehicle_LongHaul_LowLoad(string file, int runIdx)
		{
			var inputData = (IDeclarationInputDataProvider)JSONInputDataFactory.ReadJsonJob(file);
			var dataReader = new DeclarationModeVectoRunDataFactory(inputData, null);
			var runData = dataReader.NextRun().ToArray();

			Assert.AreEqual(6, runData.Length);

			// long haul, min load
			DeclarationAdapterTestHelper.AssertVehicleData(runData[runIdx].VehicleData,
				vehicleCategory: VehicleCategory.RigidTruck,
				vehicleClass: VehicleClass.Class2,
				axleConfiguration: AxleConfiguration.AxleConfig_4x2,
				wheelsInertia: 57,
				totalVehicleWeight: CurbWeight + 1900 + 3400 + 603.917 + 710,
				totalRollResistance: 0.006954);
		}

		[TestCase(Class2RigidTruckNoEMSJob, 1)]
		public void TestClass2_Vehicle_LongHaul_RefLoad(string file, int runIdx)
		{
			var inputData = (IDeclarationInputDataProvider)JSONInputDataFactory.ReadJsonJob(file);
			var dataReader = new DeclarationModeVectoRunDataFactory(inputData, null);
			var runData = dataReader.NextRun().ToArray();

			// long haul, ref load
			DeclarationAdapterTestHelper.AssertVehicleData(runData[runIdx].VehicleData,
				vehicleCategory: VehicleCategory.RigidTruck,
				vehicleClass: VehicleClass.Class2,
				axleConfiguration: AxleConfiguration.AxleConfig_4x2,
				wheelsInertia: 57,
				totalVehicleWeight: CurbWeight + 1900 + 3400 + 4541.176 + 5325,
				totalRollResistance: 0.0065733);
		}


		[TestCase(Class2RigidTruckNoEMSJob, 2)]
		public void TestClass2_Vehicle_RegionalDel_LowLoad(string file, int runIdx)
		{
			var inputData = (IDeclarationInputDataProvider)JSONInputDataFactory.ReadJsonJob(file);
			var dataReader = new DeclarationModeVectoRunDataFactory(inputData, null);
			var runData = dataReader.NextRun().ToArray();

			// regional del., min load
			DeclarationAdapterTestHelper.AssertVehicleData(runData[runIdx].VehicleData,
				vehicleCategory: VehicleCategory.RigidTruck,
				vehicleClass: VehicleClass.Class2,
				axleConfiguration: AxleConfiguration.AxleConfig_4x2,
				wheelsInertia: 39,
				totalVehicleWeight: CurbWeight + 1900 + 603.917,
				totalRollResistance: 0.007461);
		}

		[TestCase(Class2RigidTruckNoEMSJob, 3)]
		public void TestClass2_Vehicle_RegionalDel_RefLoad(string file, int runIdx)
		{
			var inputData = (IDeclarationInputDataProvider)JSONInputDataFactory.ReadJsonJob(file);
			var dataReader = new DeclarationModeVectoRunDataFactory(inputData, null);
			var runData = dataReader.NextRun().ToArray();

			// regional del., ref load
			DeclarationAdapterTestHelper.AssertVehicleData(runData[runIdx].VehicleData,
				vehicleCategory: VehicleCategory.RigidTruck,
				vehicleClass: VehicleClass.Class2,
				axleConfiguration: AxleConfiguration.AxleConfig_4x2,
				wheelsInertia: 39,
				totalVehicleWeight: CurbWeight + 1900 + 3019.588,
				totalRollResistance: 0.007248);
		}


		[TestCase(Class2RigidTruckNoEMSJob, 4)]
		public void TestClass2_Vehicle_UrbanDel_LowLoad(string file, int runIdx)
		{
			var inputData = (IDeclarationInputDataProvider)JSONInputDataFactory.ReadJsonJob(file);
			var dataReader = new DeclarationModeVectoRunDataFactory(inputData, null);
			var runData = dataReader.NextRun().ToArray();

			// municipal, min load
			DeclarationAdapterTestHelper.AssertVehicleData(runData[runIdx].VehicleData,
				vehicleCategory: VehicleCategory.RigidTruck,
				vehicleClass: VehicleClass.Class2,
				axleConfiguration: AxleConfiguration.AxleConfig_4x2,
				wheelsInertia: 39,
				totalVehicleWeight: CurbWeight + 1900 + 603.917,
				totalRollResistance: 0.007461);
		}

		[TestCase(Class2RigidTruckNoEMSJob, 5)]
		public void TestClass2_Vehicle_UrbanDel_RefLoad(string file, int runIdx)
		{
			var inputData = (IDeclarationInputDataProvider)JSONInputDataFactory.ReadJsonJob(file);
			var dataReader = new DeclarationModeVectoRunDataFactory(inputData, null);
			var runData = dataReader.NextRun().ToArray();

			// municipal, min load
			DeclarationAdapterTestHelper.AssertVehicleData(runData[runIdx].VehicleData,
				vehicleCategory: VehicleCategory.RigidTruck,
				vehicleClass: VehicleClass.Class2,
				axleConfiguration: AxleConfiguration.AxleConfig_4x2,
				wheelsInertia: 39,
				totalVehicleWeight: CurbWeight + 1900 + 3019.588,
				totalRollResistance: 0.007248);
		}
	}
}