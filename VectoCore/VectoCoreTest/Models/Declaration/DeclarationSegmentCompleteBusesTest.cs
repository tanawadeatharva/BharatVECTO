using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.Models.Declaration;

namespace TUGraz.VectoCore.Tests.Models.Declaration
{
	[TestFixture]
	public class DeclarationSegmentCompleteBusesTest
	{
		[
			TestCase(2, VehicleCode.CE, "31a"),
			TestCase(2, VehicleCode.CE, "31b"),
			TestCase(2, VehicleCode.CF, "31c"),
			TestCase(2, VehicleCode.CI, "31d"),
			TestCase(2, VehicleCode.CJ, "31e"),
			TestCase(2, VehicleCode.CA, "32a"),
			TestCase(2, VehicleCode.CA, "31b"),
			TestCase(2, VehicleCode.CA, "31c"),
			TestCase(2, VehicleCode.CA, "31d"),
			TestCase(2, VehicleCode.CB, "31e"),
			TestCase(2, VehicleCode.CB, "31f"),
		]
		public void SegmentLookupTest(int numberOfAxles, VehicleCode vehicleCode, string vehicleParameterGroup)
		{

			var segment = DeclarationData.CompletedBusSegments.Lookup(numberOfAxles, vehicleCode, vehicleParameterGroup);

			//Assert.AreEqual(10, segment.Missions.Length);
		}
	}
}
