/*
* Copyright 2015 European Union
*
* Licensed under the EUPL (the "Licence");
* You may not use this work except in compliance with the Licence.
* You may obtain a copy of the Licence at:
*
* http://ec.europa.eu/idabc/eupl
*
* Unless required by applicable law or agreed to in writing, software 
* distributed under the Licence is distributed on an "AS IS" basis,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the Licence for the specific language governing permissions and 
* limitations under the Licence.
*/

using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TUGraz.VectoCore.Exceptions;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.Tests.Utils;

namespace TUGraz.VectoCore.Tests.Models.Simulation
{
	[TestClass]
	public class LossMapRangeValidationTest
	{
		public const string ShiftPolygonFile = @"TestData\Components\ShiftPolygons.vgbs";
		public const string AccelerationFile = @"TestData\Components\Truck.vacc";
		public const string EngineFile = @"TestData\Components\40t_Long_Haul_Truck.veng";
		public const string AxleGearLossMap = @"TestData\Components\Axle 40t Truck.vtlm";
		public const string GearboxIndirectLoss = @"TestData\Components\Indirect Gear.vtlm";
		public const string GearboxDirectLoss = @"TestData\Components\Direct Gear.vtlm";
		public const string GearboxInsufficient = @"TestData\Components\Insufficient Gear.vtlm";
		public const string GearboxShiftPolygonFile = @"TestData\Components\ShiftPolygons.vgbs";
		public const string GearboxFullLoadCurveFile = @"TestData\Components\Gearbox.vfld";

		[TestMethod]
		public void LossMapValid()
		{
			var gearboxData = CreateGearboxData(GearboxDirectLoss, GearboxIndirectLoss);
			var engineData = MockSimulationDataFactory.CreateEngineDataFromFile(EngineFile);
			var axleGearData = CreateAxleGearData(AxleGearLossMap);
			SimulatorFactory.CheckLossMapRangeForFullLoadCurves(gearboxData, engineData, axleGearData);
		}

		[TestMethod]
		public void LossMapInvalidAxle()
		{
			var gearboxData = CreateGearboxData(GearboxDirectLoss, GearboxIndirectLoss);
			var engineData = MockSimulationDataFactory.CreateEngineDataFromFile(EngineFile);
			var axleGearData = CreateAxleGearData(GearboxIndirectLoss);
			AssertHelper.Exception<VectoException>(() => {
				SimulatorFactory.CheckLossMapRangeForFullLoadCurves(gearboxData, engineData, axleGearData);
			});
		}

		[TestMethod]
		public void LossMapInvalid()
		{
			var gearboxData = CreateGearboxData(GearboxInsufficient, GearboxInsufficient);
			var engineData = MockSimulationDataFactory.CreateEngineDataFromFile(EngineFile);
			var axleGearData = CreateAxleGearData(AxleGearLossMap);
			SimulatorFactory.CheckLossMapRangeForFullLoadCurves(gearboxData, engineData, axleGearData);
		}


		private static GearboxData CreateGearboxData(string directlossMap, string indirectLossMap)
		{
			var ratios = new[] { 14.93, 11.64, 9.02, 7.04, 5.64, 4.4, 3.39, 2.65, 2.05, 1.6, 1.28, 1.0 };
			return new GearboxData {
				Gears = ratios.Select((ratio, i) =>
					Tuple.Create((uint)i,
						new GearData {
							FullLoadCurve = FullLoadCurve.ReadFromFile(GearboxFullLoadCurveFile),
							LossMap = TransmissionLossMap.ReadFromFile(ratio != 1.0 ? directlossMap : indirectLossMap, ratio,
								string.Format("Gear {0}", i)),
							Ratio = ratio,
							ShiftPolygon = ShiftPolygon.ReadFromFile(ShiftPolygonFile)
						}))
					.ToDictionary(k => k.Item1 + 1, v => v.Item2)
			};
		}

		private static AxleGearData CreateAxleGearData(string lossMap)
		{
			const double ratio = 2.59;
			return new AxleGearData {
				Ratio = ratio,
				LossMap = TransmissionLossMap.ReadFromFile(lossMap, ratio, "AxleGear")
			};
		}
	}
}