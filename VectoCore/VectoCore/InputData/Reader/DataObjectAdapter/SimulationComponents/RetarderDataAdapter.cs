using System;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents.Interfaces;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.SimulationComponent.Data;

namespace TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents
{
    public class RetarderDataAdapter : IRetarderDataAdapter
	{
		public RetarderData CreateRetarderData(IRetarderInputData retarder, ArchitectureID architecture)
		{
			return SetCommonRetarderData(retarder, architecture);
		}


		private bool TypeValid(RetarderType type, ArchitectureID archId, out string errorMsg)
		{
			var valid = true;
			errorMsg = "";
			if (archId == ArchitectureID.UNKNOWN) {
				//Conventional vehicle
				return true;
				
			}

			switch (type) {
				case RetarderType.None:
					valid = true;
					break;
				case RetarderType.TransmissionInputRetarder:
				case RetarderType.TransmissionOutputRetarder:
					valid = archId.IsParallelHybridVehicle() || archId.IsOneOf(ArchitectureID.P_IHPC, ArchitectureID.S2, ArchitectureID.E2);


					break;
				case RetarderType.EngineRetarder:
					valid = archId.IsParallelHybridVehicle() || archId.IsOneOf(ArchitectureID.P_IHPC);
					break;
				case RetarderType.LossesIncludedInTransmission:
					valid = archId.IsParallelHybridVehicle() || archId.IsOneOf(ArchitectureID.P_IHPC, ArchitectureID.S2, ArchitectureID.S_IEPC, ArchitectureID.E2);
                    break;
				case RetarderType.AxlegearInputRetarder:
					valid = archId.IsOneOf(ArchitectureID.E3, ArchitectureID.S3, ArchitectureID.S_IEPC);
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(type), type, null);
			}


			if (!valid) {
				errorMsg = $"Invalid retardertype for architecture [{type} - {archId}";
			}





			return valid;
		}

		private void SetRatioAndLossMap(IRetarderInputData inputData, RetarderData retarderData)
		{
			switch (inputData.Type) {
				case RetarderType.TransmissionInputRetarder:
				case RetarderType.TransmissionOutputRetarder:
				case RetarderType.AxlegearInputRetarder:
					retarderData.LossMap = RetarderLossMapReader.Create(inputData.LossMap);
					retarderData.Ratio = inputData.Ratio;
					break;
                case RetarderType.LossesIncludedInTransmission:
				case RetarderType.EngineRetarder:
				case RetarderType.None:
					retarderData.Ratio = 1.0;
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		internal RetarderData SetCommonRetarderData(IRetarderInputData retarderInputData,
			ArchitectureID architecture)
		{
			if (!TypeValid(retarderInputData.Type, architecture, out var errorMsg)) {
				throw new VectoException("Error while Reading Retarder Data: {0}", errorMsg);
            }


			var retarder = new RetarderData { Type = retarderInputData?.Type ?? RetarderType.None };
			SetRatioAndLossMap(retarderInputData, retarder);

			if (retarder.Type.IsDedicatedComponent())
			{
					retarder.SavedInDeclarationMode = retarderInputData.SavedInDeclarationMode;
					retarder.Manufacturer = retarderInputData.Manufacturer;
					retarder.ModelName = retarderInputData.Model;
					retarder.Date = retarderInputData.Date;
					retarder.CertificationMethod = retarderInputData.CertificationMethod;
					retarder.CertificationNumber = retarderInputData.CertificationNumber;
					retarder.DigestValueInput = retarderInputData.DigestValue != null ? retarderInputData.DigestValue.DigestValue : "";
			}

			return retarder;
		}
		
	}

	public class GenericRetarderDataAdapter : IRetarderDataAdapter
	{
		private readonly GenericBusRetarderData _genericRetarderData = new GenericBusRetarderData();
		public RetarderData CreateRetarderData(IRetarderInputData retarder, ArchitectureID architecture)
		{
			return _genericRetarderData.CreateGenericBusRetarderData(retarder);
		}
	}
}