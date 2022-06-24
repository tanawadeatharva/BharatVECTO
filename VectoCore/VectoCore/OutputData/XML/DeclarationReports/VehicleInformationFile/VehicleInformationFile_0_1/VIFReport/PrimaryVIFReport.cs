using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUGraz.VectoCommon.InputData;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.VehicleInformationFile.VehicleInformationFile_0_1.VIFReport
{
	internal class PrimaryVIFReportBase : AbstractVIFReport
	{
		private string _outputDataType;
		public PrimaryVIFReportBase(IVIFReportFactory vifFactory) : base(vifFactory) { }

		#region Overrides of AbstractVIFReport

		public override string OutputDataType => _outputDataType;

		public override void InitializeVehicleData(IDeclarationInputDataProvider inputData)
		{



			//Vehicle
			//InputDataSignature
			//ManufacturerRecordSignature
			//Results
			//ApplicationInformation


			throw new NotImplementedException();
		}

		#endregion
	}

	


}
