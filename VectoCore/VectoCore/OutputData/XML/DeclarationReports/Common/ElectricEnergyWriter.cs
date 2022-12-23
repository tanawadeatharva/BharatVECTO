using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.Common
{

    public abstract class ElectricEnergyConsumptionWriterBase : AbstractResultWriter, IElectricEnergyConsumptionWriter
    {
        public ElectricEnergyConsumptionWriterBase(ICommonResultsWriterFactory factory, XNamespace ns) : base(factory, ns) { }

        #region Overrides of AbstractResultWriter

        public virtual XElement GetElement(IResultEntry entry)
        {
            return new XElement(TNS + ElectricEnergyConsumptionXMLElementName,
                GetEnergyConsumption(entry.ElectricEnergyConsumption, entry.Distance, entry.Payload, entry.CargoVolume,
                    entry.PassengerCount).Select(x =>
                    new XElement(TNS + XMLNames.Report_Result_EnergyConsumption, x.ValueAsUnit(3, 1)))
            );
        }

        public virtual XElement GetElement(IWeightedResult weighted)
        {
            return new XElement(TNS + ElectricEnergyConsumptionXMLElementName,
                GetEnergyConsumption(weighted.ElectricEnergyConsumption, weighted.Distance, weighted.Payload, weighted.CargoVolume,
                    weighted.PassengerCount).Select(x =>
                    new XElement(TNS + XMLNames.Report_Result_EnergyConsumption, x.ValueAsUnit(3, 1)))
            );

        }

        protected abstract IList<ConvertedSI> GetEnergyConsumption(WattSecond elEnergy, Meter distance,
            Kilogram payload, CubicMeter volume, double? passengers);

        #endregion

		protected virtual string ElectricEnergyConsumptionXMLElementName => XMLNames.Report_ResultEntry_ElectricEnergyConsumption;
	}

    public class LorryElectricEnergyConsumptionWriter : ElectricEnergyConsumptionWriterBase
    {
        public LorryElectricEnergyConsumptionWriter(ICommonResultsWriterFactory factory, XNamespace ns) : base(factory, ns) { }

        #region Overrides of ElectricEnergyConsumptionWriterBase

        protected override IList<ConvertedSI> GetEnergyConsumption(WattSecond elEnergy, Meter distance,
            Kilogram payload, CubicMeter volume, double? passengers)
		{
			var retVal = new List<ConvertedSI>() {
				(elEnergy / distance).ConvertToKiloWattHourPerKiloMeter(),
				(elEnergy / distance / payload).ConvertToKiloWattHourPerTonKiloMeter(),
			};
			if (volume?.IsGreater(0) ?? false) {
				retVal.Add((elEnergy / distance / volume).ConvertToKiloWattHourPerCubicMeterKiloMeter());
			}

			retVal.AddRange(new[] {
				(elEnergy / distance).ConvertToMegaJoulePerKiloMeter(),
				(elEnergy / distance / payload).ConvertToMegaJoulePerTonKiloMeter()
			});
            if (volume?.IsGreater(0) ?? false){
                retVal.Add((elEnergy / distance / volume).ConvertToMegaJoulePerCubicMeterKiloMeter());
            }
            return retVal;
        }

        #endregion
    }

    public class BusElectricEnergyConsumptionWriter : ElectricEnergyConsumptionWriterBase
    {
        public BusElectricEnergyConsumptionWriter(ICommonResultsWriterFactory factory, XNamespace ns) : base(factory, ns) { }

        #region Overrides of ElectricEnergyConsumptionWriterBase

        protected override IList<ConvertedSI> GetEnergyConsumption(WattSecond elEnergy, Meter distance,
            Kilogram payload, CubicMeter volume, double? passengers)
        {
            return new[] {
                (elEnergy / distance).ConvertToKiloWattHourPerKiloMeter(),
                (elEnergy / distance / passengers.Value).ConvertToKiloWattHourPerPassengerKiloMeter(),

                (elEnergy / distance).ConvertToMegaJoulePerKiloMeter(),
                (elEnergy / distance / passengers.Value).ConvertToMegaJoulePerPassengerKiloMeter(),
            };
        }

        #endregion
    }
}