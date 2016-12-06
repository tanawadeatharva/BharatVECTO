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
	public class CycleGearbox : AbstractGearbox<CycleGearbox.CycleGearboxState>
	{
		protected bool? TorqueConverterActive;

		protected internal readonly TorqueConverter TorqueConverter;

		public CycleGearbox(IVehicleContainer container, GearboxData gearboxModelData, KilogramSquareMeter engineInertia)
			: base(container, gearboxModelData)
		{
			if (!gearboxModelData.Type.AutomaticTransmission()) {
				return;
			}
			var strategy = new CycleShiftStrategy { Gearbox = this };
			TorqueConverter = new TorqueConverter(this, strategy, container, gearboxModelData.TorqueConverterData, engineInertia);
			if (TorqueConverter == null) {
				throw new VectoException("Torque Converter required for AT transmission!");
			}
		}

		public override void Connect(ITnOutPort other)
		{
			base.Connect(other);
			if (TorqueConverter != null)
				TorqueConverter.NextComponent = other;
		}

		public override IResponse Initialize(NewtonMeter outTorque, PerSecond outAngularVelocity)
		{
			var dt = Constants.SimulationSettings.TargetTimeInterval;

			Gear = DataBus.CycleData.LeftSample.Gear;
			TorqueConverterActive = DataBus.CycleData.LeftSample.TorqueConverterActive;

			if (TorqueConverter != null && TorqueConverterActive == null) {
				throw new VectoSimulationException("Driving cycle does not contain information about TorqueConverter!");
			}

			var inAngularVelocity = DataBus.EngineIdleSpeed;
			var inTorque = 0.SI<NewtonMeter>();
			IResponse response;

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

				response = (TorqueConverterActive != null && TorqueConverterActive.Value)
					? TorqueConverter.Initialize(inTorque, inAngularVelocity)
					: NextComponent.Initialize(inTorque, inAngularVelocity);
			}
			CurrentState.SetState(inTorque, inAngularVelocity, outTorque, outAngularVelocity);
			PreviousState.SetState(inTorque, inAngularVelocity, outTorque, outAngularVelocity);
			PreviousState.InertiaTorqueLossOut = 0.SI<NewtonMeter>();
			PreviousState.Gear = Gear;

			response = NextComponent.Initialize(inTorque, inAngularVelocity);
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

			// mk 2016-11-30: added additional check for outAngularVelocity due to failing test: MeasuredSpeed_Gear_AT_PS_Run
			var retVal = Gear == 0 || outAngularVelocity.IsEqual(0)
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
			var torqueConverterLocked = TorqueConverterActive == null || !TorqueConverterActive.Value;

			var effectiveRatio = ModelData.Gears[Gear].Ratio;
			var effectiveLossMap = ModelData.Gears[Gear].LossMap;
			if (!torqueConverterLocked) {
				effectiveRatio = ModelData.Gears[Gear].TorqueConverterRatio;
				effectiveLossMap = ModelData.Gears[Gear].TorqueConverterGearLossMap;
			}

			var avgOutAngularVelocity = (PreviousState.OutAngularVelocity + outAngularVelocity) / 2.0;
			var inTorqueLossResult = effectiveLossMap.GetTorqueLoss(avgOutAngularVelocity, outTorque);
			CurrentState.TorqueLossResult = inTorqueLossResult;
			var inTorque = outTorque / effectiveRatio + inTorqueLossResult.Value;
			CurrentState.TorqueLossResult = inTorqueLossResult;

			if (!torqueConverterLocked && !ModelData.Gears[Gear].HasTorqueConverter) {
				throw new VectoSimulationException("Torque converter requested by strategy for gear without torque converter!");
			}

			var inAngularVelocity = outAngularVelocity * effectiveRatio;

			if (ModelData.Type.AutomaticTransmission() && torqueConverterLocked &&
				inAngularVelocity.IsSmaller(DataBus.EngineIdleSpeed)) {
				Log.Error(
					"ERROR: EngineSpeed is lower than Idlespeed in Measuredspeed-Cycle with given Gear (Automatic Transmission). AbsTime: {0}, Gear: {1} TC-Active: {2}, EngineSpeed: {3}",
					absTime, Gear, !torqueConverterLocked, inAngularVelocity.AsRPM);
				return new ResponseEngineSpeedTooLow { Source = this };
			}

			if (!inAngularVelocity.IsEqual(0)) {
				// MQ 19.2.2016: check! inertia is related to output side, torque loss accounts to input side
				CurrentState.InertiaTorqueLossOut =
					Formulas.InertiaPower(outAngularVelocity, PreviousState.OutAngularVelocity, ModelData.Inertia, dt) /
					avgOutAngularVelocity;
				inTorque += CurrentState.InertiaTorqueLossOut / effectiveRatio;
			} else {
				CurrentState.InertiaTorqueLossOut = 0.SI<NewtonMeter>();
			}

			if (dryRun) {
				if (TorqueConverter != null && !torqueConverterLocked) {
					return TorqueConverter.Request(absTime, dt, inTorque, inAngularVelocity, true);
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
			CurrentState.TransmissionTorqueLoss = inTorque - (outTorque / effectiveRatio);

			CurrentState.SetState(inTorque, inAngularVelocity, outTorque, outAngularVelocity);
			CurrentState.Gear = Gear;
			// end critical section

			if (TorqueConverter != null && !torqueConverterLocked) {
				CurrentState.TorqueConverterActive = true;
				return TorqueConverter.Request(absTime, dt, inTorque, inAngularVelocity);
			}
			if (outTorque.IsSmaller(0) && inAngularVelocity.IsSmaller(DataBus.EngineIdleSpeed)) {
				Log.Warn("engine speed would fall below idle speed - disengage! gear from cycle: {0}, vehicle speed: {1}", Gear,
					DataBus.VehicleSpeed);
				Gear = 0;
				return RequestDisengaged(absTime, dt, outTorque, outAngularVelocity, dryRun);
			}
			if (TorqueConverter != null)
				TorqueConverter.Locked(CurrentState.InTorque, CurrentState.InAngularVelocity);
			var response = NextComponent.Request(absTime, dt, inTorque, inAngularVelocity);
			response.GearboxPowerRequest = outTorque * avgOutAngularVelocity;
			return response;
		}

		/// <summary>
		/// Handles Requests when no gear is disengaged
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

			if ((outTorque * avgOutAngularVelocity).IsGreater(0.SI<Watt>(), Constants.SimulationSettings.LineSearchTolerance) &&
				!outAngularVelocity.IsEqual(0)) {
				return new ResponseOverload {
					Source = this,
					Delta = outTorque * avgOutAngularVelocity,
					GearboxPowerRequest = outTorque * avgOutAngularVelocity
				};
			}

			if ((outTorque * avgOutAngularVelocity).IsSmaller(0.SI<Watt>(), Constants.SimulationSettings.LineSearchTolerance)) {
				return new ResponseUnderload {
					Source = this,
					Delta = outTorque * avgOutAngularVelocity,
					GearboxPowerRequest = outTorque * avgOutAngularVelocity
				};
			}

			CurrentState.SetState(0.SI<NewtonMeter>(), 0.SI<PerSecond>(), outTorque, outAngularVelocity);
			CurrentState.Gear = Gear;

			var motoringSpeed = DataBus.EngineSpeed;
			var first = (ResponseDryRun)NextComponent.Request(absTime, dt, outTorque, motoringSpeed, true);
			try {
				motoringSpeed = SearchAlgorithm.Search(motoringSpeed, first.DeltaDragLoad,
					Constants.SimulationSettings.EngineIdlingSearchInterval,
					getYValue: result => ((ResponseDryRun)result).DeltaDragLoad,
					evaluateFunction: n => NextComponent.Request(absTime, dt, 0.SI<NewtonMeter>(), n, true),
					criterion: result => ((ResponseDryRun)result).DeltaDragLoad.Value());
			} catch (VectoException) {
				Log.Warn("CycleGearbox could not find motoring speed for disengaged state.");
			}
			motoringSpeed = motoringSpeed.LimitTo(DataBus.EngineIdleSpeed, DataBus.EngineSpeed);

			if (TorqueConverter != null)
				TorqueConverter.Locked(CurrentState.InTorque, motoringSpeed);

			var disengagedResponse = NextComponent.Request(absTime, dt, 0.SI<NewtonMeter>(), motoringSpeed);
			disengagedResponse.GearboxPowerRequest = outTorque * avgOutAngularVelocity;
			return disengagedResponse;
		}

		protected override void DoWriteModalResults(IModalDataContainer container)
		{
			var avgInAngularSpeed = (PreviousState.InAngularVelocity + CurrentState.InAngularVelocity) / 2.0;
			container[ModalResultField.Gear] = Gear;
			container[ModalResultField.P_gbx_loss] = CurrentState.TransmissionTorqueLoss * avgInAngularSpeed;
			container[ModalResultField.P_gbx_inertia] = CurrentState.InertiaTorqueLossOut * avgInAngularSpeed;
			container[ModalResultField.P_gbx_in] = CurrentState.InTorque * avgInAngularSpeed;

			if (ModelData.Type.AutomaticTransmission()) {
				container[ModalResultField.TC_Locked] = !CurrentState.TorqueConverterActive;
			}
			// torque converter fields are written by TorqueConverter (if present), called from Vehicle container 
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

		public override uint NextGear
		{
			get { return DataBus.CycleData.RightSample.Gear; }
		}

		public override bool ClutchClosed(Second absTime)
		{
			return (DataBus.DriverBehavior == DrivingBehavior.Braking
						? DataBus.CycleData.LeftSample.Gear
						: DataBus.CycleData.RightSample.Gear) != 0;
		}

		#endregion

		public class CycleGearboxState : GearboxState
		{
			public bool TorqueConverterActive;
		}

		public class CycleShiftStrategy : IShiftStrategy
		{
			public bool ShiftRequired(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity,
				NewtonMeter inTorque,
				PerSecond inAngularVelocity, uint gear, Second lastShiftTime)
			{
				return false;
			}

			public uint InitGear(Second absTime, Second dt, NewtonMeter torque, PerSecond outAngularVelocity)
			{
				throw new System.NotImplementedException();
			}

			public uint Engage(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity)
			{
				throw new System.NotImplementedException();
			}

			public void Disengage(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outEngineSpeed)
			{
				throw new System.NotImplementedException();
			}

			public IGearbox Gearbox { get; set; }

			public uint NextGear
			{
				get { throw new System.NotImplementedException(); }
			}
		}
	}
}