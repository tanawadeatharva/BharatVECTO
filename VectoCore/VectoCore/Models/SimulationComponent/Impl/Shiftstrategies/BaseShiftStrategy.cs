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

using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
    public abstract class BaseShiftStrategy<T> : LoggingObject, IShiftStrategy where T : class, IGearbox
	{
		protected readonly IVehicleContainer DataBus;
		protected readonly GearboxData GearboxModelData;
		protected readonly ShiftStrategyParameters GearshiftParams;
		protected readonly VectoRunData RunData;

        protected readonly GearList Gears;

		protected T _gearbox;

		protected GearshiftPosition _nextGear;

        protected ISimplePowertrainBuilder PowertrainBuilder { get; private set; }

		protected BaseShiftStrategy(IVehicleContainer dataBus)
		{
			VelocityDropData = new VelocityRollingLookup();
			PowertrainBuilder = dataBus.SimplePowertrainBuilder;
			GearboxModelData = dataBus.RunData.GearboxData;
			GearshiftParams = dataBus.RunData.GearshiftParameters;
			DataBus = dataBus;
			RunData = dataBus.RunData;

			Gears = GearboxModelData.GearList;
		}

		public virtual IGearbox Gearbox {
			get => _gearbox;
			set {
				var myGearbox = value as T;
				if (myGearbox == null) {
					throw new VectoException("This shift strategy can't handle gearbox of type {0}", value.GetType());
				}
				_gearbox = myGearbox;
			}
		}

        public virtual bool ShiftRequired(Second absTime, Second dt, NewtonMeter outTorque,
			PerSecond outAngularVelocity, NewtonMeter inTorque, PerSecond inAngularVelocity, GearshiftPosition gear,
			Second lastShiftTime, IResponse response)
		{
			CheckGearshiftRequired = true;
			var retVal = DoCheckShiftRequired(absTime, dt, outTorque, outAngularVelocity, inTorque, inAngularVelocity, gear, lastShiftTime, response);
			CheckGearshiftRequired = false;
			return retVal;
		}

		protected abstract bool DoCheckShiftRequired(Second absTime, Second dt, NewtonMeter outTorque,
			PerSecond outAngularVelocity, NewtonMeter inTorque, PerSecond inAngularVelocity, GearshiftPosition gear,
			Second lastShiftTime, IResponse response);

		public abstract GearshiftPosition InitGear(Second absTime, Second dt, NewtonMeter torque, PerSecond outAngularVelocity);
		
		public abstract GearshiftPosition Engage(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity);
		
		public abstract void Disengage(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity);

		public virtual GearshiftPosition NextGear => _nextGear;

		public bool CheckGearshiftRequired { get; protected set; }
		
		public GearshiftPosition MaxStartGear { get; protected set; }

		public VelocityRollingLookup VelocityDropData { get; protected set; }

		public virtual void Request(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity)
		{ }

		public virtual void WriteModalResults(IModalDataContainer container)
		{ }

        #region Helper Functions ICE operating point vs. Upshift/Downshift curve

		/// <summary>
		/// Tests if the operating point is below the down-shift curve (=outside of shift curve).
		/// </summary>
		/// <param name="gear">The gear.</param>
		/// <param name="inTorque">The in torque.</param>
		/// <param name="inEngineSpeed">The in engine speed.</param>
		/// <returns><c>true</c> if the operating point is below the down-shift curv; otherwise, <c>false</c>.</returns>
		protected virtual bool IsBelowDownShiftCurve(GearshiftPosition gear, NewtonMeter inTorque, PerSecond inEngineSpeed)
		{
			if (!Gears.HasPredecessor(gear)) {
				return false;
			}
			return GearboxModelData.Gears[gear.Gear].ShiftPolygon.IsBelowDownshiftCurve(inTorque, inEngineSpeed);
		}

        /// <summary>
        /// Tests if the operating point is below the down-shift curve (=outside of shift curve).
        /// </summary>
        /// <param name="gear">The gear.</param>
        /// <param name="inTorque">The in torque.</param>
        /// <param name="inEngineSpeed">The in engine speed.</param>
        /// <returns><c>true</c> if the operating point is below the down-shift curve; otherwise, <c>false</c>.</returns>
        protected virtual bool IsAboveDownShiftCurve(GearshiftPosition gear, NewtonMeter inTorque, PerSecond inEngineSpeed)
		{
			if (!Gears.HasPredecessor(gear)) {
				return true;
			}
			return GearboxModelData.Gears[gear.Gear].ShiftPolygon.IsAboveDownshiftCurve(inTorque, inEngineSpeed);
		}

		/// <summary>
		/// Tests if the operating point is below the extended down-shift curve (=outside of shift curve).
		/// </summary>
		/// <param name="gear">The gear.</param>
		/// <param name="inTorque">The in torque.</param>
		/// <param name="inEngineSpeed">The in engine speed.</param>
		/// <returns><c>true</c> if the operating point is below the extended down-shift curve; otherwise, <c>false</c>.</returns>
		protected virtual bool IsBelowExtendedDownShiftCurve(GearshiftPosition gear, NewtonMeter inTorque, PerSecond inEngineSpeed)
		{
			if (!Gears.HasPredecessor(gear)) {
				return false;
			}
			return GearboxModelData.Gears[gear.Gear].ExtendedShiftPolygon.IsBelowDownshiftCurve(inTorque, inEngineSpeed);
		}

		/// <summary>
		/// Tests if the operating point is above the down-shift curve (=outside of shift curve).
		/// </summary>
		/// <param name="gear">The gear.</param>
		/// <param name="inTorque">The in torque.</param>
		/// <param name="inEngineSpeed">The in engine speed.</param>
		/// <returns><c>true</c> if the operating point is above the extended down-shift curve; otherwise, <c>false</c>.</returns>
		protected virtual bool IsAboveExtendedDownShiftCurve(GearshiftPosition gear, NewtonMeter inTorque, PerSecond inEngineSpeed)
		{
			if (!Gears.HasPredecessor(gear)) {
				return true;
			}
			return GearboxModelData.Gears[gear.Gear].ExtendedShiftPolygon.IsAboveDownshiftCurve(inTorque, inEngineSpeed);
		}

		/// <summary>
		/// Tests if the operating point is above the up-shift curve (=outside of shift curve).
		/// </summary>
		/// <param name="gear">The gear.</param>
		/// <param name="inTorque">The in torque.</param>
		/// <param name="inEngineSpeed">The in engine speed.</param>
		/// <returns><c>true</c> if the operating point is above the up-shift curve; otherwise, <c>false</c>.</returns>
		protected virtual bool IsAboveUpShiftCurve(GearshiftPosition gear, NewtonMeter inTorque, PerSecond inEngineSpeed)
		{
			if (!Gears.HasSuccessor(gear)) {
				return false;
			}
			return GearboxModelData.Gears[gear.Gear].ShiftPolygon.IsAboveUpshiftCurve(inTorque, inEngineSpeed);
		}

		/// <summary>
		/// Tests if the operating point is velow the up-shift curve (=outside of shift curve).
		/// </summary>
		/// <param name="gear">The gear.</param>
		/// <param name="inTorque">The in torque.</param>
		/// <param name="inEngineSpeed">The in engine speed.</param>
		/// <returns><c>true</c> if the operating point is below the up-shift curve; otherwise, <c>false</c>.</returns>
		protected virtual bool IsBelowUpShiftCurve(GearshiftPosition gear, NewtonMeter inTorque, PerSecond inEngineSpeed)
		{
			if (!Gears.HasSuccessor(gear)) {
				return true;
			}
			return GearboxModelData.Gears[gear.Gear].ShiftPolygon.IsBelowUpshiftCurve(inTorque, inEngineSpeed);
		}

		#endregion

		#region Helper Functions checking engine speed

		protected virtual bool SpeedTooLowForEngine(GearshiftPosition gear, PerSecond outAngularSpeed) =>
			(outAngularSpeed * GearboxModelData.Gears[gear.Gear].Ratio).IsSmaller(DataBus.EngineInfo.EngineIdleSpeed);

		protected virtual bool SpeedTooHighForEngine(GearshiftPosition gear, PerSecond outAngularSpeed) =>
			(outAngularSpeed * GearboxModelData.Gears[gear.Gear].Ratio).IsGreaterOrEqual(VectoMath.Min(
				GearboxModelData.Gears[gear.Gear].MaxSpeed,
				DataBus.EngineInfo.EngineN95hSpeed - 1.RPMtoRad()));

		#endregion

		protected MeterPerSquareSecond EstimateAccelerationForGear(GearshiftPosition gear, PerSecond gbxAngularVelocityOut)
		{
			if (!Gears.Contains(gear)) {
				throw new VectoSimulationException("EstimateAccelerationForGear: invalid gear: {0}", gear);
			}

			var vehicleSpeed = DataBus.VehicleInfo.VehicleSpeed;

			var nextEngineSpeed = gbxAngularVelocityOut * GearboxModelData.Gears[gear.Gear].Ratio;
			var maxEnginePower = DataBus.EngineInfo.EngineStationaryFullPower(nextEngineSpeed);

			var avgSlope =
				((DataBus.DrivingCycleInfo.CycleLookAhead(Constants.SimulationSettings.GearboxLookaheadForAccelerationEstimation).Altitude -
				DataBus.DrivingCycleInfo.Altitude) / Constants.SimulationSettings.GearboxLookaheadForAccelerationEstimation).Value().SI<Radian>();

			var airDragLoss = DataBus.VehicleInfo.AirDragResistance(vehicleSpeed, vehicleSpeed) * DataBus.VehicleInfo.VehicleSpeed;
			var rollResistanceLoss = DataBus.VehicleInfo.RollingResistance(avgSlope) * DataBus.VehicleInfo.VehicleSpeed;
			var gearboxLoss = GearboxModelData.Gears[gear.Gear].LossMap.GetTorqueLoss(gbxAngularVelocityOut,
				maxEnginePower / nextEngineSpeed * GearboxModelData.Gears[gear.Gear].Ratio).Value * nextEngineSpeed;
			//DataBus.GearboxLoss();
			var slopeLoss = DataBus.VehicleInfo.SlopeResistance(avgSlope) * DataBus.VehicleInfo.VehicleSpeed;
			var axleLoss = DataBus.AxlegearInfo.AxlegearLoss();

			var accelerationPower = maxEnginePower - gearboxLoss - axleLoss - airDragLoss - rollResistanceLoss - slopeLoss;

			var acceleration = accelerationPower / DataBus.VehicleInfo.VehicleSpeed / (DataBus.VehicleInfo.TotalMass + DataBus.WheelsInfo.ReducedMassWheels);

			return acceleration.Cast<MeterPerSquareSecond>();
		}

	}
}