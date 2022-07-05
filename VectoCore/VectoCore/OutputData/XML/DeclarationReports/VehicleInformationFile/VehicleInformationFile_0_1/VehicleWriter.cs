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
			commonVehicleGroup.Add(_vifReportFactory.GetConventionalComponentType().GetElement(inputData));

			return new XElement(_vif + XMLNames.Component_Vehicle,
				new XAttribute(_xsi + "type", "vif: ConventionalVehicleVIFType"),
				commonVehicleGroup);
		}

		#endregion
	}
}
