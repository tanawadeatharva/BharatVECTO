using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents
{
	public interface IPTODataAdapter
	{
		PTOData CreatePTOTransmissionData(IPTOTransmissionInputData pto);
		PTOData CreateDefaultPTOData();
	}

	public class PTODataAdapterLorry : IPTODataAdapter
	{
		public static PTOData DefaultPTOData()
		{
			return new PTOData()
			{
				TransmissionType = DeclarationData.PTO.DefaultPTOTechnology,
				LossMap = PTOIdleLossMapReader.ReadFromStream(RessourceHelper.ReadStream(DeclarationData.PTO.DefaultPTOIdleLosses)),
				PTOCycle =
					DrivingCycleDataReader.ReadFromStream(RessourceHelper.ReadStream(DeclarationData.PTO.DefaultPTOActivationCycle),
						CycleType.PTO, "PTO", false)
			};
		}
        public PTOData CreateDefaultPTOData()
		{
			return DefaultPTOData();
		}
        public PTOData CreatePTOTransmissionData(IPTOTransmissionInputData pto)
		{
			if (pto != null && pto.PTOTransmissionType != "None")
			{
				return new PTOData
				{
					TransmissionType = pto.PTOTransmissionType,
					LossMap = PTOIdleLossMapReader.GetZeroLossMap(),
				};
			}

			return null;
		}
	}

	public class PTODataAdapterBus : IPTODataAdapter
	{
		#region Implementation of IPTODataAdapter

		public PTOData CreatePTOTransmissionData(IPTOTransmissionInputData pto)
		{
			return null;
		}

		public PTOData CreateDefaultPTOData()
		{
			return null;
		}

		#endregion
	}
}