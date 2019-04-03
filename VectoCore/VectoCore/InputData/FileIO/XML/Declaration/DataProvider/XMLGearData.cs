using System.Xml;
using TUGraz.IVT.VectoXML;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider
{
	public abstract class XMLAbstractGearData : AbstractXMLType
	{
		
		protected TableData _lossmap;

		protected XMLAbstractGearData(XmlNode gearNode, string sourceFile) : base(gearNode)
		{
			SourceFile = sourceFile;
		}

		public string SourceFile { get; }

		#region Implementation of ITransmissionInputData

		public virtual int Gear
		{
			get {
				return XmlConvert.ToUInt16(
					BaseNode.Attributes?.GetNamedItem(XMLNames.Gearbox_Gear_GearNumber_Attr).InnerText ?? "0");
			}
		}

		public virtual double Ratio
		{
			get { return GetString(XMLNames.Gearbox_Gear_Ratio).ToDouble(double.NaN); }
		}

		public virtual TableData LossMap
		{
			get {
				return _lossmap ?? (_lossmap = XMLHelper.ReadTableData(
					AttributeMappings.TransmissionLossmapMapping,
					GetNodes(new[] { XMLNames.Gearbox_Gear_TorqueLossMap, XMLNames.Gearbox_Gear_TorqueLossMap_Entry })));
			}
		}

		public virtual double Efficiency
		{
			get { return double.NaN; }
		}

		public virtual NewtonMeter MaxTorque
		{
			get { return GetNode(XMLNames.Gearbox_Gears_MaxTorque, required: false)?.InnerText.ToDouble().SI<NewtonMeter>(); }
		}

		public virtual PerSecond MaxInputSpeed
		{
			get { return GetNode(XMLNames.Gearbox_Gear_MaxSpeed, required: false)?.InnerText.ToDouble().RPMtoRad(); }
		}

		public virtual TableData ShiftPolygon
		{
			get { return null; }
		}

		

		#endregion
	}

	public class XMLGearDataV10 : XMLAbstractGearData, IXMLGearData
	{
		public const string NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V10;

		protected DataSource _dataSource;

		public XMLGearDataV10(XmlNode gearNode, string sourceFile) : base(gearNode, sourceFile) { }

		public virtual DataSource DataSource
		{
			get { return _dataSource ?? (_dataSource = new DataSource() { SourceFile = SourceFile, SourceType = DataSourceType.XMLEmbedded, SourceVersion = XMLHelper.GetVersionFromNamespaceUri(NAMESPACE_URI) }); }
		}
	}
}
