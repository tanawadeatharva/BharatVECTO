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