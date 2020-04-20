using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VECTO3GUI.Helper
{
	public static class Extension
	{
		public static object GetNestedPropertyValue(this object currentObj, string name)
		{
			if (currentObj == null || (string.IsNullOrEmpty(name)))
			{
				return null;
			}
			var split = name.Split('.');

			for (int i = 0; i < split.Length; i++)
			{
				var currentPart = split[i];
				if (currentObj == null) { return null; }

				var type = currentObj.GetType();
				var info = type.GetProperty(currentPart);
				if (info == null) { return null; }
				currentObj = info.GetValue(currentObj, null);
			}
			return currentObj;
		}

	}
}
