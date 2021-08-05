/*
* This file is part of VECTO.
*
* Copyright © 2012-2019 European Union
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
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Xml;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using Newtonsoft.Json;
using Ninject;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using TUGraz.VectoCore.InputData.Reader.Impl;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.FileIO;
using TUGraz.VectoCore.OutputData.ModFilter;
using TUGraz.VectoCore.OutputData.XML;
using Formatting = Newtonsoft.Json.Formatting;

namespace TUGraz.VectoCore.Models.Simulation.Impl
{
	public class SimulatorFactory : LoggingObject, ISimulatorFactory
	{
		private static int _jobNumberCounter;
		
		private readonly ExecutionMode _mode;

		private Func<ISimulatorFactory> _followingSimulatorFactoryCreator = null;

		private bool _simulate = true;

		public ISimulatorFactory FollowUpSimulatorFactory
		{
			get => CreateFollowUpSimulatorFactory ? _followingSimulatorFactoryCreator?.Invoke() : null;
		}

		public bool CreateFollowUpSimulatorFactory = false;
		



		public SimulatorFactory(ExecutionMode mode, IInputDataProvider dataProvider, IOutputDataWriter writer) : this(mode, dataProvider, writer, null, null, true)
		{
			
		}

		public SimulatorFactory(ExecutionMode mode, IInputDataProvider dataProvider, IOutputDataWriter writer,
			IDeclarationReport declarationReport = null, IVTPReport vtpReport = null, bool validate = true)
		{
			System.Diagnostics.Debug.WriteLine("Created Simulator Factory");
			Log.Info("########## VectoCore Version {0} ##########", Assembly.GetExecutingAssembly().GetName().Version);
			JobNumber = Interlocked.Increment(ref _jobNumberCounter);
			_mode = mode;
			ReportWriter = writer;
			Validate = validate;

			ThreadPool.GetMinThreads(out var workerThreads, out var completionThreads);
			if (workerThreads < 12) {
				workerThreads = 12;
			}
			ThreadPool.SetMinThreads(workerThreads, completionThreads);

			switch (mode) {
				case ExecutionMode.Declaration:
					CreateDeclarationDataReader(dataProvider, declarationReport, vtpReport);
					break;
				case ExecutionMode.Engineering:
					CreateEngineeringDataReader(dataProvider);
					break;
				default:
					throw new VectoException("Unkown factory mode in SimulatorFactory: {0}", mode);
			}
		}

		private void CreateDeclarationDataReader(IInputDataProvider dataProvider, IDeclarationReport declarationReport, IVTPReport vtpReport)
		{
			if (dataProvider is IVTPDeclarationInputDataProvider vtpProvider) {
				var report = vtpReport ?? new XMLVTPReport(ReportWriter);
				if (vtpProvider.JobInputData.Vehicle.VehicleCategory.IsLorry()) {
					DataReader = new DeclarationVTPModeVectoRunDataFactoryLorries(vtpProvider, report);
				}
				if (vtpProvider.JobInputData.Vehicle.VehicleCategory.IsBus()) {
					DataReader = new DeclarationVTPModeVectoRunDataFactoryHeavyBusPrimary(vtpProvider, report);
				}
				return;
			}

			if (dataProvider is ISingleBusInputDataProvider) {
				var singleBus = dataProvider as ISingleBusInputDataProvider;
				var report = declarationReport ?? new XMLDeclarationReport(ReportWriter);
				DataReader = new DeclarationModeSingleBusVectoRunDataFactory(singleBus, report);
				return;
			}
			if (dataProvider is IDeclarationInputDataProvider declDataProvider) {
				if (declDataProvider.JobInputData.Vehicle.VehicleCategory.IsLorry()) {
					var report = declarationReport ?? new XMLDeclarationReport(ReportWriter);
					DataReader = new DeclarationModeTruckVectoRunDataFactory(declDataProvider, report);
					return;
				}

				switch (declDataProvider.JobInputData.Vehicle.VehicleCategory) {
					case VehicleCategory.HeavyBusCompletedVehicle:
						var reportCompleted = declarationReport ??
											new XMLDeclarationReportCompletedVehicle(ReportWriter,
												declDataProvider.JobInputData.Vehicle.VehicleCategory == VehicleCategory.HeavyBusPrimaryVehicle) {
												PrimaryVehicleReportInputData = declDataProvider.PrimaryVehicleData,
						};
						DataReader = new DeclarationModeCompletedBusVectoRunDataFactory(declDataProvider, reportCompleted);
						return;
					case VehicleCategory.HeavyBusPrimaryVehicle:
						var reportPrimary = declarationReport ??
											new XMLDeclarationReportPrimaryVehicle(ReportWriter,
												declDataProvider.JobInputData.Vehicle.VehicleCategory == VehicleCategory.HeavyBusPrimaryVehicle);
						DataReader = new DeclarationModePrimaryBusVectoRunDataFactory(declDataProvider, reportPrimary);
						return;
					default:
						System.Diagnostics.Debug.Assert(false);
						break;
				}
			}

			if (dataProvider is IMultistageVIFInputData multistageVifInputData) {
				//ToDo FK: check if data completed == true && final 
				var inputComplete = multistageVifInputData.MultistageJobInputData.JobInputData.InputComplete;
				var declType = multistageVifInputData.MultistageJobInputData.JobInputData.ConsolidateManufacturingStage
					?.Vehicle.VehicleDeclarationType;
				var final = declType == VehicleDeclarationType.final;
				var exempted = multistageVifInputData.MultistageJobInputData.JobInputData.ConsolidateManufacturingStage?
					.Vehicle.ExemptedVehicle == true;


				if (multistageVifInputData.VehicleInputData == null) { // eigener writer für in-memory
					var reportCompleted = new XMLDeclarationReportCompletedVehicle(ReportWriter, true) {
						PrimaryVehicleReportInputData = multistageVifInputData.MultistageJobInputData.JobInputData.PrimaryVehicle,
					};
					DataReader = new DeclarationModeCompletedMultistageBusVectoRunDataFactory(
						multistageVifInputData.MultistageJobInputData,
						reportCompleted);
					if (!((final || exempted) && inputComplete)) {
						_simulate = false;
						//_followingSimulatorFactoryCreator = () => {
						//	var container = new StandardKernel(
						//		new VectoNinjectModule()
						//	);
						//	var inputDataReader = container.Get<IXMLInputDataReader>();

						//	var mode = _mode;
						//	var inputData =
						//		inputDataReader.CreateDeclaration(((FileOutputWriter)ReportWriter)
						//			.XMLMultistageReportFileName);

						//	return new SimulatorFactory(
						//		mode: _mode,
						//		dataProvider: new XMLDeclarationVIFInputData(
						//			inputData as IMultistageBusInputDataProvider, null),
						//		writer: ReportWriter,
						//		declarationReport: reportCompleted, 
						//		vtpReport: vtpReport,
						//		validate: Validate);

						//};
					}

				} else {
					var report = declarationReport ?? new XMLDeclarationReportMultistageBusVehicle(ReportWriter);
					DataReader = new DeclarationModeMultistageBusVectoRunDataFactory(multistageVifInputData, report);

					_followingSimulatorFactoryCreator = () => {
							var container = new StandardKernel(
								new VectoNinjectModule()
							);
							var inputDataReader = container.Get<IXMLInputDataReader>();

							var mode = _mode;
							var inputData =
								inputDataReader.CreateDeclaration(((FileOutputWriter)ReportWriter)
									.XMLMultistageReportFileName);
							return new SimulatorFactory(_mode, new XMLDeclarationVIFInputData(inputData as IMultistageBusInputDataProvider, null), ReportWriter, report, vtpReport, Validate) {
								
							};
					};

				}
				return;
			}

			if (dataProvider is IMultistagePrimaryAndStageInputDataProvider multiStagePrimaryAndStageInputData)
			{
				System.Diagnostics.Debug.Assert(multiStagePrimaryAndStageInputData.PrimaryVehicle.JobInputData.Vehicle.VehicleCategory == VehicleCategory.HeavyBusPrimaryVehicle);

				var tempOutputWriter = new TempFileOutputWriter(ReportWriter.JobFile, ReportType.DeclarationReportManufacturerXML);
				var originalReportWriter = ReportWriter;
				ReportWriter = tempOutputWriter;

				var tempPrimaryReport = new XMLDeclarationReportPrimaryVehicle(tempOutputWriter, true);
				
				DataReader = new DeclarationModePrimaryBusVectoRunDataFactory(multiStagePrimaryAndStageInputData.PrimaryVehicle, tempPrimaryReport);


				var reportPrimary = new XMLDeclarationReportPrimaryVehicle(ReportWriter,
										true);
				


				CreateFollowUpSimulatorFactory = true;
				_followingSimulatorFactoryCreator = (() => {
					//replace with dependency injection 
					var container = new StandardKernel(
						new VectoNinjectModule()
					);
					try {
						var inputDataReader = container.Get<IXMLInputDataReader>();
						var primaryInputData = inputDataReader.CreateDeclaration(tempOutputWriter
							.GetDocument(ReportType.DeclarationReportPrimaryVehicleXML).CreateReader());
						//var primaryInputData = inputDataReader.CreateDeclaration(((FileOutputWriter)ReportWriter).XMLPrimaryVehicleReportName);
						var vifInputData = new XMLDeclarationVIFInputData(
							primaryInputData as IMultistageBusInputDataProvider,
							multiStagePrimaryAndStageInputData.StageInputData);

						var manStagesCount =
							vifInputData.MultistageJobInputData.JobInputData.ManufacturingStages?.Count ?? -1;

						originalReportWriter.NumberOfManufacturingStages = manStagesCount;
						var factory = new SimulatorFactory(_mode,
							vifInputData, originalReportWriter,
							null,
							vtpReport,
							Validate) {
							CreateFollowUpSimulatorFactory = true,
						};
						return factory;
					} catch (Exception ex) {
						Log.Error($"Failed to create additional Simulation run: {ex.Message}");
						return null;
					}
				});
				return;
			}


			throw new VectoException("Unknown InputData for Declaration Mode!");
		}


		private void CreateEngineeringDataReader(IInputDataProvider dataProvider)
		{
			if (dataProvider is IVTPEngineeringInputDataProvider) {
				var vtpProvider = dataProvider as IVTPEngineeringInputDataProvider;
				if (vtpProvider.JobInputData.Vehicle.VehicleCategory.IsLorry()) {
					DataReader = new EngineeringVTPModeVectoRunDataFactoryLorries(vtpProvider);
				}
				if (vtpProvider.JobInputData.Vehicle.VehicleCategory.IsBus()) {
					DataReader = new EngineeringVTPModeVectoRunDataFactoryHeavyBusPrimary(vtpProvider);
				}
				return;
			}
			if (dataProvider is IEngineeringInputDataProvider) {
				var engDataProvider = dataProvider as IEngineeringInputDataProvider;
				if (engDataProvider.JobInputData.JobType == VectoSimulationJobType.EngineOnlySimulation) {
					DataReader = new EngineOnlyVectoRunDataFactory(engDataProvider);
				} else {
					DataReader = new EngineeringModeVectoRunDataFactory(engDataProvider);
				}
				return;
			}
			throw  new VectoException("Unknown InputData for Engineering Mode!");
		}

		public bool Validate { get; set; }

		public IVectoRunDataFactory DataReader { get; private set; }

		public SummaryDataContainer SumData { get; set; }

		public IOutputDataWriter ReportWriter { get; private set; }

		public int JobNumber { get; set; }

		public bool WriteModalResults { get; set; }
		public bool ModalResults1Hz { get; set; }
		public bool ActualModalData { get; set; }

		public bool SerializeVectoRunData { get; set; }

		/// <summary>
		/// Creates powertrain and initializes it with the component's data.
		/// </summary>
		/// <returns>new VectoRun Instance</returns>
		public IEnumerable<IVectoRun> SimulationRuns()
		{
			var i = 0;
			var warning1Hz = false;
			if (!_simulate) {
				yield break;
			}
			foreach (var data in DataReader.NextRun()) {
				var current = i++;
				var d = data;
				data.JobRunId = current;
				yield return data.Exempted || data.MultistageRun ? GetExemptedRun(data) : GetNonExemptedRun(data, current, d, ref warning1Hz);
			}
		}

		private IVectoRun GetExemptedRun(VectoRunData data)
		{
			if (data.Report != null) {
				data.Report.PrepareResult(data.Loading, data.Mission, data.EngineData?.FuelMode ?? 0, data);
			}
			return new ExemptedRun(new ExemptedRunContainer(data.ExecutionMode) { RunData = data }, modData => {
				if (data.Report != null) {
					data.Report.AddResult(data.Loading, data.Mission, data.EngineData?.FuelMode ?? 0, data, modData);
				}
			});
		}

		private IVectoRun GetNonExemptedRun(VectoRunData data, int current, VectoRunData d, ref bool warning1Hz)
		{
			var addReportResult = PrepareReport(data);
			if (!data.Cycle.CycleType.IsDistanceBased() && ModalResults1Hz && !warning1Hz) {
				Log.Error("Output filter for 1Hz results is only available for distance-based cycles!");
				warning1Hz = true;
			}
			var fuels = data.EngineData != null ? data.EngineData.Fuels.Select(x => x.FuelData).ToList() : new List<IFuelProperties>();
			IModalDataContainer modContainer =
				new ModalDataContainer(
					data, ReportWriter,
					_mode == ExecutionMode.Declaration ? addReportResult : null,
					GetModDataFilter(data)) {
					WriteModalResults = _mode != ExecutionMode.Declaration || WriteModalResults
				};


			// TODO: MQ 20200410 - Remove for official release!
			if (SerializeVectoRunData) {
				File.WriteAllText(
					Path.Combine(
						(ReportWriter as FileOutputWriter)?.BasePath ?? "", $"{data.JobName}_{data.Cycle.Name}{data.ModFileSuffix}.json"),
					JsonConvert.SerializeObject(data, Formatting.Indented));
			}

			var builder = new PowertrainBuilder(
				modContainer, modData => {
					if (SumData != null) {
						SumData.Write(modData, JobNumber, current, d);
					}
				});

			var run = GetVectoRun(data, builder);

			if (Validate) {
				ValidateVectoRunData(
					run, data.JobType, data.ElectricMachinesData.FirstOrDefault()?.Item1, data.GearboxData?.Type,
					data.Mission != null && data.Mission.MissionType.IsEMS());
			}
			return run;
		}

		private IModalDataFilter[] GetModDataFilter(VectoRunData data)
		{
			var modDataFilter = ModalResults1Hz
				? new IModalDataFilter[] { new ModalData1HzFilter() }
				: null;

			if (ActualModalData) {
				modDataFilter = new IModalDataFilter[] { new ActualModalDataFilter(), };
			}
			return data.Cycle.CycleType.IsDistanceBased() && ModalResults1Hz || ActualModalData ? modDataFilter : null;
		}

		private void ValidateVectoRunData(VectoRun run, VectoSimulationJobType jobType, PowertrainPosition? emPosition, GearboxType? gearboxtype, bool isEms)
		{
			var validationErrors = run.Validate(_mode, jobType, emPosition, gearboxtype, isEms);
			if (validationErrors.Any()) {
				throw new VectoException("Validation of Run-Data Failed: " +
										string.Join("\n", validationErrors.Select(r => r.ErrorMessage + string.Join("; ", r.MemberNames))));
			}
		}

		private static VectoRun GetVectoRun(VectoRunData data, PowertrainBuilder builder)
		{
			VectoRun run;
			switch (data.Cycle.CycleType) {
				case CycleType.DistanceBased:
					if ((data.SimulationType & SimulationType.DistanceCycle) == 0) {
						throw new VectoException("Distance-based cycle can not be simulated in {0} mode", data.SimulationType);
					}
					run = new DistanceRun(builder.Build(data));
					break;
				case CycleType.EngineOnly:
					if ((data.SimulationType & SimulationType.EngineOnly) == 0) {
						throw new VectoException("Engine-only cycle can not be simulated in {0} mode", data.SimulationType);
					}
					run = new TimeRun(builder.Build(data));
					break;
				case CycleType.VTP:
					if ((data.SimulationType & SimulationType.VerificationTest) == 0) {
						throw new VectoException("VTP-cycle can not be simulated in {0} mode", data.SimulationType);
					}
					run = new TimeRun(builder.Build(data));
					break;

				case CycleType.PWheel:
				case CycleType.MeasuredSpeed:
				case CycleType.MeasuredSpeedGear:
					if ((data.SimulationType & (SimulationType.PWheel | SimulationType.MeasuredSpeedCycle)) == 0) {
						throw new VectoException("{1}-cycle can not be simulated in {0} mode", data.SimulationType, data.Cycle.CycleType);
					}
					run = new TimeRun(builder.Build(data));
					break;
				case CycleType.PTO:
					throw new VectoException("PTO Cycle can not be used as main cycle!");
				default:
					throw new ArgumentOutOfRangeException("CycleType unknown:" + data.Cycle.CycleType);
			}
			return run;
		}

		private static Action<ModalDataContainer> PrepareReport(VectoRunData data)
		{
			if (data.Report != null) {
				data.Report.PrepareResult(data.Loading, data.Mission, data.EngineData.FuelMode, data);
			}
			Action<ModalDataContainer> addReportResult = modData => {
				if (data.Report != null) {
					data.Report.AddResult(data.Loading, data.Mission, data.EngineData.FuelMode, data, modData);
				}
			};
			return addReportResult;
		}
	}
}

