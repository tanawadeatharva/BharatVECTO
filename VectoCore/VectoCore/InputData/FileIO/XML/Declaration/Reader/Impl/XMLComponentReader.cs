using System;
using System.Xml;
using Ninject;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Factory;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using TUGraz.VectoCore.InputData.FileIO.XML.Engineering.Factory;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Reader.Impl
{
	public class XMLComponentReaderV10 : AbstractComponentReader, IXMLComponentReader
	{
		public const string NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V10;

		protected IXMLDeclarationVehicleData Vehicle;

		private IVehicleComponentsDeclaration _components;
		private IGearboxDeclarationInputData _gearboxInputData;
		private IAxleGearInputData _axlegearInputData;
		private IAngledriveInputData _angledriveInputData;
		private IEngineDeclarationInputData _engineInputData;
		private IRetarderInputData _retarderInputData;
		private IAxlesDeclarationInputData _axlesInputData;
		private IAirdragDeclarationInputData _airdragInputData;
		private IAuxiliariesDeclarationInputData _auxiliaryInputData;
		private ITorqueConverterDeclarationInputData _torqueConverterInputData;


		[Inject]
		public IDeclarationInjectFactory Factory { protected get; set; }

		public XMLComponentReaderV10(IXMLDeclarationVehicleData vehicle, XmlNode componentsNode, bool verifyXML) : base(
			vehicle, componentsNode, verifyXML)
		{
			if (componentsNode == null) {
				throw new VectoException("component node must not be null!");
			}

			Vehicle = vehicle;
		}

		#region Implementation of IXMLComponentReader

		public IVehicleComponentsDeclaration ComponentInputData
		{
			get { return _components ?? (_components = CreateComponent(XMLNames.Vehicle_Components, ComponentsCreator)); }
		}

		public IAirdragDeclarationInputData AirdragInputData
		{
			get {
				return _airdragInputData ?? (_airdragInputData = CreateComponent(XMLNames.Component_AirDrag, AirdragCreator, true));
			}
		}
		
		public IGearboxDeclarationInputData GearboxInputData
		{
			get {
				return _gearboxInputData ?? (_gearboxInputData = CreateComponent(XMLNames.Component_Gearbox, GearboxCreator));
			}
		}

		public ITorqueConverterDeclarationInputData TorqueConverterInputData
		{
			get {
				return _torqueConverterInputData ?? (_torqueConverterInputData = CreateComponent(
							XMLNames.Component_TorqueConverter, TorqueConverterCreator));
			}
		}

		public ITransmissionInputData CreateGear(XmlNode gearNode)
		{
			var version = XMLHelper.GetSchemaVersion(gearNode);
			try {
				return Factory.CreateGearData(version, gearNode);
			} catch (Exception e) {
				var gearNumber = gearNode.Attributes?.GetNamedItem(XMLNames.Gearbox_Gear_GearNumber_Attr).InnerText;
				throw new VectoException(
					"Unsupported XML Version! Node: {0} Gear: {1} Version: {2}", e, gearNode.LocalName, gearNumber, version);
			}
		}

		public IAuxiliaryDeclarationInputData CreateAuxiliary(XmlNode auxNode)
		{
			var version = XMLHelper.GetSchemaVersion(auxNode);
			try {
				return Factory.CreateAuxiliaryData(version, auxNode, Vehicle);
			} catch (Exception e) {
				throw new VectoException("Unsupported XML version! Node: {0} version: {1}", e, auxNode.LocalName, version);
			}
		}

		public IAxleDeclarationInputData CreateAxle(XmlNode axleNode)
		{
			var version = XMLHelper.GetSchemaVersion(axleNode);
			try {
				var axle = Factory.CreateAxleData(version, Vehicle, axleNode, (Vehicle as IXMLResource).DataSource.SourceFile);
				axle.Reader = Factory.CreateComponentReader(version, Vehicle, axleNode, VerifyXML);
				return axle;
			} catch (Exception e) {
				var axleNumber = axleNode.Attributes?.GetNamedItem(XMLNames.AxleWheels_Axles_Axle_AxleNumber_Attr).InnerText;
				throw new VectoException(
					"Unsupported XML Version! Node: {0} Axle: {1} Version: {2}", e, axleNode.LocalName, axleNumber, version);
			}
		}


		public IAxleGearInputData AxleGearInputData
		{
			get {
				return _axlegearInputData ?? (_axlegearInputData = CreateComponent(XMLNames.Component_Axlegear, AxlegearCreator));
			}
		}


		public IAngledriveInputData AngledriveInputData
		{
			get {
				return _angledriveInputData ??
						(_angledriveInputData = CreateComponent(XMLNames.Component_Angledrive, AngledriveCreator, true));
			}
		}

		public IEngineDeclarationInputData EngineInputData
		{
			get { return _engineInputData ?? (_engineInputData = CreateComponent(XMLNames.Component_Engine, EngineCreator)); }
		}


		public IAuxiliariesDeclarationInputData AuxiliaryData
		{
			get {
				return _auxiliaryInputData ??
						(_auxiliaryInputData = CreateComponent(XMLNames.Component_Auxiliaries, AuxiliaryCreator));
			}
		}


		public IRetarderInputData RetarderInputData
		{
			get {
				return _retarderInputData ??
						(_retarderInputData = CreateComponent(XMLNames.Component_Retarder, RetarderCreator, true));
			}
		}


		public IAxlesDeclarationInputData AxlesDeclarationInputData
		{
			get {
				return _axlesInputData ?? (_axlesInputData = CreateComponent(XMLNames.Component_AxleWheels, AxleWheelsCreator));
			}
		}

		public ITyreDeclarationInputData Tyre
		{
			get { return CreateComponent(XMLNames.AxleWheels_Axles_Axle_Tyre, TyreCreator); }
		}

		#endregion

		protected IAirdragDeclarationInputData AirdragCreator(string version, XmlNode componentNode, string sourceFile)
		{
			if (version == null) {
				return new XMLDeclarationAirdragDataProviderV10(Vehicle, null, sourceFile);
			}
			return Factory.CreateAirdragData(version, Vehicle, componentNode, sourceFile);
		}

		protected IGearboxDeclarationInputData GearboxCreator(string version, XmlNode componentNode, string sourceFile)
		{
			var gbx = Factory.CreateGearboxData(version, Vehicle, componentNode, sourceFile);
			gbx.Reader = Factory.CreateComponentReader(version, Vehicle, componentNode, VerifyXML);
			return gbx;
		}

		protected ITorqueConverterDeclarationInputData TorqueConverterCreator(
			string version, XmlNode componentNode, string sourceFile)
		{
			if (version == null) {
				return new XMLDeclarationTorqueConverterDataProviderV10(Vehicle, componentNode, sourceFile);
			}

			return Factory.CreateTorqueconverterData(version, Vehicle, componentNode, sourceFile);
		}

		protected IAxleGearInputData AxlegearCreator(string version, XmlNode componentNode, string sourceFile)
		{
			return Factory.CreateAxlegearData(version, Vehicle, componentNode, sourceFile);
		}

		protected IAngledriveInputData AngledriveCreator(string version, XmlNode componentNode, string sourceFile)
		{
			if (version == null) {
				return new XMLDeclarationAngledriveDataProviderV10(Vehicle, componentNode, sourceFile);
			}

			return Factory.CreateAngledriveData(version, Vehicle, componentNode, sourceFile);
		}

		protected IEngineDeclarationInputData EngineCreator(string version, XmlNode componentNode, string sourceFile)
		{
			return Factory.CreateEngineData(version, Vehicle, componentNode, sourceFile);
		}

		protected IRetarderInputData RetarderCreator(string version, XmlNode componentNode, string sourceFile)
		{
			if (version == null) {
				return new XMLDeclarationRetarderDataProviderV10(Vehicle, componentNode, sourceFile);
			}

			return Factory.CreateRetarderData(version, Vehicle, componentNode, sourceFile);
		}

		protected IAxlesDeclarationInputData AxleWheelsCreator(string version, XmlNode componentNode, string sourceFile)
		{
			var axles = Factory.CreateAxleWheels(version, Vehicle, componentNode, sourceFile);
			axles.Reader = Factory.CreateComponentReader(version, Vehicle, componentNode, VerifyXML);
			return axles;
		}


		protected ITyreDeclarationInputData TyreCreator(string version, XmlNode componentNode, string sourceFile)
		{
			return Factory.CreateTyre(version, Vehicle, componentNode, sourceFile);
		}


		protected IAuxiliariesDeclarationInputData AuxiliaryCreator(string version, XmlNode componentNode, string sourceFile)
		{
			var aux = Factory.CreateAuxiliariesData(version, Vehicle, componentNode, sourceFile);
			aux.Reader = Factory.CreateComponentReader(version, Vehicle, componentNode, VerifyXML);
			return aux;
		}


		protected IVehicleComponentsDeclaration ComponentsCreator(string version, XmlNode componentNode, string sourcefile)
		{
			var components = Factory.CreateComponentData(version, Vehicle, componentNode, sourcefile);
			components.ComponentReader = Factory.CreateComponentReader(version, Vehicle, componentNode, VerifyXML);
			return components;
		}
	}
}
