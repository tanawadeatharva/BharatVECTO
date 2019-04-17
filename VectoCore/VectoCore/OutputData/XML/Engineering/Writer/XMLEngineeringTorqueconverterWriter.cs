using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using TUGraz.IVT.VectoXML;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.OutputData.XML.Engineering.Interfaces;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.OutputData.XML.Engineering.Writer
{
	internal class XMLEngineeringTorqueconverterWriterV10 : AbstractComponentWriter<ITorqueConverterEngineeringInputData>,
		IXMLEngineeringTorqueconverterWriter
	{
		private XNamespace _componentDataNamespace;

		public const string NAMESPACE_URI = XMLDefinitions.ENGINEERING_DEFINITONS_NAMESPACE_V10;

		public XMLEngineeringTorqueconverterWriterV10() : base("TorqueConverterEngineeringDataType") { }

		#region Overrides of AbstractXMLWriter

		public override XNamespace ComponentDataNamespace
		{
			get { return _componentDataNamespace ?? (_componentDataNamespace = Writer.RegisterNamespace(NAMESPACE_URI)); }
		}

		#endregion

		#region Overrides of AbstractComponentWriter<ITorqueConverterEngineeringInputData>

		protected override object[] DoWriteXML(ITorqueConverterEngineeringInputData data)
		{
			var tns = ComponentDataNamespace;
			return new object[] {
				GetXMLTypeAttribute(),
				new XAttribute(XMLNames.Component_ID_Attr, string.Format("TC-{0}", data.Model)),
				GetDefaultComponentElements(data),
				new XElement(tns + XMLNames.TorqueConverter_ReferenceRPM, data.ReferenceRPM.AsRPM.ToXMLFormat()),
				new XElement(
					tns + XMLNames.TorqueConverter_Characteristics,
					Writer.Configuration.SingleFile
						? EmbedDataTable(
							data.TCData, AttributeMappings.TorqueConverterDataMapping,
							precision: new Dictionary<string, uint>() {
								{ TorqueConverterDataReader.Fields.SpeedRatio, 4 }
							})
						: ExtCSVResource(
							data.TCData,
							Path.Combine(
								Writer.Configuration.BasePath, Writer.RemoveInvalidFileCharacters(Path.GetFileName(data.TCData.Source))))),
				new XElement(tns + XMLNames.TorqueConverter_Inertia, data.Inertia.Value())
			};
		}

		#endregion
	}
}
