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

			for (var i = 0; i < 3; i++) {
				AssertMission(
					segment.Missions[i],
					missionType: missions[i],
					cdxA: 4.9,
					length: 12,
					width: 2.55,
					height: 2.7,
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
					length: 12,
					width: 2.55,
					height: 3.7,
					curbMass: 10000,
					refLoad: 6929.064,
					lowLoad: 1385.8128,
					axleWeightDistribution: new[] { 0.375, 0.625 }
				);
			}
			
			AssertMission(
				segment.Missions[6],
				missionType: MissionType.Interurban,
				cdxA: 4.6,
				length: 12,
				width: 2.55,
				height: 3.0,
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
				height: 3.0,
				curbMass: 10000,
				refLoad: 2737.476,
				lowLoad: 547.4952,
				axleWeightDistribution: new[] { 0.375, 0.625 }
			);

			AssertMission(
					segment.Missions[8],
					missionType: MissionType.Interurban,
					cdxA: 5.2,
					length: 12,
					width: 2.55,
					height: 3.9,
					curbMass: 10000,
					refLoad: 5866.02,
					lowLoad: 1173.204,
					axleWeightDistribution: new[] { 0.375, 0.625 }
				);
			AssertMission(
				segment.Missions[9],
				missionType: MissionType.Coach,
				cdxA: 5.2,
				length: 12,
				width: 2.55,
				height: 3.9,
				curbMass: 10000,
				refLoad: 3910.68,
				lowLoad: 782.136,
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
					length: 14,
					width: 2.55,
					height: 2.7,
					curbMass: 12000,
					refLoad: 6658.56,
					lowLoad: 1331.712,
					axleWeightDistribution: new[] { 0.273, 0.454, 0.273 }
				);
			}
			for (var i = 3; i < 6; i++) {
				AssertMission(
					segment.Missions[i],
					missionType: missions[i % 3],
					cdxA: 6.3,
					length: 14,
					width: 2.55,
					height: 3.7,
					curbMass: 12000,
					refLoad: 8212.224,
					lowLoad: 1642.4448,
					axleWeightDistribution: new[] { 0.273, 0.454, 0.273 }
				);
			}

			AssertMission(
				segment.Missions[6],
				missionType: MissionType.Interurban,
				cdxA: 4.7,
				length: 14,
				width: 2.55,
				height: 3.0,
				curbMass: 12000,
				refLoad: 5098.368,
				lowLoad: 1019.6736,
				axleWeightDistribution: new[] { 0.273, 0.454, 0.273 }
			);
			AssertMission(
				segment.Missions[7],
				missionType: MissionType.Coach,
				cdxA: 4.7,
				length: 14,
				width: 2.55,
				height: 3.0,
				curbMass: 12000,
				refLoad: 3244.416,
				lowLoad: 648.8832,
				axleWeightDistribution: new[] { 0.273, 0.454, 0.273 }
			);

			AssertMission(
					segment.Missions[8],
					missionType: MissionType.Interurban,
					cdxA: 5.3,
					length: 14,
					width: 2.55,
					height: 3.9,
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
				height: 3.9,
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
					length: 18,
					width: 2.55,
					height: 2.7,
					curbMass: 12000,
					refLoad: 8739.36,
					lowLoad: 1747.872,
					axleWeightDistribution: new[] { 0.243, 0.352, 0.405 }
				);
			}
			for (var i = 3; i < 6; i++) {
				AssertMission(
					segment.Missions[i],
					missionType: missions[i % 3],
					cdxA: 6.4,
					length: 18,
					width: 2.55,
					height: 3.7,
					curbMass: 12000,
					refLoad: 10778.544,
					lowLoad: 2155.7088,
					axleWeightDistribution: new[] { 0.243, 0.352, 0.405 }
				);
			}

			AssertMission(
				segment.Missions[6],
				missionType: MissionType.Interurban,
				cdxA: 4.8,
				length: 18,
				width: 2.55,
				height: 3.0,
				curbMass: 12000,
				refLoad: 6691.608,
				lowLoad: 1338.3216,
				axleWeightDistribution: new[] { 0.243, 0.352, 0.405 }
			);
			AssertMission(
				segment.Missions[7],
				missionType: MissionType.Coach,
				cdxA: 4.8,
				length: 18,
				width: 2.55,
				height: 3.0,
				curbMass: 12000,
				refLoad: 4258.296,
				lowLoad: 851.6592,
				axleWeightDistribution: new[] { 0.243, 0.352, 0.405 }
			);

			AssertMission(
					segment.Missions[8],
					missionType: MissionType.Interurban,
					cdxA: 5.4,
					length: 18,
					width: 2.55,
					height: 3.9,
					curbMass: 12000,
					refLoad: 9124.92,
					lowLoad: 1824.984,
					axleWeightDistribution: new[] { 0.243, 0.352, 0.405 }
				);
			AssertMission(
				segment.Missions[9],
				missionType: MissionType.Coach,
				cdxA: 5.4,
				length: 18,
				width: 2.55,
				height: 3.9,
				curbMass: 12000,
				refLoad: 6083.28,
				lowLoad: 1216.656,
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
					height: 2.7,
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
					height: 3.7,
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
				height: 3.0,
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
				height: 3.0,
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
					height: 3.9,
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
				height: 3.9,
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
					length: 20,
					width: 2.55,
					height: 2.7,
					curbMass: 14000,
					refLoad: 9779.76,
					lowLoad: 1955.952,
					axleWeightDistribution: new[] { 0.200, 0.282, 0.324, 0.194 }
				);
			}
			for (var i = 3; i < 6; i++) {
				AssertMission(
					segment.Missions[i],
					missionType: missions[i % 3],
					cdxA: 6.5,
					length: 20,
					width: 2.55,
					height: 3.7,
					curbMass: 14000,
					refLoad: 12061.704,
					lowLoad: 2412.3408,
					axleWeightDistribution: new[] { 0.200, 0.282, 0.324, 0.194 }
				);
			}

			AssertMission(
				segment.Missions[6],
				missionType: MissionType.Interurban,
				cdxA: 4.9,
				length: 20,
				width: 2.55,
				height: 3.0,
				curbMass: 14000,
				refLoad: 7488.228,
				lowLoad: 1497.6456,
				axleWeightDistribution: new[] { 0.200, 0.282, 0.324, 0.194 }
			);
			AssertMission(
				segment.Missions[7],
				missionType: MissionType.Coach,
				cdxA: 4.9,
				length: 20,
				width: 2.55,
				height: 3.0,
				curbMass: 14000,
				refLoad: 4765.236,
				lowLoad: 953.0472,
				axleWeightDistribution: new[] { 0.200, 0.282, 0.324, 0.194 }
			);

			AssertMission(
					segment.Missions[8],
					missionType: MissionType.Interurban,
					cdxA: 5.5,
					length: 20,
					width: 2.55,
					height: 3.9,
					curbMass: 14000,
					refLoad: 10211.22,
					lowLoad: 2042.244,
					axleWeightDistribution: new[] { 0.200, 0.282, 0.324, 0.194 }
				);
			AssertMission(
				segment.Missions[9],
				missionType: MissionType.Coach,
				cdxA: 5.5,
				length: 20,
				width: 2.55,
				height: 3.9,
				curbMass: 14000,
				refLoad: 6807.48,
				lowLoad: 1361.496,
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
			Assert.AreEqual(height, m.VehicleHeight.Value(), 1e-9);
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
