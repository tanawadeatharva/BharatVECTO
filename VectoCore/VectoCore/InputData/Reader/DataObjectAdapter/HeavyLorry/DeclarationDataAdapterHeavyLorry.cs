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
using TUGraz.VectoCore.Models.SimulationComponent.Data.ElectricComponents.Battery;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Engine;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.HeavyLorry
{
	public class DeclarationDataAdapterHeavyLorry
	{
		public abstract class LorryBase : AbstractSimulationDataAdapter, ILorryDeclarationDataAdapter
		{
			#region Implementation of IDeclarationDataAdapter

			private readonly IDriverDataAdapter _driverDataAdapter = new LorryDriverDataAdapter();
			protected readonly IVehicleDataAdapter _vehicleDataAdapter = new LorryVehicleDataAdapter();
			private readonly IAxleGearDataAdapter _axleGearDataAdapter = new AxleGearDataAdapter();
			private readonly IRetarderDataAdapter _retarderDataAdapter = new RetarderDataAdapter();
			private readonly IAirdragDataAdapter _airdragDataAdapter = new AirdragDataAdapter();

			private IAngledriveDataAdapter _angleDriveDataAdapter = new AngledriveDataAdapter();
			public DriverData CreateDriverData(Segment segment)
			{
				return _driverDataAdapter.CreateDriverData(segment);
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

			public abstract void CreateREESSData(IElectricStorageSystemDeclarationInputData componentsElectricStorage,
				VectoSimulationJobType jobType, bool ovc, Action<BatterySystemData> setBatteryData,
				Action<SuperCapData> setSuperCapData);


			public virtual BatterySystemData CreateBatteryData(IElectricStorageSystemDeclarationInputData componentsElectricStorage, VectoSimulationJobType jobType, bool ovc)
			{
				throw new NotImplementedException();
			}

			public virtual SuperCapData CreateSuperCapData(IElectricStorageSystemDeclarationInputData componentsElectricStorage)
			{
				throw new NotImplementedException();
			}

			public virtual HybridStrategyParameters CreateHybridStrategy(BatterySystemData runDataBatteryData,
				SuperCapData runDataSuperCapData, Kilogram vehicleMass, VectoRunData.OvcHevMode ovcMode, LoadingType loading, VehicleClass vehicleClass, MissionType missionType)
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

			public abstract PTOData CreatePTOCycleData(IGearboxDeclarationInputData gbx, IPTOTransmissionInputData pto);
			public abstract PTOData CreatePTOTransmissionData(IPTOTransmissionInputData ptoData,
				IGearboxDeclarationInputData gbx);


			public abstract IList<VectoRunData.AuxData> CreateAuxiliaryData(IAuxiliariesDeclarationInputData auxData,
				IBusAuxiliariesDeclarationData busAuxData,
				MissionType missionType, VehicleClass vehicleClass, Meter vehicleLength, int? numSteeredAxles,
				VectoSimulationJobType jobType);

			public AxleGearData CreateDummyAxleGearData(IGearboxDeclarationInputData gbxData)
			{
				return _axleGearDataAdapter.CreateDummyAxleGearData(gbxData);
			}

			#endregion

			public virtual List<Tuple<PowertrainPosition, ElectricMotorData>> CreateIEPCElectricMachines(
				IIEPCDeclarationInputData iepc, Volt averageVoltage)
			{
				throw new NotImplementedException();
			}
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
			private readonly IPTODataAdapter _ptoDataAdapter = new PTODataAdapterLorry();
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

			public override PTOData CreatePTOTransmissionData(IPTOTransmissionInputData ptoData,
				IGearboxDeclarationInputData gbx)
			{
				return _ptoDataAdapter.CreatePTOTransmissionData(ptoData, gbx);
			}

			public override PTOData CreatePTOCycleData(IGearboxDeclarationInputData gbx, IPTOTransmissionInputData pto)
			{
				return _ptoDataAdapter.CreateDefaultPTOData(pto, gbx);
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
				Meter vehicleLength, int? numSteeredAxles, VectoSimulationJobType jobType)
			{
				return _auxAdapter.CreateAuxiliaryData(auxInputData, busAuxData, mission, hvdClass, vehicleLength,
					numSteeredAxles, jobType);
			}

			public override void CreateREESSData(IElectricStorageSystemDeclarationInputData componentsElectricStorage,
				VectoSimulationJobType jobType, bool ovc, Action<BatterySystemData> setBatteryData,
				Action<SuperCapData> setSuperCapData)
			{
				throw new NotImplementedException();
			}
		}

		public abstract class Hybrid : LorryBase
		{
			private IEngineDataAdapter _engineDataAdapter = new CombustionEngineComponentDataAdapter();
			private ElectricMachinesDataAdapter _electricMachinesDataAdapter = new ElectricMachinesDataAdapter();
			private ElectricStorageAdapter _eletricStorageAdapter = new ElectricStorageAdapter();
			protected IAuxiliaryDataAdapter _auxAdapter = new HeavyLorryAuxiliaryDataAdapter();
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

			public override BatterySystemData CreateBatteryData(IElectricStorageSystemDeclarationInputData batteryInputData, VectoSimulationJobType jobType, bool ovc)
			{
				return _eletricStorageAdapter.CreateBatteryData(batteryInputData, jobType, ovc);
			}

			public override SuperCapData CreateSuperCapData(IElectricStorageSystemDeclarationInputData reessInputData)
			{
				return _eletricStorageAdapter.CreateSuperCapData(reessInputData);
			}

			public override IList<VectoRunData.AuxData> CreateAuxiliaryData(IAuxiliariesDeclarationInputData auxData,
				IBusAuxiliariesDeclarationData busAuxData,
				MissionType missionType, VehicleClass vehicleClass, Meter vehicleLength, int? numSteeredAxles,
				VectoSimulationJobType jobType)
			{
				return _auxAdapter.CreateAuxiliaryData(auxData, busAuxData, missionType, vehicleClass, vehicleLength,
					numSteeredAxles, jobType);
			}

			public override void CreateREESSData(IElectricStorageSystemDeclarationInputData componentsElectricStorage,
				VectoSimulationJobType jobType, bool ovc, Action<BatterySystemData> setBatteryData, Action<SuperCapData> setSuperCapData)
			{
				var batteryData = _eletricStorageAdapter.CreateBatteryData(componentsElectricStorage, jobType, ovc);
				var superCapData = _eletricStorageAdapter.CreateSuperCapData(componentsElectricStorage);

				if (batteryData != null) {
					setBatteryData(batteryData);
				}
				if (superCapData != null) {
					setSuperCapData(superCapData);
				}

				if (batteryData != null && superCapData != null) {
					throw new VectoException("Either battery or super cap must be provided");
				}
			}

			#endregion
		}

		public abstract class SerialHybrid : Hybrid
		{
			#region Overrides of LorryBase

			protected override GearboxType[] SupportedGearboxTypes { get; }

			private SerialHybridStrategyParameterDataAdapter _hybridStrategyParameterData =
				new SerialHybridStrategyParameterDataAdapter();
			private readonly ElectricPTODataAdapter _ptoDataAdapter = new ElectricPTODataAdapter();

			public override HybridStrategyParameters CreateHybridStrategy(BatterySystemData runDataBatteryData,
				SuperCapData runDataSuperCapData, Kilogram vehicleMass, VectoRunData.OvcHevMode ovcMode, LoadingType loading, VehicleClass vehicleClass, MissionType missionType)
			{
				return _hybridStrategyParameterData.CreateHybridStrategyParameters(runDataBatteryData,
					runDataSuperCapData, vehicleMass, ovcMode);
			}

			#endregion

			#region Overrides of LorryBase

			public override PTOData CreatePTOCycleData(IGearboxDeclarationInputData gbx, IPTOTransmissionInputData pto)
			{
				return _ptoDataAdapter.CreateDefaultPTOData(pto, gbx);
			}

			public override PTOData CreatePTOTransmissionData(IPTOTransmissionInputData ptoData,
				IGearboxDeclarationInputData gbx)
			{
				return _ptoDataAdapter.CreatePTOTransmissionData(ptoData, gbx);
			}
			public override void CreateREESSData(IElectricStorageSystemDeclarationInputData componentsElectricStorage,
				VectoSimulationJobType jobType, bool ovc, Action<BatterySystemData> setBatteryData, Action<SuperCapData> setSuperCapData)
			{
				base.CreateREESSData(componentsElectricStorage, jobType, ovc, setBatteryData, setSuperCapData);
			}




			#endregion
		}

		public abstract class ParallelHybrid : Hybrid
		{
			private ParallelHybridStrategyParameterDataAdapter _hybridStrategyDataAdapter =
				new ParallelHybridStrategyParameterDataAdapter();

			private PTODataAdapterLorry _ptoAdapterLorry = new PTODataAdapterLorry();
			protected override GearboxType[] SupportedGearboxTypes => new[]
				{ GearboxType.AMT, GearboxType.ATPowerSplit, GearboxType.ATSerial };
			private IGearboxDataAdapter _gearboxDataAdapter = new GearboxDataAdapter(new TorqueConverterDataAdapter());
			//private GearboxDataAdapter _gearboxDataAdapter = new GearboxDataAdapter(null);

			//private ElectricStorageAdapter _electricStorageAdapter = new ElectricStorageAdapter();
			#region Overrides of LorryBase

			public override ShiftStrategyParameters CreateGearshiftData(double axleRatio, PerSecond engineIdlingSpeed, GearboxType type, int gearsCount)
			{

				return _gearboxDataAdapter.CreateGearshiftData(axleRatio, engineIdlingSpeed, type, gearsCount);
			}

			#region Overrides of AbstractSimulationDataAdapter

			protected override TransmissionLossMap CreateGearLossMap(ITransmissionInputData gear, uint i, bool useEfficiencyFallback,
				VehicleCategory vehicleCategory, GearboxType gearboxType)
			{
				return base.CreateGearLossMap(gear, i, useEfficiencyFallback, vehicleCategory, gearboxType);
			}

			#endregion

			public override GearboxData CreateGearboxData(IVehicleDeclarationInputData inputData, VectoRunData runData,
				IShiftPolygonCalculator shiftPolygonCalc)
			{
				return _gearboxDataAdapter.CreateGearboxData(inputData, runData, shiftPolygonCalc, SupportedGearboxTypes);
			}

			public override IList<VectoRunData.AuxData> CreateAuxiliaryData(IAuxiliariesDeclarationInputData auxData,
				IBusAuxiliariesDeclarationData busAuxData,
				MissionType missionType, VehicleClass vehicleClass, Meter vehicleLength, int? numSteeredAxles,
				VectoSimulationJobType jobType)
			{
				return base.CreateAuxiliaryData(auxData, busAuxData, missionType, vehicleClass, vehicleLength,
					numSteeredAxles, jobType);
			}

			public override void CreateREESSData(IElectricStorageSystemDeclarationInputData componentsElectricStorage,
				VectoSimulationJobType jobType, bool ovc, Action<BatterySystemData> setBatteryData, Action<SuperCapData> setSuperCapData)
			{
				base.CreateREESSData(componentsElectricStorage, jobType, ovc, setBatteryData, setSuperCapData);
				
			}

			public override HybridStrategyParameters CreateHybridStrategy(BatterySystemData runDataBatteryData,
				SuperCapData runDataSuperCapData, Kilogram vehicleMass, VectoRunData.OvcHevMode ovcMode, LoadingType loading, VehicleClass vehicleClass, MissionType missionType)
			{
				return _hybridStrategyDataAdapter.CreateHybridStrategyParameters(runDataBatteryData, runDataSuperCapData, ovcMode, loading, vehicleClass, missionType);
			}

			#endregion

			#region Overrides of LorryBase

			public override PTOData CreatePTOCycleData(IGearboxDeclarationInputData gbx, IPTOTransmissionInputData pto)
			{
				return _ptoAdapterLorry.CreateDefaultPTOData(pto, gbx);
			}

			public override PTOData CreatePTOTransmissionData(IPTOTransmissionInputData ptoData,
				IGearboxDeclarationInputData gbx)
			{
				return _ptoAdapterLorry.CreatePTOTransmissionData(ptoData, gbx);
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
			private readonly ElectricPTODataAdapter _ptoDataAdapter = new ElectricPTODataAdapter();

			private readonly HeavyLorryPEVAuxiliaryDataAdapter
				_auxDataAdapter = new HeavyLorryPEVAuxiliaryDataAdapter();

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

			public override IList<VectoRunData.AuxData> CreateAuxiliaryData(IAuxiliariesDeclarationInputData auxData,
				IBusAuxiliariesDeclarationData busAuxData,
				MissionType missionType, VehicleClass vehicleClass, Meter vehicleLength, int? numSteeredAxles,
				VectoSimulationJobType jobType)
			{
				return _auxDataAdapter.CreateAuxiliaryData(auxData, null, missionType, vehicleClass, vehicleLength, numSteeredAxles,
					jobType);
			}

			public override IList<Tuple<PowertrainPosition, ElectricMotorData>> CreateElectricMachines(IElectricMachinesDeclarationInputData electricMachines, IDictionary<PowertrainPosition, IList<Tuple<Volt, TableData>>> torqueLimits,
				Volt averageVoltage, GearList gears = null)
			{
				return _electricMachineAdapter.CreateElectricMachines(electricMachines, torqueLimits, averageVoltage, gears);
			}

			public override void CreateREESSData(IElectricStorageSystemDeclarationInputData componentsElectricStorage,
				VectoSimulationJobType jobType, bool ovc, Action<BatterySystemData> setBatteryData, Action<SuperCapData> setSuperCapData)
			{
				var batteryData = _electricStorageAdapter.CreateBatteryData(componentsElectricStorage, jobType, ovc);
				var superCapData = _electricStorageAdapter.CreateSuperCapData(componentsElectricStorage);

				
				if (batteryData == null) {
					throw new VectoException("Could not create BatterySystem for PEV");
				}
				setBatteryData(batteryData);
				
				
				if (superCapData != null) {
					throw new VectoException("Supercaps are not allowed for PEVs");
				}
				

			}

			public override BatterySystemData CreateBatteryData(IElectricStorageSystemDeclarationInputData componentsElectricStorage, VectoSimulationJobType jobType, bool ovc)
			{
				return _electricStorageAdapter.CreateBatteryData(batteryInputData: componentsElectricStorage, jobType: jobType, ovc: ovc);
			}

			public override SuperCapData CreateSuperCapData(IElectricStorageSystemDeclarationInputData componentsElectricStorage)
			{
				return _electricStorageAdapter.CreateSuperCapData(componentsElectricStorage);
			}

			public override PTOData CreatePTOCycleData(IGearboxDeclarationInputData gbx, IPTOTransmissionInputData pto)
			{
				return _ptoDataAdapter.CreateDefaultPTOData(pto, gbx);
			}

			public override PTOData CreatePTOTransmissionData(IPTOTransmissionInputData ptoData,
				IGearboxDeclarationInputData gbx)
			{
				var result = _ptoDataAdapter.CreatePTOTransmissionData(ptoData, gbx);
				return result;
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

			public override ShiftStrategyParameters CreateGearshiftData(double axleRatio, PerSecond engineIdlingSpeed,
				GearboxType gearboxType, int gearsCount)
			{
				return _gearBoxDataAdaper.CreateGearshiftData(axleRatio, engineIdlingSpeed, gearboxType, gearsCount);
			}

			#endregion
		}
		public class HEV_S3 : SerialHybrid { }
		public class HEV_S4 : SerialHybrid { }

		public class HEV_S_IEPC : SerialHybrid
		{
			
			protected override GearboxType[] SupportedGearboxTypes => new[]
				{ GearboxType.AMT, GearboxType.ATPowerSplit, GearboxType.ATSerial, GearboxType.APTN };



			private IGearboxDataAdapter _gearBoxDataAdaper = new IEPCGearboxDataAdapter();
			private ElectricMachinesDataAdapter _electricMachinesDataAdapter = new ElectricMachinesDataAdapter();

			public override GearboxData CreateGearboxData(IVehicleDeclarationInputData inputData, VectoRunData runData,
				IShiftPolygonCalculator shiftPolygonCalc)
			{
				return _gearBoxDataAdaper.CreateGearboxData(inputData, runData, shiftPolygonCalc, SupportedGearboxTypes);
			}

			public override ShiftStrategyParameters CreateGearshiftData(double axleRatio, PerSecond engineIdlingSpeed,
				GearboxType gearboxType, int gearsCount)
			{
				return _gearBoxDataAdaper.CreateGearshiftData(axleRatio, engineIdlingSpeed, gearboxType, gearsCount);
			}

			public override List<Tuple<PowertrainPosition, ElectricMotorData>> CreateIEPCElectricMachines(
				IIEPCDeclarationInputData iepc, Volt averageVoltage)
			{
				return _electricMachinesDataAdapter.CreateIEPCElectricMachines(iepc, averageVoltage);
			}
		}

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
			#region Overrides of LorryBase

			private ElectricMachinesDataAdapter _emDataAdapter = new ElectricMachinesDataAdapter();
			private IEPCGearboxDataAdapter _gearboxDataAdapter = new IEPCGearboxDataAdapter();
			public override List<Tuple<PowertrainPosition, ElectricMotorData>> CreateIEPCElectricMachines(IIEPCDeclarationInputData iepc, Volt averageVoltage)
			{
				return _emDataAdapter.CreateIEPCElectricMachines(iepc, averageVoltage);
			}

			#region Overrides of BatteryElectric

			public override GearboxData CreateGearboxData(IVehicleDeclarationInputData inputData, VectoRunData runData,
				IShiftPolygonCalculator shiftPolygonCalc)
			{
				return _gearboxDataAdapter.CreateGearboxData(inputData, runData, shiftPolygonCalc, new GearboxType[]
					{
						GearboxType.APTN
				});
			}

			#endregion

			#endregion
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

			public override void CreateREESSData(IElectricStorageSystemDeclarationInputData componentsElectricStorage,
				VectoSimulationJobType jobType, bool ovc, Action<BatterySystemData> setBatteryData, Action<SuperCapData> setSuperCapData)
			{
				throw new NotImplementedException();
			}

			public override PTOData CreatePTOCycleData(IGearboxDeclarationInputData gbx, IPTOTransmissionInputData pto)
			{
				throw new NotImplementedException();
			}

			public override PTOData CreatePTOTransmissionData(IPTOTransmissionInputData ptoData,
				IGearboxDeclarationInputData gbx)
			{
				throw new NotImplementedException();
			}

			public override ShiftStrategyParameters CreateGearshiftData(double axleRatio, PerSecond engineIdlingSpeed, GearboxType gearboxType, int gearsCount)
			{
				throw new NotImplementedException();
			}

			public override IList<VectoRunData.AuxData> CreateAuxiliaryData(IAuxiliariesDeclarationInputData auxData,
				IBusAuxiliariesDeclarationData busAuxData,
				MissionType missionType, VehicleClass vehicleClass, Meter vehicleLength, int? numSteeredAxles,
				VectoSimulationJobType jobType)
			{
				throw new NotImplementedException();
			}

			#endregion
		}

























	}
}