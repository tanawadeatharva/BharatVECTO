using System;
using System.Data;
using System.Linq;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.Declaration
{
	public sealed class FuelData : LookupData<FuelType, FuelData.Entry>
	{
		private static FuelData _instance;

		public static FuelData Instance()
		{
			return _instance ?? (_instance = new FuelData());
		}

		private FuelData() : base() {}

		protected override string ResourceId
		{
			get { return DeclarationData.DeclarationDataResourcePrefix + ".FuelTypes.csv"; }
		}

		protected override string ErrorMessage
		{
			get { throw new InvalidOperationException("ErrorMessage not applicable."); }
		}

		public static Entry Diesel
		{
			get { return Instance().Lookup(FuelType.DieselCI); }
		}

		protected override void ParseData(DataTable table)
		{
			Data = table.Rows.Cast<DataRow>()
				.Select(r => {
					var density = r.Field<string>("fueldensity");
					return new Entry(
						r.Field<string>(0).ParseEnum<FuelType>(),
						string.IsNullOrWhiteSpace(density) ? null : density.ToDouble(0).SI<KilogramPerCubicMeter>(),
						r.ParseDouble("co2perfuelweight"),
						r.ParseDouble("lowerheatingvalue").SI().Kilo.Joule.Per.Kilo.Gramm.Cast<JoulePerKilogramm>()
						);
				})
				.ToDictionary(e => e.FuelType);
		}

		public FuelType[] GetFuelTypes()
		{
			return Data.Keys.ToArray();
		}

		public class Entry
		{
			public Entry(FuelType type, KilogramPerCubicMeter density, double weight, JoulePerKilogramm heatingValue)
			{
				FuelType = type;
				FuelDensity = density;
				CO2PerFuelWeight = weight;
				LowerHeatingValue = heatingValue;
			}

			public FuelType FuelType { get; private set; }

			public KilogramPerCubicMeter FuelDensity { get; private set; }

			public double CO2PerFuelWeight { get; private set; }

			public JoulePerKilogramm LowerHeatingValue { get; private set; }
		}
	}
}