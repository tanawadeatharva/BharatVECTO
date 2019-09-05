using System.Xml;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.XML.Common;
using TUGraz.VectoCore.InputData.FileIO.XML.Engineering.Interfaces;
using TUGraz.VectoCore.InputData.FileIO.XML.Engineering.Reader;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Engineering.DataProvider
{
	internal class XMLEngineeringDriverDataProviderV07 : AbstractCommonComponentType, IXMLEngineeringDriverData
	{
		public static readonly XNamespace NAMESPACE_URI = XMLDefinitions.ENGINEERING_DEFINITONS_NAMESPACE_V07;

		public const string XSD_TYPE = "DriverModelType";

		public static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI, XSD_TYPE);

		private ILookaheadCoastingInputData _lookahead;
		private IOverSpeedEngineeringInputData _overspeed;
		private IXMLDriverAcceleration _accCurve;
		private IGearshiftEngineeringInputData _shiftParameters;

		public XMLEngineeringDriverDataProviderV07(
			IXMLEngineeringInputData inputData,
			XmlNode driverDataNode, string fsBasePath)
			: base(driverDataNode, fsBasePath)
		{
			SourceType = (inputData as IXMLResource).DataSource.SourceFile == fsBasePath
				? DataSourceType.XMLEmbedded
				: DataSourceType.XMLFile;
		}

		public IXMLDriverDataReader Reader { protected get; set; }

		public virtual IDriverAccelerationData AccelerationCurve
		{
			get { return (_accCurve ?? (_accCurve = Reader.AccelerationCurveData)).AccelerationCurve; }
		}

		public virtual ILookaheadCoastingInputData Lookahead
		{
			get { return _lookahead ?? (_lookahead = Reader.LookAheadData); }
		}

		public virtual IGearshiftEngineeringInputData GearshiftInputData
		{
			get { return _shiftParameters ?? (_shiftParameters = Reader.ShiftParameters); }
		}

		public virtual IEngineStopStartEngineeringInputData EngineStopStartData { get { return null; } }


		public virtual IEcoRollEngineeringInputData EcoRollData
		{
			get { return null; }
		}

		public virtual IPCCEngineeringInputData PCCData
		{
			get { return null; }
		}

		public virtual IOverSpeedEngineeringInputData OverSpeedData
		{
			get { return _overspeed ?? (_overspeed = Reader.OverspeedData); }
		}

		#region Overrides of AbstractXMLResource

		protected override XNamespace SchemaNamespace
		{
			get { return NAMESPACE_URI; }
		}

		protected override DataSourceType SourceType { get; }

		#endregion
	}

	internal class XMLEngineeringDriverDataProviderV10 : XMLEngineeringDriverDataProviderV07
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.ENGINEERING_DEFINITONS_NAMESPACE_V10;

		public new const string XSD_TYPE = "DriverModelEngineeringType";

		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI, XSD_TYPE);

		protected IEngineStopStartEngineeringInputData _engineStopStart;

		protected IEcoRollEngineeringInputData _ecoRollData;

		protected IPCCEngineeringInputData _pccData;


		public XMLEngineeringDriverDataProviderV10(
			IXMLEngineeringInputData inputData, XmlNode driverDataNode, string fsBasePath) : base(
			inputData, driverDataNode, fsBasePath) { }


		protected override XNamespace SchemaNamespace
		{
			get { return NAMESPACE_URI; }
		}

		public override IEngineStopStartEngineeringInputData EngineStopStartData
		{
			get { return _engineStopStart ?? (_engineStopStart = Reader.EngineStopStartData); }
		}

		public override IEcoRollEngineeringInputData EcoRollData
		{
			get { return _ecoRollData ?? (_ecoRollData = Reader.EcoRollData); }
		}

		
		public override IPCCEngineeringInputData PCCData
		{
			get { return _pccData ?? (_pccData = Reader.PCCData); }
		}
	}

	
}
