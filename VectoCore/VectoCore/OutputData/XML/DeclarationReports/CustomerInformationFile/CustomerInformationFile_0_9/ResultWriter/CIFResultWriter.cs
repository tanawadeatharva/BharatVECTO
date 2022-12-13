using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.CustomerInformationFile.CustomerInformationFile_0_9.ResultWriter
{
	public abstract class AbstractResultsWriter : IResultsWriter
	{
		protected static readonly XNamespace Cif = "urn:tugraz:ivt:VectoAPI:CustomerOutput:v0.9";

		protected readonly ICifResultsWriterFactory _cifFactory;

		protected AbstractResultsWriter(ICifResultsWriterFactory cifFactory)
		{
			_cifFactory = cifFactory;
		}

		#region Implementation of IResultsWriter

		public virtual XElement GenerateResults(List<IResultEntry> results)
		{
			var ordered = GetOrderedResults(results);
			var allSuccess = results.All(x => x.Status == VectoRun.Status.Success);
			return new XElement(Cif + "Results",
				new XElement(Cif + XMLNames.Report_Result_Status, allSuccess ? "success" : "error"),
				ordered.Select(x =>
					x.Status == VectoRun.Status.Success
						? ResultSuccessWriter.GetElement(x)
						: ResultErrorWriter.GetElement(x)),
				allSuccess ? SummaryWriter.GetElement(ordered) : null
			);
		}

		public abstract ICifSummaryWriter SummaryWriter { get; }

		#endregion
		protected abstract IResultGroupWriter ResultSuccessWriter { get; }

		protected abstract IResultGroupWriter ResultErrorWriter { get; }

		protected virtual IList<IResultEntry> GetOrderedResults(List<IResultEntry> results)
		{
			return results.OrderBy(x => x.VehicleClass)
				.ThenBy(x => x.FuelMode)
				.ThenBy(x => x.Mission)
				.ThenBy(x => x.LoadingType).ToArray();
		}

		protected virtual List<IOVCResultEntry> GetOrderedResultsOVC(List<IResultEntry> results)
		{
			if (!results.All(x => x.OVCMode.IsOneOf(VectoRunData.OvcHevMode.ChargeSustaining, VectoRunData.OvcHevMode.ChargeDepleting))) {
				throw new VectoException(
					"Simulation runs for OVC vehicles must be either Charge Sustaining or Charge Depleting!");
			}

			var retVal = new List<IOVCResultEntry>(results.Count / 2);
			var cdEntries = results.Where(x => x.OVCMode == VectoRunData.OvcHevMode.ChargeSustaining)
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

				var combined = new OvcResultEntry() {
					ChargeSustainingResult = csEntry,
					ChargeDepletingResult = cdEntry,
					Weighted = DeclarationData.CalculateWeightedResult(cdEntry, csEntry)
				};
				retVal.Add(combined);
			}
			return retVal;
		}

	}

	public class CIFResultsWriter
	{

		public class ConventionalLorry : AbstractResultsWriter
		{
			public ConventionalLorry(ICifResultsWriterFactory cifFactory) : base(cifFactory) { }

			#region Overrides of AbstractResultsWriter

			protected override IResultGroupWriter ResultSuccessWriter => _cifFactory.GetLorryConvSuccessResultWriter();

			protected override IResultGroupWriter ResultErrorWriter => _cifFactory.GetLorryErrorResultWriter();

			public override ICifSummaryWriter SummaryWriter => _cifFactory.GetLorryConvSummaryWriter();

			#endregion
		}

		public class HEVNonOVCLorry : AbstractResultsWriter
		{
			public HEVNonOVCLorry(ICifResultsWriterFactory cifFactory) : base(cifFactory) { }

			protected override IResultGroupWriter ResultSuccessWriter => _cifFactory.GetLorryHEVNonOVCSuccessResultWriter();
			protected override IResultGroupWriter ResultErrorWriter => _cifFactory.GetLorryErrorResultWriter();

			public override ICifSummaryWriter SummaryWriter => _cifFactory.GetLorryHEVNonOVCSummaryWriter();

		}

		public class HEVOVCLorry : AbstractResultsWriter
		{
			public HEVOVCLorry(ICifResultsWriterFactory cifFactory) : base(cifFactory) { }

			#region Overrides of AbstractResultsWriter

			public override XElement GenerateResults(List<IResultEntry> results)
			{
				var ordered = GetOrderedResultsOVC(results);
				var allSuccess = results.All(x => x.Status == VectoRun.Status.Success);
				return new XElement(Cif + "Results",
					new XElement(Cif + XMLNames.Report_Result_Status, allSuccess ? "success" : "error"),
					ordered.Select(x =>
						x.ChargeDepletingResult.Status == VectoRun.Status.Success &&
						x.ChargeSustainingResult.Status == VectoRun.Status.Success
							? ResultSuccessWriter.GetElement(x)
							: ResultErrorWriter.GetElement(x)),
					SummaryWriter.GetElement(ordered)
				);
			}

			#endregion

			protected override IResultGroupWriter ResultSuccessWriter => _cifFactory.GetLorryHEVOVCSuccessResultWriter();
			protected override IResultGroupWriter ResultErrorWriter => _cifFactory.GetLorryErrorResultWriter();
			public override ICifSummaryWriter SummaryWriter => _cifFactory.GetLorryHEVOVCSummaryWriter();

		}

		public class PEVLorry : AbstractResultsWriter
		{
			public PEVLorry(ICifResultsWriterFactory cifFactory) : base(cifFactory) { }

			protected override IResultGroupWriter ResultSuccessWriter => _cifFactory.GetLorryPEVSuccessResultWriter();
			protected override IResultGroupWriter ResultErrorWriter => _cifFactory.GetLorryErrorResultWriter();

			public override ICifSummaryWriter SummaryWriter => _cifFactory.GetLorryPEVSummaryWriter();

		}

		public class ConventionalBus : AbstractResultsWriter
		{
			public ConventionalBus(ICifResultsWriterFactory cifFactory) : base(cifFactory) { }

			protected override IResultGroupWriter ResultSuccessWriter => _cifFactory.GetBusConvSuccessResultWriter();
			protected override IResultGroupWriter ResultErrorWriter => _cifFactory.GetBusErrorResultWriter();
			public override ICifSummaryWriter SummaryWriter => _cifFactory.GetBusConvSummaryWriter();

		}

		public class HEVNonOVCBus : AbstractResultsWriter
		{
			public HEVNonOVCBus(ICifResultsWriterFactory cifFactory) : base(cifFactory) { }

			protected override IResultGroupWriter ResultSuccessWriter => _cifFactory.GetBusHEVNonOVCSuccessResultWriter();
			protected override IResultGroupWriter ResultErrorWriter => _cifFactory.GetLorryErrorResultWriter();
			public override ICifSummaryWriter SummaryWriter => _cifFactory.GetBusHEVNonOVCSummaryWriter();

		}

		public class HEVOVCBus : AbstractResultsWriter
		{
			public HEVOVCBus(ICifResultsWriterFactory cifFactory) : base(cifFactory) { }

			public override XElement GenerateResults(List<IResultEntry> results)
			{
				var ordered = GetOrderedResultsOVC(results);
				var allSuccess = results.All(x => x.Status == VectoRun.Status.Success);
				return new XElement(Cif + "Results",
					new XElement(Cif + XMLNames.Report_Result_Status, allSuccess ? "success" : "error"),
					ordered.Select(x =>
						x.ChargeDepletingResult.Status == VectoRun.Status.Success &&
						x.ChargeSustainingResult.Status == VectoRun.Status.Success
							? ResultSuccessWriter.GetElement(x)
							: ResultErrorWriter.GetElement(x)),
					SummaryWriter.GetElement(ordered)
				);
			}

			protected override IResultGroupWriter ResultSuccessWriter => _cifFactory.GetBusHEVOVCSuccessResultWriter();
			protected override IResultGroupWriter ResultErrorWriter => _cifFactory.GetBusErrorResultWriter();
			public override ICifSummaryWriter SummaryWriter => _cifFactory.GetBusHEVOVCSummaryWriter();

		}

		public class PEVBus : AbstractResultsWriter
		{
			public PEVBus(ICifResultsWriterFactory cifFactory) : base(cifFactory) { }

			protected override IResultGroupWriter ResultSuccessWriter => _cifFactory.GetBusPEVSuccessResultWriter();
			protected override IResultGroupWriter ResultErrorWriter => _cifFactory.GetBusErrorResultWriter();
			public override ICifSummaryWriter SummaryWriter => _cifFactory.GetBusPEVSummaryWriter();

		}

		public class ExemptedResultsWriter : AbstractResultsWriter
		{
			public ExemptedResultsWriter(ICifResultsWriterFactory cifFactory) : base(cifFactory) { }

			#region Implementation of IResultsWriter

			public override XElement GenerateResults(List<IResultEntry> results)
			{
				return new XElement(Cif + "Results",
					new XElement(Cif + "Status", "success"),
					new XElement(Cif + "ExemptedVehicle"));
			}

			protected override IResultGroupWriter ResultSuccessWriter => null;
			protected override IResultGroupWriter ResultErrorWriter => null;
			public override ICifSummaryWriter SummaryWriter => null;


			#endregion


		}
	}
}