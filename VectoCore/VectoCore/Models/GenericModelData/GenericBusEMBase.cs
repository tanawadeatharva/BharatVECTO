using System.IO;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.GenericModelData
{
	public abstract class GenericBusEMBase
	{
		#region Constants

		public const string MotorSpeedNorm = "n_norm";
		public const string TorqueNorm = "T_norm";
		public const string PowerElectricalNorm = "Pel_norm";
		
		
		protected string GenericEfficiencyMap_ASM  {get; set;}
		protected string GenericEfficiencyMap_PSM  {get; set;}

		#endregion

		protected TableData GetNormalizedEfficiencyMap(ElectricMachineType electricMachineType)
		{
			switch (electricMachineType)
			{
				case ElectricMachineType.ASM:
				case ElectricMachineType.ESM:
				case ElectricMachineType.RM:
					return ReadCsvResource(GenericEfficiencyMap_ASM);
				case ElectricMachineType.PSM:
					return ReadCsvResource(GenericEfficiencyMap_PSM);
				default:
					return null;
			}
		}

		protected TableData ReadCsvResource(string ressourceId)
		{
			var tmp = ressourceId.Replace(DeclarationData.DeclarationDataResourcePrefix + ".", "");
			var parts = tmp.Split('.');
			var fileName = Path.Combine("Declaration", string.Join(".", parts[parts.Length - 2], parts[parts.Length - 1]));
			if (File.Exists(fileName))
			{
				return VectoCSVFile.Read(fileName);
			}

			return VectoCSVFile.ReadStream(RessourceHelper.ReadStream(ressourceId), source: ressourceId);
		}
	}
}
