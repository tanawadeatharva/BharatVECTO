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
using System.Numerics;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.Electrics;
using TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules.Electrics;
using TUGraz.VectoCore.Models.Connector.Ports;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.SimulationComponent;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Impl.Auxiliaries;
using TUGraz.VectoCore.Models.SimulationComponent.Impl.Gearbox;
using TUGraz.VectoCore.Models.SimulationComponent.Impl.Shiftstrategies;
using TUGraz.VectoCore.Models.SimulationComponent.Strategies;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.Utils;
using ElectricSystem = TUGraz.VectoCore.Models.SimulationComponent.Impl.ElectricSystem;
using Wheels = TUGraz.VectoCore.Models.SimulationComponent.Impl.Wheels;

namespace TUGraz.VectoCore.Models.Simulation.Impl
{
    /// <summary>
    /// Provides Methods to build a simulator with a powertrain step by step.
    /// </summary>
    public class PowertrainBuilder : PowertrainBuilderBase, IPowertrainBuilder
	{
		private readonly Dictionary<CycleType, Dictionary<VectoSimulationJobType, Func<VectoRunData, IModalDataContainer, ISumData, IVehicleContainer>>> _builders;
		
		private readonly Dictionary<PowertrainPosition, Func<VectoRunData, IVehicleContainer, ElectricSystem, IPowerTrainComponent, IElectricMotor>> _MeasuredSpeedBEVBuilders;
		private readonly Dictionary<PowertrainPosition, Func<VectoRunData, IVehicleContainer, ElectricSystem, PWheelCycle, IElectricMotor>> _PWheelBEVBuilders;


		private IVehicleContainerFactory _vehicleContainerFactory;

		public PowertrainBuilder(IVehicleContainerFactory vehicleContainerFactory, IShiftStrategyFactory shiftStrategyFactory) : base(shiftStrategyFactory)
		{
			_vehicleContainerFactory = vehicleContainerFactory;

			var distanceBuilders = new Dictionary<VectoSimulationJobType, Func<VectoRunData, IModalDataContainer, ISumData, IVehicleContainer>>()
			{
				{ VectoSimulationJobType.ConventionalVehicle, BuildFullPowertrainConventional },
				{ VectoSimulationJobType.ParallelHybridVehicle, BuildFullPowertrainParallelHybrid },
				{ VectoSimulationJobType.IHPC, BuildFullPowertrainParallelHybrid },
				{ VectoSimulationJobType.SerialHybridVehicle, BuildFullPowertrainSerialHybrid },
				{ VectoSimulationJobType.BatteryElectricVehicle, BuildFullPowertrainBatteryElectric },
				{ VectoSimulationJobType.EngineOnlySimulation, BuildEngineOnly },
				{ VectoSimulationJobType.IEPC_E, BuildFullPowertrainIEPCE },
				{ VectoSimulationJobType.IEPC_S, BuildFullPowertrainIEPCSerial }
			};

			var pWheelBuilders = new Dictionary<VectoSimulationJobType, Func<VectoRunData, IModalDataContainer, ISumData, IVehicleContainer>>()
			{
				{ VectoSimulationJobType.ConventionalVehicle, BuildPWheelConventional },
				{ VectoSimulationJobType.BatteryElectricVehicle, BuildPWheelBatteryElectric },
				{ VectoSimulationJobType.IEPC_E, BuildPWheelBatteryElectric }
			};
			
			var measuredSpeedGearBuilders = new Dictionary<VectoSimulationJobType, Func<VectoRunData, IModalDataContainer, ISumData, IVehicleContainer>>()
			{
				{ VectoSimulationJobType.ConventionalVehicle, BuildMeasuredSpeedGearConventional },
				{ VectoSimulationJobType.BatteryElectricVehicle, BuildMeasuredSpeedGearBatteryElectric },
				{ VectoSimulationJobType.IEPC_E, BuildMeasuredSpeedGearIEPC },
				{ VectoSimulationJobType.ParallelHybridVehicle, BuildMeasuredSpeedGearHybrid }
			};
			
			var measuredSpeedBuilders = new Dictionary<VectoSimulationJobType, Func<VectoRunData, IModalDataContainer, ISumData, IVehicleContainer>>()
			{
				{ VectoSimulationJobType.ConventionalVehicle, BuildMeasuredSpeedConventional },
				{ VectoSimulationJobType.BatteryElectricVehicle, BuildMeasuredSpeedBatteryElectric },
				{ VectoSimulationJobType.IEPC_E, BuildMeasuredSpeedBatteryElectric },
				{ VectoSimulationJobType.ParallelHybridVehicle, BuildMeasuredSpeedHybrid }
			};
			
			var vtpBuilders = new Dictionary<VectoSimulationJobType, Func<VectoRunData, IModalDataContainer, ISumData, IVehicleContainer>>()
			{
				{ VectoSimulationJobType.ConventionalVehicle, BuildVTPConventional } 
			};
			
			var engineOnlyBuilders =  new Dictionary<VectoSimulationJobType, Func<VectoRunData, IModalDataContainer, ISumData, IVehicleContainer>>()
			{
				{ VectoSimulationJobType.ConventionalVehicle, BuildEngineOnly },
				{ VectoSimulationJobType.EngineOnlySimulation, BuildEngineOnly }
			};
			
			_builders = new Dictionary<CycleType, Dictionary<VectoSimulationJobType, Func<VectoRunData, IModalDataContainer, ISumData, IVehicleContainer>>>()
			{
				{ CycleType.DistanceBased, distanceBuilders	},
				{ CycleType.PWheel, pWheelBuilders },
				{ CycleType.MeasuredSpeed, measuredSpeedBuilders },
				{ CycleType.MeasuredSpeedGear, measuredSpeedGearBuilders }, 
				{ CycleType.VTP, vtpBuilders },
				{ CycleType.EngineOnly, engineOnlyBuilders }
			};

			_MeasuredSpeedBEVBuilders = new Dictionary<PowertrainPosition, Func<VectoRunData, IVehicleContainer, ElectricSystem, IPowerTrainComponent, IElectricMotor>>()
			{
				{ PowertrainPosition.BatteryElectricE2, BuildMeasuredSpeedForE2 },
				{ PowertrainPosition.BatteryElectricE3, BuildMeasuredSpeedForE3 },
				{ PowertrainPosition.BatteryElectricE4, BuildMeasuredSpeedForE4 },
				{ PowertrainPosition.IEPC, BuildMeasuredSpeedForIEPC }
			};
			
			_PWheelBEVBuilders = new Dictionary<PowertrainPosition, Func<VectoRunData, IVehicleContainer, ElectricSystem, PWheelCycle, IElectricMotor>>()
			{
				{ PowertrainPosition.BatteryElectricE2, BuildPWheelForE2 },
				{ PowertrainPosition.BatteryElectricE3, BuildPWheelForE3 },
				{ PowertrainPosition.BatteryElectricE4, BuildPWheelForE4 },
				{ PowertrainPosition.IEPC, BuildPWheelForIEPC }
			};
			
			
        }

		public IVehicleContainer Build(VectoRunData data, IModalDataContainer modData, ISumData sumWriter = null)
		{
			var cycleType = data.Cycle.CycleType;	
			var jobType = data.JobType;

			if (!_builders.ContainsKey(cycleType) || !_builders[cycleType].ContainsKey(jobType)) {
				throw new ArgumentException($"Powertrain Builder: cannot build {cycleType} powertrain for job type: {jobType}");
            }

			return _builders[cycleType][jobType].Invoke(data, modData, sumWriter);
		}

		public IExemptedVehicleContainer BuildExempted(VectoRunData data)
		{
			var container = _vehicleContainerFactory.CreateExemptedVehicleContainer(data, null, null);
			return container;
		}

        /// <summary>
        /// Builds an engine only powertrain.
        /// <code>
        /// PowertrainDrivingCycle
        /// └StopStartCombustionEngine
        ///  └(Aux)
        /// </code>
        /// </summary>
		private IVehicleContainer BuildEngineOnly(VectoRunData data, IModalDataContainer modData, ISumData _sumWriter)
		{
			if (_sumWriter == null)
				throw new ArgumentNullException(nameof(_sumWriter));
			if (data.Cycle.CycleType != CycleType.EngineOnly) {
				throw new VectoException("CycleType must be EngineOnly.");
			}

			var container = GetVehicleContainer(ExecutionMode.Engineering, data, modData, _sumWriter);
				//new VehicleContainer(ExecutionMode.Engineering, _simplePowertrainBuilder, modData, _sumWriter) { RunData = data };

			var cycle = new PowertrainDrivingCycle(container, data.Cycle);
			var engine = new EngineOnlyCombustionEngine(container, data.EngineData);
			var directAux = new EngineAuxiliary(container);

			cycle.InPort().Connect(engine.OutPort());
			engine.Connect(directAux.Port());
			directAux.AddCycle(Constants.Auxiliaries.Cycle);

			new EngineOnlyGearboxInfo(container);
			new ZeroMileageCounter(container);
			new DummyDriverInfo(container);

			return container;
        }

        /// <summary>
        /// Builds a PWheel powertrain.
        /// <code>
        /// PWheelCycle
        /// └AxleGear
        ///  ├(Angledrive)
        ///  ├(TransmissionOutputRetarder)
        ///  └CycleGearbox
        ///   ├(TransmissionInputRetarder)
        ///   └Clutch
        ///    └StopStartCombustionEngine
        ///     └(Aux)
        /// </code>
        /// </summary>
        private IVehicleContainer BuildPWheelConventional(VectoRunData data, IModalDataContainer modData, ISumData _sumWriter)
		{
			if (_sumWriter == null)
				throw new ArgumentNullException(nameof(_sumWriter));
			if (data.Cycle.CycleType != CycleType.PWheel) {
				throw new VectoException("CycleType must be PWheel.");
			}

			var container = GetVehicleContainer(ExecutionMode.Engineering, data, modData, _sumWriter);

			var engine = new StopStartCombustionEngine(container, data.EngineData, pt1Disabled: true);
			new PWheelCycle(container, data.Cycle)
				.AddComponent(new AxleGear(container, data.AxleGearData))
				.AddComponent(data.AngledriveData != null ? new Angledrive(container, data.AngledriveData) : null)
				.AddComponent(GetRetarder(RetarderType.TransmissionOutputRetarder, data.Retarder, container))
				.AddComponent(new CycleGearbox(container, data))
				.AddComponent(GetRetarder(RetarderType.TransmissionInputRetarder, data.Retarder, container))
				.AddComponent(new Clutch(container, data.EngineData))
				.AddComponent(engine, GetIdleController(data.PTO, engine, container));
			AddAuxiliaries(engine, container, data);

			new ZeroMileageCounter(container);
			return container;
		}

		/// <summary>
		/// Builds a VTP powertrain.
		/// <code>
		/// VTPCycle
		/// └AxleGear
		///  ├(Angledrive)
		///  ├(TransmissionOutputRetarder)
		///  └VTPGearbox
		///   ├(TransmissionInputRetarder)
		///   └Clutch
		///    └VTPCombustionEngine
		///     └(VTPTruckAuxiliaries or VTPBusAuxiliaries)
		/// </code>
		/// </summary>
		private IVehicleContainer BuildVTPConventional(VectoRunData data, IModalDataContainer modData, ISumData sumWriter)
		{
			if (data.Cycle.CycleType != CycleType.VTP) {
				throw new VectoException("CycleType must be VTP.");
			}

			if ((data.ExecutionMode == ExecutionMode.Declaration) && data.VehicleData.VehicleCategory.IsBus()) {
				throw new VectoException("VTP in Declaration mode is not allowed for buses.");
			}

			var container = GetVehicleContainer(ExecutionMode.Engineering, data, modData, sumWriter);
				//new VehicleContainer(data.ExecutionMode, _simplePowertrainBuilder, modData, sumWriter) { RunData = data };
			var engine = new VTPCombustionEngine(container, data, pt1Disabled: true);

			new VTPCycle(container, data.Cycle)
				.AddComponent(new AxleGear(container, data.AxleGearData))
				.AddComponent(data.AngledriveData != null ? new Angledrive(container, data.AngledriveData) : null)
				.AddComponent(GetRetarder(RetarderType.TransmissionOutputRetarder, data.Retarder, container))
				.AddComponent(new VTPGearbox(container, data))
				.AddComponent(GetRetarder(RetarderType.TransmissionInputRetarder, data.Retarder, container))
				.AddComponent(new Clutch(container, data.EngineData))
				.AddComponent(engine, new CombustionEngine.CombustionEngineNoDoubleClutchIdleController(engine, container));

			new ZeroMileageCounter(container);

			if (data.VehicleData.VehicleCategory.IsLorry()) {
				AddVTPTruckAuxiliaries(data, container, engine);
			} else if (data.VehicleData.VehicleCategory.IsBus()) {
				AddVTPBusAuxiliaries(data, container, engine);
			}

			return container;
		}

		/// <summary>
        /// Builds a measured speed powertrain.
        /// <code>
        /// MeasuredSpeedDrivingCycle
        /// └Vehicle
        ///  └Wheels
        ///   └Brakes
        ///    └AxleGear
        ///     ├(Angledrive)
        ///     ├(TransmissionOutputRetarder)
        ///     └Gearbox, ATGearbox, or APTNGearbox
        ///      ├(TransmissionInputRetarder)
        ///      ├(Clutch)
        ///      └StopStartCombustionEngine
        ///       └(Aux)
        /// </code>
        /// </summary>
        private IVehicleContainer BuildMeasuredSpeedConventional(VectoRunData data, IModalDataContainer modData, ISumData sumWriter)
		{
			if (data.Cycle.CycleType != CycleType.MeasuredSpeed) {
				throw new VectoException("CycleType must be MeasuredSpeed.");
			}

			var container = GetVehicleContainer(ExecutionMode.Engineering, data, modData, sumWriter);
			//new VehicleContainer(ExecutionMode.Engineering, _simplePowertrainBuilder, modData, sumWriter) { RunData = data };
			var engine = new StopStartCombustionEngine(container, data.EngineData);
			new MeasuredSpeedDrivingCycle(container, data.Cycle)
				.AddComponent(new Vehicle(container, data.VehicleData, data.AirdragData))
				.AddComponent(new Wheels(container, data.VehicleData.DynamicTyreRadius, data.VehicleData.WheelsInertia))
				.AddComponent(new Brakes(container))
				.AddComponent(new AxleGear(container, data.AxleGearData))
				.AddComponent(data.AngledriveData != null ? new Angledrive(container, data.AngledriveData) : null)
				.AddComponent(GetRetarder(RetarderType.TransmissionOutputRetarder, data.Retarder, container))
				.AddComponent(GetGearbox(container))
				.AddComponent(GetRetarder(RetarderType.TransmissionInputRetarder, data.Retarder, container))
				.AddComponent(data.GearboxData.Type.ManualTransmission() ? new Clutch(container, data.EngineData) : null)
				.AddComponent(engine, GetIdleController(data.PTO, engine, container));
			AddAuxiliaries(engine, container, data);
			return container;
        }

		private IVehicleContainer BuildMeasuredSpeedGearHybrid(VectoRunData data, IModalDataContainer modData, ISumData sumWriter)
		{
			VerifyCycleType(data, CycleType.MeasuredSpeedGear);
			
			var container = GetVehicleContainer(ExecutionMode.Engineering, data, modData, sumWriter);
			//new VehicleContainer(data.ExecutionMode, _simplePowertrainBuilder, modData, sumWriter) { RunData = data };
			
			var es = ConnectREESS(data, container);

			var strategy = (data.GearboxData.Type.ManualTransmission() || data.GearboxData.Type == GearboxType.IHPC) 
				? (IHybridControlStrategy) new MeasuredSpeedGearHybridStrategy(data, container)
				: (IHybridControlStrategy) new MeasuredSpeedGearATHybridStrategy(data, container);

			// add engine before gearbox so that gearbox can obtain if an ICE is available already in constructor
			var engine = new StopStartCombustionEngine(container, data.EngineData);
			var gearbox = new MeasuredSpeedHybridsCycleGearbox(container, data);
			
			var ctl = new MeasuredSpeedGearHybridController(container, strategy, es) { Gearbox = gearbox, Engine = engine };

			var position = data.ElectricMachinesData[0].Item1;

			TimeRunHybridComponents components = new TimeRunHybridComponents() 
			{
				Cycle = new MeasuredSpeedDrivingCycle(container, data.Cycle),
				Engine = engine,
				Gearbox =  gearbox,
				Clutch = AddClutch(data, container),
				IdleController = GetIdleController(data.PTO, engine, container),
				ElectricMotor = GetElectricMachine<MeasuredSpeedGearHybridsElectricMotor>(position, data.ElectricMachinesData, container, es, ctl),
				HybridController = ctl
			};

			_timerunGearHybridBuilders[position].Invoke(data, container, components);

			var dcdc = new DCDCConverter(container, data.DCDCData.DCDCEfficiency);

            AddHighVoltageAuxiliaries(data, container, es, dcdc);
			AddHybridBusAuxiliaries(data, container, es, dcdc);

			return container;
		}

		

		/// <summary>
		/// Builds a measured speed (with gear) powertrain.
		/// <code>
		/// MeasuredSpeedDrivingCycle
		/// └Vehicle
		///  └Wheels
		///   └Brakes
		///    └AxleGear
		///     ├(Angledrive)
		///     ├(TransmissionOutputRetarder)
		///     └CycleGearbox
		///      ├(TransmissionInputRetarder)
		///      ├(Clutch)
		///      └StopStartCombustionEngine
		///       └(Aux)
		/// </code>
		/// </summary>
		private IVehicleContainer BuildMeasuredSpeedGearConventional(VectoRunData data, IModalDataContainer modData, ISumData sumWriter)
		{
			if (data.Cycle.CycleType != CycleType.MeasuredSpeedGear) {
				throw new VectoException("CycleType must be MeasuredSpeed with Gear.");
			}

			var container = GetVehicleContainer(ExecutionMode.Engineering, data, modData, sumWriter);
			var engine = new StopStartCombustionEngine(container, data.EngineData);
			new MeasuredSpeedDrivingCycle(container, data.Cycle)
				.AddComponent(new Vehicle(container, data.VehicleData, data.AirdragData))
				.AddComponent(new Wheels(container, data.VehicleData.DynamicTyreRadius, data.VehicleData.WheelsInertia))
				.AddComponent(new Brakes(container))
				.AddComponent(new AxleGear(container, data.AxleGearData))
				.AddComponent(data.AngledriveData != null ? new Angledrive(container, data.AngledriveData) : null)
				.AddComponent(GetRetarder(RetarderType.TransmissionOutputRetarder, data.Retarder, container))
				.AddComponent(new CycleGearbox(container, data))
				.AddComponent(GetRetarder(RetarderType.TransmissionInputRetarder, data.Retarder, container))
				.AddComponent(data.GearboxData.Type.ManualTransmission() ? new Clutch(container, data.EngineData) : null)
				.AddComponent(engine);
			AddAuxiliaries(engine, container, data);

			new ATClutchInfo(container);
			return container;
		}

		/// <summary>
		/// Builds a distance-based conventional powertrain.
		/// <code>
		/// DistanceBasedDrivingCycle
		/// └Driver
		///  └Vehicle
		///   └Wheels
		///    └Brakes
		///     └AxleGear
		///      ├(Angledrive)
		///      ├(TransmissionOutputRetarder)
		///      └Gearbox, ATGearbox, or APTNGearbox
		///       ├(TransmissionInputRetarder)
		///       ├(Clutch)
		///       └StopStartCombustionEngine
		///        └(Aux)
		/// </code>
		/// </summary>
		private IVehicleContainer BuildFullPowertrainConventional(VectoRunData data, IModalDataContainer modData, ISumData sumWriter)
		{
			if (data.Cycle.CycleType != CycleType.DistanceBased) {
				throw new VectoException("CycleType must be DistanceBased");
			}

			var container = GetVehicleContainer(data.ExecutionMode, data, modData, sumWriter);
			//new VehicleContainer(data.ExecutionMode, _simplePowertrainBuilder, modData, sumWriter) { RunData = data };
			var cycle = new DistanceBasedDrivingCycle(container, data.Cycle);
			var engine = new StopStartCombustionEngine(container, data.EngineData);
			var idleController = GetIdleController(data.PTO, engine, container);
			cycle.IdleController = idleController as IdleControllerSwitcher;

			var gearbox = GetGearbox(container);

			cycle.AddComponent(new Driver(container, data.DriverData, new DefaultDriverStrategy(container)))
				.AddComponent(new Vehicle(container, data.VehicleData, data.AirdragData))
				.AddComponent(new Wheels(container, data.VehicleData.DynamicTyreRadius, data.VehicleData.WheelsInertia))
				.AddComponent(new Brakes(container))
				.AddComponent(new AxleGear(container, data.AxleGearData))
				.AddComponent(data.AngledriveData != null ? new Angledrive(container, data.AngledriveData) : null)
				.AddComponent(GetRetarder(RetarderType.TransmissionOutputRetarder, data.Retarder, container))
				.AddComponent(gearbox)
				.AddComponent(GetRetarder(RetarderType.TransmissionInputRetarder, data.Retarder, container))
				.AddComponent(data.GearboxData.Type.ManualTransmission() ? new Clutch(container, data.EngineData) : null)
				.AddComponent(engine, idleController);
			AddAuxiliaries(engine, container, data);

			if (gearbox is IAPTGearbox atGbx) {
				atGbx.IdleController = idleController;
			}

			return container;
		}

		/// <summary>
		/// Builds a distance-based parallel hybrid powertrain.
		/// <code>
		/// DistanceBasedDrivingCycle
		/// └Driver
		///  └Vehicle
		///   └Wheels
		///    └HybridController
		///     └Brakes
		///      ├(Engine P4)
		///      └AxleGear
		///       ├(Engine P3)
		///       ├(Angledrive)
		///       ├(TransmissionOutputRetarder)
		///       └Gearbox, ATGearbox, or APTNGearbox
		///        ├(TransmissionInputRetarder)
		///        ├(Engine P2.5)
		///        ├(Engine P2)
		///        ├(SwitchableClutch)
		///        ├(Engine P1)
		///        └StopStartCombustionEngine
		///         └(Aux)
		/// </code>
		/// </summary>
		private IVehicleContainer BuildFullPowertrainParallelHybrid(VectoRunData data, IModalDataContainer modData, ISumData sumWriter)
		{
			if (data.SavedInDeclarationMode) {
				throw new NotImplementedException();
			}
			//if (sumWriter == null)
			//	throw new ArgumentNullException(nameof(sumWriter));
			if (data.Cycle.CycleType != CycleType.DistanceBased) {
				throw new VectoException("CycleType must be DistanceBased");
			}
			if (data.ElectricMachinesData.Any(x => x.Item1 == PowertrainPosition.GEN)) {
				throw new VectoException("ParallelHybrid does not support GEN set.");
			}
			if (data.ElectricMachinesData.Count != 1) {
				throw new VectoException("ParallelHybrid needs exactly one electric motor.");
			}
			if (data.ElectricMachinesData.Any(e => e.Item1 == PowertrainPosition.HybridP0)) {
				throw new VectoException("P0 Hybrids are modeled as SmartAlternator in the BusAuxiliary model.");
			}

			var container = GetVehicleContainer(data.ExecutionMode, data, modData, sumWriter);
			//new VehicleContainer(data.ExecutionMode, _simplePowertrainBuilder, modData, sumWriter) { RunData = data };
			var es = ConnectREESS(data, container);

            HybridController ctl;
			SwitchableClutch clutch = null;
			
			if (data.GearboxData.Type.ManualTransmission() || data.GearboxData.Type == GearboxType.IHPC) {
				ctl = new HybridController(container,new HybridStrategy(data, container), es);
				clutch = new SwitchableClutch(container, data.EngineData);
			} else {
				ctl = new HybridController(container, new HybridStrategyAT(data, container), es);
				new ATClutchInfo(container);
			}
		

			// add engine before gearbox in the container, that gearbox can obtain it
			var engine = new StopStartCombustionEngine(container, data.EngineData);

			var gearbox = GetGearbox(container, ctl.ShiftStrategy);
			if (!(gearbox is IHybridControlledGearbox gbx)) {
				throw new VectoException($"Gearbox can not be used for parallel hybrid: ${gearbox?.GetType()}");
			}

			ctl.Gearbox = gbx;
			ctl.Engine = engine;

			if ((data.SuperCapData != null || data.BatteryData != null) && data.EngineData.WHRType.IsElectrical()) {
				var dcDcConverterEfficiency = DeclarationData.WHRChargerEfficiency;
				var whrCharger = new WHRCharger(container, dcDcConverterEfficiency);
				es.Connect(whrCharger);
				engine.WHRCharger = whrCharger;
			}

			var cycle = new DistanceBasedDrivingCycle(container, data.Cycle);
			var idleController = GetIdleController(data.PTO, engine, container);
			cycle.IdleController = idleController as IdleControllerSwitcher;
			cycle.AddComponent(new Driver(container, data.DriverData, new DefaultDriverStrategy(container)))
				.AddComponent(new Vehicle(container, data.VehicleData, data.AirdragData))
				.AddComponent(new Wheels(container, data.VehicleData.DynamicTyreRadius, data.VehicleData.WheelsInertia))
				.AddComponent(ctl)
				.AddComponent(new Brakes(container))
				.AddComponent(GetElectricMachine(PowertrainPosition.HybridP4, data.ElectricMachinesData, container, es, ctl))
				.AddComponent(new AxleGear(container, data.AxleGearData))
				.AddComponent(GetElectricMachine(PowertrainPosition.HybridP3, data.ElectricMachinesData, container, es, ctl))
				.AddComponent(data.AngledriveData != null ? new Angledrive(container, data.AngledriveData) : null)
				.AddComponent(GetRetarder(RetarderType.TransmissionOutputRetarder, data.Retarder, container))
				.AddComponent(gearbox)
				.AddComponent(GetRetarder(RetarderType.TransmissionInputRetarder, data.Retarder, container))
				.AddComponent(GetElectricMachine(PowertrainPosition.HybridP2_5, data.ElectricMachinesData, container, es, ctl))
				.AddComponent(GetElectricMachine(PowertrainPosition.HybridP2, data.ElectricMachinesData, container, es, ctl))
				.AddComponent(GetElectricMachine(PowertrainPosition.IHPC, data.ElectricMachinesData, container, es, ctl))
				.AddComponent(clutch)
				.AddComponent(GetElectricMachine(PowertrainPosition.HybridP1, data.ElectricMachinesData, container, es, ctl))
				.AddComponent(engine, idleController);
			AddAuxiliaries(engine, container, data);

			if (data.ElectricMachinesData.Any(x => x.Item1 == PowertrainPosition.HybridP1)) {
				// this has to be done _after_ the powertrain is connected together so that the cluch already has its nextComponent set (necessary in the idle controlelr)
				if (gearbox is IAPTGearbox atGbx) {
					atGbx.IdleController = idleController;
				} else {
					clutch.IdleController = idleController;
				}
			}

			var dcdc = new DCDCConverter(container, data.DCDCData.DCDCEfficiency);

            if (data.BusAuxiliaries != null) {
				if (container.BusAux is BusAuxiliariesAdapter busAux) {
					var auxCfg = data.BusAuxiliaries;
					var electricStorage = auxCfg.ElectricalUserInputsConfig.AlternatorType == AlternatorType.Smart
						? new SimpleBattery(container, auxCfg.ElectricalUserInputsConfig.ElectricStorageCapacity, auxCfg.ElectricalUserInputsConfig.StoredEnergyEfficiency)
						: (ISimpleBattery)new NoBattery(container);
					busAux.ElectricStorage = electricStorage;
					if (data.BusAuxiliaries.ElectricalUserInputsConfig.ConnectESToREESS) {
						busAux.DCDCConverter = dcdc;
						es.Connect(dcdc);
					}
				} else {
					throw new VectoException("BusAux data set but no BusAux component found!");
				}
			}
						
			AddHighVoltageAuxiliaries(data, container, es, dcdc);

			///TODO: remove
			data.ElectricAuxDemand = 0.SI<Watt>();

			return container;
		}

        private IVehicleContainer BuildMeasuredSpeedHybrid(VectoRunData data, IModalDataContainer modData, ISumData sumWriter)
		{
			VerifyCycleType(data, CycleType.MeasuredSpeed);

			var container = GetVehicleContainer(ExecutionMode.Engineering, data, modData, sumWriter);
			//new VehicleContainer(data.ExecutionMode, _simplePowertrainBuilder, modData, sumWriter) { RunData = data };
			var es = ConnectREESS(data, container);

			SwitchableClutch clutch = AddClutch(data, container);

			var strategy = (data.GearboxData.Type.ManualTransmission() || data.GearboxData.Type == GearboxType.IHPC)
				? (IHybridControlStrategy) new MeasuredSpeedHybridStrategy(data, container)
				: (IHybridControlStrategy) new MeasuredSpeedHybridStrategyAT(data, container);
			
			HybridController ctl = new HybridController(container, strategy, es);

			// add engine before gearbox so that gearbox can obtain if an ICE is available already in constructor
			var engine = new StopStartCombustionEngine(container, data.EngineData);
			var gearbox = GetGearbox(container, ctl.ShiftStrategy);

			var idleController = GetIdleController(data.PTO, engine, container);
			
			ctl.Gearbox = gearbox as IHybridControlledGearbox;
			ctl.Engine = engine;

			var cycle = new MeasuredSpeedDrivingCycle(container, data.Cycle);

			cycle
				.AddComponent(new Vehicle(container, data.VehicleData, data.AirdragData))
				.AddComponent(new Wheels(container, data.VehicleData.DynamicTyreRadius, data.VehicleData.WheelsInertia))
				.AddComponent(ctl)
				.AddComponent(new Brakes(container))
				.AddComponent(GetElectricMachine(PowertrainPosition.HybridP4, data.ElectricMachinesData, container, es, ctl))
				.AddComponent(new AxleGear(container, data.AxleGearData))
				.AddComponent(GetElectricMachine(PowertrainPosition.HybridP3, data.ElectricMachinesData, container, es, ctl))
				.AddComponent(data.AngledriveData != null ? new Angledrive(container, data.AngledriveData) : null)
				.AddComponent(GetRetarder(RetarderType.TransmissionOutputRetarder, data.Retarder, container))
				.AddComponent(gearbox)
				.AddComponent(GetRetarder(RetarderType.TransmissionInputRetarder, data.Retarder, container))
				.AddComponent(GetElectricMachine(PowertrainPosition.HybridP2_5, data.ElectricMachinesData, container, es, ctl))
				.AddComponent(GetElectricMachine(PowertrainPosition.HybridP2, data.ElectricMachinesData, container, es, ctl))
				.AddComponent(GetElectricMachine(PowertrainPosition.IHPC, data.ElectricMachinesData, container, es, ctl))
				.AddComponent(clutch)
				.AddComponent(GetElectricMachine(PowertrainPosition.HybridP1, data.ElectricMachinesData, container, es, ctl))
				.AddComponent(engine, idleController);
			AddAuxiliaries(engine, container, data);

			SetIdleControllerForHybridP1(data, gearbox, idleController, clutch);

			var dcdc = new DCDCConverter(container, data.DCDCData.DCDCEfficiency);
			AddHighVoltageAuxiliaries(data, container, es, dcdc);
			AddHybridBusAuxiliaries(data, container, es, dcdc);

			return container;
		}


		/// <summary>
		/// Builds a distance-based serial hybrid powertrain for either E4, E3, or E2.
		/// <code>
		/// DistanceBasedDrivingCycle
		/// └Driver
		///  └Vehicle
		///   └Wheels
		///    └SerialHybridController
		///     └Brakes
		///      │ └Engine E4
		///      └AxleGear
		///       │ ├(AxlegearInputRetarder)
		///       │ └Engine E3
		///       ├(AngleDrive)
		///       ├(TransmissionOutputRetarder)
		///       └PEVGearbox or APTNGearbox
		///        ├(TransmissionInputRetarder)
		///        └Engine E2
		/// </code>
		/// </summary>
		private IVehicleContainer BuildFullPowertrainSerialHybrid(VectoRunData data, IModalDataContainer modData, ISumData sumWriter)
		{
			//if (sumWriter == null)
			//	throw new ArgumentNullException(nameof(sumWriter));
			if (data.Cycle.CycleType != CycleType.DistanceBased) {
				throw new VectoException("CycleType must be DistanceBased");
			}
			if (data.ElectricMachinesData.Count(x => x.Item1 == PowertrainPosition.GEN) != 1) {
				throw new VectoException("SerialHybrid needs exactly one GEN set.");
			}
			if (data.ElectricMachinesData.Count(x => x.Item1 != PowertrainPosition.GEN) != 1) {
				throw new VectoException("SerialHybrid needs exactly one electric motor.");
			}

			var container = GetVehicleContainer(data.ExecutionMode, data, modData, sumWriter);
			//new VehicleContainer(data.ExecutionMode, _simplePowertrainBuilder, modData, sumWriter) { RunData = data };
			var es = ConnectREESS(data, container);
			var strategy = data.GearboxData != null && data.GearboxData.Type.AutomaticTransmission()
				? (IHybridControlStrategy)new SerialHybridStrategyAT(data, container)
				: new SerialHybridStrategy(data, container);

			var ctl = new SerialHybridController(container, strategy, es);

			var engine = new StopStartCombustionEngine(container, data.EngineData);

			var idleController = engine.IdleController;
			ctl.Engine = engine;

			if ((data.SuperCapData != null || data.BatteryData != null) && data.EngineData.WHRType.IsElectrical()) {
				var dcDcConverterEfficiency = DeclarationData.WHRChargerEfficiency;
				var whrCharger = new WHRCharger(container, dcDcConverterEfficiency);
				es.Connect(whrCharger);
				engine.WHRCharger = whrCharger;

			}

			var cycle = new DistanceBasedDrivingCycle(container, data.Cycle);
			var powertrain = cycle
				.AddComponent(new Driver(container, data.DriverData, new DefaultDriverStrategy(container)))
				.AddComponent(new Vehicle(container, data.VehicleData, data.AirdragData))
				.AddComponent(new Wheels(container, data.VehicleData.DynamicTyreRadius, data.VehicleData.WheelsInertia))
				.AddComponent(ctl)
				.AddComponent(new Brakes(container));

			var pos = data.ElectricMachinesData.First(x => x.Item1 != PowertrainPosition.GEN).Item1;
			switch (pos) {
				case PowertrainPosition.BatteryElectricE4:
					//-->Engine E4
					powertrain.AddComponent(GetElectricMachine(PowertrainPosition.BatteryElectricE4,
						data.ElectricMachinesData, container, es, ctl));
					new DummyGearboxInfo(container, new GearshiftPosition(0));
					new DummyAxleGearInfo(container);
					new ATClutchInfo(container);
					break;

				case PowertrainPosition.BatteryElectricE3:
					//-->AxleGear-->(AxlegearInputRetarder)-->Engine E3
					powertrain
						.AddComponent(new AxleGear(container, data.AxleGearData))
						.AddComponent(GetRetarder(RetarderType.AxlegearInputRetarder, data.Retarder, container))
						.AddComponent(GetElectricMachine(PowertrainPosition.BatteryElectricE3, data.ElectricMachinesData, container, es, ctl));
					new DummyGearboxInfo(container, new GearshiftPosition(0));
					new ATClutchInfo(container);
					break;

				case PowertrainPosition.BatteryElectricE2:
                    //-->AxleGear-->(AngleDrive)-->(TransmissionOutputRetarder)-->PEVGearbox or APTNGearbox-->(TransmissionInputRetarder)-->Engine E2
					AbstractAMTGearbox gearbox;
					if (data.GearboxData.Type == GearboxType.APTN) {
						gearbox = new APTNGearbox(container, new APTNShiftStrategy(container));
					} else {
						gearbox = new PEVGearbox(container, new PEVAMTShiftStrategy(container));
					}

					powertrain
						.AddComponent(new AxleGear(container, data.AxleGearData))
						.AddComponent(data.AngledriveData != null ? new Angledrive(container, data.AngledriveData) : null)
						.AddComponent(GetRetarder(RetarderType.TransmissionOutputRetarder, data.Retarder, container))
						.AddComponent(gearbox)
						.AddComponent(GetRetarder(RetarderType.TransmissionInputRetarder, data.Retarder, container))
						.AddComponent(GetPEVPTO(container, data))
						.AddComponent(GetElectricMachine(PowertrainPosition.BatteryElectricE2,
							data.ElectricMachinesData, container, es, ctl));
					new ATClutchInfo(container);
					break;

				default:
					throw new ArgumentOutOfRangeException(nameof(pos), pos, "Invalid engine powertrain position for serial hybrid vehicle.");
			}

			ctl.GenSet.AddComponent(GetElectricMachine(PowertrainPosition.GEN, data.ElectricMachinesData, container, es,
					ctl))
				.AddComponent(engine, idleController);
			AddAuxiliariesSerialHybrid(engine,container, data);

			var dcdc = new DCDCConverter(container,
				data.DCDCData.DCDCEfficiency);
            if (data.BusAuxiliaries != null) {
				if (!data.BusAuxiliaries.ElectricalUserInputsConfig.ConnectESToREESS) {
					throw new VectoException("BusAux must be supplied from REESS!");
				}
                if (container.BusAux is BusAuxiliariesAdapter busAux) {
					var auxCfg = data.BusAuxiliaries;
					var electricStorage = auxCfg.ElectricalUserInputsConfig.AlternatorType == AlternatorType.Smart
						? new SimpleBattery(container, auxCfg.ElectricalUserInputsConfig.ElectricStorageCapacity, auxCfg.ElectricalUserInputsConfig.StoredEnergyEfficiency)
						: (ISimpleBattery)new NoBattery(container);
					busAux.ElectricStorage = electricStorage;
					if (data.BusAuxiliaries.ElectricalUserInputsConfig.ConnectESToREESS) {
						busAux.DCDCConverter = dcdc;
						es.Connect(dcdc);
					}

					//new DummyVehicleInfo(container);
				} else {
					throw new VectoException("BusAux data set but no BusAux component found!");
				}
			}

			AddElectricAuxiliaries(data, container, es, cycle, dcdc);

			return container;
		}

		/// <summary>
		/// Builds a battery electric powertrain for either E4, E3, or E2.
		/// <code>
		/// DistanceBasedDrivingCycle
		/// └Driver
		///  └Vehicle
		///   └Wheels
		///    └Brakes
		///     │ └Engine E4
		///     └AxleGear
		///      │ ├(AxlegearInputRetarder)
		///      | └Engine E3
		///      ├(Angledrive)
		///      ├(TransmissionOutputRetarder)
		///      └PEVGearbox or APTNGearbox
		///       ├(TransmissionInputRetarder)
		///       └Engine E2
		/// </code>
		/// </summary>
		private IVehicleContainer BuildFullPowertrainBatteryElectric(VectoRunData data, IModalDataContainer modData, ISumData sumWriter)
		{
			if (data.Cycle.CycleType != CycleType.DistanceBased) {
				throw new VectoException("CycleType must be DistanceBased");
			}
			if (data.ElectricMachinesData.Any(x => x.Item1 == PowertrainPosition.GEN)) {
				throw new VectoException("Battery electric vehicle does not support GEN set.");
			}
			if (data.ElectricMachinesData.Count != 1) {
				throw new VectoException("Battery electric vehicle needs exactly one electric motor.");
			}
			
			var container = GetVehicleContainer(data.ExecutionMode, data, modData, sumWriter);
			//new VehicleContainer(data.ExecutionMode, _simplePowertrainBuilder, modData, sumWriter) { RunData = data };
			var es = ConnectREESS(data, container);

			var ctl = new BatteryElectricMotorController(container, es);

			var cycle = new DistanceBasedDrivingCycle(container, data.Cycle);

			var powertrain = cycle
				.AddComponent(new Driver(container, data.DriverData, new DefaultDriverStrategy(container)))
				.AddComponent(new Vehicle(container, data.VehicleData, data.AirdragData))
				.AddComponent(new Wheels(container, data.VehicleData.DynamicTyreRadius, data.VehicleData.WheelsInertia))
				.AddComponent(new Brakes(container));

			var pos = data.ElectricMachinesData.First().Item1;
			IElectricMotor em;
			switch (pos) {
				case PowertrainPosition.BatteryElectricE4:
					//-->Engine E4
					em = GetElectricMachine(PowertrainPosition.BatteryElectricE4, data.ElectricMachinesData, container, es, ctl);
					powertrain.AddComponent(em);
					new DummyGearboxInfo(container);
					new DummyAxleGearInfo(container);
					new ATClutchInfo(container);
					break;

				case PowertrainPosition.BatteryElectricE3:
					//-->AxleGear-->(AxlegearInputRetarder)-->Engine E3
					em = GetElectricMachine(PowertrainPosition.BatteryElectricE3, data.ElectricMachinesData, container, es, ctl);
					powertrain
						.AddComponent(new AxleGear(container, data.AxleGearData))
						.AddComponent(GetRetarder(RetarderType.AxlegearInputRetarder, data.Retarder, container))
						.AddComponent(em);
					new DummyGearboxInfo(container);
					new ATClutchInfo(container);
					break;

				case PowertrainPosition.BatteryElectricE2:
					//-->AxleGear-->(Angledrive)-->(TransmissionOutputRetarder)-->APTNGearbox or PEVGearbox-->(TransmissionInputRetarder)-->Engine E2
					var gearbox = data.GearboxData.Type == GearboxType.APTN
						? (AbstractAMTGearbox)new APTNGearbox(container, new APTNShiftStrategy(container))
						: new PEVGearbox(container, new PEVAMTShiftStrategy(container));
					em = GetElectricMachine(PowertrainPosition.BatteryElectricE2, data.ElectricMachinesData, container, es, ctl);
					powertrain
						.AddComponent(new AxleGear(container, data.AxleGearData))
						.AddComponent(data.AngledriveData != null ? new Angledrive(container, data.AngledriveData) : null)
						.AddComponent(GetRetarder(RetarderType.TransmissionOutputRetarder, data.Retarder, container))
						.AddComponent(gearbox)
						.AddComponent(GetRetarder(RetarderType.TransmissionInputRetarder, data.Retarder, container))
						.AddComponent(GetPEVPTO(container, data))
						.AddComponent(em);

					new ATClutchInfo(container);
					break;

				default:
					throw new ArgumentOutOfRangeException(nameof(pos), pos, "Invalid engine powertrain position for BatteryElectric Vehicle");
			}

			new DummyEngineInfo(container);

			var dcdc = new DCDCConverter(container, data.DCDCData.DCDCEfficiency);
            if (data.BusAuxiliaries != null) {
				if (!data.BusAuxiliaries.ElectricalUserInputsConfig.ConnectESToREESS) {
					throw new VectoException("BusAux must be supplied from REESS!");
				}

				var auxCfg = data.BusAuxiliaries;
				var busAux = new BusAuxiliariesAdapter(container, auxCfg);
				var electricStorage = new NoBattery(container);
				busAux.ElectricStorage = electricStorage;
	
				busAux.DCDCConverter = dcdc;
				es.Connect(dcdc);
				em.BusAux = busAux;
			} 

			AddElectricAuxiliaries(data, container, es, cycle, dcdc);

            return container;
		}
		
		private IVehicleContainer BuildPWheelBatteryElectric(VectoRunData data, IModalDataContainer modData, ISumData sumWriter)
        {
			VerifyCycleType(data, CycleType.PWheel);
			ValidateBatteryElectric(data);

			var container = GetVehicleContainer(data.ExecutionMode, data, modData, sumWriter);
			//new VehicleContainer(data.ExecutionMode, _simplePowertrainBuilder, modData, sumWriter) { RunData = data };
			var es = ConnectREESS(data, container);
			
			var powertrain = new PWheelCycle(container, data.Cycle);
			
			var position = data.ElectricMachinesData.First().Item1;
			
			IElectricMotor em = _PWheelBEVBuilders[position].Invoke(data, container, es, powertrain);

			var dcdc = new DCDCConverter(container, GetDCDCEfficiency(data));
            if (data.BusAuxiliaries != null) {
				AddBEVBusAuxiliaries(data, container, es, em, dcdc);
			}
			else {
				AddElectricAuxiliaries(data, container, es, null, dcdc);
			}

			return container;
        }

        private IElectricMotor BuildPWheelForIEPC(VectoRunData data, IVehicleContainer container, ElectricSystem es, PWheelCycle cycle)
        { 
			var ctl = new PWheelBatteryElectricMotorController(container, es);
			
			IElectricMotor em = GetElectricMachine(PowertrainPosition.IEPC, data.ElectricMachinesData, container, es, ctl);

			ITnInProvider powertrain = cycle;

			if (data.AxleGearData != null) {
				powertrain = cycle
					.AddComponent(new AxleGear(container, data.AxleGearData))
					.AddComponent(GetRetarder(RetarderType.AxlegearInputRetarder, data.Retarder, container));
            }

			powertrain
				.AddComponent(new BEVCycleGearbox(container, data))
				.AddComponent(em);

			new ATClutchInfo(container);
			new DummyEngineInfo(container);

			if (data.AxleGearData == null) {
				new DummyAxleGearInfo(container);
			}

			return em;
		}

        private IElectricMotor BuildPWheelForE2(VectoRunData data, IVehicleContainer container, ElectricSystem es, PWheelCycle powertrain)
		{
			var ctl = new PWheelBatteryElectricMotorController(container, es);
			
			IElectricMotor em = GetElectricMachine(PowertrainPosition.BatteryElectricE2, data.ElectricMachinesData, container, es, ctl);
			
			powertrain
				.AddComponent(new AxleGear(container, data.AxleGearData))
				.AddComponent(data.AngledriveData != null ? new Angledrive(container, data.AngledriveData) : null)
				.AddComponent(GetRetarder(RetarderType.TransmissionOutputRetarder, data.Retarder, container))
				.AddComponent(new BEVCycleGearbox(container, data))
				.AddComponent(GetRetarder(RetarderType.TransmissionInputRetarder, data.Retarder, container))
				.AddComponent(GetPEVPTO(container, data))
				.AddComponent(em);

			new ATClutchInfo(container);
			new DummyEngineInfo(container);
			new ZeroMileageCounter(container);

			return em;
        }

		private IElectricMotor BuildPWheelForE3(VectoRunData data, IVehicleContainer container, ElectricSystem es, PWheelCycle powertrain)
		{
			var ctl = new PWheelBatteryElectricMotorController(container, es);

			IElectricMotor em = GetElectricMachine(PowertrainPosition.BatteryElectricE3, data.ElectricMachinesData, container, es, ctl);
			
			powertrain
				.AddComponent(new AxleGear(container, data.AxleGearData))
				.AddComponent(GetRetarder(RetarderType.AxlegearInputRetarder, data.Retarder, container))
				.AddComponent(em);

			new DummyGearboxInfo(container);
			new ATClutchInfo(container);
			new DummyEngineInfo(container);

			return em;
        }

		private IElectricMotor BuildPWheelForE4(VectoRunData data, IVehicleContainer container, ElectricSystem es, PWheelCycle powertrain)
		{
			var ctl = new PWheelBatteryElectricMotorController(container, es);

			IElectricMotor em = GetElectricMachine(PowertrainPosition.BatteryElectricE4, data.ElectricMachinesData, container, es, ctl);
			
			powertrain.AddComponent(em);
			new DummyGearboxInfo(container);
			new DummyAxleGearInfo(container);
			new ATClutchInfo(container);

			return em;
        }

		private IVehicleContainer BuildMeasuredSpeedBatteryElectric(VectoRunData data, IModalDataContainer modData, ISumData sumWriter)
        {
			VerifyCycleType(data, CycleType.MeasuredSpeed);
			ValidateBatteryElectric(data);

			var container = GetVehicleContainer(data.ExecutionMode, data, modData, sumWriter);
			//new VehicleContainer(data.ExecutionMode, _simplePowertrainBuilder, modData, sumWriter) { RunData = data };
			var es = ConnectREESS(data, container);

			IPowerTrainComponent powertrain = new MeasuredSpeedDrivingCycle(container, data.Cycle)
				.AddComponent(new Vehicle(container, data.VehicleData, data.AirdragData))
				.AddComponent(new Wheels(container, data.VehicleData.DynamicTyreRadius, data.VehicleData.WheelsInertia))
				.AddComponent(new Brakes(container));

			var position = data.ElectricMachinesData.First().Item1;
			
			IElectricMotor em = _MeasuredSpeedBEVBuilders[position].Invoke(data, container, es, powertrain);

			
			var dcdc = new DCDCConverter(container,GetDCDCEfficiency(data));
            if (data.BusAuxiliaries != null) {
				AddBEVBusAuxiliaries(data, container, es, em, dcdc);
			}
			else {
				AddElectricAuxiliaries(data, container, es, null, dcdc);
			}

			return container;
        }

		private IElectricMotor BuildMeasuredSpeedForIEPC(VectoRunData data, IVehicleContainer container, ElectricSystem es, IPowerTrainComponent powertrain)
        { 
			var ctl = new BatteryElectricMotorController(container, es);

			var gearbox = data.GearboxData.Gears.Count > 1
				? (IGearbox)new IEPCGearbox(container, new APTNShiftStrategy(container))
				: new SingleSpeedGearbox(container, data.GearboxData);

			IElectricMotor em = GetElectricMachine(PowertrainPosition.IEPC, data.ElectricMachinesData, container, es, ctl);

			powertrain.AddComponent((data.AxleGearData != null) ? new AxleGear(container, data.AxleGearData) : null)
				.AddComponent(GetRetarder(RetarderType.AxlegearInputRetarder, data.Retarder, container))
				.AddComponent(gearbox)
				.AddComponent(em);

			new ATClutchInfo(container);
			new DummyEngineInfo(container);

			if (data.AxleGearData == null) {
				new DummyAxleGearInfo(container);
			}

			return em;
		}

        private IElectricMotor BuildMeasuredSpeedForE2(VectoRunData data, IVehicleContainer container, ElectricSystem es, IPowerTrainComponent powertrain)
        {
			var ctl = new BatteryElectricMotorController(container, es);

            var strategy = (data.GearboxData.Type == GearboxType.APTN) ? 
				new APTNShiftStrategy(container) : new PEVAMTShiftStrategy(container);

			var gearbox = (data.GearboxData.Type == GearboxType.APTN) ? 
				(AbstractAMTGearbox) new APTNGearbox(container, strategy) : new PEVGearbox(container, strategy);

			IElectricMotor em = GetElectricMachine(PowertrainPosition.BatteryElectricE2, data.ElectricMachinesData, container, es, ctl);

			powertrain.AddComponent(new AxleGear(container, data.AxleGearData))
				.AddComponent(data.AngledriveData != null ? new Angledrive(container, data.AngledriveData) : null)
				.AddComponent(GetRetarder(RetarderType.TransmissionOutputRetarder, data.Retarder, container))
				.AddComponent(gearbox)
				.AddComponent(GetRetarder(RetarderType.TransmissionInputRetarder, data.Retarder, container))
				.AddComponent(GetPEVPTO(container, data))
				.AddComponent(em);

			new ATClutchInfo(container);
				
			return em;
        }

        private IElectricMotor BuildMeasuredSpeedForE3(VectoRunData data, IVehicleContainer container, ElectricSystem es, IPowerTrainComponent powertrain)
        { 
			var ctl = new BatteryElectricMotorController(container, es);
			
			IElectricMotor em = GetElectricMachine(PowertrainPosition.BatteryElectricE3, data.ElectricMachinesData, container, es, ctl);

			powertrain.AddComponent(new AxleGear(container, data.AxleGearData))
				.AddComponent(GetRetarder(RetarderType.AxlegearInputRetarder, data.Retarder, container))
				.AddComponent(em);

			new DummyGearboxInfo(container);
			new ATClutchInfo(container);
			new DummyEngineInfo(container);

			return em;
        }

        private IElectricMotor BuildMeasuredSpeedForE4(VectoRunData data, IVehicleContainer container, ElectricSystem es, IPowerTrainComponent powertrain)
        {
			var ctl = new BatteryElectricMotorController(container, es);
			
			IElectricMotor em = GetElectricMachine(PowertrainPosition.BatteryElectricE4, data.ElectricMachinesData, container, es, ctl);

			powertrain.AddComponent(em);

			new DummyGearboxInfo(container);
			new DummyAxleGearInfo(container);
			new ATClutchInfo(container);

			return em;
		}

        private void AddBEVBusAuxiliaries(VectoRunData data, IVehicleContainer container, ElectricSystem es, IElectricMotor em, DCDCConverter dcdc)
        {
			if (data.BusAuxiliaries == null) {
				return;
			}
			
			if (!data.BusAuxiliaries.ElectricalUserInputsConfig.ConnectESToREESS) {
				throw new VectoException("BusAux must be supplied from REESS!");
			}

			var busAux = new BusAuxiliariesAdapter(container, data.BusAuxiliaries);			


			busAux.DCDCConverter = dcdc;
			busAux.ElectricStorage = new NoBattery(container);

			es.Connect(dcdc);
			em.BusAux = busAux;
		}

        private IVehicleContainer BuildMeasuredSpeedGearBatteryElectric(VectoRunData data, IModalDataContainer modData, ISumData sumWriter)
        {
			VerifyCycleType(data, CycleType.MeasuredSpeedGear);
			ValidateBatteryElectric(data);

			var container = GetVehicleContainer(data.ExecutionMode, data, modData, sumWriter);
			//new VehicleContainer(data.ExecutionMode, _simplePowertrainBuilder, modData, sumWriter) { RunData = data };
			var es = ConnectREESS(data, container);
			
			var position = data.ElectricMachinesData.First().Item1;

			if (position != PowertrainPosition.BatteryElectricE2) {
				throw new ArgumentOutOfRangeException(nameof(position), position, null);
            }

			var ctl = new BatteryElectricMotorController(container, es);
			IElectricMotor em = GetElectricMachine(position, data.ElectricMachinesData, container, es, ctl);

			var timeBasedCycle = new MeasuredSpeedDrivingCycle(container, data.Cycle);
			
			timeBasedCycle
				.AddComponent(new Vehicle(container, data.VehicleData, data.AirdragData))
				.AddComponent(new Wheels(container, data.VehicleData.DynamicTyreRadius, data.VehicleData.WheelsInertia))
				.AddComponent(new Brakes(container))
				.AddComponent(new AxleGear(container, data.AxleGearData))
				.AddComponent(data.AngledriveData != null ? new Angledrive(container, data.AngledriveData) : null)
				.AddComponent(GetRetarder(RetarderType.TransmissionOutputRetarder, data.Retarder, container))
				.AddComponent(new BEVCycleGearbox(container, data))
				.AddComponent(GetRetarder(RetarderType.TransmissionInputRetarder, data.Retarder, container))
				.AddComponent(GetPEVPTO(container, data))
				.AddComponent(em);

			new ATClutchInfo(container);
			new DummyEngineInfo(container);

			var dcdc = new DCDCConverter(container, GetDCDCEfficiency(data));
            if (data.BusAuxiliaries != null) {
				AddBEVBusAuxiliaries(data, container, es, em, dcdc);
			}
			else {
				AddElectricAuxiliaries(data, container, es, null, dcdc);
			}

			return container;
		}

		private IVehicleContainer BuildMeasuredSpeedGearIEPC(VectoRunData data, IModalDataContainer modData, ISumData sumWriter)
        {
			VerifyCycleType(data, CycleType.MeasuredSpeedGear);
			ValidateBatteryElectric(data);

			var container = GetVehicleContainer(data.ExecutionMode, data, modData, sumWriter);
			//new VehicleContainer(data.ExecutionMode, _simplePowertrainBuilder, modData, sumWriter) { RunData = data };
			var es = ConnectREESS(data, container);
			
			var position = data.ElectricMachinesData.First().Item1;

			if (position != PowertrainPosition.IEPC) {
				throw new ArgumentOutOfRangeException(nameof(position), position, null);
            }

			var ctl = new BatteryElectricMotorController(container, es);
			IElectricMotor em = GetElectricMachine(position, data.ElectricMachinesData, container, es, ctl);

			var timeBasedCycle = new MeasuredSpeedDrivingCycle(container, data.Cycle);
			
			timeBasedCycle
				.AddComponent(new Vehicle(container, data.VehicleData, data.AirdragData))
				.AddComponent(new Wheels(container, data.VehicleData.DynamicTyreRadius, data.VehicleData.WheelsInertia))
				.AddComponent(new Brakes(container))
				.AddComponent(data.AxleGearData != null ? new AxleGear(container, data.AxleGearData) : null)
				.AddComponent(GetRetarder(RetarderType.AxlegearInputRetarder, data.Retarder, container))
				.AddComponent(new BEVCycleGearbox(container, data))
				.AddComponent(em);

			new ATClutchInfo(container);
			new DummyEngineInfo(container);

			if (data.AxleGearData == null) {
				new DummyAxleGearInfo(container);
			}

			var dcdc = new DCDCConverter(container, GetDCDCEfficiency(data));
            if (data.BusAuxiliaries != null) {
				AddBEVBusAuxiliaries(data, container, es, em, dcdc);
			}
			else {
				AddElectricAuxiliaries(data, container, es, null,dcdc);
			}

			return container;
		}

		private void ValidateBatteryElectric(VectoRunData data)
        {
			if (data.ElectricMachinesData.Count > 1) {
				throw new VectoException("Electric motors on multiple positions not supported");
			}

			if (data.BatteryData != null && data.SuperCapData != null) {
				throw new VectoException("Only one REESS is supported.");
			}

			var position = data.ElectricMachinesData.First().Item1;

			if (position == PowertrainPosition.HybridPositionNotSet) {
				throw new VectoException("invalid powertrain position");
            }

			if (position == PowertrainPosition.HybridP0 ||
				position == PowertrainPosition.HybridP1 ||
				position == PowertrainPosition.HybridP2 ||
				position == PowertrainPosition.HybridP3 ||
				position == PowertrainPosition.HybridP4) {

				throw new VectoException("BatteryElectric Vehicle does not support parallel powertrain configurations");
			}
		}

		/// <summary>
		/// Builds a battery electric powertrain for either E4, E3, or E2.
		/// <code>
		/// DistanceBasedDrivingCycle
		/// └Driver
		///  └Vehicle
		///   └Wheels
		///    └Brakes
		///     └AxleGear
		///      │ ├(AxlegearInputRetarder)
		///      | ├Singlespeed Gearbox
		///      | └Engine IEPC
		///      └ APTNGearbox
		///       └Engine IEPC
		/// </code>
		/// </summary>
		private IVehicleContainer BuildFullPowertrainIEPCE(VectoRunData data, IModalDataContainer modData, ISumData sumWriter)
		{
			if (data.Cycle.CycleType != CycleType.DistanceBased) {
				throw new VectoException("CycleType must be DistanceBased");
			}
			if (data.ElectricMachinesData.Any(x => x.Item1 == PowertrainPosition.GEN)) {
				throw new VectoException("IEPC vehicle does not support GEN set.");
			}
			if (data.ElectricMachinesData.Count != 1) {
				throw new VectoException("IEPC vehicle needs exactly one electric motor.");
			}

			var container = GetVehicleContainer(data.ExecutionMode, data, modData, sumWriter);
			//new VehicleContainer(data.ExecutionMode, _simplePowertrainBuilder, modData, sumWriter) { RunData = data };
			var es = ConnectREESS(data, container);

			var ctl = new BatteryElectricMotorController(container, es);

			var cycle = new DistanceBasedDrivingCycle(container, data.Cycle);
			var powertrain = cycle
				.AddComponent(new Driver(container, data.DriverData, new DefaultDriverStrategy(container)))
				.AddComponent(new Vehicle(container, data.VehicleData, data.AirdragData))
				.AddComponent(new Wheels(container, data.VehicleData.DynamicTyreRadius, data.VehicleData.WheelsInertia))
				.AddComponent(new Brakes(container));

			var pos = data.ElectricMachinesData.First().Item1;
			IElectricMotor em;
			if (pos != PowertrainPosition.IEPC) {
				throw new ArgumentOutOfRangeException(nameof(pos), pos, "Invalid engine powertrain position for BatteryElectric Vehicle");
			}
			
			//-->AxleGear-->APTNGearbox or SinglespeedGearbox-->Engine E2
			var gearbox = data.GearboxData.Gears.Count > 1
				? (IGearbox)new IEPCGearbox(container, new APTNShiftStrategy(container))
				: new SingleSpeedGearbox(container, data.GearboxData);
			em = GetElectricMachine(PowertrainPosition.IEPC, data.ElectricMachinesData, container, es, ctl);
			powertrain
				.AddComponent(data.AxleGearData != null ? new AxleGear(container, data.AxleGearData) : null)
				.AddComponent(GetRetarder(RetarderType.AxlegearInputRetarder, data.Retarder, container))
				.AddComponent(gearbox)
				.AddComponent(em);

			new ATClutchInfo(container);
			if (data.AxleGearData == null) {
				new DummyAxleGearInfo(container);
			}
			new DummyEngineInfo(container);

			var dcdc = new DCDCConverter(container, data.DCDCData.DCDCEfficiency);
			if (data.BusAuxiliaries != null) {
				if (!data.BusAuxiliaries.ElectricalUserInputsConfig.ConnectESToREESS) {
					throw new VectoException("BusAux must be supplied from REESS!");
				}

				var auxCfg = data.BusAuxiliaries;
				var busAux = new BusAuxiliariesAdapter(container, auxCfg);
				var electricStorage = new NoBattery(container);
				busAux.ElectricStorage = electricStorage;

				busAux.DCDCConverter = dcdc;
				es.Connect(dcdc);
				em.BusAux = busAux;
			}

			AddElectricAuxiliaries(data, container, es, cycle, dcdc);

            return container;
		}

		/// <summary>
		/// Builds a battery electric powertrain for either E4, E3, or E2.
		/// <code>
		/// DistanceBasedDrivingCycle
		/// └Driver
		///  └Vehicle
		///   └Wheels
		///    └Brakes
		///     └AxleGear
		///      │ ├(AxlegearInputRetarder)
		///      | ├Singlespeed Gearbox
		///      | └Engine IEPC
		///      └ APTNGearbox
		///       └Engine IEPC
		/// </code>
		/// </summary>
		private IVehicleContainer BuildFullPowertrainIEPCSerial(VectoRunData data, IModalDataContainer modData, ISumData sumWriter)
		{
			//if (_sumWriter == null)
			//	throw new ArgumentNullException(nameof(_sumWriter));
			if (data.Cycle.CycleType != CycleType.DistanceBased) {
				throw new VectoException("CycleType must be DistanceBased");
			}
			if (data.ElectricMachinesData.Count(x => x.Item1 == PowertrainPosition.GEN) != 1) {
				throw new VectoException("IEPC vehicle needs exactly one GEN set.");
			}
			if (data.ElectricMachinesData.Count(x => x.Item1 != PowertrainPosition.GEN) != 1) {
				throw new VectoException("IEPC vehicle needs exactly one electric motor.");
			}

			var container = GetVehicleContainer(data.ExecutionMode, data, modData, sumWriter);
			//new VehicleContainer(data.ExecutionMode, _simplePowertrainBuilder, modData, _sumWriter) { RunData = data };
			var es = ConnectREESS(data, container);

			var strategy = new SerialHybridStrategy(data, container);
			var ctl = new SerialHybridController(container, strategy, es);

			var engine = new StopStartCombustionEngine(container, data.EngineData);

			var idleController = engine.IdleController;
			ctl.Engine = engine;

            if (data.BusAuxiliaries != null)
            {
                var aux = new HighVoltageElectricAuxiliary(container);
                aux.AddConstant("P_aux_el", data.ElectricAuxDemand ?? 0.SI<Watt>());
                es.Connect(aux);
            }
                
			var cycle = new DistanceBasedDrivingCycle(container, data.Cycle);
			var powertrain = cycle
				.AddComponent(new Driver(container, data.DriverData, new DefaultDriverStrategy(container)))
				.AddComponent(new Vehicle(container, data.VehicleData, data.AirdragData))
				.AddComponent(new Wheels(container, data.VehicleData.DynamicTyreRadius, data.VehicleData.WheelsInertia))
				.AddComponent(ctl)
				.AddComponent(new Brakes(container));

			var pos = data.ElectricMachinesData.First(x => x.Item1 != PowertrainPosition.GEN).Item1;
			IElectricMotor em;
			if (pos != PowertrainPosition.IEPC) {
				throw new ArgumentOutOfRangeException(nameof(pos), pos, "Invalid engine powertrain position for BatteryElectric Vehicle");
			}

			//-->AxleGear-->APTNGearbox or SinglespeedGearbox-->Engine E2
			var gearbox = data.GearboxData.Gears.Count > 1
				? (IGearbox)new IEPCGearbox(container, new APTNShiftStrategy(container))
				: new SingleSpeedGearbox(container, data.GearboxData);
			em = GetElectricMachine(PowertrainPosition.IEPC, data.ElectricMachinesData, container, es, ctl);
			powertrain
				.AddComponent(data.AxleGearData != null ? new AxleGear(container, data.AxleGearData) : null)
				.AddComponent(GetRetarder(RetarderType.AxlegearInputRetarder, data.Retarder, container))
				.AddComponent(gearbox)
				.AddComponent(em);

			new ATClutchInfo(container);
			if (data.AxleGearData == null) {
				new DummyAxleGearInfo(container);
			}

			ctl.GenSet.AddComponent(GetElectricMachine(PowertrainPosition.GEN, data.ElectricMachinesData, container, es, ctl))
				.AddComponent(engine, idleController);
			AddAuxiliaries(engine, container, data);

			var dcdc = new DCDCConverter(container, data.DCDCData.DCDCEfficiency);
            if (data.BusAuxiliaries != null) {
				if (container.BusAux is BusAuxiliariesAdapter busAux) {
					if (!data.BusAuxiliaries.ElectricalUserInputsConfig.ConnectESToREESS) {
						throw new VectoException("BusAux must be supplied from REESS!");
					}

					var auxCfg = data.BusAuxiliaries;
					var electricStorage = new NoBattery(container);
					busAux.ElectricStorage = electricStorage;
					busAux.DCDCConverter = dcdc;
					es.Connect(dcdc);
					//em.BusAux = busAux;
				}
			} else {
                AddElectricAuxiliaries(data, container, es, cycle,dcdc);
			}
						
			return container;
		}

		protected IVehicleContainer GetVehicleContainer(ExecutionMode mode, VectoRunData runData, IModalDataContainer modData, ISumData sumWriter)
		{
			var container = _vehicleContainerFactory.CreateVehicleContainer(runData, modData, sumWriter);
			return container;
		}

		protected IGearbox GetGearbox(IVehicleContainer container, IShiftStrategy strategy = null)
		{
			strategy = strategy ?? GetShiftStrategy(container);

			var isMeasuredSpeedHybrid = (container.RunData.JobType == VectoSimulationJobType.ParallelHybridVehicle)
										&& (container.RunData.Cycle.CycleType == CycleType.MeasuredSpeed);
			
			switch (container.RunData.GearboxData.Type) {
				case GearboxType.AMT:
				case GearboxType.MT:
					return isMeasuredSpeedHybrid
						? (AbstractAMTGearbox) new MeasuredSpeedHybridsGearbox(container, strategy)
						: new AMTGearbox(container, strategy);
				case GearboxType.ATPowerSplit:
				case GearboxType.ATSerial:
					new ATClutchInfo(container);
					return new APTGearbox(container, strategy);
				case GearboxType.APTN:
					return new APTNGearbox(container, strategy);
				case GearboxType.IHPC:
					return new APTNGearbox(container, strategy);
				default:
					throw new ArgumentOutOfRangeException("Unknown Gearbox Type",
						container.RunData.GearboxData.Type.ToString());
			}
		}

    }

	public class SimpleCharger : IElectricChargerPort, IUpdateable
	{
		#region Implementation of IElectricChargerPort
		private Watt _chargingPower;
		public SimpleCharger() => _chargingPower = 0.SI<Watt>();
		public Watt Initialize() => _chargingPower = 0.SI<Watt>();
		public Watt PowerDemand(Second absTime, Second dt, Watt powerDemandEletricMotor, Watt auxPower, bool dryRun) => _chargingPower;
		#endregion

		#region Implementation of IUpdateable
		public bool UpdateFrom(object other)
		{
			if (other is IElectricSystemInfo es) {
				_chargingPower = es.ChargePower;
				return true;
			}

			if (other is Watt w) {
				_chargingPower = w;
				return true;
			}

			return false;
		}
		#endregion
	}

	internal class DummyEngineInfo : VectoSimulationComponent, IEngineInfo, IEngineControl
	{
		public DummyEngineInfo(IVehicleContainer container) : base(container)
		{
			EngineIdleSpeed = 100.RPMtoRad();
			EngineSpeed = 100.RPMtoRad();
		}

		#region Overrides of VectoSimulationComponent

		protected override void DoWriteModalResults(Second time, Second simulationInterval, IModalDataContainer container)
		{
		}

		protected override void DoCommitSimulationStep(Second time, Second simulationInterval)
		{
		}

		#endregion

		#region Implementation of IEngineInfo

		public PerSecond EngineSpeed { get; }
		public NewtonMeter EngineTorque { get; }
		public Watt EngineStationaryFullPower(PerSecond angularSpeed)
		{
			return null;
		}

		public Watt EngineDynamicFullLoadPower(PerSecond avgEngineSpeed, Second dt)
		{
			return null;
		}

		public Watt EngineDragPower(PerSecond angularSpeed)
		{
			return 0.SI<Watt>();
		}

		public Watt EngineAuxDemand(PerSecond avgEngineSpeed, Second dt)
		{
			return null;
		}

		public PerSecond EngineIdleSpeed { get; }
		public PerSecond EngineRatedSpeed { get; }
		public PerSecond EngineN95hSpeed { get; }
		public PerSecond EngineN80hSpeed { get; }
		public bool EngineOn { get; private set; }

		#endregion

		#region Implementation of IEngineControl

		public bool CombustionEngineOn { get => false; set { } }

		#endregion

		protected override bool DoUpdateFrom(object other)
		{
			if (other is IEngineInfo info) {
				EngineOn = info.EngineOn;
				return true;
			} else {
				return false;
			}

		}
	}

	public class SimpleElectricMotorControl : IElectricMotorControl
	{
		public bool EmOff { get; set; }

		public NewtonMeter MechanicalAssistPower(Second absTime, Second dt, NewtonMeter outTorque, PerSecond prevOutAngularVelocity,
			PerSecond currOutAngularVelocity, NewtonMeter maxDriveTorque, NewtonMeter maxRecuperationTorque,
			PowertrainPosition position, bool dryRun)
		{
			if (EmOff) {
				return null;
			}

			
			if (dryRun) {
				return -outTorque;
			}
			return (-outTorque).LimitTo(maxDriveTorque ?? 0.SI<NewtonMeter>(), maxRecuperationTorque ?? VectoMath.Max(maxDriveTorque, 0.SI<NewtonMeter>()));
		}
	}

	public class GensetMotorController : IGensetMotorController
    {
		public GensetMotorController(IVehicleContainer container, ElectricSystem es)
		{

		}

		#region Implementation of IElectricMotorControl

		public NewtonMeter MechanicalAssistPower(Second absTime, Second dt, NewtonMeter outTorque, PerSecond prevOutAngularVelocity,
			PerSecond currOutAngularVelocity, NewtonMeter maxDriveTorque, NewtonMeter maxRecuperationTorque,
			PowertrainPosition position, bool dryRun)
		{
			return EMTorque;
		}

		public NewtonMeter EMTorque { get; set; }

		#endregion
	}

	[Obsolete("Replaced with SimpleElectricMotorControl")]
	public class DummyElectricMotorControl : IElectricMotorControl
	{
		#region Implementation of IElectricMotorControl

		public NewtonMeter MechanicalAssistPower(Second absTime, Second dt, NewtonMeter outTorque, PerSecond prevOutAngularVelocity, PerSecond currOutAngularVelocity, NewtonMeter maxDriveTorque, NewtonMeter maxRecuperationTorque, PowertrainPosition position, bool dryRun)
		{
			return EmTorque;
		}

		public NewtonMeter EmTorque;

		#endregion
	}

	internal class DummyDriverInfo : VectoSimulationComponent, IDriverInfo
	{
		public DummyDriverInfo(IVehicleContainer container) : base(container)
		{

		}

		#region Overrides of VectoSimulationComponent

		protected override void DoWriteModalResults(Second time, Second simulationInterval, IModalDataContainer container)
		{

		}

		protected override void DoCommitSimulationStep(Second time, Second simulationInterval)
		{

		}

		#endregion

		#region Implementation of IDriverInfo

		public DrivingBehavior DriverBehavior => DrivingBehavior.Accelerating;

		public DrivingAction DrivingAction => DrivingAction.Accelerate;

		public MeterPerSquareSecond DriverAcceleration => 0.SI<MeterPerSquareSecond>();
		public PCCStates PCCState => PCCStates.OutsideSegment;
		public MeterPerSecond NextBrakeTriggerSpeed => 0.SI<MeterPerSecond>();
		public MeterPerSecond ApplyOverspeed(MeterPerSecond targetSpeed) => targetSpeed;

		#endregion

		protected override bool DoUpdateFrom(object other) => false;
	}

	internal class EngineOnlyGearboxInfo : VectoSimulationComponent, IGearboxInfo
	{
		public EngineOnlyGearboxInfo(IVehicleContainer container) : base(container)
		{

		}

		#region Overrides of VectoSimulationComponent

		protected override void DoWriteModalResults(Second time, Second simulationInterval, IModalDataContainer container)
		{

		}

		protected override void DoCommitSimulationStep(Second time, Second simulationInterval)
		{

		}

		#endregion

		#region Implementation of IGearboxInfo

		public IShiftStrategy Strategy => throw new VectoException("No Gearbox available.");

		public GearboxType GearboxType => GearboxType.DrivingCycle;

		public GearshiftPosition Gear => new GearshiftPosition(0);

		public bool TCLocked => true;

		public MeterPerSecond StartSpeed => throw new VectoException("No Gearbox available. StartSpeed unknown.");

		public MeterPerSquareSecond StartAcceleration => throw new VectoException("No Gearbox available. StartAcceleration unknown.");

		public Watt GearboxLoss()
		{
			throw new VectoException("No Gearbox available.");
		}

		public Second LastShift => throw new VectoException("No Gearbox available.");

		public Second LastUpshift => throw new VectoException("No Gearbox available.");

		public Second LastDownshift => throw new VectoException("No Gearbox available.");

		public GearData GetGearData(uint gear)
		{
			throw new VectoException("No Gearbox available.");
		}

		public GearshiftPosition NextGear => throw new VectoException("No Gearbox available.");

		public Second TractionInterruption => throw new NotImplementedException();

		public uint NumGears => throw new NotImplementedException();
		public bool Disengaged { get; }

		public bool DisengageGearbox => throw new VectoException("No Gearbox available.");

		public bool GearEngaged(Second absTime)
		{
			return true;
		}

		public bool RequestAfterGearshift { get; set; }

		#endregion

		protected override bool DoUpdateFrom(object other) => false;
	}

	internal class ZeroMileageCounter : VectoSimulationComponent, IMileageCounter
	{
		public ZeroMileageCounter(IVehicleContainer container) : base(container)
		{

		}

		#region Overrides of VectoSimulationComponent

		protected override void DoWriteModalResults(Second time, Second simulationInterval, IModalDataContainer container)
		{

		}

		protected override void DoCommitSimulationStep(Second time, Second simulationInterval)
		{

		}

		#endregion

		#region Implementation of IMileageCounter

		public Meter Distance => 0.SI<Meter>();

		#endregion

		protected override bool DoUpdateFrom(object other) => false;
	}

	public class DummyVehicleInfo : VectoSimulationComponent, IVehicleInfo
	{
		public DummyVehicleInfo(IVehicleContainer container) : base(container)
		{

		}

		#region Overrides of VectoSimulationComponent

		protected override void DoWriteModalResults(Second time, Second simulationInterval, IModalDataContainer container)
		{

		}

		protected override void DoCommitSimulationStep(Second time, Second simulationInterval)
		{

		}

		#endregion

		#region Implementation of IVehicleInfo

		public MeterPerSecond VehicleSpeed => 0.SI<MeterPerSecond>();

		public bool VehicleStopped => false;

		public Kilogram VehicleMass => throw new NotImplementedException();

		public Kilogram VehicleLoading => throw new NotImplementedException();

		public Kilogram TotalMass => throw new NotImplementedException();

		public CubicMeter CargoVolume => throw new NotImplementedException();

		public Newton AirDragResistance(MeterPerSecond previousVelocity, MeterPerSecond nextVelocity)
		{
			throw new NotImplementedException();
		}

		public Newton RollingResistance(Radian gradient)
		{
			throw new NotImplementedException();
		}

		public Newton SlopeResistance(Radian gradient)
		{
			throw new NotImplementedException();
		}

		public MeterPerSecond MaxVehicleSpeed => throw new NotImplementedException();

		#endregion

		protected override bool DoUpdateFrom(object other) => false;
	}

	public class TimeRunHybridComponents
    {
		internal MeasuredSpeedDrivingCycle Cycle { get; set; }

		public StopStartCombustionEngine Engine { get; set; }

		public IGearbox Gearbox { get; set; }

		public SwitchableClutch Clutch { get; set; }

		public IIdleController IdleController { get; set; }

		public IElectricMotor ElectricMotor	{ get; set; }

		public IHybridController HybridController { get; set; }
	}
}