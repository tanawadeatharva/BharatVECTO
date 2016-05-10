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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NLog;
using NLog.Config;
using NLog.Targets;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Engine;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.Tests.Utils;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Tests.Models.SimulationComponentData
{
	[TestClass]
	public class FullLoadCurveTest
	{
		private const string CoachEngineFLD = @"TestData\Components\24t Coach.vfld";
		private const double Tolerance = 0.0001;

		public static List<string> LogList = new List<string>();

		[TestMethod]
		public void TestFullLoadStaticTorque()
		{
			var fldCurve = EngineFullLoadCurve.ReadFromFile(CoachEngineFLD);

			Assert.AreEqual(1180, fldCurve.FullLoadStationaryTorque(560.RPMtoRad()).Value(), Tolerance);
			Assert.AreEqual(1352, fldCurve.FullLoadStationaryTorque(2000.RPMtoRad()).Value(), Tolerance);
			Assert.AreEqual(1231, fldCurve.FullLoadStationaryTorque(580.RPMtoRad()).Value(), Tolerance);
		}

		[TestMethod]
		public void TestFullLoadEngineSpeedRated()
		{
			var fldCurve = EngineFullLoadCurve.ReadFromFile(CoachEngineFLD);
			Assert.AreEqual(181.8444, fldCurve.RatedSpeed.Value(), Tolerance);
		}

		[TestMethod]
		public void TestFullLoadStaticPower()
		{
			var fldCurve = EngineFullLoadCurve.ReadFromFile(CoachEngineFLD);

			Assert.AreEqual(69198.814183, fldCurve.FullLoadStationaryPower(560.RPMtoRad()).Value(), Tolerance);
			Assert.AreEqual(283162.218372, fldCurve.FullLoadStationaryPower(2000.RPMtoRad()).Value(), Tolerance);
			Assert.AreEqual(74767.810760, fldCurve.FullLoadStationaryPower(580.RPMtoRad()).Value(), Tolerance);
		}

		[TestMethod]
		public void TestDragLoadStaticTorque()
		{
			var fldCurve = EngineFullLoadCurve.ReadFromFile(CoachEngineFLD);

			Assert.AreEqual(-149, fldCurve.DragLoadStationaryTorque(560.RPMtoRad()).Value(), Tolerance);
			Assert.AreEqual(-301, fldCurve.DragLoadStationaryTorque(2000.RPMtoRad()).Value(), Tolerance);
			Assert.AreEqual(-148.5, fldCurve.DragLoadStationaryTorque(580.RPMtoRad()).Value(), Tolerance);
			Assert.AreEqual(-150, fldCurve.DragLoadStationaryTorque(520.RPMtoRad()).Value(), Tolerance);
			Assert.AreEqual(-339, fldCurve.DragLoadStationaryTorque(2200.RPMtoRad()).Value(), Tolerance);
		}

		[TestMethod]
		public void TestDragLoadStaticPower()
		{
			var fldCurve = EngineFullLoadCurve.ReadFromFile(CoachEngineFLD);

			Assert.AreEqual(-8737.81636, fldCurve.DragLoadStationaryPower(560.RPMtoRad()).Value(), Tolerance);
			Assert.AreEqual(-63041.29254, fldCurve.DragLoadStationaryPower(2000.RPMtoRad()).Value(), Tolerance);
			Assert.AreEqual(-9019.51251, fldCurve.DragLoadStationaryPower(580.RPMtoRad()).Value(), Tolerance);
		}

		[TestMethod]
		public void TestPT1()
		{
			var fldCurve = EngineFullLoadCurve.ReadFromFile(CoachEngineFLD);

			Assert.AreEqual(0.6, fldCurve.PT1(560.RPMtoRad()).Value(), Tolerance);
			Assert.AreEqual(0.25, fldCurve.PT1(2000.RPMtoRad()).Value(), Tolerance);
			Assert.AreEqual(0.37, fldCurve.PT1(1700.RPMtoRad()).Value(), Tolerance);
		}

		[TestMethod]
		public void TestPreferredSpeed()
		{
			var fldCurve = EngineFullLoadCurve.ReadFromFile(CoachEngineFLD);
			fldCurve.EngineData = new CombustionEngineData { IdleSpeed = 560.RPMtoRad() };
			AssertHelper.AreRelativeEqual(130.691151551712.SI<PerSecond>(), fldCurve.PreferredSpeed);
			var totalArea = fldCurve.ComputeArea(fldCurve.EngineData.IdleSpeed, fldCurve.N95hSpeed);
			Assert.AreEqual((0.51 * totalArea).Value(),
				fldCurve.ComputeArea(fldCurve.EngineData.IdleSpeed, fldCurve.PreferredSpeed).Value(), 1E-3);
			AssertHelper.AreRelativeEqual(194.515816596908.SI<PerSecond>(), fldCurve.N95hSpeed);
			AssertHelper.AreRelativeEqual(94.2463966015023.SI<PerSecond>(), fldCurve.LoSpeed);
			AssertHelper.AreRelativeEqual(219.084329211505.SI<PerSecond>(), fldCurve.HiSpeed);
			AssertHelper.AreRelativeEqual(2300.SI<NewtonMeter>(), fldCurve.MaxLoadTorque);
			AssertHelper.AreRelativeEqual(-320.SI<NewtonMeter>(), fldCurve.MaxDragTorque);
		}

		[TestMethod]
		public void TestPreferredSpeed2()
		{
			var fldData = new[] {
				"560,1180,-149,0.6",
				"600,1282,-148,0.6",
				"800,1791,-149,0.6",
				"1000,2300,-160,0.6",
				"1200,2400,-179,0.6",
				"1400,2300,-203,0.6",
				"1600,2079,-235,0.49",
				"1800,1857,-264,0.25",
				"2000,1352,-301,0.25",
				"2100,1100,-320,0.25",
			};
			var fldEntries = InputDataHelper.InputDataAsStream("n [U/min],Mfull [Nm],Mdrag [Nm],<PT1> [s] ", fldData);
			var fldCurve = EngineFullLoadCurve.Create(VectoCSVFile.ReadStream(fldEntries));
			fldCurve.EngineData = new CombustionEngineData { IdleSpeed = 560.RPMtoRad() };

			var totalArea = fldCurve.ComputeArea(fldCurve.EngineData.IdleSpeed, fldCurve.N95hSpeed);
			Assert.AreEqual((0.51 * totalArea).Value(),
				fldCurve.ComputeArea(fldCurve.EngineData.IdleSpeed, fldCurve.PreferredSpeed).Value(), 1E-3);
			//AssertHelper.AreRelativeEqual(130.691151551712.SI<PerSecond>(), fldCurve.PreferredSpeed);
		}

		/// <summary>
		///     [VECTO-78]
		/// </summary>
		[TestMethod]
		public void Test_FileRead_WrongFileFormat_InsufficientColumns()
		{
			AssertHelper.Exception<VectoException>(
				() => EngineFullLoadCurve.ReadFromFile(@"TestData\Components\FullLoadCurve insufficient columns.vfld"),
				"ERROR while reading FullLoadCurve File: Engine FullLoadCurve Data File must consist of at least 3 columns.");
		}

		/// <summary>
		/// [VECTO-78]
		/// </summary>
		[TestMethod]
		public void Test_FileRead_HeaderColumnsNotNamedCorrectly()
		{
			LogList.Clear();
			var target = new MethodCallTarget {
				ClassName = typeof(FullLoadCurveTest).AssemblyQualifiedName,
				MethodName = "LogMethod_Test_FileRead_HeaderColumnsNotNamedCorrectly"
			};
			target.Parameters.Add(new MethodCallParameter("${level}"));
			target.Parameters.Add(new MethodCallParameter("${message}"));
			SimpleConfigurator.ConfigureForTargetLogging(target, LogLevel.Warn);
			EngineFullLoadCurve.ReadFromFile(@"TestData\Components\FullLoadCurve wrong header.vfld");
			Assert.IsTrue(
				LogList.Contains(
					"FullLoadCurve: Header Line is not valid. Expected: \'engine speed, full load torque, motoring torque\', Got: \'n, Mfull, Mdrag, PT1\'. Falling back to column index."));
			LogList.Clear();
		}

		public static void LogMethod_Test_FileRead_HeaderColumnsNotNamedCorrectly(string level, string message)
		{
			LogList.Add(message);
		}

		/// <summary>
		///     [VECTO-78]
		/// </summary>
		[TestMethod]
		public void Test_FileRead_NoHeader()
		{
			var curve = EngineFullLoadCurve.ReadFromFile(@"TestData\Components\FullLoadCurve no header.vfld");
			var result = curve.FullLoadStationaryTorque(1.SI<PerSecond>());
			Assert.AreNotEqual(result.Value(), 0.0);
		}

		/// <summary>
		///     [VECTO-78]
		/// </summary>
		[TestMethod]
		public void Test_FileRead_InsufficientEntries()
		{
			AssertHelper.Exception<VectoException>(
				() => EngineFullLoadCurve.ReadFromFile(@"TestData\Components\FullLoadCurve insufficient entries.vfld"),
				"ERROR while reading FullLoadCurve File: FullLoadCurve must consist of at least two lines with numeric values (below file header)");
		}

		[TestMethod]
		public void FullLoad_LossMap_Test()
		{
			var engineData = new CombustionEngineData {
				FullLoadCurve = EngineFullLoadCurve.ReadFromFile(@"TestData\Components\12t Delivery Truck.vfld"),
				IdleSpeed = 560.RPMtoRad()
			};

			var gearboxData = new GearboxData();
			gearboxData.Gears[1] = new GearData {
				LossMap = TransmissionLossMap.ReadFromFile(@"TestData\Components\limited.vtlm", 1, "1"),
				Ratio = 1
			};

			var axleGearData = new AxleGearData() {
				AxleGear = new GearData {
					Ratio = 1,
					LossMap = TransmissionLossMap.ReadFromFile(@"TestData\Components\limited.vtlm", 1, "1"),
				}
			};

			var runData = new VectoRunData { GearboxData = gearboxData, EngineData = engineData, AxleGearData = axleGearData };
			Assert.IsFalse(runData.IsValid());
		}

		/// <summary>
		///     [VECTO-190]
		/// </summary>
		[TestMethod]
		public void TestSortingFullLoadEntries()
		{
			var fldEntries = new[] {
				"600,1282,-148,0.6			 ",
				"799.9999999,1791,-149,0.6	 ",
				"560,1180,-149,0.6			 ",
				"1000,2300,-160,0.6			 ",
				"1599.999999,2079,-235,0.49	 ",
				"1200,2300,-179,0.6			 ",
				"1800,1857,-264,0.25		 ",
				"1400,2300,-203,0.6			 ",
				"2000.000001,1352,-301,0.25	 ",
				"2100,1100,-320,0.25		 ",
			};

			var fldCurve =
				EngineFullLoadCurve.Create(
					VectoCSVFile.ReadStream(InputDataHelper.InputDataAsStream("n [U/min],Mfull [Nm],Mdrag [Nm],<PT1> [s]", fldEntries)));

			Assert.AreEqual(1180, fldCurve.FullLoadStationaryTorque(560.RPMtoRad()).Value(), Tolerance);
			Assert.AreEqual(1352, fldCurve.FullLoadStationaryTorque(2000.RPMtoRad()).Value(), Tolerance);
			Assert.AreEqual(1231, fldCurve.FullLoadStationaryTorque(580.RPMtoRad()).Value(), Tolerance);
		}
	}
}