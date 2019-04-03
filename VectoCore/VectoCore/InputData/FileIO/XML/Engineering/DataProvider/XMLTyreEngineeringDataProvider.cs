using System.Xml;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.XML.Engineering.Interfaces;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Engineering.DataProvider
{
	internal class XMLTyreEngineeringDataProviderV10 : AbstractEngineeringXMLComponentDataProvider, IXMLTyreData
	{
		public const string NAMESPACE_URI = XMLDefinitions.ENGINEERING_DEFINITONS_NAMESPACE_V10;

		public XMLTyreEngineeringDataProviderV10(IXMLEngineeringVehicleData vehicle, XmlNode baseNode, string source) :
			base(vehicle, baseNode, source)
		{
			SourceType = (vehicle as IXMLResource).DataSource.SourceFile == source ? DataSourceType.XMLEmbedded : DataSourceType.XMLFile;
		}

		#region Implementation of ITyreDeclarationInputData

		public virtual string Dimension
		{
			get { return GetNode(XMLNames.AxleWheels_Axles_Axle_Dimension, required:false)?.InnerText; }
		}

		public virtual double RollResistanceCoefficient
		{
			get { return GetNode(XMLNames.AxleWheels_Axles_Axle_RRCISO, required: false)?.InnerText.ToDouble() ?? double.NaN; }
		}

		public virtual Newton TyreTestLoad
		{
			get { return GetNode(XMLNames.AxleWheels_Axles_Axle_FzISO, required: false)?.InnerText.ToDouble().SI<Newton>(); }
		}

		#endregion

		#region Implementation of ITyreEngineeringInputData

		public virtual KilogramSquareMeter Inertia
		{
			get { return GetNode(XMLNames.AxleWheels_Axles_Axle_Inertia, required: false)?.InnerText.ToDouble().SI<KilogramSquareMeter>(); }
		}

		public virtual Meter DynamicTyreRadius
		{
			get {
				return GetNode(XMLNames.AxleWheels_Axles_Axle_DynamicTyreRadius, required: false)?.InnerText.ToDouble().SI(Unit.SI.Milli.Meter).Cast<Meter>();
			}
		}

		#endregion

		#region Overrides of AbstractXMLResource

		protected override string SchemaNamespace { get { return NAMESPACE_URI; } }

		protected override DataSourceType SourceType { get; }

		#endregion
	}

	internal class XMLTyreEngineeringDataProviderV10TEST : XMLTyreEngineeringDataProviderV10
	{
		public new const string NAMESPACE_URI = XMLDefinitions.ENGINEERING_DEFINITONS_NAMESPACE_V10_TEST;

		public XMLTyreEngineeringDataProviderV10TEST(IXMLEngineeringVehicleData vehicle, XmlNode baseNode, string source) : base(vehicle, baseNode, source) { }

		protected override string SchemaNamespace { get { return NAMESPACE_URI; } }
	}
}
