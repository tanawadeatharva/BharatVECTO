/*
* This file is part of VECTO.
*
* Copyright © 2012-2016 European Union
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
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.Simulation.Impl
{
	/// <summary>
	/// Provides Methods to build a simulator with a powertrain step by step.
	/// </summary>
	public class PowertrainBuilder
	{
		private readonly IModalDataContainer _modData;
		private readonly WriteSumData _sumWriter;

		public PowertrainBuilder(IModalDataContainer modData, WriteSumData sumWriter = null)
		{
			if (modData == null) {
				throw new VectoException("Modal Data Container can't be null");
			}
			_modData = modData;
			_sumWriter = sumWriter;
		}

		public VehicleContainer Build(VectoRunData data)
		{
			switch (data.Cycle.CycleType) {
				case CycleType.EngineOnly:
					return BuildEngineOnly(data);
				case CycleType.PWheel:
					return BuildPWheel(data);
				case CycleType.MeasuredSpeed:
					return BuildMeasuredSpeed(data);
				case CycleType.MeasuredSpeedGear:
					return BuildMeasuredSpeedGear(data);
			}
			return BuildFullPowertrain(data);
		}

		private VehicleContainer BuildEngineOnly(VectoRunData data)
		{
			if (data.Cycle.CycleType != CycleType.EngineOnly) {
				throw new VectoException("CycleType must be EngineOnly.");
			}

			var container = new VehicleContainer(ExecutionMode.Engineering, _modData, _sumWriter) { RunData = data };
			var cycle = new PowertrainDrivingCycle(container, data.Cycle);

			var directAux = new EngineAuxiliary(container);
			directAux.AddDirect();

			var engine = new EngineOnlyCombustionEngine(container, data.EngineData);
			engine.Connect(directAux.Port());

			cycle.InPort().Connect(engine.OutPort());
			return container;
		}

		private VehicleContainer BuildPWheel(VectoRunData data)
		{
			if (data.Cycle.CycleType != CycleType.PWheel) {
				throw new VectoException("CycleType must be PWheel.");
			}

			var container = new VehicleContainer(ExecutionMode.Engineering, _modData, _sumWriter) { RunData = data };
			var gearbox = new CycleGearbox(container, data.GearboxData);

			// PWheelCycle --> AxleGear --> CycleClutch --> Engine <-- Aux
			new PWheelCycle(container, data.Cycle, data.AxleGearData.AxleGear.Ratio,
				gearbox.ModelData.Gears.ToDictionary(g => g.Key, g => g.Value.Ratio))
				.AddComponent(new AxleGear(container, data.AxleGearData))
				.AddComponent(data.AngularGearData != null ? new AngularGear(container, data.AngularGearData) : null)
				.AddRetarderAndGearbox(data.Retarder, gearbox, container)
				.AddComponent(new Clutch(container, data.EngineData))
				.AddComponent(new CombustionEngine(container, data.EngineData, pt1Disabled: true))
				.AddAuxiliaries(container, data);

			return container;
		}

		private VehicleContainer BuildMeasuredSpeed(VectoRunData data)
		{
			if (data.Cycle.CycleType != CycleType.MeasuredSpeed) {
				throw new VectoException("CycleType must be MeasuredSpeed.");
			}

			var container = new VehicleContainer(ExecutionMode.Engineering, _modData, _sumWriter) { RunData = data };

			// MeasuredSpeedDrivingCycle --> vehicle --> wheels --> brakes 
			// --> axleGear --> (retarder) --> GearBox --> (retarder) --> Clutch --> engine <-- Aux
			var powertrain = new MeasuredSpeedDrivingCycle(container, data.Cycle)
				.AddComponent(new Vehicle(container, data.VehicleData))
				.AddComponent(new Wheels(container, data.VehicleData.DynamicTyreRadius, data.VehicleData.WheelsInertia))
				.AddComponent(new Brakes(container))
				.AddComponent(new AxleGear(container, data.AxleGearData))
				.AddComponent(data.AngularGearData != null ? new AngularGear(container, data.AngularGearData) : null)
				.AddRetarderAndGearbox(data.Retarder, GetGearbox(container, data.GearboxData), container);
			if (data.GearboxData.Type.ManualTransmission()) {
				powertrain = powertrain.AddComponent(new Clutch(container, data.EngineData));
			}
			powertrain.AddComponent(new CombustionEngine(container, data.EngineData))
				.AddAuxiliaries(container, data);
			_modData.HasTorqueConverter = data.GearboxData.Type.AutomaticTransmission();

			return container;
		}

		private VehicleContainer BuildMeasuredSpeedGear(VectoRunData data)
		{
			if (data.Cycle.CycleType != CycleType.MeasuredSpeedGear) {
				throw new VectoException("CycleType must be MeasuredSpeed with Gear.");
			}

			var container = new VehicleContainer(ExecutionMode.Engineering, _modData, _sumWriter) { RunData = data };

			// MeasuredSpeedDrivingCycle --> vehicle --> wheels --> brakes 
			// --> axleGear --> (retarder) --> CycleGearBox --> (retarder) --> CycleClutch --> engine <-- Aux
			var powertrain = new MeasuredSpeedDrivingCycle(container, data.Cycle)
				.AddComponent(new Vehicle(container, data.VehicleData))
				.AddComponent(new Wheels(container, data.VehicleData.DynamicTyreRadius, data.VehicleData.WheelsInertia))
				.AddComponent(new Brakes(container))
				.AddComponent(new AxleGear(container, data.AxleGearData))
				.AddComponent(data.AngularGearData != null ? new AngularGear(container, data.AngularGearData) : null)
				.AddRetarderAndGearbox(data.Retarder, new CycleGearbox(container, data.GearboxData), container);
			if (data.GearboxData.Type.ManualTransmission()) {
				powertrain = powertrain.AddComponent(new Clutch(container, data.EngineData));
			}
			powertrain.AddComponent(new CombustionEngine(container, data.EngineData))
				.AddAuxiliaries(container, data);

			return container;
		}

		private VehicleContainer BuildFullPowertrain(VectoRunData data)
		{
			if (data.Cycle.CycleType != CycleType.DistanceBased) {
				throw new VectoException("CycleType must be DistanceBased");
			}

			var container = new VehicleContainer(data.ExecutionMode, _modData, _sumWriter) { RunData = data };

			// DistanceBasedDrivingCycle --> driver --> vehicle --> wheels 
			// --> axleGear --> (retarder) --> gearBox --> (retarder) --> clutch --> engine <-- Aux
			var powertrain = new DistanceBasedDrivingCycle(container, data.Cycle)
				.AddComponent(new Driver(container, data.DriverData, new DefaultDriverStrategy()))
				.AddComponent(new Vehicle(container, data.VehicleData))
				.AddComponent(new Wheels(container, data.VehicleData.DynamicTyreRadius, data.VehicleData.WheelsInertia))
				.AddComponent(new Brakes(container))
				.AddComponent(new AxleGear(container, data.AxleGearData))
				.AddComponent(data.AngularGearData != null ? new AngularGear(container, data.AngularGearData) : null)
				.AddRetarderAndGearbox(data.Retarder, GetGearbox(container, data.GearboxData), container);
			if (data.GearboxData.Type.ManualTransmission()) {
				powertrain = powertrain.AddComponent(new Clutch(container, data.EngineData));
			}
			powertrain.AddComponent(new CombustionEngine(container, data.EngineData))
				.AddAuxiliaries(container, data);

			_modData.HasTorqueConverter = data.GearboxData.Type.AutomaticTransmission();

			return container;
		}

		internal static IEngineAuxInProvider CreateAdvancedAuxiliaries(VectoRunData data, IVehicleContainer container)
		{
			var conventionalAux = CreateAuxiliaries(data, container);
			var busAux = new BusAuxiliariesAdapter(container, data.AdvancedAux.AdvancedAuxiliaryFilePath, data.Cycle.Name,
				data.VehicleData.TotalVehicleWeight(), data.EngineData.ConsumptionMap, data.EngineData.IdleSpeed, conventionalAux);
			return busAux;
		}

		internal static EngineAuxiliary CreateAuxiliaries(VectoRunData data, IVehicleContainer container)
		{
			var aux = new EngineAuxiliary(container);
			foreach (var auxData in data.Aux) {
				// id's in upper case
				var id = auxData.ID.ToUpper();

				switch (auxData.DemandType) {
					case AuxiliaryDemandType.Constant:
						aux.AddConstant(id, auxData.PowerDemand);
						break;
					case AuxiliaryDemandType.Direct:
						aux.AddDirect();
						break;
					case AuxiliaryDemandType.Mapping:
						aux.AddMapping(id, auxData.Data);
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
				container.ModalData.AddAuxiliary(id);
			}
			return aux;
		}

		private static IGearbox GetGearbox(IVehicleContainer container, GearboxData data)
		{
			IShiftStrategy strategy;
			switch (data.Type) {
				case GearboxType.AMT:
					strategy = new AMTShiftStrategy(data, container);
					break;
				case GearboxType.MT:
					strategy = new MTShiftStrategy(data, container);
					break;
				case GearboxType.ATPowerSplit:
				case GearboxType.ATSerial:
					strategy = new ATShiftStrategy(data, container);
					return new ATGearbox(container, data, strategy);
				default:
					throw new VectoSimulationException("Unknown Gearbox Type: {0}", data.Type);
			}
			return new Gearbox(container, data, strategy);
		}
	}
}