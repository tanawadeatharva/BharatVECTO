using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportXMLTypeWriter;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.VehicleInformationFile.VehicleInformationFile_0_1
{
	public abstract class VehicleWriter : IXmlTypeWriter
	{
		protected readonly IVIFReportFactory _vifReportFactory;
		protected XNamespace _vif = "urn:tugraz:ivt:VectoAPI:DeclarationOutput:VehicleInterimFile:v0.1";
		protected XNamespace _xsi = XNamespace.Get("http://www.w3.org/2001/XMLSchema-instance");


		public VehicleWriter(IVIFReportFactory vifReportFactory)
		{
			_vifReportFactory = vifReportFactory;
		}
		

		#region Implementation of IXmlTypeWriter

		public abstract XElement GetElement(IDeclarationInputDataProvider inputData);


		#endregion
	}


	public class VIFConventionalVehicle : VehicleWriter
	{
		public VIFConventionalVehicle(IVIFReportFactory vifReportFactory) : base(vifReportFactory)
		{
		}
		
		#region Overrides of VehicleWriter
		
		public override XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			var commonVehicleGroup = _vifReportFactory.GetConventionalVehicleGroup().GetElements(inputData);

			var component = new XElement(_vif + XMLNames.Vehicle_Components,
							new XAttribute(_xsi + "type", "vif:Vehicle_Conventional_ComponentsVIFType"),
							_vifReportFactory.GetEngineType().GetElement(inputData),
							_vifReportFactory.GetTransmissionType().GetElement(inputData),
							_vifReportFactory.GetTorqueConvertType().GetElement(inputData),
							_vifReportFactory.GetAngelDriveType().GetElement(inputData),
							_vifReportFactory.GetAxlegearType().GetElement(inputData),
							_vifReportFactory.GetAxleWheelsType().GetElement(inputData),
							_vifReportFactory.GetAuxiliaryType().GetElement(inputData));

			commonVehicleGroup.Add(component);

			return new XElement(_vif + XMLNames.Component_Vehicle, commonVehicleGroup);
		}

		#endregion
	}
}
