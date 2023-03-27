using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Runtime.InteropServices.ComTypes;
using System.Security.RightsManagement;
using System.Xml;
using System.Xml.Linq;
using Moq;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.SimulationComponent;
using TUGraz.VectoCore.Utils;

namespace Vecto3GUI2020Test;

public static class InputMock
{
	public static IVehicleDeclarationInputData GetMockVehicle(out Mock<IVehicleDeclarationInputData> mock, bool createAirdrag = false)
	{
		mock = new Mock<IVehicleDeclarationInputData>();


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
		mock.SetupGet(v => v.Components).Returns(() => {
				var exp =
					new List<Tuple<Expression<Func<IVehicleComponentsDeclaration, IAirdragDeclarationInputData>>,
						IAirdragDeclarationInputData>>();
				if (createAirdrag) {
					exp.Add(new Tuple<Expression<Func<IVehicleComponentsDeclaration, IAirdragDeclarationInputData>>, IAirdragDeclarationInputData>(
						e => e.AirdragInputData,
						CreateAirdragComponentData(out _)));
				}


				return GetMockComponent(out _, exp.ToArray());
			}				
			);

		var obj = mock.Object;
		


		return mock.Object;
	}

	public static IVehicleDeclarationInputData GetMockVehicle()
	{
		return GetMockVehicle(out _, false);
	}

	public static IVehicleDeclarationInputData GetMockVehicle(bool createAirdrag)
	{
		return GetMockVehicle(out _, createAirdrag);
	}

	public static IAirdragDeclarationInputData CreateAirdragComponentData(out Mock<IAirdragDeclarationInputData> mock)
	{
		mock = new Mock<IAirdragDeclarationInputData>();
		mock.SetupGet(a => a.Manufacturer).Returns("Manufacturer");
		mock.SetupGet(a => a.Model).Returns("Model");
		mock.SetupGet(a => a.Date).Returns(DateTime.Today);
		mock.SetupGet(a => a.AppVersion).Returns("APPVERSION");
		mock.SetupGet(a => a.SavedInDeclarationMode).Returns(true);
        mock.SetupGet(a => a.AirDragArea).Returns(6.66.SI<SquareMeter>());
		mock.SetupGet(a => a.AirDragArea_0).Returns(7.77.SI<SquareMeter>());
		mock.SetupGet(a => a.TransferredAirDragArea).Returns(8.88.SI<SquareMeter>());
		return mock.Object;
	}

	public static IDeclarationJobInputData GetDeclarationJobInputData(out Mock<IDeclarationJobInputData> mock, VectoSimulationJobType jobType)
	{
		mock = new Mock<IDeclarationJobInputData>();
		mock.SetupGet(a => a.JobType).Returns(jobType);
		









		return mock.Object;
	}

	public static IDeclarationInputDataProvider GetDeclarationInputDataProvider(
		out Mock<IDeclarationInputDataProvider> mock, VectoSimulationJobType jobType, XNamespace nameSpace)
	{
		mock = new Mock<IDeclarationInputDataProvider>();
		mock.SetupGet(d => d.JobInputData).Returns(GetDeclarationJobInputData(out _, jobType));
		mock.SetupGet(d => d.PrimaryVehicleData).Returns(GetPrimaryVehicleData(out _, jobType));
		mock.SetupGet(d => d.DataSource).Returns(new DataSource() {
			SourceFile = "Mock",
			SourceType = DataSourceType.XMLFile,
			SourceVersion = nameSpace.GetVersionFromNamespaceUri(),
		});






		return mock.Object;
	}

	private static IPrimaryVehicleInformationInputDataProvider GetPrimaryVehicleData(out Mock<IPrimaryVehicleInformationInputDataProvider> mock, VectoSimulationJobType jobType)
	{
		mock = new Mock<IPrimaryVehicleInformationInputDataProvider>();





		return mock.Object;
	}




	public static IVehicleComponentsDeclaration GetMockComponent(out Mock<IVehicleComponentsDeclaration> mock,
		params Tuple<Expression<Func<IVehicleComponentsDeclaration, IAirdragDeclarationInputData>>, IAirdragDeclarationInputData>[] expressions)
	{
		mock = new Mock<IVehicleComponentsDeclaration>();
		foreach (var expression in expressions) {
			mock.Setup(expression.Item1).Returns(expression.Item2);
		}
		return mock.Object;
	}
	
	// Syntax GetMockVehicle().AddAirdragComponent().SetAirdragVersion(). and so on ...
	public static IVehicleDeclarationInputData AddAirdragComponent(this IVehicleDeclarationInputData mocked)
	{
		throw new NotFiniteNumberException();
		
		return mocked;
	}
	public static IVehicleDeclarationInputData SetAirdragVersion(this IVehicleDeclarationInputData mocked, XNamespace version)
	{
		if (mocked.Components.AirdragInputData == null) {
			throw new VectoException("Airdrag not mocked");
		};

		var airdrag = Mock.Get(mocked.Components.AirdragInputData);
		airdrag.SetupGet((a) => a.DataSource).Returns(new DataSource() {
			SourceFile = "mocked",
			SourceType = DataSourceType.XMLFile,
			Type = XMLNames.AirDrag_Data_Type_Attr,
			SourceVersion = version.GetVersionFromNamespaceUri(),
		});


		return mocked;
	}
}