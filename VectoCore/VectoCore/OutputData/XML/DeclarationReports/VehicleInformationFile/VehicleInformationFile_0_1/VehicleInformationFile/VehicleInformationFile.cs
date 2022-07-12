using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUGraz.VectoCommon.InputData;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.VehicleInformationFile.VehicleInformationFile_0_1.VIFReport
{
	internal class VehicleInformationFile : AbstractVehicleInformationFile
	{
		private string _outputDataType;

		public VehicleInformationFile(IVIFReportFactory vifFactory) : base(vifFactory)
		{
			_tns = VIF;
		}

		#region Overrides of AbstractVIFReport

		public override string OutputDataType => _outputDataType;

		public override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{
			Vehicle = _vifFactory.GetConventionalVehicleType().GetElement(inputData);
		}

		#endregion
	}

	internal class Conventional_PrimaryBus_VIF : VehicleInformationFile
	{
		public Conventional_PrimaryBus_VIF(IVIFReportFactory vifFactory) : base(vifFactory) { }
	}


}
