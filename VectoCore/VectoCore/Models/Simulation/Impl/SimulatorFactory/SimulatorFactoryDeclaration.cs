using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Ninject;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider;
using TUGraz.VectoCore.InputData.Reader.Impl;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.FileIO;
using TUGraz.VectoCore.OutputData.XML;

namespace TUGraz.VectoCore.Models.Simulation.Impl.SimulatorFactory
{
    public class SimulatorFactoryDeclaration : SimulatorFactory
    {
		//private readonly ISimulatorFactoryFactory _simFactoryFactory;

		public SimulatorFactoryDeclaration(IInputDataProvider dataProvider, 
			IOutputDataWriter writer,
			IDeclarationReport declarationReport, 
			IVTPReport vtpReport,
			bool validate) : base(ExecutionMode.Declaration, writer, validate)
		{
			_simulate = CanBeSimulated(dataProvider);
			CreateDeclarationDataReader(dataProvider, declarationReport, vtpReport);
		}

		private bool CanBeSimulated(IInputDataProvider dataProvider)
		{
			if (dataProvider is IMultistageVIFInputData multistageVifInputData &&
				multistageVifInputData.VehicleInputData == null) {
				var inputComplete = multistageVifInputData.MultistageJobInputData.JobInputData.InputComplete;
				var declType = multistageVifInputData.MultistageJobInputData.JobInputData.ConsolidateManufacturingStage
					?.Vehicle.VehicleDeclarationType;
				var final = declType == VehicleDeclarationType.final;
				var exempted = multistageVifInputData.MultistageJobInputData.JobInputData.ConsolidateManufacturingStage?
					.Vehicle.ExemptedVehicle == true;

				if (!((final || exempted) && inputComplete)) {
					return false;
				}
			}


			return true;
		}

		private void CreateDeclarationDataReader(IInputDataProvider dataProvider, IDeclarationReport declarationReport, IVTPReport vtpReport)
		{
			switch (dataProvider) {
				case IVTPDeclarationInputDataProvider vtpProvider: {
					DataReader = CreateRunDataReader(vtpProvider, vtpReport);
					return;
				}
				case ISingleBusInputDataProvider singleBusProvider: {
					DataReader = CreateRunDataReader(singleBusProvider, declarationReport);
					return;
				}
				case IDeclarationInputDataProvider declDataProvider: {
					DataReader = CreateRunDataReader(declDataProvider, declarationReport);
					return;
				}
				case IMultistageVIFInputData multistageVifInputData: {
					if (multistageVifInputData.VehicleInputData == null)
					{
						var reportCompleted = new XMLDeclarationReportCompletedVehicle(ReportWriter, true)
						{
							PrimaryVehicleReportInputData = multistageVifInputData.MultistageJobInputData.JobInputData.PrimaryVehicle,
						};
						DataReader = new DeclarationModeCompletedMultistageBusVectoRunDataFactory(
							multistageVifInputData.MultistageJobInputData,
							reportCompleted);

						

					}
					else
					{
						var report = declarationReport ?? new XMLDeclarationReportMultistageBusVehicle(ReportWriter);

						//DataReader = CreateRunDataReader(multistageVifInputData, report);
						DataReader = new DeclarationModeMultistageBusVectoRunDataFactory(multistageVifInputData, report);

						_followingSimulatorFactoryCreator = () => {
							var container = new StandardKernel(
								new VectoNinjectModule()
							);
							var inputDataReader = container.Get<IXMLInputDataReader>();
							var inputData =
								inputDataReader.CreateDeclaration(
									XmlReader.Create(ReportWriter.MultistageXmlReport.ToString().ToStream()));
#pragma warning disable 618
							return CreateSimulatorFactory(_mode, new XMLDeclarationVIFInputData(inputData as IMultistageBusInputDataProvider, null), ReportWriter, report, vtpReport, Validate);
#pragma warning restore 618
						};

					}
					return;
				}
				case IMultistagePrimaryAndStageInputDataProvider multiStagePrimaryAndStageInputData: {
					System.Diagnostics.Debug.Assert(multiStagePrimaryAndStageInputData.PrimaryVehicle.JobInputData.Vehicle.VehicleCategory == VehicleCategory.HeavyBusPrimaryVehicle);

					var tempOutputWriter = new TempFileOutputWriter(ReportWriter, ReportType.DeclarationReportManufacturerXML);
					var originalReportWriter = ReportWriter;
					ReportWriter = tempOutputWriter;

					var tempPrimaryReport = new XMLDeclarationReportPrimaryVehicle(tempOutputWriter, true);


					DataReader = CreateRunDataReader(multiStagePrimaryAndStageInputData.PrimaryVehicle, tempPrimaryReport);
					//DataReader = new DeclarationModePrimaryBusVectoRunDataFactory(multiStagePrimaryAndStageInputData.PrimaryVehicle, tempPrimaryReport);


					CreateFollowUpSimulatorFactory = true;
					_followingSimulatorFactoryCreator = (() => {
						//replace with dependency injection 
						var container = new StandardKernel(
							new VectoNinjectModule()
						);
						try
						{
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
#pragma warning disable 618
							var factory = CreateSimulatorFactory(_mode,
#pragma warning restore 618
								vifInputData, originalReportWriter,
								null,
								vtpReport,
								Validate);
							factory.CreateFollowUpSimulatorFactory = true;
							return factory;
						}
						catch (Exception ex)
						{
							Log.Error($"Failed to create additional Simulation run: {ex.Message}");
							return null;
						}
					});
					return;
				}
				default:
					throw new VectoException("Unknown InputData for Declaration Mode!");
			}
		}

		private IVectoRunDataFactory CreateRunDataReader(IDeclarationInputDataProvider declDataProvider,
			IDeclarationReport declarationReport)
		{
			var vehicleCategory = declDataProvider.JobInputData.Vehicle.VehicleCategory;
			if(vehicleCategory.IsLorry()) {
				var report = declarationReport ?? new XMLDeclarationReport(ReportWriter);
				return new DeclarationModeTruckVectoRunDataFactory(declDataProvider, report);
			}

			if(vehicleCategory.IsBus())
				switch (declDataProvider.JobInputData.Vehicle.VehicleCategory)
				{
					case VehicleCategory.HeavyBusCompletedVehicle:
						var reportCompleted = declarationReport ??
											new XMLDeclarationReportCompletedVehicle(ReportWriter,
												declDataProvider.JobInputData.Vehicle.VehicleCategory == VehicleCategory.HeavyBusPrimaryVehicle)
											{
												PrimaryVehicleReportInputData = declDataProvider.PrimaryVehicleData,
											};
						return new DeclarationModeCompletedBusVectoRunDataFactory(declDataProvider, reportCompleted);
					case VehicleCategory.HeavyBusPrimaryVehicle:
						var reportPrimary = declarationReport ??
											new XMLDeclarationReportPrimaryVehicle(ReportWriter,
												declDataProvider.JobInputData.Vehicle.VehicleCategory == VehicleCategory.HeavyBusPrimaryVehicle);
						return new DeclarationModePrimaryBusVectoRunDataFactory(declDataProvider, reportPrimary);
						
					default:

						break;
				}

			throw new Exception(
				$"Could not create RunDataFactory for Vehicle Category{vehicleCategory}");
		}

		private IVectoRunDataFactory CreateRunDataReader(ISingleBusInputDataProvider singleBusProvider,
			IDeclarationReport declarationReport)
		{
			var report = declarationReport ?? new XMLDeclarationReport(ReportWriter);
			return new DeclarationModeSingleBusVectoRunDataFactory(singleBusProvider, report);
		}

		private IVectoRunDataFactory CreateRunDataReader(IVTPDeclarationInputDataProvider vtpProvider,
			IVTPReport vtpReport)
		{
			var report = vtpReport ?? new XMLVTPReport(ReportWriter);
			if (vtpProvider.JobInputData.Vehicle.VehicleCategory.IsLorry()) {
				return new DeclarationVTPModeVectoRunDataFactoryLorries(vtpProvider, report);
			}

			if (vtpProvider.JobInputData.Vehicle.VehicleCategory.IsBus()) {
				return new DeclarationVTPModeVectoRunDataFactoryHeavyBusPrimary(vtpProvider, report);
			}

			throw new Exception(
				$"Could not create RunDataFactory for Vehicle Category{vtpProvider.JobInputData.Vehicle.VehicleCategory}");
		}

		#region Overrides of SimulatorFactory

		public override IOutputDataWriter ReportWriter { get; protected set; }

		#endregion
	}
}
