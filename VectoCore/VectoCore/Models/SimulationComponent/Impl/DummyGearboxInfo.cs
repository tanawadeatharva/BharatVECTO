using System;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
	public class DummyGearboxInfo : VectoSimulationComponent, IGearboxInfo
	{
		public DummyGearboxInfo(VehicleContainer container) : base(container)
		{

		}

		#region Overrides of VectoSimulationComponent

		protected override void DoWriteModalResults(Second time, Second simulationInterval, IModalDataContainer container) { }

		protected override void DoCommitSimulationStep(Second time, Second simulationInterval) { }

		#endregion

		#region Implementation of IGearboxInfo

		public GearboxType GearboxType
		{
			get { return GearboxType.AMT; }
		}

		public GearshiftPosition Gear
		{
			get { return new GearshiftPosition(1); }
		}

		public bool TCLocked
		{
			get { return true; }
		}

		public MeterPerSecond StartSpeed
		{
			get { return DeclarationData.GearboxTCU.StartSpeed; }
		}

		public MeterPerSquareSecond StartAcceleration
		{
			get { return DeclarationData.GearboxTCU.StartAcceleration; }
		}

		public Watt GearboxLoss()
		{
			return 0.SI<Watt>();
		}

		public Second LastShift
		{
			get { return -double.MaxValue.SI<Second>(); }
		}

		public Second LastUpshift
		{
			get { return -double.MaxValue.SI<Second>(); }
		}

		public Second LastDownshift
		{
			get { return -double.MaxValue.SI<Second>(); }
		}

		public GearData GetGearData(uint gear)
		{
			throw new NotImplementedException();
		}

		public GearshiftPosition NextGear
		{
			get { throw new NotImplementedException(); }
		}

		public Second TractionInterruption
		{
			get { return 0.SI<Second>(); }
		}

		public uint NumGears
		{
			get { return 1; }
		}

		public bool DisengageGearbox
		{
			get { return false; }
		}

		public bool GearEngaged(Second absTime)
		{
			return true;
		}

		#endregion
	}
}