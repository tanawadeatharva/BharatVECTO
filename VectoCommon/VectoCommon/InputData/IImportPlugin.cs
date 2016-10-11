namespace TUGraz.VectoCommon.InputData
{
	public interface IImportPlugin
	{
		string Key { get; }

		string Name { get; }

		string ImportJob();
	}
}