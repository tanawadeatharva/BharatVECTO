/*
* This file is part of VECTO.
*
* Copyright © 2012-2019 European Union
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
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Reader;
using TUGraz.VectoCore.InputData.Impl;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider
{
	public class XMLDeclarationVehicleDataProviderV10 : AbstractCommonComponentType, IXMLDeclarationVehicleData
	{
		public static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V10;

		public const string XSD_TYPE = "VehicleDeclarationType";

		public static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		protected IVehicleComponentsDeclaration _components;
		protected IPTOTransmissionInputData _ptoData;
		protected XmlElement _componentNode;
		protected XmlElement _ptoNode;
		protected XmlElement _adasNode;


		public XMLDeclarationVehicleDataProviderV10(IXMLDeclarationJobInputData jobData, XmlNode xmlNode, string sourceFile)
			: base(xmlNode, sourceFile)
		{
			Job = jobData;
			SourceType = DataSourceType.XMLEmbedded;
		}


		public virtual XmlElement ComponentNode
		{
			get {
				if (ExemptedVehicle) {
					return null;
				}

				return _componentNode ?? (_componentNode = GetNode(XMLNames.Vehicle_Components) as XmlElement);
			}
		}

		public virtual IXMLComponentReader ComponentReader { protected get; set; }

		public virtual XmlElement PTONode
		{
			get {
				if (ExemptedVehicle) {
					return null;
				}

				return _ptoNode ?? (_ptoNode = GetNode(XMLNames.Vehicle_PTO) as XmlElement);
			}
		}

		public virtual IXMLPTOReader PTOReader { protected get; set; }

		public virtual XmlElement ADASNode
		{
			get { return _adasNode ?? (_adasNode = GetNode(XMLNames.Vehicle_ADAS, required: false) as XmlElement); }
		}

		public virtual IXMLADASReader ADASReader { protected get; set; }

		public virtual IXMLDeclarationJobInputData Job { get; }

		public virtual string Identifier
		{
			get { return GetAttribute(BaseNode, XMLNames.Component_ID_Attr); }
		}

		public virtual bool ExemptedVehicle
		{
			get { return ElementExists(XMLNames.Vehicle_HybridElectricHDV) && ElementExists(XMLNames.Vehicle_DualFuelVehicle); }
		}

		public virtual string VIN
		{
			get { return GetString(XMLNames.Vehicle_VIN); }
		}

		public virtual LegislativeClass? LegislativeClass
		{
			get { return GetString(XMLNames.Vehicle_LegislativeClass).ParseEnum<LegislativeClass>(); }
		}

		public virtual VehicleCategory VehicleCategory
		{
			get {
				var val = GetString(XMLNames.Vehicle_VehicleCategory);
				if ("Rigid Lorry".Equals(val, StringComparison.InvariantCultureIgnoreCase)) {
					return VehicleCategory.RigidTruck;
				}

				return val.ParseEnum<VehicleCategory>();
			}
		}

		public virtual Kilogram CurbMassChassis
		{
			get { return GetDouble(XMLNames.Vehicle_CurbMassChassis).SI<Kilogram>(); }
		}


		public virtual Kilogram GrossVehicleMassRating
		{
			get { return GetDouble(XMLNames.Vehicle_GrossVehicleMass).SI<Kilogram>(); }
		}

		public virtual IList<ITorqueLimitInputData> TorqueLimits
		{
			get {
				var retVal = new List<ITorqueLimitInputData>();
				var limits = GetNodes(new[] { XMLNames.Vehicle_TorqueLimits, XMLNames.Vehicle_TorqueLimits_Entry });
				foreach (XmlNode current in limits) {
					if (current.Attributes != null) {
						retVal.Add(
							new TorqueLimitInputData() {
								Gear = GetAttribute(current, XMLNames.Vehicle_TorqueLimits_Entry_Gear_Attr).ToInt(),
								MaxTorque = GetAttribute(current, XMLNames.Vehicle_TorqueLimits_Entry_MaxTorque_Attr)
									.ToDouble().SI<NewtonMeter>()
							});
					}
				}

				return retVal;
			}
		}

		public virtual AxleConfiguration AxleConfiguration
		{
			get { return AxleConfigurationHelper.Parse(GetString(XMLNames.Vehicle_AxleConfiguration)); }
		}

		public virtual string ManufacturerAddress
		{
			get { return GetString(XMLNames.Component_ManufacturerAddress); }
		}

		public virtual PerSecond EngineIdleSpeed
		{
			get { return GetDouble(XMLNames.Vehicle_IdlingSpeed).RPMtoRad(); }
		}

		public virtual double RetarderRatio
		{
			get { return GetDouble(XMLNames.Vehicle_RetarderRatio); }
		}

		public virtual IPTOTransmissionInputData PTOTransmissionInputData
		{
			get { return _ptoData ?? (_ptoData = PTOReader.PTOInputData); }
		}

		public virtual RetarderType RetarderType
		{
			get {
				var value = GetString(XMLNames.Vehicle_RetarderType); //.ParseEnum<RetarderType>(); 
				switch (value) {
					case "None": return RetarderType.None;
					case "Losses included in Gearbox": return RetarderType.LossesIncludedInTransmission;
					case "Engine Retarder": return RetarderType.EngineRetarder;
					case "Transmission Input Retarder": return RetarderType.TransmissionInputRetarder;
					case "Transmission Output Retarder": return RetarderType.TransmissionOutputRetarder;
				}

				throw new ArgumentOutOfRangeException("RetarderType", value);
			}
		}

		public virtual AngledriveType AngledriveType
		{
			get { return GetString(XMLNames.Vehicle_AngledriveType).ParseEnum<AngledriveType>(); }
		}

		public virtual bool VocationalVehicle
		{
			get { return XmlConvert.ToBoolean(GetString(XMLNames.Vehicle_VocationalVehicle)); }
		}

		public virtual bool SleeperCab
		{
			get { return XmlConvert.ToBoolean(GetString(XMLNames.Vehicle_SleeperCab)); }
		}

		public virtual bool? AirdragModifiedMultistage { get; }

		public virtual TankSystem? TankSystem
		{
			get {
				return ElementExists(XMLNames.Vehicle_NgTankSystem)
					? EnumHelper.ParseEnum<TankSystem>(GetString(XMLNames.Vehicle_NgTankSystem))
					: (TankSystem?)null;
			}
		}

		public virtual IAdvancedDriverAssistantSystemDeclarationInputData ADAS
		{
			get { return ADASReader.ADASInputData; }
		}

		public virtual bool ZeroEmissionVehicle
		{
			get { return XmlConvert.ToBoolean(GetString(XMLNames.Vehicle_ZeroEmissionVehicle)); }
		}

		public virtual bool HybridElectricHDV
		{
			get { return XmlConvert.ToBoolean(GetString(XMLNames.Vehicle_HybridElectricHDV)); }
		}

		public virtual bool DualFuelVehicle
		{
			get { return XmlConvert.ToBoolean(GetString(XMLNames.Vehicle_DualFuelVehicle)); }
		}

		public virtual Watt MaxNetPower1
		{
			get {
				return ElementExists(XMLNames.Vehicle_MaxNetPower1)
					? GetDouble(XMLNames.Vehicle_MaxNetPower1).SI<Watt>()
					: null;
			}
		}

		public virtual Watt MaxNetPower2
		{
			get {
				return ElementExists(XMLNames.Vehicle_MaxNetPower2)
					? GetDouble(XMLNames.Vehicle_MaxNetPower2).SI<Watt>()
					: null;
			}
		}

		public virtual RegistrationClass? RegisteredClass
		{
			get { return RegistrationClass.unknown; }
		}

		public virtual int? NumberOfPassengersUpperDeck
		{
			get { return 0; }
		}

		public virtual int? NumberOfPassengersLowerDeck
		{
			get { return 0; }
		}

		public virtual CubicMeter CargoVolume
		{
			get { return 0.SI<CubicMeter>(); }
		}

		public virtual VehicleCode? VehicleCode
		{
			get { return VectoCommon.Models.VehicleCode.NOT_APPLICABLE; }
		}

		public virtual bool? LowEntry
		{
			get { return false; }
		}

		public virtual bool Articulated
		{
			get { return false; }
		}

		public virtual Meter Height
		{
			get { return null; }
		}

		public virtual Meter Length
		{
			get { return null; }
		}

		public virtual Meter Width
		{
			get { return null; }
		}

		public virtual Meter EntranceHeight
		{
			get { return null; }
		}

		public virtual ConsumerTechnology? DoorDriveTechnology { get { return ConsumerTechnology.Unknown; } }
		public virtual VehicleDeclarationType VehicleDeclarationType { get; }


		public virtual IVehicleComponentsDeclaration Components
		{
			get { return _components ?? (_components = ComponentReader.ComponentInputData); }
		}

		#region Implementation of IAdvancedDriverAssistantSystemDeclarationInputData

		#endregion

		#region Overrides of AbstractXMLResource

		protected override XNamespace SchemaNamespace
		{
			get { return NAMESPACE_URI; }
		}

		protected override DataSourceType SourceType { get; }

		#endregion
	}

	// ---------------------------------------------------------------------------------------

	public class XMLDeclarationVehicleDataProviderV20 : XMLDeclarationVehicleDataProviderV10
	{
		/*
		 * use default values for new parameters introduced in 2019/318 (amendment of 2017/2400
		 */

		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V20;

		public new const string XSD_TYPE = "VehicleDeclarationType";

		public new static readonly string QUALIFIED_XSD_TYPE =
			XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		public XMLDeclarationVehicleDataProviderV20(IXMLDeclarationJobInputData jobData, XmlNode xmlNode, string sourceFile) :
			base(jobData, xmlNode, sourceFile) { }

		protected override XNamespace SchemaNamespace
		{
			get { return NAMESPACE_URI; }
		}

		public override bool VocationalVehicle
		{
			get { return false; }
		}

		public override bool SleeperCab
		{
			get { return true; }
		}

		public override TankSystem? TankSystem
		{
			get { return VectoCommon.InputData.TankSystem.Compressed; }
		}

		public override IAdvancedDriverAssistantSystemDeclarationInputData ADAS
		{
			get { return new ADASDefaultValues(); }
		}

		public override bool ZeroEmissionVehicle
		{
			get { return false; }
		}

		public override bool HybridElectricHDV
		{
			get { return false; }
		}

		public override bool DualFuelVehicle
		{
			get { return false; }
		}

		public override Watt MaxNetPower1
		{
			get { return null; }
		}

		public override Watt MaxNetPower2
		{
			get { return null; }
		}

		public class ADASDefaultValues : IAdvancedDriverAssistantSystemDeclarationInputData
		{
			#region Implementation of IAdvancedDriverAssistantSystemDeclarationInputData

			public bool EngineStopStart
			{
				get { return false; }
			}

			public EcoRollType EcoRoll
			{
				get { return EcoRollType.None; }
			}

			public PredictiveCruiseControlType PredictiveCruiseControl
			{
				get { return PredictiveCruiseControlType.None; }
			}

			public bool? ATEcoRollReleaseLockupClutch
			{
				get { return null; }
			}

			public XmlNode XMLSource
			{
				get { return null; }
			}

			#endregion
		}
	}

	// ---------------------------------------------------------------------------------------

	public class XMLDeclarationVehicleDataProviderV21 : XMLDeclarationVehicleDataProviderV10
	{
		/*
		 * added new parameters introduced in 2019/318 (amendment of 2017/2400) (already implemented in version 1.0)
		 */

		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V21;

		public new const string XSD_TYPE = "VehicleDeclarationType";

		public new static readonly string QUALIFIED_XSD_TYPE =
			XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);


		public XMLDeclarationVehicleDataProviderV21(IXMLDeclarationJobInputData jobData, XmlNode xmlNode, string sourceFile) :
			base(jobData, xmlNode, sourceFile) { }

		#region Overrides of XMLDeclarationVehicleDataProviderV10

		#endregion

		public override VehicleCategory VehicleCategory
		{
			get {
				var val = GetString(XMLNames.Vehicle_VehicleCategory);
				if ("Rigid Lorry".Equals(val, StringComparison.InvariantCultureIgnoreCase)) {
					return VehicleCategory.RigidTruck;
				}

				return val.ParseEnum<VehicleCategory>();
			}
		}

		protected override XNamespace SchemaNamespace
		{
			get { return NAMESPACE_URI; }
		}
	}

	// ---------------------------------------------------------------------------------------

	public class XMLDeclarationExemptedVehicleDataProviderV22 : XMLDeclarationVehicleDataProviderV20
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V21;

		public new const string XSD_TYPE = "ExemptedVehicleDeclarationType";

		public new static readonly string QUALIFIED_XSD_TYPE =
			XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		public XMLDeclarationExemptedVehicleDataProviderV22(
			IXMLDeclarationJobInputData jobData, XmlNode xmlNode, string sourceFile) : base(jobData, xmlNode, sourceFile)
		{
			SourceType = DataSourceType.XMLEmbedded;
		}

		#region Overrides of AbstractXMLResource

		protected override XNamespace SchemaNamespace
		{
			get { return NAMESPACE_URI; }
		}

		protected override DataSourceType SourceType { get; }

		#endregion

		#region Implementation of IVehicleDeclarationInputData

		public override bool ExemptedVehicle
		{
			get { return true; }
		}

		public override AxleConfiguration AxleConfiguration
		{
			get { return AxleConfiguration.AxleConfig_Undefined; }
		}

		public override IList<ITorqueLimitInputData> TorqueLimits
		{
			get { return new List<ITorqueLimitInputData>(); }
		}

		public override PerSecond EngineIdleSpeed
		{
			get { return null; }
		}

		public override bool VocationalVehicle
		{
			get { return false; }
		}

		public override bool SleeperCab
		{
			get { return false; }
		}

		public override TankSystem? TankSystem
		{
			get { return null; }
		}

		public override IAdvancedDriverAssistantSystemDeclarationInputData ADAS
		{
			get { return null; }
		}

		public override bool ZeroEmissionVehicle
		{
			get { return XmlConvert.ToBoolean(GetString(XMLNames.Vehicle_ZeroEmissionVehicle)); }
		}

		public override bool HybridElectricHDV
		{
			get { return XmlConvert.ToBoolean(GetString(XMLNames.Vehicle_HybridElectricHDV)); }
		}

		public override bool DualFuelVehicle
		{
			get { return XmlConvert.ToBoolean(GetString(XMLNames.Vehicle_DualFuelVehicle)); }
		}

		public override Watt MaxNetPower1
		{
			get { return GetDouble(XMLNames.Vehicle_MaxNetPower1).SI<Watt>(); }
		}

		public override Watt MaxNetPower2
		{
			get { return GetDouble(XMLNames.Vehicle_MaxNetPower2).SI<Watt>(); }
		}

		public override IVehicleComponentsDeclaration Components
		{
			get { return null; }
		}

		#endregion

		#region Implementation of IXMLDeclarationVehicleData

		public override XmlElement ComponentNode
		{
			get { return null; }
		}

		public override XmlElement PTONode
		{
			get { return null; }
		}

		public override XmlElement ADASNode
		{
			get { return null; }
		}

		public override AngledriveType AngledriveType
		{
			get { return AngledriveType.None; }
		}

		public override RetarderType RetarderType
		{
			get { return RetarderType.None; }
		}

		public override double RetarderRatio
		{
			get { return 0; }
		}

		public override IPTOTransmissionInputData PTOTransmissionInputData
		{
			get { return null; }
		}

		#endregion
	}

	// ---------------------------------------------------------------------------------------


	public class XMLDeclarationPrimaryBusVehicleDataProviderV26 : XMLDeclarationVehicleDataProviderV20
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V26;

		public new const string XSD_TYPE = "PrimaryVehicleDeclarationType";

		public new static readonly string QUALIFIED_XSD_TYPE =
			XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		public XMLDeclarationPrimaryBusVehicleDataProviderV26(
			IXMLDeclarationJobInputData jobData, XmlNode xmlNode, string sourceFile) : base(jobData, xmlNode, sourceFile)
		{
			SourceType = DataSourceType.XMLEmbedded;
			
		}


		#region Overrides of XMLDeclarationVehicleDataProviderV10

		public override bool SleeperCab
		{
			get { return false; }
		}

		public override IAdvancedDriverAssistantSystemDeclarationInputData ADAS
		{
			get { return ADASReader.ADASInputData; }
		}

		public override XmlElement PTONode
		{
			get { return null; }
		}

		#region Overrides of XMLDeclarationVehicleDataProviderV10

		public override LegislativeClass? LegislativeClass
		{
			get { return VectoCommon.Models.LegislativeClass.M3; }
		}

		#endregion

		public override IPTOTransmissionInputData PTOTransmissionInputData
		{
			get { return null; }
		}

		public override VehicleCategory VehicleCategory
		{
			get { return VehicleCategory.HeavyBusPrimaryVehicle; }
		}

		public override bool Articulated
		{
			get { return GetBool(XMLNames.Vehicle_Articulated); }
		}

		public override Kilogram CurbMassChassis
		{
			get { return null; }
		}

		public override Kilogram GrossVehicleMassRating
		{
			get { return GetDouble(XMLNames.Vehicle_TPMLM).SI<Kilogram>(); }
		}

		public override Meter EntranceHeight
		{
			get { return null; }
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

	public class XMLDeclarationMediumLorryVehicleDataProviderV26 : XMLDeclarationVehicleDataProviderV21
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V26;

		public new const string XSD_TYPE = "VehicleMediumLorryDeclarationType";

		public new static readonly string QUALIFIED_XSD_TYPE =
			XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		public XMLDeclarationMediumLorryVehicleDataProviderV26(
			IXMLDeclarationJobInputData jobData, XmlNode xmlNode, string sourceFile) : base(jobData, xmlNode, sourceFile) { }

		#region Overrides of XMLDeclarationVehicleDataProviderV10

		public override bool SleeperCab
		{
			get { return false; }
		}

		public override bool VocationalVehicle
		{
			get { return false; }
		}

		public override IPTOTransmissionInputData PTOTransmissionInputData
		{
			get { return null; }
		}

		public override XmlElement PTONode
		{
			get { return null; }
		}

		public override Kilogram GrossVehicleMassRating
		{
			get { return GetDouble(XMLNames.Vehicle_TPMLM).SI<Kilogram>(); }
		}

		public override CubicMeter CargoVolume
		{
			get
			{
				if (VehicleCategory == VehicleCategory.Van && !ElementExists(XMLNames.Vehicle_CargoVolume)) {
					throw new VectoException("Medium lorries with type Van require the input parameter cargo volume!");
				}
				return ElementExists(XMLNames.Vehicle_CargoVolume) ? GetDouble(XMLNames.Vehicle_CargoVolume).SI<CubicMeter>()
					: 0.SI<CubicMeter>();
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

	public class XMLDeclarationCompletedBusDataProviderV26 : XMLDeclarationVehicleDataProviderV20
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V26;

		public new const string XSD_TYPE = "CompletedVehicleDeclarationType";

		public new static readonly string QUALIFIED_XSD_TYPE =
			XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		public XMLDeclarationCompletedBusDataProviderV26(
			IXMLDeclarationJobInputData jobData, XmlNode xmlNode, string sourceFile) : base(jobData, xmlNode, sourceFile)
		{
			SourceType = DataSourceType.XMLEmbedded;
		}


		#region Overrides of XMLDeclarationVehicleDataProviderV10

		public override VehicleCategory VehicleCategory
		{
			get { return VehicleCategory.HeavyBusCompletedVehicle; }
		}

		public override RegistrationClass? RegisteredClass
		{
			get { return RegistrationClassHelper.Parse(GetString(XMLNames.Vehicle_RegisteredClass)).First(); }
		}

		public override VehicleCode? VehicleCode
		{
			get { return GetString(XMLNames.Vehicle_VehicleCode).ParseEnum<VehicleCode>(); }
		}

		//TechnicalPermissibleMaximumLadenMass
		public override Kilogram GrossVehicleMassRating
		{
			get { return GetDouble(XMLNames.TPMLM).SI<Kilogram>(); }
		}

		public override TankSystem? TankSystem
		{
			get
			{
				return ElementExists(XMLNames.Vehicle_NgTankSystem)
						? EnumHelper.ParseEnum<TankSystem>(GetString(XMLNames.Vehicle_NgTankSystem))
						: (TankSystem?)null;
			}
		}

		public override int? NumberOfPassengersLowerDeck
		{
			get {
				var node = GetNode(XMLNames.Bus_LowerDeck);
				return XmlConvert.ToInt32(node.InnerText);
			}
		}

		public override int? NumberOfPassengersUpperDeck
		{
			get {
				var node = GetNode(XMLNames.Bus_UpperDeck);
				return XmlConvert.ToInt32(node.InnerText);
			}
		}

		//HeightIntegratedBody
		public override Meter Height
		{
			get { return GetDouble(XMLNames.Bus_HeighIntegratedBody).SI<Meter>(); }
		}

		//VehicleLength
		public override Meter Length
		{
			get { return GetDouble(XMLNames.Bus_VehicleLength).SI<Meter>(); }
		}

		//VehicleWidth
		public override Meter Width
		{
			get { return GetDouble(XMLNames.Bus_VehicleWidth).SI<Meter>(); }
		}

		public override XmlElement PTONode
		{
			get { return null; }
		}

		#endregion

		
		public override bool? LowEntry
		{
			get { return GetBool(XMLNames.Bus_LowEntry); }
		}

		public override Meter EntranceHeight
		{
			get { return GetDouble(XMLNames.Bus_EntranceHeight).SI<Meter>(); }
		}

		public override ConsumerTechnology? DoorDriveTechnology
		{
			get { return ConsumerTechnologyHelper.Parse(GetString(XMLNames.BusAux_PneumaticSystem_DoorDriveTechnology)); }
		}
		

		#region Overrides of AbstractXMLResource

		protected override XNamespace SchemaNamespace
		{
			get { return NAMESPACE_URI; }
		}

		protected override DataSourceType SourceType { get; }

		#endregion
	}


	public class XMLDeclarationMultistagePrimaryVehicleBusDataProviderV01 : AbstractCommonComponentType, IXMLDeclarationVehicleData
	{

		public static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_MULTISTAGE_BUS_VEHICLE_NAMESPACE_VO1;

		public const string XSD_TYPE = "VehiclePIFType";

		public static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		protected IXMLPrimaryVehicleBusJobInputData BusJobData;
		private XmlElement _adasNode;
		private IAdvancedDriverAssistantSystemDeclarationInputData _adas;
		private XmlElement _componentNode;
		private IVehicleComponentsDeclaration _components;
		
		public XMLDeclarationMultistagePrimaryVehicleBusDataProviderV01(
			IXMLPrimaryVehicleBusJobInputData busJobData, XmlNode xmlNode, string sourceFile)
			: base(xmlNode, sourceFile)
		{
			BusJobData = busJobData;
		}

		#region Overrides of AbstractCommonComponentType

		public override string Manufacturer
		{
			get { return GetString(XMLNames.ManufacturerPrimaryVehicle); }
		}

		public string ManufacturerAddress
		{
			get { return GetString(XMLNames.ManufacturerAddressPrimaryVehicle); }
		}

		#endregion


		#region IXMLDeclarationVehicleData interface

		public string VIN
		{
			get { return GetString(XMLNames.Vehicle_VIN); }
		}

		public VehicleCategory VehicleCategory
		{
			get { return VehicleCategoryHelper.Parse(GetString(XMLNames.Vehicle_VehicleCategory)); }
		}

		public AxleConfiguration AxleConfiguration
		{
			get { return AxleConfigurationHelper.Parse(GetString(XMLNames.Vehicle_AxleConfiguration)); }
		}

		//TechnicalPermissibleMaximumLadenMass
		public Kilogram GrossVehicleMassRating
		{
			get { return GetDouble(XMLNames.TPMLM).SI<Kilogram>(); }
		}

		//IdlingSpeed
		public PerSecond EngineIdleSpeed
		{
			get { return GetDouble(XMLNames.Engine_IdlingSpeed).RPMtoRad(); }
		}

		public RetarderType RetarderType
		{
			get { return GetString(XMLNames.Vehicle_RetarderType).ParseEnum<RetarderType>(); }
		}

		public double RetarderRatio
		{
			get { return GetDouble(XMLNames.Vehicle_RetarderRatio); }
		}

		public AngledriveType AngledriveType
		{
			get { return GetString(XMLNames.Vehicle_AngledriveType).ParseEnum<AngledriveType>(); }
		}

		public bool ZeroEmissionVehicle
		{
			get { return GetBool(XMLNames.Vehicle_ZeroEmissionVehicle); }
		}

		public XmlElement ADASNode
		{
			get { return _adasNode ?? (_adasNode = GetNode(XMLNames.Vehicle_ADAS, required: false) as XmlElement); }
		}

		public IXMLADASReader ADASReader { get; set; }

		public IAdvancedDriverAssistantSystemDeclarationInputData ADAS
		{
			get { return _adas ?? (_adas = ADASReader.ADASInputData); }
		}


		public IList<ITorqueLimitInputData> TorqueLimits
		{
			get { return ReadTorqueLimits(); }
		}

		public XmlElement ComponentNode
		{
			get
			{
				if (ExemptedVehicle)
				{
					return null;
				}

				return _componentNode ?? (_componentNode = GetNode(XMLNames.Vehicle_Components) as XmlElement);
			}
		}

		public IXMLComponentReader ComponentReader { get; set; }

		public Meter EntranceHeight { get; }

		public virtual ConsumerTechnology? DoorDriveTechnology
		{
			get { return ConsumerTechnology.Unknown; }
		}

		public virtual VehicleDeclarationType VehicleDeclarationType { get; }


		public IVehicleComponentsDeclaration Components
		{
			get { return _components ?? (_components = ComponentReader.ComponentInputData); }
		}


		#region  Non seeded Properties

		public string Identifier { get; }
		public bool ExemptedVehicle { get; }
		public LegislativeClass? LegislativeClass { get; }
		public int? NumberOfPassengersUpperDeck { get; }
		public int? NumberOfPassengersLowerDeck { get; }
		public CubicMeter CargoVolume
		{
			get { return 0.SI<CubicMeter>(); }
		}
		public Kilogram CurbMassChassis { get; }
		public bool VocationalVehicle { get; }
		public bool SleeperCab { get; }
		public bool? AirdragModifiedMultistage { get; }
		public TankSystem? TankSystem { get; }

		public bool HybridElectricHDV { get; }
		public bool DualFuelVehicle { get; }
		public Watt MaxNetPower1 { get; }
		public Watt MaxNetPower2 { get; }
		public RegistrationClass? RegisteredClass { get; }
		public VehicleCode? VehicleCode { get; }
		public bool? LowEntry { get; }
		public bool Articulated { get { return GetBool(XMLNames.Vehicle_Articulated); } }
		public Meter Height { get; }
		public Meter Length { get; }
		public Meter Width { get; }

		public XmlElement PTONode { get; }
		public IXMLPTOReader PTOReader { get; set; }
		public IPTOTransmissionInputData PTOTransmissionInputData { get; }

		#endregion

		#endregion

		#region Overrides of AbstractXMLResource

		protected override XNamespace SchemaNamespace
		{
			get { return NAMESPACE_URI; }
		}

		protected override DataSourceType SourceType
		{
			get { return DataSourceType.XMLFile; }
		}

		#endregion


		private IList<ITorqueLimitInputData> ReadTorqueLimits()
		{
			var torqueLimits = new List<ITorqueLimitInputData>();
			var limits = GetNodes(new[] { XMLNames.Vehicle_TorqueLimits, XMLNames.Vehicle_TorqueLimits_Entry });
			foreach (XmlNode current in limits)
			{
				if (current.Attributes != null)
				{
					torqueLimits.Add(
						new TorqueLimitInputData()
						{
							Gear = GetAttribute(current, XMLNames.Vehicle_TorqueLimits_Entry_Gear_Attr).ToInt(),
							MaxTorque = GetAttribute(current, XMLNames.Vehicle_TorqueLimits_Entry_MaxTorque_Attr)
								.ToDouble().SI<NewtonMeter>()
						});
				}
			}

			return torqueLimits;
		}
	}



	// ---------------------------------------------------------------------------------------

	public class XMLDeclarationInterimStageBusDataProviderV28 : XMLDeclarationVehicleDataProviderV20
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V28;
		public new const string XSD_TYPE = "InterimStageInputType";
		public new static readonly string QUALIFIED_XSD_TYPE =
			XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		private IAdvancedDriverAssistantSystemDeclarationInputData _adas;
		
		public XMLDeclarationInterimStageBusDataProviderV28(
			IXMLDeclarationJobInputData jobData, XmlNode xmlNode, string sourceFile) 
			: base(jobData, xmlNode, sourceFile) { }

		public override string Model
		{
			get
			{
				return ElementExists(XMLNames.Component_Model)
					? GetString(XMLNames.Component_Model) : null;
			}
		}

		public override LegislativeClass? LegislativeClass
		{
			get
			{
				return ElementExists(XMLNames.Bus_LegislativeCategory)
					? GetString(XMLNames.Bus_LegislativeCategory).ParseEnum<LegislativeClass>()
					:  (LegislativeClass?)null;
			}
		}
		
		public override Kilogram CurbMassChassis
		{
			get
			{
				return ElementExists(XMLNames.Bus_CorrectedActualMass)
					? GetDouble(XMLNames.Bus_CorrectedActualMass).SI<Kilogram>()
					: null;
			}
		}
		
		public override Kilogram GrossVehicleMassRating
		{
			get { 
				return ElementExists(XMLNames.Vehicle_TPMLM)
					? GetDouble(XMLNames.Vehicle_TPMLM).SI<Kilogram>()
					: null;
			}
		}

		public override bool? AirdragModifiedMultistage
		{
			get
			{
				return ElementExists(XMLNames.Bus_AirdragModifiedMultistage)
					? GetBool(XMLNames.Bus_AirdragModifiedMultistage)
					: (bool?)null;
			}
		}

		public override RegistrationClass? RegisteredClass
		{
			get
			{
				return ElementExists(XMLNames.Vehicle_RegisteredClass)
					? RegistrationClassHelper.Parse(GetString(XMLNames.Vehicle_RegisteredClass)).First()
					: null;
			}
		}

		public override TankSystem? TankSystem
		{
			get
			{
				return ElementExists(XMLNames.Vehicle_NgTankSystem)
					? EnumHelper.ParseEnum<TankSystem>(GetString(XMLNames.Vehicle_NgTankSystem))
					: (TankSystem?)null;
			}
		}


		public override int? NumberOfPassengersLowerDeck
		{
			get
			{
				if (!ElementExists(XMLNames.Bus_NumberPassengersLowerDeck))
					return null;
				var node = GetNode(XMLNames.Bus_NumberPassengersLowerDeck);
				return XmlConvert.ToInt32(node.InnerText);
			}
		}

		public override int? NumberOfPassengersUpperDeck
		{
			get
			{
				if (!ElementExists(XMLNames.Bus_NumberPassengersUpperDeck))
					return null;
				var node = GetNode(XMLNames.Bus_NumberPassengersUpperDeck);
				return XmlConvert.ToInt32(node.InnerText);
			}
		}

		public override VehicleCode? VehicleCode
		{
			get
			{
				return ElementExists(XMLNames.Vehicle_VehicleCode)
					? GetString(XMLNames.Vehicle_VehicleCode).ParseEnum<VehicleCode>()
					: (VehicleCode?)null;
			}
		}

		public override bool? LowEntry
		{
			get 
			{ 
				return ElementExists(XMLNames.Bus_LowEntry) 
					? GetBool(XMLNames.Bus_LowEntry)
					: (bool?)null;
			}
		}

		public override Meter Height
		{
			get
			{
				return ElementExists(XMLNames.Bus_HeighIntegratedBody)
					? GetDouble(XMLNames.Bus_HeighIntegratedBody).SI<Meter>() * 0.001 
					: null;
			}
		}

		public override Meter Length
		{
			get
			{
				return ElementExists(XMLNames.Bus_VehicleLength)
					? GetDouble(XMLNames.Bus_VehicleLength).SI<Meter>() * 0.001
					: null;
			}
		}

		public override Meter Width
		{
			get
			{
				return ElementExists(XMLNames.Bus_VehicleWidth)
					? GetDouble(XMLNames.Bus_VehicleWidth).SI<Meter>() * 0.001
					: null;
			}
		}

		public override Meter EntranceHeight
		{
			get
			{
				return ElementExists(XMLNames.Bus_EntranceHeight)
					? GetDouble(XMLNames.Bus_EntranceHeight).SI<Meter>() * 0.001
					: null;
			}
		}

		public override ConsumerTechnology? DoorDriveTechnology
		{
			get
			{
				return ElementExists(XMLNames.BusAux_PneumaticSystem_DoorDriveTechnology)
					? ConsumerTechnologyHelper.Parse(GetString(XMLNames.BusAux_PneumaticSystem_DoorDriveTechnology))
					: (ConsumerTechnology?)null;
			}
		}

		public override VehicleDeclarationType VehicleDeclarationType
		{
			get { return VehicleDeclarationTypeHelper.Parse(GetString(XMLNames.Bus_DeclarationType)); }
		}


		public override XmlElement ADASNode
		{
			get
			{
				return _adasNode ?? (_adasNode = GetNode(XMLNames.Vehicle_ADAS, required: false) as XmlElement);
			}
		}


		public override XmlElement ComponentNode
		{
			get
			{
				if (ExemptedVehicle)
					return null;

				return _componentNode ?? (_componentNode = GetNode(XMLNames.Vehicle_Components, required:false) as XmlElement);
			}
		}


		public override IAdvancedDriverAssistantSystemDeclarationInputData ADAS
		{
			get
			{
				if (ADASNode == null)
					return null;
				return _adas ?? (_adas = ADASReader.ADASInputData);
			}
		}

		public override IVehicleComponentsDeclaration Components
		{
			get 
			{ 
				if (ComponentNode == null)
					return null;
				return _components ?? (_components = ComponentReader.ComponentInputData);
			}
		}
		
		public override XmlElement PTONode
		{
			get { return null; }
		}




		#region Overrides of AbstractXMLResource

		protected override DataSourceType SourceType
		{
			get { return DataSourceType.XMLFile; }
		}
		
		protected override XNamespace SchemaNamespace
		{
			get { return NAMESPACE_URI; }
		}
		
		#endregion

	}

	// ---------------------------------------------------------------------------------------

	public class XMLDeclarationExemptedInterimStageBusDataProviderV28 : XMLDeclarationVehicleDataProviderV20
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V28;
		public new const string XSD_TYPE = "ExemptedInterimStageInputType";
		public new static readonly string QUALIFIED_XSD_TYPE =
			XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);
		
		public XMLDeclarationExemptedInterimStageBusDataProviderV28(IXMLDeclarationJobInputData jobData,
			XmlNode xmlNode, string sourceFile)
			: base(jobData, xmlNode, sourceFile) {}


		public override string Model
		{
			get
			{
				return ElementExists(XMLNames.Component_Model)
					? GetString(XMLNames.Component_Model) : null;
			}
		}

		public override LegislativeClass? LegislativeClass
		{
			get
			{
				return ElementExists(XMLNames.Bus_LegislativeCategory)
					? GetString(XMLNames.Bus_LegislativeCategory).ParseEnum<LegislativeClass>()
					: (LegislativeClass?)null;
			}
		}

		public override Kilogram CurbMassChassis
		{
			get
			{
				return ElementExists(XMLNames.Bus_CorrectedActualMass)
					? GetDouble(XMLNames.Bus_CorrectedActualMass).SI<Kilogram>()
					: null;
			}
		}

		public override Kilogram GrossVehicleMassRating
		{
			get
			{
				return ElementExists(XMLNames.Vehicle_TPMLM)
					? GetDouble(XMLNames.Vehicle_TPMLM).SI<Kilogram>()
					: null;
			}
		}
		
		public override RegistrationClass? RegisteredClass
		{
			get
			{
				return ElementExists(XMLNames.Vehicle_RegisteredClass)
					? RegistrationClassHelper.Parse(GetString(XMLNames.Vehicle_RegisteredClass)).First()
					: null;
			}
		}

		public override int? NumberOfPassengersLowerDeck
		{
			get
			{
				if (!ElementExists(XMLNames.Bus_NumberPassengersLowerDeck))
					return null;
				var node = GetNode(XMLNames.Bus_NumberPassengersLowerDeck);
				return XmlConvert.ToInt32(node.InnerText);
			}
		}

		public override int? NumberOfPassengersUpperDeck
		{
			get
			{
				if (!ElementExists(XMLNames.Bus_NumberPassengersUpperDeck))
					return null;
				var node = GetNode(XMLNames.Bus_NumberPassengersUpperDeck);
				return XmlConvert.ToInt32(node.InnerText);
			}
		}

		public override VehicleCode? VehicleCode
		{
			get
			{
				return ElementExists(XMLNames.Vehicle_VehicleCode)
					? GetString(XMLNames.Vehicle_VehicleCode).ParseEnum<VehicleCode>()
					: (VehicleCode?)null;
			}
		}

		public override bool? LowEntry
		{
			get
			{
				return ElementExists(XMLNames.Bus_LowEntry)
					? GetBool(XMLNames.Bus_LowEntry)
					: (bool?)null;
			}
		}

		public override Meter Height
		{
			get
			{
				return ElementExists(XMLNames.Bus_HeighIntegratedBody)
					? GetDouble(XMLNames.Bus_HeighIntegratedBody).SI<Meter>() * 0.001
					: null;
			}
		}
		
		public override XmlElement PTONode
		{
			get { return null; }
		}

		public override XmlElement ComponentNode
		{
			get{ return null; }
		}

		public override IVehicleComponentsDeclaration Components
		{
			get { return null; }
		}

		#region Overrides of AbstractXMLResource

		protected override XNamespace SchemaNamespace
		{
			get { return NAMESPACE_URI; }
		}

		protected override DataSourceType SourceType
		{
			get { return DataSourceType.XMLFile; }
		}


		#endregion
	}
}


