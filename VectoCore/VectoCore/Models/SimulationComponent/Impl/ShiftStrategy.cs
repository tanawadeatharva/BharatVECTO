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
using System.Drawing.Design;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
	/// <summary>
	/// Class ShiftStrategy is a base class for shift strategies. Implements some helper methods for checking the shift curves.
	/// </summary>
	public abstract class ShiftStrategy : IShiftStrategy
	{
		protected IDataBus DataBus;

		protected GearboxData Data;

		protected ShiftStrategy(GearboxData data, IDataBus dataBus)
		{
			DataBus = dataBus;
			Data = data;
		}

		public abstract uint Engage(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outEngineSpeed);

		public abstract void Disengage(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outEngineSpeed);

		public abstract bool ShiftRequired(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity,
			NewtonMeter inTorque, PerSecond inAngularSpeed, uint gear, Second lastShiftTime);

		public abstract uint InitGear(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outEngineSpeed);

		public Gearbox Gearbox { get; set; }

		protected MeterPerSquareSecond EstimateAccelerationForGear(uint gear, PerSecond gbxAngularVelocityOut)
		{
			if (gear == 0 || gear > Gearbox.ModelData.Gears.Count) {
				throw new VectoSimulationException("invalid gear: {0}", gear);
			}

			var vehicleSpeed = DataBus.VehicleSpeed;

			var nextEngineSpeed = gbxAngularVelocityOut * Gearbox.ModelData.Gears[gear].Ratio;
			var maxEnginePower = DataBus.EngineStationaryFullPower(nextEngineSpeed);

			var avgSlope =
				((DataBus.CycleLookAhead(100.SI<Meter>()).Altitude - DataBus.Altitude) / 100.SI<Meter>()).Value().SI<Radian>();

			var airDragLoss = DataBus.AirDragResistance(vehicleSpeed, vehicleSpeed) * DataBus.VehicleSpeed;
			var rollResistanceLoss = DataBus.RollingResistance(avgSlope) * DataBus.VehicleSpeed;
			var gearboxLoss = Gearbox.ModelData.Gears[gear].LossMap.GetTorqueLoss(gbxAngularVelocityOut,
				maxEnginePower / nextEngineSpeed * Gearbox.ModelData.Gears[gear].Ratio).Value * nextEngineSpeed;
			//DataBus.GearboxLoss();
			var slopeLoss = DataBus.SlopeResistance(avgSlope) * DataBus.VehicleSpeed;
			var axleLoss = DataBus.AxlegearLoss();

			var accelerationPower = maxEnginePower - gearboxLoss - axleLoss - airDragLoss - rollResistanceLoss - slopeLoss;

			var acceleration = accelerationPower / DataBus.VehicleSpeed / (DataBus.TotalMass + DataBus.ReducedMassWheels);

			return acceleration.Cast<MeterPerSquareSecond>();
		}


		/// <summary>
		/// Tests if the operating point is below the down-shift curve (=outside of shift curve).
		/// </summary>
		/// <param name="gear">The gear.</param>
		/// <param name="inTorque">The in torque.</param>
		/// <param name="inEngineSpeed">The in engine speed.</param>
		/// <returns><c>true</c> if the operating point is below the down-shift curv; otherwise, <c>false</c>.</returns>
		protected virtual bool IsBelowDownShiftCurve(uint gear, NewtonMeter inTorque, PerSecond inEngineSpeed)
		{
			if (gear <= 1) {
				return false;
			}

			var downSection = Data.Gears[gear].ShiftPolygon.Downshift.GetSection(entry => entry.AngularSpeed < inEngineSpeed);
			if (downSection.Item2.AngularSpeed < inEngineSpeed) {
				return false;
			}

			return ShiftPolygon.IsLeftOf(inEngineSpeed, inTorque, downSection);
		}

		/// <summary>
		/// Tests if the operating point is above the up-shift curve (=outside of shift curve).
		/// </summary>
		/// <param name="gear">The gear.</param>
		/// <param name="inTorque">The in torque.</param>
		/// <param name="inEngineSpeed">The in engine speed.</param>
		/// <returns><c>true</c> if the operating point is above the up-shift curve; otherwise, <c>false</c>.</returns>
		protected virtual bool IsAboveUpShiftCurve(uint gear, NewtonMeter inTorque, PerSecond inEngineSpeed)
		{
			if (gear >= Data.Gears.Count) {
				return false;
			}

			var upSection = Data.Gears[gear].ShiftPolygon.Upshift.GetSection(entry => entry.AngularSpeed < inEngineSpeed);

			if (upSection.Item2.AngularSpeed < inEngineSpeed) {
				return true;
			}

			return ShiftPolygon.IsRightOf(inEngineSpeed, inTorque, upSection);
		}
	}
}