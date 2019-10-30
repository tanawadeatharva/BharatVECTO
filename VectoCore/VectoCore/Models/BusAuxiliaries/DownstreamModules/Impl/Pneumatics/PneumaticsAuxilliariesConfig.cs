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

using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.Pneumatics
{
	public class PneumaticsAuxilliariesConfig : IPneumaticsAuxilliariesConfig
	{
		public NormLiterPerSecond AdBlueNIperMinute { get; set; }

		public NormLiterPerSecond AirControlledSuspensionNIperMinute { get; set; }

		public NormLiterPerKilogram BrakingNoRetarderNIperKG { get; set; }

		public NormLiterPerKilogram BrakingWithRetarderNIperKG { get; set; }

		public NormLiterPerKilogramMeter BreakingPerKneelingNIperKGinMM { get; set; }

		public PerSecond DeadVolBlowOutsPerLitresperHour { get; set; }

		public NormLiter DeadVolumeLitres { get; set; }

		public double NonSmartRegenFractionTotalAirDemand { get; set; }

		public double OverrunUtilisationForCompressionFraction { get; set; }

		public NormLiter PerDoorOpeningNI { get; set; }

		public NormLiterPerKilogram PerStopBrakeActuationNIperKG { get; set; }

		public double SmartRegenFractionTotalAirDemand { get; set; }

	}
}
