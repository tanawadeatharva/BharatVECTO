using TUGraz.VectoCommon.InputData;

namespace TUGraz.VectoCommon.OutputData
{
	public interface IExportPlugin
	{
		string Key { get; }

		string Name { get; }

		void ExportJob(IInputDataProvider data);
	}
}