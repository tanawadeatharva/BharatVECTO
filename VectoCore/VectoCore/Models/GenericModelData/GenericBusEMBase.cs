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
			return VectoCSVFile.ReadStream(RessourceHelper.ReadStream(ressourceId), source: ressourceId);
		}
	}
}
