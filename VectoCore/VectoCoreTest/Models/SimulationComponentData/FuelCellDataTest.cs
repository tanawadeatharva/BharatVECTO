using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using NUnit.Framework;
using NUnit.Framework.Internal;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.SimulationComponent.Data.ElectricComponents;

namespace TUGraz.VectoCore.Tests.Models.SimulationComponentData;

[TestFixture]
public class FuelCellDataTest
{
	[Test]
	public void FuelCellMassFlowMap_MinMaxPowerTest() {
		var massFlowMap = GetFuelCellMassFlowMap(1);


		Assert.That(massFlowMap.MinPower.Value(), Is.EqualTo(30*1e3));
		Assert.That(massFlowMap.MaxPower.Value(), Is.EqualTo(300*1e3));
		Assert.That(massFlowMap.MinEffPower.Value(), Is.EqualTo(60*1e3));
	}
	//[TestCase(30.0, 2002.0)] <- without timeslicing
	[TestCase(30.0, 1501.5)]
	[TestCase(60.0, 3003.0)]
	[TestCase(255.0, 17017.0)]
	[TestCase(300.0, 21450.0)]
	//interpolate
	[TestCase(70, 2502.5)]
	public void FuelCellMassFlowMap_LookupTest(double power_kW, double exp_H2_g_p_h) {
		var massFlowMap = GetFuelCellMassFlowMap(1);

		var power = power_kW.SI(Unit.SI.Kilo.Watt).Cast<Watt>();
		var expFuelConsumption = exp_H2_g_p_h.SI(Unit.SI.Gramm.Per.Hour).Cast<KilogramPerSecond>();

		var fc = massFlowMap.Lookup(power);

		Assert.That(fc, Is.EqualTo(expFuelConsumption));
	}

	[TestCase(600)]
	//[TestCase(10)] <- supported now with timeslicing 
	[TestCase(-100)]
	public void FuelCellMassFlowMap_LookupTestFail(double power_kW) {
		var massFlowMap = GetFuelCellMassFlowMap(1);

		var power = power_kW.SI(Unit.SI.Kilo.Watt).Cast<Watt>();

		Assert.That(() => massFlowMap.Lookup(power), Throws.TypeOf(typeof(VectoException)));
	}


    [Test]
	public void InvalidFuelCellMassFlowMapTest() {
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
    public void FuelCellString_FuelCellCountTest(double power_kW, int activeFc) {
		var map = GetFuelCellMassFlowMap(1);
		var power = power_kW.SI(Unit.SI.Kilo.Watt).Cast<Watt>();
		var fcString = new FuelCellStringMassFlowMap(map, 3);
		Assert.That(fcString.GetActiveFuelCellCount(power), Is.EqualTo(activeFc));
	}

	[Test]
	public void FuelCellString_MinMaxPowerTest() {
		var map = GetFuelCellMassFlowMap(1);
		var fcString = new FuelCellStringMassFlowMap(map, 3);
		Assert.That(fcString.MinPower.Value(), Is.EqualTo(30*1e3));
		Assert.That(fcString.MaxPower.Value(), Is.EqualTo(900*1e3));
		Assert.That(fcString.MinPowerEff.Value(), Is.EqualTo(60 * 1e3));
	}

	[TestCase(30, 1501.5)]
	[TestCase(60, 3003)]
    [TestCase(120, 6006)]
	[TestCase(180, 9009)]

    public void FuelCellString_LookupTest(double power_kW, double exp_H2_g_p_h) {
		var map = GetFuelCellMassFlowMap(1);
		var fcString = new FuelCellStringMassFlowMap(map, 3);

		var power = power_kW.SI(Unit.SI.Kilo.Watt).Cast<Watt>();
		var expFuelConsumption = exp_H2_g_p_h.SI(Unit.SI.Gramm.Per.Hour).Cast<KilogramPerSecond>();


        Assert.That(fcString.Lookup(power), Is.EqualTo(expFuelConsumption));
	}

	[Test]
	public void FuelCellString_MeasuredPointsTest()
	{
		var fcMap = GetFuelCellMassFlowMap(1);
		var fcString = new FuelCellStringMassFlowMap(fcMap, 3);
		var measuredPoints = fcString.MeasuredPoints;


		var expected = new List<double>() {
			30,60,105,120,180,315, 450, 585, 765, 900
		}.Select(d => (d * 1e3).SI<Watt>()).ToList();




		Assert.That(expected.Count, Is.EqualTo(measuredPoints.Count));
		for (int i = 0; i < measuredPoints.Count; i++) {
			Assert.That(measuredPoints[i], Is.EqualTo(expected[i]));
		}
	}


	//[TestCase(1, 1, 3, 3)]
	//[TestCase(1, 1, 1, 0)]
	//[TestCase(1,2, 3, 3)]
	[Combinatorial]
	public void FuelCellSystem_GetShareSmokeTest([Range(1,3)] int string1Variant, [Range(1,3)]int string2Variant, [Range(1,3)]int string1Count,  [Range(0,3)]int string2Count)
	{
		var fuelCellSystemMassFlowMap = new FuelCellSystemMassFlowMap(GetFuelCellStringMassFlowMap(
				count: string1Count, 
				variant:string2Variant),
			string2Count != 0 
				? GetFuelCellStringMassFlowMap(
				count: string2Count, 
				variant:string2Variant) : null);


		//var power = power_kW.SI(Unit.SI.Kilo.Watt).Cast<Watt>();



		var fuelCellSystemShare = new FuelCellSystemShareMap(fuelCellSystemMassFlowMap);

		var step = (fuelCellSystemMassFlowMap.MaxPower - fuelCellSystemMassFlowMap.MinPower) / 10000;
		var power = fuelCellSystemMassFlowMap.MinPower;
		while (power.IsSmallerOrEqual(fuelCellSystemMassFlowMap.MaxPower)) {
			var share = fuelCellSystemShare.Lookup(power);
			power += step;
		}
	}

	[TestCase(47700, new double[]{0.0, 1, 0.37, 0.63})]
	[TestCase(1, new double[] { 0.0, 1 })]
	[TestCase(150e3, new double[] { 0.2, 0.8 })]
	public void FuelCellSystem_GetShareTestEqualFuelCell(double power_W, double[] expected_a_s)
	{
		TestShares(power_W, expected_a_s, 1, 1);
	}

	[TestCase(200e3, new double[]{0.3})]
	[TestCase(1, new double[] { 0 })]
	[TestCase(550e3, new double[] { 0.24 })]
	[TestCase(150e3,  new double[] { 0.07 })]
    public void FuelCellSystem_GetShareTestDifferentFuelCellSameRange(double power_W, double[] expected_a_s)
	{
		Assert.That(expected_a_s.Count() > 0);
		TestShares(power_W, expected_a_s, 1, 2);
	}

	private void TestShares(double power_W, double[] expected_a_s, int variantFc1, int variantFc2, int countfc1 = 3, int countfc2 = 3)
	{
		var fuelCellSystemMassFlowMap = new FuelCellSystemMassFlowMap(GetFuelCellStringMassFlowMap(variant: variantFc1, count: countfc1),
			GetFuelCellStringMassFlowMap(variant: variantFc2, count: countfc2));

		var power = power_W.SI(Unit.SI.Watt).Cast<Watt>();


		TestContext.Progress.WriteLine(fuelCellSystemMassFlowMap.FuelCell1.MinPowerEff);

		var fuelCellSystemShare = new FuelCellSystemShareMap(fuelCellSystemMassFlowMap);
		var shares = fuelCellSystemShare.GetSharesWithLowestFuelConsumption(power).ToList();


		var share = fuelCellSystemShare.Lookup(power);

		var orderedShares = shares.OrderBy(a => a.Share.ShareA).ToArray();
		foreach (var fuelCellShareEntry in orderedShares) {
			TestContext.Progress.WriteLine(
				$"Share a:{fuelCellShareEntry.Share.ShareA} b: {fuelCellShareEntry.Share.ShareB}");
		}

		Assert.AreEqual(expected_a_s.Length, shares.Count());
		var orderedExpected = expected_a_s.OrderBy(a => a).ToArray();
		for (int i = 0; i < shares.Count(); i++) {
			Assert.That(orderedShares[i].Share.ShareA.IsEqual(orderedExpected[i], 1E-02),
				$"Expected {orderedExpected[i]} but was {orderedShares[i].Share.ShareA}");
		}
	}



	[TestCase(493e3, new double[]{0.60}, 1, 1)]
	[TestCase(495e3, new double[]{0.60}, 1, 1)]
	[TestCase(494e3, new double[] { 0.15 }, 3, 3)]
    public void FuelCellSystem_GetShareTestDifferentFuelCellDifferentRange(double power_W, double[] expected_a_s, int countfc1, int countfc2)
	{
		TestShares(power_W, expected_a_s, 1,3, countfc1:countfc1, countfc2:countfc2);

	}

	[TestCase(214000, 3,3, 2, Description="The maximum power of the fuel cell is smaller than 2x minEffPower")]
	public void GetActiveFuelCellCount(double power_W, int fcVariant, int fcCount, int expectedFcCount)
	{
		var fc = GetFuelCellStringMassFlowMap(fcVariant, fcCount);
		var p = power_W.SI<Watt>();
		Assert.That(fc.GetActiveFuelCellCount(p), Is.EqualTo(expectedFcCount));

	}




    public FuelCellStringMassFlowMap GetFuelCellStringMassFlowMap(int variant = 1, int count = 1)
	{
		return new FuelCellStringMassFlowMap(GetFuelCellMassFlowMap(variant), count);
	}




    public FuelCellMassFlowMap GetFuelCellMassFlowMap(int variant = 1)
	{
		FuelCellMassFlowMap CreateFuelCellMassFlowMap((double, double)[] valueTuples)
		{
			return new FuelCellMassFlowMap(valueTuples.Select(e => new FuelCellMassFlowMap.MassFlowMapEntry() {
				P_el_out = e.Item1.SI(Unit.SI.Kilo.Watt).Cast<Watt>(),
				H2 = e.Item2.SI(Unit.SI.Gramm.Per.Hour).Cast<KilogramPerSecond>()
			}).ToArray());
		}


		var variant1 = new (double, double)[] {
			//P_el_out[kW],   m_H2[g / h]
			(30.0, 2002.0),
			(60.0, 3003.0),
			(105.0, 5436.5),
			(150.0, 8662.5),
			(195.0, 12199.7),
			(255.0, 17017.0),
			(300.0, 21450.0),
		};

		var variant2 = new (double, double)[] {
			//P_el_out [kW],	m_H2 [g/h]
			(30.0, 2002.0),
			(70.0, 3003.0),
			(140.0, 4436.5),
			(150.0, 7662.5),
			(195.0, 14199.7),
			(255.0, 19017.0),
			(300.0, 28450.0),
		};

		var variant3 = new (double, double)[] {
			//P_el_out [kW],	m_H2 [g/h]
			(30.0, 2002.0),
			(70.0, 3003.0),
			(140.0, 4436.5),
			(150.0, 7662.5),
			(195.0, 14199.7),
		};


        switch (variant) {
			case 1:
				return CreateFuelCellMassFlowMap(variant1);
			case 2:
				return CreateFuelCellMassFlowMap(variant2);
			case 3:
				return CreateFuelCellMassFlowMap(variant3);
            default:
				throw new ArgumentOutOfRangeException();
		}
	}
}