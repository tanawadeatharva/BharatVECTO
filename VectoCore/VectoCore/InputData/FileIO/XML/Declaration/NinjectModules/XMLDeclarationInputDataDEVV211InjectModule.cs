using Ninject.Modules;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration.NinjectModules
{

	public class XMLDeclarationInputDataDEVV211InjectModule : NinjectModule
	{
		#region Overrides of NinjectModule

		public override void Load()
		{
			Bind<IXMLEngineDeclarationInputData>().To<XMLDeclarationEngineDataProvider_DEV_V211>()
				.Named(XMLDeclarationEngineDataProvider_DEV_V211.QUALIFIED_XSD_TYPE);
		}

		#endregion
	}
}