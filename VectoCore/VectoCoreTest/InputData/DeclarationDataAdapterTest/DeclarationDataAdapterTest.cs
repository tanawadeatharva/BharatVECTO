using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Moq;
using Ninject;
using NUnit.Framework;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.HeavyLorry;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents;
using TUGraz.VectoCore.Utils.Ninject;

namespace TUGraz.VectoCore.Tests.InputData.DeclarationDataAdapterTest
{
	[TestFixture]
	public class DeclarationDataAdapterTest
	{
		private IDeclarationDataAdapterFactory _declarationDataAdapterFactory;

		public void OneTimeSetup()
		{
			var kernel = new StandardKernel(new VectoNinjectModule());
			_declarationDataAdapterFactory = kernel.Get<IDeclarationDataAdapterFactory>();
		}
		#region LorriesSerialHybridVehicle

		



		#endregion

		[TestCase(VectoSimulationJobType.BatteryElectricVehicle,
			new []{ "Full electric steering gear" },
			"Vacuum pump + elec. driven",
			true,
			2,
			TestName="PEVAuxFailSteeredAxle")]
		[TestCase(VectoSimulationJobType.BatteryElectricVehicle,
			new[] { "Full electric steering gear" },
			"Vacuum pump + elec. driven",
			false,
			TestName = "PEVAuxPass")]
		[TestCase(VectoSimulationJobType.BatteryElectricVehicle,
			new[] { "Full electric steering gear" },
			"Medium Supply 1-stage + mech. clutch + AMS",
			true,
			TestName = "PEVAuxFailPS")]
		[TestCase(VectoSimulationJobType.SerialHybridVehicle,
			new[] { "Full electric steering gear" },
			"Vacuum pump + elec. driven",
			false,
			TestName = "S-HEV Aux Pass")]
		[TestCase(VectoSimulationJobType.ParallelHybridVehicle,
			new[] { "Full electric steering gear" },
			"Vacuum pump + elec. driven",
			false,
			TestName = "PEVAuxPass")]
		public void HeavyLorryPEVAuxiliaryDataAdapterFailTest(VectoSimulationJobType jobType, string[] spTechnologies, string psTechnology, bool fail, int? steeredAxles = null)
		{
			var dataAdapter = new HeavyLorryPEVAuxiliaryDataAdapter();
			
			var auxData = new Mock<IAuxiliariesDeclarationInputData>();

			Mock<IAuxiliaryDeclarationInputData> steeringSystem = new Mock<IAuxiliaryDeclarationInputData>();
			foreach(var spTechnoly in spTechnologies)
			{
				steeringSystem.SetType(AuxiliaryType.SteeringPump)
					.AddTechnology(spTechnoly);
			}
			
			var hvac = new Mock<IAuxiliaryDeclarationInputData>()
				.SetType(AuxiliaryType.HVAC)
				.AddTechnology("Default");
			var pneumatic = new Mock<IAuxiliaryDeclarationInputData>()
				.SetType(AuxiliaryType.PneumaticSystem)
				.AddTechnology(psTechnology);
			var elSystem = new Mock<IAuxiliaryDeclarationInputData>()
				.SetType(AuxiliaryType.ElectricSystem)
				.AddTechnology("Standard technology");

			


			auxData.AddAuxiliaries(steeringSystem.Object, hvac.Object, pneumatic.Object, elSystem.Object);
			try {
				dataAdapter.CreateAuxiliaryData(auxData.Object, null, MissionType.LongHaul, VehicleClass.Class12,
					4.SI<Meter>(), steeredAxles ?? 1, VectoSimulationJobType.BatteryElectricVehicle);
			} catch (Exception ex) {
				if (fail) {
					Assert.Pass($"Expected Exception {ex.Message}");
				} else {
					throw new Exception("Exception occured", ex);
				}
			}







		}
		

	}

	public static class AuxiliariesInputMockHelper
	{
		public static Mock<IAuxiliariesDeclarationInputData> AddAuxiliary(this Mock<IAuxiliariesDeclarationInputData> mock, IAuxiliaryDeclarationInputData aux)
		{
			var list = mock.Object?.Auxiliaries ?? new List<IAuxiliaryDeclarationInputData>();
			list.Add(aux);
			mock.Setup(aux => aux.Auxiliaries)
				.Returns(list);

			return mock;
		}

		public static Mock<IAuxiliariesDeclarationInputData> AddAuxiliaries(
			this Mock<IAuxiliariesDeclarationInputData> mock, params IAuxiliaryDeclarationInputData[] aux)
		{
			
			foreach (var auxiliary in aux) {
				mock = mock.AddAuxiliary(auxiliary);
			}


			return mock;
		}

		public static Mock<IAuxiliaryDeclarationInputData> SetType(this Mock<IAuxiliaryDeclarationInputData> mock, AuxiliaryType type)
		{
			mock.Setup(aux => aux.Type).Returns(type);
			return mock;
		}

		public static Mock<IAuxiliaryDeclarationInputData> AddTechnology(this Mock<IAuxiliaryDeclarationInputData> mock, string technology)
		{
			var list = mock.Object.Technology ?? new List<string>();
			list.Add(technology);
			mock.Setup(aux => aux.Technology).Returns(list);
			return mock;
		}
	}

}