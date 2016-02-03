using System;
using System.Data;
using System.IO;
using System.Text;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.OutputData.FileIO
{
	public class FileOutputWriter : LoggingObject, IOutputDataWriter
	{
		private readonly string _basePath;
		private readonly string _jobName;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="jobFile">full path of the json job-file. jobName and basePath are extracted</param>
		public FileOutputWriter(string jobFile)
			: this(Path.GetFileNameWithoutExtension(jobFile), Path.GetDirectoryName(jobFile)) {}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="jobName">Name of the job, used for the filename of the sum-file</param>
		/// <param name="basePath">path where to store the sum-file and report</param>
		public FileOutputWriter(string jobName, string basePath)
		{
			_jobName = jobName;
			_basePath = basePath;
		}

		public string GetModDataFileName(string runName, string cycleName, string runSuffix)
		{
			string modFileName;
			if (!string.IsNullOrWhiteSpace(cycleName) || !string.IsNullOrWhiteSpace(runSuffix)) {
				modFileName = string.Format("{0}_{1}{2}{3}", runName, cycleName, runSuffix, Constants.FileExtensions.ModDataFile);
			} else {
				modFileName = string.Format("{0}{1}", runName, Constants.FileExtensions.ModDataFile);
			}

			return Path.Combine(_basePath, modFileName);
		}

		public void WriteModData(string runName, string cycleName, string runSuffix, DataTable modData)
		{
			VectoCSVFile.Write(GetModDataFileName(runName, cycleName, runSuffix), modData);
		}

		public string GetSumFileName()
		{
			return Path.Combine(_basePath, Path.GetFileNameWithoutExtension(_jobName) + Constants.FileExtensions.SumFile);
		}

		public void WriteSumData(DataTable data)
		{
			VectoCSVFile.Write(GetSumFileName(), data);
		}


		public Stream WriterStream(ReportType type)
		{
			switch (type) {
				case ReportType.DeclarationReportPdf:
					return new FileStream(Path.Combine(_basePath, _jobName + ".pdf"), FileMode.Create);
				default:
					throw new ArgumentOutOfRangeException("type");
			}
		}
	}
}