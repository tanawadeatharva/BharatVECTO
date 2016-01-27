/*
* Copyright 2015 European Union
*
* Licensed under the EUPL (the "Licence");
* You may not use this work except in compliance with the Licence.
* You may obtain a copy of the Licence at:
*
* http://ec.europa.eu/idabc/eupl5
*
* Unless required by applicable law or agreed to in writing, software 
* distributed under the Licence is distributed on an "AS IS" basis,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the Licence for the specific language governing permissions and 
* limitations under the Licence.
*/

using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using Font = System.Drawing.Font;
using Image = iTextSharp.text.Image;
using Rectangle = System.Drawing.Rectangle;

namespace TUGraz.VectoCore.OutputData
{
	/// <summary>
	/// Class for creating a declaration report.
	/// </summary>
	public abstract class DeclarationReport
	{
		/// <summary>
		/// Container class for one mission and the modData for the different loadings.
		/// </summary>
		protected class ResultContainer
		{
			/// <summary>
			/// The mission
			/// </summary>
			public Mission Mission;

			/// <summary>
			/// Dictionary of LoadingTypes and their resulting Modal Data
			/// </summary>
			public Dictionary<LoadingType, IModalDataContainer> ModData;
		}

		/// <summary>
		/// Dictionary of MissionTypes and their corresponding results.
		/// </summary>
		protected readonly Dictionary<MissionType, ResultContainer> _missions =
			new Dictionary<MissionType, DeclarationReport.ResultContainer>();

		/// <summary>
		/// The full load curve.
		/// </summary>
		internal FullLoadCurve Flc { get; set; }

		/// <summary>
		/// The declaration segment from the segment table
		/// </summary>
		internal Segment Segment { get; set; }

		/// <summary>
		/// The creator name for the report.
		/// </summary>
		public string Creator { get; internal set; }

		/// <summary>
		/// The name of the job file (report name will be the same)
		/// </summary>
		public string JobName { get; set; }

		/// <summary>
		/// The result count determines how many results must be given before the report gets written.
		/// </summary>
		public int ResultCount { get; set; }


		/// <summary>
		/// Adds the result of one run for the specific mission and loading. If all runs finished (given by the resultCount) the report will be written.
		/// </summary>
		/// <param name="loadingType">Type of the loading.</param>
		/// <param name="mission">The mission.</param>
		/// <param name="modData">The mod data.</param>
		[MethodImpl(MethodImplOptions.Synchronized)]
		public void AddResult(LoadingType loadingType, Mission mission, IModalDataContainer modData)
		{
			if (!_missions.ContainsKey(mission.MissionType)) {
				_missions[mission.MissionType] = new DeclarationReport.ResultContainer {
					Mission = mission,
					ModData = new Dictionary<LoadingType, IModalDataContainer>()
				};
			}
			_missions[mission.MissionType].ModData[loadingType] = modData;


			if (ResultCount == _missions.Sum(v => v.Value.ModData.Count)) {
				DoWriteReport();
			}
		}

		protected internal abstract void DoWriteReport();

		public void InitializeReport(VectoRunData modelData, Segment segment)
		{
			Segment = segment;
			ResultCount = segment.Missions.Sum(m => m.Loadings.Count);

			DoInitializeReport(modelData, segment);
		}

		protected abstract void DoInitializeReport(VectoRunData modelData, Segment segment);
	}
}