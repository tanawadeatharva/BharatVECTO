/*
* This file is part of VECTO.
*
* Copyright © 2012-2016 European Union
*
* Developed by Graz University of Technology,
*              Institute of Internal Combustion Engines and Thermodynamics,
*              Institute of Technical Informatics
*
* VECTO is licensed under the EUPL, Version 1.1 or - as soon they will be approved
* by the European Commission - subsequent versions of the EUPL (the "Licence");
* You may not use VECTO except in compliance with the Licence.
* You may obtain a copy of the Licence at:
*
* https://joinup.ec.europa.eu/community/eupl/og_page/eupl
*
* Unless required by applicable law or agreed to in writing, VECTO
* distributed under the Licence is distributed on an "AS IS" basis,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the Licence for the specific language governing permissions and
* limitations under the Licence.
*
* Authors:
*   Stefan Hausberger, hausberger@ivt.tugraz.at, IVT, Graz University of Technology
*   Christian Kreiner, christian.kreiner@tugraz.at, ITI, Graz University of Technology
*   Michael Krisper, michael.krisper@tugraz.at, ITI, Graz University of Technology
*   Raphael Luz, luz@ivt.tugraz.at, IVT, Graz University of Technology
*   Markus Quaritsch, markus.quaritsch@tugraz.at, IVT, Graz University of Technology
*   Martin Rexeis, rexeis@ivt.tugraz.at, IVT, Graz University of Technology
*/

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
	public class SummaryFileWriter : LoggingObject, ISummaryWriter
	{
		public readonly string SumFileName;

		public SummaryFileWriter(string jobName)
		{
			SumFileName = Path.ChangeExtension(jobName, Constants.FileExtensions.SumFile);
		}

		public void WriteSumData(DataTable data)
		{
			VectoCSVFile.Write(SumFileName, data);
		}
	}
	
	public class FileOutputWriter : LoggingObject, IOutputDataWriter
	{
		private readonly string JobFile;

		private string BasePath { get { return Path.GetDirectoryName(JobFile); } }
		private string ModFileName { get { return Path.GetDirectoryName(JobFile); } }
		private string PDFReportName { get { return Path.ChangeExtension(JobFile, Constants.FileExtensions.PDFReport); } }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="jobFile">full path of the json job-file. jobName and basePath are extracted</param>
		public FileOutputWriter(string jobFile)
		{
			JobFile = jobFile;
		}
		
		public string GetModDataFileName(string runName, string cycleName, string runSuffix)
		{
			string modFileName;
			if (!string.IsNullOrWhiteSpace(cycleName) || !string.IsNullOrWhiteSpace(runSuffix)) {
				modFileName = string.Format("{0}_{1}{2}{3}", runName, cycleName, runSuffix, Constants.FileExtensions.ModDataFile);
			} else {
				modFileName = string.Format("{0}{1}", runName, Constants.FileExtensions.ModDataFile);
			}

			return Path.Combine(BasePath, modFileName);
		}

		public void WriteModData(string runName, string cycleName, string runSuffix, DataTable modData)
		{
			VectoCSVFile.Write(GetModDataFileName(runName, cycleName, runSuffix), modData);
		}

		public Stream WriteStream(ReportType type)
		{
			switch (type) {
				case ReportType.DeclarationReportPdf:
					return new FileStream(PDFReportName, FileMode.Create);
				default:
					throw new ArgumentOutOfRangeException("type");
			}
		}
	}
}