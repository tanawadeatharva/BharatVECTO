using System.Data;
using NUnit.Framework;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.Models.GenericModelData;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Tests.Models.Declaration
{

	[TestFixture]
	public class TestGenericBusEMotorData
	{
		private const string OutputShaftSpeedColumn = "outShaftSpeed";
		private const string MaxTorqueColumn = "maxTorque";
		private const string MinTorqueColumn = "minTorque";
		private TableData fullLoadCurve;

		
		[OneTimeSetUp]
		public void Init()
		{
			SetFullLoadCurveData();
		}

		private void SetFullLoadCurveData()
		{
			fullLoadCurve = new TableData();
			fullLoadCurve.Columns.Add(OutputShaftSpeedColumn);
			fullLoadCurve.Columns.Add(MaxTorqueColumn);
			fullLoadCurve.Columns.Add(MinTorqueColumn);
			for (int i = 0; i < 22; i++) {
				fullLoadCurve.Rows.Add(fullLoadCurve.NewRow());
			}

			fullLoadCurve.Rows[0][OutputShaftSpeedColumn] = "0.00";
			fullLoadCurve.Rows[1][OutputShaftSpeedColumn] = "14.96";
			fullLoadCurve.Rows[2][OutputShaftSpeedColumn] = "151.09";
			fullLoadCurve.Rows[3][OutputShaftSpeedColumn] = "302.19";
			fullLoadCurve.Rows[4][OutputShaftSpeedColumn] = "452.92";
			fullLoadCurve.Rows[5][OutputShaftSpeedColumn] = "604.01";
			fullLoadCurve.Rows[6][OutputShaftSpeedColumn] = "755.11";
			fullLoadCurve.Rows[7][OutputShaftSpeedColumn] = "906.20";
			fullLoadCurve.Rows[8][OutputShaftSpeedColumn] = "1057.30";
			fullLoadCurve.Rows[9][OutputShaftSpeedColumn] = "1208.03";
			fullLoadCurve.Rows[10][OutputShaftSpeedColumn] = "1359.12";
			fullLoadCurve.Rows[11][OutputShaftSpeedColumn] = "1510.22";
			fullLoadCurve.Rows[12][OutputShaftSpeedColumn] = "1661.31";
			fullLoadCurve.Rows[13][OutputShaftSpeedColumn] = "1812.41";
			fullLoadCurve.Rows[14][OutputShaftSpeedColumn] = "1963.14";
			fullLoadCurve.Rows[15][OutputShaftSpeedColumn] = "2114.23";
			fullLoadCurve.Rows[16][OutputShaftSpeedColumn] = "2265.33";
			fullLoadCurve.Rows[17][OutputShaftSpeedColumn] = "2416.42";
			fullLoadCurve.Rows[18][OutputShaftSpeedColumn] = "2567.52";
			fullLoadCurve.Rows[19][OutputShaftSpeedColumn] = "2718.25";
			fullLoadCurve.Rows[20][OutputShaftSpeedColumn] = "2869.34";
			fullLoadCurve.Rows[21][OutputShaftSpeedColumn] = "3020.44";
			
			fullLoadCurve.Rows[0][MaxTorqueColumn] = "4027.80";
			fullLoadCurve.Rows[1][MaxTorqueColumn] = "4010.00";
			fullLoadCurve.Rows[2][MaxTorqueColumn] = "3980.00";
			fullLoadCurve.Rows[3][MaxTorqueColumn] = "4010.00";
			fullLoadCurve.Rows[4][MaxTorqueColumn] = "3950.00";
			fullLoadCurve.Rows[5][MaxTorqueColumn] = "3900.00";
			fullLoadCurve.Rows[6][MaxTorqueColumn] = "3950.00";
			fullLoadCurve.Rows[7][MaxTorqueColumn] = "3356.50";
			fullLoadCurve.Rows[8][MaxTorqueColumn] = "2876.98";
			fullLoadCurve.Rows[9][MaxTorqueColumn] = "2517.38";
			fullLoadCurve.Rows[10][MaxTorqueColumn] = "2237.68";
			fullLoadCurve.Rows[11][MaxTorqueColumn] = "2013.90";
			fullLoadCurve.Rows[12][MaxTorqueColumn] = "1830.82";
			fullLoadCurve.Rows[13][MaxTorqueColumn] = "1678.25";
			fullLoadCurve.Rows[14][MaxTorqueColumn] = "1549.15";
			fullLoadCurve.Rows[15][MaxTorqueColumn] = "1438.52";
			fullLoadCurve.Rows[16][MaxTorqueColumn] = "1342.60";
			fullLoadCurve.Rows[17][MaxTorqueColumn] = "1258.71";
			fullLoadCurve.Rows[18][MaxTorqueColumn] = "1184.66";
			fullLoadCurve.Rows[19][MaxTorqueColumn] = "1118.82";
			fullLoadCurve.Rows[20][MaxTorqueColumn] = "1059.96";
			fullLoadCurve.Rows[21][MaxTorqueColumn] = "1006.95";

			foreach (DataRow row in fullLoadCurve.Rows) {
				row[MinTorqueColumn] = row.ParseDouble(MaxTorqueColumn) * -1;
			}
		}

		[TestCase()]
		public void TestFullLoadCurveRatedPointSearch()
		{
			var result = GenericRatedPointHelper.GetRatedPointOfFullLoadCurve(fullLoadCurve);
			Assert.IsNotNull(result); 
			Assert.AreEqual(755.11 , result.NRated.Value(), 1e-2);
			Assert.AreEqual(4027.8000, result.TRated.Value(), 1e-4);
			Assert.AreEqual(318.4980 , result.PRated.Value(), 1e-4);
		}
	}
}
