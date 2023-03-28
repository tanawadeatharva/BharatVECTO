using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents.Interfaces;
using TUGraz.VectoCore.Models.BusAuxiliaries;
using TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.Electrics;
using TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.Pneumatics;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent;
using TUGraz.VectoCore.Models.SimulationComponent.Data;

namespace TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.CompletedBus.Specific
{
    public abstract class DeclarationDataAdapterSpecificCompletedBus
	{
		public abstract class CompletedBusDeclarationBase : AbstractSimulationDataAdapter, ISpecificCompletedBusDeclarationDataAdapter
        {
			private readonly IDriverDataAdapter _driverDataAdapter = new CompletedBusSpecificDriverDataAdapter();
			protected readonly IVehicleDataAdapter _vehicleDataAdapter = new CompletedBusSpecificVehicleDataAdapter();
			protected readonly IAirdragDataAdapter _airdragDataAdapter = new CompletedBusSpecificAirdragDataAdapter();
			private readonly IEngineDataAdapter _engineDataAdapter = new GenericCombustionEngineComponentDataAdapter();
			
			private readonly ICompletedBusAuxiliaryDataAdapter _auxDataAdapter =
				new SpecificCompletedBusAuxiliaryDataAdapter(new PrimaryBusAuxiliaryDataAdapter());

			#region Implementation of ISpecificCompletedBusDeclarationDataAdapter

			public IAuxiliaryConfig CreateBusAuxiliariesData(Mission mission, IVehicleDeclarationInputData primaryVehicle,
				IVehicleDeclarationInputData completedVehicle, VectoRunData runData)
			{
                //throw new NotImplementedException();
                return _auxDataAdapter.CreateBusAuxiliariesData(mission, primaryVehicle, completedVehicle, runData);
            }

			public VehicleData CreateVehicleData(IVehicleDeclarationInputData primaryVehicle,
				IVehicleDeclarationInputData completedVehicle, Segment segment, Mission mission, KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading)
			{
				return _vehicleDataAdapter.CreateVehicleData(primaryVehicle, completedVehicle, segment, mission,
					loading);
			}

			public IList<VectoRunData.AuxData> CreateAuxiliaryData(IAuxiliariesDeclarationInputData auxData, IBusAuxiliariesDeclarationData busAuxData,
				MissionType missionType, VehicleClass vehicleClass, Meter vehicleLength, int? numSteeredAxles, VectoSimulationJobType jobType)
			{
				//throw new NotImplementedException();
				return _auxDataAdapter.CreateAuxiliaryData(auxData, busAuxData, missionType, vehicleClass,
					vehicleLength, numSteeredAxles, jobType);
			}

			public AirdragData CreateAirdragData(IVehicleDeclarationInputData completedVehicle, Mission mission)
			{
				return _airdragDataAdapter.CreateAirdragData(completedVehicle, mission);
			}

			public CombustionEngineData CreateEngineData(IVehicleDeclarationInputData primaryVehicle, int modeIdx, Mission mission)
			{
				return _engineDataAdapter.CreateEngineData(primaryVehicle, modeIdx, mission);
			}

			#endregion

			#region Implementation of IDeclarationDataAdapter

			public VehicleData CreateVehicleData(IVehicleDeclarationInputData vehicle, Segment segment, Mission mission,
				KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading, bool allowVocational)
			{
				throw new NotImplementedException();
			}

			#endregion
		}


        public class Conventional : CompletedBusDeclarationBase { }
        public class HEV_S2 : CompletedBusDeclarationBase { }
        public class HEV_S3 : CompletedBusDeclarationBase { }
        public class HEV_S4 : CompletedBusDeclarationBase { }
        public class HEV_S_IEPC : CompletedBusDeclarationBase { }
        public class HEV_P1 : CompletedBusDeclarationBase { }
        public class HEV_P2 : CompletedBusDeclarationBase { }
        public class HEV_P2_5 : CompletedBusDeclarationBase { }
        public class HEV_P3 : CompletedBusDeclarationBase { }
        public class HEV_P4 : CompletedBusDeclarationBase { }
        public class PEV_E2 : CompletedBusDeclarationBase { }
        public class PEV_E3 : CompletedBusDeclarationBase { }
        public class PEV_E4 : CompletedBusDeclarationBase { }
        public class PEV_E_IEPC : CompletedBusDeclarationBase { }
		public class Exempted : CompletedBusDeclarationBase { }
	}
}