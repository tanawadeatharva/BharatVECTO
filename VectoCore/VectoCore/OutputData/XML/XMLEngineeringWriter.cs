using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Resources;
using TUGraz.VectoCore.Utils;

namespace TUGraz.IVT.VectoXML.Writer
{
	public class XMLEngineeringWriter : AbstractXMLWriter
	{
		private readonly bool _singleFile;
		private readonly XNamespace _declarationNamespace;


		public XMLEngineeringWriter(string basePath, bool singleFile, string vendor) : base(basePath, vendor)
		{
			_singleFile = singleFile;

			tns = Constants.XML.VectoEngineeringDefinitionsNS; // "urn:tugraz:ivt:VectoAPI:EngineeringDefinitions:v0.6";
			rootNamespace = Constants.XML.VectoEngineeringInputNS; // "urn:tugraz:ivt:VectoAPI:EngineeringInput:v0.6";
			_declarationNamespace = Constants.XML.VectoDeclarationDefinitionsNS;
			//"urn:tugraz:ivt:VectoAPI:DeclarationDefinitions:v0.6";
		}

		public XDocument GenerateVectoJob(IEngineeringInputDataProvider data)
		{
			var xsi = XNamespace.Get("http://www.w3.org/2001/XMLSchema-instance");
			var xsd = XNamespace.Get("http://www.w3.org/2001/XMLSchema");

			// <tns:VectoInputEngineering 
			//    xmlns="urn:tugraz:ivt:VectoAPI:EngineeringDefinitions:v0.6" 
			//    xmlns:tns="urn:tugraz:ivt:VectoAPI:EngineeringInput:v0.6" 
			//    xmlns:vdecdef="urn:tugraz:ivt:VectoAPI:DeclarationDefinitions:v0.6" 
			//    schemaVersion="0.6" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" 
			//    xsi:schemaLocation="urn:tugraz:ivt:VectoAPI:EngineeringInput:v0.6 VectoEngineeringInput.xsd">
			var job = new XDocument();
			job.Add(new XElement(rootNamespace + XMLNames.VectoInputEngineering,
				new XAttribute("schemaVersion", SchemaVersion),
				new XAttribute(XNamespace.Xmlns + "xsi", xsi.NamespaceName),
				new XAttribute("xmlns", tns),
				new XAttribute(XNamespace.Xmlns + "tns", rootNamespace),
				new XAttribute(XNamespace.Xmlns + "vdecdef", _declarationNamespace),
				new XAttribute(xsi + "schemaLocation",
					string.Format("{0} {1}VectoEngineeringInput.xsd", rootNamespace, SchemaLocationBaseUrl)),
				data.JobInputData().EngineOnlyMode
					? CreateEngineOnly(data)
					: CreateEngineeringJob(data))
				);
			return job;
		}

		protected XElement[] CreateEngineOnly(IEngineeringInputDataProvider data)
		{
			return new[] {
				new XElement(tns + XMLNames.VectoJob_EngineOnlyMode, true),
				CreateEngine(data.EngineInputData, false),
				CreateMissions(data.JobInputData().Cycles)
			};
		}

		protected XElement[] CreateEngineeringJob(IEngineeringInputDataProvider data)
		{
			return new[] {
				new XElement(tns + XMLNames.VectoJob_EngineOnlyMode, false),
				CreateVehicle(data),
				CreateDriverModel(data),
				CreateMissions(data.JobInputData().Cycles)
			};
		}

		private XElement CreateMissions(IEnumerable<ICycleData> data)
		{
			var retVal = new XElement(tns + XMLNames.VectoJob_MissionCycles);
			foreach (var cycle in data) {
				var filename = cycle.CycleData.Source;
				if (filename == null) {
					continue;
				}
				VectoCSVFile.Write(Path.Combine(BasePath, Path.GetFileName(filename)), cycle.CycleData);
				retVal.Add(new XElement(tns + XMLNames.Missions_Cycle,
					new XAttribute(XMLNames.ExtResource_Type_Attr, XMLNames.ExtResource_Type_Value_CSV),
					new XAttribute(XMLNames.ExtResource_File_Attr, Path.GetFileName(filename))
					)
					);
			}
			return retVal;
		}

		private XElement CreateDriverModel(IEngineeringInputDataProvider engineering)
		{
			var driver = engineering.DriverInputData;
			var gbx = engineering.GearboxInputData;
			var lookahead = driver.Lookahead;
			var overspeed = driver.OverSpeedEcoRoll;

			return new XElement(tns + XMLNames.Component_DriverModel,
				new XElement(tns + XMLNames.DriverModel_LookAheadCoasting,
					new XElement(tns + XMLNames.DriverModel_LookAheadCoasting_Enabled, lookahead.Enabled),
					new XElement(tns + XMLNames.DriverModel_LookAheadCoasting_MinSpeed, lookahead.MinSpeed.AsKmph),
					new XElement(tns + XMLNames.DriverModel_LookAheadCoasting_PreviewDistanceFactor, lookahead.LookaheadDistanceFactor),
					new XElement(tns + XMLNames.DriverModel_LookAheadCoasting_DecisionFactorOffset,
						lookahead.CoastingDecisionFactorOffset),
					new XElement(tns + XMLNames.DriverModel_LookAheadCoasting_DecisionFactorScaling,
						lookahead.CoastingDecisionFactorScaling),
					lookahead.CoastingDecisionFactorTargetSpeedLookup == null
						? null
						: new XElement(tns + XMLNames.DriverModel_LookAheadCoasting_SpeedDependentDecisionFactor,
							_singleFile
								? EmbedDataTable(lookahead.CoastingDecisionFactorTargetSpeedLookup,
									AttributeMappings.CoastingDFTargetSpeedLookupMapping)
								: ExtCSVResource(lookahead.CoastingDecisionFactorTargetSpeedLookup, "Driver_LAC_TargetspeedLookup.csv")),
					lookahead.CoastingDecisionFactorVelocityDropLookup == null
						? null
						: new XElement(tns + XMLNames.DriverModel_LookAheadCoasting_VelocityDropDecisionFactor,
							_singleFile
								? EmbedDataTable(lookahead.CoastingDecisionFactorVelocityDropLookup,
									AttributeMappings.CoastingDFVelocityDropLookupMapping)
								: ExtCSVResource(lookahead.CoastingDecisionFactorVelocityDropLookup, "Driver_LAC_VelocityDropLookup.csv"))
					),
				new XElement(tns + XMLNames.DriverModel_Overspeed,
					new XElement(tns + XMLNames.DriverModel_Overspeed_Mode, driver.OverSpeedEcoRoll.Mode),
					new XElement(tns + XMLNames.DriverModel_Overspeed_MinSpeed, overspeed.MinSpeed.AsKmph),
					new XElement(tns + XMLNames.DriverModel_Overspeed_AllowedOverspeed, overspeed.OverSpeed.AsKmph),
					new XElement(tns + XMLNames.DriverModel_Overspeed_AllowedUnderspeed, overspeed.UnderSpeed.AsKmph)
					),
				driver.AccelerationCurve == null
					? null
					: new XElement(tns + XMLNames.DriverModel_DriverAccelerationCurve,
						_singleFile
							? EmbedDataTable(driver.AccelerationCurve, AttributeMappings.DriverAccelerationCurveMapping)
							: ExtCSVResource(driver.AccelerationCurve,
								Path.GetFileName(driver.AccelerationCurve.Source ?? "Driver.vacc"))
						),
				new XElement(tns + XMLNames.DriverModel_ShiftStrategyParameters,
					new XElement(tns + XMLNames.DriverModel_ShiftStrategyParameters_UpshiftMinAcceleration,
						gbx.UpshiftMinAcceleration.Value()),
					new XElement(tns + XMLNames.DriverModel_ShiftStrategyParameters_DownshiftAfterUpshiftDelay,
						gbx.DownshiftAfterUpshiftDelay.Value()),
					new XElement(tns + XMLNames.DriverModel_ShiftStrategyParameters_UpshiftAfterDownshiftDelay,
						gbx.UpshiftAfterDownshiftDelay.Value()),
					new XElement(tns + XMLNames.DriverModel_ShiftStrategyParameters_TorqueReserve, gbx.TorqueReserve),
					new XElement(tns + XMLNames.DriverModel_ShiftStrategyParameters_TimeBetweenGearshift,
						gbx.MinTimeBetweenGearshift.Value()),
					new XElement(tns + XMLNames.DriverModel_ShiftStrategyParameters_StartSpeed, gbx.StartSpeed.Value()),
					new XElement(tns + XMLNames.DriverModel_ShiftStrategyParameters_StartAcceleration, gbx.StartAcceleration.Value()),
					new XElement(tns + XMLNames.DriverModel_ShiftStrategyParameters_StartTorqueReserve, gbx.StartTorqueReserve))
				);
		}

		protected XElement CreateVehicle(IEngineeringInputDataProvider data)
		{
			var retarder = data.RetarderInputData;
			var gearbox = data.GearboxInputData;
			var vehicle = data.VehicleInputData;
			var angledrive = data.AngledriveInputData;
			var pto = data.PTOTransmissionInputData;

			return new XElement(tns + XMLNames.Component_Vehicle,
				new XAttribute(XMLNames.Component_ID_Attr, "VEH-" + vehicle.ModelName),
				GetDefaultComponentElements(vehicle.TypeId, vehicle.ModelName),
				new XElement(tns + XMLNames.Vehicle_VehicleCategory, GetVehicleCategoryXML(vehicle.VehicleCategory)),
				new XElement(tns + XMLNames.Vehicle_AxleConfiguration, vehicle.AxleConfiguration.GetName()),
				new XElement(tns + XMLNames.Vehicle_CurbWeightChassis, vehicle.CurbWeightChassis.Value()),
				new XElement(tns + XMLNames.Vehicle_GrossVehicleMass, vehicle.GrossVehicleMassRating.Value()),
				new XElement(tns + XMLNames.Vehicle_AirDragArea, vehicle.AirDragArea.Value()),
				new XElement(tns + XMLNames.Vehicle_RetarderType, GetRetarterTypeXML(retarder.Type)),
				retarder.Type.IsDedicatedComponent() ? new XElement(tns + XMLNames.Vehicle_RetarderRatio, retarder.Ratio) : null,
				new XElement(tns + XMLNames.Vehicle_AngledriveType, angledrive.Type),
				new XElement(tns + XMLNames.Vehicle_PTOType, pto.PTOTransmissionType),
				GetPTOData(pto),
				new XElement(tns + XMLNames.Vehicle_CurbWeightExtra, vehicle.CurbWeightExtra.Value()),
				new XElement(tns + XMLNames.Vehicle_Loading, vehicle.Loading.Value()),
				new XElement(tns + XMLNames.Vehicle_CrossWindCorrectionMode, GetCorrectionModeXML(vehicle.CrossWindCorrectionMode)),
				GetCrossWindCorrectionData(vehicle),
				new XElement(tns + XMLNames.Vehicle_Components,
					CreateEngine(data.EngineInputData),
					CreateGearbox(gearbox),
					angledrive.Type == AngledriveType.SeparateAngledrive ? CreateAngleDrive(angledrive) : null,
					retarder.Type.IsDedicatedComponent() ? CreateRetarder(retarder) : null,
					CreateAxlegear(data.AxleGearInputData),
					CreateAxleWheels(data.VehicleInputData),
					CreateAuxiliaries(data.AuxiliaryInputData(), RemoveInvalidFileCharacters(data.VehicleInputData.ModelName))
					),
				new XElement(tns + XMLNames.Vehicle_AdvancedDriverAssist,
					new XElement(tns + XMLNames.Vehicle_AdvancedDriverAssist_EngineStartStop,
						new XElement(tns + XMLNames.Vehicle_AdvancedDriverAssist_EngineStartStop_Enabled, false))
					));
		}


		private object[] GetPTOData(IPTOTransmissionInputData pto)
		{
			if (pto.PTOTransmissionType == "None") {
				return null;
			}
			var ptoLossMap = new XElement(tns + XMLNames.Vehicle_PTOIdleLossMap);
			if (_singleFile) {
				ptoLossMap.Add(EmbedDataTable(pto.PTOLossMap, AttributeMappings.PTOLossMap));
			} else {
				ptoLossMap.Add(ExtCSVResource(pto.PTOLossMap, "PTO_LossMap.vptol"));
			}
			var ptoCycle = new XElement(tns + XMLNames.Vehicle_PTOCycle);
			if (_singleFile) {
				ptoCycle.Add(EmbedDataTable(pto.PTOCycle, AttributeMappings.PTOCycleMap));
			} else {
				ptoCycle.Add(ExtCSVResource(pto.PTOCycle, "PTO_cycle.vptoc"));
			}

			return new object[] { ptoLossMap, ptoCycle };
		}

		private XElement GetCrossWindCorrectionData(IVehicleEngineeringInputData vehicle)
		{
			if (vehicle.CrossWindCorrectionMode == CrossWindCorrectionMode.NoCorrection ||
				vehicle.CrossWindCorrectionMode == CrossWindCorrectionMode.DeclarationModeCorrection) {
				return null;
			}

			var correctionMap = new XElement(tns + XMLNames.Vehicle_CrosswindCorrectionData);

			if (_singleFile) {
				correctionMap.Add(EmbedDataTable(vehicle.CrosswindCorrectionMap, AttributeMappings.CrossWindCorrectionMapping));
			} else {
				var ext = vehicle.CrossWindCorrectionMode == CrossWindCorrectionMode.SpeedDependentCorrectionFactor
					? "vcdv"
					: "vcdb";
				correctionMap.Add(ExtCSVResource(vehicle.CrosswindCorrectionMap, "CrossWindCorrection." + ext));
			}

			return correctionMap;
		}

		private XElement CreateAngleDrive(IAngledriveInputData data)
		{
			var angledrive = new XElement(tns + XMLNames.Component_Angledrive,
				new XElement(tns + XMLNames.ComponentDataWrapper,
					new XAttribute(XMLNames.Component_ID_Attr, "ANGL-" + data.ModelName),
					GetDefaultComponentElements(data.TypeId, data.ModelName), new XElement(tns + XMLNames.AngleDrive_Ratio, data.Ratio),
					data.LossMap == null
						? new XElement(tns + XMLNames.AngleDrive_Efficiency, data.Efficiency)
						: new XElement(tns + XMLNames.AngleDrive_TorqueLossMap, GetTransmissionLossMap(data.LossMap))));
			//if (_singleFile) {
			return angledrive;
			//}
			//return ExtComponent(XMLNames.Component_Angledrive, angledrive,
			//	string.Format("ANGL_{0}.xml", RemoveInvalidFileCharacters(data.ModelName)));
		}

		private string RemoveInvalidFileCharacters(string filename)
		{
			string regexSearch = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
			Regex r = new Regex(string.Format("[{0}]", Regex.Escape(regexSearch)));
			return r.Replace(filename, "");
		}

		public XElement CreateAuxiliaries(IAuxiliariesEngineeringInputData data, string fileSuffix)
		{
			var auxList = new List<XElement>();
			foreach (var auxData in data.Auxiliaries) {
				var entry = new XElement(tns + XMLNames.Auxiliaries_Auxiliary,
					new XAttribute(XMLNames.Auxiliaries_Auxiliary_ID_Attr, auxData.ID));
				if (auxData.ConstantPowerDemand > 0) {
					entry.Add(new XElement(tns + XMLNames.Auxiliaries_Auxiliary_ConstantAuxLoad, auxData.ConstantPowerDemand.Value()));
					auxList.Add(entry);
					continue;
				}
				if (_singleFile) {
					entry.Add(new XElement(tns + XMLNames.Auxiliaries_Auxiliary_TransmissionRatioToEngine, auxData.TransmissionRatio),
						new XElement(tns + XMLNames.Auxiliaries_Auxiliary_EfficiencyToEngine, auxData.EfficiencyToEngine),
						new XElement(tns + XMLNames.Auxiliaries_Auxiliary_EfficiencyAuxSupply, auxData.EfficiencyToSupply),
						new XElement(tns + XMLNames.Auxiliaries_Auxiliary_AuxMap,
							EmbedDataTable(auxData.DemandMap, AttributeMappings.AuxMapMapping)));
				} else {
					entry.Add(ExtCSVResource(auxData.DemandMap, Path.GetFileName(auxData.DemandMap.Source)));
				}
			}

			var aux = new XElement(tns + XMLNames.Component_Auxiliaries,
				new XElement(tns + XMLNames.ComponentDataWrapper, new XAttribute(XMLNames.Component_ID_Attr, "AUX-none"), auxList));
			if (_singleFile) {
				return aux;
			}
			return ExtComponent(XMLNames.Component_Auxiliaries, aux, string.Format("AUX_{0}.xml", fileSuffix));
		}

		public XElement CreateAxleWheels(IVehicleEngineeringInputData data)
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
					GetDefaultComponentElements(string.Format("WHEEL-{0}_{1}", i, axle.Wheels), axle.Wheels),
					string.IsNullOrWhiteSpace(axle.Wheels)
						? null
						: new XElement(tns + XMLNames.AxleWheels_Axles_Axle_Dimension, axle.Wheels),
					new XElement(tns + XMLNames.AxleWheels_Axles_Axle_RRCISO, axle.RollResistanceCoefficient),
					new XElement(tns + XMLNames.AxleWheels_Axles_Axle_FzISO, axle.TyreTestLoad.Value()),
					new XElement(tns + XMLNames.AxleWheels_Axles_Axle_WeightShare, axle.AxleWeightShare),
					new XElement(tns + XMLNames.AxleWheels_Axles_Axle_Inertia, axle.Inertia.Value()),
					i == 1
						? new XElement(tns + XMLNames.AxleWheels_Axles_Axle_DynamicTyreRadius, data.DynamicTyreRadius.Value() * 1000)
						: null));
			}

			var axleWheels = new XElement(tns + XMLNames.Component_AxleWheels,
				new XElement(tns + XMLNames.ComponentDataWrapper,
					new XAttribute(XMLNames.Component_ID_Attr, string.Format("AXLWHL-{0}", data.AxleConfiguration.GetName())),
					new XElement(tns + XMLNames.AxleWheels_Axles, axles)));
			if (_singleFile) {
				return axleWheels;
			}
			return ExtComponent(XMLNames.Component_AxleWheels, axleWheels,
				String.Format("AXLWHL-{0}.xml", data.AxleConfiguration.GetName()));
		}

		public XElement CreateAxlegear(IAxleGearInputData data)
		{
			var typeId = string.Format((string)"AXLGEAR-{0:0.000}", data.Ratio);
			var axl = new XElement(tns + XMLNames.Component_Axlegear,
				new XElement(tns + XMLNames.ComponentDataWrapper, new XAttribute(XMLNames.Component_ID_Attr, typeId),
					GetDefaultComponentElements(typeId, "N.A."), new XElement(tns + XMLNames.Axlegear_Ratio, data.Ratio),
					data.LossMap == null
						? new XElement(tns + XMLNames.Axlegear_Efficiency, data.Efficiency)
						: new XElement(tns + XMLNames.Axlegear_TorqueLossMap, GetTransmissionLossMap(data.LossMap))));
			if (_singleFile) {
				return axl;
			}
			return ExtComponent(XMLNames.Component_Axlegear, axl, string.Format("AXL_{0:0.00}.xml", data.Ratio));
		}


		public XElement CreateRetarder(IRetarderInputData data)
		{
			var retarder = new XElement(tns + XMLNames.Component_Retarder,
				new XElement(tns + XMLNames.ComponentDataWrapper, new XAttribute(XMLNames.Component_ID_Attr, "RET-none"),
					GetDefaultComponentElements(data.TypeId, data.ModelName),
					new XElement(tns + XMLNames.Retarder_RetarderLossMap,
						_singleFile
							? EmbedDataTable(data.LossMap, AttributeMappings.RetarderLossmapMapping)
							: ExtCSVResource(data.LossMap, string.Format("RET_{0}.vrlm", RemoveInvalidFileCharacters(data.ModelName))))));
			//if (_singleFile) {
			return retarder;
			//}
			//return ExtComponent(XMLNames.Component_Retarder, retarder,
			//	string.Format("RET_{0}.xml", RemoveInvalidFileCharacters(data.ModelName)));
		}

		protected XElement CreateGearbox(IGearboxEngineeringInputData data)
		{
			var gears = new XElement(tns + XMLNames.Gearbox_Gears);
			var i = 1;
			foreach (var gearData in data.Gears) {
				var gear = new XElement(tns + XMLNames.Gearbox_Gears_Gear,
					new XAttribute(XMLNames.Gearbox_Gear_GearNumber_Attr, i++),
					new XElement(tns + XMLNames.Gearbox_Gear_Ratio, gearData.Ratio),
					gearData.MaxTorque != null
						? new XElement(tns + XMLNames.Gearbox_Gears_MaxTorque, gearData.MaxTorque.Value())
						: null);
				if (gearData.LossMap != null) {
					gear.Add(new XElement(tns + XMLNames.Gearbox_Gear_TorqueLossMap, GetTransmissionLossMap(gearData.LossMap)));
				} else {
					gear.Add(new XElement(tns + XMLNames.Gearbox_Gear_Efficiency, gearData.Efficiency));
				}
				if (gearData.ShiftPolygon != null) {
					gear.Add(new XElement(tns + XMLNames.Gearbox_Gears_Gear_ShiftPolygon, CreateShiftPolygon(gearData.ShiftPolygon)));
				}

				gears.Add(gear);
			}
			var gbx = new XElement(tns + XMLNames.Component_Gearbox,
				new XElement(tns + XMLNames.ComponentDataWrapper,
					new XAttribute(XMLNames.Component_ID_Attr, string.Format("GBX-{0}", data.ModelName)),
					GetDefaultComponentElements(string.Format("GBX-{0}", data.ModelName), data.ModelName),
					new XElement(tns + XMLNames.Gearbox_TransmissionType, GearboxtypeToXML(data.Type)),
					new XElement(tns + XMLNames.Gearbox_Inertia, data.Inertia.Value()),
					new XElement(tns + XMLNames.Gearbox_TractionInterruption, data.TractionInterruption.Value()), gears),
				data.Type.AutomaticTransmission() ? CreateTorqueConverter(data) : null);

			if (_singleFile) {
				return gbx;
			}
			return ExtComponent(XMLNames.Component_Gearbox, gbx, string.Format("GBX-{0}.xml", data.ModelName));
		}

		private XElement CreateTorqueConverter(IGearboxEngineeringInputData data)
		{
			var torqueConverterData = data.TorqueConverter;
			var tc = new XElement(tns + XMLNames.Component_TorqueConverter,
				new XElement(tns + XMLNames.ComponentDataWrapper,
					new XElement(tns + XMLNames.TorqueConverter_ReferenceRPM, torqueConverterData.ReferenceRPM.AsRPM),
					new XElement(tns + XMLNames.TorqueConverter_Characteristics,
						_singleFile
							? EmbedDataTable(torqueConverterData.TCData, AttributeMappings.TorqueConverterDataMapping)
							: ExtCSVResource(torqueConverterData.TCData, Path.GetFileName(torqueConverterData.TCData.Source))),
					new XElement(tns + XMLNames.TorqueConverter_Inertia, torqueConverterData.Inertia.Value())));
			if (_singleFile) {
				return tc;
			}
			return new XElement(tns + XMLNames.Component_TorqueConverter,
				ExtComponent(XMLNames.Component_TorqueConverter, tc, "TorqueConverter.xml"));
		}

		private object[] GetTransmissionLossMap(TableData lossmap)
		{
			if (_singleFile) {
				return EmbedDataTable(lossmap, AttributeMappings.TransmissionLossmapMapping);
			}
			return ExtCSVResource(lossmap, Path.GetFileName(lossmap.Source));
		}

		private object[] CreateShiftPolygon(TableData shiftPolygon)
		{
			if (_singleFile) {
				return EmbedDataTable(shiftPolygon, AttributeMappings.ShiftPolygonMapping);
			}
			return ExtCSVResource(shiftPolygon, Path.GetFileName(shiftPolygon.Source));
		}

		public XElement CreateEngine(IEngineEngineeringInputData data, bool allowSeparateFile = true)
		{
			var engine = new XElement(tns + XMLNames.Component_Engine,
				new XElement(tns + XMLNames.ComponentDataWrapper,
					new XAttribute(XMLNames.Component_ID_Attr, string.Format("ENG-{0}", data.ModelName)),
					GetDefaultComponentElements(string.Format("ENG-{0}", data.ModelName), data.ModelName),
					new XElement(tns + XMLNames.Engine_Displacement, data.Displacement.Value() * 1000 * 1000),
					new XElement(tns + XMLNames.Engine_IdlingSpeed, data.IdleSpeed.AsRPM),
					new XElement(tns + XMLNames.Engine_Inertia, data.Inertia.Value()),
					new XElement(tns + XMLNames.Engine_WHTCEngineering, data.WHTCEngineering),
					new XElement(tns + XMLNames.Engine_FuelConsumptionMap, GetFuelConsumptionMap(data)),
					new XElement(tns + XMLNames.Engine_FullLoadAndDragCurve, GetFullLoadDragCurve(data))));
			if (!allowSeparateFile || _singleFile) {
				return engine;
			}
			return ExtComponent(XMLNames.Component_Engine, engine, string.Format("ENG-{0}.xml", data.ModelName));
		}

		private XElement ExtComponent(string component, XElement engine, string filename)
		{
			var xsi = XNamespace.Get("http://www.w3.org/2001/XMLSchema-instance");
			var xsd = XNamespace.Get("http://www.w3.org/2001/XMLSchema");


			var xml = new XDocument();
			xml.Add(new XElement(rootNamespace + XMLNames.VectoComponentEngineering,
				new XAttribute("schemaVersion", SchemaVersion), new XAttribute(XNamespace.Xmlns + "xsi", xsi.NamespaceName),
				new XAttribute("xmlns", tns), new XAttribute(XNamespace.Xmlns + "tns", rootNamespace),
				new XAttribute(XNamespace.Xmlns + "vdecdef", _declarationNamespace),
				new XAttribute(xsi + "schemaLocation",
					string.Format("{0} {1}VectoEngineeringInput.xsd", rootNamespace, SchemaLocationBaseUrl)), engine));

			xml.Save(Path.Combine(BasePath, filename));

			var retVal = new XElement(tns + XMLNames.ExternalResource,
				new XAttribute(XMLNames.ExtResource_Type_Attr, XMLNames.ExtResource_Type_Value_XML),
				new XAttribute(XMLNames.ExtResource_Component_Attr, component),
				new XAttribute(XMLNames.ExtResource_File_Attr, filename));
			return retVal;
		}

		private object[] GetFullLoadDragCurve(IEngineEngineeringInputData data)
		{
			if (_singleFile) {
				return EmbedDataTable(data.FullLoadCurve, AttributeMappings.EngineFullLoadCurveMapping);
			}
			var filename = string.Format("ENG_{0}.vfld", data.ModelName);
			return ExtCSVResource(data.FullLoadCurve, filename);
		}

		private object[] GetFuelConsumptionMap(IEngineEngineeringInputData data)
		{
			if (_singleFile) {
				return EmbedDataTable(data.FuelConsumptionMap, AttributeMappings.FuelConsumptionMapMapping);
			}

			var filename = string.Format("ENG_{0}.vmap", data.ModelName);
			return ExtCSVResource(data.FuelConsumptionMap, filename);
		}


		private object[] ExtCSVResource(DataTable data, string filename)
		{
			VectoCSVFile.Write(Path.Combine(BasePath, filename), data);
			return new object[] {
				new XElement(tns + XMLNames.ExternalResource,
					new XAttribute(XMLNames.ExtResource_Type_Attr, XMLNames.ExtResource_Type_Value_CSV),
					new XAttribute(XMLNames.ExtResource_File_Attr, filename))
			};
		}
	}
}