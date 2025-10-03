namespace TUGraz.VectoCore.OutputData.XML
{
	public class XMLDeclarationReportFactoryNinjectModule : AbstractNinjectModule
	{
		#region Overrides of NinjectModule

		public override void Load()
		{
			Bind<IXMLDeclarationReportFactory>().To<XMLDeclarationReportFactory>().InSingletonScope();
		}

		#endregion
	}
}