using System.Xml;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.InputData.FileIO.XML.Engineering.Interfaces;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Engineering.DataProvider
{
	internal class XMLEngineeringVehicleComponentsDataProviderV07 : AbstractEngineeringXMLComponentDataProvider, IXMLEngineeringVehicleComponentsData
	{
		public static readonly XNamespace NAMESPACE_URI = XMLDefinitions.ENGINEERING_DEFINITONS_NAMESPACE_V07;

		public const string XSD_TYPE = "VehicleComponentsType";

		public static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI, XSD_TYPE);

		protected IAirdragEngineeringInputData _airdragInputData;
		protected IGearboxEngineeringInputData _gearboxInputData;
		protected IAxleGearInputData _axleGearInputData;
		protected IAngledriveInputData _angledriveInputData;
		protected IEngineEngineeringInputData _engineInputData;
		protected IRetarderInputData _retarderInputData;
		protected IAuxiliariesEngineeringInputData _auxInputData;
		protected IAxlesEngineeringInputData _axleWheels;
		protected ITorqueConverterEngineeringInputData _torqueConverterInputData;

		public XMLEngineeringVehicleComponentsDataProviderV07(
			IXMLEngineeringVehicleData vehicle, XmlNode baseNode, string source) : base(vehicle, baseNode, source)
		{
			SourceType = DataSourceType.XMLEmbedded;
		}

		#region Implementation of IVehicleComponentsEngineering

		public override DataSource DataSource
		{
			get { return ((IXMLResource)Vehicle).DataSource; }
		}

		public IXMLComponentsReader ComponentReader { protected get; set; }

		protected override XNamespace SchemaNamespace { get { return NAMESPACE_URI; } }

		protected override DataSourceType SourceType { get; }

		public virtual IAirdragEngineeringInputData AirdragInputData
		{
			get { return _airdragInputData ?? (_airdragInputData = ComponentReader.AirdragInputData); }
		}

		public virtual IGearboxEngineeringInputData GearboxInputData
		{
			get { return _gearboxInputData ?? (_gearboxInputData = ComponentReader.GearboxData); }
		}
		public virtual ITorqueConverterEngineeringInputData TorqueConverterInputData
		{
			get { return _torqueConverterInputData ?? (_torqueConverterInputData = ComponentReader.TorqueConverter); }
		}

		public virtual IAxleGearInputData AxleGearInputData
		{
			get { return _axleGearInputData ?? (_axleGearInputData = ComponentReader.AxleGearInputData); }
		}
		
		public virtual IAngledriveInputData AngledriveInputData
		{
			get { return _angledriveInputData ?? (_angledriveInputData = ComponentReader.AngularGearInputData); }
		}

		public virtual IEngineEngineeringInputData EngineInputData
		{
			get { return _engineInputData ?? (_engineInputData = ComponentReader.EngineInputData); }
		}

		public virtual IAuxiliariesEngineeringInputData AuxiliaryInputData
		{
			get { return _auxInputData ?? (_auxInputData = ComponentReader.AuxiliaryData); }
		}

		public virtual IRetarderInputData RetarderInputData
		{
			get { return _retarderInputData ?? (_retarderInputData = ComponentReader.RetarderInputData); }
		}

		public IPTOTransmissionInputData PTOTransmissionInputData { get { return Vehicle; } }

		public IAxlesEngineeringInputData AxleWheels
		{
			get { return _axleWheels ?? (_axleWheels = ComponentReader.AxlesEngineeringInputData); }
		}

		#endregion

		#region Implementation of IXMLResource

		
		#endregion

	}

	internal class XMLEngineeringVehicleComponentsDataProviderV10 : XMLEngineeringVehicleComponentsDataProviderV07
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.ENGINEERING_DEFINITONS_NAMESPACE_V10;

		public new const string XSD_TYPE = "VehicleComponentsType";

		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI, XSD_TYPE);

		public XMLEngineeringVehicleComponentsDataProviderV10(IXMLEngineeringVehicleData vehicle, XmlNode baseNode, string source) : base(vehicle, baseNode, source) { }

		protected override XNamespace SchemaNamespace { get { return NAMESPACE_URI; } }

	}
}
