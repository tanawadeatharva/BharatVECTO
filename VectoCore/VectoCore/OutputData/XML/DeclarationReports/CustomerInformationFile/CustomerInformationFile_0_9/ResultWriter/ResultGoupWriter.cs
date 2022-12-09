using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.CustomerInformationFile.CustomerInformationFile_0_9.
	ResultWriter
{

	public abstract class AbstractResultWriter : IResultGroupWriter
	{
		protected static readonly XNamespace Cif = "urn:tugraz:ivt:VectoAPI:CustomerOutput:v0.9";
		protected static readonly XNamespace xsi = "http://www.w3.org/2001/XMLSchema-instance";

		protected ICifResultsWriterFactory _cifFactory;

		protected AbstractResultWriter(ICifResultsWriterFactory cifFactory)
		{
			_cifFactory = cifFactory;
		}

		#region Implementation of IResultGroupWriter

		public abstract XElement GetElement(IResultEntry entry);

		public virtual XElement GetElement(Tuple<IResultEntry, IResultEntry> entry)
		{
			throw new NotImplementedException();
		}

		#endregion

		protected XElement[] GetCO2Lorry(IResultEntry entry)
		{
			return new[] {
				new XElement(Cif + XMLNames.Report_Results_CO2,
					new XAttribute(XMLNames.Report_Results_Unit_Attr, "g/km"),
					(entry.CO2Total / entry.Distance).ConvertToGrammPerKiloMeter().ToMinSignificantDigits(3, 2)),
				new XElement(Cif + XMLNames.Report_Results_CO2,
					new XAttribute(XMLNames.Report_Results_Unit_Attr, "g/km"),
					(entry.CO2Total / entry.Distance / entry.Payload).ConvertToGrammPerTonKilometer().ToMinSignificantDigits(3, 2)),
				new XElement(Cif + XMLNames.Report_Results_CO2,
					new XAttribute(XMLNames.Report_Results_Unit_Attr, "g/km"),
					(entry.CO2Total / entry.Distance / entry.CargoVolume).ConvertToGrammPerCubicMeterKiloMeter().ToMinSignificantDigits(3, 2)),
			};
		}
	}

	public class ErrorResultWriter : AbstractResultWriter
	{
		public ErrorResultWriter(ICifResultsWriterFactory cifFactory) : base(cifFactory) { }


		#region Overrides of AbstractResultWriter

		// <n1:Mission>longhaul</n1:Mission>
		//<n1:SimulationParameters>
		//<TotalVehicleMass unit = "kg" > 7800 </ TotalVehicleMass >
		//< Payload unit="kg">2300</Payload>
		//</n1:SimulationParameters>
		public override XElement GetElement(IResultEntry entry)
		{
			if (entry.Status == VectoRun.Status.Success) {
				throw new Exception("At least one entry needs to be unsuccessful!");
			}
			return new XElement(Cif + XMLNames.Report_Result_Result,
				new XAttribute(XMLNames.Report_Result_Status_Attr, "error"),
				new XAttribute(xsi + "type", "ResultErrorType"),
				_cifFactory.GetMissionWriter().GetElement(entry),
				_cifFactory.GetSimulationParameterWriter().GetElement(entry),
				new XElement(Cif + XMLNames.Report_Results_Error, entry.Error),
				new XElement(Cif + XMLNames.Report_Results_ErrorDetails, entry.StackTrace)
				);
		}

		public override XElement GetElement(Tuple<IResultEntry, IResultEntry> entry)
		{
			var errorEntry = new[] {entry.Item1, entry.Item2}.FirstOrDefault(x => x.Status != VectoRun.Status.Success);
			if (errorEntry == null) {
				throw new Exception("At least one entry needs to be unsuccessful!");
			}
			return new XElement(Cif + XMLNames.Report_Result_Result,
				new XAttribute(XMLNames.Report_Result_Status_Attr, "error"),
				new XAttribute(xsi + "type", "ResultErrorType"),
				_cifFactory.GetMissionWriter().GetElement(errorEntry),
				_cifFactory.GetSimulationParameterWriter().GetElement(errorEntry),
				new XElement(Cif + XMLNames.Report_Results_Error, errorEntry.Error),
				new XElement(Cif + XMLNames.Report_Results_ErrorDetails, errorEntry.StackTrace)
			);
		}

		#endregion
	}

	public class ResultMissionWriter : AbstractResultWriter
	{
		public ResultMissionWriter(ICifResultsWriterFactory cifFactory) : base(cifFactory) { }

		#region Overrides of AbstractResultWriter

		public override XElement GetElement(IResultEntry entry)
		{
			return new XElement(Cif + XMLNames.Report_Result_Mission, entry.Mission.ToXMLFormat());
		}

		#endregion
	}

	public class ResultSimulationParameterLorryWriter : AbstractResultWriter
	{
		public ResultSimulationParameterLorryWriter(ICifResultsWriterFactory cifFactory) : base(cifFactory) { }

		#region Overrides of AbstractResultWriter

		public override XElement GetElement(IResultEntry entry)
		{
			return new XElement(Cif + XMLNames.Report_ResultEntry_SimulationParameters,
				new XElement(Cif + XMLNames.Report_ResultEntry_TotalVehicleMass,
					XMLHelper.ValueAsUnit(entry.TotalVehicleMass, XMLNames.Unit_kg)),
				new XElement(Cif + XMLNames.Report_ResultEntry_Payload,
					XMLHelper.ValueAsUnit(entry.Payload, XMLNames.Unit_kg)));
		}

		#endregion
	}

	public class LorryOVCResultWriter : AbstractResultWriter
	{

		public LorryOVCResultWriter(ICifResultsWriterFactory cifFactory) : base(cifFactory) { }

		#region Implementation of IResultGroupWriter

		public override XElement GetElement(IResultEntry entry)
		{
			throw new NotImplementedException();
		}

		public override XElement GetElement(Tuple<IResultEntry, IResultEntry> entry)
		{
			var cs = new[] { entry.Item1, entry.Item2 }.FirstOrDefault(x => x.OVCMode == VectoRunData.OvcHevMode.ChargeSustaining);
			var cd = new[] { entry.Item1, entry.Item2 }.FirstOrDefault(x => x.OVCMode == VectoRunData.OvcHevMode.ChargeDepleting);
			return new XElement(Cif + XMLNames.Report_Result_Result,
				new XAttribute(XMLNames.Report_Result_Status_Attr, "success"),
				new XAttribute(xsi + "type", "ResultSuccessOVCHEVType"),
				_cifFactory.GetMissionWriter().GetElement(entry.Item1),
				_cifFactory.GetSimulationParameterWriter().GetElement(entry.Item1),
				_cifFactory.GetLorryOVCResultWriterChargeDepleting().GetElement(cd),
				_cifFactory.GetLorryOVCResultWriterChargeSustaining().GetElement(cs)
			);
		}

		#endregion
	}

	public class LorryOVCChargeDepletingWriter : AbstractResultWriter
	{
		public LorryOVCChargeDepletingWriter(ICifResultsWriterFactory cifFactory) : base(cifFactory) { }

		#region Overrides of AbstractResultWriter

		public override XElement GetElement(IResultEntry entry)
		{
			return new XElement(Cif + "OVCMode",
				new XAttribute("type", "charge depleting"),
				new XElement(Cif + XMLNames.Report_ResultEntry_AverageSpeed, XMLHelper.ValueAsUnit(entry.AverageSpeed, XMLNames.Unit_kmph, 1)),
				entry.FuelData.Select(f =>
					_cifFactory.GetFuelConsumptionLorry().GetElement(entry, entry.FuelConsumptionFinal(f.FuelType))),
				_cifFactory.GetElectricEnergyConsumptionLorry().GetElement(entry),
				GetCO2Lorry(entry)
			);
		}


		#endregion
	}

	public class LorryOVCChargeSustainingWriter : AbstractResultWriter
	{
		public LorryOVCChargeSustainingWriter(ICifResultsWriterFactory cifFactory) : base(cifFactory) { }

		#region Overrides of AbstractResultWriter

		public override XElement GetElement(IResultEntry entry)
		{
			return new XElement(Cif + "OVCMode",
				new XAttribute("type", "charge depleting"),
				new XElement(Cif + XMLNames.Report_ResultEntry_AverageSpeed, XMLHelper.ValueAsUnit(entry.AverageSpeed, XMLNames.Unit_kmph, 1)),
				entry.FuelData.Select(f =>
					_cifFactory.GetFuelConsumptionLorry().GetElement(entry, entry.FuelConsumptionFinal(f.FuelType))),
				GetCO2Lorry(entry)
			);
		}

		#endregion
	}

	public class LorryFuelConsumptionWriter : AbstractResultWriter, IFuelConsumptionWriter
	{
		public LorryFuelConsumptionWriter(ICifResultsWriterFactory cifFactory) : base(cifFactory) { }

		#region Implementation of IFuelConsumptionWriter

		public XElement GetElement(IResultEntry entry, IFuelConsumptionCorrection fc)
		{
			return new XElement(Cif + XMLNames.Report_Results_Fuel,
				new XAttribute(XMLNames.Report_Results_Fuel_Type_Attr, fc.Fuel.FuelType.ToXMLFormat()),
				new XElement(Cif + XMLNames.Report_Results_FuelConsumption,
					XMLHelper.ValueAsUnit(
						(fc.TotalFuelConsumptionCorrected / entry.Distance).ConvertToGrammPerKiloMeter(), 3, 1)
				),
				new XElement(Cif + XMLNames.Report_Results_FuelConsumption,
					XMLHelper.ValueAsUnit(
						(fc.TotalFuelConsumptionCorrected / entry.Distance / entry.Payload)
						.ConvertToGrammPerTonKilometer(), 3, 1)
				),
				new XElement(Cif + XMLNames.Report_Results_FuelConsumption,
					XMLHelper.ValueAsUnit(
						(fc.TotalFuelConsumptionCorrected / entry.Distance / entry.CargoVolume)
						.ConvertToGrammPerCubicMeterKiloMeter(), 3, 1)
				)
			);
		}

		#endregion

		#region Overrides of AbstractResultWriter

		public override XElement GetElement(IResultEntry entry)
		{
			throw new NotImplementedException();
		}

		#endregion
	}

	public class LorryElectricEnergyConsumptionWriter : AbstractResultWriter
	{
		public LorryElectricEnergyConsumptionWriter(ICifResultsWriterFactory cifFactory) : base(cifFactory) { }

		#region Overrides of AbstractResultWriter

		public override XElement GetElement(IResultEntry entry)
		{
			return null;
		}

		#endregion
	}
}

