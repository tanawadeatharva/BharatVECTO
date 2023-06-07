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
		public RetarderData CreateRetarderData(IRetarderInputData retarder, PowertrainPosition position = PowertrainPosition.HybridPositionNotSet)
		{
			return SetCommonRetarderData(retarder);
		}
		internal static RetarderData SetCommonRetarderData(IRetarderInputData retarderInputData,
			PowertrainPosition position = PowertrainPosition.HybridPositionNotSet)
		{
			try
			{
				var retarder = new RetarderData { Type = retarderInputData?.Type ?? RetarderType.None};

				switch (retarder.Type)
				{
					case RetarderType.TransmissionInputRetarder:
					case RetarderType.TransmissionOutputRetarder:
						if (!(position.IsParallelHybrid() || position.IsOneOf(PowertrainPosition.HybridPositionNotSet, PowertrainPosition.BatteryElectricE2)))
						{
							throw new ArgumentException("Transmission retarder is only allowed in powertrains that " +
														"contain a gearbox: Conventional, HEV-P, and PEV-E2.", nameof(retarder));
						}

						retarder.LossMap = RetarderLossMapReader.Create(retarderInputData.LossMap);
						retarder.Ratio = retarderInputData.Ratio;
						break;

					case RetarderType.AxlegearInputRetarder:
						if (position != PowertrainPosition.BatteryElectricE3)
							throw new ArgumentException("AxlegearInputRetarder is only allowed for PEV-E3, HEV-S3, S-IEPC, E-IEPC. ", nameof(retarder));
						retarder.LossMap = RetarderLossMapReader.Create(retarderInputData.LossMap);
						retarder.Ratio = retarderInputData.Ratio;
						break;

					case RetarderType.None:
					case RetarderType.LossesIncludedInTransmission:
					case RetarderType.EngineRetarder:
						retarder.Ratio = 1;
						break;

					default:
						throw new ArgumentOutOfRangeException(nameof(retarder), retarder.Type, "RetarderType unknown");
				}

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
			catch (Exception e)
			{
				throw new VectoException("Error while Reading Retarder Data: {0}", e.Message);
			}
		}
	}

	public class GenericRetarderDataAdapter : IRetarderDataAdapter
	{
		private readonly GenericBusRetarderData _genericRetarderData = new GenericBusRetarderData();
		public  RetarderData CreateRetarderData(IRetarderInputData retarder, PowertrainPosition position = PowertrainPosition.HybridPositionNotSet)
		{
			return _genericRetarderData.CreateGenericBusRetarderData(retarder);
		}
	}
}