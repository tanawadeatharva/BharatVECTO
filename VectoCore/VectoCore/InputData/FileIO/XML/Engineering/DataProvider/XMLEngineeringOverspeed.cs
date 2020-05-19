using System;
using System.Xml;
using System.Xml.Linq;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.XML.Common;
using TUGraz.VectoCore.InputData.FileIO.XML.Engineering.Interfaces;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Engineering.DataProvider
{
	internal class XMLEngineeringOverspeedV07 : AbstractXMLType, IXMLOverspeedData
	{
		public static readonly XNamespace NAMESPACE_URI = XMLDefinitions.ENGINEERING_DEFINITONS_NAMESPACE_V07;

		public const string XSD_TYPE = "OverspeedEngineeringType";

		public static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI, XSD_TYPE);


		protected IXMLEngineeringDriverData DriverData;

		public XMLEngineeringOverspeedV07(IXMLEngineeringDriverData driverData, XmlNode node) : base(node)
		{
			DriverData = driverData;
		}

		#region Implementation of IOverSpeedEcoRollDeclarationInputData

		public virtual bool Enabled
		{
			get { return GetNode(XMLNames.DriverModel_Overspeed_Mode, required: false)?.InnerText.Equals("Overspeed", StringComparison.InvariantCultureIgnoreCase) ?? false; }
		}

		#endregion

		#region Implementation of IOverSpeedEcoRollEngineeringInputData

		public virtual MeterPerSecond MinSpeed
		{
			get { return GetNode(XMLNames.DriverModel_Overspeed_MinSpeed, required: false)?.InnerText.ToDouble().KMPHtoMeterPerSecond(); }
		}

		public virtual MeterPerSecond OverSpeed
		{
			get { return GetNode(XMLNames.DriverModel_Overspeed_AllowedOverspeed, required: false)?.InnerText.ToDouble().KMPHtoMeterPerSecond(); }
		}

		public virtual MeterPerSecond UnderSpeed
		{
			get {
				return GetNode(XMLNames.DriverModel_Overspeed_AllowedUnderspeed, required: false)?.InnerText.ToDouble().KMPHtoMeterPerSecond();
			}
		}

		#endregion
	}

	internal class XMLEngineeringOverspeedV10 : XMLEngineeringOverspeedV07
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.ENGINEERING_DEFINITONS_NAMESPACE_V10;

		public new const string XSD_TYPE = "OverspeedEngineeringType";

		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI, XSD_TYPE);



		public XMLEngineeringOverspeedV10(IXMLEngineeringDriverData driverData, XmlNode node) : base(driverData, node) { }

		public override bool Enabled
		{
			get { return XmlConvert.ToBoolean(GetNode(XMLNames.DriverModel_Overspeed_Enabled, required: false)?.InnerText ?? "false"); }
		}

	}
}
