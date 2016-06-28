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
using System.Globalization;
using NUnit.Framework;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Tests.Utils;

namespace TUGraz.VectoCore.Tests.Integration.ShiftStrategy
{
	[TestFixture]
	public class ShiftStrategyTest
	{
		[TestFixtureSetUp]
		public void DisableLogging()
		{
			//LogManager.DisableLogging();
#if TRACE
			GraphWriter.Enable();
#else
			GraphWriter.Disable();
#endif

			GraphWriter.Xfields = new[] { ModalResultField.dist };

			GraphWriter.Yfields = new[] {
				ModalResultField.v_act, ModalResultField.acc, ModalResultField.n_eng_avg, ModalResultField.Gear,
				ModalResultField.P_eng_out, /*ModalResultField.T_eng_fcmap, */ ModalResultField.FCMap,
			};
			GraphWriter.PlotDrivingMode = true;
			GraphWriter.Series1Label = "Vecto 3";
		}

		[Test,
		TestCase(75, 42.5, 4.5),
		TestCase(75, 42.5, 3.5),
		TestCase(75, 42.5, 2.1),
		TestCase(65, 42.5, 2.3),
		]
		public void Truck_Shifting_Test(double v1, double v2, double slope)
		{
			Assert.IsTrue(v1 > v2);

			var cycle = new[] {
				// <s>,<v>,<grad>,<stop>
				string.Format(CultureInfo.InvariantCulture, "  0,  {0}, {2},  0", v1, v2, slope),
				string.Format(CultureInfo.InvariantCulture, "1000, {1}, {2},  0", v1, v2, slope),
				string.Format(CultureInfo.InvariantCulture, "1100, {1},   0,  0", v1, v2, slope)
			};
			System.IO.Directory.CreateDirectory(string.Format(@"Shiftt_{0}_{1}", v1, v2, slope));
			var slopePrefix = "";
			if (!slope.IsEqual(0)) {
				slopePrefix = slope > 0 ? "uh_" : "dh_";
			}
			var modFile = string.Format(@"Truck_Shift_{0}_{1}_{3}{2:0.0}.vmod", v1, v2, Math.Abs(slope),
				slopePrefix);
			var cycleData = SimpleDrivingCycles.CreateCycleData(cycle);
			var run = Truck40tPowerTrain.CreateEngineeringRun(cycleData, modFile, gbxType: GearboxType.MT);

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);

			GraphWriter.Write(modFile);
		}
	}
}