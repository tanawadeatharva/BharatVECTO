/*
* Copyright 2015, 2016 Graz University of Technology,
* Institute of Internal Combustion Engines and Thermodynamics,
* Institute of Technical Informatics
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

using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Engine;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.Utils;

#pragma warning disable 169

namespace TUGraz.VectoCore.Tests.Models.SimulationComponentData
{
	[TestClass]
	[SuppressMessage("ReSharper", "InconsistentNaming")]
	[SuppressMessage("ReSharper", "UnusedMember.Local")]
	public class CombustionEngineDataValidationTestClass
	{
		/// <summary>
		/// VECTO-107 Check valid range of input parameters
		/// </summary>
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
			fullLoad.Columns.Add("Engine speed");
			fullLoad.Columns.Add("max torque");
			fullLoad.Columns.Add("drag torque");
			fullLoad.Columns.Add("PT1");
			fullLoad.Rows.Add("3", "3", "-3", "3");
			fullLoad.Rows.Add("3", "3", "-3", "3");

			var data = new CombustionEngineData {
				MakeAndModel = "asdf",
				Displacement = 6374.SI().Cubic.Centi.Meter.Cast<CubicMeter>(),
				IdleSpeed = 560.RPMtoRad(),
				Inertia = 1.SI<KilogramSquareMeter>(),
				WHTCUrban = 1.SI().Gramm.Per.Kilo.Watt.Hour.Cast<KilogramPerWattSecond>(),
				WHTCRural = 1.SI().Gramm.Per.Kilo.Watt.Hour.Cast<KilogramPerWattSecond>(),
				WHTCMotorway = 1.SI().Gramm.Per.Kilo.Watt.Hour.Cast<KilogramPerWattSecond>(),
				FullLoadCurve = EngineFullLoadCurve.Create(fullLoad),
				ConsumptionMap = FuelConsumptionMap.Create(fuelConsumption)
			};
			data.FullLoadCurve.EngineData = data;

			var results = data.Validate();
			Assert.IsFalse(results.Any(), "Validation Failed: " + string.Join("; ", results.Select(r => r.ErrorMessage)));
			Assert.IsTrue(data.IsValid());
		}

		/// <summary>
		/// VECTO-107 Check valid range of input parameters
		/// </summary>
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
				AxleGear = new GearData {
				Ratio = 1,
				LossMap = TransmissionLossMap.ReadFromFile(@"TestData\Components\limited.vtlm", 1, "1"),
				}
			};

			container.RunData = new VectoRunData {
				GearboxData = gearboxData,
				EngineData = engineData,
				AxleGearData = axleGearData
			};

			var results = data.Validate();
			Assert.IsFalse(results.Any(), "Validation Failed: " + string.Join("; ", results.Select(r => r.ErrorMessage)));
		}

		/// <summary>
		/// VECTO-107 Check valid range of input parameters
		/// </summary>
		[TestMethod]
		public void Validation_Test()
		{
			var results = new DataObject().Validate();

			// every field and property should be tested except private parent fields and properties and 
			// (4*4+1) * 2 = 17*2= 34 - 4 private parent fields (+2 public field and property which are tested twice) = 32
			Assert.AreEqual(32, results.Count, "Validation Error: " + string.Join("\n_eng_avg", results.Select(r => r.ErrorMessage)));
		}


		public class DeepDataObject
		{
			[Required, Range(41, 42)] protected int public_field = 5;
		}


		public abstract class ParentDataObject
		{
			#region 4 parent instance fields

			[Required, Range(1, 2)] private int private_parent_field = 7;
			[Required, Range(3, 4)] protected int protected_parent_field = 7;
			[Required, Range(5, 6)] internal int internal_parent_field = 7;
			[Required, Range(7, 8)] public int public_parent_field = 5;

			#endregion

			#region 4 parent static field

			[Required, Range(43, 44)] private static int private_static_parent_field = 7;
			[Required, Range(43, 44)] protected static int protected_static_parent_field = 7;
			[Required, Range(50, 51)] internal static int internal_static_parent_field = 7;
			[Required, Range(45, 46)] public static int public_static_parent_field = 7;

			#endregion

			#region 4 parent instance properties

			[Required, Range(11, 12)]
			private int private_parent_property
			{
				get { return 7; }
			}

			[Required, Range(13, 14)]
			protected int protected_parent_property
			{
				get { return 7; }
			}

			[Required, Range(15, 16)]
			internal int internal_parent_property
			{
				get { return 7; }
			}

			[Required, Range(17, 18)]
			public int public_parent_property
			{
				get { return 7; }
			}

			#endregion

			#region 4 parent static properties

			[Required, Range(19, 20)]
			private static int private_static_parent_property
			{
				get { return 7; }
			}

			[Required, Range(19, 20)]
			protected static int protected_static_parent_property
			{
				get { return 7; }
			}

			[Required, Range(19, 20)]
			internal static int internal_static_parent_property
			{
				get { return 7; }
			}

			[Required, Range(19, 20)]
			public static int public_static_parent_property
			{
				get { return 7; }
			}

			#endregion

			#region 1 parent sub objects

			[Required, ValidateObject] public DeepDataObject parent_sub_object = new DeepDataObject();

			#endregion

			private void just_to_remove_compiler_warnings()
			{
				private_parent_field = private_static_parent_field;
			}
		}

		public class DataObject : ParentDataObject
		{
			#region 4 instance fields

			[Required, Range(1, 2)] private int private_field = 7;
			[Required, Range(3, 4)] protected int protected_field = 7;
			[Required, Range(5, 6)] internal int internal_field = 7;
			[Required, Range(7, 8)] public int public_field = 5;

			#endregion

			#region 4 static field

			[Required, Range(43, 44)] private static int private_static_field = 7;
			[Required, Range(43, 44)] protected static int protected_static_field = 7;
			[Required, Range(50, 51)] internal static int internal_static_field = 7;
			[Required, Range(45, 46)] public static int public_static_field = 7;

			#endregion

			#region 4 instance properties

			[Required, Range(11, 12)]
			private int private_property
			{
				get { return 7; }
			}

			[Required, Range(13, 14)]
			protected int protected_property
			{
				get { return 7; }
			}

			[Required, Range(15, 16)]
			internal int internal_property
			{
				get { return 7; }
			}

			[Required, Range(17, 18)]
			public int public_property
			{
				get { return 7; }
			}

			#endregion

			#region 4 static properties

			[Required, Range(19, 20)]
			private static int private_static_property
			{
				get { return 7; }
			}

			[Required, Range(19, 20)]
			protected static int protected_static_property
			{
				get { return 7; }
			}

			[Required, Range(19, 20)]
			internal static int internal_static_property
			{
				get { return 7; }
			}

			[Required, Range(19, 20)]
			public static int public_static_property
			{
				get { return 7; }
			}

			#endregion

			#region 1 sub objects

			[Required, ValidateObject] public DeepDataObject sub_object = new DeepDataObject();

			#endregion

			private void just_to_remove_compiler_warnings()
			{
				private_field = private_static_field;
			}
		}
	}
}