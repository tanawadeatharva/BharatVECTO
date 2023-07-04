using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.CustomerInformationFile.CustomerInformationFile_0_9.ResultWriter;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.Common
{
    public abstract class CO2WriterBase : AbstractResultWriter, ICO2Writer
    {
        protected CO2WriterBase(ICommonResultsWriterFactory factory, XNamespace ns) : base(factory, ns) { }

        public virtual XElement[] GetElements(IResultEntry entry)
		{
			return GetCO2ResultEntries(entry.CO2Total, entry.Distance, entry.Payload, entry.CargoVolume,
					entry.PassengerCount)
				.Select(x => new XElement(TNS + XMLNames.Report_Results_CO2, x.GetElement()))
				.ToArray();
		}

        public virtual XElement[] GetElements(IWeightedResult entry)
		{
			return GetCO2ResultEntries(entry.CO2Total, entry.Distance, entry.Payload, entry.CargoVolume,
					entry.PassengerCount)
				.Select(x => new XElement(TNS + XMLNames.Report_Results_CO2, x.GetElement()))
				.ToArray();
		}

		protected abstract IList<FormattedReportValue> GetCO2ResultEntries(Kilogram co2, Meter distance, Kilogram payload,
			CubicMeter volume, double? passengers);

		protected object[] Format3Significant1Decimal(ConvertedSI value)
		{
			return value.ValueAsUnit(3, 1);
		}

		protected object[] Format1Decimal(ConvertedSI value)
		{
			return value.ValueAsUnit(1);
		}

		protected object[] Format2Decimal(ConvertedSI value)
		{
			return value.ValueAsUnit(2);
		}

    }

    public class FormattedReportValue
	{
		public ConvertedSI Value { get; }

		protected Func<ConvertedSI, object[]> Formatter;

		public FormattedReportValue(ConvertedSI value, Func<ConvertedSI, object[]> formatter = null)
		{
			Value = value;
			Formatter = formatter ?? DefaultFormat;
		}

		public object[] GetElement()
		{
			return Formatter(Value);
		}

		protected object[] DefaultFormat(ConvertedSI value)
		{
			return Value.ValueAsUnit(3, 1);
		}

	}

    public class LorryCO2Writer : CO2WriterBase
    {
        public LorryCO2Writer(ICommonResultsWriterFactory factory, XNamespace ns) : base(factory, ns) { }


		protected override IList<FormattedReportValue> GetCO2ResultEntries(Kilogram CO2Total, Meter distance, Kilogram payload,
			CubicMeter volume, double? passengers)
		{
			var retVal = new List<FormattedReportValue>() {
				new FormattedReportValue((CO2Total / distance).ConvertToGrammPerKiloMeter()),
				new FormattedReportValue((CO2Total / distance / payload).ConvertToGrammPerTonKilometer(), Format1Decimal),
			};
			if (volume.IsGreater(0)) {
				retVal.Add(new FormattedReportValue((CO2Total / distance / volume).ConvertToGrammPerCubicMeterKiloMeter()));
			}
			return retVal;
		}

	}

    public class BusCO2Writer : CO2WriterBase
    {
        public BusCO2Writer(ICommonResultsWriterFactory factory, XNamespace ns) : base(factory, ns) { }

		protected override IList<FormattedReportValue> GetCO2ResultEntries(Kilogram CO2Total, Meter distance, Kilogram payload,
			CubicMeter volume, double? passengers)
		{
			return new[] {
				new FormattedReportValue((CO2Total / distance).ConvertToGrammPerKiloMeter()),
				new FormattedReportValue((CO2Total / distance / passengers.Value).ConvertToGrammPerPassengerKilometer(), Format2Decimal),
			};
		}

	}

    public class BusPEVCO2Writer : BusCO2Writer
    {
        public BusPEVCO2Writer(ICommonResultsWriterFactory factory, XNamespace ns) : base(factory, ns) { }

        #region Implementation of ICO2Writer

        public override XElement[] GetElements(IResultEntry entry)
        {
            if (entry.AuxHeaterFuel == null) {
                return null;
            }

            var tmp = _factory.GetFuelConsumptionBus(_factory, TNS);
			return new[] {
				new XElement(TNS + XMLNames.Report_ResultEntry_FCZEVAuxHeater,
					new XAttribute(XMLNames.Report_Results_Fuel_Type_Attr, entry.AuxHeaterFuel.FuelType.ToXMLFormat()),
					tmp?.GetFuelConsumptionEntries(entry.ZEV_FuelConsumption_AuxHtr, entry.AuxHeaterFuel, entry.Distance,
						entry.Payload, entry.CargoVolume, entry.PassengerCount).Select(x =>
						new XElement(TNS + XMLNames.Report_Results_FuelConsumption, x.ValueAsUnit(3, 1)))
				),
				new XElement(TNS + XMLNames.Report_ResultEntry_CO2ZEVAuxHeater,
					GetCO2ResultEntries(entry.ZEV_CO2, entry.Distance, entry.Payload, entry.CargoVolume,
						entry.PassengerCount).Select(x =>
						new XElement(TNS + XMLNames.Report_Results_CO2, x.GetElement()))
				)
			};
		}

        public override XElement[] GetElements(IWeightedResult entry)
        {
            if (entry.AuxHeaterFuel == null) {
                return null;
            }

            var tmp = _factory.GetFuelConsumptionBus(_factory, TNS) as BusFuelConsumptionWriter;
            return new[] {
                new XElement(TNS + XMLNames.Report_ResultEntry_FCZEVAuxHeater,
                    new XAttribute(XMLNames.Report_Results_Fuel_Type_Attr, entry.AuxHeaterFuel.FuelType.ToXMLFormat()),
                    tmp?.GetFuelConsumptionEntries(entry.ZEV_FuelConsumption_AuxHtr, entry.AuxHeaterFuel, entry.Distance,
                        entry.Payload, entry.CargoVolume, entry.PassengerCount).Select(x =>
                        new XElement(TNS + XMLNames.Report_Results_FuelConsumption, x.ValueAsUnit(3, 1)))
                ),
                new XElement(TNS + XMLNames.Report_ResultEntry_CO2ZEVAuxHeater,
                    GetCO2ResultEntries(entry.ZEV_CO2, entry.Distance, entry.Payload, entry.CargoVolume,
                        entry.PassengerCount).Select(x =>
                        new XElement(TNS + XMLNames.Report_Results_CO2, x.GetElement()))
                )
            };
        }

        #endregion

    }
}