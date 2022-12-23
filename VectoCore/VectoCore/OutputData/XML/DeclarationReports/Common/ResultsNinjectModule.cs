using Ninject.Extensions.Factory;
using Ninject.Modules;
using TUGraz.VectoCore.Utils.Ninject;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.Common
{
	public class ResultsNinjectModule : NinjectModule
	{

		private VehicleTypeAndArchitectureStringHelperResults _namingHelper =
			new VehicleTypeAndArchitectureStringHelperResults();

		#region Overrides of NinjectModule

		public override void Load()
		{
			Bind<IResultsWriterFactory>().To<ResultWriterFactory>().InSingletonScope();

			Bind<IInternalResultWriterFactory>().ToFactory(() => new CombineArgumentsToNameInstanceProvider(
				new CombineArgumentsToNameInstanceProvider.MethodSettings() {
					combineToNameDelegate = _namingHelper.CreateName,
					skipArguments = 1,
					takeArguments = 1,
					methods = new[] {
						typeof(IInternalResultWriterFactory).GetMethod(nameof(IInternalResultWriterFactory
							.GetCIFResultsWriter)),
						typeof(IInternalResultWriterFactory).GetMethod(nameof(IInternalResultWriterFactory
							.GetMRFResultsWriter)),
						typeof(IInternalResultWriterFactory).GetMethod(nameof(IInternalResultWriterFactory
							.GetVIFResultsWriter))

					}
				})).InSingletonScope();
		}

		#endregion
	}
}