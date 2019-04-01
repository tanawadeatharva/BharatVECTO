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

		public IAirdragDeclarationInputData AirdragInputData
		{
			get { return _airdragInputData ?? (_airdragInputData = ComponentReader.AirdragInputData); }
		}

		public IGearboxDeclarationInputData GearboxInputData
		{
			get { return _gearboxInputData ?? (_gearboxInputData = ComponentReader.GearboxInputData); }
		}

		public ITorqueConverterDeclarationInputData TorqueConverterInputData
		{
			get { return GearboxInputData.TorqueConverter; }
		}

		public IAxleGearInputData AxleGearInputData
		{
			get { return _axleGearInputData ?? (_axleGearInputData = ComponentReader.AxleGearInputData); }
		}

		public IAngledriveInputData AngledriveInputData
		{
			get { return _angledriveInputData ?? (_angledriveInputData = ComponentReader.AngledriveInputData); }
		}

		public IEngineDeclarationInputData EngineInputData
		{
			get { return _engineInputData ?? (_engineInputData = ComponentReader.EngineInputData); }
		}

		IAuxiliariesDeclarationInputData IVehicleComponentsDeclaration.AuxiliaryInputData
		{
			get { return _auxInputData ?? (_auxInputData = ComponentReader.AuxiliaryData); }
		}

		public IRetarderInputData RetarderInputData
		{
			get { return _retarderInputData ?? (_retarderInputData = ComponentReader.RetarderInputData); }
		}

		public IPTOTransmissionInputData PTOTransmissionInputData
		{
			get { return _vehicle.PTOTransmissionInputData; }
		}

		public IAxlesDeclarationInputData AxleWheels
		{
			get { return _axleWheels ?? (_axleWheels = ComponentReader.AxlesDeclarationInputData); }
		}

		#endregion

		#region Implementation of IXMLVehicleComponentsDeclaration

		public IXMLComponentReader ComponentReader { protected get; set; }
		
		
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
