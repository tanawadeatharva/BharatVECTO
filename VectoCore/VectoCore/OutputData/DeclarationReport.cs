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

using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.OutputData.PDF;

namespace TUGraz.VectoCore.OutputData
{
	public interface IDeclarationReport
	{
		void PrepareResult(LoadingType loading, Mission mission);
		void AddResult(LoadingType loadingType, Mission mission, IModalDataContainer modData);
		void InitializeReport(VectoRunData modelData, Segment segment);
		string Creator { get; set; }
		string JobName { get; set; }
	}

	/// <summary>
	/// Class for creating a declaration report.
	/// </summary>
	public abstract class DeclarationReport<T> : IDeclarationReport where T : new()
	{
		///// <summary>
		///// Container class for one mission and the modData for the different loadings.
		///// </summary>
		//protected class ResultContainer<TResultEntry>
		//{
		//	/// <summary>
		//	/// The mission
		//	/// </summary>
		//	public Mission Mission;

		//	/// <summary>
		//	/// Dictionary of LoadingTypes and DeclarationResults
		//	/// </summary>
		//	public Dictionary<LoadingType, TResultEntry> ModData;
		//}
		public class ResultContainer<TEntry>
		{
			//public Stream PDFPage;
			public Mission Mission;

			public MissionProfile MissionProfile;

			public Dictionary<LoadingType, TEntry> ModData;
		}

		public class MissionProfile
		{
			public readonly IList<double> DistanceKm;
			public readonly IList<double> TargetSpeed;
			public readonly IList<double> Altitude;

			public MissionProfile(IModalDataContainer m)
			{
				DistanceKm = m.GetValues<Meter>(ModalResultField.dist).Select(v => v.ConvertTo().Kilo.Meter).ToDouble();
				TargetSpeed =
					m.GetValues<MeterPerSecond>(ModalResultField.v_targ).Select(v => v.ConvertTo().Kilo.Meter.Per.Hour).ToDouble();
				Altitude = m.GetValues<Meter>(ModalResultField.altitude).ToDouble();
			}
		}

		/// <summary>
		/// Dictionary of MissionTypes and their corresponding results.
		/// </summary>
		protected readonly Dictionary<MissionType, ResultContainer<T>> Missions =
			new Dictionary<MissionType, ResultContainer<T>>();

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
		public string Creator { get; set; }

		/// <summary>
		/// The name of the job file (report name will be the same)
		/// </summary>
		public string JobName { get; set; }

		/// <summary>
		/// The result count determines how many results must be given before the report gets written.
		/// </summary>
		private int _resultCount;

		[MethodImpl(MethodImplOptions.Synchronized)]
		public void PrepareResult(LoadingType loading, Mission mission)
		{
			if (!Missions.ContainsKey(mission.MissionType)) {
				Missions[mission.MissionType] = new ResultContainer<T>() {
					Mission = mission,
					ModData = new Dictionary<LoadingType, T>()
				};
			}
			Missions[mission.MissionType].ModData[loading] = new T();
		}


		[MethodImpl(MethodImplOptions.Synchronized)]
		public void AddResult(LoadingType loadingType, Mission mission, IModalDataContainer modData)
		{
			if (modData.RunStatus != VectoRun.Status.Success) {
				//Missions.Clear();
				return;
			}
			if (!Missions.ContainsKey(mission.MissionType)) {
				throw new VectoException("Unknown mission type {0} for generating declaration report", mission.MissionType);
			}
			if (!Missions[mission.MissionType].ModData.ContainsKey(loadingType)) {
				throw new VectoException("Unknown loading type {0} for mission {1}", loadingType, mission.MissionType);
			}
			if (Missions[mission.MissionType].MissionProfile == null) {
				Missions[mission.MissionType].MissionProfile = new MissionProfile(modData);
			}
			_resultCount--;

			DoAddResult(Missions[mission.MissionType].ModData[loadingType], loadingType, mission, modData);

			if (_resultCount == 0) {
				DoWriteReport();
				Flc = null;
				Segment = null;
			}
		}

		/// <summary>
		/// Adds the result of one run for the specific mission and loading. If all runs finished (given by the resultCount) the report will be written.
		/// </summary>
		/// <param name="entry"></param>
		/// <param name="loadingType">Type of the loading.</param>
		/// <param name="mission">The mission.</param>
		/// <param name="modData">The mod data.</param>
		[MethodImpl(MethodImplOptions.Synchronized)]
		protected abstract void DoAddResult(T entry, LoadingType loadingType, Mission mission, IModalDataContainer modData);


		protected internal abstract void DoWriteReport();


		public void InitializeReport(VectoRunData modelData, Segment segment)
		{
			Segment = segment;
			_resultCount = segment.Missions.Sum(m => m.Loadings.Count);

			DoInitializeReport(modelData, segment);
		}

		protected abstract void DoInitializeReport(VectoRunData modelData, Segment segment);
	}
}