namespace TUGraz.VectoCommon.Models
{
	public enum LegislativeClass
	{
		Unknown,
		N2,
		N3
	}

	public static class LegislativeClassHelper
	{
		public static string GetLabel(this LegislativeClass self)
		{
			return self.ToString();
		}

		public static string ToXMLFormat(this LegislativeClass self)
		{
			return self.ToString();
		}
	}
}