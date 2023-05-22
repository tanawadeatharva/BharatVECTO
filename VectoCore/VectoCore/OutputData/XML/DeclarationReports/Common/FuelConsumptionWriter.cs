using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.OutputData.ModDataPostprocessing;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.CustomerInformationFile.CustomerInformationFile_0_9.ResultWriter;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.Common
{
    public abstract class FuelConsumptionWriterBase : AbstractResultWriter, IFuelConsumptionWriter
    {
        protected FuelConsumptionWriterBase(ICommonResultsWriterFactory factory, XNamespace ns) : base(factory, ns) { }

        #region Implementation of IFuelConsumptionWriter

        public XElement GetElement(IResultEntry entry, IFuelConsumptionCorrection fc)
        {
			return GetElement(fc.TotalFuelConsumptionCorrected, fc.Fuel, entry.Distance, entry.Payload, entry.CargoVolume, entry.PassengerCount);
        }

        public XElement GetElement(IWeightedResult entry, IFuelProperties fuel, Kilogram consumption)
		{
			return GetElement(consumption, fuel, entry.Distance, entry.Payload, entry.CargoVolume, entry.PassengerCount);
		}

		protected virtual XElement GetElement(Kilogram consumption, IFuelProperties fuel, Meter distance, Kilogram payLoad, CubicMeter cargoVolume, double? passengerCount)
		{
			return new XElement(TNS + XMLNames.Report_Results_Fuel,
				new XAttribute(XMLNames.Report_Results_Fuel_Type_Attr, fuel.FuelType.ToXMLFormat()),
				GetFuelConsumptionEntries(consumption, fuel, distance, payLoad, cargoVolume, passengerCount).Select(x => new XElement(TNS + XMLNames.Report_Results_FuelConsumption, x.ValueAsUnit(3, 1)))
			);
        }

        #endregion

		public abstract IList<ConvertedSI> GetFuelConsumptionEntries(Kilogram fc,
			IFuelProperties fuel, Meter distance, Kilogram payload, CubicMeter volume,
			double? passenger);

    }

    public class LorryFuelConsumptionWriter : FuelConsumptionWriterBase
    {
        public LorryFuelConsumptionWriter(ICommonResultsWriterFactory factory, XNamespace ns) : base(factory, ns) { }


        #region Overrides of FuelConsumptionWriterBase

        public override IList<ConvertedSI> GetFuelConsumptionEntries(Kilogram fc, IFuelProperties fuel, Meter distance, Kilogram payload, CubicMeter volume, double? passenger)
        {
            var retVal = new List<ConvertedSI> {
                (fc / distance).ConvertToGrammPerKiloMeter(),
                (fc / distance /payload).ConvertToGrammPerTonKilometer()};
            if (volume.IsGreater(0)) {
				retVal.Add((fc / distance / volume).ConvertToGrammPerCubicMeterKiloMeter());
			}

			retVal.AddRange(new [] {
                (fc * fuel.LowerHeatingValueVecto / distance).ConvertToMegaJoulePerKilometer(),
                (fc * fuel.LowerHeatingValueVecto / distance / payload).ConvertToMegaJoulePerTonKiloMeter(),
			});
			if (volume.IsGreater(0)) {
                retVal.Add((fc * fuel.LowerHeatingValueVecto / distance / volume).ConvertToMegaJoulePerCubicMeterKiloMeter());
			}

            if (fuel.FuelDensity != null) {
                retVal.AddRange(new[] {
                    (fc / fuel.FuelDensity / distance).ConvertToLiterPer100KiloMeter(),
                    (fc / fuel.FuelDensity / distance / payload).ConvertToLiterPerTonKiloMeter(),
				});
				if (volume.IsGreater(0)) {
					retVal.Add((fc / fuel.FuelDensity / distance / volume).ConvertToLiterPerCubicMeterKiloMeter());
				}
            }

            return retVal;
        }

        #endregion
    }

    public class BusFuelConsumptionWriter : FuelConsumptionWriterBase
    {
        public BusFuelConsumptionWriter(ICommonResultsWriterFactory factory, XNamespace ns) : base(factory, ns) { }


        #region Overrides of FuelConsumptionWriterBase

        public override IList<ConvertedSI> GetFuelConsumptionEntries(Kilogram fc, IFuelProperties fuel,
            Meter distance, Kilogram payload, CubicMeter volume, double? passenger)
        {
            var retVal = new List<ConvertedSI> {
                (fc / distance).ConvertToGrammPerKiloMeter(),
                (fc / distance / passenger.Value).ConvertToGrammPerPassengerKilometer(),

                (fc * fuel.LowerHeatingValueVecto / distance).ConvertToMegaJoulePerKilometer(),
                (fc * fuel.LowerHeatingValueVecto / distance / passenger.Value)
                .ConvertToMegaJoulePerPassengerKilometer(),
            };

            if (fuel.FuelDensity != null) {
                retVal.AddRange(new[] {
                    (fc / fuel.FuelDensity / distance).ConvertToLiterPer100KiloMeter(),
                    (fc / fuel.FuelDensity / distance / passenger.Value).ConvertToLiterPerPassengerKiloMeter(),
                });
            }

            return retVal;
        }

        #endregion

    }
}