using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TUGraz.VectoCommon.Models
{
	public enum RegistrationClass
	{
		I,
		I_II,
		II,
		II_III,
		III,
		A,
		B,
		unknown
	}
	
	public static class RegistrationClassHelper
	{
		public static string GetLabel(this RegistrationClass self)
		{
			switch (self) {
				case RegistrationClass.I_II:
					return "I+II";
				case RegistrationClass.II_III:
					return "II+III";
				default:
					return self.ToString();
			}
		}

		public static RegistrationClass[] Parse(string registrationClasses)
		{
			var classes = registrationClasses.Split('/');
			if (classes.Length > 0) {
				var result = new List<RegistrationClass>();
				for (int i = 0; i < classes.Length; i++) {

					var regClass = ParseRegistrationClassString(classes[i]);
					if(regClass != RegistrationClass.unknown)
						result.Add(regClass);
				}
				return result.ToArray();
			}
			return null;
		}
		
		private static RegistrationClass ParseRegistrationClassString(string registrationClass)
		{
			if (RegistrationClass.I.GetLabel() == registrationClass)
				return RegistrationClass.I;

			if (RegistrationClass.II.GetLabel() == registrationClass)
				return RegistrationClass.II;

			if (RegistrationClass.III.GetLabel() == registrationClass)
				return RegistrationClass.III;

			if (RegistrationClass.II_III.GetLabel() == registrationClass)
				return RegistrationClass.II_III;

			if (RegistrationClass.I_II.GetLabel() == registrationClass)
				return RegistrationClass.I_II;

			if (RegistrationClass.A.GetLabel() == registrationClass)
				return RegistrationClass.A;

			if (RegistrationClass.B.GetLabel() == registrationClass)
				return RegistrationClass.B;

			return RegistrationClass.unknown;
		}
	}
}
