using System.IO;
using System.Xml;
using System.Xml.XPath;
using NUnit.Framework;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.FileIO;
using TUGraz.VectoCore.OutputData.XML;


namespace TUGraz.VectoCore.Tests.Integration
{
	[TestFixture]
	public class XMLReportTest
	{
		[OneTimeSetUp]
		public void RunBeforeAnyTests()
		{
			Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);
		}

		[TestCase]
		public void TestXMLReportMetaInformation()
		{
			var jobfile = @"Testdata\XML\XMLReaderDeclaration\vecto_vehicle-sample.xml";
			var dataProvider = new XMLDeclarationInputDataProvider(XmlReader.Create(jobfile), true);
			var writer = new FileOutputWriter(jobfile);
			var xmlReport = new XMLDeclarationReport(writer);
			var sumData = new SummaryDataContainer(writer);
			var jobContainer = new JobContainer(sumData);

			if (File.Exists(writer.SumFileName)) {
				File.Delete(writer.SumFileName);
			}

			var runsFactory = new SimulatorFactory(ExecutionMode.Declaration, dataProvider, writer, xmlReport) {
				WriteModalResults = false,
				Validate = false,
			};
			jobContainer.AddRuns(runsFactory);
			jobContainer.Execute();
			jobContainer.WaitFinished();

			var manufacturerReport = xmlReport.FullReport;

			Assert.AreEqual(2, manufacturerReport.XPathSelectElement("//*[local-name()='VehicleGroup']").Value.ToInt());

			//var sumRow = sumData.Table.Rows[1];
			//Assert.AreEqual(dataProvider.VehicleInputData.Manufacturer, sumRow[SummaryDataContainer.VEHICLE_MANUFACTURER]);
			//Assert.AreEqual(dataProvider.VehicleInputData.Model, sumRow[SummaryDataContainer.VEHICLE_MODEL]);
			//Assert.AreEqual(dataProvider.VehicleInputData.VIN, sumRow[SummaryDataContainer.VIN_NUMBER]);
			//Assert.AreEqual(dataProvider.EngineInputData.Manufacturer, sumRow[SummaryDataContainer.ENGINE_MANUFACTURER]);
			//Assert.AreEqual(dataProvider.EngineInputData.Model, sumRow[SummaryDataContainer.ENGINE_MODEL]);
			//Assert.AreEqual(dataProvider.EngineInputData.FuelType.GetLabel(), sumRow[SummaryDataContainer.ENGINE_FUEL_TYPE]);
			//Assert.AreEqual((dataProvider.EngineInputData.RatedPowerDeclared.ConvertTo().Kilo.Watt.Value()),
			//	((SI)sumRow[SummaryDataContainer.ENGINE_RATED_POWER]).Value());
			//Assert.AreEqual(dataProvider.EngineInputData.RatedSpeedDeclared.AsRPM,
			//	((SI)sumRow[SummaryDataContainer.ENGINE_RATED_SPEED]).Value());
			//Assert.AreEqual(dataProvider.EngineInputData.Displacement.ConvertTo().Cubic.Centi.Meter.Value(),
			//	((SI)sumRow[SummaryDataContainer.ENGINE_DISPLACEMENT]).Value());
			//Assert.AreEqual(dataProvider.GearboxInputData.Manufacturer, sumRow[SummaryDataContainer.GEARBOX_MANUFACTURER]);
			//Assert.AreEqual(dataProvider.GearboxInputData.Model, sumRow[SummaryDataContainer.GEARBOX_MODEL]);
			//Assert.AreEqual(dataProvider.AxleGearInputData.Manufacturer, sumRow[SummaryDataContainer.AXLE_MANUFACTURER]);
			//Assert.AreEqual(dataProvider.AxleGearInputData.Model, sumRow[SummaryDataContainer.AXLE_MODEL]);

			//Assert.AreEqual(dataProvider.EngineInputData.ColdHotBalancingFactor, sumRow[SummaryDataContainer.ENGINE_BF_COLD_HOT]);
			//Assert.AreEqual(dataProvider.EngineInputData.CorrectionFactorRegPer, sumRow[SummaryDataContainer.ENGINE_CF_REG_PER]);
			//Assert.AreEqual(dataProvider.EngineInputData.WHTCRural, sumRow[SummaryDataContainer.ENGINE_WHTC_RURAL]);
			//Assert.AreEqual(dataProvider.EngineInputData.WHTCUrban, sumRow[SummaryDataContainer.ENGINE_WHTC_URBAN]);
			//Assert.AreEqual(dataProvider.EngineInputData.WHTCMotorway, sumRow[SummaryDataContainer.ENGINE_WHTC_MOTORWAY]);
		}
	}
}
