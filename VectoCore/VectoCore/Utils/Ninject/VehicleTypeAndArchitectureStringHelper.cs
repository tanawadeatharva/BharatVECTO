using System;
using System.Linq;
using System.Data.Common;
using System.Diagnostics;
using System.Threading;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.CustomerInformationFile.CustomerInformationFile_0_9.CIFWriter;

namespace TUGraz.VectoCore.Utils.Ninject
{
	public interface IVehicleTypeAndArchitectureStringHelperReport
	{
		string GetName(string vehicleType, VectoSimulationJobType jobType, ArchitectureID archId = ArchitectureID.UNKNOWN,
			bool exempted = false, bool iepc = false, bool ihpc = false);

		object[] ToParams(string vehicleType, VectoSimulationJobType jobType, ArchitectureID archId,
			bool exempted, bool iepc, bool ihpc);

		CombineArgumentsToNameInstanceProvider.CombineToName CreateName { get; }
	}

	

	public class VehicleTypeAndArchitectureStringHelperReport : IVehicleTypeAndArchitectureStringHelperReport
	{
		public  CombineArgumentsToNameInstanceProvider.CombineToName CreateName { get; } = (arguments => {

			//may be called with first argument of type string (when defining the bindings) or VehicleCategory when using the factory
			string vehicleType = arguments[0] as string;
			if (arguments[0] is VehicleCategory vehicleCategory) {
				vehicleType = vehicleCategory.GetVehicleType();
			}
			
			
			
			VectoSimulationJobType jobType = (VectoSimulationJobType)arguments[1];
			ArchitectureID archId = (ArchitectureID)arguments[2];
			bool exempted = (bool)arguments[3];
			bool iepc = (bool)arguments[4];
			bool ihpc = (bool)arguments[5];



			string result = "";
			if (exempted) {
				result += exempted + vehicleType;
			} else {
				if (vehicleType == VehicleCategoryHelper.Lorry || vehicleType == VehicleCategoryHelper.PrimaryBus || vehicleType == VehicleCategoryHelper.CompletedBus || vehicleType == VehicleCategoryHelper.Van) {
					if (jobType == VectoSimulationJobType.ParallelHybridVehicle || ihpc) {
						result += "HEV-Px/IHPC";
					}else if (jobType == VectoSimulationJobType.SerialHybridVehicle) {
						result += "HEV-" + archId;
					}else if (jobType == VectoSimulationJobType.BatteryElectricVehicle) {
						if (iepc) {
							result += "PEV-IEPC";
						} else {
							result += "PEV" + archId;
						}
					}else if (jobType == VectoSimulationJobType.ConventionalVehicle) {
						result += "Conventional";
					}
				}else if (vehicleType == VehicleCategoryHelper.CompletedBus) {
					result += jobType;
				}

				result += vehicleType;
			}


			return result;
		});
		public virtual string GetName(string vehicleType, VectoSimulationJobType jobType, ArchitectureID archId = ArchitectureID.UNKNOWN,
			bool exempted = false, bool iepc = false, bool ihpc = false)
		{

			return CreateName(ToParams(vehicleType, jobType, archId, exempted, iepc, ihpc));
		}



		public virtual object[] ToParams(string vehicleType, VectoSimulationJobType jobType, ArchitectureID archId,
			bool exempted, bool iepc, bool ihpc)
		{
			if (!VehicleCategoryHelper.SuperCategories.Contains(vehicleType))
			{
				throw new Exception(
					$"String provided for {nameof(vehicleType)} must match the strings in {nameof(VehicleCategoryHelper.SuperCategories)}");
			};
			return new[] { (object)vehicleType, jobType, archId, exempted, iepc, ihpc };
		}
	}

	public class VehicleTypeAndArchitectureStringHelperRundata
	{
		public CombineArgumentsToNameInstanceProvider.CombineToName CreateName { get; } = arguments => {
			if (arguments[0] is VehicleClassification classification) {
                return GetName(classification);
            } else {
				throw new ArgumentException($"{nameof(arguments)}[0] must be of type {typeof(VehicleClassification)}");
			}
		};

		#region Overrides of VehicleTypeAndArchitectureStringHelperReportBase

		private static string GetName(VehicleClassification classification)
		{
			return classification.GetHashCode().ToString();
		}

		public string GetName(string vehicleType, VectoSimulationJobType jobType, ArchitectureID archId = ArchitectureID.UNKNOWN,
			bool exempted = false, bool iepc = false, bool ihpc = false)
		{
			return GetName(new VehicleClassification(jobType, archId, vehicleType, exempted, iepc, ihpc));
		}

		public string GetName(string vehicleType, bool exempted)
		{
			return GetName(vehicleType, VectoSimulationJobType.ConventionalVehicle, exempted: exempted);
		}

		public string GetSingleBusName(VectoSimulationJobType jobType, ArchitectureID archId, bool exempted = false)
		{
			return GetName(new VehicleClassification(jobType, archId, null, exempted, false, false, true));
		}

		#endregion
		public struct VehicleClassification
		{
			private readonly VectoSimulationJobType _jobType;

			private VectoSimulationJobType JobType
			{
				get
				{
					if (Exempted) {
						return VectoSimulationJobType.ConventionalVehicle; //ignored for exempted vehicles
					}
					return _jobType;
				}
			}

			private readonly ArchitectureID _archId;

			private ArchitectureID ArchId
			{
				get
				{
					if (Exempted) {
						return ArchitectureID.UNKNOWN; //ignored for exempted vehicles
                    }
					return _archId;
				}
			}

			private readonly string _vehicleType;

			private string VehicleType => _vehicleType;

			private readonly bool _exempted;

			private bool Exempted
			{
				get
				{
					return _exempted;
				}
			}

			private readonly bool _iepc;

			private bool Iepc => _iepc;

			private readonly bool _ihpc;

			private bool Ihpc => _ihpc;

			private readonly bool _isSingleBus;
			private const string _singlebus = "SingleBus";
			private bool IsSingleBus => _isSingleBus;

			public VehicleClassification(VectoSimulationJobType jobType, ArchitectureID archId, string vehicleType, bool exempted, bool iepc, bool ihpc, bool singleBus = false)
			{
				_iepc = iepc;
				_ihpc = ihpc;
				_exempted = exempted;
				_vehicleType = singleBus ? _singlebus : vehicleType;
				_archId = archId;
				_jobType = jobType;
				_isSingleBus = singleBus;
			}

			public VehicleClassification(IVehicleDeclarationInputData inputData) : this()
			{
				//Iepc = (inputData.Components?.ElectricMachines?.Entries)?.Count(electric => electric.ElectricMachine.IHPCType != "None") > 0;
				//Ihpc = (inputData.Components?.IEPC != null);
				_iepc = false;
				_ihpc = false;
				_exempted = inputData.ExemptedVehicle;
				_vehicleType = inputData.VehicleCategory.GetVehicleType();
				_archId = inputData.ArchitectureID;
				_jobType = inputData.VehicleType;
				_isSingleBus = false;
			}


			public VehicleClassification(IMultistageVIFInputData inputData) : this()
			{
				//Iepc = (inputData.MultistageJobInputData..Components?.ElectricMachines?.Entries)?.Count(electric => electric.ElectricMachine.IHPCType != "None") > 0;
				//Ihpc = (inputData.Components?.IEPC != null);
				_iepc = false;
				_ihpc = false;
				_exempted = inputData.MultistageJobInputData.JobInputData.PrimaryVehicle.Vehicle.ExemptedVehicle;
				_vehicleType = inputData.MultistageJobInputData.JobInputData.ConsolidateManufacturingStage.Vehicle.VehicleCategory.GetVehicleType();
				_archId = inputData.MultistageJobInputData.JobInputData.PrimaryVehicle.Vehicle.ArchitectureID;
				_jobType = inputData.MultistageJobInputData.JobInputData.PrimaryVehicle.Vehicle.VehicleType;
				_isSingleBus = false;
				//inputData.MultistageJobInputData.JobInputData.ConsolidateManufacturingStage.
			}

			public VehicleClassification(ISingleBusInputDataProvider singleBus) : this()
			{
				_isSingleBus = true;
				_vehicleType = _singlebus;
				_archId = singleBus.PrimaryVehicle.ArchitectureID;
				_jobType = singleBus.PrimaryVehicle.VehicleType;
			}

			public bool Equals(VehicleClassification other)
			{
				return JobType == other.JobType && ArchId == other.ArchId && VehicleType == other.VehicleType && Exempted == other.Exempted && Iepc == other.Iepc && Ihpc == other.Ihpc;
			}

			public override bool Equals(object obj)
			{
				return obj is VehicleClassification other && Equals(other);
			}

			public override int GetHashCode()
			{
				unchecked
				{
					var hashCode = (int)JobType;
					hashCode = (hashCode * 397) ^ (int)ArchId;
					hashCode = (hashCode * 397) ^ (VehicleType != null ? VehicleType.GetHashCode() : 0);
					hashCode = (hashCode * 397) ^ Exempted.GetHashCode();
					hashCode = (hashCode * 397) ^ Iepc.GetHashCode();
					hashCode = (hashCode * 397) ^ Ihpc.GetHashCode();
					return hashCode;
				}
			}

			#region Overrides of ValueType

			public override string ToString()
			{
				return string.Join("\n", VehicleType, JobType, ArchId, Exempted ? "Exempted" : "Non Exempted",
					IsSingleBus ? "Single Bus" : "");
			}

			#endregion
		}
	}
}