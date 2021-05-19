using System;
using System.Collections.Generic;
using System.Linq;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.InputData.Reader.Impl;
using TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.Electrics;
using TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.HVAC;
using TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.Pneumatics;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;

namespace TUGraz.VectoCore.InputData.Reader.DataObjectAdapter
{
	public class DeclarationDataAdapterSingleBus : DeclarationDataAdapterCompletedBusSpecific
	{
		#region Implementation of IDeclarationDataAdapter

		public override VehicleData CreateVehicleData(IVehicleDeclarationInputData vehicle, Segment segment, Mission mission, KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading, bool allowVocational)
		{
			var busFloorArea = DeclarationData.BusAuxiliaries.CalculateBusFloorSurfaceArea(
				CompletedVehicle.Length,
				CompletedVehicle.Width);
			var passengerCountRef = busFloorArea * (loading.Key == LoadingType.LowLoading
				? mission.BusParameter.PassengerDensityLow
				: mission.BusParameter.PassengerDensityRef);
			var passengerCountDecl = CompletedVehicle.NumberOfPassengersUpperDeck + CompletedVehicle.NumberOfPassengersLowerDeck;

			//var refLoad = passengerCount * mission.MissionType.GetAveragePassengerMass();
			if (loading.Key != LoadingType.ReferenceLoad && loading.Key != LoadingType.LowLoading) {
				throw new VectoException("Unhandled loading type: {0}", loading.Key);
			}

			var passengerCountCalc = loading.Key == LoadingType.ReferenceLoad
				? VectoMath.Min(passengerCountRef, (int)passengerCountDecl)
				: passengerCountRef * mission.MissionType.GetLowLoadFactorBus();
			var payload = passengerCountCalc * mission.MissionType.GetAveragePassengerMass();

			var retVal = CreateNonExemptedVehicleData(vehicle, segment, mission, payload, passengerCountCalc, allowVocational);
			retVal.CurbMass = CompletedVehicle.CurbMassChassis;
			return retVal;
		}

		#endregion


		protected override TransmissionLossMap CreateGearLossMap(ITransmissionInputData gear, uint i, bool useEfficiencyFallback, VehicleCategory vehicleCategory, GearboxType gearboxType)
		{
			return TransmissionLossMapReader.Create(gear.LossMap, gear.Ratio, string.Format("Gear {0}", i + 1), true);
		}


		protected override TorqueConverterData CreateTorqueConverterData(GearboxType gearboxType,
			ITorqueConverterDeclarationInputData torqueConverter, double ratio, CombustionEngineData engineData)
		{
			return TorqueConverterDataReader.Create(
				torqueConverter.TCData,
				DeclarationData.TorqueConverter.ReferenceRPM, DeclarationData.TorqueConverter.MaxInputSpeed,
				ExecutionMode.Declaration, ratio,
				DeclarationData.TorqueConverter.CLUpshiftMinAcceleration,
				DeclarationData.TorqueConverter.CCUpshiftMinAcceleration);
		}


		public override AxleGearData CreateAxleGearData(IAxleGearInputData data)
		{
			var retVal = SetCommonAxleGearData(data);
			retVal.AxleGear.LossMap = ReadAxleLossMap(data, false);
			return retVal;
		}

		#region Overrides of DeclarationDataAdapterCompletedBusGeneric

		public override RetarderData CreateRetarderData(IRetarderInputData retarder)
		{
			return SetCommonRetarderData(retarder);
		}

		#endregion

		public ISingleBusInputDataProvider SingleBusInputData { get; set; }

		protected IVehicleDeclarationInputData CompletedVehicle
		{
			get { return SingleBusInputData?.CompletedVehicle; }
		}

	}
}
