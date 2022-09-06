using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Models.SimulationComponent.Data;

namespace TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents
{
	public interface IPTODataAdapter
	{
		PTOData CreatePTOTransmissionData(IPTOTransmissionInputData pto);
	}

	public class PTODataAdapterLorry : IPTODataAdapter
	{
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

		#endregion
	}
}