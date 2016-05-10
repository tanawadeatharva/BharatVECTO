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
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TUGraz.VectoCore.InputData.Reader;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.Tests.Utils;
using TUGraz.VectoCore.Utils;

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
		public const string GearboxLimited = @"TestData\Components\limited.vtlm";
		public const string GearboxShiftPolygonFile = @"TestData\Components\ShiftPolygons.vgbs";
		public const string GearboxFullLoadCurveFile = @"TestData\Components\Gearbox.vfld";

		/// <summary>
		/// VECTO-173
		/// </summary>
		[TestMethod]
		public void LossMapValid()
		{
			var gearboxData = CreateGearboxData(GearboxDirectLoss, GearboxIndirectLoss);
			var engineData = MockSimulationDataFactory.CreateEngineDataFromFile(EngineFile);
			var axleGearData = CreateAxleGearData(AxleGearLossMap);

			var runData = new VectoRunData { GearboxData = gearboxData, EngineData = engineData, AxleGearData = axleGearData };

			var result = VectoRunData.ValidateRunData(runData, new ValidationContext(runData));
			Assert.IsTrue(ValidationResult.Success == result);
			Assert.IsFalse(runData.IsValid());
		}

		/// <summary>
		/// VECTO-173
		/// </summary>
		[TestMethod]
		public void LossMapInvalidAxle()
		{
			var gearboxData = CreateGearboxData(GearboxDirectLoss, GearboxIndirectLoss);
			var engineData = MockSimulationDataFactory.CreateEngineDataFromFile(EngineFile);
			var axleGearData = CreateAxleGearData(GearboxLimited);

			var runData = new VectoRunData { GearboxData = gearboxData, EngineData = engineData, AxleGearData = axleGearData };
			Assert.IsFalse(runData.IsValid());
		}

		/// <summary>
		/// VECTO-173
		/// </summary>
		[TestMethod]
		public void LossMapLimited()
		{
			var gearboxData = CreateGearboxData(GearboxLimited, GearboxLimited);
			var engineData = MockSimulationDataFactory.CreateEngineDataFromFile(EngineFile);
			var axleGearData = CreateAxleGearData(AxleGearLossMap);
			var runData = new VectoRunData { GearboxData = gearboxData, EngineData = engineData, AxleGearData = axleGearData };
			var result = VectoRunData.ValidateRunData(runData, new ValidationContext(runData));
			Assert.IsFalse(ValidationResult.Success == result);
		}

		/// <summary>
		/// VECTO-173
		/// </summary>
		[TestMethod]
		public void LossMapAxleLossMapMissing()
		{
			var gearboxData = CreateGearboxData(GearboxDirectLoss, GearboxIndirectLoss);
			var engineData = MockSimulationDataFactory.CreateEngineDataFromFile(EngineFile);
			var runData = new VectoRunData { GearboxData = gearboxData, EngineData = engineData };
			var result = VectoRunData.ValidateRunData(runData, new ValidationContext(runData));
			Assert.IsTrue(ValidationResult.Success == result);
			Assert.IsFalse(runData.IsValid());
		}

		/// <summary>
		/// VECTO-173
		/// </summary>
		[TestMethod]
		public void LossMapGearLossMapMissing()
		{
			var engineData = MockSimulationDataFactory.CreateEngineDataFromFile(EngineFile);
			var axleGearData = CreateAxleGearData(AxleGearLossMap);

			var runData = new VectoRunData { EngineData = engineData, AxleGearData = axleGearData };
			var result = VectoRunData.ValidateRunData(runData, new ValidationContext(runData));
			Assert.IsTrue(ValidationResult.Success == result);
			Assert.IsFalse(runData.IsValid());
		}


		private static GearboxData CreateGearboxData(string directlossMap, string indirectLossMap)
		{
			var ratios = new[] { 14.93, 11.64, 9.02, 7.04, 5.64, 4.4, 3.39, 2.65, 2.05, 1.6, 1.28, 1.0 };
			return new GearboxData {
				Gears = ratios.Select((ratio, i) =>
					Tuple.Create((uint)i,
						new GearData {
							FullLoadCurve = FullLoadCurveReader.ReadFromFile(GearboxFullLoadCurveFile),
							LossMap = TransmissionLossMap.ReadFromFile(ratio != 1.0 ? directlossMap : indirectLossMap, ratio,
								string.Format("Gear {0}", i)),
							Ratio = ratio,
							ShiftPolygon = ShiftPolygonReader.ReadFromFile(ShiftPolygonFile)
						}))
					.ToDictionary(k => k.Item1 + 1, v => v.Item2)
			};
		}

		private static AxleGearData CreateAxleGearData(string lossMap)
		{
			const double ratio = 2.59;
			return new AxleGearData {
				AxleGear = new GearData {
					Ratio = ratio,
					LossMap = TransmissionLossMap.ReadFromFile(lossMap, ratio, "AxleGear")
				}
			};
		}
	}
}