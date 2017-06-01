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
		public const double CdxA = 4.83;

		[TestCase(Class2RigidTruckNoEMSJob, 0)]
		public void TestClass2_Vehicle_LongHaul_LowLoad(string file, int runIdx)
		{
			var runData = DeclarationAdapterTestHelper.CreateVectoRunData(file);

			Assert.AreEqual(6, runData.Length);

			// long haul, min load
			DeclarationAdapterTestHelper.AssertVehicleData(runData[runIdx].VehicleData, runData[runIdx].AirdragData,
				vehicleCategory: VehicleCategory.RigidTruck,
				vehicleClass: VehicleClass.Class2,
				axleConfiguration: AxleConfiguration.AxleConfig_4x2,
				wheelsInertia: 57,
				totalVehicleWeight: CurbWeight + 1900 + 3400 + 603.917 + 710,
				totalRollResistance: 0.006954,
				aerodynamicDragArea: CdxA + 1.3);
		}

		[TestCase(Class2RigidTruckNoEMSJob, 1)]
		public void TestClass2_Vehicle_LongHaul_RefLoad(string file, int runIdx)
		{
			var runData = DeclarationAdapterTestHelper.CreateVectoRunData(file);

			// long haul, ref load
			DeclarationAdapterTestHelper.AssertVehicleData(runData[runIdx].VehicleData, runData[runIdx].AirdragData,
				vehicleCategory: VehicleCategory.RigidTruck,
				vehicleClass: VehicleClass.Class2,
				axleConfiguration: AxleConfiguration.AxleConfig_4x2,
				wheelsInertia: 57,
				totalVehicleWeight: CurbWeight + 1900 + 3400 + 4541.176 + 5325,
				totalRollResistance: 0.0065733,
				aerodynamicDragArea: CdxA + 1.3);
		}


		[TestCase(Class2RigidTruckNoEMSJob, 2)]
		public void TestClass2_Vehicle_RegionalDel_LowLoad(string file, int runIdx)
		{
			var runData = DeclarationAdapterTestHelper.CreateVectoRunData(file);

			// regional del., min load
			DeclarationAdapterTestHelper.AssertVehicleData(runData[runIdx].VehicleData, runData[runIdx].AirdragData,
				vehicleCategory: VehicleCategory.RigidTruck,
				vehicleClass: VehicleClass.Class2,
				axleConfiguration: AxleConfiguration.AxleConfig_4x2,
				wheelsInertia: 39,
				totalVehicleWeight: CurbWeight + 1900 + 603.917,
				totalRollResistance: 0.007461,
				aerodynamicDragArea: CdxA);
		}

		[TestCase(Class2RigidTruckNoEMSJob, 3)]
		public void TestClass2_Vehicle_RegionalDel_RefLoad(string file, int runIdx)
		{
			var runData = DeclarationAdapterTestHelper.CreateVectoRunData(file);

			// regional del., ref load
			DeclarationAdapterTestHelper.AssertVehicleData(runData[runIdx].VehicleData, runData[runIdx].AirdragData,
				vehicleCategory: VehicleCategory.RigidTruck,
				vehicleClass: VehicleClass.Class2,
				axleConfiguration: AxleConfiguration.AxleConfig_4x2,
				wheelsInertia: 39,
				totalVehicleWeight: CurbWeight + 1900 + 3019.588,
				totalRollResistance: 0.007248,
				aerodynamicDragArea: CdxA);
		}


		[TestCase(Class2RigidTruckNoEMSJob, 4)]
		public void TestClass2_Vehicle_UrbanDel_LowLoad(string file, int runIdx)
		{
			var runData = DeclarationAdapterTestHelper.CreateVectoRunData(file);
			// municipal, min load
			DeclarationAdapterTestHelper.AssertVehicleData(runData[runIdx].VehicleData, runData[runIdx].AirdragData,
				vehicleCategory: VehicleCategory.RigidTruck,
				vehicleClass: VehicleClass.Class2,
				axleConfiguration: AxleConfiguration.AxleConfig_4x2,
				wheelsInertia: 39,
				totalVehicleWeight: CurbWeight + 1900 + 603.917,
				totalRollResistance: 0.007461,
				aerodynamicDragArea: CdxA);
		}

		[TestCase(Class2RigidTruckNoEMSJob, 5)]
		public void TestClass2_Vehicle_UrbanDel_RefLoad(string file, int runIdx)
		{
			var runData = DeclarationAdapterTestHelper.CreateVectoRunData(file);

			// municipal, min load
			DeclarationAdapterTestHelper.AssertVehicleData(runData[runIdx].VehicleData, runData[runIdx].AirdragData,
				vehicleCategory: VehicleCategory.RigidTruck,
				vehicleClass: VehicleClass.Class2,
				axleConfiguration: AxleConfiguration.AxleConfig_4x2,
				wheelsInertia: 39,
				totalVehicleWeight: CurbWeight + 1900 + 3019.588,
				totalRollResistance: 0.007248,
				aerodynamicDragArea: CdxA);
		}
	}
}