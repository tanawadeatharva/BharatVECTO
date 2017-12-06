using System.IO;
using NUnit.Framework;
using NUnit.Framework.Internal;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.Tests.Utils;
using TUGraz.VECTO;

namespace TUGraz.VectoCore.Tests.FileIO
{
	[TestFixture]
	public class JsonWriteTest
	{
		[OneTimeSetUp]
		public void RunBeforeAnyTests()
		{
			Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);
		}

		[TestCase(@"TestData\Generic Vehicles\Declaration Mode\Class9_RigidTruck_6x2\Class9_RigidTruck.vveh")]
		public void SaveVehicleFileDecl(string vehicleFile)
		{
			var outFile = "json_vehicle_write_test.vveh";

			var input = JSONInputDataFactory.ReadComponentData(vehicleFile);
			var inputProvider = input as IEngineeringInputDataProvider;
			Assert.NotNull(inputProvider);

			var vehicleInput = inputProvider.JobInputData.Vehicle;

			var writer = JSONFileWriter.Instance;

			VECTO_Global.Cfg = new VECTO.Configuration() {DeclMode = true};
			writer.SaveVehicle(vehicleInput, vehicleInput.AirdragInputData, vehicleInput.RetarderInputData, vehicleInput.PTOTransmissionInputData, vehicleInput.AngledriveInputData, outFile);

			var savedData = JSONInputDataFactory.ReadComponentData(outFile);
			var savedInprovider = savedData as IEngineeringInputDataProvider;
			Assert.NotNull(savedInprovider);
			//Assert.AreEqual(vehicleInput, savedInprovider.JobInputData.Vehicle);

			AssertHelper.PublicPropertiesEqual(typeof(IVehicleDeclarationInputData),vehicleInput, savedInprovider.JobInputData.Vehicle, 
				new [] {"Source", "GearboxInputData", "EngineInputData", "TorqueConverterInputData", "AxleGearInputData"});
		}
		

		[TestCase(@"TestData\Generic Vehicles\Declaration Mode\Class9_RigidTruck_6x2\Engine_324kW_12.7l.veng")]
		public void SaveEngineFileDecl(string engineFile)
		{
			var outFile = "json_engine_write_test.veng";

			var input = JSONInputDataFactory.ReadComponentData(engineFile);
			var inputProvider = input as IEngineeringInputDataProvider;
			Assert.NotNull(inputProvider);

			var engineInputData = inputProvider.JobInputData.Vehicle.EngineInputData;

			var writer = JSONFileWriter.Instance;

			VECTO_Global.Cfg = new VECTO.Configuration() { DeclMode = true };
			writer.SaveEngine(engineInputData,outFile);

			var savedData = JSONInputDataFactory.ReadComponentData(outFile);
			var savedInprovider = savedData as IEngineeringInputDataProvider;
			Assert.NotNull(savedInprovider);

			AssertHelper.PublicPropertiesEqual(typeof(IEngineDeclarationInputData),engineInputData, savedInprovider.JobInputData.Vehicle.EngineInputData,
				new[] { "Source" });
		}

		[TestCase(@"TestData\Generic Vehicles\Declaration Mode\Class9_RigidTruck_6x2\AMT_12.vgbx")]
		public void SaveGearboxAxlegearFileDecl(string engineFile)
		{
			var outFile = "json_gearbox_write_test.vgbx";

			var input = JSONInputDataFactory.ReadComponentData(engineFile);
			var inputProvider = input as IEngineeringInputDataProvider;
			Assert.NotNull(inputProvider);

			var vehicleInputData = inputProvider.JobInputData.Vehicle;

			var writer = JSONFileWriter.Instance;

			VECTO_Global.Cfg = new VECTO.Configuration() { DeclMode = true };
			writer.SaveGearbox(vehicleInputData.GearboxInputData, vehicleInputData.AxleGearInputData, outFile);

			var savedData = JSONInputDataFactory.ReadComponentData(outFile);
			var savedInprovider = savedData as IEngineeringInputDataProvider;
			Assert.NotNull(savedInprovider);

			AssertHelper.PublicPropertiesEqual(typeof(IGearboxDeclarationInputData), vehicleInputData.GearboxInputData, savedInprovider.JobInputData.Vehicle.GearboxInputData,
				new[] { "Source" });
			AssertHelper.PublicPropertiesEqual(typeof(IAxleDeclarationInputData), vehicleInputData.AxleGearInputData, savedInprovider.JobInputData.Vehicle.AxleGearInputData,
				new[] { "Source" });
		}


	}
}