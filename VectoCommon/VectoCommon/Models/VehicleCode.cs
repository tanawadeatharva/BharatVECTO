namespace TUGraz.VectoCommon.Models {
	public enum VehicleCode
	{
		NOT_APPLICABLE,
		CA,
		CB, 
		CC,
		CD,
		CE,
		CF,
		CG,
		CI,
		CJ
	}

	public static class VehicleCodeHelper
	{
		public static string GetLabel(this VehicleCode self)
		{
			return self.ToString();
		}

		public static string ToXMLFormat(this VehicleCode self)
		{
			return self.ToString();
		}
	}

}