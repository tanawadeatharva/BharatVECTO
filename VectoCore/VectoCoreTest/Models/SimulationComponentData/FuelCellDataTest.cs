using System;
using System.Security.Cryptography.X509Certificates;
using NUnit.Framework;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.SimulationComponent.Data.ElectricComponents;

namespace TUGraz.VectoCore.Tests.Models.SimulationComponentData;

[TestFixture]
public class FuelCellDataTest
{
	[Test]
	public void FuelCellMassFlowMap_MinMaxPowerTest()
	{
		var massFlowMap = GetFuelCellMassFlowMap(1);


		Assert.That(massFlowMap.MinPower.Value(), Is.EqualTo(30*1e3));
		Assert.That(massFlowMap.MaxPower.Value(), Is.EqualTo(300*1e3));
		Assert.That(massFlowMap.MinPowerEff.Value(), Is.EqualTo(60*1e3));
	}
	[TestCase(30.0, 2002.0)]
	[TestCase(60.0, 3003.0)]
	[TestCase(255.0, 17017.0)]
	[TestCase(300.0, 21450.0)]
	//interpolate
	[TestCase(45, 2502.5)]
	public void FuelCellMassFlowMap_LookupTest(double power_kW, double exp_H2_g_p_h)
	{
		var massFlowMap = GetFuelCellMassFlowMap(1);

		var power = power_kW.SI(Unit.SI.Kilo.Watt).Cast<Watt>();
		var expFuelConsumption = exp_H2_g_p_h.SI(Unit.SI.Gramm.Per.Hour).Cast<KilogramPerSecond>();

		var fc = massFlowMap.Lookup(power);

		Assert.That(fc, Is.EqualTo(expFuelConsumption));
	}

	[TestCase(600)]
	[TestCase(10)]
	[TestCase(-100)]
	public void FuelCellMassFlowMap_LookupTestFail(double power_kW)
	{
		var massFlowMap = GetFuelCellMassFlowMap(1);

		var power = power_kW.SI(Unit.SI.Kilo.Watt).Cast<Watt>();

		Assert.That(() => massFlowMap.Lookup(power), Throws.TypeOf(typeof(VectoException)));
	}


    [Test]
	public void InvalidFuelCellMassFlowMapTest()
	{
		Assert.Fail("Add checks if entry is present twice" +
					"Add checks there are enough entries present" +
					"..." +
					"" +
					"");
	}





	[TestCase(30, 1)]
	[TestCase(60, 1)]
	[TestCase(90, 1)]
	[TestCase(110, 1)]

	[TestCase(120, 2)]


	[TestCase(180, 3)]

	[TestCase(900, 3)]
    public void FuelCellString_FuelCellCountTest(double power_kW, int activeFc)
	{
		var map = GetFuelCellMassFlowMap(1);
		var power = power_kW.SI(Unit.SI.Kilo.Watt).Cast<Watt>();
		var fcString = new FuelCellStringMassFlowMap(map, 3);
		Assert.That(fcString.GetActiveFuelCellCount(power), Is.EqualTo(activeFc));
	}

	[Test]
	public void FuelCellString_MinMaxPowerTest()
	{
		var map = GetFuelCellMassFlowMap(1);
		var fcString = new FuelCellStringMassFlowMap(map, 3);
		Assert.That(fcString.MinPower.Value(), Is.EqualTo(30*1e3));
		Assert.That(fcString.MaxPower.Value(), Is.EqualTo(900*1e3));
		Assert.That(fcString.MinPowerEff.Value(), Is.EqualTo(60 * 1e3));
	}

	[TestCase(30, 2002)]
	[TestCase(60, 3003)]
    [TestCase(120, 6006)]
	[TestCase(180, 9009)]

    public void FuelCellString_LookupTest(double power_kW, double exp_H2_g_p_h)
	{
		var map = GetFuelCellMassFlowMap(1);
		var fcString = new FuelCellStringMassFlowMap(map, 3);

		var power = power_kW.SI(Unit.SI.Kilo.Watt).Cast<Watt>();
		var expFuelConsumption = exp_H2_g_p_h.SI(Unit.SI.Gramm.Per.Hour).Cast<KilogramPerSecond>();


        Assert.That(fcString.Lookup(power), Is.EqualTo(expFuelConsumption));
	}










    public FuelCellMassFlowMap GetFuelCellMassFlowMap(int variant = 1)
	{
		switch (variant)
		{
			case 1:
				return new FuelCellMassFlowMap(
					new FuelCellMassFlowMap.MassFlowMapEntry[] {
						new() {
							P_el_out = 30.0.SI(Unit.SI.Kilo.Watt).Cast<Watt>(),
							H2 = 2002.0.SI(Unit.SI.Gramm.Per.Hour).Cast<KilogramPerSecond>() },
						new() {
							P_el_out = 60.0.SI(Unit.SI.Kilo.Watt).Cast<Watt>(),
							H2 = 3003.0.SI(Unit.SI.Gramm.Per.Hour).Cast<KilogramPerSecond>() },
						new() {
							P_el_out = 105.0.SI(Unit.SI.Kilo.Watt).Cast<Watt>(),
							H2 = 5436.5.SI(Unit.SI.Gramm.Per.Hour).Cast<KilogramPerSecond>() },
						new() {
							P_el_out = 150.0.SI(Unit.SI.Kilo.Watt).Cast<Watt>(),
							H2 = 8662.5.SI(Unit.SI.Gramm.Per.Hour).Cast<KilogramPerSecond>() },
						new() {
							P_el_out = 195.0.SI(Unit.SI.Kilo.Watt).Cast<Watt>(),
							H2 = 12199.7.SI(Unit.SI.Gramm.Per.Hour).Cast<KilogramPerSecond>() },
						new() {
							P_el_out = 255.0.SI(Unit.SI.Kilo.Watt).Cast<Watt>(),
							H2 = 17017.0.SI(Unit.SI.Gramm.Per.Hour).Cast<KilogramPerSecond>() },
						new() {
							P_el_out = 300.0.SI(Unit.SI.Kilo.Watt).Cast<Watt>(),
							H2 = 21450.0.SI(Unit.SI.Gramm.Per.Hour).Cast<KilogramPerSecond>() },
					});

			default:
				throw new ArgumentOutOfRangeException();
		}




	}


}