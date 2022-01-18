using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ninject;
using NUnit.Framework;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.FileIO;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.LorryManufacturerReport;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportXMLTypeWriter;
using TUGraz.VectoCore.Tests.Integration.CompletedBus;
using TUGraz.VectoCore.Tests.Models.Simulation;

namespace TUGraz.VectoCore.Tests.XML.Reports
{
    [TestFixture]
    public class ManufacturerReportWriterTest
	{


		private ISimulatorFactory _simulatorFactory;

		private IOutputDataWriter _outputWriter;
		private StandardKernel _kernel;
		private IXMLInputDataReader _xmlReader;
		private IManufacturerReportFactory _mrfFactory;

		//private ISimulatorFactory CreateSimulatorFactory(IInputDataProvider dataProvider, IOutputDataWriter writer, bool validate = true)
		//{
		//	return new SimulatorFactory(mode: ExecutionMode.Declaration, dataProvider: dataProvider, writer: writer,
		//		validate: validate);
		//}


		//private async Task RunSimulation(ISimulatorFactory simulatorFactory)
		//{
		//	var jobContainer = new JobContainer(new MockSumWriter());
		//	jobContainer.AddRuns(simulatorFactory);

		//	jobContainer.Execute(true);
		//	Assert.DoesNotThrow(() => jobContainer.WaitFinished(), "Error during Simulation");
		//}

		//private async Task StartSimulation(string jobFile, bool validate = true)
		//{
		//	Assert.IsTrue(File.Exists(jobFile), $"File {jobFile} not found");
		//	IOutputDataWriter writer = new FileOutputWriter(jobFile);
		//	IInputDataProvider dataProvider = _xmlReader.CreateDeclaration(jobFile);
		//	var simFactory = CreateSimulatorFactory(dataProvider, writer, validate);
		//	await RunSimulation(simFactory);
		//}

		[OneTimeSetUp]
		public void OneTimeSetup()
		{
			_kernel = new StandardKernel(
				new VectoNinjectModule()
			);

		}

		[SetUp]
		public void SetUp()
		{
			//_outputWriter = new FileOutputWriter()
			_xmlReader = _kernel.Get<IXMLInputDataReader>();
			_mrfFactory = _kernel.Get<IManufacturerReportFactory>();
		}




		[TestCase(@"TestData\XML\XMLReaderDeclaration\SchemaVersion2.10\Distributed\HeavyLorry\Conventional_heavyLorry_AMT.xml")]
	//	[TestCase(@"TestData\XML\XMLReaderDeclaration\SchemaVersion2.10\Distributed\HeavyLorry\HEV-S_heavyLorry_S3.xml")]
	//	[TestCase(@"TestData\XML\XMLReaderDeclaration\SchemaVersion2.10\Distributed\HeavyLorry\HEV-S_heavyLorry_AMT_S2.xml")]
	//	[TestCase(@"TestData\XML\XMLReaderDeclaration\SchemaVersion2.10\Distributed\HeavyLorry\HEV-S_heavyLorry_IEPC-S.xml")]
	//	[TestCase(@"TestData\XML\XMLReaderDeclaration\SchemaVersion2.10\Distributed\HeavyLorry\HEV-S_heavyLorry_S3.xml")]
	//	[TestCase(@"TestData\XML\XMLReaderDeclaration\SchemaVersion2.10\Distributed\HeavyLorry\HEV-S_heavyLorry_S4.xml")]
	//	[TestCase(@"TestData\XML\XMLReaderDeclaration\SchemaVersion2.10\Distributed\HeavyLorry\HEV_heavyLorry_AMT_Px.xml")]
	//	[TestCase(@"TestData\XML\XMLReaderDeclaration\SchemaVersion2.10\Distributed\HeavyLorry\HEV_heavyLorry_AMT_Px_IHPC.xml")]
	//	[TestCase(@"TestData\XML\XMLReaderDeclaration\SchemaVersion2.10\Distributed\HeavyLorry\IEPC_heavyLorry.xml")]
	//	[TestCase(@"TestData\XML\XMLReaderDeclaration\SchemaVersion2.10\Distributed\HeavyLorry\PEV_heavyLorry_AMT_E2.xml")]
	//	[TestCase(@"TestData\XML\XMLReaderDeclaration\SchemaVersion2.10\Distributed\HeavyLorry\PEV_heavyLorry_APT-N_E2.xml")]
	//	[TestCase(@"TestData\XML\XMLReaderDeclaration\SchemaVersion2.10\Distributed\HeavyLorry\PEV_heavyLorry_E3.xml")]
	//	[TestCase(@"TestData\Integration\DeclarationMode\Class4_Vocational\Rigid Truck_4x2_vehicle-class-4_EURO6_2018.xml")]
		public async Task ConventionalLorryMRFTest(string fileName)
		{
			Assert.IsFalse(string.IsNullOrEmpty(fileName));
			IDeclarationInputDataProvider dataProvider = _xmlReader.CreateDeclaration(fileName);

			var arch = dataProvider.JobInputData.Vehicle.ArchitectureID;

            dataProvider.JobInputData.Vehicle.VehicleCategory.GetVehicleType();// HEV/PEV - Sx/Px
			var ihpc = (dataProvider.JobInputData.Vehicle.Components.ElectricMachines?.Entries)?.Count(electric => electric.ElectricMachine.IHPCType != "None") > 0;
			var iepc = (dataProvider.JobInputData.Vehicle.Components.IEPC != null);
			var report = _mrfFactory.GetManufacturerReport(
				dataProvider.JobInputData.Vehicle.VehicleCategory.GetVehicleType(),
				dataProvider.JobInputData.JobType,
				dataProvider.JobInputData.Vehicle.ArchitectureID,
				dataProvider.JobInputData.Vehicle.ExemptedVehicle, 
				iepc,
				ihpc) as ConventionalLorryManufacturerReport;
			Assert.NotNull(report);
			report.InitializeVehicleData(dataProvider);
            TestContext.WriteLine(report.Vehicle);
		}

		[TestCase(@"TestData\XML\XMLReaderDeclaration\SchemaVersion2.10\Distributed\HeavyLorry\HEV_heavyLorry_AMT_Px_IHPC.xml")]
		public async Task HEV_Px_LorryMRFTest(string fileName)
		{
			Assert.IsFalse(string.IsNullOrEmpty(fileName));
			IDeclarationInputDataProvider dataProvider = _xmlReader.CreateDeclaration(fileName);

			var arch = dataProvider.JobInputData.Vehicle.ArchitectureID;

			dataProvider.JobInputData.Vehicle.VehicleCategory.GetVehicleType();// HEV/PEV - Sx/Px
			var ihpc = (dataProvider.JobInputData.Vehicle.Components.ElectricMachines?.Entries)?.Count(electric => electric.ElectricMachine.IHPCType != "None") > 0;
			var iepc = (dataProvider.JobInputData.Vehicle.Components.IEPC != null);
			var report = _mrfFactory.GetManufacturerReport(
				dataProvider.JobInputData.Vehicle.VehicleCategory.GetVehicleType(),
				dataProvider.JobInputData.JobType,
				dataProvider.JobInputData.Vehicle.ArchitectureID,
				dataProvider.JobInputData.Vehicle.ExemptedVehicle,
				iepc,
				ihpc) as HEV_Px_IHPC_LorryManufacturerReport;
			Assert.NotNull(report);
			report.InitializeVehicleData(dataProvider);
			TestContext.WriteLine(report.Vehicle);
		}

		[TestCase(@"TestData\XML\XMLReaderDeclaration\SchemaVersion2.10\Distributed\HeavyLorry\HEV-S_heavyLorry_AMT_S2.xml")]
		public async Task HEV_S2_LorryMRFTest(string fileName)
		{
			Assert.IsFalse(string.IsNullOrEmpty(fileName));
			IDeclarationInputDataProvider dataProvider = _xmlReader.CreateDeclaration(fileName);

			var arch = dataProvider.JobInputData.Vehicle.ArchitectureID;

			dataProvider.JobInputData.Vehicle.VehicleCategory.GetVehicleType();// HEV/PEV - Sx/Px
			var ihpc = (dataProvider.JobInputData.Vehicle.Components.ElectricMachines?.Entries)?.Count(electric => electric.ElectricMachine.IHPCType != "None") > 0;
			var iepc = (dataProvider.JobInputData.Vehicle.Components.IEPC != null);
			var report = _mrfFactory.GetManufacturerReport(
				dataProvider.JobInputData.Vehicle.VehicleCategory.GetVehicleType(),
				dataProvider.JobInputData.JobType,
				dataProvider.JobInputData.Vehicle.ArchitectureID,
				dataProvider.JobInputData.Vehicle.ExemptedVehicle,
				iepc,
				ihpc) as HEV_S2_LorryManufacturerReport;
			Assert.NotNull(report);
			report.InitializeVehicleData(dataProvider);
			TestContext.WriteLine(report.Vehicle);
		}

		[TestCase(@"TestData\XML\XMLReaderDeclaration\SchemaVersion2.10\Distributed\HeavyLorry\HEV-S_heavyLorry_S3.xml")]
		public async Task HEV_S3_LorryMRFTest(string fileName)
		{
			Assert.IsFalse(string.IsNullOrEmpty(fileName));
			IDeclarationInputDataProvider dataProvider = _xmlReader.CreateDeclaration(fileName);

			var arch = dataProvider.JobInputData.Vehicle.ArchitectureID;

			dataProvider.JobInputData.Vehicle.VehicleCategory.GetVehicleType();// HEV/PEV - Sx/Px
			var ihpc = (dataProvider.JobInputData.Vehicle.Components.ElectricMachines?.Entries)?.Count(electric => electric.ElectricMachine.IHPCType != "None") > 0;
			var iepc = (dataProvider.JobInputData.Vehicle.Components.IEPC != null);
			var report = _mrfFactory.GetManufacturerReport(
				dataProvider.JobInputData.Vehicle.VehicleCategory.GetVehicleType(),
				dataProvider.JobInputData.JobType,
				dataProvider.JobInputData.Vehicle.ArchitectureID,
				dataProvider.JobInputData.Vehicle.ExemptedVehicle,
				iepc,
				ihpc) as HEV_S3_LorryManufacturerReport;
			Assert.NotNull(report);
			report.InitializeVehicleData(dataProvider);
			TestContext.WriteLine(report.Vehicle);
		}

		[TestCase(@"TestData\XML\XMLReaderDeclaration\SchemaVersion2.10\Distributed\HeavyLorry\HEV-S_heavyLorry_S4.xml")]
		public async Task HEV_S4_LorryMRFTest(string fileName)
		{
			Assert.IsFalse(string.IsNullOrEmpty(fileName));
			IDeclarationInputDataProvider dataProvider = _xmlReader.CreateDeclaration(fileName);

			var arch = dataProvider.JobInputData.Vehicle.ArchitectureID;

			dataProvider.JobInputData.Vehicle.VehicleCategory.GetVehicleType();// HEV/PEV - Sx/Px
			var ihpc = (dataProvider.JobInputData.Vehicle.Components.ElectricMachines?.Entries)?.Count(electric => electric.ElectricMachine.IHPCType != "None") > 0;
			var iepc = (dataProvider.JobInputData.Vehicle.Components.IEPC != null);
			var report = _mrfFactory.GetManufacturerReport(
				dataProvider.JobInputData.Vehicle.VehicleCategory.GetVehicleType(),
				dataProvider.JobInputData.JobType,
				dataProvider.JobInputData.Vehicle.ArchitectureID,
				dataProvider.JobInputData.Vehicle.ExemptedVehicle,
				iepc,
				ihpc) as HEV_S4_LorryManufacturerReport;
			Assert.NotNull(report);
			report.InitializeVehicleData(dataProvider);
			TestContext.WriteLine(report.Vehicle);
		}

		[TestCase(@"TestData\XML\XMLReaderDeclaration\SchemaVersion2.10\Distributed\HeavyLorry\HEV-S_heavyLorry_IEPC-S.xml")]
		public async Task HEV_IEPC_S_LorryMRFTest(string fileName)
		{
			Assert.IsFalse(string.IsNullOrEmpty(fileName));
			IDeclarationInputDataProvider dataProvider = _xmlReader.CreateDeclaration(fileName);

			var arch = dataProvider.JobInputData.Vehicle.ArchitectureID;

			dataProvider.JobInputData.Vehicle.VehicleCategory.GetVehicleType();// HEV/PEV - Sx/Px
			var ihpc = (dataProvider.JobInputData.Vehicle.Components.ElectricMachines?.Entries)?.Count(electric => electric.ElectricMachine.IHPCType != "None") > 0;
			var iepc = (dataProvider.JobInputData.Vehicle.Components.IEPC != null);
			var report = _mrfFactory.GetManufacturerReport(
				dataProvider.JobInputData.Vehicle.VehicleCategory.GetVehicleType(),
				dataProvider.JobInputData.JobType,
				dataProvider.JobInputData.Vehicle.ArchitectureID,
				dataProvider.JobInputData.Vehicle.ExemptedVehicle,
				iepc,
				ihpc);
			var concreteReport = report as HEV_IEPC_S_LorryManufacturerReport;
			Assert.NotNull(concreteReport);
			report.InitializeVehicleData(dataProvider);
			TestContext.WriteLine(concreteReport.Vehicle);
		}

		[TestCase(@"TestData\XML\XMLReaderDeclaration\SchemaVersion2.10\Distributed\HeavyLorry\PEV_heavyLorry_AMT_E2.xml")]
		public async Task PEV_E2_LorryMRFTest(string fileName)
		{
			Assert.IsFalse(string.IsNullOrEmpty(fileName));
			IDeclarationInputDataProvider dataProvider = _xmlReader.CreateDeclaration(fileName);

			var arch = dataProvider.JobInputData.Vehicle.ArchitectureID;

			dataProvider.JobInputData.Vehicle.VehicleCategory.GetVehicleType();// HEV/PEV - Sx/Px
			var ihpc = (dataProvider.JobInputData.Vehicle.Components.ElectricMachines?.Entries)?.Count(electric => electric.ElectricMachine.IHPCType != "None") > 0;
			var iepc = (dataProvider.JobInputData.Vehicle.Components.IEPC != null);
			var report = _mrfFactory.GetManufacturerReport(
				dataProvider.JobInputData.Vehicle.VehicleCategory.GetVehicleType(),
				dataProvider.JobInputData.JobType,
				dataProvider.JobInputData.Vehicle.ArchitectureID,
				dataProvider.JobInputData.Vehicle.ExemptedVehicle,
				iepc,
				ihpc) as PEV_E2_LorryManufacturerReport;
			Assert.NotNull(report);
			report.InitializeVehicleData(dataProvider);
			TestContext.WriteLine(report.Vehicle);
		}

		[TestCase(@"TestData\XML\XMLReaderDeclaration\SchemaVersion2.10\Distributed\HeavyLorry\PEV_heavyLorry_E3.xml")]
		public async Task PEV_E3_LorryMRFTest(string fileName)
		{
			Assert.IsFalse(string.IsNullOrEmpty(fileName));
			IDeclarationInputDataProvider dataProvider = _xmlReader.CreateDeclaration(fileName);

			var arch = dataProvider.JobInputData.Vehicle.ArchitectureID;

			dataProvider.JobInputData.Vehicle.VehicleCategory.GetVehicleType();// HEV/PEV - Sx/Px
			var ihpc = (dataProvider.JobInputData.Vehicle.Components.ElectricMachines?.Entries)?.Count(electric => electric.ElectricMachine.IHPCType != "None") > 0;
			var iepc = (dataProvider.JobInputData.Vehicle.Components.IEPC != null);
			var report = _mrfFactory.GetManufacturerReport(
				dataProvider.JobInputData.Vehicle.VehicleCategory.GetVehicleType(),
				dataProvider.JobInputData.JobType,
				dataProvider.JobInputData.Vehicle.ArchitectureID,
				dataProvider.JobInputData.Vehicle.ExemptedVehicle,
				iepc,
				ihpc) as PEV_E3_LorryManufacturerReport;
			Assert.NotNull(report);
			report.InitializeVehicleData(dataProvider);
			TestContext.WriteLine(report.Vehicle);
		}

		[TestCase(@"TestData\XML\XMLReaderDeclaration\SchemaVersion2.10\Distributed\HeavyLorry\PEV_heavyLorry_E4.xml")]
		public async Task PEV_E4_LorryMRFTest(string fileName)
		{
			Assert.IsFalse(string.IsNullOrEmpty(fileName));
			IDeclarationInputDataProvider dataProvider = _xmlReader.CreateDeclaration(fileName);

			var arch = dataProvider.JobInputData.Vehicle.ArchitectureID;

			dataProvider.JobInputData.Vehicle.VehicleCategory.GetVehicleType();// HEV/PEV - Sx/Px
			var ihpc = (dataProvider.JobInputData.Vehicle.Components.ElectricMachines?.Entries)?.Count(electric => electric.ElectricMachine.IHPCType != "None") > 0;
			var iepc = (dataProvider.JobInputData.Vehicle.Components.IEPC != null);
			var report = _mrfFactory.GetManufacturerReport(
				dataProvider.JobInputData.Vehicle.VehicleCategory.GetVehicleType(),
				dataProvider.JobInputData.JobType,
				dataProvider.JobInputData.Vehicle.ArchitectureID,
				dataProvider.JobInputData.Vehicle.ExemptedVehicle,
				iepc,
				ihpc) as PEV_E4_LorryManufacturerReport;
			Assert.NotNull(report);
			report.InitializeVehicleData(dataProvider);
			TestContext.WriteLine(report.Vehicle);
		}

		[TestCase(@"TestData\XML\XMLReaderDeclaration\SchemaVersion2.10\Distributed\PrimaryBus\Conventional_primaryBus_AMT.xml")]
		//[TestCase(@"TestData\XML\XMLReaderDeclaration\SchemaVersion2.10\Distributed\PrimaryBus\HEV-S_primaryBus_AMT_S2.xml")]
		public void ConventionalPrimaryBusTest(string fileName)
		{
			Assert.IsFalse(string.IsNullOrEmpty(fileName));
			IDeclarationInputDataProvider dataProvider = _xmlReader.CreateDeclaration(fileName);

			var arch = dataProvider.JobInputData.Vehicle.ArchitectureID;

			dataProvider.JobInputData.Vehicle.VehicleCategory.GetVehicleType();// HEV/PEV - Sx/Px
			var ihpc = (dataProvider.JobInputData.Vehicle.Components.ElectricMachines?.Entries)?.Count(electric => electric.ElectricMachine.IHPCType != "None") > 0;
			var iepc = (dataProvider.JobInputData.Vehicle.Components.IEPC != null);
			var report = _mrfFactory.GetManufacturerReport(
				dataProvider.JobInputData.Vehicle.VehicleCategory.GetVehicleType(),
				dataProvider.JobInputData.JobType,
				dataProvider.JobInputData.Vehicle.ArchitectureID,
				dataProvider.JobInputData.Vehicle.ExemptedVehicle,
				iepc,
				ihpc) as Conventional_PrimaryBus_ManufacturerReport;
			Assert.NotNull(report);
			report.InitializeVehicleData(dataProvider);
			TestContext.WriteLine(report.Vehicle);
		}

		


		[TestCase]
		public void MRFFactoryTest()
		{
			Assert.IsTrue(_mrfFactory.GetConventionalLorryManufacturerReport().GetType() == typeof(ConventionalLorryManufacturerReport));
		}

		[TestCase(@"TestData\XML\XMLReaderDeclaration\SchemaVersion2.10\Distributed\PrimaryBus\Conventional_primaryBus_AMT.xml")]
		public async Task ConventionalPrimaryBusMRFTest(string fileName)
		{
			//await StartSimulation(fileName);
		}

		[TestCase(@"TestData\XML\XMLReaderDeclaration\SchemaVersion2.10\Distributed\ExemptedVehicles\exempted_completedBus_input_full.xml")]
		public async Task ExemptedHeavyLorryMRFTest(string fileName)
		{
			//await StartSimulation(fileName);
		}
		[TearDown]
		public void TearDown()
		{

		}

    }


}
