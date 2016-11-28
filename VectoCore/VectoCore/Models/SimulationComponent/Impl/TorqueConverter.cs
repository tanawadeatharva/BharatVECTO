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

using System.Collections.Generic;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.Connector.Ports;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
	public class TorqueConverter : StatefulVectoSimulationComponent<TorqueConverter.TorqueConverterComponentState>,
		ITnInPort, ITnOutPort
	{
		protected readonly IGearboxInfo Gearbox;

		protected readonly IShiftStrategy ShiftStrategy;

		protected TorqueConverterData ModelData;
		private readonly KilogramSquareMeter _engineInertia;

		//protected bool SearchingTcOperatingPoint;

		public ITnOutPort NextComponent { protected internal get; set; }

		public TorqueConverter(IGearboxInfo gearbox, IShiftStrategy shiftStrategy, IVehicleContainer container,
			TorqueConverterData tcData, KilogramSquareMeter engineInertia) : base(container)
		{
			Gearbox = gearbox;
			ShiftStrategy = shiftStrategy;
			ModelData = tcData;
			_engineInertia = engineInertia;
		}

		public void Connect(ITnOutPort other)
		{
			NextComponent = other;
		}

		public IResponse Initialize(NewtonMeter outTorque, PerSecond outAngularVelocity)
		{
			var operatingPointList = ModelData.FindOperatingPoint(outTorque, outAngularVelocity, DataBus.EngineIdleSpeed);
			TorqueConverterOperatingPoint operatingPoint;
			if (operatingPointList.Count > 0) {
				operatingPoint = SelectOperatingPoint(operatingPointList);
			} else {
				Log.Warn(
					"TorqueConverter Initialize: No operating point found. Using output as input values as fallback for initialize.");
				operatingPoint = new TorqueConverterOperatingPoint {
					OutAngularVelocity = outAngularVelocity,
					OutTorque = outTorque,
					InAngularVelocity = outAngularVelocity,
					InTorque = outTorque
				};
			}
			var retVal = NextComponent.Initialize(operatingPoint.InTorque, operatingPoint.InAngularVelocity);
			PreviousState.SetState(operatingPoint.InTorque, operatingPoint.InAngularVelocity, operatingPoint.OutTorque,
				operatingPoint.OutAngularVelocity);
			return retVal;
		}

		public IResponse Request(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity,
			bool dryRun = false)
		{
            //var avgAngularVelocity = (PreviousState.OutAngularVelocity + outAngularVelocity)/ 2;
			if (dryRun) {
				var dryOperatingPoint = FindOperatingPoint(outTorque, outAngularVelocity);
				var engineResponse = (ResponseDryRun)
					NextComponent.Request(absTime, dt, dryOperatingPoint.InTorque, dryOperatingPoint.InAngularVelocity, true);

				dryOperatingPoint = outTorque.IsGreater(0) && DataBus.BrakePower.IsEqual(0)
					? GetMaxPowerOperatingPoint(dt, outAngularVelocity, engineResponse, PreviousState.InPower())
					: GetDragPowerOperatingPoint(dt, outAngularVelocity, engineResponse, PreviousState.InPower());

                var dryAvgPower = (PreviousState.InAngularVelocity * PreviousState.InTorque + dryOperatingPoint.InAngularVelocity * dryOperatingPoint.InTorque) / 2;
                var dryInTorque = dryAvgPower / ((PreviousState.InAngularVelocity + dryOperatingPoint.InAngularVelocity) / 2);
				var engineResponse2 = (ResponseDryRun)NextComponent.Request(absTime, dt, dryInTorque, dryOperatingPoint.InAngularVelocity, true);

				var delta = (outTorque - dryOperatingPoint.OutTorque) *
							(PreviousState.OutAngularVelocity + dryOperatingPoint.OutAngularVelocity) / 2.0;

				return new ResponseDryRun() {
					Source = this,
					DeltaFullLoad = delta,
					DeltaDragLoad = delta,
					TorqueConverterOperatingPoint = dryOperatingPoint
				};
			}
			var operatingPoint = FindOperatingPoint(outTorque, outAngularVelocity);
			var ratio = Gearbox.GetGearData(Gearbox.Gear).TorqueConverterRatio;
			if (ShiftStrategy.ShiftRequired(absTime, dt, outTorque * ratio, outAngularVelocity / ratio, operatingPoint.InTorque,
				operatingPoint.InAngularVelocity, Gearbox.Gear, Gearbox.LastShift)) {
				return new ResponseGearShift() { Source = this };
			}
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

            //var inAngularVelocity = 2 * operatingPoint.InAngularVelocity - PreviousState.InAngularVelocity;
            CurrentState.SetState(operatingPoint.InTorque, operatingPoint.InAngularVelocity, outTorque, outAngularVelocity);
			CurrentState.OperatingPoint = operatingPoint;
            var avgPower = (PreviousState.InAngularVelocity * PreviousState.InTorque + CurrentState.InAngularVelocity * CurrentState.InTorque)/2;
		    var inTorque = avgPower / ((PreviousState.InAngularVelocity + CurrentState.InAngularVelocity) / 2);
		    var retVal = NextComponent.Request(absTime, dt, inTorque, operatingPoint.InAngularVelocity);
			return retVal;
		}

		private TorqueConverterOperatingPoint GetDragPowerOperatingPoint(Second dt, PerSecond outAngularVelocity,
            ResponseDryRun engineResponse, Watt previousPower)
		{
			try {
				var operatingPoint =
					ModelData.FindOperatingPointForPowerDemand(engineResponse.DragPower - engineResponse.AuxiliariesPowerDemand,
						DataBus.EngineSpeed, outAngularVelocity, _engineInertia, dt, previousPower);
				if (operatingPoint.InAngularVelocity.IsGreater(DataBus.EngineRatedSpeed)) {
					operatingPoint = ModelData.FindOperatingPoint(DataBus.EngineRatedSpeed, outAngularVelocity);
				}
				if (operatingPoint.InAngularVelocity.IsSmaller(DataBus.EngineIdleSpeed)) {
					operatingPoint = ModelData.FindOperatingPoint(DataBus.EngineIdleSpeed, outAngularVelocity);
				}
				return operatingPoint;
			} catch (VectoException ve) {
				Log.Error(ve, "failed to find torque converter operating point for DragPower {0}", engineResponse.DragPower);
				//throw;
				return ModelData.FindOperatingPoint(engineResponse.EngineSpeed, outAngularVelocity);
			}
		}

		private TorqueConverterOperatingPoint GetMaxPowerOperatingPoint(Second dt, PerSecond outAngularVelocity,
			ResponseDryRun engineResponse, Watt previousPower)
		{
			try {
				var operatingPoint =
					ModelData.FindOperatingPointForPowerDemand(
						engineResponse.DynamicFullLoadPower - engineResponse.AuxiliariesPowerDemand,
						DataBus.EngineSpeed, outAngularVelocity, _engineInertia, dt, previousPower);
				if (operatingPoint.InAngularVelocity.IsGreater(DataBus.EngineRatedSpeed)) {
					operatingPoint = ModelData.FindOperatingPoint(DataBus.EngineRatedSpeed, outAngularVelocity);
				}
				return operatingPoint;
			} catch (VectoException ve) {
				Log.Error(ve, "failed to find torque converter operating point for MaxPower {0}",
					engineResponse.DynamicFullLoadPower);
				throw;
			}
		}

		protected internal TorqueConverterOperatingPoint FindOperatingPoint(NewtonMeter outTorque,
			PerSecond outAngularVelocity)
		{
			var operatingPointList = ModelData.FindOperatingPoint(outTorque, outAngularVelocity, DataBus.EngineIdleSpeed);
			if (operatingPointList.Count == 0) {
				Log.Debug("TorqueConverter: Failed to find torque converter operating point, fallback: creeping");
				var tqOperatingPoint = ModelData.FindOperatingPoint(DataBus.EngineIdleSpeed, outAngularVelocity);
				return tqOperatingPoint;
			}

			var operatingPoint = SelectOperatingPoint(operatingPointList);
			if (operatingPoint.InAngularVelocity.IsSmaller(DataBus.EngineIdleSpeed)) {
				throw new VectoException("Invalid operating point, inAngularVelocity below engine's idle speed: {0}",
					operatingPoint.InAngularVelocity);
			}
			if (operatingPoint.InAngularVelocity.IsGreater(DataBus.EngineRatedSpeed)) {
				operatingPoint = ModelData.FindOperatingPoint(DataBus.EngineRatedSpeed, outAngularVelocity);
			}
			return operatingPoint;
		}

		private TorqueConverterOperatingPoint SelectOperatingPoint(IList<TorqueConverterOperatingPoint> operatingPointList)
		{
			if (operatingPointList.Count == 1) {
				return operatingPointList[0];
			}

			var filtered = operatingPointList.Where(x =>
					(x.InTorque * x.InAngularVelocity).IsSmallerOrEqual(DataBus.EngineStationaryFullPower(x.InAngularVelocity),
						Constants.SimulationSettings.LineSearchTolerance.SI<Watt>()) &&
					(x.InTorque * x.InAngularVelocity).IsGreaterOrEqual(DataBus.EngineDragPower(x.InAngularVelocity),
						Constants.SimulationSettings.LineSearchTolerance.SI<Watt>())
			).ToList();

			if (filtered.Count == 1) {
				return filtered.First();
			}

			return operatingPointList[0];
		}

		protected override void DoWriteModalResults(IModalDataContainer container)
		{
			if (CurrentState.OperatingPoint == null) {
				container[ModalResultField.TorqueConverterTorqueRatio] = 1.0;
				container[ModalResultField.TorqueConverterSpeedRatio] = 1.0;
			} else {
				container[ModalResultField.TorqueConverterTorqueRatio] = CurrentState.OperatingPoint.TorqueRatio;
				container[ModalResultField.TorqueConverterSpeedRatio] = CurrentState.OperatingPoint.SpeedRatio;
			}
			container[ModalResultField.TC_TorqueIn] = CurrentState.InTorque;
			container[ModalResultField.TC_TorqueOut] = CurrentState.OutTorque;
			container[ModalResultField.TC_angularSpeedIn] = CurrentState.InAngularVelocity;
			container[ModalResultField.TC_angularSpeedOut] = CurrentState.OutAngularVelocity;

			var avgOutVelocity = (PreviousState.OutAngularVelocity + CurrentState.OutAngularVelocity) / 2.0;
			var avgInVelocity = (PreviousState.InAngularVelocity + CurrentState.InAngularVelocity) / 2.0;
			container[ModalResultField.P_TC_out] = CurrentState.OutTorque * avgOutVelocity;
			container[ModalResultField.P_TC_loss] = CurrentState.InTorque * avgInVelocity -
													CurrentState.OutTorque * avgOutVelocity;
		}

		protected override void DoCommitSimulationStep()
		{
			//SearchingTcOperatingPoint = false;
			AdvanceState();
		}

		public void Locked(NewtonMeter outTorque, PerSecond outAngularVelocity)
		{
			CurrentState.SetState(outTorque, outAngularVelocity, outTorque, outAngularVelocity);
		}

		public class TorqueConverterComponentState : SimpleComponentState
		{
			public TorqueConverterOperatingPoint OperatingPoint;

		    public Watt InPower()
		    {
		        return InAngularVelocity * InTorque;
		    }
		}
	}
}