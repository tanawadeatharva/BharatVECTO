using System.Threading;

namespace TUGraz.VectoCommon.InputData
{
	public interface IInputDataPlugin
	{
		string Key { get; }

		string Name { get; }

		bool CanHandleJob(string filename); 

		IInputDataProvider ReadVectoJob(string filename);

		string[] KnownExtensions { get; }
	}
}