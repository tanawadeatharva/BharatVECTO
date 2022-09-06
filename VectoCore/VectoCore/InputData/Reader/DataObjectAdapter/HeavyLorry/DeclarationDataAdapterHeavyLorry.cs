/*
* This file is part of VECTO.
*
* Copyright © 2012-2019 European Union
*
* Developed by Graz University of Technology,
*              Institute of Internal Combustion Engines and Thermodynamics,
*              Institute of Technical Informatics
*
* VECTO is licensed under the EUPL, Version 1.1 or - as soon they will be approved
* by the European Commission - subsequent versions of the EUPL (the "Licence");
* You may not use VECTO except in compliance with the Licence.
* You may obtain a copy of the Licence at:
*
* https://joinup.ec.europa.eu/community/eupl/og_page/eupl
*
* Unless required by applicable law or agreed to in writing, VECTO
* distributed under the Licence is distributed on an "AS IS" basis,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the Licence for the specific language governing permissions and
* limitations under the Licence.
*
* Authors:
*   Stefan Hausberger, hausberger@ivt.tugraz.at, IVT, Graz University of Technology
*   Christian Kreiner, christian.kreiner@tugraz.at, ITI, Graz University of Technology
*   Michael Krisper, michael.krisper@tugraz.at, ITI, Graz University of Technology
*   Raphael Luz, luz@ivt.tugraz.at, IVT, Graz University of Technology
*   Markus Quaritsch, markus.quaritsch@tugraz.at, IVT, Graz University of Technology
*   Martin Rexeis, rexeis@ivt.tugraz.at, IVT, Graz University of Technology
*/

using System;
using System.Collections.Generic;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents;
using TUGraz.VectoCore.InputData.Reader.ShiftStrategy;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Engine;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.HeavyLorry
{
	public partial class DeclarationDataAdapterHeavyLorry : AbstractSimulationDataAdapter, IDeclarationDataAdapter
	{
		public abstract class LorryBase : AbstractSimulationDataAdapter, IDeclarationDataAdapter
		{
			#region Implementation of IDeclarationDataAdapter

			private readonly IDriverDataAdapter _driverDataAdapter = new LorryDriverDataAdapter();
			protected readonly IVehicleDataAdapter _vehicleDataAdapter = new LorryVehicleDataAdapter();
			private readonly IAxleGearDataAdapter _axleGearDataAdapter = new AxleGearDataAdapterBase();
			private readonly IRetarderDataAdapter _retarderDataAdapter = new RetarderDataAdapter();
			private readonly IAirdragDataAdapter _airdragDataAdapter = new AirdragDataAdapter();
			private readonly IPTODataAdapter _ptoDataAdapter = new PTODataAdapterLorry();
			public DriverData CreateDriverData()
			{
				return _driverDataAdapter.CreateDriverData();
			}

			public virtual VehicleData CreateVehicleData(IVehicleDeclarationInputData vehicle, Segment segment,
				Mission mission,
				KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading, bool allowVocational)
			{
				return _vehicleDataAdapter.CreateVehicleData(vehicle, segment, mission, loading.Value.Item1,
					loading.Value.Item2, allowVocational);
			}

			public virtual AirdragData CreateAirdragData(IAirdragDeclarationInputData airdragData, Mission mission,
				Segment segment)
			{
				return _airdragDataAdapter.CreateAirdragData(airdragData, mission, segment);
			}

			public AxleGearData CreateAxleGearData(IAxleGearInputData axlegearData)
			{
				return _axleGearDataAdapter.CreateAxleGearData(axlegearData);
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

			public abstract ShiftStrategyParameters CreateGearshiftData(GearboxData gbx, double axleRatio,
				PerSecond engineIdlingSpeed);

			public RetarderData CreateRetarderData(IRetarderInputData retarderData)
			{
				return _retarderDataAdapter.CreateRetarderData(retarderData);
			}

			public PTOData CreatePTOTransmissionData(IPTOTransmissionInputData ptoData)
			{
				return _ptoDataAdapter.CreatePTOTransmissionData(ptoData);
			}

			public abstract IList<VectoRunData.AuxData> CreateAuxiliaryData(IAuxiliariesDeclarationInputData auxData,
				IBusAuxiliariesDeclarationData busAuxData,
				MissionType missionType, VehicleClass vehicleClass, Meter vehicleLength, int? numSteeredAxles);

			public AxleGearData CreateDummyAxleGearData(IGearboxDeclarationInputData gbxData)
			{
				return _axleGearDataAdapter.CreateDummyAxleGearData(gbxData);
			}

			#endregion
		}

		public class Conventional : LorryBase
		{
			public static readonly GearboxType[] SupportedGearboxTypes =
				{ GearboxType.MT, GearboxType.AMT, GearboxType.ATPowerSplit, GearboxType.ATSerial };

			private IEngineDataAdapter _engineDataAdapter = new CombustionEngineComponentDataAdapter();
			private IGearboxDataAdapter _gearboxDataAdapter = new GearboxDataAdapter(new TorqueConverterDataAdapter());
			private IAngledriveDataAdapter _angleDriveDataAdapter = new AngledriveDataAdapter();
			private IAuxiliaryDataAdapter _auxAdapter = new HeavyLorryAuxiliaryDataAdapter();
			public override CombustionEngineData CreateEngineData(
				IVehicleDeclarationInputData vehicle, IEngineModeDeclarationInputData mode, Mission mission)
			{
				return _engineDataAdapter.CreateEngineData(vehicle, mode, mission);
			}



			public GearboxData CreateGearboxData(IVehicleDeclarationInputData inputData, VectoRunData runData,
				IShiftPolygonCalculator shiftPolygonCalc)
			{
				return _gearboxDataAdapter.CreateGearboxData(inputData, runData, shiftPolygonCalc, SupportedGearboxTypes);
			}

			protected virtual TorqueConverterData CreateTorqueConverterData(GearboxType gearboxType,
				ITorqueConverterDeclarationInputData torqueConverter, double ratio,
				CombustionEngineData componentsEngineInputData)
			{
				return TorqueConverterDataReader.Create(
					torqueConverter.TCData,
					DeclarationData.TorqueConverter.ReferenceRPM, DeclarationData.TorqueConverter.MaxInputSpeed,
					ExecutionMode.Declaration, ratio,
					DeclarationData.TorqueConverter.CLUpshiftMinAcceleration,
					DeclarationData.TorqueConverter.CCUpshiftMinAcceleration);
			}

			#region Overrides of LorryBase

			public override ShiftStrategyParameters CreateGearshiftData(GearboxData gbx, double axleRatio, PerSecond engineIdlingSpeed)
			{
				return _gearboxDataAdapter.CreateGearshiftData(gbx, axleRatio, engineIdlingSpeed);
			}

			#endregion


			public override AngledriveData CreateAngledriveData(IAngledriveInputData data)
			{
				return _angleDriveDataAdapter.CreateAngledriveData(data, false);
			}


			public override IList<VectoRunData.AuxData> CreateAuxiliaryData(
				IAuxiliariesDeclarationInputData auxInputData,
				IBusAuxiliariesDeclarationData busAuxData, MissionType mission, VehicleClass hvdClass,
				Meter vehicleLength, int? numSteeredAxles)
			{
				return _auxAdapter.CreateAuxiliaryData(auxInputData, busAuxData, mission, hvdClass, vehicleLength,
					numSteeredAxles);
			}
		}

		public abstract class SerialHybrid : LorryBase
		{
			#region Overrides of LorryBase

			public override ShiftStrategyParameters CreateGearshiftData(GearboxData gbx, double axleRatio, PerSecond engineIdlingSpeed)
			{
				throw new NotImplementedException();
			}

			public override IList<VectoRunData.AuxData> CreateAuxiliaryData(IAuxiliariesDeclarationInputData auxData, IBusAuxiliariesDeclarationData busAuxData,
				MissionType missionType, VehicleClass vehicleClass, Meter vehicleLength, int? numSteeredAxles)
			{
				throw new NotImplementedException();
			}

			#endregion
		}

		public abstract class ParallelHybrid : LorryBase
		{
			#region Overrides of LorryBase

			public override ShiftStrategyParameters CreateGearshiftData(GearboxData gbx, double axleRatio, PerSecond engineIdlingSpeed)
			{
				throw new NotImplementedException();
			}

			public override IList<VectoRunData.AuxData> CreateAuxiliaryData(IAuxiliariesDeclarationInputData auxData, IBusAuxiliariesDeclarationData busAuxData,
				MissionType missionType, VehicleClass vehicleClass, Meter vehicleLength, int? numSteeredAxles)
			{
				throw new NotImplementedException();
			}

			#endregion
		}

		public abstract class BatteryElectric : LorryBase
		{
			#region Overrides of LorryBase

			public override ShiftStrategyParameters CreateGearshiftData(GearboxData gbx, double axleRatio, PerSecond engineIdlingSpeed)
			{
				throw new NotImplementedException();
			}

			public override IList<VectoRunData.AuxData> CreateAuxiliaryData(IAuxiliariesDeclarationInputData auxData, IBusAuxiliariesDeclarationData busAuxData,
				MissionType missionType, VehicleClass vehicleClass, Meter vehicleLength, int? numSteeredAxles)
			{
				throw new NotImplementedException();
			}

			#endregion
		}
		public class HEV_S2 : SerialHybrid { }
		public class HEV_S3 : SerialHybrid { }
		public class HEV_S4 : SerialHybrid { }
		public class HEV_S_IEPC : SerialHybrid { }
		public class HEV_P1 : ParallelHybrid { }
		public class HEV_P2 : ParallelHybrid { }
		public class HEV_P2_5 : ParallelHybrid { }
		public class HEV_P3 : ParallelHybrid { }
		public class HEV_P4 : ParallelHybrid { }
		public class PEV_E2 : BatteryElectric { }
		public class PEV_E3 : BatteryElectric { }
		public class PEV_E4 : BatteryElectric { }
		public class PEV_E_IEPC : BatteryElectric { }
		public class Exempted : LorryBase
		{
			#region Overrides of LorryBase

			public override VehicleData CreateVehicleData(IVehicleDeclarationInputData vehicle, Segment segment, Mission mission,
				KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading, bool allowVocational)
			{
				return _vehicleDataAdapter.CreateExemptedVehicleData(vehicle);
			}

			public override ShiftStrategyParameters CreateGearshiftData(GearboxData gbx, double axleRatio, PerSecond engineIdlingSpeed)
			{
				throw new NotImplementedException();
			}

			public override IList<VectoRunData.AuxData> CreateAuxiliaryData(IAuxiliariesDeclarationInputData auxData, IBusAuxiliariesDeclarationData busAuxData,
				MissionType missionType, VehicleClass vehicleClass, Meter vehicleLength, int? numSteeredAxles)
			{
				throw new NotImplementedException();
			}

			#endregion
		}

























	}
}