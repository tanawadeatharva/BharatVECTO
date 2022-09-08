
using System;
using System.Collections.Generic;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents
{
	public interface IVehicleDataAdapter
	{
		VehicleData CreateVehicleData(IVehicleDeclarationInputData data, Segment segment, Mission mission,
			Kilogram loading, double? passengerCount, bool allowVocational);
		VehicleData CreateExemptedVehicleData(IVehicleDeclarationInputData data);
	}
    internal abstract class VehicleDataAdapter : ComponentDataAdapterBase, IVehicleDataAdapter
	{
		public static NewtonMeter VehMaxTorque(
			ITransmissionInputData gear, int numGears,
			Dictionary<int, ITorqueLimitInputData> limits,
			NewtonMeter maxEngineTorque)
		{
			if (gear.Gear - 1 >= numGears / 2)
			{
				// only upper half of gears can limit if max-torque <= 0.95 of engine max torque
				if (limits.TryGetValue(gear.Gear, out var limit) && limit.MaxTorque <=
					DeclarationData.Engine.TorqueLimitVehicleFactor * maxEngineTorque)
				{
					return limit.MaxTorque;
				}
			}

			return null;
		} 
		internal static IADASDataAdapter _adasDataAdapter = new ADASDataAdapter();
		internal static VehicleData SetCommonVehicleData(IVehicleDeclarationInputData data)
		{
			var retVal = new VehicleData
			{
				InputData = data,
				SavedInDeclarationMode = data.SavedInDeclarationMode,
				Manufacturer = data.Manufacturer,
				ModelName = data.Model,
				Date = data.Date,
				//CertificationNumber = data.CertificationNumber,
				DigestValueInput = data.DigestValue != null ? data.DigestValue.DigestValue : "",
				VehicleCategory = data.VehicleCategory,
				CurbMass = data.CurbMassChassis,
				GrossVehicleMass = data.GrossVehicleMassRating,
				AirDensity = Physics.AirDensity,
			};

			return retVal;
		}


		#region Implementation of IVehicleDataAdapter

		public VehicleData CreateVehicleData(IVehicleDeclarationInputData data,
			Segment segment,
			Mission mission,
			Kilogram loading,
			double? passengerCount,
			bool allowVocational)
		{
			CheckDeclarationMode(data, "Vehicle");
			return DoCreateVehicleData(data, segment, mission, loading, passengerCount, allowVocational);
		}
		public VehicleData CreateVehicleData(IVehicleDeclarationInputData data,
			Segment segment,
			Mission mission,
			KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading,
			bool allowVocational)
		{
			CheckDeclarationMode(data, "Vehicle");
			return DoCreateVehicleData(data, segment, mission, loading.Value.Item1, loading.Value.Item2, allowVocational);
		}
		protected abstract VehicleData DoCreateVehicleData(IVehicleDeclarationInputData data,
			Segment segment,
			Mission mission,
			Kilogram loading,
			double? passengerCount,
			bool allowVocational);

		public VehicleData CreateExemptedVehicleData(IVehicleDeclarationInputData data)
		{
			CheckDeclarationMode(data, "Vehicle");
			return DoCreateExemptedVehicleData(data);
		}

		protected abstract VehicleData DoCreateExemptedVehicleData(IVehicleDeclarationInputData data);

		protected static VehicleData GetVehicleData(IVehicleDeclarationInputData data,
			Segment segment, Mission mission, Kilogram loading, double? passengerCount, bool allowVocational)
		{
			var retVal = SetCommonVehicleData(data);
			retVal.LegislativeClass = data.LegislativeClass;
			retVal.AxleConfiguration = data.AxleConfiguration;
			retVal.AirDensity = DeclarationData.AirDensity;
			retVal.VIN = data.VIN;
			retVal.ManufacturerAddress = data.ManufacturerAddress;
			//			retVal.LegislativeClass = data.LegislativeClass;
			retVal.ZeroEmissionVehicle = data.ZeroEmissionVehicle;
			retVal.VehicleClass = segment.VehicleClass;
			retVal.SleeperCab = retVal.VehicleClass.IsMediumLorry() ? false : data.SleeperCab;
			retVal.TrailerGrossVehicleMass = mission.Trailer.Sum(t => t.TrailerGrossVehicleWeight).DefaultIfNull(0);

			retVal.BodyAndTrailerMass =
				mission.BodyCurbWeight + mission.Trailer.Sum(t => t.TrailerCurbWeight).DefaultIfNull(0);

			retVal.Loading = loading;
			retVal.PassengerCount = passengerCount;
			retVal.DynamicTyreRadius =
				data.Components.AxleWheels.AxlesDeclaration.Where(axle => axle.AxleType == AxleType.VehicleDriven)
					.Select(da => DeclarationData.Wheels.Lookup(da.Tyre.Dimension).DynamicTyreRadius)
					.Average();
			if (segment.VehicleClass.IsMediumLorry() && segment.VehicleClass.IsVan())
			{
				retVal.CargoVolume = data.CargoVolume;
			}
			else
			{
				retVal.CargoVolume = mission.MissionType != MissionType.Construction
					? mission.TotalCargoVolume
					: 0.SI<CubicMeter>();
			}

			retVal.VocationalVehicle = allowVocational && data.VocationalVehicle;
			retVal.ADAS = _adasDataAdapter.CreateADAS(data.ADAS);

			var axles = data.Components.AxleWheels.AxlesDeclaration;
			if (axles.Count < mission.AxleWeightDistribution.Length)
			{
				throw new VectoException(
					"Vehicle does not contain sufficient axles. {0} axles defined, {1} axles required",
					axles.Count, mission.AxleWeightDistribution.Length);
			}

			var axleData = new List<Axle>();
			for (var i = 0; i < mission.AxleWeightDistribution.Length; i++)
			{
				var axleInput = axles[i];
				var axle = new Axle
				{
					WheelsDimension = axleInput.Tyre.Dimension,
					AxleType = axleInput.AxleType,
					AxleWeightShare = mission.AxleWeightDistribution[i],
					TwinTyres = axleInput.TwinTyres,
					RollResistanceCoefficient = axleInput.Tyre.RollResistanceCoefficient,
					TyreTestLoad = axleInput.Tyre.TyreTestLoad,
					FuelEfficiencyClass = axleInput.Tyre.FuelEfficiencyClass,
					Inertia = DeclarationData.Wheels.Lookup(axleInput.Tyre.Dimension.RemoveWhitespace()).Inertia,
					CertificationNumber = axleInput.Tyre.CertificationNumber,
					DigestValueInput = axleInput.Tyre.DigestValue == null
						? ""
						: axleInput.Tyre.DigestValue.DigestValue,
				};
				axleData.Add(axle);
			}

			foreach (var trailer in mission.Trailer)
			{
				axleData.AddRange(
					trailer.TrailerWheels.Select(
						trailerWheel => new Axle
						{
							AxleType = AxleType.Trailer,
							AxleWeightShare = trailer.TrailerAxleWeightShare / trailer.TrailerWheels.Count,
							TwinTyres = DeclarationData.Trailer.TwinTyres,
							RollResistanceCoefficient = DeclarationData.Trailer.RollResistanceCoefficient,
							TyreTestLoad = DeclarationData.Trailer.TyreTestLoad.SI<Newton>(),
							FuelEfficiencyClass = DeclarationData.Trailer.FuelEfficiencyClass,
							Inertia = trailerWheel.Inertia,
							WheelsDimension = trailerWheel.WheelType
						}));
			}

			retVal.AxleData = axleData;
			return retVal;
		}
		#endregion
	}

	internal class LorryVehicleDataAdapter : VehicleDataAdapter
	{
		protected override VehicleData DoCreateVehicleData(IVehicleDeclarationInputData data,
			Segment segment, Mission mission, Kilogram loading, double? passengerCount, bool allowVocational)
		{
			return GetVehicleData(data, segment, mission, loading, passengerCount, allowVocational);
		}

		protected override VehicleData DoCreateExemptedVehicleData(IVehicleDeclarationInputData data)
		{
			var exempted = SetCommonVehicleData(data);
			exempted.VIN = data.VIN;
			exempted.ManufacturerAddress = data.ManufacturerAddress;
			exempted.LegislativeClass = data.LegislativeClass;
			exempted.ZeroEmissionVehicle = data.ZeroEmissionVehicle;
			exempted.HybridElectricHDV = data.HybridElectricHDV;
			exempted.DualFuelVehicle = data.DualFuelVehicle;
			exempted.MaxNetPower1 = data.MaxNetPower1;
			exempted.MaxNetPower2 = data.MaxNetPower2;
			return exempted;
		}
	}

	internal class PrimaryBusVehicleDataAdapter : LorryVehicleDataAdapter
	{
		#region Overrides of VehicleDataAdapter

		protected override VehicleData DoCreateVehicleData(IVehicleDeclarationInputData data, Segment segment, Mission mission, Kilogram loading,
			double? passengerCount, bool allowVocational)
		{
			var retVal = base.DoCreateVehicleData(data, segment, mission, loading, passengerCount, allowVocational);
			if (data.ExemptedVehicle)
			{ 
				System.Diagnostics.Debug.Assert( false, "CreateExemptedVehicleData should be used");
				return retVal;
			}
			retVal.CurbMass = mission.CurbMass;
			retVal.GrossVehicleMass = 40000.SI<Kilogram>();
			return retVal;
		}

		#endregion

		protected override VehicleData DoCreateExemptedVehicleData(IVehicleDeclarationInputData data)
		{
			var exempted = new VehicleData
			{
				InputData = data,
				SavedInDeclarationMode = data.SavedInDeclarationMode,
				Manufacturer = data.Manufacturer,
				ModelName = data.Model,
				Date = data.Date,
				//CertificationNumber = data.CertificationNumber,
				DigestValueInput = data.DigestValue != null ? data.DigestValue.DigestValue : "",
				VehicleCategory = data.VehicleCategory,
				//CurbMass = data.CurbMassChassis,
				GrossVehicleMass = data.GrossVehicleMassRating,
				AirDensity = Physics.AirDensity,
			};
			exempted.VIN = data.VIN;
			exempted.ManufacturerAddress = data.ManufacturerAddress;
			exempted.LegislativeClass = data.LegislativeClass;
			exempted.ZeroEmissionVehicle = data.ZeroEmissionVehicle;
			exempted.HybridElectricHDV = data.HybridElectricHDV;
			exempted.DualFuelVehicle = data.DualFuelVehicle;
			exempted.MaxNetPower1 = data.MaxNetPower1;
			exempted.MaxNetPower2 = data.MaxNetPower2;
			exempted.AxleConfiguration = data.AxleConfiguration;
			return exempted;
		}

	}

	internal class CompletedBusGenericVehicleDataAdapter : PrimaryBusVehicleDataAdapter
	{
		#region Overrides of PrimaryBusVehicleDataAdapter

		protected override VehicleData DoCreateVehicleData(IVehicleDeclarationInputData data, Segment segment, Mission mission, Kilogram loading,
			double? passengerCount, bool allowVocational)
		{
			var retVal = base.DoCreateVehicleData(data, segment, mission, loading, passengerCount, allowVocational);
			retVal.GrossVehicleMass = data.GrossVehicleMassRating;
			if (retVal.TotalVehicleMass.IsGreater(retVal.GrossVehicleMass))
			{
				throw new VectoException("Total Vehicle Mass exceeds Gross Vehicle Mass for completed bus generic ({0}/{1})", retVal.TotalVehicleMass, retVal.GrossVehicleMass);
			}
			return retVal;
		}

		protected override VehicleData DoCreateExemptedVehicleData(IVehicleDeclarationInputData data)
		{
			var retVal = base.DoCreateExemptedVehicleData(data);
			retVal.GrossVehicleMass = data.GrossVehicleMassRating;
			if (retVal.TotalVehicleMass.IsGreater(retVal.GrossVehicleMass))
			{
				throw new VectoException("Total Vehicle Mass exceeds Gross Vehicle Mass for completed bus generic ({0}/{1})", retVal.TotalVehicleMass, retVal.GrossVehicleMass);
			}
			return retVal;
		}

		#endregion
	}

	internal class SingleBusVehicleDataAdapter : VehicleDataAdapter
	{
		#region Overrides of VehicleDataAdapter

		public VehicleData CreateVehicleData(ISingleBusInputDataProvider data, Segment segment, Mission mission, KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading, bool allowVocational)
		{
			var completedVehicle = data.CompletedVehicle;
			var vehicle = data.JobInputData.Vehicle;
			var busFloorArea = DeclarationData.BusAuxiliaries.CalculateBusFloorSurfaceArea(
				completedVehicle.Length,
				completedVehicle.Width);
			var passengerCountRef = busFloorArea * (loading.Key == LoadingType.LowLoading
				? mission.BusParameter.PassengerDensityLow
				: mission.BusParameter.PassengerDensityRef);
			var passengerCountDecl = completedVehicle.NumberPassengerSeatsUpperDeck +
									completedVehicle.NumberPassengerSeatsLowerDeck
									+ (mission.MissionType == MissionType.Coach
										? 0
										: completedVehicle.NumberPassengersStandingLowerDeck +
										completedVehicle.NumberPassengersStandingUpperDeck);

			//var refLoad = passengerCount * mission.MissionType.GetAveragePassengerMass();
			if (loading.Key != LoadingType.ReferenceLoad && loading.Key != LoadingType.LowLoading)
			{
				throw new VectoException("Unhandled loading type: {0}", loading.Key);
			}

			var passengerCountCalc = loading.Key == LoadingType.ReferenceLoad
				? VectoMath.Min(passengerCountRef, (int)passengerCountDecl)
				: passengerCountRef * mission.MissionType.GetLowLoadFactorBus();
			var payload = passengerCountCalc * mission.MissionType.GetAveragePassengerMass();

			var retVal = DoCreateVehicleData(vehicle, segment, mission, payload, passengerCountCalc, allowVocational);
			retVal.CurbMass = completedVehicle.CurbMassChassis;
			return retVal;
		}

		protected override VehicleData DoCreateVehicleData(IVehicleDeclarationInputData data, Segment segment, Mission mission, Kilogram loading,
			double? passengerCount, bool allowVocational)
		{
			return VehicleDataAdapter.GetVehicleData(data, segment, mission, loading, passengerCount, allowVocational);
		}

		protected override VehicleData DoCreateExemptedVehicleData(IVehicleDeclarationInputData data)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
