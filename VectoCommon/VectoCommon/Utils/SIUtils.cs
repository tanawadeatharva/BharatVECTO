using System;

namespace TUGraz.VectoCommon.Utils
{
	public struct SIUtils
	{
		public static bool CompareUnits(int[] units1, int[] units2)
		{
			for (var i = 0; i < units1.Length; i++)
			{
				if (units1[i] != units2[i])
				{
					return false;
				}
			}
			return true;
		}

		public static int[] CombineUnits(int[] units1, int[] units2)
		{
			var units = new int[units1.Length];
			for (var i = 0; i < units1.Length; i++)
			{
				units[i] = units1[i] + units2[i];
			}
			return units;

		}

		public static int[] MultiplyUnits(int[] units, int factor)
		{
			var result = new int[units.Length];
			for (var i = 0; i < units.Length; i++)
			{
				if (units[i] != 0)
				{
					result[i] = units[i] * factor;
				}
			}
			return result;
		}
	}


	public struct Unit
	{
		// TODO mk-2017-09-14: must be exact the same definition as in the SI class - find a way to de-duplicate this
		private static readonly string[] Unitnames = { "kg", "m", "s", "A", "K", "mol", "cd" };

		public static UnitInstance SI
		{
			get
			{
				return new UnitInstance(new[] { 0, 0, 0, 0, 0, 0, 0 }, 1, 1, 1);
			}
		}
		
		public static string GetUnitString(int[] siUnitParam, bool isGramm)
		{
			var numerator = "";
			var denominator = "";

			for (var i = 0; i < siUnitParam.Length; i++)
			{
				var currentValue = siUnitParam[i];

				var exponent = Math.Abs(currentValue);
				var exponentStr = "";
				if (currentValue != 0)
				{
					var currentUnit = Unitnames[i];
					if (currentUnit == "kg" && isGramm)
					{
						currentUnit = "g";
					}

					if (exponent > 1)
					{
						exponentStr = "^" + exponent;
					}

					if (currentValue > 0)
					{
						numerator += currentUnit + exponentStr;

					}
					else if (currentValue < 0)
					{
						denominator += currentUnit + exponentStr;
					}
				}
			}
			string result;

			if (numerator == "")
			{
				if (denominator == "")
				{
					result = "-";
				}
				else
				{
					result = "1/" + denominator;
				}
			}
			else
			{
				if (denominator == "")
				{
					result = numerator;
				}
				else
				{
					result = numerator + "/" + denominator;
				}
			}

			return result;
		}
	}

	public struct UnitInstance
	{
		/// <summary>
		/// kg, m, s, A, K, mol, cd
		/// </summary>
		private readonly int[] _units;

		private int _exponent;
		private int _reciproc;

		[Flags]
		public enum IsMass
		{
			IsKilo = 0x01,     //0001
			IsGramm = 0x02,    //0010
			IsKiloGramm = 0x04 //0100
		}
		private IsMass _isMassOption;

		public double Factor { get; private set; }

		public UnitInstance(int[] units, double factor, int exponent, int reciproc)
		{
			_units = units;
			Factor = factor;
			_exponent = exponent;
			_reciproc = reciproc;
			_isMassOption = IsMass.IsKiloGramm;
		}

		public IsMass GetGrammMode()
		{
			return _isMassOption;
		}

		public int[] GetSIUnits()
		{
			return _units;
		}

		/// <summary>
		/// [g] (to basic unit: [kg])
		/// </summary>
		public UnitInstance Gramm
		{
			get
			{
				_units[0] += 1 * _reciproc * _exponent;

				var factor = Math.Pow(1000, _reciproc * _exponent);

				if ((_isMassOption & IsMass.IsKilo) == IsMass.IsKilo)
				{
					//is Kilo -> Kilogramm are selected
					_isMassOption = _isMassOption | ~IsMass.IsGramm;
					_isMassOption = _isMassOption | IsMass.IsKiloGramm;
					Factor /= factor;
				}
				else
				{
					//is not kilo -> Gramm are selected
					_isMassOption = _isMassOption | IsMass.IsGramm;
					_isMassOption = _isMassOption | ~IsMass.IsKiloGramm;
					//factorValue *= factor;
				}
				_isMassOption = _isMassOption & ~IsMass.IsKilo;


				//factorValue /= Math.Pow(1000, exponent * reciproc);
				return this; // not work
			}
		}

		/// <summary>
		/// Takes all following terms as cubic terms (=to the power of 3).
		/// </summary>
		public UnitInstance Cubic
		{
			get
			{
				_isMassOption = _isMassOption & ~IsMass.IsKilo;
				_exponent = 3;
				return this;
			}
		}

		/// <summary>
		/// [s] Converts to/from Second. Internally everything is stored in seconds.
		/// </summary>
		public UnitInstance Hour
		{
			get
			{
				_isMassOption = _isMassOption & ~IsMass.IsKilo;

				var reciprocAndExponent = _reciproc * _exponent;
				_units[2] += 1 * reciprocAndExponent;

				Factor *= Math.Pow(3600, reciprocAndExponent);

				return this;
			}
		}

		/// <summary>
		/// Quantifier for Kilo (1000).
		/// </summary>
		public UnitInstance Kilo
		{
			get
			{
				_isMassOption = _isMassOption | IsMass.IsKilo;
				Factor *= Math.Pow(1000, _exponent * _reciproc);

				return this;
			}
		}

		/// <summary>
		/// [m]
		/// </summary>
		public UnitInstance Meter
		{
			get
			{
				_isMassOption = _isMassOption & ~IsMass.IsKilo;
				_units[1] += 1 * _reciproc * _exponent;
				return this;
			}
		}

		/// <summary>
		/// Quantifier for milli (1/1000).
		/// </summary>
		public UnitInstance Milli
		{
			get
			{
				_isMassOption = _isMassOption & ~IsMass.IsKilo;
				Factor /= Math.Pow(1000, _exponent * _reciproc);
				return this;
			}
		}

		/// <summary>
		/// Quantifier for Centi (1/100)
		/// </summary>
		public UnitInstance Centi
		{
			get
			{
				_isMassOption = _isMassOption & ~IsMass.IsKilo;
				Factor /= Math.Pow(100, _exponent * _reciproc);
				return this;
			}
		}

		/// <summary>
		/// Quantifier for Dezi (1/10)
		/// </summary>
		public UnitInstance Dezi
		{
			get
			{
				_isMassOption = _isMassOption & ~IsMass.IsKilo;
				Factor /= Math.Pow(10, _exponent * _reciproc);
				return this;
			}
		}

		/// <summary>
		/// [s] Converts to/from Second. Internally everything is stored in seconds.
		/// </summary>
		public UnitInstance Minute
		{
			get
			{
				_isMassOption = _isMassOption & ~IsMass.IsKilo;
				var reciprocAndExponent = _reciproc * _exponent;
				_units[2] += 1 * reciprocAndExponent;
				Factor *= Math.Pow(60, reciprocAndExponent);
				return this;
			}
		}

		/// <summary>
		/// [N]
		/// </summary>
		public UnitInstance Newton
		{
			get
			{
				_isMassOption = _isMassOption & ~IsMass.IsKilo;
				var reciprocAndExponent = _reciproc * _exponent;
				_units[0] += 1 * reciprocAndExponent;
				_units[1] += 1 * reciprocAndExponent;
				_units[2] -= 2 * reciprocAndExponent;

				return this;
			}
		}

		/// <summary>
		/// Defines the denominator by the terms following after the Per.
		/// </summary>
		public UnitInstance Per
		{
			get
			{
				_isMassOption = _isMassOption & ~IsMass.IsKilo;
				_exponent = 1;
				_reciproc = _reciproc * -1;
				return this;
			}
		}

		/// <summary>
		/// [-]. Defines radian. Only virtual. Has no real SI unit.
		/// </summary>
		public UnitInstance Radian
		{
			get
			{
				_isMassOption = _isMassOption & ~IsMass.IsKilo;
				return this;
			}
		}

		/// <summary>
		/// [-]. Converts to/from Radiant. Internally everything is stored in radian.
		/// </summary>
		public UnitInstance Rounds
		{
			get
			{
				_isMassOption = _isMassOption & ~IsMass.IsKilo;
				Factor *= Math.Pow(2 * Math.PI, _exponent * _reciproc);
				return this;
			}
		}

		/// <summary>
		/// [s]
		/// </summary>
		public UnitInstance Second
		{
			get
			{
				_isMassOption = _isMassOption & ~IsMass.IsKilo;
				_units[2] += 1 * _reciproc * _exponent;
				return this;
			}
		}

		/// <summary>
		/// Takes all following terms as quadratic terms (=to the power of 2).
		/// </summary>
		public UnitInstance Square
		{
			get
			{
				_isMassOption = _isMassOption & ~IsMass.IsKilo;
				_exponent = 2;
				return this;
			}
		}

		/// <summary>
		/// [t] (to basic unit: [kg])
		/// </summary>
		public UnitInstance Ton
		{
			get
			{
				// remove Gramm and Kilo and KiloGramm is selected.
				_isMassOption = _isMassOption & ~IsMass.IsKilo;
				_isMassOption = _isMassOption | ~IsMass.IsGramm;
				_isMassOption = _isMassOption | IsMass.IsKiloGramm;

				var reciprocAndExponent = _reciproc * _exponent;
				_units[0] += 1 * reciprocAndExponent;
				Factor *= Math.Pow(1000, reciprocAndExponent);

				return this;
			}
		}

		/// <summary>
		/// [W]
		/// </summary>
		public UnitInstance Watt
		{
			get
			{
				_isMassOption = _isMassOption & ~IsMass.IsKilo;

				var reciprocAndExponent = _reciproc * _exponent;
				_units[0] += 1 * reciprocAndExponent;
				_units[1] += 2 * reciprocAndExponent;
				_units[2] -= 3 * reciprocAndExponent;

				return this;
			}
		}
		public UnitInstance Joule
		{
			get
			{
				_isMassOption = _isMassOption & ~IsMass.IsKilo;

				var reciprocAndExponent = _reciproc * _exponent;
				_units[0] += 1 * reciprocAndExponent;
				_units[1] += 2 * reciprocAndExponent;
				_units[2] -= 2 * reciprocAndExponent;

				return this;
			}
		}

		public UnitInstance Liter
		{
			get
			{
				_isMassOption = _isMassOption & ~IsMass.IsKilo;

				var reciprocAndExponent = _reciproc * _exponent;
				_units[1] += 3 * reciprocAndExponent;
				Factor /= Math.Pow(1000, reciprocAndExponent);

				return this;
			}
		}
	}
}
