using System.Xml;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.InputData.FileIO.XML.Engineering.Interfaces;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Engineering.DataProvider
{
	internal class XMLEngineeringVehicleComponentsDataProviderV07 : AbstractEngineeringXMLComponentDataProvider, IXMLEngineeringVehicleComponentsData
	{
		public const string NAMESPACE_URI = XMLDefinitions.ENGINEERING_DEFINITONS_NAMESPACE_V07;

		private IAirdragEngineeringInputData _airdragInputData;
		private IGearboxEngineeringInputData _gearboxInputData;
		private IAxleGearInputData _axleGearInputData;
		private IAngledriveInputData _angledriveInputData;
		private IEngineEngineeringInputData _engineInputData;
		private IRetarderInputData _retarderInputData;
		private IAuxiliariesEngineeringInputData _auxInputData;
		private IAxlesEngineeringInputData _axleWheels;

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

		protected override string SchemaNamespace { get { return NAMESPACE_URI; } }

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
			get { return GearboxInputData.TorqueConverter; }
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
		public new const string NAMESPACE_URI = XMLDefinitions.ENGINEERING_DEFINITONS_NAMESPACE_V10;

		public XMLEngineeringVehicleComponentsDataProviderV10(IXMLEngineeringVehicleData vehicle, XmlNode baseNode, string source) : base(vehicle, baseNode, source) { }

		protected override string SchemaNamespace { get { return NAMESPACE_URI; } }

	}
}
