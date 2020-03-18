using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Ninject;
using Ninject.Planning.Bindings.Resolvers;
using NUnit.Framework;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.OutputData.FileIO;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Tests.Integration.CompletedBus
{

	[TestFixture()]
	public class CompletedBusFactorMethodTest
	{
		class RelatedRun
		{
			public VectoRunData VectoRunDataSpezificBody { get; set; }
			public VectoRunData VectoRunDataGenericBody { get; set; }

		}

		private List<RelatedRun> relatedRuns;
		private Segment primarySegment;
		private Segment completedSegment;


		[OneTimeSetUp]
		public void RunBeforeAnyTests()
		{
			Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);

			relatedRuns = new List<RelatedRun>();

			//SetBusSegments();
		}


		private void SetBusSegments()
		{
			PrimaryBusSegment();
			CompletedBusSegment();
		}
		
		private void PrimaryBusSegment()
		{
			var category = VehicleCategory.HeavyBusPrimaryVehicle;
			var axleConfiguration = AxleConfiguration.AxleConfig_4x2;
			var floorType = FloorType.HighFloor;
			var articulated = false;
			var doubleDecker = true;
			
			primarySegment = DeclarationData.PrimaryBusSegments.Lookup(category, axleConfiguration, articulated, floorType, doubleDecker);
		}

		private void CompletedBusSegment()
		{
			var numberOfAxles = 2;
			var vehicleCode = VehicleCode.CB;
			var registrationClass = RegistrationClass.II_III;
			var passengersLowerDeck = 30;
			var bodyHeight = 3.SI<Meter>();
			var lowEntry = false;

			completedSegment = DeclarationData.CompletedBusSegments.Lookup(numberOfAxles, vehicleCode, registrationClass, passengersLowerDeck, bodyHeight, lowEntry);
		}

		[TestCase()]
		public void TestCompletedBus()
		{
			var relativeJobPath = @"TestData\Integration\Buses\FactorMethod\CompletedBus.vecto";

			var writer = new FileOutputWriter(Path.Combine(Path.GetDirectoryName(relativeJobPath), Path.GetFileName(relativeJobPath)));
			var inputData = JSONInputDataFactory.ReadJsonJob(relativeJobPath);

			var factory = new SimulatorFactory(ExecutionMode.Declaration, inputData, writer) {
				WriteModalResults = true,
				//ActualModalData = true,
				Validate = false
			};
			//var sumContainer = new SummaryDataContainer(writer);
			//var jobContainer = new JobContainer(sumContainer);


			var runs = factory.DataReader.NextRun().ToList();
			Assert.IsTrue(runs.Count == 8 || runs.Count == 12);

			SetRelatedVehicleParts(runs);

			var index = 0;
			for (int i = 0; i < relatedRuns.Count; i++) {
				AssertVehicleData(relatedRuns[i], ref index);
				AssertAirdragData(relatedRuns[i]);
				//AssertEngineData(relatedRuns[i]);
			}
			
		}

		#region Vehicle Data Asserts

		private void AssertVehicleData(RelatedRun relatedRun, ref int index)
		{
			var genericVehicleData = relatedRun.VectoRunDataGenericBody.VehicleData;
			var specificVehicleData = relatedRun.VectoRunDataSpezificBody.VehicleData;

			Assert.AreEqual(AxleConfiguration.AxleConfig_4x2, genericVehicleData.AxleConfiguration);
			Assert.AreEqual(genericVehicleData.AxleConfiguration, specificVehicleData.AxleConfiguration);

			Assert.AreEqual(10000, genericVehicleData.CurbMass.Value());
			Assert.AreEqual(8300, specificVehicleData.CurbMass.Value());

			Assert.AreEqual(0, genericVehicleData.BodyAndTrailerMass.Value());
			Assert.AreEqual(genericVehicleData.BodyAndTrailerMass, specificVehicleData.BodyAndTrailerMass);
			
			AssertLoading(genericVehicleData.Loading, specificVehicleData.Loading, ref index);

			Assert.AreEqual(0.4992, genericVehicleData.DynamicTyreRadius.Value(), 1e-0);
			Assert.AreEqual(genericVehicleData.DynamicTyreRadius, specificVehicleData.DynamicTyreRadius);

			AssertADASData(genericVehicleData.ADAS, specificVehicleData.ADAS);

			AssertAxles(genericVehicleData.AxleData, specificVehicleData.AxleData);
		}

		private void AssertLoading(Kilogram genericLoading, Kilogram specificLoading, ref int index)
		{
			switch (index) {
				case 0:
					Assert.AreEqual(5051.2950, genericLoading.Value(), 1e-0);
					Assert.AreEqual(2309.4738, specificLoading.Value(), 1e-0);//lowLoading
					break;
				case 1:
					Assert.AreEqual(5051.2950, genericLoading.Value(),1e-0);
					Assert.AreEqual(2130, specificLoading.Value(), 1e-0);
					break;
				case 2:
					Assert.AreEqual(3367.53, genericLoading.Value(), 1e-0);
					Assert.AreEqual(1539.6492, specificLoading.Value(), 1e-0);
					break;
				case 3:
					Assert.AreEqual(3367.53, genericLoading.Value(), 1e-0);
					Assert.AreEqual(2130.0, specificLoading.Value(), 1e-0);
					break;
			}
			index++;
		}

		private void AssertADASData(VehicleData.ADASData genericAdasData, VehicleData.ADASData specificAdasData)
		{
			Assert.AreEqual(false, genericAdasData.EngineStopStart);
			Assert.AreEqual(genericAdasData.EngineStopStart, specificAdasData.EngineStopStart);
			Assert.AreEqual(EcoRollType.None, genericAdasData.EcoRoll);
			Assert.AreEqual(genericAdasData.EcoRoll, specificAdasData.EcoRoll);
			Assert.AreEqual(PredictiveCruiseControlType.None, genericAdasData.PredictiveCruiseControl);
			Assert.AreEqual(genericAdasData.PredictiveCruiseControl, specificAdasData.PredictiveCruiseControl);
		}

		private void AssertAxles(List<Axle> genericAxles, List<Axle> specificAxles)
		{
			Assert.AreEqual(2, genericAxles.Count);
			Assert.AreEqual(genericAxles.Count, specificAxles.Count);

			for (int i = 0; i < genericAxles.Count; i++)
			{
				Assert.AreEqual("315/70 R22.5", genericAxles[i].WheelsDimension);
				Assert.AreEqual(genericAxles[i].WheelsDimension, specificAxles[i].WheelsDimension);

				Assert.AreEqual(14.9, genericAxles[i].Inertia.Value());
				Assert.AreEqual(genericAxles[i].Inertia, specificAxles[i].Inertia);

				Assert.AreEqual(31300, genericAxles[i].TyreTestLoad.Value());
				Assert.AreEqual(genericAxles[i].TyreTestLoad, specificAxles[i].TyreTestLoad);
				if (i == 0)
				{
					Assert.AreEqual(0.375, genericAxles[i].AxleWeightShare);
					Assert.AreEqual(genericAxles[i].AxleWeightShare, specificAxles[i].AxleWeightShare);
					Assert.AreEqual(AxleType.VehicleNonDriven, genericAxles[i].AxleType);
					Assert.AreEqual(false, genericAxles[i].TwinTyres);
				}
				else if (i == 1)
				{
					Assert.AreEqual(0.625, genericAxles[i].AxleWeightShare);
					Assert.AreEqual(genericAxles[i].AxleWeightShare, specificAxles[i].AxleWeightShare);
					Assert.AreEqual(AxleType.VehicleDriven, genericAxles[i].AxleType);
					Assert.AreEqual(true, genericAxles[i].TwinTyres);
				}
				Assert.AreEqual(genericAxles[i].TwinTyres, specificAxles[i].TwinTyres);
				Assert.AreEqual(genericAxles[i].AxleType, specificAxles[i].AxleType);
			}
		}
		#endregion

		#region Airdrag Data Asserts

		private void AssertAirdragData(RelatedRun relatedRun)
		{
			var genericAirdragData = relatedRun.VectoRunDataGenericBody.AirdragData;
			var specificAirdragData = relatedRun.VectoRunDataSpezificBody.AirdragData;
			
			var genericDragArea = 5.2.SI<SquareMeter>();
			var specificDragArea = 6.34.SI<SquareMeter>();

			var genericVehicleHeight = 3.7.SI<Meter>();
			var specificVehicleHeight = 3.0.SI<Meter>() + 0.30.SI<Meter>();

			var genericCrosswind = GetCrosswindCorrection("CoachBus", genericDragArea, genericVehicleHeight);
			var specificCrosswind = GetCrosswindCorrection("CoachBus", specificDragArea, specificVehicleHeight);
			
			var genericValueExpected = genericCrosswind.AverageAirDragPowerLoss(20.KMPHtoMeterPerSecond(),
				21.KMPHtoMeterPerSecond(), Physics.AirDensity).Value();

			var currentGenericValue = genericAirdragData.CrossWindCorrectionCurve.AverageAirDragPowerLoss(
				20.KMPHtoMeterPerSecond(),21.KMPHtoMeterPerSecond(), Physics.AirDensity).Value();

			var expectedSpecificValue = specificCrosswind.AverageAirDragPowerLoss(21.KMPHtoMeterPerSecond(),
				22.KMPHtoMeterPerSecond(), Physics.AirDensity).Value();

			var currentSpecificValue = specificAirdragData.CrossWindCorrectionCurve.AverageAirDragPowerLoss(21.KMPHtoMeterPerSecond(),
				22.KMPHtoMeterPerSecond(), Physics.AirDensity).Value();


			Assert.AreEqual(CrossWindCorrectionMode.DeclarationModeCorrection, genericAirdragData.CrossWindCorrectionMode);
			Assert.AreEqual(genericAirdragData.CrossWindCorrectionMode, specificAirdragData.CrossWindCorrectionMode);

			Assert.AreEqual(genericValueExpected, currentGenericValue);
			Assert.AreEqual(expectedSpecificValue, currentSpecificValue);
			
			Assert.AreEqual(genericDragArea,  genericAirdragData.DeclaredAirdragArea);
			Assert.AreEqual(genericDragArea, genericAirdragData.CrossWindCorrectionCurve.AirDragArea);
			Assert.AreEqual(specificDragArea,  specificAirdragData.DeclaredAirdragArea);
			Assert.AreEqual(specificDragArea,  specificAirdragData.CrossWindCorrectionCurve.AirDragArea);
		}


		#endregion

		#region Engine Data Asserts

		private void AssertEngineData(RelatedRun relatedRun)
		{
			var genericEngine = relatedRun.VectoRunDataGenericBody.EngineData;
			var specificEngine = relatedRun.VectoRunDataSpezificBody.EngineData;

			

		}



		#endregion


		private CrosswindCorrectionCdxALookup GetCrosswindCorrection(string crossWindCorrectionParams,
			SquareMeter aerodynamicDragArea, Meter vehicleHeight)
		{
		  return new CrosswindCorrectionCdxALookup(
				aerodynamicDragArea,
				DeclarationDataAdapterHeavyLorry.GetDeclarationAirResistanceCurve(
					crossWindCorrectionParams,
					aerodynamicDragArea,
					vehicleHeight),
				CrossWindCorrectionMode.DeclarationModeCorrection);
		}



		private void SetRelatedVehicleParts(List<VectoRunData> runs)
		{
			for (int i = 0; i < runs.Count; i++) {
				var relatedRun = new RelatedRun {
					VectoRunDataSpezificBody = runs[i],
					VectoRunDataGenericBody = runs[i+1]
				};
				relatedRuns.Add(relatedRun);
				i++;
			}
		}
	}
}
