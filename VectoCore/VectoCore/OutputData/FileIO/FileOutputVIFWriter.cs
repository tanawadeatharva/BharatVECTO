using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;


namespace TUGraz.VectoCore.OutputData.FileIO
{
	public class FileOutputVIFWriter : FileOutputWriter
	{
		public const string REPORT_ENDING_PREFIX = "VIF_Report_";

		private string _jobFile;
		private readonly int _numberOfManufacturingStages;
		
		public string XMLMultistageReportFileName
		{
			get { return Path.ChangeExtension(_jobFile, $"{REPORT_ENDING_PREFIX}{_numberOfManufacturingStages + 2}.xml"); }
		}
		
		public FileOutputVIFWriter(string jobFile, int numberOfManufacturingStages) : base(jobFile)
		{
			_jobFile = jobFile;
			RemoveExistingEndingPrefix();
			_numberOfManufacturingStages = numberOfManufacturingStages;
		}
		
		private void RemoveExistingEndingPrefix()
		{
			var vifReportIndex = _jobFile.IndexOf(REPORT_ENDING_PREFIX, StringComparison.Ordinal);
			if (vifReportIndex == -1)
				return;

			if (!_jobFile.Contains(REPORT_ENDING_PREFIX))
				return;

			_jobFile = $"{_jobFile.Substring(0, vifReportIndex - 1)}.xml";
		}


		public override void WriteReport(ReportType type, XDocument data)
		{
			var fileName = (string)null;
			switch (type) {
				case ReportType.DeclarationReportMultistageVehicleXML:
					fileName = XMLMultistageReportFileName;
					break;
				default:
					base.WriteReport(type, data);
					break;
			}

			if (fileName == null)
				return;

			using (var writer = new FileStream(fileName, FileMode.Create))
			{
				using (var xmlWriter = new XmlTextWriter(writer, Encoding.UTF8))
				{
					xmlWriter.Formatting = Formatting.Indented;
					data.WriteTo(xmlWriter);
					xmlWriter.Flush();
					xmlWriter.Close();
				}
			}

		}
	}
}
