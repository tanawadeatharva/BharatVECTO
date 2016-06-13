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

using System.Diagnostics;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
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
	public class CycleGearbox : StatefulVectoSimulationComponent<Gearbox.GearboxState>, IGearbox, ITnOutPort, ITnInPort,
		IClutchInfo
	{
		/// <summary>
		/// The next port.
		/// </summary>
		protected ITnOutPort NextComponent;

		/// <summary>
		/// The data and settings for the gearbox.
		/// </summary>
		[ValidateObject] internal readonly GearboxData ModelData;

		public bool ClutchClosed(Second absTime)
		{
			return DataBus.CycleData.LeftSample.Gear != 0;
		}

		public CycleGearbox(IVehicleContainer container, GearboxData gearboxModelData)
			: base(container)
		{
			ModelData = gearboxModelData;
		}

		#region ITnInProvider

		public ITnInPort InPort()
		{
			return this;
		}

		#endregion

		#region ITnOutProvider

		[DebuggerHidden]
		public ITnOutPort OutPort()
		{
			return this;
		}

		#endregion

		#region IGearboxCockpit

		/// <summary>
		/// The current gear.
		/// </summary>
		public uint Gear { get; private set; }

		[DebuggerHidden]
		public MeterPerSecond StartSpeed
		{
			get { return ModelData.StartSpeed; }
		}

		[DebuggerHidden]
		public MeterPerSquareSecond StartAcceleration
		{
			get { return ModelData.StartAcceleration; }
		}

		public FullLoadCurve GearFullLoadCurve
		{
			get { return Gear == 0 ? null : ModelData.Gears[Gear].FullLoadCurve; }
		}

		public Watt GearboxLoss(PerSecond inAngularVelocity, NewtonMeter inTorque)
		{
			var outTorque = ModelData.Gears[Gear].LossMap.GetOutTorque(inAngularVelocity, inTorque, true);
			var torqueLoss = inTorque - outTorque * ModelData.Gears[Gear].Ratio;

			return torqueLoss * inAngularVelocity;
		}

		#endregion

		#region ITnInPort

		public void Connect(ITnOutPort other)
		{
			NextComponent = other;
		}

		#endregion

		#region ITnOutPort

		public IResponse Initialize(NewtonMeter outTorque, PerSecond outAngularVelocity)
		{
			var dt = Constants.SimulationSettings.TargetTimeInterval;

			Gear = DataBus.CycleData.LeftSample.Gear;

			PerSecond inAngularVelocity;
			NewtonMeter inTorque;

			if (Gear != 0) {
				inAngularVelocity = outAngularVelocity * ModelData.Gears[Gear].Ratio;
				var inTorqueLoss = ModelData.Gears[Gear].LossMap.GetTorqueLoss(outAngularVelocity, outTorque);
				inTorque = outTorque / ModelData.Gears[Gear].Ratio - inTorqueLoss;

				var torqueLossInertia = outAngularVelocity.IsEqual(0)
					? 0.SI<NewtonMeter>()
					: Formulas.InertiaPower(inAngularVelocity, PreviousState.InAngularVelocity, ModelData.Inertia, dt) /
					inAngularVelocity;

				inTorque += torqueLossInertia;
			} else {
				inTorque = 0.SI<NewtonMeter>();
				inAngularVelocity = 0.RPMtoRad();
			}
			PreviousState.SetState(inTorque, inAngularVelocity, outTorque, outAngularVelocity);
			PreviousState.InertiaTorqueLossOut = 0.SI<NewtonMeter>();
			PreviousState.Gear = Gear;

			var response = NextComponent.Initialize(inTorque, inAngularVelocity);
			response.GearboxPowerRequest = inTorque * inAngularVelocity;
			return response;
		}

		/// <summary>
		/// Special Initialize with gear as additional param. For initial Gear-Searching Purposes.
		/// </summary>
		/// <param name="gear"></param>
		/// <param name="outTorque"></param>
		/// <param name="outAngularVelocity"></param>
		/// <returns></returns>
		internal ResponseDryRun Initialize(uint gear, NewtonMeter outTorque, PerSecond outAngularVelocity)
		{
			var inAngularVelocity = outAngularVelocity * ModelData.Gears[gear].Ratio;
			var inTorqueLoss = ModelData.Gears[Gear].LossMap.GetTorqueLoss(outAngularVelocity, outTorque);
			var inTorque = outTorque / ModelData.Gears[Gear].Ratio - inTorqueLoss;

			if (!inAngularVelocity.IsEqual(0)) {
				var alpha = (ModelData.Inertia.IsEqual(0))
					? 0.SI<PerSquareSecond>()
					: outTorque / ModelData.Inertia;

				var inertiaPowerLoss = Formulas.InertiaPower(inAngularVelocity, alpha, ModelData.Inertia,
					Constants.SimulationSettings.TargetTimeInterval);
				inTorque += inertiaPowerLoss / inAngularVelocity;
			}

			var response = NextComponent.Initialize(inTorque, inAngularVelocity);
			response.Switch().
				Case<ResponseSuccess>().
				Case<ResponseOverload>().
				Case<ResponseUnderload>().
				Default(r => { throw new UnexpectedResponseException("Gearbox.Initialize", r); });

			var fullLoadGearbox = ModelData.Gears[gear].FullLoadCurve.FullLoadStationaryTorque(inAngularVelocity) *
								inAngularVelocity;
			var fullLoadEngine = DataBus.EngineStationaryFullPower(inAngularVelocity);

			var fullLoad = VectoMath.Min(fullLoadGearbox, fullLoadEngine);

			return new ResponseDryRun {
				Source = this,
				EnginePowerRequest = response.EnginePowerRequest,
				ClutchPowerRequest = response.ClutchPowerRequest,
				GearboxPowerRequest = outTorque * outAngularVelocity,
				DeltaFullLoad = response.EnginePowerRequest - fullLoad
			};
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
		public IResponse Request(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, bool dryRun)
		{
			Log.Debug("Gearbox Power Request: torque: {0}, angularVelocity: {1}", outTorque, outAngularVelocity);
			Gear = DataBus.CycleData.LeftSample.Gear;

			var avgOutAngularVelocity = (PreviousState.OutAngularVelocity + outAngularVelocity) / 2.0;

			if (Gear == 0) {
				//disengaged
				if (dryRun) {
					// if gearbox is disengaged the 0-line is the limit for drag and full load
					return new ResponseDryRun {
						Source = this,
						GearboxPowerRequest = outTorque * avgOutAngularVelocity,
						DeltaDragLoad = outTorque * avgOutAngularVelocity,
						DeltaFullLoad = outTorque * avgOutAngularVelocity,
					};
				}

				if ((outTorque * avgOutAngularVelocity).IsGreater(0.SI<Watt>(),
					Constants.SimulationSettings.LineSearchTolerance)) {
					return new ResponseOverload {
						Source = this,
						Delta = outTorque * avgOutAngularVelocity,
						GearboxPowerRequest = outTorque * avgOutAngularVelocity
					};
				}

				if ((outTorque * avgOutAngularVelocity).IsSmaller(0.SI<Watt>(),
					Constants.SimulationSettings.LineSearchTolerance)) {
					return new ResponseUnderload {
						Source = this,
						Delta = outTorque * avgOutAngularVelocity,
						GearboxPowerRequest = outTorque * avgOutAngularVelocity
					};
				}

				CurrentState.SetState(0.SI<NewtonMeter>(), 0.RPMtoRad(), outTorque, outAngularVelocity);
				CurrentState.Gear = Gear;

				var disengagedResponse = NextComponent.Request(absTime, dt, 0.SI<NewtonMeter>(), null);
				disengagedResponse.GearboxPowerRequest = outTorque * avgOutAngularVelocity;
				return disengagedResponse;
			} else {
				//engaged
				var inTorqueLoss = ModelData.Gears[Gear].LossMap.GetTorqueLoss(avgOutAngularVelocity, outTorque);
				var inTorque = outTorque / ModelData.Gears[Gear].Ratio - inTorqueLoss;
				var inAngularVelocity = outAngularVelocity * ModelData.Gears[Gear].Ratio;

				if (dryRun) {
					var dryRunResponse = NextComponent.Request(absTime, dt, inTorque, inAngularVelocity, true);
					dryRunResponse.GearboxPowerRequest = outTorque * avgOutAngularVelocity;
					return dryRunResponse;
				}

				// this code has to be _after_ the check for a potential gear-shift!
				// (the above block issues dry-run requests and thus may update the CurrentState!)
				CurrentState.TransmissionTorqueLoss = inTorque - (outTorque / ModelData.Gears[Gear].Ratio);
				if (!inAngularVelocity.IsEqual(0)) {
					// MQ 19.2.2016: check! inertia is related to output side, torque loss accounts to input side
					CurrentState.InertiaTorqueLossOut =
						Formulas.InertiaPower(outAngularVelocity, PreviousState.OutAngularVelocity, ModelData.Inertia, dt) /
						avgOutAngularVelocity;
					inTorque += CurrentState.InertiaTorqueLossOut / ModelData.Gears[Gear].Ratio;
				} else {
					CurrentState.InertiaTorqueLossOut = 0.SI<NewtonMeter>();
				}
				CurrentState.SetState(inTorque, inAngularVelocity, outTorque, outAngularVelocity);
				CurrentState.Gear = Gear;
				// end critical section

				var response = NextComponent.Request(absTime, dt, inTorque, inAngularVelocity);
				response.GearboxPowerRequest = outTorque * avgOutAngularVelocity;
				return response;
			}
		}

		#endregion

		#region VectoSimulationComponent

		protected override void DoWriteModalResults(IModalDataContainer container)
		{
			container[ModalResultField.Gear] = Gear;

			var avgInAngularSpeed = Gear != 0
				? (PreviousState.OutAngularVelocity + CurrentState.OutAngularVelocity) / 2.0 * ModelData.Gears[Gear].Ratio
				: 0.RPMtoRad();

			container[ModalResultField.P_gbx_loss] = CurrentState.TransmissionTorqueLoss * avgInAngularSpeed;
			container[ModalResultField.P_gbx_inertia] = CurrentState.InertiaTorqueLossOut * avgInAngularSpeed;
			container[ModalResultField.P_gbx_in] = CurrentState.InTorque * avgInAngularSpeed;
		}

		protected override void DoCommitSimulationStep()
		{
			if (Gear != 0) {
				if (ModelData.Gears[Gear].LossMap.Extrapolated) {
					Log.Warn(
						"Gear {0} LossMap data was extrapolated: range for loss map is not sufficient: n:{1}, torque:{2}",
						Gear, CurrentState.OutAngularVelocity.ConvertTo().Rounds.Per.Minute, CurrentState.OutTorque);
					if (DataBus.ExecutionMode == ExecutionMode.Declaration) {
						throw new VectoException(
							"Gear {0} LossMap data was extrapolated in Declaration Mode: range for loss map is not sufficient: n:{1}, torque:{2}",
							Gear, CurrentState.OutAngularVelocity.ConvertTo().Rounds.Per.Minute, CurrentState.OutTorque);
					}
				}
			}

			AdvanceState();
		}

		#endregion
	}
}