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

using TUGraz.VectoCore.Utils;
using TUGraz.VectoCore.Exceptions;
using TUGraz.VectoCore.Tests.Utils;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.InputData.Reader;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.FileIO;

namespace TUGraz.VectoCore.Tests.Models.Simulation
{
	[TestClass]
	public class AuxTests
	{
		[TestMethod]
		public void AuxWriteModFileSumFile()

		{
			var fileWriter = new FileOutputWriter("AuxWriteModFileSumFile", "");
			var modData = new ModalDataContainer("AuxWriteModFileSumFile", fileWriter);
			modData.AddAuxiliary("FAN");
			modData.AddAuxiliary("PS");
			modData.AddAuxiliary("STP");
			modData.AddAuxiliary("ES");
			modData.AddAuxiliary("AC");

			var sumWriter = new SummaryDataContainer(fileWriter);
			var container = new VehicleContainer(modData,
				(writer, mass, loading) => sumWriter.Write(modData, "", "", "", null, null));
			var data = DrivingCycleDataReader.ReadFromFile(@"TestData\Cycles\LongHaul_short.vdri", CycleType.DistanceBased, false);
			var mockcycle = new MockDrivingCycle(container, data);

			var aux = new EngineAuxiliary(container);

			var hdvClass = VehicleClass.Class5;
			var mission = MissionType.LongHaul;

			aux.AddConstant("FAN",
				DeclarationData.Fan.Lookup(MissionType.LongHaul, "Hydraulic driven - Constant displacement pump"));
			aux.AddConstant("PS", DeclarationData.PneumaticSystem.Lookup(mission, hdvClass));
			aux.AddConstant("STP",
				DeclarationData.SteeringPump.Lookup(MissionType.LongHaul, hdvClass, "Variable displacement"));
			aux.AddConstant("ES", DeclarationData.ElectricSystem.Lookup(mission, null));
			aux.AddConstant("AC",
				DeclarationData.HeatingVentilationAirConditioning.Lookup(mission, hdvClass));

			var speed = 1400.RPMtoRad();
			var torque = 500.SI<NewtonMeter>();
			var t = 0.SI<Second>();
			var dt = 1.SI<Second>();

			aux.Initialize(torque, speed);
			for (var i = 0; i < 11; i++) {
				aux.PowerDemand(t, dt, torque, torque, speed);
				modData[ModalResultField.dist] = i.SI<Meter>();
				modData[ModalResultField.P_eng_out] = 0.SI<Watt>();
				modData[ModalResultField.acc] = 0.SI<MeterPerSquareSecond>();
				container.CommitSimulationStep(t, dt);
				t += dt;
			}

			container.FinishSimulation();
			sumWriter.Finish();

			var testColumns = new[] { "P_aux_FAN", "P_aux_STP", "P_aux_AC", "P_aux_ES", "P_aux_PS", "P_aux" };

			ResultFileHelper.TestModFile(@"TestData\Results\EngineOnlyCycles\40t_Long_Haul_Truck_Long_Haul_Empty Loading.vmod",
				@"AuxWriteModFileSumFile.vmod", testColumns);
			ResultFileHelper.TestSumFile(@"TestData\Results\EngineOnlyCycles\AuxWriteModFileSumFile.vsum",
				@"AuxWriteModFileSumFile.vsum");
		}

		[TestMethod]
		public void AuxConstant()
		{
			var dataWriter = new MockModalDataContainer();
			var container = new VehicleContainer(dataWriter);
			//var port = new MockTnOutPort();
			var aux = new EngineAuxiliary(container);

			var constPower = 1200.SI<Watt>();
			aux.AddConstant("CONSTANT", constPower);

			var speed = 2358.RPMtoRad();
			var torque = 500.SI<NewtonMeter>();
			var t = 0.SI<Second>();

			aux.Initialize(torque, speed);
			var auxDemand = aux.PowerDemand(t, t, torque, torque, speed);
			AssertHelper.AreRelativeEqual(constPower / speed, auxDemand);

			speed = 2358.RPMtoRad();
			torque = 1500.SI<NewtonMeter>();
			aux.Initialize(torque, speed);
			auxDemand = aux.PowerDemand(t, t, torque, torque, speed);
			AssertHelper.AreRelativeEqual(constPower / speed, auxDemand);

			speed = 1500.RPMtoRad();
			torque = 1500.SI<NewtonMeter>();
			aux.Initialize(torque, speed);
			auxDemand = aux.PowerDemand(t, t, torque, torque, speed);
			AssertHelper.AreRelativeEqual(constPower / speed, auxDemand);
		}

		[TestMethod]
		public void AuxDirect()
		{
			var dataWriter = new MockModalDataContainer();
			var container = new VehicleContainer(dataWriter);
			var data = DrivingCycleDataReader.ReadFromFile(@"TestData\Cycles\Coach time based short.vdri",
				CycleType.MeasuredSpeed, false);
			var cycle = new MockDrivingCycle(container, data);

			var aux = new EngineAuxiliary(container);

			aux.AddDirect();

			var speed = 2358.RPMtoRad();
			var torque = 500.SI<NewtonMeter>();

			var t = 0.SI<Second>();

			var expected = new[] { 6100, 3100, 2300, 4500, 6100 };
			foreach (var e in expected) {
				aux.Initialize(torque, speed);
				var auxDemand = aux.PowerDemand(t, t, torque, torque, speed);

				AssertHelper.AreRelativeEqual((e.SI<Watt>() / speed).Value(), auxDemand.Value());
				cycle.CommitSimulationStep(null);
			}
		}

		[TestMethod]
		public void AuxAllCombined()
		{
			var dataWriter = new MockModalDataContainer();
			dataWriter.AddAuxiliary("ALT1");
			dataWriter.AddAuxiliary("CONSTANT");

			var container = new VehicleContainer(dataWriter);
			var data = DrivingCycleDataReader.ReadFromFile(@"TestData\Cycles\Coach time based short.vdri",
				CycleType.MeasuredSpeed, false);
			// cycle ALT1 is set to values to equal the first few fixed points in the auxiliary file.
			// ALT1.aux file: nAuxiliary speed 2358: 0, 0.38, 0.49, 0.64, ...
			// ALT1 in cycle file: 0, 0.3724 (=0.38*0.96), 0.4802 (=0.49*0.96), 0.6272 (0.64*0.96), ...

			var cycle = new MockDrivingCycle(container, data);

			var aux = new EngineAuxiliary(container);

			var auxData = AuxiliaryData.ReadFromFile(@"TestData\Components\24t_Coach_ALT.vaux");
			// ratio = 4.078
			// efficiency_engine = 0.96
			// efficiency_supply = 0.98

			aux.AddMapping("ALT1", auxData);
			aux.AddDirect();
			var constPower = 1200.SI<Watt>();
			aux.AddConstant("CONSTANT", constPower);

			var speed = 578.22461991.RPMtoRad(); // = 2358 (nAuxiliary) * ratio
			var torque = 500.SI<NewtonMeter>();
			var t = 0.SI<Second>();
			var expected = new[] {
				1200 + 6100 + 72.9166666666667,
				// = 1000 * 0.07 (nAuxiliary=2358 and psupply=0) / 0.98 (efficiency_supply)
				1200 + 3100 + 677.083333333333,
				// = 1000 * 0.65 (nAuxiliary=2358 and psupply=0.38) / 0.98 (efficiency_supply)
				1200 + 2300 + 822.916666666667,
				// = 1000 * 0.79 (nAuxiliary=2358 and psupply=0.49) / 0.98 (efficiency_supply)
				1200 + 4500 + 1031.25, // = ...
				1200 + 6100 + 1166.66666666667,
				1200 + 6100 + 1656.25,
				1200 + 6100 + 2072.91666666667,
				1200 + 6100 + 2510.41666666667,
				1200 + 6100 + 2979.16666666667,
				1200 + 6100 + 3322.91666666667,
				1200 + 6100 + 3656.25
			};

			foreach (var e in expected) {
				aux.Initialize(torque, speed);
				var auxDemand = aux.PowerDemand(t, t, torque, torque, speed);

				AssertHelper.AreRelativeEqual((e.SI<Watt>() / speed).Value(), auxDemand.Value());

				cycle.CommitSimulationStep(null);
			}
		}

		[TestMethod]
		public void AuxMapping()
		{
			var auxId = "ALT1";
			var dataWriter = new MockModalDataContainer();
			dataWriter.AddAuxiliary(auxId);

			var container = new VehicleContainer(dataWriter);
			var data = DrivingCycleDataReader.ReadFromFile(@"TestData\Cycles\Coach time based short.vdri",
				CycleType.MeasuredSpeed, false);
			// cycle ALT1 is set to values to equal the first few fixed points in the auxiliary file.
			// ALT1.aux file: nAuxiliary speed 2358: 0, 0.38, 0.49, 0.64, ...
			// ALT1 in cycle file: 0, 0.3724 (=0.38*0.96), 0.4802 (=0.49*0.96), 0.6272 (0.64*0.96), ...

			var cycle = new MockDrivingCycle(container, data);
			var port = new MockTnOutPort();

			var aux = new EngineAuxiliary(container);

			var auxData = AuxiliaryData.ReadFromFile(@"TestData\Components\24t_Coach_ALT.vaux");
			// ratio = 4.078
			// efficiency_engine = 0.96
			// efficiency_supply = 0.98

			aux.AddMapping(auxId, auxData);

			var speed = 578.22461991.RPMtoRad(); // = 2358 (nAuxiliary) * ratio
			var torque = 500.SI<NewtonMeter>();
			var t = 0.SI<Second>();
			var expected = new[] {
				72.9166666666667,
				// = 1000 * 0.07 (pmech from aux file at nAuxiliary=2358 and psupply=0) / 0.98 (efficiency_supply)
				677.083333333333, // = 1000 * 0.65 (nAuxiliary=2358 and psupply=0.38) / 0.98
				822.916666666667, // = 1000 * 0.79 (nAuxiliary=2358 and psupply=0.49) / 0.98
				1031.25, // = ...
				1166.66666666667,
				1656.25,
				2072.91666666667,
				2510.41666666667,
				2979.16666666667,
				3322.91666666667,
				3656.25
			};

			foreach (var e in expected) {
				aux.Initialize(torque, speed);
				var auxDemand = aux.PowerDemand(t, t, torque, torque, speed);

				AssertHelper.AreRelativeEqual((e.SI<Watt>() / speed).Value(), auxDemand);

				cycle.CommitSimulationStep(null);
			}
		}

		[TestMethod]
		public void AuxColumnMissing()
		{
			var container = new VehicleContainer();
			var data = DrivingCycleDataReader.ReadFromFile(@"TestData\Cycles\Coach time based short.vdri",
				CycleType.MeasuredSpeed, false);
			var cycle = new MockDrivingCycle(container, data);

			var aux = new EngineAuxiliary(container);
			AssertHelper.Exception<VectoException>(() => aux.AddMapping("NONEXISTING_AUX", null),
				"driving cycle does not contain column for auxiliary: NONEXISTING_AUX");
		}

		[TestMethod]
		public void AuxFileMissing()
		{
			AssertHelper.Exception<VectoException>(() => AuxiliaryData.ReadFromFile(@"NOT_EXISTING_AUX_FILE.vaux"),
				"Auxiliary file not found: NOT_EXISTING_AUX_FILE.vaux");
		}

		[TestMethod]
		public void AuxReadJobFileDeclarationMode()
		{
			var fileWriter = new FileOutputWriter("AuxReadJobFileDeclarationMode", "");
			var sumData = new SummaryDataContainer(fileWriter);
			var jobContainer = new JobContainer(sumData);

			var inputData = JSONInputDataFactory.ReadJsonJob(
				@"TestData\Generic Vehicles\Declaration Mode\40t Long Haul Truck\40t_Long_Haul_Truck.vecto");
			var runsFactory = new SimulatorFactory(ExecutionMode.Declaration, inputData, fileWriter);

			jobContainer.AddRuns(runsFactory);
		}

		[TestMethod]
		public void AuxReadJobFileEngineeringMode()
		{
			var fileWriter = new FileOutputWriter("AuxReadJobFileEngineeringMode", "");
			var sumData = new SummaryDataContainer(fileWriter);
			var jobContainer = new JobContainer(sumData);

			var inputData =
				JSONInputDataFactory.ReadJsonJob(@"TestData\Generic Vehicles\Engineering Mode\24t Coach\24t Coach.vecto");
			var runsFactory = new SimulatorFactory(ExecutionMode.Engineering, inputData, fileWriter);

			jobContainer.AddRuns(runsFactory);
		}

		[TestMethod]
		public void AuxDeclarationWrongConfiguration()
		{
			var fileWriter = new FileOutputWriter("AuxReadJobFileDeclarationMode", "");
			var sumData = new SummaryDataContainer(fileWriter);
			var jobContainer = new JobContainer(sumData);

			var inputData = JSONInputDataFactory.ReadJsonJob(@"TestData\Jobs\40t_Long_Haul_Truck_wrong_AUX.vecto");
			var runsFactory = new SimulatorFactory(ExecutionMode.Declaration, inputData, fileWriter);

			AssertHelper.Exception<VectoException>(() => jobContainer.AddRuns(runsFactory));
		}
	}
}