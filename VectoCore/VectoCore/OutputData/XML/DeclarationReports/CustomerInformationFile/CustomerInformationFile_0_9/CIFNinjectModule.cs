using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ninject.Extensions.Factory;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9;
using TUGraz.VectoCore.Utils.Ninject;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.CustomerInformationFile.CustomerInformationFile_0_9
{
    internal class CIFNinjectModule : MRFNinjectModule
    {
		#region Overrides of VectoNinjectModule

		public override void Load()
		{
			Bind<ICustomerInformationFileFactory>().ToFactory(() => new CombineArgumentsToNameInstanceProvider(
				nameCombinationMethod,
				4,
				4,
				typeof(ICustomerInformationFileFactory).GetMethod(nameof(ICustomerInformationFileFactory
					.GetCustomerReport)))).InSingletonScope();






		}

		#endregion
	}
}
