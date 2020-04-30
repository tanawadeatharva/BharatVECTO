using System;
using System.Collections.Generic;
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
				VehicleClass.ClassP31_32),
			TestCase(
				VehicleCategory.HeavyBusPrimaryVehicle, AxleConfiguration.AxleConfig_4x2, FloorType.HighFloor, true, false,
				VehicleClass.ClassP31_32),
			TestCase(
				VehicleCategory.HeavyBusPrimaryVehicle, AxleConfiguration.AxleConfig_4x2, FloorType.LowFloor, false, true,
				VehicleClass.ClassP31_32),
			TestCase(
				VehicleCategory.HeavyBusPrimaryVehicle, AxleConfiguration.AxleConfig_4x2, FloorType.HighFloor, true, true,
				VehicleClass.ClassP31_32),
			TestCase(
				VehicleCategory.HeavyBusPrimaryVehicle, AxleConfiguration.AxleConfig_6x2, FloorType.LowFloor, false, false,
				VehicleClass.ClassP33_34),
			TestCase(
				VehicleCategory.HeavyBusPrimaryVehicle, AxleConfiguration.AxleConfig_6x2, FloorType.HighFloor, true, false,
				VehicleClass.ClassP33_34),
			TestCase(
				VehicleCategory.HeavyBusPrimaryVehicle, AxleConfiguration.AxleConfig_6x4, FloorType.LowFloor, false, false,
				VehicleClass.ClassP33_34),
			TestCase(
				VehicleCategory.HeavyBusPrimaryVehicle, AxleConfiguration.AxleConfig_6x4, FloorType.HighFloor, true, false,
				VehicleClass.ClassP33_34),
			TestCase(
				VehicleCategory.HeavyBusPrimaryVehicle, AxleConfiguration.AxleConfig_6x2, FloorType.LowFloor, false, true,
				VehicleClass.ClassP35_36),
			TestCase(
				VehicleCategory.HeavyBusPrimaryVehicle, AxleConfiguration.AxleConfig_6x2, FloorType.HighFloor, true, true,
				VehicleClass.ClassP35_36),
			TestCase(
				VehicleCategory.HeavyBusPrimaryVehicle, AxleConfiguration.AxleConfig_6x4, FloorType.LowFloor, false, true,
				VehicleClass.ClassP35_36),
			TestCase(
				VehicleCategory.HeavyBusPrimaryVehicle, AxleConfiguration.AxleConfig_6x4, FloorType.HighFloor, true, true,
				VehicleClass.ClassP35_36),
			TestCase(
				VehicleCategory.HeavyBusPrimaryVehicle, AxleConfiguration.AxleConfig_8x2, FloorType.LowFloor, false, false,
				VehicleClass.ClassP37_38),
			TestCase(
				VehicleCategory.HeavyBusPrimaryVehicle, AxleConfiguration.AxleConfig_8x2, FloorType.HighFloor, true, false,
				VehicleClass.ClassP37_38),
			TestCase(
				VehicleCategory.HeavyBusPrimaryVehicle, AxleConfiguration.AxleConfig_8x4, FloorType.LowFloor, false, false,
				VehicleClass.ClassP37_38),
			TestCase(
				VehicleCategory.HeavyBusPrimaryVehicle, AxleConfiguration.AxleConfig_8x4, FloorType.HighFloor, true, false,
				VehicleClass.ClassP37_38),
			TestCase(
				VehicleCategory.HeavyBusPrimaryVehicle, AxleConfiguration.AxleConfig_8x2, FloorType.LowFloor, false, true,
				VehicleClass.ClassP39_40),
			TestCase(
				VehicleCategory.HeavyBusPrimaryVehicle, AxleConfiguration.AxleConfig_8x2, FloorType.HighFloor, true, true,
				VehicleClass.ClassP39_40),
			TestCase(
				VehicleCategory.HeavyBusPrimaryVehicle, AxleConfiguration.AxleConfig_8x4, FloorType.LowFloor, false, true,
				VehicleClass.ClassP39_40),
			TestCase(
				VehicleCategory.HeavyBusPrimaryVehicle, AxleConfiguration.AxleConfig_8x4, FloorType.HighFloor, true, true,
				VehicleClass.ClassP39_40),
		]
		public void SegmentLookupTest(
			VehicleCategory category, AxleConfiguration axleConfiguration, FloorType floorType, bool doubleDecker,
			bool articulated, VehicleClass expectedClass)
		{
			var segment = DeclarationData.PrimaryBusSegments.Lookup(
				category, axleConfiguration, articulated, floorType);

			Assert.AreEqual(10, segment.Missions.Length);
			Assert.AreEqual(expectedClass, segment.VehicleClass);
		}


		[TestCase()]
		public void TestPrimaryBusGroup31_32Test()
		{
			var vehicleData = new {
				VehicleCategory = VehicleCategory.HeavyBusPrimaryVehicle,
				AxleConfiguration = AxleConfiguration.AxleConfig_4x2,
				Articulated = false,
				FloorType = FloorType.LowFloor,
				DoubleDecker = false
			};
			var segment = DeclarationData.PrimaryBusSegments.Lookup(
				vehicleData.VehicleCategory, vehicleData.AxleConfiguration, vehicleData.Articulated, vehicleData.FloorType);

			Assert.AreEqual(10, segment.Missions.Length);
			Assert.AreEqual(VehicleClass.ClassP31_32, segment.VehicleClass);


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
					curbMass: 11975,
					refLoad: 5618.16,
					lowLoad: 1123.632,
					axleWeightDistribution: new[] { 0.375, 0.625 },
					expVehicleEquipment: GetExpectedVehicleEquipment(3, 2, 0, 0)
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
					curbMass: 12350,
					refLoad: 5966.694,
					lowLoad: 1193.3388,
					axleWeightDistribution: new[] { 0.375, 0.625 },
					expVehicleEquipment: GetExpectedVehicleEquipment(3, 3, 0, 0)
				);
			}

			AssertMission(
				segment.Missions[6],
				missionType: MissionType.Interurban,
				cdxA: 4.6,
				length: 12,
				width: 2.55,
				height: 3.15,
				curbMass: 13150,
				refLoad: 4301.748,
				lowLoad: 1075.437,
				axleWeightDistribution: new[] { 0.375, 0.625 },
				expVehicleEquipment: GetExpectedVehicleEquipment(2, 2, 0.5, 0.5)
			);
			AssertMission(
				segment.Missions[7],
				missionType: MissionType.Coach,
				cdxA: 4.6,
				length: 12,
				width: 2.55,
				height: 3.15,
				curbMass: 13150,
				refLoad: 2737.476,
				lowLoad: 547.4952 * 2,
				axleWeightDistribution: new[] { 0.375, 0.625 },
				expVehicleEquipment: GetExpectedVehicleEquipment(2, 2, 0.5, 0.5)
			);

			AssertMission(
					segment.Missions[8],
					missionType: MissionType.Interurban,
					cdxA: 5.2,
					length: 10.5,
					width: 2.55,
					height: 3.7,
					curbMass: 13400,
					refLoad: 5051.295,
					lowLoad: 1262.82375,
					axleWeightDistribution: new[] { 0.375, 0.625 },
					expVehicleEquipment: GetExpectedVehicleEquipment(1, 2, 1, 1)
				);
			AssertMission(
				segment.Missions[9],
				missionType: MissionType.Coach,
				cdxA: 5.2,
				length: 10.5,
				width: 2.55,
				height: 3.7,
				curbMass: 13400,
				refLoad: 3367.53,
				lowLoad: 673.506 * 2,
				axleWeightDistribution: new[] { 0.375, 0.625 },
				expVehicleEquipment: GetExpectedVehicleEquipment(1, 2, 1, 1)
			);

		}

		[TestCase()]
		public void TestPrimaryBusGroup33_34Test()
		{
			var vehicleData = new {
				VehicleCategory = VehicleCategory.HeavyBusPrimaryVehicle,
				AxleConfiguration = AxleConfiguration.AxleConfig_6x2,
				Articulated = false,
				FloorType = FloorType.LowFloor,
				DoubleDecker = false
			};
			var segment = DeclarationData.PrimaryBusSegments.Lookup(
				vehicleData.VehicleCategory, vehicleData.AxleConfiguration, vehicleData.Articulated, vehicleData.FloorType);

			Assert.AreEqual(10, segment.Missions.Length);
			Assert.AreEqual(VehicleClass.ClassP33_34, segment.VehicleClass);

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
					curbMass: 14175,
					refLoad: 6762.6,
					lowLoad: 1352.52,
					axleWeightDistribution: new[] { 0.273, 0.454, 0.273 },
					expVehicleEquipment: GetExpectedVehicleEquipment(3, 2, 0, 0)
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
					curbMass: 14725,
					refLoad: 7891.434,
					lowLoad: 1578.2868,
					axleWeightDistribution: new[] { 0.273, 0.454, 0.273 },
					expVehicleEquipment: GetExpectedVehicleEquipment(3, 3, 0, 0)
				);
			}

			AssertMission(
				segment.Missions[6],
				missionType: MissionType.Interurban,
				cdxA: 4.7,
				length: 13.8,
				width: 2.55,
				height: 3.15,
				curbMass: 15213,
				refLoad: 5018.706,
				lowLoad: 1254.6765,
				axleWeightDistribution: new[] { 0.273, 0.454, 0.273 },
				expVehicleEquipment: GetExpectedVehicleEquipment(2, 2, 0.5, 0.5)
			);
			AssertMission(
				segment.Missions[7],
				missionType: MissionType.Coach,
				cdxA: 4.7,
				length: 13.8,
				width: 2.55,
				height: 3.15,
				curbMass: 15213,
				refLoad: 3193.722,
				lowLoad: 638.7444 * 2,
				axleWeightDistribution: new[] { 0.273, 0.454, 0.273 },
				expVehicleEquipment: GetExpectedVehicleEquipment(2, 2, 0.5, 0.5)
			);

			AssertMission(
					segment.Missions[8],
					missionType: MissionType.Interurban,
					cdxA: 5.3,
					length: 14,
					width: 2.55,
					height: 3.7,
					curbMass: 17850,
					refLoad: 6952.32,
					lowLoad: 1738.08,
					axleWeightDistribution: new[] { 0.273, 0.454, 0.273 },
					expVehicleEquipment: GetExpectedVehicleEquipment(1, 4, 1, 1.5)
				);
			AssertMission(
				segment.Missions[9],
				missionType: MissionType.Coach,
				cdxA: 5.3,
				length: 14,
				width: 2.55,
				height: 3.7,
				curbMass: 17850,
				refLoad: 4634.88,
				lowLoad: 926.976 * 2,
				axleWeightDistribution: new[] { 0.273, 0.454, 0.273 },
				expVehicleEquipment: GetExpectedVehicleEquipment(1, 4, 1, 1.5)
			);

		}

		[TestCase()]
		public void TestPrimaryBusGroup35_36Test()
		{
			var vehicleData = new {
				VehicleCategory = VehicleCategory.HeavyBusPrimaryVehicle,
				AxleConfiguration = AxleConfiguration.AxleConfig_6x2,
				Articulated = true,
				FloorType = FloorType.LowFloor,
				DoubleDecker = false
			};
			var segment = DeclarationData.PrimaryBusSegments.Lookup(
				vehicleData.VehicleCategory, vehicleData.AxleConfiguration, vehicleData.Articulated, vehicleData.FloorType);

			Assert.AreEqual(10, segment.Missions.Length);
			Assert.AreEqual(VehicleClass.ClassP35_36, segment.VehicleClass);

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
					curbMass: 17800,
					refLoad: 8843.4,
					lowLoad: 1768.68,
					axleWeightDistribution: new[] { 0.243, 0.352, 0.405 },
					expVehicleEquipment: GetExpectedVehicleEquipment(3, 3, 0, 0)
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
					curbMass: 20250,
					refLoad: 10906.86,
					lowLoad: 2181.372,
					axleWeightDistribution: new[] { 0.243, 0.352, 0.405 },
					expVehicleEquipment: GetExpectedVehicleEquipment(null, null, null, null)
				);
			}

			AssertMission(
				segment.Missions[6],
				missionType: MissionType.Interurban,
				cdxA: 4.8,
				length: 18.2,
				width: 2.55,
				height: 3.15,
				curbMass: 19267,
				refLoad: 6771.27,
				lowLoad: 1692.8175,
				axleWeightDistribution: new[] { 0.243, 0.352, 0.405 },
				expVehicleEquipment: GetExpectedVehicleEquipment(null, null, null, null)
			);
			AssertMission(
				segment.Missions[7],
				missionType: MissionType.Coach,
				cdxA: 4.8,
				length: 18.2,
				width: 2.55,
				height: 3.15,
				curbMass: 19267,
				refLoad: 4308.99,
				lowLoad: 861.798 * 2,
				axleWeightDistribution: new[] { 0.243, 0.352, 0.405 },
				expVehicleEquipment: GetExpectedVehicleEquipment(null, null, null, null)
			);
			AssertMission(
					segment.Missions[8],
					missionType: MissionType.Interurban,
					cdxA: 5.4,
					length: 18.2,
					width: 2.55,
					height: 3.7,
					curbMass: 21375,
					refLoad: 9233.55,
					lowLoad: 2308.3875,
					axleWeightDistribution: new[] { 0.243, 0.352, 0.405 },
					expVehicleEquipment: GetExpectedVehicleEquipment(null, null, null, null)
			);
			AssertMission(
				segment.Missions[9],
				missionType: MissionType.Coach,
				cdxA: 5.4,
				length: 18.2,
				width: 2.55,
				height: 3.7,
				curbMass: 21375,
				refLoad: 6155.7,
				lowLoad: 1231.14 * 2,
				axleWeightDistribution: new[] { 0.243, 0.352, 0.405 },
				expVehicleEquipment: GetExpectedVehicleEquipment(null, null, null, null)
			);

		}

		[TestCase()]
		public void TestPrimaryBusGroup37_38Test()
		{
			var vehicleData = new {
				VehicleCategory = VehicleCategory.HeavyBusPrimaryVehicle,
				AxleConfiguration = AxleConfiguration.AxleConfig_8x2,
				Articulated = false,
				FloorType = FloorType.LowFloor,
				DoubleDecker = false
			};
			var segment = DeclarationData.PrimaryBusSegments.Lookup(
				vehicleData.VehicleCategory, vehicleData.AxleConfiguration, vehicleData.Articulated, vehicleData.FloorType);

			Assert.AreEqual(10, segment.Missions.Length);
			Assert.AreEqual(VehicleClass.ClassP37_38, segment.VehicleClass);

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
					curbMass: 15000,
					refLoad: 7178.76,
					lowLoad: 1435.752,
					axleWeightDistribution: new[] { 0.214, 0.214, 0.358, 0.214 },
					expVehicleEquipment: GetExpectedVehicleEquipment(null, null, null, null)
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
					curbMass: 18700,
					refLoad: 8853.804,
					lowLoad: 1770.7608,
					axleWeightDistribution: new[] { 0.214, 0.214, 0.358, 0.214 },
					expVehicleEquipment: GetExpectedVehicleEquipment(null, null, null, null)
				);
			}

			AssertMission(
				segment.Missions[6],
				missionType: MissionType.Interurban,
				cdxA: 4.8,
				length: 15,
				width: 2.55,
				height: 3.15,
				curbMass: 17500,
				refLoad: 5496.6780,
				lowLoad: 1374.16950,
				axleWeightDistribution: new[] { 0.214, 0.214, 0.358, 0.214 },
				expVehicleEquipment: GetExpectedVehicleEquipment(null, null, null, null)
			);
			AssertMission(
				segment.Missions[7],
				missionType: MissionType.Coach,
				cdxA: 4.8,
				length: 15,
				width: 2.55,
				height: 3.15,
				curbMass: 17500,
				refLoad: 3497.886,
				lowLoad: 699.5772 * 2,
				axleWeightDistribution: new[] { 0.214, 0.214, 0.358, 0.214 },
				expVehicleEquipment: GetExpectedVehicleEquipment(null, null, null, null)
			);

			AssertMission(
					segment.Missions[8],
					missionType: MissionType.Interurban,
					cdxA: 5.4,
					length: 15,
					width: 2.55,
					height: 3.7,
					curbMass: 21200,
					refLoad: 7495.47,
					lowLoad: 1873.86750,
					axleWeightDistribution: new[] { 0.214, 0.214, 0.358, 0.214 },
					expVehicleEquipment: GetExpectedVehicleEquipment(null, null, null, null)
				);
			AssertMission(
				segment.Missions[9],
				missionType: MissionType.Coach,
				cdxA: 5.4,
				length: 15,
				width: 2.55,
				height: 3.7,
				curbMass: 21200,
				refLoad: 4996.98,
				lowLoad: 999.396 * 2,
				axleWeightDistribution: new[] { 0.214, 0.214, 0.358, 0.214 },
				expVehicleEquipment: GetExpectedVehicleEquipment(null, null, null, null)
			);

		}

		[TestCase()]
		public void TestPrimaryBusGroup39_40Test()
		{
			var vehicleData = new {
				VehicleCategory = VehicleCategory.HeavyBusPrimaryVehicle,
				AxleConfiguration = AxleConfiguration.AxleConfig_8x2,
				Articulated = true,
				FloorType = FloorType.LowFloor,
				DoubleDecker = false
			};
			var segment = DeclarationData.PrimaryBusSegments.Lookup(
				vehicleData.VehicleCategory, vehicleData.AxleConfiguration, vehicleData.Articulated, vehicleData.FloorType);

			Assert.AreEqual(10, segment.Missions.Length);
			Assert.AreEqual(VehicleClass.ClassP39_40, segment.VehicleClass);

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
					curbMass: 19600,
					refLoad: 10299.96,
					lowLoad: 2059.992,
					axleWeightDistribution: new[] { 0.200, 0.282, 0.324, 0.194 }, 
					expVehicleEquipment: GetExpectedVehicleEquipment(3, 3, 0, 0)
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
					curbMass: 24800,
					refLoad: 12703.284,
					lowLoad: 2540.6568,
					axleWeightDistribution: new[] { 0.200, 0.282, 0.324, 0.194 },
					expVehicleEquipment: GetExpectedVehicleEquipment(null, null, null, null)
				);
			}

			AssertMission(
				segment.Missions[6],
				missionType: MissionType.Interurban,
				cdxA: 4.9,
				length: 21,
				width: 2.55,
				height: 3.15,
				curbMass: 20950,
				refLoad: 7886.538,
				lowLoad: 1971.6345,
				axleWeightDistribution: new[] { 0.200, 0.282, 0.324, 0.194 },
				expVehicleEquipment: GetExpectedVehicleEquipment(null, null, null, null)
			);
			AssertMission(
				segment.Missions[7],
				missionType: MissionType.Coach,
				cdxA: 4.9,
				length: 21,
				width: 2.55,
				height: 3.15,
				curbMass: 20950,
				refLoad: 5018.706,
				lowLoad: 1003.7412 * 2,
				axleWeightDistribution: new[] { 0.200, 0.282, 0.324, 0.194 },
				expVehicleEquipment: GetExpectedVehicleEquipment(null, null, null, null)
			);

			AssertMission(
					segment.Missions[8],
					missionType: MissionType.Interurban,
					cdxA: 5.5,
					length: 21,
					width: 2.55,
					height: 3.7,
					curbMass: 24600,
					refLoad: 10754.37,
					lowLoad: 2688.5925,
					axleWeightDistribution: new[] { 0.200, 0.282, 0.324, 0.194 },
					expVehicleEquipment: GetExpectedVehicleEquipment(null, null, null, null)
				);
			AssertMission(
				segment.Missions[9],
				missionType: MissionType.Coach,
				cdxA: 5.5,
				length: 21,
				width: 2.55,
				height: 3.7,
				curbMass: 24600,
				refLoad: 7169.58,
				lowLoad: 1433.916 * 2,
				axleWeightDistribution: new[] { 0.200, 0.282, 0.324, 0.194 },
				expVehicleEquipment: GetExpectedVehicleEquipment(null, null, null, null)
			);

		}


		private void AssertMission(
			Mission m, MissionType missionType, double cdxA, double length, double width, double height, double curbMass,
			double refLoad, double lowLoad, double[] axleWeightDistribution, Dictionary<string, double> expVehicleEquipment)
		{
			Assert.AreEqual(missionType, m.MissionType);
			Assert.AreEqual(cdxA, m.DefaultCDxA.Value(), 1e-9);
			Assert.AreEqual(length, m.BusParameter.VehicleLength.Value(), 1e-9);
			Assert.AreEqual(width, m.BusParameter.VehicleWidth.Value(), 1e-9);
			Assert.AreEqual(height, m.BusParameter.BodyHeight.Value(), 1e-9);
			Assert.AreEqual(curbMass, m.CurbMass.Value(), 1e-9);
			Assert.AreEqual(refLoad, m.RefLoad.Value(), 1e-9);
			Assert.AreEqual(lowLoad, m.LowLoad.Value(), 1e-9);
			foreach (var tuple in axleWeightDistribution.ZipAll(m.AxleWeightDistribution, Tuple.Create))
			{
				Assert.AreEqual(tuple.Item1, tuple.Item2, 1e-0, "Axle distribution not equal.\nexpected: {0}\nactual: {1}", string.Join(",", axleWeightDistribution),
					string.Join(",", m.AxleWeightDistribution));
			}

			foreach (var entry in expVehicleEquipment) {
				Assert.AreEqual(entry.Value, m.BusParameter.ElectricalConsumers[entry.Key]);
			}
			
		}

		private Dictionary<string, double> GetExpectedVehicleEquipment(double? externalDisplays, double? internalDisplays, double? fridge,
			double? kitchenStandard)
		{
			var retVal = new Dictionary<string, double>();

			if (externalDisplays.HasValue) {
				retVal["External displays"] = externalDisplays.Value;
			}
			if (internalDisplays.HasValue) {
				retVal["Internal displays"] = internalDisplays.Value;
			}
			if (fridge.HasValue) {
				retVal["Fridge"] = fridge.Value;
			}
			if (kitchenStandard.HasValue) {
				retVal["Kitchen Standard"] = kitchenStandard.Value;
			};
			return retVal;
		}
	}
}
