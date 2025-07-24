using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.VehicleInformationFile.VehicleInformationFile_0_1
{
	public abstract class CompletedComponentVIFType : IReportMultistepCompletedBusTypeWriter
	{
		protected readonly IVIFReportInterimFactory _vifReportFactory;
		protected XNamespace _vif = XMLDefinitions.VEHICLE_INTERIM_FILE_TARGET_VERSION;
		protected XNamespace _v27 = "urn:tugraz:ivt:VectoAPI:DeclarationDefinitions:v2.7";
		protected XNamespace _xsi = XNamespace.Get("http://www.w3.org/2001/XMLSchema-instance");

		protected CompletedComponentVIFType(IVIFReportInterimFactory vifReportFactory)
		{
			_vifReportFactory = vifReportFactory;
		}

		#region Implementation of IReportMultistepCompletedBusTypeWriter

		public abstract XElement GetElement(IMultistageVIFInputData inputData);

		#endregion
	}


	public class ConventionalComponentsInterimVIFType : CompletedComponentVIFType
	{
		public ConventionalComponentsInterimVIFType(IVIFReportInterimFactory vifReportFactory) : base(vifReportFactory) { }

		#region Overrides of CompletedComponentVIFType

		public override XElement GetElement(IMultistageVIFInputData inputData)
		{
			var airDrag = _vifReportFactory.GetInterimAirdragType().GetElement(inputData);
			var auxiliaries = _vifReportFactory.GetInterimConventionalAuxiliariesType().GetElement(inputData);

			if (airDrag == null && auxiliaries == null) {
				return null;
			}

			return new XElement(_v27 + XMLNames.Vehicle_Components,
				new XAttribute(_xsi + XMLNames.XSIType, "Components_Conventional_CompletedBusType"),
				airDrag,
				auxiliaries);

		}

		#endregion
	}


	public class XEVComponentsInterimVIFType : CompletedComponentVIFType
	{
		public XEVComponentsInterimVIFType(IVIFReportInterimFactory vifReportFactory) : base(vifReportFactory) { }

		#region Overrides of CompletedComponentVIFType

		public override XElement GetElement(IMultistageVIFInputData inputData)
		{
			var airDrag = _vifReportFactory.GetInterimAirdragType().GetElement(inputData);
			var auxiliaries = _vifReportFactory.GetInterimxEVAuxiliariesType().GetElement(inputData);

			if (airDrag == null && auxiliaries == null) {
				return null;
			}

			return new XElement(_v27 + XMLNames.Vehicle_Components,
				new XAttribute(_xsi + XMLNames.XSIType, "Components_xEV_CompletedBusType"),
				airDrag,
				auxiliaries);

		}

		#endregion
	}
}