using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Declaration;

namespace TUGraz.VectoCore.Tests.Models.Declaration
{
	[TestFixture]
	public class DeclarationSegmentComplete2AxleBusesTest
	{
		private MissionType[] _missionsTypes;

		[OneTimeSetUp]
		public void RunBeforeAnyTest()
		{
			_missionsTypes = new[]
				{ MissionType.HeavyUrban, MissionType.Urban, MissionType.Suburban, MissionType.Interurban, MissionType.Coach };
		}

		[
			TestCase(AxleConfiguration.AxleConfig_4x2, VehicleCode.CE, RegistrationClass.I, 0, 0, false, VehicleClass.ClassCB31a, 3),
			TestCase(AxleConfiguration.AxleConfig_4x2, VehicleCode.CE, RegistrationClass.I, 0, 0, true, VehicleClass.ClassCB31b, 3),
			TestCase(AxleConfiguration.AxleConfig_4x2, VehicleCode.CF, RegistrationClass.I, 0, 0, false, VehicleClass.ClassCB31c, 3),
			TestCase(AxleConfiguration.AxleConfig_4x2, VehicleCode.CI, RegistrationClass.I, 0, 0, false, VehicleClass.ClassCB31d, 3),
			TestCase(AxleConfiguration.AxleConfig_4x2, VehicleCode.CJ, RegistrationClass.I, 0, 0, false, VehicleClass.ClassCB31e, 3),
			TestCase(AxleConfiguration.AxleConfig_4x2, VehicleCode.CA, RegistrationClass.II, 0, 0, false, VehicleClass.ClassCB32a, 2),
			TestCase(AxleConfiguration.AxleConfig_4x2, VehicleCode.CA, RegistrationClass.II_III, 0, 3.1, false, VehicleClass.ClassCB32b, 2),
			TestCase(AxleConfiguration.AxleConfig_4x2, VehicleCode.CA, RegistrationClass.II_III, 0, 3.1001, false, VehicleClass.ClassCB32c, 2),
			TestCase(AxleConfiguration.AxleConfig_4x2, VehicleCode.CA, RegistrationClass.III, 0, 0, false, VehicleClass.ClassCB32d, 2),
			TestCase(AxleConfiguration.AxleConfig_4x2, VehicleCode.CB, RegistrationClass.II, 6, 0, false, VehicleClass.ClassCB32e, 2),
			TestCase(AxleConfiguration.AxleConfig_4x2, VehicleCode.CB, RegistrationClass.II, 7, 0, false, VehicleClass.ClassCB32f, 2),
		]
		public void SegmentLookupTest(AxleConfiguration axleConfig, VehicleCode vehicleCode, RegistrationClass registrationClass, int passengersLowerDeck, double bodyHeight, bool lowEntry,  VehicleClass vehicleParameterGroup, int numberOfMissions)
		{
			var segment = DeclarationData.CompletedBusSegments.Lookup(axleConfig.NumAxles(), vehicleCode, registrationClass, passengersLowerDeck, bodyHeight.SI<Meter>(), lowEntry);
			Assert.AreEqual(numberOfMissions, segment.Missions.Length);
			Assert.AreEqual(vehicleParameterGroup, segment.VehicleClass);
		}

		[TestCase(2, VehicleCode.CE, "31a")]
		public void TestComplete2AxlesCompleteBus31A(AxleConfiguration axleConfig, VehicleCode vehicleCode, RegistrationClass registrationClass, int passengersLowerDeck, double bodyHeight,  string vehicleParameterGroup, int numberOfMissions)
		{
			var segment = DeclarationData.CompletedBusSegments.Lookup(axleConfig.NumAxles(), vehicleCode, registrationClass, passengersLowerDeck, bodyHeight.SI<Meter>());
			Assert.AreEqual(3, segment.Missions.Length);

			for (int i = 0; i < segment.Missions.Length; i++)
			{
				var mission = segment.Missions[i];

				Assert.AreEqual(_missionsTypes[i], mission.MissionType);
				Assert.AreEqual(4.9, mission.DefaultCDxA.Value());
				Assert.AreEqual(false, mission.AirDragMeasurement);

				AssertBusParameters(
					isDoubleDeck: false,
					floorType: FloorType.LowFloor,
					vehicleCode: vehicleCode,
					busParameters: mission.BusParameter
				);

				AssertAxleDistribution(axle1: 37.5, axle2: 62.5, axle3: 0, axle4: 0, axleLoadDistribution: mission.AxleWeightDistribution);
				AssertVehicleEquipment(externalDisplays: 3, internalDisplays: 2, fridge: 0, kitchenStandard: 0,
					vehicleEquipment: mission.BusParameter.VehicleEquipment);
			}
		}

		[TestCase(2, VehicleCode.CE, "31b")]
		public void TestComplete2AxlesCompleteBus31B(AxleConfiguration axleConfig, VehicleCode vehicleCode, RegistrationClass registrationClass, int passengersLowerDeck, double bodyHeight, string vehicleParameterGroup, int numberOfMissions)
		{
			var segment = DeclarationData.CompletedBusSegments.Lookup(axleConfig.NumAxles(), vehicleCode, registrationClass, passengersLowerDeck, bodyHeight.SI<Meter>());

			Assert.AreEqual(3, segment.Missions.Length);

			for (int i = 0; i < segment.Missions.Length; i++)
			{
				var mission = segment.Missions[i];

				Assert.AreEqual(_missionsTypes[i], mission.MissionType);
				Assert.AreEqual(4.9, mission.DefaultCDxA.Value());
				Assert.AreEqual(false, mission.AirDragMeasurement);

				AssertBusParameters(
					isDoubleDeck: false,
					floorType: FloorType.LowFloor,
					vehicleCode: vehicleCode,
					busParameters: mission.BusParameter
				);

				AssertAxleDistribution(axle1: 37.5, axle2: 62.5, axle3: 0, axle4: 0, axleLoadDistribution: mission.AxleWeightDistribution);
				AssertVehicleEquipment(externalDisplays: 3, internalDisplays: 2, fridge: 0, kitchenStandard: 0, vehicleEquipment: mission.BusParameter.VehicleEquipment);
			}
		}

		[TestCase(2, VehicleCode.CF, "31c")]
		public void TestComplete2AxlesCompleteBus31C(AxleConfiguration axleConfig, VehicleCode vehicleCode, RegistrationClass registrationClass, int passengersLowerDeck, double bodyHeight,  string vehicleParameterGroup, int numberOfMissions)
		{
			var segment = DeclarationData.CompletedBusSegments.Lookup(axleConfig.NumAxles(), vehicleCode, registrationClass, passengersLowerDeck, bodyHeight.SI<Meter>());

			Assert.AreEqual(3, segment.Missions.Length);

			for (int i = 0; i < segment.Missions.Length; i++)
			{
				var mission = segment.Missions[i];

				Assert.AreEqual(_missionsTypes[i], mission.MissionType);
				Assert.AreEqual(6.2, mission.DefaultCDxA.Value());
				Assert.AreEqual(false, mission.AirDragMeasurement);

				AssertBusParameters(
					isDoubleDeck: true,
					floorType: FloorType.LowFloor,
					vehicleCode: vehicleCode,
					busParameters: mission.BusParameter
				);

				AssertAxleDistribution(axle1: 37.5, axle2: 62.5, axle3: 0, axle4: 0, axleLoadDistribution: mission.AxleWeightDistribution);
				AssertVehicleEquipment(externalDisplays: 3, internalDisplays: 3, fridge: 0, kitchenStandard: 0, vehicleEquipment: mission.BusParameter.VehicleEquipment);
			}
		}

		[TestCase(2, VehicleCode.CI, "31d")]
		public void TestComplete2AxlesCompleteBus31D(AxleConfiguration axleConfig, VehicleCode vehicleCode, RegistrationClass registrationClass, int passengersLowerDeck, double bodyHeight,  string vehicleParameterGroup, int numberOfMissions)
		{
			var segment = DeclarationData.CompletedBusSegments.Lookup(axleConfig.NumAxles(), vehicleCode, registrationClass, passengersLowerDeck, bodyHeight.SI<Meter>());

			Assert.AreEqual(3, segment.Missions.Length);

			for (int i = 0; i < segment.Missions.Length; i++)
			{
				var mission = segment.Missions[i];

				Assert.AreEqual(_missionsTypes[i], mission.MissionType);
				Assert.AreEqual(5.7, mission.DefaultCDxA.Value());
				Assert.AreEqual(false, mission.AirDragMeasurement);

				AssertBusParameters(
					isDoubleDeck: false,
					floorType: FloorType.Unknown,
					vehicleCode: vehicleCode,
					busParameters: mission.BusParameter
				);

				AssertAxleDistribution(axle1: 37.5, axle2: 62.5, axle3: 0, axle4: 0, axleLoadDistribution: mission.AxleWeightDistribution);
				AssertVehicleEquipment(externalDisplays: 1, internalDisplays: 1, fridge: 0, kitchenStandard: 0, vehicleEquipment: mission.BusParameter.VehicleEquipment);
			}
		}

		[TestCase(2, VehicleCode.CJ, "31e")]
		public void TestComplete2AxlesCompleteBus31E(AxleConfiguration axleConfig, VehicleCode vehicleCode, RegistrationClass registrationClass, int passengersLowerDeck, double bodyHeight,  string vehicleParameterGroup, int numberOfMissions)
		{
			var segment = DeclarationData.CompletedBusSegments.Lookup(axleConfig.NumAxles(), vehicleCode, registrationClass, passengersLowerDeck, bodyHeight.SI<Meter>());

			Assert.AreEqual(3, segment.Missions.Length);

			for (int i = 0; i < segment.Missions.Length; i++)
			{
				var mission = segment.Missions[i];

				Assert.AreEqual(_missionsTypes[i], mission.MissionType);
				Assert.AreEqual(7, mission.DefaultCDxA.Value());
				Assert.AreEqual(false, mission.AirDragMeasurement);

				AssertBusParameters(
					isDoubleDeck: true,
					floorType: FloorType.Unknown,
					vehicleCode: vehicleCode,
					busParameters: mission.BusParameter
				);

				AssertAxleDistribution(axle1: 37.5, axle2: 62.5, axle3: 0, axle4: 0, axleLoadDistribution: mission.AxleWeightDistribution);
				AssertVehicleEquipment(externalDisplays: 2, internalDisplays: 1, fridge: 0, kitchenStandard: 0, vehicleEquipment: mission.BusParameter.VehicleEquipment);
			}
		}

		[TestCase(2, VehicleCode.CA, "32a")]
		public void TestComplete2AxlesCompleteBus32A(AxleConfiguration axleConfig, VehicleCode vehicleCode, RegistrationClass registrationClass, int passengersLowerDeck, double bodyHeight,  string vehicleParameterGroup, int numberOfMissions)
		{
			var segment = DeclarationData.CompletedBusSegments.Lookup(axleConfig.NumAxles(), vehicleCode, registrationClass, passengersLowerDeck, bodyHeight.SI<Meter>());

			Assert.AreEqual(2, segment.Missions.Length);

			for (int i = 0; i < segment.Missions.Length; i++)
			{
				var mission = segment.Missions[i];

				Assert.AreEqual(_missionsTypes[i + 3], mission.MissionType);
				Assert.AreEqual(4.6, mission.DefaultCDxA.Value());
				Assert.AreEqual(true, mission.AirDragMeasurement);

				AssertBusParameters(
					isDoubleDeck: false,
					floorType: FloorType.HighFloor,
					vehicleCode: vehicleCode,
					busParameters: mission.BusParameter
				);

				AssertAxleDistribution(axle1: 37.5, axle2: 62.5, axle3: 0, axle4: 0, axleLoadDistribution: mission.AxleWeightDistribution);
				AssertVehicleEquipment(externalDisplays: 3, internalDisplays: 2, fridge: 0, kitchenStandard: 0, vehicleEquipment: mission.BusParameter.VehicleEquipment);
			}
		}

		[TestCase(2, VehicleCode.CA, "32b")]
		public void TestComplete2AxlesCompleteBus32B(AxleConfiguration axleConfig, VehicleCode vehicleCode, RegistrationClass registrationClass, int passengersLowerDeck, double bodyHeight,  string vehicleParameterGroup, int numberOfMissions)
		{
			var segment = DeclarationData.CompletedBusSegments.Lookup(axleConfig.NumAxles(), vehicleCode, registrationClass, passengersLowerDeck, bodyHeight.SI<Meter>());

			Assert.AreEqual(2, segment.Missions.Length);

			for (int i = 0; i < segment.Missions.Length; i++)
			{
				var mission = segment.Missions[i];

				Assert.AreEqual(_missionsTypes[i + 3], mission.MissionType);
				Assert.AreEqual(4.6, mission.DefaultCDxA.Value());
				Assert.AreEqual(true, mission.AirDragMeasurement);

				AssertBusParameters(
					isDoubleDeck: false,
					floorType: FloorType.HighFloor,
					vehicleCode: vehicleCode,
					busParameters: mission.BusParameter
				);

				AssertAxleDistribution(axle1: 37.5, axle2: 62.5, axle3: 0, axle4: 0, axleLoadDistribution: mission.AxleWeightDistribution);
				AssertVehicleEquipment(externalDisplays: 3, internalDisplays: 2, fridge: 0, kitchenStandard: 0, vehicleEquipment: mission.BusParameter.VehicleEquipment);
			}
		}

		[TestCase(2, VehicleCode.CA, "32c")]
		public void TestComplete2AxlesCompleteBus32C(AxleConfiguration axleConfig, VehicleCode vehicleCode, RegistrationClass registrationClass, int passengersLowerDeck, double bodyHeight,  string vehicleParameterGroup, int numberOfMissions)
		{
			var segment = DeclarationData.CompletedBusSegments.Lookup(axleConfig.NumAxles(), vehicleCode, registrationClass, passengersLowerDeck, bodyHeight.SI<Meter>());

			Assert.AreEqual(2, segment.Missions.Length);

			for (int i = 0; i < segment.Missions.Length; i++)
			{
				var mission = segment.Missions[i];

				Assert.AreEqual(_missionsTypes[i + 3], mission.MissionType);
				Assert.AreEqual(4.6, mission.DefaultCDxA.Value());
				Assert.AreEqual(true, mission.AirDragMeasurement);

				AssertBusParameters(
					isDoubleDeck: false,
					floorType: FloorType.HighFloor,
					vehicleCode: vehicleCode,
					busParameters: mission.BusParameter
				);

				AssertAxleDistribution(axle1: 37.5, axle2: 62.5, axle3: 0, axle4: 0, axleLoadDistribution: mission.AxleWeightDistribution);
				AssertVehicleEquipment(externalDisplays: 1, internalDisplays: 2, fridge: 1, kitchenStandard: 1, vehicleEquipment: mission.BusParameter.VehicleEquipment);
			}
		}

		[TestCase(2, VehicleCode.CA, "32d")]
		public void TestComplete2AxlesCompleteBus32D(AxleConfiguration axleConfig, VehicleCode vehicleCode, RegistrationClass registrationClass, int passengersLowerDeck, double bodyHeight,  string vehicleParameterGroup, int numberOfMissions)
		{
			var segment = DeclarationData.CompletedBusSegments.Lookup(axleConfig.NumAxles(), vehicleCode, registrationClass, passengersLowerDeck, bodyHeight.SI<Meter>());

			Assert.AreEqual(2, segment.Missions.Length);

			for (int i = 0; i < segment.Missions.Length; i++)
			{
				var mission = segment.Missions[i];

				Assert.AreEqual(_missionsTypes[i + 3], mission.MissionType);
				Assert.AreEqual(4.6, mission.DefaultCDxA.Value());
				Assert.AreEqual(true, mission.AirDragMeasurement);

				AssertBusParameters(
					isDoubleDeck: false,
					floorType: FloorType.HighFloor,
					vehicleCode: vehicleCode,
					busParameters: mission.BusParameter
				);

				AssertAxleDistribution(axle1: 37.5, axle2: 62.5, axle3: 0, axle4: 0, axleLoadDistribution: mission.AxleWeightDistribution);
				AssertVehicleEquipment(externalDisplays: 1, internalDisplays: 2, fridge: 1, kitchenStandard: 1, vehicleEquipment: mission.BusParameter.VehicleEquipment);
			}
		}
		[TestCase(2, VehicleCode.CB, "32e")]
		public void TestComplete2AxlesCompleteBus32E(AxleConfiguration axleConfig, VehicleCode vehicleCode, RegistrationClass registrationClass, int passengersLowerDeck, double bodyHeight,  string vehicleParameterGroup, int numberOfMissions)
		{
			var segment = DeclarationData.CompletedBusSegments.Lookup(axleConfig.NumAxles(), vehicleCode, registrationClass, passengersLowerDeck, bodyHeight.SI<Meter>());

			Assert.AreEqual(2, segment.Missions.Length);

			for (int i = 0; i < segment.Missions.Length; i++)
			{
				var mission = segment.Missions[i];

				Assert.AreEqual(_missionsTypes[i + 3], mission.MissionType);
				Assert.AreEqual(5.2, mission.DefaultCDxA.Value());
				Assert.AreEqual(true, mission.AirDragMeasurement);

				AssertBusParameters(
					isDoubleDeck: true,
					floorType: FloorType.HighFloor,
					vehicleCode: vehicleCode,
					busParameters: mission.BusParameter
				);

				
				AssertAxleDistribution(axle1: 37.5, axle2: 62.5, axle3: 0, axle4: 0, axleLoadDistribution: mission.AxleWeightDistribution);
				AssertVehicleEquipment(externalDisplays: 1, internalDisplays: 2, fridge: 1, kitchenStandard: 1, vehicleEquipment: mission.BusParameter.VehicleEquipment);
			}
		}

		[TestCase(2, VehicleCode.CB, "32f")]
		public void TestComplete2AxlesCompleteBus32F(AxleConfiguration axleConfig, VehicleCode vehicleCode, RegistrationClass registrationClass, int passengersLowerDeck, double bodyHeight,  string vehicleParameterGroup, int numberOfMissions)
		{
			var segment = DeclarationData.CompletedBusSegments.Lookup(axleConfig.NumAxles(), vehicleCode, registrationClass, passengersLowerDeck, bodyHeight.SI<Meter>());

			Assert.AreEqual(2, segment.Missions.Length);

			for (int i = 0; i < segment.Missions.Length; i++)
			{
				var mission = segment.Missions[i];

				Assert.AreEqual(_missionsTypes[i + 3], mission.MissionType);
				Assert.AreEqual(5.2, mission.DefaultCDxA.Value());
				Assert.AreEqual(true, mission.AirDragMeasurement);

				AssertBusParameters(
					isDoubleDeck: true,
					floorType: FloorType.HighFloor,
					vehicleCode: vehicleCode,
					busParameters: mission.BusParameter
				);

				AssertAxleDistribution(axle1: 37.5, axle2: 62.5, axle3: 0, axle4: 0, axleLoadDistribution: mission.AxleWeightDistribution);
				AssertVehicleEquipment(externalDisplays: 1, internalDisplays: 2, fridge: 1, kitchenStandard: 1, vehicleEquipment: mission.BusParameter.VehicleEquipment);
			}
		}

		#region Assert Methods

		private void AssertBusParameters(bool isDoubleDeck, FloorType floorType, VehicleCode vehicleCode, BusParameters busParameters)
		{
			Assert.AreEqual(isDoubleDeck, vehicleCode.IsDoubleDeckBus());
			

			Assert.AreEqual(floorType, busParameters.FloorType);
			

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


		private void AssertAxleDistribution(double axle1, double axle2, double axle3, double axle4,
			double[] axleLoadDistribution)
		{
			Assert.AreEqual(axle1, axleLoadDistribution[0]);
			Assert.AreEqual(axle2, axleLoadDistribution[1]);
			if (axle3 > 0) {
				Assert.AreEqual(axle3, axleLoadDistribution[2]);
			}
			if (axle4 > 0) {
				Assert.AreEqual(axle4, axleLoadDistribution[3]);
			}
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
