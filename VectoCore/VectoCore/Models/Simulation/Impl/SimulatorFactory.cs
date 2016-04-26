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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Threading;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.InputData;
using TUGraz.VectoCore.InputData.Reader.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.PDF;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.Simulation.Impl
{
	public class SimulatorFactory : LoggingObject
	{
		private static int _jobNumberCounter;

		private readonly ExecutionMode _mode;

		public SimulatorFactory(ExecutionMode mode, IInputDataProvider dataProvider, IOutputDataWriter writer,
			DeclarationReport report = null)
		{
			Log.Fatal("########## VectoCore Version {0} ##########", Assembly.GetExecutingAssembly().GetName().Version);
			JobNumber = Interlocked.Increment(ref _jobNumberCounter);
			_mode = mode;
			ModWriter = writer;
			switch (mode) {
				case ExecutionMode.Declaration:
					var declDataProvider = dataProvider as IDeclarationInputDataProvider;
					if (declDataProvider == null) {
						throw new VectoException("InputDataProvider does not implement DeclarationData interface");
					}
					report = report ?? new PDFDeclarationReport(writer);
					var windowsIdentity = WindowsIdentity.GetCurrent();
					report.Creator = windowsIdentity != null ? windowsIdentity.Name : "N/A";
					report.JobName = declDataProvider.JobInputData().JobName;
					DataReader = new DeclarationModeVectoRunDataFactory(declDataProvider, report);
					break;
				case ExecutionMode.Engineering:
					var engDataProvider = dataProvider as IEngineeringInputDataProvider;
					if (engDataProvider == null) {
						throw new VectoException("InputDataProvider does not implement Engineering interface");
					}
					DataReader = new EngineeringModeVectoRunDataFactory(engDataProvider);
					break;
				case ExecutionMode.EngineOnly:
					var engineDataProvider = dataProvider as IEngineeringInputDataProvider;
					if (engineDataProvider == null) {
						throw new VectoException("InputDataProvider does not implement Engineering interface");
					}
					DataReader = new EngineOnlyVectoRunDataFactory(engineDataProvider);
					break;
				default:
					throw new VectoException("Unkown factory mode in SimulatorFactory: {0}", mode);
			}
		}

		public IVectoRunDataFactory DataReader { get; private set; }

		public SummaryDataContainer SumData { get; set; }

		public IOutputDataWriter ModWriter { get; private set; }


		public int JobNumber { get; set; }

		public bool WriteModalResults { get; set; }

		/// <summary>
		/// Creates powertrain and initializes it with the component's data.
		/// </summary>
		/// <returns>new VectoRun Instance</returns>
		public IEnumerable<IVectoRun> SimulationRuns()
		{
			var i = 0;
			foreach (var data in DataReader.NextRun()) {
				var d = data;
				IModalDataContainer modContainer =
					new ModalDataContainer(data, ModWriter,
						writer => {
							if (d.Report != null) {
								d.Report.AddResult(d.Loading, d.Mission, writer);
							}
						}, _mode) {
							WriteAdvancedAux = data.AdvancedAux != null && data.AdvancedAux.AuxiliaryAssembly == AuxiliaryModel.Advanced
						};
				modContainer.WriteModalResults = WriteModalResults;
				var current = i++;
				var builder = new PowertrainBuilder(modContainer, (writer, mass, loading) =>
					SumData.Write(modContainer, d.JobName, string.Format("{0}-{1}", JobNumber, current),
						d.Cycle.Name + Constants.FileExtensions.CycleFile, mass, loading));

				VectoRun run;

				switch (data.Cycle.CycleType) {
					case CycleType.DistanceBased:
						run = new DistanceRun(builder.Build(data));
						break;
					case CycleType.EngineOnly:
					case CycleType.PWheel:
					case CycleType.MeasuredSpeed:
					case CycleType.MeasuredSpeedGear:
						run = new TimeRun(builder.Build(data));
						break;
					default:
						throw new ArgumentOutOfRangeException("CycleType unknown:" + data.Cycle.CycleType);
				}

				var validationErrors = run.Validate();
				if (validationErrors.Any()) {
					throw new VectoException("Validation of Run-Data Failed: " + "\n".Join(validationErrors.Select(r => r.ErrorMessage)));
				}


				yield return run;
			}
		}
	}
}