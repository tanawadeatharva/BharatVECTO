using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.OutputData.XML
{
	public class XMLPrimaryVehicleReport
	{
		protected XNamespace tns = "urn:tugraz:ivt:VectoAPI:DeclarationOutput:PrimaryBusInformation:HeavyBus:v0.1";
		protected XNamespace di = "http://www.w3.org/2000/09/xmldsig#";

		protected XNamespace RootNS = "urn:tugraz:ivt:VectoAPI:PrimaryVehicleInformation";

		protected XElement VehiclePart;

		protected XElement InputDataIntegrity;

		protected XElement Results;

		public XMLPrimaryVehicleReport() { }

		public XDocument Report
		{
			get { return null; }
		}


		public void GenerateReport(XElement mrfHash) { }

		public virtual void Initialize(VectoRunData modelData, List<List<FuelData.Entry>> fuelModes)
		{
			VehiclePart.Add(
				new XElement(tns + XMLNames.Component_Model, modelData.VehicleData.ModelName),
				new XElement(tns + XMLNames.Component_Manufacturer, modelData.VehicleData.Manufacturer),
				new XElement(tns + XMLNames.Component_ManufacturerAddress, modelData.VehicleData.ManufacturerAddress),
				new XElement(tns + XMLNames.Vehicle_VIN, modelData.VehicleData.VIN),
				new XElement(tns + XMLNames.Vehicle_LegislativeClass, modelData.VehicleData.LegislativeClass.ToXMLFormat()),
				new XElement(
					tns + XMLNames.Vehicle_GrossVehicleMass,
					XMLHelper.ValueAsUnit(modelData.VehicleData.GrossVehicleMass, XMLNames.Unit_t, 1)),
				new XElement(
					tns + XMLNames.Vehicle_CurbMassChassis, XMLHelper.ValueAsUnit(modelData.VehicleData.CurbMass, XMLNames.Unit_kg)),
				new XElement(tns + XMLNames.Vehicle_ZeroEmissionVehicle, modelData.VehicleData.ZeroEmissionVehicle),
				new XElement(tns + XMLNames.Vehicle_HybridElectricHDV, modelData.VehicleData.HybridElectricHDV),
				new XElement(tns + XMLNames.Vehicle_DualFuelVehicle, modelData.VehicleData.DualFuelVehicle),
				new[] {
					new XElement(tns + XMLNames.Vehicle_AxleConfiguration, modelData.VehicleData.AxleConfiguration.GetName()),
					new XElement(tns + XMLNames.Report_Vehicle_VehicleGroup, modelData.VehicleData.VehicleClass.GetClassNumber()),
					new XElement(tns + XMLNames.Vehicle_VocationalVehicle, modelData.VehicleData.VocationalVehicle),
					new XElement(tns + XMLNames.Vehicle_SleeperCab, modelData.VehicleData.SleeperCab),
					new XElement(tns + XMLNames.Vehicle_PTO, modelData.PTO != null),
					GetADAS(modelData.VehicleData.ADAS),
					GetTorqueLimits(modelData.EngineData),
					VehicleComponents(modelData, fuelModes)
				}
			);
		}

		private XElement GetADAS(VehicleData.ADASData adasData)
		{
			return new XElement(tns + XMLNames.Vehicle_ADAS,
								new XElement(tns + XMLNames.Vehicle_ADAS_EngineStopStart, adasData.EngineStopStart),
								new XElement(tns + XMLNames.Vehicle_ADAS_EcoRollWithoutEngineStop, adasData.EcoRoll.WithoutEngineStop()),
								new XElement(tns + XMLNames.Vehicle_ADAS_EcoRollWithEngineStopStart, adasData.EcoRoll.WithEngineStop()),
								new XElement(tns + XMLNames.Vehicle_ADAS_PCC, adasData.PredictiveCruiseControl != PredictiveCruiseControlType.None)
			);
		}

		private XElement GetTorqueLimits(CombustionEngineData modelData)
		{
			var limits = new List<XElement>();
			var maxTorque = modelData.FullLoadCurves[0].MaxTorque;
			for (uint i = 1; i < modelData.FullLoadCurves.Count; i++) {
				if (!maxTorque.IsEqual(modelData.FullLoadCurves[i].MaxTorque, 1e-3.SI<NewtonMeter>())) {
					limits.Add(
						new XElement(
							tns + XMLNames.Vehicle_TorqueLimits_Entry,
							new XAttribute(XMLNames.Vehicle_TorqueLimits_Entry_Gear_Attr, i),
							new XAttribute(
								XMLNames.XMLManufacturerReport_torqueLimit,
								modelData.FullLoadCurves[i].MaxTorque.ToXMLFormat(0)),
							new XAttribute(XMLNames.Report_Results_Unit_Attr, XMLNames.Unit_Nm),
							new XAttribute(
								XMLNames.XMLManufacturerReport_torqueLimitPercent,
								(modelData.FullLoadCurves[i].MaxTorque / maxTorque * 100).ToXMLFormat(1))));
				}
			}

			return limits.Count == 0
				? null
				: new XElement(tns + XMLNames.Vehicle_TorqueLimits, limits.Cast<object>().ToArray());
		}

		private XElement VehicleComponents(VectoRunData modelData, List<List<FuelData.Entry>> fuelModes)
		{
			return null;
			//return new XElement(tns + XMLNames.Vehicle_Components,
			//					GetEngineDescription(modelData.EngineData, fuelModes),
			//					GetGearboxDescription(modelData.GearboxData),
			//					GetAngledriveDescription(modelData.AngledriveData),
			//					GetAxlegearDescription(modelData.AxleGearData),
			//					GetAxleWheelsDescription(modelData.VehicleData),
			//					GetAuxiliariesDescription(modelData.Aux)
			//);
		}
	}
}
