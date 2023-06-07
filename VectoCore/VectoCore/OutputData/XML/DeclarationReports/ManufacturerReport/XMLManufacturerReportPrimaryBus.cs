using System.Linq;
using System.Xml.Linq;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.OutputData.XML {
	public class XMLManufacturerReportPrimaryBus : AbstractXMLManufacturerReport
	{
		public override void Initialize(VectoRunData modelData)
		{
			VehiclePart.Add(
				new XAttribute(xsi + XMLNames.XSIType, "VehiclePrimaryBusType"),
				new XElement(tns + XMLNames.Component_Model, modelData.VehicleData.ModelName),
				new XElement(tns + XMLNames.Component_Manufacturer, modelData.VehicleData.Manufacturer),
				new XElement(tns + XMLNames.Component_ManufacturerAddress, modelData.VehicleData.ManufacturerAddress),
				new XElement(tns + XMLNames.Vehicle_VIN, modelData.VehicleData.VIN),
				new XElement(
					tns + XMLNames.Vehicle_GrossVehicleMass,
					XMLHelper.ValueAsUnit(modelData.VehicleData.InputData.GrossVehicleMassRating, XMLNames.Unit_t, 1)),
				new XElement(
					tns + XMLNames.Vehicle_CurbMassChassis, XMLHelper.ValueAsUnit(modelData.VehicleData.CurbMass, XMLNames.Unit_kg)),
				new XElement(tns + XMLNames.Vehicle_ZeroEmissionVehicle, modelData.VehicleData.ZeroEmissionVehicle),
				new XElement(tns + XMLNames.Vehicle_HybridElectricHDV, modelData.VehicleData.HybridElectricHDV),
				new XElement(tns + XMLNames.Vehicle_DualFuelVehicle, modelData.VehicleData.DualFuelVehicle),
				new XElement(tns + XMLNames.Vehicle_AxleConfiguration, modelData.VehicleData.AxleConfiguration.GetName()),
				new XElement(tns + XMLNames.Report_Vehicle_VehicleGroup, modelData.VehicleData.VehicleClass.GetClassNumber()),

				GetADAS(modelData.VehicleData.ADAS),
				GetTorqueLimits(modelData.EngineData),
				VehicleComponents(modelData)
				
			);

			InputDataIntegrity = GetInputDataSignature(modelData);
		}

		protected override XElement VehicleComponents(VectoRunData modelData)
		{
			return new XElement(
				tns + XMLNames.Vehicle_Components,
				new XAttribute(xsi + XMLNames.XSIType, "ComponentsPrimaryBusType"),
				GetEngineDescription(modelData.EngineData, modelData.VehicleData.InputData.TankSystem),
				GetGearboxDescription(modelData.GearboxData),
				GetTorqueConverterDescription(modelData.GearboxData.TorqueConverterData),
				GetRetarderDescription(modelData.Retarder),
				GetAngledriveDescription(modelData.AngledriveData),
				GetAxlegearDescription(modelData.AxleGearData),
				GetAxleWheelsDescription(modelData.VehicleData),
				GetAuxiliariesDescription(modelData)
			);
		}

		protected override XElement GetAuxiliariesDescription(VectoRunData modelData)
		{
			var busAuxiliaries = modelData.BusAuxiliaries;
			var busAuxXML = busAuxiliaries.InputData.XMLSource;
			var ns = XNamespace.Get(busAuxXML.FirstChild.SchemaInfo.SchemaType.QualifiedName.Namespace);
			const string auxPrefix = "aux";
			return new XElement(
				tns + XMLNames.Component_Auxiliaries,
				new XAttribute(XNamespace.Xmlns + auxPrefix, ns.NamespaceName),
				new XAttribute(xsi + XMLNames.XSIType, $"{auxPrefix}:{busAuxXML.FirstChild.SchemaInfo.SchemaType.QualifiedName.Name}"),
				XElement.Parse(busAuxXML.InnerXml).Elements()
			);
		}
	}
}