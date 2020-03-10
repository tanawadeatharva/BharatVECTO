using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.Models.Declaration;

namespace TUGraz.VectoCore.Tests.Models.Declaration
{
	[TestFixture]
	public class DeclarationSegmentComplete4AxleBusesTest
	{
		private MissionType[] _missionsTypes;

		[OneTimeSetUp]
		public void RunBeforeAnyTest()
		{
			_missionsTypes = new[]
				{ MissionType.HeavyUrban, MissionType.Urban, MissionType.Suburban, MissionType.Interurban, MissionType.Coach };
		}

		[
			TestCase(4, VehicleCode.CE, "37a", 3),
			TestCase(4, VehicleCode.CE, "37b", 3),
			TestCase(4, VehicleCode.CF, "37c", 3),
			TestCase(4, VehicleCode.CI, "37d", 3),
			TestCase(4, VehicleCode.CJ, "37e", 3),
			TestCase(4, VehicleCode.CA, "38a", 2),
			TestCase(4, VehicleCode.CA, "38b", 2),
			TestCase(4, VehicleCode.CA, "38c", 2),
			TestCase(4, VehicleCode.CA, "38d", 2),
			TestCase(4, VehicleCode.CB, "38e", 2),
			TestCase(4, VehicleCode.CB, "38f", 2),
			TestCase(4, VehicleCode.CG, "39a", 3),
			TestCase(4, VehicleCode.CG, "39b", 3),
			TestCase(4, VehicleCode.CH, "39c", 3),
			TestCase(4, VehicleCode.CC, "40a", 2),
			TestCase(4, VehicleCode.CC, "40b", 2),
			TestCase(4, VehicleCode.CC, "40c", 2),
			TestCase(4, VehicleCode.CC, "40d", 2),
			TestCase(4, VehicleCode.CD, "40e", 2),
			TestCase(4, VehicleCode.CD, "40f", 2),
		]
		public void SegmentLookupTest(int numberOfAxles, VehicleCode vehicleCode, string vehicleParameterGroup, int numberOfMissions)
		{
			var segment = DeclarationData.CompletedBusSegments.Lookup(numberOfAxles, vehicleCode, vehicleParameterGroup);
			Assert.AreEqual(numberOfMissions, segment.Missions.Length);
		}

		[TestCase(4, VehicleCode.CE, "37a")]
		public void TestComplete4AxlesCompleteBus37A(int numberOfAxles, VehicleCode vehicleCode, string vehicleParameterGroup)
		{
			var segment =
				DeclarationData.CompletedBusSegments.Lookup(numberOfAxles, vehicleCode, vehicleParameterGroup);
			Assert.AreEqual(3, segment.Missions.Length);

			for (int i = 0; i < segment.Missions.Length; i++)
			{
				var mission = segment.Missions[i];

				Assert.AreEqual(_missionsTypes[i], mission.MissionType);
				Assert.AreEqual(5.1, mission.DefaultCDxA.Value());
				Assert.AreEqual(false, mission.AirDragMeasurement);

				AssertBusParameters(
					isDoubleDeck: false,
					numberOfAxles: numberOfAxles,
					isArticulated: false,
					floorType: FloorType.LowFloor,
					vehicleCode: vehicleCode,
					regClasses: "I/I+II/II/A",
					lowEntry: false,
					numPassengersLower: 0,
					passengersLowerOrEqual: null,
					bodyHeight: null,
					bodyHeightLowerOrEqual: null,
					vehicleParam: vehicleParameterGroup,
					busParameters: mission.BusParameter
				);

				AssertAveragePassengers(heavyUrban: 3, urban: 3, suburban: 3, interurban: 0, coach: 0, busParameters: mission.BusParameter);
				AssertAxleDistribution(axle1: 21.4, axle2: 21.4, axle3: 35.8, axle4: 21.4, axleLoadDistribution: mission.BusParameter.AxleLoadDistribution);
				AssertVehicleEquipment(externalDisplays: 0, internalDisplays: 0, fridge: 0, kitchenStandard: 0, vehicleEquipment: mission.BusParameter.VehicleEquipment);
			}
		}

		[TestCase(4, VehicleCode.CE, "37b")]
		public void TestComplete4AxlesCompleteBus37B(int numberOfAxles, VehicleCode vehicleCode, string vehicleParameterGroup)
		{
			var segment = DeclarationData.CompletedBusSegments.Lookup(numberOfAxles, vehicleCode, vehicleParameterGroup);

			Assert.AreEqual(3, segment.Missions.Length);

			for (int i = 0; i < segment.Missions.Length; i++)
			{
				var mission = segment.Missions[i];

				Assert.AreEqual(_missionsTypes[i], mission.MissionType);
				Assert.AreEqual(5.1, mission.DefaultCDxA.Value());
				Assert.AreEqual(false, mission.AirDragMeasurement);

				AssertBusParameters(
					isDoubleDeck: false,
					numberOfAxles: numberOfAxles,
					isArticulated: false,
					floorType: FloorType.LowFloor,
					vehicleCode: vehicleCode,
					regClasses: "I/I+II/II/A",
					lowEntry: true,
					numPassengersLower: 0,
					passengersLowerOrEqual: null,
					bodyHeight: null,
					bodyHeightLowerOrEqual: null,
					vehicleParam: vehicleParameterGroup,
					busParameters: mission.BusParameter
				);

				AssertAveragePassengers(heavyUrban: 3, urban: 3, suburban: 3, interurban: 0, coach: 0, busParameters: mission.BusParameter);
				AssertAxleDistribution(axle1: 21.4, axle2: 21.4, axle3: 35.8, axle4: 21.4, axleLoadDistribution: mission.BusParameter.AxleLoadDistribution);
				AssertVehicleEquipment(externalDisplays: 0, internalDisplays: 0, fridge: 0, kitchenStandard: 0, vehicleEquipment: mission.BusParameter.VehicleEquipment);
			}
		}

		[TestCase(4, VehicleCode.CF, "37c")]
		public void TestComplete4AxlesCompleteBus37C(int numberOfAxles, VehicleCode vehicleCode, string vehicleParameterGroup)
		{
			var segment = DeclarationData.CompletedBusSegments.Lookup(numberOfAxles, vehicleCode, vehicleParameterGroup);

			Assert.AreEqual(3, segment.Missions.Length);

			for (int i = 0; i < segment.Missions.Length; i++)
			{
				var mission = segment.Missions[i];

				Assert.AreEqual(_missionsTypes[i], mission.MissionType);
				Assert.AreEqual(6.4, mission.DefaultCDxA.Value());
				Assert.AreEqual(false, mission.AirDragMeasurement);

				AssertBusParameters(
					isDoubleDeck: true,
					numberOfAxles: numberOfAxles,
					isArticulated: false,
					floorType: FloorType.LowFloor,
					vehicleCode: vehicleCode,
					regClasses: "I/I+II/II/A",
					lowEntry: null,
					numPassengersLower: 0,
					passengersLowerOrEqual: null,
					bodyHeight: null,
					bodyHeightLowerOrEqual: null,
					vehicleParam: vehicleParameterGroup,
					busParameters: mission.BusParameter
				);

				AssertAveragePassengers(heavyUrban: 3.7, urban: 3.7, suburban: 3.7, interurban: 0, coach: 0, busParameters: mission.BusParameter);
				AssertAxleDistribution(axle1: 21.4, axle2: 21.4, axle3: 35.8, axle4: 21.4, axleLoadDistribution: mission.BusParameter.AxleLoadDistribution);
				AssertVehicleEquipment(externalDisplays: 0, internalDisplays: 0, fridge: 0, kitchenStandard: 0, vehicleEquipment: mission.BusParameter.VehicleEquipment);
			}
		}

		[TestCase(4, VehicleCode.CI, "37d")]
		public void TestComplete4AxlesCompleteBus37D(int numberOfAxles, VehicleCode vehicleCode, string vehicleParameterGroup)
		{
			var segment = DeclarationData.CompletedBusSegments.Lookup(numberOfAxles, vehicleCode, vehicleParameterGroup);

			Assert.AreEqual(3, segment.Missions.Length);

			for (int i = 0; i < segment.Missions.Length; i++)
			{
				var mission = segment.Missions[i];

				Assert.AreEqual(_missionsTypes[i], mission.MissionType);
				Assert.AreEqual(5.9, mission.DefaultCDxA.Value());
				Assert.AreEqual(false, mission.AirDragMeasurement);

				AssertBusParameters(
					isDoubleDeck: false,
					numberOfAxles: numberOfAxles,
					isArticulated: false,
					floorType: FloorType.Unknown,
					vehicleCode: vehicleCode,
					regClasses: "I/I+II/II/II+III/III/A/B",
					lowEntry: null,
					numPassengersLower: 0,
					passengersLowerOrEqual: null,
					bodyHeight: null,
					bodyHeightLowerOrEqual: null,
					vehicleParam: vehicleParameterGroup,
					busParameters: mission.BusParameter
				);

				AssertAveragePassengers(heavyUrban: 3, urban: 3, suburban: 3, interurban: 0, coach: 0, busParameters: mission.BusParameter);
				AssertAxleDistribution(axle1: 21.4, axle2: 21.4, axle3: 35.8, axle4: 21.4, axleLoadDistribution: mission.BusParameter.AxleLoadDistribution);
				AssertVehicleEquipment(externalDisplays: 0, internalDisplays: 0, fridge: 0, kitchenStandard: 0, vehicleEquipment: mission.BusParameter.VehicleEquipment);
			}
		}

		[TestCase(4, VehicleCode.CJ, "37e")]
		public void TestComplete4AxlesCompleteBus37E(int numberOfAxles, VehicleCode vehicleCode, string vehicleParameterGroup)
		{
			var segment = DeclarationData.CompletedBusSegments.Lookup(numberOfAxles, vehicleCode, vehicleParameterGroup);

			Assert.AreEqual(3, segment.Missions.Length);

			for (int i = 0; i < segment.Missions.Length; i++)
			{
				var mission = segment.Missions[i];

				Assert.AreEqual(_missionsTypes[i], mission.MissionType);
				Assert.AreEqual(7.2, mission.DefaultCDxA.Value());
				Assert.AreEqual(false, mission.AirDragMeasurement);

				AssertBusParameters(
					isDoubleDeck: true,
					numberOfAxles: numberOfAxles,
					isArticulated: false,
					floorType: FloorType.Unknown,
					vehicleCode: vehicleCode,
					regClasses: "I/I+II/II/II+III/III/A/B",
					lowEntry: null,
					numPassengersLower: 0,
					passengersLowerOrEqual: null,
					bodyHeight: null,
					bodyHeightLowerOrEqual: null,
					vehicleParam: vehicleParameterGroup,
					busParameters: mission.BusParameter
				);

				AssertAveragePassengers(heavyUrban: 3.7, urban: 3.7, suburban: 3.7, interurban: 0, coach: 0, busParameters: mission.BusParameter);
				AssertAxleDistribution(axle1: 21.4, axle2: 21.4, axle3: 35.8, axle4: 21.4, axleLoadDistribution: mission.BusParameter.AxleLoadDistribution);
				AssertVehicleEquipment(externalDisplays: 0, internalDisplays: 0, fridge: 0, kitchenStandard: 0, vehicleEquipment: mission.BusParameter.VehicleEquipment);
			}
		}

		[TestCase(4, VehicleCode.CA, "38a")]
		public void TestComplete4AxlesCompleteBus38A(int numberOfAxles, VehicleCode vehicleCode, string vehicleParameterGroup)
		{
			var segment = DeclarationData.CompletedBusSegments.Lookup(numberOfAxles, vehicleCode, vehicleParameterGroup);

			Assert.AreEqual(2, segment.Missions.Length);

			for (int i = 0; i < segment.Missions.Length; i++)
			{
				var mission = segment.Missions[i];

				Assert.AreEqual(_missionsTypes[i+3], mission.MissionType);
				Assert.AreEqual(4.8, mission.DefaultCDxA.Value());
				Assert.AreEqual(true, mission.AirDragMeasurement);

				AssertBusParameters(
					isDoubleDeck: false,
					numberOfAxles: numberOfAxles,
					isArticulated: false,
					floorType: FloorType.HighFloor,
					vehicleCode: vehicleCode,
					regClasses: "II",
					lowEntry: null,
					numPassengersLower: 0,
					passengersLowerOrEqual: null,
					bodyHeight: null,
					bodyHeightLowerOrEqual: null,
					vehicleParam: vehicleParameterGroup,
					busParameters: mission.BusParameter
				);

				AssertAveragePassengers(heavyUrban: 0, urban: 0, suburban: 0, interurban: 2.2, coach: 1.4, busParameters: mission.BusParameter);
				AssertAxleDistribution(axle1: 21.4, axle2: 21.4, axle3: 35.8, axle4: 21.4, axleLoadDistribution: mission.BusParameter.AxleLoadDistribution);
				AssertVehicleEquipment(externalDisplays: 0, internalDisplays: 0, fridge: 0, kitchenStandard: 0, vehicleEquipment: mission.BusParameter.VehicleEquipment);
			}
		}

		[TestCase(4, VehicleCode.CA, "38b")]
		public void TestComplete4AxlesCompleteBus38B(int numberOfAxles, VehicleCode vehicleCode, string vehicleParameterGroup)
		{
			var segment = DeclarationData.CompletedBusSegments.Lookup(numberOfAxles, vehicleCode, vehicleParameterGroup);

			Assert.AreEqual(2, segment.Missions.Length);

			for (int i = 0; i < segment.Missions.Length; i++)
			{
				var mission = segment.Missions[i];

				Assert.AreEqual(_missionsTypes[i+3], mission.MissionType);
				Assert.AreEqual(4.8, mission.DefaultCDxA.Value());
				Assert.AreEqual(true, mission.AirDragMeasurement);

				AssertBusParameters(
					isDoubleDeck: false,
					numberOfAxles: numberOfAxles,
					isArticulated: false,
					floorType: FloorType.HighFloor,
					vehicleCode: vehicleCode,
					regClasses: "II+III",
					lowEntry: null,
					numPassengersLower: 0,
					passengersLowerOrEqual: null,
					bodyHeight: 3100,
					bodyHeightLowerOrEqual: true,
					vehicleParam: vehicleParameterGroup,
					busParameters: mission.BusParameter
				);

				AssertAveragePassengers(heavyUrban: 0, urban: 0, suburban: 0, interurban: 2.2, coach: 1.4, busParameters: mission.BusParameter);
				AssertAxleDistribution(axle1: 21.4, axle2: 21.4, axle3: 35.8, axle4: 21.4, axleLoadDistribution: mission.BusParameter.AxleLoadDistribution);
				AssertVehicleEquipment(externalDisplays: 0, internalDisplays: 0, fridge: 0, kitchenStandard: 0, vehicleEquipment: mission.BusParameter.VehicleEquipment);
			}
		}

		[TestCase(4, VehicleCode.CA, "38c")]
		public void TestComplete4AxlesCompleteBus38C(int numberOfAxles, VehicleCode vehicleCode, string vehicleParameterGroup)
		{
			var segment = DeclarationData.CompletedBusSegments.Lookup(numberOfAxles, vehicleCode, vehicleParameterGroup);

			Assert.AreEqual(2, segment.Missions.Length);

			for (int i = 0; i < segment.Missions.Length; i++)
			{
				var mission = segment.Missions[i];

				Assert.AreEqual(_missionsTypes[i+3], mission.MissionType);
				Assert.AreEqual(4.8, mission.DefaultCDxA.Value());
				Assert.AreEqual(true, mission.AirDragMeasurement);

				AssertBusParameters(
					isDoubleDeck: false,
					numberOfAxles: numberOfAxles,
					isArticulated: false,
					floorType: FloorType.HighFloor,
					vehicleCode: vehicleCode,
					regClasses: "II+III",
					lowEntry: null,
					numPassengersLower: 0,
					passengersLowerOrEqual: null,
					bodyHeight: 3100,
					bodyHeightLowerOrEqual: false,
					vehicleParam: vehicleParameterGroup,
					busParameters: mission.BusParameter
				);

				AssertAveragePassengers(heavyUrban: 0, urban: 0, suburban: 0, interurban: 2.2, coach: 1.4, busParameters: mission.BusParameter);
				AssertAxleDistribution(axle1: 21.4, axle2: 21.4, axle3: 35.8, axle4: 21.4, axleLoadDistribution: mission.BusParameter.AxleLoadDistribution);
				AssertVehicleEquipment(externalDisplays: 0, internalDisplays: 0, fridge: 0, kitchenStandard: 0, vehicleEquipment: mission.BusParameter.VehicleEquipment);
			}
		}

		[TestCase(4, VehicleCode.CA, "38d")]
		public void TestComplete4AxlesCompleteBus38D(int numberOfAxles, VehicleCode vehicleCode, string vehicleParameterGroup)
		{
			var segment = DeclarationData.CompletedBusSegments.Lookup(numberOfAxles, vehicleCode, vehicleParameterGroup);

			Assert.AreEqual(2, segment.Missions.Length);

			for (int i = 0; i < segment.Missions.Length; i++)
			{
				var mission = segment.Missions[i];

				Assert.AreEqual(_missionsTypes[i+3], mission.MissionType);
				Assert.AreEqual(4.8, mission.DefaultCDxA.Value());
				Assert.AreEqual(true, mission.AirDragMeasurement);

				AssertBusParameters(
					isDoubleDeck: false,
					numberOfAxles: numberOfAxles,
					isArticulated: false,
					floorType: FloorType.HighFloor,
					vehicleCode: vehicleCode,
					regClasses: "III/B",
					lowEntry: null,
					numPassengersLower: 0,
					passengersLowerOrEqual: null,
					bodyHeight: null,
					bodyHeightLowerOrEqual: null,
					vehicleParam: vehicleParameterGroup,
					busParameters: mission.BusParameter
				);

				AssertAveragePassengers(heavyUrban: 0, urban: 0, suburban: 0, interurban: 2.2, coach: 1.4, busParameters: mission.BusParameter);
				AssertAxleDistribution(axle1: 21.4, axle2: 21.4, axle3: 35.8, axle4: 21.4, axleLoadDistribution: mission.BusParameter.AxleLoadDistribution);
				AssertVehicleEquipment(externalDisplays: 0, internalDisplays: 0, fridge: 0, kitchenStandard: 0, vehicleEquipment: mission.BusParameter.VehicleEquipment);
			}
		}

		[TestCase(4, VehicleCode.CB, "38e")]
		public void TestComplete4AxlesCompleteBus38E(int numberOfAxles, VehicleCode vehicleCode, string vehicleParameterGroup)
		{
			var segment = DeclarationData.CompletedBusSegments.Lookup(numberOfAxles, vehicleCode, vehicleParameterGroup);

			Assert.AreEqual(2, segment.Missions.Length);

			for (int i = 0; i < segment.Missions.Length; i++)
			{
				var mission = segment.Missions[i];

				Assert.AreEqual(_missionsTypes[i+3], mission.MissionType);
				Assert.AreEqual(5.4, mission.DefaultCDxA.Value());
				Assert.AreEqual(true, mission.AirDragMeasurement);

				AssertBusParameters(
					isDoubleDeck: true,
					numberOfAxles: numberOfAxles,
					isArticulated: false,
					floorType: FloorType.HighFloor,
					vehicleCode: vehicleCode,
					regClasses: "II/II+III/III/B",
					lowEntry: null,
					numPassengersLower: 6,
					passengersLowerOrEqual: true,
					bodyHeight: null,
					bodyHeightLowerOrEqual: null,
					vehicleParam: vehicleParameterGroup,
					busParameters: mission.BusParameter
				);

				AssertAveragePassengers(heavyUrban: 0, urban: 0, suburban: 0, interurban: 2.2, coach: 1.4, busParameters: mission.BusParameter);
				AssertAxleDistribution(axle1: 21.4, axle2: 21.4, axle3: 35.8, axle4: 21.4, axleLoadDistribution: mission.BusParameter.AxleLoadDistribution);
				AssertVehicleEquipment(externalDisplays: 0, internalDisplays: 0, fridge: 0, kitchenStandard: 0, vehicleEquipment: mission.BusParameter.VehicleEquipment);
			}
		}

		[TestCase(4, VehicleCode.CB, "38f")]
		public void TestComplete4AxlesCompleteBus38F(int numberOfAxles, VehicleCode vehicleCode, string vehicleParameterGroup)
		{
			var segment = DeclarationData.CompletedBusSegments.Lookup(numberOfAxles, vehicleCode, vehicleParameterGroup);

			Assert.AreEqual(2, segment.Missions.Length);

			for (int i = 0; i < segment.Missions.Length; i++)
			{
				var mission = segment.Missions[i];

				Assert.AreEqual(_missionsTypes[i+3], mission.MissionType);
				Assert.AreEqual(5.4, mission.DefaultCDxA.Value());
				Assert.AreEqual(true, mission.AirDragMeasurement);

				AssertBusParameters(
					isDoubleDeck: true,
					numberOfAxles: numberOfAxles,
					isArticulated: false,
					floorType: FloorType.HighFloor,
					vehicleCode: vehicleCode,
					regClasses: "II/II+III/III/B",
					lowEntry: null,
					numPassengersLower: 6,
					passengersLowerOrEqual: false,
					bodyHeight: null,
					bodyHeightLowerOrEqual: null,
					vehicleParam: vehicleParameterGroup,
					busParameters: mission.BusParameter
				);

				AssertAveragePassengers(heavyUrban: 0, urban: 0, suburban: 0, interurban: 3, coach: 2, busParameters: mission.BusParameter);
				AssertAxleDistribution(axle1: 21.4, axle2: 21.4, axle3: 35.8, axle4: 21.4, axleLoadDistribution: mission.BusParameter.AxleLoadDistribution);
				AssertVehicleEquipment(externalDisplays: 0, internalDisplays: 0, fridge: 0, kitchenStandard: 0, vehicleEquipment: mission.BusParameter.VehicleEquipment);
			}
		}

		[TestCase(4, VehicleCode.CG, "39a")]
		public void TestComplete4AxlesCompleteBus39A(int numberOfAxles, VehicleCode vehicleCode, string vehicleParameterGroup)
		{
			var segment = DeclarationData.CompletedBusSegments.Lookup(numberOfAxles, vehicleCode, vehicleParameterGroup);

			Assert.AreEqual(3, segment.Missions.Length);

			for (int i = 0; i < segment.Missions.Length; i++)
			{
				var mission = segment.Missions[i];

				Assert.AreEqual(_missionsTypes[i], mission.MissionType);
				Assert.AreEqual(5.2, mission.DefaultCDxA.Value());
				Assert.AreEqual(false, mission.AirDragMeasurement);

				AssertBusParameters(
					isDoubleDeck: false,
					numberOfAxles: numberOfAxles,
					isArticulated: true,
					floorType: FloorType.LowFloor,
					vehicleCode: vehicleCode,
					regClasses: "I/I+II/II/A",
					lowEntry: false,
					numPassengersLower: 0,
					passengersLowerOrEqual: null,
					bodyHeight: null,
					bodyHeightLowerOrEqual: null,
					vehicleParam: vehicleParameterGroup,
					busParameters: mission.BusParameter
				);

				AssertAveragePassengers(heavyUrban: 3, urban: 3, suburban: 3, interurban: 0, coach: 0, busParameters: mission.BusParameter);
				AssertAxleDistribution(axle1: 21.4, axle2: 21.4, axle3: 35.8, axle4: 21.4, axleLoadDistribution: mission.BusParameter.AxleLoadDistribution);
				AssertVehicleEquipment(externalDisplays: 3, internalDisplays: 3, fridge: 0, kitchenStandard: 0, vehicleEquipment: mission.BusParameter.VehicleEquipment);
			}
		}

		[TestCase(4, VehicleCode.CG, "39b")]
		public void TestComplete4AxlesCompleteBus39B(int numberOfAxles, VehicleCode vehicleCode, string vehicleParameterGroup)
		{
			var segment = DeclarationData.CompletedBusSegments.Lookup(numberOfAxles, vehicleCode, vehicleParameterGroup);

			Assert.AreEqual(3, segment.Missions.Length);

			for (int i = 0; i < segment.Missions.Length; i++)
			{
				var mission = segment.Missions[i];

				Assert.AreEqual(_missionsTypes[i], mission.MissionType);
				Assert.AreEqual(5.2, mission.DefaultCDxA.Value());
				Assert.AreEqual(false, mission.AirDragMeasurement);

				AssertBusParameters(
					isDoubleDeck: false,
					numberOfAxles: numberOfAxles,
					isArticulated: true,
					floorType: FloorType.LowFloor,
					vehicleCode: vehicleCode,
					regClasses: "I/I+II/II/A",
					lowEntry: true,
					numPassengersLower: 0,
					passengersLowerOrEqual: null,
					bodyHeight: null,
					bodyHeightLowerOrEqual: null,
					vehicleParam: vehicleParameterGroup,
					busParameters: mission.BusParameter
				);

				AssertAveragePassengers(heavyUrban: 3, urban: 3, suburban: 3, interurban: 0, coach: 0, busParameters: mission.BusParameter);
				AssertAxleDistribution(axle1: 21.4, axle2: 21.4, axle3: 35.8, axle4: 21.4, axleLoadDistribution: mission.BusParameter.AxleLoadDistribution);
				AssertVehicleEquipment(externalDisplays: 3, internalDisplays: 3, fridge: 0, kitchenStandard: 0, vehicleEquipment: mission.BusParameter.VehicleEquipment);
			}
		}

		[TestCase(4, VehicleCode.CH, "39c")]
		public void TestComplete4AxlesCompleteBus39C(int numberOfAxles, VehicleCode vehicleCode, string vehicleParameterGroup)
		{
			var segment = DeclarationData.CompletedBusSegments.Lookup(numberOfAxles, vehicleCode, vehicleParameterGroup);

			Assert.AreEqual(3, segment.Missions.Length);

			for (int i = 0; i < segment.Missions.Length; i++)
			{
				var mission = segment.Missions[i];

				Assert.AreEqual(_missionsTypes[i], mission.MissionType);
				Assert.AreEqual(6.5, mission.DefaultCDxA.Value());
				Assert.AreEqual(false, mission.AirDragMeasurement);

				AssertBusParameters(
					isDoubleDeck: true,
					numberOfAxles: numberOfAxles,
					isArticulated: true,
					floorType: FloorType.LowFloor,
					vehicleCode: vehicleCode,
					regClasses: "I/I+II/II/A",
					lowEntry: null,
					numPassengersLower: 0,
					passengersLowerOrEqual: null,
					bodyHeight: null,
					bodyHeightLowerOrEqual: null,
					vehicleParam: vehicleParameterGroup,
					busParameters: mission.BusParameter
				);

				AssertAveragePassengers(heavyUrban: 3.7, urban: 3.7, suburban: 3.7, interurban: 0, coach: 0, busParameters: mission.BusParameter);
				AssertAxleDistribution(axle1: 21.4, axle2: 21.4, axle3: 35.8, axle4: 21.4, axleLoadDistribution: mission.BusParameter.AxleLoadDistribution);
				AssertVehicleEquipment(externalDisplays: 0, internalDisplays: 0, fridge: 0, kitchenStandard: 0, vehicleEquipment: mission.BusParameter.VehicleEquipment);
			}
		}

		[TestCase(4, VehicleCode.CC, "40a")]
		public void TestComplete4AxlesCompleteBus40A(int numberOfAxles, VehicleCode vehicleCode, string vehicleParameterGroup)
		{
			var segment = DeclarationData.CompletedBusSegments.Lookup(numberOfAxles, vehicleCode, vehicleParameterGroup);

			Assert.AreEqual(2, segment.Missions.Length);

			for (int i = 0; i < segment.Missions.Length; i++)
			{
				var mission = segment.Missions[i];

				Assert.AreEqual(_missionsTypes[i+3], mission.MissionType);
				Assert.AreEqual(4.9, mission.DefaultCDxA.Value());
				Assert.AreEqual(true, mission.AirDragMeasurement);

				AssertBusParameters(
					isDoubleDeck: false,
					numberOfAxles: numberOfAxles,
					isArticulated: true,
					floorType: FloorType.HighFloor,
					vehicleCode: vehicleCode,
					regClasses: "II",
					lowEntry: null,
					numPassengersLower: 0,
					passengersLowerOrEqual: null,
					bodyHeight: null,
					bodyHeightLowerOrEqual: null,
					vehicleParam: vehicleParameterGroup,
					busParameters: mission.BusParameter
				);

				AssertAveragePassengers(heavyUrban: 0, urban: 0, suburban: 0, interurban: 2.2, coach: 1.4, busParameters: mission.BusParameter);
				AssertAxleDistribution(axle1: 21.4, axle2: 21.4, axle3: 35.8, axle4: 21.4, axleLoadDistribution: mission.BusParameter.AxleLoadDistribution);
				AssertVehicleEquipment(externalDisplays: 0, internalDisplays: 0, fridge: 0, kitchenStandard: 0, vehicleEquipment: mission.BusParameter.VehicleEquipment);
			}
		}

		[TestCase(4, VehicleCode.CC, "40b")]
		public void TestComplete4AxlesCompleteBus40B(int numberOfAxles, VehicleCode vehicleCode, string vehicleParameterGroup)
		{
			var segment = DeclarationData.CompletedBusSegments.Lookup(numberOfAxles, vehicleCode, vehicleParameterGroup);

			Assert.AreEqual(2, segment.Missions.Length);

			for (int i = 0; i < segment.Missions.Length; i++)
			{
				var mission = segment.Missions[i];

				Assert.AreEqual(_missionsTypes[i+3], mission.MissionType);
				Assert.AreEqual(4.9, mission.DefaultCDxA.Value());
				Assert.AreEqual(true, mission.AirDragMeasurement);

				AssertBusParameters(
					isDoubleDeck: false,
					numberOfAxles: numberOfAxles,
					isArticulated: true,
					floorType: FloorType.HighFloor,
					vehicleCode: vehicleCode,
					regClasses: "II+III",
					lowEntry: null,
					numPassengersLower: 0,
					passengersLowerOrEqual: null,
					bodyHeight: 3100,
					bodyHeightLowerOrEqual: true,
					vehicleParam: vehicleParameterGroup,
					busParameters: mission.BusParameter
				);

				AssertAveragePassengers(heavyUrban: 0, urban: 0, suburban: 0, interurban: 2.2, coach: 1.4, busParameters: mission.BusParameter);
				AssertAxleDistribution(axle1: 21.4, axle2: 21.4, axle3: 35.8, axle4: 21.4, axleLoadDistribution: mission.BusParameter.AxleLoadDistribution);
				AssertVehicleEquipment(externalDisplays: 0, internalDisplays: 0, fridge: 0, kitchenStandard: 0, vehicleEquipment: mission.BusParameter.VehicleEquipment);
			}
		}

		[TestCase(4, VehicleCode.CC, "40c")]
		public void TestComplete4AxlesCompleteBus40C(int numberOfAxles, VehicleCode vehicleCode, string vehicleParameterGroup)
		{
			var segment = DeclarationData.CompletedBusSegments.Lookup(numberOfAxles, vehicleCode, vehicleParameterGroup);

			Assert.AreEqual(2, segment.Missions.Length);

			for (int i = 0; i < segment.Missions.Length; i++)
			{
				var mission = segment.Missions[i];

				Assert.AreEqual(_missionsTypes[i+3], mission.MissionType);
				Assert.AreEqual(4.9, mission.DefaultCDxA.Value());
				Assert.AreEqual(true, mission.AirDragMeasurement);

				AssertBusParameters(
					isDoubleDeck: false,
					numberOfAxles: numberOfAxles,
					isArticulated: true,
					floorType: FloorType.HighFloor,
					vehicleCode: vehicleCode,
					regClasses: "II+III",
					lowEntry: null,
					numPassengersLower: 0,
					passengersLowerOrEqual: null,
					bodyHeight: 3100,
					bodyHeightLowerOrEqual: false,
					vehicleParam: vehicleParameterGroup,
					busParameters: mission.BusParameter
				);

				AssertAveragePassengers(heavyUrban: 0, urban: 0, suburban: 0, interurban: 2.2, coach: 1.4, busParameters: mission.BusParameter);
				AssertAxleDistribution(axle1: 21.4, axle2: 21.4, axle3: 35.8, axle4: 21.4, axleLoadDistribution: mission.BusParameter.AxleLoadDistribution);
				AssertVehicleEquipment(externalDisplays: 0, internalDisplays: 0, fridge: 0, kitchenStandard: 0, vehicleEquipment: mission.BusParameter.VehicleEquipment);
			}
		}

		[TestCase(4, VehicleCode.CC, "40d")]
		public void TestComplete4AxlesCompleteBus40D(int numberOfAxles, VehicleCode vehicleCode, string vehicleParameterGroup)
		{
			var segment = DeclarationData.CompletedBusSegments.Lookup(numberOfAxles, vehicleCode, vehicleParameterGroup);

			Assert.AreEqual(2, segment.Missions.Length);

			for (int i = 0; i < segment.Missions.Length; i++)
			{
				var mission = segment.Missions[i];

				Assert.AreEqual(_missionsTypes[i+3], mission.MissionType);
				Assert.AreEqual(4.9, mission.DefaultCDxA.Value());
				Assert.AreEqual(true, mission.AirDragMeasurement);

				AssertBusParameters(
					isDoubleDeck: false,
					numberOfAxles: numberOfAxles,
					isArticulated: true,
					floorType: FloorType.HighFloor,
					vehicleCode: vehicleCode,
					regClasses: "III/B",
					lowEntry: null,
					numPassengersLower: 0,
					passengersLowerOrEqual: null,
					bodyHeight: null,
					bodyHeightLowerOrEqual: null,
					vehicleParam: vehicleParameterGroup,
					busParameters: mission.BusParameter
				);

				AssertAveragePassengers(heavyUrban: 0, urban: 0, suburban: 0, interurban: 2.2, coach: 1.4, busParameters: mission.BusParameter);
				AssertAxleDistribution(axle1: 21.4, axle2: 21.4, axle3: 35.8, axle4: 21.4, axleLoadDistribution: mission.BusParameter.AxleLoadDistribution);
				AssertVehicleEquipment(externalDisplays: 0, internalDisplays: 0, fridge: 0, kitchenStandard: 0, vehicleEquipment: mission.BusParameter.VehicleEquipment);
			}
		}

		[TestCase(4, VehicleCode.CD, "40e")]
		public void TestComplete4AxlesCompleteBus40E(int numberOfAxles, VehicleCode vehicleCode, string vehicleParameterGroup)
		{
			var segment = DeclarationData.CompletedBusSegments.Lookup(numberOfAxles, vehicleCode, vehicleParameterGroup);

			Assert.AreEqual(2, segment.Missions.Length);

			for (int i = 0; i < segment.Missions.Length; i++)
			{
				var mission = segment.Missions[i];

				Assert.AreEqual(_missionsTypes[i+3], mission.MissionType);
				Assert.AreEqual(5.5, mission.DefaultCDxA.Value());
				Assert.AreEqual(true, mission.AirDragMeasurement);

				AssertBusParameters(
					isDoubleDeck: true,
					numberOfAxles: numberOfAxles,
					isArticulated: true,
					floorType: FloorType.HighFloor,
					vehicleCode: vehicleCode,
					regClasses: "II/II+III/III/B",
					lowEntry: null,
					numPassengersLower: 6,
					passengersLowerOrEqual: true,
					bodyHeight: null,
					bodyHeightLowerOrEqual: null,
					vehicleParam: vehicleParameterGroup,
					busParameters: mission.BusParameter
				);

				AssertAveragePassengers(heavyUrban: 0, urban: 0, suburban: 0, interurban: 2.2, coach: 1.4, busParameters: mission.BusParameter);
				AssertAxleDistribution(axle1: 21.4, axle2: 21.4, axle3: 35.8, axle4: 21.4, axleLoadDistribution: mission.BusParameter.AxleLoadDistribution);
				AssertVehicleEquipment(externalDisplays: 0, internalDisplays: 0, fridge: 0, kitchenStandard: 0, vehicleEquipment: mission.BusParameter.VehicleEquipment);
			}
		}

		[TestCase(4, VehicleCode.CD, "40f")]
		public void TestComplete4AxlesCompleteBus40F(int numberOfAxles, VehicleCode vehicleCode, string vehicleParameterGroup)
		{
			var segment = DeclarationData.CompletedBusSegments.Lookup(numberOfAxles, vehicleCode, vehicleParameterGroup);

			Assert.AreEqual(2, segment.Missions.Length);

			for (int i = 0; i < segment.Missions.Length; i++)
			{
				var mission = segment.Missions[i];

				Assert.AreEqual(_missionsTypes[i+3], mission.MissionType);
				Assert.AreEqual(5.5, mission.DefaultCDxA.Value());
				Assert.AreEqual(true, mission.AirDragMeasurement);

				AssertBusParameters(
					isDoubleDeck: true,
					numberOfAxles: numberOfAxles,
					isArticulated: true,
					floorType: FloorType.HighFloor,
					vehicleCode: vehicleCode,
					regClasses: "II/II+III/III/B",
					lowEntry: null,
					numPassengersLower: 6,
					passengersLowerOrEqual: false,
					bodyHeight: null,
					bodyHeightLowerOrEqual: null,
					vehicleParam: vehicleParameterGroup,
					busParameters: mission.BusParameter
				);

				AssertAveragePassengers(heavyUrban: 0, urban: 0, suburban: 0, interurban: 3, coach: 2, busParameters: mission.BusParameter);
				AssertAxleDistribution(axle1: 21.4, axle2: 21.4, axle3: 35.8, axle4: 21.4, axleLoadDistribution: mission.BusParameter.AxleLoadDistribution);
				AssertVehicleEquipment(externalDisplays: 0, internalDisplays: 0, fridge: 0, kitchenStandard: 0, vehicleEquipment: mission.BusParameter.VehicleEquipment);
			}
		}

		#region Assert Methods

		private void AssertBusParameters(bool isDoubleDeck, int numberOfAxles, bool isArticulated, FloorType floorType, VehicleCode vehicleCode, string regClasses,
			bool? lowEntry, double numPassengersLower, bool? passengersLowerOrEqual, double? bodyHeight, bool? bodyHeightLowerOrEqual,
			string vehicleParam, BusParameters busParameters)
		{
			Assert.AreEqual(isDoubleDeck, vehicleCode.IsDoubleDeckBus());
			Assert.AreEqual(numberOfAxles, busParameters.NumberOfAxles);
			Assert.AreEqual(isArticulated, busParameters.IsArticulated);
			Assert.AreEqual(isArticulated, busParameters.IsArticulated);

			Assert.AreEqual(floorType, busParameters.FloorType);
			if (floorType == FloorType.Unknown)
				Assert.IsTrue(vehicleCode.IsOpenDeckBus());

			Assert.AreEqual(vehicleCode, busParameters.VehicleCode);
			AssertRegistrationClasses(regClasses, busParameters.RegistrationClasses);
			Assert.AreEqual(lowEntry, busParameters.LowEntry);
			Assert.AreEqual(numPassengersLower, busParameters.NumberPassengersLowerDeck);
			Assert.AreEqual(passengersLowerOrEqual, busParameters.PassengersSeatsLowerOrEqual);
			Assert.AreEqual(bodyHeight, busParameters.BodyHeight?.Value());
			Assert.AreEqual(bodyHeightLowerOrEqual, busParameters.BodyHeightLowerOrEqual);
			Assert.AreEqual(vehicleParam, busParameters.VehicleParameterGroup);
		}


		private void AssertRegistrationClasses(string registrationClasses, RegistrationClass[] actualClasses)
		{
			var expectedClasses = RegistrationClassHelper.Parse(registrationClasses);
			Assert.AreEqual(expectedClasses.Length, actualClasses.Length);

			for (int i = 0; i < expectedClasses.Length; i++)
			{
				Assert.IsTrue(actualClasses.Contains(expectedClasses[i]));
			}
		}


		private void AssertAveragePassengers(double heavyUrban, double urban, double suburban, double interurban, double coach,
			BusParameters busParameters)
		{
			Assert.AreEqual(heavyUrban, busParameters.PassengersHeavyUrban);
			Assert.AreEqual(urban, busParameters.PassengersUrban);
			Assert.AreEqual(suburban, busParameters.PassengersSuburban);
			Assert.AreEqual(interurban, busParameters.PassengersInterurban);
			Assert.AreEqual(coach, busParameters.PassengersCoach);
		}


		private void AssertAxleDistribution(double axle1, double axle2, double axle3, double axle4,
			AxleLoadDistribution axleLoadDistribution)
		{
			Assert.AreEqual(axle1, axleLoadDistribution.Axle01);
			Assert.AreEqual(axle2, axleLoadDistribution.Axle02);
			Assert.AreEqual(axle3, axleLoadDistribution.Axle03);
			Assert.AreEqual(axle4, axleLoadDistribution.Axle04);
		}


		private void AssertVehicleEquipment(double externalDisplays, double internalDisplays, double fridge,
			double kitchenStandard, VehicleEquipment vehicleEquipment)
		{
			Assert.AreEqual(externalDisplays, vehicleEquipment.ExternalDisplays);
			Assert.AreEqual(internalDisplays, vehicleEquipment.InternalDisplays);
			Assert.AreEqual(fridge, vehicleEquipment.Fridge);
			Assert.AreEqual(kitchenStandard, vehicleEquipment.KitchenStandard);
		}

		#endregion


	}
}
