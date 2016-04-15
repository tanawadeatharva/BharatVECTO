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
	}
}