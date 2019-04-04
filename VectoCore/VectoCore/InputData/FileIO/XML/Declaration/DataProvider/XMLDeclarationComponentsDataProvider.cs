using System.Xml;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Reader;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider
{
	public class XMLDeclarationComponentsDataProviderV10 : AbstractCommonComponentType, IXMLVehicleComponentsDeclaration
	{
		public const string NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V10;


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


		public XMLDeclarationComponentsDataProviderV10(IXMLDeclarationVehicleData vehicle, XmlNode componentNode, string sourceFile) :
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
			get { return GearboxInputData.TorqueConverter; }
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

		#endregion

		#region Implementation of IXMLVehicleComponentsDeclaration

		public virtual IXMLComponentReader ComponentReader { protected get; set; }
		
		
		#endregion


		#region Overrides of AbstractXMLResource

		protected override string SchemaNamespace
		{
			get { return NAMESPACE_URI; }
		}

		protected override DataSourceType SourceType { get; }

		#endregion
	}
}
