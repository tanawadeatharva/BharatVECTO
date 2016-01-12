using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Engine;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Tests.Models.SimulationComponentData
{
	[TestClass]
	public class CombustionEngineDataValidationTestClass
	{
		[TestMethod]
		public void Validation_CombustionEngineData()
		{
			var fuelConsumption = new DataTable();
			fuelConsumption.Columns.Add("");
			fuelConsumption.Columns.Add("");
			fuelConsumption.Columns.Add("");
			fuelConsumption.Rows.Add("1", "1", "1");
			fuelConsumption.Rows.Add("2", "2", "2");
			fuelConsumption.Rows.Add("3", "3", "3");

			var fullLoad = new DataTable();
			fullLoad.Columns.Add("");
			fullLoad.Columns.Add("");
			fullLoad.Columns.Add("");
			fullLoad.Columns.Add("");
			fullLoad.Rows.Add("3", "3", "3", "3");
			fullLoad.Rows.Add("3", "3", "3", "3");

			var data = new CombustionEngineData {
				ModelName = "asdf",
				Displacement = 1.SI<CubicMeter>(),
				IdleSpeed = 560.RPMtoRad(),
				Inertia = 1.SI<KilogramSquareMeter>(),
				WHTCUrban = 1.SI<KilogramPerWattSecond>(),
				WHTCRural = 1.SI<KilogramPerWattSecond>(),
				WHTCMotorway = 1.SI<KilogramPerWattSecond>(),
				FullLoadCurve = EngineFullLoadCurve.Create(fullLoad),
				ConsumptionMap = FuelConsumptionMap.Create(fuelConsumption)
			};
			var results = data.Validate();
			Assert.IsFalse(results.Any(), "Validation Failed: " + string.Join("; ", results.Select(r => r.ErrorMessage)));
			Assert.IsTrue(data.IsValid());
		}

		[TestMethod]
		public void Validation_VectoRun()
		{
			var container = new VehicleContainer();
			var data = new DistanceRun(container);
			var engineData = new CombustionEngineData {
				FullLoadCurve = EngineFullLoadCurve.ReadFromFile(@"TestData\Components\12t Delivery Truck.vfld"),
				IdleSpeed = 560.RPMtoRad()
			};

			var gearboxData = new GearboxData();
			gearboxData.Gears[1] = new GearData {
				LossMap = TransmissionLossMap.ReadFromFile(@"TestData\Components\Direct Gear.vtlm", 1, "1"),
				Ratio = 1
			};

			var axleGearData = new AxleGearData {
				Ratio = 1,
				LossMap = TransmissionLossMap.ReadFromFile(@"TestData\Components\limited.vtlm", 1, "1"),
			};

			container.RunData = new VectoRunData {
				GearboxData = gearboxData,
				EngineData = engineData,
				AxleGearData = axleGearData
			};

			var results = data.Validate();
			Assert.IsFalse(results.Any(), "Validation Failed: " + string.Join("; ", results.Select(r => r.ErrorMessage)));
		}


		[TestMethod]
		public void Validation_Test()
		{
			var data = new Data();
			data.lu = new deep();
			var results = data.Validate();
			Assert.IsFalse(results.Any(), "Validation Failed: " + string.Join(" ", results.Select(r => r.ErrorMessage)));
		}


		public class deep
		{
			[Required, Range(10, 16)] public int mah = 45;
		}


		public abstract class Muh
		{
			[Required, Range(6, 9)]
			protected int higss { get; set; }

			[Required, ValidateObject] public deep lu;
		}


		public class Data : Muh
		{
			[Required, Range(2, 5)] private int bla = 7;
		}
	}
}