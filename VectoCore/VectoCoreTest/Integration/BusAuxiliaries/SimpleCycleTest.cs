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
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.Tests.Utils;

namespace TUGraz.VectoCore.Tests.Integration.BusAuxiliaries
{
	[TestClass]
	public class SimpleCycleTest
	{
		[TestInitialize]
		public void Init()
		{
			LogManager.DisableLogging();
#if TRACE
			GraphWriter.Enable();
#else
			GraphWriter.Disable();
#endif
		
			GraphWriter.Xfields = new[] { ModalResultField.time, ModalResultField.dist };

			GraphWriter.Yfields = new[] {
				ModalResultField.v_act, ModalResultField.acc, ModalResultField.n_eng_avg, ModalResultField.Gear,
				ModalResultField.P_eng_out, ModalResultField.P_aux, ModalResultField.AA_TotalCycleFC_Grams
			};

			GraphWriter.Series1Label = "Vecto 3";
			GraphWriter.Series2Label = "Vecto 2.0+AUX";
		}

		#region Accelerate

		[TestMethod, TestCategory("ComparisonAAUX")]
		public void Coach_AAUX_Accelerate_20_60_level()
		{
			var cycle = SimpleDrivingCycles.CreateCycleData(SimpleDrivingCycles.CycleAccelerate_20_60_Level);
			var run = CoachAdvancedAuxPowertrain.CreateEngineeringRun(cycle, "Coach_AAUX_Accelerate_20_60_level.vmod");

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);

			GraphWriter.Write("Coach_AAUX_Accelerate_20_60_level.vmod",
				@"..\..\TestData\Integration\BusAuxiliaries\Vecto2.0\24t Coach_AAUX_Cycle_Accelerate_20_60_level.vmod");
		}

		[TestMethod, TestCategory("ComparisonAAUX")]
		public void Coach_AAUX_Accelerate_20_60_uphill_5()
		{
			var cycle = SimpleDrivingCycles.CreateCycleData(SimpleDrivingCycles.CycleAccelerate_20_60_uphilll_5);
			var run = CoachAdvancedAuxPowertrain.CreateEngineeringRun(cycle,
				"Coach_AAUX_Accelerate_20_60_uphill_5.vmod");

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);

			GraphWriter.Write("Coach_AAUX_Accelerate_20_60_uphill_5.vmod",
				@"..\..\TestData\Integration\BusAuxiliaries\Vecto2.0\24t Coach_AAUX_Cycle_Accelerate_20_60_uphill_5.vmod");
		}

		[TestMethod, TestCategory("ComparisonAAUX")]
		public void Coach_AAUX_Accelerate_20_60_downhill_5()
		{
			var cycle = SimpleDrivingCycles.CreateCycleData(SimpleDrivingCycles.CycleAccelerate_20_60_downhill_5);
			var run = CoachAdvancedAuxPowertrain.CreateEngineeringRun(cycle,
				"Coach_AAUX_Accelerate_20_60_downhill_5.vmod");

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);

			GraphWriter.Write("Coach_AAUX_Accelerate_20_60_downhill_5.vmod",
				@"..\..\TestData\Integration\BusAuxiliaries\Vecto2.0\24t Coach_AAUX_Cycle_Accelerate_20_60_downhill_5.vmod");
		}

		[TestMethod, TestCategory("ComparisonAAUX")]
		public void Coach_AAUX_Accelerate_20_60_uphill_25()
		{
			var cycle = SimpleDrivingCycles.CreateCycleData(SimpleDrivingCycles.CycleAccelerate_20_60_uphill_25);
			var run = CoachAdvancedAuxPowertrain.CreateEngineeringRun(cycle,
				"Coach_AAUX_Accelerate_20_60_uphill_25.vmod");

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);

			GraphWriter.Write("Coach_AAUX_Accelerate_20_60_uphill_25.vmod",
				@"..\..\TestData\Integration\BusAuxiliaries\Vecto2.0\24t Coach_AAUX_Cycle_Accelerate_20_60_uphill_25.vmod");
		}

		[TestMethod, TestCategory("ComparisonAAUX")]
		public void Coach_AAUX_Accelerate_20_60_downhill_25()
		{
			var cycle = SimpleDrivingCycles.CreateCycleData(SimpleDrivingCycles.CycleAccelerate_20_60_downhill_25);
			var run = CoachAdvancedAuxPowertrain.CreateEngineeringRun(cycle,
				"Coach_AAUX_Accelerate_20_60_downhill_25.vmod");

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);

			GraphWriter.Write("Coach_AAUX_Accelerate_20_60_downhill_25.vmod",
				@"..\..\TestData\Integration\BusAuxiliaries\Vecto2.0\24t Coach_AAUX_Cycle_Accelerate_20_60_downhill_25.vmod");
		}

		[TestMethod, TestCategory("ComparisonAAUX")]
		public void Coach_AAUX_Accelerate_20_60_uphill_15()
		{
			var cycle = SimpleDrivingCycles.CreateCycleData(SimpleDrivingCycles.CycleAccelerate_20_60_uphill_15);
			var run = CoachAdvancedAuxPowertrain.CreateEngineeringRun(cycle,
				"Coach_AAUX_Accelerate_20_60_uphill_15.vmod");

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);

			GraphWriter.Write("Coach_AAUX_Accelerate_20_60_uphill_15.vmod",
				@"..\..\TestData\Integration\BusAuxiliaries\Vecto2.0\24t Coach_AAUX_Cycle_Accelerate_20_60_uphill_15.vmod");
		}

		[TestMethod, TestCategory("ComparisonAAUX")]
		public void Coach_AAUX_Accelerate_20_60_downhill_15()
		{
			var cycle = SimpleDrivingCycles.CreateCycleData(SimpleDrivingCycles.CycleAccelerate_20_60_downhill_15);
			var run = CoachAdvancedAuxPowertrain.CreateEngineeringRun(cycle,
				"Coach_AAUX_Accelerate_20_60_downhill_15.vmod");

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);

			GraphWriter.Write("Coach_AAUX_Accelerate_20_60_downhill_15.vmod",
				@"..\..\TestData\Integration\BusAuxiliaries\Vecto2.0\24t Coach_AAUX_Cycle_Accelerate_20_60_downhill_15.vmod");
		}

		[TestMethod, TestCategory("ComparisonAAUX")]
		public void Coach_AAUX_Accelerate_0_85_level()
		{
			var cycle = SimpleDrivingCycles.CreateCycleData(SimpleDrivingCycles.CycleAccelerate_0_85_level);
			var run = CoachAdvancedAuxPowertrain.CreateEngineeringRun(cycle, "Coach_AAUX_Accelerate_0_85_level.vmod");

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);

			GraphWriter.Write("Coach_AAUX_Accelerate_0_85_level.vmod",
				@"..\..\TestData\Integration\BusAuxiliaries\Vecto2.0\24t Coach_AAUX_Cycle_Accelerate_0_85_level.vmod");
		}

		[TestMethod, TestCategory("ComparisonAAUX")]
		public void Coach_AAUX_Accelerate_0_85_uphill_1()
		{
			var cycle = SimpleDrivingCycles.CreateCycleData(SimpleDrivingCycles.CycleAccelerate_0_85_uphill_1);
			var run = CoachAdvancedAuxPowertrain.CreateEngineeringRun(cycle, "Coach_AAUX_Accelerate_0_85_uphill_1.vmod");

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);

			GraphWriter.Write("Coach_AAUX_Accelerate_0_85_uphill_1.vmod",
				@"..\..\TestData\Integration\BusAuxiliaries\Vecto2.0\24t Coach_AAUX_Cycle_Accelerate_0_85_uphill_1.vmod");
		}

		[TestMethod, TestCategory("ComparisonAAUX")]
		public void Coach_AAUX_Accelerate_0_85_uphill_2()
		{
			var cycle = SimpleDrivingCycles.CreateCycleData(SimpleDrivingCycles.CycleAccelerate_0_85_uphill_2);
			var run = CoachAdvancedAuxPowertrain.CreateEngineeringRun(cycle, "Coach_AAUX_Accelerate_0_85_uphill_2.vmod");

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);

			GraphWriter.Write("Coach_AAUX_Accelerate_0_85_uphill_2.vmod",
				@"..\..\TestData\Integration\BusAuxiliaries\Vecto2.0\24t Coach_AAUX_Cycle_Accelerate_0_85_uphill_2.vmod");
		}

		[TestMethod, TestCategory("ComparisonAAUX")]
		public void Coach_AAUX_Accelerate_0_85_uphill_5()
		{
			var cycle = SimpleDrivingCycles.CreateCycleData(SimpleDrivingCycles.CycleAccelerate_0_85_uphill_5);
			var run = CoachAdvancedAuxPowertrain.CreateEngineeringRun(cycle, "Coach_AAUX_Accelerate_0_85_uphill_5.vmod");

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);

			GraphWriter.Write("Coach_AAUX_Accelerate_0_85_uphill_5.vmod",
				@"..\..\TestData\Integration\BusAuxiliaries\Vecto2.0\24t Coach_AAUX_Cycle_Accelerate_0_85_uphill_5.vmod");
		}

		[TestMethod, TestCategory("ComparisonAAUX")]
		public void Coach_AAUX_Accelerate_0_85_downhill_5()
		{
			var cycle = SimpleDrivingCycles.CreateCycleData(SimpleDrivingCycles.CycleAccelerate_0_85_downhill_5);
			var run = CoachAdvancedAuxPowertrain.CreateEngineeringRun(cycle,
				"Coach_AAUX_Accelerate_0_85_downhill_5.vmod");

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);

			GraphWriter.Write("Coach_AAUX_Accelerate_0_85_downhill_5.vmod",
				@"..\..\TestData\Integration\BusAuxiliaries\Vecto2.0\24t Coach_AAUX_Cycle_Accelerate_0_85_downhill_5.vmod");
		}

		[TestMethod, TestCategory("ComparisonAAUX")]
		public void Coach_AAUX_Accelerate_0_85_uphill_25()
		{
			var cycle = SimpleDrivingCycles.CreateCycleData(SimpleDrivingCycles.CycleAccelerate_0_85_uphill_25);
			var run = CoachAdvancedAuxPowertrain.CreateEngineeringRun(cycle,
				"Coach_AAUX_Accelerate_0_85_uphill_25.vmod");

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);

			GraphWriter.Write("Coach_AAUX_Accelerate_0_85_uphill_25.vmod",
				@"..\..\TestData\Integration\BusAuxiliaries\Vecto2.0\24t Coach_AAUX_Cycle_Accelerate_0_85_uphill_25.vmod");
		}

		[TestMethod, TestCategory("ComparisonAAUX")]
		public void Coach_AAUX_Accelerate_0_85_downhill_25()
		{
			var cycle = SimpleDrivingCycles.CreateCycleData(SimpleDrivingCycles.CycleAccelerate_0_85_downhill_25);
			var run = CoachAdvancedAuxPowertrain.CreateEngineeringRun(cycle,
				"Coach_AAUX_Accelerate_0_85_downhill_25.vmod");

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);

			GraphWriter.Write("Coach_AAUX_Accelerate_0_85_downhill_25.vmod",
				@"..\..\TestData\Integration\BusAuxiliaries\Vecto2.0\24t Coach_AAUX_Cycle_Accelerate_0_85_downhill_25.vmod");
		}

		[TestMethod, TestCategory("ComparisonAAUX"),]
		public void Coach_AAUX_Accelerate_0_85_uphill_10()
		{
			var cycle = SimpleDrivingCycles.CreateCycleData(SimpleDrivingCycles.CycleAccelerate_0_85_uphill_10);
			var run = CoachAdvancedAuxPowertrain.CreateEngineeringRun(cycle,
				"Coach_AAUX_Accelerate_0_85_uphill_10.vmod");

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);

			GraphWriter.Write("Coach_AAUX_Accelerate_0_85_uphill_10.vmod",
				@"..\..\TestData\Integration\BusAuxiliaries\Vecto2.0\24t Coach_AAUX_Cycle_Accelerate_0_85_uphill_10.vmod");
		}

		[TestMethod, TestCategory("ComparisonAAUX")]
		public void Coach_AAUX_Accelerate_0_85_downhill_15()
		{
			var cycle = SimpleDrivingCycles.CreateCycleData(SimpleDrivingCycles.CycleAccelerate_0_85_downhill_15);
			var run = CoachAdvancedAuxPowertrain.CreateEngineeringRun(cycle,
				"Coach_AAUX_Accelerate_0_85_downhill_15.vmod");

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);

			GraphWriter.Write("Coach_AAUX_Accelerate_0_85_downhill_15.vmod",
				@"..\..\TestData\Integration\BusAuxiliaries\Vecto2.0\24t Coach_AAUX_Cycle_Accelerate_0_85_downhill_15.vmod");
		}

		[TestMethod, TestCategory("ComparisonAAUX")]
		public void Coach_AAUX_Accelerate_stop_0_85_level()
		{
			var cycle = SimpleDrivingCycles.CreateCycleData(SimpleDrivingCycles.CycleAccelerate_stop_0_85_level);
			var run = CoachAdvancedAuxPowertrain.CreateEngineeringRun(cycle,
				"Coach_AAUX_Accelerate_stop_0_85_level.vmod");

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);

			GraphWriter.Write("Coach_AAUX_Accelerate_stop_0_85_level.vmod",
				@"..\..\TestData\Integration\BusAuxiliaries\Vecto2.0\24t Coach_AAUX_Cycle_Accelerate_stop_0_85_level.vmod");
		}

		[TestMethod, TestCategory("ComparisonAAUX")]
		public void Coach_AAUX_Accelerate_20_22_uphill_5()
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

		#region Decelerate

		[TestMethod, TestCategory("ComparisonAAUX")]
		public void Coach_AAUX_Decelerate_22_20_downhill_5()
		{
			var cycle = SimpleDrivingCycles.CreateCycleData(SimpleDrivingCycles.CycleDecelerate_22_20_downhill_5);
			var run = CoachAdvancedAuxPowertrain.CreateEngineeringRun(cycle,
				"Coach_AAUX_Decelerate_22_20_downhill_5.vmod");

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);

			GraphWriter.Write("Coach_AAUX_Decelerate_22_20_downhill_5.vmod",
				@"..\..\TestData\Integration\BusAuxiliaries\Vecto2.0\24t Coach_AAUX_Cycle_Decelerate_22_20_downhill_5.vmod");
		}

		[TestMethod, TestCategory("ComparisonAAUX")]
		public void Coach_AAUX_Decelerate_60_20_level()
		{
			var cycle = SimpleDrivingCycles.CreateCycleData(SimpleDrivingCycles.CycleDecelerate_60_20_level);
			var run = CoachAdvancedAuxPowertrain.CreateEngineeringRun(cycle, "Coach_AAUX_Decelerate_60_20_level.vmod");

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);

			GraphWriter.Write("Coach_AAUX_Decelerate_60_20_level.vmod",
				@"..\..\TestData\Integration\BusAuxiliaries\Vecto2.0\24t Coach_AAUX_Cycle_Decelerate_60_20_level.vmod");
		}

		[TestMethod, TestCategory("ComparisonAAUX")]
		public void Coach_AAUX_Decelerate_45_0_level()
		{
			var cycle = SimpleDrivingCycles.CreateCycleData(SimpleDrivingCycles.CycleDecelerate_45_0_level);
			var run = CoachAdvancedAuxPowertrain.CreateEngineeringRun(cycle, "Coach_AAUX_Decelerate_45_0_level.vmod");

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);

			GraphWriter.Write("Coach_AAUX_Decelerate_45_0_level.vmod",
				@"..\..\TestData\Integration\BusAuxiliaries\Vecto2.0\24t Coach_AAUX_Cycle_Decelerate_45_0_level.vmod");
		}

		[TestMethod, TestCategory("ComparisonAAUX")]
		public void Coach_AAUX_Decelerate_45_0_uphill_5()
		{
			var cycle = SimpleDrivingCycles.CreateCycleData(SimpleDrivingCycles.CycleDecelerate_45_0_uphill_5);
			var run = CoachAdvancedAuxPowertrain.CreateEngineeringRun(cycle, "Coach_AAUX_Decelerate_45_0_uphill_5.vmod");

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);

			GraphWriter.Write("Coach_AAUX_Decelerate_45_0_uphill_5.vmod",
				@"..\..\TestData\Integration\BusAuxiliaries\Vecto2.0\24t Coach_AAUX_Cycle_Decelerate_45_0_uphill_5.vmod");
		}

		[TestMethod, TestCategory("ComparisonAAUX")]
		public void Coach_AAUX_Decelerate_45_0_downhill_5()
		{
			var cycle = SimpleDrivingCycles.CreateCycleData(SimpleDrivingCycles.CycleDecelerate_45_0_downhill_5);
			var run = CoachAdvancedAuxPowertrain.CreateEngineeringRun(cycle, "Coach_AAUX_Decelerate_45_0_downhill_5.vmod");

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);

			GraphWriter.Write("Coach_AAUX_Decelerate_45_0_downhill_5.vmod",
				@"..\..\TestData\Integration\BusAuxiliaries\Vecto2.0\24t Coach_AAUX_Cycle_Decelerate_45_0_downhill_5.vmod");
		}

		[TestMethod, TestCategory("ComparisonAAUX")]
		public void Coach_AAUX_Decelerate_60_20_uphill_5()
		{
			var cycle = SimpleDrivingCycles.CreateCycleData(SimpleDrivingCycles.CycleDecelerate_60_20_uphill_5);
			var run = CoachAdvancedAuxPowertrain.CreateEngineeringRun(cycle, "Coach_AAUX_Decelerate_60_20_uphill_5.vmod");

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);

			GraphWriter.Write("Coach_AAUX_Decelerate_60_20_uphill_5.vmod",
				@"..\..\TestData\Integration\BusAuxiliaries\Vecto2.0\24t Coach_AAUX_Cycle_Decelerate_60_20_uphill_5.vmod");
		}

		[TestMethod, TestCategory("ComparisonAAUX")]
		public void Coach_AAUX_Decelerate_60_20_downhill_5()
		{
			var cycle = SimpleDrivingCycles.CreateCycleData(SimpleDrivingCycles.CycleDecelerate_60_20_downhill_5);
			var run = CoachAdvancedAuxPowertrain.CreateEngineeringRun(cycle,
				"Coach_AAUX_Decelerate_60_20_downhill_5.vmod");

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);

			GraphWriter.Write("Coach_AAUX_Decelerate_60_20_downhill_5.vmod",
				@"..\..\TestData\Integration\BusAuxiliaries\Vecto2.0\24t Coach_AAUX_Cycle_Decelerate_60_20_downhill_5.vmod");
		}

		//[TestMethod, TestCategory("ComparisonAAUX"), TestCategory("LongRunning")]
		//public void Decelerate_60_20_uphill_25()
		//{
		//	var cycle = SimpleDrivingCycles.CreateCycleData(SimpleDrivingCycles.CycleDecelerate_60_20_uphill_25);
		//	var run = CoachAdvancedAuxPowertrain.CreateEngineeringRun(cycle, "Coach_AAUX_Decelerate_60_20_uphill_25.vmod");

		//	run.Run();
		//	Assert.IsTrue(run.FinishedWithoutErrors);

		//	GraphWriter.Write("Coach_AAUX_Decelerate_60_20_uphill_25.vmod",
		//		@"..\..\TestData\Integration\BusAuxiliaries\Vecto2.0\24t Coach_AAUX_Cycle_Decelerate_60_20_uphill_25.vmod");
		//}

		[TestMethod, TestCategory("ComparisonAAUX")]
		public void Coach_AAUX_Decelerate_60_20_downhill_25()
		{
			var cycle = SimpleDrivingCycles.CreateCycleData(SimpleDrivingCycles.CycleDecelerate_60_20_downhill_25);
			var run = CoachAdvancedAuxPowertrain.CreateEngineeringRun(cycle,
				"Coach_AAUX_Decelerate_60_20_downhill_25.vmod");

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);

			GraphWriter.Write("Coach_AAUX_Decelerate_60_20_downhill_25.vmod",
				@"..\..\TestData\Integration\BusAuxiliaries\Vecto2.0\24t Coach_AAUX_Cycle_Decelerate_60_20_downhill_25.vmod");
		}

		//[TestMethod, TestCategory("ComparisonAAUX"), TestCategory("LongRunning")]
		//public void Coach_AAUX_Decelerate_60_20_uphill_15()
		//{
		//	var cycle = SimpleDrivingCycles.CreateCycleData(SimpleDrivingCycles.CycleDecelerate_60_20_uphill_15);
		//	var run = CoachAdvancedAuxPowertrain.CreateEngineeringRun(cycle, "Coach_AAUX_Decelerate_60_20_uphill_15.vmod");

		//	run.Run();
		//	Assert.IsTrue(run.FinishedWithoutErrors);

		//	GraphWriter.Write("Coach_AAUX_Decelerate_60_20_uphill_15.vmod",
		//		@"..\..\TestData\Integration\BusAuxiliaries\Vecto2.0\24t Coach_AAUX_Cycle_Decelerate_60_20_uphill_15.vmod");
		//}

		[TestMethod, TestCategory("ComparisonAAUX")]
		public void Coach_AAUX_Decelerate_60_20_downhill_15()
		{
			var cycle = SimpleDrivingCycles.CreateCycleData(SimpleDrivingCycles.CycleDecelerate_60_20_downhill_15);
			var run = CoachAdvancedAuxPowertrain.CreateEngineeringRun(cycle,
				"Coach_AAUX_Decelerate_60_20_downhill_15.vmod");

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);

			GraphWriter.Write("Coach_AAUX_Decelerate_60_20_downhill_15.vmod",
				@"..\..\TestData\Integration\BusAuxiliaries\Vecto2.0\24t Coach_AAUX_Cycle_Decelerate_60_20_downhill_15.vmod");
		}

		[TestMethod, TestCategory("ComparisonAAUX")]
		public void Coach_AAUX_Decelerate_80_0_level()
		{
			var cycle = SimpleDrivingCycles.CreateCycleData(SimpleDrivingCycles.CycleDecelerate_80_0_level);
			var run = CoachAdvancedAuxPowertrain.CreateEngineeringRun(cycle, "Coach_AAUX_Decelerate_80_0_level.vmod");

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);

			GraphWriter.Write("Coach_AAUX_Decelerate_80_0_level.vmod",
				@"..\..\TestData\Integration\BusAuxiliaries\Vecto2.0\24t Coach_AAUX_Cycle_Decelerate_80_0_level.vmod");
		}

		[TestMethod, TestCategory("ComparisonAAUX")]
		public void Coach_AAUX_Decelerate_80_0_uphill_5()
		{
			var cycle = SimpleDrivingCycles.CreateCycleData(SimpleDrivingCycles.CycleDecelerate_80_0_uphill_5);
			var run = CoachAdvancedAuxPowertrain.CreateEngineeringRun(cycle, "Coach_AAUX_Decelerate_80_0_uphill_5.vmod");

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);

			GraphWriter.Write("Coach_AAUX_Decelerate_80_0_uphill_5.vmod",
				@"..\..\TestData\Integration\BusAuxiliaries\Vecto2.0\24t Coach_AAUX_Cycle_Decelerate_80_0_uphill_5.vmod");
		}

		[TestMethod, TestCategory("ComparisonAAUX")]
		public void Coach_AAUX_Decelerate_80_0_downhill_5()
		{
			var cycle = SimpleDrivingCycles.CreateCycleData(SimpleDrivingCycles.CycleDecelerate_80_0_downhill_5);
			var run = CoachAdvancedAuxPowertrain.CreateEngineeringRun(cycle, "Coach_AAUX_Decelerate_80_0_downhill_5.vmod");

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);

			GraphWriter.Write("Coach_AAUX_Decelerate_80_0_downhill_5.vmod",
				@"..\..\TestData\Integration\BusAuxiliaries\Vecto2.0\24t Coach_AAUX_Cycle_Decelerate_80_0_downhill_5.vmod");
		}

		//[TestMethod, TestCategory("ComparisonAAUX"), TestCategory("LongRunning")]
		//public void Decelerate_80_0_uphill_25()
		//{
		//	var cycle = SimpleDrivingCycles.CreateCycleData(SimpleDrivingCycles.CycleDecelerate_80_0_uphill_25);
		//	var run = CoachAdvancedAuxPowertrain.CreateEngineeringRun(cycle,
		//		"Coach_AAUX_Decelerate_80_0_steep_uphill_25.vmod");

		//	run.Run();
		//	Assert.IsTrue(run.FinishedWithoutErrors);

		//	GraphWriter.Write("Coach_AAUX_Decelerate_80_0_steep_uphill_25.vmod",
		//		@"..\..\TestData\Integration\BusAuxiliaries\Vecto2.0\24t Coach_AAUX_Cycle_Decelerate_80_0_steep_uphill_25.vmod");
		//}

		[TestMethod, TestCategory("ComparisonAAUX"), TestCategory("LongRunning")]
		public void Coach_AAUX_Decelerate_80_0_downhill_25()
		{
			var cycle = SimpleDrivingCycles.CreateCycleData(SimpleDrivingCycles.CycleDecelerate_80_0_downhill_25);
			var run = CoachAdvancedAuxPowertrain.CreateEngineeringRun(cycle,
				"Coach_AAUX_Decelerate_80_0_downhill_25.vmod");

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);

			GraphWriter.Write("Coach_AAUX_Decelerate_80_0_downhill_25.vmod",
				@"..\..\TestData\Integration\BusAuxiliaries\Vecto2.0\24t Coach_AAUX_Cycle_Decelerate_80_0_downhill_25.vmod");
		}

		//[TestMethod, TestCategory("ComparisonAAUX")]
		//public void Coach_AAUX_Decelerate_80_0_uphill_3()
		//{
		//	var cycle = SimpleDrivingCycles.CreateCycleData(SimpleDrivingCycles.CycleDecelerate_80_0_uphill_3);
		//	var run = CoachAdvancedAuxPowertrain.CreateEngineeringRun(cycle, "Coach_AAUX_Decelerate_80_0_uphill_3.vmod");

		//	run.Run();
		//	Assert.IsTrue(run.FinishedWithoutErrors);

		//	GraphWriter.Write("Coach_AAUX_Decelerate_80_0_uphill_3.vmod",
		//		@"..\..\TestData\Integration\BusAuxiliaripublic void Coach_es\Vecto2.0\24t Coach_AAUX_Cycle_Decelerate_80_0_uphill_3.vmod");
		//}

		//[TestMethod, TestCategory("ComparisonAAUX"), TestCategory("LongRunning")]
		//public void Decelerate_80_0_uphill_15()
		//{
		//	var cycle = SimpleDrivingCycles.CreateCycleData(SimpleDrivingCycles.CycleDecelerate_80_0_uphill_15);
		//	var run = CoachAdvancedAuxPowertrain.CreateEngineeringRun(cycle,
		//		"Coach_AAUX_Decelerate_80_0_steep_uphill_15.vmod");

		//	run.Run();
		//	Assert.IsTrue(run.FinishedWithoutErrors);

		//	GraphWriter.Write("Coach_AAUX_Decelerate_80_0_steep_uphill_15.vmod",
		//		@"..\..\TestData\Integration\BusAuxiliaries\Vecto2.0\24t Coach_AAUX_Cycle_Decelerate_80_0_steep_uphill_15.vmod");
		//}

		[TestMethod, TestCategory("ComparisonAAUX")]
		public void Coach_AAUX_Decelerate_80_0_downhill_15()
		{
			var cycle = SimpleDrivingCycles.CreateCycleData(SimpleDrivingCycles.CycleDecelerate_80_0_downhill_15);
			var run = CoachAdvancedAuxPowertrain.CreateEngineeringRun(cycle,
				"Coach_AAUX_Decelerate_80_0_downhill_15.vmod");

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);

			GraphWriter.Write("Coach_AAUX_Decelerate_80_0_downhill_15.vmod",
				@"..\..\TestData\Integration\BusAuxiliaries\Vecto2.0\24t Coach_AAUX_Cycle_Decelerate_80_0_downhill_15.vmod");
		}

		#endregion

		#region Drive

		[TestMethod, TestCategory("ComparisonAAUX")]
		public void Coach_AAUX_Drive_80_level()
		{
			var cycle = SimpleDrivingCycles.CreateCycleData(SimpleDrivingCycles.CycleDrive_80_level);
			var run = CoachAdvancedAuxPowertrain.CreateEngineeringRun(cycle, "Coach_AAUX_Drive_80_level.vmod");

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);

			GraphWriter.Write("Coach_AAUX_Drive_80_level.vmod",
				@"..\..\TestData\Integration\BusAuxiliaries\Vecto2.0\24t Coach_AAUX_Cycle_Drive_80_level.vmod");
		}

		[TestMethod, TestCategory("ComparisonAAUX")]
		public void Coach_AAUX_Drive_80_uphill_5()
		{
			var cycle = SimpleDrivingCycles.CreateCycleData(SimpleDrivingCycles.CycleDrive_80_uphill_5);
			var run = CoachAdvancedAuxPowertrain.CreateEngineeringRun(cycle, "Coach_AAUX_Drive_80_uphill_5.vmod");

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);

			GraphWriter.Write("Coach_AAUX_Drive_80_uphill_5.vmod",
				@"..\..\TestData\Integration\BusAuxiliaries\Vecto2.0\24t Coach_AAUX_Cycle_Drive_80_uphill_5.vmod");
		}

		[TestMethod, TestCategory("ComparisonAAUX")]
		public void Coach_AAUX_Drive_80_downhill_5()
		{
			var cycle = SimpleDrivingCycles.CreateCycleData(SimpleDrivingCycles.CycleDrive_80_downhill_5);
			var run = CoachAdvancedAuxPowertrain.CreateEngineeringRun(cycle, "Coach_AAUX_Drive_80_downhill_5.vmod");

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);

			GraphWriter.Write("Coach_AAUX_Drive_80_downhill_5.vmod",
				@"..\..\TestData\Integration\BusAuxiliaries\Vecto2.0\24t Coach_AAUX_Cycle_Drive_80_downhill_5.vmod");
		}

		[TestMethod, TestCategory("ComparisonAAUX")]
		public void Coach_AAUX_Drive_20_downhill_15()
		{
			var cycle = SimpleDrivingCycles.CreateCycleData(SimpleDrivingCycles.CycleDrive_20_downhill_15);
			var run = CoachAdvancedAuxPowertrain.CreateEngineeringRun(cycle, "Coach_AAUX_Drive_20_downhill_15.vmod");

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);

			GraphWriter.Write("Coach_AAUX_Drive_20_downhill_15.vmod",
				@"..\..\TestData\Integration\BusAuxiliaries\Vecto2.0\24t Coach_AAUX_Cycle_Drive_20_downhill_15.vmod");
		}

		[TestMethod, TestCategory("ComparisonAAUX")]
		public void Coach_AAUX_Drive_30_downhill_15()
		{
			var cycle = SimpleDrivingCycles.CreateCycleData(SimpleDrivingCycles.CycleDrive_30_downhill_15);
			var run = CoachAdvancedAuxPowertrain.CreateEngineeringRun(cycle, "Coach_AAUX_Drive_30_downhill_15.vmod");

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);

			GraphWriter.Write("Coach_AAUX_Drive_30_downhill_15.vmod",
				@"..\..\TestData\Integration\BusAuxiliaries\Vecto2.0\24t Coach_AAUX_Cycle_Drive_30_downhill_15.vmod");
		}

		[TestMethod, TestCategory("ComparisonAAUX")]
		public void Coach_AAUX_Drive_50_downhill_15()
		{
			var cycle = SimpleDrivingCycles.CreateCycleData(SimpleDrivingCycles.CycleDrive_50_downhill_15);
			var run = CoachAdvancedAuxPowertrain.CreateEngineeringRun(cycle, "Coach_AAUX_Drive_50_downhill_15.vmod");

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);

			GraphWriter.Write("Coach_AAUX_Drive_50_downhill_15.vmod",
				@"..\..\TestData\Integration\BusAuxiliaries\Vecto2.0\24t Coach_AAUX_Cycle_Drive_50_downhill_15.vmod");
		}

		[TestMethod, TestCategory("ComparisonAAUX"), TestCategory("LongRunning")]
		public void Coach_AAUX_Drive_80_uphill_25()
		{
			var cycle = SimpleDrivingCycles.CreateCycleData(SimpleDrivingCycles.CycleDrive_80_uphill_25);
			var run = CoachAdvancedAuxPowertrain.CreateEngineeringRun(cycle, "Coach_AAUX_Drive_80_uphill_25.vmod");

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);

			//			GraphWriter.Write("Coach_AAUX_Drive_80_uphill_25.vmod",
			//				@"..\..\TestData\Integration\BusAuxiliaries\Vecto2.0\24t Coach_AAUX_Cycle_Drive_80_uphill_25.vmod");
		}

		[TestMethod, TestCategory("ComparisonAAUX")]
		public void Coach_AAUX_Drive_80_downhill_15()
		{
			var cycle = SimpleDrivingCycles.CreateCycleData(SimpleDrivingCycles.CycleDrive_80_downhill_15);
			var run = CoachAdvancedAuxPowertrain.CreateEngineeringRun(cycle, "Coach_AAUX_Drive_80_downhill_15.vmod");

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);

			GraphWriter.Write("Coach_AAUX_Drive_80_downhill_15.vmod",
				@"..\..\TestData\Integration\BusAuxiliaries\Vecto2.0\24t Coach_AAUX_Cycle_Drive_80_downhill_15.vmod");
		}

		[TestMethod, TestCategory("ComparisonAAUX")]
		public void Coach_AAUX_Drive_80_uphill_15()
		{
			var cycle = SimpleDrivingCycles.CreateCycleData(SimpleDrivingCycles.CycleDrive_80_uphill_15);
			var run = CoachAdvancedAuxPowertrain.CreateEngineeringRun(cycle, "Coach_AAUX_Drive_80_uphill_15.vmod");

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);

			GraphWriter.Write("Coach_AAUX_Drive_80_uphill_15.vmod",
				@"..\..\TestData\Integration\BusAuxiliaries\Vecto2.0\24t Coach_AAUX_Cycle_Drive_80_uphill_15.vmod");
		}

		[TestMethod, TestCategory("ComparisonAAUX")]
		public void Coach_AAUX_Drive_10_level()
		{
			var cycle = SimpleDrivingCycles.CreateCycleData(SimpleDrivingCycles.CycleDrive_10_level);
			var run = CoachAdvancedAuxPowertrain.CreateEngineeringRun(cycle, "Coach_AAUX_Drive_10_level.vmod");

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);

			GraphWriter.Write("Coach_AAUX_Drive_10_level.vmod",
				@"..\..\TestData\Integration\BusAuxiliaries\Vecto2.0\24t Coach_AAUX_Cycle_Drive_10_level.vmod");
		}

		[TestMethod, TestCategory("ComparisonAAUX")]
		public void Coach_AAUX_Drive_10_uphill_5()
		{
			var cycle = SimpleDrivingCycles.CreateCycleData(new[] {
				// <s>,<v>,<grad>,<stop>
				"   0,  10, 5,    0",
				"800,  10, 5,    0",
			});
			var run = CoachAdvancedAuxPowertrain.CreateEngineeringRun(cycle, "Coach_AAUX_Drive_10_uphill_5.vmod");

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);

			GraphWriter.Write("Coach_AAUX_Drive_10_uphill_5.vmod",
				@"..\..\TestData\Integration\BusAuxiliaries\Vecto2.0\24t Coach_AAUX_Cycle_Drive_10_uphill_5.vmod");
		}

		//[TestMethod, TestCategory("ComparisonAAUX")]
		//public void Coach_AAUX_Drive_10_downhill_5()
		//{
		//	var cycle = SimpleDrivingCycles.CreateCycleData(SimpleDrivingCycles.CycleDrive_10_downhill_5);
		//	var run = CoachAdvancedAuxPowertrain.CreateEngineeringRun(cycle, "Coach_AAUX_Drive_10_downhill_5.vmod");

		//	run.Run();
		//	Assert.IsTrue(run.FinishedWithoutErrors);

		//	GraphWriter.Write("Coach_AAUX_Drive_10_downhill_5.vmod",
		//		@"..\..\TestData\Integration\BusAuxiliaries\Vecto2.0\24t Coach_AAUX_Cycle_Drive_10_downhill_5.vmod");
		//}

		[TestMethod, TestCategory("ComparisonAAUX")]
		public void Coach_AAUX_Drive_10_downhill_25()
		{
			var cycle = SimpleDrivingCycles.CreateCycleData(SimpleDrivingCycles.CycleDrive_10_downhill_25);
			var run = CoachAdvancedAuxPowertrain.CreateEngineeringRun(cycle, "Coach_AAUX_Drive_10_downhill_25.vmod");

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);

			GraphWriter.Write("Coach_AAUX_Drive_10_downhill_25.vmod",
				@"..\..\TestData\Integration\BusAuxiliaries\Vecto2.0\24t Coach_AAUX_Cycle_Drive_10_downhill_25.vmod");
		}

		[TestMethod, TestCategory("ComparisonAAUX")]
		public void Coach_AAUX_Drive_10_uphill_25()
		{
			var cycle = SimpleDrivingCycles.CreateCycleData(SimpleDrivingCycles.CycleDrive_10_uphill_25);
			var run = CoachAdvancedAuxPowertrain.CreateEngineeringRun(cycle, "Coach_AAUX_Drive_10_uphill_25.vmod");

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);

			GraphWriter.Write("Coach_AAUX_Drive_10_uphill_25.vmod",
				@"..\..\TestData\Integration\BusAuxiliaries\Vecto2.0\24t Coach_AAUX_Cycle_Drive_10_uphill_25.vmod");
		}

		[TestMethod, TestCategory("ComparisonAAUX")]
		public void Coach_AAUX_Drive_10_downhill_15()
		{
			var cycle = SimpleDrivingCycles.CreateCycleData(SimpleDrivingCycles.CycleDrive_10_downhill_15);
			var run = CoachAdvancedAuxPowertrain.CreateEngineeringRun(cycle, "Coach_AAUX_Drive_10_downhill_15.vmod");

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);

			GraphWriter.Write("Coach_AAUX_Drive_10_downhill_15.vmod",
				@"..\..\TestData\Integration\BusAuxiliaries\Vecto2.0\24t Coach_AAUX_Cycle_Drive_10_downhill_15.vmod");
		}

		[TestMethod, TestCategory("ComparisonAAUX")]
		public void Coach_AAUX_Drive_10_uphill_15()
		{
			var cycle = SimpleDrivingCycles.CreateCycleData(SimpleDrivingCycles.CycleDrive_10_uphill_15);
			var run = CoachAdvancedAuxPowertrain.CreateEngineeringRun(cycle, "Coach_AAUX_Drive_10_uphill_15.vmod");

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);

			GraphWriter.Write("Coach_AAUX_Drive_10_uphill_15.vmod",
				@"..\..\TestData\Integration\BusAuxiliaries\Vecto2.0\24t Coach_AAUX_Cycle_Drive_10_uphill_15.vmod");
		}

		#endregion

		#region Slope

		[TestMethod, TestCategory("ComparisonAAUX")]
		public void Coach_AAUX_Drive_80_Increasing_Slope()
		{
			var cycle = SimpleDrivingCycles.CreateCycleData(SimpleDrivingCycles.CycleDrive_80_Increasing_Slope);
			var run = CoachAdvancedAuxPowertrain.CreateEngineeringRun(cycle, "Coach_AAUX_Drive_80_slope_inc.vmod");

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);

			GraphWriter.Write("Coach_AAUX_Drive_80_slope_inc.vmod",
				@"..\..\TestData\Integration\BusAuxiliaries\Vecto2.0\24t Coach_AAUX_Cycle_Drive_80_Increasing_Slope.vmod");
		}

		[TestMethod, TestCategory("ComparisonAAUX")]
		public void Coach_AAUX_Drive_50_Increasing_Slope()
		{
			var cycle = SimpleDrivingCycles.CreateCycleData(SimpleDrivingCycles.CycleDrive_50_Increasing_Slope);
			var run = CoachAdvancedAuxPowertrain.CreateEngineeringRun(cycle, "Coach_AAUX_Drive_50_slope_inc.vmod");

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);

			GraphWriter.Write("Coach_AAUX_Drive_50_slope_inc.vmod",
				@"..\..\TestData\Integration\BusAuxiliaries\Vecto2.0\24t Coach_AAUX_Cycle_Drive_50_Increasing_Slope.vmod");
		}

		[TestMethod, TestCategory("ComparisonAAUX")]
		public void Coach_AAUX_Drive_30_Increasing_Slope()
		{
			var cycle = SimpleDrivingCycles.CreateCycleData(SimpleDrivingCycles.CycleDrive_30_Increasing_Slope);
			var run = CoachAdvancedAuxPowertrain.CreateEngineeringRun(cycle, "Coach_AAUX_Drive_30_slope_inc.vmod");

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);

			GraphWriter.Write("Coach_AAUX_Drive_30_slope_inc.vmod",
				@"..\..\TestData\Integration\BusAuxiliaries\Vecto2.0\24t Coach_AAUX_Cycle_Drive_30_Increasing_Slope.vmod");
		}

		[TestMethod, TestCategory("ComparisonAAUX")]
		public void Coach_AAUX_Drive_80_Decreasing_Slope()
		{
			var cycle = SimpleDrivingCycles.CreateCycleData(SimpleDrivingCycles.CycleDrive_80_Decreasing_Slope);
			var run = CoachAdvancedAuxPowertrain.CreateEngineeringRun(cycle, "Coach_AAUX_Drive_80_slope_dec.vmod");

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);

			GraphWriter.Write("Coach_AAUX_Drive_80_slope_dec.vmod",
				@"..\..\TestData\Integration\BusAuxiliaries\Vecto2.0\24t Coach_AAUX_Cycle_Drive_80_Decreasing_Slope.vmod");
		}

		[TestMethod, TestCategory("ComparisonAAUX")]
		public void Coach_AAUX_Drive_50_Decreasing_Slope()
		{
			var cycle = SimpleDrivingCycles.CreateCycleData(SimpleDrivingCycles.CycleDrive_50_Decreasing_Slope);
			var run = CoachAdvancedAuxPowertrain.CreateEngineeringRun(cycle, "Coach_AAUX_Drive_50_slope_dec.vmod");

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);

			GraphWriter.Write("Coach_AAUX_Drive_50_slope_dec.vmod",
				@"..\..\TestData\Integration\BusAuxiliaries\Vecto2.0\24t Coach_AAUX_Cycle_Drive_50_Decreasing_Slope.vmod");
		}

		[TestMethod, TestCategory("ComparisonAAUX")]
		public void Coach_AAUX_Drive_30_Decreasing_Slope()
		{
			var cycle = SimpleDrivingCycles.CreateCycleData(SimpleDrivingCycles.CycleDrive_30_Decreasing_Slope);
			var run = CoachAdvancedAuxPowertrain.CreateEngineeringRun(cycle, "Coach_AAUX_Drive_30_slope_dec.vmod");

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);

			GraphWriter.Write("Coach_AAUX_Drive_30_slope_dec.vmod",
				@"..\..\TestData\Integration\BusAuxiliaries\Vecto2.0\24t Coach_AAUX_Cycle_Drive_30_Decreasing_Slope.vmod");
		}

		[TestMethod, TestCategory("ComparisonAAUX")]
		public void Coach_AAUX_Drive_80_Dec_Increasing_Slope()
		{
			var cycle = SimpleDrivingCycles.CreateCycleData(SimpleDrivingCycles.CycleDrive_80_Dec_Increasing_Slope);
			var run = CoachAdvancedAuxPowertrain.CreateEngineeringRun(cycle, "Coach_AAUX_Drive_80_slope_dec-inc.vmod");

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);

			GraphWriter.Write("Coach_AAUX_Drive_80_slope_dec-inc.vmod",
				@"..\..\TestData\Integration\BusAuxiliaries\Vecto2.0\24t Coach_AAUX_Cycle_Drive_80_Dec_Increasing_Slope.vmod");
		}

		[TestMethod, TestCategory("ComparisonAAUX")]
		public void Coach_AAUX_Drive_50_Dec_Increasing_Slope()
		{
			var cycle = SimpleDrivingCycles.CreateCycleData(SimpleDrivingCycles.CycleDrive_50_Dec_Increasing_Slope);
			var run = CoachAdvancedAuxPowertrain.CreateEngineeringRun(cycle, "Coach_AAUX_Drive_50_slope_dec-inc.vmod");

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);

			GraphWriter.Write("Coach_AAUX_Drive_50_slope_dec-inc.vmod",
				@"..\..\TestData\Integration\BusAuxiliaries\Vecto2.0\24t Coach_AAUX_Cycle_Drive_50_Dec_Increasing_Slope.vmod");
		}

		[TestMethod, TestCategory("ComparisonAAUX")]
		public void Coach_AAUX_Drive_30_Dec_Increasing_Slope()
		{
			var cycle = SimpleDrivingCycles.CreateCycleData(SimpleDrivingCycles.CycleDrive_30_Dec_Increasing_Slope);
			var run = CoachAdvancedAuxPowertrain.CreateEngineeringRun(cycle, "Coach_AAUX_Drive_30_slope_dec-inc.vmod");

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);

			GraphWriter.Write("Coach_AAUX_Drive_30_slope_dec-inc.vmod",
				@"..\..\TestData\Integration\BusAuxiliaries\Vecto2.0\24t Coach_AAUX_Cycle_Drive_30_Dec_Increasing_Slope.vmod");
		}

		#endregion

		#region Misc

		[TestMethod, TestCategory("ComparisonAAUX")]
		public void Coach_AAUX_DecelerateWhileBrake_80_0_level()
		{
			var cycle = SimpleDrivingCycles.CreateCycleData(SimpleDrivingCycles.CycleDecelerateWhileBrake_80_0_level);
			var run = CoachAdvancedAuxPowertrain.CreateEngineeringRun(cycle,
				"Coach_AAUX_DecelerateWhileBrake_80_0_level.vmod");

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);

			GraphWriter.Write("Coach_AAUX_DecelerateWhileBrake_80_0_level.vmod",
				@"..\..\TestData\Integration\BusAuxiliaries\Vecto2.0\24t Coach_AAUX_Cycle_DecelerateWhileBrake_80_0_level.vmod");
		}

		[TestMethod, TestCategory("ComparisonAAUX")]
		public void Coach_AAUX_AccelerateWhileBrake_80_0_level()
		{
			var cycle = SimpleDrivingCycles.CreateCycleData(SimpleDrivingCycles.CycleAccelerateWhileBrake_80_0_level);
			var run = CoachAdvancedAuxPowertrain.CreateEngineeringRun(cycle,
				"Coach_AAUX_AccelerateWhileBrake_80_0_level.vmod");

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);

			GraphWriter.Write("Coach_AAUX_AccelerateWhileBrake_80_0_level.vmod",
				@"..\..\TestData\Integration\BusAuxiliaries\Vecto2.0\24t Coach_AAUX_Cycle_AccelerateWhileBrake_80_0_level.vmod");
		}

		[TestMethod, TestCategory("ComparisonAAUX")]
		public void Coach_AAUX_AccelerateAtBrake_80_0_level()
		{
			var cycle = SimpleDrivingCycles.CreateCycleData(SimpleDrivingCycles.CycleAccelerateAtBrake_80_0_level);
			var run = CoachAdvancedAuxPowertrain.CreateEngineeringRun(cycle,
				"Coach_AAUX_AccelerateAtBrake_80_0_level.vmod");

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);

			GraphWriter.Write("Coach_AAUX_AccelerateAtBrake_80_0_level.vmod",
				@"..\..\TestData\Integration\BusAuxiliaries\Vecto2.0\24t Coach_AAUX_Cycle_AccelerateAtBrake_80_0_level.vmod");
		}

		[TestMethod, TestCategory("ComparisonAAUX")]
		public void Coach_AAUX_AccelerateBeforeBrake_80_0_level()
		{
			var cycle = SimpleDrivingCycles.CreateCycleData(SimpleDrivingCycles.CycleAccelerateBeforeBrake_80_0_level);
			var run = CoachAdvancedAuxPowertrain.CreateEngineeringRun(cycle,
				"Coach_AAUX_AccelerateBeforeBrake_80_0_level.vmod");

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);

			GraphWriter.Write("Coach_AAUX_AccelerateBeforeBrake_80_0_level.vmod",
				@"..\..\TestData\Integration\BusAuxiliaries\Vecto2.0\24t Coach_AAUX_Cycle_AccelerateBeforeBrake_80_0_level.vmod");
		}

		[TestMethod, TestCategory("ComparisonAAUX")]
		public void Coach_AAUX_Drive_stop_85_stop_85_level()
		{
			var cycle = SimpleDrivingCycles.CreateCycleData(SimpleDrivingCycles.CycleDrive_stop_85_stop_85_level);
			var run = CoachAdvancedAuxPowertrain.CreateEngineeringRun(cycle,
				"Coach_AAUX_Drive_stop_85_stop_85_level.vmod");

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);

			GraphWriter.Write("Coach_AAUX_Drive_stop_85_stop_85_level.vmod",
				@"..\..\TestData\Integration\BusAuxiliaries\Vecto2.0\24t Coach_AAUX_Cycle_Drive_stop_85_stop_85_level.vmod");
		}

		#endregion

		//#region AccelerateOverspeed

		//[TestMethod, TestCategory("ComparisonAAUX")]
		//public void Coach_AAUX_Accelerate_0_85_downhill_5_overspeed()
		//{
		//	var cycle = SimpleDrivingCycles.CreateCycleData(SimpleDrivingCycles.CycleAccelerate_0_85_downhill_5);
		//	var run = CoachAdvancedAuxPowertrain.CreateEngineeringRun(cycle,
		//		"Coach_AAUX_Accelerate_0_85_downhill_5-overspeed.vmod", true);

		//	run.Run();
		//	Assert.IsTrue(run.FinishedWithoutErrors);

		//	GraphWriter.Write("Coach_AAUX_Accelerate_0_85_downhill_5-overspeed.vmod",
		//		@"..\..\TestData\Integration\BusAuxiliaries\Vecto2.0_Overspeed\24t Coach_AAUX_Cycle_Accelerate_0_85_downhill_5.vmod");
		//}

		//[TestMethod, TestCategory("ComparisonAAUX")]
		//public void Coach_AAUX_Accelerate_0_85_downhill_3_overspeed()
		//{
		//	var cycle = SimpleDrivingCycles.CreateCycleData(SimpleDrivingCycles.CycleAccelerate_0_85_downhill_3);
		//	var run = CoachAdvancedAuxPowertrain.CreateEngineeringRun(cycle,
		//		"Coach_AAUX_Accelerate_0_85_downhill_3-overspeed.vmod", true);

		//	run.Run();
		//	Assert.IsTrue(run.FinishedWithoutErrors);

		//	GraphWriter.Write("Coach_AAUX_Accelerate_0_85_downhill_3-overspeed.vmod",
		//		@"..\..\TestData\Integration\BusAuxiliaries\Vecto2.0_Overspeed\24t Coach_AAUX_Cycle_Accelerate_0_85_downhill_3.vmod");
		//}

		//[TestMethod, TestCategory("ComparisonAAUX")]
		//public void Coach_AAUX_Accelerate_0_85_downhill_1_overspeed()
		//{
		//	var cycle = SimpleDrivingCycles.CreateCycleData(SimpleDrivingCycles.CycleAccelerate_0_85_downhill_1);
		//	var run = CoachAdvancedAuxPowertrain.CreateEngineeringRun(cycle,
		//		"Coach_AAUX_Accelerate_0_85_downhill_1-overspeed.vmod", true);

		//	run.Run();
		//	Assert.IsTrue(run.FinishedWithoutErrors);

		//	GraphWriter.Write("Coach_AAUX_Accelerate_0_85_downhill_1-overspeed.vmod",
		//		@"..\..\TestData\Integration\BusAuxiliaries\Vecto2.0_Overspeed\24t Coach_AAUX_Cycle_Accelerate_0_85_downhill_1.vmod");
		//}

		//[TestMethod, TestCategory("ComparisonAAUX")]
		//public void Coach_AAUX_Accelerate_0_60_downhill_5_overspeed()
		//{
		//	var cycle = SimpleDrivingCycles.CreateCycleData(SimpleDrivingCycles.CycleAccelerate_0_60_downhill_5);
		//	var run = CoachAdvancedAuxPowertrain.CreateEngineeringRun(cycle,
		//		"Coach_AAUX_Accelerate_0_60_downhill_5-overspeed.vmod", true);

		//	run.Run();
		//	Assert.IsTrue(run.FinishedWithoutErrors);

		//	GraphWriter.Write("Coach_AAUX_Accelerate_0_60_downhill_5-overspeed.vmod",
		//		@"..\..\TestData\Integration\BusAuxiliaries\Vecto2.0_Overspeed\24t Coach_AAUX_Cycle_Accelerate_0_60_downhill_5.vmod");
		//}

		//[TestMethod, TestCategory("ComparisonAAUX")]
		//public void Coach_AAUX_Accelerate_0_60_downhill_3_overspeed()
		//{
		//	var cycle = SimpleDrivingCycles.CreateCycleData(SimpleDrivingCycles.CycleAccelerate_0_60_downhill_3);
		//	var run = CoachAdvancedAuxPowertrain.CreateEngineeringRun(cycle,
		//		"Coach_AAUX_Accelerate_0_60_downhill_3-overspeed.vmod", true);

		//	run.Run();
		//	Assert.IsTrue(run.FinishedWithoutErrors);

		//	GraphWriter.Write("Coach_AAUX_Accelerate_0_60_downhill_3-overspeed.vmod",
		//		@"..\..\TestData\Integration\BusAuxiliaries\Vecto2.0_Overspeed\24t Coach_AAUX_Cycle_Accelerate_0_60_downhill_3.vmod");
		//}

		//[TestMethod, TestCategory("ComparisonAAUX")]
		//public void Coach_AAUX_Accelerate_0_60_downhill_1_overspeed()
		//{
		//	var cycle = SimpleDrivingCycles.CreateCycleData(SimpleDrivingCycles.CycleAccelerate_0_60_downhill_1);
		//	var run = CoachAdvancedAuxPowertrain.CreateEngineeringRun(cycle,
		//		"Coach_AAUX_Accelerate_0_60_downhill_1-overspeed.vmod", true);

		//	run.Run();
		//	Assert.IsTrue(run.FinishedWithoutErrors);

		//	GraphWriter.Write("Coach_AAUX_Accelerate_0_60_downhill_1-overspeed.vmod",
		//		@"..\..\TestData\Integration\BusAuxiliaries\Vecto2.0_Overspeed\24t Coach_AAUX_Cycle_Accelerate_0_60_downhill_1.vmod");
		//}

		//[TestMethod, TestCategory("ComparisonAAUX")]
		//public void Coach_AAUX_Accelerate_0_40_downhill_5_overspeed()
		//{
		//	var cycle = SimpleDrivingCycles.CreateCycleData(SimpleDrivingCycles.CycleAccelerate_0_40_downhill_5);
		//	var run = CoachAdvancedAuxPowertrain.CreateEngineeringRun(cycle,
		//		"Coach_AAUX_Accelerate_0_40_downhill_5-overspeed.vmod", true);

		//	run.Run();
		//	Assert.IsTrue(run.FinishedWithoutErrors);

		//	GraphWriter.Write("Coach_AAUX_Accelerate_0_40_downhill_5-overspeed.vmod",
		//		@"..\..\TestData\Integration\BusAuxiliaries\Vecto2.0_Overspeed\24t Coach_AAUX_Cycle_Accelerate_0_40_downhill_5.vmod");
		//}

		//[TestMethod, TestCategory("ComparisonAAUX")]
		//public void Coach_AAUX_Accelerate_0_40_downhill_3_overspeed()
		//{
		//	var cycle = SimpleDrivingCycles.CreateCycleData(SimpleDrivingCycles.CycleAccelerate_0_40_downhill_3);
		//	var run = CoachAdvancedAuxPowertrain.CreateEngineeringRun(cycle,
		//		"Coach_AAUX_Accelerate_0_40_downhill_3-overspeed.vmod", true);

		//	run.Run();
		//	Assert.IsTrue(run.FinishedWithoutErrors);

		//	GraphWriter.Write("Coach_AAUX_Accelerate_0_40_downhill_3-overspeed.vmod",
		//		@"..\..\TestData\Integration\BusAuxiliaries\Vecto2.0_Overspeed\24t Coach_AAUX_Cycle_Accelerate_0_40_downhill_3.vmod");
		//}

		//[TestMethod, TestCategory("ComparisonAAUX")]
		//public void Coach_AAUX_Accelerate_0_40_downhill_1_overspeed()
		//{
		//	var cycle = SimpleDrivingCycles.CreateCycleData(SimpleDrivingCycles.CycleAccelerate_0_40_downhill_1);
		//	var run = CoachAdvancedAuxPowertrain.CreateEngineeringRun(cycle,
		//		"Coach_AAUX_Accelerate_0_40_downhill_1-overspeed.vmod", true);

		//	run.Run();
		//	Assert.IsTrue(run.FinishedWithoutErrors);

		//	GraphWriter.Write("Coach_AAUX_Accelerate_0_40_downhill_1-overspeed.vmod",
		//		@"..\..\TestData\Integration\BusAuxiliaries\Vecto2.0_Overspeed\24t Coach_AAUX_Cycle_Accelerate_0_40_downhill_1.vmod");
		//}

		//#endregion

		[TestMethod, TestCategory("CycleTest")]
		public void Coach_AAUX_Accelerate_0_40_downhill_1_overspeed()
		{
			var cycle = SimpleDrivingCycles.ReadDeclarationCycle("Coach");
			var run = CoachAdvancedAuxPowertrain.CreateEngineeringRun(cycle,
				"Coach_AAUX_CoachCycle.vmod", true);

			//((DistanceBasedDrivingCycle)((VehicleContainer)run.GetContainer()).Cycle).SetDriveOffDistance(123613.SI<Meter>());

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);

			//GraphWriter.Write("Coach_AAUX_Accelerate_0_40_downhill_1-overspeed.vmod",
			//	@"..\..\TestData\Integration\BusAuxiliaries\Vecto2.0_Overspeed\24t Coach_AAUX_Cycle_Accelerate_0_40_downhill_1.vmod");
		}

		//@"E:\QUAM\Workspace\VECTO_quam\Generic Vehicles\Engineering Mode\24t Coach\24t Coach_AAux.vecto"
	}
}