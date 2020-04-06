using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Ninject;
using Ninject.Planning.Bindings.Resolvers;
using NUnit.Framework;
using TUGraz.VECTO;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.OutputData.FileIO;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter;
using TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.HVAC;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Engine;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Tests.Integration.CompletedBus
{

	[TestFixture()]
	public class CompletedBusFactorMethodTest
	{
		const string JobFile = @"TestData\Integration\Buses\FactorMethod\CompletedBus.vecto";

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
			var writer = new FileOutputWriter(Path.Combine(Path.GetDirectoryName(JobFile), Path.GetFileName(JobFile)));
			var inputData = JSONInputDataFactory.ReadJsonJob(JobFile);

			var factory = new SimulatorFactory(ExecutionMode.Declaration, inputData, writer)
			{
				WriteModalResults = true,
				//ActualModalData = true,
				Validate = false
			};
			//var sumContainer = new SummaryDataContainer(writer);
			//var jobContainer = new JobContainer(sumContainer);


			var runs = factory.DataReader.NextRun().ToList();
			Assert.IsTrue(runs.Count == 8 || runs.Count == 12);

			SetRelatedVehicleParts(runs);

			for (int i = 0; i < relatedRuns.Count; i++)
			{
				AssertVehicleData(relatedRuns[i],i);
				AssertAirdragData(relatedRuns[i]);
				AssertEngineData(relatedRuns[i]);
				AssertGearbox(relatedRuns[i]);
				AssertTorqueConverter(relatedRuns[i]);
				AssertAxlegearData(relatedRuns[i]);
				AssertAngledriveData(relatedRuns[i]);
				AssertAngledriveData(relatedRuns[i]);
				AssertAuxiliaryData(relatedRuns[i]);
				AssertElectricalUserInputConfig(relatedRuns[i]);
				AssertPneumaticUserInputsConfig(relatedRuns[i]);
				AssertPneumaticConsumerDemand(relatedRuns[i]);
				AssertSSMBusParameters(relatedRuns[i], i);
				AssertTechnologyBenefits(relatedRuns[i]);
				AssertBoundaryConditions(relatedRuns[i]);
				AssertEnvironmentalConditions(relatedRuns[i]);
				AssertSSMInputs(relatedRuns[i], i);
				AssertRetarder(relatedRuns[i]);
				AssertDriverData(relatedRuns[i]);
			}

		}

		#region Vehicle Data Asserts

		private void AssertVehicleData(RelatedRun relatedRun, int currentIndex)
		{
			var genericVehicleData = relatedRun.VectoRunDataGenericBody.VehicleData;
			var specificVehicleData = relatedRun.VectoRunDataSpezificBody.VehicleData;

			Assert.AreEqual(AxleConfiguration.AxleConfig_4x2, genericVehicleData.AxleConfiguration);
			Assert.AreEqual(genericVehicleData.AxleConfiguration, specificVehicleData.AxleConfiguration);

			Assert.AreEqual(10000, genericVehicleData.CurbMass.Value());
			Assert.AreEqual(8300, specificVehicleData.CurbMass.Value());

			Assert.AreEqual(0, genericVehicleData.BodyAndTrailerMass.Value());
			Assert.AreEqual(genericVehicleData.BodyAndTrailerMass, specificVehicleData.BodyAndTrailerMass);

			AssertLoading(genericVehicleData.Loading, specificVehicleData.Loading, currentIndex);

			Assert.AreEqual(0.4992, genericVehicleData.DynamicTyreRadius.Value(), 1e-0);
			Assert.AreEqual(genericVehicleData.DynamicTyreRadius, specificVehicleData.DynamicTyreRadius);

			AssertADASData(genericVehicleData.ADAS, specificVehicleData.ADAS);

			AssertAxles(genericVehicleData.AxleData, specificVehicleData.AxleData);
		}

		private void AssertLoading(Kilogram genericLoading, Kilogram specificLoading, int index)
		{
			switch (index)
			{
				case 0:
					Assert.AreEqual(5051.2950, genericLoading.Value(), 1e-4);
					Assert.AreEqual(2309.4738, specificLoading.Value(), 1e-4);
					break;
				case 1:
					Assert.AreEqual(5051.2950, genericLoading.Value(), 1e-4);
					Assert.AreEqual(2130, specificLoading.Value(), 1e-0);
					break;
				case 2:
					Assert.AreEqual(3367.53, genericLoading.Value(), 1e-2);
					Assert.AreEqual(1539.6492, specificLoading.Value(), 1e-4);
					break;
				case 3:
					Assert.AreEqual(3367.53, genericLoading.Value(), 1e-2);
					Assert.AreEqual(2130.0, specificLoading.Value(), 1e-0);
					break;
			}
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
				20.KMPHtoMeterPerSecond(), 21.KMPHtoMeterPerSecond(), Physics.AirDensity).Value();

			var expectedSpecificValue = specificCrosswind.AverageAirDragPowerLoss(21.KMPHtoMeterPerSecond(),
				22.KMPHtoMeterPerSecond(), Physics.AirDensity).Value();

			var currentSpecificValue = specificAirdragData.CrossWindCorrectionCurve.AverageAirDragPowerLoss(21.KMPHtoMeterPerSecond(),
				22.KMPHtoMeterPerSecond(), Physics.AirDensity).Value();


			Assert.AreEqual(CrossWindCorrectionMode.DeclarationModeCorrection, genericAirdragData.CrossWindCorrectionMode);
			Assert.AreEqual(genericAirdragData.CrossWindCorrectionMode, specificAirdragData.CrossWindCorrectionMode);

			Assert.AreEqual(genericValueExpected, currentGenericValue);
			Assert.AreEqual(expectedSpecificValue, currentSpecificValue);

			Assert.AreEqual(genericDragArea, genericAirdragData.DeclaredAirdragArea);
			Assert.AreEqual(genericDragArea, genericAirdragData.CrossWindCorrectionCurve.AirDragArea);
			Assert.AreEqual(specificDragArea, specificAirdragData.DeclaredAirdragArea);
			Assert.AreEqual(specificDragArea, specificAirdragData.CrossWindCorrectionCurve.AirDragArea);
		}


		#endregion

		#region Engine Data Asserts

		private void AssertEngineData(RelatedRun relatedRun)
		{
			var genericEngine = relatedRun.VectoRunDataGenericBody.EngineData;
			var specificEngine = relatedRun.VectoRunDataSpezificBody.EngineData;

			Assert.AreEqual(7, genericEngine.FullLoadCurves.Count);
			AssertFullLoadAndDragCurve(genericEngine.FullLoadCurves[0].FullLoadEntries);
			AssertFullLoadAndDragCurve(specificEngine.FullLoadCurves[0].FullLoadEntries);

			Assert.AreEqual(700 * Constants.RPMToRad, genericEngine.IdleSpeed.Value(), 1e-9);
			Assert.AreEqual(genericEngine.IdleSpeed, specificEngine.IdleSpeed);

			Assert.AreEqual(8.SI<CubicMeter>().Value() * 1E-6, genericEngine.Displacement.Value());
			Assert.AreEqual(genericEngine.Displacement, specificEngine.Displacement);

			Assert.AreEqual(
				DeclarationData.Engine.TorqueConverterInertia +
				DeclarationData.Engine.EngineBaseInertia +
				DeclarationData.Engine.EngineDisplacementInertia * genericEngine.Displacement, genericEngine.Inertia);
			Assert.AreEqual(genericEngine.Inertia, specificEngine.Inertia);

			Assert.AreEqual(WHRType.None, genericEngine.WHRType);
			Assert.AreEqual(genericEngine.WHRType, specificEngine.WHRType);

			Assert.AreEqual(null, genericEngine.ElectricalWHR);
			Assert.AreEqual(genericEngine.ElectricalWHR, specificEngine.ElectricalWHR);

			Assert.AreEqual(null, genericEngine.MechanicalWHR);
			Assert.AreEqual(genericEngine.MechanicalWHR, specificEngine.MechanicalWHR);

			Assert.AreEqual(1, genericEngine.Fuels.Count);
			Assert.AreEqual(genericEngine.Fuels.Count, specificEngine.Fuels.Count);

			AssertFuel(genericEngine.Fuels[0]);
			AssertFuel(specificEngine.Fuels[0]);
		}


		private void AssertFullLoadAndDragCurve(List<EngineFullLoadCurve.FullLoadCurveEntry> entries)
		{
			Assert.AreEqual(12, entries.Count);
			AssertFullLoadAndDragCurveEntry(600.00, 546.02, -39.66, entries[0]);
			AssertFullLoadAndDragCurveEntry(800.00, 760.78, -48.83, entries[1]);
			AssertFullLoadAndDragCurveEntry(1000.00, 973.29, -56.44, entries[2]);
			AssertFullLoadAndDragCurveEntry(1200.00, 1092.03, -67.29, entries[3]);
			AssertFullLoadAndDragCurveEntry(1400.00, 1092.03, -77.58, entries[4]);
			AssertFullLoadAndDragCurveEntry(1600.00, 1092.03, -87.88, entries[5]);
			AssertFullLoadAndDragCurveEntry(1800.00, 1022.52, -94.11, entries[6]);
			AssertFullLoadAndDragCurveEntry(2000.00, 944.17, -100.76, entries[7]);
			AssertFullLoadAndDragCurveEntry(2200.00, 868.12, -113.36, entries[8]);
			AssertFullLoadAndDragCurveEntry(2400.00, 741.99, -122.60, entries[9]);
			AssertFullLoadAndDragCurveEntry(2500.00, 647.29, -126.66, entries[10]);
			AssertFullLoadAndDragCurveEntry(2600.00, 0.00, -132.07, entries[11]);
		}

		private void AssertFullLoadAndDragCurveEntry(double engineSpeed, double maxTorque, double dragTorque,
			EngineFullLoadCurve.FullLoadCurveEntry entry)
		{
			Assert.AreEqual(engineSpeed * Constants.RPMToRad, entry.EngineSpeed.Value(), 1e-9);
			Assert.AreEqual(maxTorque.SI<NewtonMeter>(), entry.TorqueFullLoad);
			Assert.AreEqual(dragTorque.SI<NewtonMeter>(), entry.TorqueDrag);
		}

		private void AssertFuel(CombustionEngineFuelData fuel)
		{
			Assert.AreEqual(1, fuel.WHTCMotorway);
			Assert.AreEqual(1, fuel.WHTCRural);
			Assert.AreEqual(1, fuel.WHTCUrban);
			Assert.AreEqual(1, fuel.ColdHotCorrectionFactor);
			Assert.AreEqual(1, fuel.CorrectionFactorRegPer);

			//fuel.ConsumptionMap ???
		}

		#endregion

		#region Gearbox Asserts

		private void AssertGearbox(RelatedRun relatedRun)
		{
			var genericGearbox = relatedRun.VectoRunDataGenericBody.GearboxData;
			var specificGearbox = relatedRun.VectoRunDataSpezificBody.GearboxData;

			Assert.AreEqual(0, genericGearbox.Inertia.Value());
			Assert.AreEqual(genericGearbox.Inertia, specificGearbox.Inertia);

			Assert.AreEqual(0.0.SI<Second>(), genericGearbox.TractionInterruption);
			Assert.AreEqual(specificGearbox.TractionInterruption, genericGearbox.TractionInterruption);

			Assert.AreEqual(6, genericGearbox.Gears.Count);
			Assert.AreEqual(genericGearbox.Gears.Count, specificGearbox.Gears.Count);

			AssertGearShiftParameters(relatedRun);
			AssertGears(genericGearbox.Gears.Values.ToList());
			AssertGears(specificGearbox.Gears.Values.ToList());
			AssertGearsLossmap(
				genericGearbox.Gears.Values.ToList(), specificGearbox.Gears.Values.ToList());
		}

		private void AssertGears(IList<GearData> gears)
		{
			AssertGear(3.364, 1900.SI<NewtonMeter>(), 262, gears[0]);
			AssertGear(1.909, 1900.SI<NewtonMeter>(), 262, gears[1]);
			AssertGear(1.421, null, 262, gears[2]);
			AssertGear(1.000, null, 262, gears[3]);
			AssertGear(0.720, null, 262, gears[4]);
			AssertGear(0.615, null, 262, gears[5]);
		}

		private void AssertGear(double ratio, NewtonMeter maxTorque, double maxSpeed, GearData gear)
		{
			Assert.AreEqual(ratio, gear.Ratio);
			Assert.AreEqual(maxTorque, gear.MaxTorque);
			Assert.AreEqual(maxSpeed, gear.MaxSpeed.AsRPM);
		}

		private void AssertGearsLossmap(IList<GearData> genericGearData, IList<GearData> specificGearData)
		{
			Assert.AreEqual(6, genericGearData.Count);
			Assert.AreEqual(genericGearData.Count, specificGearData.Count);
			for (int i = 0; i < genericGearData.Count; i++) {
				Assert.IsNotNull(genericGearData[i].LossMap);
				Assert.AreEqual(genericGearData[i].LossMap, specificGearData[i].LossMap);
			}
		}

		private void AssertGearShiftParameters(RelatedRun relatedRun)
		{
			Assert.AreEqual(relatedRun.VectoRunDataGenericBody.ShiftStrategy, relatedRun.VectoRunDataSpezificBody.ShiftStrategy);
		}


		#endregion

		#region Torque Converter Asserts

		private void AssertTorqueConverter(RelatedRun relatedRun)
		{
			var genericTorqueConverterData= relatedRun.VectoRunDataGenericBody.GearboxData.TorqueConverterData;
			var specificTorqueConverterData = relatedRun.VectoRunDataSpezificBody.GearboxData.TorqueConverterData;

			Assert.AreEqual(1000.RPMtoRad(), genericTorqueConverterData.ReferenceSpeed);
			Assert.AreEqual(genericTorqueConverterData.ReferenceSpeed, specificTorqueConverterData.ReferenceSpeed);

			Assert.AreEqual(DeclarationData.TorqueConverter.MaxInputSpeed, genericTorqueConverterData.TorqueConverterSpeedLimit);
			Assert.AreEqual(genericTorqueConverterData.TorqueConverterSpeedLimit, specificTorqueConverterData.TorqueConverterSpeedLimit);

			Assert.AreEqual(0.1.SI<MeterPerSquareSecond>(), genericTorqueConverterData.CCUpshiftMinAcceleration);
			Assert.AreEqual(0.1.SI<MeterPerSquareSecond>(), genericTorqueConverterData.CLUpshiftMinAcceleration);

			Assert.AreEqual(genericTorqueConverterData.CCUpshiftMinAcceleration, specificTorqueConverterData.CCUpshiftMinAcceleration);
			Assert.AreEqual(genericTorqueConverterData.CLUpshiftMinAcceleration, specificTorqueConverterData.CLUpshiftMinAcceleration);

			Assert.AreEqual(genericTorqueConverterData.TorqueConverterEntries, specificTorqueConverterData.TorqueConverterEntries);
		}


		#endregion

		#region Axlegear Data Asserts

		private void AssertAxlegearData(RelatedRun relatedRun)
		{
			var genericAxlegearData = relatedRun.VectoRunDataGenericBody.AxleGearData;
			var specificAxlegearData = relatedRun.VectoRunDataSpezificBody.AxleGearData;

			Assert.AreEqual(6.500, genericAxlegearData.AxleGear.Ratio);
			Assert.AreEqual(genericAxlegearData.AxleGear.Ratio, specificAxlegearData.AxleGear.Ratio);

			Assert.AreEqual(AxleLineType.SinglePortalAxle, genericAxlegearData.LineType);
			Assert.AreEqual(genericAxlegearData.LineType, specificAxlegearData.LineType);

			Assert.IsNotNull(genericAxlegearData.AxleGear.LossMap);
			AssertAxlegearLossMap(genericAxlegearData.AxleGear.LossMap);
			Assert.AreEqual(genericAxlegearData.AxleGear.LossMap, specificAxlegearData.AxleGear.LossMap);
		}

		private void AssertAxlegearLossMap(TransmissionLossMap lossMap)
		{
			Assert.AreEqual(12, lossMap._entries.Count);

			AssertLossmapEntry(0, -1491.6797, 46.7818, lossMap._entries[0]);
			AssertLossmapEntry(0, -22.2920, 16.1695, lossMap._entries[1]);
			AssertLossmapEntry(0, 54.6311, 16.1695, lossMap._entries[2]);
			AssertLossmapEntry(0, 1585.2433, 46.7818, lossMap._entries[3]);
			AssertLossmapEntry(325, -1491.6797, 46.7818, lossMap._entries[4]);
			AssertLossmapEntry(325, -22.2920, 16.1695, lossMap._entries[5]);
			AssertLossmapEntry(325, 54.6311, 16.1695, lossMap._entries[6]);
			AssertLossmapEntry(325, 1585.2433, 46.7818, lossMap._entries[7]);
			AssertLossmapEntry(32500, -1110.9105, 427.5510, lossMap._entries[8]);
			AssertLossmapEntry(32500, 358.4772, 396.9388, lossMap._entries[9]);
			AssertLossmapEntry(32500, 435.4003, 396.9388, lossMap._entries[10]);
			AssertLossmapEntry(32500, 1966.0126, 427.5510, lossMap._entries[11]);

		}

		private void AssertLossmapEntry(double inputSpeed, double inputTorque, double torqueLoss,
			TransmissionLossMap.GearLossMapEntry entry)
		{
			Assert.AreEqual(inputSpeed.RPMtoRad(), entry.InputSpeed);
			Assert.AreEqual(inputTorque, entry.InputTorque.Value(),1e-4);
			Assert.AreEqual(torqueLoss, entry.TorqueLoss.Value(), 1e-4);
		}

		#endregion
		
		#region Angledrive Data Asserts

		private void AssertAngledriveData(RelatedRun relatedRun)
		{
			var genericAngledriveData = relatedRun.VectoRunDataGenericBody.AngledriveData;
			var specificAngledriveData = relatedRun.VectoRunDataSpezificBody.AngledriveData;

			Assert.AreEqual(null, genericAngledriveData);
			Assert.AreEqual(genericAngledriveData, specificAngledriveData);
		}


		#endregion

		#region Auxiliary Data Asserts

		private void AssertAuxiliaryData(RelatedRun relatedRun)
		{
			var genericAuxiliaryData = relatedRun.VectoRunDataGenericBody.Aux;
			var specificAuxiliaryData = relatedRun.VectoRunDataSpezificBody.Aux;

			Assert.AreEqual(2, genericAuxiliaryData.Count());
			Assert.AreEqual("Hydraulic driven - Constant displacement pump", genericAuxiliaryData.First().Technology.First());
			Assert.AreEqual("Variable displacement elec. controlled", genericAuxiliaryData.Last().Technology.First());

			Assert.AreEqual("Hydraulic driven - Constant displacement pump", specificAuxiliaryData.First().Technology.First());
			Assert.AreEqual("Variable displacement elec. controlled", specificAuxiliaryData.Last().Technology.First());
		}

		#endregion

		#region Bus Auxiliary Electrical UserInput Config Asserts

		private void AssertElectricalUserInputConfig(RelatedRun relatedRun)
		{
			var genericElectric = 
				relatedRun.VectoRunDataGenericBody.BusAuxiliaries.ElectricalUserInputsConfig;
			var specificElectric =
				relatedRun.VectoRunDataSpezificBody.BusAuxiliaries.ElectricalUserInputsConfig;
			
			Assert.AreEqual(false, genericElectric.SmartElectrical);
			Assert.AreEqual(genericElectric.SmartElectrical, specificElectric.SmartElectrical);

			Assert.AreEqual(null, genericElectric.MaxAlternatorPower);
			Assert.AreEqual(genericElectric.MaxAlternatorPower, specificElectric.MaxAlternatorPower);

			Assert.AreEqual(null, genericElectric.ElectricStorageCapacity);
			Assert.AreEqual(genericElectric.ElectricStorageCapacity, specificElectric.ElectricStorageCapacity);

			Assert.AreEqual(0.7 ,genericElectric.AlternatorMap.GetEfficiency(0.RPMtoRad(),0.0.SI<Ampere>()));
			Assert.AreEqual(genericElectric.AlternatorMap, specificElectric.AlternatorMap); 

			Assert.AreEqual(Constants.BusAuxiliaries.ElectricSystem.AlternatorGearEfficiency, genericElectric.AlternatorGearEfficiency);
			Assert.AreEqual(genericElectric.AlternatorGearEfficiency, specificElectric.AlternatorGearEfficiency);

			Assert.AreEqual(Constants.BusAuxiliaries.ElectricalConsumers.DoorActuationTimeSecond, genericElectric.DoorActuationTimeSecond);
			Assert.AreEqual(genericElectric.DoorActuationTimeSecond, specificElectric.DoorActuationTimeSecond);

			//ToDo Test AverageCurrentDemandInclBaseLoad & AverageCurrentDemandWithoutBaseLoad
			//Assert.AreEqual(0, genericElectric.AverageCurrentDemandInclBaseLoad);
			//Assert.AreEqual(0, genericElectric.AverageCurrentDemandWithoutBaseLoad);

			//Assert.AreEqual(0, specificElectric.AverageCurrentDemandInclBaseLoad);
			//Assert.AreEqual(0, specificElectric.AverageCurrentDemandWithoutBaseLoad);

			Assert.AreEqual(null, genericElectric.ResultCardIdle);
			Assert.AreEqual( genericElectric.ResultCardIdle, specificElectric.ResultCardIdle);

			Assert.AreEqual(null, genericElectric.ResultCardTraction);
			Assert.AreEqual(genericElectric.ResultCardTraction, specificElectric.ResultCardTraction);

			Assert.AreEqual(null, genericElectric.ResultCardOverrun);
			Assert.AreEqual(genericElectric.ResultCardOverrun, specificElectric.ResultCardOverrun);
		}



		#endregion Asserts

		#region Pneumatic User Inputs Config Asserts

		private void AssertPneumaticUserInputsConfig(RelatedRun relatedRun)
		{
			var genericPneumaticUI = relatedRun.VectoRunDataGenericBody.BusAuxiliaries.PneumaticUserInputsConfig;
			var specificPneumaticUI = relatedRun.VectoRunDataSpezificBody.BusAuxiliaries.PneumaticUserInputsConfig;

			Assert.IsNotNull(genericPneumaticUI.CompressorMap);
			Assert.AreEqual(genericPneumaticUI.CompressorMap, specificPneumaticUI.CompressorMap);

			Assert.AreEqual(Constants.BusAuxiliaries.PneumaticUserConfig.CompressorGearEfficiency, genericPneumaticUI.CompressorGearEfficiency);
			Assert.AreEqual(genericPneumaticUI.CompressorGearEfficiency, specificPneumaticUI.CompressorGearEfficiency);

			Assert.AreEqual(1.0, genericPneumaticUI.CompressorGearRatio);
			Assert.AreEqual(genericPneumaticUI.CompressorGearRatio, specificPneumaticUI.CompressorGearRatio);

			Assert.AreEqual(false, genericPneumaticUI.SmartAirCompression);
			Assert.AreEqual(genericPneumaticUI.SmartAirCompression, specificPneumaticUI.SmartAirCompression);

			Assert.AreEqual(false, genericPneumaticUI.SmartRegeneration);
			Assert.AreEqual(genericPneumaticUI.SmartRegeneration, specificPneumaticUI.SmartRegeneration);

			Assert.AreEqual(Constants.BusAuxiliaries.PneumaticUserConfig.DefaultKneelingHeight, genericPneumaticUI.KneelingHeight);
			Assert.AreEqual(VectoMath.Max(0.SI<Meter>(), 0.120.SI<Meter>() - Constants.BusParameters.EntranceHeight), specificPneumaticUI.KneelingHeight);

			Assert.AreEqual(ConsumerTechnology.Electrically, genericPneumaticUI.AirSuspensionControl);
			Assert.AreEqual(genericPneumaticUI.AirSuspensionControl, specificPneumaticUI.AirSuspensionControl);

			Assert.AreEqual(ConsumerTechnology.Pneumatically, genericPneumaticUI.AdBlueDosing);
			Assert.AreEqual(genericPneumaticUI.AdBlueDosing, specificPneumaticUI.AdBlueDosing);

			Assert.AreEqual(ConsumerTechnology.Pneumatically, genericPneumaticUI.Doors);
			Assert.AreEqual(ConsumerTechnology.Pneumatically, specificPneumaticUI.Doors);
		}

		#endregion

		#region Pneumatic Consumer Demand Asserts

		private void AssertPneumaticConsumerDemand(RelatedRun relatedRun)
		{
			var genericConsumer = relatedRun.VectoRunDataGenericBody.BusAuxiliaries.PneumaticAuxillariesConfig;
			var specificConsumer = relatedRun.VectoRunDataSpezificBody.BusAuxiliaries.PneumaticAuxillariesConfig;
			
			Assert.AreEqual(Constants.BusAuxiliaries.PneumaticConsumersDemands.AdBlueInjection, genericConsumer.AdBlueInjection);
			Assert.AreEqual( genericConsumer.AdBlueInjection, specificConsumer.AdBlueInjection);
			Assert.AreEqual(Constants.BusAuxiliaries.PneumaticConsumersDemands.AirControlledSuspension, genericConsumer.AirControlledSuspension);
			Assert.AreEqual(genericConsumer.AirControlledSuspension, specificConsumer.AirControlledSuspension);
			Assert.AreEqual(Constants.BusAuxiliaries.PneumaticConsumersDemands.BrakingWithRetarder, genericConsumer.Braking);
			Assert.AreEqual(genericConsumer.Braking, specificConsumer.Braking);
			Assert.AreEqual(Constants.BusAuxiliaries.PneumaticConsumersDemands.BreakingAndKneeling, genericConsumer.BreakingWithKneeling);
			Assert.AreEqual(genericConsumer.BreakingWithKneeling, specificConsumer.BreakingWithKneeling);
			Assert.AreEqual(Constants.BusAuxiliaries.PneumaticConsumersDemands.DeadVolBlowOuts, genericConsumer.DeadVolBlowOuts);
			Assert.AreEqual(genericConsumer.DeadVolBlowOuts, specificConsumer.DeadVolBlowOuts);
			Assert.AreEqual(Constants.BusAuxiliaries.PneumaticConsumersDemands.DeadVolume, genericConsumer.DeadVolume);
			Assert.AreEqual(genericConsumer.DeadVolume, specificConsumer.DeadVolume);
			Assert.AreEqual(Constants.BusAuxiliaries.PneumaticConsumersDemands.NonSmartRegenFractionTotalAirDemand,
				genericConsumer.NonSmartRegenFractionTotalAirDemand);
			Assert.AreEqual(genericConsumer.NonSmartRegenFractionTotalAirDemand, specificConsumer.NonSmartRegenFractionTotalAirDemand);
			Assert.AreEqual(Constants.BusAuxiliaries.PneumaticConsumersDemands.SmartRegenFractionTotalAirDemand,
				genericConsumer.SmartRegenFractionTotalAirDemand);
			Assert.AreEqual(genericConsumer.SmartRegenFractionTotalAirDemand, specificConsumer.SmartRegenFractionTotalAirDemand);
			Assert.AreEqual(Constants.BusAuxiliaries.PneumaticConsumersDemands.OverrunUtilisationForCompressionFraction,
				genericConsumer.OverrunUtilisationForCompressionFraction);
			Assert.AreEqual(genericConsumer.OverrunUtilisationForCompressionFraction, specificConsumer.OverrunUtilisationForCompressionFraction);
			Assert.AreEqual(Constants.BusAuxiliaries.PneumaticConsumersDemands.DoorOpening, genericConsumer.DoorOpening);
			Assert.AreEqual(genericConsumer.DoorOpening, specificConsumer.DoorOpening);
			Assert.AreEqual(Constants.BusAuxiliaries.PneumaticConsumersDemands.StopBrakeActuation, genericConsumer.StopBrakeActuation);
			Assert.AreEqual(genericConsumer.StopBrakeActuation, specificConsumer.StopBrakeActuation);
		}

		#endregion
		
		#region SSMBusParameters Asserts

		private void AssertSSMBusParameters(RelatedRun relatedRun, int currentIndex)
		{
			var genericBusParam = relatedRun.VectoRunDataGenericBody.BusAuxiliaries.SSMInputs.BusParameters;
			var specificBusParam = relatedRun.VectoRunDataSpezificBody.BusAuxiliaries.SSMInputs.BusParameters;
			
			AssertLoading(genericBusParam.NumberOfPassengers.SI<Kilogram>(), 
				specificBusParam.NumberOfPassengers.SI<Kilogram>(), currentIndex);
			
			Assert.AreEqual(FloorType.HighFloor, genericBusParam.BusFloorType);
			Assert.AreEqual(FloorType.HighFloor, specificBusParam.BusFloorType);

			Assert.AreEqual(34.2500, genericBusParam.BusWindowSurface.Value());
			Assert.AreEqual(37.5750, specificBusParam.BusWindowSurface.Value());

			Assert.AreEqual(150.1200, genericBusParam.BusSurfaceArea.Value());
			Assert.AreEqual(146.6130, specificBusParam.BusSurfaceArea.Value());

			Assert.AreEqual(48.1950, genericBusParam.BusVolume.Value());
			Assert.AreEqual(54.2997, specificBusParam.BusVolume.Value());
		}

		#endregion

		#region Technolgy Benefits Asserts

		private void AssertTechnologyBenefits(RelatedRun relatedRun)
		{
			var genericTechnolgyBenefit = relatedRun.VectoRunDataGenericBody.BusAuxiliaries.SSMInputs.Technologies;
			var specificTechnolgyBenefit = relatedRun.VectoRunDataSpezificBody.BusAuxiliaries.SSMInputs.Technologies;

			Assert.AreEqual(0.08, genericTechnolgyBenefit.CValueVariation);
			Assert.AreEqual(0.06, genericTechnolgyBenefit.HValueVariation);
			Assert.AreEqual(0.08, genericTechnolgyBenefit.VCValueVariation);
			Assert.AreEqual(0.06, genericTechnolgyBenefit.VHValueVariation);
			Assert.AreEqual(0.04, genericTechnolgyBenefit.VVValueVariation);

			Assert.AreEqual(0.08, specificTechnolgyBenefit.CValueVariation);
			Assert.AreEqual(0.08, specificTechnolgyBenefit.HValueVariation);
			Assert.AreEqual(0.08, specificTechnolgyBenefit.VCValueVariation);
			Assert.AreEqual(0.08, specificTechnolgyBenefit.VHValueVariation);
			Assert.AreEqual(0.04, specificTechnolgyBenefit.VVValueVariation);
		}

		#endregion

		#region Boundary Conditions Asserts

		private void AssertBoundaryConditions(RelatedRun relatedRun)
		{
			var genericBound = relatedRun.VectoRunDataGenericBody.BusAuxiliaries.SSMInputs.BoundaryConditions;
			var specificBound = relatedRun.VectoRunDataGenericBody.BusAuxiliaries.SSMInputs.BoundaryConditions;
		
			Assert.AreEqual(Constants.BusAuxiliaries.SteadyStateModel.GFactor, genericBound.GFactor);
			Assert.AreEqual(genericBound.GFactor, specificBound.GFactor);

			Assert.AreEqual(Constants.BusAuxiliaries.SteadyStateModel.HeatingBoundaryTemperature,
				genericBound.HeatingBoundaryTemperature);
			Assert.AreEqual(genericBound.HeatingBoundaryTemperature, specificBound.HeatingBoundaryTemperature);

			Assert.AreEqual(Constants.BusAuxiliaries.SteadyStateModel.CoolingBoundaryTemperature, 
				genericBound.CoolingBoundaryTemperature);
			Assert.AreEqual(genericBound.CoolingBoundaryTemperature, specificBound.CoolingBoundaryTemperature);

			Assert.AreEqual(Constants.BusAuxiliaries.SteadyStateModel.SpecificVentilationPower, 
				genericBound.SpecificVentilationPower);
			Assert.AreEqual(genericBound.SpecificVentilationPower, specificBound.SpecificVentilationPower);

			Assert.AreEqual(Constants.BusAuxiliaries.SteadyStateModel.AuxHeaterEfficiency,
				genericBound.AuxHeaterEfficiency);
			Assert.AreEqual(genericBound.AuxHeaterEfficiency, specificBound.AuxHeaterEfficiency);

			Assert.AreEqual(Constants.BusAuxiliaries.SteadyStateModel.MaxPossibleBenefitFromTechnologyList,
				genericBound.MaxPossibleBenefitFromTechnologyList);
			Assert.AreEqual(genericBound.MaxPossibleBenefitFromTechnologyList, 
				specificBound.MaxPossibleBenefitFromTechnologyList);
		}

		#endregion

		#region Environmental Conditions Asserts

		private void AssertEnvironmentalConditions(RelatedRun relatedRun)
		{
			var genericEnv = relatedRun.VectoRunDataGenericBody.BusAuxiliaries.SSMInputs.EnvironmentalConditions;
			var specificEnv = relatedRun.VectoRunDataSpezificBody.BusAuxiliaries.SSMInputs.EnvironmentalConditions;

			Assert.AreEqual(Constants.BusAuxiliaries.SteadyStateModel.DefaultSolar, genericEnv.DefaultConditions.Solar);
			Assert.AreEqual(genericEnv.DefaultConditions.Solar, specificEnv.DefaultConditions.Solar);
			Assert.AreEqual(Constants.BusAuxiliaries.SteadyStateModel.DefaultTemperature, genericEnv.DefaultConditions.Temperature);
			Assert.AreEqual(genericEnv.DefaultConditions.Temperature, specificEnv.DefaultConditions.Temperature);
			Assert.AreEqual(1.0, genericEnv.DefaultConditions.Weighting);
			Assert.AreEqual(genericEnv.DefaultConditions.Weighting, specificEnv.DefaultConditions.Weighting);
			Assert.AreEqual(DeclarationData.BusAuxiliaries.DefaultEnvironmentalConditions, genericEnv.EnvironmentalConditionsMap);
			Assert.AreEqual(genericEnv.EnvironmentalConditionsMap, specificEnv.EnvironmentalConditionsMap);
		}

		#endregion

		#region SSMInputs Asserts

		private void AssertSSMInputs(RelatedRun relatedRun, int currentIndex)
		{
			var genericSSMInput = (SSMInputs) relatedRun.VectoRunDataGenericBody.BusAuxiliaries.SSMInputs;
			var specificSSMInput = (SSMInputs)relatedRun.VectoRunDataSpezificBody.BusAuxiliaries.SSMInputs;

			AssertLoading(genericSSMInput.NumberOfPassengers.SI<Kilogram>(),
				specificSSMInput.NumberOfPassengers.SI<Kilogram>(), currentIndex);

			AssertHVACMaxCoolingPower(genericSSMInput.HVACMaxCoolingPower.Value(),
				specificSSMInput.HVACMaxCoolingPower.Value(), currentIndex);

			AssertCOP(genericSSMInput.COP, specificSSMInput.COP, currentIndex);

			Assert.AreEqual(true, genericSSMInput.VentilationOnDuringHeating);
			Assert.AreEqual(true, specificSSMInput.VentilationOnDuringHeating);
			Assert.AreEqual(true, genericSSMInput.VentilationWhenBothHeatingAndACInactive);
			Assert.AreEqual(true, specificSSMInput.VentilationWhenBothHeatingAndACInactive);
			Assert.AreEqual(true, genericSSMInput.VentilationDuringAC);
			Assert.AreEqual(true, specificSSMInput.VentilationDuringAC);

			Assert.AreEqual(30000, genericSSMInput.FuelFiredHeaterPower.Value());
			Assert.AreEqual(0, specificSSMInput.FuelFiredHeaterPower.Value());

			Assert.AreEqual(Constants.BusAuxiliaries.Heater.FuelEnergyToHeatToCoolant,
				genericSSMInput.FuelEnergyToHeatToCoolant);
			Assert.AreEqual(genericSSMInput.FuelEnergyToHeatToCoolant, specificSSMInput.FuelEnergyToHeatToCoolant);

			Assert.AreEqual(Constants.BusAuxiliaries.Heater.CoolantHeatTransferredToAirCabinHeater,
				genericSSMInput.CoolantHeatTransferredToAirCabinHeater);
			Assert.AreEqual(genericSSMInput.CoolantHeatTransferredToAirCabinHeater,
				specificSSMInput.CoolantHeatTransferredToAirCabinHeater);
		}


		private void AssertHVACMaxCoolingPower(double genericValue,double specificValue, int currentIndex)
		{
			switch (currentIndex) {
				case 0:
				case 1://Interurban
					Assert.AreEqual(28302.3750, genericValue);
					Assert.AreEqual(33507.3425, specificValue);
					break;
				case 2:
				case 3://Coach
					Assert.AreEqual(42260.875, genericValue);
					Assert.AreEqual(48797.2525, specificValue);
					break;
			}
		}

		private void AssertCOP(double genericValue, double specificValue, int currentIndex)
		{
			switch (currentIndex)
			{
				case 0:
				case 1://Interurban
					Assert.AreEqual(3.5, genericValue);
					Assert.AreEqual(3.559554528, specificValue, 1e-9);
					break;
				case 2:
				case 3://Coach
					Assert.AreEqual(3.5, genericValue);
					Assert.AreEqual(3.564261972, specificValue, 1e-9);
					break;
			}
		}

		#endregion

		#region Retarder Asserts

		private void AssertRetarder(RelatedRun relatedRun)
		{
			var genericRetarder = relatedRun.VectoRunDataGenericBody.Retarder;
			var specificRetarder = relatedRun.VectoRunDataSpezificBody.Retarder;

			Assert.AreEqual(1, genericRetarder.Ratio);
			Assert.AreEqual( genericRetarder.Ratio, specificRetarder.Ratio);

			Assert.AreEqual(RetarderType.TransmissionOutputRetarder, genericRetarder.Type);
			Assert.AreEqual(genericRetarder.Type, specificRetarder.Type);

			Assert.AreEqual(genericRetarder.LossMap, specificRetarder.LossMap);
		}

		#endregion

		#region  Driver Data Asserts

		private void AssertDriverData(RelatedRun relatedRun)
		{
			var genericDriver = relatedRun.VectoRunDataGenericBody.DriverData;
			var specificDriver = relatedRun.VectoRunDataGenericBody.DriverData;

			Assert.IsNotNull(genericDriver.AccelerationCurve);
			Assert.IsNotNull(specificDriver.AccelerationCurve);
			Assert.AreEqual(genericDriver.AccelerationCurve, specificDriver.AccelerationCurve);

			Assert.AreEqual(DeclarationData.Driver.LookAhead.Enabled, genericDriver.LookAheadCoasting.Enabled);
			Assert.AreEqual(DeclarationData.Driver.LookAhead.MinimumSpeed, genericDriver.LookAheadCoasting.MinSpeed);
			Assert.IsNotNull(genericDriver.LookAheadCoasting.LookAheadDecisionFactor);
			Assert.AreEqual(DeclarationData.Driver.LookAhead.LookAheadDistanceFactor,
				genericDriver.LookAheadCoasting.LookAheadDistanceFactor);
			Assert.AreEqual(genericDriver.LookAheadCoasting, specificDriver.LookAheadCoasting);

			Assert.AreEqual(true, genericDriver.OverSpeed.Enabled);
			Assert.AreEqual(DeclarationData.Driver.OverSpeed.MinSpeed, genericDriver.OverSpeed.MinSpeed);
			Assert.AreEqual(DeclarationData.Driver.OverSpeed.AllowedOverSpeed, genericDriver.OverSpeed.OverSpeed);
			Assert.AreEqual(genericDriver.OverSpeed, specificDriver.OverSpeed);

			AssertStopStartData(genericDriver.EngineStopStart);
			AssertEcoRoll(genericDriver.EcoRoll);
			AssertPccData(genericDriver.PCC);

			AssertStopStartData(specificDriver.EngineStopStart);
			AssertEcoRoll(specificDriver.EcoRoll);
			AssertPccData(specificDriver.PCC);
		}

		private void AssertStopStartData(DriverData.EngineStopStartData engineStopStart)
		{
			Assert.AreEqual(DeclarationData.Driver.EngineStopStart.ActivationDelay, engineStopStart.EngineOffStandStillActivationDelay);
			Assert.AreEqual(DeclarationData.Driver.EngineStopStart.MaxEngineOffTimespan, engineStopStart.MaxEngineOffTimespan);
			Assert.AreEqual(DeclarationData.Driver.EngineStopStart.UtilityFactor, engineStopStart.UtilityFactor);
		}

		private void AssertEcoRoll(DriverData.EcoRollData ecoRoll)
		{
			Assert.AreEqual(DeclarationData.Driver.EcoRoll.UnderspeedThreshold, ecoRoll.UnderspeedThreshold);
			Assert.AreEqual(DeclarationData.Driver.EcoRoll.MinSpeed, ecoRoll.MinSpeed);
			Assert.AreEqual(DeclarationData.Driver.EcoRoll.ActivationDelay, ecoRoll.ActivationPhaseDuration);
			Assert.AreEqual(DeclarationData.Driver.EcoRoll.AccelerationLowerLimit, ecoRoll.AccelerationLowerLimit);
			Assert.AreEqual(DeclarationData.Driver.EcoRoll.AccelerationUpperLimit, ecoRoll.AccelerationUpperLimit);
		}

		private void AssertPccData(DriverData.PCCData pccData)
		{
			Assert.AreEqual(DeclarationData.Driver.PCC.PCCEnableSpeed, pccData.PCCEnableSpeed);
			Assert.AreEqual(DeclarationData.Driver.PCC.MinSpeed, pccData.MinSpeed);
			Assert.AreEqual(DeclarationData.Driver.PCC.PreviewDistanceUseCase1, pccData.PreviewDistanceUseCase1);
			Assert.AreEqual(DeclarationData.Driver.PCC.PreviewDistanceUseCase2, pccData.PreviewDistanceUseCase2);
			Assert.AreEqual(DeclarationData.Driver.PCC.Underspeed, pccData.UnderSpeed);
			Assert.AreEqual(DeclarationData.Driver.PCC.OverspeedUseCase3, pccData.OverspeedUseCase3);
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
			for (int i = 0; i < runs.Count; i++)
			{
				var relatedRun = new RelatedRun
				{
					VectoRunDataSpezificBody = runs[i],
					VectoRunDataGenericBody = runs[i + 1]
				};
				relatedRuns.Add(relatedRun);
				i++;
			}
		}


		[TestCase()]
		public void PrintModelParameters()
		{
			var writer = new FileOutputWriter(Path.Combine(Path.GetDirectoryName(JobFile), Path.GetFileName(JobFile)));
			var inputData = JSONInputDataFactory.ReadJsonJob(JobFile);

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
			var pair = relatedRuns.First();

			var json = JToken.FromObject(pair.VectoRunDataGenericBody);
			File.WriteAllText($"{pair.VectoRunDataGenericBody.JobName}_{pair.VectoRunDataGenericBody.ModFileSuffix}_Generic.json", JsonConvert.SerializeObject(json, Formatting.Indented));

			//Console.WriteLine("Generic Body");
			//PrintVectoRunData(pair.VectoRunDataGenericBody);
			//Console.WriteLine("========================");
			//Console.WriteLine("Specific Body");
			//PrintVectoRunData(pair.VectoRunDataSpezificBody);
		}

		private void PrintVectoRunData(VectoRunData runData)
		{
			const BindingFlags flags =
				BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public |
				BindingFlags.FlattenHierarchy;
			var properties = runData.GetType().GetProperties(flags);
			foreach (var p in properties)
			{

				var val = p.GetValue(runData);
					
			}
		}
	}
}
