using System;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportXMLTypeWriter
{
    public abstract class MRFVehicleType : AbstractMrfXmlType
    {
		

		public MRFVehicleType(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }
	}


	public class MRF_ConventionalLorryVehicleType : MRFVehicleType
	{
		public MRF_ConventionalLorryVehicleType(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Overrides of AbstractMRFComponentWriter

		#endregion

		#region Overrides of AbstractMRFComponentWriter

		public override XElement GetXmlType(IDeclarationInputDataProvider inputData)
		{
			var vehicle = inputData.JobInputData.Vehicle;
			return new XElement(_mrf + XMLNames.Component_Vehicle,
				_mrfFactory.GetConventionalLorryVehicleOutputGroup().GetElements(inputData),

				_mrfFactory.GetEngineTorqueLimitationsType().GetXmlType(inputData),
				_mrfFactory.GetConventionalLorryComponentsType().GetXmlType(inputData)




				);
				
		}

		#endregion
	}

	public class MRF_HEV_Px_IHPC_LorryVehicleType : MRFVehicleType
	{
		public MRF_HEV_Px_IHPC_LorryVehicleType(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Overrides of AbstractMRFComponentWriter
		#endregion

		#region Overrides of AbstractMRFComponentWriter

		public override XElement GetXmlType(IDeclarationInputDataProvider inputData)
		{
			return new XElement(_mrf + XMLNames.Component_Vehicle);//, _mrfFactory.Get)
		}

		#endregion
	}
}
