using System;
using System.Collections.Generic;
using System.Linq;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter;
using TUGraz.VectoCore.Models.BusAuxiliaries;
using TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.Electrics;
using TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.HVAC;
using TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.Pneumatics;
using TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules.Electrics;

namespace TUGraz.VectoCore.InputData.Reader.Impl
{
	public class DeclarationDataAdapterCompletedBus
	{
		//Specific

		public DriverData CreateDriverData()
		{
			throw new System.NotImplementedException();
		}

		public AirdragData CreateAirdragData(IVehicleDeclarationInputData completedVehicle, Mission mission)
		{
			var airdragData = new AirdragData
			{
				CrossWindCorrectionMode = CrossWindCorrectionMode.DeclarationModeCorrection
			};

			SquareMeter aerodynamicDragArea;
			if (!mission.BusParameter.AirDragMeasurementAllowed || completedVehicle.Components.AirdragInputData.AirDragArea == null)
				aerodynamicDragArea = mission.DefaultCDxA;
			else
				aerodynamicDragArea = completedVehicle.Components.AirdragInputData.AirDragArea;

			var vehicleHeight = completedVehicle.Height + mission.BusParameter.DeltaHeight;

			airdragData.CrossWindCorrectionCurve = new CrosswindCorrectionCdxALookup(
				aerodynamicDragArea,
					DeclarationDataAdapterHeavyLorry.GetDeclarationAirResistanceCurve(
						mission.CrossWindCorrectionParameters,
						aerodynamicDragArea,
						vehicleHeight),
						CrossWindCorrectionMode.DeclarationModeCorrection);

			airdragData.DeclaredAirdragArea = aerodynamicDragArea;

			return airdragData;
		}
		
		public CombustionEngineData CreateEngineData()
		{
			throw new System.NotImplementedException();
		}


		public RetarderData CreateRetarderData(IRetarderInputData retarderInputData)
		{
			throw new System.NotImplementedException();
		}

		public ShiftStrategyParameters CreateGearshiftData(GearboxData gearboxData, double axleRatio, PerSecond idleSpeed)
		{
			throw new System.NotImplementedException();
		}

		public VehicleData CreateVehicleData(IVehicleDeclarationInputData pifVehicle,
			IVehicleDeclarationInputData completedVehicle, Mission mission, KeyValuePair<LoadingType, Kilogram> loading)
		{
			var vehicleData = new VehicleData
			{
				AxleConfiguration = pifVehicle.AxleConfiguration,
				CurbMass = completedVehicle.CurbMassChassis,
				BodyAndTrailerMass = 0.SI<Kilogram>(),
				Loading = GetLoading(completedVehicle, mission, loading),
				GrossVehicleMass = completedVehicle.GrossVehicleMassRating,
				DynamicTyreRadius = GetDynamicTyreRadius(pifVehicle.Components.AxleWheels.AxlesDeclaration),
				AxleData = GetAxles(pifVehicle.Components.AxleWheels.AxlesDeclaration, mission.AxleWeightDistribution)
			};

			var adas = new VehicleData.ADASData
			{
				EngineStopStart = pifVehicle.ADAS.EngineStopStart,
				EcoRoll = pifVehicle.ADAS.EcoRoll,
				PredictiveCruiseControl = pifVehicle.ADAS.PredictiveCruiseControl
			};

			vehicleData.ADAS = adas;

			return vehicleData;
		}

		public ElectricsUserInputsConfig CreateElectricsUserInputsConfig(IBusAuxiliariesDeclarationData primaryBusAuxiliaries,
			IVehicleDeclarationInputData completedVehicle, Mission mission, IAlternatorMap alternatorMap)
		{
			var actions = DeclarationData.BusAuxiliaries.ActuationsMap.Lookup(mission.MissionType);
			var currentDemand = CalculateAverageCurrent(mission, completedVehicle, actions);

			var electricUI = new ElectricsUserInputsConfig {
				SmartElectrical = primaryBusAuxiliaries.ElectricSupply.SmartElectrics,
				MaxAlternatorPower = primaryBusAuxiliaries.ElectricSupply.MaxAlternatorPower,
				ElectricStorageCapacity = primaryBusAuxiliaries.ElectricSupply.ElectricStorageCapacity,
				AlternatorMap = alternatorMap,
				AlternatorGearEfficiency = Constants.BusAuxiliaries.ElectricSystem.AlternatorGearEfficiency,
				AverageCurrentDemandInclBaseLoad = currentDemand.Item1,
				AverageCurrentDemandWithoutBaseLoad = currentDemand.Item2,
				DoorActuationTimeSecond = Constants.BusAuxiliaries.ElectricalConsumers.DoorActuationTimeSecond,
			};
			
			return electricUI;
		}

		public PneumaticUserInputsConfig CreatePneumaticUserInputsConfig(IBusAuxiliariesDeclarationData primaryBusAuxiliaries,
			IVehicleDeclarationInputData completedVehicle, ICompressorMap compressorMap)
		{
			var pneumaticUI = new PneumaticUserInputsConfig {
				CompressorMap = compressorMap,
				CompressorGearEfficiency = Constants.BusAuxiliaries.PneumaticUserConfig.CompressorGearEfficiency,
				CompressorGearRatio = primaryBusAuxiliaries.PneumaticSupply.Ratio,
				SmartAirCompression = primaryBusAuxiliaries.PneumaticSupply.SmartAirCompression,
				SmartRegeneration = primaryBusAuxiliaries.PneumaticSupply.SmartRegeneration,
				KneelingHeight = VectoMath.Max(0.SI<Meter>(),
					completedVehicle.EntranceHeight - Constants.BusParameters.EntranceHeight),
				AirSuspensionControl = primaryBusAuxiliaries.PneumaticConsumers.AirsuspensionControl,
				AdBlueDosing = primaryBusAuxiliaries.PneumaticConsumers.AdBlueDosing,
				Doors = completedVehicle.Components.BusAuxiliaries.PneumaticConsumers.DoorDriveTechnology
			};

			return pneumaticUI;

		}

		public void SetSSMBusParameters(SSMInputs ssmInputs, IVehicleDeclarationInputData completedVehicle, Mission mission,
			KeyValuePair<LoadingType, Kilogram> loading)
		{
			var busAuxiliaries = completedVehicle.Components.BusAuxiliaries;
			var isDoubleDecker = completedVehicle.VehicleCode.IsDoubleDeckBus();
			var floorType = GetFloorType(completedVehicle.VehicleCode);

			var hvacBusLength = busAuxiliaries.HVACAux.SystemConfiguration == BusHVACSystemConfiguration.Configuration2
				? 2 * Constants.BusParameters.DriverCompartmentLength
				: completedVehicle.Length;

			var hvacBusHeight = DeclarationData.BusAuxiliaries.CalculateInternalHeight(GetFloorType(completedVehicle.VehicleCode),
				isDoubleDecker, completedVehicle.Height);

			ssmInputs.NumberOfPassengers = GetLoading(completedVehicle, mission, loading).Value();
			ssmInputs.BusFloorType = floorType;
			
			ssmInputs.BusWindowSurface = DeclarationData.BusAuxiliaries.WindowHeight(isDoubleDecker) * hvacBusLength +
										DeclarationData.BusAuxiliaries.FrontAndRearWindowArea(isDoubleDecker);
			ssmInputs.BusSurfaceArea = 2 * (hvacBusLength * completedVehicle.Width + hvacBusLength *
											completedVehicle.Height + completedVehicle.Width * completedVehicle.Height);
			ssmInputs.BusVolume = hvacBusLength * completedVehicle.Width * hvacBusHeight;
		}

		public IEnumerable<VectoRunData.AuxData> CreateAuxiliaryData(
			IAuxiliariesDeclarationInputData auxiliaryInputData, IBusAuxiliariesDeclarationData mergedBusAux,
			MissionType mission, VehicleClass vehicleClass, Meter vehicleLength)
		{
			throw new System.NotImplementedException();
		}


		public TechnologyBenefits CreateTechnologyBenefits(IVehicleDeclarationInputData completedVehicle,
			IBusAuxiliariesDeclarationData primaryBusAux, DeclarationDataAdapterPrimaryBus dataAdapterPrimary)
		{
			var onVehicle = new List<SSMTechnology>();
			var completedBuxAux = completedVehicle.Components.BusAuxiliaries;
			var floortype = GetFloorType(completedVehicle.VehicleCode);

			foreach (var item in DeclarationData.BusAuxiliaries.SSMTechnologyList)
			{
				if ("Double-glazing".Equals(item.BenefitName, StringComparison.InvariantCultureIgnoreCase) &&
					(completedBuxAux?.HVACAux.DoubleGlasing ?? false))
				{
					onVehicle.Add(item);
				}
				if ("Heat pump systems".Equals(item.BenefitName, StringComparison.InvariantCultureIgnoreCase) &&
					(completedBuxAux?.HVACAux.HeatPump ?? false))
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

			return dataAdapterPrimary.SelectBenefitForFloorType(floortype, onVehicle);
		}

		public void SetSSMInputs(SSMInputs ssmInputs, Mission mission, KeyValuePair<LoadingType, Kilogram> loading,
			IVehicleDeclarationInputData completedVehicle)
		{
			var coolingPower = CalculateMaxCoolingPower(completedVehicle, mission.MissionType);

			var busAux = completedVehicle.Components.BusAuxiliaries.HVACAux;
			var floorType = GetFloorType(completedVehicle.VehicleCode);

			ssmInputs.NumberOfPassengers = GetLoading(completedVehicle, mission, loading).Value();
			ssmInputs.HVACMaxCoolingPower = coolingPower.Item1 + coolingPower.Item2;
			ssmInputs.COP = DeclarationData.BusAuxiliaries.CalculateCOP(
				coolingPower.Item1, busAux.CompressorTypeDriver, coolingPower.Item2, busAux.CompressorTypePassenger,
				floorType);

			ssmInputs.VentilationOnDuringHeating = true;
			ssmInputs.VentilationWhenBothHeatingAndACInactive = true;
			ssmInputs.VentilationDuringAC = true;

			ssmInputs.FuelFiredHeaterPower =  busAux.AuxHeaterPower;
			ssmInputs.FuelEnergyToHeatToCoolant = Constants.BusAuxiliaries.Heater.FuelEnergyToHeatToCoolant;
			ssmInputs.CoolantHeatTransferredToAirCabinHeater =
				Constants.BusAuxiliaries.Heater.CoolantHeatTransferredToAirCabinHeater;
		}

		private Tuple<Watt, Watt> CalculateMaxCoolingPower(IVehicleDeclarationInputData completedVehicle,
			MissionType missionType)
		{
			var isDoubleDecker = VehicleCodeHelper.IsDoubleDeckBus(completedVehicle.VehicleCode);
			var floorType = GetFloorType(completedVehicle.VehicleCode);
			var hvacConfiguration = completedVehicle.Components.BusAuxiliaries.HVACAux.SystemConfiguration;
			
			var length = DeclarationData.BusAuxiliaries.CalculateInternalLength(
			 	completedVehicle.Length ,isDoubleDecker, floorType, 
				completedVehicle.NumberOfPassengersLowerDeck);
			var height = DeclarationData.BusAuxiliaries.CalculateInternalHeight(floorType, isDoubleDecker, completedVehicle.Height);
			var volume = length * height * completedVehicle.Width;

			var driver = DeclarationData.BusAuxiliaries.HVACMaxCoolingPower.DriverMaxCoolingPower(
			 	hvacConfiguration, missionType);
			var passenger = DeclarationData.BusAuxiliaries.HVACMaxCoolingPower.PassengerMaxCoolingPower(
				hvacConfiguration, missionType, volume);

			return Tuple.Create(driver, passenger);
		}




		#region Avarage Current Demand Calculation


		private Tuple<Ampere, Ampere> CalculateAverageCurrent( Mission mission, IVehicleDeclarationInputData vehicleData,
			IActuations actuations)
		{
			var avgInclBase = 0.SI<Ampere>();
			var avgWithoutBase = 0.SI<Ampere>();
			var doorDutyCycleFraction =
				(actuations.ParkBrakeAndDoors * Constants.BusAuxiliaries.ElectricalConsumers.DoorActuationTimeSecond) /
				actuations.CycleTime;
			var busAux = vehicleData.Components.BusAuxiliaries;
			var electricDoors = false;
			var floorType = GetFloorType(vehicleData.VehicleCode);

			foreach (var consumer in DeclarationData.BusAuxiliaries.DefaultElectricConsumerList.Items)
			{
				var nbr = GetNumberOfElectricalConsumersInVehicle(consumer.NumberInActualVehicle, mission,
					vehicleData.Length, floorType);

				var dutyCycle = electricDoors && consumer.ConsumerName.Equals(
									Constants.BusAuxiliaries.ElectricalConsumers.DoorsPerVehicleConsumer,
									StringComparison.CurrentCultureIgnoreCase)
					? doorDutyCycleFraction
					: consumer.PhaseIdleTractionOn;

				var current = consumer.NominalCurrent(mission.MissionType) * dutyCycle * nbr;
				if (consumer.Bonus && !VehicleHasElectricalConsumer(consumer.ConsumerName, busAux))
				{
					current = 0.SI<Ampere>();
				}

				avgInclBase += current;
				if (!consumer.BaseVehicle)
				{
					avgWithoutBase += current;
				}
			}

			return Tuple.Create(avgInclBase, avgWithoutBase);
		}
		
		private bool VehicleHasElectricalConsumer(string consumerName, IBusAuxiliariesDeclarationData busAux)
		{
			if (consumerName == "Day running lights LED bonus" && busAux.ElectricConsumers.DayrunninglightsLED)
				return true;
			if (consumerName == "Position lights LED bonus" && busAux.ElectricConsumers.PositionlightsLED)
				return true;
			if (consumerName == "Brake lights LED bonus" && busAux.ElectricConsumers.BrakelightsLED)
				return true;
			if (consumerName == "Interior lights LED bonus" && busAux.ElectricConsumers.InteriorLightsLED)
				return true;
			if (consumerName == "Headlights LED bonus" && busAux.ElectricConsumers.HeadlightsLED)
				return true;

			return false;
		}
		
		private double GetNumberOfElectricalConsumersInVehicle(string nbr, Mission mission, Meter vehicleLength, FloorType floorType)
		{
			if ("f_IntLight(L_CoC)".Equals(nbr, StringComparison.InvariantCultureIgnoreCase))
			{
				var busParams = mission.BusParameter;
				return DeclarationData.BusAuxiliaries.CalculateLengthInteriorLights(
						vehicleLength, busParams.DoubleDecker, floorType, busParams.NumberPassengersLowerDeck)
					.Value();
			}

			return nbr.ToDouble();
		}

		private FloorType GetFloorType(VehicleCode vehicleCode)
		{
			switch (vehicleCode)
			{
				case VehicleCode.CA:
				case VehicleCode.CB:
				case VehicleCode.CC:
				case VehicleCode.CD:
					return FloorType.HighFloor;
				default:
					return FloorType.LowFloor;
			}
		}

		#endregion
		
		#region Vehicle Data Getter

		private List<Axle> GetAxles(IList<IAxleDeclarationInputData> axleWheels, double[] axlesDistribution)
		{
			var axles = new List<Axle>();
			for (int i = 0; i < axleWheels.Count; i++)
			{
				var axle = new Axle
				{
					WheelsDimension = axleWheels[i].Tyre.Dimension,
					Inertia = DeclarationData.Wheels
						.Lookup(axleWheels[i].Tyre.Dimension.RemoveWhitespace()).Inertia,
					TyreTestLoad = axleWheels[i].Tyre.TyreTestLoad,
					AxleWeightShare = axlesDistribution[i],
					TwinTyres = axleWheels[i].TwinTyres,
					AxleType = axleWheels[i].AxleType
				};
				axles.Add(axle);
			}

			return axles;
		}

		private Meter GetDynamicTyreRadius(IList<IAxleDeclarationInputData> axleWheels)
		{
			Meter dynamicTyreRadius = null;

			for (int i = 0; i < axleWheels.Count; i++)
			{
				if (axleWheels[i].AxleType == AxleType.VehicleDriven)
				{
					dynamicTyreRadius = DeclarationData.Wheels.Lookup(axleWheels[i].Tyre.Dimension.RemoveWhitespace()).DynamicTyreRadius;
					break;
				}
			}

			return dynamicTyreRadius;
		}

		private Kilogram GetLoading(IVehicleDeclarationInputData completedVehicle, Mission mission, KeyValuePair<LoadingType, Kilogram> loading)
		{
			var busFloorArea = DeclarationData.BusAuxiliaries.CalculateBusFloorSurfaceArea(completedVehicle.Length,
				completedVehicle.Width);
			var passengerCountRef = busFloorArea * mission.BusParameter.PassengerDensity;
			var passengerCountDecl = completedVehicle.NuberOfPassengersUpperDeck + completedVehicle.NumberOfPassengersLowerDeck;
			
			if (loading.Key != LoadingType.ReferenceLoad && loading.Key != LoadingType.LowLoading)
			{
				throw new VectoException("Unhandled loading type: {0}", loading.Key);
			}


			return 
				(loading.Key == LoadingType.ReferenceLoad
					? VectoMath.Min(passengerCountRef, passengerCountDecl)
					: passengerCountRef * mission.MissionType.GetLowLoadFactorBus()) * mission.MissionType.GetAveragePassengerMass();
		}

		#endregion


	}
}
