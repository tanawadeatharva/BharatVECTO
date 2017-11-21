/*
* This file is part of VECTO.
*
* Copyright © 2012-2017 European Union
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
using System.Linq;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
	internal class VTPCycle : PWheelCycle
	{
		private uint StartGear;

		public VTPCycle(VehicleContainer container, IDrivingCycleData cycle, double axleGearRatio,
			VehicleData vehicleData, Dictionary<uint, double> gearRatios) : base(container, cycle) { }

		public override IResponse Initialize()
		{
			SelectStartGear();
			return base.Initialize();
		}

		private void SelectStartGear()
		{
			var transmissionRatio = RunData.AxleGearData.AxleGear.Ratio *
									(RunData.AngledriveData == null ? 1.0 : RunData.AngledriveData.Angledrive.Ratio) /
									RunData.VehicleData.DynamicTyreRadius;
			var cardanStartSpeed = (RunData.GearboxData.StartSpeed * transmissionRatio).Cast<PerSecond>();
			var minEngineSpeed = (RunData.EngineData.FullLoadCurves[0].RatedSpeed - RunData.EngineData.IdleSpeed) *
								Constants.SimulationSettings.ClutchClosingSpeedNorm + RunData.EngineData.IdleSpeed;
			var wheelStartTorque =
				(RunData.VehicleData.VehicleCategory == VehicleCategory.Tractor
					? 40000.SI<Kilogram>()
					: RunData.VehicleData.GrossVehicleWeight) * RunData.GearboxData.StartAcceleration *
				RunData.VehicleData.DynamicTyreRadius;
			var wheelStartSpeed = RunData.GearboxData.StartSpeed / RunData.VehicleData.DynamicTyreRadius;
			CycleIterator.LeftSample.WheelAngularVelocity = wheelStartSpeed;
			var maxStartGear = 1u;
			foreach (var gearData in RunData.GearboxData.Gears.Reverse())
				if (cardanStartSpeed * gearData.Value.Ratio > minEngineSpeed) {
					maxStartGear = gearData.Key;
					break;
				}
			for (var gear = maxStartGear; gear > 1; gear--) {
				var inAngularSpeed = cardanStartSpeed * RunData.GearboxData.Gears[gear].Ratio;

				var ratedSpeed = DataBus.EngineRatedSpeed;
				if (inAngularSpeed > ratedSpeed || inAngularSpeed.IsEqual(0))
					continue;

				var response = Initialize(gear, wheelStartTorque, wheelStartSpeed);

				var fullLoadPower = response.DynamicFullLoadPower; //EnginePowerRequest - response.DeltaFullLoad;
				var reserve = 1 - response.EnginePowerRequest / fullLoadPower;

				if (response.EngineSpeed > DataBus.EngineIdleSpeed && reserve >= RunData.GearboxData.StartTorqueReserve) {
					StartGear = gear;
					return;
				}
			}
			StartGear = 1;
		}

		internal ResponseDryRun Initialize(uint gear, NewtonMeter outTorque, PerSecond outAngularVelocity)
		{
			CycleIterator.RightSample.Gear = gear;
			//var inAngularVelocity = outAngularVelocity * RunData.GearboxData.Gears[gear].Ratio;
			//var torqueLossResult = RunData.GearboxData.Gears[gear].LossMap.GetTorqueLoss(outAngularVelocity, outTorque);
			//var inTorque = outTorque / RunData.GearboxData.Gears[gear].Ratio + torqueLossResult.Value;

			var response =
				(ResponseDryRun)
				NextComponent.Request(0.SI<Second>(), Constants.SimulationSettings.TargetTimeInterval, outTorque,
					outAngularVelocity, true);

			//var fullLoad = DataBus.EngineStationaryFullPower(inAngularVelocity);

			return new ResponseDryRun {
				Source = this,
				EnginePowerRequest = response.EnginePowerRequest,
				EngineSpeed = response.EngineSpeed,
				DynamicFullLoadPower = response.DynamicFullLoadPower,
				ClutchPowerRequest = response.ClutchPowerRequest,
				GearboxPowerRequest = outTorque * outAngularVelocity,
				//DeltaFullLoad = response.EnginePowerRequest - fullLoad
			};
		}

		protected override void InitializeCycleData()
		{
			FirstRun = false;
			var minEngineSpeed = (RunData.EngineData.FullLoadCurves[0].RatedSpeed - RunData.EngineData.IdleSpeed) *
								Constants.SimulationSettings.ClutchClosingSpeedNorm + RunData.EngineData.IdleSpeed;

			var gearRatios = RunData.GearboxData.Gears.ToDictionary(g => g.Key, g => g.Value.Ratio);

			var stopped = false;

			foreach (var entry in Data.Entries) {
				stopped = stopped || entry.VehicleTargetSpeed.IsEqual(0.KMPHtoMeterPerSecond(),
							0.3.KMPHtoMeterPerSecond());
				entry.AngularVelocity =
					entry.VehicleTargetSpeed.IsEqual(0.KMPHtoMeterPerSecond(), 0.3.KMPHtoMeterPerSecond())
						? 0.RPMtoRad()
						: entry.WheelAngularVelocity;

				var cardanSpeed = entry.WheelAngularVelocity *
								RunData.AxleGearData.AxleGear.Ratio * (RunData.AngledriveData?.Angledrive.Ratio ?? 1);
				if (cardanSpeed.IsEqual(0.RPMtoRad(), 1.RPMtoRad())) {
					entry.Gear = 0;
					continue;
				}
				var ratio = (entry.EngineSpeed / cardanSpeed).Value();
				var gear = gearRatios.Aggregate((x, y) =>
					Math.Abs(x.Value / ratio - 1) < Math.Abs(y.Value / ratio - 1) ? x : y).Key;


				//entry.Gear = entry.EngineSpeed < (RunData.EngineData.IdleSpeed + 50.RPMtoRad()) && entry.VehicleTargetSpeed < 5.KMPHtoMeterPerSecond() ? 0 :  gear;
				if (stopped && gear <= StartGear) {
					entry.Gear = entry.VehicleTargetSpeed.IsEqual(0.KMPHtoMeterPerSecond(),
						0.3.KMPHtoMeterPerSecond())
						? 0
						: StartGear;
				} else {
					entry.Gear = gear == 1 && cardanSpeed * gearRatios[1] <= minEngineSpeed ? 0 : gear;
				}
				if (gear > StartGear)
					stopped = false;
			}
		}

		public override bool VehicleStopped
		{
			get
			{
				if (CycleIterator.LeftSample.Gear == 0)
					return true;
				if (CycleIterator.LeftSample.Gear != StartGear)
					return false;

				var transmissionRatio = RunData.AxleGearData.AxleGear.Ratio *
										(RunData.AngledriveData?.Angledrive.Ratio ?? 1.0);
				return CycleIterator.LeftSample.WheelAngularVelocity * transmissionRatio *
						RunData.GearboxData.Gears[CycleIterator.LeftSample.Gear].Ratio < DataBus.EngineIdleSpeed;
				//return CycleIterator.LeftSample.VehicleTargetSpeed.IsEqual(0.KMPHtoMeterPerSecond(),
				//    0.3.KMPHtoMeterPerSecond());
			}
		}

		protected override void DoWriteModalResults(IModalDataContainer container)
		{
			base.DoWriteModalResults(container);
			container[ModalResultField.v_act] = CycleIterator.LeftSample.VehicleTargetSpeed;
		}
	}
}