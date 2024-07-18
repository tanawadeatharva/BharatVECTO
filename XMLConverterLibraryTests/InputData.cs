using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XMLConverterLibraryTests
{
	internal class InputData
	{
		public const string V1_0 = "TestData/Vehicles/SchemaVersion1.0";
		public const string EXEMPTED_V1_0 = $"{V1_0}/vecto_vehicle-exempted-sample.xml";
		public const string EXEMPTED_MISSING_OPTIONAL_V1_0 = $"{V1_0}/vecto_vehicle-exempted-sample_MissingOptional.xml";
		public const string CLASS_5_V1_0 = $"{V1_0}/Tractor_4x2_vehicle-class-5_5_t_0.xml";
		public const string CLASS_5_MISSING_OPTIONAL_V1_0 = $"{V1_0}/Tractor_4x2_vehicle-class-5_5_t_0_MissingOptional.xml";
		public const string NEW_PARAMS_V1_0 = $"{V1_0}/vecto_vehicle-new_parameters-sample.xml";
		public const string LNG_V1_0 = $"{V1_0}/vecto_vehicle-sample_LNG.xml";
		public const string SAMPLE_V1_0 = $"{V1_0}/vecto_vehicle-sample.xml";
		public const string SAMPLE_AT_V1_0 = $"{V1_0}/vecto_vehicle-sample_AT.xml";
		public const string CERT_OPTIONS_V1_0 = $"{V1_0}/vecto_vehicle-sample_certificationOptions.xml";
		public const string SAMPLE_FULL_V1_0 = $"{V1_0}/vecto_vehicle-sample_FULL.xml";
		public const string SAMPLE_FULL_UPD_V1_0 = $"{V1_0}/vecto_vehicle-sample_FULL_updated.xml";
		public const string SAMPLE_NO_AIRDRAG_V1_0 = $"{V1_0}/vecto_vehicle-sample_noAirdrag.xml";
		public const string SAMPLE_NO_TORQUE_LIMITS_V1_0 = $"{V1_0}/vecto_vehicle-sample_torqueLimits.xml";

		public const string V2_0 = "TestData/Vehicles/SchemaVersion2.0";
		public const string CLASS_5_V2_0 = $"{V2_0}/Tractor_4x2_vehicle-class-5_5_t_0.xml";
		public const string MISSING_OPTIONALS_V2_0 = $"{V2_0}/vecto_vehicle_MissingOptionals.xml";
		public const string COMPONENTS_V2_0 = $"{V2_0}/vecto_vehicle-components_1.0.xml";
		public const string NEW_PARAMS_V2_0 = $"{V2_0}/vecto_vehicle-new_parameters-sample.xml";
		public const string LNG_V2_0 = $"{V2_0}/vecto_vehicle-sample_LNG.xml";

		public const string V2_1 = "TestData/Vehicles/SchemaVersion2.1";
		public const string CLASS_5_V2_1 = $"{V2_1}/Tractor_4x2_vehicle-class-5_5_t_0.xml";
		public const string EXEMPTED_V2_1 = $"{V2_1}/vecto_vehicle-exempted-sample.xml";
		public const string EXEMPTED_MISSING_OPTIONAL_V2_1 = $"{V2_1}/vecto_vehicle-exempted_MissingOptional.xml";
		public const string NEW_PARAMS_V2_1 = $"{V2_1}/vecto_vehicle-new_parameters-sample.xml";
		public const string LNG_V2_1 = $"{V2_1}/vecto_vehicle-sample_LNG.xml";
		public const string TYRE_25_V2_1 = $"{V2_1}/vecto_vehicle-tyre25.xml";

		public const string V2_2 = "TestData/Vehicles/SchemaVersion2.2";
		public const string CLASS_5_V2_2 = $"{V2_2}/Tractor_4x2_vehicle-class-5_5_t_0.xml";
		public const string LNG_V2_2 = $"{V2_2}/vecto_vehicle-sample_LNG.xml";

		public const string V2_2_1 = "TestData/Vehicles/SchemaVersion2.2.1";
		public const string EXEMPTED_V2_2_1 = $"{V2_2_1}/vecto_vehicle-exempted-sample.xml";
		public const string EXEMPTED_MISSING_OPTIONAL_V2_2_1 = $"{V2_2_1}/vecto_vehicle-exempted-sample_MissingOptional.xml";

		public const string V2_3 = "TestData/Vehicles/SchemaVersion2.3";
		public const string DUAL_MODE_DUAL_FUEL_V2_3 = $"{V2_3}/vehicle_sampleDualModeDualFuel.xml";
		public const string DUAL_MODE_DUAL_FUEL_WHR_V2_3 = $"{V2_3}/vehicle_sampleDualModeDualFuel_WHR.xml";
		public const string SINGLE_MODE_DUAL_FUEL_V2_3 = $"{V2_3}/vehicle_sampleSingleModeDualFuel.xml";
		public const string SINGLE_MODE_DUAL_FUEL_WHR_V2_3 = $"{V2_3}/vehicle_sampleSingleModeDualFuel_WHR.xml";
		public const string SINGLE_MODE_SINGLE_FUEL_WHR_V2_3 = $"{V2_3}/vehicle_sampleSingleModeSingleFuel_WHR.xml";
	}
}
