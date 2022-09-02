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
				return classification.GetHashCode().ToString();
			} else {
				throw new ArgumentException($"{nameof(arguments)}[0] must be of type {typeof(VehicleClassification)}");
			}
		};

		#region Overrides of VehicleTypeAndArchitectureStringHelperReportBase

		private string GetName(VehicleClassification classification)
		{
			return classification.GetHashCode().ToString();
		}

		public string GetName(string vehicleType, VectoSimulationJobType jobType, ArchitectureID archId = ArchitectureID.UNKNOWN,
			bool exempted = false, bool iepc = false, bool ihpc = false)
		{
			return GetName(new VehicleClassification(jobType, archId, vehicleType, exempted, iepc, ihpc));
		}

		#endregion

		public struct VehicleClassification
		{

			private VectoSimulationJobType JobType { get; }
			private ArchitectureID ArchId { get; }
			private string VehicleType { get; }
			private bool Exempted { get; }
			private bool Iepc { get; }
			private bool Ihpc { get; }

			public VehicleClassification(VectoSimulationJobType jobType, ArchitectureID archId, string vehicleType, bool exempted, bool iepc, bool ihpc)
			{
				Iepc = iepc;
				Ihpc = ihpc;
				Exempted = exempted;
				VehicleType = vehicleType;
				ArchId = archId;
				JobType = jobType;
			}

			public VehicleClassification(IVehicleDeclarationInputData inputData) : this()
			{
				//Iepc = (inputData.Components?.ElectricMachines?.Entries)?.Count(electric => electric.ElectricMachine.IHPCType != "None") > 0;
				//Ihpc = (inputData.Components?.IEPC != null);
				Iepc = false;
				Ihpc = false;
				Exempted = inputData.ExemptedVehicle;
				VehicleType = inputData.VehicleCategory.GetVehicleType();
				ArchId = inputData.ArchitectureID;
				JobType = inputData.VehicleType;
				
			}


			public VehicleClassification(IMultistageVIFInputData inputData) : this()
			{
				//Iepc = (inputData.MultistageJobInputData..Components?.ElectricMachines?.Entries)?.Count(electric => electric.ElectricMachine.IHPCType != "None") > 0;
				//Ihpc = (inputData.Components?.IEPC != null);
				Iepc = false;
				Ihpc = false;
				Exempted = inputData.MultistageJobInputData.JobInputData.PrimaryVehicle.Vehicle.ExemptedVehicle;
				VehicleType = inputData.MultistageJobInputData.JobInputData.ConsolidateManufacturingStage.Vehicle.VehicleCategory.GetVehicleType();
				ArchId = inputData.MultistageJobInputData.JobInputData.PrimaryVehicle.Vehicle.ArchitectureID;
				JobType = inputData.MultistageJobInputData.JobInputData.PrimaryVehicle.Vehicle.VehicleType;

				//inputData.MultistageJobInputData.JobInputData.ConsolidateManufacturingStage.
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
		}
	}
}