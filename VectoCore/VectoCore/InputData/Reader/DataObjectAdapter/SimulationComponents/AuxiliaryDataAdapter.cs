using System;
using System.Collections.Generic;
using System.Linq;
using Ninject.Infrastructure.Language;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.PrimaryBus;
using TUGraz.VectoCore.InputData.Reader.Impl.DeclarationMode.HeavyLorryRunDataFactory;
using TUGraz.VectoCore.Models.BusAuxiliaries;
using TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.Electrics;
using TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.HVAC;
using TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.Pneumatics;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents
{
	public interface IAuxiliaryDataAdapter
	{
		IList<VectoRunData.AuxData> CreateAuxiliaryData(IAuxiliariesDeclarationInputData auxInputData,
			IBusAuxiliariesDeclarationData busAuxData, MissionType mission, VehicleClass hvdClass, Meter vehicleLength,
			int? numSteeredAxles, VectoSimulationJobType jobType);
	}

	public interface ICompletedBusAuxiliaryDataAdapter : IPrimaryBusAuxiliaryDataAdapter
	{
		IAuxiliaryConfig CreateBusAuxiliariesData(Mission mission, IVehicleDeclarationInputData primaryVehicle,
			IVehicleDeclarationInputData completedVehicle, VectoRunData runData);
	}

	public interface IPrimaryBusAuxiliaryDataAdapter : IAuxiliaryDataAdapter
	{
		AuxiliaryConfig CreateBusAuxiliariesData(Mission mission, IVehicleDeclarationInputData primaryVehicle, VectoRunData runData);
		
		//IDictionary<string, AuxiliaryDataAdapter.ElectricConsumerEntry> GetElectricConsumers(Mission mission, IVehicleDeclarationInputData completedVehicle, IActuations actuations, VehicleClass vehicleClass);
		
		//ElectricsUserInputsConfig GetDefaultElectricalUserConfig();
		
		//double CalculateAlternatorEfficiency(IList<IAlternatorDeclarationInputData> alternators);
		
		//IPneumaticsConsumersDemand CreatePneumaticAuxConfig(RetarderType retarderType);
		
		//SSMInputs GetDefaulSSMInputs(IFuelProperties heatingFuel);
		
		//TechnologyBenefits SelectBenefitForFloorType(FloorType floorType, List<SSMTechnology> onVehicle);
	}


	public abstract class AuxiliaryDataAdapter : ComponentDataAdapterBase, IAuxiliaryDataAdapter
	{
		public class ElectricConsumerEntry
		{

			public ElectricConsumerEntry()
			{
				ActiveDuringEngineStopStandstill = true;
				ActiveDuringEngineStopDriving = true;
			}

			public bool ActiveDuringEngineStopDriving { get; set; }

			public bool ActiveDuringEngineStopStandstill { get; set; }

			public bool BaseVehicle { get; set; }
			public Ampere Current { get; set; }
		}
		public IList<VectoRunData.AuxData> CreateAuxiliaryData(IAuxiliariesDeclarationInputData auxInputData,
			IBusAuxiliariesDeclarationData busAuxData, 
			MissionType mission, 
			VehicleClass hvdClass, 
			Meter vehicleLength,
			int? numSteeredAxles, 
			VectoSimulationJobType jobType)
		{
			CheckDeclarationMode(auxInputData, "AuxiliariesData");
			return DoCreateAuxiliaryData(auxInputData, busAuxData, mission, hvdClass, vehicleLength, numSteeredAxles, jobType);
		}

		public abstract AuxiliaryConfig CreateBusAuxiliariesData(Mission mission, IVehicleDeclarationInputData primaryVehicle, VectoRunData runData);

		protected abstract IList<VectoRunData.AuxData> DoCreateAuxiliaryData(
			IAuxiliariesDeclarationInputData auxInputData,
			IBusAuxiliariesDeclarationData busAuxData, MissionType mission, VehicleClass hdvClass, Meter vehicleLength,
			int? numSteeredAxles, VectoSimulationJobType jobType);

		protected static bool CreateConditioningAux(VectoSimulationJobType jobType)
		{
			if (jobType == VectoSimulationJobType.ConventionalVehicle) {
				return false;
			} else {
				return true;
			}
		}
	}

	public class HeavyLorryPEVAuxiliaryDataAdapter : HeavyLorryAuxiliaryDataAdapter
	{
		protected internal override HashSet<AuxiliaryType> AuxiliaryTypes { get; } = new HashSet<AuxiliaryType>() {
			AuxiliaryType.ElectricSystem,
			AuxiliaryType.HVAC,
			AuxiliaryType.PneumaticSystem,
			AuxiliaryType.SteeringPump
		};

		protected override string errorStringVehicleType => "battery electric";
	}

	public class HeavyLorryAuxiliaryDataAdapter : AuxiliaryDataAdapter
	{
		protected internal virtual HashSet<AuxiliaryType> AuxiliaryTypes { get; } = new HashSet<AuxiliaryType>() {
			AuxiliaryType.ElectricSystem,
			AuxiliaryType.HVAC,
			AuxiliaryType.PneumaticSystem,
			AuxiliaryType.Fan,
			AuxiliaryType.SteeringPump
		};
		protected virtual string errorStringVehicleType => "conventional/hybrid";

		#region Overrides of AuxiliaryDataAdapter

		public override AuxiliaryConfig CreateBusAuxiliariesData(Mission mission, IVehicleDeclarationInputData primaryVehicle, VectoRunData runData)
		{
			throw new System.NotImplementedException();
		}

		

		protected override IList<VectoRunData.AuxData> DoCreateAuxiliaryData(
			IAuxiliariesDeclarationInputData auxInputData, IBusAuxiliariesDeclarationData busAuxData,
			MissionType mission, VehicleClass hdvClass, Meter vehicleLength, int? numSteeredAxles,
			VectoSimulationJobType jobType)
		{
			var retVal = new List<VectoRunData.AuxData>();

			if (!new HashSet<AuxiliaryType>(auxInputData.Auxiliaries.Select(aux => aux.Type)).SetEquals(AuxiliaryTypes)) {
				var error = string.Format(
					"In Declaration Mode exactly {0} Auxiliaries must be defined for {2} vehicles: {1}",
					AuxiliaryTypes.Count, string.Join(", ", AuxiliaryTypes.Select(aux => aux.ToString())), errorStringVehicleType);
				Log.Error(error);
				throw new VectoException(
					error);
			}

			var alternatorEfficiency = DeclarationData.AlternatorEfficiency;
			

			foreach (var auxType in AuxiliaryTypes)
			{
				var auxData = auxInputData.Auxiliaries.FirstOrDefault(a => a.Type == auxType);
				if (auxData == null)
				{
					throw new VectoException("Auxiliary {0} not found.", auxType);
				}

				var aux = new VectoRunData.AuxData
				{
					DemandType = AuxiliaryDemandType.Constant,
					Technology = auxData.Technology,
					MissionType = mission,
				};

				mission = mission.GetNonEMSMissionType();
				switch (auxType)
				{
					case AuxiliaryType.Fan:
						AddFan(mission, hdvClass, jobType, auxData, aux, alternatorEfficiency, retVal);
						break;
					case AuxiliaryType.SteeringPump:
						AddSteeringPumps(mission, hdvClass, numSteeredAxles, jobType, auxData, alternatorEfficiency, aux, retVal);
						break;
					case AuxiliaryType.HVAC:
						AddHVAC(mission, jobType, hdvClass, aux, auxData, 1/DeclarationData.HVACElectricEfficiencyFactor, retVal);
						break;
					case AuxiliaryType.PneumaticSystem:
						AddPneumaticSystem(mission, jobType, auxData, alternatorEfficiency, aux,retVal);
						break;
					case AuxiliaryType.ElectricSystem:
						AddElectricSystem(mission, hdvClass, jobType, aux, auxData, alternatorEfficiency, retVal);
						break;
					default: continue;
				}
			}

			if (CreateConditioningAux(jobType)) {
				AddConditioning(mission, jobType, retVal, hdvClass);
			}

			return retVal;
		}

		private static void AddConditioning(MissionType mission, VectoSimulationJobType jobType, List<VectoRunData.AuxData> auxDataList, VehicleClass hdv)
		{


			var aux = new VectoRunData.AuxData()
			{
				IsFullyElectric = true,
				MissionType = mission,
				DemandType = AuxiliaryDemandType.Dynamic,
				ID = Constants.Auxiliaries.IDs.Cond,
				ConnectToREESS = true,
				PowerDemandElectric = DeclarationData.Conditioning.LookupPowerDemand(hdv, mission),
			};

			auxDataList.Add(aux);
		}


		private static void AddElectricSystem(MissionType mission, VehicleClass hdvClass,
			VectoSimulationJobType vectoSimulationJobType,
			VectoRunData.AuxData aux,
			IAuxiliaryDeclarationInputData auxData, double alternatorEfficiency, List<VectoRunData.AuxData> auxDataList)
		{
			aux.PowerDemandMech = DeclarationData.ElectricSystem.Lookup(hdvClass, mission, auxData.Technology.FirstOrDefault()).PowerDemand;
			aux.ID = Constants.Auxiliaries.IDs.ElectricSystem;
			aux.PowerDemandElectric = aux.PowerDemandMech * alternatorEfficiency;
			aux.ConnectToREESS = vectoSimulationJobType.IsOneOf(VectoSimulationJobType.BatteryElectricVehicle,
				VectoSimulationJobType.SerialHybridVehicle, VectoSimulationJobType.IEPC_E, VectoSimulationJobType.IEPC_S);
			auxDataList.Add(aux);
		}

		private static void AddPneumaticSystem(MissionType mission, VectoSimulationJobType jobType,
			IAuxiliaryDeclarationInputData auxData, double alternatorEfficiency, VectoRunData.AuxData aux,
			List<VectoRunData.AuxData> auxDataList)
		{
			if (!DeclarationData.PneumaticSystem.IsApplicable(jobType,
					auxData.Technology.FirstOrDefault())) {
				throw new VectoException(
					$"Pneumatic system technology'{auxData.Technology.FirstOrDefault()}' is not applicable for '{jobType}'");
			}

			aux.PowerDemandMech = DeclarationData.PneumaticSystem.Lookup(mission, auxData.Technology.FirstOrDefault())
				.PowerDemand;
			aux.ID = Constants.Auxiliaries.IDs.PneumaticSystem;
			aux.IsFullyElectric = DeclarationData.PneumaticSystem.IsFullyElectric(auxData.Technology.FirstOrDefault());
			if (aux.IsFullyElectric) {
				aux.PowerDemandElectric =
					DeclarationData.PneumaticSystem.GetElectricPowerDemand(mission,
						auxData.Technology.FirstOrDefault());
			} else {
				aux.PowerDemandElectric = aux.PowerDemandMech * alternatorEfficiency;
			}

			aux.ConnectToREESS = aux.IsFullyElectric && jobType.IsOneOf(
				VectoSimulationJobType.BatteryElectricVehicle, 
				VectoSimulationJobType.SerialHybridVehicle, 
				VectoSimulationJobType.IEPC_E, 
				VectoSimulationJobType.IEPC_S,
				VectoSimulationJobType.ParallelHybridVehicle);
			auxDataList.Add(aux);
		}

		private static void AddHVAC(MissionType mission, VectoSimulationJobType vectoSimulationJobType,
			VehicleClass hdvClass, VectoRunData.AuxData aux,
			IAuxiliaryDeclarationInputData auxData, double efficiency, List<VectoRunData.AuxData> auxDataList)
		{
			aux.PowerDemandMech = DeclarationData.HeatingVentilationAirConditioning.Lookup(
				mission,
				auxData.Technology.FirstOrDefault(), hdvClass).PowerDemand;
			aux.ID = Constants.Auxiliaries.IDs.HeatingVentilationAirCondition;
			aux.PowerDemandElectric = aux.PowerDemandMech * efficiency;

			aux.ConnectToREESS = (vectoSimulationJobType == VectoSimulationJobType.BatteryElectricVehicle ||
								vectoSimulationJobType == VectoSimulationJobType.SerialHybridVehicle);
			auxDataList.Add(aux);
			return;
		}

		private static void AddSteeringPumps(MissionType mission, VehicleClass hdvClass, int? numSteeredAxles,
			VectoSimulationJobType jobType, IAuxiliaryDeclarationInputData auxData, double alternatorEfficiency,
			VectoRunData.AuxData aux,
			List<VectoRunData.AuxData> auxDataList)
		{
			

			if (!DeclarationData.SteeringPump.IsApplicable(auxData.Technology, jobType)) {
				throw new VectoException(
					$"At least one steering pump technology of '{string.Join(",", auxData.Technology)}' is not applicable for '{jobType}'");
			}

			if (numSteeredAxles.HasValue && auxData.Technology.Count != numSteeredAxles.Value) {
				throw new VectoException(
					$"Number of steering pump technologies does not match number of steered axles ({numSteeredAxles.Value}, {auxData.Technology.Count})");
			}

			
			var powerDemand = DeclarationData.SteeringPump.Lookup(mission, hdvClass, auxData.Technology);
			var spMech = new VectoRunData.AuxData
			{
				DemandType = AuxiliaryDemandType.Constant,
				Technology = auxData.Technology.Where(tech => !DeclarationData.SteeringPump.IsFullyElectric(tech))
					.ToList(),
				IsFullyElectric = false,
				ID = Constants.Auxiliaries.IDs.SteeringPump,
				PowerDemandMech = powerDemand.mechanicalPumps,

				MissionType = mission,
			};


			
		
			if (powerDemand.electricPumps.IsGreater(0))
			{
				var spElectric = new VectoRunData.AuxData
				{
					DemandType = AuxiliaryDemandType.Constant,
					Technology = auxData.Technology.Where(tech => DeclarationData.SteeringPump.IsFullyElectric(tech))
						.ToList(),
					IsFullyElectric = true,
					ConnectToREESS = true,
					ID = Constants.Auxiliaries.IDs.SteeringPump_el,
					PowerDemandElectric = powerDemand.electricPumps * alternatorEfficiency,
					PowerDemandMech = powerDemand.electricPumps,
					MissionType = mission,
				};

				
	

				if (jobType.IsOneOf(VectoSimulationJobType.ConventionalVehicle,
						VectoSimulationJobType.EngineOnlySimulation)) {
					//For a conventional vehicle the electric steering pump power demand is added to the power demand of the mechanical steering pumpss
					spElectric.ConnectToREESS = false;
					spMech.PowerDemandMech += spElectric.PowerDemandMech;
				} else {
					//For a vehicle with REESS the electric part of the steering pump power demand is treated as separate component
					auxDataList.Add(spElectric);
				}
			}
			

			if (spMech.PowerDemandMech.IsGreater(0))
			{
				auxDataList.Add(spMech);
			}


		}

		private static void AddFan(MissionType mission, VehicleClass hdvClass, VectoSimulationJobType jobType,
			IAuxiliaryDeclarationInputData auxData, VectoRunData.AuxData aux, double alternatorEfficiency,
			List<VectoRunData.AuxData> auxDataList)
		{
			if (!DeclarationData.Fan.IsApplicable(hdvClass, jobType, auxData.Technology.FirstOrDefault())) {
				throw new VectoException(
					$"Fan technology '{auxData.Technology.FirstOrDefault()}' is not applicable for '{jobType}'");
			}





			aux.PowerDemandMech = DeclarationData.Fan.LookupPowerDemand(hdvClass, mission, auxData.Technology.FirstOrDefault());
			aux.ID = Constants.Auxiliaries.IDs.Fan;
			aux.IsFullyElectric = DeclarationData.Fan.IsFullyElectric(hdvClass, auxData.Technology.FirstOrDefault());
			aux.PowerDemandElectric = aux.PowerDemandMech * alternatorEfficiency;
			aux.ConnectToREESS = aux.IsFullyElectric && (!jobType.IsOneOf(VectoSimulationJobType.ConventionalVehicle,
				VectoSimulationJobType.EngineOnlySimulation));
			auxDataList.Add(aux);
		}
		#endregion
	}

	public class PrimaryBusAuxiliaryDataAdapter : AuxiliaryDataAdapter, IPrimaryBusAuxiliaryDataAdapter
	{
		public ElectricsUserInputsConfig GetDefaultElectricalUserConfig()
		{
			return new ElectricsUserInputsConfig()
			{
				PowerNetVoltage = Constants.BusAuxiliaries.ElectricSystem.PowernetVoltage,
				StoredEnergyEfficiency = Constants.BusAuxiliaries.ElectricSystem.StoredEnergyEfficiency,
				ResultCardIdle = new DummyResultCard(),
				ResultCardOverrun = new DummyResultCard(),
				ResultCardTraction = new DummyResultCard(),
				AlternatorGearEfficiency = Constants.BusAuxiliaries.ElectricSystem.AlternatorGearEfficiency,
				DoorActuationTimeSecond = Constants.BusAuxiliaries.ElectricalConsumers.DoorActuationTimeSecond,
			};
		}

		private double GetNumberOfElectricalConsumersForMission(Mission mission, ElectricalConsumer consumer,
			IVehicleDeclarationInputData vehicleData)
		{
			if (consumer.ConsumerName.Equals(Constants.BusAuxiliaries.ElectricalConsumers.DoorsPerVehicleConsumer,
					StringComparison.CurrentCultureIgnoreCase))
			{
				var count = DeclarationData.BusAuxiliaries.DefaultElectricConsumerList.Items.First(x =>
					x.ConsumerName.Equals(
						Constants.BusAuxiliaries.ElectricalConsumers.DoorsPerVehicleConsumer,
						StringComparison.CurrentCultureIgnoreCase)).NumberInActualVehicle.ToDouble();

				switch (vehicleData.DoorDriveTechnology)
				{
					case ConsumerTechnology.Electrically:
						return count;
					case ConsumerTechnology.Mixed:
						return count * 0.5;
					default:
						return 0;
				}
			}

			return mission.BusParameter.ElectricalConsumers.GetVECTOValueOrDefault(consumer.ConsumerName, 0);
		}

		protected virtual Dictionary<string, ElectricConsumerEntry> GetDefaultElectricConsumers(
			Mission mission, IVehicleDeclarationInputData vehicleData, IActuations actuations)
		{
			var retVal = new Dictionary<string, ElectricConsumerEntry>();
			var doorDutyCycleFraction =
				(actuations.ParkBrakeAndDoors * Constants.BusAuxiliaries.ElectricalConsumers.DoorActuationTimeSecond) /
				actuations.CycleTime;
			var busAux = vehicleData.Components.BusAuxiliaries;
			var electricDoors = vehicleData.DoorDriveTechnology == ConsumerTechnology.Electrically ||
								vehicleData.DoorDriveTechnology == ConsumerTechnology.Mixed;

			foreach (var consumer in DeclarationData.BusAuxiliaries.DefaultElectricConsumerList.Items)
			{
				var applied = consumer.DefaultConsumer || consumer.Bonus
					? 1.0
					: GetNumberOfElectricalConsumersForMission(mission, consumer, vehicleData);
				var nbr = consumer.DefaultConsumer
					? GetNumberOfElectricalConsumersInVehicle(consumer.NumberInActualVehicle, mission, vehicleData)
					: 1.0;

				var dutyCycle = electricDoors && consumer.ConsumerName.Equals(
					Constants.BusAuxiliaries.ElectricalConsumers.DoorsPerVehicleConsumer,
					StringComparison.CurrentCultureIgnoreCase)
					? doorDutyCycleFraction
					: consumer.PhaseIdleTractionOn;

				var current = applied * consumer.NominalCurrent(mission.MissionType) * dutyCycle * nbr;
				if (consumer.Bonus && !VehicleHasElectricalConsumer(consumer.ConsumerName, busAux))
				{
					current = 0.SI<Ampere>();
				}

				retVal[consumer.ConsumerName] = new ElectricConsumerEntry
				{
					BaseVehicle = consumer.BaseVehicle,
					Current = current
				};
			}

			return retVal;
		}

		protected virtual bool VehicleHasElectricalConsumer(string consumerName, IBusAuxiliariesDeclarationData busAux)
		{
			switch (consumerName)
			{
				case "Day running lights LED bonus":
				case "Position lights LED bonus":
				case "Brake lights LED bonus": // return false;
				case "Interior lights LED bonus":
				case "Headlights LED bonus": return true;
				default: return false;
			}
		}
		protected virtual double CalculateLengthDependentElectricalConsumers(Mission mission, IVehicleDeclarationInputData vehicleData)
		{
			var busParams = mission.BusParameter;
			return DeclarationData.BusAuxiliaries.CalculateLengthInteriorLights(
					busParams.VehicleLength, busParams.VehicleCode, busParams.NumberPassengersLowerDeck)
				.Value();
		}

		protected virtual double GetNumberOfElectricalConsumersInVehicle(string nbr, Mission mission, IVehicleDeclarationInputData vehicleData)
		{
			if ("f_IntLight(L_CoC)".Equals(nbr, StringComparison.InvariantCultureIgnoreCase))
			{
				return CalculateLengthDependentElectricalConsumers(mission, vehicleData);
			}

			return nbr.ToDouble();
		}

		public IDictionary<string, ElectricConsumerEntry> GetElectricConsumers(Mission mission, IVehicleDeclarationInputData vehicleData, IActuations actuations, VehicleClass vehicleClass)
		{
			var retVal = GetDefaultElectricConsumers(mission, vehicleData, actuations);

			foreach (var entry in GetElectricAuxConsumers(mission, vehicleData, vehicleClass, vehicleData.Components.BusAuxiliaries))
			{
				retVal[entry.Key] = entry.Value;
			}

			return retVal;
		}
		protected virtual Dictionary<string, ElectricConsumerEntry> GetElectricAuxConsumers(Mission mission, IVehicleDeclarationInputData vehicleData, VehicleClass vehicleClass, IBusAuxiliariesDeclarationData busAux)
		{
			var retVal = new Dictionary<string, ElectricConsumerEntry>();
			var spPower = DeclarationData.SteeringPumpBus.LookupElectricalPowerDemand(
				mission.MissionType, busAux.SteeringPumpTechnology,
				vehicleData.Length ?? mission.BusParameter.VehicleLength);
			retVal[Constants.Auxiliaries.IDs.SteeringPump] = new ElectricConsumerEntry
			{
				ActiveDuringEngineStopStandstill = false,
				BaseVehicle = false,
				Current = spPower / Constants.BusAuxiliaries.ElectricSystem.PowernetVoltage
			};

			var fanPower = DeclarationData.Fan.LookupElectricalPowerDemand(
				vehicleClass, mission.MissionType, busAux.FanTechnology);
			retVal[Constants.Auxiliaries.IDs.Fan] = new ElectricConsumerEntry
			{
				ActiveDuringEngineStopStandstill = false,
				ActiveDuringEngineStopDriving = false,
				BaseVehicle = false,
				Current = fanPower / Constants.BusAuxiliaries.ElectricSystem.PowernetVoltage
			};
			return retVal;
		}
		public double CalculateAlternatorEfficiency(IList<IAlternatorDeclarationInputData> alternators)
		{
			return DeclarationData.BusAuxiliaries.AlternatorTechnologies.Lookup("default");
		}

		private ElectricsUserInputsConfig GetElectricalUserConfig(
			Mission mission, IVehicleDeclarationInputData vehicleData, IActuations actuations, VehicleClass vehicleClass)
		{
			var currentDemand = GetElectricConsumers(mission, vehicleData, actuations, vehicleClass);
			var busAux = vehicleData.Components.BusAuxiliaries;

			var retVal = GetDefaultElectricalUserConfig();

			retVal.AlternatorType = busAux.ElectricSupply.AlternatorTechnology;
			retVal.ElectricalConsumers = currentDemand;
			retVal.AlternatorMap = new SimpleAlternator(CalculateAlternatorEfficiency(busAux.ElectricSupply.Alternators));

			switch (retVal.AlternatorType)
			{
				case AlternatorType.Smart when busAux.ElectricSupply.Alternators.Count == 0:
					throw new VectoException("at least one alternator is required when specifying smart electrics!");
				case AlternatorType.Smart when busAux.ElectricSupply.ElectricStorage.Count == 0:
					throw new VectoException("at least one electric storage (battery or capacitor) is required when specifying smart electrics!");
			}

			retVal.MaxAlternatorPower = busAux.ElectricSupply.Alternators.Sum(x => x.RatedVoltage * x.RatedCurrent);
			retVal.ElectricStorageCapacity = busAux.ElectricSupply.ElectricStorage.Sum(x => x.ElectricStorageCapacity) ?? 0.SI<WattSecond>();

			return retVal;
		}
		protected virtual PneumaticUserInputsConfig GetPneumaticUserConfig(IVehicleDeclarationInputData vehicleData, Mission mission)
		{
			var busAux = vehicleData.Components.BusAuxiliaries;

			//throw new NotImplementedException();
			return new PneumaticUserInputsConfig()
			{
				KneelingHeight = VectoMath.Max(0.SI<Meter>(), mission.BusParameter.EntranceHeight - Constants.BusParameters.EntranceHeight),
				CompressorGearEfficiency = Constants.BusAuxiliaries.PneumaticUserConfig.CompressorGearEfficiency,
				CompressorGearRatio = busAux.PneumaticSupply.Ratio,
				CompressorMap = DeclarationData.BusAuxiliaries.GetCompressorMap(busAux.PneumaticSupply.CompressorSize, busAux.PneumaticSupply.Clutch),
				SmartAirCompression = busAux.PneumaticSupply.SmartAirCompression,
				SmartRegeneration = busAux.PneumaticSupply.SmartRegeneration,
				AdBlueDosing = busAux.PneumaticConsumers.AdBlueDosing,
				Doors = ConsumerTechnology.Pneumatically,
				AirSuspensionControl = busAux.PneumaticConsumers.AirsuspensionControl,
			};
		}
		public IPneumaticsConsumersDemand CreatePneumaticAuxConfig(RetarderType retarderType)
		{
			return new PneumaticsConsumersDemand()
			{
				AdBlueInjection = Constants.BusAuxiliaries.PneumaticConsumersDemands.AdBlueInjection,
				AirControlledSuspension = Constants.BusAuxiliaries.PneumaticConsumersDemands.AirControlledSuspension,
				Braking = retarderType == RetarderType.None
					? Constants.BusAuxiliaries.PneumaticConsumersDemands.BrakingNoRetarder
					: Constants.BusAuxiliaries.PneumaticConsumersDemands.BrakingWithRetarder,
				BreakingWithKneeling = Constants.BusAuxiliaries.PneumaticConsumersDemands.BreakingAndKneeling,
				DeadVolBlowOuts = Constants.BusAuxiliaries.PneumaticConsumersDemands.DeadVolBlowOuts,
				DeadVolume = Constants.BusAuxiliaries.PneumaticConsumersDemands.DeadVolume,
				NonSmartRegenFractionTotalAirDemand =
					Constants.BusAuxiliaries.PneumaticConsumersDemands.NonSmartRegenFractionTotalAirDemand,
				SmartRegenFractionTotalAirDemand =
					Constants.BusAuxiliaries.PneumaticConsumersDemands.SmartRegenFractionTotalAirDemand,
				OverrunUtilisationForCompressionFraction =
					Constants.BusAuxiliaries.PneumaticConsumersDemands.OverrunUtilisationForCompressionFraction,
				DoorOpening = Constants.BusAuxiliaries.PneumaticConsumersDemands.DoorOpening,
				StopBrakeActuation = Constants.BusAuxiliaries.PneumaticConsumersDemands.StopBrakeActuation,
			};
		}



		public virtual SSMInputs CreateSSMModelParameters(IBusAuxiliariesDeclarationData busAuxInputData,
			Mission mission,
			LoadingType loadingType, BusHVACSystemConfiguration applicableHVACConfiguration,
			HeatPumpType driverHeatpumpType, HeatPumpType passengerHeatpumpType, Watt auxHeaterPower,
			IFuelProperties heatingFuel, bool cooling)
		{
			var busParams = mission.BusParameter;
			
			var isDoubleDecker = busParams.VehicleCode.IsDoubleDeckerBus();
			var internalLength = applicableHVACConfiguration == BusHVACSystemConfiguration.Configuration2
				? 2 * Constants.BusParameters.DriverCompartmentLength // OK
				: DeclarationData.BusAuxiliaries.CalculateInternalLength(busParams.VehicleLength,
				busParams.VehicleCode, 10); // missing: correction length for low floor buses
			var ventilationLength = DeclarationData.BusAuxiliaries.CalculateInternalLength(busParams.VehicleLength,
				busParams.VehicleCode, 10);
			var internalHeight = DeclarationData.BusAuxiliaries.CalculateInternalHeight(mission.BusParameter.VehicleCode, RegistrationClass.II, busParams.BodyHeight);
			var coolingPower = CalculateMaxCoolingPower(mission, applicableHVACConfiguration);
			var heatingPower = CalculateMaxHeatingPower(mission, applicableHVACConfiguration);

			var retVal = GetDefaulSSMInputs(heatingFuel);
			retVal.BusFloorType = busParams.VehicleCode.GetFloorType();
			retVal.Technologies = GetSSMTechnologyBenefits(busAuxInputData, mission.BusParameter.VehicleCode.GetFloorType());

			retVal.FuelFiredHeaterPower = auxHeaterPower;
			retVal.BusWindowSurface = DeclarationData.BusAuxiliaries.WindowHeight(busParams.DoubleDecker) * internalLength +
									DeclarationData.BusAuxiliaries.FrontAndRearWindowArea(busParams.DoubleDecker);
			retVal.BusSurfaceArea = 2 * (internalLength * busParams.VehicleWidth + internalLength * internalHeight +
										(isDoubleDecker ? 2.0 : 1.0) * busParams.VehicleWidth * busParams.BodyHeight);
			retVal.BusVolumeVentilation = ventilationLength * busParams.VehicleWidth * internalHeight;

			retVal.UValue = DeclarationData.BusAuxiliaries.UValue(busParams.VehicleCode.GetFloorType());
			retVal.NumberOfPassengers =
				DeclarationData.BusAuxiliaries.CalculateBusFloorSurfaceArea(internalLength, busParams.VehicleWidth) *
				(loadingType == LoadingType.LowLoading ? mission.BusParameter.PassengerDensityLow : mission.BusParameter.PassengerDensityRef) *
				(loadingType == LoadingType.LowLoading ? mission.MissionType.GetLowLoadFactorBus() : 1.0) + 1; // add driver for 'heat input'
			retVal.VentilationRate = DeclarationData.BusAuxiliaries.VentilationRate(applicableHVACConfiguration, false);
			retVal.VentilationRateHeating = DeclarationData.BusAuxiliaries.VentilationRate(applicableHVACConfiguration, true);

			//retVal.HVACMaxCoolingPower = coolingPower.Item1 + coolingPower.Item2;
			retVal.HVACMaxCoolingPowerDriver = coolingPower.Item1;
			retVal.HVACMaxCoolingPowerPassenger = coolingPower.Item2;
			retVal.MaxHeatingPowerDriver = heatingPower.Item1;
			retVal.MaxHeatingPowerPassenger = heatingPower.Item2;

			retVal.HeatPumpTypeDriverCompartment = driverHeatpumpType;
			retVal.HeatPumpTypePassengerCompartment = passengerHeatpumpType;

			retVal.HVACSystemConfiguration = applicableHVACConfiguration;

			//retVal.HVACCompressorType = passengerHeatpumpType; // use passenger compartment

			if (cooling) {
				retVal.DriverCompartmentLength = applicableHVACConfiguration.RequiresDriverAC()
					? applicableHVACConfiguration.IsOneOf(BusHVACSystemConfiguration.Configuration2,
						BusHVACSystemConfiguration.Configuration4)
						? 2 * Constants.BusParameters.DriverCompartmentLength
						: Constants.BusParameters.DriverCompartmentLength
					: 0.SI<Meter>();
				retVal.PassengerCompartmentLength = applicableHVACConfiguration.RequiresPassengerAC()
					? applicableHVACConfiguration.IsOneOf(BusHVACSystemConfiguration.Configuration2,
						BusHVACSystemConfiguration.Configuration4)
						? 0.SI<Meter>()
						: internalLength - Constants.BusParameters.DriverCompartmentLength
					: 0.SI<Meter>();
			} else {
				retVal.DriverCompartmentLength = applicableHVACConfiguration.RequiresDriverAC()
					? Constants.BusParameters.DriverCompartmentLength
					: 0.SI<Meter>();
				retVal.PassengerCompartmentLength = applicableHVACConfiguration.RequiresDriverAC()
					? internalLength - Constants.BusParameters.DriverCompartmentLength
					: internalLength;
			}

			return retVal;
		}

		private HVACParameters GetHVACParams(VectoSimulationJobType vehicleType, BusParameters busParams)
		{
			switch (vehicleType) {
				case VectoSimulationJobType.ConventionalVehicle:
					return busParams.HVACConventional;
				case VectoSimulationJobType.ParallelHybridVehicle:
				case VectoSimulationJobType.SerialHybridVehicle:
				case VectoSimulationJobType.IEPC_S:
				case VectoSimulationJobType.IHPC:
					return busParams.HVACHEV;
				case VectoSimulationJobType.BatteryElectricVehicle:
				case VectoSimulationJobType.IEPC_E:
					return busParams.HVACPEV;
				default:
					throw new ArgumentOutOfRangeException(nameof(vehicleType), vehicleType, null);
			}
		}

		protected virtual TechnologyBenefits GetSSMTechnologyBenefits(IBusAuxiliariesDeclarationData inputData, FloorType floorType)
		{
			var onVehicle = new List<SSMTechnology>();
			foreach (var item in DeclarationData.BusAuxiliaries.SSMTechnologyList)
			{
				if ("Adjustable coolant thermostat".Equals(item.BenefitName, StringComparison.InvariantCultureIgnoreCase) &&
					(inputData?.HVACAux.AdjustableCoolantThermostat ?? false))
				{
					onVehicle.Add(item);
				}

				if ("Engine waste gas heat exchanger".Equals(item.BenefitName, StringComparison.InvariantCultureIgnoreCase) &&
					(inputData?.HVACAux.EngineWasteGasHeatExchanger ?? false))
				{
					onVehicle.Add(item);
				}
			}

			return SelectBenefitForFloorType(floorType, onVehicle);
		}

		public virtual TechnologyBenefits SelectBenefitForFloorType(FloorType floorType, List<SSMTechnology> onVehicle)
		{
			var retVal = new TechnologyBenefits();

			switch (floorType)
			{
				case FloorType.LowFloor:
					retVal.CValueVariation = onVehicle.Sum(x => x.LowFloorC);
					retVal.HValueVariation = onVehicle.Sum(x => x.LowFloorH);
					retVal.VCValueVariation = onVehicle.Sum(x => x.ActiveVC ? x.LowFloorV : 0);
					retVal.VHValueVariation = onVehicle.Sum(x => x.ActiveVH ? x.LowFloorV : 0);
					retVal.VVValueVariation = onVehicle.Sum(x => x.ActiveVV ? x.LowFloorV : 0);
					break;
				case FloorType.HighFloor:
					retVal.CValueVariation = onVehicle.Sum(x => x.RaisedFloorC);
					retVal.HValueVariation = onVehicle.Sum(x => x.RaisedFloorH);
					retVal.VCValueVariation = onVehicle.Sum(x => x.ActiveVC ? x.RaisedFloorV : 0);
					retVal.VHValueVariation = onVehicle.Sum(x => x.ActiveVH ? x.RaisedFloorV : 0);
					retVal.VVValueVariation = onVehicle.Sum(x => x.ActiveVV ? x.RaisedFloorV : 0);
					break;
				case FloorType.SemiLowFloor:
					retVal.CValueVariation = onVehicle.Sum(x => x.SemiLowFloorC);
					retVal.HValueVariation = onVehicle.Sum(x => x.SemiLowFloorH);
					retVal.VCValueVariation = onVehicle.Sum(x => x.ActiveVC ? x.SemiLowFloorV : 0);
					retVal.VHValueVariation = onVehicle.Sum(x => x.ActiveVH ? x.SemiLowFloorV : 0);
					retVal.VVValueVariation = onVehicle.Sum(x => x.ActiveVV ? x.SemiLowFloorV : 0);
					break;
			}

			return retVal;
		}

		public SSMInputs GetDefaulSSMInputs(IFuelProperties heatingFuel)
		{
			return new SSMInputs(null, heatingFuel)
			{
				DefaultConditions = new EnvironmentalConditionMapEntry(
					Constants.BusAuxiliaries.SteadyStateModel.DefaultTemperature,
					Constants.BusAuxiliaries.SteadyStateModel.DefaultSolar,
					1.0),
				EnvironmentalConditionsMap = DeclarationData.BusAuxiliaries.DefaultEnvironmentalConditions,
				HeatingBoundaryTemperature = Constants.BusAuxiliaries.SteadyStateModel.HeatingBoundaryTemperature,
				CoolingBoundaryTemperature = Constants.BusAuxiliaries.SteadyStateModel.CoolingBoundaryTemperature,

				SpecificVentilationPower = Constants.BusAuxiliaries.SteadyStateModel.SpecificVentilationPower,

				AuxHeaterEfficiency = Constants.BusAuxiliaries.SteadyStateModel.AuxHeaterEfficiency,
				FuelEnergyToHeatToCoolant = Constants.BusAuxiliaries.Heater.FuelEnergyToHeatToCoolant,
				CoolantHeatTransferredToAirCabinHeater = Constants.BusAuxiliaries.Heater.CoolantHeatTransferredToAirCabinHeater,
				ElectricWasteHeatToCoolant = Constants.BusAuxiliaries.Heater.ElectricWasteHeatToCoolant,
				GFactor = Constants.BusAuxiliaries.SteadyStateModel.GFactor,

				VentilationOnDuringHeating = true,
				VentilationWhenBothHeatingAndACInactive = true,
				VentilationDuringAC = true,

				MaxPossibleBenefitFromTechnologyList =
					Constants.BusAuxiliaries.SteadyStateModel.MaxPossibleBenefitFromTechnologyList,
			};
		}

		protected virtual Tuple<Watt, Watt> CalculateMaxCoolingPower(Mission mission, BusHVACSystemConfiguration hvacConfiguration)
		{
			var busParams = mission.BusParameter;

			var length = DeclarationData.BusAuxiliaries.CalculateInternalLength(
				busParams.VehicleLength, busParams.VehicleCode,
				busParams.NumberPassengersLowerDeck);
			var height = DeclarationData.BusAuxiliaries.CalculateInternalHeight(busParams.VehicleCode, RegistrationClass.II, busParams.BodyHeight);
			var volume = length * height * busParams.VehicleWidth;

			var driver = DeclarationData.BusAuxiliaries.HVACMaxCoolingPower.DriverMaxCoolingPower(
				hvacConfiguration, mission.MissionType);
			var passenger = DeclarationData.BusAuxiliaries.HVACMaxCoolingPower.PassengerMaxCoolingPower(
				hvacConfiguration, mission.MissionType, volume);

			return Tuple.Create(driver, passenger);
		}

		protected virtual Tuple<Watt, Watt> CalculateMaxHeatingPower(Mission mission, BusHVACSystemConfiguration hvacConfiguration)
		{
			var busParams = mission.BusParameter;

			var length = DeclarationData.BusAuxiliaries.CalculateInternalLength(
				busParams.VehicleLength, busParams.VehicleCode,
				busParams.NumberPassengersLowerDeck);
			var height = DeclarationData.BusAuxiliaries.CalculateInternalHeight(busParams.VehicleCode, RegistrationClass.II, busParams.BodyHeight);
			var volume = length * height * busParams.VehicleWidth;

			var driver = DeclarationData.BusAuxiliaries.HVACMaxHeatingPower.DriverMaxHeatingPower(
				hvacConfiguration, mission.MissionType);
			var passenger = DeclarationData.BusAuxiliaries.HVACMaxHeatingPower.PassengerMaxHeatingPower(
				hvacConfiguration, mission.MissionType, volume);

			return Tuple.Create(driver, passenger);
		}

		#region Overrides of AuxiliaryDataAdapter

		public override AuxiliaryConfig CreateBusAuxiliariesData(Mission mission, IVehicleDeclarationInputData primaryVehicle, VectoRunData runData)
		{
			var actuations = DeclarationData.BusAuxiliaries.ActuationsMap.Lookup(runData.Mission.MissionType);

			var hvacParams = GetHVACParams(primaryVehicle.VehicleType, mission.BusParameter);
			
			var applicableHVACConfigCooling = DeclarationData.BusAuxiliaries.GetHVACConfig(hvacParams.HVACConfiguration,
				HeatPumpType.none, hvacParams.HeatPumpTypePassengerCompartmentCooling);
			var applicableHVACConfigHeating = DeclarationData.BusAuxiliaries.GetHVACConfig(hvacParams.HVACConfiguration,
				HeatPumpType.none, hvacParams.HeatPumpTypePassengerCompartmentHeating);

			var ssmCooling = CreateSSMModelParameters(primaryVehicle.Components.BusAuxiliaries, mission,
				runData.Loading, applicableHVACConfigCooling, HeatPumpType.none,
				hvacParams.HeatPumpTypePassengerCompartmentCooling, hvacParams.HVACAuxHeaterPower, FuelData.Diesel, true);
			var ssmHeating = CreateSSMModelParameters(primaryVehicle.Components.BusAuxiliaries, mission,
				runData.Loading, applicableHVACConfigHeating, HeatPumpType.none,
				hvacParams.HeatPumpTypePassengerCompartmentHeating, hvacParams.HVACAuxHeaterPower, FuelData.Diesel,
				false);
			ssmHeating.ElectricHeater = GetElectricHeater(mission, runData);
			ssmHeating.HeatingDistributions = DeclarationData.BusAuxiliaries.HeatingDistributionCases;

			var retVal = new AuxiliaryConfig
			{
				InputData = primaryVehicle.Components.BusAuxiliaries,
				ElectricalUserInputsConfig = GetElectricalUserConfig(mission, primaryVehicle, actuations, runData.VehicleData.VehicleClass),
				PneumaticUserInputsConfig = GetPneumaticUserConfig(primaryVehicle, mission),
				PneumaticAuxillariesConfig = CreatePneumaticAuxConfig(runData.Retarder.Type),
				Actuations = actuations,
				SSMInputsCooling = ssmCooling,
				SSMInputsHeating = ssmHeating,
				VehicleData = runData.VehicleData,
			};

			return retVal;
		}

		private HeaterType GetElectricHeater(Mission mission, VectoRunData runData)
		{
			HVACParameters hvacParams = null;
			switch (runData.JobType) {
				case VectoSimulationJobType.ConventionalVehicle:
					hvacParams = mission.BusParameter.HVACConventional;
					break;
				case VectoSimulationJobType.ParallelHybridVehicle:
				case VectoSimulationJobType.SerialHybridVehicle:
				case VectoSimulationJobType.IEPC_S:
				case VectoSimulationJobType.IHPC:
					hvacParams = mission.BusParameter.HVACHEV;
					break;
				case VectoSimulationJobType.BatteryElectricVehicle:
				case VectoSimulationJobType.IEPC_E:
					hvacParams = mission.BusParameter.HVACPEV;
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			return hvacParams.WaterElectricHeater ? HeaterType.WaterElectricHeater : HeaterType.None;
		}

		protected override IList<VectoRunData.AuxData> DoCreateAuxiliaryData(
			IAuxiliariesDeclarationInputData auxInputData, IBusAuxiliariesDeclarationData busAuxData,
			MissionType mission, VehicleClass hdvClass, Meter vehicleLength, int? numSteeredAxles,
			VectoSimulationJobType jobType)
		{
			if (auxInputData != null)
			{
				throw new VectoException("Only BusAuxiliaries can be provided as input!");
			}

			if (numSteeredAxles.HasValue && busAuxData.SteeringPumpTechnology.Count != numSteeredAxles.Value)
			{
				throw new VectoException($"Number of steering pump technologies does not match number of steered axles ({numSteeredAxles.Value}, {busAuxData.SteeringPumpTechnology.Count})");
			}
			var retVal = new List<VectoRunData.AuxData>();

			retVal.Add(
				new VectoRunData.AuxData()
				{
					DemandType = AuxiliaryDemandType.Constant,
					Technology = new List<string>() { busAuxData.FanTechnology },
					ID = Constants.Auxiliaries.IDs.Fan,
					PowerDemandMech = DeclarationData.Fan.LookupMechanicalPowerDemand(hdvClass, mission, busAuxData.FanTechnology)
				});
			retVal.Add(
				new VectoRunData.AuxData()
				{
					DemandType = AuxiliaryDemandType.Constant,
					Technology = busAuxData.SteeringPumpTechnology,
					ID = Constants.Auxiliaries.IDs.SteeringPump,
					PowerDemandMech = DeclarationData.SteeringPumpBus.LookupMechanicalPowerDemand(
						mission, busAuxData.SteeringPumpTechnology, vehicleLength)
				});
			return retVal;
		}

		#endregion
	}

	public class GenericCompletedBusAuxiliaryDataAdapter : PrimaryBusAuxiliaryDataAdapter, ICompletedBusAuxiliaryDataAdapter
	{
		#region Implementation of ICompletedBusAuxiliaryDataAdapter

		public IAuxiliaryConfig CreateBusAuxiliariesData(Mission mission, IVehicleDeclarationInputData primaryVehicle,
			IVehicleDeclarationInputData completedVehicle, VectoRunData runData)
		{
			if (completedVehicle != null) {
				throw new ArgumentException("Completed Vehicle must not be provided");
			}
			return CreateBusAuxiliariesData(mission, primaryVehicle, runData);
		}

		#endregion
	}

	public class SpecificCompletedBusAuxiliaryDataAdapter : PrimaryBusAuxiliaryDataAdapter, ICompletedBusAuxiliaryDataAdapter
	{
		private readonly PrimaryBusAuxiliaryDataAdapter _primaryBusDataAdapter;

		public SpecificCompletedBusAuxiliaryDataAdapter(IPrimaryBusAuxiliaryDataAdapter primaryBusDataAdapter)
		{
			_primaryBusDataAdapter = primaryBusDataAdapter as PrimaryBusAuxiliaryDataAdapter;
		}
		
		#region Avarage Current Demand Calculation

		protected override bool VehicleHasElectricalConsumer(string consumerName, IBusAuxiliariesDeclarationData busAux)
		{
			if (consumerName == "Day running lights LED bonus" && (bool)busAux.ElectricConsumers.DayrunninglightsLED)
				return true;
			if (consumerName == "Position lights LED bonus" && (bool)busAux.ElectricConsumers.PositionlightsLED)
				return true;
			if (consumerName == "Brake lights LED bonus" && (bool)busAux.ElectricConsumers.BrakelightsLED)
				return true;
			if (consumerName == "Interior lights LED bonus" && (bool)busAux.ElectricConsumers.InteriorLightsLED)
				return true;
			if (consumerName == "Headlights LED bonus" && (bool)busAux.ElectricConsumers.HeadlightsLED)
				return true;

			return false;
		}

		protected override double CalculateLengthDependentElectricalConsumers(Mission mission, IVehicleDeclarationInputData vehicleData)
		{
			var busParams = mission.BusParameter;
			return DeclarationData.BusAuxiliaries.CalculateLengthInteriorLights(
					vehicleData.Length, vehicleData.VehicleCode, busParams.NumberPassengersLowerDeck)
				.Value();
		}


		#endregion

		private Tuple<Watt, Watt> CalculateMaxCoolingPower(IVehicleDeclarationInputData completedVehicle,
			IVehicleDeclarationInputData primaryVehicle,
			Mission mission, BusHVACSystemConfiguration hvacConfiguration)
		{
			//var hvacConfiguration = completedVehicle.Components.BusAuxiliaries.HVACAux.SystemConfiguration;
			if (hvacConfiguration.IsOneOf(BusHVACSystemConfiguration.Configuration0, BusHVACSystemConfiguration.Unknown))
			{
				throw new VectoException(
					$"HVAC Configuration {hvacConfiguration.ToXmlFormat()} is invalid for final step");
			}

			var correctionLengthDrivetrainVolume = DeclarationData.BusAuxiliaries.CorrectionLengthDrivetrainVolume(
				completedVehicle.VehicleCode, completedVehicle.LowEntry, primaryVehicle.AxleConfiguration.NumAxles(),
				primaryVehicle.Articulated);

			var passengerCompartmentLength = DeclarationData.BusAuxiliaries.CalculateInternalLength(
				completedVehicle.Length, completedVehicle.VehicleCode,
				(int)completedVehicle.NumberPassengerSeatsLowerDeck) - Constants.BusParameters.DriverCompartmentLength - correctionLengthDrivetrainVolume;

			var internalHeight = DeclarationData.BusAuxiliaries.CalculateInternalHeight(completedVehicle.VehicleCode, completedVehicle.RegisteredClass, completedVehicle.Height);
			var volume = passengerCompartmentLength * internalHeight * completedVehicle.Width;

			var driver = DeclarationData.BusAuxiliaries.HVACMaxCoolingPower.DriverMaxCoolingPower(
				hvacConfiguration, mission.MissionType);
			var passenger = DeclarationData.BusAuxiliaries.HVACMaxCoolingPower.PassengerMaxCoolingPower(
				hvacConfiguration, mission.MissionType, volume);

			return Tuple.Create(driver, passenger);
		}

		private Tuple<Watt, Watt> CalculateMaxHeatingPower(IVehicleDeclarationInputData completedVehicle,
			IVehicleDeclarationInputData primaryVehicle,
			Mission mission, BusHVACSystemConfiguration hvacConfiguration)
		{
			//var hvacConfiguration = completedVehicle.Components.BusAuxiliaries.HVACAux.SystemConfiguration;
			if (hvacConfiguration.IsOneOf(BusHVACSystemConfiguration.Configuration0, BusHVACSystemConfiguration.Unknown)) {
				throw new VectoException(
					$"HVAC Configuration {hvacConfiguration.ToXmlFormat()} is invalid for final step");
			}

			var correctionLengthDrivetrainVolume = DeclarationData.BusAuxiliaries.CorrectionLengthDrivetrainVolume(
				completedVehicle.VehicleCode, completedVehicle.LowEntry, primaryVehicle.AxleConfiguration.NumAxles(),
				primaryVehicle.Articulated);

			var passengerCompartmentLength = DeclarationData.BusAuxiliaries.CalculateInternalLength(
				completedVehicle.Length, completedVehicle.VehicleCode,
				(int)completedVehicle.NumberPassengerSeatsLowerDeck) - Constants.BusParameters.DriverCompartmentLength - correctionLengthDrivetrainVolume;

			var internalHeight = DeclarationData.BusAuxiliaries.CalculateInternalHeight(completedVehicle.VehicleCode, completedVehicle.RegisteredClass, completedVehicle.Height);
			var volume = passengerCompartmentLength * internalHeight * completedVehicle.Width;

			var driver = DeclarationData.BusAuxiliaries.HVACMaxHeatingPower.DriverMaxHeatingPower(
				hvacConfiguration, mission.MissionType);
			var passenger = DeclarationData.BusAuxiliaries.HVACMaxHeatingPower.PassengerMaxHeatingPower(
				hvacConfiguration, mission.MissionType, volume);

			return Tuple.Create(driver, passenger);
		}



		private PneumaticUserInputsConfig CreatePneumaticUserInputsConfig(IBusAuxiliariesDeclarationData primaryBusAuxiliaries,
			IVehicleDeclarationInputData completedVehicle)
		{
			return new PneumaticUserInputsConfig
			{
				CompressorMap = completedVehicle.VehicleType == VectoSimulationJobType.BatteryElectricVehicle
					? null
					: DeclarationData.BusAuxiliaries.GetCompressorMap(
						primaryBusAuxiliaries.PneumaticSupply.CompressorSize,
						primaryBusAuxiliaries.PneumaticSupply.Clutch),
				CompressorGearEfficiency = Constants.BusAuxiliaries.PneumaticUserConfig.CompressorGearEfficiency,
				CompressorGearRatio = completedVehicle.VehicleType == VectoSimulationJobType.BatteryElectricVehicle
					? 0 : primaryBusAuxiliaries.PneumaticSupply.Ratio,
				SmartAirCompression = primaryBusAuxiliaries.PneumaticSupply.SmartAirCompression,
				SmartRegeneration = primaryBusAuxiliaries.PneumaticSupply.SmartRegeneration,
				KneelingHeight = VectoMath.Max(0.SI<Meter>(),
					completedVehicle.EntranceHeight - Constants.BusParameters.EntranceHeight),
				AirSuspensionControl = primaryBusAuxiliaries.PneumaticConsumers.AirsuspensionControl,
				AdBlueDosing = primaryBusAuxiliaries.PneumaticConsumers.AdBlueDosing,
				Doors = completedVehicle.DoorDriveTechnology
			};
		}

		private (SSMInputs, SSMInputs) GetCompletedSSMInput(Mission mission,
			IVehicleDeclarationInputData completedVehicle,
			IVehicleDeclarationInputData primaryVehicle, LoadingType loadingType)
		{
			
			var hvacConfiguration = completedVehicle.Components.BusAuxiliaries.HVACAux.SystemConfiguration;
			var busAux = completedVehicle.Components.BusAuxiliaries.HVACAux;

			if (hvacConfiguration == null || hvacConfiguration.Value == BusHVACSystemConfiguration.Configuration0) {
				throw new VectoException("HVAC Configuration has to be set for completed stage!");
			}

			if (mission.BusParameter.SeparateAirDistributionDuctsHVACCfg.Contains(hvacConfiguration.Value) &&
				(busAux.SeparateAirDistributionDucts == null || !busAux.SeparateAirDistributionDucts.Value)) {
				throw new VectoException(
					"Input parameter 'separate air distribution ducts' has to be set to 'true' for vehicle group '{0}' and HVAC configuration '{1}'",
					mission.BusParameter.BusGroup.GetClassNumber(), hvacConfiguration.GetName());
			}

			if (completedVehicle.NumberPassengerSeatsLowerDeck == null) {
				throw new VectoException("NumberOfPassengerSeatsLowerDeck input parameter is required");
			}

			if (completedVehicle.NumberPassengerSeatsUpperDeck == null) {
				throw new VectoException("NumberOfPassengerSeatsUpperDeck input parameter is required");
			}

			if (completedVehicle.NumberPassengersStandingLowerDeck == null) {
				throw new VectoException("NumberOfPassengersStandingLowerDeck input parameter is required");
			}

			if (completedVehicle.NumberPassengersStandingUpperDeck == null) {
				throw new VectoException("NumberOfPassengersStandingUpperDeck input parameter is required");
			}

			if (busAux.HeatPumpTypeCoolingDriverCompartment == null) {
				throw new VectoException("HeatPumpTypeDriverCompartment Cooling input parameter is required");
			}

			if (busAux.HeatPumpTypeHeatingDriverCompartment == null) {
				throw new VectoException("HeatPumpTypeDriverCompartment Heating input parameter is required");
			}

			if (busAux.HeatPumpTypeCoolingPassengerCompartment == null) {
				throw new VectoException("HeatPumpTypePassengerCompartment Cooling input parameter is required");
			}

			if (busAux.HeatPumpTypeHeatingPassengerCompartment == null) {
				throw new VectoException("HeatPumpTypePassengerCompartment Heating input parameter is required");
			}

			var hvacConfigCooling = DeclarationData.BusAuxiliaries.GetHVACConfig(hvacConfiguration.Value,
				busAux.HeatPumpTypeCoolingDriverCompartment.Value, busAux.HeatPumpTypeCoolingPassengerCompartment.Value);
			var hvacConfigHeating = DeclarationData.BusAuxiliaries.GetHVACConfig(hvacConfiguration.Value,
				busAux.HeatPumpTypeHeatingDriverCompartment.Value, busAux.HeatPumpTypeHeatingPassengerCompartment.Value);

			if (hvacConfigHeating != hvacConfiguration && hvacConfigCooling != hvacConfiguration) {
				throw new VectoException(
					$"The HVAC System Configuration must be either matched for the case heating or cooling input: {hvacConfiguration.GetName()},  h:{hvacConfigHeating.GetName()}/c:{hvacConfigCooling.GetName()}");
			}

			var xEVBus = !primaryVehicle.VehicleType.IsOneOf(VectoSimulationJobType.ConventionalVehicle,
				VectoSimulationJobType.EngineOnlySimulation);
			if (xEVBus && busAux.AirElectricHeater == null) {
				throw new VectoException("AirElectricHeater input parameter is required for xEV vehicles");
			}
			if (xEVBus && busAux.WaterElectricHeater == null) {
				throw new VectoException("AirElectricHeater input parameter is required for xEV vehicles");
			}
			if (xEVBus && busAux.OtherHeatingTechnology == null) {
				throw new VectoException("AirElectricHeater input parameter is required for xEV vehicles");
			}

			//if (hvacConfiguration.RequiresDriverAC() &&
			//	(busAux.HeatPumpTypeCoolingDriverCompartment == HeatPumpType.none ||
			//	busAux.HeatPumpTypeHeatingDriverCompartment == HeatPumpType.none)) {
			//	throw new VectoException("HVAC System Configuration {0} requires a Driver Heatpump Technology",
			//		hvacConfiguration);
			//}
			//if (hvacConfiguration.RequiresPassengerAC() &&
			//	(busAux.HeatPumpTypeCoolingPassengerCompartment == HeatPumpType.none ||
			//	busAux.HeatPumpTypeHeatingPassengerCompartment == HeatPumpType.none)) {
			//	throw new VectoException("HVAC System Configuration {0} requires a Passenger Heatpump Technology",
			//		hvacConfiguration);
			//}

			var applicableSystemConfigCooling = DeclarationData.BusAuxiliaries.GetHVACConfig(hvacConfiguration.Value,
				busAux.HeatPumpTypeCoolingDriverCompartment.Value, busAux.HeatPumpTypeCoolingPassengerCompartment.Value);
			var applicableSystemConfigHeating = DeclarationData.BusAuxiliaries.GetHVACConfig(hvacConfiguration.Value,
				busAux.HeatPumpTypeHeatingDriverCompartment.Value, busAux.HeatPumpTypeHeatingPassengerCompartment.Value);

			var ssmCooling = DoGetSsmInputs(mission, completedVehicle, primaryVehicle, loadingType,
				applicableSystemConfigCooling, busAux.HeatPumpTypeCoolingDriverCompartment.Value,
				busAux.HeatPumpTypeCoolingPassengerCompartment.Value, true);
			ssmCooling.ElectricHeater = HeaterType.None;
			var ssmHeating = DoGetSsmInputs(mission, completedVehicle, primaryVehicle, loadingType,
				applicableSystemConfigHeating, busAux.HeatPumpTypeHeatingDriverCompartment.Value,
				busAux.HeatPumpTypeHeatingPassengerCompartment.Value, false);
			ssmHeating.ElectricHeater = GetElectricHeater(busAux);
			ssmHeating.HeatingDistributions = DeclarationData.BusAuxiliaries.HeatingDistributionCases;
			return (ssmCooling, ssmHeating);
		}

		private HeaterType GetElectricHeater(IHVACBusAuxiliariesDeclarationData busAux)
		{
			var retVal = HeaterType.None;
			if (busAux.AirElectricHeater.HasValue && busAux.AirElectricHeater.Value) {
				retVal |= HeaterType.AirElectricHeater;
			}
			if (busAux.WaterElectricHeater.HasValue && busAux.WaterElectricHeater.Value) {
				retVal |= HeaterType.WaterElectricHeater;
			}
			if (busAux.OtherHeatingTechnology.HasValue && busAux.OtherHeatingTechnology.Value) {
				retVal |= HeaterType.OtherElectricHeating;
			}

			return retVal;
		}

		private SSMInputs DoGetSsmInputs(Mission mission, IVehicleDeclarationInputData completedVehicle,
			IVehicleDeclarationInputData primaryVehicle, LoadingType loadingType,
			BusHVACSystemConfiguration hvacConfiguration, HeatPumpType heatPumpTypeDriverCompartment,
			HeatPumpType heatPumpTypePassengerCompartment, bool cooling)
		{
			var isDoubleDecker = completedVehicle.VehicleCode.IsDoubleDeckerBus();

			var driverAcOnly = hvacConfiguration.IsOneOf(BusHVACSystemConfiguration.Configuration2,
				BusHVACSystemConfiguration.Configuration4);
			var internalLength = cooling && driverAcOnly
				? 2 * Constants.BusParameters.DriverCompartmentLength // OK
				: DeclarationData.BusAuxiliaries.CalculateInternalLength(
					completedVehicle.Length, completedVehicle.VehicleCode,
					completedVehicle.NumberPassengerSeatsLowerDeck.Value);
			var ventilationLength = DeclarationData.BusAuxiliaries.CalculateInternalLength(completedVehicle.Length,
				completedVehicle.VehicleCode,
				completedVehicle.NumberPassengerSeatsLowerDeck.Value);
			var correctionLengthDrivetrainVolume = DeclarationData.BusAuxiliaries.CorrectionLengthDrivetrainVolume(
				completedVehicle.VehicleCode, completedVehicle.LowEntry, primaryVehicle.AxleConfiguration.NumAxles(),
				primaryVehicle.Articulated);

			var internalHeight = DeclarationData.BusAuxiliaries.CalculateInternalHeight(completedVehicle.VehicleCode, completedVehicle.RegisteredClass, completedVehicle.Height);
			var correctedBusWidth = DeclarationData.BusAuxiliaries.CorrectedBusWidth(completedVehicle.Width);

			var coolingPower = CalculateMaxCoolingPower(completedVehicle, primaryVehicle, mission, hvacConfiguration);
			var heatingPower = CalculateMaxHeatingPower(completedVehicle, primaryVehicle, mission, hvacConfiguration);
			var ssmInputs = _primaryBusDataAdapter.GetDefaulSSMInputs(FuelData.Diesel);

			ssmInputs.BusFloorType = completedVehicle.VehicleCode.GetFloorType();
			ssmInputs.Technologies = CreateTechnologyBenefits(completedVehicle, primaryVehicle.Components.BusAuxiliaries);
			ssmInputs.FuelFiredHeaterPower = completedVehicle.Components.BusAuxiliaries.HVACAux.AuxHeaterPower;
			ssmInputs.BusWindowSurface = DeclarationData.BusAuxiliaries.WindowHeight(isDoubleDecker) * internalLength +
										DeclarationData.BusAuxiliaries.FrontAndRearWindowArea(isDoubleDecker);
			ssmInputs.BusSurfaceArea = 2 * (completedVehicle.Length * correctedBusWidth + internalLength *
											internalHeight + (isDoubleDecker ? 2.0 : 1.0) * correctedBusWidth * completedVehicle.Height); // use equations sent by Tobias
			ssmInputs.BusVolumeVentilation = (ventilationLength - correctionLengthDrivetrainVolume) * correctedBusWidth * internalHeight;

			ssmInputs.UValue = DeclarationData.BusAuxiliaries.UValue(completedVehicle.VehicleCode.GetFloorType());
			ssmInputs.NumberOfPassengers = DeclarationData.GetNumberOfPassengers(
				mission, internalLength, correctedBusWidth,
				(completedVehicle.NumberPassengerSeatsLowerDeck ?? 0) + (completedVehicle.NumberPassengerSeatsUpperDeck ?? 0),
				(completedVehicle.NumberPassengersStandingLowerDeck ?? 0) + (completedVehicle.NumberPassengersStandingUpperDeck ?? 0),
				loadingType) + 1; // add driver for 'heat input' // passenger count can't be null as this is checked in the calling method already. use ?? to avoid compiler warning
			ssmInputs.VentilationRate = DeclarationData.BusAuxiliaries.VentilationRate(hvacConfiguration, false);
			ssmInputs.VentilationRateHeating = DeclarationData.BusAuxiliaries.VentilationRate(hvacConfiguration, true);

			//ssmInputs.HVACMaxCoolingPower = coolingPower.Item1 + coolingPower.Item2;
			ssmInputs.HVACMaxCoolingPowerDriver = coolingPower.Item1;
			ssmInputs.HVACMaxCoolingPowerPassenger = coolingPower.Item2;
			ssmInputs.MaxHeatingPowerDriver = heatingPower.Item1;
			ssmInputs.MaxHeatingPowerPassenger = heatingPower.Item2;

			//ssmInputs.HeatPumpTypeHeatingDriverCompartment = busAux.HeatPumpTypeHeatingDriverCompartment.Value;
			ssmInputs.HeatPumpTypeDriverCompartment = heatPumpTypeDriverCompartment;
			//ssmInputs.HeatPumpTypeHeatingPassengerCompartment = busAux.HeatPumpTypeHeatingPassengerCompartment.Value;
			ssmInputs.HeatPumpTypePassengerCompartment = heatPumpTypePassengerCompartment;

			ssmInputs.HVACSystemConfiguration = hvacConfiguration;

			if (cooling) {
				ssmInputs.DriverCompartmentLength = hvacConfiguration.RequiresDriverAC()
					? driverAcOnly
						? 2 * Constants.BusParameters.DriverCompartmentLength
						: Constants.BusParameters.DriverCompartmentLength
					: 0.SI<Meter>();
				ssmInputs.PassengerCompartmentLength = hvacConfiguration.RequiresPassengerAC()
					? driverAcOnly
						? 0.SI<Meter>()
						: internalLength - Constants.BusParameters.DriverCompartmentLength
					: 0.SI<Meter>();
			} else {
				ssmInputs.DriverCompartmentLength = hvacConfiguration.RequiresDriverAC()
					? Constants.BusParameters.DriverCompartmentLength 
					: 0.SI<Meter>();
				ssmInputs.PassengerCompartmentLength = hvacConfiguration.RequiresDriverAC()
					? internalLength - Constants.BusParameters.DriverCompartmentLength
					: internalLength;
			}

			//ssmInputs.HVACCompressorType = heatPumpTypePassengerCompartment; // use passenger compartment

			return ssmInputs;
		}


		private TechnologyBenefits CreateTechnologyBenefits(IVehicleDeclarationInputData completedVehicle,
			IBusAuxiliariesDeclarationData primaryBusAux)
		{
			var onVehicle = new List<SSMTechnology>();
			var completedBuxAux = completedVehicle.Components.BusAuxiliaries;

			foreach (var item in DeclarationData.BusAuxiliaries.SSMTechnologyList)
			{
				if ("Double-glazing".Equals(item.BenefitName, StringComparison.InvariantCultureIgnoreCase) &&
					(completedBuxAux?.HVACAux.DoubleGlazing ?? false))
				{
					onVehicle.Add(item);
				}
				if ("Adjustable auxiliary heater".Equals(item.BenefitName, StringComparison.InvariantCultureIgnoreCase) &&
					(completedBuxAux?.HVACAux.AdjustableAuxiliaryHeater ?? false))
				{
					onVehicle.Add(item);
				}
				if ("Separate air distribution ducts".Equals(item.BenefitName, StringComparison.InvariantCultureIgnoreCase) &&
					(completedBuxAux?.HVACAux.SeparateAirDistributionDucts ?? false))
				{
					onVehicle.Add(item);
				}
				if ("Adjustable coolant thermostat".Equals(item.BenefitName, StringComparison.InvariantCultureIgnoreCase) &&
					(primaryBusAux?.HVACAux.AdjustableCoolantThermostat ?? false))
				{
					onVehicle.Add(item);
				}
				if ("Engine waste gas heat exchanger".Equals(item.BenefitName, StringComparison.InvariantCultureIgnoreCase) &&
					(primaryBusAux?.HVACAux.EngineWasteGasHeatExchanger ?? false))
				{
					onVehicle.Add(item);
				}
			}

			return _primaryBusDataAdapter.SelectBenefitForFloorType(completedVehicle.VehicleCode.GetFloorType(), onVehicle);
		}

		protected override Dictionary<string, AuxiliaryDataAdapter.ElectricConsumerEntry> GetElectricAuxConsumers(Mission mission, IVehicleDeclarationInputData vehicleData, VehicleClass vehicleClass, IBusAuxiliariesDeclarationData busAux)
		{
			return new Dictionary<string, AuxiliaryDataAdapter.ElectricConsumerEntry>();
		}

		protected virtual Dictionary<string, AuxiliaryDataAdapter.ElectricConsumerEntry> GetElectricAuxConsumersPrimary(Mission mission, IVehicleDeclarationInputData vehicleData, VehicleClass vehicleClass, IBusAuxiliariesDeclarationData busAuxPrimary)
		{
			var retVal = new Dictionary<string, AuxiliaryDataAdapter.ElectricConsumerEntry>();
			var spPower = DeclarationData.SteeringPumpBus.LookupElectricalPowerDemand(
				mission.MissionType, busAuxPrimary.SteeringPumpTechnology,
				vehicleData.Length ?? mission.BusParameter.VehicleLength);
			retVal[Constants.Auxiliaries.IDs.SteeringPump] = new AuxiliaryDataAdapter.ElectricConsumerEntry
			{
				ActiveDuringEngineStopStandstill = false,
				BaseVehicle = false,
				Current = spPower / Constants.BusAuxiliaries.ElectricSystem.PowernetVoltage
			};

			var fanPower = vehicleData.VehicleType == VectoSimulationJobType.BatteryElectricVehicle
				? 0.SI<Watt>()
				: DeclarationData.Fan.LookupElectricalPowerDemand(
					vehicleClass, mission.MissionType, busAuxPrimary.FanTechnology);
			retVal[Constants.Auxiliaries.IDs.Fan] = new AuxiliaryDataAdapter.ElectricConsumerEntry
			{
				ActiveDuringEngineStopStandstill = false,
				ActiveDuringEngineStopDriving = false,
				BaseVehicle = false,
				Current = fanPower / Constants.BusAuxiliaries.ElectricSystem.PowernetVoltage
			};
			return retVal;
		}

		protected virtual ElectricsUserInputsConfig CreateElectricsUserInputsConfig(IVehicleDeclarationInputData primaryVehicle,
			IVehicleDeclarationInputData completedVehicle, Mission mission, IActuations actuations, VehicleClass vehicleClass)
		{
			var currentDemand = GetElectricConsumers(mission, completedVehicle, actuations, vehicleClass);

			// add electrical steering pump or electric fan defined in primary vehicle
			foreach (var entry in GetElectricAuxConsumersPrimary(mission, completedVehicle, vehicleClass, primaryVehicle.Components.BusAuxiliaries))
			{
				currentDemand[entry.Key] = entry.Value;
			}

			var retVal = GetDefaultElectricalUserConfig();

			var primaryBusAuxiliaries = primaryVehicle.Components.BusAuxiliaries;
			retVal.AlternatorType = primaryBusAuxiliaries.ElectricSupply.AlternatorTechnology;
			retVal.ElectricalConsumers = (Dictionary<string, AuxiliaryDataAdapter.ElectricConsumerEntry>)currentDemand;
			retVal.AlternatorMap = new SimpleAlternator(
				_primaryBusDataAdapter.CalculateAlternatorEfficiency(
					primaryBusAuxiliaries.ElectricSupply.Alternators
						.Concat(completedVehicle.Components.BusAuxiliaries.ElectricSupply?.Alternators ??
								new List<IAlternatorDeclarationInputData>()).ToList()));
			switch (retVal.AlternatorType)
			{
				case AlternatorType.Smart when primaryBusAuxiliaries.ElectricSupply.Alternators.Count == 0:
					throw new VectoException("at least one alternator is required when specifying smart electrics!");
				case AlternatorType.Smart when primaryBusAuxiliaries.ElectricSupply.ElectricStorage.Count == 0:
					throw new VectoException("at least one electric storage (battery or capacitor) is required when specifying smart electrics!");
			}

			retVal.MaxAlternatorPower = primaryBusAuxiliaries.ElectricSupply.Alternators.Sum(x => x.RatedVoltage * x.RatedCurrent);
			retVal.ElectricStorageCapacity = primaryBusAuxiliaries.ElectricSupply.ElectricStorage.Sum(x => x.ElectricStorageCapacity) ?? 0.SI<WattSecond>();

			return retVal;
		}
		#region Implementation of ICompletedBusAuxiliaryDataAdapter

		public IAuxiliaryConfig CreateBusAuxiliariesData(Mission mission, IVehicleDeclarationInputData primaryVehicle,
			IVehicleDeclarationInputData completedVehicle, VectoRunData runData)
		{
			var actuations = DeclarationData.BusAuxiliaries.ActuationsMap.Lookup(runData.Mission.MissionType);
			var primaryBusAuxiliaries = primaryVehicle.Components.BusAuxiliaries;

			var (ssmCooling, ssmHeating) =
				GetCompletedSSMInput(mission, completedVehicle, primaryVehicle, runData.Loading);

			return new AuxiliaryConfig
			{
				InputData = completedVehicle.Components.BusAuxiliaries,
				ElectricalUserInputsConfig = CreateElectricsUserInputsConfig(
					primaryVehicle, completedVehicle, mission, actuations, runData.VehicleData.VehicleClass),
				PneumaticUserInputsConfig = CreatePneumaticUserInputsConfig(
					primaryBusAuxiliaries, completedVehicle),
				PneumaticAuxillariesConfig = _primaryBusDataAdapter.CreatePneumaticAuxConfig(runData.Retarder.Type),
				Actuations = actuations,
				SSMInputsCooling = ssmCooling,
				SSMInputsHeating = ssmHeating,
				VehicleData = runData.VehicleData
			};
		}

		#endregion
	}

}