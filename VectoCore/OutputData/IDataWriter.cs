using System.Data;
using System.IO;
using TUGraz.VectoCore.Models.Simulation.Data;

namespace TUGraz.VectoCore.OutputData
{
	public interface IOutputDataWriter : IModalDataWriter, ISummaryWriter, IReportWriter {}

	public interface IModalDataWriter
	{
		void WriteModData(string runName, string cycleName, string runSuffix, DataTable modData);
	}


	public interface ISummaryWriter
	{
		void WriteSumData(DataTable sortedAndFilteredTable);
	}

	public interface IReportWriter
	{
		Stream WriterStream(ReportType type);
	}

	public enum ReportType
	{
		DeclarationReportPdf
	}
}