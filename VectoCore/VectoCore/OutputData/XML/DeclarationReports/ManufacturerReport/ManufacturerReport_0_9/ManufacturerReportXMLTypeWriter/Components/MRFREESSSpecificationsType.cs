using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Battery;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportXMLTypeWriter.Components
{
    internal class MRFREESSSpecificationsType : AbstractMrfXmlType, IMrfXmlType
	{
		public MRFREESSSpecificationsType(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Overrides of AbstractMrfXmlType

		public XElement GetXmlType(IDeclarationInputDataProvider inputData)
		{
			var reessElements = inputData.JobInputData.Vehicle.Components.ElectricStorage.ElectricStorageElements;

			var result = new XElement(_mrf + "REESSSpecifications");

			foreach (var electricStorage in reessElements.OrderBy((element) => element.StringId)) {
				var reessXElement = new XElement(_mrf + "REESS",
					new XElement(_mrf + XMLNames.Battery_StringID, electricStorage.StringId));
				result.Add(reessXElement);
				if (electricStorage.REESSPack.StorageType == REESSType.Battery &&
					electricStorage.REESSPack is IBatteryPackDeclarationInputData battery) {
					reessXElement.Add(new XElement(_mrf + XMLNames.ElectricEnergyStorage_Battery,
						new XElement(_mrf + XMLNames.Component_Model, battery.Model),
						new XElement(_mrf + XMLNames.Component_CertificationNumber, battery.CertificationNumber),
						new XElement(_mrf + XMLNames.DI_Signature_Reference_DigestValue,
							battery.DigestValue.DigestValue),
						new XElement(_mrf + XMLNames.BusAux_ElectricSystem_NominalVoltage, BatterySOCReader.Create(battery.VoltageCurve).Lookup(0.5)),
						new XElement(_mrf + "TotalStorageCapacity", battery.Capacity.AsAmpHour.ToXMLFormat(0)),
						new XElement(_mrf + "TotalUsableCapacityInSimulation", "TODO"),
						new XElement(_mrf + XMLNames.Component_CertificationMethod, battery.CertificationMethod),
						new XElement(_mrf + XMLNames.DI_Signature_Reference_DigestValue, battery.DigestValue.DigestValue))
					);
				}else if (electricStorage.REESSPack.StorageType == REESSType.SuperCap &&
						electricStorage.REESSPack is ISuperCapDeclarationInputData superCap) {
					reessXElement.Add(new XElement(_mrf + XMLNames.ElectricEnergyStorage_Capacitor),
						new XElement(_mrf + XMLNames.Component_Model, superCap.Model),
						new XElement(_mrf + XMLNames.Component_CertificationNumber, superCap.CertificationNumber),
						new XElement(_mrf + XMLNames.Capacitor_Capacitance, superCap.Capacity.ToXMLFormat()),
						new XElement(_mrf + XMLNames.Capacitor_MinVoltage, superCap.MinVoltage),
						new XElement(_mrf + XMLNames.Capacitor_MaxVoltage, superCap.MinVoltage),
						new XElement(_mrf + XMLNames.DI_Signature_Reference_DigestValue, superCap.DigestValue.DigestValue)
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
