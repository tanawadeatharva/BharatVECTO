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
using TUGraz.VectoCore.Exceptions;
using TUGraz.VectoCore.Models.Connector.Ports;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.OutputData;

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
			_modData = modData;
			_sumWriter = sumWriter;
		}

		public VehicleContainer Build(VectoRunData data)
		{
			if (data.IsEngineOnly) {
				return BuildEngineOnly(data);
			}
			if (data.Cycle.CycleType == CycleType.PWheel) {
				return BuildPWheel(data);
			}
			if (data.Cycle.CycleType == CycleType.MeasuredSpeed) {
				return BuildMeasuredSpeed(data);
			}


			return BuildFullPowertrain(data);
		}

		private VehicleContainer BuildEngineOnly(VectoRunData data)
		{
			var container = new VehicleContainer(_modData, _sumWriter, ExecutionMode.EngineOnly);
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
			var container = new VehicleContainer(_modData, _sumWriter, ExecutionMode.Engineering);
			data.GearboxData.Type = GearboxType.PWheel;
			var gearbox = GetGearbox(container, data.GearboxData);

			var cycle = new PWheelCycle(container, data.Cycle, data.AxleGearData.AxleGear.Ratio, (Gearbox)gearbox);

			var tmp = AddComponent(cycle, new AxleGear(container, data.AxleGearData));


			switch (data.Retarder.Type) {
				case RetarderData.RetarderType.Primary:
					tmp = AddComponent(tmp, new Retarder(container, data.Retarder.LossMap, data.Retarder.Ratio));
					tmp = AddComponent(tmp, gearbox);
					break;
				case RetarderData.RetarderType.Secondary:
					tmp = AddComponent(tmp, gearbox);
					tmp = AddComponent(tmp, new Retarder(container, data.Retarder.LossMap, data.Retarder.Ratio));
					break;
				case RetarderData.RetarderType.None:
					tmp = AddComponent(tmp, new DummyRetarder(container));
					tmp = AddComponent(tmp, gearbox);
					break;
				case RetarderData.RetarderType.LossesIncludedInTransmission:
					tmp = AddComponent(tmp, new DummyRetarder(container));
					tmp = AddComponent(tmp, gearbox);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			var engine = new CombustionEngine(container, data.EngineData, pt1Disabled: true);
			var clutch = new PWheelClutch(container, engine.IdleController);

			// gearbox --> clutch
			tmp = AddComponent(tmp, clutch);

			// clutch --> engine

			AddComponent(tmp, engine);

			// engine --> aux
			if (data.Aux != null) {
				var aux = new EngineAuxiliary(container);
				foreach (var auxData in data.Aux) {
					switch (auxData.DemandType) {
						case AuxiliaryDemandType.Constant:
							aux.AddConstant(auxData.ID, auxData.PowerDemand);
							break;
						case AuxiliaryDemandType.Direct:
							aux.AddDirect();
							break;
						case AuxiliaryDemandType.Mapping:
							aux.AddMapping(auxData.ID, auxData.Data);
							break;
					}
					_modData.AddAuxiliary(auxData.ID);
				}
				engine.Connect(aux.Port());
			}
			engine.IdleController.RequestPort = clutch.IdleControlPort;

			return container;
		}


		private VehicleContainer BuildMeasuredSpeed(VectoRunData data)
		{
			var container = new VehicleContainer(_modData, _sumWriter, ExecutionMode.Engineering) {
				RunData = data,
				ExecutionMode = ExecutionMode.Engineering
			};
			data.GearboxData.Type = GearboxType.PWheel;
			var gearbox = GetGearbox(container, data.GearboxData);

			var cycle = new MeasuredSpeedCycle(container, data.Cycle, (Gearbox)gearbox);
			var vehicle = AddComponent(cycle, new Vehicle(container, data.VehicleData));
			var wheels = AddComponent(vehicle,
				new Wheels(container, data.VehicleData.DynamicTyreRadius, data.VehicleData.WheelsInertia));
			var brakes = AddComponent(wheels, new Brakes(container));
			var tmp = AddComponent(brakes, new AxleGear(container, data.AxleGearData));


			switch (data.Retarder.Type) {
				case RetarderData.RetarderType.Primary:
					tmp = AddComponent(tmp, new Retarder(container, data.Retarder.LossMap, data.Retarder.Ratio));
					tmp = AddComponent(tmp, gearbox);
					break;
				case RetarderData.RetarderType.Secondary:
					tmp = AddComponent(tmp, gearbox);
					tmp = AddComponent(tmp, new Retarder(container, data.Retarder.LossMap, data.Retarder.Ratio));
					break;
				case RetarderData.RetarderType.None:
					tmp = AddComponent(tmp, gearbox);
					tmp = AddComponent(tmp, new DummyRetarder(container));
					break;
				case RetarderData.RetarderType.LossesIncludedInTransmission:
					tmp = AddComponent(tmp, gearbox);
					tmp = AddComponent(tmp, new DummyRetarder(container));
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			var engine = new CombustionEngine(container, data.EngineData);
			var clutch = new Clutch(container, data.EngineData, engine.IdleController);

			// gearbox --> clutch
			tmp = AddComponent(tmp, clutch);

			// clutch --> direct aux --> ... --> aux_XXX --> directAux
			if (data.Aux != null) {
				var aux = new EngineAuxiliary(container);
				foreach (var auxData in data.Aux) {
					switch (auxData.DemandType) {
						case AuxiliaryDemandType.Constant:
							aux.AddConstant(auxData.ID, auxData.PowerDemand);
							break;
						case AuxiliaryDemandType.Direct:
							aux.AddDirect();
							break;
						case AuxiliaryDemandType.Mapping:
							aux.AddMapping(auxData.ID, auxData.Data);
							break;
					}
					_modData.AddAuxiliary(auxData.ID);
				}
				engine.Connect(aux.Port());
			}
			// connect aux --> engine
			AddComponent(tmp, engine);

			engine.IdleController.RequestPort = clutch.IdleControlPort;

			return container;
		}


		private VehicleContainer BuildFullPowertrain(VectoRunData data)
		{
			var container = new VehicleContainer(_modData, _sumWriter, ExecutionMode.EngineOnly) { RunData = data };
			IDrivingCycle cycle;
			switch (data.Cycle.CycleType) {
				case CycleType.DistanceBased:
					cycle = new DistanceBasedDrivingCycle(container, data.Cycle);
					break;
				case CycleType.TimeBased:
					cycle = new TimeBasedDrivingCycle(container, data.Cycle);
					break;
				default:
					throw new VectoSimulationException("Powertrain Builder cannot build FullPowertrain for Cycle Type {0}",
						data.Cycle.CycleType);
			}
			// cycle --> driver --> vehicle --> wheels --> axleGear --> retarder --> gearBox
			var driver = AddComponent(cycle, new Driver(container, data.DriverData, new DefaultDriverStrategy()));
			var vehicle = AddComponent(driver, new Vehicle(container, data.VehicleData));
			var wheels = AddComponent(vehicle,
				new Wheels(container, data.VehicleData.DynamicTyreRadius, data.VehicleData.WheelsInertia));
			var brakes = AddComponent(wheels, new Brakes(container));
			var tmp = AddComponent(brakes, new AxleGear(container, data.AxleGearData));

			switch (data.Retarder.Type) {
				case RetarderData.RetarderType.Primary:
					tmp = AddComponent(tmp, new Retarder(container, data.Retarder.LossMap, data.Retarder.Ratio));
					tmp = AddComponent(tmp, GetGearbox(container, data.GearboxData));
					break;
				case RetarderData.RetarderType.Secondary:
					tmp = AddComponent(tmp, GetGearbox(container, data.GearboxData));
					tmp = AddComponent(tmp, new Retarder(container, data.Retarder.LossMap, data.Retarder.Ratio));
					break;
				case RetarderData.RetarderType.None:
					tmp = AddComponent(tmp, GetGearbox(container, data.GearboxData));
					tmp = AddComponent(tmp, new DummyRetarder(container));
					break;
				case RetarderData.RetarderType.LossesIncludedInTransmission:
					tmp = AddComponent(tmp, GetGearbox(container, data.GearboxData));
					tmp = AddComponent(tmp, new DummyRetarder(container));
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			var engine = new CombustionEngine(container, data.EngineData);
			var clutch = new Clutch(container, data.EngineData, engine.IdleController);

			// gearbox --> clutch
			tmp = AddComponent(tmp, clutch);

			// clutch --> engine			
			AddComponent(tmp, engine);

			// connect aux --> engine
			if (data.Aux != null) {
				var aux = new EngineAuxiliary(container);
				foreach (var auxData in data.Aux) {
					switch (auxData.DemandType) {
						case AuxiliaryDemandType.Constant:
							aux.AddConstant(auxData.ID, auxData.PowerDemand);
							break;
						case AuxiliaryDemandType.Direct:
							aux.AddDirect();
							break;
						case AuxiliaryDemandType.Mapping:
							aux.AddMapping(auxData.ID, auxData.Data);
							break;
					}
					_modData.AddAuxiliary(auxData.ID);
				}
				engine.Connect(aux.Port());
			}

			engine.IdleController.RequestPort = clutch.IdleControlPort;

			return container;
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
				case GearboxType.AT:
					strategy = new ATShiftStrategy(data, container);
					break;
				case GearboxType.Custom:
					strategy = new CustomShiftStrategy(data, container);
					break;
				case GearboxType.PWheel:
					strategy = new PWheelShiftStrategy(data, container);
					break;
				default:
					throw new VectoSimulationException("Unknown Gearbox Type: {0}", data.Type);
			}
			return new Gearbox(container, data, strategy);
		}

		private static IDriver AddComponent(IDrivingCycleInProvider prev, IDriver next)
		{
			prev.InPort().Connect(next.OutPort());
			return next;
		}

		private static IVehicle AddComponent(IDriverDemandInProvider prev, IVehicle next)
		{
			prev.InPort().Connect(next.OutPort());
			return next;
		}

		private static IWheels AddComponent(IFvInProvider prev, IWheels next)
		{
			prev.InPort().Connect(next.OutPort());
			return next;
		}

		private static IPowerTrainComponent AddComponent(ITnInProvider prev, IPowerTrainComponent next)
		{
			prev.InPort().Connect(next.OutPort());
			return next;
		}

		private static IPowerTrainComponent AddComponent(IPowerTrainComponent prev, IPowerTrainComponent next)
		{
			prev.InPort().Connect(next.OutPort());
			return next;
		}

		private static void AddComponent(ITnInProvider prev, ITnOutProvider next)
		{
			prev.InPort().Connect(next.OutPort());
		}
	}
}