using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestPlatform.Utilities;
using Ninject;
using NUnit.Framework;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.OutputData;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Reader;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.FileIO;

namespace TUGraz.VectoCore.Tests.Integration
{
	[TestFixture]
    public class MockUpVectoTest
	{




		private IKernel _vectoKernel;
		private ISimulatorFactoryFactory _simFactoryFactory;
		private ISimulatorFactory _simulatorFactory;
		private IXMLInputDataReader _inputDataReader;

		#region TestFiles
		protected const string ConventionalHeavyLorry =
			@"TestData\XML\XMLReaderDeclaration\SchemaVersion2.4\Distributed\HeavyLorry\Conventional_heavyLorry_AMT.xml";
		protected const string HEV_Px_HeavyLorry =
			@"TestData\XML\XMLReaderDeclaration\SchemaVersion2.4\Distributed\HeavyLorry\HEV_heavyLorry_AMT_Px_IHPC.xml";
		protected const string HEV_S2_HeavyLorry =
			@"TestData\XML\XMLReaderDeclaration\SchemaVersion2.4\Distributed\HeavyLorry\HEV-S_heavyLorry_AMT_S2.xml";
		protected const string HEV_S3_HeavyLorry =
			@"TestData\XML\XMLReaderDeclaration\SchemaVersion2.4\Distributed\HeavyLorry\HEV-S_heavyLorry_S3.xml";
		protected const string HEV_S4_HeavyLorry =
			@"TestData\XML\XMLReaderDeclaration\SchemaVersion2.4\Distributed\HeavyLorry\HEV-S_heavyLorry_S4.xml";
		protected const string HEV_IEPC_S_HeavyLorry =
			@"TestData\XML\XMLReaderDeclaration\SchemaVersion2.4\Distributed\HeavyLorry\HEV-S_heavyLorry_IEPC-S.xml";
		protected const string PEV_E2_HeavyLorry =
			@"TestData\XML\XMLReaderDeclaration\SchemaVersion2.4\Distributed\HeavyLorry\PEV_heavyLorry_AMT_E2.xml";
		protected const string PEV_E3_HeavyLorry =
			@"TestData\XML\XMLReaderDeclaration\SchemaVersion2.4\Distributed\HeavyLorry\PEV_heavyLorry_E3.xml";
		protected const string PEV_E4_HeavyLorry =
			@"TestData\XML\XMLReaderDeclaration\SchemaVersion2.4\Distributed\HeavyLorry\PEV_heavyLorry_E4.xml";
		protected const string PEV_IEPC_HeavyLorry =
			@"TestData\XML\XMLReaderDeclaration\SchemaVersion2.4\Distributed\HeavyLorry\IEPC_heavyLorry.xml";

		protected const string Conventional_PrimaryBus =
			@"TestData\XML\XMLReaderDeclaration\SchemaVersion2.4\Distributed\PrimaryBus\Conventional_primaryBus_AMT.xml";
		protected const string HEV_Px_IHPC_PrimaryBus =
			@"TestData\XML\XMLReaderDeclaration\SchemaVersion2.4\Distributed\PrimaryBus\HEV_primaryBus_AMT_Px.xml";
		protected const string HEV_S2_PrimaryBus =
			@"TestData\XML\XMLReaderDeclaration\SchemaVersion2.4\Distributed\PrimaryBus\HEV-S_primaryBus_AMT_S2.xml";
		protected const string HEV_S3_PrimaryBus =
			@"TestData\XML\XMLReaderDeclaration\SchemaVersion2.4\Distributed\PrimaryBus\HEV-S_primaryBus_S3.xml";
		protected const string HEV_S4_PrimaryBus =
			@"TestData\XML\XMLReaderDeclaration\SchemaVersion2.4\Distributed\PrimaryBus\HEV-S_primaryBus_S4.xml";
		protected const string HEV_IEPC_S_PrimaryBus =
			@"TestData\XML\XMLReaderDeclaration\SchemaVersion2.4\Distributed\PrimaryBus\HEV-S_primaryBus_IEPC-S.xml";
		protected const string PEV_E2_PrimaryBus =
			@"TestData\XML\XMLReaderDeclaration\SchemaVersion2.4\Distributed\PrimaryBus\PEV_primaryBus_AMT_E2.xml";
		protected const string PEV_E3_PrimaryBus =
			@"TestData\XML\XMLReaderDeclaration\SchemaVersion2.4\Distributed\PrimaryBus\PEV_primaryBus_E3.xml";
		protected const string PEV_E4_PrimaryBus =
			@"TestData\XML\XMLReaderDeclaration\SchemaVersion2.4\Distributed\PrimaryBus\PEV_primaryBus_E4.xml";
		protected const string PEV_IEPC_PrimaryBus =
			@"TestData\XML\XMLReaderDeclaration\SchemaVersion2.4\Distributed\PrimaryBus\IEPC_primaryBus.xml";


		protected const string Conventional_CompletedBus = @"TestData\XML\XMLReaderDeclaration\SchemaVersionMultistage.0.1\vecto_multistage_conventional_final_vif.VIF_Report_1.xml";

		#endregion


		[OneTimeSetUp]
		public void OneTimeSetup()
		{
			_vectoKernel = new StandardKernel(
				new VectoNinjectModule()
				);

			_simFactoryFactory = _vectoKernel.Get<ISimulatorFactoryFactory>();
			Assert.NotNull(_simFactoryFactory);
			_inputDataReader = _vectoKernel.Get<IXMLInputDataReader>();
			Assert.NotNull(_inputDataReader);
		}

		public void Setup()
		{
			

		}

		public IOutputDataWriter GetOutputFileWriter(string subDirectory, string originalFilePath)
		{
			var path = Path.Combine(Path.Combine(Path.GetFullPath(subDirectory)), Path.GetFileName(originalFilePath));
			return new FileOutputWriter(path);
		}

		

		

		[TestCase(ConventionalHeavyLorry)]
		public void ConventionalHeavyLorryMockupTest(string fileName)
		{
			var inputProvider = _inputDataReader.Create(fileName);
			var fileWriter = GetOutputFileWriter(nameof(ConventionalHeavyLorryMockupTest), fileName);
			var sumWriter = new SummaryDataContainer(fileWriter);
			var jobContainer = new JobContainer(sumWriter);

			_simulatorFactory =
				_simFactoryFactory.Factory(ExecutionMode.Declaration, inputProvider, fileWriter, null, null, true);
			_simulatorFactory.MockUpRun = true;

			jobContainer.AddRuns(_simulatorFactory);
			jobContainer.Execute(true);
			jobContainer.WaitFinished();




		}










	}
}
