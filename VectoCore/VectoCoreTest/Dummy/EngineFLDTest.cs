using System;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Engine;

namespace TUGraz.VectoCore.Tests.Dummy
{
	[TestClass]
	public class EngineFLDTest
	{
		[TestMethod]
		public void CompareFLDLookupPerformance()
		{
			var engineFile1 = @"TestData\Components\40t_Long_Haul_Truck.vfld";
			var engineFile2 = @"E:\QUAM\Downloads\EngineFLD\Map_375c_BB1390_modTUG_R49_375c_BB1386.vfld";

			var map1 = EngineFullLoadCurve.ReadFromFile(engineFile1, true);
			var map2 = EngineFullLoadCurve.ReadFromFile(engineFile2, true);

			map1.FullLoadStationaryTorque(1000.RPMtoRad());
			map2.FullLoadStationaryTorque(1000.RPMtoRad());

			foreach (var map in new[] { map1, map2 }) {
				var rand = new Random();
				var stopWatch = Stopwatch.StartNew();


				for (var i = 0; i < 500000; i++) {
					var angularVelocity = rand.Next(1000, 1400).RPMtoRad();

					var tqMax = map.FullLoadStationaryTorque(angularVelocity);
				}

				stopWatch.Stop();
				Debug.Print("{0}", stopWatch.ElapsedMilliseconds);
			}
		}

		[TestMethod]
		public void LookupTest()
		{
			var engineFile2 = @"E:\QUAM\Downloads\EngineFLD\Map_375c_BB1390_modTUG_R49_375c_BB1386.vfld";

			var map = EngineFullLoadCurve.ReadFromFile(engineFile2, true);

			Assert.AreEqual(1208, map.FullLoadStationaryTorque(500.RPMtoRad()).Value(), 1e-3);

			Assert.AreEqual(27, map.FullLoadStationaryTorque(2202.RPMtoRad()).Value(), 1e-3);

			Assert.AreEqual(2341.5714, map.FullLoadStationaryTorque(1500.RPMtoRad()).Value(), 1e-3);
			Assert.AreEqual(376.7142, map.FullLoadStationaryTorque(2175.RPMtoRad()).Value(), 1e-3);

			Assert.AreEqual(1544, map.FullLoadStationaryTorque(628.RPMtoRad()).Value(), 1e-3);
		}
	}
}