using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
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
	public class ElectricMotor : StatefulProviderComponent<ElectricMotorState, ITnOutPort, ITnInPort, ITnOutPort>, IPowerTrainComponent, IElectricMotor, ITnOutPort, ITnInPort
	{

		protected IElectricSystem ElectricPower;
		protected IElectricMotorControl Control;
		protected ElectricMotorData ModelData;
		private PerSecond _maxSpeed;

		public ElectricMotor(IVehicleContainer container, ElectricMotorData data, IElectricMotorControl control, PowertrainPosition position) : base(container)
		{
			Control = control;
			ModelData = data;
			Position = position;
			container.AddComponent(this); // We have to do this again because in the base class the position is unknown!
		}

		public PowertrainPosition Position { get; }
		public PerSecond MaxSpeed
		{
			get { return _maxSpeed ?? (_maxSpeed = ModelData.FullLoadCurve.FullLoadEntries.MaxBy(x => x.MotorSpeed).MotorSpeed); }
		}

		public Watt DragPower(PerSecond electricMotorSpeed)
		{
			return ModelData.DragCurve.Lookup(electricMotorSpeed) * electricMotorSpeed;
		}

    public IResponse Initialize(NewtonMeter outTorque, PerSecond outAngularVelocity)
		{
			PreviousState.OutAngularVelocity = outAngularVelocity;
			PreviousState.OutTorque = outTorque;
			PreviousState.InAngularVelocity = outAngularVelocity;
			PreviousState.InTorque = outTorque;
			if (NextComponent == null) {
				return new ResponseSuccess(this) {
					Engine = {
						PowerRequest = outTorque * outAngularVelocity,
						EngineSpeed = outAngularVelocity
					}
				};
			}
			if (!DataBus.EngineCtl.CombustionEngineOn)
			{
				PreviousState.InTorque = 0.SI<NewtonMeter>();
				PreviousState.InAngularVelocity = outAngularVelocity;
			}
			return NextComponent.Initialize(PreviousState.InTorque, PreviousState.InAngularVelocity);
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
			var avgSpeed = (PreviousState.OutAngularVelocity + outAngularVelocity) / 2;
			var maxDriveTorque =  GetMaxDriveTorque(absTime, dt, avgSpeed);
			var maxRecuperationTorque = GetMaxRecuperationTorque(absTime, dt, avgSpeed);
			
			var retVal = HandleRequest(absTime, dt, outTorque, outAngularVelocity, dryRun, maxDriveTorque, maxRecuperationTorque);
			
			retVal.ElectricMotor.MaxDriveTorque = maxDriveTorque;
			retVal.ElectricMotor.MaxRecuperationTorque = maxRecuperationTorque;
			retVal.ElectricMotor.AngularVelocity = avgSpeed;
			retVal.ElectricMotor.PowerRequest = outTorque * avgSpeed;
			if (!dryRun) {
				CurrentState.DragMax = maxRecuperationTorque;
				CurrentState.DriveMax = maxDriveTorque;
				CurrentState.ElectricPowerToBattery = retVal.ElectricSystem.ConsumerPower;
			}
			return retVal;
		}

		private NewtonMeter GetMaxRecuperationTorque(Second absTime, Second dt, PerSecond avgSpeed)
		{
			var maxEmTorque = ModelData.FullLoadCurve.FullGenerationTorque(avgSpeed);
			var electricSystemResponse = ElectricPower.Request(absTime, dt, 0.SI<Watt>(), true);
			var maxBatPower = electricSystemResponse.MaxPowerDrag;

			var maxBatRecuperationTorque = maxBatPower.IsEqual(0) ? 0.SI<NewtonMeter>() : ModelData.EfficiencyMap.LookupTorque(maxBatPower, avgSpeed, maxEmTorque);
			var maxTorqueRecuperate = VectoMath.Min(maxEmTorque, maxBatRecuperationTorque);
			return maxTorqueRecuperate < 0 ? null : maxTorqueRecuperate;
		}

		private NewtonMeter GetMaxDriveTorque(Second absTime, Second dt, PerSecond avgSpeed)
		{
			var maxEmTorque = ModelData.FullLoadCurve.FullLoadDriveTorque(avgSpeed);
			var electricSystemResponse = ElectricPower.Request(absTime, dt, 0.SI<Watt>(), true);
			var maxBatPower = electricSystemResponse.MaxPowerDrive;

			var maxBatDriveTorque = maxBatPower.IsEqual(0) ? ModelData.DragCurve.Lookup(avgSpeed) : ModelData.EfficiencyMap.LookupTorque(maxBatPower, avgSpeed, maxEmTorque);
			//if (maxBatDriveTorque == null) {
			//	return ModelData.DragCurve.Lookup(avgSpeed);
			//}
			var maxTorqueDrive = VectoMath.Max(maxEmTorque, maxBatDriveTorque);
			return maxTorqueDrive > 0 ? null : maxTorqueDrive;
		}


		protected virtual IResponse HandleRequest(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, bool dryRun, NewtonMeter maxDriveTorque, NewtonMeter maxRecuperationTorque)
		{

			var avgSpeed = (PreviousState.OutAngularVelocity + outAngularVelocity) / 2;
			var inertiaTorqueLoss = avgSpeed.IsEqual(0)
				? 0.SI<NewtonMeter>()
				: Formulas.InertiaPower(outAngularVelocity, PreviousState.OutAngularVelocity, ModelData.Inertia, dt) / avgSpeed;
			var inTorque = outTorque + inertiaTorqueLoss;
			//var maxDriveTorque = ModelData.FullLoadCurve.FullLoadDriveTorque(avgSpeed);
			//var maxDragTorque = ModelData.FullLoadCurve.FullGenerationTorque(avgSpeed);
			if (!dryRun) {
				CurrentState.InertiaTorqueLoss = inertiaTorqueLoss;
				CurrentState.OutTorque = outTorque;
			}

			if (ElectricPower == null) {
				var retVal = ForwardRequest(absTime, dt, inTorque, inTorque, outAngularVelocity, null, dryRun);
				return retVal;
			}
			var eMotorTorque = Control.MechanicalAssistPower(absTime, dt, inTorque, PreviousState.OutAngularVelocity, outAngularVelocity,  maxDriveTorque, maxRecuperationTorque, Position, dryRun);

			if (Position == PowertrainPosition.HybridP2 && !DataBus.GearboxInfo.GearEngaged(absTime)/* && !DataBus.ClutchInfo.ClutchClosed(absTime)*/) {
				// electric motor is between gearbox and clutch, but no gear is engaged...
				if (eMotorTorque != null) {
					throw new VectoSimulationException("electric motor cannot provide torque when gearbox and clutch are disengaged");
				}
				var electricSystemResponse = ElectricPower.Request(absTime, dt, 0.SI<Watt>(), dryRun);
				if (!dryRun) {
					if (!(electricSystemResponse is ElectricSystemResponseSuccess)) {
						throw new VectoException("unexpected response from electric system: {0}", electricSystemResponse);
					}
					SetState(inTorque, outAngularVelocity);
				}
				var retVal = NextComponent.Request(absTime, dt, outTorque, outAngularVelocity, dryRun);
				retVal.ElectricMotor.ElectricMotorPowerMech = 0.SI<Watt>();
				retVal.ElectricSystem = electricSystemResponse;
				retVal.ElectricMotor.InertiaPowerDemand = 0.SI<Watt>(); // inertiaTorqueLoss * avgSpeed;
				return retVal;
			}

			if (eMotorTorque == null) {
				var retVal = ElectricMotorOff(absTime, dt, outTorque, outAngularVelocity, dryRun);
				retVal.ElectricMotor.InertiaPowerDemand = inertiaTorqueLoss * avgSpeed;
				return retVal;
			}
			//if (eMotorTorque.IsEqual(0, 1e-3))
			//{
			//	var electricSystemResponse = ElectricPower.Request(absTime, dt, 0.SI<Watt>(), dryRun);

			//	var retVal = ForwardRequest(absTime, dt, inTorque, inTorque, outAngularVelocity, dryRun);
			//	retVal.ElectricSystem = electricSystemResponse;
			//	retVal.ElectricMotor.ElectricMotorPowerMech = 0.SI<Watt>();
			//	return retVal;
			//}

			if (!dryRun && !eMotorTorque.IsBetween(maxDriveTorque ?? 0.SI<NewtonMeter>(), maxRecuperationTorque ?? 0.SI<NewtonMeter>())) {
				throw new VectoException("Invalid operating point provided by strategy! SupportPower: {0}, max Power: {1}, min Power: {2}", eMotorTorque, maxDriveTorque, maxRecuperationTorque);
			}

			var electricPower = ModelData.EfficiencyMap
				.LookupElectricPower(avgSpeed, eMotorTorque, DataBus.ExecutionMode != ExecutionMode.Declaration).ElectricalPower;

			var electricSupplyResponse = ElectricPower.Request(absTime, dt, electricPower, dryRun);
			//if (!dryRun && !(electricSupplyResponse is ElectricSystemResponseSuccess) &&
			//	electricPower > electricSupplyResponse.MaxPowerDrag) {
			//	// can't charge all power into the battery - probably it's full
			//	// dissipate remaining power
			//	electricSupplyResponse = ElectricPower.Request(absTime, dt, electricSupplyResponse.MaxPowerDrag);
			//	CurrentState.ElectricBrakePower = electricPower - electricSupplyResponse.MaxPowerDrag;
			//}
			if (!dryRun && !(electricSupplyResponse is ElectricSystemResponseSuccess)) {
				throw new VectoException(
						"Invalid operating point provided by strategy! SupportPower: {0}, req. electric Power: {1}, battery demand motor: {3}, max Power from Battery: {2}",
						eMotorTorque, electricPower,
						eMotorTorque < 0 ? electricSupplyResponse.MaxPowerDrive : electricSupplyResponse.MaxPowerDrag, electricSupplyResponse.ConsumerPower);
			}

			var response = ForwardRequest(absTime, dt, inTorque, inTorque + eMotorTorque, outAngularVelocity, electricSupplyResponse, dryRun);

			response.ElectricSystem = electricSupplyResponse;
			response.ElectricMotor.InertiaPowerDemand = inertiaTorqueLoss * avgSpeed;

			return response;
		}

		private IResponse ElectricMotorOff(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, bool dryRun)
		{
			var avgSpeed = (PreviousState.InAngularVelocity + outAngularVelocity) / 2.0;
			var torqueLoss = ModelData.DragCurve.Lookup(avgSpeed);
			var inTorque = outTorque + torqueLoss;

			if (!dryRun) {
				SetState(inTorque, outAngularVelocity);
			}
			var electricSystemResponse = ElectricPower.Request(absTime, dt, 0.SI<Watt>(), dryRun);

			var retVal = NextComponent == null
				? RequestElectricMotorOnly(absTime, dt, outTorque, inTorque, outAngularVelocity, dryRun, avgSpeed, electricSystemResponse)
				: NextComponent.Request(absTime, dt, inTorque, outAngularVelocity, dryRun);
			retVal.ElectricMotor.ElectricMotorPowerMech = (inTorque - outTorque) * avgSpeed;
			retVal.ElectricSystem = electricSystemResponse;
			return retVal;
		}

		public IResponse ForwardRequest(Second absTime, Second dt, NewtonMeter outTorque, NewtonMeter inTorque, PerSecond outAngularVelocity,
		 IElectricSystemResponse electricSystemResponse, bool dryRun = false)
		{
			var avgSpeed = (PreviousState.OutAngularVelocity + outAngularVelocity) / 2;
			if (NextComponent == null)
			{
				return RequestElectricMotorOnly(absTime, dt, outTorque, inTorque, outAngularVelocity, dryRun, avgSpeed, electricSystemResponse);
			}

			if (!dryRun) {
				SetState(inTorque, outAngularVelocity);
			}
			var retVal = NextComponent.Request(absTime, dt, inTorque, outAngularVelocity, dryRun);
			retVal.ElectricMotor.ElectricMotorPowerMech = (inTorque - outTorque) * avgSpeed;
			return retVal;
		}

		private IResponse RequestElectricMotorOnly(Second absTime, Second dt, NewtonMeter outTorque, NewtonMeter inTorque, PerSecond outAngularVelocity, bool dryRun, PerSecond avgSpeed, IElectricSystemResponse electricSystemResponse)
		{
			var remainingPower = inTorque * avgSpeed;
			if (dryRun)
			{
				//var driveTorque = Control.MaxDriveTorque(avgSpeed, dt);
				var dragTorque = 0.SI<NewtonMeter>(); //Control.MaxDragTorque(avgSpeed, dt);
				var powerDemand = outTorque * avgSpeed;
				return new ResponseDryRun(this) {
					Engine = { 
					EngineSpeed = avgSpeed,
					},
					DeltaFullLoad = remainingPower, //powerDemand + driveTorque * avgSpeed,
					DeltaDragLoad = remainingPower, // powerDemand + dragTorque * avgSpeed,
				};
			}

			if ((inTorque * avgSpeed).IsEqual(0, Constants.SimulationSettings.LineSearchTolerance)) {
				SetState(inTorque, outAngularVelocity);
				if (electricSystemResponse.MaxPowerDrive.IsGreaterOrEqual(0)) {
					return new ResponseBatteryEmpty(this);
				}
				return new ResponseSuccess(this) {
					ElectricMotor = {
						ElectricMotorPowerMech = (inTorque - outTorque) * avgSpeed,
					},
					Engine = {
						PowerRequest = 0.SI<Watt>(),
						EngineSpeed = outAngularVelocity
					},
				};
			}

			AbstractResponse response;

			if (remainingPower > 0)
			{
				response = new ResponseOverload(this) { Delta = remainingPower };
			}
			else
			{
				response = new ResponseUnderload(this) { Delta = remainingPower };
			}
			response.Engine.EngineSpeed = avgSpeed;
			return response;
		}

		private void SetState(NewtonMeter inTorque, PerSecond outAngularVelocity)
		{
			CurrentState.OutAngularVelocity = outAngularVelocity;
			CurrentState.InAngularVelocity = outAngularVelocity;
			//CurrentState.OutTorque = outTorque;
			CurrentState.InTorque = inTorque;
		}




		protected override void DoWriteModalResults(Second time, Second simulationInterval, IModalDataContainer container)
		{
			var avgSpeed = (PreviousState.OutAngularVelocity + CurrentState.OutAngularVelocity) / 2;
			container[ModalResultField.n_electricMotor_, Position] = avgSpeed;
			container[ModalResultField.T_electricMotor_, Position] = CurrentState.InTorque - CurrentState.OutTorque;
			container[ModalResultField.T_electricMotor_full_, Position] = CurrentState.DriveMax;
			container[ModalResultField.T_electricMotor_drag_, Position] = CurrentState.DragMax;
			container[ModalResultField.P_electricMotor_mech_, Position] = (CurrentState.InTorque - CurrentState.OutTorque) * avgSpeed;
			container[ModalResultField.P_electricMotor_out_, Position] = CurrentState.OutTorque * avgSpeed;
			container[ModalResultField.P_electricMotor_in_, Position] = CurrentState.InTorque * avgSpeed;
			container[ModalResultField.P_electricMotor_el_, Position] = CurrentState.ElectricPowerToBattery;
			//container[ModalResultField.P_electricMotor_brake_, Position] = CurrentState.ElectricBrakePower;
			container[ModalResultField.P_electricMotor_drag_max_, Position] = (CurrentState.DragMax ?? 0.SI<NewtonMeter>()) * avgSpeed;
			container[ModalResultField.P_electricMotor_drive_max_, Position] = (CurrentState.DriveMax ?? 0.SI<NewtonMeter>()) * avgSpeed;
			container[ModalResultField.P_electricMotorLoss_, Position] = (CurrentState.InTorque - CurrentState.OutTorque) * avgSpeed - (CurrentState.ElectricPowerToBattery);
			container[ModalResultField.P_electricMotorInertiaLoss_, Position] = CurrentState.InertiaTorqueLoss * avgSpeed;
		}

		//public NewtonMeter ElectricDragTorque(PerSecond electricMotorSpeed, Second dt, DrivingBehavior drivingBehavior)
		//{
		//	return Control.MaxDragTorque(electricMotorSpeed, dt);
		//}



		public PerSecond ElectricMotorSpeed
		{
			get { return PreviousState.InAngularVelocity; }
		}

		public void Connect(IElectricSystem powersupply)
		{
			ElectricPower = powersupply;
		}
	}

	public class ElectricMotorState : SimpleComponentState
	{
		public NewtonMeter DriveMax;
		public NewtonMeter DragMax;
		public Watt ElectricPowerToBattery;
		//public Watt ElectricBrakePower = 0.SI<Watt>();
		public NewtonMeter InertiaTorqueLoss;
	}
}