using Ninject.Modules;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration.NinjectModules {
	public class XMLDeclarationInputDataV22InjectModule : NinjectModule
	{
		#region Overrides of NinjectModule

		public override void Load()
		{
			Bind<IXMLTyreDeclarationInputData>().To<XMLDeclarationTyreDataProviderV22>().Named(
				XMLHelper.GetVersionFromNamespaceUri(XMLDeclarationTyreDataProviderV22.NAMESPACE_URI));
		}

		#endregion
	}
}