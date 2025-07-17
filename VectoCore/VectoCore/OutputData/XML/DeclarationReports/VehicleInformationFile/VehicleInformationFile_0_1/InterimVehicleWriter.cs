using System.Linq;
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

		protected XNamespace _vif = XMLDefinitions.VEHICLE_INTERIM_FILE_TARGET_VERSION;
		protected XNamespace _v27 = "urn:tugraz:ivt:VectoAPI:DeclarationDefinitions:v2.7";
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
				? new XElement(_v27 + XMLNames.Vehicle_NgTankSystem, vehicleInput.TankSystem.ToString())
				: null;
			var bodyworkCode = vehicleInput.VehicleCode.HasValue
				? new XElement(_v27 + XMLNames.Vehicle_BodyworkCode, vehicleInput.VehicleCode.ToXMLFormat())
				: null;
			var lowEntry = vehicleInput.LowEntry.HasValue
				? new XElement(_v27 + XMLNames.Bus_LowEntry, vehicleInput.LowEntry)
				: null;
			var doordriveTechnology = vehicleInput.DoorDriveTechnology.HasValue
				? new XElement(_v27 + XMLNames.Bus_DoorDriveTechnology,
					vehicleInput.DoorDriveTechnology.ToXMLFormat())
				: null;
			var vehicleTypeApprovalNumber = string.IsNullOrWhiteSpace(vehicleInput.VehicleTypeApprovalNumber)
				? null
				: new XElement(_v27 + XMLNames.VehicleTypeApprovalNumber, vehicleInput.VehicleTypeApprovalNumber);

            var primaryVehicle = inputData.MultistageJobInputData.JobInputData.PrimaryVehicle.Vehicle;
            var h2PropertiesVehicle = (vehicleInput.H2StorageUsableCapacity != null) ? vehicleInput : primaryVehicle;
			var isH2ICE = h2PropertiesVehicle.H2StorageUsableCapacity != null;

			return new XElement(_vif + XMLNames.Component_Vehicle,
				new XAttribute(XMLNames.Component_ID_Attr, GetVehicleID()),
				new XAttribute(_xsi + XMLNames.XSIType, "Vehicle_Conventional_CompletedBusDeclarationType"),
				new XAttribute("xmlns", _v27),
				//new XAttribute(XNamespace.Xmlns + "xsi", _xsi),
				_vifReportFactory.GetCompletedBusGeneralParametersGroup().GetElements(inputData),
				_vifReportFactory.GetCompletedBusParametersGroup().GetElements(inputData),
				ngTankSystem,
				_vifReportFactory.GetCompletedBusPassengerCountGroup().GetElements(inputData),
				bodyworkCode,
				lowEntry,
				_vifReportFactory.GetCompletedBusDimensionsGroup().GetElements(inputData),
				doordriveTechnology,
				new XElement(_v27 + XMLNames.Bus_VehicleDeclarationType,
					inputData.VehicleInputData.VehicleDeclarationType.GetLabel()),
				vehicleTypeApprovalNumber,
				_vifReportFactory.GetConventionalInterimADASType().GetXmlType(inputData.VehicleInputData.ADAS),
				isH2ICE ? new XElement(_v27 + "H2StorageUsableCapacity", h2PropertiesVehicle.H2StorageUsableCapacity.ToXMLFormat(1)) : null,
                isH2ICE ? new XElement(_v27 + "HydrogenStorageTechnology", h2PropertiesVehicle.HydrogenStorageTechnology?.ToXMLFormat()) : null,
                _vifReportFactory.GetConventionalInterimComponentsType().GetElement(inputData)
			);
		}

		#endregion
	}

	public class HEVInterimVehicleType : InterimVehicleWriter
	{
		public HEVInterimVehicleType(IVIFReportInterimFactory vifReportFactory) : base(vifReportFactory) { }

        protected virtual string VehicleTypeXSD => "Vehicle_HEV_CompletedBusDeclarationType";

        public override XElement GetElement(IMultistageVIFInputData inputData)
		{
			var vehicleInput = inputData.VehicleInputData;
			var ngTankSystem = vehicleInput.TankSystem.HasValue
				? new XElement(_v27 + XMLNames.Vehicle_NgTankSystem, vehicleInput.TankSystem.ToString())
				: null;
			var bodyworkCode = vehicleInput.VehicleCode.HasValue
				? new XElement(_v27 + XMLNames.Vehicle_BodyworkCode, vehicleInput.VehicleCode.ToXMLFormat())
				: null;
			var lowEntry = vehicleInput.LowEntry.HasValue
				? new XElement(_v27 + XMLNames.Bus_LowEntry, vehicleInput.LowEntry)
				: null;
			var doordriveTechnology = vehicleInput.DoorDriveTechnology.HasValue
				? new XElement(_v27 + XMLNames.Bus_DoorDriveTechnology,
					vehicleInput.DoorDriveTechnology.ToXMLFormat())
				: null;
			var vehicleTypeApprovalNumber = string.IsNullOrWhiteSpace(vehicleInput.VehicleTypeApprovalNumber)
				? null
				: new XElement(_v27 + XMLNames.VehicleTypeApprovalNumber, vehicleInput.VehicleTypeApprovalNumber);

			return new XElement(_vif + XMLNames.Component_Vehicle,
				new XAttribute(XMLNames.Component_ID_Attr, GetVehicleID()),
				new XAttribute(_xsi + XMLNames.XSIType, VehicleTypeXSD),
				new XAttribute("xmlns", _v27),
				//new XAttribute(XNamespace.Xmlns + "xsi", _xsi),
				_vifReportFactory.GetCompletedBusGeneralParametersGroup().GetElements(inputData),
				_vifReportFactory.GetCompletedBusParametersGroup().GetElements(inputData),
				ngTankSystem,
				_vifReportFactory.GetCompletedBusPassengerCountGroup().GetElements(inputData),
				bodyworkCode,
				lowEntry,
				_vifReportFactory.GetCompletedBusDimensionsGroup().GetElements(inputData),
				doordriveTechnology,
				new XElement(_v27 + XMLNames.Bus_VehicleDeclarationType,
					inputData.VehicleInputData.VehicleDeclarationType.GetLabel()),
				vehicleTypeApprovalNumber,
				_vifReportFactory.GetHEVInterimADASType().GetXmlType(inputData.VehicleInputData.ADAS),
				_vifReportFactory.GetxEVInterimComponentsType().GetElement(inputData)
			);
		}
	}

	public class PEVInterimVehicleType : InterimVehicleWriter
	{
		public PEVInterimVehicleType(IVIFReportInterimFactory vifReportFactory) : base(vifReportFactory) { }

        protected virtual string VehicleTypeXSD => "Vehicle_PEV_CompletedBusDeclarationType";

        public override XElement GetElement(IMultistageVIFInputData inputData)
		{
			var vehicleInput = inputData.VehicleInputData;
			var bodyworkCode = vehicleInput.VehicleCode.HasValue
				? new XElement(_v27 + XMLNames.Vehicle_BodyworkCode, vehicleInput.VehicleCode.ToXMLFormat())
				: null;
			var lowEntry = vehicleInput.LowEntry.HasValue
				? new XElement(_v27 + XMLNames.Bus_LowEntry, vehicleInput.LowEntry)
				: null;
			var doordriveTechnology = vehicleInput.DoorDriveTechnology.HasValue
				? new XElement(_v27 + XMLNames.Bus_DoorDriveTechnology,
					vehicleInput.DoorDriveTechnology.ToXMLFormat())
				: null;
			var vehicleTypeApprovalNumber = string.IsNullOrWhiteSpace(vehicleInput.VehicleTypeApprovalNumber)
				? null
				: new XElement(_v27 + XMLNames.VehicleTypeApprovalNumber, vehicleInput.VehicleTypeApprovalNumber);

			return new XElement(_vif + XMLNames.Component_Vehicle,
				new XAttribute(XMLNames.Component_ID_Attr, GetVehicleID()),
				new XAttribute(_xsi + XMLNames.XSIType, VehicleTypeXSD),
				new XAttribute("xmlns", _v27),
				//new XAttribute(XNamespace.Xmlns + "xsi", _xsi),
				_vifReportFactory.GetCompletedBusGeneralParametersGroup().GetElements(inputData),
				_vifReportFactory.GetCompletedBusParametersGroup().GetElements(inputData),
				_vifReportFactory.GetCompletedBusPassengerCountGroup().GetElements(inputData),
				bodyworkCode,
				lowEntry,
				_vifReportFactory.GetCompletedBusDimensionsGroup().GetElements(inputData),
				doordriveTechnology,
				new XElement(_v27 + XMLNames.Bus_VehicleDeclarationType,
					inputData.VehicleInputData.VehicleDeclarationType.GetLabel()),
				vehicleTypeApprovalNumber,
				_vifReportFactory.GetPEVInterimADASType().GetXmlType(inputData.VehicleInputData.ADAS),
				_vifReportFactory.GetxEVInterimComponentsType().GetElement(inputData)
			);
		}
	}

	public class FCHV_InterimVehicleType : InterimVehicleWriter
	{
        public FCHV_InterimVehicleType(IVIFReportInterimFactory vifReportFactory) : base(vifReportFactory) { }

		protected virtual string VehicleTypeXSD => "Vehicle_FCHV_CompletedBusDeclarationType";

        public override XElement GetElement(IMultistageVIFInputData inputData)
        {
            var vehicle = inputData.VehicleInputData;

            var bodyworkCode = vehicle.VehicleCode.HasValue
                ? new XElement(_v27 + XMLNames.Vehicle_BodyworkCode, vehicle.VehicleCode.ToXMLFormat())
                : null;

            var lowEntry = vehicle.LowEntry.HasValue
                ? new XElement(_v27 + XMLNames.Bus_LowEntry, vehicle.LowEntry)
                : null;

            var doordriveTechnology = vehicle.DoorDriveTechnology.HasValue
                ? new XElement(_v27 + XMLNames.Bus_DoorDriveTechnology,
                    vehicle.DoorDriveTechnology.ToXMLFormat())
                : null;

            var vehicleTypeApprovalNumber = string.IsNullOrWhiteSpace(vehicle.VehicleTypeApprovalNumber)
                ? null
                : new XElement(_v27 + XMLNames.VehicleTypeApprovalNumber, vehicle.VehicleTypeApprovalNumber);

			var primaryVehicle = inputData.MultistageJobInputData.JobInputData.PrimaryVehicle.Vehicle;

            return new XElement(_vif + XMLNames.Component_Vehicle,
                new XAttribute(XMLNames.Component_ID_Attr, GetVehicleID()),
                new XAttribute(_xsi + XMLNames.XSIType, VehicleTypeXSD),
                new XAttribute("xmlns", _v27),
                _vifReportFactory.GetCompletedBusGeneralParametersGroup().GetElements(inputData),
                _vifReportFactory.GetCompletedBusParametersGroup().GetElements(inputData),
                _vifReportFactory.GetCompletedBusPassengerCountGroup().GetElements(inputData),
                bodyworkCode,
                lowEntry,
                _vifReportFactory.GetCompletedBusDimensionsGroup().GetElements(inputData),
                doordriveTechnology,
                new XElement(_v27 + XMLNames.Bus_VehicleDeclarationType,
                    inputData.VehicleInputData.VehicleDeclarationType.GetLabel()),
                vehicleTypeApprovalNumber,
				new XElement(_v27 + "H2StorageUsableCapacity", primaryVehicle.H2StorageUsableCapacity.ToXMLFormat(1)),
				new XElement(_v27 + "HydrogenStorageTechnology", primaryVehicle.HydrogenStorageTechnology),
				new XElement(_v27 + "DynamicChargingTechnology", primaryVehicle.DynamicChargingTechnology.ToXMLFormat()),
				_vifReportFactory.GetPEVInterimADASType().GetXmlType(inputData.VehicleInputData.ADAS),
                _vifReportFactory.GetxEVInterimComponentsType().GetElement(inputData)
            );
        }
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
				new XAttribute("xmlns", _v27),
				_vifReportFactory.GetCompletedBusGeneralParametersGroup().GetElements(inputData),
				vehicleInput.Model != null
					? new XElement(_v27 + XMLNames.Component_Model, vehicleInput.Model) : null,
				vehicleInput.LegislativeClass != null
					? new XElement(_v27 + XMLNames.Vehicle_LegislativeCategory, vehicleInput.LegislativeClass.ToXMLFormat()) : null,
				vehicleInput.CurbMassChassis != null
					? new XElement(_v27 + XMLNames.CorrectedActualMass, vehicleInput.CurbMassChassis.ToXMLFormat(0)) : null,
				vehicleInput.GrossVehicleMassRating != null
					? new XElement(_v27 + XMLNames.TPMLM, vehicleInput.GrossVehicleMassRating.ToXMLFormat(0)) : null,
				vehicleInput.AirdragModifiedMultistep != null ?
					new XElement(_v27 + XMLNames.Bus_AirdragModifiedMultistep, vehicleInput.AirdragModifiedMultistep) : null,
				vehicleInput.RegisteredClass != null && vehicleInput.RegisteredClass != RegistrationClass.unknown
					? new XElement(_v27 + XMLNames.Vehicle_RegisteredClass, vehicleInput.RegisteredClass.ToXMLFormat()) : null,
				_vifReportFactory.GetCompletedBusPassengerCountGroup().GetElements(inputData),
				vehicleInput.VehicleCode.HasValue
					? new XElement(_v27 + XMLNames.Vehicle_BodyworkCode, vehicleInput.VehicleCode.ToXMLFormat())
					: null,
				vehicleInput.LowEntry != null
					? new XElement(_v27 + XMLNames.Bus_LowEntry, vehicleInput.LowEntry) : null,
				vehicleInput.Height != null
					? new XElement(_v27 + XMLNames.Bus_HeightIntegratedBody, vehicleInput.Height.ConvertToMilliMeter().ToXMLFormat(0)) : null

			);
		}

		#endregion
	}
}