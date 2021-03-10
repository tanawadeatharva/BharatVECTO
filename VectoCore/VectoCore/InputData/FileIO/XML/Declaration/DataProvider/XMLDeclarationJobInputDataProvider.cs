using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.InputData.FileIO.XML.Common;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Reader;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider
{
	public class XMLDeclarationJobInputDataProviderV10 : AbstractXMLResource, IXMLDeclarationJobInputData
	{
		public static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V10;

		public const string XSD_TYPE = "VectoDeclarationJobType";

		public static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		protected IVehicleDeclarationInputData _vehicle;

		public XMLDeclarationJobInputDataProviderV10(XmlNode node, IXMLDeclarationInputData inputProvider, string fileName) :
			base(node, fileName)
		{
			InputData = inputProvider;
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

		#region Implementation of IDeclarationJobInputData

		public virtual bool SavedInDeclarationMode
		{
			get { return true; }
		}

		public virtual IVehicleDeclarationInputData Vehicle
		{
			get { return _vehicle ?? (_vehicle = Reader.CreateVehicle); }
		}

		public virtual string JobName
		{
			get { return Vehicle.Identifier; }
		}

		public virtual string ShiftStrategy { get { return null; } }

		#endregion

		#region Implementation of IXMLDeclarationJobInputData

		public virtual IXMLJobDataReader Reader { protected get; set; }
		public virtual IXMLDeclarationInputData InputData { get; }

		#endregion
	}

	// ---------------------------------------------------------------------------------------

	public class XMLDeclarationJobInputDataProviderV20 : XMLDeclarationJobInputDataProviderV10
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V20;

		public new const string XSD_TYPE = "VectoDeclarationJobType";

		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		public XMLDeclarationJobInputDataProviderV20(XmlNode node, IXMLDeclarationInputData inputProvider, string fileName) :
			base(node, inputProvider, fileName) { }

		protected override XNamespace SchemaNamespace
		{
			get { return NAMESPACE_URI; }
		}
	}

	// ---------------------------------------------------------------------------------------

	public class XMLDeclarationPrimaryVehicleBusJobInputDataProviderV01 : AbstractXMLResource, IXMLPrimaryVehicleBusJobInputData
	{

		public static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_PRIMARY_BUS_VEHICLE_URI_V01;

		public const string XSD_TYPE = "PrimaryVehicleHeavyBusDataType";

		public static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);


		protected IVehicleDeclarationInputData _vehicle;


		public XMLDeclarationPrimaryVehicleBusJobInputDataProviderV01(XmlNode node, IXMLPrimaryVehicleBusInputData inputProvider,
			string fileName) : base(node, fileName)
		{
			InputData = inputProvider;
		}

		protected override XNamespace SchemaNamespace
		{
			get { return NAMESPACE_URI; }
		}

		public string ShiftStrategy
		{
			get { return null; }
		}

		public bool SavedInDeclarationMode
		{
			get { return true; }
		}

		public string JobName
		{
			get { return Vehicle.Identifier; }
		}
		
		protected override DataSourceType SourceType
		{
			get { return DataSourceType.XMLFile; }
		}

		public IVehicleDeclarationInputData Vehicle
		{
			get { return _vehicle ?? (_vehicle = Reader.CreateVehicle); }
		}

		public IXMLJobDataReader Reader { protected get; set; }
		public IXMLPrimaryVehicleBusInputData InputData { get; }
	}

	public class XMLDeclarationMultistageJobInputDataV01 : AbstractXMLResource, IXMLDeclarationMultistageJobInputData
	{
		public static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_MULTISTAGE_BUS_VEHICLE_NAMESPACE_VO1;

		public const string XSD_TYPE = "VectoOutputMultistageType";

		public static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);
		private IPrimaryVehicleInformationInputDataProvider _primaryVehicle;
		private IList<IManufacturingStageInputData> _manufacturingStages;


		public XMLDeclarationMultistageJobInputDataV01(XmlNode node, IXMLMultistageBusInputDataProvider inputProvider, string fileName) : base(node, fileName)
		{
			InputData = inputProvider;
		}

		public IPrimaryVehicleInformationInputDataProvider PrimaryVehicle
		{
			get { return _primaryVehicle ?? (_primaryVehicle = Reader.PrimaryVehicle); }
		}

		public IList<IManufacturingStageInputData> ManufacturingStages
		{
			get { return _manufacturingStages ?? (_manufacturingStages = Reader.ManufacturingStages); }
		}

		public IXMLMultistageJobReader Reader { get; set; }
		public IXMLMultistageBusInputDataProvider InputData { get; }
		protected override XNamespace SchemaNamespace { get; }
		protected override DataSourceType SourceType { get; }
	}
}
