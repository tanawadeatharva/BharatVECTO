using System;
using System.Collections.Generic;
using System.Dynamic;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.OutputData;
// ReSharper disable ConvertToNullCoalescingCompoundAssignment

namespace TUGraz.VectoCore.Models.Simulation.Impl
{
//	public interface IFollowUpRunCreator
//	{
//		public 
//	}

public interface IFollowUpRunCreator
{
	bool RunAgain(Action<VectoRunData> runAgainAction, IVectoRun run, Action beforeNextRun);
}


public class NoFollowUpRunCreator : LoggingObject, IFollowUpRunCreator
{
	#region Implementation of IFollowUpRunCreator

	public bool RunAgain(Action runAgainAction, IVectoRun run)
	{
		return false;
	}

	#endregion

	#region Implementation of IFollowUpRunCreator

	public bool RunAgain(Action runAgainAction, IVectoRun run, Action beforeNextRun)
	{
		return false;
	}

	#endregion

	#region Implementation of IFollowUpRunCreator

	public bool RunAgain(Action<VectoRunData> runAgainAction, IVectoRun run, Action beforeNextRun)
	{
		return false;
	}

	#endregion
}
/// <summary>
	/// This class decides if a run is executed another time, stores the relevant data from the current run and prepares the VehicleContainer for the next run
	/// </summary>
	public class FollowUpOvcRunCreator : LoggingObject, IFollowUpRunCreator
{
		public delegate void ManipulateRunData(int it, int total_it, VectoRunData runData, IModalDataContainer prevModData);
		private int additionalIterations = 2;
		private string original_modfile_suffix = null;

		private int iteration = 0;


		private List<IterationResultEntry> iterationResults = new List<IterationResultEntry>(3) {
		};

		public FollowUpOvcRunCreator()
		{
		}

		/// <summary>
		/// Determines if a run should be simulated again, if the run should be simulated again,
		/// necessary simulation components are reset, the rundata is adjusted and the runAgainAction is executed
		/// </summary>
		/// <param name="runAgainAction"></param>
		/// <param name="run"></param>
		/// <return>true if the run is executed again, false otherwise</return>
		public bool RunAgain(Action<VectoRunData> runAgainAction, IVectoRun run, Action beforeNextRun)
		{
			if (run.GetContainer().RunData.OVCMode == VectoRunData.OvcHevMode.ChargeSustaining) {
				return false;
			}
#pragma warning disable IDE0054 // Verbundzuweisung verwenden
            original_modfile_suffix = original_modfile_suffix ?? (original_modfile_suffix = run.GetContainer().RunData.ModFileSuffix);
#pragma warning restore IDE0054 // Verbundzuweisung verwenden
            var container = run.GetContainer();
			iteration++;
			if (!ShouldRunAgain(run)) {
				return false;
			}
			
			beforeNextRun();
			var modData = container.ModalData;
			
			ResetSimulationComponents(container);
			UpdateRunData(container.RunData, run);
			runAgainAction(container.RunData);
			ResetRunData(container.RunData);
			
			return true;
		}

		private void ResetRunData(VectoRunData containerRunData)
		{
			
		}

		private void UpdateRunData(VectoRunData runData, IVectoRun prevRun)
		{
			var container = prevRun.GetContainer();
			// updated container.RunData.HybridStrategyParameters;

			runData.ModFileSuffix = original_modfile_suffix + iteration;
			var deltaSoc = container.ModalData.REESSDeltaSoc();
			SetEquivalenceFactor(runData, deltaSoc);
		}

		private void SetEquivalenceFactor(VectoRunData runData, double deltaSOC)
		{
			var factorCharge = runData.HybridStrategyParameters.EquivalenceFactorCharge /
								runData.HybridStrategyParameters.EquivalenceFactor;
			var factorDischarge = runData.HybridStrategyParameters.EquivalenceFactorDischarge /
								runData.HybridStrategyParameters.EquivalenceFactor;

			switch (iteration) {
				case 1:

					var k_0_4 = DeclarationData.HEVStrategyParameters.LookupSlope(runData.Mission.MissionType,
						runData.VehicleData.VehicleClass, runData.Loading);
					var soc_usable = runData.HybridStrategyParameters.MaxSoC - runData.HybridStrategyParameters.MinSoC;

					var k = k_0_4 + 0.2 * (soc_usable - 0.4); //soc usable < 0.4?

					var f_equiv = runData.HybridStrategyParameters.EquivalenceFactor;
					var f_equiv_2 = f_equiv - (deltaSOC / k);
					

					iterationResults.Add(new IterationResultEntry() {
						f_equiv = f_equiv,
						d_soc = deltaSOC,
					});

					runData.HybridStrategyParameters.EquivalenceFactor = f_equiv_2;
					runData.HybridStrategyParameters.EquivalenceFactorCharge = f_equiv_2 * factorCharge;
					runData.HybridStrategyParameters.EquivalenceFactorDischarge = f_equiv_2 * factorDischarge;
					iterationResults.Add(new IterationResultEntry() {
						f_equiv = f_equiv_2
					});

					break;
				case 2:
					iterationResults[1].d_soc = deltaSOC;

					var d_soc_1 = iterationResults[0].d_soc;
					var d_soc_2 = iterationResults[1].d_soc;
					var f_equiv_1 = iterationResults[0].f_equiv;
					f_equiv_2 = iterationResults[1].f_equiv;

					var f_equiv_3 = (((0 - d_soc_1) / (d_soc_2 - d_soc_1)) * (f_equiv_2 - f_equiv_1)) + f_equiv_1;
					runData.HybridStrategyParameters.EquivalenceFactor = f_equiv_3;
					runData.HybridStrategyParameters.EquivalenceFactorCharge = f_equiv_3 * factorCharge;
					runData.HybridStrategyParameters.EquivalenceFactorDischarge = f_equiv_3 * factorDischarge;
					
					break;
				default:
					throw new VectoException("Iteration 3 not implemented");
			}
		}

		private void ResetSimulationComponents(IVehicleContainer vehicleContainer)
		{
			//vehicleContainer.RunStatus = VectoRun.Status.Pending;
			//vehicleContainer.ResetComponents();
		}

		private bool ShouldRunAgain(IVectoRun run)
		{
			Log.Info(string.Format("Run {0} again!", run.RunName));
			return (iteration <= additionalIterations);
		}

		private class IterationResultEntry
		{
			public double f_equiv;
			public double d_soc;
		}
	}
}