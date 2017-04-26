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
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.InputData.Impl;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Resources;

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

		public VehicleCategory VehicleCategory
		{
			get { return GetElementValue(XMLNames.Vehicle_VehicleCategory).ParseEnum<VehicleCategory>(); }
		}

		public Kilogram CurbWeightChassis
		{
			get { return GetDoubleElementValue(XMLNames.Vehicle_CurbWeightChassis).SI<Kilogram>(); }
		}

		public Kilogram CurbWeightExtra
		{
			get { return GetDoubleElementValue(XMLNames.Vehicle_CurbWeightExtra).SI<Kilogram>(); }
		}

		public Kilogram GrossVehicleMassRating
		{
			get { return GetDoubleElementValue(XMLNames.Vehicle_GrossVehicleMass).SI<Kilogram>(); }
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
					Helper.QueryConstraint(XMLNames.AxleWheels_Axles_Axle, XMLNames.AxleWheels_Axles_Axle_AxleType_Attr,
						AxleType.VehicleDriven.ToString()), // query
					XMLNames.AxleWheels_Axles_Axle_DynamicTyreRadius
					);
				return (GetDoubleElementValue(queryPath) / 1000).SI<Meter>();
			}
		}


		public SquareMeter AirDragArea
		{
			get { return GetDoubleElementValue(XMLNames.Vehicle_AirDragArea).SI<SquareMeter>(); }
		}


		public AxleConfiguration AxleConfiguration
		{
			get { return AxleConfigurationHelper.Parse(GetElementValue(XMLNames.Vehicle_AxleConfiguration)); }
		}

		public IList<IAxleEngineeringInputData> Axles
		{
			get { return AxleEngineeringInput().Cast<IAxleEngineeringInputData>().ToList(); }
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
				var dimension = axles.Current.SelectSingleNode(Helper.Query(XMLNames.AxleWheels_Axles_Axle_Dimension), Manager);
				var rollResistance = axles.Current.SelectSingleNode(Helper.Query(XMLNames.AxleWheels_Axles_Axle_RRCISO), Manager);
				var tyreTestLoad = axles.Current.SelectSingleNode(Helper.Query(XMLNames.AxleWheels_Axles_Axle_FzISO), Manager);
				var weightShare = axles.Current.SelectSingleNode(Helper.Query(XMLNames.AxleWheels_Axles_Axle_WeightShare), Manager);
				var inertia = axles.Current.SelectSingleNode(Helper.Query(XMLNames.AxleWheels_Axles_Axle_Inertia), Manager);
				var axleNumber = axles.Current.GetAttribute(XMLNames.AxleWheels_Axles_Axle_AxleNumber_Attr, "").ToInt();
				if (axleNumber < 1 || axleNumber > retVal.Length) {
					throw new VectoException("Axle #{0} exceeds axle count", axleNumber);
				}
				if (retVal[axleNumber - 1] != null) {
					throw new VectoException("Axle #{0} defined multiple times!", axleNumber);
				}
				retVal[axleNumber - 1] = new AxleInputData {
					AxleType = axles.Current.GetAttribute(XMLNames.AxleWheels_Axles_Axle_AxleType_Attr, "").ParseEnum<AxleType>(),
					TwinTyres = XmlConvert.ToBoolean(axles.Current.GetAttribute(XMLNames.AxleWheels_Axles_Axle_TwinTyres_Attr, "")),
					TyreTestLoad = tyreTestLoad == null ? null : tyreTestLoad.ValueAsDouble.SI<Newton>(),
					RollResistanceCoefficient = rollResistance == null ? double.NaN : rollResistance.ValueAsDouble,
					Wheels = dimension == null ? null : dimension.Value,
					AxleWeightShare = weightShare == null ? 0 : weightShare.ValueAsDouble,
					Inertia = inertia == null ? null : inertia.ValueAsDouble.SI<KilogramSquareMeter>()
				};
			}
			return retVal;
		}


		public CrossWindCorrectionMode CrossWindCorrectionMode
		{
			get { return GetElementValue(XMLNames.Vehicle_CrossWindCorrectionMode).ParseEnum<CrossWindCorrectionMode>(); }
		}

		public TableData CrosswindCorrectionMap
		{
			get {
				return ReadTableData(AttributeMappings.CrossWindCorrectionMapping,
					Helper.Query(XMLNames.Vehicle_CrosswindCorrectionData, XMLNames.Vehicle_CrosswindCorrectionData_Entry));
			}
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


		public XMLEngineeringAngledriveDataProvider GetAngularGearInputData(XmlReaderSettings settings)
		{
			return new XMLEngineeringAngledriveDataProvider(InputData, XMLDocument,
				Helper.Query(XBasePath, XMLNames.Vehicle_Components, XMLNames.Component_Angledrive, XMLNames.ComponentDataWrapper)
				, FSBasePath);
		}

		#region "PTO"

		public IPTOTransmissionInputData GetPTOData(XmlReaderSettings settings)
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

		public IStartStopDeclarationInputData StartStop
		{
			get {
				var node =
					Navigator.SelectSingleNode(
						Helper.Query(XBasePath, XMLNames.Vehicle_AdvancedDriverAssist,
							XMLNames.Vehicle_AdvancedDriverAssist_EngineStartStop,
							XMLNames.Vehicle_AdvancedDriverAssist_EngineStartStop_Enabled), Manager);
				return new StartStopInputData() {
					Enabled = node != null && XmlConvert.ToBoolean(node.Value)
				};
			}
		}

		public IStartStopEngineeringInputData StartStopEngineering
		{
			get {
				var delayPath = Helper.Query(XMLNames.Vehicle_AdvancedDriverAssist,
					XMLNames.Vehicle_AdvancedDriverAssist_EngineStartStop,
					XMLNames.Vehicle_AdvancedDriverAssist_EngineStartStop_ActivationDelay);
				var minTimePath = Helper.Query(XMLNames.Vehicle_AdvancedDriverAssist,
					XMLNames.Vehicle_AdvancedDriverAssist_EngineStartStop,
					XMLNames.Vehicle_AdvancedDriverAssist_EngineStartStop_MinOnTime);
				var maxSpeedPath = Helper.Query(XMLNames.Vehicle_AdvancedDriverAssist,
					XMLNames.Vehicle_AdvancedDriverAssist_EngineStartStop,
					XMLNames.Vehicle_AdvancedDriverAssist_EngineStartStop_MaxSpeed);
				var retVal = new StartStopInputData {
					Enabled = XmlConvert.ToBoolean(GetElementValue(
						Helper.Query(XMLNames.Vehicle_AdvancedDriverAssist,
							XMLNames.Vehicle_AdvancedDriverAssist_EngineStartStop,
							XMLNames.Vehicle_AdvancedDriverAssist_EngineStartStop_Enabled))),
					Delay = ElementExists(delayPath)
						? GetDoubleElementValue(delayPath).SI<Second>()
						: DeclarationData.Driver.StartStop.Delay,
					MinTime = ElementExists(minTimePath)
						? GetDoubleElementValue(minTimePath).SI<Second>()
						: DeclarationData.Driver.StartStop.MinTime,
					MaxSpeed = ElementExists(maxSpeedPath)
						? GetDoubleElementValue(maxSpeedPath).KMPHtoMeterPerSecond()
						: DeclarationData.Driver.StartStop.MaxSpeed
				};

				return retVal;
			}
		}

		#endregion
	}
}