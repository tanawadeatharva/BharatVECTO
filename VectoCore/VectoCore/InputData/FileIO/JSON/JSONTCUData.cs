using System;
using System.IO;
using Newtonsoft.Json.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.JSON
{
	public class JSONTCUDataV1 : JSONFile, IGearshiftEngineeringInputData
	{
		public JSONTCUDataV1(JObject json, string filename, bool tolerateMissing) : base(json, filename, tolerateMissing) { }

		#region Implementation of IGearshiftEngineeringInputData

		public MeterPerSecond StartSpeed
		{
			get { return Body.GetEx<double>("StartSpeed").KMPHtoMeterPerSecond(); }
		}

		public MeterPerSquareSecond StartAcceleration
		{
			get { return Body.GetEx<double>("StartAcceleration").SI<MeterPerSquareSecond>(); }
		}

		public Second GearResidenceTime
		{
			get { return Body.GetEx<double>("GearResidenceTime").SI<Second>(); }
		}

		public double DnT99LHMin1
		{
			get { return Body.GetEx<double>("Dn_Tq99L_high_min_1"); }
		}

		public double DnT99LHMin2
		{
			get { return Body.GetEx<double>("Dn_Tq99L_high_min_2"); }
		}

		public int AllowedGearRangeUp
		{
			get { return Body.GetEx<int>("GearRangeUp"); }
		}

		public int AllowedGearRangeDown
		{
			get { return Body.GetEx<int>("GearRangeDown"); }
		}

		public Second LookBackInterval
		{
			get { return Body.GetEx<double>("LookBackDriver").SI<Second>(); }
		}

		public Watt AvgCardanPowerThresholdPropulsion
		{
			get { return Body.GetEx<double>("P_card_avg_threshold").SI<Watt>(); }
		}

		public Watt CurrCardanPowerThresholdPropulsion
		{
			get { return Body.GetEx<double>("P_card_curr_threshold").SI<Watt>(); }
		}

		public double TargetSpeedDeviationFactor
		{
			get { return Body.GetEx<double>("Diff_curr_targ_vel"); }
		}

		public double EngineSpeedHighDriveOffFactor
		{
			get { return Body.GetEx<double>("EngineSpeedHighDriveOffFactor"); }
		}

		public double RatingFactorCurrentGear
		{
			get { return Body.GetEx<double>("Rating_current_gear"); }
		}

		public TableData AccelerationReserveLookup
		{
			get {
				try {
					return ReadTableData(Body.GetEx<string>("AccelerationReserveLookup"), "AccelerationReserveLookup");
				} catch (Exception) {
					if (!TolerateMissing) {
						throw;
					}

					return new TableData(
						Path.Combine(BasePath, Body["AccelerationReserveLookup"].ToString()) + MissingFileSuffix,
						DataSourceType.Missing);
				}
			}
		}

		public TableData ShareTorque99L
		{
			get {
				try {
					return ReadTableData(Body.GetEx<string>("ShareTorque99L"), "ShareTorque99L");
				} catch (Exception) {
					if (!TolerateMissing) {
						throw;
					}

					return new TableData(
						Path.Combine(BasePath, Body["ShareTorque99L"].ToString()) + MissingFileSuffix,
						DataSourceType.Missing);
				}
			}
		}

		public TableData PredictionDurationLookup
		{
			get {
				try {
					return ReadTableData(Body.GetEx<string>("PredictionDurationLookup"), "PredictionDurationLookup");
				} catch (Exception) {
					if (!TolerateMissing) {
						throw;
					}

					return new TableData(
						Path.Combine(BasePath, Body["PredictionDurationLookup"].ToString()) + MissingFileSuffix,
						DataSourceType.Missing);
				}
			}
		}

		public TableData ShareIdleLow
		{
			get {
				try {
					return ReadTableData(Body.GetEx<string>("ShareIdleLow"), "ShareIdleLow");
				} catch (Exception) {
					if (!TolerateMissing) {
						throw;
					}

					return new TableData(
						Path.Combine(BasePath, Body["ShareIdleLow"].ToString()) + MissingFileSuffix,
						DataSourceType.Missing);
				}
			}
		}

		public TableData ShareEngineHigh
		{
			get {
				try {
					return ReadTableData(Body.GetEx<string>("ShareEngineHigh"), "ShareEngineHigh");
				} catch (Exception) {
					if (!TolerateMissing) {
						throw;
					}

					return new TableData(
						Path.Combine(BasePath, Body["ShareEngineHigh"].ToString()) + MissingFileSuffix,
						DataSourceType.Missing);
				}
			}
		}

		#endregion
	}
}
