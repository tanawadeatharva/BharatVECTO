using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.Resources;

namespace TUGraz.IVT.VectoXML.Writer
{
	public class XMLDeclarationWriter : AbstractXMLWriter
	{
		//private readonly XNamespace _vectoNs = @"../../../API/VectoInput.xsd";


		public XMLDeclarationWriter(string basePath, string vendor) : base(basePath, vendor)
		{
			tns = "urn:tugraz:ivt:VectoAPI:DeclarationDefinitions:v0.6";
			rootNamespace = "urn:tugraz:ivt:VectoAPI:DeclarationInput:v0.6";
		}

		public XDocument GenerateVectoJob(IDeclarationInputDataProvider data)
		{
			var xsi = XNamespace.Get("http://www.w3.org/2001/XMLSchema-instance");

			//<tns:VectoInputDeclaration 
			//  xmlns="urn:tugraz:ivt:VectoAPI:DeclarationDefinitions:v0.6" 
			//  xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" schemaVersion="0.6" 
			//  xmlns:tns="urn:tugraz:ivt:VectoAPI:DeclarationInput:v0.6" 
			//  xsi:schemaLocation="urn:tugraz:ivt:VectoAPI:DeclarationInput:v0.6 http://markus.quaritsch.at/VECTO/VectoInput.xsd">
			var job = new XDocument();
			job.Add(new XElement(rootNamespace + XMLNames.VectoInputDeclaration,
				new XAttribute("schemaVersion", SchemaVersion),
				new XAttribute(XNamespace.Xmlns + "xsi", xsi.NamespaceName),
				new XAttribute("xmlns", tns),
				new XAttribute(XNamespace.Xmlns + "tns", rootNamespace),
				new XAttribute(xsi + "schemaLocation",
					string.Format("{0} {1}VectoInput.xsd", rootNamespace, SchemaLocationBaseUrl)),
				CreateDeclarationJob(data))
				);
			return job;
		}

		protected XElement[] CreateDeclarationJob(IDeclarationInputDataProvider data)
		{
			return new[] {
				CreateVehicle(data)
			};
		}

		protected XElement CreateVehicle(IDeclarationInputDataProvider data)
		{
			var retarder = data.RetarderInputData;
			var gearbox = data.GearboxInputData;
			var vehicle = data.VehicleInputData;
			var angledrive = data.AngledriveInputData;

			var aux = data.AuxiliaryInputData();
			var numSteeredaxles = aux.Auxiliaries.First(x => x.Type == AuxiliaryType.SteeringPump).Technology.Count;
			//var pto = data.PTOTransmissionInputData;

			return new XElement(tns + XMLNames.Component_Vehicle,
				new XAttribute(XMLNames.Component_ID_Attr, "VEH-" + vehicle.ModelName),
				GetDefaultComponentElements(vehicle.TypeId, vehicle.ModelName),
				new XElement(tns + XMLNames.Vehicle_VehicleCategory, GetVehicleCategoryXML(vehicle.VehicleCategory)),
				new XElement(tns + XMLNames.Vehicle_AxleConfiguration, vehicle.AxleConfiguration.GetName()),
				new XElement(tns + XMLNames.Vehicle_CurbWeightChassis, vehicle.CurbWeightChassis.Value()),
				new XElement(tns + XMLNames.Vehicle_GrossVehicleMass, vehicle.GrossVehicleMassRating.Value()),
				new XElement(tns + XMLNames.Vehicle_AirDragArea, vehicle.AirDragArea.Value()),
				new XElement(tns + XMLNames.Vehicle_SteeredAxles, numSteeredaxles),
				new XElement(tns + XMLNames.Vehicle_RetarderType, GetRetarterTypeXML(retarder.Type)),
				retarder.Type.IsDedicatedComponent() ? new XElement(tns + XMLNames.Vehicle_RetarderRatio, retarder.Ratio) : null,
				new XElement(tns + XMLNames.Vehicle_AngledriveType, angledrive.Type),
				new XElement(tns + XMLNames.Vehicle_PTOType, "None"),
				new XElement(tns + XMLNames.Vehicle_Components,
					CreateEngine(data.EngineInputData),
					CreateGearbox(gearbox, data),
					angledrive.Type == AngledriveType.SeparateAngledrive ? CreateAngleDrive(angledrive) : null,
					retarder.Type.IsDedicatedComponent() ? CreateRetarder(retarder) : null,
					CreateAxlegear(data.AxleGearInputData),
					CreateAxleWheels(data.VehicleInputData),
					CreateAuxiliaries(data.AuxiliaryInputData())
					),
				new XElement(tns + XMLNames.Vehicle_AdvancedDriverAssist,
					new XElement(tns + XMLNames.Vehicle_AdvancedDriverAssist_EngineStartStop,
						new XElement(tns + XMLNames.Vehicle_AdvancedDriverAssist_EngineStartStop_Enabled,
							data.DriverInputData.StartStop.Enabled)),
					new XElement(tns + XMLNames.DriverModel_Overspeed,
						new XElement(tns + XMLNames.DriverModel_Overspeed_Mode, data.DriverInputData.OverSpeedEcoRoll.Mode))
					));
		}

		protected XElement CreateEngine(IEngineDeclarationInputData data)
		{
			return new XElement(tns + XMLNames.Component_Engine,
				new XElement(tns + XMLNames.ComponentDataWrapper,
					new XAttribute(XMLNames.Component_ID_Attr, string.Format("ENG-{0}", data.ModelName)),
					GetDefaultComponentElements(string.Format("ENG-{0}", data.ModelName), data.ModelName),
					new XElement(tns + XMLNames.Engine_Displacement, data.Displacement.Value() * 1000 * 1000),
					new XElement(tns + XMLNames.Engine_IdlingSpeed, data.IdleSpeed.AsRPM),
					new XElement(tns + XMLNames.Engine_WHTCUrban, data.WHTCUrban),
					new XElement(tns + XMLNames.Engine_WHTCRural, data.WHTCRural),
					new XElement(tns + XMLNames.Engine_WHTCMotorway, data.WHTCMotorway),
					new XElement(tns + XMLNames.Engine_ColdHotBalancingFactor, data.ColdHotBalancingFactor),
					new XElement(tns + XMLNames.Engine_FuelConsumptionMap,
						EmbedDataTable(data.FuelConsumptionMap, AttributeMappings.FuelConsumptionMapMapping)),
					new XElement(tns + XMLNames.Engine_FullLoadAndDragCurve,
						EmbedDataTable(data.FullLoadCurve, AttributeMappings.EngineFullLoadCurveMapping))
					)
				);
		}

		protected XElement CreateGearbox(IGearboxDeclarationInputData gbxData, IDeclarationInputDataProvider inputData)
		{
			var gears = new XElement(tns + XMLNames.Gearbox_Gears);
			var i = 1;
			foreach (var gearData in gbxData.Gears) {
				var gear = new XElement(tns + XMLNames.Gearbox_Gears_Gear,
					new XAttribute(XMLNames.Gearbox_Gear_GearNumber_Attr, i++),
					new XElement(tns + XMLNames.Gearbox_Gear_Ratio, gearData.Ratio),
					gearData.MaxTorque != null
						? new XElement(tns + XMLNames.Gearbox_Gears_MaxTorque, gearData.MaxTorque.Value())
						: null,
					new XElement(tns + XMLNames.Gearbox_Gear_TorqueLossMap,
						EmbedDataTable(gearData.LossMap, AttributeMappings.TransmissionLossmapMapping))
					);
				gears.Add(gear);
			}
			return new XElement(tns + XMLNames.Component_Gearbox,
				new XElement(tns + XMLNames.ComponentDataWrapper,
					new XAttribute(XMLNames.Component_ID_Attr, string.Format("GBX-{0}", gbxData.ModelName)),
					GetDefaultComponentElements(string.Format("GBX-{0}", gbxData.ModelName), gbxData.ModelName),
					new XElement(tns + XMLNames.Gearbox_TransmissionType, GearboxtypeToXML(gbxData.Type)),
					gears
					),
				gbxData.Type.AutomaticTransmission() ? CreateTorqueConverter(inputData.TorqueConverterInputData) : null);
		}


		private XElement CreateTorqueConverter(ITorqueConverterDeclarationInputData data)
		{
			return new XElement(tns + XMLNames.Component_TorqueConverter,
				new XElement(tns + XMLNames.ComponentDataWrapper,
					new XElement(tns + XMLNames.TorqueConverter_Characteristics,
						EmbedDataTable(data.TCData, AttributeMappings.TorqueConverterDataMapping)
						)
					));
		}

		private XElement CreateAngleDrive(IAngledriveInputData data)
		{
			return new XElement(tns + XMLNames.Component_Angledrive,
				new XElement(tns + XMLNames.ComponentDataWrapper,
					new XAttribute(XMLNames.Component_ID_Attr, "ANGL-" + data.ModelName),
					GetDefaultComponentElements(data.TypeId, data.ModelName),
					new XElement(tns + XMLNames.AngleDrive_Ratio, data.Ratio),
					new XElement(tns + XMLNames.AngleDrive_TorqueLossMap,
						EmbedDataTable(data.LossMap, AttributeMappings.TransmissionLossmapMapping))));
		}

		public XElement CreateRetarder(IRetarderInputData data)
		{
			return new XElement(tns + XMLNames.Component_Retarder,
				new XElement(tns + XMLNames.ComponentDataWrapper,
					new XAttribute(XMLNames.Component_ID_Attr, "RET-none"),
					GetDefaultComponentElements(data.TypeId, data.ModelName),
					new XElement(tns + XMLNames.Retarder_RetarderLossMap,
						EmbedDataTable(data.LossMap, AttributeMappings.RetarderLossmapMapping)
						)
					)
				);
		}

		public XElement CreateAxlegear(IAxleGearInputData data)
		{
			var typeId = string.Format("AXLGEAR-{0:0.000}", data.Ratio);
			return new XElement(tns + XMLNames.Component_Axlegear,
				new XElement(tns + XMLNames.ComponentDataWrapper,
					new XAttribute(XMLNames.Component_ID_Attr, typeId),
					GetDefaultComponentElements(typeId, "N.A."),
					new XElement(tns + XMLNames.Axlegear_Ratio, data.Ratio),
					new XElement(tns + XMLNames.Axlegear_TorqueLossMap,
						EmbedDataTable(data.LossMap, AttributeMappings.TransmissionLossmapMapping)))
				);
		}

		public XElement CreateAxleWheels(IVehicleDeclarationInputData data)
		{
			var axleData = data.Axles;
			var numAxles = axleData.Count;
			var axles = new List<XElement>(numAxles);
			for (var i = 0; i < numAxles; i++) {
				var axle = axleData[i];
				axles.Add(new XElement(tns + XMLNames.AxleWheels_Axles_Axle,
					new XAttribute(XMLNames.AxleWheels_Axles_Axle_AxleNumber_Attr, i + 1),
					new XAttribute(XMLNames.AxleWheels_Axles_Axle_TwinTyres_Attr, axle.TwinTyres),
					new XAttribute(XMLNames.AxleWheels_Axles_Axle_AxleType_Attr,
						i == 1 ? AxleType.VehicleDriven.ToString() : AxleType.VehicleNonDriven.ToString()),
					GetDefaultComponentElements(string.Format("WHEEL-{0}_{1}", i,
						axle.Wheels), axle.Wheels),
					new XElement(tns + XMLNames.AxleWheels_Axles_Axle_Dimension, axle.Wheels),
					new XElement(tns + XMLNames.AxleWheels_Axles_Axle_RRCISO, axle.RollResistanceCoefficient),
					new XElement(tns + XMLNames.AxleWheels_Axles_Axle_FzISO, axle.TyreTestLoad.Value())
					));
			}

			return new XElement(tns + XMLNames.Component_AxleWheels,
				new XElement(tns + XMLNames.ComponentDataWrapper,
					new XAttribute(XMLNames.Component_ID_Attr,
						string.Format("AXLWHL-{0}", data.AxleConfiguration.GetName())),
					new XElement(tns + XMLNames.AxleWheels_Axles, axles))
				);
		}

		public XElement CreateAuxiliaries(IAuxiliariesDeclarationInputData data)
		{
			var auxList = new Dictionary<AuxiliaryType, XElement>();
			foreach (var auxData in data.Auxiliaries) {
				var entry = new XElement(tns + AuxTypeToXML(auxData.Type),
					auxData.Technology.Select(x => new XElement(tns + XMLNames.Auxiliaries_Auxiliary_Technology, x)).ToArray<object>());
				auxList[auxData.Type] = entry;
			}
			var aux = new XElement(tns + XMLNames.ComponentDataWrapper,
				new XAttribute(XMLNames.Component_ID_Attr, "AUX-DECL")
				);
			foreach (
				var key in
					new[] {
						AuxiliaryType.Fan, AuxiliaryType.SteeringPump, AuxiliaryType.ElectricSystem, AuxiliaryType.PneumaticSystem,
						AuxiliaryType.HVAC
					}) {
				aux.Add(auxList[key]);
			}
			return new XElement(tns + XMLNames.Component_Auxiliaries, aux);
		}


		private string AuxTypeToXML(AuxiliaryType type)
		{
			return type.ToString();
		}
	}
}