using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;

namespace TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents
{
	public abstract class ComponentDataAdapterBase : LoggingObject
	{
		protected void CheckDeclarationMode(IComponentInputData component, string componentName)
		{
			if (component == null) {
				return;
			}
			if (!component.SavedInDeclarationMode)
			{
				Log.Warn("{0} not in Declaration Mode!", componentName);
			}
		}
		protected void CheckDeclarationMode(IAuxiliariesDeclarationInputData aux, string componentName)
		{
			if (aux == null) {
				return;
			}
			if (!aux.SavedInDeclarationMode)
			{
				Log.Warn("{0} not in Declaration Mode!", componentName);
			}
		}
	}
}

