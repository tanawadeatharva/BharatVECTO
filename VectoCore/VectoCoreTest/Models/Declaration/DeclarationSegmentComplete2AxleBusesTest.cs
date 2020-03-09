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
	public class DeclarationSegmentComplete2AxleBusesTest
	{

		private MissionType[] missionsTypes;

		[OneTimeSetUp]
		public void RunBeforeAnyTest()
		{
			missionsTypes = new[]
				{ MissionType.HeavyUrban, MissionType.Urban, MissionType.Suburban, MissionType.Interurban, MissionType.Coach };
		}

		[
			TestCase(2, VehicleCode.CE, "31a"),
			TestCase(2, VehicleCode.CE, "31b"),
			TestCase(2, VehicleCode.CF, "31c"),
			TestCase(2, VehicleCode.CI, "31d"),
			TestCase(2, VehicleCode.CJ, "31e"),
			TestCase(2, VehicleCode.CA, "32a"),
			TestCase(2, VehicleCode.CA, "32b"),
			TestCase(2, VehicleCode.CA, "32c"),
			TestCase(2, VehicleCode.CA, "32d"),
			TestCase(2, VehicleCode.CB, "32e"),
			TestCase(2, VehicleCode.CB, "32f"),
		]
		public void SegmentLookupTest(int numberOfAxles, VehicleCode vehicleCode, string vehicleParameterGroup)
		{
			var segment = DeclarationData.CompletedBusSegments.Lookup(numberOfAxles, vehicleCode, vehicleParameterGroup);
			Assert.AreEqual(5, segment.Missions.Length);
		}

		[TestCase(2, VehicleCode.CE, "31a")]
		public void TestComplete2AxlesCompleteBus31A(int numberOfAxles, VehicleCode vehicleCode, string vehicleParameterGroup)
		{
			var segment = DeclarationData.CompletedBusSegments.Lookup(numberOfAxles, vehicleCode, vehicleParameterGroup);
			Assert.AreEqual(missionsTypes.Length, segment.Missions.Length);
			
			for (int i = 0; i < segment.Missions.Length; i++) {
				var mission = segment.Missions[i];

				Assert.AreEqual(missionsTypes[i], mission.MissionType);
				Assert.AreEqual(4.9, mission.DefaultCDxA.Value());
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
				AssertAxleDistribution(axle1: 37.5, axle2: 62.5, axle3: 0, axle4: 0, axleLoadDistribution: mission.BusParameter.AxleLoadDistribution);
				AssertVehicleEquipment(externalDisplays: 3, internalDisplays: 2, fridge: 0, kitchenStandard: 0,
					vehicleEquipment: mission.BusParameter.VehicleEquipment);
			}
		}

		[TestCase(2, VehicleCode.CE, "31b")]
		public void TestComplete2AxlesCompleteBus31B(int numberOfAxles, VehicleCode vehicleCode, string vehicleParameterGroup)
		{
			var segment = DeclarationData.CompletedBusSegments.Lookup(numberOfAxles, vehicleCode, vehicleParameterGroup);

			Assert.AreEqual(missionsTypes.Length, segment.Missions.Length);

			for (int i = 0; i < segment.Missions.Length; i++) {
				var mission = segment.Missions[i];

				Assert.AreEqual(missionsTypes[i], mission.MissionType);
				Assert.AreEqual(4.9, mission.DefaultCDxA.Value());
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
				AssertAxleDistribution(axle1: 37.5, axle2: 62.5, axle3: 0, axle4: 0, axleLoadDistribution: mission.BusParameter.AxleLoadDistribution);
				AssertVehicleEquipment(externalDisplays: 3, internalDisplays: 2, fridge: 0, kitchenStandard: 0, vehicleEquipment: mission.BusParameter.VehicleEquipment);
			}
		}

		[TestCase(2, VehicleCode.CF, "31c")]
		public void TestComplete2AxlesCompleteBus31C(int numberOfAxles, VehicleCode vehicleCode, string vehicleParameterGroup)
		{
			var segment = DeclarationData.CompletedBusSegments.Lookup(numberOfAxles, vehicleCode, vehicleParameterGroup);

			Assert.AreEqual(missionsTypes.Length, segment.Missions.Length);

			for (int i = 0; i < segment.Missions.Length; i++)
			{
				var mission = segment.Missions[i];

				Assert.AreEqual(missionsTypes[i], mission.MissionType);
				Assert.AreEqual(6.2, mission.DefaultCDxA.Value());
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
					vehicleParam:vehicleParameterGroup,
					busParameters: mission.BusParameter
				);

				AssertAveragePassengers(heavyUrban: 3.7, urban: 3.7, suburban: 3.7, interurban: 0, coach: 0, busParameters: mission.BusParameter);
				AssertAxleDistribution(axle1: 37.5, axle2: 62.5, axle3: 0, axle4: 0, axleLoadDistribution: mission.BusParameter.AxleLoadDistribution);
				AssertVehicleEquipment(externalDisplays: 3, internalDisplays: 3, fridge: 0, kitchenStandard: 0, vehicleEquipment: mission.BusParameter.VehicleEquipment);
			}
		}

		[TestCase(2, VehicleCode.CI, "31d")]
		public void TestComplete2AxlesCompleteBus31D(int numberOfAxles, VehicleCode vehicleCode, string vehicleParameterGroup)
		{
			var segment = DeclarationData.CompletedBusSegments.Lookup(numberOfAxles, vehicleCode, vehicleParameterGroup);

			Assert.AreEqual(missionsTypes.Length, segment.Missions.Length);

			for (int i = 0; i < segment.Missions.Length; i++)
			{
				var mission = segment.Missions[i];

				Assert.AreEqual(missionsTypes[i], mission.MissionType);
				Assert.AreEqual(5.7, mission.DefaultCDxA.Value());
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
				AssertAxleDistribution(axle1: 37.5, axle2: 62.5, axle3: 0, axle4: 0, axleLoadDistribution: mission.BusParameter.AxleLoadDistribution);
				AssertVehicleEquipment(externalDisplays: 1, internalDisplays: 1, fridge: 0, kitchenStandard: 0, vehicleEquipment: mission.BusParameter.VehicleEquipment);
			}
		}

		[TestCase(2, VehicleCode.CJ, "31e")]
		public void TestComplete2AxlesCompleteBus31E(int numberOfAxles, VehicleCode vehicleCode, string vehicleParameterGroup)
		{
			var segment = DeclarationData.CompletedBusSegments.Lookup(numberOfAxles, vehicleCode, vehicleParameterGroup);

			Assert.AreEqual(missionsTypes.Length, segment.Missions.Length);

			for (int i = 0; i < segment.Missions.Length; i++)
			{
				var mission = segment.Missions[i];

				Assert.AreEqual(missionsTypes[i], mission.MissionType);
				Assert.AreEqual(7, mission.DefaultCDxA.Value());
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
				AssertAxleDistribution(axle1: 37.5, axle2: 62.5, axle3: 0, axle4: 0, axleLoadDistribution: mission.BusParameter.AxleLoadDistribution);
				AssertVehicleEquipment(externalDisplays: 2, internalDisplays: 1, fridge: 0, kitchenStandard: 0, vehicleEquipment: mission.BusParameter.VehicleEquipment);
			}
		}

		[TestCase(2, VehicleCode.CA, "32a")]
		public void TestComplete2AxlesCompleteBus32A(int numberOfAxles, VehicleCode vehicleCode, string vehicleParameterGroup)
		{
			var segment = DeclarationData.CompletedBusSegments.Lookup(numberOfAxles, vehicleCode, vehicleParameterGroup);

			Assert.AreEqual(missionsTypes.Length, segment.Missions.Length);

			for (int i = 0; i < segment.Missions.Length; i++)
			{
				var mission = segment.Missions[i];

				Assert.AreEqual(missionsTypes[i], mission.MissionType);
				Assert.AreEqual(4.6, mission.DefaultCDxA.Value());
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
				AssertAxleDistribution(axle1: 37.5, axle2: 62.5, axle3: 0, axle4: 0, axleLoadDistribution: mission.BusParameter.AxleLoadDistribution);
				AssertVehicleEquipment(externalDisplays: 3, internalDisplays: 2, fridge: 0, kitchenStandard: 0, vehicleEquipment: mission.BusParameter.VehicleEquipment);
			}
		}

		[TestCase(2, VehicleCode.CA, "32b")]
		public void TestComplete2AxlesCompleteBus32B(int numberOfAxles, VehicleCode vehicleCode, string vehicleParameterGroup)
		{
			var segment = DeclarationData.CompletedBusSegments.Lookup(numberOfAxles, vehicleCode, vehicleParameterGroup);

			Assert.AreEqual(missionsTypes.Length, segment.Missions.Length);

			for (int i = 0; i < segment.Missions.Length; i++)
			{
				var mission = segment.Missions[i];

				Assert.AreEqual(missionsTypes[i], mission.MissionType);
				Assert.AreEqual(4.6, mission.DefaultCDxA.Value());
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
				AssertAxleDistribution(axle1: 37.5, axle2: 62.5, axle3: 0, axle4: 0, axleLoadDistribution: mission.BusParameter.AxleLoadDistribution);
				AssertVehicleEquipment(externalDisplays: 3, internalDisplays: 2, fridge: 0, kitchenStandard: 0, vehicleEquipment: mission.BusParameter.VehicleEquipment);
			}
		}

		[TestCase(2, VehicleCode.CA, "32c")]
		public void TestComplete2AxlesCompleteBus32C(int numberOfAxles, VehicleCode vehicleCode, string vehicleParameterGroup)
		{
			var segment = DeclarationData.CompletedBusSegments.Lookup(numberOfAxles, vehicleCode, vehicleParameterGroup);

			Assert.AreEqual(missionsTypes.Length, segment.Missions.Length);

			for (int i = 0; i < segment.Missions.Length; i++)
			{
				var mission = segment.Missions[i];

				Assert.AreEqual(missionsTypes[i], mission.MissionType);
				Assert.AreEqual(4.6, mission.DefaultCDxA.Value());
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
				AssertAxleDistribution(axle1: 37.5, axle2: 62.5, axle3: 0, axle4: 0, axleLoadDistribution: mission.BusParameter.AxleLoadDistribution);
				AssertVehicleEquipment(externalDisplays: 1, internalDisplays: 2, fridge: 1, kitchenStandard: 1, vehicleEquipment: mission.BusParameter.VehicleEquipment);
			}
		}

		[TestCase(2, VehicleCode.CA, "32d")]
		public void TestComplete2AxlesCompleteBus32D(int numberOfAxles, VehicleCode vehicleCode, string vehicleParameterGroup)
		{
			var segment = DeclarationData.CompletedBusSegments.Lookup(numberOfAxles, vehicleCode, vehicleParameterGroup);

			Assert.AreEqual(missionsTypes.Length, segment.Missions.Length);

			for (int i = 0; i < segment.Missions.Length; i++)
			{
				var mission = segment.Missions[i];

				Assert.AreEqual(missionsTypes[i], mission.MissionType);
				Assert.AreEqual(4.6, mission.DefaultCDxA.Value());
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
				AssertAxleDistribution(axle1: 37.5, axle2: 62.5, axle3: 0, axle4: 0, axleLoadDistribution: mission.BusParameter.AxleLoadDistribution);
				AssertVehicleEquipment(externalDisplays: 1, internalDisplays: 2, fridge: 1, kitchenStandard: 1, vehicleEquipment: mission.BusParameter.VehicleEquipment);
			}
		}
		[TestCase(2, VehicleCode.CB, "32e")]
		public void TestComplete2AxlesCompleteBus32E(int numberOfAxles, VehicleCode vehicleCode, string vehicleParameterGroup)
		{
			var segment = DeclarationData.CompletedBusSegments.Lookup(numberOfAxles, vehicleCode, vehicleParameterGroup);

			Assert.AreEqual(missionsTypes.Length, segment.Missions.Length);

			for (int i = 0; i < segment.Missions.Length; i++)
			{
				var mission = segment.Missions[i];

				Assert.AreEqual(missionsTypes[i], mission.MissionType);
				Assert.AreEqual(5.2, mission.DefaultCDxA.Value());
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
				AssertAxleDistribution(axle1: 37.5, axle2: 62.5, axle3: 0, axle4: 0, axleLoadDistribution: mission.BusParameter.AxleLoadDistribution);
				AssertVehicleEquipment(externalDisplays: 1, internalDisplays: 2, fridge: 1, kitchenStandard: 1, vehicleEquipment: mission.BusParameter.VehicleEquipment);
			}
		}

		[TestCase(2, VehicleCode.CB, "32f")]
		public void TestComplete2AxlesCompleteBus32F(int numberOfAxles, VehicleCode vehicleCode, string vehicleParameterGroup)
		{
			var segment = DeclarationData.CompletedBusSegments.Lookup(numberOfAxles, vehicleCode, vehicleParameterGroup);

			Assert.AreEqual(missionsTypes.Length, segment.Missions.Length);

			for (int i = 0; i < segment.Missions.Length; i++)
			{
				var mission = segment.Missions[i];

				Assert.AreEqual(missionsTypes[i], mission.MissionType);
				Assert.AreEqual(5.2, mission.DefaultCDxA.Value());
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
				AssertAxleDistribution(axle1: 37.5, axle2: 62.5, axle3: 0, axle4: 0, axleLoadDistribution: mission.BusParameter.AxleLoadDistribution);
				AssertVehicleEquipment(externalDisplays: 1, internalDisplays: 2, fridge: 1, kitchenStandard: 1, vehicleEquipment: mission.BusParameter.VehicleEquipment);
			}
		}

		
		private void AssertBusParameters(bool isDoubleDeck, int numberOfAxles, bool isArticulated, FloorType floorType,  VehicleCode vehicleCode, string regClasses, 
			bool? lowEntry, double numPassengersLower, bool? passengersLowerOrEqual, double? bodyHeight, bool? bodyHeightLowerOrEqual, 
			string vehicleParam, BusParameters busParameters)
		{
			Assert.AreEqual(isDoubleDeck, vehicleCode.IsDoubleDeckBus());
			Assert.AreEqual(numberOfAxles, busParameters.NumberOfAxles);
			Assert.AreEqual(isArticulated, busParameters.IsArticulated);
			Assert.AreEqual(isArticulated, busParameters.IsArticulated);

			Assert.AreEqual(floorType, busParameters.FloorType);
			if(floorType == FloorType.Unknown)
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
			Assert.AreEqual(heavyUrban , busParameters.PassengersHeavyUrban);
			Assert.AreEqual(urban, busParameters.PassengersUrban);
			Assert.AreEqual(suburban, busParameters.PassengersSuburban);
			Assert.AreEqual(interurban , busParameters.PassengersInterurban);
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
	}
}
