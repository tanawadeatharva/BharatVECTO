using System;
using System.Collections.Generic;
using System.Linq;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;

namespace TUGraz.VectoCore.InputData.Reader.Impl
{
	internal class EngineeringVTPModeVectoRunDataFactoryHeavyBusPrimary : DeclarationVTPModeVectoRunDataFactoryHeavyBusPrimary
	{
		public EngineeringVTPModeVectoRunDataFactoryHeavyBusPrimary(IVTPEngineeringInputDataProvider ivtpProvider, IPrimaryBusDeclarationDataAdapter declarationDataAdapter) : base(
			ivtpProvider, null, declarationDataAdapter)
		{
		}

		protected override IEnumerable<VectoRunData> GetNextRun()
		{
			return JobInputData.Cycles.Select(
				cycle =>
				{
					var drivingCycle = DrivingCycleDataReader.ReadFromDataTable(cycle.CycleData, cycle.Name, false);
					// loading is not relevant as we use P_wheel
					var runData = CreateVectoRunData(Segment, Segment.Missions.First(), new Tuple<Kilogram, double?>(0.SI<Kilogram>(), null));
					runData.Cycle = new DrivingCycleProxy(drivingCycle, cycle.Name);
					runData.Aux = AuxVTP;
					runData.FanDataVTP = GetFanData();
					runData.ExecutionMode = ExecutionMode.Engineering;
					runData.SimulationType = SimulationType.VerificationTest;
					runData.Mission = new Mission()
					{
						MissionType = MissionType.VerificationTest
					};
					runData.DriverData = Driverdata;

					var mileageCorrection = GetMileagecorrectionFactor(JobInputData.Mileage);
					var correctionFactors = JobInputData.FuelNCVs.ToDictionary(
						keySelector: f => f.Type,
						elementSelector: f => (f.NCV / DeclarationData.FuelData.Lookup(
							f.Type,
							JobInputData.Vehicle.TankSystem).LowerHeatingValueVecto).Value() * mileageCorrection);

					runData.VTPData = new VTPData()
					{
						CorrectionFactors = correctionFactors,
						FuelNCVs = JobInputData.FuelNCVs
					};
					runData.TorqueDriftLeftWheel = JobInputData.TorqueDriftLeftWheel;
					runData.TorqueDriftRightWheel = JobInputData.TorqueDriftRightWheel;
					return runData;
				});
		}

		protected override AuxFanData GetFanData()
		{
			return new AuxFanData()
			{
				FanCoefficients = JobInputData.FanPowerCoefficents.ToArray(),
				FanDiameter = JobInputData.FanDiameter,
			};
		}
	}
}