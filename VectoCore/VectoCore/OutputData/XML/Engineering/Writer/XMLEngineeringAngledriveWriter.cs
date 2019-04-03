using System.IO;
using System.Xml.Linq;
using TUGraz.IVT.VectoXML;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.OutputData.XML.Engineering.Interfaces;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.OutputData.XML.Engineering.Writer
{
	internal class XMLEngineeringAngledriveWriterV10 : AbstractComponentWriter<IAngledriveInputData>,
		IXMLEngineeringAngledriveWriter
	{
		private XNamespace _componentDataNamespace;

		public const string NAMESPACE_URI = XMLDefinitions.ENGINEERING_DEFINITONS_NAMESPACE_V10;

		public XMLEngineeringAngledriveWriterV10() : base("AngledriveDataEngineeringType") { }

		#region Overrides of AbstractXMLWriter

		public override XNamespace ComponentDataNamespace
		{
			get { return _componentDataNamespace ?? (_componentDataNamespace = Writer.RegisterNamespace(NAMESPACE_URI)); }
		}

		#endregion

		#region Overrides of AbstractComponentWriter<IAxleGearInputData>

		protected override object[] DoWriteXML(IAngledriveInputData data)
		{
			var tns = ComponentDataNamespace;
			var typeId = string.Format("ANGLDRV-{0:0.000}", data.Ratio);
			return new object[] {
				GetXMLTypeAttribute(),
				new XAttribute(XMLNames.Component_ID_Attr, typeId),

				GetDefaultComponentElements(data),
				new XElement(tns + XMLNames.Axlegear_Ratio, data.Ratio.ToXMLFormat(3)),
				data.LossMap == null
					? new XElement(
						tns + XMLNames.Axlegear_TorqueLossMap,
						new XElement(tns + XMLNames.Axlegear_Efficiency, data.Efficiency))
					: new XElement(tns + XMLNames.Axlegear_TorqueLossMap, GetTransmissionLossMap(data.LossMap))
			};
		}

		#endregion

		protected virtual object[] GetTransmissionLossMap(TableData lossmap)
		{
			if (Writer.Configuration.SingleFile) {
				return EmbedDataTable(lossmap, AttributeMappings.TransmissionLossmapMapping);
			}

			return ExtCSVResource(lossmap, Path.Combine(Writer.Configuration.BasePath, Writer.RemoveInvalidFileCharacters(Path.GetFileName(lossmap.Source))));
		}
	}
}
