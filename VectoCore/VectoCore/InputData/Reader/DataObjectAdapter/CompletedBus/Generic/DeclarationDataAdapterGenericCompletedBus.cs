using System;
using System.Collections.Generic;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent;
using TUGraz.VectoCore.Models.SimulationComponent.Data;

namespace TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.CompletedBus.Generic
{
	public abstract class DeclarationDataAdapterGenericCompletedBus
	{
		public abstract class CompletedBusBase : IGenericCompletedBusDataAdapter
		{
			private readonly IRetarderDataAdapter _retarderDataAdapter = new GenericRetarderDataAdapter();
            #region Implementation of IDeclarationDataAdapter

			private readonly IDriverDataAdapter _driverDataAdapter = new CompletedBusGenericDriverDataAdapter();
            public DriverData CreateDriverData()
            {
                return _driverDataAdapter.CreateDriverData();
            }

            public VehicleData CreateVehicleData(IVehicleDeclarationInputData vehicle, Segment segment, Mission mission,
                KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading, bool allowVocational)
            {
                throw new NotImplementedException();
            }

            public AirdragData CreateAirdragData(IAirdragDeclarationInputData airdragData, Mission mission, Segment segment)
            {
                throw new NotImplementedException();
            }

            public AxleGearData CreateAxleGearData(IAxleGearInputData axlegearData)
            {
                throw new NotImplementedException();
            }

            public AngledriveData CreateAngledriveData(IAngledriveInputData angledriveData)
            {
                throw new NotImplementedException();
            }

            public CombustionEngineData CreateEngineData(IVehicleDeclarationInputData vehicle, IEngineModeDeclarationInputData engineMode,
                Mission mission)
            {
                throw new NotImplementedException();
            }

            public GearboxData CreateGearboxData(IVehicleDeclarationInputData inputData, VectoRunData runData,
                IShiftPolygonCalculator shiftPolygonCalc)
            {
                throw new NotImplementedException();
            }

            public ShiftStrategyParameters CreateGearshiftData(GearboxData gbx, double axleRatio, PerSecond engineIdlingSpeed)
            {
                throw new NotImplementedException();
            }

            public RetarderData CreateRetarderData(IRetarderInputData retarderData)
			{
				return _retarderDataAdapter.CreateRetarderData(retarderData);
			}

            public PTOData CreatePTOTransmissionData(IPTOTransmissionInputData ptoData)
            {
                throw new NotImplementedException();
            }

            public IList<VectoRunData.AuxData> CreateAuxiliaryData(IAuxiliariesDeclarationInputData auxData, IBusAuxiliariesDeclarationData busAuxData,
                MissionType missionType, VehicleClass vehicleClass, Meter vehicleLength, int? numSteeredAxles)
            {
                throw new NotImplementedException();
            }

            public AxleGearData CreateDummyAxleGearData(IGearboxDeclarationInputData gbxData)
            {
                throw new NotImplementedException();
            }

            #endregion
		}
		public class Conventional : CompletedBusBase { }
		public class HEV_S2 : CompletedBusBase { }
		public class HEV_S3 : CompletedBusBase { }
		public class HEV_S4 : CompletedBusBase { }
		public class HEV_S_IEPC : CompletedBusBase { }
		public class HEV_P1 : CompletedBusBase { }
		public class HEV_P2 : CompletedBusBase { }
		public class HEV_P2_5 : CompletedBusBase { }
		public class HEV_P3 : CompletedBusBase { }
		public class HEV_P4 : CompletedBusBase { }
		public class PEV_E2 : CompletedBusBase { }
		public class PEV_E3 : CompletedBusBase { }
		public class PEV_E4 : CompletedBusBase { }
		public class PEV_E_IEPC : CompletedBusBase { }
    }
}