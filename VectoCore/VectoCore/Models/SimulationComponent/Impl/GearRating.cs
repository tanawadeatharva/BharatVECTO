using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl {

	internal enum GearRatingCase
	{
		A = 1, // inside preferred speed range and inside torque range
		B, // inside engine speed limits, torque demand too high
		C, // valid gear, gear residience time below threshold
		D, // inside engine speed limits, outside preferred speed range
		E // outside engine speed limits
	}


	internal struct GearRating
	{
		public GearRating(GearRatingCase ratingCase, double rating, PerSecond maxEngineSpeed)
		{
			RatingCase = ratingCase;
			Rating = rating;
			MaxEngineSpeed = maxEngineSpeed;
		}

		public double Rating { get; }

		public GearRatingCase RatingCase { get; }
		public PerSecond MaxEngineSpeed { get; set; }

		public double NumericValue
		{
			get { return ((int)RatingCase - 1) * 1e3 + (Rating * 100).LimitTo(0, 1e3-10); }
		}

		public static bool operator <(GearRating first, GearRating second)
		{
			return first.RatingCase < second.RatingCase || (first.RatingCase == second.RatingCase && first.Rating < second.Rating);
		}

		public static bool operator >(GearRating first, GearRating second)
		{
			return first.RatingCase > second.RatingCase || (first.RatingCase == second.RatingCase && first.Rating > second.Rating);
		}

		public override string ToString()
		{
			return string.Format("{0} / {1} ({2})", RatingCase, Rating, NumericValue);
		}
	}
}