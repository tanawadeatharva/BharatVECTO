using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportXMLTypeWriter.Components
{
    internal class MrfreessSpecificationsTypeWriter : AbstractMrfXmlType, IXmlTypeWriter
	{
		public MrfreessSpecificationsTypeWriter(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Overrides of AbstractMrfXmlType

		public XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			var reessElements = inputData.JobInputData.Vehicle.Components.ElectricStorage.ElectricStorageElements;

			var result = new XElement(_mrf + "REESSSpecifications");

			foreach (var electricStorage in reessElements.OrderBy((element) => element.StringId)) {
				if (electricStorage.REESSPack.StorageType == REESSType.Battery &&
					electricStorage.REESSPack is IBatteryPackDeclarationInputData battery) {
					result.Add(new XElement(_mrf + XMLNames.ElectricEnergyStorage_Battery,
						new XAttribute("stringId", electricStorage.StringId),
						new XElement(_mrf + XMLNames.Component_Model, battery.Model),
						new XElement(_mrf + XMLNames.Component_CertificationNumber, battery.CertificationNumber),
						new XElement(_mrf + XMLNames.DI_Signature_Reference_DigestValue, battery.DigestValue?.DigestValue ?? ""),
						new XElement(_mrf + XMLNames.BusAux_ElectricSystem_NominalVoltage, BatterySOCReader.Create(battery.VoltageCurve).Lookup(0.5).ToXMLFormat(0)),
						new XElement(_mrf + "TotalStorageCapacity", battery.TotalStorageCapacity().ValueAsUnit("kWh", 0)),
						new XElement(_mrf + "TotalUsableCapacityInSimulation", battery.TotalUsableCapacityInSimulation().ValueAsUnit("kWh"), 0),
						new XElement(_mrf + XMLNames.Component_CertificationMethod, battery.CertificationMethod.ToXMLFormat())
						)
					);
				} else if (electricStorage.REESSPack.StorageType == REESSType.SuperCap &&
						electricStorage.REESSPack is ISuperCapDeclarationInputData superCap) {
					result.Add(new XElement(_mrf + XMLNames.ElectricEnergyStorage_Capacitor,
							new XElement(_mrf + XMLNames.Component_Model, superCap.Model),
							new XElement(_mrf + XMLNames.Component_CertificationNumber, superCap.CertificationNumber),
							new XElement(_mrf + XMLNames.DI_Signature_Reference_DigestValue,
								superCap.DigestValue?.DigestValue ?? ""),
							new XElement(_mrf + XMLNames.Capacitor_Capacitance, superCap.Capacity.ToXMLFormat()),
							new XElement(_mrf + XMLNames.Capacitor_MinVoltage, superCap.MinVoltage.ToXMLFormat(2)),
							new XElement(_mrf + XMLNames.Capacitor_MaxVoltage, superCap.MaxVoltage.ToXMLFormat(2))
						)
					);
				} else {
					throw new VectoException("Invalid REESS type");
				}
			}
			return result;
		}

		#endregion
	}
}
