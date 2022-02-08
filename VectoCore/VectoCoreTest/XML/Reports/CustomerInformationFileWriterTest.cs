using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.CustomerInformationFile;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport;

namespace TUGraz.VectoCore.Tests.XML.Reports
{
    [TestFixture]
    internal class CustomerInformationFileWriterTest : MRF_CIF_WriterTestBase
    {
		private IXMLCustomerReport GetCustomerReport(string fileName,
			out IDeclarationInputDataProvider dataProvider)
		{
			Assert.IsFalse(string.IsNullOrEmpty(fileName));
			dataProvider = _xmlReader.CreateDeclaration(fileName);

			var arch = dataProvider.JobInputData.Vehicle.ArchitectureID;

			dataProvider.JobInputData.Vehicle.VehicleCategory.GetVehicleType(); // HEV/PEV - Sx/Px
			var ihpc = (dataProvider.JobInputData.Vehicle.Components.ElectricMachines?.Entries)?.Count(electric =>
				electric.ElectricMachine.IHPCType != "None") > 0;
			var iepc = (dataProvider.JobInputData.Vehicle.Components.IEPC != null);
			var report = _cifFactory.GetCustomerReport(
				dataProvider.JobInputData.Vehicle.VehicleCategory,
				dataProvider.JobInputData.JobType,
				dataProvider.JobInputData.Vehicle.ArchitectureID,
				dataProvider.JobInputData.Vehicle.ExemptedVehicle,
				iepc,
				ihpc);
			return report;
		}


		[TestCase(ConventionalHeavyLorry)]
		public void ConventionalLorryCIFTest(string fileName)
		{
			var report = GetCustomerReport(fileName, out var dataProvider) as 

		}






	}
}
