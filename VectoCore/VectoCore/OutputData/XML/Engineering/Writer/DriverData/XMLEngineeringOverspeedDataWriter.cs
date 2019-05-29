using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.OutputData.XML.Engineering.Interfaces;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.OutputData.XML.Engineering.Writer
{
	internal class XMLEngineeringOverspeedDataWriterV10 : AbstractXMLWriter, IXMLOverspeedDataWriter
	{
		private XNamespace _componentDataNamespace;

		public const string NAMESPACE_URI = XMLDefinitions.ENGINEERING_DEFINITONS_NAMESPACE_V10;

		public XMLEngineeringOverspeedDataWriterV10() : base("OverspeedEngineeringType") { }

		#region Implementation of IXMLEngineeringComponentWriter

		public override object[] WriteXML(IDriverModelData inputData)
		{
			var overspeed = inputData as IOverSpeedEcoRollEngineeringInputData;
			var ns = ComponentDataNamespace;
			if (overspeed == null) {
				return new object[] { };
			}

			if (overspeed.Mode != DriverMode.Off) {
				return new object[] {
					new XElement(ns + XMLNames.DriverModel_Overspeed_Mode, overspeed.Mode),
					new XElement(ns + XMLNames.DriverModel_Overspeed_MinSpeed, overspeed.MinSpeed.AsKmph),
					new XElement(ns + XMLNames.DriverModel_Overspeed_AllowedOverspeed, overspeed.OverSpeed.AsKmph),
					new XElement(ns + XMLNames.DriverModel_Overspeed_AllowedUnderspeed, overspeed.UnderSpeed.AsKmph)
				};
			}

			return new object[] {
				new XElement(ns + XMLNames.DriverModel_Overspeed_Mode, overspeed.Mode),
			};
		}

		#endregion

		#region Overrides of AbstractXMLWriter

		public override XNamespace ComponentDataNamespace
		{
			get { return _componentDataNamespace ?? (_componentDataNamespace = Writer.RegisterNamespace(NAMESPACE_URI)); }
		}

		#endregion
	}
}
