using System;
using System.Xml;
using Ninject;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.InputData.FileIO.XML.Engineering.DataProvider;
using TUGraz.VectoCore.InputData.FileIO.XML.Engineering.Factory;
using TUGraz.VectoCore.InputData.FileIO.XML.Engineering.Impl;
using TUGraz.VectoCore.InputData.FileIO.XML.Engineering.Interfaces;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Engineering.Reader
{
	internal class XMLComponentsEngineeringReaderV07 : AbstractExternalResourceReader, IXMLComponentsReader
	{
		public const string NAMESPACE_URI = XMLDefinitions.ENGINEERING_DEFINITONS_NAMESPACE_V07;

		//protected string Version;
		protected IXMLEngineeringVehicleData Vehicle;

		protected IAxleGearInputData _axleGearInputData;
		protected IAngledriveInputData _angularGearInputData;
		protected IEngineEngineeringInputData _engineInputData;
		protected IRetarderInputData _retarderInputData;
		protected IAuxiliariesEngineeringInputData _auxiliaryData;
		protected IGearboxEngineeringInputData _gearboxData;
		protected IAirdragEngineeringInputData _airdragInputData;
		protected ITorqueConverterEngineeringInputData _torqueConverterInputData;
		protected IAxlesEngineeringInputData _axlesInputData;
		protected ITyreEngineeringInputData _tyre;
		protected IVehicleComponentsEngineering _components;

		[Inject]
		public IEngineeringInjectFactory Factory { protected get; set; }

		public XMLComponentsEngineeringReaderV07(IXMLEngineeringVehicleData vehicle, XmlNode componentsNode, bool verifyXML) :
			base(vehicle, componentsNode, verifyXML)
		{
			if (componentsNode == null) {
				throw new VectoException("component node must not be null!");
			}

			BaseNode = componentsNode;
			Vehicle = vehicle;
		}

		#region Implementation of IEngineeringComponentsFactory

		public virtual IAxleGearInputData AxleGearInputData
		{
			get {
				return _axleGearInputData ?? (_axleGearInputData = CreateComponent(XMLNames.Component_Axlegear, AxlegearCreator));
			}
		}

		public virtual IAngledriveInputData AngularGearInputData
		{
			get {
				return _angularGearInputData ?? (_angularGearInputData = CreateComponent(
							XMLNames.Component_Angledrive, AngledriveCreator, false, true));
			}
		}

		public virtual IEngineEngineeringInputData EngineInputData
		{
			get { return _engineInputData ?? (_engineInputData = CreateComponent(XMLNames.Component_Engine, EngineCreator)); }
		}

		public virtual IRetarderInputData RetarderInputData
		{
			get {
				return _retarderInputData ?? (_retarderInputData = CreateComponent(
							XMLNames.Component_Retarder, RetarderCreator, false, true));
			}
		}

		public virtual IAuxiliariesEngineeringInputData AuxiliaryData
		{
			get {
				return _auxiliaryData ?? (_auxiliaryData = CreateComponent(XMLNames.Component_Auxiliaries, AuxiliariesCreator));
			}
		}

		public virtual IGearboxEngineeringInputData GearboxData
		{
			get { return _gearboxData ?? (_gearboxData = CreateComponent(XMLNames.Component_Gearbox, GearboxCreator)); }
		}

		public virtual IPTOTransmissionInputData PTOData
		{
			get { return Vehicle; }
		}

		public virtual IAirdragEngineeringInputData AirdragInputData
		{
			get {
				return _airdragInputData ?? (_airdragInputData = CreateComponent(XMLNames.Component_AirDrag, AirdragCreator));
			}
		}

		public virtual IAxlesEngineeringInputData AxlesEngineeringInputData
		{
			get { return _axlesInputData ?? (_axlesInputData = CreateComponent(XMLNames.Component_AxleWheels, AxlesCreator)); }
		}

		public virtual ITorqueConverterEngineeringInputData TorqueConverter
		{
			get {
				return _torqueConverterInputData ?? (_torqueConverterInputData = CreateComponent(
							XMLNames.Component_TorqueConverter, TorqueConverterCreator, true, true));
			}
		}

		public virtual ITyreEngineeringInputData Tyre
		{
			get { return _tyre ?? (_tyre = CreateComponent(XMLNames.AxleWheels_Axles_Axle_Tyre, TyreCreator)); }
		}

		public virtual IVehicleComponentsEngineering ComponentInputData { get {
			return _components ?? (_components = CreateComponent(XMLNames.Vehicle_Components, ComponentsCreator));
		} }




		public IAxleEngineeringInputData CreateAxle(XmlNode axleNode)
		{
			var version = XMLHelper.GetSchemaVersion(axleNode);
			try {
				var axle = Factory.CreateAxleData(version, axleNode, Vehicle);
				axle.Reader = Factory.CreateAxleReader(version, Vehicle, axleNode, VerifyXML);
				return axle;
			} catch (Exception e) {
				var axleNumber = axleNode.Attributes?.GetNamedItem(XMLNames.AxleWheels_Axles_Axle_AxleNumber_Attr).InnerText;
				throw new VectoException(
					"Unsupported XML Version! Node: {0} Axle: {1} Version: {2}", e, axleNode.LocalName, axleNumber, version);
			}
		}

		public ITransmissionInputData CreateGear(XmlNode gearNode)
		{
			var version = XMLHelper.GetSchemaVersion(gearNode);
			try {
				return Factory.CreateGearData(version, gearNode, (Vehicle as IXMLResource).DataSource.SourcePath);
			} catch (Exception e) {
				var gearNumber = gearNode.Attributes?.GetNamedItem(XMLNames.Gearbox_Gear_GearNumber_Attr).InnerText;
				throw new VectoException(
					"Unsupported XML Version! Node: {0} Gear: {1} Version: {2}", e, gearNode.LocalName, gearNumber, version);
			}
		}

		public IAuxiliaryEngineeringInputData Create(XmlNode auxNode)
		{
			var version = XMLHelper.GetSchemaVersion(auxNode);
			try {
				return Factory.CreateAuxData(version, auxNode, (Vehicle as IXMLResource).DataSource.SourcePath);
			} catch (Exception e) {
				throw new VectoException("Unsupported XML Version! Node: {0} Version: {2}", e, auxNode.LocalName, version);
			}
		}

		#endregion

		#region FactoryMethods

		protected IVehicleComponentsEngineering ComponentsCreator(string version, XmlNode componentNode, string sourcefile)
		{
			var components = Factory.CreateComponentData(version, Vehicle, componentNode, sourcefile);
			components.ComponentReader = Factory.CreateComponentReader(version, Vehicle, componentNode, VerifyXML);
			return components;
		}

		protected virtual IAxleGearInputData AxlegearCreator(string version, XmlNode componentNode, string sourceFile)
		{
			return Factory.CreateAxlegearData(version, Vehicle, componentNode, sourceFile);
		}

		protected IAngledriveInputData AngledriveCreator(string version, XmlNode componentNode, string sourceFile)
		{
			if (version == null) {
				return new XMLEngineeringAngledriveDataProviderV07(Vehicle, componentNode, sourceFile);
			}

			return Factory.CreateAngledriveData(version, Vehicle, componentNode, sourceFile);
		}

		protected virtual IEngineEngineeringInputData EngineCreator(string version, XmlNode componentNode, string sourceFile)
		{
			return Factory.CreateEngineData(version, Vehicle, componentNode, sourceFile);
		}

		protected virtual IRetarderInputData RetarderCreator(string version, XmlNode componentNode, string sourceFile)
		{
			if (version == null) {
				return new XMLEngineeringRetarderDataProviderV07(Vehicle, componentNode, sourceFile);
			}

			return Factory.CreateRetarderData(version, Vehicle, componentNode, sourceFile);
		}

		protected virtual IAuxiliariesEngineeringInputData AuxiliariesCreator(
			string version, XmlNode componentNode, string sourceFile)
		{
			var aux = Factory.CreateAuxiliariesData(version, Vehicle, componentNode, sourceFile);
			aux.Reader = Factory.CreateComponentReader(version, Vehicle, componentNode, VerifyXML);
			return aux;
		}

		protected virtual IGearboxEngineeringInputData GearboxCreator(
			string version, XmlNode componentNode, string sourceFile)
		{
			var gbx = Factory.CreateGearboxData(version, Vehicle, componentNode, sourceFile);
			gbx.Reader = Factory.CreateComponentReader(version, Vehicle, componentNode, VerifyXML);
			return gbx;
		}

		protected virtual IAirdragEngineeringInputData AirdragCreator(
			string version, XmlNode componentNode, string sourceFile)
		{
			return Factory.CreateAirdragData(version, Vehicle, componentNode, sourceFile);
		}

		protected ITorqueConverterEngineeringInputData TorqueConverterCreator(
			string version, XmlNode componentNode, string sourceFile)
		{
			if (version == null) {
				return new XMLEngineeringTorqueConverterDataProviderV07(Vehicle, componentNode, sourceFile);
			}

			return Factory.CreateTorqueconverterData(version, Vehicle, componentNode, sourceFile);
		}

		protected IAxlesEngineeringInputData AxlesCreator(string version, XmlNode componentNode, string sourceFile)
		{
			var axles = Factory.CreateAxlesData(version, Vehicle, componentNode, sourceFile);
			axles.Reader = Factory.CreateComponentReader(version, Vehicle, componentNode, VerifyXML);
			return axles;
		}

		protected ITyreEngineeringInputData TyreCreator(string version, XmlNode componentNode, string sourceFile)
		{
			return Factory.CreateTyre(version, Vehicle, componentNode, sourceFile);
		}

		#endregion
	}

	internal class XMLComponentsEngineeringReaderV10 : XMLComponentsEngineeringReaderV07
	{
		public new const string NAMESPACE_URI = XMLDefinitions.ENGINEERING_DEFINITONS_NAMESPACE_V10;

		public XMLComponentsEngineeringReaderV10(IXMLEngineeringVehicleData vehicle, XmlNode componentsNode, bool verifyXML) :
			base(vehicle, componentsNode, verifyXML) { }
	}

	internal class XMLComponentsEngineeringReaderV10TEST : XMLComponentsEngineeringReaderV10
	{
		public new const string NAMESPACE_URI = XMLDefinitions.ENGINEERING_DEFINITONS_NAMESPACE_V10_TEST;

		public XMLComponentsEngineeringReaderV10TEST(IXMLEngineeringVehicleData vehicle, XmlNode componentsNode, bool verifyXML) : base(vehicle, componentsNode, verifyXML) { }

		public override ITyreEngineeringInputData Tyre
		{
			get { return _tyre ?? (_tyre = CreateComponent("Tire", TyreCreator)); }
		}
	}
}
