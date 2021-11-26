using System;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Cryptography;
using NLog.LayoutRenderers;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.Connector.Ports;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
	
	public class ElectricMotor : StatefulProviderComponent<ElectricMotorState, ITnOutPort, ITnInPort, ITnOutPort>, IPowerTrainComponent, IElectricMotor, ITnOutPort, ITnInPort
	{

		protected IElectricSystem ElectricPower;
		internal IElectricMotorControl Control { get; }
		protected ElectricMotorData ModelData;
		private PerSecond _maxSpeed;

		protected internal Joule ThermalBuffer = 0.SI<Joule>();
		protected internal bool DeRatingActive;
		
		public Joule OverloadBuffer { get; }
		public NewtonMeter ContinuousTorque { get; }

		public Watt ContinuousPowerLoss { get; }

		public BusAuxiliariesAdapter BusAux { protected get; set; }

		public ElectricMotor(IVehicleContainer container, ElectricMotorData data, IElectricMotorControl control, PowertrainPosition position) : base(container)
		{
			Control = control;
			ModelData = data;
			Position = position;

			if (Position == PowertrainPosition.HybridP2_5) {
				TransmissionRatioPerGear = ModelData.RatioPerGear;
				if (TransmissionRatioPerGear == null ||
					TransmissionRatioPerGear.Length < container.RunData.GearboxData.GearList.Count()) {
					throw new VectoException("For powertrain configuration P2.5 a EM ratio for every gear has to be provided!");
				}
			}

			container.AddComponent(this); // We have to do this again because in the base class the position is unknown!

			ContinuousTorque = ModelData.ContinuousTorque;
			var voltage = ModelData.EfficiencyData.VoltageLevels.First().Voltage;
			var contElPwr =
				ModelData.EfficiencyData.LookupElectricPower(voltage, ModelData.ContinuousTorqueSpeed, -ContinuousTorque).ElectricalPower ??
				ModelData.EfficiencyData.LookupElectricPower(voltage, ModelData.ContinuousTorqueSpeed, ModelData.EfficiencyData.FullLoadDriveTorque(voltage, ModelData.ContinuousTorqueSpeed), true).ElectricalPower;
			ContinuousPowerLoss = -contElPwr - ContinuousTorque * ModelData.ContinuousTorqueSpeed; // loss needs to be positive
			
			var peakElPwr = ModelData.EfficiencyData.LookupElectricPower(voltage, ModelData.OverloadTestSpeed, -ModelData.OverloadTorque, true)
				.ElectricalPower;
			var peakPwrLoss = -peakElPwr - ModelData.OverloadTorque * ModelData.OverloadTestSpeed; // losses need to be positive

			OverloadBuffer = (peakPwrLoss - ContinuousPowerLoss) * ModelData.OverloadTime;
			if (OverloadBuffer.IsSmallerOrEqual(0) && !(container is SimplePowertrainContainer)) {
				Log.Error("Overload buffer for thermal de-rating is negative! Please check electric motor data!");
			}
		}

		public double[] TransmissionRatioPerGear { get; set; }

		public PowertrainPosition Position { get; }
		public PerSecond MaxSpeed => _maxSpeed ?? (_maxSpeed = ModelData.EfficiencyData.MaxSpeed / ModelData.RatioADC);

		public Watt DragPower(Volt volt, PerSecond electricMotorSpeed)
		{
			return ModelData.DragCurve.Lookup(electricMotorSpeed) * electricMotorSpeed;
		}

		public Watt MaxPowerDrive(Volt volt, PerSecond electricMotorSpeed)
		{
			return ModelData.EfficiencyData.FullLoadDriveTorque(volt, electricMotorSpeed) * electricMotorSpeed;
		}

		public NewtonMeter GetTorqueForElectricPower(Volt volt, Watt electricPower, PerSecond avgEmSpeed, Second dt)
		{
			var maxTorque = electricPower > 0
				? GetMaxRecuperationTorque(volt, dt, avgEmSpeed)
				: GetMaxDriveTorque(volt, dt, avgEmSpeed);
			var tqEmMap = ModelData.EfficiencyData.EfficiencyMapLookupTorque(volt, electricPower, avgEmSpeed, maxTorque);
			if (tqEmMap == null) {
				return null;
			}

			var emSpeed = avgEmSpeed * 2 - PreviousState.EMSpeed;

			var tqInertia = Formulas.InertiaPower(emSpeed, PreviousState.EMSpeed, ModelData.Inertia, dt) / avgEmSpeed;
			var tqEm = tqEmMap + tqInertia;
			var tqDt = ConvertEmTorqueToDrivetrain(avgEmSpeed, tqEm);
			return tqDt;

		}

		public IResponse Initialize(NewtonMeter outTorque, PerSecond outAngularVelocity)
		{
			var emOutAngularVelocity = outAngularVelocity * ModelData.RatioADC;
			var emOutTorque = outTorque / ModelData.RatioADC;

			PreviousState.EMSpeed = emOutAngularVelocity;
			PreviousState.EMTorque = 0.SI<NewtonMeter>();

			PreviousState.DrivetrainSpeed = outAngularVelocity;
			PreviousState.DrivetrainOutTorque = outTorque;

			if (NextComponent == null) {
				var busAuxPwr = BusAux?.Initialize(0.SI<NewtonMeter>(), 1.RPMtoRad()) ?? 0.SI<NewtonMeter>();
				if (!busAuxPwr.IsEqual(0)) {
					Log.Warn("Check BusAux config for PEV!");
				}

				return new ResponseSuccess(this) {
					Engine = {
						PowerRequest = emOutTorque * emOutAngularVelocity,
						EngineSpeed = emOutAngularVelocity
					}
				};
			}
			if (!DataBus.EngineCtl.CombustionEngineOn) {
				PreviousState.DrivetrainInTorque = 0.SI<NewtonMeter>();
				//PreviousState.InAngularVelocity = emOutAngularVelocity;
			}
			return NextComponent.Initialize(outTorque, outAngularVelocity);
			//return NextComponent.Initialize(PreviousState.InTorque, PreviousState.InAngularVelocity);
		}

		/// <summary>
		/// If the electric motor is operated in generator mode, it adds positive torque to the request for the next component,
		/// if the electric motor is operated in drive mode, it adds negative torque (i.e., supports the next component)
		/// </summary>
		/// <param name="absTime"></param>
		/// <param name="dt"></param>
		/// <param name="outTorque"></param>
		/// <param name="outAngularVelocity"></param>
		/// <param name="dryRun"></param>
		/// <returns></returns>
		public IResponse Request(
			Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, bool dryRun = false)
		{
			if (TransmissionRatioPerGear == null) {
				return DoHandleRequest(absTime, dt, outTorque, outAngularVelocity, dryRun, 1.0);
			}

			var gear = DataBus.GearboxInfo.Gear;
			if (gear.Gear == 0) {
				gear = DataBus.GearboxInfo.NextGear;
			}
			var ratio = TransmissionRatioPerGear[gear.Gear - 1];
			return DoHandleRequest(absTime, dt, outTorque * ratio, outAngularVelocity / ratio, dryRun, ratio);
		}

		public IResponse DoHandleRequest(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity,
			bool dryRun, double ratio)
		{
			var voltage = DataBus.BatteryInfo.InternalVoltage;

			var iceOn =  Position == PowertrainPosition.HybridP1 &&
						!DataBus.EngineInfo.EngineOn && DataBus.EngineCtl.CombustionEngineOn;
			var prevDtSpeed = iceOn ? DataBus.EngineInfo.EngineSpeed : PreviousState.DrivetrainSpeed;
			var prevEmSpeed = iceOn ? prevDtSpeed * ModelData.RatioADC : PreviousState.EMSpeed;

			var avgDtSpeed = (prevDtSpeed + outAngularVelocity) / 2;
			var emSpeed = outAngularVelocity * ModelData.RatioADC;

			var avgEmSpeed = (prevEmSpeed + emSpeed) / 2;
			var inertiaTorqueEm = avgEmSpeed.IsEqual(0)
				? 0.SI<NewtonMeter>()
				: Formulas.InertiaPower(avgEmSpeed, PreviousState.EMSpeed, ModelData.Inertia, dt) / avgEmSpeed;

			var maxDriveTorqueEmMap = GetMaxDriveTorque(voltage, dt, avgEmSpeed);
			var maxRecuperationTorqueEmMap = GetMaxRecuperationTorque(voltage, dt, avgEmSpeed);

			// inertia has to be added here. drive torque is negative, when accelerating inertia is positive and thus 'reduces' drive torque, i.e 'less negative'
			var maxDriveTorqueEm = maxDriveTorqueEmMap == null ? null : maxDriveTorqueEmMap + inertiaTorqueEm;
			// inertia has to be added here. recuperation torque is positive, when accelerating inertia is positive and adds more drag to the drivetrain, 
			var maxRecuperationTorqueEm = maxRecuperationTorqueEmMap == null ? null : maxRecuperationTorqueEmMap + inertiaTorqueEm;
			if (maxDriveTorqueEm != null && maxDriveTorqueEm.IsGreaterOrEqual(0)) {
				maxDriveTorqueEm = null;
			}
			if (maxRecuperationTorqueEm != null && maxRecuperationTorqueEm.IsSmallerOrEqual(0)) {
				// max recuperation torque may get negative due to torque losses
				maxRecuperationTorqueEm = null;
			}

			var maxDriveTorqueDt = maxDriveTorqueEm == null ? null : ConvertEmTorqueToDrivetrain(avgEmSpeed, maxDriveTorqueEm);
			var maxRecuperationTorqueDt = maxRecuperationTorqueEm == null ? null : ConvertEmTorqueToDrivetrain(avgEmSpeed, maxRecuperationTorqueEm);
			
			// control returns torque that shall be applied on the drivetrain. calculate backward to the EM
			var emTorqueDt = Control.MechanicalAssistPower(absTime, dt, outTorque,
				PreviousState.DrivetrainSpeed, outAngularVelocity, maxDriveTorqueDt, maxRecuperationTorqueDt, Position, dryRun);

			var emTorque = emTorqueDt == null ? null : ConvertDrivetrainTorqueToEm(avgDtSpeed, emTorqueDt);
			var emOff = emTorqueDt == null;

			if (!dryRun && !DataBus.IsTestPowertrain && emTorqueDt != null && NextComponent != null && (emTorque.IsSmaller(maxDriveTorqueEm ?? 0.SI<NewtonMeter>(), 1e-3) ||
																		emTorque.IsGreater(maxRecuperationTorqueEm ?? 0.SI<NewtonMeter>(), 1e-3))) {
				// check if provided EM torque (drivetrain) is valid)
				if ((!avgDtSpeed.IsEqual(DataBus.HybridControllerInfo.ElectricMotorSpeed(Position) / ModelData.RatioADC) ||
															!dt.IsEqual(DataBus.HybridControllerInfo.SimulationInterval))) {
					return new ResponseInvalidOperatingPoint(this);
				}
				throw new VectoException(
					"Invalid operating point provided by strategy! EM Torque: {0}, max Drive Torque: {1}, min Recup Torque: {2}",
					emTorqueDt, maxDriveTorqueDt, maxRecuperationTorqueDt);
			}


			
			if ((Position == PowertrainPosition.HybridP2 || Position == PowertrainPosition.HybridP2_5 ) && !DataBus.GearboxInfo.GearEngaged(absTime)) {
				// electric motor is between gearbox and clutch, but no gear is engaged...
				if (emTorque != null) {
					if (!DataBus.HybridControllerInfo.GearboxEngaged || (DataBus.HybridControllerInfo.GearboxEngaged && !DataBus.GearboxInfo.GearEngaged(absTime))) {
						return new ResponseInvalidOperatingPoint(this) {
							ElectricMotor = {
								MaxDriveTorque = maxDriveTorqueDt,
								MaxRecuperationTorque = maxRecuperationTorqueDt,
								AngularVelocity = avgDtSpeed, // avgemspeed??
								AvgDrivetrainSpeed = avgDtSpeed,
								PowerRequest = outTorque * avgDtSpeed
							}
						};
					}

					if (!dryRun) {
						throw new VectoSimulationException(
							"electric motor cannot provide torque when gearbox and clutch are disengaged");
					}
				}
				// gearbox is disengaged - ignore em inertia and drag...
				emTorqueDt = 0.SI<NewtonMeter>();
				emTorque = 0.SI<NewtonMeter>();
			}

			if (Position == PowertrainPosition.BatteryElectricE2 && !DataBus.GearboxInfo.GearEngaged(absTime)) {
				// electric motor is after the gearbox but no gear engaged - ignore inertia and drag...
				emTorqueDt = 0.SI<NewtonMeter>();
				emTorque = 0.SI<NewtonMeter>();
			}

			if (Position == PowertrainPosition.HybridP1 && !DataBus.EngineCtl.CombustionEngineOn) {
				// electric motor is directly connected to the ICE, ICE is off and EM is off - do not apply drag loss
				emTorqueDt = 0.SI<NewtonMeter>();
				emTorque = 0.SI<NewtonMeter>();
				emOff = true;
			}

			if (ElectricPower == null || emTorqueDt == null) {
				// no electric system or EM shall be off - apply drag only
				// if EM is off, calculate EM drag torque 'forward' to be applied on drivetrain
				// add inertia, drag is positive
				emTorque =  ModelData.DragCurve.Lookup(avgEmSpeed) + inertiaTorqueEm;
				emTorqueDt = ConvertEmTorqueToDrivetrain(avgEmSpeed, emTorque);
				emOff = true;
			}

			// inertia torque 'brakes' - electric motor has to provide this torque in addition (T_inertia > 0 when angular speed increases)
			// emTorque < 0 when propelling, emTorqueMap needs to be 'more negative' to provide torque for inertia
			// emTorque > 0 when recuperating, inertia 'brakes' in addition, emTorqueMap is decreased
			var emTorqueMap = emTorque - inertiaTorqueEm ;
			if (emOff) {
				// not used later 
				emTorqueMap = null;
			}

			var electricPower = emOff || (ModelData.DragCurve.Lookup(avgEmSpeed) + inertiaTorqueEm).IsEqual(emTorque, 1e-3.SI<NewtonMeter>())
				? 0.SI<Watt>()
				: ModelData.EfficiencyData
					.LookupElectricPower(voltage, avgEmSpeed, emTorqueMap, DataBus.ExecutionMode != ExecutionMode.Declaration)
					.ElectricalPower;

			var electricSupplyResponse =
				ElectricPower.Request(absTime, dt, electricPower, dryRun);
			//if (!dryRun && !DataBus.IsTestPowertrain && BatteryElectricPowertrain && electricSupplyResponse is ElectricSystemUnderloadResponse) {
			//	return new ResponseBatteryEmpty(this, electricSupplyResponse);
			//}
			if (NextComponent != null && !dryRun && !DataBus.IsTestPowertrain && !emOff && !(electricSupplyResponse is ElectricSystemResponseSuccess)) {
				if ( !avgEmSpeed.IsEqual(DataBus.HybridControllerInfo.ElectricMotorSpeed(Position) / ModelData.RatioADC)) {
					return new ResponseInvalidOperatingPoint(this);
				}
				throw new VectoException(
					"Invalid operating point provided by strategy! EM Torque: {0}, req. electric Power: {1}, battery demand motor: {3}, max Power from Battery: {2}",
					emTorque, electricPower,
					emTorque < 0 ? electricSupplyResponse.MaxPowerDrive : electricSupplyResponse.MaxPowerDrag, electricSupplyResponse.ConsumerPower);
			}

			var inTorqueDt = outTorque + emTorqueDt;

			IResponse retVal;
			if (NextComponent == null) {
				if (electricSupplyResponse.MaxPowerDrive.IsEqual(0.SI<Watt>(), 100.SI<Watt>())) {
					retVal = new ResponseBatteryEmpty(this, electricSupplyResponse);
					return retVal;
				}
				// electric motor only
				var remainingPower = inTorqueDt * avgDtSpeed;
				if (dryRun) {
					retVal = new ResponseDryRun(this) {
						Engine = { EngineSpeed = avgDtSpeed},
						ElectricMotor = {
							ElectricMotorPowerMech = (inTorqueDt - outTorque) * avgDtSpeed,
						},
						DeltaFullLoad =  remainingPower,
						DeltaDragLoad = remainingPower,
						DeltaFullLoadTorque = inTorqueDt,
						DeltaDragLoadTorque = inTorqueDt,

					};
				} else {

					if (remainingPower.IsEqual(0, Constants.SimulationSettings.LineSearchTolerance)) {
						//if (electricSupplyResponse.MaxPowerDrive.IsGreaterOrEqual(0)) {
						if (electricSupplyResponse is ElectricSystemUnderloadResponse) {
							retVal = new ResponseBatteryEmpty(this, electricSupplyResponse);
						} else {
							retVal = new ResponseSuccess(this) {
								ElectricMotor = {
									ElectricMotorPowerMech = (inTorqueDt - outTorque) * avgDtSpeed,
								},
								Engine = {
									PowerRequest = 0.SI<Watt>(),
									EngineSpeed = outAngularVelocity
								},
							};
							var busAuxPwr = BusAux?.TorqueDemand(absTime, dt, outTorque, outAngularVelocity) ?? 0.SI<NewtonMeter>();
							if (!busAuxPwr.IsEqual(0)) {
								Log.Warn("Check BusAux config for PEV!");
							}
						}
					} else {
						if (remainingPower > 0) {
							retVal = new ResponseOverload(this) { Delta = remainingPower };
						} else {
							retVal = new ResponseUnderload(this) { Delta = remainingPower };
						}

						retVal.Engine.EngineSpeed = avgDtSpeed;
					}
				}
			} else {
				retVal = NextComponent.Request(absTime, dt, inTorqueDt / ratio, outAngularVelocity * ratio, dryRun);
				retVal.ElectricMotor.ElectricMotorPowerMech = (inTorqueDt - outTorque) * avgDtSpeed;
			}

			retVal.ElectricMotor.TotalTorqueDemand = inTorqueDt;

			retVal.ElectricMotor.MaxDriveTorque = maxDriveTorqueDt;
			retVal.ElectricMotor.MaxDriveTorqueEM = maxDriveTorqueEm;
			retVal.ElectricMotor.MaxRecuperationTorque = maxRecuperationTorqueDt;
			retVal.ElectricMotor.MaxRecuperationTorqueEM = maxRecuperationTorqueEm;
			retVal.ElectricMotor.AngularVelocity = avgEmSpeed;
			retVal.ElectricMotor.AvgDrivetrainSpeed = avgDtSpeed;
			retVal.ElectricMotor.DeRatingActive = DeRatingActive;

			retVal.ElectricMotor.TorqueRequest = outTorque;
			retVal.ElectricMotor.TorqueRequestEmMap = emTorqueMap;
			retVal.ElectricMotor.InertiaTorque =
				avgDtSpeed.IsEqual(0) ? 0.SI<NewtonMeter>() : inertiaTorqueEm * avgEmSpeed / avgDtSpeed;
			
			retVal.ElectricMotor.PowerRequest = outTorque * outAngularVelocity;
			retVal.ElectricMotor.InertiaPowerDemand = inertiaTorqueEm * avgEmSpeed;
			retVal.ElectricSystem = electricSupplyResponse;
			
			if (!dryRun) {
				CurrentState.IceSwitchedOn = iceOn;
				CurrentState.ICEOnSpeed = DataBus.EngineInfo?.EngineSpeed;
				CurrentState.EMSpeed = emSpeed;
				CurrentState.EMTorque = emOff ? null : emTorque;
				CurrentState.EmTorqueMap = emTorqueMap;
				CurrentState.DragMax = maxRecuperationTorqueEmMap;
				CurrentState.DriveMax = maxDriveTorqueEmMap;

				CurrentState.InertiaTorqueLoss = inertiaTorqueEm;
				CurrentState.DrivetrainSpeed = outAngularVelocity;
				CurrentState.DrivetrainInTorque = inTorqueDt;
				CurrentState.DrivetrainOutTorque = outTorque;

				CurrentState.TransmissionTorqueLoss = avgDtSpeed.IsEqual(0) ? 0.SI<NewtonMeter>() :
					((inTorqueDt - outTorque) * avgDtSpeed - emTorque * avgEmSpeed) / avgDtSpeed;

				CurrentState.ElectricPowerToBattery = retVal.ElectricSystem?.ConsumerPower;

			}
			return retVal;
		}

		private NewtonMeter GetMaxRecuperationTorque(Volt volt, Second dt, PerSecond avgSpeed)
		{
			var tqContinuousPwr = DeRatingActive ? ContinuousTorque : null;
			
			var maxEmTorque = VectoMath.Min(tqContinuousPwr, ModelData.EfficiencyData.FullGenerationTorque(volt, avgSpeed));
			var electricSystemResponse = ElectricPower.Request(0.SI<Second>(), dt, 0.SI<Watt>(), true);
			var maxBatPower = electricSystemResponse.MaxPowerDrag;

			if (maxBatPower.IsSmallerOrEqual(0, 1e-3)) {
				// has to be positive for recuperation - battery is full
				return null;
			}

			var maxBatRecuperationTorque = maxBatPower.IsEqual(0, 1e-3)
				? ModelData.DragCurve.Lookup(avgSpeed)
				: ModelData.EfficiencyData.EfficiencyMapLookupTorque(volt, maxBatPower, avgSpeed, maxEmTorque);
			var maxTorqueRecuperate = VectoMath.Min(maxEmTorque, maxBatRecuperationTorque);
			if (maxTorqueRecuperate < 0) {
				return null;
			}

			var elPower = ModelData.EfficiencyData.LookupElectricPower(volt, avgSpeed, maxTorqueRecuperate);
			if (elPower.ElectricalPower?.IsSmallerOrEqual(0) ?? true) {
				return null;
			}
			return maxTorqueRecuperate;
		}

		private NewtonMeter GetMaxDriveTorque(Volt volt, Second dt, PerSecond avgSpeed)
		{
			var tqContinuousPwr = DeRatingActive ? -ContinuousTorque : null;
			
			var maxEmTorque = VectoMath.Max(tqContinuousPwr ,ModelData.EfficiencyData.FullLoadDriveTorque(volt, avgSpeed));
			var electricSystemResponse = ElectricPower.Request(0.SI<Second>(), dt, 0.SI<Watt>(), true);
			var maxBatPower = electricSystemResponse.MaxPowerDrive;

			if (maxBatPower.IsGreater(0, 1e-3)) {
				// has to be negative for propelling - so battery is below min SoC
				return null;
			}

			var maxBatDriveTorque = maxBatPower.IsEqual(0, 1e-3)
				? ModelData.DragCurve.Lookup(avgSpeed)
				: ModelData.EfficiencyData.EfficiencyMapLookupTorque(volt, maxBatPower, avgSpeed, maxEmTorque);
			
			var maxTorqueDrive = VectoMath.Max(maxEmTorque, maxBatDriveTorque);
			return maxTorqueDrive > 0 ? null : maxTorqueDrive;
		}


		protected NewtonMeter ConvertEmTorqueToDrivetrain(PerSecond emSpeed, NewtonMeter emTorque)
		{
			var dtTorque = ModelData.TransmissionLossMap.GetOutTorque(emSpeed, emTorque);

			var dtSpeed = emSpeed / ModelData.RatioADC;
			var emTorqueBwd = ConvertDrivetrainTorqueToEm(dtSpeed, dtTorque);
			if (!emTorque.IsEqual(emTorqueBwd, 1e-8.SI<NewtonMeter>())) {
				Log.Debug("Forward Calculation and Backward Calculation do not match...");
				dtTorque = SearchAlgorithm.Search(emTorque, (emTorqueBwd - emTorque) * 1e3, emTorque / 10,
					getYValue: r => (r as NewtonMeter - emTorque) * 1e3,
					evaluateFunction: x => ConvertDrivetrainTorqueToEm(dtSpeed, x),
					criterion: r => {
						var i = r as NewtonMeter;
						return (i - emTorque).Value() * 1e3;
					});
			}

			return dtTorque;
			//return emTorque * ModelData.Ratio *
			//		(emTorque < 0 ? ModelData.TransmissionLossMap : 1 / ModelData.TransmissionLossMap);
		}

		protected NewtonMeter ConvertDrivetrainTorqueToEm(PerSecond dtSpeed, NewtonMeter dtTorque)
		{
			var torqueLoss = ModelData.TransmissionLossMap.GetTorqueLoss(dtSpeed, dtTorque);

			var emTorque = dtTorque / ModelData.RatioADC + torqueLoss.Value;

			return emTorque;
			//return dtTorque / ModelData.Ratio *
			//		(dtTorque < 0 ? 1 / ModelData.TransmissionLossMap : ModelData.TransmissionLossMap);
		}


		protected override void DoWriteModalResults(Second time, Second simulationInterval, IModalDataContainer container)
		{
			var prevDtSpeed = CurrentState.IceSwitchedOn ? CurrentState.ICEOnSpeed : PreviousState.DrivetrainSpeed;
			var prevEmSpeed = CurrentState.IceSwitchedOn ? prevDtSpeed * ModelData.RatioADC : PreviousState.EMSpeed;

			var avgEMSpeed = (prevEmSpeed + CurrentState.EMSpeed) / 2;
			var avgDTSpeed = (prevDtSpeed + CurrentState.DrivetrainSpeed) / 2;

			container[ModalResultField.EM_ratio_, Position] = ModelData.RatioADC.SI<Scalar>();
			container[ModalResultField.n_EM_electricMotor_, Position] = avgEMSpeed;
			container[ModalResultField.T_EM_electricMotor_, Position] = CurrentState.EMTorque;
			container[ModalResultField.T_EM_electricMotor_map_, Position] = CurrentState.EmTorqueMap;

			container[ModalResultField.T_EM_electricMotor_drive_max_, Position] = CurrentState.DriveMax;
			container[ModalResultField.T_EM_electricMotor_gen_max_, Position] = CurrentState.DragMax;

			container[ModalResultField.P_EM_electricMotor_gen_max_, Position] = (CurrentState.DragMax ?? 0.SI<NewtonMeter>()) * avgEMSpeed;
			container[ModalResultField.P_EM_electricMotor_drive_max_, Position] = (CurrentState.DriveMax ?? 0.SI<NewtonMeter>()) * avgEMSpeed;
			
			container[ModalResultField.P_EM_electricMotor_em_mech_, Position] = (CurrentState.EMTorque ?? 0.SI<NewtonMeter>() ) * avgEMSpeed;
			container[ModalResultField.P_EM_electricMotor_em_mech_map_, Position] = (CurrentState.EmTorqueMap ?? 0.SI<NewtonMeter>()) * avgEMSpeed;


			container[ModalResultField.P_EM_in_, Position] = CurrentState.DrivetrainInTorque * avgDTSpeed;
			container[ModalResultField.P_EM_out_, Position] = CurrentState.DrivetrainOutTorque * avgDTSpeed;
			container[ModalResultField.P_EM_mech_, Position] = (CurrentState.DrivetrainInTorque - CurrentState.DrivetrainOutTorque) * avgDTSpeed;
			
			container[ModalResultField.P_EM_electricMotor_el_, Position] = CurrentState.ElectricPowerToBattery;
			
			container[ModalResultField.P_EM_electricMotorLoss_, Position] = (CurrentState.EmTorqueMap ?? 0.SI<NewtonMeter>()) * avgEMSpeed - CurrentState.ElectricPowerToBattery;

			container[ModalResultField.P_EM_TransmissionLoss_, Position] = CurrentState.TransmissionTorqueLoss * avgDTSpeed;

			container[ModalResultField.P_EM_electricMotorInertiaLoss_, Position] = CurrentState.InertiaTorqueLoss * avgEMSpeed;

			container[ModalResultField.P_EM_loss_, Position] = (CurrentState.DrivetrainInTorque - CurrentState.DrivetrainOutTorque) * avgDTSpeed - CurrentState.ElectricPowerToBattery;

			container[ModalResultField.EM_Off_, Position] = CurrentState.EMTorque == null ? 1.SI<Scalar>() : 0.SI<Scalar>();

			var losses = (CurrentState.EmTorqueMap ?? 0.SI<NewtonMeter>()) * avgEMSpeed - CurrentState.ElectricPowerToBattery;
			var contribution = (losses - ContinuousPowerLoss) * simulationInterval;
			if (OverloadBuffer.Value() != 0) { // mk2021-08-03 overloadbuffer was 0 in Test Case: "ADASTestPEV.TestPCCEngineeringSampleCases G5Eng PCC12 Case A"
				container[ModalResultField.ElectricMotor_OvlBuffer_, Position] = VectoMath.Max(0, (ThermalBuffer + contribution) / OverloadBuffer);
			}
				
			if (NextComponent == null && BusAux != null) {
				BusAux.DoWriteModalResultsICE(time, simulationInterval, container);
			}
		}

		protected override void DoCommitSimulationStep(Second time, Second simulationInterval)
		{
			var avgSpeed = (PreviousState.EMSpeed + CurrentState.EMSpeed) / 2;
			var losses = (CurrentState.EMTorque ?? 0.SI<NewtonMeter>()) * avgSpeed - CurrentState.ElectricPowerToBattery;
			ThermalBuffer += (losses - ContinuousPowerLoss) * simulationInterval;
			if (ThermalBuffer < 0) {
				ThermalBuffer = 0.SI<Joule>();
			}

			if (DeRatingActive) {
				if (ThermalBuffer.IsSmallerOrEqual(OverloadBuffer * ModelData.OverloadRegenerationFactor)) {
					DeRatingActive = false;
				}
			} else {
				if (ThermalBuffer.IsGreater(OverloadBuffer)) {
					DeRatingActive = true;
				}
			}
			base.DoCommitSimulationStep(time, simulationInterval);
		}

		
		//public NewtonMeter ElectricDragTorque(PerSecond electricMotorSpeed, Second dt, DrivingBehavior drivingBehavior)
		//{
		//	return Control.MaxDragTorque(electricMotorSpeed, dt);
		//}



		public PerSecond ElectricMotorSpeed => PreviousState.EMSpeed;

		public void Connect(IElectricSystem powersupply)
		{
			ElectricPower = powersupply;
		}

	}

	public class ElectricMotorState // : SimpleComponentState
	{
		public ElectricMotorState()
		{
			EMSpeed = 0.RPMtoRad();
		}

		public PerSecond DrivetrainSpeed = 0.RPMtoRad();
		public NewtonMeter DrivetrainInTorque = 0.SI<NewtonMeter>();
		public NewtonMeter DrivetrainOutTorque = 0.SI<NewtonMeter>();
		public NewtonMeter TransmissionTorqueLoss;

		public PerSecond EMSpeed { get; set; }
		public bool IceSwitchedOn { get; set; }
		public PerSecond ICEOnSpeed { get; set; }

		public NewtonMeter EMTorque;
		public NewtonMeter EmTorqueMap;

		public NewtonMeter DriveMax;
		public NewtonMeter DragMax;
		public NewtonMeter InertiaTorqueLoss;


		public Watt ElectricPowerToBattery;
	}
}