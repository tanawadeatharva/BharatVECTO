using System;
using System.Xml;
using Ninject;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.InputData.FileIO.XML.Engineering.DataProvider;
using TUGraz.VectoCore.InputData.FileIO.XML.Engineering.Factory;
using TUGraz.VectoCore.InputData.FileIO.XML.Engineering.Interfaces;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Engineering.Reader.Impl {
	internal class XMLDriverDataReaderV07 : IXMLDriverDataReader
	{
		public const string NAMESPACE_URI = XMLDefinitions.ENGINEERING_DEFINITONS_NAMESPACE_V07;

		public const string XSD_TYPE = "DriverModelType";

		public static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI, XSD_TYPE);

		protected XmlNode DriverDataNode;
		protected IXMLEngineeringDriverData DriverData;

		[Inject]
		public IEngineeringInjectFactory Factory { protected get; set; }

		public XMLDriverDataReaderV07(IXMLEngineeringDriverData driverData, XmlNode driverDataNode)
		{
			DriverData = driverData;
			DriverDataNode = driverDataNode;
		}

		public ILookaheadCoastingInputData LookAheadData
		{
			get {
				return CreateData(
					XMLNames.DriverModel_LookAheadCoasting, (version, node) => Factory.CreateLookAheadData(version, DriverData, node));
			}
		}

		public IOverSpeedEngineeringInputData OverspeedData
		{
			get {
				return CreateData(
					XMLNames.DriverModel_Overspeed, (version, node) => Factory.CreateOverspeedData(version, DriverData, node));
			}
		}

		
		public IEcoRollEngineeringInputData EcoRollData
		{
			get {
				return CreateData(
					XMLNames.DriverModel_EcoRoll,
					(version, node) => version == null ? null : Factory.CreateEcoRollData(version, DriverData, node), false);
			}
		}

		public IPCCEngineeringInputData PCCData
		{
			get {
				return CreateData(
					"PCCParameters",
					(version, node) => version == null ? null : Factory.CreatePCCData(version, DriverData, node), false);
			}
		}

		public IXMLDriverAcceleration AccelerationCurveData
		{
			get {
				return CreateData(
					XMLNames.DriverModel_DriverAccelerationCurve,
					(version, node) => version == null
						? DefaultAccelerationCurve()
						: Factory.CreateAccelerationCurveData(version, DriverData, node), false);
			}
		}

		public IGearshiftEngineeringInputData ShiftParameters
		{
			get {
				return CreateData(
					XMLNames.DriverModel_ShiftStrategyParameters, ShiftParametersCreator, false);
			}
		}

		public IEngineStopStartEngineeringInputData EngineStopStartData
		{
			get {
				return CreateData(
					XMLNames.DriverModel_EngineStopStartParameters, 
					(version, node) => version == null ? null : Factory.CreateEngineStopStartData(version, DriverData, node), false);
			}
		}

		private IGearshiftEngineeringInputData ShiftParametersCreator(string version, XmlNode node)
		{
			if (version == null) {
				return new XMLEngineeringGearshiftDataV07(null);
			}
			return Factory.CreateShiftParametersData(version, node);
		}

		protected virtual T CreateData<T>(string elementName, Func<string, XmlNode, T> creator, bool required = true)
		{
			var node = GetNode(elementName, required);
			if (!required && node == null) {
				try {
					return creator(null, node);
				} catch (Exception e) {
					throw new VectoException("Failed to create dummy data provider", e);
				}
			}

			var version = XMLHelper.GetXsdType(node.SchemaInfo.SchemaType);
			try {
				return creator(version, node);
			} catch (Exception e) {
				throw new VectoException("Unsupported XML Version! Node: {0} Version: {1}", e, node.LocalName, version);
			}
		}

		
		protected virtual XmlNode GetNode(string elementName, bool required = true)
		{
			var retVal =
				DriverDataNode.SelectSingleNode(XMLHelper.QueryLocalName(elementName));
			if (required && retVal == null) {
				throw new VectoException("Element {0} not found!", elementName);
			}

			return retVal;
		}

		private static IXMLDriverAcceleration DefaultAccelerationCurve()
		{
			try {
				var resourceName = DeclarationData.DeclarationDataResourcePrefix + ".VACC.Truck" +
									Constants.FileExtensions.DriverAccelerationCurve;
				return new XMLDriverAccelerationV07(
					VectoCSVFile.ReadStream(RessourceHelper.ReadStream(resourceName), source: resourceName));
			} catch (Exception e) {
				throw new VectoException("Failed to read Driver Acceleration Curve: " + e.Message, e);
			}
		}
	}
	
	internal class XMLDriverDataReaderV10 : XMLDriverDataReaderV07
	{
		public new const string NAMESPACE_URI = XMLDefinitions.ENGINEERING_DEFINITONS_NAMESPACE_V10;

		public new const string XSD_TYPE = "DriverModelEngineeringType";

		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI, XSD_TYPE);


		public XMLDriverDataReaderV10(IXMLEngineeringDriverData driverData, XmlNode driverDataNode) : base(driverData, driverDataNode) { }
	}

}