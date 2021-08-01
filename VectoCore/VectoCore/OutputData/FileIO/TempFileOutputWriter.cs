using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Xml.Linq;
using TUGraz.VectoCommon.Models;

namespace TUGraz.VectoCore.OutputData.FileIO
{
	public class TempFileOutputWriter : FileOutputWriter, IOutputDataWriter
	{
		private Dictionary<ReportType, XDocument> _writtenReports = new Dictionary<ReportType, XDocument>();

		public TempFileOutputWriter(string jobFile) : base(jobFile)
		{

		}

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

			

			_writtenReports.Add(type, data);
		}

		public override void WriteReport(ReportType type, Stream data)
		{
			throw new NotImplementedException("PDF is not supported by TempFileOutputWriter");
		}
		#endregion

		public XDocument GetDocument(ReportType type)
		{
			
			var docStored = _writtenReports.TryGetValue(type, out var report);
			if (!docStored) {
				Log.Warn($"No Document with type {type} stored");
			}
			return report;
		}
	}
}