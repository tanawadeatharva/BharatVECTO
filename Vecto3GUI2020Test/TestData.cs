using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Ninject.Parameters;

namespace Vecto3GUI2020Test
{
    internal static class TestData
    {
		private const string TestDataBasePath = @"TestData\";
		private const string XMLBasePath = TestDataBasePath + @"XML\";
		private const string SchemaVersionMultistep = XMLBasePath + @"SchemaVersionMultistage.0.1\";
		private const string SchemaVersion2_4 = XMLBasePath + @"SchemaVersion2.4\";

        public const string FinalVif = SchemaVersionMultistep + "vecto_multistage_conventional_final_vif.VIF_Report_1.xml";
		public const string NewVifCompletedConventional = TestDataBasePath + "Case2/newVifCompletedConventional.vecto";
		public const string NewVifExempted = TestDataBasePath + "Case2/newVifExempted.vecto";
		public const string NewVifInterimDiesel = TestDataBasePath + "Case1/newVifInterimDiesel.vecto";
		public const string NewVifExemptedIncomplete = TestDataBasePath + "Case1/newVifExemptedIncomplete.vecto";
		public const string PrimaryHeavybusSampleXML = SchemaVersion2_4 + "vecto_vehicle-primary_heavyBus-sample.xml";
		public const string HeavylorryHevSHeavylorryS3XML = SchemaVersion2_4 + @"Distributed\HeavyLorry\HEV-S_heavyLorry_S3.xml";
		public const string primaryDecimalTestFile = $"{TestDataBasePath}/bugreports/PrimaryDecimal/primary_heavyBus group41_nonSmart_rounded_decimals.xml";
		public const string consolidated_multiple_stages = SchemaVersionMultistep + "vecto_multistage_consolidated_multiple_stages.xml";
		public const string consolidated_multiple_stages_airdrag = SchemaVersionMultistep + "vecto_multistage_consolidated_multiple_stages_airdrag.xml";
		public const string consolidated_multiple_stages_hev = SchemaVersionMultistep + "vecto_multistage_consolidated_multiple_stages_hev.xml";
		public const string consolidated_one_stage = SchemaVersionMultistep + "vecto_multistage_consolidated_one_stage.xml";
		public const string primary_vehicle_only = SchemaVersionMultistep + "vecto_multistage_primary_vehicle_only.xml";

		public const string exempted_primary_vif = SchemaVersionMultistep + "exempted_primary_heavyBus.VIF.xml";
		public const string stageInputFullSample = XMLBasePath + "vecto_vehicle-stage_input_full-sample.xml";
		public const string airdragLoadTestFile = SchemaVersionMultistep + "AirdragLoadTestFile.xml";
		public const string airdragLoadTestFilev2 = SchemaVersionMultistep + "AirdragLoadTestFilev2.xml";

		public const string exemptedCompleted = SchemaVersionMultistep + "exempted_completed.VIF_Report_2.xml";


		public const string _finalVif = "vecto_multistage_conventional_final_vif.VIF_Report_1.xml";
	}
}
