using System.Data;

namespace TUGraz.VectoCommon.InputData
{
	public class TableData : DataTable
	{
		public enum DataSourceType
		{
			Embedded,
			CSVFile,
		}

		public TableData(string fileName)
		{
			SourceType = DataSourceType.CSVFile;
			Source = fileName;
		}

		public TableData()
		{
			SourceType = DataSourceType.Embedded;
			Source = "";
		}

		public DataSourceType SourceType { get; protected set; }

		public string Source { get; protected set; }
	}
}