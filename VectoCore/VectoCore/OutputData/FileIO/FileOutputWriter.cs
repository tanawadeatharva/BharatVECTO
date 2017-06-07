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
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.OutputData.FileIO
{
	public class FileOutputWriter : LoggingObject, IOutputDataWriter
	{
		private readonly string _jobFile;

		private string BasePath
		{
			get { return Path.GetDirectoryName(_jobFile); }
		}

		public string PDFReportName
		{
			get { return Path.ChangeExtension(_jobFile, Constants.FileExtensions.PDFReport); }
		}

		public string XMLFullReportName
		{
			get { return Path.ChangeExtension(_jobFile, "RESULT.xml"); }
		}

		public string XMLCoCReportName
		{
			get { return Path.ChangeExtension(_jobFile, "COC.xml"); }
		}


		public string SumFileName
		{
			get { return Path.ChangeExtension(_jobFile, Constants.FileExtensions.SumFile); }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="jobFile">full path of the json job-file. jobName and basePath are extracted.</param>
		public FileOutputWriter(string jobFile)
		{
			_jobFile = jobFile;
		}

		public void WriteSumData(DataTable data)
		{
			VectoCSVFile.Write(SumFileName, data);
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
				case ReportType.DeclarationReportXMLFulll:
					return new FileStream(XMLFullReportName, FileMode.Create);
				case ReportType.DeclarationReportXMLCOC:
					return new FileStream(XMLCoCReportName, FileMode.Create);
				default:

					throw new ArgumentOutOfRangeException("type");
			}
		}
	}
}