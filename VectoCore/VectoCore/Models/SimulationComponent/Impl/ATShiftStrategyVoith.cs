using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Engine;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
	public class ATShiftStrategyVoith : ATShiftStrategy
	{
		public const double DownhillSlope = -5;
		public const double UphillSlope = 5;

		protected ShiftStrategyParameters shiftParameters;

		protected internal Dictionary<int, ShiftLineSet> UpshiftLines = new Dictionary<int, ShiftLineSet>();
		protected internal Dictionary<int, ShiftLineSet> DownshiftLines = new Dictionary<int, ShiftLineSet>();
		protected Kilogram MinMass;
		protected Kilogram MaxMass;
		protected EngineFullLoadCurve FullLoadCurve;
		protected MeterPerSquareSecond _accMin = 0.SI<MeterPerSquareSecond>();
		private MeterPerSquareSecond _accMax = 0.SI<MeterPerSquareSecond>();
		private int _loadStage;
		private List<SchmittTrigger> LoadStageSteps;
		private Radian roadGradient;
		private MeterPerSquareSecond driverAcceleration;

		public ATShiftStrategyVoith(VectoRunData data, IDataBus dataBus) : base(data, dataBus)
		{
			shiftParameters = data.GearshiftParameters;
			InitializeShiftLines(shiftParameters.GearshiftLines);
			LoadStageSteps = new List<SchmittTrigger>();
			foreach (var entry in shiftParameters.LoadstageThresholds) {
				LoadStageSteps.Add(new SchmittTrigger(entry));
			}

			MinMass = data.VehicleData.MinimumVehicleMass;
			MaxMass = data.VehicleData.MaximumVehicleMass;
			FullLoadCurve = data.EngineData.FullLoadCurves[0];
		}

		private void InitializeShiftLines(TableData lines)
		{
			var slopeDh = VectoMath.InclinationToAngle(DownhillSlope / 100.0);
			var slopeLevel = VectoMath.InclinationToAngle(0);
			var slopeUh = VectoMath.InclinationToAngle(UphillSlope / 100.0);

			foreach (DataRow row in lines.Rows) {
				var shift = row[ShiftLinesColumns.Shift].ToString().Split('-');
				var g1 = shift[0].ToInt();
				var g2 = shift[1].ToInt();
				var upshift = true;
				if (g1 + 1 == g2) {
					// upshift
					upshift = true;
				} else if (g2 + 1 == g1) {
					// downshift
					upshift = false;
				} else {
					throw new VectoException("invalid shift entry: {0}-{1}", g1, g2);
				}

				var loadStage = row[ShiftLinesColumns.LoadStage].ToString().ToInt();

				var nDhAmaxLower = row.Field<string>(ShiftLinesColumns.nDhAmaxLower).ToDouble().RPMtoRad();
				var nLevelAmaxLower = row.Field<string>(ShiftLinesColumns.nLevelAmaxLower).ToDouble()
										.RPMtoRad();
				var nUhAmaxLower = row.Field<string>(ShiftLinesColumns.nUhAmaxLower).ToDouble().RPMtoRad();

				var nDhAminLower = GetAlternativeIfEmpty(row, ShiftLinesColumns.nDhAminLower, ShiftLinesColumns.nDhAmaxLower)
					.RPMtoRad();
				var nLevelAminLower = GetAlternativeIfEmpty(
					row, ShiftLinesColumns.nLevelAminLower, ShiftLinesColumns.nLevelAmaxLower).RPMtoRad();
				var nUhAminLower = GetAlternativeIfEmpty(row, ShiftLinesColumns.nUhAminLower, ShiftLinesColumns.nUhAmaxLower)
					.RPMtoRad();

				var nDhAminUpper = GetAlternativeIfEmpty(
						row, ShiftLinesColumns.nDhAminUpper, ShiftLinesColumns.nDhAminLower, ShiftLinesColumns.nDhAmaxLower)
					.RPMtoRad();
				var nLevelAminUpper = GetAlternativeIfEmpty(
						row, ShiftLinesColumns.nLevelAminUpper, ShiftLinesColumns.nLevelAminLower, ShiftLinesColumns.nLevelAmaxLower)
					.RPMtoRad();
				var nUhAminUpper = GetAlternativeIfEmpty(
						row, ShiftLinesColumns.nUhAminUpper, ShiftLinesColumns.nUhAminLower, ShiftLinesColumns.nUhAmaxLower)
					.RPMtoRad();

				var nDhAmaxUpper = GetAlternativeIfEmpty(row, ShiftLinesColumns.nDhAmaxUpper, ShiftLinesColumns.nDhAmaxLower)
					.RPMtoRad();
				var nLevelAmaxUpper = GetAlternativeIfEmpty(
						row, ShiftLinesColumns.nLevelAmaxUpper, ShiftLinesColumns.nLevelAmaxLower)
					.RPMtoRad();
				var nUhAmaxUpper = GetAlternativeIfEmpty(row, ShiftLinesColumns.nUhAmaxUpper, ShiftLinesColumns.nUhAmaxLower)
					.RPMtoRad();

				ShiftLineSet shiftLineSet;
				if (upshift) {
					if (!UpshiftLines.ContainsKey(g1)) {
						UpshiftLines[g1] = new ShiftLineSet();
					}
					shiftLineSet = UpshiftLines[g1];
					if (shiftLineSet.LoadStages.ContainsKey(loadStage)) {
						throw new VectoException(
							"Gearshift entries for upshift {0}-{1} load stage {2} already defined!", g1, g2, loadStage);
					}
				} else {
					if (!DownshiftLines.ContainsKey(g1)) {
						DownshiftLines[g1] = new ShiftLineSet();
					}
					shiftLineSet = DownshiftLines[g1];
					if (shiftLineSet.LoadStages.ContainsKey(loadStage)) {
						throw new VectoException(
							"Gearshift entries for downshift {0}-{1} load stage {2} already defined!", g1, g2, loadStage);
					}
				}

				var entry = new ShiftLines();

				entry.LowerBound.entriesAMin.Add(Tuple.Create(slopeDh, nDhAminLower));
				entry.LowerBound.entriesAMin.Add(Tuple.Create(slopeLevel, nLevelAminLower));
				entry.LowerBound.entriesAMin.Add(Tuple.Create(slopeUh, nUhAminLower));

				entry.LowerBound.entriesAMax.Add(Tuple.Create(slopeDh, nDhAmaxLower));
				entry.LowerBound.entriesAMax.Add(Tuple.Create(slopeLevel, nLevelAmaxLower));
				entry.LowerBound.entriesAMax.Add(Tuple.Create(slopeUh, nUhAmaxLower));

				entry.UpperBound.entriesAMin.Add(Tuple.Create(slopeDh, nDhAminUpper));
				entry.UpperBound.entriesAMin.Add(Tuple.Create(slopeLevel, nLevelAminUpper));
				entry.UpperBound.entriesAMin.Add(Tuple.Create(slopeUh, nUhAminUpper));

				entry.UpperBound.entriesAMax.Add(Tuple.Create(slopeDh, nDhAmaxUpper));
				entry.UpperBound.entriesAMax.Add(Tuple.Create(slopeLevel, nLevelAmaxUpper));
				entry.UpperBound.entriesAMax.Add(Tuple.Create(slopeUh, nUhAmaxUpper));

				shiftLineSet.LoadStages[loadStage] = entry;
			}
		}

		private double GetAlternativeIfEmpty(DataRow row, string col1, string col2, string col3)
		{
			return string.IsNullOrWhiteSpace(row[col1].ToString())
				? (string.IsNullOrWhiteSpace(row[col2].ToString())
					? row.Field<string>(col3).ToDouble()
					: row.Field<string>(col2).ToDouble())
				: row.Field<string>(col1).ToDouble();
		}

		private double GetAlternativeIfEmpty(DataRow row, string col1, string col2)
		{
			return string.IsNullOrWhiteSpace(row[col1].ToString())
				? row.Field<string>(col2).ToDouble()
				: row.Field<string>(col1).ToDouble();
		}

		public new static string Name
		{
			get { return "AT shift strategy Voith proposal"; }
		}


		protected Watt EstimateAccelerrationPower(PerSecond gbxOutSpeed, NewtonMeter gbxOutTorque)
		{
			var vehicleSpeed = DataBus.VehicleSpeed;
			var avgSlope =
			((DataBus.CycleLookAhead(Constants.SimulationSettings.GearboxLookaheadForAccelerationEstimation).Altitude -
			DataBus.Altitude) / Constants.SimulationSettings.GearboxLookaheadForAccelerationEstimation).Value().SI<Radian>();

			var airDragLoss = DataBus.AirDragResistance(vehicleSpeed, vehicleSpeed) * DataBus.VehicleSpeed;
			var rollResistanceLoss = DataBus.RollingResistance(avgSlope) * DataBus.VehicleSpeed;

			//DataBus.GearboxLoss();
			var slopeLoss = DataBus.SlopeResistance(avgSlope) * DataBus.VehicleSpeed;
			var axleLoss = DataBus.AxlegearLoss();

			return gbxOutSpeed * gbxOutTorque - axleLoss - airDragLoss - rollResistanceLoss - slopeLoss;
		}

		#region Overrides of ATShiftStrategy

		public override void Request(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity)
		{
			driverAcceleration = DataBus.DriverAcceleration;
			roadGradient = DataBus.RoadGradient;
			base.Request(absTime, dt, outTorque, outAngularVelocity);
		}

		public override bool ShiftRequired(
			Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, NewtonMeter inTorque,
			PerSecond inAngularVelocity, uint gear, Second lastShiftTime, IResponse response)
		{
			var accPower = EstimateAccelerrationPower(outAngularVelocity, outTorque);

			_accMin = (accPower / DataBus.VehicleSpeed / (MaxMass + DataBus.ReducedMassWheels)).Cast<MeterPerSquareSecond>();
			_accMax = (accPower / DataBus.VehicleSpeed / (MinMass + DataBus.ReducedMassWheels)).Cast<MeterPerSquareSecond>();

			//var engineLoadPercent = inTorque / FullLoadCurve.FullLoadStationaryTorque(inAngularVelocity);
			var engineLoadPercent = inTorque / response.EngineDynamicFullLoadTorque;
			_loadStage = GetLoadStage(engineLoadPercent);

			return base.ShiftRequired(
				absTime, dt, outTorque, outAngularVelocity, inTorque, inAngularVelocity, gear, lastShiftTime, response);
		}

		private int GetLoadStage(double engineLoadPercent)
		{
			var sum = 1;
			foreach (var entry in LoadStageSteps) {
				sum += entry.GetOutput(engineLoadPercent * 100);
			}

			return sum;
		}


		protected override bool CheckUpshift(
			Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, NewtonMeter inTorque,
			PerSecond inAngularVelocity, uint gear, Second lastShiftTime)
		{
			var shiftTimeReached = (absTime - lastShiftTime).IsGreaterOrEqual(ModelData.ShiftTime);
			if (!shiftTimeReached) {
				return false;
			}

			var currentGear = ModelData.Gears[gear];
			if (gear >= ModelData.Gears.Keys.Max()) {
				return false;
			}

			var nextGear = _gearbox.TorqueConverterLocked ? gear + 1 : gear;

			var shiftSpeed = UpshiftLines[(int)gear].LookupShiftSpeed(
				_loadStage, DataBus.RoadGradient, DataBus.DriverAcceleration, _accMin, _accMax);
			var shiftSpeedGbxOut = shiftSpeed / ModelData.Gears[nextGear].Ratio;
			if (outAngularVelocity > shiftSpeedGbxOut) {
				Upshift(absTime, gear);
				return true;
			}

			return false;
		}


		protected override bool CheckDownshift(
			Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, NewtonMeter inTorque,
			PerSecond inAngularVelocity, uint gear, Second lastShiftTime)
		{
			var shiftTimeReached = (absTime - lastShiftTime).IsGreaterOrEqual(ModelData.ShiftTime);
			if (!shiftTimeReached) {
				return false;
			}

			if (gear == 1) {
				return false;
			}

			var shiftSpeed = DownshiftLines[(int)gear].LookupShiftSpeed(
				_loadStage, DataBus.RoadGradient, DataBus.DriverAcceleration, -0.4.SI<MeterPerSquareSecond>(),
				-0.2.SI<MeterPerSquareSecond>());
			if (inAngularVelocity < shiftSpeed) {
				Downshift(absTime, gear);
				return true;
			}

			return false;
		}

		#region Overrides of BaseShiftStrategy

		public override void WriteModalResults(IModalDataContainer container)
		{
			container.SetDataValue("loadStage", _loadStage);
			container.SetDataValue("accMin", _accMin.Value());
			container.SetDataValue("accMax", _accMax.Value());

			if (_loadStage == 0 || roadGradient == null) {
				container.SetDataValue("S12_UPS", 0);
				container.SetDataValue("S23_UPS", 0);
				container.SetDataValue("S34_UPS", 0);
			} else {
				container.SetDataValue(
					"S12_UPS", UpshiftLines[1].LookupShiftSpeed(_loadStage, roadGradient, driverAcceleration, _accMin, _accMax).AsRPM);
				container.SetDataValue(
					"S23_UPS", UpshiftLines[2].LookupShiftSpeed(_loadStage, roadGradient, driverAcceleration, _accMin, _accMax).AsRPM);
				container.SetDataValue(
					"S34_UPS", UpshiftLines[3].LookupShiftSpeed(_loadStage, roadGradient, driverAcceleration, _accMin, _accMax).AsRPM);
			}
			base.WriteModalResults(container);
		}

		#endregion

		#endregion

		protected class ShiftLinesColumns
		{
			public const string Shift = "shift";
			public const string LoadStage = "loadstage";

			public const string nDhAminLower = "n_dh_amin_lower";
			public const string nLevelAminLower = "n_level_amin_lower";
			public const string nUhAminLower = "n_uh_amin_lower";

			public const string nDhAmaxLower = "n_dh_amax_lower";
			public const string nLevelAmaxLower = "n_level_amax_lower";
			public const string nUhAmaxLower = "n_uh_amax_lower";

			public const string nDhAminUpper = "n_dh_amin_upper";
			public const string nLevelAminUpper = "n_level_amin_upper";
			public const string nUhAminUpper = "n_uh_amin_upper";

			public const string nDhAmaxUpper = "n_dh_amax_upper";
			public const string nLevelAmaxUpper = "n_level_amax_upper";
			public const string nUhAmaxUpper = "n_uh_amax_upper";
		}
	}

	public class ShiftLineSet
	{
		public Dictionary<int, ShiftLines> LoadStages = new Dictionary<int, ShiftLines>();

		public PerSecond LookupShiftSpeed(
			int loadStage, Radian gradient, MeterPerSquareSecond acceleration, MeterPerSquareSecond aMin,
			MeterPerSquareSecond aMax)
		{
			if (!LoadStages.ContainsKey(loadStage)) {
				throw new VectoException("No Shiftlines for load stage {0} found", loadStage);
			}

			var shiftLinesSet = LoadStages[loadStage];

			//var slope = (Math.Tan(gradient.Value()) * 100).LimitTo(
			//	ATShiftStrategyVoith.DownhillSlope, ATShiftStrategyVoith.UphillSlope);

			gradient = gradient.LimitTo(
				VectoMath.InclinationToAngle(ATShiftStrategyVoith.DownhillSlope),
				VectoMath.InclinationToAngle(ATShiftStrategyVoith.UphillSlope));
			var shiftSpeedsLower = shiftLinesSet.LowerBound.LookupShiftSpeed(gradient);
			var shiftSpeedsUpper = shiftLinesSet.UpperBound.LookupShiftSpeed(gradient);
			var acc = aMin > aMax ? acceleration.LimitTo(aMax, aMin) : acceleration.LimitTo(aMin, aMax);

			var shiftSpeed1 = VectoMath.Interpolate(
				aMin, aMax, shiftSpeedsLower.ShiftSpeedAMin, shiftSpeedsLower.ShiftSpeedAMax, acc);
			var shiftSpeed2 = VectoMath.Interpolate(
				aMin, aMax, shiftSpeedsUpper.ShiftSpeedAMin, shiftSpeedsUpper.ShiftSpeedAMax, acc);

			return (shiftSpeed1 + shiftSpeed2) / 2.0;
		}
	}

	public class ShiftLines
	{
		public readonly ShiftSpeeds LowerBound = new ShiftSpeeds();
		public readonly ShiftSpeeds UpperBound = new ShiftSpeeds();
	}

	public class ShiftSpeeds
	{
		//private Tuple<Radian, Tuple<PerSecond, PerSecond>>[] entries;

		internal readonly List<Tuple<Radian, PerSecond>> entriesAMin = new List<Tuple<Radian, PerSecond>>();
		internal readonly List<Tuple<Radian, PerSecond>> entriesAMax = new List<Tuple<Radian, PerSecond>>();


		public ShiftSpeedTuple LookupShiftSpeed(Radian gradent)
		{
			var sectLow = entriesAMin.GetSection(x => x.Item1 < gradent);
			var sectHigh = entriesAMax.GetSection(x => x.Item1 < gradent);

			return new ShiftSpeedTuple(
				VectoMath.Interpolate(sectLow.Item1.Item1, sectLow.Item2.Item1, sectLow.Item1.Item2, sectLow.Item2.Item2, gradent),
				VectoMath.Interpolate(
					sectHigh.Item1.Item1, sectHigh.Item2.Item1, sectHigh.Item1.Item2, sectHigh.Item2.Item2, gradent));
		}
	}

	public class ShiftSpeedTuple
	{
		public PerSecond ShiftSpeedAMin { get; }
		public PerSecond ShiftSpeedAMax { get; }

		public ShiftSpeedTuple(PerSecond shiftSpeedAMin, PerSecond shiftSpeedAMax)
		{
			ShiftSpeedAMin = shiftSpeedAMin;
			ShiftSpeedAMax = shiftSpeedAMax;
		}
	}
}
