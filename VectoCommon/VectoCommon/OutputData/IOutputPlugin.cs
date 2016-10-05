namespace TUGraz.VectoCommon.OutputData
{
	public interface IOutputPlugin
	{
		string Key { get; }

		string Name { get; }

		IOutputFileWriter Instance { get; }
	}
}