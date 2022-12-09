using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.CustomerInformationFile.CustomerInformationFile_0_9.ResultWriter
{
	public interface IResultsWriter
	{
		XElement GenerateResults(List<IResultEntry> results);
	}

	public interface IResultGroupWriter
	{
		XElement GetElement(IResultEntry entry);

		XElement GetElement(Tuple<IResultEntry, IResultEntry> entry);
	}

	public interface IFuelConsumptionWriter
	{
		XElement GetElement(IResultEntry entry, IFuelConsumptionCorrection fuelConsumptionCorrection);
	}
	public interface ICifResultsWriterFactory
	{
		IResultGroupWriter GetLorryOVCErrorResultWriter();

		IResultGroupWriter GetLorryOVCSuccessResultWriter();
		IResultGroupWriter GetMissionWriter();
		IResultGroupWriter GetSimulationParameterWriter();
		IResultGroupWriter GetLorryOVCResultWriterChargeDepleting();
		IResultGroupWriter GetLorryOVCResultWriterChargeSustaining();
		IFuelConsumptionWriter GetFuelConsumptionLorry();
		IResultGroupWriter GetElectricEnergyConsumptionLorry();
	}

	public abstract class AbstractResultsWriter : IResultsWriter
	{
		protected static readonly XNamespace Cif = "urn:tugraz:ivt:VectoAPI:CustomerOutput:v0.9";

		protected readonly ICifResultsWriterFactory _cifFactory;

		protected AbstractResultsWriter(ICifResultsWriterFactory cifFactory)
		{
			_cifFactory = cifFactory;
		}

		#region Implementation of IResultsWriter

		//public abstract XElement GenerateResults(List<IResultEntry> results);

		public virtual XElement GenerateResults(List<IResultEntry> results)
		{
			return null;
		}

		#endregion

		protected List<Tuple<IResultEntry, IResultEntry>> GetOrderedResultsOVC(List<IResultEntry> results)
		{
			if (!results.All(x => x.OVCMode.IsOneOf(VectoRunData.OvcHevMode.ChargeSustaining, VectoRunData.OvcHevMode.ChargeDepleting))) {
				throw new VectoException(
					"Simulation runs for OVC vehicles must be either Charge Sustaining or Charge Depleting!");
			}

			var retVal = new List<Tuple<IResultEntry, IResultEntry>>(results.Count / 2);
			var cdEntries = results.Where(x => x.OVCMode == VectoRunData.OvcHevMode.ChargeSustaining)
				.OrderBy(x => x.VehicleClass)
				.ThenBy(x => x.FuelMode)
				.ThenBy(x => x.Mission)
				.ThenBy(x => x.LoadingType)
				.ToList();
			foreach (var entry in cdEntries) {
				var match = results.FirstOrDefault(x => x.OVCMode != entry.OVCMode &&
														x.VehicleClass == entry.VehicleClass &&
														x.FuelMode == entry.FuelMode &&
														x.Mission == entry.Mission &&
														x.LoadingType == entry.LoadingType);
				if (match == null) {
					throw new VectoException(
						$"no matching result for {entry.Mission}, {entry.LoadingType}, {entry.FuelMode} found!");
				}
				retVal.Add(Tuple.Create(entry, match));
			}
			return retVal;
		}

		protected XElement GetSummary(List<Tuple<IResultEntry, IResultEntry>> orderedResults)
		{
			var allSuccess = orderedResults.All(x =>
				x.Item1.Status == VectoRun.Status.Success && x.Item2.Status == VectoRun.Status.Success);
			if (!allSuccess) {
				// do not write summary unless all simulation runs are successful!
				return null;
			}
			//throw new NotImplementedException();
			return null;
		}

		protected XElement GetSummary(List<IResultEntry> results)
		{
			var allSuccess = results.All(x => x.Status == VectoRun.Status.Success);
			if (!allSuccess) {
				// do not write summary unless all simulation runs are successful!
				return null;
			}
			throw new NotImplementedException();
		}
	}

	public class CIFResultsWriter
	{
		public class ConventionalLorry : AbstractResultsWriter
		{
			public ConventionalLorry(ICifResultsWriterFactory cifFactory) : base(cifFactory) { }
		}

		public class HEVNonOVCLorry : AbstractResultsWriter
		{
			public HEVNonOVCLorry(ICifResultsWriterFactory cifFactory) : base(cifFactory) { }
		}

		public class HEVOVCLorry : AbstractResultsWriter
		{
			#region Overrides of AbstractResultsWriter

			public override XElement GenerateResults(List<IResultEntry> results)
			{
				var ordered = GetOrderedResultsOVC(results);
				var allSuccess = results.All(x => x.Status == VectoRun.Status.Success);
				return new XElement(Cif + "Results",
					new XElement(Cif + XMLNames.Report_Result_Status, allSuccess ? "success" : "error"),
					ordered.Select(x =>
						x.Item1.Status == VectoRun.Status.Success && x.Item2.Status == VectoRun.Status.Success
							? _cifFactory.GetLorryOVCSuccessResultWriter().GetElement(x)
							: _cifFactory.GetLorryOVCErrorResultWriter().GetElement(x)),
					GetSummary(ordered));
			}

			

			#endregion

			public HEVOVCLorry(ICifResultsWriterFactory cifFactory) : base(cifFactory) { }
		}

		public class PEVLorry : AbstractResultsWriter
		{
			public PEVLorry(ICifResultsWriterFactory cifFactory) : base(cifFactory) { }
		}

		public class ConventionalBus : AbstractResultsWriter
		{
			public ConventionalBus(ICifResultsWriterFactory cifFactory) : base(cifFactory) { }
		}

		public class HEVNonOVCBus : AbstractResultsWriter
		{
			public HEVNonOVCBus(ICifResultsWriterFactory cifFactory) : base(cifFactory) { }
		}

		public class HEVOVCBus : AbstractResultsWriter
		{
			public HEVOVCBus(ICifResultsWriterFactory cifFactory) : base(cifFactory) { }
		}

		public class PEVBus : AbstractResultsWriter
		{
			public PEVBus(ICifResultsWriterFactory cifFactory) : base(cifFactory) { }
		}

		public class ExemptedResultsWriter : AbstractResultsWriter
		{
			

			#region Implementation of IResultsWriter

			public override XElement GenerateResults(List<IResultEntry> results)
			{
				return new XElement(Cif + "Results",
					new XElement(Cif + "Status", "success"),
					new XElement(Cif + "ExemptedVehicle"));
			}

			#endregion

			public ExemptedResultsWriter(ICifResultsWriterFactory cifFactory) : base(cifFactory) { }
		}
	}
}