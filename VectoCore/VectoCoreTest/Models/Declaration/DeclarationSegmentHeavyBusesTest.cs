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

		
		private void AssertMission(
			Mission m, MissionType missionType, double cdxA, double length, double width, double height, double curbMass,
			double refLoad, double lowLoad, double[] axleWeightDistribution)
		{
			Assert.AreEqual(missionType, m.MissionType);
			Assert.AreEqual(cdxA, m.DefaultCDxA.Value(), 1e-9);
			Assert.AreEqual(length, m.VehicleLength.Value(), 1e-9);
			Assert.AreEqual(width, m.VehicleWidth.Value(), 1e-9);
			Assert.AreEqual(height, m.VehicleHeight.Value(), 1e-9);
			Assert.AreEqual(curbMass, m.CurbMass.Value(), 1e-9);
			Assert.AreEqual(refLoad, m.RefLoad.Value(), 1e-9);
			Assert.AreEqual(lowLoad, m.LowLoad.Value(), 1e-9);
			CollectionAssert.AreEqual(axleWeightDistribution, m.AxleWeightDistribution,
									"Axle distribution not equal.\nexpected: {0}\nactual: {1}", string.Join(",", axleWeightDistribution),
									string.Join(",", m.AxleWeightDistribution));
		}
	}
}
