using Ninject.Modules;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration;

namespace TUGraz.VectoCore
{
	public class VectoNinjectModule : NinjectModule
	{
		#region Overrides of NinjectModule

		public override void Load()
		{
			LoadModule<XMLDeclarationReaderInjectModule>();

			// load module engineering reader

			// use ninject for simulator factory?
		}

		#endregion

		protected virtual void LoadModule<T>() where T : class, INinjectModule, new()
		{
			if (Kernel != null && !Kernel.HasModule(typeof(T).FullName)) {
				Kernel.Load(new[] { new T() });
			}

		}
	}
}
