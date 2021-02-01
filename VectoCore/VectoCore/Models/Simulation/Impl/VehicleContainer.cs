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
using System.Collections.ObjectModel;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.Impl;
using TUGraz.VectoCore.Models.Connector.Ports;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.SimulationComponent;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.Simulation.Impl
{
	public class VehicleContainer : LoggingObject, IVehicleContainer, IPowertainInfo
	{
		private List<Tuple<int, VectoSimulationComponent>> _components =
			new List<Tuple<int, VectoSimulationComponent>>();

		public virtual IEngineInfo EngineInfo { get; protected internal set; }
		public virtual IEngineControl EngineCtl { get; protected set; }
		public virtual IGearboxInfo GearboxInfo { get; protected set; }
		public virtual IGearboxControl GearboxCtl { get; protected set; }
		public virtual IAxlegearInfo AxlegearInfo { get; protected set; }
		public virtual IAngledriveInfo AngledriveInfo { get; protected set; }
		public virtual IVehicleInfo VehicleInfo { get; protected set; }
		public virtual IBrakes Brakes { get; protected set; }
		public virtual IWheelsInfo WheelsInfo { get; protected set; }
		public virtual IDriverInfo DriverInfo { get; protected set; }
		public virtual IHybridController HybridController { get; protected set; }

		public virtual IMileageCounter MileageCounter { get; protected set; }

		public virtual IClutchInfo ClutchInfo { get; protected set; }

		public virtual IDrivingCycleInfo DrivingCycleInfo { get; protected set; }

		public IRESSInfo BatteryInfo { get; protected set; }
		public ITorqueConverterInfo TorqueConverterInfo { get; protected set; }

		public virtual ITorqueConverterControl TorqueConverterCtl { get; private set; }

		public virtual bool IsTestPowertrain
		{
			get { return false; }
		}

		internal ISimulationOutPort Cycle;

		internal IModalDataContainer ModData;

		internal WriteSumData WriteSumData;

		internal readonly IList<ISimulationPreprocessor> Preprocessors = new List<ISimulationPreprocessor>();

		internal readonly Dictionary<PowertrainPosition, IElectricMotorInfo> ElectricMotors =
			new Dictionary<PowertrainPosition, IElectricMotorInfo>();

		
		public VehicleContainer(ExecutionMode executionMode, IModalDataContainer modData = null,
			WriteSumData writeSumData = null)
		{
			ModData = modData;
			WriteSumData = writeSumData ?? delegate { };
			ExecutionMode = executionMode;
		}

		#region IVehicleContainer

		public virtual IModalDataContainer ModalData
		{
			get { return ModData; }
		}

		public virtual ISimulationOutPort GetCycleOutPort()
		{
			return Cycle;
		}


		public virtual Second AbsTime { get; set; }
		public IElectricMotorInfo ElectricMotorInfo(PowertrainPosition pos)
		{
			return ElectricMotors[pos];
		}



		public IPowertainInfo PowertrainInfo
		{
			get { return this; }
		}

		public IHybridControllerInfo HybridControllerInfo
		{
			get { return HybridController; }
		}

		public IHybridControllerCtl HybridControllerCtl
		{
			get { return HybridController; }
		}

		

		public virtual void AddComponent(VectoSimulationComponent component)
		{
			var commitPriority = 0;
			var ignoreComponent = false;
			component.Switch()
				.If<IEngineInfo>(c => {
					EngineInfo = c;
					commitPriority = 2;
					HasCombustionEngine = true;
				})
				.If<IEngineControl>(c => { EngineCtl = c; })
				.If<IDriverInfo>(c => DriverInfo = c)
				.If<IGearboxInfo>(c => {
					GearboxInfo = c;
					commitPriority = 4;
					HasGearbox = true;
				})
				.If<IGearboxControl>(c => GearboxCtl = c)
				.If<ITorqueConverterInfo>(c => TorqueConverterInfo = c)
				.If<ITorqueConverterControl>(c =>  TorqueConverterCtl = c)
				.If<IAxlegearInfo>(c => AxlegearInfo = c)
				.If<IAngledriveInfo>(c => AngledriveInfo = c)
				.If<IWheelsInfo>(c => WheelsInfo = c)
				.If<IVehicleInfo>(c => {
					VehicleInfo = c;
					commitPriority = 5;
				})
				.If<ISimulationOutPort>(c => Cycle = c)
				.If<IMileageCounter>(c => MileageCounter = c)
				.If<IBrakes>(c => Brakes = c)
				.If<IClutchInfo>(c => ClutchInfo = c)
				.If<IDrivingCycleInfo>(c => {
					DrivingCycleInfo = c;
					commitPriority = 6;
				})
				.If<PTOCycleController>(c => { commitPriority = 99; })
				.If<VTPCycle>(_ => { commitPriority = 0; })
				.If<IElectricMotorInfo>(c => {
					if (c.Position == PowertrainPosition.HybridPositionNotSet) {
						ignoreComponent = true;
						return;
					}
					if (ElectricMotors.ContainsKey(c.Position)) {
						throw new VectoException("There is already an electric machine at position {0}",
							c.Position);
					}

					ElectricMotors[c.Position] = c;
					HasElectricMotor = true;
				})
				.If<IHybridController>(c => { HybridController = c; })
				.If<IRESSInfo>(c => BatteryInfo = c);

			if (ignoreComponent) {
				return;
			}
			_components.Add(Tuple.Create(commitPriority, component));
			_components = _components.OrderBy(x => x.Item1).Reverse().ToList();
		}


		public virtual void CommitSimulationStep(Second time, Second simulationInterval)
		{
			Log.Info("VehicleContainer committing simulation. time: {0}, dist: {1}, speed: {2}", time,
				MileageCounter.Distance, VehicleInfo?.VehicleSpeed ?? 0.KMPHtoMeterPerSecond());


			foreach (var component in _components) {
				component.Item2.CommitSimulationStep(time, simulationInterval, ModData);
			}

			if (ModData != null) {
				ModData[ModalResultField.drivingBehavior] = DriverInfo.DriverBehavior;
				ModData[ModalResultField.time] = time + simulationInterval / 2;
				ModData[ModalResultField.simulationInterval] = simulationInterval;
				ModData.CommitSimulationStep();
			}
		}

		public virtual void FinishSimulationRun(Exception e = null)
		{
			Log.Info("VehicleContainer finishing simulation.");
			ModData?.Finish(RunStatus, e);

			WriteSumData(ModData);

			ModData?.FinishSimulation();
			DrivingCycleInfo?.FinishSimulation();
		}

		public virtual IEnumerable<ISimulationPreprocessor> GetPreprocessingRuns
		{
			get { return new ReadOnlyCollection<ISimulationPreprocessor>(Preprocessors); }
		}

		public virtual void AddPreprocessor(ISimulationPreprocessor simulationPreprocessor)
		{
			Preprocessors.Add(simulationPreprocessor);
		}

		public virtual void StartSimulationRun()
		{
			ModData?.Reset();
		}

		public virtual VectoRun.Status RunStatus { get; set; }

		#endregion

		public IReadOnlyCollection<VectoSimulationComponent> SimulationComponents()
		{
			return new ReadOnlyCollection<VectoSimulationComponent>(_components.Select(x => x.Item2).ToList());
		}

		public virtual bool HasElectricMotor { get; private set; }

		public PowertrainPosition[] ElectricMotorPositions
		{
			get { return ElectricMotors.Keys.ToArray(); }
		}

		public virtual bool HasCombustionEngine { get; private set; }

		public virtual bool HasGearbox { get; private set; }

		
		public virtual VectoRunData RunData { get; set; }
		public virtual ExecutionMode ExecutionMode { get; }


		
	}

	public class ExemptedRunContainer : VehicleContainer
	{
		private IMileageCounter _mileageCounter;
		private IVehicleInfo _vehicleInfo;

		private IGearboxInfo _gearboxInfo;

		public ExemptedRunContainer(
			ExecutionMode executionMode, IModalDataContainer modData = null, WriteSumData writeSumData = null) : base(
			executionMode, modData, writeSumData)
		{
			_mileageCounter = new ZeroMileageCounter(this);
			_vehicleInfo = new DummyVehicleInfo(this);
			_gearboxInfo = new EngineOnlyGearboxInfo(this);
		}

		#region Overrides of VehicleContainer

		public override IMileageCounter MileageCounter
		{
			get { return _mileageCounter; }
			
		}

		#endregion

		#region Overrides of VehicleContainer

		public override IVehicleInfo VehicleInfo
		{
			get { return _vehicleInfo; }
		}

		#endregion

		#region Overrides of VehicleContainer

		public override IGearboxInfo GearboxInfo
		{
			get { return _gearboxInfo; }
		}

		#endregion
	}
}