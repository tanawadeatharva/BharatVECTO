using Ninject.Modules;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration;
using TUGraz.VectoCore.InputData.FileIO.XML.Engineering;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.OutputData.XML;

namespace TUGraz.VectoCore
{
	public abstract class AbstractNinjectModule : NinjectModule
	{
		protected virtual void LoadModule<T>() where T : class, INinjectModule, new()
		{
			if (Kernel != null && !Kernel.HasModule(typeof(T).FullName)) {
				Kernel.Load(new[] { new T() });
			}

		}
	}

	public class VectoNinjectModule : AbstractNinjectModule
	{
		#region Overrides of NinjectModule

		public override void Load()
		{
			LoadModule<XMLInputDataNinjectModule>();

			LoadModule<XMLEngineeringWriterInjectModule>();

			LoadModule<SimulationFactoryNinjectModule>();
		}

		#endregion

		
	}
}
