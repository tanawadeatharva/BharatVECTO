using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Moq;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Utils;

namespace Vecto3GUI2020Test.MockInput;

internal static class MockVehicle
{
	public static IVehicleDeclarationInputData GetMockVehicle(string type, XNamespace version)
	{
		var mock = new Mock<IVehicleDeclarationInputData>();


		mock.SetupGet(v => v.Identifier).Returns("VEH-1234567890");
		mock.SetupGet(v => v.Manufacturer).Returns("Some Manufacturer");
		mock.SetupGet(v => v.ManufacturerAddress).Returns("Baker Street 221b");
		mock.SetupGet(v => v.VIN).Returns("VEH-1234567890");
		mock.SetupGet(v => v.Date).Returns(DateTime.Today);
		mock.SetupGet(v => v.Model).Returns("Fillmore");
		mock.SetupGet(v => v.LegislativeClass).Returns(LegislativeClass.M3);
		mock.SetupGet(v => v.CurbMassChassis).Returns(12000.SI<Kilogram>());
		mock.SetupGet(v => v.GrossVehicleMassRating).Returns(15000.SI<Kilogram>());
		mock.SetupGet(v => v.AirdragModifiedMultistep).Returns(true);
		mock.SetupGet(v => v.TankSystem).Returns(TankSystem.Compressed);
		mock.SetupGet(v => v.RegisteredClass).Returns(RegistrationClass.II_III);
		mock.SetupGet(v => v.NumberPassengerSeatsLowerDeck).Returns(1);
		mock.SetupGet(v => v.NumberPassengersStandingLowerDeck).Returns(10);
		mock.SetupGet(v => v.NumberPassengerSeatsUpperDeck).Returns(11);
		mock.SetupGet(v => v.NumberPassengersStandingUpperDeck).Returns(2);
		mock.SetupGet(v => v.VehicleCode).Returns(VehicleCode.CB);
		mock.SetupGet(v => v.LowEntry).Returns(false);
		mock.SetupGet(v => v.Height).Returns(2.5.SI<Meter>());
		mock.SetupGet(v => v.Length).Returns(9.5.SI<Meter>());
		mock.SetupGet(v => v.Width).Returns(2.5.SI<Meter>());
		mock.SetupGet(v => v.EntranceHeight).Returns(150E-3.SI<Meter>());
		mock.SetupGet(v => v.DoorDriveTechnology).Returns(ConsumerTechnology.Electrically);
		mock.SetupGet(v => v.VehicleDeclarationType).Returns(VehicleDeclarationType.interim);
		mock.SetupGet(v => v.VehicleTypeApprovalNumber).Returns("1234567890");
		mock.SetupGet(v => v.Components).Returns((IVehicleComponentsDeclaration)GetComponentsMock());

		var obj = mock.Object;
		mock.SetupGet(v => v.DataSource).Returns(MockInput.GetMockDataSource(
			type,
			version));


		return mock.Object;
	}

	public static IVehicleDeclarationInputData GetPrimaryVehicle(string type)
	{
		var mock = new Mock<IVehicleDeclarationInputData>();
		mock.SetupGet(v => v.DataSource)
			.Returns(MockInput.GetMockDataSource(type,
				XMLDefinitions.DECLARATION_MULTISTAGE_BUS_VEHICLE_NAMESPACE_VO1));





		return mock.Object;
	}

	private static IVehicleComponentsDeclaration GetComponentsMock()
	{
		var mock = new Mock<IVehicleComponentsDeclaration>();
		return mock.Object;
	}

	public static IPrimaryVehicleInformationInputDataProvider GetPrimaryVehicleInformation(string xsdType)
	{
		var mock = new Mock<IPrimaryVehicleInformationInputDataProvider>();
		mock.SetupGet(m => m.Vehicle)
			.Returns(MockVehicle.GetPrimaryVehicle(
				xsdType));




		return mock.Object;
	}

	public static IList<IManufacturingStageInputData> GetManufacturingStages(int nrOfStages, string type, XNamespace version)
	{
		var result = new List<IManufacturingStageInputData>(nrOfStages);
		for (var i = 2; i <= nrOfStages + 1; i++) {
			result.Add(GetManufacturingStage(type, version, i));
		}


		return result;

	}

	public static IManufacturingStageInputData GetManufacturingStage(string type, XNamespace version, int stepCount)
	{
		var mock = new Mock<IManufacturingStageInputData>();
		mock.SetupGet(m => m.Vehicle).Returns(GetMockVehicle(type, version));
		mock.SetupGet(m => m.StepCount).Returns(stepCount);


		return mock.Object;
	}


}