/*
* Copyright 2015 European Union
*
* Licensed under the EUPL (the "Licence");
* You may not use this work except in compliance with the Licence.
* You may obtain a copy of the Licence at:
*
* http://ec.europa.eu/idabc/eupl5
*
* Unless required by applicable law or agreed to in writing, software 
* distributed under the Licence is distributed on an "AS IS" basis,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the Licence for the specific language governing permissions and 
* limitations under the Licence.
*/

using System;
using System.Linq;
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
		private readonly VehicleContainer _container;
		private readonly IModalDataContainer _modData;


		public PowertrainBuilder(IModalDataContainer modData, WriteSumData sumWriter = null)
		{
			_modData = modData;
			_container = new VehicleContainer(modData, sumWriter);
		}

		public VehicleContainer Build(VectoRunData data)
		{
			if (data.IsEngineOnly) {
				return BuildEngineOnly(data);
			}
			if (data.Cycle.CycleType == CycleType.PWheel) {
				return BuildPWheel(data);
			}

			return BuildFullPowertrain(data);
		}

		private VehicleContainer BuildEngineOnly(VectoRunData data)
		{
			var cycle = new PowertrainDrivingCycle(_container, data.Cycle);

			var directAux = new Auxiliary(_container);
			directAux.AddDirect();

			cycle.InPort().Connect(directAux.OutPort());

			var engine = new EngineOnlyCombustionEngine(_container, data.EngineData);
			directAux.InPort().Connect(engine.OutPort());

			return _container;
		}

		private VehicleContainer BuildPWheel(VectoRunData data)
		{
			data.GearboxData.Type = GearboxType.PWheel;
			var gearbox = GetGearbox(_container, data.GearboxData);

			var cycle = new PWheelCycle(_container, data.Cycle, data.AxleGearData.Ratio, (Gearbox)gearbox);

			var tmp = AddComponent(cycle, new AxleGear(_container, data.AxleGearData));


			switch (data.Retarder.Type) {
				case RetarderData.RetarderType.Primary:
					tmp = AddComponent(tmp, new Retarder(_container, data.Retarder.LossMap));
					tmp = AddComponent(tmp, gearbox);
					break;
				case RetarderData.RetarderType.Secondary:
					tmp = AddComponent(tmp, gearbox);
					tmp = AddComponent(tmp, new Retarder(_container, data.Retarder.LossMap));
					break;
				case RetarderData.RetarderType.None:
					tmp = AddComponent(tmp, gearbox);
					break;
				case RetarderData.RetarderType.LossesIncludedInTransmission:
					tmp = AddComponent(tmp, gearbox);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			// pWheel: pt1 disabled!!
			var engine = new CombustionEngine(_container, data.EngineData, pt1Disabled: true);
			var clutch = new Clutch(_container, data.EngineData, engine.IdleController);

			// gearbox --> clutch
			tmp = AddComponent(tmp, clutch);

			// clutch --> direct aux --> ... --> aux_XXX --> directAux
			if (data.Aux != null) {
				var aux = new Auxiliary(_container);
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
				tmp = AddComponent(tmp, aux);
			}
			// connect aux --> engine
			AddComponent(tmp, engine);

			engine.IdleController.RequestPort = clutch.IdleControlPort;

			return _container;
		}


		private VehicleContainer BuildFullPowertrain(VectoRunData data)
		{
			_container.RunData = data;
			IDrivingCycle cycle;
			switch (data.Cycle.CycleType) {
				case CycleType.DistanceBased:
					cycle = new DistanceBasedDrivingCycle(_container, data.Cycle);
					break;
				case CycleType.TimeBased:
					cycle = new TimeBasedDrivingCycle(_container, data.Cycle);
					break;
				default:
					throw new VectoSimulationException("Unhandled Cycle Type");
			}
			// cycle --> driver --> vehicle --> wheels --> axleGear --> retarder --> gearBox
			var driver = AddComponent(cycle, new Driver(_container, data.DriverData, new DefaultDriverStrategy()));
			var vehicle = AddComponent(driver, new Vehicle(_container, data.VehicleData));
			var wheels = AddComponent(vehicle, new Wheels(_container, data.VehicleData.DynamicTyreRadius));
			var brakes = AddComponent(wheels, new Brakes(_container));
			var tmp = AddComponent(brakes, new AxleGear(_container, data.AxleGearData));

			switch (data.Retarder.Type) {
				case RetarderData.RetarderType.Primary:
					tmp = AddComponent(tmp, new Retarder(_container, data.Retarder.LossMap));
					tmp = AddComponent(tmp, GetGearbox(_container, data.GearboxData));
					break;
				case RetarderData.RetarderType.Secondary:
					tmp = AddComponent(tmp, GetGearbox(_container, data.GearboxData));
					tmp = AddComponent(tmp, new Retarder(_container, data.Retarder.LossMap));
					break;
				case RetarderData.RetarderType.None:
					tmp = AddComponent(tmp, GetGearbox(_container, data.GearboxData));
					break;
				case RetarderData.RetarderType.LossesIncludedInTransmission:
					tmp = AddComponent(tmp, GetGearbox(_container, data.GearboxData));
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			var engine = new CombustionEngine(_container, data.EngineData);
			var clutch = new Clutch(_container, data.EngineData, engine.IdleController);

			// gearbox --> clutch
			tmp = AddComponent(tmp, clutch);


			// clutch --> direct aux --> ... --> aux_XXX --> directAux
			if (data.Aux != null) {
				var aux = new Auxiliary(_container);
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
				tmp = AddComponent(tmp, aux);
			}
			// connect aux --> engine
			AddComponent(tmp, engine);

			engine.IdleController.RequestPort = clutch.IdleControlPort;

			return _container;
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