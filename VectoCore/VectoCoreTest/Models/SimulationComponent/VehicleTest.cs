/*
* This file is part of VECTO.
*
* Copyright © 2012-2016 European Union
*
* Developed by Graz University of Technology,
*              Institute of Internal Combustion Engines and Thermodynamics,
*              Institute of Technical Informatics
*
* VECTO is licensed under the EUPL, Version 1.1 or - as soon they will be approved
* by the European Commission - subsequent versions of the EUPL (the "Licence");
* You may not use VECTO except in compliance with the Licence.
* You may obtain a copy of the Licence at:
*
* https://joinup.ec.europa.eu/community/eupl/og_page/eupl
*
* Unless required by applicable law or agreed to in writing, VECTO
* distributed under the Licence is distributed on an "AS IS" basis,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the Licence for the specific language governing permissions and
* limitations under the Licence.
*
* Authors:
*   Stefan Hausberger, hausberger@ivt.tugraz.at, IVT, Graz University of Technology
*   Christian Kreiner, christian.kreiner@tugraz.at, ITI, Graz University of Technology
*   Michael Krisper, michael.krisper@tugraz.at, ITI, Graz University of Technology
*   Raphael Luz, luz@ivt.tugraz.at, IVT, Graz University of Technology
*   Markus Quaritsch, markus.quaritsch@tugraz.at, IVT, Graz University of Technology
*   Martin Rexeis, rexeis@ivt.tugraz.at, IVT, Graz University of Technology
*/

using System;
using System.IO;
using NUnit.Framework;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.Tests.Utils;

namespace TUGraz.VectoCore.Tests.Models.SimulationComponent
{
	[TestFixture]
	public class VehicleTest
	{
		private const string VehicleDataFileCoach = @"TestData\Components\24t Coach.vveh";
		private const string VehicleDataFileTruck = @"TestData\Components\40t_Long_Haul_Truck.vveh";
		public static readonly double Tolerance = 0.001;

		[Test]
		public void VehiclePortTest()
		{
			var container = new VehicleContainer(ExecutionMode.Engineering);

			//var reader = new EngineeringModeSimulationDataReader();
			var vehicleData = MockSimulationDataFactory.CreateVehicleDataFromFile(VehicleDataFileCoach);
			//VehicleData.ReadFromFile(VehicleDataFile);
			//vehicleData.CrossWindCorrectionMode = CrossWindCorrectionMode.NoCorrection;
			var vehicle = new Vehicle(container, vehicleData);
			var driver = new MockDriver(container) { DriverBehavior = DrivingBehavior.Driving };
			var mockPort = new MockFvOutPort();
			vehicle.InPort().Connect(mockPort);

			vehicle.Initialize(17.210535.SI<MeterPerSecond>(), 0.SI<Radian>());

			var requestPort = vehicle.OutPort();

			var absTime = 0.SI<Second>();
			var dt = 1.SI<Second>();

			var accell = -0.256231159.SI<MeterPerSquareSecond>();
			var gradient = Math.Atan(0.00366547048).SI<Radian>();

			requestPort.Request(absTime, dt, accell, gradient);

			Assert.AreEqual(-2305.43268, mockPort.Force.Value(), 0.0001);
			Assert.AreEqual(16.954303841, mockPort.Velocity.Value(), 0.0001);
		}

		[Test,
		TestCase(0, 0, 0.5, 0),
		TestCase(0, 1, 0.5, 0.59839),
		TestCase(60, 0, 0.5, 1329.7558),
		TestCase(60, 1, 0.5, 1365.386),
		TestCase(60, 0.5, 0.5, 1347.457494),
		TestCase(72, 0.5, 0.5, 1852.8837),
		TestCase(72, 1, 3, 2093.7506)]
		public void VehicleAirResistanceTest(double vehicleSpeed, double acceleration, double dt, double expected)
		{
			var container = new VehicleContainer(ExecutionMode.Declaration);

			var vehicleData = MockSimulationDataFactory.CreateVehicleDataFromFile(VehicleDataFileTruck);
			//vehicleData.AerodynamicDragAera = 6.46.SI<SquareMeter>();
			vehicleData.CrossWindCorrectionCurve = new CrosswindCorrectionCdxALookup(
				DeclarationDataAdapter.GetDeclarationAirResistanceCurve("TractorSemitrailer",
					6.46.SI<SquareMeter>()), CrossWindCorrectionMode.DeclarationModeCorrection);
			var vehicle = new Vehicle(container, vehicleData);

			var mockPort = new MockFvOutPort();
			vehicle.InPort().Connect(mockPort);

			// ====================

			vehicle.Initialize(vehicleSpeed.KMPHtoMeterPerSecond(), 0.SI<Radian>());

			var nextSpeed = vehicleSpeed.KMPHtoMeterPerSecond() + acceleration.SI<MeterPerSquareSecond>() * dt.SI<Second>();
			var avgForce = vehicle.AirDragResistance(vehicleSpeed.KMPHtoMeterPerSecond(), nextSpeed);
			Assert.AreEqual(expected, avgForce.Value(), Tolerance);
		}

		[Test]
		public void VehicleAirDragPowerLossDeclarationTest()
		{
			var container = new VehicleContainer(ExecutionMode.Declaration);

			var vehicleData = MockSimulationDataFactory.CreateVehicleDataFromFile(VehicleDataFileTruck);
			vehicleData.CrossWindCorrectionCurve = new CrosswindCorrectionCdxALookup(
				DeclarationDataAdapter.GetDeclarationAirResistanceCurve("TractorSemitrailer",
					6.2985.SI<SquareMeter>()), CrossWindCorrectionMode.DeclarationModeCorrection);

			var vehicle = new Vehicle(container, vehicleData);
			var driver = new MockDriver(container) { DriverBehavior = DrivingBehavior.Driving };
			var mockPort = new MockFvOutPort();
			vehicle.InPort().Connect(mockPort);

			var writer = new MockModalDataContainer();

			vehicle.Initialize(80.KMPHtoMeterPerSecond(), 0.SI<Radian>());

			var absTime = 0.SI<Second>();
			var dt = 0.5.SI<Second>();

			vehicle.Request(absTime, dt, 0.SI<MeterPerSquareSecond>(), 0.SI<Radian>());
			vehicle.CommitSimulationStep(writer);

			Assert.AreEqual(48033.405, ((SI)writer[ModalResultField.P_air]).Value(), 0.1);

			vehicle.Request(absTime, dt, 1.SI<MeterPerSquareSecond>(), 0.SI<Radian>());
			vehicle.CommitSimulationStep(writer);
			Assert.AreEqual(49566.561, ((SI)writer[ModalResultField.P_air]).Value(), 0.1);
		}

		[Test,
		TestCase(5.19, 0, 1.173),
		TestCase(5.19, 40, 1.173),
		TestCase(5.19, 60, 1.173),
		TestCase(5.19, 80, 1.109),
		TestCase(5.19, 100, 1.075),
		TestCase(5.19, 62.5, 1.163),
		]
		public void VehicleAirDragSpeedDependentTest(double crossSectionArea, double velocity, double expectedFactor)
		{
			var data = new[] {
				"v_veh in km/h,Cd factor in -",
				"0,1.173 ",
				"5,1.173 ",
				"10,1.173",
				"15,1.173",
				"20,1.173",
				"25,1.173",
				"30,1.173",
				"35,1.173",
				"40,1.173",
				"45,1.173",
				"50,1.173",
				"55,1.173",
				"60,1.173",
				"65,1.153",
				"70,1.136",
				"75,1.121",
				"80,1.109",
				"85,1.099",
				"90,1.090",
				"95,1.082",
				"100,1.075"
			};
			var correctionData = new MemoryStream();
			var writer = new StreamWriter(correctionData);
			foreach (var entry in data) {
				writer.WriteLine(entry);
			}
			writer.Flush();
			correctionData.Seek(0, SeekOrigin.Begin);

			var cwcc =
				new CrosswindCorrectionCdxALookup(
					CrossWindCorrectionCurveReader.ReadSpeedDependentCorrectionCurveFromStream(correctionData,
						crossSectionArea.SI<SquareMeter>()), CrossWindCorrectionMode.SpeedDependentCorrectionFactor);

			Assert.AreEqual(crossSectionArea * expectedFactor,
				cwcc.EffectiveAirDragArea(velocity.KMPHtoMeterPerSecond()).Value(),
				Tolerance);
		}
	}
}