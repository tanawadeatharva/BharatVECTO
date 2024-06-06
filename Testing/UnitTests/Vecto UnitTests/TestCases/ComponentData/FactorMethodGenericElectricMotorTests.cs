using NUnit.Framework;
using TUGraz.VectoCore.Models.GenericModelData;
using TUGraz.VectoCore.Tests.Utils;
using TUGraz.VectoCore.Utils;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace TUGraz.Vecto.UnitTests.TestCases.ComponentData;

public class FactorMethodGenericElectricMotorTests
{
	[TestCase()]
	public void TestFullLoadCurveRatedPointSearch()
	{
		var fullLoadCurve = InputDataHelper.InputDataAsTableData(EMFldHdr, EMFldData);

		var emResult = GenericRatedPointHelper.GetRatedPointOfFullLoadCurveAtEM(fullLoadCurve);
		Assert.IsNotNull(emResult);
		Assert.AreEqual(755.11, emResult.NRated.AsRPM, 1e-2, "Wrong speed");
		Assert.AreEqual(4027.8000, emResult.TRated.Value(), 1e-4, "Wrong torque");
		Assert.AreEqual(318498.02032, emResult.PRated.Value(), 1e-4, "Wrong power");

		var iepcResult = GenericRatedPointHelper.GetRatedPointOfFullLoadCurveAtIEPC(fullLoadCurve, 1, 1, 0.95, 1);
		Assert.IsNotNull(iepcResult);
		Assert.AreEqual(755.11, iepcResult.NRated.AsRPM, 1e-2);
		Assert.AreEqual(4239.7894, iepcResult.TRated.Value(), 1e-4);
		Assert.AreEqual(335261.0740, iepcResult.PRated.Value(), 1e-2);
	}

	private const string EMFldHdr = "outShaftSpeed, maxTorque, minTorque";

	private static readonly string[] EMFldData = new[] {
		"0.00, 4027.80, -4027.80",
		"14.96, 4010.00, -4010.00",
		"151.09, 3980.00, -3980.00",
		"302.19, 4010.00, -4010.00",
		"452.92, 3950.00, -3950.00",
		"604.01, 3900.00, -3900.00",
		"755.11, 3950.00, -3950.00",
		"906.20, 3356.50, -3356.50",
		"1057.30, 2876.98, -2876.98",
		"1208.03, 2517.38, -2517.38",
		"1359.12, 2237.68, -2237.68",
		"1510.22, 2013.90, -2013.90",
		"1661.31, 1830.82, -1830.82",
		"1812.41, 1678.25, -1678.25",
		"1963.14, 1549.15, -1549.15",
		"2114.23, 1438.52, -1438.52",
		"2265.33, 1342.60, -1342.60",
		"2416.42, 1258.71, -1258.71",
		"2567.52, 1184.66, -1184.66",
		"2718.25, 1118.82, -1118.82",
		"2869.34, 1059.96, -1059.96",
		"3020.44, 1006.95, -1006.95",
    };
}