using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.Common
{
    public abstract class AbstractResultsWriter : IResultsWriter
    {

        #region Implementation of IResultsWriter

        public virtual XElement GenerateResults(List<IResultEntry> results)
        {
            var ordered = GetOrderedResults(results);
            var allSuccess = results.All(x => x.Status.IsOneOf(VectoRun.Status.Success, VectoRun.Status.PrimaryBusSimulationIgnore));
			return new XElement(TNS + XMLNames.Report_Results,
				new XElement(TNS + XMLNames.Report_Result_Status,
					allSuccess ? XMLNames.Report_Results_Status_Success_Val : XMLNames.Report_Results_Status_Error_Val),
				ordered.Select(x => GetResultWriter(x.Status).GetElement(x)),
				allSuccess ? SummaryWriter.GetElement(ordered) : null
			);
		}

        #endregion

		protected virtual IResultGroupWriter GetResultWriter(VectoRun.Status status)
		{
			switch (status) {
				case VectoRun.Status.Pending:
				case VectoRun.Status.Running:
				case VectoRun.Status.REESSEmpty:
					throw new VectoException($"No result Writer available for run status ${status}");
				case VectoRun.Status.Success:
					return ResultSuccessWriter;
				case VectoRun.Status.Canceled:
				case VectoRun.Status.Aborted:
					return ResultErrorWriter;
				case VectoRun.Status.PrimaryBusSimulationIgnore:
					return ResultSuccessWriter;
				default:
					throw new ArgumentOutOfRangeException(nameof(status), status, null);
			}
		}

        protected abstract XNamespace TNS { get; }

        protected abstract IResultGroupWriter ResultSuccessWriter { get; }

        protected abstract IResultGroupWriter ResultErrorWriter { get; }

		//protected abstract IResultGroupWriter ResultIgnoreWriter { get; }

        public abstract IReportResultsSummaryWriter SummaryWriter { get; }


        protected virtual IList<IResultEntry> GetOrderedResults(List<IResultEntry> results)
        {
            return results.OrderBy(x => x.VehicleClass)
                .ThenBy(x => x.FuelMode)
                .ThenBy(x => x.Mission)
                .ThenBy(x => x.LoadingType).ToArray();
        }

        protected virtual List<IOVCResultEntry> GetOrderedResultsOVC(List<IResultEntry> results)
        {
            if (!results.All(x => x.OVCMode.IsOneOf(OvcHevMode.ChargeSustaining, OvcHevMode.ChargeDepleting))) {
                throw new VectoException(
                    "Simulation runs for OVC vehicles must be either Charge Sustaining or Charge Depleting!");
            }

            var retVal = new List<IOVCResultEntry>(results.Count / 2);
            var cdEntries = results.Where(x => x.OVCMode == OvcHevMode.ChargeDepleting)
                .OrderBy(x => x.VehicleClass)
                .ThenBy(x => x.FuelMode)
                .ThenBy(x => x.Mission)
                .ThenBy(x => x.LoadingType)
                .ToList();
            foreach (var cdEntry in cdEntries) {
                var csEntry = results.FirstOrDefault(x => x.OVCMode != cdEntry.OVCMode &&
                                                        x.VehicleClass == cdEntry.VehicleClass &&
                                                        x.FuelMode == cdEntry.FuelMode &&
                                                        x.Mission == cdEntry.Mission &&
                                                        x.LoadingType == cdEntry.LoadingType);
                if (csEntry == null) {
                    throw new VectoException(
                        $"no matching result for {cdEntry.Mission}, {cdEntry.LoadingType}, {cdEntry.FuelMode} found!");
                }

				
				var weightedResult = cdEntry.VehicleClass.IsCompletedBus() ? 
					DeclarationData.CalculateWeightedResultCompletedBus(cdEntry, csEntry) :
					DeclarationData.CalculateWeightedResult(cdEntry, csEntry);

				var combined = new OvcResultEntry()
				{
					ChargeSustainingResult = csEntry,
					ChargeDepletingResult = cdEntry,
					Weighted = weightedResult
				};
				retVal.Add(combined);
            }
            return retVal;
        }


		
		
	}
}