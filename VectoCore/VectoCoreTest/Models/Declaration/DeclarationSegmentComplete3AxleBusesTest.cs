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
	public class DeclarationSegmentComplete3AxleBusesTest
	{
		private MissionType[] _missionsTypes;

		[OneTimeSetUp]
		public void RunBeforeAnyTest()
		{
			_missionsTypes = new[]
				{ MissionType.HeavyUrban, MissionType.Urban, MissionType.Suburban, MissionType.Interurban, MissionType.Coach };
		}

		[
			TestCase(3, VehicleCode.CE, "33a"),
			TestCase(3, VehicleCode.CE, "33b"),
			TestCase(3, VehicleCode.CF, "33c"),
			TestCase(3, VehicleCode.CI, "33d"),
			TestCase(3, VehicleCode.CJ, "33e"),
			TestCase(3, VehicleCode.CA, "34a"),
			TestCase(3, VehicleCode.CA, "34b"),
			TestCase(3, VehicleCode.CA, "34c"),
			TestCase(3, VehicleCode.CA, "34d"),
			TestCase(3, VehicleCode.CB, "34e"),
			TestCase(3, VehicleCode.CB, "34f"),
			TestCase(3, VehicleCode.CG, "35a"),
			TestCase(3, VehicleCode.CG, "35b"),
			TestCase(3, VehicleCode.CH, "35c"),
			TestCase(3, VehicleCode.CC, "36a"),
			TestCase(3, VehicleCode.CC, "36b"),
			TestCase(3, VehicleCode.CC, "36c"),
			TestCase(3, VehicleCode.CC, "36d"),
			TestCase(3, VehicleCode.CD, "36e"),
			TestCase(3, VehicleCode.CD, "36f"),
		]
		public void SegmentLookupTest(int numberOfAxles, VehicleCode vehicleCode, string vehicleParameterGroup)
		{
			var segment = DeclarationData.CompletedBusSegments.Lookup(numberOfAxles, vehicleCode, vehicleParameterGroup);
			Assert.AreEqual(5, segment.Missions.Length);
		}

		[TestCase(3, VehicleCode.CE, "33a")]
		public void TestComplete3AxlesCompleteBus33A(int numberOfAxles, VehicleCode vehicleCode, string vehicleParameterGroup)
		{
			var segment =
				DeclarationData.CompletedBusSegments.Lookup(numberOfAxles, vehicleCode, vehicleParameterGroup);
			Assert.AreEqual(_missionsTypes.Length, segment.Missions.Length);

			for (int i = 0; i < segment.Missions.Length; i++) {
				var mission = segment.Missions[i];

				Assert.AreEqual(_missionsTypes[i], mission.MissionType);
				Assert.AreEqual(5.0, mission.DefaultCDxA.Value());
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
				AssertAxleDistribution(axle1: 27.3, axle2: 45.4, axle3: 27.3, axle4: 0, axleLoadDistribution: mission.BusParameter.AxleLoadDistribution);
				AssertVehicleEquipment(externalDisplays: 3, internalDisplays: 2, fridge: 0, kitchenStandard: 0, vehicleEquipment: mission.BusParameter.VehicleEquipment);
			}
		}

		[TestCase(3, VehicleCode.CE, "33b")]
		public void TestComplete3AxlesCompleteBus33B(int numberOfAxles, VehicleCode vehicleCode, string vehicleParameterGroup)
		{
			var segment = DeclarationData.CompletedBusSegments.Lookup(numberOfAxles, vehicleCode, vehicleParameterGroup);

			Assert.AreEqual(_missionsTypes.Length, segment.Missions.Length);

			for (int i = 0; i < segment.Missions.Length; i++)
			{
				var mission = segment.Missions[i];

				Assert.AreEqual(_missionsTypes[i], mission.MissionType);
				Assert.AreEqual(5.0, mission.DefaultCDxA.Value());
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
				AssertAxleDistribution(axle1: 27.3, axle2: 45.4, axle3: 27.3, axle4: 0, axleLoadDistribution: mission.BusParameter.AxleLoadDistribution);
				AssertVehicleEquipment(externalDisplays: 3, internalDisplays: 2, fridge: 0, kitchenStandard: 0, vehicleEquipment: mission.BusParameter.VehicleEquipment);
			}
		}

		[TestCase(3, VehicleCode.CF, "33c")]
		public void TestComplete3AxlesCompleteBus33C(int numberOfAxles, VehicleCode vehicleCode, string vehicleParameterGroup)
		{
			var segment = DeclarationData.CompletedBusSegments.Lookup(numberOfAxles, vehicleCode, vehicleParameterGroup);

			Assert.AreEqual(_missionsTypes.Length, segment.Missions.Length);

			for (int i = 0; i < segment.Missions.Length; i++)
			{
				var mission = segment.Missions[i];

				Assert.AreEqual(_missionsTypes[i], mission.MissionType);
				Assert.AreEqual(6.3, mission.DefaultCDxA.Value());
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
				AssertAxleDistribution(axle1: 27.3, axle2: 45.4, axle3: 27.3, axle4: 0, axleLoadDistribution: mission.BusParameter.AxleLoadDistribution);
				AssertVehicleEquipment(externalDisplays: 3, internalDisplays: 3, fridge: 0, kitchenStandard: 0, vehicleEquipment: mission.BusParameter.VehicleEquipment);
			}
		}
		
		[TestCase(3, VehicleCode.CI, "33d")]
		public void TestComplete3AxlesCompleteBus33D(int numberOfAxles, VehicleCode vehicleCode, string vehicleParameterGroup)
		{
			var segment = DeclarationData.CompletedBusSegments.Lookup(numberOfAxles, vehicleCode, vehicleParameterGroup);

			Assert.AreEqual(_missionsTypes.Length, segment.Missions.Length);

			for (int i = 0; i < segment.Missions.Length; i++)
			{
				var mission = segment.Missions[i];

				Assert.AreEqual(_missionsTypes[i], mission.MissionType);
				Assert.AreEqual(5.8, mission.DefaultCDxA.Value());
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
				AssertAxleDistribution(axle1: 27.3, axle2: 45.4, axle3: 27.3, axle4: 0, axleLoadDistribution: mission.BusParameter.AxleLoadDistribution);
				AssertVehicleEquipment(externalDisplays: 1, internalDisplays: 1, fridge: 0, kitchenStandard: 0, vehicleEquipment: mission.BusParameter.VehicleEquipment);
			}
		}

		[TestCase(3, VehicleCode.CJ, "33e")]
		public void TestComplete3AxlesCompleteBus33E(int numberOfAxles, VehicleCode vehicleCode, string vehicleParameterGroup)
		{
			var segment = DeclarationData.CompletedBusSegments.Lookup(numberOfAxles, vehicleCode, vehicleParameterGroup);

			Assert.AreEqual(_missionsTypes.Length, segment.Missions.Length);

			for (int i = 0; i < segment.Missions.Length; i++)
			{
				var mission = segment.Missions[i];

				Assert.AreEqual(_missionsTypes[i], mission.MissionType);
				Assert.AreEqual(7.1, mission.DefaultCDxA.Value());
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
				AssertAxleDistribution(axle1: 27.3, axle2: 45.4, axle3: 27.3, axle4: 0, axleLoadDistribution: mission.BusParameter.AxleLoadDistribution);
				AssertVehicleEquipment(externalDisplays: 2, internalDisplays: 1, fridge: 0, kitchenStandard: 0, vehicleEquipment: mission.BusParameter.VehicleEquipment);
			}
		}

		[TestCase(3, VehicleCode.CA, "34a")]
		public void TestComplete3AxlesCompleteBus34A(int numberOfAxles, VehicleCode vehicleCode, string vehicleParameterGroup)
		{
			var segment = DeclarationData.CompletedBusSegments.Lookup(numberOfAxles, vehicleCode, vehicleParameterGroup);

			Assert.AreEqual(_missionsTypes.Length, segment.Missions.Length);

			for (int i = 0; i < segment.Missions.Length; i++)
			{
				var mission = segment.Missions[i];

				Assert.AreEqual(_missionsTypes[i], mission.MissionType);
				Assert.AreEqual(4.7, mission.DefaultCDxA.Value());
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
				AssertAxleDistribution(axle1: 27.3, axle2: 45.4, axle3: 27.3, axle4: 0, axleLoadDistribution: mission.BusParameter.AxleLoadDistribution);
				AssertVehicleEquipment(externalDisplays: 3, internalDisplays: 2, fridge: 0, kitchenStandard: 0, vehicleEquipment: mission.BusParameter.VehicleEquipment);
			}
		}

		[TestCase(3, VehicleCode.CA, "34b")]
		public void TestComplete3AxlesCompleteBus34B(int numberOfAxles, VehicleCode vehicleCode, string vehicleParameterGroup)
		{
			var segment = DeclarationData.CompletedBusSegments.Lookup(numberOfAxles, vehicleCode, vehicleParameterGroup);

			Assert.AreEqual(_missionsTypes.Length, segment.Missions.Length);

			for (int i = 0; i < segment.Missions.Length; i++)
			{
				var mission = segment.Missions[i];

				Assert.AreEqual(_missionsTypes[i], mission.MissionType);
				Assert.AreEqual(4.7, mission.DefaultCDxA.Value());
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
				AssertAxleDistribution(axle1: 27.3, axle2: 45.4, axle3: 27.3, axle4: 0, axleLoadDistribution: mission.BusParameter.AxleLoadDistribution);
				AssertVehicleEquipment(externalDisplays: 3, internalDisplays: 2, fridge: 0, kitchenStandard: 0, vehicleEquipment: mission.BusParameter.VehicleEquipment);
			}
		}

		[TestCase(3, VehicleCode.CA, "34c")]
		public void TestComplete3AxlesCompleteBus34C(int numberOfAxles, VehicleCode vehicleCode, string vehicleParameterGroup)
		{
			var segment = DeclarationData.CompletedBusSegments.Lookup(numberOfAxles, vehicleCode, vehicleParameterGroup);

			Assert.AreEqual(_missionsTypes.Length, segment.Missions.Length);

			for (int i = 0; i < segment.Missions.Length; i++)
			{
				var mission = segment.Missions[i];

				Assert.AreEqual(_missionsTypes[i], mission.MissionType);
				Assert.AreEqual(4.7, mission.DefaultCDxA.Value());
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
				AssertAxleDistribution(axle1: 27.3, axle2: 45.4, axle3: 27.3, axle4: 0, axleLoadDistribution: mission.BusParameter.AxleLoadDistribution);
				AssertVehicleEquipment(externalDisplays: 1, internalDisplays: 2, fridge: 1, kitchenStandard: 1, vehicleEquipment: mission.BusParameter.VehicleEquipment);
			}
		}

		[TestCase(3, VehicleCode.CA, "34d")]
		public void TestComplete3AxlesCompleteBus34D(int numberOfAxles, VehicleCode vehicleCode, string vehicleParameterGroup)
		{
			var segment = DeclarationData.CompletedBusSegments.Lookup(numberOfAxles, vehicleCode, vehicleParameterGroup);

			Assert.AreEqual(_missionsTypes.Length, segment.Missions.Length);

			for (int i = 0; i < segment.Missions.Length; i++)
			{
				var mission = segment.Missions[i];

				Assert.AreEqual(_missionsTypes[i], mission.MissionType);
				Assert.AreEqual(4.7, mission.DefaultCDxA.Value());
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
				AssertAxleDistribution(axle1: 27.3, axle2: 45.4, axle3: 27.3, axle4: 0, axleLoadDistribution: mission.BusParameter.AxleLoadDistribution);
				AssertVehicleEquipment(externalDisplays: 1, internalDisplays: 2, fridge: 1, kitchenStandard: 1, vehicleEquipment: mission.BusParameter.VehicleEquipment);
			}
		}

		[TestCase(3, VehicleCode.CB, "34e")]
		public void TestComplete3AxlesCompleteBus34E(int numberOfAxles, VehicleCode vehicleCode, string vehicleParameterGroup)
		{
			var segment = DeclarationData.CompletedBusSegments.Lookup(numberOfAxles, vehicleCode, vehicleParameterGroup);

			Assert.AreEqual(_missionsTypes.Length, segment.Missions.Length);

			for (int i = 0; i < segment.Missions.Length; i++)
			{
				var mission = segment.Missions[i];

				Assert.AreEqual(_missionsTypes[i], mission.MissionType);
				Assert.AreEqual(5.3, mission.DefaultCDxA.Value());
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
				AssertAxleDistribution(axle1: 27.3, axle2: 45.4, axle3: 27.3, axle4: 0, axleLoadDistribution: mission.BusParameter.AxleLoadDistribution);
				AssertVehicleEquipment(externalDisplays: 1, internalDisplays: 3, fridge: 1, kitchenStandard: 1, vehicleEquipment: mission.BusParameter.VehicleEquipment);
			}
		}

		[TestCase(3, VehicleCode.CB, "34f")]
		public void TestComplete3AxlesCompleteBus34F(int numberOfAxles, VehicleCode vehicleCode, string vehicleParameterGroup)
		{
			var segment = DeclarationData.CompletedBusSegments.Lookup(numberOfAxles, vehicleCode, vehicleParameterGroup);

			Assert.AreEqual(_missionsTypes.Length, segment.Missions.Length);

			for (int i = 0; i < segment.Missions.Length; i++)
			{
				var mission = segment.Missions[i];

				Assert.AreEqual(_missionsTypes[i], mission.MissionType);
				Assert.AreEqual(5.3, mission.DefaultCDxA.Value());
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
				AssertAxleDistribution(axle1: 27.3, axle2: 45.4, axle3: 27.3, axle4: 0, axleLoadDistribution: mission.BusParameter.AxleLoadDistribution);
				AssertVehicleEquipment(externalDisplays: 1, internalDisplays: 4, fridge: 1, kitchenStandard: 1.5, vehicleEquipment: mission.BusParameter.VehicleEquipment);
			}
		}

		[TestCase(3, VehicleCode.CG, "35a")]
		public void TestComplete3AxlesCompleteBus35A(int numberOfAxles, VehicleCode vehicleCode, string vehicleParameterGroup)
		{
			var segment = DeclarationData.CompletedBusSegments.Lookup(numberOfAxles, vehicleCode, vehicleParameterGroup);

			Assert.AreEqual(_missionsTypes.Length, segment.Missions.Length);

			for (int i = 0; i < segment.Missions.Length; i++)
			{
				var mission = segment.Missions[i];

				Assert.AreEqual(_missionsTypes[i], mission.MissionType);
				Assert.AreEqual(5.1, mission.DefaultCDxA.Value());
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
				AssertAxleDistribution(axle1: 27.3, axle2: 45.4, axle3: 27.3, axle4: 0, axleLoadDistribution: mission.BusParameter.AxleLoadDistribution);
				AssertVehicleEquipment(externalDisplays: 3, internalDisplays: 3, fridge: 0, kitchenStandard: 0, vehicleEquipment: mission.BusParameter.VehicleEquipment);
			}
		}

		[TestCase(3, VehicleCode.CG, "35b")]
		public void TestComplete3AxlesCompleteBus35B(int numberOfAxles, VehicleCode vehicleCode, string vehicleParameterGroup)
		{
			var segment = DeclarationData.CompletedBusSegments.Lookup(numberOfAxles, vehicleCode, vehicleParameterGroup);

			Assert.AreEqual(_missionsTypes.Length, segment.Missions.Length);

			for (int i = 0; i < segment.Missions.Length; i++)
			{
				var mission = segment.Missions[i];

				Assert.AreEqual(_missionsTypes[i], mission.MissionType);
				Assert.AreEqual(5.1, mission.DefaultCDxA.Value());
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
				AssertAxleDistribution(axle1: 27.3, axle2: 45.4, axle3: 27.3, axle4: 0, axleLoadDistribution: mission.BusParameter.AxleLoadDistribution);
				AssertVehicleEquipment(externalDisplays: 3, internalDisplays: 3, fridge: 0, kitchenStandard: 0, vehicleEquipment: mission.BusParameter.VehicleEquipment);
			}
		}

		[TestCase(3, VehicleCode.CH, "35c")]
		public void TestComplete3AxlesCompleteBus35C(int numberOfAxles, VehicleCode vehicleCode, string vehicleParameterGroup)
		{
			var segment = DeclarationData.CompletedBusSegments.Lookup(numberOfAxles, vehicleCode, vehicleParameterGroup);

			Assert.AreEqual(_missionsTypes.Length, segment.Missions.Length);

			for (int i = 0; i < segment.Missions.Length; i++)
			{
				var mission = segment.Missions[i];

				Assert.AreEqual(_missionsTypes[i], mission.MissionType);
				Assert.AreEqual(6.4, mission.DefaultCDxA.Value());
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
				AssertAxleDistribution(axle1: 27.3, axle2: 45.4, axle3: 27.3, axle4: 0, axleLoadDistribution: mission.BusParameter.AxleLoadDistribution);
				AssertVehicleEquipment(externalDisplays: 0, internalDisplays: 0, fridge: 0, kitchenStandard: 0, vehicleEquipment: mission.BusParameter.VehicleEquipment);
			}
		}

		[TestCase(3, VehicleCode.CC, "36a")]
		public void TestComplete3AxlesCompleteBus36A(int numberOfAxles, VehicleCode vehicleCode, string vehicleParameterGroup)
		{
			var segment = DeclarationData.CompletedBusSegments.Lookup(numberOfAxles, vehicleCode, vehicleParameterGroup);

			Assert.AreEqual(_missionsTypes.Length, segment.Missions.Length);

			for (int i = 0; i < segment.Missions.Length; i++)
			{
				var mission = segment.Missions[i];

				Assert.AreEqual(_missionsTypes[i], mission.MissionType);
				Assert.AreEqual(4.8, mission.DefaultCDxA.Value());
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
				AssertAxleDistribution(axle1: 27.3, axle2: 45.4, axle3: 27.3, axle4: 0, axleLoadDistribution: mission.BusParameter.AxleLoadDistribution);
				AssertVehicleEquipment(externalDisplays: 0, internalDisplays: 0, fridge: 0, kitchenStandard: 0, vehicleEquipment: mission.BusParameter.VehicleEquipment);
			}
		}

		[TestCase(3, VehicleCode.CC, "36b")]
		public void TestComplete3AxlesCompleteBus36B(int numberOfAxles, VehicleCode vehicleCode, string vehicleParameterGroup)
		{
			var segment = DeclarationData.CompletedBusSegments.Lookup(numberOfAxles, vehicleCode, vehicleParameterGroup);

			Assert.AreEqual(_missionsTypes.Length, segment.Missions.Length);

			for (int i = 0; i < segment.Missions.Length; i++)
			{
				var mission = segment.Missions[i];

				Assert.AreEqual(_missionsTypes[i], mission.MissionType);
				Assert.AreEqual(4.8, mission.DefaultCDxA.Value());
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
				AssertAxleDistribution(axle1: 27.3, axle2: 45.4, axle3: 27.3, axle4: 0, axleLoadDistribution: mission.BusParameter.AxleLoadDistribution);
				AssertVehicleEquipment(externalDisplays: 0, internalDisplays: 0, fridge: 0, kitchenStandard: 0, vehicleEquipment: mission.BusParameter.VehicleEquipment);
			}
		}

		[TestCase(3, VehicleCode.CC, "36c")]
		public void TestComplete3AxlesCompleteBus36C(int numberOfAxles, VehicleCode vehicleCode, string vehicleParameterGroup)
		{
			var segment = DeclarationData.CompletedBusSegments.Lookup(numberOfAxles, vehicleCode, vehicleParameterGroup);

			Assert.AreEqual(_missionsTypes.Length, segment.Missions.Length);

			for (int i = 0; i < segment.Missions.Length; i++)
			{
				var mission = segment.Missions[i];

				Assert.AreEqual(_missionsTypes[i], mission.MissionType);
				Assert.AreEqual(4.8, mission.DefaultCDxA.Value());
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
				AssertAxleDistribution(axle1: 27.3, axle2: 45.4, axle3: 27.3, axle4: 0, axleLoadDistribution: mission.BusParameter.AxleLoadDistribution);
				AssertVehicleEquipment(externalDisplays: 0, internalDisplays: 0, fridge: 0, kitchenStandard: 0, vehicleEquipment: mission.BusParameter.VehicleEquipment);
			}
		}

		[TestCase(3, VehicleCode.CC, "36d")]
		public void TestComplete3AxlesCompleteBus36D(int numberOfAxles, VehicleCode vehicleCode, string vehicleParameterGroup)
		{
			var segment = DeclarationData.CompletedBusSegments.Lookup(numberOfAxles, vehicleCode, vehicleParameterGroup);

			Assert.AreEqual(_missionsTypes.Length, segment.Missions.Length);

			for (int i = 0; i < segment.Missions.Length; i++)
			{
				var mission = segment.Missions[i];

				Assert.AreEqual(_missionsTypes[i], mission.MissionType);
				Assert.AreEqual(4.8, mission.DefaultCDxA.Value());
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
				AssertAxleDistribution(axle1: 27.3, axle2: 45.4, axle3: 27.3, axle4: 0, axleLoadDistribution: mission.BusParameter.AxleLoadDistribution);
				AssertVehicleEquipment(externalDisplays: 0, internalDisplays: 0, fridge: 0, kitchenStandard: 0, vehicleEquipment: mission.BusParameter.VehicleEquipment);
			}
		}

		[TestCase(3, VehicleCode.CD, "36e")]
		public void TestComplete3AxlesCompleteBus36E(int numberOfAxles, VehicleCode vehicleCode, string vehicleParameterGroup)
		{
			var segment = DeclarationData.CompletedBusSegments.Lookup(numberOfAxles, vehicleCode, vehicleParameterGroup);

			Assert.AreEqual(_missionsTypes.Length, segment.Missions.Length);

			for (int i = 0; i < segment.Missions.Length; i++)
			{
				var mission = segment.Missions[i];

				Assert.AreEqual(_missionsTypes[i], mission.MissionType);
				Assert.AreEqual(5.4, mission.DefaultCDxA.Value());
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
				AssertAxleDistribution(axle1: 27.3, axle2: 45.4, axle3: 27.3, axle4: 0, axleLoadDistribution: mission.BusParameter.AxleLoadDistribution);
				AssertVehicleEquipment(externalDisplays: 0, internalDisplays: 0, fridge: 0, kitchenStandard: 0, vehicleEquipment: mission.BusParameter.VehicleEquipment);
			}
		}

		[TestCase(3, VehicleCode.CD, "36f")]
		public void TestComplete3AxlesCompleteBus36F(int numberOfAxles, VehicleCode vehicleCode, string vehicleParameterGroup)
		{
			var segment = DeclarationData.CompletedBusSegments.Lookup(numberOfAxles, vehicleCode, vehicleParameterGroup);

			Assert.AreEqual(_missionsTypes.Length, segment.Missions.Length);

			for (int i = 0; i < segment.Missions.Length; i++)
			{
				var mission = segment.Missions[i];

				Assert.AreEqual(_missionsTypes[i], mission.MissionType);
				Assert.AreEqual(5.4, mission.DefaultCDxA.Value());
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
				AssertAxleDistribution(axle1: 27.3, axle2: 45.4, axle3: 27.3, axle4: 0, axleLoadDistribution: mission.BusParameter.AxleLoadDistribution);
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
