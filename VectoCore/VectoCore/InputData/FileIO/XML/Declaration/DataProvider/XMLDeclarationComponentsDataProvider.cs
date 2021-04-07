using System;
using System.Xml;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Reader;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider
{
	public class XMLDeclarationComponentsDataProviderV10 : AbstractCommonComponentType, IXMLVehicleComponentsDeclaration
	{
		public static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V10;

		public const string XSD_TYPE = "VehicleComponentsType";

		public static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		protected IAirdragDeclarationInputData _airdragInputData;
		protected IGearboxDeclarationInputData _gearboxInputData;
		protected IAxleGearInputData _axleGearInputData;
		protected IAngledriveInputData _angledriveInputData;
		protected IEngineDeclarationInputData _engineInputData;
		protected IAuxiliariesDeclarationInputData _auxInputData;
		protected IRetarderInputData _retarderInputData;
		protected IAxlesDeclarationInputData _axleWheels;
		protected IPTOTransmissionInputData _ptoInputData;
		protected IXMLDeclarationVehicleData _vehicle;
		protected ITorqueConverterDeclarationInputData _torqueconverterInputData;


		public XMLDeclarationComponentsDataProviderV10(
			IXMLDeclarationVehicleData vehicle, XmlNode componentNode, string sourceFile) :
			base(componentNode, sourceFile)
		{
			SourceType = DataSourceType.XMLFile;
			_vehicle = vehicle;
		}

		#region Implementation of IVehicleComponentsDeclaration

		public virtual IAirdragDeclarationInputData AirdragInputData
		{
			get { return _airdragInputData ?? (_airdragInputData = ComponentReader.AirdragInputData); }
		}

		public virtual IGearboxDeclarationInputData GearboxInputData
		{
			get { return _gearboxInputData ?? (_gearboxInputData = ComponentReader.GearboxInputData); }
		}


		public virtual ITorqueConverterDeclarationInputData TorqueConverterInputData
		{
			get { return _torqueconverterInputData ?? (_torqueconverterInputData = ComponentReader.TorqueConverterInputData); }
		}

		public virtual IAxleGearInputData AxleGearInputData
		{
			get { return _axleGearInputData ?? (_axleGearInputData = ComponentReader.AxleGearInputData); }
		}

		public virtual IAngledriveInputData AngledriveInputData
		{
			get { return _angledriveInputData ?? (_angledriveInputData = ComponentReader.AngledriveInputData); }
		}

		public virtual IEngineDeclarationInputData EngineInputData
		{
			get { return _engineInputData ?? (_engineInputData = ComponentReader.EngineInputData); }
		}

		IAuxiliariesDeclarationInputData IVehicleComponentsDeclaration.AuxiliaryInputData
		{
			get { return _auxInputData ?? (_auxInputData = ComponentReader.AuxiliaryData); }
		}

		public virtual IRetarderInputData RetarderInputData
		{
			get { return _retarderInputData ?? (_retarderInputData = ComponentReader.RetarderInputData); }
		}

		public virtual IPTOTransmissionInputData PTOTransmissionInputData
		{
			get { return _vehicle.PTOTransmissionInputData; }
		}

		public virtual IAxlesDeclarationInputData AxleWheels
		{
			get { return _axleWheels ?? (_axleWheels = ComponentReader.AxlesDeclarationInputData); }
		}

		public virtual IBusAuxiliariesDeclarationData BusAuxiliaries { get { return null; } }
		public virtual IElectricStorageDeclarationInputData ElectricStorage { get { return null; } }
		public virtual IElectricMachinesDeclarationInputData ElectricMachines { get { return null; } }

		#endregion

		#region Implementation of IXMLVehicleComponentsDeclaration

		public virtual IXMLComponentReader ComponentReader { protected get; set; }

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

	public class XMLDeclarationComponentsDataProviderV20 : XMLDeclarationComponentsDataProviderV10
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V20;

		public new const string XSD_TYPE = "VehicleComponentsType";

		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		public XMLDeclarationComponentsDataProviderV20(
			IXMLDeclarationVehicleData vehicle, XmlNode componentNode, string sourceFile) : base(
			vehicle, componentNode, sourceFile)
		{ }

		protected override XNamespace SchemaNamespace
		{
			get { return NAMESPACE_URI; }
		}
	}

	// ---------------------------------------------------------------------------------------

	public class XMLDeclarationPrimaryBusComponentsDataProviderV26 : XMLDeclarationComponentsDataProviderV10, IXMLVehicleComponentsDeclaration
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V26;

		public new const string XSD_TYPE = "PrimaryVehicleComponentsDeclarationType";

		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		private IBusAuxiliariesDeclarationData _busAuxiliaries;

		public XMLDeclarationPrimaryBusComponentsDataProviderV26(
			IXMLDeclarationVehicleData vehicle, XmlNode componentNode, string sourceFile) : base(
			vehicle, componentNode, sourceFile)
		{ }


		IAuxiliariesDeclarationInputData IVehicleComponentsDeclaration.AuxiliaryInputData
		{
			get { return null; }
		}

		public override IBusAuxiliariesDeclarationData BusAuxiliaries { get { return _busAuxiliaries ?? (_busAuxiliaries = ComponentReader.BusAuxiliariesInputData); } }

		protected override XNamespace SchemaNamespace
		{
			get { return NAMESPACE_URI; }
		}
	}

	// ---------------------------------------------------------------------------------------

	public class XMLDeclarationComponentsDataProviderNoAxlegearV26 : XMLDeclarationComponentsDataProviderV10
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V20;

		public new const string XSD_TYPE = "VehicleComponentsNoAxlegearType";

		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		public XMLDeclarationComponentsDataProviderNoAxlegearV26(
			IXMLDeclarationVehicleData vehicle, XmlNode componentNode, string sourceFile) : base(
			vehicle, componentNode, sourceFile)
		{ }

		#region Overrides of XMLDeclarationComponentsDataProviderV10

		public override IAxleGearInputData AxleGearInputData
		{
			get
			{
				return null;
				//throw new NotSupportedException("No Axlegeardata available"); 
			}
		}

		#endregion

		protected override XNamespace SchemaNamespace
		{
			get { return NAMESPACE_URI; }
		}
	}

	// ---------------------------------------------------------------------------------------


	public class XMLDeclarationCompleteBusComponentsDataProviderV26 : XMLDeclarationComponentsDataProviderV10, IXMLVehicleComponentsDeclaration
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V26;

		public new const string XSD_TYPE = "CompletedVehicleComponentsDeclarationType";

		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		private IBusAuxiliariesDeclarationData _busAuxiliaries;

		public XMLDeclarationCompleteBusComponentsDataProviderV26(
			IXMLDeclarationVehicleData vehicle, XmlNode componentNode, string sourceFile) : base(
			vehicle, componentNode, sourceFile)
		{ }

		IGearboxDeclarationInputData IVehicleComponentsDeclaration.GearboxInputData
		{
			get { return null; }
		}

		IAuxiliariesDeclarationInputData IVehicleComponentsDeclaration.AuxiliaryInputData
		{
			get { return null; }
		}

		public override IBusAuxiliariesDeclarationData BusAuxiliaries { get { return _busAuxiliaries ?? (_busAuxiliaries = ComponentReader.BusAuxiliariesInputData); } }

		protected override XNamespace SchemaNamespace
		{
			get { return NAMESPACE_URI; }
		}
	}


	// ---------------------------------------------------------------------------------------


	public class XMLDeclarationComponentsMultistagePrimaryVehicleBusDataProviderV01 : XMLDeclarationComponentsDataProviderV10,
		IXMLVehicleComponentsDeclaration, IRetarderInputData
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_MULTISTAGE_BUS_VEHICLE_NAMESPACE_VO1;

		public new const string XSD_TYPE = "VehicleComponentsPIFType";

		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		private IBusAuxiliariesDeclarationData _busAuxiliaries;

		public XMLDeclarationComponentsMultistagePrimaryVehicleBusDataProviderV01(IXMLDeclarationVehicleData vehicle,
			XmlNode componentNode, string sourceFile) : base(vehicle, componentNode, sourceFile) { }


		IRetarderInputData IVehicleComponentsDeclaration.RetarderInputData
		{
			get { return this; }
		}

		IAirdragDeclarationInputData IVehicleComponentsDeclaration.AirdragInputData
		{
			get { return null; }
		}

		IAuxiliariesDeclarationInputData IVehicleComponentsDeclaration.AuxiliaryInputData
		{
			get { return null; }
		}

		public override IBusAuxiliariesDeclarationData BusAuxiliaries
		{
			get { return _busAuxiliaries ?? (_busAuxiliaries = ComponentReader.BusAuxiliariesInputData); }
		}

		protected override XNamespace SchemaNamespace
		{
			get { return NAMESPACE_URI; }
		}

		#region IRetarderInputData Interface Implementation

		public RetarderType Type { get { return _vehicle.RetarderType; } }
		public double Ratio { get { return _vehicle.RetarderRatio; } }
		public TableData LossMap { get; }

		#endregion
	}


	// ---------------------------------------------------------------------------------------

	public class XMLDeclarationInterimStageBusComponentsDataProviderV28 : XMLDeclarationComponentsDataProviderV10,
		IXMLVehicleComponentsDeclaration
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V28;

		public new const string XSD_TYPE = "CompletedVehicleComponentsDeclarationType";

		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);


		private IBusAuxiliariesDeclarationData _busAuxiliaries;


		public XMLDeclarationInterimStageBusComponentsDataProviderV28(IXMLDeclarationVehicleData vehicle,
				XmlNode componentNode, string sourceFile) : base(vehicle, componentNode, sourceFile) { }


		public override IAirdragDeclarationInputData AirdragInputData
		{
			get
			{
				if (!ElementExists(XMLNames.Component_AirDrag))
					return null;
				
				return _airdragInputData ?? (_airdragInputData = ComponentReader.AirdragInputData);
			}
		}


		public override IGearboxDeclarationInputData GearboxInputData
		{
			get { return null; }
		}

		public override ITorqueConverterDeclarationInputData TorqueConverterInputData
		{
			get { return null; }
		}

		public override IAxleGearInputData AxleGearInputData
		{
			get { return null; }
		}

		public override IAngledriveInputData AngledriveInputData
		{
			get { return null; }
		}

		public override IEngineDeclarationInputData EngineInputData
		{
			get { return null; }
		}

		public override IRetarderInputData RetarderInputData
		{
			get { return null; }
		}

		public override IPTOTransmissionInputData PTOTransmissionInputData
		{
			get { return null; }
		}

		public override IAxlesDeclarationInputData AxleWheels
		{
			get { return null; }
		}


		public override IBusAuxiliariesDeclarationData BusAuxiliaries
		{
			get
			{
				if (!ElementExists(XMLNames.Component_Auxiliaries))
					return null;

				return _busAuxiliaries ?? (_busAuxiliaries = GetBusAuxiliaries());
			}
		}

		private IBusAuxiliariesDeclarationData GetBusAuxiliaries()
		{
			var busAux = ComponentReader.BusAuxiliariesInputData;

			if (busAux.ElectricConsumers == null && busAux.HVACAux == null &&
				busAux.PneumaticConsumers == null && busAux.PneumaticSupply == null &&
				busAux.ElectricSupply == null && busAux.FanTechnology == null &&
				busAux.SteeringPumpTechnology == null)
				return null;
			return busAux;
		}
	}
}