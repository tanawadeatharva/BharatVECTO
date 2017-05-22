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
	public class DeclarationDataAdapterTest_Class5
	{
		public const string Class5TractorPTOJob =
			@"TestData\Integration\DeclarationMode\Class5_Tractor_4x2\Class5_Tractor_DECL.vecto";

		public const int CurbWeight = 8229;

		public const double CdxA = 5.3;

		[TestCase(Class5TractorPTOJob, 0)]
		public void TestClass5_Vehicle_LongHaul_LowLoad(string file, int runIdx)
		{
			var runData = DeclarationAdapterTestHelper.CreateVectoRunData(file);

			Assert.AreEqual(8, runData.Length);

			// long haul, min load
			DeclarationAdapterTestHelper.AssertVehicleData(runData[runIdx].VehicleData,
				vehicleCategory: VehicleCategory.Tractor,
				vehicleClass: VehicleClass.Class5,
				axleConfiguration: AxleConfiguration.AxleConfig_4x2,
				wheelsInertia: 204.6,
				totalVehicleWeight: CurbWeight + 7500 + 2600,
				totalRollResistance: 0.0062662,
				aerodynamicDragArea: CdxA);
		}

		[TestCase(Class5TractorPTOJob, 1)]
		public void TestClass5_Vehicle_LongHaul_RefLoad(string file, int runIdx)
		{
			var runData = DeclarationAdapterTestHelper.CreateVectoRunData(file);

			// long haul, ref load
			DeclarationAdapterTestHelper.AssertVehicleData(runData[runIdx].VehicleData,
				vehicleCategory: VehicleCategory.Tractor,
				vehicleClass: VehicleClass.Class5,
				axleConfiguration: AxleConfiguration.AxleConfig_4x2,
				wheelsInertia: 204.6,
				totalVehicleWeight: CurbWeight + 7500 + 19300,
				totalRollResistance: 0.00587322,
				aerodynamicDragArea: CdxA);
		}

		[TestCase(Class5TractorPTOJob, 2)]
		public void TestClass5_Vehicle_LongHaul_EMS_LowLoad(string file, int runIdx)
		{
			var runData = DeclarationAdapterTestHelper.CreateVectoRunData(file);

			Assert.AreEqual(8, runData.Length);

			// long haul, min load
			DeclarationAdapterTestHelper.AssertVehicleData(runData[runIdx].VehicleData,
				vehicleCategory: VehicleCategory.Tractor,
				vehicleClass: VehicleClass.Class5,
				axleConfiguration: AxleConfiguration.AxleConfig_4x2,
				wheelsInertia: 281.4,
				totalVehicleWeight: CurbWeight + 7500 + 5400 + 3500,
				totalRollResistance: 0.0062159,
				aerodynamicDragArea: CdxA + 1.5);
		}

		[TestCase(Class5TractorPTOJob, 3)]
		public void TestClass5_Vehicle_LongHaul_EMS_RefLoad(string file, int runIdx)
		{
			var runData = DeclarationAdapterTestHelper.CreateVectoRunData(file);

			// long haul, ref load
			DeclarationAdapterTestHelper.AssertVehicleData(runData[runIdx].VehicleData,
				vehicleCategory: VehicleCategory.Tractor,
				vehicleClass: VehicleClass.Class5,
				axleConfiguration: AxleConfiguration.AxleConfig_4x2,
				wheelsInertia: 281.4,
				totalVehicleWeight: CurbWeight + 7500 + 5400 + 26500,
				totalRollResistance: 0.0058192,
				aerodynamicDragArea: CdxA + 1.5);
		}

		[TestCase(Class5TractorPTOJob, 4)]
		public void TestClass5_Vehicle_RegionalDel_LowLoad(string file, int runIdx)
		{
			var runData = DeclarationAdapterTestHelper.CreateVectoRunData(file);

			// regional del., min load
			DeclarationAdapterTestHelper.AssertVehicleData(runData[runIdx].VehicleData,
				vehicleCategory: VehicleCategory.Tractor,
				vehicleClass: VehicleClass.Class5,
				axleConfiguration: AxleConfiguration.AxleConfig_4x2,
				wheelsInertia: 204.6,
				totalVehicleWeight: CurbWeight + 7500 + 2600,
				totalRollResistance: 0.0062565,
				aerodynamicDragArea: CdxA);
		}

		[TestCase(Class5TractorPTOJob, 5)]
		public void TestClass5_Vehicle_RegionalDel_RefLoad(string file, int runIdx)
		{
			var runData = DeclarationAdapterTestHelper.CreateVectoRunData(file);

			// regional del., ref load
			DeclarationAdapterTestHelper.AssertVehicleData(runData[runIdx].VehicleData,
				vehicleCategory: VehicleCategory.Tractor,
				vehicleClass: VehicleClass.Class5,
				axleConfiguration: AxleConfiguration.AxleConfig_4x2,
				wheelsInertia: 204.6,
				totalVehicleWeight: CurbWeight + 7500 + 12900,
				totalRollResistance: 0.0059836,
				aerodynamicDragArea: CdxA);
		}

		[TestCase(Class5TractorPTOJob, 6)]
		public void TestClass5_Vehicle_RegionalDelEMS_LowLoad(string file, int runIdx)
		{
			var runData = DeclarationAdapterTestHelper.CreateVectoRunData(file);

			// regional del., min load
			DeclarationAdapterTestHelper.AssertVehicleData(runData[runIdx].VehicleData,
				vehicleCategory: VehicleCategory.Tractor,
				vehicleClass: VehicleClass.Class5,
				axleConfiguration: AxleConfiguration.AxleConfig_4x2,
				wheelsInertia: 281.4,
				totalVehicleWeight: CurbWeight + 7500 + 5400 + 3500,
				totalRollResistance: 0.006259,
				aerodynamicDragArea: CdxA + 1.5);
		}

		[TestCase(Class5TractorPTOJob, 7)]
		public void TestClass5_Vehicle_RegionalDelEMS_RefLoad(string file, int runIdx)
		{
			var runData = DeclarationAdapterTestHelper.CreateVectoRunData(file);

			// regional del., ref load
			DeclarationAdapterTestHelper.AssertVehicleData(runData[runIdx].VehicleData,
				vehicleCategory: VehicleCategory.Tractor,
				vehicleClass: VehicleClass.Class5,
				axleConfiguration: AxleConfiguration.AxleConfig_4x2,
				wheelsInertia: 281.4,
				totalVehicleWeight: CurbWeight + 7500 + 5400 + 17500,
				totalRollResistance: 0.0059836,
				aerodynamicDragArea: CdxA + 1.5);
		}
	}
}