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
		public const string Class5TractorNoEMS_PTOJob =
			@"TestData\Integration\DeclarationMode\Class5_Tractor_4x2\Class5_Tractor_DECL.vecto";

		public const int CurbWeight = 8229;

		[TestCase(Class5TractorNoEMS_PTOJob, 0)]
		public void TestClass5_Vehicle_LongHaul_LowLoad(string file, int runIdx)
		{
			var inputData = (IDeclarationInputDataProvider)JSONInputDataFactory.ReadJsonJob(file);
			var dataReader = new DeclarationModeVectoRunDataFactory(inputData, null);
			var runData = dataReader.NextRun().ToArray();

			Assert.AreEqual(4, runData.Length);

			// long haul, min load
			DeclarationAdapterTestHelper.AssertVehicleData(runData[runIdx].VehicleData,
				vehicleCategory: VehicleCategory.Tractor,
				vehicleClass: VehicleClass.Class5,
				axleConfiguration: AxleConfiguration.AxleConfig_4x2,
				wheelsInertia: 204.6,
				totalVehicleWeight: CurbWeight + 7500 + 2600,
				totalRollResistance: 0.0062662);
		}

		[TestCase(Class5TractorNoEMS_PTOJob, 1)]
		public void TestClass5_Vehicle_LongHaul_RefLoad(string file, int runIdx)
		{
			var inputData = (IDeclarationInputDataProvider)JSONInputDataFactory.ReadJsonJob(file);
			var dataReader = new DeclarationModeVectoRunDataFactory(inputData, null);
			var runData = dataReader.NextRun().ToArray();

			// long haul, ref load
			DeclarationAdapterTestHelper.AssertVehicleData(runData[runIdx].VehicleData,
				vehicleCategory: VehicleCategory.Tractor,
				vehicleClass: VehicleClass.Class5,
				axleConfiguration: AxleConfiguration.AxleConfig_4x2,
				wheelsInertia: 204.6,
				totalVehicleWeight: CurbWeight + 7500 + 19300,
				totalRollResistance: 0.00587322);
		}


		[TestCase(Class5TractorNoEMS_PTOJob, 2)]
		public void TestClass5_Vehicle_RegionalDel_LowLoad(string file, int runIdx)
		{
			var inputData = (IDeclarationInputDataProvider)JSONInputDataFactory.ReadJsonJob(file);
			var dataReader = new DeclarationModeVectoRunDataFactory(inputData, null);
			var runData = dataReader.NextRun().ToArray();

			// regional del., min load
			DeclarationAdapterTestHelper.AssertVehicleData(runData[runIdx].VehicleData,
				vehicleCategory: VehicleCategory.Tractor,
				vehicleClass: VehicleClass.Class5,
				axleConfiguration: AxleConfiguration.AxleConfig_4x2,
				wheelsInertia: 204.6,
				totalVehicleWeight: CurbWeight + 7500 + 2600,
				totalRollResistance: 0.0062565);
		}

		[TestCase(Class5TractorNoEMS_PTOJob, 3)]
		public void TestClass5_Vehicle_RegionalDel_RefLoad(string file, int runIdx)
		{
			var inputData = (IDeclarationInputDataProvider)JSONInputDataFactory.ReadJsonJob(file);
			var dataReader = new DeclarationModeVectoRunDataFactory(inputData, null);
			var runData = dataReader.NextRun().ToArray();

			// regional del., ref load
			DeclarationAdapterTestHelper.AssertVehicleData(runData[runIdx].VehicleData,
				vehicleCategory: VehicleCategory.Tractor,
				vehicleClass: VehicleClass.Class5,
				axleConfiguration: AxleConfiguration.AxleConfig_4x2,
				wheelsInertia: 204.6,
				totalVehicleWeight: CurbWeight + 7500 + 12900,
				totalRollResistance: 0.0059836);
		}
	}
}