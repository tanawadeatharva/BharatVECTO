using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Xml.Linq;
using Castle.Core.Internal;
using TUGraz.VectoCommon.Models;

namespace TUGraz.VectoCore.OutputData.FileIO
{
	public class TempFileOutputWriter : FileOutputWriter
	{
		private readonly Dictionary<ReportType, XDocument> _storedReports = new Dictionary<ReportType, XDocument>();
		private readonly HashSet<ReportType> _reportsToWrite;

		#region Overrides of FileOutputWriter

		public override string XMLFullReportName => Path.ChangeExtension(_jobFile, "RSLT_MANUFACTURER_PRIMARY.xml");

		#endregion

		/// <summary>
		/// Stores all written Documents by default.
		/// </summary>
		/// <param name="jobFile"></param>
		/// <param name="reportsToWrite">ReportTypes specified here are written to disk</param>
		public TempFileOutputWriter(string jobFile, params ReportType[] reportsToWrite) : base(jobFile)
		{
			_reportsToWrite = new HashSet<ReportType>();
			if (!reportsToWrite.IsNullOrEmpty()) {
				foreach (var reportType in reportsToWrite) {
					_reportsToWrite.Add(reportType);
				}
			}
		}

		#region Overrides of FileOutputWriter

		public override IDictionary<ReportType, string> GetWrittenFiles()
		{
			return base.GetWrittenFiles();
		}

		#endregion

		protected TempFileOutputWriter(string jobFile, int numberOfManufacturingStages) : base(jobFile,
			numberOfManufacturingStages)
		{

		}

		#region Overrides of FileOutputWriter
		public override void WriteReport(ReportType type, XDocument data)
		{
			if (type == ReportType.DeclarationReportPdf) {
				throw new ArgumentOutOfRangeException("PDF is not supported by TempFileOutputWriter");
			}

			if (_reportsToWrite.Contains(type)) {
				base.WriteReport(type, data);
			}

			_storedReports.Add(type, data);
		}

		public override void WriteReport(ReportType type, Stream data)
		{
			throw new NotImplementedException("PDF is not supported by TempFileOutputWriter");
		}
		#endregion

		public XDocument GetDocument(ReportType type)
		{
			
			var docStored = _storedReports.TryGetValue(type, out var report);
			if (!docStored) {
				Log.Warn($"No Document with type {type} stored");
			}
			return report;
		}
	}
}