using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using TUGraz.IVT.VectoXML;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.XML.Engineering.Interfaces;
using TUGraz.VectoCore.InputData.Impl;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Engineering.DataProvider
{
	internal class XMLEngineeringVehicleDataProviderV07 : AbstractVehicleEngineeringType, IXMLEngineeringVehicleData
	{
		public static readonly XNamespace NAMESPACE_URI = XMLDefinitions.ENGINEERING_DEFINITONS_NAMESPACE_V07;

		public const string XSD_TYPE = "VehicleEngineeringType";

		public static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI, XSD_TYPE);

		

		private IVehicleComponentsEngineering _components;
		protected XmlElement _componentNode;


		public XMLEngineeringVehicleDataProviderV07(
			IXMLEngineeringJobInputData jobProvider, XmlNode vehicleNode, string fullFilename) :
			base(vehicleNode, fullFilename)
		{
			Job = jobProvider;
			SourceType = jobProvider.DataSource.SourceFile == fullFilename ? DataSourceType.XMLEmbedded : DataSourceType.XMLFile;
		}

		public IXMLComponentsReader ComponentReader { protected get; set; }

		public virtual XmlElement ComponentNode
		{
			get { return _componentNode ?? (_componentNode = GetNode(XMLNames.Vehicle_Components) as XmlElement); }
		}

		#region Implementation of IComponentInputData

		public override CertificationMethod CertificationMethod
		{
			get { return CertificationMethod.NotCertified; }
		}

		public override string CertificationNumber
		{
			get { return "N.A."; }
		}

		public override DigestData DigestValue
		{
			get { return null; }
		}

		#endregion

		#region Implementation of IVehicleDeclarationInputData

		public string Identifier { get; }

		public bool ExemptedVehicle
		{
			get { return false; }
		}

		public virtual string VIN
		{
			get { return GetString(XMLNames.Vehicle_VIN); }
		}

		public virtual LegislativeClass LegislativeClass
		{
			get { return GetString(XMLNames.Vehicle_LegislativeClass).ParseEnum<LegislativeClass>(); }
		}

		public virtual VehicleCategory VehicleCategory
		{
			get {
				return GetNode(XMLNames.Vehicle_VehicleCategory, required: false)?.InnerText.ParseEnum<VehicleCategory>() ??
						VehicleCategory.Unknown;
			}
		}

		public virtual AxleConfiguration AxleConfiguration
		{
			get { return AxleConfigurationHelper.Parse(GetString(XMLNames.Vehicle_AxleConfiguration)); }
		}

		public virtual Kilogram CurbMassChassis
		{
			get { return GetDouble(XMLNames.Vehicle_CurbMassChassis).SI<Kilogram>(); }
		}

		public virtual Kilogram CurbMassExtra
		{
			get { return GetDouble(XMLNames.Vehicle_CurbMassExtra).SI<Kilogram>(); }
		}

		public virtual Kilogram GrossVehicleMassRating
		{
			get { return GetDouble(XMLNames.Vehicle_GrossVehicleMass).SI<Kilogram>(); }
		}

		public virtual string ManufacturerAddress
		{
			get { return GetString(XMLNames.Component_ManufacturerAddress); }
		}

		public virtual PerSecond EngineIdleSpeed
		{
			get { return GetDouble(XMLNames.Vehicle_IdlingSpeed).RPMtoRad(); }
		}

		public bool VocationalVehicle
		{
			get { return false; }
		}

		public bool SleeperCab
		{
			get { return false; }
		}

		public TankSystem? TankSystem
		{
			get {
				return ElementExists(XMLNames.Vehicle_NgTankSystem)
					? EnumHelper.ParseEnum<TankSystem>(GetString(XMLNames.Vehicle_NgTankSystem))
					: (TankSystem?)null;
			}
		}

		
		public bool ZeroEmissionVehicle
		{
			get { return false; }
		}

		public bool HybridElectricHDV
		{
			get { return false; }
		}

		public bool DualFuelVehicle
		{
			get { return false; }
		}

		public Watt MaxNetPower1
		{
			get { return null; }
		}

		public Watt MaxNetPower2
		{
			get { return null; }
		}

		public virtual RegistrationClass RegisteredClass { get { return RegistrationClass.unknown;} }
		public virtual int NumberOfPassengersUpperDeck { get { return 0; } }
		public virtual int NumberOfPassengersLowerDeck { get { return 0; } }
		public virtual VehicleCode VehicleCode { get { return VehicleCode.NOT_APPLICABLE; } }
		public virtual bool LowEntry { get { return false; } }
		public virtual bool Articulated { get { return false; } }
		

		public virtual Meter Width { get { return null; } }
		public virtual Meter EntranceHeight { get { return null; } }
		public ConsumerTechnology DoorDriveTechnology { get { return ConsumerTechnology.Unknown; } }

		IVehicleComponentsDeclaration IVehicleDeclarationInputData.Components
		{
			get { return null; }
		}

		IAdvancedDriverAssistantSystemDeclarationInputData IVehicleDeclarationInputData.ADAS
		{
			get { return null; }
		}


		public IAdvancedDriverAssistantSystemsEngineering ADAS
		{
			get { return null; }
		}

		public virtual Kilogram Loading
		{
			get { return GetDouble(XMLNames.Vehicle_Loading).SI<Kilogram>(); }
		}

		public virtual Meter DynamicTyreRadius
		{
			get {
				var queryString = XMLHelper.QueryLocalName(
					XMLNames.Vehicle_Components,
					XMLNames.Component_AxleWheels,
					XMLNames.ComponentDataWrapper,
					XMLNames.AxleWheels_Axles,
					XMLNames.AxleWheels_Axles_Axle
				);
				queryString += string.Format(
					"/*[local-name()='{0}' and text()='{1}']/ancestor-or-self::*[local-name()='{2}']//*[local-name()='{3}']",
					XMLNames.AxleWheels_Axles_Axle_AxleType, AxleType.VehicleDriven.ToString(),
					XMLNames.AxleWheels_Axles_Axle, XMLNames.AxleWheels_Axles_Axle_DynamicTyreRadius);
				var node = BaseNode.SelectSingleNode(queryString);
				return node?.InnerText.ToDouble().SI(Unit.SI.Milli.Meter).Cast<Meter>();
			}
		}

		public virtual IList<ITorqueLimitInputData> TorqueLimits
		{
			get {
				var retVal = new List<ITorqueLimitInputData>();

				var tqLimits = GetNode(XMLNames.Vehicle_TorqueLimits, required: false);
				if (tqLimits == null) {
					return retVal;
				}

				var entries = GetNodes(XMLNames.Vehicle_TorqueLimits_Entry, tqLimits);
				foreach (XmlNode entry in entries) {
					retVal.Add(
						new TorqueLimitInputData() {
							Gear = GetAttribute(entry, XMLNames.Vehicle_TorqueLimits_Entry_Gear_Attr).ToInt(),
							MaxTorque = GetAttribute(entry, XMLNames.Vehicle_TorqueLimits_Entry_MaxTorque_Attr).ToDouble().SI<NewtonMeter>()
						});
				}

				return retVal;
			}
		}


		public virtual Meter Height
		{
			get { return GetNode("VehicleHeight")?.InnerText.ToDouble().SI<Meter>(); }
		}

		public virtual Meter Length { get { return null; } }

		public IVehicleComponentsEngineering Components
		{
			get { return _components ?? (_components = ComponentReader.ComponentInputData); }
		}

		#endregion


		public virtual RetarderType RetarderType
		{
			get { return GetString(XMLNames.Vehicle_RetarderType).ParseEnum<RetarderType>(); }
		}

		public virtual double RetarderRatio
		{
			get { return GetDouble(XMLNames.Vehicle_RetarderRatio); }
		}


		public virtual AngledriveType AngledriveType
		{
			get { return GetString(XMLNames.Vehicle_AngledriveType).ParseEnum<AngledriveType>(); }
		}

		public virtual IXMLEngineeringJobInputData Job { get; }

		#region Implementation of IPTOTransmissionInputData

		public virtual string PTOTransmissionType
		{
			get { return GetString(XMLNames.Vehicle_PTOType); }
		}

		public virtual TableData PTOLossMap
		{
			get {
				return XMLHelper.ReadEntriesOrResource(
					BaseNode, DataSource.SourcePath, XMLNames.Vehicle_PTOIdleLossMap, XMLNames.Vehicle_PTOIdleLossMap_Entry,
					AttributeMappings.PTOLossMap);
			}
		}

		public virtual TableData PTOCycle
		{
			get {
				return XMLHelper.ReadEntriesOrResource(
					BaseNode, DataSource.SourcePath, XMLNames.Vehicle_PTOCycle, XMLNames.Vehicle_PTOCycle_Entry,
					AttributeMappings.PTOCycleMap);
			}
		}

		#endregion

		#region Overrides of AbstractXMLResource

		protected override XNamespace SchemaNamespace
		{
			get { return NAMESPACE_URI; }
		}

		protected override DataSourceType SourceType { get; }

		#endregion
	}


	internal class XMLEngineeringVehicleDataProviderV10 : XMLEngineeringVehicleDataProviderV07
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.ENGINEERING_DEFINITONS_NAMESPACE_V10;

		//public new const string XSD_TYPE = "VehicleEngineeringType";

		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI, XSD_TYPE);

		public XMLEngineeringVehicleDataProviderV10(
			IXMLEngineeringJobInputData jobProvider, XmlNode vehicleNode, string fullFilename) : base(
			jobProvider, vehicleNode, fullFilename) { }

		#region Overrides of XMLEngineeringVehicleDataProviderV07

		protected override XNamespace SchemaNamespace
		{
			get { return NAMESPACE_URI; }
		}

		#endregion
	}
}
