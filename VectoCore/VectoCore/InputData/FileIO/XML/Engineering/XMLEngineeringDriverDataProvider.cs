/*
* This file is part of VECTO.
*
* Copyright © 2012-2016 European Union
*
* Developed by Graz University of Technology,
*              Institute of Internal Combustion Engines and Thermodynamics,
*              Institute of Technical Informatics
*
* VECTO is licensed under the EUPL, Version 1.1 or - as soon they will be approved
* by the European Commission - subsequent versions of the EUPL (the "Licence");
* You may not use VECTO except in compliance with the Licence.
* You may obtain a copy of the Licence at:
*
* https://joinup.ec.europa.eu/community/eupl/og_page/eupl
*
* Unless required by applicable law or agreed to in writing, VECTO
* distributed under the Licence is distributed on an "AS IS" basis,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the Licence for the specific language governing permissions and
* limitations under the Licence.
*
* Authors:
*   Stefan Hausberger, hausberger@ivt.tugraz.at, IVT, Graz University of Technology
*   Christian Kreiner, christian.kreiner@tugraz.at, ITI, Graz University of Technology
*   Michael Krisper, michael.krisper@tugraz.at, ITI, Graz University of Technology
*   Raphael Luz, luz@ivt.tugraz.at, IVT, Graz University of Technology
*   Markus Quaritsch, markus.quaritsch@tugraz.at, IVT, Graz University of Technology
*   Martin Rexeis, rexeis@ivt.tugraz.at, IVT, Graz University of Technology
*/

using System;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using TUGraz.IVT.VectoXML;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.InputData.Impl;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Engineering
{
	public class XMLEngineeringDriverDataProvider : AbstractEngineeringXMLComponentDataProvider,
		IDriverEngineeringInputData
	{
		public XMLEngineeringDriverDataProvider(XMLEngineeringInputDataProvider xmlEngineeringJobInputDataProvider,
			XPathDocument driverDocument, string xmlBasePath, string fsBasePath)
			: base(xmlEngineeringJobInputDataProvider, driverDocument, xmlBasePath, fsBasePath) {}

		IOverSpeedEcoRollEngineeringInputData IDriverEngineeringInputData.OverSpeedEcoRoll
		{
			get {
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
			get {
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
			get {
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

		public IOverSpeedEcoRollDeclarationInputData OverSpeedEcoRoll
		{
			get {
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