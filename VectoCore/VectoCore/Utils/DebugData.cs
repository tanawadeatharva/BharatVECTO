using System.Collections.Generic;
using System.Diagnostics;

namespace TUGraz.VectoCore.Utils
{
	public class DebugData
	{
		private readonly List<dynamic> _data;

		public DebugData()
		{
			#if DEBUG
				_data = new List<dynamic>();
			#endif
		}

		[Conditional("DEBUG")]
		public void Add(dynamic value)
		{
			_data.Add(value);
		}

		public override string ToString()
		{
			#if DEBUG
				return string.Join("\n", _data);
			#else
				return "-";
			#endif
		}
	}
}