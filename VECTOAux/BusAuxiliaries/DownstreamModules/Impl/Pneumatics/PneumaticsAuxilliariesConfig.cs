// Copyright 2017 European Union.
// Licensed under the EUPL (the 'Licence');
// 
// * You may not use this work except in compliance with the Licence.
// * You may obtain a copy of the Licence at: http://ec.europa.eu/idabc/eupl
// * Unless required by applicable law or agreed to in writing,
// software distributed under the Licence is distributed on an "AS IS" basis,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// 
// See the LICENSE.txt for the specific language governing permissions and limitations.

using TUGraz.VectoCore.BusAuxiliaries.Interfaces.DownstreamModules.PneumaticSystem;

namespace TUGraz.VectoCore.BusAuxiliaries.DownstreamModules.Impl.Pneumatics
{
	public class PneumaticsAuxilliariesConfig : IPneumaticsAuxilliariesConfig
	{
		public double AdBlueNIperMinute { get; set; }

		public double AirControlledSuspensionNIperMinute { get; set; }

		public double BrakingNoRetarderNIperKG { get; set; }

		public double BrakingWithRetarderNIperKG { get; set; }

		public double BreakingPerKneelingNIperKGinMM { get; set; }

		public double DeadVolBlowOutsPerLitresperHour { get; set; }

		public double DeadVolumeLitres { get; set; }

		public double NonSmartRegenFractionTotalAirDemand { get; set; }

		public double OverrunUtilisationForCompressionFraction { get; set; }

		public double PerDoorOpeningNI { get; set; }

		public double PerStopBrakeActuationNIperKG { get; set; }

		public double SmartRegenFractionTotalAirDemand { get; set; }


		public PneumaticsAuxilliariesConfig(bool setToDefaults = false)
		{
			if (setToDefaults)
				SetDefaults();
		}

		public void SetDefaults()
		{
			AdBlueNIperMinute = 21.25;
			AirControlledSuspensionNIperMinute = 15;
			BrakingNoRetarderNIperKG = 0.00081;
			BrakingWithRetarderNIperKG = 0.0006;
			BreakingPerKneelingNIperKGinMM = 0.000066;
			DeadVolBlowOutsPerLitresperHour = 24;
			DeadVolumeLitres = 30;
			NonSmartRegenFractionTotalAirDemand = 0.26;
			OverrunUtilisationForCompressionFraction = 0.97;
			PerDoorOpeningNI = 12.7;
			PerStopBrakeActuationNIperKG = 0.00064;
			SmartRegenFractionTotalAirDemand = 0.12;
		}
	}
}
