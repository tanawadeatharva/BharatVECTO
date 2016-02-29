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
		private PerSecond PreviousOutAngularSpeed { get; set; }
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
			var inTorque = outTorque / _data.Gears[Gear].Ratio;
			var torqueLoss = _data.Gears[Gear].LossMap.GetTorqueLoss(outAngularVelocity, outTorque);
			inTorque += torqueLoss;

			var torqueLossInertia = outAngularVelocity.IsEqual(0)
				? 0.SI<NewtonMeter>()
				: Formulas.InertiaPower(outAngularVelocity, PreviousOutAngularSpeed, _data.Inertia, dt) / inAngularVelocity;

			inTorque += torqueLossInertia;

			var response = NextComponent.Initialize(inTorque, inAngularVelocity);
			PreviousOutAngularSpeed = outAngularVelocity;
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
			var torqueLoss = _data.Gears[Gear].LossMap.GetTorqueLoss(outAngularVelocity, outTorque);
			var inTorque = outTorque / _data.Gears[Gear].Ratio + torqueLoss;

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