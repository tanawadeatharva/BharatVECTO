using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Ninject;
using NUnit.Framework;
using NUnit.Framework.Internal;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.OutputData.FileIO;
using TUGraz.VectoCore.Tests.Utils;

namespace TUGraz.VectoCore.Tests.Integration.CompletedBus
{
	[TestFixture]
	public class CompletedBusSanityCheckTests
	{
		private IXMLInputDataReader _xmlInputReader;
		public const string CompletedFile32 = @"TestData\Integration\Buses\FactorMethod\vecto_vehicle-completed_heavyBus_41.xml";
		public const string CompletedFile33b1 = @"TestData\Integration\Buses\FactorMethod\CompletedHeavyBus_33b1.RSLT_VIF.xml";
		//public const string CompletedFile33b1 = @"TestData\Integration\Buses\FactorMethod\vecto_vehicle-completed_heavyBus_42.xml";
		public const string  PifFile_33_34 = @"TestData\Integration\Buses\FactorMethod\primary_heavyBus group42_SmartPS.RSLT_PIF.xml";

        [OneTimeSetUp]
		public void RunBeforeAnyTests()
		{
			Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);
			var kernel = new StandardKernel(new VectoNinjectModule());
			_xmlInputReader = kernel.Get<IXMLInputDataReader>();
		}

        [TestCase(CompletedFile33b1, BusHVACSystemConfiguration.Configuration8, false, TestName = "CompletedBus AirDistribution sanitycheck error, grp 33b1 cfg 8/false"),
		TestCase(CompletedFile33b1, BusHVACSystemConfiguration.Configuration9, false, TestName = "CompletedBus AirDistribution sanitycheck error, grp 33b1 cfg 9/false"),
		TestCase(CompletedFile33b1, BusHVACSystemConfiguration.Configuration10, false, TestName = "CompletedBus AirDistribution sanitycheck error, grp 33b1 cfg 10/false"),

		TestCase(CompletedFile32, BusHVACSystemConfiguration.Configuration1, false, TestName = "CompletedBus AirDistribution sanitycheck error, grp 34b cfg 1/false"),
		TestCase(CompletedFile32, BusHVACSystemConfiguration.Configuration2, false, TestName = "CompletedBus AirDistribution sanitycheck error, grp 34b cfg 2/false"),
		TestCase(CompletedFile32, BusHVACSystemConfiguration.Configuration3, false, TestName = "CompletedBus AirDistribution sanitycheck error, grp 34b cfg 3/false"),
		TestCase(CompletedFile32, BusHVACSystemConfiguration.Configuration4, false, TestName = "CompletedBus AirDistribution sanitycheck error, grp 34b cfg 4/false"),
		TestCase(CompletedFile32, BusHVACSystemConfiguration.Configuration5, false, TestName = "CompletedBus AirDistribution sanitycheck error, grp 34b cfg 5/false"),
		TestCase(CompletedFile32, BusHVACSystemConfiguration.Configuration6, false, TestName = "CompletedBus AirDistribution sanitycheck error, grp 34b cfg 6/false"),
		TestCase(CompletedFile32, BusHVACSystemConfiguration.Configuration7, false, TestName = "CompletedBus AirDistribution sanitycheck error, grp 34b cfg 7/false"),
		TestCase(CompletedFile32, BusHVACSystemConfiguration.Configuration8, false, TestName = "CompletedBus AirDistribution sanitycheck error, grp 34b cfg 8/false"),
		TestCase(CompletedFile32, BusHVACSystemConfiguration.Configuration9, false, TestName = "CompletedBus AirDistribution sanitycheck error, grp 34b cfg 9/false"),
		TestCase(CompletedFile32, BusHVACSystemConfiguration.Configuration10, false, TestName = "CompletedBus AirDistribution sanitycheck error, grp 34b cfg 10/false"),
        ]
        public void TestSeparateAirDistributionDuctsError(string completedJob, BusHVACSystemConfiguration hvacConfig, bool separateDucts)
		{
			var modified = GetModifiedXML(completedJob, hvacConfig, separateDucts);

			var writer = new FileOutputWriter("SanityCheckTest");
            //var inputData = new MockCompletedBusInputData(XmlReader.Create(PifFile_33_34), modified);
			var inputData = _xmlInputReader.CreateDeclaration(modified);

			var factory = new SimulatorFactory(ExecutionMode.Declaration, inputData, writer)
			{
				WriteModalResults = true,
				Validate = false
			};

			AssertHelper.Exception<VectoException>(() => {
				var runs = factory.DataReader.NextRun().ToList();}, messageContains: "Input parameter 'separate air distribution ducts' has to be set to 'true' for vehicle group ");
        }

        [
		TestCase(CompletedFile33b1, BusHVACSystemConfiguration.Configuration8, true, TestName = "CompletedBus AirDistribution sanitycheck OK, grp 33b1 cfg 8/true"),
		TestCase(CompletedFile33b1, BusHVACSystemConfiguration.Configuration9, true, TestName = "CompletedBus AirDistribution sanitycheck OK, grp 33b1 cfg 9/true"),
		TestCase(CompletedFile33b1, BusHVACSystemConfiguration.Configuration10, true, TestName = "CompletedBus AirDistribution sanitycheck OK, grp 33b1 cfg 10/true"),

		TestCase(CompletedFile32, BusHVACSystemConfiguration.Configuration1, true, TestName = "CompletedBus AirDistribution sanitycheck OK, grp 34b cfg 1/true"),
		TestCase(CompletedFile32, BusHVACSystemConfiguration.Configuration2, true, TestName = "CompletedBus AirDistribution sanitycheck OK, grp 34b cfg 2/true"),
		TestCase(CompletedFile32, BusHVACSystemConfiguration.Configuration3, true, TestName = "CompletedBus AirDistribution sanitycheck OK, grp 34b cfg 3/true"),
		TestCase(CompletedFile32, BusHVACSystemConfiguration.Configuration4, true, TestName = "CompletedBus AirDistribution sanitycheck OK, grp 34b cfg 4/true"),
		TestCase(CompletedFile32, BusHVACSystemConfiguration.Configuration5, true, TestName = "CompletedBus AirDistribution sanitycheck OK, grp 34b cfg 5/true"),
		TestCase(CompletedFile32, BusHVACSystemConfiguration.Configuration6, true, TestName = "CompletedBus AirDistribution sanitycheck OK, grp 34b cfg 6/true"),
		TestCase(CompletedFile32, BusHVACSystemConfiguration.Configuration7, true, TestName = "CompletedBus AirDistribution sanitycheck OK, grp 34b cfg 7/true"),
		TestCase(CompletedFile32, BusHVACSystemConfiguration.Configuration8, true, TestName = "CompletedBus AirDistribution sanitycheck OK, grp 34b cfg 8/true"),
		TestCase(CompletedFile32, BusHVACSystemConfiguration.Configuration9, true, TestName = "CompletedBus AirDistribution sanitycheck OK, grp 34b cfg 9/true"),
		TestCase(CompletedFile32, BusHVACSystemConfiguration.Configuration10, true, TestName = "CompletedBus AirDistribution sanitycheck OK, grp 34b cfg 10/true"),

		TestCase(CompletedFile33b1, BusHVACSystemConfiguration.Configuration1, false, TestName = "CompletedBus AirDistribution sanitycheck OK, grp 33b1 cfg 1/false"),
		TestCase(CompletedFile33b1, BusHVACSystemConfiguration.Configuration2, false, TestName = "CompletedBus AirDistribution sanitycheck OK, grp 33b1 cfg 2/false"),
		TestCase(CompletedFile33b1, BusHVACSystemConfiguration.Configuration3, false, TestName = "CompletedBus AirDistribution sanitycheck OK, grp 33b1 cfg 3/false"),
		TestCase(CompletedFile33b1, BusHVACSystemConfiguration.Configuration4, false, TestName = "CompletedBus AirDistribution sanitycheck OK, grp 33b1 cfg 4/false"),
		TestCase(CompletedFile33b1, BusHVACSystemConfiguration.Configuration5, false, TestName = "CompletedBus AirDistribution sanitycheck OK, grp 33b1 cfg 5/false"),
		TestCase(CompletedFile33b1, BusHVACSystemConfiguration.Configuration6, false, TestName = "CompletedBus AirDistribution sanitycheck OK, grp 33b1 cfg 6/false"),
		TestCase(CompletedFile33b1, BusHVACSystemConfiguration.Configuration7, false, TestName = "CompletedBus AirDistribution sanitycheck OK, grp 33b1 cfg 7/false"),

		TestCase(CompletedFile33b1, BusHVACSystemConfiguration.Configuration1, true, TestName = "CompletedBus AirDistribution sanitycheck OK, grp 33b1 cfg 1/true"),
		TestCase(CompletedFile33b1, BusHVACSystemConfiguration.Configuration2, true, TestName = "CompletedBus AirDistribution sanitycheck OK, grp 33b1 cfg 2/true"),
		TestCase(CompletedFile33b1, BusHVACSystemConfiguration.Configuration3, true, TestName = "CompletedBus AirDistribution sanitycheck OK, grp 33b1 cfg 3/true"),
		TestCase(CompletedFile33b1, BusHVACSystemConfiguration.Configuration4, true, TestName = "CompletedBus AirDistribution sanitycheck OK, grp 33b1 cfg 4/true"),
		TestCase(CompletedFile33b1, BusHVACSystemConfiguration.Configuration5, true, TestName = "CompletedBus AirDistribution sanitycheck OK, grp 33b1 cfg 5/true"),
		TestCase(CompletedFile33b1, BusHVACSystemConfiguration.Configuration6, true, TestName = "CompletedBus AirDistribution sanitycheck OK, grp 33b1 cfg 6/true"),
		TestCase(CompletedFile33b1, BusHVACSystemConfiguration.Configuration7, true, TestName = "CompletedBus AirDistribution sanitycheck OK, grp 33b1 cfg 7/true"),
		]
        public void TestSeparateAirDistributionDuctsOk(string completedJob, BusHVACSystemConfiguration hvacConfig, bool separateDucts)
        {
            var modified = GetModifiedXML(completedJob, hvacConfig, separateDucts);

            var writer = new FileOutputWriter("SanityCheckTest");
            //var inputData = new MockCompletedBusInputData(XmlReader.Create(PifFile_33_34), modified);
			var inputData = new MockCompletedBusInputData(modified);

			var factory = new SimulatorFactory(ExecutionMode.Declaration, inputData, writer)
            {
                WriteModalResults = true,
                Validate = false
            };

            //AssertHelper.Exception<VectoException>(() => {
                var runs = factory.DataReader.NextRun().ToList();
            //}, messageContains: "Input parameter 'separate air distribution ducts' has to be set to 'true' for vehicle group ");
        }

        private static XmlReader GetModifiedXML(string completedJob, BusHVACSystemConfiguration? hvacConfig, bool separateDucts)
		{
			var completedXML = new XmlDocument();
			completedXML.Load(completedJob);

			var hvacCfgNode = completedXML.SelectSingleNode("//*[local-name()='SystemConfiguration']");
			hvacCfgNode.InnerText = hvacConfig.GetXmlFormat();

			var airDuctsNode = completedXML.SelectSingleNode("//*[local-name()='SeparateAirDistributionDucts']");
			airDuctsNode.InnerText = XmlConvert.ToString(separateDucts);
			var modified = XmlReader.Create(new StringReader(completedXML.OuterXml));
			return modified;
		}
	}

	public class MockCompletedBusInputData : IInputDataProvider, IMultistageBusInputDataProvider
	{
		private IMultistageBusInputDataProvider input;
	
		public MockCompletedBusInputData(XmlReader vif)
		{
			var kernel = new StandardKernel(new VectoNinjectModule());
			var _xmlInputReader = kernel.Get<IXMLInputDataReader>();


			input = _xmlInputReader.CreateDeclaration(vif) as IMultistageBusInputDataProvider;
			
			//JobName = Vehicle.VIN;
		}

		public IDeclarationMultistageJobInputData JobInputData
		{
			get { return input.JobInputData; }
		}


		IDeclarationJobInputData IDeclarationInputDataProvider.JobInputData => null;

		public IPrimaryVehicleInformationInputDataProvider PrimaryVehicleData
		{
			get { return input.PrimaryVehicleData; }
		}
        public XElement XMLHash { get; }

		public bool SavedInDeclarationMode
		{
			get { return true; }
		}

		public IPrimaryVehicleInformationInputDataProvider PrimaryVehicle
		{
			get { return PrimaryVehicleData; }
		}
        public IList<IManufacturingStageInputData> ManufacturingStages
		{
			get { return input.JobInputData.ManufacturingStages; }
		}
        public IManufacturingStageInputData ConsolidateManufacturingStage
		{
			get { return input.JobInputData.ConsolidateManufacturingStage; }
		}

		public VectoSimulationJobType JobType
		{
			get { return VectoSimulationJobType.ConventionalVehicle; }
		}

		public bool InputComplete { get; }

		public DataSource DataSource { get; }
	}
}