/*
* Copyright 2015, 2016 Graz University of Technology,
* Institute of Internal Combustion Engines and Thermodynamics,
* Institute of Technical Informatics
*
* Licensed under the EUPL (the "Licence");
* You may not use this work except in compliance with the Licence.
* You may obtain a copy of the Licence at:
*
* http://ec.europa.eu/idabc/eupl
*
* Unless required by applicable law or agreed to in writing, software 
* distributed under the Licence is distributed on an "AS IS" basis,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the Licence for the specific language governing permissions and 
* limitations under the Licence.
*/

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