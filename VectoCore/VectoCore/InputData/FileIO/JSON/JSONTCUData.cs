using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
			get { return Body.GetValueOrDefault<double>("StartSpeed")?.KMPHtoMeterPerSecond(); }
		}

		public MeterPerSquareSecond StartAcceleration
		{
			get { return Body.GetValueOrDefault<double>("StartAcceleration")?.SI<MeterPerSquareSecond>(); }
		}

		public Second GearResidenceTime
		{
			get { return Body.GetValueOrDefault<double>("GearResidenceTime")?.SI<Second>(); }
		}

		public double? DnT99LHMin1
		{
			get { return Body.GetValueOrDefault<double>("Dn_Tq99L_high_min_1"); }
		}

		public double? DnT99LHMin2
		{
			get { return Body.GetValueOrDefault<double>("Dn_Tq99L_high_min_2"); }
		}

		public int? AllowedGearRangeUp
		{
			get { return Body.GetValueOrDefault<int>("GearRangeUp"); }
		}

		public int? AllowedGearRangeDown
		{
			get { return Body.GetValueOrDefault<int>("GearRangeDown"); }
		}

		public Second LookBackInterval
		{
			get { return Body.GetValueOrDefault<double>("LookBackDriver")?.SI<Second>(); }
		}

		public Watt AvgCardanPowerThresholdPropulsion
		{
			get { return Body.GetValueOrDefault<double>("P_card_avg_threshold")?.SI<Watt>(); }
		}

		public Watt CurrCardanPowerThresholdPropulsion
		{
			get { return Body.GetValueOrDefault<double>("P_card_curr_threshold")?.SI<Watt>(); }
		}

		public double? TargetSpeedDeviationFactor
		{
			get { return Body.GetValueOrDefault<double>("Diff_curr_targ_vel"); }
		}

		public double? EngineSpeedHighDriveOffFactor
		{
			get { return Body.GetValueOrDefault<double>("EngineSpeedHighDriveOffFactor"); }
		}

		public double? RatingFactorCurrentGear
		{
			get { return Body.GetValueOrDefault<double>("Rating_current_gear"); }
		}

		public TableData AccelerationReserveLookup
		{
			get {
				try {
					return ReadTableData(Body["AccelerationReserveLookup"]?.ToString(), "AccelerationReserveLookup", false);
				} catch (Exception) {
					if (!TolerateMissing) {
						throw;
					}

					return new TableData(
						Path.Combine(BasePath, Body["AccelerationReserveLookup"]?.ToString() ?? "") + MissingFileSuffix,
						DataSourceType.Missing);
				}
			}
		}

		public TableData ShareTorque99L
		{
			get {
				try {
					return ReadTableData(Body["ShareTorque99L"]?.ToString(), "ShareTorque99L", false);
				} catch (Exception) {
					if (!TolerateMissing) {
						throw;
					}

					return new TableData(
						Path.Combine(BasePath, Body["ShareTorque99L"]?.ToString() ?? "") + MissingFileSuffix,
						DataSourceType.Missing);
				}
			}
		}

		public TableData PredictionDurationLookup
		{
			get {
				try {
					return ReadTableData(Body["PredictionDurationLookup"]?.ToString(), "PredictionDurationLookup", false);
				} catch (Exception) {
					if (!TolerateMissing) {
						throw;
					}

					return new TableData(
						Path.Combine(BasePath, Body["PredictionDurationLookup"]?.ToString() ?? "") + MissingFileSuffix,
						DataSourceType.Missing);
				}
			}
		}

		public TableData ShareIdleLow
		{
			get {
				try {
					return ReadTableData(Body["ShareIdleLow"]?.ToString(), "ShareIdleLow", false);
				} catch (Exception) {
					if (!TolerateMissing) {
						throw;
					}

					return new TableData(
						Path.Combine(BasePath, Body["ShareIdleLow"]?.ToString() ?? "") + MissingFileSuffix,
						DataSourceType.Missing);
				}
			}
		}

		public TableData ShareEngineHigh
		{
			get {
				try {
					return ReadTableData(Body["ShareEngineHigh"]?.ToString(), "ShareEngineHigh", false);
				} catch (Exception) {
					if (!TolerateMissing) {
						throw;
					}

					return new TableData(
						Path.Combine(BasePath, Body["ShareEngineHigh"]?.ToString() ?? "") + MissingFileSuffix,
						DataSourceType.Missing);
				}
			}
		}

		public Second DriverAccelerationLookBackInterval { get {return Body.GetValueOrDefault<double>("DriverAccelerationLookBackInterval")?.SI<Second>();} }
		public MeterPerSquareSecond DriverAccelerationThresholdLow { get { return Body.GetValueOrDefault<double>("DriverAccelerationThresholdLow")?.SI<MeterPerSquareSecond>(); } }

		public double? RatioEarlyUpshiftFC
		{
			get {
				return Body.GetValueOrDefault<double>("RatioEarlyUpshiftFC");
			}
		}

		public double? RatioEarlyDownshiftFC
		{
			get {
				return Body.GetValueOrDefault<double>("RatioEarlyDownshiftFC");
			}
		}

		public TableData LoadStageShiftLines
		{
			get {
				try {
					return ReadTableData(Body["LoadStageShiftLines"]?.ToString(), "LoadStageShiftLines", false);
				} catch (Exception) {
					return null;
				}
			}
		}

		public IList<double> LoadStageThresoldsUp
		{
			get { return (Body["LoadStageThresoldsUp"]?.ToString() ?? "").Split(';').Select(x => x.ToDouble(0)).ToList(); }
		}

		public IList<double> LoadStageThresoldsDown
		{
			get {
				return (Body["LoadStageThresoldsDown"]?.ToString() ?? "").Split(';').Select(x => x.ToDouble(0)).ToList();
			}
		}

		#endregion

	}
}
