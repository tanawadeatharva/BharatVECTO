using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.CustomerInformationFile.CustomerInformationFile_0_9.ResultWriter
{
	public interface IResultsWriter
	{
		XElement GenerateResults(List<IResultEntry> results);
	}

	public interface IResultGroupWriter
	{
		XElement GetElement(IResultEntry entry);

		XElement GetElement(IOVCResultEntry entry);
	}

	public interface IFuelConsumptionWriter
	{
		XElement GetElement(IResultEntry entry, IFuelConsumptionCorrection fuelConsumptionCorrection);
		XElement GetElement(IWeightedResult entry, IFuelProperties fuel, Kilogram consumption);

	}

	public interface ICO2Writer
	{
		XElement[] GetElement(IResultEntry entry);

		XElement[] GetElement(IOVCResultEntry entry);
	}

	public interface ICifSummaryWriter
	{
		XElement GetElement(IList<IResultEntry> entries);

		XElement GetElement(IList<IOVCResultEntry> entries);
	}


	public interface ICifResultsWriterFactory
	{
		IResultGroupWriter GetLorryOVCSuccessResultWriter();
		IResultGroupWriter GetLorryOVCErrorResultWriter();

		IResultGroupWriter GetBusOVCSuccessResultWriter();
		IResultGroupWriter GetBusOVCErrorResultWriter();

		IResultGroupWriter GetMissionWriter();
		IResultGroupWriter GetLorrySimulationParameterWriter();
		IResultGroupWriter GetLorryOVCResultWriterChargeDepleting();
		IResultGroupWriter GetLorryOVCResultWriterChargeSustaining();
		IResultGroupWriter GetLorryOVCSummaryWriter();
		IFuelConsumptionWriter GetFuelConsumptionLorry();
		IResultGroupWriter GetElectricEnergyConsumptionLorry();
		ICO2Writer GetCO2ResultLorry();

		ICifSummaryWriter GetLorryOVCCifSummaryWriter();


		IResultGroupWriter GetBusSimulationParameterWriter();
		IResultGroupWriter GetBusOVCResultWriterChargeDepleting();
		IResultGroupWriter GetBusOVCResultWriterChargeSustaining();
		IResultGroupWriter GetBusOVCSummaryWriter();

		IFuelConsumptionWriter GetFuelConsumptionBus();
		IResultGroupWriter GetElectricEnergyConsumptionBus();
		ICO2Writer GetCO2ResultBus();

		ICifSummaryWriter GetBusOVCCifSummaryWriter();

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

		protected List<IOVCResultEntry> GetOrderedResultsOVC(List<IResultEntry> results)
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

		//protected XElement GetSummary(List<IOVCResultEntry> orderedResults)
		//{
		//	var allSuccess = orderedResults.All(x =>
		//		x.ChargeDepletingResult.Status == VectoRun.Status.Success && x.ChargeSustainingResult.Status == VectoRun.Status.Success);
		//	if (!allSuccess) {
		//		// do not write summary unless all simulation runs are successful!
		//		return null;
		//	}

		//	return null;
		//	//return new XElement(Cif + "Summary", 
		//	//	new XElement(Cif + XMLNames.Report_ResultEntry_AverageSpeed, XMLHelper.ValueAsUnit()))
		//}

		//protected XElement GetSummary(List<IResultEntry> results)
		//{
		//	var allSuccess = results.All(x => x.Status == VectoRun.Status.Success);
		//	if (!allSuccess) {
		//		// do not write summary unless all simulation runs are successful!
		//		return null;
		//	}
		//	throw new NotImplementedException();
		//}
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
						x.ChargeDepletingResult.Status == VectoRun.Status.Success &&
						x.ChargeSustainingResult.Status == VectoRun.Status.Success
							? _cifFactory.GetLorryOVCSuccessResultWriter().GetElement(x)
							: _cifFactory.GetLorryOVCErrorResultWriter().GetElement(x)),
					_cifFactory.GetLorryOVCCifSummaryWriter().GetElement(ordered)
				);
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

			public override XElement GenerateResults(List<IResultEntry> results)
			{
				var ordered = GetOrderedResultsOVC(results);
				var allSuccess = results.All(x => x.Status == VectoRun.Status.Success);
				return new XElement(Cif + "Results",
					new XElement(Cif + XMLNames.Report_Result_Status, allSuccess ? "success" : "error"),
					ordered.Select(x =>
						x.ChargeDepletingResult.Status == VectoRun.Status.Success &&
						x.ChargeSustainingResult.Status == VectoRun.Status.Success
							? _cifFactory.GetBusOVCSuccessResultWriter().GetElement(x)
							: _cifFactory.GetBusOVCErrorResultWriter().GetElement(x)),
					_cifFactory.GetBusOVCCifSummaryWriter().GetElement(ordered)
				);
			}
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