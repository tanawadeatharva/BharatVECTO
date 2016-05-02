using System.Collections.Generic;
using System.Diagnostics;

namespace TUGraz.VectoCore.Utils
{
	public class DebugData
	{
		internal readonly List<dynamic> Data;

		public DebugData()
		{
#if DEBUG
			Data = new List<dynamic>();
#endif
		}

		[Conditional("DEBUG")]
		public void Add(dynamic value)
		{
			Data.Add(value);
		}

		public override string ToString()
		{
#if DEBUG
			return string.Join("\n", Data);
#else
				return "-";
			#endif
		}
	}
}