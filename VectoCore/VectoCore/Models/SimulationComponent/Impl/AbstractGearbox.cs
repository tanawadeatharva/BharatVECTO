using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Connector.Ports;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
	public abstract class AbstractGearbox<TStateType> :
		StatefulProviderComponent<TStateType, ITnOutPort, ITnInPort, ITnOutPort>, ITnOutPort, ITnInPort, IGearbox, IClutchInfo
		where TStateType : GearboxState, new()
	{
		/// <summary>
		/// The data and settings for the gearbox.
		/// </summary>
		[Required, ValidateObject] internal readonly GearboxData ModelData;

		protected IAuxPort Auxiliary;

		protected AbstractGearbox(IVehicleContainer container, GearboxData gearboxModelData) : base(container)
		{
			ModelData = gearboxModelData;
		}

		#region ITnOutPort

		public abstract IResponse Request(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity,
			bool dryRun = false);

		public abstract IResponse Initialize(NewtonMeter outTorque, PerSecond outAngularVelocity);

		#endregion

		#region IGearboxCockpit

		public GearboxType GearboxType
		{
			get { return ModelData.Type; }
		}

		/// <summary>
		/// The current gear.
		/// </summary>
		public uint Gear { get; protected internal set; }

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

		public NewtonMeter GearMaxTorque
		{
			get { return Gear == 0 || !ModelData.Gears.ContainsKey(Gear) ? null : ModelData.Gears[Gear].MaxTorque; }
		}

		public Watt GearboxLoss()
		{
			//var outTorque = ModelData.Gears[Gear].LossMap.GetOutTorque(inAngularVelocity, inTorque, true);
			//var torqueLoss = inTorque - outTorque * ModelData.Gears[Gear].Ratio;

			//return torqueLoss * inAngularVelocity;

			return (PreviousState.TransmissionTorqueLoss +
					PreviousState.InertiaTorqueLossOut / ModelData.Gears[PreviousState.Gear].Ratio) * PreviousState.InAngularVelocity;
		}

		#endregion

		#region IAuxPortProvider

		public void Connect(IAuxPort aux)
		{
			Auxiliary = aux;
		}

		#endregion

		public abstract bool ClutchClosed(Second absTime);
	}

	public class GearboxState : SimpleComponentState
	{
		public NewtonMeter InertiaTorqueLossOut = 0.SI<NewtonMeter>();
		public NewtonMeter TransmissionTorqueLoss = 0.SI<NewtonMeter>();
		public uint Gear;
		public TransmissionLossMap.LossMapResult TorqueLossResult;
	}
}