using System.IO;
using System.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TUGraz.IVT.VectoXML.Writer;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration;
using TUGraz.VectoCore.InputData.FileIO.XML.Engineering;
using TUGraz.VectoCore.OutputData.XML;

namespace TUGraz.VectoCore.Tests.XML
{
	[TestClass]
	public class XMLWritingTests
	{
		const string EngineOnlyJob = @"TestData\XML\EngineOnlyJob\EngineOnly.vecto";
		const string EngineeringJob = @"TestData\XML\EngineeringJob\Class5_Tractor_4x2\Class5_Tractor_ENG.vecto";

		const string EngineeringJobFull =
			@"TestData\XML\XMLWriter\EngineeringJob\Class5_Tractor_4x2\Class5_Tractor_ENG-FULL.vecto";

		const string DeclarationJob = @"TestData\XML\XMLWriter\DeclarationJob\Class5_Tractor_4x2\Class5_Tractor_DECL.vecto";

		const string DeclarationJobFull =
			@"TestData\XML\XMLWriter\DeclarationJob\Class5_Tractor_4x2\Class5_Tractor_DECL-FULL.vecto";


		[TestMethod]
		public void TestWriteEngineOnlySingleFile()
		{
			var outFile = "EngineOnlyJobSingleFile.xml";

			if (File.Exists(outFile)) {
				File.Delete(outFile);
			}

			var inputData = JSONInputDataFactory.ReadJsonJob(EngineOnlyJob);
			var job =
				new XMLEngineeringWriter(".", true, "TU Graz, IVT").GenerateVectoJob((IEngineeringInputDataProvider)inputData);
			job.Save(outFile);

			//var reader = XmlReader.Create(outFile);
			var xml = new XMLEngineeringInputDataProvider(outFile, true);

			Assert.IsNotNull(xml);
			Assert.AreEqual("ENG-175kW 6.8l Engine", xml.JobInputData().JobName);
		}

		[TestMethod]
		public void TestWriteEngineeringSingleFile()
		{
			var outFile = "EngineeringJobSingleFile.xml";

			var inputData = JSONInputDataFactory.ReadJsonJob(EngineeringJob);
			var job =
				new XMLEngineeringWriter(".", true, "TU Graz, IVT").GenerateVectoJob((IEngineeringInputDataProvider)inputData);
			job.Save(outFile);

			var xml = new XMLEngineeringInputDataProvider(outFile, true);

			Assert.IsNotNull(xml);
			Assert.AreEqual("VEH-N/A", xml.JobInputData().JobName);
		}

		[TestMethod]
		public void TestWriteEngineeringSingleFileFull()
		{
			var outFile = "EngineeringJobSingleFileFull.xml";

			var inputData = JSONInputDataFactory.ReadJsonJob(EngineeringJobFull);
			var job =
				new XMLEngineeringWriter(".", true, "TU Graz, IVT").GenerateVectoJob((IEngineeringInputDataProvider)inputData);
			job.Save(outFile);

			var xml = new XMLEngineeringInputDataProvider(outFile, true);

			Assert.IsNotNull(xml);
			Assert.AreEqual("VEH-N/A", xml.JobInputData().JobName);
		}

		[TestMethod]
		public void TestWriteEngineeringMultipleFilesFull()
		{
			var outFile = "EngineeringJobMultipleFilesFull.xml";
			var outDir = "Engineering_MultipleFiles_Full";
			Directory.CreateDirectory(outDir);

			var inputData = JSONInputDataFactory.ReadJsonJob(EngineeringJobFull);
			var job =
				new XMLEngineeringWriter(outDir, false, "TU Graz, IVT").GenerateVectoJob((IEngineeringInputDataProvider)inputData);
			job.Save(Path.Combine(outDir, outFile));

			var xml = new XMLEngineeringInputDataProvider(Path.Combine(outDir, outFile), true);

			Assert.IsNotNull(xml);
			Assert.AreEqual("VEH-N/A", xml.JobInputData().JobName);
		}

		[TestMethod]
		public void TestWriteEngineeringMultipleFiles()
		{
			var inputData = JSONInputDataFactory.ReadJsonJob(EngineeringJob);
			Directory.CreateDirectory("Engineering_MultipleFiles");
			var job =
				new XMLEngineeringWriter("Engineering_MultipleFiles", false, "TU Graz, IVT").GenerateVectoJob(
					(IEngineeringInputDataProvider)inputData);
			job.Save("Engineering_MultipleFiles/EngineeringJobMultipleFiles.xml");

			//var xml = new XMLEngineeringInputDataProvider(outFile, true);

			//Assert.IsNotNull(xml);
			//Assert.AreEqual("VEH-N/A", xml.JobInputData().JobName);
		}

		[TestMethod]
		public void TestWriteDeclarationJob()
		{
			var outputFile = "DeclarationJobSingleFile.xml";

			if (File.Exists(outputFile)) {
				File.Delete(outputFile);
			}

			var inputData = JSONInputDataFactory.ReadJsonJob(DeclarationJob);

			var job = new XMLDeclarationWriter("TUG_IVT").GenerateVectoJob((IDeclarationInputDataProvider)inputData);

			job.Save(outputFile);

			var reader = XmlReader.Create(outputFile);
			var xml = new XMLInputDataProvider(reader, true);

			Assert.IsNotNull(xml);
			Assert.AreEqual("VEH-N/A", xml.JobInputData().JobName);
		}

		[TestMethod]
		public void TestWriteDeclarationJobFull()
		{
			var outputFile = "DeclarationJobFullSingleFile.xml";

			if (File.Exists(outputFile)) {
				File.Delete(outputFile);
			}

			var inputData = JSONInputDataFactory.ReadJsonJob(DeclarationJobFull);

			var job = new XMLDeclarationWriter( "TUG_IVT").GenerateVectoJob((IDeclarationInputDataProvider)inputData);

			job.Save(outputFile);

			var reader = XmlReader.Create(outputFile);
			var xml = new XMLInputDataProvider(reader, true);

			Assert.IsNotNull(xml);
			Assert.AreEqual("VEH-N/A", xml.JobInputData().JobName);
		}
	}
}