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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Connector.Ports;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.SimulationComponent;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.Simulation.Impl
{
	public class VehicleContainer : LoggingObject, IVehicleContainer
	{
		internal readonly SortedList<int, VectoSimulationComponent> Components =
			new SortedList<int, VectoSimulationComponent>();

		internal IEngineInfo Engine;
		internal IGearboxInfo Gearbox;
		internal IVehicleInfo Vehicle;
		internal IBrakes Brakes;
		internal IDriverInfo Driver;

		internal IMileageCounter MilageCounter;

		internal IClutchInfo Clutch;

		internal IDrivingCycleInfo DrivingCycle;

		internal IRoadLookAhead Road;

		internal ISimulationOutPort Cycle;

		internal IModalDataContainer ModData;
		internal WriteSumData WriteSumData;

		#region IGearCockpit

		public uint Gear
		{
			[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design",
				"CA1065:DoNotRaiseExceptionsInUnexpectedLocations")]
			get
			{
				if (Gearbox == null) {
					throw new VectoException("no gearbox available!");
				}
				return Gearbox.Gear;
			}
		}

		public MeterPerSecond StartSpeed
		{
			[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design",
				"CA1065:DoNotRaiseExceptionsInUnexpectedLocations")]
			get
			{
				if (Gearbox == null) {
					throw new VectoException("No Gearbox available. StartSpeed unkown");
				}
				return Gearbox.StartSpeed;
			}
		}

		public MeterPerSquareSecond StartAcceleration
		{
			[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design",
				"CA1065:DoNotRaiseExceptionsInUnexpectedLocations")]
			get
			{
				if (Gearbox == null) {
					throw new VectoException("No Gearbox available. StartAcceleration unknown.");
				}
				return Gearbox.StartAcceleration;
			}
		}

		public FullLoadCurve GearFullLoadCurve
		{
			get { return Gearbox != null ? Gearbox.GearFullLoadCurve : null; }
		}

		#endregion

		#region IEngineCockpit

		public PerSecond EngineSpeed
		{
			[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design",
				"CA1065:DoNotRaiseExceptionsInUnexpectedLocations")]
			get
			{
				if (Engine == null) {
					throw new VectoException("no engine available!");
				}
				return Engine.EngineSpeed;
			}
		}

		public Watt EngineStationaryFullPower(PerSecond angularSpeed)
		{
			return Engine.EngineStationaryFullPower(angularSpeed);
		}

		public Watt EngineDragPower(PerSecond angularSpeed)
		{
			return Engine.EngineDragPower(angularSpeed);
		}

		public PerSecond EngineIdleSpeed
		{
			get { return Engine.EngineIdleSpeed; }
		}

		public PerSecond EngineRatedSpeed
		{
			get { return Engine.EngineRatedSpeed; }
		}

		#endregion

		#region IVehicleCockpit

		public MeterPerSecond VehicleSpeed
		{
			get { return Vehicle != null ? Vehicle.VehicleSpeed : 0.SI<MeterPerSecond>(); }
		}

		public Kilogram VehicleMass
		{
			get { return Vehicle != null ? Vehicle.VehicleMass : 0.SI<Kilogram>(); }
		}

		public Kilogram VehicleLoading
		{
			get { return Vehicle != null ? Vehicle.VehicleLoading : 0.SI<Kilogram>(); }
		}

		public Kilogram TotalMass
		{
			get { return Vehicle != null ? Vehicle.TotalMass : 0.SI<Kilogram>(); }
		}

		#endregion

		public VehicleContainer(ExecutionMode executionMode, IModalDataContainer modData = null,
			WriteSumData writeSumData = null)
		{
			ModData = modData;
			WriteSumData = writeSumData ?? delegate { };
			ExecutionMode = executionMode;
		}

		#region IVehicleContainer

		public IModalDataContainer ModalData
		{
			get { return ModData; }
		}

		public ISimulationOutPort GetCycleOutPort()
		{
			return Cycle;
		}

		public virtual void AddComponent(VectoSimulationComponent component)
		{
			var commitPriority = 0;

			component.Switch()
				.If<IEngineInfo>(c => {
					Engine = c;
					commitPriority = 2;
				})
				.If<IDriverInfo>(c => Driver = c)
				.If<IGearboxInfo>(c => {
					Gearbox = c;
					commitPriority = 4;
				})
				.If<IVehicleInfo>(c => {
					Vehicle = c;
					commitPriority = 5;
				})
				.If<ISimulationOutPort>(c => Cycle = c)
				.If<IMileageCounter>(c => MilageCounter = c)
				.If<IBrakes>(c => Brakes = c)
				.If<IRoadLookAhead>(c => Road = c)
				.If<IClutchInfo>(c => Clutch = c)
				.If<IDrivingCycleInfo>(c => {
					DrivingCycle = c;
					commitPriority = 3;
				});

			Components.Add(-commitPriority, component);
		}


		public void CommitSimulationStep(Second time, Second simulationInterval)
		{
			Log.Info("VehicleContainer committing simulation. time: {0}, dist: {1}, speed: {2}", time,
				ExecutionMode == ExecutionMode.EngineOnly ? null : Distance, VehicleSpeed);

			foreach (var component in Components) {
				component.Value.CommitSimulationStep(ModData);
			}

			if (ModData != null) {
				ModData[ModalResultField.time] = time + simulationInterval / 2;
				ModData[ModalResultField.simulationInterval] = simulationInterval;
				ModData.CommitSimulationStep();
			}
		}

		public void FinishSimulation()
		{
			Log.Info("VehicleContainer finishing simulation.");
			ModData.Finish(RunStatus);

			WriteSumData(ModData, VehicleMass, VehicleLoading);
		}

		public VectoRun.Status RunStatus { get; set; }

		#endregion

		public IReadOnlyCollection<VectoSimulationComponent> SimulationComponents()
		{
			return new ReadOnlyCollection<VectoSimulationComponent>(Components.Select(x => x.Value).ToList());
		}

		public Meter Distance
		{
			get
			{
				if (MilageCounter == null) {
					Log.Warn("No MileageCounter in VehicleContainer. Distance cannot be measured.");
					return 0.SI<Meter>();
				}
				return MilageCounter.Distance;
			}
		}

		public IReadOnlyList<DrivingCycleData.DrivingCycleEntry> LookAhead(Meter lookaheadDistance)
		{
			return Road.LookAhead(lookaheadDistance);
		}

		public IReadOnlyList<DrivingCycleData.DrivingCycleEntry> LookAhead(Second time)
		{
			return Road.LookAhead(time);
		}

		public Watt BrakePower
		{
			get { return Brakes.BrakePower; }
			set { Brakes.BrakePower = value; }
		}

		public bool ClutchClosed(Second absTime)
		{
			if (Clutch == null) {
				Log.Warn("No Clutch in VehicleContainer. ClutchClosed set to constant true!");
				return true;
			}
			return Clutch.ClutchClosed(absTime);
		}

		public bool VehicleStopped
		{
			get { return Vehicle.VehicleStopped; }
		}

		public DrivingBehavior DriverBehavior
		{
			get { return Driver.DriverBehavior; }
		}

		public Meter CycleStartDistance
		{
			get { return Road == null ? 0.SI<Meter>() : Road.CycleStartDistance; }
		}

		public VectoRunData RunData { get; set; }
		public ExecutionMode ExecutionMode { get; set; }

		public CycleData CycleData
		{
			get { return DrivingCycle.CycleData; }
		}
	}
}