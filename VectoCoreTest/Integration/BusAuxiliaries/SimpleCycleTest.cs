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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NLog;
using TUGraz.VectoCore.Tests.Utils;

namespace TUGraz.VectoCore.Tests.Integration.BusAuxiliaries
{
	[TestClass]
	public class SimpleCycleTest
	{
		[TestInitialize]
		public void DisableLogging()
		{
			LogManager.DisableLogging();
			//GraphWriter.Disable();
		}

		#region Accelerate

		[TestMethod]
		public void Coach_Accelerate_20_60_level()
		{
			var cycle = SimpleDrivingCycles.CreateCycleData(SimpleDrivingCycles.CycleAccelerate_20_60_Level);
			var run = CoachAdvancedAuxPowertrain.CreateEngineeringRun(cycle, "Coach_AAUX_Accelerate_20_60_level.vmod");

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);

			GraphWriter.Write("Coach_AAUX_Accelerate_20_60_level.vmod",
				@"..\..\TestData\Integration\BusAuxiliaries\Vecto2.0\24t Coach_AAUX_Cycle_Accelerate_20_60_level.vmod");
		}

		[TestMethod]
		public void Coach_Accelerate_20_60_uphill_5()
		{
			var cycle = SimpleDrivingCycles.CreateCycleData(SimpleDrivingCycles.CycleAccelerate_20_60_uphilll_5);
			var run = CoachAdvancedAuxPowertrain.CreateEngineeringRun(cycle,
				"Coach_AAUX_Accelerate_20_60_uphill_5.vmod");

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);

			GraphWriter.Write("Coach_AAUX_Accelerate_20_60_uphill_5.vmod",
				@"..\..\TestData\Integration\BusAuxiliaries\Vecto2.0\24t Coach_AAUX_Cycle_Accelerate_20_60_uphill_5.vmod");
		}


		[TestMethod]
		public void Coach_Accelerate_20_60_downhill_5()
		{
			var cycle = SimpleDrivingCycles.CreateCycleData(SimpleDrivingCycles.CycleAccelerate_20_60_downhill_5);
			var run = CoachAdvancedAuxPowertrain.CreateEngineeringRun(cycle,
				"Coach_AAUX_Accelerate_20_60_downhill_5.vmod");

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);

			GraphWriter.Write("Coach_AAUX_Accelerate_20_60_downhill_5.vmod",
				@"..\..\TestData\Integration\BusAuxiliaries\Vecto2.0\24t Coach_AAUX_Cycle_Accelerate_20_60_downhill_5.vmod");
		}


		[TestMethod]
		public void Coach_Accelerate_20_60_uphill_25()
		{
			var cycle = SimpleDrivingCycles.CreateCycleData(SimpleDrivingCycles.CycleAccelerate_20_60_uphill_25);
			var run = CoachAdvancedAuxPowertrain.CreateEngineeringRun(cycle,
				"Coach_AAUX_Accelerate_20_60_uphill_25.vmod");

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);

			GraphWriter.Write("Coach_AAUX_Accelerate_20_60_uphill_25.vmod",
				@"..\..\TestData\Integration\BusAuxiliaries\Vecto2.0\24t Coach_AAUX_Cycle_Accelerate_20_60_uphill_25.vmod");
		}

		[TestMethod]
		public void Coach_Accelerate_20_60_downhill_25()
		{
			var cycle = SimpleDrivingCycles.CreateCycleData(SimpleDrivingCycles.CycleAccelerate_20_60_downhill_25);
			var run = CoachAdvancedAuxPowertrain.CreateEngineeringRun(cycle,
				"Coach_AAUX_Accelerate_20_60_downhill_25.vmod");

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);

			GraphWriter.Write("Coach_AAUX_Accelerate_20_60_downhill_25.vmod",
				@"..\..\TestData\Integration\BusAuxiliaries\Vecto2.0\24t Coach_AAUX_Cycle_Accelerate_20_60_downhill_25.vmod");
		}

		[TestMethod]
		public void Coach_Accelerate_20_60_uphill_15()
		{
			var cycle = SimpleDrivingCycles.CreateCycleData(SimpleDrivingCycles.CycleAccelerate_20_60_uphill_15);
			var run = CoachAdvancedAuxPowertrain.CreateEngineeringRun(cycle,
				"Coach_AAUX_Accelerate_20_60_uphill_15.vmod");

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);

			GraphWriter.Write("Coach_AAUX_Accelerate_20_60_uphill_15.vmod",
				@"..\..\TestData\Integration\BusAuxiliaries\Vecto2.0\24t Coach_AAUX_Cycle_Accelerate_20_60_uphill_15.vmod");
		}

		[TestMethod]
		public void Coach_Accelerate_20_60_downhill_15()
		{
			var cycle = SimpleDrivingCycles.CreateCycleData(SimpleDrivingCycles.CycleAccelerate_20_60_downhill_15);
			var run = CoachAdvancedAuxPowertrain.CreateEngineeringRun(cycle,
				"Coach_AAUX_Accelerate_20_60_downhill_15.vmod");

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);

			GraphWriter.Write("Coach_AAUX_Accelerate_20_60_downhill_15.vmod",
				@"..\..\TestData\Integration\BusAuxiliaries\Vecto2.0\24t Coach_AAUX_Cycle_Accelerate_20_60_downhill_15.vmod");
		}

		[TestMethod]
		public void Coach_Accelerate_0_85_level()
		{
			var cycle = SimpleDrivingCycles.CreateCycleData(SimpleDrivingCycles.CycleAccelerate_0_85_level);
			var run = CoachAdvancedAuxPowertrain.CreateEngineeringRun(cycle, "Coach_AAUX_Accelerate_0_85_level.vmod");

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);

			GraphWriter.Write("Coach_AAUX_Accelerate_0_85_level.vmod",
				@"..\..\TestData\Integration\BusAuxiliaries\Vecto2.0\24t Coach_AAUX_Cycle_Accelerate_0_85_level.vmod");
		}

		[TestMethod]
		public void Coach_Accelerate_0_85_uphill_1()
		{
			var cycle = SimpleDrivingCycles.CreateCycleData(SimpleDrivingCycles.CycleAccelerate_0_85_uphill_1);
			var run = CoachAdvancedAuxPowertrain.CreateEngineeringRun(cycle, "Coach_AAUX_Accelerate_0_85_uphill_1.vmod");

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);

			GraphWriter.Write("Coach_AAUX_Accelerate_0_85_uphill_1.vmod",
				@"..\..\TestData\Integration\BusAuxiliaries\Vecto2.0\24t Coach_AAUX_Cycle_Accelerate_0_85_uphill_1.vmod");
		}

		[TestMethod]
		public void Coach_Accelerate_0_85_uphill_2()
		{
			var cycle = SimpleDrivingCycles.CreateCycleData(SimpleDrivingCycles.CycleAccelerate_0_85_uphill_2);
			var run = CoachAdvancedAuxPowertrain.CreateEngineeringRun(cycle, "Coach_AAUX_Accelerate_0_85_uphill_2.vmod");

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);

			GraphWriter.Write("Coach_AAUX_Accelerate_0_85_uphill_2.vmod",
				@"..\..\TestData\Integration\BusAuxiliaries\Vecto2.0\24t Coach_AAUX_Cycle_Accelerate_0_85_uphill_2.vmod");
		}

		[TestMethod]
		public void Coach_Accelerate_0_85_uphill_5()
		{
			var cycle = SimpleDrivingCycles.CreateCycleData(SimpleDrivingCycles.CycleAccelerate_0_85_uphill_5);
			var run = CoachAdvancedAuxPowertrain.CreateEngineeringRun(cycle, "Coach_AAUX_Accelerate_0_85_uphill_5.vmod");

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);

			GraphWriter.Write("Coach_AAUX_Accelerate_0_85_uphill_5.vmod",
				@"..\..\TestData\Integration\BusAuxiliaries\Vecto2.0\24t Coach_AAUX_Cycle_Accelerate_0_85_uphill_5.vmod");
		}

		[TestMethod]
		public void Coach_Accelerate_0_85_downhill_5()
		{
			var cycle = SimpleDrivingCycles.CreateCycleData(SimpleDrivingCycles.CycleAccelerate_0_85_downhill_5);
			var run = CoachAdvancedAuxPowertrain.CreateEngineeringRun(cycle,
				"Coach_AAUX_Accelerate_0_85_downhill_5.vmod");

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);

			GraphWriter.Write("Coach_AAUX_Accelerate_0_85_downhill_5.vmod",
				@"..\..\TestData\Integration\BusAuxiliaries\Vecto2.0\24t Coach_AAUX_Cycle_Accelerate_0_85_downhill_5.vmod");
		}

		[TestMethod]
		public void Coach_Accelerate_0_85_uphill_25()
		{
			var cycle = SimpleDrivingCycles.CreateCycleData(SimpleDrivingCycles.CycleAccelerate_0_85_uphill_25);
			var run = CoachAdvancedAuxPowertrain.CreateEngineeringRun(cycle,
				"Coach_AAUX_Accelerate_0_85_uphill_25.vmod");

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);

			GraphWriter.Write("Coach_AAUX_Accelerate_0_85_uphill_25.vmod",
				@"..\..\TestData\Integration\BusAuxiliaries\Vecto2.0\24t Coach_AAUX_Cycle_Accelerate_0_85_uphill_25.vmod");
		}

		[TestMethod]
		public void Coach_Accelerate_0_85_downhill_25()
		{
			var cycle = SimpleDrivingCycles.CreateCycleData(SimpleDrivingCycles.CycleAccelerate_0_85_downhill_25);
			var run = CoachAdvancedAuxPowertrain.CreateEngineeringRun(cycle,
				"Coach_AAUX_Accelerate_0_85_downhill_25.vmod");

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);

			GraphWriter.Write("Coach_AAUX_Accelerate_0_85_downhill_25.vmod",
				@"..\..\TestData\Integration\BusAuxiliaries\Vecto2.0\24t Coach_AAUX_Cycle_Accelerate_0_85_downhill_25.vmod");
		}

		[TestMethod]
		public void Coach_Accelerate_0_85_uphill_10()
		{
			var cycle = SimpleDrivingCycles.CreateCycleData(SimpleDrivingCycles.CycleAccelerate_0_85_uphill_10);
			var run = CoachAdvancedAuxPowertrain.CreateEngineeringRun(cycle,
				"Coach_AAUX_Accelerate_0_85_uphill_10.vmod");

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);

			GraphWriter.Write("Coach_AAUX_Accelerate_0_85_uphill_10.vmod",
				@"..\..\TestData\Integration\BusAuxiliaries\Vecto2.0\24t Coach_AAUX_Cycle_Accelerate_0_85_uphill_10.vmod");
		}

		[TestMethod]
		public void Coach_Accelerate_0_85_downhill_15()
		{
			var cycle = SimpleDrivingCycles.CreateCycleData(SimpleDrivingCycles.CycleAccelerate_0_85_downhill_15);
			var run = CoachAdvancedAuxPowertrain.CreateEngineeringRun(cycle,
				"Coach_AAUX_Accelerate_0_85_downhill_15.vmod");

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);

			GraphWriter.Write("Coach_AAUX_Accelerate_0_85_downhill_15.vmod",
				@"..\..\TestData\Integration\BusAuxiliaries\Vecto2.0\24t Coach_AAUX_Cycle_Accelerate_0_85_downhill_15.vmod");
		}

		[TestMethod]
		public void Coach_Accelerate_stop_0_85_level()
		{
			var cycle = SimpleDrivingCycles.CreateCycleData(SimpleDrivingCycles.CycleAccelerate_stop_0_85_level);
			var run = CoachAdvancedAuxPowertrain.CreateEngineeringRun(cycle,
				"Coach_AAUX_Accelerate_stop_0_85_level.vmod");

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);

			GraphWriter.Write("Coach_AAUX_Accelerate_stop_0_85_level.vmod",
				@"..\..\TestData\Integration\BusAuxiliaries\Vecto2.0\24t Coach_AAUX_Cycle_Accelerate_stop_0_85_level.vmod");
		}


		[TestMethod]
		public void Coach_Accelerate_20_22_uphill_5()
		{
			var cycle = SimpleDrivingCycles.CreateCycleData(SimpleDrivingCycles.CycleAccelerate_20_22_uphill_5);
			var run = CoachAdvancedAuxPowertrain.CreateEngineeringRun(cycle,
				"Coach_AAUX_Accelerate_20_22_uphill_5.vmod");

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);

			GraphWriter.Write("Coach_AAUX_Accelerate_20_22_uphill_5.vmod",
				@"..\..\TestData\Integration\BusAuxiliaries\Vecto2.0\24t Coach_AAUX_Cycle_Accelerate_20_22_uphill_5.vmod");
		}

		#endregion

		[TestMethod]
		public void TestSimpleCycle()
		{
			var cycle = SimpleDrivingCycles.CreateCycleData(SimpleDrivingCycles.CycleDrive_stop_85_stop_85_level);
			var run = CoachAdvancedAuxPowertrain.CreateEngineeringRun(cycle,
				"Coach_AAux_DriverStrategy_Drive_stop_85_stop_85_level.vmod");

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);
		}
	}
}