using System.Xml.Linq;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Hashing;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.VehicleInformationFile.VehicleInformationFile_0_1
{
	public abstract class InterimVehicleWriter : IXmlMultistepTypeWriter
	{
		protected readonly IVIFReportInterimFactory _vifReportFactory;

		protected XNamespace _vif = "urn:tugraz:ivt:VectoAPI:DeclarationOutput:VehicleInterimFile:v0.1";
		protected XNamespace _v24 = "urn:tugraz:ivt:VectoAPI:DeclarationDefinitions:v2.4";
		protected XNamespace _xsi = XNamespace.Get("http://www.w3.org/2001/XMLSchema-instance");

		protected InterimVehicleWriter(IVIFReportInterimFactory vifReportFactory)
		{
			_vifReportFactory = vifReportFactory;
		}

		#region Implementation of IXmlMultistepTypeWriter

		public abstract XElement GetElement(IMultistageVIFInputData inputData);

		#endregion
	}

	public class ConventionalInterimVehicleType : InterimVehicleWriter
	{
		public ConventionalInterimVehicleType(IVIFReportInterimFactory vifReportFactory) : base(vifReportFactory) { }

		#region Overrides of VehicleWriter

		public override XElement GetElement(IMultistageVIFInputData inputData)
		{
			var vehicleId = $"{VectoComponents.Vehicle.HashIdPrefix()}{XMLHelper.GetGUID()}";
			var vehicleInput = inputData.VehicleInputData;
			var ngTankSystem = vehicleInput.TankSystem.HasValue
				? new XElement(_v24 + XMLNames.Vehicle_NgTankSystem, vehicleInput.TankSystem.ToString())
				: null;
			var bodyworkCode = vehicleInput.VehicleCode.HasValue
				? new XElement(_v24 + XMLNames.Vehicle_BodyworkCode, vehicleInput.VehicleCode.ToXMLFormat())
				: null;
			var lowEntry = vehicleInput.LowEntry.HasValue
				? new XElement(_v24 + XMLNames.Bus_LowEntry, vehicleInput.LowEntry)
				: null;
			var doordriveTechnology = vehicleInput.DoorDriveTechnology.HasValue
				? new XElement(_v24 + XMLNames.Bus_DoorDriveTechnology,
					vehicleInput.DoorDriveTechnology.ToXMLFormat())
				: null;
			var vehicleTypeApprovalNumber = string.IsNullOrWhiteSpace(vehicleInput.VehicleTypeApprovalNumber)
				? null
				: new XElement(_v24 + XMLNames.VehicleTypeApprovalNumber, vehicleInput.VehicleTypeApprovalNumber);

			return new XElement(_vif + XMLNames.Component_Vehicle,
				new XAttribute(XMLNames.Component_ID_Attr, vehicleId),
				new XAttribute(_xsi + XMLNames.XSIType, "Vehicle_Conventional_CompletedBusDeclarationType"),
				new XAttribute("xmlns", _v24),
				//new XAttribute(XNamespace.Xmlns + "xsi", _xsi),
				_vifReportFactory.GetCompletedBusGeneralParametersGroup().GetElements(inputData),
				_vifReportFactory.GetCompletedBusParametersGroup().GetElements(inputData),
				ngTankSystem,
				_vifReportFactory.GetCompletedBusPassengerCountGroup().GetElements(inputData),
				bodyworkCode,
				lowEntry,
				_vifReportFactory.GetCompletedBusDimensionsGroup().GetElements(inputData),
				doordriveTechnology,
				new XElement(_v24 + XMLNames.Bus_VehicleDeclarationType,
					inputData.VehicleInputData.VehicleDeclarationType.GetLabel()),
				vehicleTypeApprovalNumber,
				_vifReportFactory.GetCompletedADASType().GetXmlType(inputData.VehicleInputData.ADAS),
				_vifReportFactory.GetCompletedComponentsType().GetElement(inputData)
			);
		}

		#endregion
	}
}