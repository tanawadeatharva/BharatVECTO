using System;
using System.Collections.Generic;
using System.Diagnostics;
using Castle.Components.DictionaryAdapter;
using Moq;
using NUnit.Framework;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents.StrategyDataAdapter;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.ElectricComponents.Battery;
using TUGraz.VectoCore.Tests.Utils;

namespace TUGraz.VectoCore.Tests.Models.Declaration.DataAdapter;


[TestFixture]
public class HybridStrategyDataAdapterTest
{

    [TestCase(0.7375, 0.2625, 11273.9176, 778.61, 247500, false, 0.624646591, 0.307641364)]
	[TestCase(0.6, 0.2, 11000, 778.61, 80000, false, 0.582967134, 0.234065732)]
	[TestCase(0.6, 0.2, 11000, 778.61, 5000, true, 0.268131464, 0.634065732)]
	public void SerialHybridStrategyTest(double bat_soc_max, double bat_soc_min, double vehicle_mass, double nominalVoltage, double nominalCapacity, bool exception, double expected_target_soc, double expected_min_soc)
	{
		var mass = vehicle_mass.SI<Kilogram>();
		var v_nom = nominalVoltage.SI<Volt>();
		var cap_nom = nominalCapacity.SI<AmpereSecond>();


		var dataAdapter = new SerialHybridStrategyParameterDataAdapter();
		var batterySystemData = new BatterySystemData() {
			Batteries = new EditableList<Tuple<int, BatteryData>>() {
				new Tuple<int, BatteryData>(1, new BatteryData() {
					Capacity = cap_nom,
					ChargeSustainingBattery = false,
					BatteryId = -42,
					MaxSOC = bat_soc_max,
					MinSOC = bat_soc_min,
					SOCMap = new SOCMap(new SOCMap.SOCMapEntry[] {
						new SOCMap.SOCMapEntry() {
							BatteryVolts = Math.Max((v_nom + (v_nom * 0.1)).Value(), 0).SI<Volt>(),
							SOC = 0.1
						},

						new SOCMap.SOCMapEntry() {
							BatteryVolts = v_nom,
							SOC = 0.5
						},

						new SOCMap.SOCMapEntry() {
							BatteryVolts = v_nom - (v_nom * 0.1),
							SOC = 0.9
						}
					})
				})
			}
		};

		HybridStrategyParameters parameters = null;
		try {
			parameters = dataAdapter.CreateHybridStrategyParameters(batterySystemData, null, mass,
				VectoRunData.OvcHevMode.NotApplicable);
		} catch (Exception) {
			Assert.IsTrue(exception);
			Assert.Pass();
		}
		Assert.IsTrue(parameters!.TargetSoC.IsEqual(expected_target_soc));
		Assert.IsTrue(parameters.MinSoC.IsEqual(expected_min_soc));

	}

	[TestCase(1000.00000000000000 	,10.00000000000000	,1000.00000000000000	,1.00000000000000	,0.61496863314570	,0.78861497592102	,0.70714213564177	,1.00000000000000	,false)]
	//[TestCase(40000.00000000000000	,10.00000000000000	,1000.00000000000000	,25.00000000000000,	0.77784206083558,	0.62853936105471,	0.70714213564177,	1.00000000000000,	true)] //fallback 30 kmh
	[TestCase(1000.00000000000000 	,10.00000000000000	,1000.00000000000000	,25.00000000000000,	0.12338337323207,	0.99240946348263,	0.70714213564177,	1.00000000000000,	false)]
	[TestCase(40000.00000000000000	,10.00000000000000	,1000.00000000000000	,25.00000000000000,	0.33348329959851,	0.94280904158206,	0.70714213564177,	1.00000000000000,	false)]
	[TestCase(40000.00000000000000	,10.00000000000000	,1000.00000000000000	,3.00000000000000	,0.96230240877072	,0.27216552697591	,0.70714213564177	,1.00000000000000	,true)]
	public void SerialHybridSuperCapTest(double mass_kg, double U_min_V, double U_max_V, double C_F, double exp_soc_min, double exp_soc_target, double exp_soc_initial, double exp_soc_max, bool exception = false)
	{
		var dataAdapter = new SerialHybridStrategyParameterDataAdapter();

		var mass = mass_kg.SI<Kilogram>();
		var Umin_sc = U_min_V.SI<Volt>();
		var Umax_sc = U_max_V.SI<Volt>();
		var Cap_sc = C_F.SI<Farad>();


		var scData = new SuperCapData() {
			Capacity = Cap_sc,
			MaxVoltage = Umax_sc,
			MinVoltage = Umin_sc,
		};
		if (exception) {
			Assert.Throws<VectoException>(() =>
				dataAdapter.CreateHybridStrategyParameters(null, scData, mass,
					VectoRunData.OvcHevMode.ChargeSustaining));
			Assert.Pass();
		}
		var parameter = dataAdapter.CreateHybridStrategyParameters(null, scData, mass, VectoRunData.OvcHevMode.ChargeSustaining);

		Assert.AreEqual(exp_soc_target, parameter.TargetSoC, 1E-3);
		Assert.AreEqual(exp_soc_min, parameter.MinSoC, 1E-3);
		Assert.AreEqual(exp_soc_max, parameter.MaxSoC, 1E-3);


		Assert.AreEqual(exp_soc_initial, parameter.InitialSoc, 1E-3);
	}

}