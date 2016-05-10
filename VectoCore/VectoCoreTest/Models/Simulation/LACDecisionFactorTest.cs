using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Tests.Utils;

namespace TUGraz.VectoCore.Tests.Models.Simulation
{
	[TestClass]
	public class LACDecisionFactorTest
	{
		[TestMethod]
		public void LAC_DF_Test()
		{
			var lac = new LACDecisionFactor();

			for (var vVehicle = 0.SI<MeterPerSecond>();
				vVehicle < 100.KMPHtoMeterPerSecond();
				vVehicle += 1.KMPHtoMeterPerSecond()) {
				for (var vTarget = vVehicle; vTarget > 0; vTarget -= 1.KMPHtoMeterPerSecond()) {
					var df_coast = lac.Lookup(vTarget, vVehicle - vTarget);
					if (vTarget < 48.KMPHtoMeterPerSecond())
						AssertHelper.AreRelativeEqual(df_coast, 2.5, string.Format("vVehicle: {0}, vTarget: {1}", vVehicle, vTarget));

					if (vVehicle - vTarget > 11)
						AssertHelper.AreRelativeEqual(df_coast, 2.5, string.Format("vVehicle: {0}, vTarget: {1}", vVehicle, vTarget));

					if (vTarget > 52.KMPHtoMeterPerSecond() && vVehicle - vTarget < 9.KMPHtoMeterPerSecond())
						AssertHelper.AreRelativeEqual(df_coast, 1.0, string.Format("vVehicle: {0}, vTarget: {1}", vVehicle, vTarget));
				}
			}
		}
	}
}