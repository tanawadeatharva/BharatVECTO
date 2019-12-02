using System.Xml;
using TUGraz.IVT.VectoXML;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.InputData.FileIO.XML.Common;
using TUGraz.VectoCore.InputData.FileIO.XML.Engineering.Interfaces;
using TUGraz.VectoCore.InputData.Impl;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Engineering.DataProvider
{
	internal class XMLDriverAccelerationV07 : AbstractXMLType, IXMLDriverAcceleration
	{
		public const string NAMESPACE_URI = XMLDefinitions.ENGINEERING_DEFINITONS_NAMESPACE_V07;

		public const string XSD_TYPE = "DriverAccelerationCurveEngineeringType";

		public static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI, XSD_TYPE);


		protected IXMLEngineeringDriverData DriverData;
		private IDriverAccelerationData _accelerationCurve;

		public XMLDriverAccelerationV07(IXMLEngineeringDriverData driverData, XmlNode node) : base(node)
		{
			DriverData = driverData;
		}

		public XMLDriverAccelerationV07(TableData accCurve) : base(null)
		{
			_accelerationCurve = new DriverAccelerationInputData() { AccelerationCurve = accCurve };
		}

		public virtual IDriverAccelerationData AccelerationCurve
		{
			get {
				return BaseNode == null
					? _accelerationCurve
					: new DriverAccelerationInputData() {
						AccelerationCurve = XMLHelper.ReadEntriesOrResource(
							BaseNode, DriverData.DataSource.SourcePath, null, XMLNames.DriverModel_DriverAccelerationCurve_Entry,
							AttributeMappings.DriverAccelerationCurveMapping)
					};
			}
		}
	}

	internal class XMLDriverAccelerationV10 : XMLDriverAccelerationV07
	{
		public new const string NAMESPACE_URI = XMLDefinitions.ENGINEERING_DEFINITONS_NAMESPACE_V10;

		//public new const string XSD_TYPE = "DriverAccelerationCurveType";

		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI, XSD_TYPE);



		public XMLDriverAccelerationV10(IXMLEngineeringDriverData driverData, XmlNode node) : base(driverData, node) { }
	}
}
