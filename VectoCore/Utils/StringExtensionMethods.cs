using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace TUGraz.VectoCore.Utils
{
	public static class StringExtensionMethods
	{
		public static string Join<T>(this string s, IEnumerable<T> values)
		{
			return string.Join(s, values);
		}

		public static double ToDouble(this string self)
		{
			return double.Parse(self, CultureInfo.InvariantCulture);
		}

	    public static double IndulgentParse(this string self)
	    {
	        return double.Parse(new string(self.Trim().TakeWhile(c => char.IsDigit(c) || c == '.').ToArray()), CultureInfo.InvariantCulture);
	    }
	}
}