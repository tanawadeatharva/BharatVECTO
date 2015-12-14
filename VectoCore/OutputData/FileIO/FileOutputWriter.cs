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

		public FileOutputWriter(string jobFile)
			: this(Path.GetFileNameWithoutExtension(jobFile), Path.GetDirectoryName(jobFile)) {}


		public FileOutputWriter(string jobName, string basePath)
		{
			_jobName = jobName;
			_basePath = basePath;
		}

		public string GetModDataFileName(string runName, string cycleName, string runSuffix)
		{
			//var modFilePattern = Path.Combine(_basePath,
			//	runName.Replace(Constants.FileExtensions.VectoJobFile, "") + "_{0}{1}" +
			//	Constants.FileExtensions.ModDataFile);
			//var modFileName = string.Format(modFilePattern, cycleName, runSuffix ?? "");
			var modFileName = new StringBuilder(runName);
			if (!string.IsNullOrEmpty(cycleName) || !string.IsNullOrEmpty(runSuffix)) {
				modFileName.Append("_");
				if (!string.IsNullOrEmpty(cycleName)) {
					modFileName.Append(cycleName);
				}
				if (!string.IsNullOrEmpty(runSuffix)) {
					modFileName.Append(cycleName);
				}
			}
			modFileName.Append(Constants.FileExtensions.ModDataFile);
			return modFileName.ToString();
		}

		public void WriteModData(string runName, string cycleName, string runSuffix, DataTable modData)
		{
			VectoCSVFile.Write(GetModDataFileName(runName, cycleName, runSuffix).ToString(), modData);
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