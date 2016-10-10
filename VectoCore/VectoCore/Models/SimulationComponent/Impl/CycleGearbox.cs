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

using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
	public class CycleGearbox : AbstractGearbox<CycleGearbox.CycleGearboxState>
	{
		protected bool? TorqueConverterActive;

		protected internal TorqueConverterData TorqueConverter;
		private readonly KilogramSquareMeter _engineInertia;

		public CycleGearbox(IVehicleContainer container, GearboxData gearboxModelData, KilogramSquareMeter engineInertia)
			: base(container, gearboxModelData)
		{
			_engineInertia = engineInertia;
			if (!gearboxModelData.Type.AutomaticTransmission()) {
				return;
			}
			TorqueConverter = gearboxModelData.TorqueConverterData;
			if (TorqueConverter == null) {
				throw new VectoException("Torque Converter required for AT transmission!");
			}
		}

		public override IResponse Initialize(NewtonMeter outTorque, PerSecond outAngularVelocity)
		{
			var dt = Constants.SimulationSettings.TargetTimeInterval;

			Gear = DataBus.CycleData.LeftSample.Gear;
			TorqueConverterActive = DataBus.CycleData.LeftSample.TorqueConverterActive;

			if (TorqueConverter != null && TorqueConverterActive == null) {
				throw new VectoSimulationException("Driving cycle does not contain information about TorqueConverter!");
			}

			PerSecond inAngularVelocity;
			NewtonMeter inTorque;

			if (Gear != 0) {
				inAngularVelocity = outAngularVelocity * ModelData.Gears[Gear].Ratio;
				var inTorqueLossResult = ModelData.Gears[Gear].LossMap.GetTorqueLoss(outAngularVelocity, outTorque);
				CurrentState.TorqueLossResult = inTorqueLossResult;
				inTorque = outTorque / ModelData.Gears[Gear].Ratio - inTorqueLossResult.Value;

				var torqueLossInertia = outAngularVelocity.IsEqual(0)
					? 0.SI<NewtonMeter>()
					: Formulas.InertiaPower(inAngularVelocity, PreviousState.InAngularVelocity, ModelData.Inertia, dt) /
					inAngularVelocity;

				inTorque += torqueLossInertia;
			} else {
				inTorque = 0.SI<NewtonMeter>();
				inAngularVelocity = DataBus.EngineIdleSpeed;
			}
			PreviousState.SetState(inTorque, inAngularVelocity, outTorque, outAngularVelocity);
			PreviousState.InertiaTorqueLossOut = 0.SI<NewtonMeter>();
			PreviousState.Gear = Gear;

			var response = NextComponent.Initialize(inTorque, inAngularVelocity);
			response.GearboxPowerRequest = inTorque * inAngularVelocity;
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
		public override IResponse Request(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity,
			bool dryRun = false)
		{
			Log.Debug("Gearbox Power Request: torque: {0}, angularVelocity: {1}", outTorque, outAngularVelocity);
			Gear = DataBus.DriverBehavior == DrivingBehavior.Braking
				? DataBus.CycleData.LeftSample.Gear
				: DataBus.CycleData.RightSample.Gear;

			TorqueConverterActive = DataBus.DriverBehavior == DrivingBehavior.Braking
				? DataBus.CycleData.LeftSample.TorqueConverterActive
				: DataBus.CycleData.RightSample.TorqueConverterActive;

			if (TorqueConverter != null && TorqueConverterActive == null) {
				throw new VectoSimulationException("Driving cycle does not contain information about TorqueConverter!");
			}
			if (Gear != 0 && !ModelData.Gears.ContainsKey(Gear)) {
				throw new VectoSimulationException("Requested Gear {0} from driving cycle is not available", Gear);
			}

			var retVal = Gear == 0
				? RequestDisengaged(absTime, dt, outTorque, outAngularVelocity, dryRun)
				: RequestEngaged(absTime, dt, outTorque, outAngularVelocity, dryRun);

			retVal.GearboxPowerRequest = outTorque * outAngularVelocity;
			return retVal;
		}

		/// <summary>
		/// Handles requests when a gear is engaged
		/// </summary>
		/// <param name="absTime"></param>
		/// <param name="dt"></param>
		/// <param name="outTorque"></param>
		/// <param name="outAngularVelocity"></param>
		/// <param name="dryRun"></param>
		/// <returns></returns>
		private IResponse RequestEngaged(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity,
			bool dryRun)
		{
			var avgOutAngularVelocity = (PreviousState.OutAngularVelocity + outAngularVelocity) / 2.0;
			var inTorqueLossResult = ModelData.Gears[Gear].LossMap.GetTorqueLoss(avgOutAngularVelocity, outTorque);
			var inTorque = outTorque / ModelData.Gears[Gear].Ratio + inTorqueLossResult.Value;
			CurrentState.TorqueLossResult = inTorqueLossResult;
			var inAngularVelocity = outAngularVelocity * ModelData.Gears[Gear].Ratio;

			if (!inAngularVelocity.IsEqual(0)) {
				// MQ 19.2.2016: check! inertia is related to output side, torque loss accounts to input side
				CurrentState.InertiaTorqueLossOut =
					Formulas.InertiaPower(outAngularVelocity, PreviousState.OutAngularVelocity, ModelData.Inertia, dt) /
					avgOutAngularVelocity;
				inTorque += CurrentState.InertiaTorqueLossOut / ModelData.Gears[Gear].Ratio;
			} else {
				CurrentState.InertiaTorqueLossOut = 0.SI<NewtonMeter>();
			}

			if (dryRun) {
				if (TorqueConverter != null && TorqueConverterActive != null && TorqueConverterActive.Value) {
					return RequestTorqueConverter(absTime, dt, inTorque, inAngularVelocity, true);
				}
				if (outTorque.IsSmaller(0) && inAngularVelocity.IsSmaller(DataBus.EngineIdleSpeed)) {
					//Log.Warn("engine speed would fall below idle speed - disengage! gear from cycle: {0}, vehicle speed: {1}", Gear,
					//	DataBus.VehicleSpeed);
					Gear = 0;
					return RequestDisengaged(absTime, dt, outTorque, outAngularVelocity, dryRun);
				}
				var dryRunResponse = NextComponent.Request(absTime, dt, inTorque, inAngularVelocity, true);
				dryRunResponse.GearboxPowerRequest = outTorque * avgOutAngularVelocity;
				return dryRunResponse;
			}

			// this code has to be _after_ the check for a potential gear-shift!
			// (the above block issues dry-run requests and thus may update the CurrentState!)
			CurrentState.TransmissionTorqueLoss = inTorque - (outTorque / ModelData.Gears[Gear].Ratio);

			CurrentState.SetState(inTorque, inAngularVelocity, outTorque, outAngularVelocity);
			CurrentState.Gear = Gear;
			// end critical section

			if (TorqueConverter != null && TorqueConverterActive != null && TorqueConverterActive.Value) {
				CurrentState.TorqueConverterActive = true;
				return RequestTorqueConverter(absTime, dt, inTorque, inAngularVelocity);
			}
			if (outTorque.IsSmaller(0) && inAngularVelocity.IsSmaller(DataBus.EngineIdleSpeed)) {
				Log.Warn("engine speed would fall below idle speed - disengage! gear from cycle: {0}, vehicle speed: {1}", Gear,
					DataBus.VehicleSpeed);
				Gear = 0;
				return RequestDisengaged(absTime, dt, outTorque, outAngularVelocity, dryRun);
			}
			var response = NextComponent.Request(absTime, dt, inTorque, inAngularVelocity);
			response.GearboxPowerRequest = outTorque * avgOutAngularVelocity;
			return response;
		}

		/// <summary>
		/// Handle Requests when Torque Converter is active
		/// </summary>
		/// <param name="absTime"></param>
		/// <param name="dt"></param>
		/// <param name="outTorque">torque at the output of the torque converter (to the mechanical gear)</param>
		/// <param name="outAngularVelocity">angular velocity at the output of the torque converter (to the mechanical gear)</param>
		/// <param name="dryRun"></param>
		/// <returns></returns>
		private IResponse RequestTorqueConverter(Second absTime, Second dt, NewtonMeter outTorque,
			PerSecond outAngularVelocity, bool dryRun = false)
		{
			if (dryRun) {
				return RequestTorqueConverterDryRun(absTime, dt, outTorque, outAngularVelocity);
			}
			var operatingPoint = FindOperatingPoint(outTorque, outAngularVelocity);
			CurrentState.TorqueConverterOperatingPoint = operatingPoint;
			if (!outAngularVelocity.IsEqual(operatingPoint.OutAngularVelocity) || !outTorque.IsEqual(operatingPoint.OutTorque)) {
				// a different operating point was found...
				var delta = (outTorque - operatingPoint.OutTorque) *
							(PreviousState.OutAngularVelocity + operatingPoint.OutAngularVelocity) / 2.0;
				if (!delta.IsEqual(0, Constants.SimulationSettings.LineSearchTolerance)) {
					if (delta > 0) {
						return new ResponseOverload { Source = this, Delta = delta, TorqueConverterOperatingPoint = operatingPoint };
					}
					return new ResponseUnderload { Source = this, Delta = delta, TorqueConverterOperatingPoint = operatingPoint };
				}
			}
			var tcResponse = NextComponent.Request(absTime, dt, operatingPoint.InTorque, operatingPoint.InAngularVelocity);
			return tcResponse;
		}

		/// <summary>
		/// Handle Requests when searching an operating point and torque converter is active
		/// </summary>
		/// <param name="absTime"></param>
		/// <param name="dt"></param>
		/// <param name="outTorque"></param>
		/// <param name="outAngularVelocity"></param>
		/// <returns></returns>
		private IResponse RequestTorqueConverterDryRun(Second absTime, Second dt, NewtonMeter outTorque,
			PerSecond outAngularVelocity)
		{
			var dryOperatingPoint = FindOperatingPoint(outTorque, outAngularVelocity);
			var engineResponse =
				(ResponseDryRun)NextComponent.Request(absTime, dt, dryOperatingPoint.InTorque, dryOperatingPoint.InAngularVelocity,
					true);

			var deltaTorqueConverter = (outTorque - dryOperatingPoint.OutTorque) *
										(PreviousState.OutAngularVelocity + dryOperatingPoint.OutAngularVelocity) / 2.0;

			var deltaEngine = (engineResponse.DeltaFullLoad > 0 ? engineResponse.DeltaFullLoad : 0.SI<Watt>()) +
							(engineResponse.DeltaDragLoad < 0 ? -engineResponse.DeltaDragLoad : 0.SI<Watt>());

			dryOperatingPoint = outTorque.IsGreater(0) && DataBus.BrakePower.IsEqual(0)
				? GetMaxPowerOperatingPoint(dt, outAngularVelocity, engineResponse)
				: GetDragPowerOperatingPoint(dt, outAngularVelocity, engineResponse);
			engineResponse = (ResponseDryRun)NextComponent.Request(absTime, dt, dryOperatingPoint.InTorque,
				dryOperatingPoint.InAngularVelocity, true);

			var delta = (outTorque - dryOperatingPoint.OutTorque) *
						(PreviousState.OutAngularVelocity + dryOperatingPoint.OutAngularVelocity) / 2.0;
			//deltaTorqueConverter.Value() * (deltaEngine.IsEqual(0) ? 1 : deltaEngine.Value());
			return new ResponseDryRun() {
				Source = this,
				DeltaFullLoad = delta,
				DeltaDragLoad = delta,
				TorqueConverterOperatingPoint = dryOperatingPoint
			};
		}

		/// <summary>
		/// Handles Requests when no gear is engaged
		/// </summary>
		/// <param name="absTime"></param>
		/// <param name="dt"></param>
		/// <param name="outTorque"></param>
		/// <param name="outAngularVelocity"></param>
		/// <param name="dryRun"></param>
		/// <returns></returns>
		private IResponse RequestDisengaged(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity,
			bool dryRun)
		{
			var avgOutAngularVelocity = (PreviousState.OutAngularVelocity + outAngularVelocity) / 2.0;
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

			var disengagedResponse = NextComponent.Request(absTime, dt, 0.SI<NewtonMeter>(), DataBus.EngineIdleSpeed);
			disengagedResponse.GearboxPowerRequest = outTorque * avgOutAngularVelocity;
			return disengagedResponse;
			//todo mk-2016-08-17: minus inTorqueLoss?? Why not plus?
		}

		private TorqueConverterOperatingPoint FindOperatingPoint(NewtonMeter outTorque,
			PerSecond outAngularVelocity)
		{
			try {
				var operatingPoint = TorqueConverter.FindOperatingPoint(outTorque, outAngularVelocity);
				if (operatingPoint.InAngularVelocity.IsSmaller(DataBus.EngineIdleSpeed)) {
					throw new VectoException("Invalid operating point, inAngularVelocity below engine's idle speed: {0}",
						operatingPoint.InAngularVelocity);
				}
				if (operatingPoint.InAngularVelocity.IsGreater(DataBus.EngineRatedSpeed)) {
					operatingPoint = TorqueConverter.FindOperatingPoint(DataBus.EngineRatedSpeed, outAngularVelocity);
				}
				return operatingPoint;
			} catch (VectoException ve) {
				Log.Debug(ve, "failed to find torque converter operating point, fallback: creeping");
				var tqOperatingPoint = TorqueConverter.FindOperatingPoint(DataBus.EngineIdleSpeed, outAngularVelocity);
				return tqOperatingPoint;
			}
		}

		private TorqueConverterOperatingPoint GetDragPowerOperatingPoint(Second dt, PerSecond outAngularVelocity,
			ResponseDryRun engineResponse)
		{
			try {
				var operatingPoint =
					ModelData.TorqueConverterData.FindOperatingPointForPowerDemand(
						engineResponse.DragPower - engineResponse.AuxiliariesPowerDemand,
						DataBus.EngineSpeed, outAngularVelocity, _engineInertia, dt);
				if (operatingPoint.InAngularVelocity.IsGreater(DataBus.EngineRatedSpeed)) {
					operatingPoint = ModelData.TorqueConverterData.FindOperatingPoint(DataBus.EngineRatedSpeed, outAngularVelocity);
				}
				if (operatingPoint.InAngularVelocity.IsSmaller(DataBus.EngineIdleSpeed)) {
					operatingPoint = ModelData.TorqueConverterData.FindOperatingPoint(DataBus.EngineIdleSpeed, outAngularVelocity);
				}
				return operatingPoint;
			} catch (VectoException ve) {
				Log.Error(ve, "failed to find torque converter operating point for DragPower {0}", engineResponse.DragPower);
				//throw;
				return ModelData.TorqueConverterData.FindOperatingPoint(engineResponse.EngineSpeed, outAngularVelocity);
			}
		}

		private TorqueConverterOperatingPoint GetMaxPowerOperatingPoint(Second dt, PerSecond outAngularVelocity,
			ResponseDryRun engineResponse)
		{
			try {
				var operatingPoint =
					ModelData.TorqueConverterData.FindOperatingPointForPowerDemand(
						engineResponse.DynamicFullLoadPower - engineResponse.AuxiliariesPowerDemand,
						DataBus.EngineSpeed, outAngularVelocity, _engineInertia, dt);
				if (operatingPoint.InAngularVelocity.IsGreater(DataBus.EngineRatedSpeed)) {
					operatingPoint = ModelData.TorqueConverterData.FindOperatingPoint(DataBus.EngineRatedSpeed, outAngularVelocity);
				}
				return operatingPoint;
			} catch (VectoException ve) {
				Log.Error(ve, "failed to find torque converter operating point for MaxPower {0}",
					engineResponse.DynamicFullLoadPower);
				throw;
			}
		}

		protected override void DoWriteModalResults(IModalDataContainer container)
		{
			container[ModalResultField.Gear] = Gear;
			if (ModelData.Type.AutomaticTransmission()) {
				container[ModalResultField.TC_Locked] = !CurrentState.TorqueConverterActive;
			}
			var avgInAngularSpeed = Gear != 0
				? (PreviousState.OutAngularVelocity + CurrentState.OutAngularVelocity) / 2.0 * ModelData.Gears[Gear].Ratio
				: 0.RPMtoRad();

			container[ModalResultField.P_gbx_loss] = CurrentState.TransmissionTorqueLoss * avgInAngularSpeed;
			container[ModalResultField.P_gbx_inertia] = CurrentState.InertiaTorqueLossOut * avgInAngularSpeed;
			container[ModalResultField.P_gbx_in] = CurrentState.InTorque * avgInAngularSpeed;

			if (TorqueConverter != null) {
				DoWriteTorqueConverterModalResults(container, avgInAngularSpeed);
			}
		}

		private void DoWriteTorqueConverterModalResults(IModalDataContainer container, PerSecond avgInAngularSpeed)
		{
			if (CurrentState.TorqueConverterOperatingPoint == null) {
				container[ModalResultField.TorqueConverterTorqueRatio] = 1.0;
				container[ModalResultField.TorqueConverterSpeedRatio] = 1.0;

				container[ModalResultField.TC_TorqueIn] = CurrentState.InTorque;
				container[ModalResultField.TC_TorqueOut] = CurrentState.InTorque;
				container[ModalResultField.TC_angularSpeedIn] = CurrentState.InAngularVelocity;
				container[ModalResultField.TC_angularSpeedOut] = CurrentState.OutAngularVelocity;

				container[ModalResultField.P_TC_out] = CurrentState.InTorque * avgInAngularSpeed;
				container[ModalResultField.P_TC_loss] = 0.SI<Watt>();
			} else {
				container[ModalResultField.TorqueConverterTorqueRatio] = CurrentState.TorqueConverterOperatingPoint.TorqueRatio;
				container[ModalResultField.TorqueConverterSpeedRatio] = CurrentState.TorqueConverterOperatingPoint.SpeedRatio;

				container[ModalResultField.TC_TorqueIn] = CurrentState.TorqueConverterOperatingPoint.InTorque;
				container[ModalResultField.TC_TorqueOut] = CurrentState.TorqueConverterOperatingPoint.OutTorque;
				container[ModalResultField.TC_angularSpeedIn] = CurrentState.TorqueConverterOperatingPoint.InAngularVelocity;
				container[ModalResultField.TC_angularSpeedOut] = CurrentState.TorqueConverterOperatingPoint.OutAngularVelocity;

				var avgOutVelocity = ((PreviousState.TorqueConverterOperatingPoint != null
										? PreviousState.TorqueConverterOperatingPoint.OutAngularVelocity
										: PreviousState.InAngularVelocity) +
									CurrentState.TorqueConverterOperatingPoint.OutAngularVelocity) / 2.0;
				var avgInVelocity = ((PreviousState.TorqueConverterOperatingPoint != null
										? PreviousState.TorqueConverterOperatingPoint.InAngularVelocity
										: PreviousState.InAngularVelocity) +
									CurrentState.TorqueConverterOperatingPoint.InAngularVelocity) / 2.0;
				container[ModalResultField.P_TC_out] = CurrentState.OutTorque * avgOutVelocity;
				container[ModalResultField.P_TC_loss] = CurrentState.InTorque * avgInVelocity -
														CurrentState.OutTorque * avgOutVelocity;
			}
		}

		protected override void DoCommitSimulationStep()
		{
			if (Gear != 0) {
				if (CurrentState.TorqueLossResult != null && CurrentState.TorqueLossResult.Extrapolated) {
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
			base.DoCommitSimulationStep();
		}

		#region ICluchInfo

		public override bool ClutchClosed(Second absTime)
		{
			return DataBus.CycleData.LeftSample.Gear != 0;
		}

		#endregion

		public class CycleGearboxState : GearboxState
		{
			public bool TorqueConverterActive;
			public TorqueConverterOperatingPoint TorqueConverterOperatingPoint;
		}
	}
}