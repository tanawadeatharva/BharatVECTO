using System.IO;
using System.Xml.Linq;
using TUGraz.IVT.VectoXML;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.OutputData.XML.Engineering.Interfaces;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.OutputData.XML.Engineering.Writer
{
	internal class XMLEngineeringRetarderWriterV10 : AbstractComponentWriter<IRetarderInputData>,
		IXMLEngineeringRetarderWriter
	{
		private XNamespace _componentDataNamespace;

		public const string NAMESPACE_URI = XMLDefinitions.ENGINEERING_DEFINITONS_NAMESPACE_V10;
		public XMLEngineeringRetarderWriterV10() : base("RetarderDataEngineeringType") { }

		#region Overrides of AbstractXMLWriter

		public override XNamespace ComponentDataNamespace
		{
			get { return _componentDataNamespace ?? (_componentDataNamespace = Writer.RegisterNamespace(NAMESPACE_URI)); }
		}

		#endregion

		#region Overrides of AbstractComponentWriter<IRetarderInputData>

		protected override object[] DoWriteXML(IRetarderInputData data)
		{
			var tns = ComponentDataNamespace;
			var retarder = new object[] {
				GetXMLTypeAttribute(),
				new XAttribute(XMLNames.Component_ID_Attr, "RET-none"),
				GetDefaultComponentElements(data),
				new XElement(
					tns + XMLNames.Retarder_RetarderLossMap,
					Writer.Configuration.SingleFile
						? EmbedDataTable(data.LossMap, AttributeMappings.RetarderLossmapMapping)
						: ExtCSVResource(
							data.LossMap,
							Path.Combine(
								Writer.Configuration.BasePath,
								Writer.RemoveInvalidFileCharacters(string.Format("RET_{0}.vrlm", data.Model)))))
			};
			return retarder;
		}

		#endregion
	}
}
