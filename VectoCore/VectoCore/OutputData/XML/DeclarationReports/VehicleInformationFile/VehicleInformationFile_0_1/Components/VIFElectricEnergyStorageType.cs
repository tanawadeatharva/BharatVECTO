using System.Collections.Generic;
using System.Data;
using System.Xml;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportXMLTypeWriter;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.VehicleInformationFile.VehicleInformationFile_0_1.Components
{
	public  class VIFElectricEnergyStorageType : AbstractVIFXmlType, IXmlTypeWriter
	{
		public VIFElectricEnergyStorageType(IVIFReportFactory vifFactory) : base(vifFactory) { }

		#region Implementation of IXmlTypeWriter

		public XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			var electricStorages = inputData.JobInputData.Vehicle.Components.ElectricStorage.ElectricStorageElements;
			if (electricStorages == null)
				return null;

			var result = new List<XElement>();
			
			foreach (var entry in electricStorages) {
				switch (entry.REESSPack.StorageType) {
					case REESSType.SuperCap:
						result.Add(GetCapacitor(entry as ISuperCapDeclarationInputData));
						break;
					case REESSType.Battery:
						result.Add(GetBattery(entry as IBatteryPackDeclarationInputData, entry.StringId));
						break;
				}
			}
			
			return new XElement(_vif + XMLNames.Component_ElectricEnergyStorage, result);
		}

		#endregion

		private XElement GetCapacitor(ISuperCapDeclarationInputData capacitor)
		{
			if(capacitor  == null)
				return null;

			return new XElement(_vif + XMLNames.ElectricEnergyStorage_Capacitor,
					new XElement(_vif + XMLNames.ComponentDataWrapper,
						new XAttribute(_xsi + XMLNames.XSIType, "CapacitorSystemDataType"),
						new XAttribute("id", capacitor.DigestValue.Reference),
					new XElement(_vif + XMLNames.Component_Manufacturer, capacitor.Manufacturer),
					new XElement(_vif + XMLNames.Component_Model, capacitor.Model), 
					new XElement(_vif + XMLNames.Report_Component_CertificationNumber, capacitor.CertificationNumber), 
					new XElement(_vif + XMLNames.Component_Date, XmlConvert.ToString( capacitor.Date, XmlDateTimeSerializationMode.Utc)),
					new XElement(_vif + XMLNames.Component_AppVersion, capacitor.AppVersion),
					new XElement(_vif + XMLNames.Component_CertificationMethod, capacitor.CertificationMethod.ToXMLFormat()),
					new XElement(_vif + XMLNames.Capacitor_Capacitance, capacitor.Capacity.ToXMLFormat(2)),
					new XElement(_vif + XMLNames.Capacitor_InternalResistance, capacitor.InternalResistance.ToXMLFormat(2)),
					new XElement(_vif + XMLNames.Capacitor_MinVoltage, capacitor.MinVoltage.ToXMLFormat(2)),
					new XElement(_vif + XMLNames.Capacitor_MaxVoltage, capacitor.MaxVoltage.ToXMLFormat(2)),
					new XElement(_vif + XMLNames.Capacitor_MaxChargingCurrent, capacitor.MaxCurrentCharge.ToXMLFormat(2)),
					new XElement(_vif + XMLNames.Capacitor_MaxDischargingCurrent, capacitor.MaxCurrentDischarge.ToXMLFormat(2)), 
						capacitor.TestingTemperature == null 
						? null 
						: new XElement(_vif + XMLNames.REESS_TestingTemperature, capacitor.TestingTemperature.ToXMLFormat())
					));
		}


		private  XElement GetBattery(IBatteryPackDeclarationInputData battery, int id)
		{
			if (battery == null)
				return null;

			var result = new XElement(_vif + XMLNames.ElectricEnergyStorage_Battery,
				new XElement(_vif + XMLNames.Battery_StringID, id),
				new XElement(_vif + "REESS",
					GetReess(battery),
					GetSignature(battery.DigestValue)),
				battery.MinSOC.HasValue ? null : new XElement(_vif + XMLNames.Battery_SOCmin, battery.MinSOC.Value),
				battery.MaxSOC.HasValue ? null : new XElement(_vif + XMLNames.Battery_SOCmax, battery.MaxSOC.Value)
			);
			
			return result;
		}


		private XElement GetReess(IBatteryPackDeclarationInputData battery)
		{
			return new XElement(_vif + XMLNames.ComponentDataWrapper,
				new XAttribute(_xsi + XMLNames.XSIType, "BatterySystemDataType"),
				new XAttribute("id", battery.DigestValue.Reference),
				new XElement(_vif + XMLNames.Component_Manufacturer, battery.Manufacturer),
				new XElement(_vif + XMLNames.Component_Model, battery.Model),
				new XElement(_vif + XMLNames.Report_Component_CertificationNumber, battery.CertificationNumber),
				new XElement(_vif + XMLNames.Component_Date,
					XmlConvert.ToString(battery.Date, XmlDateTimeSerializationMode.Utc)),
				new XElement(_vif + XMLNames.Component_AppVersion, battery.AppVersion),
				new XElement(_vif + XMLNames.Component_CertificationMethod, battery.CertificationMethod.ToXMLFormat()),
				new XElement(_vif + XMLNames.REESS_BatteryType, battery.BatteryType.ToString()),
				new XElement(_vif + XMLNames.REESS_RatedCapacity, battery.Capacity.ToXMLFormat(2)),
				new XElement(_vif + XMLNames.REESS_ConnectorsSubsystemsIncluded, battery.ConnectorsSubsystemsIncluded),
				new XElement(_vif + XMLNames.REESS_JunctionboxIncluded, battery.JunctionboxIncluded),
				battery.TestingTemperature == null
					? null
					: new XElement(_vif + XMLNames.REESS_TestingTemperature, battery.TestingTemperature.ToXMLFormat()),
				GetOcv(battery.VoltageCurve),
				GetInternalResistance(battery.InternalResistanceCurve),
				GetCurrentLimits(battery.MaxCurrentMap)
				);

		}


		private XElement GetOcv(DataTable voltageCurve)
		{
			var entries = new List<XElement>();

			foreach (DataRow row in voltageCurve.Rows) {
				var soc = row[XMLNames.REESS_OCV_SoC];
				var ocv = row[XMLNames.REESS_OCV_OCV].ToString().ToDouble();

				entries.Add(new XElement(_vif + XMLNames.REESS_MapEntry,
					new XAttribute(XMLNames.REESS_OCV_SoC, soc),
					new XAttribute(XMLNames.REESS_OCV_OCV, ocv.ToXMLFormat(2))));
			}
			return new XElement(_vif + XMLNames.REESS_OCV, entries);
		}


		private XElement GetInternalResistance(DataTable internalResistance)
		{
			var entries = new List<XElement>();

			foreach (DataRow row in internalResistance.Rows) {
				var soc = row[XMLNames.REESS_OCV_SoC];
				var r2 = row[XMLNames.REESS_InternalResistanceCurve_R2].ToString().ToDouble();
				var r10 = row[XMLNames.REESS_InternalResistanceCurve_R10].ToString().ToDouble();
				var r20 = row[XMLNames.REESS_InternalResistanceCurve_R20].ToString().ToDouble();
				var r120 = row[XMLNames.REESS_InternalResistanceCurve_R120].ToString();

				entries.Add(new XElement(_vif + XMLNames.REESS_MapEntry,
					new XAttribute(XMLNames.REESS_OCV_SoC, soc),
					new XAttribute(XMLNames.REESS_InternalResistanceCurve_R2, r2.ToXMLFormat(2)),
					new XAttribute(XMLNames.REESS_InternalResistanceCurve_R10, r10.ToXMLFormat(2)),
					new XAttribute(XMLNames.REESS_InternalResistanceCurve_R20, r20.ToXMLFormat(2)),
					r120.IsNullOrEmpty() ?
						null : new XAttribute(XMLNames.REESS_InternalResistanceCurve_R120, r120.ToDouble().ToXMLFormat(2))
				));
			}
			
			return new XElement(_vif + XMLNames.REESS_InternalResistanceCurve, entries);
		}


		private XElement GetCurrentLimits(DataTable batteryMaxCurrentMap)
		{
			var entries = new List<XElement>();

			foreach (DataRow row in batteryMaxCurrentMap.Rows)
			{
				var soc = row[XMLNames.REESS_CurrentLimits_SoC];
				var maxChargingCurrent = row[XMLNames.REESS_CurrentLimits_MaxChargingCurrent].ToString().ToDouble();
				var maxDischargingCurrent = row[XMLNames.REESS_CurrentLimits_MaxDischargingCurrent].ToString().ToDouble();

				entries.Add(new XElement(_vif + XMLNames.REESS_MapEntry,
					new XAttribute(XMLNames.REESS_OCV_SoC, soc),
					new XAttribute(XMLNames.REESS_CurrentLimits_MaxChargingCurrent, maxChargingCurrent.ToXMLFormat(2)),
					new XAttribute(XMLNames.REESS_CurrentLimits_MaxDischargingCurrent, maxDischargingCurrent.ToXMLFormat(2))
				));
			}

			return new XElement(_vif + XMLNames.REESS_CurrentLimits, entries);
		}
	}
}
