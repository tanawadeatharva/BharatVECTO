using System;
using System.Data;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.GenericModelData
{

	public class RatedPoint
	{
		public Watt PRated { get; }
		public PerSecond NRated { get; }
		public NewtonMeter TRated { get; }
		
		public RatedPoint(PerSecond nRated, NewtonMeter tRated)
		{
			PRated = GetPRated(nRated, tRated);
			NRated = nRated;
			TRated = tRated;
		}

		private Watt GetPRated(PerSecond nRated, NewtonMeter tRated)
		{
			return (nRated.Value() * tRated.Value() * Math.PI / 30000).SI<Watt>(); 
		}
	}


	public static class GenericRatedPointHelper
	{
		#region Constants

		public const string MotorSpeed = "n";
		public const string TorqueDrive = "T_drive";
		public const string TorqueDrag = "T_drag";
		public const string PowerDrive = "P_drive";

		public const string SlopeValue = "slope";
		public const string DeltaValue = "delta";

		#endregion
		
		public static RatedPoint GetRatedPointOfFullLoadCurve(TableData fullLoadCurve)
		{
			var curveValues = new DataTable();
			var slopeValues = new DataTable();
			SetCurveValues(fullLoadCurve, ref curveValues, ref slopeValues);

			var ratedIndex = FindRowOfRatedPoint(slopeValues);
			var nRated = curveValues.Rows[ratedIndex].ParseDouble(MotorSpeed).SI<PerSecond>();
			var tRated = GetHighestTorque(curveValues);

			return new RatedPoint (nRated, tRated);
		}

		private static void SetCurveValues(TableData fullLoadCurve, ref DataTable curveValues, ref DataTable slopeValues)
		{
			curveValues.Columns.Add(MotorSpeed);
			curveValues.Columns.Add(TorqueDrive);
			curveValues.Columns.Add(TorqueDrag);
			curveValues.Columns.Add(PowerDrive);

			slopeValues.Columns.Add(SlopeValue);
			slopeValues.Columns.Add(DeltaValue);

			for (int r = 0; r < fullLoadCurve.Rows.Count; r++)
			{
				curveValues.Rows.Add(curveValues.NewRow());

				var motorSpeed = fullLoadCurve.Rows[r].ParseDouble("outShaftSpeed");
				var torqueDrive = fullLoadCurve.Rows[r].ParseDouble("maxTorque");

				curveValues.Rows[r][MotorSpeed] = motorSpeed;
				curveValues.Rows[r][TorqueDrive] = torqueDrive;
				curveValues.Rows[r][TorqueDrag] = fullLoadCurve.Rows[r].ParseDouble("minTorque");
				curveValues.Rows[r][PowerDrive] = motorSpeed * torqueDrive * Math.PI / 30000;

				if (r == 0)
					continue;

				slopeValues.Rows.Add(slopeValues.NewRow());

				var slopeValue = (curveValues.Rows[r].ParseDouble(PowerDrive) - curveValues.Rows[r - 1].ParseDouble(PowerDrive)) /
								(curveValues.Rows[r].ParseDouble(MotorSpeed) - curveValues.Rows[r - 1].ParseDouble(MotorSpeed));
				
				slopeValues.Rows[r - 1][SlopeValue] = slopeValue;
				slopeValues.Rows[r - 1][DeltaValue] = slopeValues.Rows[r - 1].ParseDouble(SlopeValue) / slopeValues.Rows[0].ParseDouble(SlopeValue) - 1;
			}
		}

		private static NewtonMeter GetHighestTorque(DataTable curveValues)
		{
			var value = double.MinValue;

			foreach (DataRow row in curveValues.Rows) {
				var currentValue = row.ParseDouble(TorqueDrive);
				if (value < currentValue)
					value = currentValue;
			}

			return value.SI<NewtonMeter>();
		}
		
		private static int FindRowOfRatedPoint(DataTable slopeValues)
		{
			for (int r = 0; r < slopeValues.Rows.Count; r++) {

				var deltaValue = slopeValues.Rows[r].ParseDouble(DeltaValue);

				if (Math.Abs(deltaValue) > 0.2)
					return r;
			}

			return -1;
		}
	}
}
