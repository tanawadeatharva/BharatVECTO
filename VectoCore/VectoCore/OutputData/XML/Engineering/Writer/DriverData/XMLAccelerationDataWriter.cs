using System.IO;
using System.Xml.Linq;
using TUGraz.IVT.VectoXML;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.OutputData.XML.Engineering.Interfaces;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.OutputData.XML.Engineering.Writer.DriverData
{
	internal class XMLAccelerationDataWriterV10 : AbstractXMLWriter, IXMLAccelerationDataWriter
	{
		public const string NAMESPACE_URI = XMLDefinitions.ENGINEERING_DEFINITONS_NAMESPACE_V10;

		private XNamespace _componentDataNamespace;
		public XMLAccelerationDataWriterV10() : base("DriverAccelerationCurveEngineeringType") { }

		#region Overrides of AbstractXMLWriter

		public override XNamespace ComponentDataNamespace
		{
			get { return _componentDataNamespace ?? (_componentDataNamespace = Writer.RegisterNamespace(NAMESPACE_URI)); }
		}

		#endregion

		#region Implementation of IXMLEngineeringComponentWriter

		public override object[] WriteXML(IDriverModelData inputData)
		{
			var acc = inputData as IDriverAccelerationData;
			
			if (acc == null) {
				return new object[] { };
			}

			if (acc.AccelerationCurve.SourceType == DataSourceType.Embedded &&
				acc.AccelerationCurve.Source.StartsWith(DeclarationData.DeclarationDataResourcePrefix)) {

				var filename = acc.AccelerationCurve.Source.Replace(DeclarationData.DeclarationDataResourcePrefix + ".VACC.", "")
								.Replace(Constants.FileExtensions.DriverAccelerationCurve, "");
				var tns = Writer.RegisterNamespace(XMLDefinitions.ENGINEERING_DEFINITONS_NAMESPACE_V10);
				return new object[] {
					new XElement(
						tns + XMLNames.ExternalResource,
						new XAttribute(XMLNames.ExtResource_Type_Attr, XMLNames.ExtResource_Type_Value_CSV),
						new XAttribute(XMLNames.ExtResource_File_Attr, filename))
				};
			}

			return new object[] {
				Writer.Configuration.SingleFile
					? EmbedDataTable(acc.AccelerationCurve, AttributeMappings.DriverAccelerationCurveMapping)
					: ExtCSVResource(
						acc.AccelerationCurve,
						Path.GetFileName(acc.AccelerationCurve.Source ?? "Driver.vacc"))
			};
		}

		#endregion
	}
}
