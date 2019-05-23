using System.IO;
using System.Xml.Linq;
using TUGraz.IVT.VectoXML;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.OutputData.XML.Engineering.Interfaces;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.OutputData.XML.Engineering.Writer
{
	internal class XMLEngineeringAirdragWriterV10 : AbstractComponentWriter<IAirdragEngineeringInputData>,
		IXMLEngineeringAirdragWriter
	{
		private XNamespace _componentDataNamespace;

		public const string NAMESPACE_URI = XMLDefinitions.ENGINEERING_DEFINITONS_NAMESPACE_V10;
		public XMLEngineeringAirdragWriterV10() : base("AirDragDataEngineeringType") { }

		#region Overrides of AbstractXMLWriter

		public override XNamespace ComponentDataNamespace
		{
			get { return _componentDataNamespace ?? (_componentDataNamespace = Writer.RegisterNamespace(NAMESPACE_URI)); }
		}

		#endregion

		#region Overrides of AbstractComponentWriter<IAirdragEngineeringInputData>

		protected override object[] DoWriteXML(IAirdragEngineeringInputData data)
		{
			var tns = ComponentDataNamespace;
			var id = string.Format("Airdrag-{0}", data.Model);
			return new object[] {
					new XAttribute(XMLNames.Component_ID_Attr, id),
					GetXMLTypeAttribute(),
					GetDefaultComponentElements(data),
					new XElement(tns + XMLNames.Vehicle_CrossWindCorrectionMode, data.CrossWindCorrectionMode.ToXMLFormat()),
					new XElement(tns + XMLNames.Vehicle_AirDragArea, data.AirDragArea.Value().ToXMLFormat(2)),
				GetCrossWindCorrectionData(data)
			};
		}

		#endregion

		protected virtual XElement GetCrossWindCorrectionData(IAirdragEngineeringInputData airdrag)
		{
			if (airdrag.CrossWindCorrectionMode == CrossWindCorrectionMode.NoCorrection ||
				airdrag.CrossWindCorrectionMode == CrossWindCorrectionMode.DeclarationModeCorrection) {
				return null;
			}

			var correctionMap = new XElement(ComponentDataNamespace + XMLNames.Vehicle_CrosswindCorrectionData);

			if (Writer.Configuration.SingleFile) {
				correctionMap.Add(EmbedDataTable(airdrag.CrosswindCorrectionMap, AttributeMappings.CrossWindCorrectionMapping));
			} else {
				var ext = airdrag.CrossWindCorrectionMode == CrossWindCorrectionMode.SpeedDependentCorrectionFactor
					? "vcdv"
					: "vcdb";
				correctionMap.Add(ExtCSVResource(airdrag.CrosswindCorrectionMap, Path.Combine(Writer.Configuration.BasePath, Writer.RemoveInvalidFileCharacters("CrossWindCorrection." + ext))));
			}

			return correctionMap;
		}
	}
}
