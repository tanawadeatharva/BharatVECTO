using System.Collections.Generic;
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
			Assert.IsTrue(LogList[0].Contains("Torque difference rel."));
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
			Assert.IsTrue(LogList[0].Contains("Torque difference rel."));
		}

		[TestCase()]
		public void TestWheelTorqueDiffExceeds_Left()
		{
			SetupLogging();

			var torque = 0.95*DeclarationData.VTPMode.WheelTorqueZeroTolerance.Value();

			var cycleEntries = string.Format(
				@"  0   ,    0,  600, 400, {0}, {0} , 50 , 50 , 100, 3
				    0.5 ,    0,  600, 400, {0}, {0} , 50 , 50 , 100, 3
				    1   ,    0,  600, 400, {0}, {1} , 50 , 50 , 100, 3
				    1.5 ,    0,  600, 400, {1}, {1} , 50 , 50 , 100, 3
				", torque, torque + DeclarationData.VTPMode.MaxWheelTorqueZeroDifference.Value() * 1.1);

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
			Assert.IsTrue(LogList[0].Contains("Torque difference abs."));
		}

		[TestCase()]
		public void TestWheelTorqueDiffExceeds_Right()
		{
			SetupLogging();

			var torque = 0.95 * DeclarationData.VTPMode.WheelTorqueZeroTolerance.Value();

			var cycleEntries = string.Format(
				@"  0   ,    0,  600, 400, {0}, {0} , 50 , 50 , 100, 3
				    0.5 ,    0,  600, 400, {0}, {0} , 50 , 50 , 100, 3
				    1   ,    0,  600, 400, {1}, {0} , 50 , 50 , 100, 3
				    1.5 ,    0,  600, 400, {1}, {1} , 50 , 50 , 100, 3
				", torque, torque + DeclarationData.VTPMode.MaxWheelTorqueZeroDifference.Value() * 1.1);

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
			Assert.IsTrue(LogList[0].Contains("Torque difference abs."));
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

			var fanSpeed = 0.95 * DeclarationData.VTPMode.MaxFanSpeed.AsRPM;

			var cycleEntries = "";
			for (var i = 0; i < 2000; i++)
				cycleEntries += string.Format("  {0} ,    0,  600, 400, 300 , 290 , 50 , 50 , {1}, 3 \n", i / 2.0, DeclarationData.VTPMode.LowerFCThreshold.ConvertToGrammPerHour() / 1.01);

			var container = new VehicleContainer(ExecutionMode.Declaration) {
				RunData = new VectoRunData() {
					Aux = new List<VectoRunData.AuxData>()
				}
			};
			var cycle = InputDataHelper.InputDataAsStream(Header, cycleEntries.Split('\n'));
			var cycleData = DrivingCycleDataReader.ReadFromDataTable(VectoCSVFile.ReadStream(cycle), "VTP Cycle", false);
			var vtpCycle = new VTPCycle(container, cycleData);
			vtpCycle.VerifyInputData();

			Assert.Greater(LogList.Count, 1);

		}

		[TestCase()]
		public void TestFuelConsumptionLowOK()
		{
			SetupLogging();

			var fanSpeed = 0.95 * DeclarationData.VTPMode.MaxFanSpeed.AsRPM;

			var cycleEntries = "";
			for (var i = 0; i < 2000; i++)
				cycleEntries += string.Format("  {0} ,    0,  600, 400, 300 , 290 , 50 , 50 , {1}, 3 \n", i / 2.0, DeclarationData.VTPMode.LowerFCThreshold.ConvertToGrammPerHour() * 1.01);

			var container = new VehicleContainer(ExecutionMode.Declaration) {
				RunData = new VectoRunData() {
					Aux = new List<VectoRunData.AuxData>()
				}
			};
			var cycle = InputDataHelper.InputDataAsStream(Header, cycleEntries.Split('\n'));
			var cycleData = DrivingCycleDataReader.ReadFromDataTable(VectoCSVFile.ReadStream(cycle), "VTP Cycle", false);
			var vtpCycle = new VTPCycle(container, cycleData);
			vtpCycle.VerifyInputData();

			Assert.AreEqual(0, LogList.Count);

		}

		[TestCase()]
		public void TestFuelConsumptionTooHigh()
		{
			SetupLogging();

			var fanSpeed = 0.95 * DeclarationData.VTPMode.MaxFanSpeed.AsRPM;

			var cycleEntries = "";
			for (var i = 0; i < 2000; i++)
				cycleEntries += string.Format("  {0} ,    0,  600, 400, 300 , 290 , 50 , 50 , {1}, 3 \n", i / 2.0, DeclarationData.VTPMode.UpperFCThreshold.ConvertToGrammPerHour() * 1.01);

			var container = new VehicleContainer(ExecutionMode.Declaration) {
				RunData = new VectoRunData() {
					Aux = new List<VectoRunData.AuxData>()
				}
			};
			var cycle = InputDataHelper.InputDataAsStream(Header, cycleEntries.Split('\n'));
			var cycleData = DrivingCycleDataReader.ReadFromDataTable(VectoCSVFile.ReadStream(cycle), "VTP Cycle", false);
			var vtpCycle = new VTPCycle(container, cycleData);
			vtpCycle.VerifyInputData();

			Assert.Greater(LogList.Count, 1);

		}

		[TestCase()]
		public void TestFuelConsumptionHighOK()
		{
			SetupLogging();

			var fanSpeed = 0.95 * DeclarationData.VTPMode.MaxFanSpeed.AsRPM;

			var cycleEntries = "";
			for (var i = 0; i < 2000; i++)
				cycleEntries += string.Format("  {0} ,    0,  600, 400, 300 , 290 , 50 , 50 , {1}, 3 \n", i / 2.0, DeclarationData.VTPMode.UpperFCThreshold.ConvertToGrammPerHour() / 1.01);

			var container = new VehicleContainer(ExecutionMode.Declaration) {
				RunData = new VectoRunData() {
					Aux = new List<VectoRunData.AuxData>()
				}
			};
			var cycle = InputDataHelper.InputDataAsStream(Header, cycleEntries.Split('\n'));
			var cycleData = DrivingCycleDataReader.ReadFromDataTable(VectoCSVFile.ReadStream(cycle), "VTP Cycle", false);
			var vtpCycle = new VTPCycle(container, cycleData);
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
