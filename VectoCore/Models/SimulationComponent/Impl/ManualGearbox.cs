/*
* Copyright 2015, 2016 Graz University of Technology,
* Institute of Internal Combustion Engines and Thermodynamics,
* Institute of Technical Informatics
*
* Licensed under the EUPL (the "Licence");
* You may not use this work except in compliance with the Licence.
* You may obtain a copy of the Licence at:
*
* http://ec.europa.eu/idabc/eupl
*
* Unless required by applicable law or agreed to in writing, software 
* distributed under the Licence is distributed on an "AS IS" basis,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the Licence for the specific language governing permissions and 
* limitations under the Licence.
*/

using System;
using System.Diagnostics;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.Connector.Ports;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
	public class ManualGearbox : VectoSimulationComponent, IGearbox, ITnOutPort, ITnInPort, IClutchInfo
	{
		private readonly GearboxData _data;
		private ITnOutPort NextComponent { get; set; }
		private PerSecond PreviousInAngularSpeed { get; set; }
		private Watt PowerLossInertia { get; set; }
		private Watt PowerLoss { get; set; }

		private Func<uint> GetGear { get; set; }

		public uint Gear
		{
			get { return GetGear(); }
		}

		public MeterPerSecond StartSpeed
		{
			get { return _data.StartSpeed; }
		}

		public MeterPerSquareSecond StartAcceleration
		{
			get { return _data.StartAcceleration; }
		}

		public FullLoadCurve GearFullLoadCurve
		{
			get { return Gear == 0 ? null : _data.Gears[Gear].FullLoadCurve; }
		}

		public ManualGearbox(IVehicleContainer container, GearboxData gearboxData, Func<uint> getGear)
			: base(container)
		{
			_data = gearboxData;
			GetGear = getGear;
		}

		public IResponse Initialize(NewtonMeter outTorque, PerSecond outAngularVelocity)
		{
			var dt = Constants.SimulationSettings.TargetTimeInterval;
			PowerLoss = null;

			if (_data == null || _data.Gears == null) {
				var r = NextComponent.Initialize(outTorque, outAngularVelocity);
				if (r is ResponseSuccess) {
					PreviousInAngularSpeed = outAngularVelocity;
				}
				return r;
			}

			var inAngularVelocity = outAngularVelocity * _data.Gears[Gear].Ratio;
			var inTorque = _data.Gears[Gear].LossMap.GetInTorque(inAngularVelocity, outTorque);

			var torqueLossInertia = outAngularVelocity.IsEqual(0)
				? 0.SI<NewtonMeter>()
				: Formulas.InertiaPower(inAngularVelocity, PreviousInAngularSpeed, _data.Inertia, dt) / inAngularVelocity;

			inTorque += torqueLossInertia;

			var response = NextComponent.Initialize(inTorque, inAngularVelocity);
			return response;
		}

		/// <summary>
		/// Requests the Gearbox to deliver torque and angularVelocity
		/// </summary>
		/// <returns>
		/// <list type="bullet">
		/// <item><description>ResponseDryRun</description></item>
		/// <item><description>ResponseOverload</description></item>
		/// <item><description>ResponseGearshift</description></item>
		/// </list>
		/// </returns>
		public IResponse Request(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity,
			bool dryRun)
		{
			Log.Debug("Gearbox Power Request: torque: {0}, angularVelocity: {1}", outTorque, outAngularVelocity);

			if (Gear == 0) {
				var disengagedResponse = NextComponent.Request(absTime, dt, 0.SI<NewtonMeter>(), null);
				disengagedResponse.GearboxPowerRequest = outTorque * outAngularVelocity;
				PreviousInAngularSpeed = DataBus.EngineIdleSpeed;
				return disengagedResponse;
			}

			var inEngineSpeed = outAngularVelocity * _data.Gears[Gear].Ratio;
			var inTorque = _data.Gears[Gear].LossMap.GetInTorque(inEngineSpeed, outTorque);

			PowerLoss = inTorque * inEngineSpeed - outTorque * outAngularVelocity;

			if (!inEngineSpeed.IsEqual(0)) {
				PowerLossInertia = Formulas.InertiaPower(inEngineSpeed, PreviousInAngularSpeed, _data.Inertia, dt);
				inTorque += PowerLossInertia / inEngineSpeed;
			} else {
				PowerLossInertia = 0.SI<Watt>();
			}

			var response = NextComponent.Request(absTime, dt, inTorque, inEngineSpeed);
			response.GearboxPowerRequest = outTorque * outAngularVelocity;

			PreviousInAngularSpeed = inEngineSpeed;
			return response;
		}

		protected override void DoWriteModalResults(IModalDataContainer container)
		{
			container[ModalResultField.Gear] = Gear;
			container[ModalResultField.P_gbx_loss] = PowerLoss;
			container[ModalResultField.P_gbx_inertia] = PowerLossInertia;
		}


		protected override void DoCommitSimulationStep()
		{
			if (_data.Gears[Gear].LossMap.Extrapolated) {
				Log.Warn("Gear {0} LossMap data was extrapolated: range for loss map is not sufficient.", Gear);
			}

			PowerLoss = null;
			PowerLossInertia = null;
		}

		[DebuggerHidden]
		public ITnInPort InPort()
		{
			return this;
		}

		[DebuggerHidden]
		public ITnOutPort OutPort()
		{
			return this;
		}

		void ITnInPort.Connect(ITnOutPort other)
		{
			NextComponent = other;
		}

		public bool ClutchClosed(Second absTime)
		{
			return true;
		}
	}
}