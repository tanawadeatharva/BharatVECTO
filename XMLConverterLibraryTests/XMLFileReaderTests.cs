using TUGraz.VectoCore.Utils;
using XMLConverterLibrary;

namespace XMLConverterLibraryTests
{
	[TestFixture]
	[Parallelizable(ParallelScope.All)]
	public class XMLFileReaderTests
	{
		private IXMLFileReader _xmlFileReader;

		[OneTimeSetUp]
		public void Init()
		{
			Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);

			_xmlFileReader = new XMLFileReader();
		}

		[
			TestCase(InputData.CLASS_5_V1_0, XmlDocumentType.DeclarationJobData),
			TestCase(InputData.CLASS_5_MISSING_OPTIONAL_V1_0, XmlDocumentType.DeclarationJobData),
			TestCase(InputData.EXEMPTED_V1_0, XmlDocumentType.DeclarationJobData),
			TestCase(InputData.NEW_PARAMS_V1_0, XmlDocumentType.DeclarationJobData),
			TestCase(InputData.LNG_V1_0, XmlDocumentType.DeclarationJobData),
			TestCase(InputData.SAMPLE_V1_0, XmlDocumentType.DeclarationJobData),
			TestCase(InputData.SAMPLE_AT_V1_0, XmlDocumentType.DeclarationJobData),
			TestCase(InputData.CERT_OPTIONS_V1_0, XmlDocumentType.DeclarationJobData),
			TestCase(InputData.SAMPLE_FULL_V1_0, XmlDocumentType.DeclarationJobData),
			TestCase(InputData.SAMPLE_FULL_UPD_V1_0, XmlDocumentType.DeclarationJobData),
			TestCase(InputData.SAMPLE_NO_AIRDRAG_V1_0, XmlDocumentType.DeclarationJobData),
			TestCase(InputData.SAMPLE_NO_TORQUE_LIMITS_V1_0, XmlDocumentType.DeclarationJobData),

			TestCase(InputData.CLASS_5_V2_0 , XmlDocumentType.DeclarationJobData),
			TestCase(InputData.COMPONENTS_V2_0, XmlDocumentType.DeclarationJobData),
			TestCase(InputData.NEW_PARAMS_V2_0, XmlDocumentType.DeclarationJobData),
			TestCase(InputData.LNG_V2_0, XmlDocumentType.DeclarationJobData),

			TestCase(InputData.CLASS_5_V2_1, XmlDocumentType.DeclarationJobData),
			TestCase(InputData.EXEMPTED_V2_1, XmlDocumentType.DeclarationJobData),
			TestCase(InputData.NEW_PARAMS_V2_1, XmlDocumentType.DeclarationJobData),
			TestCase(InputData.LNG_V2_1, XmlDocumentType.DeclarationJobData),
			TestCase(InputData.TYRE_25_V2_1, XmlDocumentType.DeclarationJobData),

			TestCase(InputData.CLASS_5_V2_2, XmlDocumentType.DeclarationJobData),
			TestCase(InputData.LNG_V2_2, XmlDocumentType.DeclarationJobData),

			TestCase(InputData.EXEMPTED_V2_2_1, XmlDocumentType.DeclarationJobData),
			TestCase(InputData.EXEMPTED_MISSING_OPTIONAL_V2_2_1, XmlDocumentType.DeclarationJobData),

			TestCase(InputData.DUAL_MODE_DUAL_FUEL_V2_3, XmlDocumentType.DeclarationJobData),
			TestCase(InputData.DUAL_MODE_DUAL_FUEL_WHR_V2_3, XmlDocumentType.DeclarationJobData),
			TestCase(InputData.SINGLE_MODE_DUAL_FUEL_V2_3, XmlDocumentType.DeclarationJobData),
			TestCase(InputData.SINGLE_MODE_DUAL_FUEL_WHR_V2_3, XmlDocumentType.DeclarationJobData),
			TestCase(InputData.SINGLE_MODE_SINGLE_FUEL_WHR_V2_3, XmlDocumentType.DeclarationJobData),
		]
		public void Read_ShouldReturnXMLContent_WhenXMLIsValid(string xmlFile, XmlDocumentType documentType)
		{
			var result = _xmlFileReader.Read(xmlFile, documentType);

			Assert.IsTrue(!result.IsError, string.Join(Environment.NewLine, result.Errors.Select(x => x.Description)));
			Assert.IsNotNull(result.Value);
		}
	}
}