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

namespace TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.HeavyLorry
{
	public partial class DeclarationDataAdapterHeavyLorry
	{
		public abstract class LorryBase : AbstractSimulationDataAdapter, IDeclarationDataAdapter
		{
			#region Implementation of IDeclarationDataAdapter

			private readonly IDriverDataAdapter _driverDataAdapter = new LorryDriverDataAdapter();

			public DriverData CreateDriverData()
			{
				return _driverDataAdapter.CreateDriverData();
			}

			public virtual VehicleData CreateVehicleData(IVehicleDeclarationInputData vehicle, Segment segment,
				Mission mission,
				KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading, bool allowVocational)
			{
				throw new NotImplementedException();
			}

			public virtual AirdragData CreateAirdragData(IAirdragDeclarationInputData airdragData, Mission mission,
				Segment segment)
			{
				throw new NotImplementedException();
			}

			public virtual AxleGearData CreateAxleGearData(IAxleGearInputData axlegearData)
			{
				throw new NotImplementedException();
			}

			public virtual AngledriveData CreateAngledriveData(IAngledriveInputData angledriveData)
			{
				throw new NotImplementedException();
			}

			public virtual CombustionEngineData CreateEngineData(IVehicleDeclarationInputData vehicle,
				IEngineModeDeclarationInputData engineMode,
				Mission mission)
			{
				throw new NotImplementedException();
			}

			public virtual GearboxData CreateGearboxData(IVehicleDeclarationInputData inputData, VectoRunData runData,
				IShiftPolygonCalculator shiftPolygonCalc)
			{
				throw new NotImplementedException();
			}

			public virtual ShiftStrategyParameters CreateGearshiftData(GearboxData gbx, double axleRatio,
				PerSecond engineIdlingSpeed)
			{
				throw new NotImplementedException();
			}

			public virtual RetarderData CreateRetarderData(IRetarderInputData retarderData)
			{
				throw new NotImplementedException();
			}

			public virtual PTOData CreatePTOTransmissionData(IPTOTransmissionInputData ptoData)
			{
				throw new NotImplementedException();
			}

			public virtual IList<VectoRunData.AuxData> CreateAuxiliaryData(IAuxiliariesDeclarationInputData auxData,
				IBusAuxiliariesDeclarationData busAuxData,
				MissionType missionType, VehicleClass vehicleClass, Meter vehicleLength, int? numSteeredAxles)
			{
				throw new NotImplementedException();
			}

			public virtual AxleGearData CreateDummyAxleGearData(IGearboxDeclarationInputData gbxData)
			{
				throw new NotImplementedException();
			}

			#endregion
		}
	}
}