/*
* This file is part of VECTO.
*
* Copyright © 2012-2017 European Union
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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Schema;
using System.Xml.XPath;
using TUGraz.IVT.VectoXML;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.InputData.Impl;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Engineering
{
	public class XMLEngineeringVehicleDataProvider : AbstractEngineeringXMLComponentDataProvider,
		IVehicleEngineeringInputData,
		IPTOTransmissionInputData
	{
		public XMLEngineeringVehicleDataProvider(XMLEngineeringInputDataProvider xmlEngineeringJobInputDataProvider,
			XPathDocument vehicleDocument, string xmlBasePath, string fsBasePath)
			: base(xmlEngineeringJobInputDataProvider, vehicleDocument, xmlBasePath, fsBasePath) {}

		public string GetVehicleID
		{
			get { return GetAttributeValue("", XMLNames.Component_ID_Attr); }
		}

		public string VIN
		{
			get { return GetElementValue(XMLNames.Vehicle_VIN); }
		}

		public LegislativeClass LegislativeClass
		{
			get { return GetElementValue(XMLNames.Vehicle_LegislativeClass).ParseEnum<LegislativeClass>(); }
		}

		public VehicleCategory VehicleCategory
		{
			get { return GetElementValue(XMLNames.Vehicle_VehicleCategory).ParseEnum<VehicleCategory>(); }
		}

		public Kilogram CurbMassChassis
		{
			get { return GetDoubleElementValue(XMLNames.Vehicle_CurbMassChassis).SI<Kilogram>(); }
		}

		public Kilogram CurbMassExtra
		{
			get { return GetDoubleElementValue(XMLNames.Vehicle_CurbMassExtra).SI<Kilogram>(); }
		}

		public Kilogram GrossVehicleMassRating
		{
			get { return GetDoubleElementValue(XMLNames.Vehicle_GrossVehicleMass).SI<Kilogram>(); }
		}

		public IList<ITorqueLimitInputData> TorqueLimits
		{
			get {
				var retVal = new List<ITorqueLimitInputData>();
				var limits =
					Navigator.Select(Helper.Query(VehiclePath, XMLNames.Vehicle_TorqueLimits, XMLNames.Vehicle_TorqueLimits_Entry),
						Manager);
				while (limits.MoveNext()) {
					retVal.Add(new TorqueLimitInputData() {
						Gear = limits.Current.GetAttribute(XMLNames.Vehicle_TorqueLimits_Entry_Gear_Attr, "").ToInt(),
						MaxTorque =
							limits.Current.GetAttribute(XMLNames.Vehicle_TorqueLimits_Entry_MaxTorque_Attr, "").ToDouble().SI<NewtonMeter>()
					});
				}
				return retVal;
			}
		}

		public Kilogram Loading
		{
			get { return GetDoubleElementValue(XMLNames.Vehicle_Loading).SI<Kilogram>(); }
		}

		public Meter DynamicTyreRadius
		{
			get {
				var queryPath = Helper.Query(XMLNames.Vehicle_Components,
					XMLNames.Component_AxleWheels,
					XMLNames.ComponentDataWrapper,
					XMLNames.AxleWheels_Axles,
					Helper.QueryConstraint(Helper.NSPrefix(XMLNames.AxleWheels_Axles_Axle),
						Helper.NSPrefix(XMLNames.AxleWheels_Axles_Axle_AxleType),
						AxleType.VehicleDriven.ToString(), ""), // query
					XMLNames.AxleWheels_Axles_Axle_DynamicTyreRadius
					);
				return (GetDoubleElementValue(queryPath) / 1000).SI<Meter>();
			}
		}

		public Meter Height
		{
			get {
				if (ElementExists(Helper.Query("VehicleHeight"))) {
					return GetDoubleElementValue("VehicleHeight").SI<Meter>();
				}
				return null;
			}
		}


		public AxleConfiguration AxleConfiguration
		{
			get { return AxleConfigurationHelper.Parse(GetElementValue(XMLNames.Vehicle_AxleConfiguration)); }
		}

		public IList<IAxleEngineeringInputData> Axles
		{
			get { return AxleEngineeringInput().Cast<IAxleEngineeringInputData>().ToList(); }
		}

		public string ManufacturerAddress
		{
			get { return "N.A."; }
		}

		public PerSecond EngineIdleSpeed
		{
			get { return null; }
		}

		IList<IAxleDeclarationInputData> IVehicleDeclarationInputData.Axles
		{
			get { return AxleEngineeringInput().Cast<IAxleDeclarationInputData>().ToList(); }
		}

		private IEnumerable<AxleInputData> AxleEngineeringInput()
		{
			var axlePath = Helper.Query(
				XMLNames.Vehicle_Components,
				XMLNames.Component_AxleWheels,
				XMLNames.ComponentDataWrapper,
				XMLNames.AxleWheels_Axles,
				XMLNames.AxleWheels_Axles_Axle);
			var axles =
				Navigator.Select(Helper.Query(XBasePath, axlePath), Manager);

			var retVal = new AxleInputData[axles.Count];

			while (axles.MoveNext()) {
				var axleNumber = axles.Current.GetAttribute(XMLNames.AxleWheels_Axles_Axle_AxleNumber_Attr, "").ToInt();
				if (axleNumber < 1 || axleNumber > retVal.Length) {
					throw new VectoException("Axle #{0} exceeds axle count", axleNumber);
				}
				if (retVal[axleNumber - 1] != null) {
					throw new VectoException("Axle #{0} defined multiple times!", axleNumber);
				}
				var dimension = axles.Current.SelectSingleNode(Helper.Query(XMLNames.AxleWheels_Axles_Axle_Dimension), Manager);
				var rollResistance = axles.Current.SelectSingleNode(Helper.Query(XMLNames.AxleWheels_Axles_Axle_RRCISO), Manager);
				var tyreTestLoad = axles.Current.SelectSingleNode(Helper.Query(XMLNames.AxleWheels_Axles_Axle_FzISO), Manager);
				var weightShare = axles.Current.SelectSingleNode(Helper.Query(XMLNames.AxleWheels_Axles_Axle_WeightShare), Manager);
				var inertia = axles.Current.SelectSingleNode(Helper.Query(XMLNames.AxleWheels_Axles_Axle_Inertia), Manager);
				var axleType = axles.Current.SelectSingleNode(Helper.NSPrefix(XMLNames.AxleWheels_Axles_Axle_AxleType), Manager);
				var twinTyres = axles.Current.SelectSingleNode(Helper.NSPrefix(XMLNames.AxleWheels_Axles_Axle_TwinTyres), Manager);
				var steered = axles.Current.SelectSingleNode(Helper.NSPrefix(XMLNames.AxleWheels_Axles_Axle_Steered), Manager);

				retVal[axleNumber - 1] = new AxleInputData {
					AxleType = axleType == null ? AxleType.VehicleNonDriven : axleType.Value.ParseEnum<AxleType>(),
					TwinTyres = twinTyres != null && XmlConvert.ToBoolean(twinTyres.Value),
					Steered = steered != null && XmlConvert.ToBoolean(steered.Value),
					TyreTestLoad = tyreTestLoad == null ? null : tyreTestLoad.ValueAsDouble.SI<Newton>(),
					RollResistanceCoefficient = rollResistance == null ? double.NaN : rollResistance.ValueAsDouble,
					Wheels = dimension == null ? null : dimension.Value,
					AxleWeightShare = weightShare == null ? 0 : weightShare.ValueAsDouble,
					Inertia = inertia == null ? null : inertia.ValueAsDouble.SI<KilogramSquareMeter>()
				};
			}
			return retVal;
		}

		public double RetarderRatio
		{
			get { return GetDoubleElementValue(XMLNames.Vehicle_RetarderRatio); }
		}

		public RetarderType RetarderType
		{
			get { return GetElementValue(XMLNames.Vehicle_RetarderType).ParseEnum<RetarderType>(); }
		}

		public AngledriveType AngledriveType
		{
			get { return GetElementValue(XMLNames.Vehicle_AngledriveType).ParseEnum<AngledriveType>(); }
		}

		public IAirdragEngineeringInputData GetAirdragInputData(XmlReaderSettings settings)
		{
			return CreateComponentInput(XMLNames.Component_AirDrag, settings,
				(a, b, c, d) => new XMLEngineeringAirdragDataProvider(a, b, c, d));
		}

		public XMLEngineeringAxlegearDataProvider GetAxleGearInputData(XmlReaderSettings settings)
		{
			return CreateComponentInput(XMLNames.Component_Axlegear, settings,
				(a, b, c, d) => new XMLEngineeringAxlegearDataProvider(a, b, c, d));
		}

		public XMLEngineeringEngineDataProvider GetEngineInputData(XmlReaderSettings settings)
		{
			return CreateComponentInput(XMLNames.Component_Engine, settings,
				(a, b, c, d) => new XMLEngineeringEngineDataProvider(a, b, c, d));
		}

		public XMLEngineeringRetarderDataProvider GetRetarderInputData(XmlReaderSettings settings)
		{
			if (!RetarderType.IsDedicatedComponent()) {
				return new XMLEngineeringRetarderDataProvider(InputData, XMLDocument,
					Helper.Query(XBasePath, XMLNames.Vehicle_Components, XMLNames.Component_Retarder, XMLNames.ComponentDataWrapper),
					FSBasePath);
			}

			return CreateComponentInput(XMLNames.Component_Retarder, settings,
				(a, b, c, d) => new XMLEngineeringRetarderDataProvider(a, b, c, d));
		}

		public XMLEngineeringGearboxDataProvider GetGearboxData(XmlReaderSettings settings)
		{
			return CreateComponentInput(XMLNames.Component_Gearbox, settings,
				(a, b, c, d) => new XMLEngineeringGearboxDataProvider(a, b, c, d));
		}

		public XMLEngineeringAuxiliaryDataProvider GetAuxiliaryData(XmlReaderSettings settings)
		{
			return CreateComponentInput(XMLNames.Component_Auxiliaries, settings,
				(a, b, c, d) => new XMLEngineeringAuxiliaryDataProvider(a, b, c, d));
		}


		protected T CreateComponentInput<T>(string componentName, XmlReaderSettings settings,
			Func<XMLEngineeringInputDataProvider, XPathDocument, string, string, T> creator)
		{
			if (ElementExists(Helper.Query(XMLNames.Vehicle_Components, componentName))) {
				return creator(InputData, XMLDocument,
					Helper.Query(XBasePath, XMLNames.Vehicle_Components, componentName, XMLNames.ComponentDataWrapper), FSBasePath);
			}
			string componentPath = Helper.Query(XMLNames.Vehicle_Components,
				Helper.QueryConstraint(XMLNames.ExternalResource, string.Format("@component='{0}' and @type='xml'", componentName),
					null, ""));
			if (!ElementExists(componentPath)) {
				throw new VectoException("Component {0} not found!", componentName);
			}
			var componentNode =
				Navigator.SelectSingleNode(Helper.Query(XBasePath, componentPath), Manager);
			if (componentNode != null) {
				try {
					var componentFile = componentNode.GetAttribute(XMLNames.ExtResource_File_Attr, "");
					var componentDocument = new XPathDocument(XmlReader.Create(Path.Combine(FSBasePath, componentFile), settings));
					return creator(InputData, componentDocument,
						Helper.QueryAbs(Helper.NSPrefix(XMLNames.VectoComponentEngineering, Constants.XML.RootNSPrefix), componentName,
							XMLNames.ComponentDataWrapper),
						Path.GetDirectoryName(Path.Combine(Path.GetFullPath(FSBasePath), componentFile)));
				} catch (XmlSchemaValidationException validationException) {
					throw new VectoException("Validation of XML-file for component {0} failed", validationException, componentName);
				}
			}
			throw new VectoException("Component {0} not found!", componentName);
		}

		public XMLEngineeringAngledriveDataProvider GetAngularGearInputData()
		{
			return new XMLEngineeringAngledriveDataProvider(InputData, XMLDocument,
				Helper.Query(XBasePath, XMLNames.Vehicle_Components, XMLNames.Component_Angledrive, XMLNames.ComponentDataWrapper)
				, FSBasePath);
		}

		#region "PTO"

		public IPTOTransmissionInputData GetPTOData()
		{
			return this;
		}

		public string PTOTransmissionType
		{
			get { return GetElementValue(XMLNames.Vehicle_PTOType); }
		}

		public TableData PTOLossMap
		{
			get {
				if (ElementExists(Helper.Query(XMLNames.Vehicle_PTOIdleLossMap, XMLNames.Vehicle_PTOIdleLossMap_Entry))) {
					return ReadTableData(AttributeMappings.PTOLossMap,
						Helper.Query(XMLNames.Vehicle_PTOIdleLossMap, XMLNames.Vehicle_PTOIdleLossMap_Entry));
				}
				if (ElementExists(Helper.Query(XMLNames.Vehicle_PTOIdleLossMap, ExtCsvResourceTag))) {
					return ReadCSVResourceFile(XMLNames.Vehicle_PTOIdleLossMap);
				}
				return null;
			}
		}

		public TableData PTOCycle
		{
			get {
				if (ElementExists(Helper.Query(XMLNames.Vehicle_PTOCycle, XMLNames.Vehicle_PTOCycle_Entry))) {
					return ReadTableData(AttributeMappings.PTOCycleMap,
						Helper.Query(XMLNames.Vehicle_PTOCycle, XMLNames.Vehicle_PTOCycle_Entry));
				}
				if (ElementExists(Helper.Query(XMLNames.Vehicle_PTOCycle, ExtCsvResourceTag))) {
					return ReadCSVResourceFile(XMLNames.Vehicle_PTOCycle);
				}
				return null;
			}
		}

		#endregion
	}
}