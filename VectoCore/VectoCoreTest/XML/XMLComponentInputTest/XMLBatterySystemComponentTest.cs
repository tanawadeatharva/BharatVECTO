using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using Ninject;
using NUnit.Framework;
using TUGraz.IVT.VectoXML;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Factory;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Reader;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.Utils;
using XmlDocumentType = TUGraz.VectoCore.Utils.XmlDocumentType;

namespace TUGraz.VectoCore.Tests.XML.XMLComponentInputTest
{
    [TestFixture]
    internal class XMLBatterySystemComponentTest
    {
		private StandardKernel _kernel;

		private string BASEDIRComponent = @"TestData\XML\XMLReaderDeclaration\SchemaVersion2.4\Distributed\ComponentData";

		private string BASEDIRVehicle = @"TestData\XML\XMLReaderDeclaration\SchemaVersion2.4\Distributed\HeavyLorry";

        private IDeclarationInjectFactory _declarationFactory;
		private IXMLInputDataReader _inputDataFactory;

		[SetUp]
		public void Setup()
		{
			_kernel = new StandardKernel(new VectoNinjectModule());
			_inputDataFactory = _kernel.Get<IXMLInputDataReader>();
			_declarationFactory = _kernel.Get<IDeclarationInjectFactory>();
		}

		[TestCase("BatterySystem_1.xml", TestName="Measured")]
		[TestCase("BatterySystem_StdValues.xml", TestName = "StandardValues")]
        public void LoadComponent(string fileName)
		{
			var path = GetFullPath(BASEDIRComponent, fileName);
			var document = LoadAndValidate(path);
			Assert.IsNotNull(document);
			TestContext.WriteLine(document);

			var reessReader = CreateBatterySystemReader(document, path);
			switch (reessReader) {
				case XMLBatteryPackDeclarationInputDataMeasuredV23 m:
					CheckMeasured(m, document);
					break;
				case XMLBatteryPackDeclarationInputDataStandardV23 s:
					CheckStandard(s, document);
					break;
				default:
					Assert.Fail("unexpected type");
					break;
			}	

		}

		public void CheckMeasured(IXMLBatteryPackDeclarationInputData m, XmlDocument document)
		{
			var resistanceCurve = m.InternalResistanceCurve;
			BatteryInternalResistanceReader.Create(resistanceCurve, true);
			//from input file "BatterySystem_StdValues.xml"

			var uncorrected = ReadInternalResistanceFromFile(document);
			for (var rowIdx = 0; rowIdx < uncorrected.Rows.Count; rowIdx++)
			{
				for (var colIdx = 0; colIdx < uncorrected.Columns.Count; colIdx++)
				{
					if (colIdx == 0)
					{
						//SOC no change
						Assert.AreEqual(uncorrected.Rows[rowIdx].ParseDouble(colIdx), resistanceCurve.Rows[rowIdx].ParseDouble(colIdx));
						continue;
					}
					Assert.AreEqual(uncorrected.Rows[rowIdx].ParseDouble(colIdx), resistanceCurve.Rows[rowIdx].ParseDouble(colIdx), 10e-3);  //mOhm
				}
			}
        }

		public void CheckStandard(IXMLBatteryPackDeclarationInputData s, XmlDocument document)
		{
			var resistanceCurve = s.InternalResistanceCurve;
			BatteryInternalResistanceReader.Create(resistanceCurve, true);
            //from input file "BatterySystem_StdValues.xml"
			var nominalVoltage = BatterySOCReader.Create(s.VoltageCurve).Lookup(0.5); //630

			var uncorrected = ReadInternalResistanceFromFile(document);
			//dcir = 630/3.3 = 190.90909090
			var dcir = 190.90909090909090;
			for (var rowIdx = 0; rowIdx < uncorrected.Rows.Count; rowIdx++) {
				for (var colIdx = 0; colIdx < uncorrected.Columns.Count; colIdx++) {
					if (colIdx == 0) {
						//SOC no change
						Assert.AreEqual(uncorrected.Rows[rowIdx].ParseDouble(colIdx), resistanceCurve.Rows[rowIdx].ParseDouble(colIdx));
						continue;
					}
					Assert.AreEqual(uncorrected.Rows[rowIdx].ParseDouble(colIdx) * dcir, resistanceCurve.Rows[rowIdx].ParseDouble(colIdx), 10e-3);  //mOhm
                }
			}
		}

		public TableData ReadInternalResistanceFromFile(XmlDocument document)
		{
			var irc = document.GetElementsByTagName(XMLNames.REESS_InternalResistanceCurve)[0];

			var entries = irc?.ChildNodes;
            if (entries is { Count: > 0 })
			{
				return XMLHelper.ReadTableData(AttributeMappings.InternalResistanceMap, entries);
            }

			Assert.Fail();
			return null;
		}




    [TestCase(XMLBatteryPackDeclarationInputDataMeasuredV23.XSD_TYPE,
			XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V23,
			typeof(XMLBatteryPackDeclarationInputDataMeasuredV23))]
		[TestCase(XMLBatteryPackDeclarationInputDataStandardV23.XSD_TYPE,
			XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V23,
			typeof(XMLBatteryPackDeclarationInputDataStandardV23))]
        public void FactoryTest(string type, string ns, Type expType)
		{
			XNamespace nameSpace = ns;
			var provider = _declarationFactory.CreateBatteryPackDeclarationInputData(XMLHelper.CombineNamespace(nameSpace, type), null, null, null);

			Assert.AreEqual(expType, provider.GetType());
			Assert.NotNull(provider);

		}
		[DebuggerStepThrough]
		public string GetFullPath(string baseDir, string fileName)
		{
			return Path.GetFullPath((Path.Combine(baseDir, fileName)));
        }

		[TestCase("PEV_heavyLorry_E3.xml")]
		public void CreateFromVehicle(string vehicleName)
		{
			var inputData = _inputDataFactory.CreateDeclaration(GetFullPath(BASEDIRVehicle, vehicleName));
			var ressPack = inputData.JobInputData.Vehicle.Components.ElectricStorage.ElectricStorageElements[0].REESSPack;

		}

		IXMLBatteryPackDeclarationInputData CreateBatterySystemReader(XmlDocument document, string source)
		{
			var componentNode = document.FirstChild.NextSibling.SelectSingleNode($"./*[local-name()='{XMLNames.Component_BatterySystem}']");
			Assert.NotNull(componentNode);
            var dataNode = componentNode.SelectSingleNode($"./*[local-name()='{XMLNames.ComponentDataWrapper}']");
			Assert.NotNull(dataNode);
			var version = XMLHelper.GetXsdType(dataNode.SchemaInfo.SchemaType);
			var input = _declarationFactory.CreateBatteryPackDeclarationInputData(version, null, componentNode,  source);
            Assert.NotNull(input);
			return input;

		}


		public XmlDocument LoadAndValidate(string filePath)
		{
			Assert.IsTrue(File.Exists(filePath), $"{filePath} missing");
			var document = new XmlDocument();
			using (var reader = XmlReader.Create(filePath)) {
				document.Load(reader);
			}
			var xmlValidator = new XMLValidator(document, null, XMLValidator.CallBackExceptionOnError);
			Assert.IsTrue(xmlValidator.ValidateXML(XmlDocumentType.DeclarationComponentData));
			return document;
		}

		//var versionNumber = XMLHelper.GetXsdType(xmlDoc.DocumentElement?.SchemaInfo.SchemaType);
		//	try {
		//	var input = DeclarationFactory.CreateInputProvider(versionNumber, xmlDoc, source);
		//	input.Reader = DeclarationFactory.CreateInputReader(versionNumber, input, xmlDoc.DocumentElement);
		//	return input;
		//} catch (Exception e) {
		//throw new VectoException("Failed to read Declaration job version {0}", e, versionNumber);
	}
}
