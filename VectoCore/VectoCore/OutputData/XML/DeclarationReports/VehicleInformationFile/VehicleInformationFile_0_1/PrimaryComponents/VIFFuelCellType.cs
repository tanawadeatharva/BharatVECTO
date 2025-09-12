using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportXMLTypeWriter;


namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.VehicleInformationFile.VehicleInformationFile_0_1.Components
{
	public class VIFFuelCellType : AbstractVIFXmlType, IXmlTypeWriter
	{
		public VIFFuelCellType(IVIFReportFactory vifFactory) : base(vifFactory) { }

		#region Implementation of IXmlTypeWriter

		public XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			var fuelCellSystem = inputData.JobInputData.Vehicle.Components.FuelCellSystem;
			if (fuelCellSystem == null)
			{
				return null;
			}

			var moduleContent = new List<XElement>();
			foreach(var module in fuelCellSystem.FuelCellModules)
			{
				var fcXElement = new XElement(_vif + XMLNames.FuelCell_Module,
					new XElement(_vif + XMLNames.FuelCell_Count, module.Count),
					new XElement(_vif + XMLNames.ComponentDataWrapper,
						new XElement(_vif + XMLNames.Component_Manufacturer, module.FuelCell.Manufacturer),
						new XElement(_vif + XMLNames.Component_Model, module.FuelCell.Model),
						new XElement(_vif + XMLNames.Component_CertificationMethod, module.FuelCell.CertificationMethod.ToXMLFormat()),
						module.FuelCell.CertificationMethod == CertificationMethod.StandardValues
							? null
							: new XElement(_vif + XMLNames.Component_CertificationNumber, module.FuelCell.CertificationNumber),
						new XElement(_vif + XMLNames.Component_Date, XmlConvert.ToString(module.FuelCell.Date, XmlDateTimeSerializationMode.Utc)),
						new XElement(_vif + XMLNames.Component_AppVersion, module.FuelCell.AppVersion),
						new XElement(_vif + XMLNames.FuelCell_FCSRatedPower, module.FuelCell.FCSRatedPower.ToXMLFormat(0))
					),
				 	(module.MinPower != null) ? new XElement(_vif + XMLNames.FuelCell_MinPower, module.MinPower.ToXMLFormat(0)) : null,
					(module.MaxPower != null) ? new XElement(_vif + XMLNames.FuelCell_MaxPower, module.MaxPower.ToXMLFormat(0)) : null
				);

				moduleContent.Add(fcXElement);
			}

			return new XElement(_vif + XMLNames.Component_FuelCell,
				moduleContent
			);
		}

		#endregion
	}
}
