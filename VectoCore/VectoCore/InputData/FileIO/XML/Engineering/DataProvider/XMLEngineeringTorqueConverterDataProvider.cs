using System.Xml;
using TUGraz.IVT.VectoXML;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.XML.Engineering.DataProvider;
using TUGraz.VectoCore.InputData.FileIO.XML.Engineering.Interfaces;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Engineering.Impl
{
	internal class XMLEngineeringTorqueConverterDataProviderV07 : AbstractEngineeringXMLComponentDataProvider,
		IXMLTorqueconverterData
	{
		public const string NAMESPACE_URI = XMLDefinitions.ENGINEERING_DEFINITONS_NAMESPACE_V07;

		public XMLEngineeringTorqueConverterDataProviderV07(
			IXMLEngineeringVehicleData vehicle, XmlNode node, string source) : base(vehicle, node, source)
		{
			SourceType = (vehicle as IXMLResource).DataSource.SourceFile == source ? DataSourceType.XMLEmbedded : DataSourceType.XMLFile;
		}

		#region Implementation of ITorqueConverterDeclarationInputData

		public virtual TableData TCData
		{
			get {
				return XMLHelper.ReadEntriesOrResource(
					BaseNode, DataSource.SourcePath, XMLNames.TorqueConverter_Characteristics, XMLNames.TorqueConverter_Characteristics_Entry,
					AttributeMappings.TorqueConverterDataMapping);
			}
		}

		#endregion

		#region Implementation of ITorqueConverterEngineeringInputData

		public virtual PerSecond ReferenceRPM
		{
			get {
				return GetNode(XMLNames.TorqueConverter_ReferenceRPM)?.InnerText.ToDouble().RPMtoRad() ??
						DeclarationData.TorqueConverter.ReferenceRPM;
			}
		}

		public virtual KilogramSquareMeter Inertia
		{
			get {
				return GetNode(XMLNames.TorqueConverter_Inertia)?.InnerText.ToDouble().SI<KilogramSquareMeter>() ??
						0.SI<KilogramSquareMeter>();
			}
		}

		public virtual TableData ShiftPolygon
		{
			get {
				return XMLHelper.ReadEntriesOrResource(
					BaseNode, DataSource.SourcePath, XMLNames.TorqueConverter_ShiftPolygon, XMLNames.TorqueConverter_ShiftPolygon_Entry,
					AttributeMappings.ShiftPolygonMapping);
			}
		}

		public virtual PerSecond MaxInputSpeed
		{
			get { return GetNode(XMLNames.TorqueConverter_MaxInputSpeed)?.InnerText.ToDouble().RPMtoRad(); }
		}

		#endregion

		#region Overrides of AbstractXMLResource

		protected override string SchemaNamespace { get { return NAMESPACE_URI; } }
		protected override DataSourceType SourceType { get; }

		#endregion
	}

	internal class XMLEngineeringTorqueConverterDataProviderV10 : XMLEngineeringTorqueConverterDataProviderV07
	{
		public new const string NAMESPACE_URI = XMLDefinitions.ENGINEERING_DEFINITONS_NAMESPACE_V10;

		public XMLEngineeringTorqueConverterDataProviderV10(IXMLEngineeringVehicleData vehicle, XmlNode node, string source) :
			base(vehicle, node, source) { }

		#region Overrides of XMLEngineeringTorqueConverterDataProviderV07

		protected override string SchemaNamespace { get { return NAMESPACE_URI; } }

		#endregion
	}
}
