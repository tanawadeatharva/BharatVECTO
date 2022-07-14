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
	public class VIFElectricMachineGENType : AbstractVIFXmlType, IXmlTypeWriter
	{
		public VIFElectricMachineGENType(IVIFReportFactory vifFactory) : base(vifFactory) { }

		#region Implementation of IXmlTypeWriter

		public virtual XElement GetElement(IDeclarationInputDataProvider inputData)
		{

			foreach (var entry in inputData.JobInputData.Vehicle.Components.ElectricMachines.Entries) {

				if (entry.Position == PowertrainPosition.GEN)
					return GetElectricMachineGENType(entry);
			}

			return null;
		}
		
		#endregion


		private XElement GetElectricMachineGENType(ElectricMachineEntry<IElectricMotorDeclarationInputData> electricMachineData)
		{
			return new XElement(_vif + XMLNames.Component_ElectricMachineGEN,
				new XElement(_vif + XMLNames.ElectricMachine_PowertrainPosition, electricMachineData.Position.ToXmlFormat()),
				new XElement(_vif + XMLNames.ElectricMachine_Count, electricMachineData.Count),
				_vifFactory.GetElectricMachineSystemType().GetElement(electricMachineData.ElectricMachine),
				GetADC(electricMachineData.ADC));
		}

		protected virtual XElement GetADC(IADCDeclarationInputData adcData)
		{
			if (adcData == null)
				return null;
			
			var lossEntries = new List<XElement>();
			foreach (DataRow row in adcData.LossMap.Rows) {

				var inputSpeed = row[XMLNames.ADC_TorqueLossMap_InputSpeed].ToString().ToDouble();
				var inputTorque = row[XMLNames.ADC_TorqueLossMap_InputTorque];
				var torqueLoss = row[XMLNames.ADC_TorqueLossMap_TorqueLoss].ToString().ToDouble();

				lossEntries.Add(new XElement(_vif + XMLNames.ADC_TorqueLossMap_Entry,
					new XAttribute(XMLNames.ADC_TorqueLossMap_InputSpeed, inputSpeed.ToXMLFormat(2)),
					new XAttribute(XMLNames.ADC_TorqueLossMap_InputTorque, inputTorque),
					new XAttribute(XMLNames.ADC_TorqueLossMap_TorqueLoss, torqueLoss.ToXMLFormat(2))));
			}

			return new XElement(_vif + XMLNames.Component_ADC,
				new XElement(_vif + XMLNames.ComponentDataWrapper,
					new XAttribute(_xsi + XMLNames.XSIType, "ADCDataDeclarationType"),
					new XAttribute("id", adcData.DigestValue.Reference),
					new XElement(_vif + XMLNames.Component_Manufacturer, adcData.Manufacturer),
					new XElement(_vif + XMLNames.Component_Model, adcData.Model),
					new XElement(_vif + XMLNames.Report_Component_CertificationNumber, adcData.CertificationNumber),
					new XElement(_vif + XMLNames.Component_Date, XmlConvert.ToString(adcData.Date, XmlDateTimeSerializationMode.Utc)),
					new XElement(_vif + XMLNames.Component_AppVersion, adcData.AppVersion),
					new XElement(_vif + XMLNames.ADC_Ratio, adcData.Ratio.ToXMLFormat(3)),
					new XElement(_vif + XMLNames.Component_CertificationMethod, adcData.CertificationMethod.ToXMLFormat()),
					new XElement(_vif + XMLNames.ADC_TorqueLossMap, lossEntries)
				)
			);
		}



	}
}
