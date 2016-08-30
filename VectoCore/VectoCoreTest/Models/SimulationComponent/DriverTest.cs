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

using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Models.Connector.Ports;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.FileIO;
using TUGraz.VectoCore.Tests.Utils;
using TUGraz.VectoCore.Utils;
using Wheels = TUGraz.VectoCore.Models.SimulationComponent.Impl.Wheels;

namespace TUGraz.VectoCore.Tests.Models.SimulationComponent
{
	[TestClass]
	public class DriverTest
	{
		public const string JobFile = @"TestData\Jobs\24t Coach EngineOnly.vecto";
		public const string EngineFile = @"TestData\Components\24t Coach.veng";
		public const string AccelerationFile = @"TestData\Components\Coach.vacc";
		public const double Tolerance = 0.001;

		[TestMethod]
		public void DriverCoastingTest()
		{
			var engineData = MockSimulationDataFactory.CreateEngineDataFromFile(EngineFile);

			var vehicleData = CreateVehicleData(33000.SI<Kilogram>());

			var driverData = CreateDriverData();

			var fileWriter = new FileOutputWriter("Coach_MinimalPowertrain_Coasting");
			var modData = new ModalDataContainer("Coach_MinimalPowertrain_Coasting", fileWriter);
			var vehicleContainer = new VehicleContainer(ExecutionMode.Engineering, modData);
			var mockCycle = new MockDrivingCycle(vehicleContainer, null);

			var driver = new Driver(vehicleContainer, driverData, new DefaultDriverStrategy());
			var engine = new CombustionEngine(vehicleContainer, engineData);
			var clutch = new Clutch(vehicleContainer, engineData);
			dynamic tmp = AddComponent(driver, new Vehicle(vehicleContainer, vehicleData));
			tmp = AddComponent(tmp, new Wheels(vehicleContainer, vehicleData.DynamicTyreRadius, vehicleData.WheelsInertia));
			tmp = AddComponent(tmp, clutch);
			AddComponent(tmp, engine);
			clutch.IdleController = engine.IdleController;

			var gbx = new MockGearbox(vehicleContainer) { Gear = 1 };

			var driverPort = driver.OutPort();

			var velocity = 5.SI<MeterPerSecond>();
			driverPort.Initialize(velocity, 0.SI<Radian>());

			var absTime = 0.SI<Second>();

			var response = driver.DrivingActionCoast(absTime, 1.SI<Meter>(), velocity, 0.SI<Radian>());

			Assert.IsInstanceOfType(response, typeof(ResponseSuccess));

			vehicleContainer.CommitSimulationStep(absTime, response.SimulationInterval);
			absTime += response.SimulationInterval;

			Assert.AreEqual(4.9812, vehicleContainer.VehicleSpeed.Value(), Tolerance);
			Assert.AreEqual(0.2004, response.SimulationInterval.Value(), Tolerance);
			Assert.AreEqual(engine.PreviousState.FullDragTorque.Value(), engine.PreviousState.EngineTorque.Value(),
				Constants.SimulationSettings.LineSearchTolerance);

			while (vehicleContainer.VehicleSpeed > 1) {
				response = driver.DrivingActionCoast(absTime, 1.SI<Meter>(), velocity, 0.SI<Radian>());

				Assert.IsInstanceOfType(response, typeof(ResponseSuccess));

				vehicleContainer.CommitSimulationStep(absTime, response.SimulationInterval);
				absTime += response.SimulationInterval;
				modData.Finish(VectoRun.Status.Success);
			}
			modData.Finish(VectoRun.Status.Success);
		}

		[TestMethod]
		public void DriverCoastingTest2()
		{
			var engineData = MockSimulationDataFactory.CreateEngineDataFromFile(EngineFile);

			var vehicleData = CreateVehicleData(33000.SI<Kilogram>());

			var driverData = CreateDriverData();

			var fileWriter = new FileOutputWriter("Coach_MinimalPowertrain_Coasting");
			var modData = new ModalDataContainer("Coach_MinimalPowertrain_Coasting", fileWriter);
			var vehicleContainer = new VehicleContainer(ExecutionMode.Engineering, modData);
			var mockCycle = new MockDrivingCycle(vehicleContainer, null);

			var driver = new Driver(vehicleContainer, driverData, new DefaultDriverStrategy());
			var engine = new CombustionEngine(vehicleContainer, engineData);
			var clutch = new Clutch(vehicleContainer, engineData);

			dynamic tmp = AddComponent(driver, new Vehicle(vehicleContainer, vehicleData));
			tmp = AddComponent(tmp, new Wheels(vehicleContainer, vehicleData.DynamicTyreRadius, vehicleData.WheelsInertia));
			tmp = AddComponent(tmp, clutch);
			AddComponent(tmp, engine);
			clutch.IdleController = engine.IdleController;

			var gbx = new MockGearbox(vehicleContainer);
			gbx.Gear = 1;

			var driverPort = driver.OutPort();

			var gradient = VectoMath.InclinationToAngle(-0.020237973 / 100.0);
			var velocity = 5.SI<MeterPerSecond>();
			driverPort.Initialize(velocity, gradient);

			var absTime = 0.SI<Second>();

			var response = driver.DrivingActionCoast(absTime, 1.SI<Meter>(), velocity, gradient);

			Assert.IsInstanceOfType(response, typeof(ResponseSuccess));

			vehicleContainer.CommitSimulationStep(absTime, response.SimulationInterval);
			absTime += response.SimulationInterval;

			Assert.AreEqual(4.9812, vehicleContainer.VehicleSpeed.Value(), Tolerance);
			Assert.AreEqual(0.2004, response.SimulationInterval.Value(), Tolerance);
			Assert.AreEqual(engine.PreviousState.FullDragTorque.Value(), engine.PreviousState.EngineTorque.Value(),
				Constants.SimulationSettings.LineSearchTolerance);

			while (vehicleContainer.VehicleSpeed > 1) {
				response = driver.DrivingActionCoast(absTime, 1.SI<Meter>(), velocity, gradient);

				Assert.IsInstanceOfType(response, typeof(ResponseSuccess));

				vehicleContainer.CommitSimulationStep(absTime, response.SimulationInterval);
				absTime += response.SimulationInterval;
				modData.Finish(VectoRun.Status.Success);
			}
			modData.Finish(VectoRun.Status.Success);
		}

		[TestMethod]
		public void DriverOverloadTest()
		{
			var engineData = MockSimulationDataFactory.CreateEngineDataFromFile(EngineFile);

			var vehicleData = CreateVehicleData(33000.SI<Kilogram>());

			var driverData = CreateDriverData();

			var fileWriter = new FileOutputWriter("Coach_MinimalPowertrain");
			var modData = new ModalDataContainer("Coach_MinimalPowertrain", fileWriter);
			var vehicleContainer = new VehicleContainer(ExecutionMode.Engineering, modData);

			var cycle = new MockDrivingCycle(vehicleContainer, null);

			var driver = new Driver(vehicleContainer, driverData, new DefaultDriverStrategy());

			dynamic tmp = AddComponent(driver, new Vehicle(vehicleContainer, vehicleData));
			tmp = AddComponent(tmp, new Wheels(vehicleContainer, vehicleData.DynamicTyreRadius, vehicleData.WheelsInertia));
			var engine = new CombustionEngine(vehicleContainer, engineData);
			var clutch = new Clutch(vehicleContainer, engineData);
			clutch.IdleController = engine.IdleController;
			tmp = AddComponent(tmp, clutch);
			AddComponent(tmp, engine);

			var gbx = new MockGearbox(vehicleContainer);

			var driverPort = driver.OutPort();

			driverPort.Initialize(0.SI<MeterPerSecond>(), 0.SI<Radian>());

			var absTime = 0.SI<Second>();

			var response = driverPort.Request(absTime, 1.SI<Meter>(), 20.SI<MeterPerSecond>(), 0.SI<Radian>());

			Assert.IsInstanceOfType(response, typeof(ResponseSuccess));

			vehicleContainer.CommitSimulationStep(absTime, response.SimulationInterval);
			absTime += response.SimulationInterval;

			Assert.AreEqual(0.9748, modData.GetValues<SI>(ModalResultField.acc).Last().Value(), Tolerance);

			response = driverPort.Request(absTime, 1.SI<Meter>(), 20.SI<MeterPerSecond>(), 0.SI<Radian>());

			Assert.IsInstanceOfType(response, typeof(ResponseSuccess));

			vehicleContainer.CommitSimulationStep(absTime, response.SimulationInterval);
			absTime += response.SimulationInterval;

			Assert.AreEqual(0.78766, modData.GetValues<SI>(ModalResultField.acc).Last().Value(), Tolerance);
		}

		[TestMethod]
		public void DriverAccelerationTest()
		{
			var vehicleContainer = new VehicleContainer(ExecutionMode.Engineering);
			var vehicle = new MockVehicle(vehicleContainer);

			var driverData = MockSimulationDataFactory.CreateDriverDataFromFile(JobFile);
			var driver = new Driver(vehicleContainer, driverData, new DefaultDriverStrategy());

			var cycle = new MockDrivingCycle(vehicleContainer, null);

			driver.Connect(vehicle.OutPort());

			vehicle.MyVehicleSpeed = 0.SI<MeterPerSecond>();
			var absTime = 0.SI<Second>();
			var ds = 1.SI<Meter>();
			var gradient = 0.SI<Radian>();

			var targetVelocity = 5.SI<MeterPerSecond>();

			//			var response = driver.OutPort().Request(absTime, ds, targetVelocity, gradient);

			var accelerations = new[] {
				1.01570922, 1.384540943, 1.364944972, 1.350793466, 1.331848649, 1.314995215, 1.2999934,
				1.281996392, 1.255462262
			};
			var simulationIntervals = new[] {
				1.403234648, 0.553054094, 0.405255346, 0.33653593, 0.294559444, 0.26555781, 0.243971311, 0.22711761,
				0.213554656
			};

			// accelerate from 0 to just below the target velocity and test derived simulation intervals & accelerations
			for (var i = 0; i < accelerations.Length; i++) {
				var tmpResponse = driver.OutPort().Request(absTime, ds, targetVelocity, gradient);

				Assert.IsInstanceOfType(tmpResponse, typeof(ResponseSuccess));
				Assert.AreEqual(accelerations[i], vehicle.LastRequest.acceleration.Value(), Tolerance);
				Assert.AreEqual(simulationIntervals[i], tmpResponse.SimulationInterval.Value(), Tolerance);

				vehicleContainer.CommitSimulationStep(absTime, tmpResponse.SimulationInterval);
				absTime += tmpResponse.SimulationInterval;
				vehicle.MyVehicleSpeed +=
					(tmpResponse.SimulationInterval * vehicle.LastRequest.acceleration).Cast<MeterPerSecond>();
			}

			// full acceleration would exceed target velocity, driver should limit acceleration such that target velocity is reached...
			var response = driver.OutPort().Request(absTime, ds, targetVelocity, gradient);

			Assert.IsInstanceOfType(response, typeof(ResponseSuccess));
			Assert.AreEqual(0.899715479, vehicle.LastRequest.acceleration.Value(), Tolerance);
			Assert.AreEqual(0.203734517, response.SimulationInterval.Value(), Tolerance);

			vehicleContainer.CommitSimulationStep(absTime, response.SimulationInterval);
			absTime += response.SimulationInterval;
			vehicle.MyVehicleSpeed +=
				(response.SimulationInterval * vehicle.LastRequest.acceleration).Cast<MeterPerSecond>();

			Assert.AreEqual(targetVelocity.Value(), vehicle.MyVehicleSpeed.Value(), Tolerance);

			// vehicle has reached target velocity, no further acceleration necessary...

			response = driver.OutPort().Request(absTime, ds, targetVelocity, gradient);

			Assert.IsInstanceOfType(response, typeof(ResponseSuccess));
			Assert.AreEqual(0, vehicle.LastRequest.acceleration.Value(), Tolerance);
			Assert.AreEqual(0.2, response.SimulationInterval.Value(), Tolerance);
		}

		[TestMethod]
		public void DriverDecelerationTest()
		{
			var vehicleContainer = new VehicleContainer(ExecutionMode.Engineering);
			var vehicle = new MockVehicle(vehicleContainer);

			var driverData = MockSimulationDataFactory.CreateDriverDataFromFile(JobFile);
			var driver = new Driver(vehicleContainer, driverData, new DefaultDriverStrategy());

			var cycle = new MockDrivingCycle(vehicleContainer, null);

			driver.Connect(vehicle.OutPort());

			vehicle.MyVehicleSpeed = 5.SI<MeterPerSecond>();
			var absTime = 0.SI<Second>();
			var ds = 1.SI<Meter>();
			var gradient = 0.SI<Radian>();

			var targetVelocity = 0.SI<MeterPerSecond>();

			//			var response = driver.OutPort().Request(absTime, ds, targetVelocity, gradient);

			var accelerations = new[] {
				-0.68799597, -0.690581291, -0.693253225, -0.696020324, -0.698892653, -0.701882183, -0.695020765,
				-0.677731071,
				-0.660095846, -0.642072941, -0.623611107, -0.604646998, -0.58510078, -0.56497051, -0.547893288,
				-0.529859078,
				-0.510598641, -0.489688151, -0.466386685, -0.425121905
			};
			var simulationIntervals = new[] {
				0.202830428, 0.20884052, 0.215445127, 0.222749141, 0.230885341, 0.240024719, 0.250311822, 0.26182762,
				0.274732249,
				0.289322578, 0.305992262, 0.325276486, 0.34792491, 0.37502941, 0.408389927, 0.451003215, 0.5081108,
				0.590388012,
				0.724477573, 1.00152602
			};

			// accelerate from 0 to just below the target velocity and test derived simulation intervals & accelerations
			for (var i = 0; i < accelerations.Length; i++) {
				var tmpResponse = driver.OutPort().Request(absTime, ds, targetVelocity, gradient);

				Assert.IsInstanceOfType(tmpResponse, typeof(ResponseSuccess));
				Assert.AreEqual(accelerations[i], vehicle.LastRequest.acceleration.Value(), Tolerance);
				Assert.AreEqual(simulationIntervals[i], tmpResponse.SimulationInterval.Value(), Tolerance);

				vehicleContainer.CommitSimulationStep(absTime, tmpResponse.SimulationInterval);
				absTime += tmpResponse.SimulationInterval;
				vehicle.MyVehicleSpeed +=
					(tmpResponse.SimulationInterval * vehicle.LastRequest.acceleration).Cast<MeterPerSecond>();
			}

			var response = driver.OutPort().Request(absTime, ds, targetVelocity, gradient);

			Assert.IsInstanceOfType(response, typeof(ResponseSuccess));
			Assert.AreEqual(-0.308576594, vehicle.LastRequest.acceleration.Value(), Tolerance);
			Assert.AreEqual(2.545854078, response.SimulationInterval.Value(), Tolerance);

			vehicleContainer.CommitSimulationStep(absTime, response.SimulationInterval);
			//absTime += response.SimulationInterval;
			vehicle.MyVehicleSpeed +=
				(response.SimulationInterval * vehicle.LastRequest.acceleration).Cast<MeterPerSecond>();

			Assert.AreEqual(targetVelocity.Value(), vehicle.MyVehicleSpeed.Value(), Tolerance);
		}

		//==================

		private static VehicleData CreateVehicleData(Kilogram loading)
		{
			var axles = new List<Axle> {
				new Axle {
					AxleWeightShare = 0.4375,
					Inertia = 21.66667.SI<KilogramSquareMeter>(),
					RollResistanceCoefficient = 0.0055,
					TwinTyres = false,
					TyreTestLoad = 62538.75.SI<Newton>()
				},
				new Axle {
					AxleWeightShare = 0.375,
					Inertia = 10.83333.SI<KilogramSquareMeter>(),
					RollResistanceCoefficient = 0.0065,
					TwinTyres = false,
					TyreTestLoad = 52532.55.SI<Newton>()
				},
				new Axle {
					AxleWeightShare = 0.1875,
					Inertia = 21.66667.SI<KilogramSquareMeter>(),
					RollResistanceCoefficient = 0.0055,
					TwinTyres = false,
					TyreTestLoad = 62538.75.SI<Newton>()
				}
			};
			return new VehicleData {
				AxleConfiguration = AxleConfiguration.AxleConfig_4x2,
				CrossWindCorrectionCurve =
					new CrosswindCorrectionCdxALookup(CrossWindCorrectionCurveReader.GetNoCorrectionCurve(3.2634.SI<SquareMeter>()),
						CrossWindCorrectionMode.NoCorrection),
				CurbWeight = 15700.SI<Kilogram>(),
				Loading = loading,
				DynamicTyreRadius = 0.52.SI<Meter>(),
				AxleData = axles,
				SavedInDeclarationMode = false
			};
		}

		private static DriverData CreateDriverData()
		{
			return new DriverData {
				AccelerationCurve = AccelerationCurveReader.ReadFromFile(AccelerationFile),
				LookAheadCoasting = new DriverData.LACData {
					Enabled = false,
					//Deceleration = -0.5.SI<MeterPerSquareSecond>()
				},
				OverSpeedEcoRoll = new DriverData.OverSpeedEcoRollData {
					Mode = DriverMode.Off
				},
				StartStop = new VectoRunData.StartStopData {
					Enabled = false
				}
			};
		}

		// ========================

		protected virtual IDriver AddComponent(IDrivingCycle prev, IDriver next)
		{
			prev.InPort().Connect(next.OutPort());
			return next;
		}

		protected virtual IVehicle AddComponent(IDriver prev, IVehicle next)
		{
			prev.InPort().Connect(next.OutPort());
			return next;
		}

		protected virtual IWheels AddComponent(IFvInProvider prev, IWheels next)
		{
			prev.InPort().Connect(next.OutPort());
			return next;
		}

		protected virtual ITnOutProvider AddComponent(IWheels prev, ITnOutProvider next)
		{
			prev.InPort().Connect(next.OutPort());
			return next;
		}

		protected virtual IPowerTrainComponent AddComponent(IPowerTrainComponent prev, IPowerTrainComponent next)
		{
			prev.InPort().Connect(next.OutPort());
			return next;
		}

		protected virtual void AddComponent(IPowerTrainComponent prev, ITnOutProvider next)
		{
			prev.InPort().Connect(next.OutPort());
		}
	}
}