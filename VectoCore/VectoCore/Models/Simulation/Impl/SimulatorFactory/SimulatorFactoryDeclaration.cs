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
			CreateDeclarationDataReader(dataProvider, declarationReport, vtpReport);
			//_simFactoryFactory = simFactoryFactory;
		}

		private void CreateDeclarationDataReader(IInputDataProvider dataProvider, IDeclarationReport declarationReport, IVTPReport vtpReport)
		{
			if (dataProvider is IVTPDeclarationInputDataProvider vtpProvider)
			{
				var report = vtpReport ?? new XMLVTPReport(ReportWriter);
				if (vtpProvider.JobInputData.Vehicle.VehicleCategory.IsLorry())
				{
					DataReader = new DeclarationVTPModeVectoRunDataFactoryLorries(vtpProvider, report);
				}
				if (vtpProvider.JobInputData.Vehicle.VehicleCategory.IsBus())
				{
					DataReader = new DeclarationVTPModeVectoRunDataFactoryHeavyBusPrimary(vtpProvider, report);
				}
				return;
			}

			if (dataProvider is ISingleBusInputDataProvider)
			{
				var singleBus = dataProvider as ISingleBusInputDataProvider;
				var report = declarationReport ?? new XMLDeclarationReport(ReportWriter);
				DataReader = new DeclarationModeSingleBusVectoRunDataFactory(singleBus, report);
				return;
			}
			if (dataProvider is IDeclarationInputDataProvider declDataProvider)
			{
				if (declDataProvider.JobInputData.Vehicle.VehicleCategory.IsLorry())
				{
					var report = declarationReport ?? new XMLDeclarationReport(ReportWriter);
					DataReader = new DeclarationModeTruckVectoRunDataFactory(declDataProvider, report);
					return;
				}

				switch (declDataProvider.JobInputData.Vehicle.VehicleCategory)
				{
					case VehicleCategory.HeavyBusCompletedVehicle:
						var reportCompleted = declarationReport ??
											new XMLDeclarationReportCompletedVehicle(ReportWriter,
												declDataProvider.JobInputData.Vehicle.VehicleCategory == VehicleCategory.HeavyBusPrimaryVehicle)
											{
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

			if (dataProvider is IMultistageVIFInputData multistageVifInputData)
			{
				//ToDo FK: check if data completed == true && final 
				var inputComplete = multistageVifInputData.MultistageJobInputData.JobInputData.InputComplete;
				var declType = multistageVifInputData.MultistageJobInputData.JobInputData.ConsolidateManufacturingStage
					?.Vehicle.VehicleDeclarationType;
				var final = declType == VehicleDeclarationType.final;
				var exempted = multistageVifInputData.MultistageJobInputData.JobInputData.ConsolidateManufacturingStage?
					.Vehicle.ExemptedVehicle == true;


				if (multistageVifInputData.VehicleInputData == null)
				{ // eigener writer für in-memory
					var reportCompleted = new XMLDeclarationReportCompletedVehicle(ReportWriter, true)
					{
						PrimaryVehicleReportInputData = multistageVifInputData.MultistageJobInputData.JobInputData.PrimaryVehicle,
					};
					DataReader = new DeclarationModeCompletedMultistageBusVectoRunDataFactory(
						multistageVifInputData.MultistageJobInputData,
						reportCompleted);
					if (!((final || exempted) && inputComplete))
					{
						_simulate = false;
					}

				}
				else
				{
					var report = declarationReport ?? new XMLDeclarationReportMultistageBusVehicle(ReportWriter);
					DataReader = new DeclarationModeMultistageBusVectoRunDataFactory(multistageVifInputData, report);

					_followingSimulatorFactoryCreator = () => {
						var container = new StandardKernel(
							new VectoNinjectModule()
						);
						var inputDataReader = container.Get<IXMLInputDataReader>();

						var mode = _mode;
						var inputData =
							inputDataReader.CreateDeclaration(
								XmlReader.Create(ReportWriter.MultistageXmlReport.ToString().ToStream()));
						//inputDataReader.CreateDeclaration(((FileOutputWriter)ReportWriter)
						//	.XMLMultistageReportFileName);
#pragma warning disable 618
						return CreateSimulatorFactory(_mode, new XMLDeclarationVIFInputData(inputData as IMultistageBusInputDataProvider, null), ReportWriter, report, vtpReport, Validate);
#pragma warning restore 618
					};

				}
				return;
			}

			if (dataProvider is IMultistagePrimaryAndStageInputDataProvider multiStagePrimaryAndStageInputData)
			{
				System.Diagnostics.Debug.Assert(multiStagePrimaryAndStageInputData.PrimaryVehicle.JobInputData.Vehicle.VehicleCategory == VehicleCategory.HeavyBusPrimaryVehicle);

				var tempOutputWriter = new TempFileOutputWriter(ReportWriter, ReportType.DeclarationReportManufacturerXML);
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


			throw new VectoException("Unknown InputData for Declaration Mode!");
		}

		#region Overrides of SimulatorFactory

		public override IOutputDataWriter ReportWriter { get; protected set; }

		#endregion
	}
}
