using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.OutputData.XML.Engineering.Interfaces;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.OutputData.XML.Engineering.Writer.DriverData
{
	internal class XMLEngineeringOverspeedDataWriterV10 : AbstractXMLWriter, IXMLOverspeedDataWriter
	{
		private XNamespace _componentDataNamespace;

		public const string NAMESPACE_URI = XMLDefinitions.ENGINEERING_DEFINITONS_NAMESPACE_V10;

		public XMLEngineeringOverspeedDataWriterV10() : base("OverspeedEngineeringType") { }

		#region Implementation of IXMLEngineeringComponentWriter

		public override object[] WriteXML(IDriverModelData inputData)
		{
			var overspeed = inputData as IOverSpeedEngineeringInputData;
			var ns = ComponentDataNamespace;
			if (overspeed == null) {
				return new object[] { };
			}

			if (overspeed.Enabled) {
				return new object[] {
					new XElement(ns + XMLNames.DriverModel_Overspeed_Enabled, overspeed.Enabled),
					new XElement(ns + XMLNames.DriverModel_Overspeed_MinSpeed, overspeed.MinSpeed.AsKmph),
					new XElement(ns + XMLNames.DriverModel_Overspeed_AllowedOverspeed, overspeed.OverSpeed.AsKmph),
				};
			}

			return new object[] {
				new XElement(ns + XMLNames.DriverModel_Overspeed_Enabled, overspeed.Enabled),
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
