using System.Linq;
using Ninject.Extensions.Factory;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.VehicleInformationFile;
using TUGraz.VectoCore.Utils.Ninject;

namespace TUGraz.VectoCore.OutputData.XML
{
	public class XMLReportFactoryNinjectModule : AbstractNinjectModule
	{
		#region Overrides of NinjectModule

		public override void Load()
		{
			Bind<IXMLDeclarationReportFactory>().To<XMLDeclarationReportFactory>();

		}

		#endregion
	}
}