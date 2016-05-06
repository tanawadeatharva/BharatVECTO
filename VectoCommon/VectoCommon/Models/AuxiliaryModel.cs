namespace TUGraz.VectoCommon.Models
{
	public enum AuxiliaryModel
	{
		Classic,
		Advanced
	}

	public class AuxiliaryModelHelper
	{
		public static AuxiliaryModel Parse(string auxAssemblyStr)
		{
			if (string.IsNullOrEmpty(auxAssemblyStr)) {
				return AuxiliaryModel.Classic;
			}
			switch (auxAssemblyStr) {
				case "BusAuxiliaries":
					return AuxiliaryModel.Advanced;
			}
			return AuxiliaryModel.Classic;
		}
	}
}