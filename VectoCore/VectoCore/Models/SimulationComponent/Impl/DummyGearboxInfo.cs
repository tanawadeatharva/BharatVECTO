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

		public GearboxType GearboxType => GearboxType.AMT;

		public GearshiftPosition Gear => new GearshiftPosition(1);

		public bool TCLocked => true;

		public MeterPerSecond StartSpeed => DeclarationData.GearboxTCU.StartSpeed;

		public MeterPerSquareSecond StartAcceleration => DeclarationData.GearboxTCU.StartAcceleration;

		public Watt GearboxLoss()
		{
			return 0.SI<Watt>();
		}

		public Second LastShift => -double.MaxValue.SI<Second>();

		public Second LastUpshift => -double.MaxValue.SI<Second>();

		public Second LastDownshift => -double.MaxValue.SI<Second>();

		public GearData GetGearData(uint gear)
		{
			throw new NotImplementedException();
		}

		public GearshiftPosition NextGear => throw new NotImplementedException();

		public Second TractionInterruption => 0.SI<Second>();

		public uint NumGears => 1;

		public bool DisengageGearbox => false;

		public bool GearEngaged(Second absTime)
		{
			return true;
		}

		#endregion
	}
}