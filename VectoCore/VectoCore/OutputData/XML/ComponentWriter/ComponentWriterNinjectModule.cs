using System.Xml.Linq;
using Ninject.Extensions.Factory;
using TUGraz.VectoCore.OutputData.XML.GroupWriter.Declaration;
using TUGraz.VectoCore.Utils.Ninject;

namespace TUGraz.VectoCore.OutputData.XML.ComponentWriter
{
	public class ComponentWriterNinjectModule : AbstractNinjectModule{

		private string GetName(string groupName, XNamespace xNamespace)
		{
			return UseFirstTwoArgumentsAsInstanceProvider.GetName(groupName, xNamespace.ToString());
		}

		#region Overrides of NinjectModule

		public override void Load()
		{
			Bind<IComponentWriterFactory>().ToFactory(() => new UseFirstTwoArgumentsAsInstanceProvider(1, false));


			//ADASTypes
			var v24 = XMLDeclarationNamespaces.V24;
			Bind<IDeclarationAdasWriter>().To<AdasConventionalWriter>().
				Named(GetName(GroupNames.ADAS_Conventional_Type, v24));
			Bind<IDeclarationAdasWriter>().To<AdasHEVWriter>().
				Named(GetName(GroupNames.ADAS_HEV_Type, v24));
			Bind<IDeclarationAdasWriter>().To<AdasPEVWriter>().
				Named(GetName(GroupNames.ADAS_PEV_Type, v24));
			Bind<IDeclarationAdasWriter>().To<AdasIEPCWriter>().
				Named(GetName(GroupNames.ADAS_IEPC_Type, v24));





        }

		#endregion
	}
}