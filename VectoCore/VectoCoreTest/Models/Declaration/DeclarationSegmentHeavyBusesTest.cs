using System;
using NUnit.Framework;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Declaration;

namespace TUGraz.VectoCore.Tests.Models.Declaration
{
	[TestFixture]
	public class DeclarationSegmentHeavyBusesTest
	{
		[
			TestCase(
				VehicleCategory.HeavyBusPrimaryVehicle, AxleConfiguration.AxleConfig_4x2, FloorType.LowFloor, false, false,
				VehicleClass.ClassPB41),
			TestCase(
				VehicleCategory.HeavyBusPrimaryVehicle, AxleConfiguration.AxleConfig_4x2, FloorType.HighFloor, true, false,
				VehicleClass.ClassPB41),
			TestCase(
				VehicleCategory.HeavyBusPrimaryVehicle, AxleConfiguration.AxleConfig_4x2, FloorType.LowFloor, false, true,
				VehicleClass.ClassPB41),
			TestCase(
				VehicleCategory.HeavyBusPrimaryVehicle, AxleConfiguration.AxleConfig_4x2, FloorType.HighFloor, true, true,
				VehicleClass.ClassPB41),
			TestCase(
				VehicleCategory.HeavyBusPrimaryVehicle, AxleConfiguration.AxleConfig_6x2, FloorType.LowFloor, false, false,
				VehicleClass.ClassPB42),
			TestCase(
				VehicleCategory.HeavyBusPrimaryVehicle, AxleConfiguration.AxleConfig_6x2, FloorType.HighFloor, true, false,
				VehicleClass.ClassPB42),
			TestCase(
				VehicleCategory.HeavyBusPrimaryVehicle, AxleConfiguration.AxleConfig_6x4, FloorType.LowFloor, false, false,
				VehicleClass.ClassPB42),
			TestCase(
				VehicleCategory.HeavyBusPrimaryVehicle, AxleConfiguration.AxleConfig_6x4, FloorType.HighFloor, true, false,
				VehicleClass.ClassPB42),
			TestCase(
				VehicleCategory.HeavyBusPrimaryVehicle, AxleConfiguration.AxleConfig_6x2, FloorType.LowFloor, false, true,
				VehicleClass.ClassPB43),
			TestCase(
				VehicleCategory.HeavyBusPrimaryVehicle, AxleConfiguration.AxleConfig_6x2, FloorType.HighFloor, true, true,
				VehicleClass.ClassPB43),
			TestCase(
				VehicleCategory.HeavyBusPrimaryVehicle, AxleConfiguration.AxleConfig_6x4, FloorType.LowFloor, false, true,
				VehicleClass.ClassPB43),
			TestCase(
				VehicleCategory.HeavyBusPrimaryVehicle, AxleConfiguration.AxleConfig_6x4, FloorType.HighFloor, true, true,
				VehicleClass.ClassPB43),
			TestCase(
				VehicleCategory.HeavyBusPrimaryVehicle, AxleConfiguration.AxleConfig_8x2, FloorType.LowFloor, false, false,
				VehicleClass.ClassPB44),
			TestCase(
				VehicleCategory.HeavyBusPrimaryVehicle, AxleConfiguration.AxleConfig_8x2, FloorType.HighFloor, true, false,
				VehicleClass.ClassPB44),
			TestCase(
				VehicleCategory.HeavyBusPrimaryVehicle, AxleConfiguration.AxleConfig_8x4, FloorType.LowFloor, false, false,
				VehicleClass.ClassPB44),
			TestCase(
				VehicleCategory.HeavyBusPrimaryVehicle, AxleConfiguration.AxleConfig_8x4, FloorType.HighFloor, true, false,
				VehicleClass.ClassPB44),
			TestCase(
				VehicleCategory.HeavyBusPrimaryVehicle, AxleConfiguration.AxleConfig_8x2, FloorType.LowFloor, false, true,
				VehicleClass.ClassPB45),
			TestCase(
				VehicleCategory.HeavyBusPrimaryVehicle, AxleConfiguration.AxleConfig_8x2, FloorType.HighFloor, true, true,
				VehicleClass.ClassPB45),
			TestCase(
				VehicleCategory.HeavyBusPrimaryVehicle, AxleConfiguration.AxleConfig_8x4, FloorType.LowFloor, false, true,
				VehicleClass.ClassPB45),
			TestCase(
				VehicleCategory.HeavyBusPrimaryVehicle, AxleConfiguration.AxleConfig_8x4, FloorType.HighFloor, true, true,
				VehicleClass.ClassPB45),
		]
		public void SegmentLookupTest(
			VehicleCategory category, AxleConfiguration axleConfiguration, FloorType floorType, bool doubleDecker,
			bool articulated, VehicleClass expectedClass)
		{
			var segment = DeclarationData.BusSegments.Lookup(
				category, axleConfiguration, articulated, floorType, doubleDecker, true);

			Assert.AreEqual(10, segment.Missions.Length);
			Assert.AreEqual(expectedClass, segment.VehicleClass);
		}


		[TestCase()]
		public void TestPrimaryBusGroup41Test()
		{
			var vehicleData = new {
				VehicleCategory = VehicleCategory.HeavyBusPrimaryVehicle,
				AxleConfiguration = AxleConfiguration.AxleConfig_4x2,
				Articulated = false,
				FloorType = FloorType.LowFloor,
				DoubleDecker = false
			};
			var segment = DeclarationData.BusSegments.Lookup(
				vehicleData.VehicleCategory, vehicleData.AxleConfiguration, vehicleData.Articulated, vehicleData.FloorType,
				vehicleData.DoubleDecker, true);

			Assert.AreEqual(10, segment.Missions.Length);
			Assert.AreEqual(VehicleClass.ClassPB41, segment.VehicleClass);


			var missions = new[]
				{ MissionType.HeavyUrban, MissionType.Urban, MissionType.Suburban, MissionType.Interurban, MissionType.Coach };

			for (var i = 0; i < 3; i++)
			{
				AssertMission(
					segment.Missions[i],
					missionType: missions[i],
					cdxA: 4.9,
					length: 12,
					width: 2.55,
					height: 2.8,
					curbMass: 10000,
					refLoad: 5618.16,
					lowLoad: 1123.632,
					axleWeightDistribution: new[] { 0.375, 0.625 }
				);
			}
			for (var i = 3; i < 6; i++) {
				AssertMission(
					segment.Missions[i],
					missionType: missions[i % 3],
					cdxA: 6.2,
					length: 10.5,
					width: 2.55,
					height: 3.8,
					curbMass: 10000,
					refLoad: 5966.694,
					lowLoad: 1193.3388,
					axleWeightDistribution: new[] { 0.375, 0.625 }
				);
			}
			
			AssertMission(
				segment.Missions[6],
				missionType: MissionType.Interurban,
				cdxA: 4.6,
				length: 12,
				width: 2.55,
				height: 3.15,
				curbMass: 10000,
				refLoad: 4301.748,
				lowLoad: 860.3496,
				axleWeightDistribution: new[] { 0.375, 0.625 }
			);
			AssertMission(
				segment.Missions[7],
				missionType: MissionType.Coach,
				cdxA: 4.6,
				length: 12,
				width: 2.55,
				height: 3.15,
				curbMass: 10000,
				refLoad: 2737.476,
				lowLoad: 547.4952,
				axleWeightDistribution: new[] { 0.375, 0.625 }
			);

			AssertMission(
					segment.Missions[8],
					missionType: MissionType.Interurban,
					cdxA: 5.2,
					length: 10.5,
					width: 2.55,
					height: 3.7,
					curbMass: 10000,
					refLoad: 5051.295,
					lowLoad: 1010.259,
					axleWeightDistribution: new[] { 0.375, 0.625 }
				);
			AssertMission(
				segment.Missions[9],
				missionType: MissionType.Coach,
				cdxA: 5.2,
				length: 10.5,
				width: 2.55,
				height: 3.7,
				curbMass: 10000,
				refLoad: 3367.53,
				lowLoad: 673.506,
				axleWeightDistribution: new[] { 0.375, 0.625 }
			);

		}

		[TestCase()]
		public void TestPrimaryBusGroup42Test()
		{
			var vehicleData = new {
				VehicleCategory = VehicleCategory.HeavyBusPrimaryVehicle,
				AxleConfiguration = AxleConfiguration.AxleConfig_6x2,
				Articulated = false,
				FloorType = FloorType.LowFloor,
				DoubleDecker = false
			};
			var segment = DeclarationData.BusSegments.Lookup(
				vehicleData.VehicleCategory, vehicleData.AxleConfiguration, vehicleData.Articulated, vehicleData.FloorType,
				vehicleData.DoubleDecker, true);

			Assert.AreEqual(10, segment.Missions.Length);
			Assert.AreEqual(VehicleClass.ClassPB42, segment.VehicleClass);

			var missions = new[]
				{ MissionType.HeavyUrban, MissionType.Urban, MissionType.Suburban, MissionType.Interurban, MissionType.Coach };

			for (var i = 0; i < 3; i++) {
				AssertMission(
					segment.Missions[i],
					missionType: missions[i],
					cdxA: 5.0,
					length: 14.2,
					width: 2.55,
					height: 2.8,
					curbMass: 12000,
					refLoad: 6762.6,
					lowLoad: 1352.52,
					axleWeightDistribution: new[] { 0.273, 0.454, 0.273 }
				);
			}
			for (var i = 3; i < 6; i++) {
				AssertMission(
					segment.Missions[i],
					missionType: missions[i % 3],
					cdxA: 6.3,
					length: 13.5,
					width: 2.55,
					height: 3.8,
					curbMass: 12000,
					refLoad: 7891.434,
					lowLoad: 1578.2868,
					axleWeightDistribution: new[] { 0.273, 0.454, 0.273 }
				);
			}

			AssertMission(
				segment.Missions[6],
				missionType: MissionType.Interurban,
				cdxA: 4.7,
				length: 13.8,
				width: 2.55,
				height: 3.15,
				curbMass: 12000,
				refLoad: 5018.706,
				lowLoad: 1003.7412,
				axleWeightDistribution: new[] { 0.273, 0.454, 0.273 }
			);
			AssertMission(
				segment.Missions[7],
				missionType: MissionType.Coach,
				cdxA: 4.7,
				length: 13.8,
				width: 2.55,
				height: 3.15,
				curbMass: 12000,
				refLoad: 3193.722,
				lowLoad: 638.7444,
				axleWeightDistribution: new[] { 0.273, 0.454, 0.273 }
			);

			AssertMission(
					segment.Missions[8],
					missionType: MissionType.Interurban,
					cdxA: 5.3,
					length: 14,
					width: 2.55,
					height: 3.7,
					curbMass: 12000,
					refLoad: 6952.32,
					lowLoad: 1390.464,
					axleWeightDistribution: new[] { 0.273, 0.454, 0.273 }
				);
			AssertMission(
				segment.Missions[9],
				missionType: MissionType.Coach,
				cdxA: 5.3,
				length: 14,
				width: 2.55,
				height: 3.7,
				curbMass: 12000,
				refLoad: 4634.88,
				lowLoad: 926.976,
				axleWeightDistribution: new[] { 0.273, 0.454, 0.273 }
			);

		}

		[TestCase()]
		public void TestPrimaryBusGroup43Test()
		{
			var vehicleData = new {
				VehicleCategory = VehicleCategory.HeavyBusPrimaryVehicle,
				AxleConfiguration = AxleConfiguration.AxleConfig_6x2,
				Articulated = true,
				FloorType = FloorType.LowFloor,
				DoubleDecker = false
			};
			var segment = DeclarationData.BusSegments.Lookup(
				vehicleData.VehicleCategory, vehicleData.AxleConfiguration, vehicleData.Articulated, vehicleData.FloorType,
				vehicleData.DoubleDecker, true);

			Assert.AreEqual(10, segment.Missions.Length);
			Assert.AreEqual(VehicleClass.ClassPB43, segment.VehicleClass);

			var missions = new[]
				{ MissionType.HeavyUrban, MissionType.Urban, MissionType.Suburban, MissionType.Interurban, MissionType.Coach };

			for (var i = 0; i < 3; i++) {
				AssertMission(
					segment.Missions[i],
					missionType: missions[i],
					cdxA: 5.1,
					length: 18.2,
					width: 2.55,
					height: 2.8,
					curbMass: 12000,
					refLoad: 8843.4,
					lowLoad: 1768.68,
					axleWeightDistribution: new[] { 0.243, 0.352, 0.405 }
				);
			}
			for (var i = 3; i < 6; i++) {
				AssertMission(
					segment.Missions[i],
					missionType: missions[i % 3],
					cdxA: 6.4,
					length: 18.2,
					width: 2.55,
					height: 3.8,
					curbMass: 12000,
					refLoad: 10906.86,
					lowLoad: 2181.372,
					axleWeightDistribution: new[] { 0.243, 0.352, 0.405 }
				);
			}

			AssertMission(
				segment.Missions[6],
				missionType: MissionType.Interurban,
				cdxA: 4.8,
				length: 18.2,
				width: 2.55,
				height: 3.15,
				curbMass: 12000,
				refLoad: 6771.27,
				lowLoad: 1354.254,
				axleWeightDistribution: new[] { 0.243, 0.352, 0.405 }
			);
			AssertMission(
				segment.Missions[7],
				missionType: MissionType.Coach,
				cdxA: 4.8,
				length: 18.2,
				width: 2.55,
				height: 3.15,
				curbMass: 12000,
				refLoad: 4308.99,
				lowLoad: 861.798,
				axleWeightDistribution: new[] { 0.243, 0.352, 0.405 }
			);

			AssertMission(
					segment.Missions[8],
					missionType: MissionType.Interurban,
					cdxA: 5.4,
					length: 18.2,
					width: 2.55,
					height: 3.7,
					curbMass: 12000,
					refLoad: 9233.55,
					lowLoad: 1846.71,
					axleWeightDistribution: new[] { 0.243, 0.352, 0.405 }
				);
			AssertMission(
				segment.Missions[9],
				missionType: MissionType.Coach,
				cdxA: 5.4,
				length: 18.2,
				width: 2.55,
				height: 3.7,
				curbMass: 12000,
				refLoad: 6155.7,
				lowLoad: 1231.14,
				axleWeightDistribution: new[] { 0.243, 0.352, 0.405 }
			);

		}

		[TestCase()]
		public void TestPrimaryBusGroup44Test()
		{
			var vehicleData = new {
				VehicleCategory = VehicleCategory.HeavyBusPrimaryVehicle,
				AxleConfiguration = AxleConfiguration.AxleConfig_8x2,
				Articulated = false,
				FloorType = FloorType.LowFloor,
				DoubleDecker = false
			};
			var segment = DeclarationData.BusSegments.Lookup(
				vehicleData.VehicleCategory, vehicleData.AxleConfiguration, vehicleData.Articulated, vehicleData.FloorType,
				vehicleData.DoubleDecker, true);

			Assert.AreEqual(10, segment.Missions.Length);
			Assert.AreEqual(VehicleClass.ClassPB44, segment.VehicleClass);

			var missions = new[]
				{ MissionType.HeavyUrban, MissionType.Urban, MissionType.Suburban, MissionType.Interurban, MissionType.Coach };

			for (var i = 0; i < 3; i++) {
				AssertMission(
					segment.Missions[i],
					missionType: missions[i],
					cdxA: 5.1,
					length: 15,
					width: 2.55,
					height: 2.8,
					curbMass: 14000,
					refLoad: 7178.76,
					lowLoad: 1435.752,
					axleWeightDistribution: new[] { 0.214, 0.214, 0.358, 0.214 }
				);
			}
			for (var i = 3; i < 6; i++) {
				AssertMission(
					segment.Missions[i],
					missionType: missions[i % 3],
					cdxA: 6.4,
					length: 15,
					width: 2.55,
					height: 3.8,
					curbMass: 14000,
					refLoad: 8853.804,
					lowLoad: 1770.7608,
					axleWeightDistribution: new[] { 0.214, 0.214, 0.358, 0.214 }
				);
			}

			AssertMission(
				segment.Missions[6],
				missionType: MissionType.Interurban,
				cdxA: 4.8,
				length: 15,
				width: 2.55,
				height: 3.15,
				curbMass: 14000,
				refLoad: 5496.6780,
				lowLoad: 1099.3356,
				axleWeightDistribution: new[] { 0.214, 0.214, 0.358, 0.214 }
			);
			AssertMission(
				segment.Missions[7],
				missionType: MissionType.Coach,
				cdxA: 4.8,
				length: 15,
				width: 2.55,
				height: 3.15,
				curbMass: 14000,
				refLoad: 3497.886,
				lowLoad: 699.5772,
				axleWeightDistribution: new[] { 0.214, 0.214, 0.358, 0.214 }
			);

			AssertMission(
					segment.Missions[8],
					missionType: MissionType.Interurban,
					cdxA: 5.4,
					length: 15,
					width: 2.55,
					height: 3.7,
					curbMass: 14000,
					refLoad: 7495.47,
					lowLoad: 1499.094,
					axleWeightDistribution: new[] { 0.214, 0.214, 0.358, 0.214 }
				);
			AssertMission(
				segment.Missions[9],
				missionType: MissionType.Coach,
				cdxA: 5.4,
				length: 15,
				width: 2.55,
				height: 3.7,
				curbMass: 14000,
				refLoad: 4996.98,
				lowLoad: 999.396,
				axleWeightDistribution: new[] { 0.214, 0.214, 0.358, 0.214 }
			);

		}

		[TestCase()]
		public void TestPrimaryBusGroup45Test()
		{
			var vehicleData = new {
				VehicleCategory = VehicleCategory.HeavyBusPrimaryVehicle,
				AxleConfiguration = AxleConfiguration.AxleConfig_8x2,
				Articulated = true,
				FloorType = FloorType.LowFloor,
				DoubleDecker = false
			};
			var segment = DeclarationData.BusSegments.Lookup(
				vehicleData.VehicleCategory, vehicleData.AxleConfiguration, vehicleData.Articulated, vehicleData.FloorType,
				vehicleData.DoubleDecker, true);

			Assert.AreEqual(10, segment.Missions.Length);
			Assert.AreEqual(VehicleClass.ClassPB45, segment.VehicleClass);

			var missions = new[]
				{ MissionType.HeavyUrban, MissionType.Urban, MissionType.Suburban, MissionType.Interurban, MissionType.Coach };

			for (var i = 0; i < 3; i++) {
				AssertMission(
					segment.Missions[i],
					missionType: missions[i],
					cdxA: 5.2,
					length: 21,
					width: 2.55,
					height: 2.6,
					curbMass: 14000,
					refLoad: 10299.96,
					lowLoad: 2059.992,
					axleWeightDistribution: new[] { 0.200, 0.282, 0.324, 0.194 }
				);
			}
			for (var i = 3; i < 6; i++) {
				AssertMission(
					segment.Missions[i],
					missionType: missions[i % 3],
					cdxA: 6.5,
					length: 21,
					width: 2.55,
					height: 3.8,
					curbMass: 14000,
					refLoad: 12703.284,
					lowLoad: 2540.6568,
					axleWeightDistribution: new[] { 0.200, 0.282, 0.324, 0.194 }
				);
			}

			AssertMission(
				segment.Missions[6],
				missionType: MissionType.Interurban,
				cdxA: 4.9,
				length: 21,
				width: 2.55,
				height: 3.15,
				curbMass: 14000,
				refLoad: 7886.538,
				lowLoad: 1577.3076,
				axleWeightDistribution: new[] { 0.200, 0.282, 0.324, 0.194 }
			);
			AssertMission(
				segment.Missions[7],
				missionType: MissionType.Coach,
				cdxA: 4.9,
				length: 21,
				width: 2.55,
				height: 3.15,
				curbMass: 14000,
				refLoad: 5018.706,
				lowLoad: 1003.7412,
				axleWeightDistribution: new[] { 0.200, 0.282, 0.324, 0.194 }
			);

			AssertMission(
					segment.Missions[8],
					missionType: MissionType.Interurban,
					cdxA: 5.5,
					length: 21,
					width: 2.55,
					height: 3.7,
					curbMass: 14000,
					refLoad: 10754.37,
					lowLoad: 2150.874,
					axleWeightDistribution: new[] { 0.200, 0.282, 0.324, 0.194 }
				);
			AssertMission(
				segment.Missions[9],
				missionType: MissionType.Coach,
				cdxA: 5.5,
				length: 21,
				width: 2.55,
				height: 3.7,
				curbMass: 14000,
				refLoad: 7169.58,
				lowLoad: 1433.916,
				axleWeightDistribution: new[] { 0.200, 0.282, 0.324, 0.194 }
			);

		}

		
		private void AssertMission(
			Mission m, MissionType missionType, double cdxA, double length, double width, double height, double curbMass,
			double refLoad, double lowLoad, double[] axleWeightDistribution)
		{
			Assert.AreEqual(missionType, m.MissionType);
			Assert.AreEqual(cdxA, m.DefaultCDxA.Value(), 1e-9);
			Assert.AreEqual(length, m.BusParameter.VehicleLength.Value(), 1e-9);
			Assert.AreEqual(width, m.BusParameter.VehicleWidth.Value(), 1e-9);
			Assert.AreEqual(height, m.BusParameter.BodyHeight.Value(), 1e-9);
			Assert.AreEqual(curbMass, m.CurbMass.Value(), 1e-9);
			Assert.AreEqual(refLoad, m.RefLoad.Value(), 1e-9);
			Assert.AreEqual(lowLoad, m.LowLoad.Value(), 1e-9);
			foreach (var tuple in axleWeightDistribution.ZipAll(m.AxleWeightDistribution, Tuple.Create)) {
				Assert.AreEqual(tuple.Item1, tuple.Item2, 1e-0, "Axle distribution not equal.\nexpected: {0}\nactual: {1}", string.Join(",", axleWeightDistribution),
								string.Join(",", m.AxleWeightDistribution));
			}

		}
	}
}
