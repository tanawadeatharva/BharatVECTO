using System;

namespace TUGraz.VectoCommon.Models
{
	public enum AlternatorType
	{
		Conventional,
		Smart,
		None
	}

	public static class AlternatorTypeHelper
	{
		public static string GetLabel(this AlternatorType type)
		{
			switch (type) {
				case AlternatorType.Conventional:
					return "Conventional";
				case AlternatorType.Smart:
					return "Smart Alternator";
				case AlternatorType.None:
					return "No Alternator";
				default:
					throw new ArgumentOutOfRangeException(nameof(type), type, null);
			}
		}
	}
}