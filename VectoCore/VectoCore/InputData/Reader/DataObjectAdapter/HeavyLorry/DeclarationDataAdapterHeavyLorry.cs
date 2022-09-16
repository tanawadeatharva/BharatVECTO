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
using System.Data.Common;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents.StrategyDataAdapter;
using TUGraz.VectoCore.InputData.Reader.ShiftStrategy;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Battery;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Engine;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.HeavyLorry
{
	public partial class DeclarationDataAdapterHeavyLorry
	{
		public abstract class LorryBase : AbstractSimulationDataAdapter, ILorryDeclarationDataAdapter
		{
			#region Implementation of IDeclarationDataAdapter

			private readonly IDriverDataAdapter _driverDataAdapter = new LorryDriverDataAdapter();
			protected readonly IVehicleDataAdapter _vehicleDataAdapter = new LorryVehicleDataAdapter();
			private readonly IAxleGearDataAdapter _axleGearDataAdapter = new AxleGearDataAdapter();
			private readonly IRetarderDataAdapter _retarderDataAdapter = new RetarderDataAdapter();
			private readonly IAirdragDataAdapter _airdragDataAdapter = new AirdragDataAdapter();
			private readonly IPTODataAdapter _ptoDataAdapter = new PTODataAdapterLorry();
			private IAngledriveDataAdapter _angleDriveDataAdapter = new AngledriveDataAdapter();
			public DriverData CreateDriverData()
			{
				return _driverDataAdapter.CreateDriverData();
			}

			protected abstract GearboxType[] SupportedGearboxTypes { get; }

			public virtual VehicleData CreateVehicleData(IVehicleDeclarationInputData vehicle, Segment segment,
				Mission mission,
				KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading, bool allowVocational)
			{
				return _vehicleDataAdapter.CreateVehicleData(vehicle, segment, mission, loading.Value.Item1,
					loading.Value.Item2, allowVocational);
			}

			public virtual IList<Tuple<PowertrainPosition, ElectricMotorData>> CreateElectricMachines(
				IElectricMachinesDeclarationInputData electricMachines,
				IDictionary<PowertrainPosition, IList<Tuple<Volt, TableData>>> torqueLimits, Volt averageVoltage,
				GearList gears = null)
			{
				throw new NotImplementedException();
			}

			public virtual BatterySystemData CreateBatteryData(IElectricStorageSystemDeclarationInputData componentsElectricStorage)
			{
				throw new NotImplementedException();
			}

			public virtual SuperCapData CreateSuperCapData(IElectricStorageSystemDeclarationInputData componentsElectricStorage)
			{
				throw new NotImplementedException();
			}

			public virtual HybridStrategyParameters CreateHybridStrategy(BatterySystemData runDataBatteryData, SuperCapData runDataSuperCapData)
			{
				throw new NotImplementedException();
			}

			public virtual ShiftStrategyParameters CreateDummyGearshiftStrategy()
			{
				throw new NotImplementedException();
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


			public AngledriveData CreateAngledriveData(IAngledriveInputData data)
			{
				return _angleDriveDataAdapter.CreateAngledriveData(data, false);
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

			public virtual ShiftStrategyParameters CreateGearshiftData(double axleRatio,
				PerSecond engineIdlingSpeed, GearboxType gearboxType, int gearsCount)
			{
				throw new NotImplementedException();
			}

			public RetarderData CreateRetarderData(IRetarderInputData retarderData, PowertrainPosition position)
			{
				return _retarderDataAdapter.CreateRetarderData(retarderData, position);
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
			public static GearboxType[] SupportsGearboxTypes = { GearboxType.MT, GearboxType.AMT, GearboxType.ATPowerSplit, GearboxType
				.ATSerial};

			protected override GearboxType[] SupportedGearboxTypes => new []
				{ GearboxType.MT, GearboxType.AMT, GearboxType.ATPowerSplit, GearboxType.ATSerial };

			private IEngineDataAdapter _engineDataAdapter = new CombustionEngineComponentDataAdapter();
			private IGearboxDataAdapter _gearboxDataAdapter = new GearboxDataAdapter(new TorqueConverterDataAdapter());
			private IAuxiliaryDataAdapter _auxAdapter = new HeavyLorryAuxiliaryDataAdapter();
			public override CombustionEngineData CreateEngineData(
				IVehicleDeclarationInputData vehicle, IEngineModeDeclarationInputData mode, Mission mission)
			{
				return _engineDataAdapter.CreateEngineData(vehicle, mode, mission);
			}

			public override GearboxData CreateGearboxData(IVehicleDeclarationInputData inputData, VectoRunData runData,
				IShiftPolygonCalculator shiftPolygonCalc)
			{
				return _gearboxDataAdapter.CreateGearboxData(inputData, runData, shiftPolygonCalc, SupportedGearboxTypes);
			}


			#region Overrides of LorryBase

			public override ShiftStrategyParameters CreateGearshiftData(double axleRatio, PerSecond engineIdlingSpeed, GearboxType gearboxType, int gearsCount)
			{
				return _gearboxDataAdapter.CreateGearshiftData(axleRatio, engineIdlingSpeed, gearboxType, gearsCount);
			}

			#endregion




			public override IList<VectoRunData.AuxData> CreateAuxiliaryData(
				IAuxiliariesDeclarationInputData auxInputData,
				IBusAuxiliariesDeclarationData busAuxData, MissionType mission, VehicleClass hvdClass,
				Meter vehicleLength, int? numSteeredAxles)
			{
				return _auxAdapter.CreateAuxiliaryData(auxInputData, busAuxData, mission, hvdClass, vehicleLength,
					numSteeredAxles);
			}
		}

		public abstract class Hybrid : LorryBase
		{
			private IEngineDataAdapter _engineDataAdapter = new CombustionEngineComponentDataAdapter();
			private ElectricMachinesDataAdapter _electricMachinesDataAdapter = new ElectricMachinesDataAdapter();
			private ElectricStorageAdapter _eletricStorageAdapter = new ElectricStorageAdapter();
			public override CombustionEngineData CreateEngineData(
				IVehicleDeclarationInputData vehicle, IEngineModeDeclarationInputData mode, Mission mission)
			{
				return _engineDataAdapter.CreateEngineData(vehicle, mode, mission);
			}

			#region Overrides of LorryBase

			public override IList<Tuple<PowertrainPosition, ElectricMotorData>> CreateElectricMachines(
				IElectricMachinesDeclarationInputData electricMachines,
				IDictionary<PowertrainPosition, IList<Tuple<Volt, TableData>>> torqueLimits, Volt averageVoltage,
				GearList gears = null)
			{
				return _electricMachinesDataAdapter.CreateElectricMachines(electricMachines, torqueLimits, averageVoltage,
					gears);
			}

			public override BatterySystemData CreateBatteryData(IElectricStorageSystemDeclarationInputData batteryInputData)
			{
				return _eletricStorageAdapter.CreateBatteryData(batteryInputData);
			}

			public override SuperCapData CreateSuperCapData(IElectricStorageSystemDeclarationInputData reessInputData)
			{
				return _eletricStorageAdapter.CreateSuperCapData(reessInputData);
			}

			//public RetarderData CreateRetarderData 

			#endregion
		}

		public abstract class SerialHybrid : Hybrid
		{
			#region Overrides of LorryBase

			protected override GearboxType[] SupportedGearboxTypes { get; }

			public override IList<VectoRunData.AuxData> CreateAuxiliaryData(IAuxiliariesDeclarationInputData auxData, IBusAuxiliariesDeclarationData busAuxData,
				MissionType missionType, VehicleClass vehicleClass, Meter vehicleLength, int? numSteeredAxles)
			{
				throw new NotImplementedException();
			}

			#endregion
		}

		public abstract class ParallelHybrid : Hybrid
		{
			private ParallelHybridStrategyParameterDataAdapter _hybridStrategyDataAdapter =
				new ParallelHybridStrategyParameterDataAdapter();

			protected override GearboxType[] SupportedGearboxTypes => new[]
				{ GearboxType.AMT, GearboxType.ATPowerSplit, GearboxType.ATSerial };
			private GearboxDataAdapter _gearboxDataAdapter = new GearboxDataAdapter(null);
			#region Overrides of LorryBase

			public override ShiftStrategyParameters CreateGearshiftData(double axleRatio, PerSecond engineIdlingSpeed, GearboxType type, int gearsCount)
			{

				return _gearboxDataAdapter.CreateGearshiftData(axleRatio, engineIdlingSpeed, type, gearsCount);
			}

			public override IList<VectoRunData.AuxData> CreateAuxiliaryData(IAuxiliariesDeclarationInputData auxData, IBusAuxiliariesDeclarationData busAuxData,
				MissionType missionType, VehicleClass vehicleClass, Meter vehicleLength, int? numSteeredAxles)
			{
				throw new NotImplementedException();
			}

			public override HybridStrategyParameters CreateHybridStrategy(BatterySystemData runDataBatteryData, SuperCapData runDataSuperCapData)
			{
				return _hybridStrategyDataAdapter.CreateHybridStrategyParameters(runDataBatteryData, runDataSuperCapData);
			}

			#endregion
		}

		public abstract class BatteryElectric : LorryBase
		{
			#region Overrides of LorryBase

			protected override GearboxType[] SupportedGearboxTypes { get; }

			private readonly GearboxDataAdapter _gearboxDataAdapter = new GearboxDataAdapter(null);
			private readonly ElectricStorageAdapter _electricStorageAdapter = new ElectricStorageAdapter();
			private readonly ElectricMachinesDataAdapter _electricMachineAdapter = new ElectricMachinesDataAdapter();
			public override GearboxData CreateGearboxData(IVehicleDeclarationInputData inputData, VectoRunData runData,
				IShiftPolygonCalculator shiftPolygonCalc)
			{
				return _gearboxDataAdapter.CreateGearboxData(inputData, runData, shiftPolygonCalc, new[] { GearboxType.AMT });
			}

			public override ShiftStrategyParameters CreateGearshiftData(double axleRatio, PerSecond engineIdlingSpeed, GearboxType gearboxType, int gearsCount)
			{
				System.Diagnostics.Debug.Assert(engineIdlingSpeed == null);
				return _gearboxDataAdapter.CreateGearshiftData(axleRatio, null, gearboxType, gearsCount);
			}

			public override IList<VectoRunData.AuxData> CreateAuxiliaryData(IAuxiliariesDeclarationInputData auxData, IBusAuxiliariesDeclarationData busAuxData,
				MissionType missionType, VehicleClass vehicleClass, Meter vehicleLength, int? numSteeredAxles)
			{
				throw new NotImplementedException();
			}

			public override IList<Tuple<PowertrainPosition, ElectricMotorData>> CreateElectricMachines(IElectricMachinesDeclarationInputData electricMachines, IDictionary<PowertrainPosition, IList<Tuple<Volt, TableData>>> torqueLimits,
				Volt averageVoltage, GearList gears = null)
			{
				return _electricMachineAdapter.CreateElectricMachines(electricMachines, torqueLimits, averageVoltage, gears);
			}

			public override BatterySystemData CreateBatteryData(IElectricStorageSystemDeclarationInputData componentsElectricStorage)
			{
				return _electricStorageAdapter.CreateBatteryData(batteryInputData: componentsElectricStorage);
			}

			public override SuperCapData CreateSuperCapData(IElectricStorageSystemDeclarationInputData componentsElectricStorage)
			{
				return _electricStorageAdapter.CreateSuperCapData(componentsElectricStorage);
			}

			#endregion
		}

		public class HEV_S2 : SerialHybrid
		{
			#region Overrides of LorryBase
			protected override GearboxType[] SupportedGearboxTypes => new[]
				{ GearboxType.AMT, GearboxType.ATPowerSplit, GearboxType.ATSerial, GearboxType.APTN };
			private GearboxDataAdapter _gearBoxDataAdaper = new GearboxDataAdapter(new TorqueConverterDataAdapter());
			public override GearboxData CreateGearboxData(IVehicleDeclarationInputData inputData, VectoRunData runData,
				IShiftPolygonCalculator shiftPolygonCalc)
			{
				return _gearBoxDataAdaper.CreateGearboxData(inputData, runData, shiftPolygonCalc, SupportedGearboxTypes);
			}

			public override ShiftStrategyParameters CreateGearshiftData(double axleRatio, PerSecond engineIdlingSpeed, GearboxType gearboxType, int gearsCount)
			{
				return _gearBoxDataAdaper.CreateGearshiftData(axleRatio, engineIdlingSpeed, gearboxType, gearsCount);
			}

			#endregion
		}
		public class HEV_S3 : SerialHybrid { }
		public class HEV_S4 : SerialHybrid { }
		public class HEV_S_IEPC : SerialHybrid { }

		public class HEV_P1 : ParallelHybrid
		{
			
		}

		public class HEV_P2 : ParallelHybrid
		{
			protected override GearboxType[] SupportedGearboxTypes => new[]
				{ GearboxType.AMT, GearboxType.IHPC, };
		}

		public class HEV_P2_5 : ParallelHybrid
		{

		}
		public class HEV_P3 : ParallelHybrid { }
		public class HEV_P4 : ParallelHybrid { }

		public class PEV_E2 : BatteryElectric
		{
			#region Overrides of BatteryElectric

			protected override GearboxType[] SupportedGearboxTypes => new[]
				{ GearboxType.AMT, GearboxType.ATPowerSplit, GearboxType.ATSerial, GearboxType.APTN };

			#endregion
		}
		public class PEV_E3 : BatteryElectric { }
		public class PEV_E4 : BatteryElectric { }

		public class PEV_E_IEPC : BatteryElectric
		{

		}
		public class Exempted : LorryBase
		{
			#region Overrides of LorryBase

			protected override GearboxType[] SupportedGearboxTypes { get; }

			public override VehicleData CreateVehicleData(IVehicleDeclarationInputData vehicle, Segment segment, Mission mission,
				KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading, bool allowVocational)
			{
				return _vehicleDataAdapter.CreateExemptedVehicleData(vehicle);
			}

			public override ShiftStrategyParameters CreateGearshiftData(double axleRatio, PerSecond engineIdlingSpeed, GearboxType gearboxType, int gearsCount)
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