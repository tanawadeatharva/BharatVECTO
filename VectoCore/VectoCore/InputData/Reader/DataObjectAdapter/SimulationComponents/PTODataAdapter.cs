using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
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

        public PTOData CreateDefaultPTOData()
		{
			return new PTOData()
			{
				TransmissionType = DeclarationData.PTO.DefaultPTOTechnology,
				LossMap = PTOIdleLossMapReader.ReadFromStream(RessourceHelper.ReadStream(DeclarationData.PTO.DefaultPTOIdleLosses)),
				PTOCycle =
					DrivingCycleDataReader.ReadFromStream(RessourceHelper.ReadStream(DeclarationData.PTO.DefaultPTOActivationCycle),
						CycleType.PTO, "PTO", false),
				TransmissionPowerDemand = null,
				TransmissionPowerDemandElectrical = null
			};
		}
        public PTOData CreatePTOTransmissionData(IPTOTransmissionInputData pto)
		{
			if (pto != null && pto.PTOTransmissionType != "None"){
				var powerDemand = DeclarationData.PTOTransmission.Lookup(pto.PTOTransmissionType).PowerDemand;
				return new PTOData
				{
					TransmissionPowerDemand = powerDemand,
					//TransmissionPowerDemandElectrical = 
					TransmissionType = pto.PTOTransmissionType,
					LossMap = PTOIdleLossMapReader.GetZeroLossMap(),
				};
			}

			return null;
		}
	}

	public class ElectricPTODataAdapter : PTODataAdapterLorry
	{
		public PTOData CreateDefaultPTOData()
		{
			return new PTOData()
			{
				TransmissionType = DeclarationData.PTO.DefaultPTOTechnology, //Consider vehicles with transmission ? 
				LossMap = PTOIdleLossMapReader.GetZeroLossMap(),
				PTOCycle =
					DrivingCycleDataReader.ReadFromStream(RessourceHelper.ReadStream(DeclarationData.PTO.DefaultE_PTOActivationCycle),
						CycleType.EPTO, "PTO", false),
				TransmissionPowerDemand = null,
				TransmissionPowerDemandElectrical = null
			};
		}

		public PTOData CreatePTOTransmissionData(IPTOTransmissionInputData pto, VectoSimulationJobType jobType)
		{
			if (pto != null && pto.PTOTransmissionType != "None")
			{
				var powerDemand = DeclarationData.PTOTransmission.Lookup(pto.PTOTransmissionType).PowerDemand;
				return new PTOData
				{
					TransmissionPowerDemand = powerDemand,
					//TransmissionPowerDemandElectrical = powerDemand * DeclarationData.AlternatorEfficiency / 
					TransmissionType = pto.PTOTransmissionType,
					LossMap = PTOIdleLossMapReader.GetZeroLossMap(),
				};
			}

			return null;
		}
	}


		/// <summary>
		/// Remove this class
		/// </summary>
	public class PTODataAdapterBus : IPTODataAdapter
	{
		public static PTOData DefaultPTOData()
		{
			return null;
		}
		#region Implementation of IPTODataAdapter

		public PTOData CreatePTOTransmissionData(IPTOTransmissionInputData pto)
		{
			return null;
		}

		public PTOData CreateDefaultPTOData()
		{
			return DefaultPTOData();
		}

		#endregion
	}
}