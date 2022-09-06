using System;
using System.Collections.Generic;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent;
using TUGraz.VectoCore.Models.SimulationComponent.Data;

namespace TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.HeavyLorry
{



	public partial class DeclarationDataAdapterHeavyLorry
	{
		[Obsolete("Use DeclarationDataAdapterHeavyLorry.Conventional instead, created automatically with NInject")]
		public DeclarationDataAdapterHeavyLorry() { }

		private IDeclarationDataAdapter _declarationDataAdapterImplementation = new Conventional();
		public static readonly GearboxType[] SupportedGearboxTypes = Conventional.SupportedGearboxTypes;

		//public static List<CrossWindCorrectionCurveReader.CrossWindCorrectionEntry> GetDeclarationAirResistanceCurve(
		//	string parameterSet, SquareMeter si, Meter meter)
		//{
		//	//return Conventional.GetDeclarationAirResistanceCurve(parameterSet, si, meter);
		//}

		#region Implementation of IDeclarationDataAdapter

		public DriverData CreateDriverData()
		{
			return _declarationDataAdapterImplementation.CreateDriverData();
		}

		public VehicleData CreateVehicleData(IVehicleDeclarationInputData vehicle, Segment segment, Mission mission,
			KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading, bool allowVocational)
		{
			return _declarationDataAdapterImplementation.CreateVehicleData(vehicle, segment, mission, loading,
				allowVocational);
		}

		public AirdragData CreateAirdragData(IAirdragDeclarationInputData airdragData, Mission mission, Segment segment)
		{
			return _declarationDataAdapterImplementation.CreateAirdragData(airdragData, mission, segment);
		}

		public AxleGearData CreateAxleGearData(IAxleGearInputData axlegearData)
		{
			return _declarationDataAdapterImplementation.CreateAxleGearData(axlegearData);
		}

		public AngledriveData CreateAngledriveData(IAngledriveInputData angledriveData)
		{
			return _declarationDataAdapterImplementation.CreateAngledriveData(angledriveData);
		}

		public CombustionEngineData CreateEngineData(IVehicleDeclarationInputData vehicle,
			IEngineModeDeclarationInputData engineMode,
			Mission mission)
		{
			return _declarationDataAdapterImplementation.CreateEngineData(vehicle, engineMode, mission);
		}

		public GearboxData CreateGearboxData(IVehicleDeclarationInputData inputData, VectoRunData runData,
			IShiftPolygonCalculator shiftPolygonCalc)
		{
			return _declarationDataAdapterImplementation.CreateGearboxData(inputData, runData, shiftPolygonCalc);
		}

		public ShiftStrategyParameters CreateGearshiftData(GearboxData gbx, double axleRatio,
			PerSecond engineIdlingSpeed)
		{
			return _declarationDataAdapterImplementation.CreateGearshiftData(gbx, axleRatio, engineIdlingSpeed);
		}

		public RetarderData CreateRetarderData(IRetarderInputData retarderData)
		{
			return _declarationDataAdapterImplementation.CreateRetarderData(retarderData);
		}

		public PTOData CreatePTOTransmissionData(IPTOTransmissionInputData ptoData)
		{
			return _declarationDataAdapterImplementation.CreatePTOTransmissionData(ptoData);
		}

		public IList<VectoRunData.AuxData> CreateAuxiliaryData(IAuxiliariesDeclarationInputData auxData,
			IBusAuxiliariesDeclarationData busAuxData,
			MissionType missionType, VehicleClass vehicleClass, Meter vehicleLength, int? numSteeredAxles)
		{
			return _declarationDataAdapterImplementation.CreateAuxiliaryData(auxData, busAuxData, missionType,
				vehicleClass, vehicleLength, numSteeredAxles);
		}

		public AxleGearData CreateDummyAxleGearData(IGearboxDeclarationInputData gbxData)
		{
			return _declarationDataAdapterImplementation.CreateDummyAxleGearData(gbxData);
		}

		#endregion
	}
}