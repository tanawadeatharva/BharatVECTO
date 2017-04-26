using System;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using TUGraz.IVT.VectoXML;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.InputData.Impl;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Resources;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Engineering
{
	public class XMLEngineeringDriverDataProvider : AbstractEngineeringXMLComponentDataProvider,
		IDriverEngineeringInputData
	{
		public XMLEngineeringDriverDataProvider(XMLEngineeringInputDataProvider xmlEngineeringJobInputDataProvider,
			XPathDocument driverDocument, string xmlBasePath, string fsBasePath)
			: base(xmlEngineeringJobInputDataProvider, driverDocument, xmlBasePath, fsBasePath) {}

		public IStartStopDeclarationInputData StartStop
		{
			get { return InputData._vehicleInputData.StartStop; }
		}

		IOverSpeedEcoRollEngineeringInputData IDriverEngineeringInputData.OverSpeedEcoRoll
		{
			get
			{
				var minSpeedPath = Helper.Query(XMLNames.Component_DriverModel,
					XMLNames.DriverModel_Overspeed, XMLNames.DriverModel_Overspeed_MinSpeed);
				var overSpeedPath = Helper.Query(XMLNames.Component_DriverModel,
					XMLNames.DriverModel_Overspeed, XMLNames.DriverModel_Overspeed_AllowedOverspeed);
				var underSpeedPath = Helper.Query(XMLNames.Component_DriverModel,
					XMLNames.DriverModel_Overspeed, XMLNames.DriverModel_Overspeed_AllowedUnderspeed);
				var retVal = new OverSpeedEcoRollInputData {
					Mode = GetElementValue(Helper.Query(XMLNames.Component_DriverModel,
						XMLNames.DriverModel_Overspeed, XMLNames.DriverModel_Overspeed_Mode)).ParseEnum<DriverMode>(),
					MinSpeed = ElementExists(minSpeedPath)
						? GetDoubleElementValue(minSpeedPath).KMPHtoMeterPerSecond()
						: DeclarationData.Driver.OverSpeedEcoRoll.MinSpeed,
					OverSpeed = ElementExists(overSpeedPath)
						? GetDoubleElementValue(overSpeedPath).KMPHtoMeterPerSecond()
						: DeclarationData.Driver.OverSpeedEcoRoll.OverSpeed,
					UnderSpeed = ElementExists(underSpeedPath)
						? GetDoubleElementValue(underSpeedPath).KMPHtoMeterPerSecond()
						: DeclarationData.Driver.OverSpeedEcoRoll.UnderSpeed
				};

				return retVal;
			}
		}

		public TableData AccelerationCurve
		{
			get
			{
				if (ElementExists(Helper.Query(XMLNames.Component_DriverModel, XMLNames.DriverModel_DriverAccelerationCurve))) {
					return
						ReadTableData(AttributeMappings.DriverAccelerationCurveMapping,
							Helper.Query(XMLNames.Component_DriverModel, XMLNames.DriverModel_DriverAccelerationCurve,
								XMLNames.DriverModel_DriverAccelerationCurve_Entry));
				}
				//Log.Warn("Could not find file for acceleration curve. Trying lookup in declaration data.");
				try {
					var resourceName = DeclarationData.DeclarationDataResourcePrefix + ".VACC.Truck" +
										Constants.FileExtensions.DriverAccelerationCurve;
					return VectoCSVFile.ReadStream(RessourceHelper.ReadStream(resourceName), source: resourceName);
				} catch (Exception e) {
					throw new VectoException("Failed to read Driver Acceleration Curve: " + e.Message, e);
				}
			}
		}

		public ILookaheadCoastingInputData Lookahead
		{
			get
			{
				var lookAheadXmlPath = Helper.Query(XMLNames.Component_DriverModel, XMLNames.DriverModel_LookAheadCoasting);

				var retVal = new LookAheadCoastingInputData {
					Enabled =
						XmlConvert.ToBoolean(
							GetElementValue(Helper.Query(lookAheadXmlPath, XMLNames.DriverModel_LookAheadCoasting_Enabled))),
					LookaheadDistanceFactor =
						ElementExists(Helper.Query(lookAheadXmlPath, XMLNames.DriverModel_LookAheadCoasting_PreviewDistanceFactor))
							? GetDoubleElementValue(Helper.Query(lookAheadXmlPath,
								XMLNames.DriverModel_LookAheadCoasting_PreviewDistanceFactor))
							: DeclarationData.Driver.LookAhead.LookAheadDistanceFactor,
					CoastingDecisionFactorOffset =
						ElementExists(Helper.Query(lookAheadXmlPath, XMLNames.DriverModel_LookAheadCoasting_DecisionFactorOffset))
							? GetDoubleElementValue(Helper.Query(lookAheadXmlPath,
								XMLNames.DriverModel_LookAheadCoasting_DecisionFactorOffset))
							: DeclarationData.Driver.LookAhead.DecisionFactorCoastingOffset,
					CoastingDecisionFactorScaling =
						ElementExists(Helper.Query(lookAheadXmlPath, XMLNames.DriverModel_LookAheadCoasting_DecisionFactorScaling))
							? GetDoubleElementValue(Helper.Query(lookAheadXmlPath,
								XMLNames.DriverModel_LookAheadCoasting_DecisionFactorScaling))
							: DeclarationData.Driver.LookAhead.DecisionFactorCoastingScaling,
					MinSpeed = ElementExists(Helper.Query(lookAheadXmlPath, XMLNames.DriverModel_LookAheadCoasting_MinSpeed))
						? GetDoubleElementValue(Helper.Query(lookAheadXmlPath, XMLNames.DriverModel_LookAheadCoasting_MinSpeed))
							.KMPHtoMeterPerSecond()
						: DeclarationData.Driver.LookAhead.MinimumSpeed
				};

				if (
					ElementExists(Helper.Query(lookAheadXmlPath, XMLNames.DriverModel_LookAheadCoasting_SpeedDependentDecisionFactor,
						XMLNames.LookAheadCoasting_SpeedDependentDecisionFactor_Entry))) {
					retVal.CoastingDecisionFactorTargetSpeedLookup = ReadTableData(
						AttributeMappings.CoastingDFTargetSpeedLookupMapping,
						Helper.Query(lookAheadXmlPath, XMLNames.DriverModel_LookAheadCoasting_SpeedDependentDecisionFactor,
							XMLNames.LookAheadCoasting_SpeedDependentDecisionFactor_Entry));
				} else if (
					ElementExists(Helper.Query(lookAheadXmlPath, XMLNames.DriverModel_LookAheadCoasting_SpeedDependentDecisionFactor,
						XMLNames.ExternalResource))) {
					var node = Navigator.SelectSingleNode(Helper.Query(XBasePath, lookAheadXmlPath,
						XMLNames.DriverModel_LookAheadCoasting_SpeedDependentDecisionFactor, XMLNames.ExternalResource), Manager);
					if (node != null &&
						XMLNames.ExtResource_Type_Value_CSV.Equals(node.GetAttribute(XMLNames.ExtResource_Type_Attr, ""))) {
						retVal.CoastingDecisionFactorTargetSpeedLookup =
							VectoCSVFile.Read(Path.Combine(FSBasePath, node.GetAttribute(XMLNames.ExtResource_File_Attr, "")));
					}
				} else {
					retVal.CoastingDecisionFactorTargetSpeedLookup = null;
				}

				if (
					ElementExists(Helper.Query(lookAheadXmlPath, XMLNames.DriverModel_LookAheadCoasting_VelocityDropDecisionFactor,
						XMLNames.LookAheadCoasting_VelocityDropDecisionFactor_Entry))) {
					retVal.CoastingDecisionFactorVelocityDropLookup =
						ReadTableData(AttributeMappings.CoastingDFVelocityDropLookupMapping,
							Helper.Query(lookAheadXmlPath, XMLNames.DriverModel_LookAheadCoasting_VelocityDropDecisionFactor,
								XMLNames.LookAheadCoasting_VelocityDropDecisionFactor_Entry));
				} else if (
					ElementExists(Helper.Query(lookAheadXmlPath, XMLNames.DriverModel_LookAheadCoasting_VelocityDropDecisionFactor,
						XMLNames.ExternalResource))) {
					var node =
						Navigator.SelectSingleNode(Helper.Query(XBasePath, lookAheadXmlPath,
							XMLNames.DriverModel_LookAheadCoasting_VelocityDropDecisionFactor, XMLNames.ExternalResource), Manager);
					if (node != null &&
						XMLNames.ExtResource_Type_Value_CSV.Equals(node.GetAttribute(XMLNames.ExtResource_Type_Attr, ""))) {
						retVal.CoastingDecisionFactorVelocityDropLookup =
							VectoCSVFile.Read(Path.Combine(FSBasePath, node.GetAttribute(XMLNames.ExtResource_File_Attr, "")));
					}
				} else {
					retVal.CoastingDecisionFactorVelocityDropLookup = null;
				}
				return retVal;
			}
		}

		IStartStopEngineeringInputData IDriverEngineeringInputData.StartStop
		{
			get { return InputData._vehicleInputData.StartStopEngineering; }
		}

		public IOverSpeedEcoRollDeclarationInputData OverSpeedEcoRoll
		{
			get
			{
				var node =
					Navigator.SelectSingleNode(Helper.Query(XBasePath, XMLNames.Component_DriverModel, XMLNames.DriverModel_Overspeed,
						XMLNames.DriverModel_Overspeed_Mode), Manager);
				return new OverSpeedEcoRollInputData() {
					Mode = node != null ? DriverData.ParseDriverMode(node.Value) : DriverMode.Off
				};
			}
		}
	}
}