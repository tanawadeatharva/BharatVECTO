using System.Xml.Linq;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Hashing;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
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

		protected string GetVehicleID()
		{
			return $"{VectoComponents.Vehicle.HashIdPrefix()}{XMLHelper.GetGUID()}";
		}
	}

	public class ConventionalInterimVehicleType : InterimVehicleWriter
	{
		public ConventionalInterimVehicleType(IVIFReportInterimFactory vifReportFactory) : base(vifReportFactory) { }

		#region Overrides of VehicleWriter

		public override XElement GetElement(IMultistageVIFInputData inputData)
		{
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
				new XAttribute(XMLNames.Component_ID_Attr, GetVehicleID()),
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
				_vifReportFactory.GetConventionalInterimADASType().GetXmlType(inputData.VehicleInputData.ADAS),
				_vifReportFactory.GetConventionalInterimComponentsType().GetElement(inputData)
			);
		}

		#endregion
	}

	public class HEVInterimVehicleType : InterimVehicleWriter
	{
		public HEVInterimVehicleType(IVIFReportInterimFactory vifReportFactory) : base(vifReportFactory) { }

		#region Overrides of InterimVehicleWriter

		public override XElement GetElement(IMultistageVIFInputData inputData)
		{
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
				new XAttribute(XMLNames.Component_ID_Attr, GetVehicleID()),
				new XAttribute(_xsi + XMLNames.XSIType, "Vehicle_HEV_CompletedBusDeclarationType"),
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
				_vifReportFactory.GetHEVInterimADASType().GetXmlType(inputData.VehicleInputData.ADAS),
				_vifReportFactory.GetxEVInterimComponentsType().GetElement(inputData)
			);
		}

		#endregion
	}

	public class PEVInterimVehicleType : InterimVehicleWriter
	{
		public PEVInterimVehicleType(IVIFReportInterimFactory vifReportFactory) : base(vifReportFactory) { }

		#region Overrides of InterimVehicleWriter

		public override XElement GetElement(IMultistageVIFInputData inputData)
		{
			var vehicleInput = inputData.VehicleInputData;
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
				new XAttribute(XMLNames.Component_ID_Attr, GetVehicleID()),
				new XAttribute(_xsi + XMLNames.XSIType, "Vehicle_PEV_CompletedBusDeclarationType"),
				new XAttribute("xmlns", _v24),
				//new XAttribute(XNamespace.Xmlns + "xsi", _xsi),
				_vifReportFactory.GetCompletedBusGeneralParametersGroup().GetElements(inputData),
				_vifReportFactory.GetCompletedBusParametersGroup().GetElements(inputData),
				_vifReportFactory.GetCompletedBusPassengerCountGroup().GetElements(inputData),
				bodyworkCode,
				lowEntry,
				_vifReportFactory.GetCompletedBusDimensionsGroup().GetElements(inputData),
				doordriveTechnology,
				new XElement(_v24 + XMLNames.Bus_VehicleDeclarationType,
					inputData.VehicleInputData.VehicleDeclarationType.GetLabel()),
				vehicleTypeApprovalNumber,
				_vifReportFactory.GetPEVInterimADASType().GetXmlType(inputData.VehicleInputData.ADAS),
				_vifReportFactory.GetxEVInterimComponentsType().GetElement(inputData)
			);
		}

		#endregion
	}

	public class IEPCInterimVehicleType : InterimVehicleWriter
	{
		public IEPCInterimVehicleType(IVIFReportInterimFactory vifReportFactory) : base(vifReportFactory) { }

		#region Overrides of InterimVehicleWriter

		public override XElement GetElement(IMultistageVIFInputData inputData)
		{
			var vehicleInput = inputData.VehicleInputData;
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
				new XAttribute(XMLNames.Component_ID_Attr, GetVehicleID()),
				new XAttribute(_xsi + XMLNames.XSIType, "Vehicle_IEPC_CompletedBusDeclarationType"),
				new XAttribute("xmlns", _v24),
				//new XAttribute(XNamespace.Xmlns + "xsi", _xsi),
				_vifReportFactory.GetCompletedBusGeneralParametersGroup().GetElements(inputData),
				_vifReportFactory.GetCompletedBusParametersGroup().GetElements(inputData),
				_vifReportFactory.GetCompletedBusPassengerCountGroup().GetElements(inputData),
				bodyworkCode,
				lowEntry,
				_vifReportFactory.GetCompletedBusDimensionsGroup().GetElements(inputData),
				doordriveTechnology,
				new XElement(_v24 + XMLNames.Bus_VehicleDeclarationType,
					inputData.VehicleInputData.VehicleDeclarationType.GetLabel()),
				vehicleTypeApprovalNumber,
				_vifReportFactory.GetIEPCInterimADASType().GetXmlType(inputData.VehicleInputData.ADAS),
				_vifReportFactory.GetxEVInterimComponentsType().GetElement(inputData)
			);
		}

		#endregion
	}

	public class ExemptedInterimVehicleType : InterimVehicleWriter
	{
		public ExemptedInterimVehicleType(IVIFReportInterimFactory vifReportFactory) : base(vifReportFactory) { }

		#region Overrides of InterimVehicleWriter

		public override XElement GetElement(IMultistageVIFInputData inputData)
		{
			var vehicleInput = inputData.VehicleInputData;
			return new XElement(_vif + XMLNames.Component_Vehicle,
				new XAttribute(XMLNames.Component_ID_Attr, GetVehicleID()),
				new XAttribute(_xsi + XMLNames.XSIType, "Vehicle_Exempted_CompletedBusDeclarationType"),
				new XAttribute("xmlns", _v24),
				_vifReportFactory.GetCompletedBusGeneralParametersGroup().GetElements(inputData),
				vehicleInput.Model != null
					? new XElement(_v24 + XMLNames.Component_Model, vehicleInput.Model) : null,
				vehicleInput.LegislativeClass != null
					? new XElement(_v24 + XMLNames.Vehicle_LegislativeCategory, vehicleInput.LegislativeClass.ToXMLFormat()) : null,
				vehicleInput.CurbMassChassis != null
					? new XElement(_v24 + XMLNames.CorrectedActualMass, vehicleInput.CurbMassChassis.ToXMLFormat(0)) : null,
				vehicleInput.GrossVehicleMassRating != null
					? new XElement(_v24 + XMLNames.TPMLM, vehicleInput.GrossVehicleMassRating.ToXMLFormat(0)) : null,
				vehicleInput.AirdragModifiedMultistep != null ?
					new XElement(_v24 + XMLNames.Bus_AirdragModifiedMultistep, vehicleInput.AirdragModifiedMultistep) : null,
				vehicleInput.RegisteredClass != null && vehicleInput.RegisteredClass != RegistrationClass.unknown
					? new XElement(_v24 + XMLNames.Vehicle_RegisteredClass, vehicleInput.RegisteredClass.ToXMLFormat()) : null,
				_vifReportFactory.GetCompletedBusPassengerCountGroup().GetElements(inputData),
				vehicleInput.VehicleCode.HasValue
					? new XElement(_v24 + XMLNames.Vehicle_BodyworkCode, vehicleInput.VehicleCode.ToXMLFormat())
					: null,
				vehicleInput.LowEntry != null
					? new XElement(_v24 + XMLNames.Bus_LowEntry, vehicleInput.LowEntry) : null,
				vehicleInput.Height != null
					? new XElement(_v24 + XMLNames.Bus_HeightIntegratedBody, vehicleInput.Height.ConvertToMilliMeter().ToXMLFormat(0)) : null

			);
		}

		#endregion
	}
}