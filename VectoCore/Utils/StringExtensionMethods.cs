/*
* Copyright 2015, 2016 Graz University of Technology,
* Institute of Internal Combustion Engines and Thermodynamics,
* Institute of Technical Informatics
*
* Licensed under the EUPL (the "Licence");
* You may not use this work except in compliance with the Licence.
* You may obtain a copy of the Licence at:
*
* http://ec.europa.eu/idabc/eupl
*
* Unless required by applicable law or agreed to in writing, software 
* distributed under the Licence is distributed on an "AS IS" basis,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the Licence for the specific language governing permissions and 
* limitations under the Licence.
*/

using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

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

		public static Stream GetStream(this string self)
		{
			return new MemoryStream(Encoding.UTF8.GetBytes(self));
		}
	}
}