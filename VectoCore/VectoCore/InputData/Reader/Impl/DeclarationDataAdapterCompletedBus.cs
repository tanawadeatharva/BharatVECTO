using System.Collections.Generic;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data;

namespace TUGraz.VectoCore.InputData.Reader.Impl
{
	public class DeclarationDataAdapterCompletedBus
	{
		//Specific

		public DriverData CreateDriverData()
		{
			throw new System.NotImplementedException();
		}

		public AirdragData CreateAirdragData(IAirdragDeclarationInputData airdragInputData, Mission mission, Segment segment)
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

		public IEnumerable<VectoRunData.AuxData> CreateAuxiliaryData(
			IAuxiliariesDeclarationInputData auxiliaryInputData, IBusAuxiliariesDeclarationData mergedBusAux,
			MissionType mission, VehicleClass vehicleClass, Meter vehicleLength)
		{
			throw new System.NotImplementedException();
		}
		
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
