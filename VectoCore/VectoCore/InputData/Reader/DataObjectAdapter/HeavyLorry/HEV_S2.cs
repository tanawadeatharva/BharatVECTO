using System;
using System.Collections.Generic;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent;
using TUGraz.VectoCore.Models.SimulationComponent.Data;

namespace TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.HeavyLorry
{
	public partial class DeclarationDataAdapterHeavyLorry
	{
		public class HEV_S2 : LorryBase
		{
			#region Overrides of LorryBase

			public override DriverData CreateDriverData()
			{
				throw new NotImplementedException();
			}

			public override VehicleData CreateVehicleData(IVehicleDeclarationInputData vehicle, Segment segment,
				Mission mission,
				KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading, bool allowVocational)
			{
				throw new NotImplementedException();
			}

			public override AirdragData CreateAirdragData(IAirdragDeclarationInputData airdragData, Mission mission,
				Segment segment)
			{
				throw new NotImplementedException();
			}

			public override AxleGearData CreateAxleGearData(IAxleGearInputData axlegearData)
			{
				throw new NotImplementedException();
			}

			public override AngledriveData CreateAngledriveData(IAngledriveInputData angledriveData)
			{
				throw new NotImplementedException();
			}

			public override CombustionEngineData CreateEngineData(IVehicleDeclarationInputData vehicle,
				IEngineModeDeclarationInputData engineMode,
				Mission mission)
			{
				throw new NotImplementedException();
			}

			public override GearboxData CreateGearboxData(IVehicleDeclarationInputData inputData, VectoRunData runData,
				IShiftPolygonCalculator shiftPolygonCalc)
			{
				throw new NotImplementedException();
			}

			public override ShiftStrategyParameters CreateGearshiftData(GearboxData gbx, double axleRatio,
				PerSecond engineIdlingSpeed)
			{
				throw new NotImplementedException();
			}

			public override RetarderData CreateRetarderData(IRetarderInputData retarderData)
			{
				throw new NotImplementedException();
			}

			public override PTOData CreatePTOTransmissionData(IPTOTransmissionInputData ptoData)
			{
				throw new NotImplementedException();
			}

			public override IList<VectoRunData.AuxData> CreateAuxiliaryData(IAuxiliariesDeclarationInputData auxData,
				IBusAuxiliariesDeclarationData busAuxData,
				MissionType missionType, VehicleClass vehicleClass, Meter vehicleLength, int? numSteeredAxles)
			{
				throw new NotImplementedException();
			}

			public override AxleGearData CreateDummyAxleGearData(IGearboxDeclarationInputData gbxData)
			{
				throw new NotImplementedException();
			}

			#endregion
		}
	}
}