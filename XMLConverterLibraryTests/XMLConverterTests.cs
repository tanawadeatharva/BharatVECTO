using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUGraz.VectoCore.Utils;
using XMLConverterLibrary;

namespace XMLConverterLibraryTests
{
	[TestFixture]
	[Parallelizable(ParallelScope.All)]
	public class XMLConverterTests
	{
		private XMLConverter _xmlJobConverter;

		[OneTimeSetUp]
		public void Init()
		{
			Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);

			_xmlJobConverter = new XMLConverter(
				new XMLFileReader(),
				new XMLJobConverterFactory(),
				new XMLFileWriter()
			);
		}

		[
			TestCase(InputData.CLASS_5_V1_0),
			TestCase(InputData.CLASS_5_MISSING_OPTIONAL_V1_0),
			TestCase(InputData.EXEMPTED_V1_0),
			TestCase(InputData.EXEMPTED_MISSING_OPTIONAL_V1_0),
			TestCase(InputData.NEW_PARAMS_V1_0),
			TestCase(InputData.LNG_V1_0),
			TestCase(InputData.SAMPLE_V1_0),
			TestCase(InputData.SAMPLE_AT_V1_0),
			TestCase(InputData.CERT_OPTIONS_V1_0),
			TestCase(InputData.SAMPLE_FULL_V1_0),
			TestCase(InputData.SAMPLE_FULL_UPD_V1_0),
			TestCase(InputData.SAMPLE_NO_AIRDRAG_V1_0),
			TestCase(InputData.SAMPLE_NO_TORQUE_LIMITS_V1_0),

			TestCase(InputData.CLASS_5_V2_0),
			TestCase(InputData.MISSING_OPTIONALS_V2_0),
			TestCase(InputData.COMPONENTS_V2_0),
			TestCase(InputData.NEW_PARAMS_V2_0),
			TestCase(InputData.LNG_V2_0),

			TestCase(InputData.CLASS_5_V2_1),
			TestCase(InputData.EXEMPTED_V2_1),
			TestCase(InputData.EXEMPTED_MISSING_OPTIONAL_V2_1),
			TestCase(InputData.NEW_PARAMS_V2_1),
			TestCase(InputData.LNG_V2_1),
			TestCase(InputData.TYRE_25_V2_1),

			TestCase(InputData.CLASS_5_V2_2),
			TestCase(InputData.LNG_V2_2),

			TestCase(InputData.EXEMPTED_V2_2_1),
			TestCase(InputData.EXEMPTED_MISSING_OPTIONAL_V2_2_1),

			TestCase(InputData.DUAL_MODE_DUAL_FUEL_V2_3),
			TestCase(InputData.DUAL_MODE_DUAL_FUEL_WHR_V2_3),
			TestCase(InputData.SINGLE_MODE_DUAL_FUEL_V2_3),
			TestCase(InputData.SINGLE_MODE_DUAL_FUEL_WHR_V2_3),
			TestCase(InputData.SINGLE_MODE_SINGLE_FUEL_WHR_V2_3),
		]
		public void Convert_ShouldCreateXMLFileOfTargetVersion_WhenInputXMLIsValid(string xmlFile)
		{
			var result = _xmlJobConverter.Convert(xmlFile, XMLUtils.V2_4);

			Assert.IsTrue(!result.IsError, string.Join(Environment.NewLine, result.Errors.Select(x => x.Description)));
			Assert.IsNotNull(result.Value);

			Console.WriteLine(result.Value);
		}


	}
}
