using System;
using System.Collections.Generic;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents;
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

			private readonly ICompletedBusAuxiliaryDataAdapter _auxDataAdapter =
				new SpecificCompletedBusAuxiliaryDataAdapter(new PrimaryBusAuxiliaryDataAdapter());

			#region Implementation of ISpecificCompletedBusDeclarationDataAdapter

			public IAuxiliaryConfig CreateBusAuxiliariesData(Mission mission, IVehicleDeclarationInputData primaryVehicle,
				IVehicleDeclarationInputData completedVehicle, VectoRunData runData)
			{
				throw new NotImplementedException();
			}

			public VehicleData CreateVehicleData(IVehicleDeclarationInputData primaryVehicle,
				IVehicleDeclarationInputData completedVehicle, Segment segment, Mission mission, KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading)
			{
				throw new NotImplementedException();
			}

			public IList<VectoRunData.AuxData> CreateAuxiliaryData(IAuxiliariesDeclarationInputData auxData, IBusAuxiliariesDeclarationData busAuxData,
				MissionType missionType, VehicleClass vehicleClass, Meter vehicleLength, int? numSteeredAxles)
			{
				throw new NotImplementedException();
			}

			public AirdragData CreateAirdragData(IVehicleDeclarationInputData completedVehicle, Mission mission)
			{
				throw new NotImplementedException();
			}

			public CombustionEngineData CreateEngineData(IVehicleDeclarationInputData primaryVehicle, int modeIdx, Mission mission)
			{
				throw new NotImplementedException();
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
	}
}