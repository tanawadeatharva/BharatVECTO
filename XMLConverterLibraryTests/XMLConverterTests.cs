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
            TestCase(InputData.CLASS_5_V1_0, XMLUtils.V2_4, Category = XMLUtils.V2_4),
			TestCase(InputData.CLASS_5_MISSING_OPTIONAL_V1_0, XMLUtils.V2_4, Category = XMLUtils.V2_4),
			TestCase(InputData.EXEMPTED_V1_0, XMLUtils.V2_4, Category = XMLUtils.V2_4),
			TestCase(InputData.EXEMPTED_MISSING_OPTIONAL_V1_0, XMLUtils.V2_4, Category = XMLUtils.V2_4),
			TestCase(InputData.NEW_PARAMS_V1_0, XMLUtils.V2_4, Category = XMLUtils.V2_4),
			TestCase(InputData.LNG_V1_0, XMLUtils.V2_4, Category = XMLUtils.V2_4),
			TestCase(InputData.SAMPLE_V1_0, XMLUtils.V2_4, Category = XMLUtils.V2_4),
			TestCase(InputData.SAMPLE_AT_V1_0, XMLUtils.V2_4, Category = XMLUtils.V2_4),
			TestCase(InputData.CERT_OPTIONS_V1_0, XMLUtils.V2_4, Category = XMLUtils.V2_4),
			TestCase(InputData.SAMPLE_FULL_V1_0, XMLUtils.V2_4, Category = XMLUtils.V2_4),
			TestCase(InputData.SAMPLE_FULL_UPD_V1_0, XMLUtils.V2_4, Category = XMLUtils.V2_4),
			TestCase(InputData.SAMPLE_NO_AIRDRAG_V1_0, XMLUtils.V2_4, Category = XMLUtils.V2_4),
			TestCase(InputData.SAMPLE_NO_TORQUE_LIMITS_V1_0, XMLUtils.V2_4, Category = XMLUtils.V2_4),

			TestCase(InputData.CLASS_5_V2_0, XMLUtils.V2_4, Category = XMLUtils.V2_4),
			TestCase(InputData.MISSING_OPTIONALS_V2_0, XMLUtils.V2_4, Category = XMLUtils.V2_4),
			TestCase(InputData.COMPONENTS_V2_0, XMLUtils.V2_4, Category = XMLUtils.V2_4),
			TestCase(InputData.NEW_PARAMS_V2_0, XMLUtils.V2_4, Category = XMLUtils.V2_4),
			TestCase(InputData.LNG_V2_0, XMLUtils.V2_4, Category = XMLUtils.V2_4),

			TestCase(InputData.CLASS_5_V2_1, XMLUtils.V2_4, Category = XMLUtils.V2_4),
			TestCase(InputData.EXEMPTED_V2_1, XMLUtils.V2_4, Category = XMLUtils.V2_4),
			TestCase(InputData.EXEMPTED_MISSING_OPTIONAL_V2_1, XMLUtils.V2_4, Category = XMLUtils.V2_4),
			TestCase(InputData.NEW_PARAMS_V2_1, XMLUtils.V2_4, Category = XMLUtils.V2_4),
			TestCase(InputData.LNG_V2_1, XMLUtils.V2_4, Category = XMLUtils.V2_4),
			TestCase(InputData.TYRE_25_V2_1, XMLUtils.V2_4, Category = XMLUtils.V2_4),

			TestCase(InputData.CLASS_5_V2_2, XMLUtils.V2_4, Category = XMLUtils.V2_4),
			TestCase(InputData.LNG_V2_2, XMLUtils.V2_4, Category = XMLUtils.V2_4),

			TestCase(InputData.EXEMPTED_V2_2_1, XMLUtils.V2_4, Category = XMLUtils.V2_4),
			TestCase(InputData.EXEMPTED_MISSING_OPTIONAL_V2_2_1, XMLUtils.V2_4, Category = XMLUtils.V2_4),

			TestCase(InputData.DUAL_MODE_DUAL_FUEL_V2_3, XMLUtils.V2_4, Category = XMLUtils.V2_4),
			TestCase(InputData.DUAL_MODE_DUAL_FUEL_WHR_V2_3, XMLUtils.V2_4, Category = XMLUtils.V2_4),
			TestCase(InputData.SINGLE_MODE_DUAL_FUEL_V2_3, XMLUtils.V2_4, Category = XMLUtils.V2_4),
			TestCase(InputData.SINGLE_MODE_DUAL_FUEL_WHR_V2_3, XMLUtils.V2_4, Category = XMLUtils.V2_4),
			TestCase(InputData.SINGLE_MODE_SINGLE_FUEL_WHR_V2_3, XMLUtils.V2_4, Category = XMLUtils.V2_4),
        
            TestCase(InputData.EXEMPTED_HEAVY_LORRY_V2_4, XMLUtils.V2_7, Category = XMLUtils.V2_7),
            TestCase(InputData.EXEMPTED_MEDIUM_LORRY_V2_4, XMLUtils.V2_7, Category = XMLUtils.V2_7),
            TestCase(InputData.EXEMPTED_PRIMARY_BUS_V2_4, XMLUtils.V2_7, Category = XMLUtils.V2_7),
            TestCase(InputData.EXEMPTED_COMPLETED_BUS_V2_4, XMLUtils.V2_7, Category = XMLUtils.V2_7),
            TestCase(InputData.EXEMPTED_COMPLETED_BUS_REQ_ONLY_V2_4, XMLUtils.V2_7, Category = XMLUtils.V2_7),
            TestCase(InputData.CONVENTIONAL_HEAVY_LORRY_V2_4, XMLUtils.V2_7, Category = XMLUtils.V2_7),
			TestCase(InputData.CONVENTIONAL_HEAVY_LORRY_REQ_ONLY_V2_4, XMLUtils.V2_7, Category = XMLUtils.V2_7),
			TestCase(InputData.CONVENTIONAL_MEDIUM_LORRY_V2_4, XMLUtils.V2_7, Category = XMLUtils.V2_7),
            TestCase(InputData.CONVENTIONAL_MEDIUM_LORRY_REQ_ONLY_V2_4, XMLUtils.V2_7, Category = XMLUtils.V2_7),
            TestCase(InputData.CONVENTIONAL_PRIMARY_BUS_V2_4, XMLUtils.V2_7, Category = XMLUtils.V2_7),
            TestCase(InputData.CONVENTIONAL_PRIMARY_BUS_REQ_ONLY_V2_4, XMLUtils.V2_7, Category = XMLUtils.V2_7),
            TestCase(InputData.CONVENTIONAL_COMPLETED_BUS_V2_4, XMLUtils.V2_7, Category = XMLUtils.V2_7),
            TestCase(InputData.CONVENTIONAL_COMPLETED_BUS_REQ_ONLY_V2_4, XMLUtils.V2_7, Category = XMLUtils.V2_7),
            TestCase(InputData.PHEV_PX_HEAVY_LORRY_V2_4, XMLUtils.V2_7, Category = XMLUtils.V2_7),
            TestCase(InputData.PHEV_PX_HEAVY_LORRY_REQ_ONLY_V2_4, XMLUtils.V2_7, Category = XMLUtils.V2_7),
            TestCase(InputData.PHEV_IHPC_HEAVY_LORRY_V2_4, XMLUtils.V2_7, Category = XMLUtils.V2_7),
            TestCase(InputData.PHEV_Px_HEAVY_LORRY_CAPACITOR_V2_4, XMLUtils.V2_7, Category = XMLUtils.V2_7),
            TestCase(InputData.PHEV_PX_MEDIUM_LORRY_V2_4, XMLUtils.V2_7, Category = XMLUtils.V2_7),
            TestCase(InputData.PHEV_PX_PRIMARY_BUS_V2_4, XMLUtils.V2_7, Category = XMLUtils.V2_7),
            TestCase(InputData.SHEV_S2_HEAVY_LORRY_V2_4, XMLUtils.V2_7, Category = XMLUtils.V2_7),
            TestCase(InputData.SHEV_S3_HEAVY_LORRY_V2_4, XMLUtils.V2_7, Category = XMLUtils.V2_7),
            TestCase(InputData.SHEV_S4_HEAVY_LORRY_V2_4, XMLUtils.V2_7, Category = XMLUtils.V2_7),
            TestCase(InputData.SHEV_S2_HEAVY_LORRY_REQ_ONLY_V2_4, XMLUtils.V2_7, Category = XMLUtils.V2_7),
            TestCase(InputData.SHEV_S2_HEAVY_LORRY_OVC_V2_4, XMLUtils.V2_7, Category = XMLUtils.V2_7),
            TestCase(InputData.SHEV_S3_HEAVY_LORRY_OVC_V2_4, XMLUtils.V2_7, Category = XMLUtils.V2_7),
            TestCase(InputData.SHEV_IEPC_HEAVY_LORRY_V2_4, XMLUtils.V2_7, Category = XMLUtils.V2_7),
            TestCase(InputData.SHEV_S2_MEDIUM_LORRY_V2_4, XMLUtils.V2_7, Category = XMLUtils.V2_7),
            TestCase(InputData.SHEV_S3_MEDIUM_LORRY_V2_4, XMLUtils.V2_7, Category = XMLUtils.V2_7),
            TestCase(InputData.SHEV_S4_MEDIUM_LORRY_V2_4, XMLUtils.V2_7, Category = XMLUtils.V2_7),
            TestCase(InputData.SHEV_IEPC_MEDIUM_LORRY_V2_4, XMLUtils.V2_7, Category = XMLUtils.V2_7),
            TestCase(InputData.SHEV_S2_PRIMARY_BUS_V2_4, XMLUtils.V2_7, Category = XMLUtils.V2_7),
            TestCase(InputData.SHEV_S3_PRIMARY_BUS_V2_4, XMLUtils.V2_7, Category = XMLUtils.V2_7),
            TestCase(InputData.SHEV_S4_PRIMARY_BUS_V2_4, XMLUtils.V2_7, Category = XMLUtils.V2_7),
            TestCase(InputData.SHEV_IEPC_PRIMARY_BUS_V2_4, XMLUtils.V2_7, Category = XMLUtils.V2_7),
            TestCase(InputData.PEV_E2_HEAVY_LORRY_V2_4, XMLUtils.V2_7, Category = XMLUtils.V2_7),
            TestCase(InputData.PEV_E3_HEAVY_LORRY_V2_4, XMLUtils.V2_7, Category = XMLUtils.V2_7),
            TestCase(InputData.PEV_E4_HEAVY_LORRY_V2_4, XMLUtils.V2_7, Category = XMLUtils.V2_7),
            TestCase(InputData.PEV_IEPC_HEAVY_LORRY_V2_4, XMLUtils.V2_7, Category = XMLUtils.V2_7),
            TestCase(InputData.PEV_E2_MEDIUM_LORRY_V2_4, XMLUtils.V2_7, Category = XMLUtils.V2_7),
            TestCase(InputData.PEV_E3_MEDIUM_LORRY_V2_4, XMLUtils.V2_7, Category = XMLUtils.V2_7),
            TestCase(InputData.PEV_E4_MEDIUM_LORRY_V2_4, XMLUtils.V2_7, Category = XMLUtils.V2_7),
            TestCase(InputData.PEV_IEPC_MEDIUM_LORRY_V2_4, XMLUtils.V2_7, Category = XMLUtils.V2_7),
            TestCase(InputData.PEV_E2_PRIMARY_BUS_V2_4, XMLUtils.V2_7, Category = XMLUtils.V2_7),
            TestCase(InputData.PEV_E3_PRIMARY_BUS_V2_4, XMLUtils.V2_7, Category = XMLUtils.V2_7),
            TestCase(InputData.PEV_E4_PRIMARY_BUS_V2_4, XMLUtils.V2_7, Category = XMLUtils.V2_7),
            TestCase(InputData.PEV_IEPC_PRIMARY_BUS_V2_4, XMLUtils.V2_7, Category = XMLUtils.V2_7),
            TestCase(InputData.HEV_COMPLETED_BUS_V2_4, XMLUtils.V2_7, Category = XMLUtils.V2_7),
            TestCase(InputData.PEV_IEPC_COMPLETED_BUS_V2_4, XMLUtils.V2_7, Category = XMLUtils.V2_7),
            TestCase(InputData.PEV_Px_COMPLETED_BUS_V2_4, XMLUtils.V2_7, Category = XMLUtils.V2_7),
            TestCase(InputData.SHEV_IEPC_HEAVY_LORRY_V26SAMPLE1_V2_4, XMLUtils.V2_7, Category = XMLUtils.V2_7),
            TestCase(InputData.SHEV_IEPC_HEAVY_LORRY_V26SAMPLE2_V2_4, XMLUtils.V2_7, Category = XMLUtils.V2_7),

            TestCase(InputData.GEN_CONVENTIONAL_COMPLETEDBUS_2_V2_4, XMLUtils.V2_7, Category = XMLUtils.V2_7),
            TestCase(InputData.GEN_EXEMPTED_COMPLETEDBUS_INPUT_FULL_V2_4, XMLUtils.V2_7, Category = XMLUtils.V2_7),
            TestCase(InputData.GEN_EXEMPTED_HEAVY_LORRY_V2_4, XMLUtils.V2_7, Category = XMLUtils.V2_7),
            TestCase(InputData.GEN_EXEMPTED_PRIMARY_HEAVYBUS_V2_4, XMLUtils.V2_7, Category = XMLUtils.V2_7),
            TestCase(InputData.GEN_GROUP2_HEV_IEPC_S_V2_4, XMLUtils.V2_7, Category = XMLUtils.V2_7),
            TestCase(InputData.GEN_GROUP2_HEV_S2_OVC_V2_4, XMLUtils.V2_7, Category = XMLUtils.V2_7),
            TestCase(InputData.GEN_GROUP5_CONV_ES_STANDARD_V2_4, XMLUtils.V2_7, Category = XMLUtils.V2_7),
            TestCase(InputData.GEN_GROUP5_HEV_P2_NON_OVC_V2_4, XMLUtils.V2_7, Category = XMLUtils.V2_7),
            TestCase(InputData.GEN_GROUP5_HEV_P2_OVC_V2_4, XMLUtils.V2_7, Category = XMLUtils.V2_7),
            TestCase(InputData.GEN_GROUP5_HEV_P2_SUPERCAP_V2_4, XMLUtils.V2_7, Category = XMLUtils.V2_7),
            TestCase(InputData.GEN_GROUP5_PEV_E4_V2_4, XMLUtils.V2_7, Category = XMLUtils.V2_7),
            TestCase(InputData.GEN_GROUP5_PEV_IEPC_E_V2_4, XMLUtils.V2_7, Category = XMLUtils.V2_7),
            TestCase(InputData.GEN_GROUP5_PEV_IEPC_E_MULTICURVE_V2_4, XMLUtils.V2_7, Category = XMLUtils.V2_7),
            TestCase(InputData.GEN_GROUP5_PEV_IEPC_E_STDVALUES_V2_4, XMLUtils.V2_7, Category = XMLUtils.V2_7),
            TestCase(InputData.GEN_HEAVYLORRY_IHPC_V2_4, XMLUtils.V2_7, Category = XMLUtils.V2_7),
            TestCase(InputData.GEN_HEV_COMPLETEDBUS_2_V2_4, XMLUtils.V2_7, Category = XMLUtils.V2_7),
            TestCase(InputData.GEN_IEPC_COMPLETEDBUS_2_V2_4, XMLUtils.V2_7, Category = XMLUtils.V2_7),
            TestCase(InputData.GEN_IEPC_PRIMARYBUS_MULTICURVES_V2_4, XMLUtils.V2_7, Category = XMLUtils.V2_7),
            TestCase(InputData.GEN_PEV_COMPLETEDBUS_2_V2_4, XMLUtils.V2_7, Category = XMLUtils.V2_7),
            TestCase(InputData.GEN_PEV_HEAVYLORRY_AMT_E2_V2_4, XMLUtils.V2_7, Category = XMLUtils.V2_7),
            TestCase(InputData.GEN_PEV_PRIMARYBUS_AMT_E2_V2_4, XMLUtils.V2_7, Category = XMLUtils.V2_7),
            TestCase(InputData.GEN_PRIMARY_HEAVYBUS_GROUP_P31_32_SMART_ES_V2_4, XMLUtils.V2_7, Category = XMLUtils.V2_7),
            TestCase(InputData.GEN_PRIMARY_HEAVYBUS_GROUP_P33_34_SMART_PS_V2_4, XMLUtils.V2_7, Category = XMLUtils.V2_7),
            TestCase(InputData.GEN_PRIMARY_HEAVYBUS_GROUP_41_NON_SMART_V2_4, XMLUtils.V2_7, Category = XMLUtils.V2_7),
            TestCase(InputData.GEN_PRIMARY_CITYBUS_IHPC_V2_4, XMLUtils.V2_7, Category = XMLUtils.V2_7),
            TestCase(InputData.GEN_PRIMARYCOACH_P2_HEV_AMT_CONV_V2_4, XMLUtils.V2_7, Category = XMLUtils.V2_7),
            TestCase(InputData.GEN_PRIMARYCOACH_P2_HEV_AMT_OVC_V2_4, XMLUtils.V2_7, Category = XMLUtils.V2_7),
            TestCase(InputData.GEN_PRIMARYCOACH_S2_BASE_AMT_V2_4, XMLUtils.V2_7, Category = XMLUtils.V2_7),
            TestCase(InputData.GEN_VECTO_VEHICLE_COMPLETED_HEAVYBUS_31B_V2_4, XMLUtils.V2_7, Category = XMLUtils.V2_7),
            TestCase(InputData.GEN_VECTO_VEHICLE_COMPLETED_HEAVYBUS_34F_V2_4, XMLUtils.V2_7, Category = XMLUtils.V2_7),
            TestCase(InputData.GEN_INPUT_AIRDRAG_ONLY_31B2_V2_4, XMLUtils.V2_7, Category = XMLUtils.V2_7),
            TestCase(InputData.GEN_INPUT_AUX_31B2_V2_4, XMLUtils.V2_7, Category = XMLUtils.V2_7),
            TestCase(InputData.GEN_VECTO_VEHICLE_COMPLETED_HEAVYBUS_31B2_V2_4, XMLUtils.V2_7, Category = XMLUtils.V2_7),
            TestCase(InputData.GEN_INPUT_AIRGRAG_ONLY_31B2_NGPI_V2_4, XMLUtils.V2_7, Category = XMLUtils.V2_7),
            TestCase(InputData.GEN_INPUT_AUX_31B2_NGPI_V2_4, XMLUtils.V2_7, Category = XMLUtils.V2_7),
            TestCase(InputData.GEN_PRIMARY_HEAVYBUS_GROUP_P31_32_NGPI_V2_4, XMLUtils.V2_7, Category = XMLUtils.V2_7),
            TestCase(InputData.GEN_VECTO_VEHICLE_COMPLETED_HEAVYBUS_31B2_NGPI_V2_4, XMLUtils.V2_7, Category = XMLUtils.V2_7),
            TestCase(InputData.GEN_VECTO_VEHICLE_COMPLETED_HEAVYBUS_34F_COMP34F_V2_4, XMLUtils.V2_7, Category = XMLUtils.V2_7),
            TestCase(InputData.GEN_VECTO_VEHICLE_COMPLETED_HEAVYBUS_39A_V2_4, XMLUtils.V2_7, Category = XMLUtils.V2_7),
            TestCase(InputData.GEN_VECTO_VEHICLE_MEDIUM_LORRY_4X2_V2_4, XMLUtils.V2_7, Category = XMLUtils.V2_7),
            TestCase(InputData.GEN_PRIMARY_HEAVYBUS_GROUP_P35_36_NONSMART_V2_4, XMLUtils.V2_7, Category = XMLUtils.V2_7),
            TestCase(InputData.GEN_PRIMARY_HEAVYBUS_GROUP_P37_38_SMART_V2_4, XMLUtils.V2_7, Category = XMLUtils.V2_7),
            TestCase(InputData.GEN_PRIMARY_HEAVYBUS_GROUP_P39_40_NONSMART_V2_4, XMLUtils.V2_7, Category = XMLUtils.V2_7),
            TestCase(InputData.GEN_PRIMARY_HEAVYBUS_GROUP_P31_32_SMART_V2_4, XMLUtils.V2_7, Category = XMLUtils.V2_7),
            TestCase(InputData.GEN_VECTO_VEHICLE_COMPLETED_HEAVYBUS_31B_FM_V2_4, XMLUtils.V2_7, Category = XMLUtils.V2_7),
            TestCase(InputData.GEN_PRIMARY_HEAVYBUS_GROUP_P33_34_SMART_FM_V2_4, XMLUtils.V2_7, Category = XMLUtils.V2_7),
            TestCase(InputData.GEN_VECTO_VEHICLE_COMPLETED_HEAVYBUS_34F_FM_V2_4, XMLUtils.V2_7, Category = XMLUtils.V2_7),

            TestCase(InputData.CLASS_5_V1_0, XMLUtils.V2_7, Category = XMLUtils.V2_7),
            TestCase(InputData.CLASS_5_MISSING_OPTIONAL_V1_0, XMLUtils.V2_7, Category = XMLUtils.V2_7),
            TestCase(InputData.EXEMPTED_V1_0, XMLUtils.V2_7, Category = XMLUtils.V2_7),
            TestCase(InputData.EXEMPTED_MISSING_OPTIONAL_V1_0, XMLUtils.V2_7, Category = XMLUtils.V2_7),
            TestCase(InputData.NEW_PARAMS_V1_0, XMLUtils.V2_7, Category = XMLUtils.V2_7),
            TestCase(InputData.LNG_V1_0, XMLUtils.V2_7, Category = XMLUtils.V2_7),
            TestCase(InputData.SAMPLE_V1_0, XMLUtils.V2_7, Category = XMLUtils.V2_7),
            TestCase(InputData.SAMPLE_AT_V1_0, XMLUtils.V2_7, Category = XMLUtils.V2_7),
            TestCase(InputData.CERT_OPTIONS_V1_0, XMLUtils.V2_7, Category = XMLUtils.V2_7),
            TestCase(InputData.SAMPLE_FULL_V1_0, XMLUtils.V2_7, Category = XMLUtils.V2_7),
            TestCase(InputData.SAMPLE_FULL_UPD_V1_0, XMLUtils.V2_7, Category = XMLUtils.V2_7),
            TestCase(InputData.SAMPLE_NO_AIRDRAG_V1_0, XMLUtils.V2_7, Category = XMLUtils.V2_7),
            TestCase(InputData.SAMPLE_NO_TORQUE_LIMITS_V1_0, XMLUtils.V2_7, Category = XMLUtils.V2_7),

            TestCase(InputData.CLASS_5_V2_0, XMLUtils.V2_7, Category = XMLUtils.V2_7),
            TestCase(InputData.MISSING_OPTIONALS_V2_0, XMLUtils.V2_7, Category = XMLUtils.V2_7),
            TestCase(InputData.COMPONENTS_V2_0, XMLUtils.V2_7, Category = XMLUtils.V2_7),
            TestCase(InputData.NEW_PARAMS_V2_0, XMLUtils.V2_7, Category = XMLUtils.V2_7),
            TestCase(InputData.LNG_V2_0, XMLUtils.V2_7, Category = XMLUtils.V2_7),

            TestCase(InputData.CLASS_5_V2_1, XMLUtils.V2_7, Category = XMLUtils.V2_7),
            TestCase(InputData.EXEMPTED_V2_1, XMLUtils.V2_7, Category = XMLUtils.V2_7),
            TestCase(InputData.EXEMPTED_MISSING_OPTIONAL_V2_1, XMLUtils.V2_7, Category = XMLUtils.V2_7),
            TestCase(InputData.NEW_PARAMS_V2_1, XMLUtils.V2_7, Category = XMLUtils.V2_7),
            TestCase(InputData.LNG_V2_1, XMLUtils.V2_7, Category = XMLUtils.V2_7),
            TestCase(InputData.TYRE_25_V2_1, XMLUtils.V2_7, Category = XMLUtils.V2_7),

            TestCase(InputData.CLASS_5_V2_2, XMLUtils.V2_7, Category = XMLUtils.V2_7),
            TestCase(InputData.LNG_V2_2, XMLUtils.V2_7, Category = XMLUtils.V2_7),

            TestCase(InputData.EXEMPTED_V2_2_1, XMLUtils.V2_7, Category = XMLUtils.V2_7),
            TestCase(InputData.EXEMPTED_MISSING_OPTIONAL_V2_2_1, XMLUtils.V2_7, Category = XMLUtils.V2_7),

            TestCase(InputData.DUAL_MODE_DUAL_FUEL_V2_3, XMLUtils.V2_7, Category = XMLUtils.V2_7),
            TestCase(InputData.DUAL_MODE_DUAL_FUEL_WHR_V2_3, XMLUtils.V2_7, Category = XMLUtils.V2_7),
            TestCase(InputData.SINGLE_MODE_DUAL_FUEL_V2_3, XMLUtils.V2_7, Category = XMLUtils.V2_7),
            TestCase(InputData.SINGLE_MODE_DUAL_FUEL_WHR_V2_3, XMLUtils.V2_7, Category = XMLUtils.V2_7),
            TestCase(InputData.SINGLE_MODE_SINGLE_FUEL_WHR_V2_3, XMLUtils.V2_7, Category = XMLUtils.V2_7),
        ]
		public void Convert_ShouldCreateXMLFileOfTargetVersion_WhenInputXMLIsValid(string xmlFile, string toVersion)
		{
			var result = _xmlJobConverter.Convert(xmlFile, toVersion);

			Assert.IsTrue(!result.IsError, string.Join(Environment.NewLine, result.Errors.Select(x => x.Description)));
			Assert.IsNotNull(result.Value);

			Console.WriteLine(result.Value);
		}


	}
}
