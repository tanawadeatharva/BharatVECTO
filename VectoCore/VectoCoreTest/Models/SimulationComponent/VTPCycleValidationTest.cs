/*
* This file is part of VECTO.
*
* Copyright © 2012-2017 European Union
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
using NLog;
using NLog.Config;
using NLog.Targets;
using NUnit.Framework;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.InputData.Reader;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.Tests.Integration;
using TUGraz.VectoCore.Tests.Models.SimulationComponentData;
using TUGraz.VectoCore.Tests.Utils;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Tests.Models.SimulationComponent
{
	[TestFixture]
	public class VTPCycleValidationTest
	{
		public static List<string> LogList = new List<string>();

		const string Header = "<t> [s],<v> [km/h],<n_eng> [rpm],<n_fan> [rpm],<tq_left> [Nm],<tq_right> [Nm],<n_wh_left> [rpm],<n_wh_right> [rpm],<fc> [g/h],<gear>";


		[TestCase()]
		public void TestWheelSpeedRatioExceeds_Left()
		{
			SetupLogging();

			var wheelSpeed = 100;

			var cycleEntries = string.Format(
				@"  0   ,    0,  600, 400, 200, 200, {0}, {0}	, 100, 3
				    0.5 ,    0,  600, 400, 200, 200, {0}, {0}	, 100, 3
				    1   ,    0,  600, 400, 200, 200, {0}, {1}	, 100, 3
				    1.5 ,    0,  600, 400, 200, 200, {1}, {1}	, 100, 3
				", wheelSpeed, wheelSpeed * DeclarationData.VTPMode.WheelSpeedDifferenceFactor * 1.1);

			var container = new VehicleContainer(ExecutionMode.Declaration) {
				RunData = new VectoRunData() {
					Aux = new List<VectoRunData.AuxData>()
				}
			};
			var cycle = InputDataHelper.InputDataAsStream(Header, cycleEntries.Split('\n'));
			var cycleData = DrivingCycleDataReader.ReadFromDataTable(VectoCSVFile.ReadStream(cycle), "VTP Cycle", false);
			var vtpCycle = new VTPCycle(container, cycleData);

			vtpCycle.VerifyInputData();

			Assert.AreEqual(1, LogList.Count);
			Assert.IsTrue(LogList[0].Contains("Wheel-speed difference rel."));
		}

		[TestCase()]
		public void TestWheelSpeedRatioExceeds_Right()
		{
			SetupLogging();

			var wheelSpeed = 100;

			var cycleEntries = string.Format(
				@"  0   ,    0,  600, 400, 200, 200, {0}, {0}	, 100, 3
				    0.5 ,    0,  600, 400, 200, 200, {0}, {0}	, 100, 3
				    1   ,    0,  600, 400, 200, 200, {0}, {1}	, 100, 3
				    1.5 ,    0,  600, 400, 200, 200, {1}, {1}	, 100, 3
				", wheelSpeed, wheelSpeed * DeclarationData.VTPMode.WheelSpeedDifferenceFactor * 1.1);

			var container = new VehicleContainer(ExecutionMode.Declaration) {
				RunData = new VectoRunData() {
					Aux = new List<VectoRunData.AuxData>()
				}
			};
			var cycle = InputDataHelper.InputDataAsStream(Header, cycleEntries.Split('\n'));
			var cycleData = DrivingCycleDataReader.ReadFromDataTable(VectoCSVFile.ReadStream(cycle), "VTP Cycle", false);
			var vtpCycle = new VTPCycle(container, cycleData);

			vtpCycle.VerifyInputData();

			Assert.AreEqual(1, LogList.Count);
			Assert.IsTrue(LogList[0].Contains("Wheel-speed difference rel."));
		}

		[TestCase()]
		public void TestWheelSpeedDifferenceStandstillExceeds_Left()
		{
			SetupLogging();

			var wheelSpeed = 0.95 * DeclarationData.VTPMode.WheelSpeedZeroTolerance.AsRPM; 

			var cycleEntries = string.Format(
				@"  0   ,    0,  600, 400, 200, 200, {0}, {0}	, 100, 3
				    0.5 ,    0,  600, 400, 200, 200, {0}, {0}	, 100, 3
				    1   ,    0,  600, 400, 200, 200, {1}, {0}	, 100, 3
				    1.5 ,    0,  600, 400, 200, 200, {1}, {1}	, 100, 3
				", wheelSpeed, wheelSpeed + DeclarationData.VTPMode.MaxWheelSpeedDifferenceStandstill.AsRPM * 1.1);

			var container = new VehicleContainer(ExecutionMode.Declaration) {
				RunData = new VectoRunData() {
					Aux = new List<VectoRunData.AuxData>()
				}
			};
			var cycle = InputDataHelper.InputDataAsStream(Header, cycleEntries.Split('\n'));
			var cycleData = DrivingCycleDataReader.ReadFromDataTable(VectoCSVFile.ReadStream(cycle), "VTP Cycle", false);
			var vtpCycle = new VTPCycle(container, cycleData);
			vtpCycle.VerifyInputData();

			Assert.AreEqual(1, LogList.Count);
			Assert.IsTrue(LogList[0].Contains("Wheel-speed difference abs."));
		}

		[TestCase()]
		public void TestWheelSpeedDifferenceStandstillExceeds_Right()
		{
			SetupLogging();

			var wheelSpeed = 0.95*DeclarationData.VTPMode.WheelSpeedZeroTolerance.AsRPM;

			var cycleEntries = string.Format(
				@"  0   ,    0,  600, 400, 200, 200, {0}, {0}	, 100, 3
				    0.5 ,    0,  600, 400, 200, 200, {0}, {0}	, 100, 3
				    1   ,    0,  600, 400, 200, 200, {0}, {1}	, 100, 3
				    1.5 ,    0,  600, 400, 200, 200, {1}, {1}	, 100, 3
				", wheelSpeed, wheelSpeed + DeclarationData.VTPMode.MaxWheelSpeedDifferenceStandstill.AsRPM * 1.1);

			var container = new VehicleContainer(ExecutionMode.Declaration) {
				RunData = new VectoRunData() {
					Aux = new List<VectoRunData.AuxData>()
				}
			};
			var cycle = InputDataHelper.InputDataAsStream(Header, cycleEntries.Split('\n'));
			var cycleData = DrivingCycleDataReader.ReadFromDataTable(VectoCSVFile.ReadStream(cycle), "VTP Cycle", false);
			var vtpCycle = new VTPCycle(container, cycleData);

			vtpCycle.VerifyInputData();

			Assert.AreEqual(1, LogList.Count);
			Assert.IsTrue(LogList[0].Contains("Wheel-speed difference abs."));
		}



		[TestCase()]
		public void TestWheelTorqueRatioExceeds_Left()
		{
			SetupLogging();

			var torque = 300;

			var cycleEntries = string.Format(
				@"  0   ,    0,  600, 400, {0}, {0} , 50 , 50 , 100, 3
				    0.5 ,    0,  600, 400, {0}, {0} , 50 , 50 , 100, 3
				    1   ,    0,  600, 400, {0}, {1} , 50 , 50 , 100, 3
				    1.5 ,    0,  600, 400, {1}, {1} , 50 , 50 , 100, 3
				", torque, torque * DeclarationData.VTPMode.WheelTorqueDifferenceFactor * 1.1);

			var container = new VehicleContainer(ExecutionMode.Declaration) {
				RunData = new VectoRunData() {
					Aux = new List<VectoRunData.AuxData>()
				}
			};
			var cycle = InputDataHelper.InputDataAsStream(Header, cycleEntries.Split('\n'));
			var cycleData = DrivingCycleDataReader.ReadFromDataTable(VectoCSVFile.ReadStream(cycle), "VTP Cycle", false);
			var vtpCycle = new VTPCycle(container, cycleData);

			vtpCycle.VerifyInputData();

			Assert.AreEqual(1, LogList.Count);
			Assert.IsTrue(LogList[0].Contains("Torque difference"));
		}


		[TestCase()]
		public void TestWheelTorqueRatioExceeds_Right()
		{
			SetupLogging();

			var torque = 300;

			var cycleEntries = string.Format(
				@"  0   ,    0,  600, 400, {0}, {0} , 50 , 50 , 100, 3
				    0.5 ,    0,  600, 400, {0}, {0} , 50 , 50 , 100, 3
				    1   ,    0,  600, 400, {1}, {0} , 50 , 50 , 100, 3
				    1.5 ,    0,  600, 400, {1}, {1} , 50 , 50 , 100, 3
				", torque, torque * DeclarationData.VTPMode.WheelTorqueDifferenceFactor * 1.1);

			var container = new VehicleContainer(ExecutionMode.Declaration) {
				RunData = new VectoRunData() {
					Aux = new List<VectoRunData.AuxData>()
				}
			};
			var cycle = InputDataHelper.InputDataAsStream(Header, cycleEntries.Split('\n'));
			var cycleData = DrivingCycleDataReader.ReadFromDataTable(VectoCSVFile.ReadStream(cycle), "VTP Cycle", false);
			var vtpCycle = new VTPCycle(container, cycleData);

			vtpCycle.VerifyInputData();

			Assert.AreEqual(1, LogList.Count);
			Assert.IsTrue(LogList[0].Contains("Torque difference"));
		}

		[TestCase()]
		public void TestWheelTorqueDiffExceeds_Left()
		{
			SetupLogging();

			var torque = 30; //0.95*DeclarationData.VTPMode.WheelTorqueZeroTolerance.Value();

			var cycleEntries = string.Format(
				@"  0   ,    0,  600, 400, {0}, {0} , 50 , 50 , 100, 3
				    0.5 ,    0,  600, 400, {0}, {0} , 50 , 50 , 100, 3
				    1   ,    0,  600, 400, {0}, {1} , 50 , 50 , 100, 3
				    1.5 ,    0,  600, 400, {1}, {1} , 50 , 50 , 100, 3
				", torque, torque + DeclarationData.VTPMode.MaxWheelTorqueDifference.Value() * 1.1);

			var container = new VehicleContainer(ExecutionMode.Declaration) {
				RunData = new VectoRunData() {
					Aux = new List<VectoRunData.AuxData>()
				}
			};
			var cycle = InputDataHelper.InputDataAsStream(Header, cycleEntries.Split('\n'));
			var cycleData = DrivingCycleDataReader.ReadFromDataTable(VectoCSVFile.ReadStream(cycle), "VTP Cycle", false);
			var vtpCycle = new VTPCycle(container, cycleData);

			vtpCycle.VerifyInputData();

			Assert.AreEqual(1, LogList.Count);
			Assert.IsTrue(LogList[0].Contains("Torque difference "));
		}

		[TestCase()]
		public void TestWheelTorqueDiffExceeds_Right()
		{
			SetupLogging();

			var torque = 30; //0.95 * DeclarationData.VTPMode.WheelTorqueZeroTolerance.Value();

			var cycleEntries = string.Format(
				@"  0   ,    0,  600, 400, {0}, {0} , 50 , 50 , 100, 3
				    0.5 ,    0,  600, 400, {0}, {0} , 50 , 50 , 100, 3
				    1   ,    0,  600, 400, {1}, {0} , 50 , 50 , 100, 3
				    1.5 ,    0,  600, 400, {1}, {1} , 50 , 50 , 100, 3
				", torque, torque + DeclarationData.VTPMode.MaxWheelTorqueDifference.Value() * 1.1);

			var container = new VehicleContainer(ExecutionMode.Declaration) {
				RunData = new VectoRunData() {
					Aux = new List<VectoRunData.AuxData>()
				}
			};
			var cycle = InputDataHelper.InputDataAsStream(Header, cycleEntries.Split('\n'));
			var cycleData = DrivingCycleDataReader.ReadFromDataTable(VectoCSVFile.ReadStream(cycle), "VTP Cycle", false);
			var vtpCycle = new VTPCycle(container, cycleData);

			vtpCycle.VerifyInputData();

			Assert.AreEqual(1, LogList.Count);
			Assert.IsTrue(LogList[0].Contains("Torque difference"));
		}

		[TestCase()]
		public void TestFanSpeedTooLow()
		{
			SetupLogging();

			var fanSpeed = 1.05 * DeclarationData.VTPMode.MinFanSpeed.AsRPM;

			var cycleEntries = string.Format(
				@"  0   ,    0,  600, {0}, 300 , 290 , 50 , 50 , 100, 3
				    0.5 ,    0,  600, {0}, 300 , 290 , 50 , 50 , 100, 3
				    1   ,    0,  600, {1}, 300 , 290 , 50 , 50 , 100, 3
				    1.5 ,    0,  600, {0}, 300 , 290 , 50 , 50 , 100, 3
				", fanSpeed, fanSpeed * 0.9);

			var container = new VehicleContainer(ExecutionMode.Declaration) {
				RunData = new VectoRunData() {
					Aux = new List<VectoRunData.AuxData>() {
						new VectoRunData.AuxData() {
							ID = Constants.Auxiliaries.IDs.Fan,
							Technology = new List<string>() { "Crankshaft mounted - On/off clutch" }
						}
					}
				}
			};
			var cycle = InputDataHelper.InputDataAsStream(Header, cycleEntries.Split('\n'));
			var cycleData = DrivingCycleDataReader.ReadFromDataTable(VectoCSVFile.ReadStream(cycle), "VTP Cycle", false);
			var vtpCycle = new VTPCycle(container, cycleData);
			vtpCycle.VerifyInputData();

			Assert.AreEqual(1, LogList.Count);
			Assert.IsTrue(LogList[0].Contains("Fan speed (non-electric) exceeds range"));
		}

		[TestCase()]
		public void TestFanSpeedTooHigh()
		{
			SetupLogging();

			var fanSpeed = 0.95 * DeclarationData.VTPMode.MaxFanSpeed.AsRPM;

			var cycleEntries = string.Format(
				@"  0   ,    0,  600, {0}, 300 , 290 , 50 , 50 , 100, 3
				    0.5 ,    0,  600, {0}, 300 , 290 , 50 , 50 , 100, 3
				    1   ,    0,  600, {1}, 300 , 290 , 50 , 50 , 100, 3
				    1.5 ,    0,  600, {0}, 300 , 290 , 50 , 50 , 100, 3
				", fanSpeed, 1.1 * fanSpeed );

			var container = new VehicleContainer(ExecutionMode.Declaration) {
				RunData = new VectoRunData() {
					Aux = new List<VectoRunData.AuxData>() {
						new VectoRunData.AuxData() {
							ID = Constants.Auxiliaries.IDs.Fan,
							Technology = new List<string>() { "Crankshaft mounted - On/off clutch" }
						}
					}
				}
			};
			var cycle = InputDataHelper.InputDataAsStream(Header, cycleEntries.Split('\n'));
			var cycleData = DrivingCycleDataReader.ReadFromDataTable(VectoCSVFile.ReadStream(cycle), "VTP Cycle", false);
			var vtpCycle = new VTPCycle(container, cycleData);
			vtpCycle.VerifyInputData();

			Assert.AreEqual(1, LogList.Count);
			Assert.IsTrue(LogList[0].Contains("Fan speed (non-electric) exceeds range"));
		}


		[TestCase()]
		public void TestFanSpeedElectricLow()
		{
			SetupLogging();

			var fanSpeed = 1.05 * DeclarationData.VTPMode.MinFanSpeed.AsRPM;

			var cycleEntries = string.Format(
				@"  0   ,    0,  600, {0}, 300 , 290 , 50 , 50 , 100, 3
				    0.5 ,    0,  600, {0}, 300 , 290 , 50 , 50 , 100, 3
				    1   ,    0,  600, {1}, 300 , 290 , 50 , 50 , 100, 3
				    1.5 ,    0,  600, {0}, 300 , 290 , 50 , 50 , 100, 3
				", fanSpeed, fanSpeed * 0.9);

			var container = new VehicleContainer(ExecutionMode.Declaration) {
				RunData = new VectoRunData() {
					Aux = new List<VectoRunData.AuxData>() {
						new VectoRunData.AuxData() {
							ID = Constants.Auxiliaries.IDs.Fan,
							Technology = new List<string>() { "Electrically driven - Electronically controlled" }
						}
					}
				}
			};
			var cycle = InputDataHelper.InputDataAsStream(Header, cycleEntries.Split('\n'));
			var cycleData = DrivingCycleDataReader.ReadFromDataTable(VectoCSVFile.ReadStream(cycle), "VTP Cycle", false);
			var vtpCycle = new VTPCycle(container, cycleData);
			vtpCycle.VerifyInputData();

			Assert.AreEqual(0, LogList.Count);
		}

		[TestCase()]
		public void TestFanSpeedElectricHigh()
		{
			SetupLogging();

			var fanSpeed = 0.95 * DeclarationData.VTPMode.MaxFanSpeed.AsRPM;

			var cycleEntries = string.Format(
				@"  0   ,    0,  600, {0}, 300 , 290 , 50 , 50 , 100, 3
				    0.5 ,    0,  600, {0}, 300 , 290 , 50 , 50 , 100, 3
				    1   ,    0,  600, {1}, 300 , 290 , 50 , 50 , 100, 3
				    1.5 ,    0,  600, {0}, 300 , 290 , 50 , 50 , 100, 3
				", fanSpeed, 1.1 * fanSpeed);

			var container = new VehicleContainer(ExecutionMode.Declaration) {
				RunData = new VectoRunData() {
					Aux = new List<VectoRunData.AuxData>() {
						new VectoRunData.AuxData() {
							ID = Constants.Auxiliaries.IDs.Fan,
							Technology = new List<string>() { "Electrically driven - Electronically controlled" }
						}
					}
				}
			};
			var cycle = InputDataHelper.InputDataAsStream(Header, cycleEntries.Split('\n'));
			var cycleData = DrivingCycleDataReader.ReadFromDataTable(VectoCSVFile.ReadStream(cycle), "VTP Cycle", false);
			var vtpCycle = new VTPCycle(container, cycleData);
			vtpCycle.VerifyInputData();

			Assert.AreEqual(0, LogList.Count);
			
		}

		[TestCase()]
		public void TestFuelConsumptionTooLow()
		{
			SetupLogging();

			// Approach: calculate min. FC for a certain FC in g/kWh (cycle demands ~10kW constant, i.e., 500NM @ 100rpm at both wheels)
			//           construct cycle with slightly decreasing FC so that FC falls below threshold 

			var fcLimit = (DeclarationData.VTPMode.LowerFCThreshold * (2 * 500.SI<NewtonMeter>() * 100.RPMtoRad())).Cast<KilogramPerSecond>();
			
			var cycleEntries = "";
			for (var i = 0; i < 2000; i++)
				cycleEntries += string.Format("  {0} ,    0,  600, 400, 500 , 500 , 100 , 100 , {1}, 3 \n", i / 2.0, (fcLimit * 1.01 * (1 - i/100000.0)).ConvertToGrammPerHour().Value);

			var container = new VehicleContainer(ExecutionMode.Declaration) {
				RunData = new VectoRunData() {
					Aux = new List<VectoRunData.AuxData>()
				}
			};
			var cycle = InputDataHelper.InputDataAsStream(Header, cycleEntries.Split('\n'));
			var cycleData = DrivingCycleDataReader.ReadFromDataTable(VectoCSVFile.ReadStream(cycle), "VTP Cycle", false);
			var vtpCycle = new VTPCycle(container, cycleData);
			vtpCycle.PrepareCycleData();
			vtpCycle.VerifyInputData();

			Assert.Greater(LogList.Count, 1);
			Assert.IsTrue(LogList.Any(x => x.StartsWith("Fuel consumption for the previous 10 [min] below threshold")));

		}

		[TestCase()]
		public void TestFuelConsumptionLowOK()
		{
			SetupLogging();

			// Approach: calculate min. FC for a certain FC in g/kWh (cycle demands ~10kW constant, i.e., 500NM @ 100rpm at both wheels)
			//           construct cycle with constant FC slightly above min. FC 

			var fcLimit = (DeclarationData.VTPMode.LowerFCThreshold * (2 * 500.SI<NewtonMeter>() * 100.RPMtoRad())).Cast<KilogramPerSecond>();

			var cycleEntries = "";
			for (var i = 0; i < 2000; i++)
				cycleEntries += string.Format("  {0} ,    0,  600, 400, 500 , 500 , 100 , 100 , {1}, 3 \n", i / 2.0, (fcLimit * 1.0001).ConvertToGrammPerHour().Value);

			var container = new VehicleContainer(ExecutionMode.Declaration) {
				RunData = new VectoRunData() {
					Aux = new List<VectoRunData.AuxData>()
				}
			};
			var cycle = InputDataHelper.InputDataAsStream(Header, cycleEntries.Split('\n'));
			var cycleData = DrivingCycleDataReader.ReadFromDataTable(VectoCSVFile.ReadStream(cycle), "VTP Cycle", false);
			var vtpCycle = new VTPCycle(container, cycleData);
			vtpCycle.PrepareCycleData();
			vtpCycle.VerifyInputData();

			Assert.AreEqual(0, LogList.Count);

		}

		[TestCase()]
		public void TestFuelConsumptionTooHigh()
		{
			SetupLogging();

			// Approach: calculate max. FC for a certain FC in g/kWh (cycle demands ~10kW constant, i.e., 500NM @ 100rpm at both wheels)
			//           construct cycle with slightly decreasing FC so that FC falls below threshold 

			var fcLimit = (DeclarationData.VTPMode.UpperFCThreshold * (2 * 500.SI<NewtonMeter>() * 100.RPMtoRad())).Cast<KilogramPerSecond>();

			var cycleEntries = "";
			for (var i = 0; i < 2000; i++)
				cycleEntries += string.Format("  {0} ,    0,  600, 400, 500 , 500 , 100 , 100 , {1}, 3 \n", i / 2.0, (fcLimit * 0.99 * (1 + i / 100000.0)).ConvertToGrammPerHour().Value);

			var container = new VehicleContainer(ExecutionMode.Declaration) {
				RunData = new VectoRunData() {
					Aux = new List<VectoRunData.AuxData>()
				}
			};
			var cycle = InputDataHelper.InputDataAsStream(Header, cycleEntries.Split('\n'));
			var cycleData = DrivingCycleDataReader.ReadFromDataTable(VectoCSVFile.ReadStream(cycle), "VTP Cycle", false);
			var vtpCycle = new VTPCycle(container, cycleData);
			vtpCycle.PrepareCycleData();
			vtpCycle.VerifyInputData();

			Assert.Greater(LogList.Count, 1);
			Assert.IsTrue(LogList.Any(x => x.StartsWith("Fuel consumption for the previous 10 [min] above threshold")));
		}

		[TestCase()]
		public void TestFuelConsumptionHighOK()
		{
			SetupLogging();

			// Approach: calculate max. FC for a certain FC in g/kWh (cycle demands ~10kW constant, i.e., 500NM @ 100rpm at both wheels)
			//           construct cycle with constant FC slightly below max FC 

			var fcLimit = (DeclarationData.VTPMode.UpperFCThreshold * (2 * 500.SI<NewtonMeter>() * 100.RPMtoRad())).Cast<KilogramPerSecond>();

			var cycleEntries = "";
			for (var i = 0; i < 2000; i++)
				cycleEntries += string.Format("  {0} ,    0,  600, 400, 500 , 500 , 100 , 100 , {1}, 3 \n", i / 2.0, (fcLimit * 0.9999).ConvertToGrammPerHour().Value);

			var container = new VehicleContainer(ExecutionMode.Declaration) {
				RunData = new VectoRunData() {
					Aux = new List<VectoRunData.AuxData>()
				}
			};
			var cycle = InputDataHelper.InputDataAsStream(Header, cycleEntries.Split('\n'));
			var cycleData = DrivingCycleDataReader.ReadFromDataTable(VectoCSVFile.ReadStream(cycle), "VTP Cycle", false);
			var vtpCycle = new VTPCycle(container, cycleData);
			vtpCycle.PrepareCycleData();
			vtpCycle.VerifyInputData();

			Assert.AreEqual(0, LogList.Count);

		}

		private static void SetupLogging()
		{
			LogList.Clear();
			var target = new MethodCallTarget {
				ClassName = typeof(VTPCycleValidationTest).AssemblyQualifiedName,
				MethodName = "LogMethod"
			};
			target.Parameters.Add(new MethodCallParameter("${level}"));
			target.Parameters.Add(new MethodCallParameter("${message}"));
			SimpleConfigurator.ConfigureForTargetLogging(target, LogLevel.Error);
		}

		
		// ReSharper disable once UnusedMember.Global -- used by logging framework, see SetupLogging method
		public static void LogMethod(string level, string message)
		{
			LogList.Add(message);
		}
	}
}
