using System.IO;
using System.Linq;
using System.Threading;
using System.Xml;
using Moq;
using Ninject;
using NUnit.Framework;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider.v24;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Reader.Impl;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.Simulation.Impl.SimulatorFactory;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.FileIO;
using TUGraz.VectoCore.Tests.Integration.CompletedBus;
using TUGraz.VectoCore.Tests.Utils;
using TUGraz.VectoCore.Tests.Utils.Ninject;

namespace TUGraz.VectoCore.Tests.Integration.IMC;

public class IMC_DeclarationTest
{
	//private ThreadLocal<StandardKernel> _kernel;
	//private IXMLInputDataReader _xmlReader;


	public const string PEV_IMC_Grp5_all = "TestData/IMC/Declaration/BEV_E2_Group5_2030/BEV_Group5_IMC.vecto";
	public const string HEV_IMC_Grp5_all = "TestData/IMC/Declaration/P2_PHEV_Group5_2030/P2_Group5_IMC.vecto";

	public const string PEV_PrimaryBus_IMC = "TestData/Integration/IMC/PEV_primaryBus_AMT_E2.xml";
	public const string PEV_CompleteBus_IMC = "TestData/Integration/IMC/PEV_completedBus_2.xml";

 //   public StandardKernel Kernel {
	//	get => _kernel.Value;
	//}

    [OneTimeSetUp]
	public void OneTimeSetup()
	{
		//_kernel = new ThreadLocal<StandardKernel>(() => new StandardKernel(new VectoNinjectModule()));
		//_xmlReader = Kernel.Get<IXMLInputDataReader>();
	}


    [TestCase(PEV_IMC_Grp5_all, 3, TestName = "PEV IMC All Declaration Grp5 RD RL")]
	public void RunJob_PEV_IMC_Declaration(string jobFile, int runIdx)
	{
		var inputProvider = JSONInputDataFactory.ReadJsonJob(jobFile);
		var writer = new FileOutputWriter(jobFile);
		var factory = SimulatorFactory.CreateSimulatorFactory(ExecutionMode.Declaration, inputProvider, writer);
		factory.Validate = false;
		factory.WriteModalResults = true;
		factory.SumData = new SummaryDataContainer(writer);
		var run = factory.SimulationRuns().ToArray()[runIdx];
		var modData = ((ModalDataContainer)((VehicleContainer)run.GetContainer()).ModData).Data;

		run.Run();
		Assert.IsTrue(run.FinishedWithoutErrors);
	}

	[TestCase(HEV_IMC_Grp5_all, MissionType.RegionalDelivery, LoadingType.ReferenceLoad, TestName = "P-HEV IMC All Declaration Grp5 RD RL")]
	public void RunJob_PHEV_IMC_Declaration(string jobFile, MissionType? mission = null, LoadingType? loading = null)
	{
		var Kernel = new StandardKernel(new VectoNinjectModule());
		if (mission != null && loading != null) {
			Kernel.Rebind<IMissionFilter>().To<TestMissionFilter>().InSingletonScope();
			var missionFilter = Kernel.Get<IMissionFilter>() as TestMissionFilter;
			missionFilter.SetMissions((mission.Value, loading.Value));
        }

		var inputProvider = JSONInputDataFactory.ReadJsonJob(jobFile);
		var writer = new FileOutputWriter(jobFile);
		var factory = Kernel.Get<ISimulatorFactoryFactory>().Factory(ExecutionMode.Declaration, inputProvider, writer, null, null);
		//SimulatorFactory.CreateSimulatorFactory(ExecutionMode.Declaration, inputProvider, writer);
		factory.Validate = false;
		factory.WriteModalResults = true;

		var sumData = new SummaryDataContainer(writer);

		var jobContainer = new JobContainer(sumData);
		jobContainer.AddRuns(factory);

		jobContainer.Execute(false);
		jobContainer.WaitFinished();
		Assert.IsTrue(jobContainer.GetProgress().All(x => x.Value.Success));
	}


	[TestCase(PEV_PrimaryBus_IMC, IMCTechnology.OverheadPantograph)]
	public void RunJob_PrimaryBus(string jobFile, IMCTechnology imcTech, MissionType? mission = null, LoadingType? loading = null)
	{
		var Kernel = new StandardKernel(new VectoNinjectModule());
		var _xmlReader = Kernel.Get<IXMLInputDataReader>();
        if (mission != null && loading != null) {
			Kernel.Rebind<IMissionFilter>().To<TestMissionFilter>().InSingletonScope();
			var missionFilter = Kernel.Get<IMissionFilter>() as TestMissionFilter;
			missionFilter.SetMissions((mission.Value, loading.Value));
		}

		Kernel.UpdateBinding<IXMLDeclarationVehicleData>(XMLDeclarationPevPrimaryBusDataProviderV24.QUALIFIED_XSD_TYPE,
			ctx => {
				var imcInputData = new Mock<XMLDeclarationPevPrimaryBusDataProviderV24>(
					(IXMLDeclarationJobInputData)ctx.Parameters.ToList()[0].GetValue(ctx, ctx.Request.Target),
					(XmlNode)ctx.Parameters.ToList()[1].GetValue(ctx, ctx.Request.Target),
					(string)ctx.Parameters.ToList()[2].GetValue(ctx, ctx.Request.Target)) { CallBase = true };
				imcInputData.Setup(v => v.InMotionCharging).Returns(() => new XMLIMCData() {
					Technology = imcTech,
				});
				return imcInputData.Object;
			});

		var writer = new FileOutputWriter(Path.Combine(Path.GetDirectoryName(jobFile), "tmp", Path.GetFileName(jobFile)));
        var inputData = _xmlReader.CreateDeclaration(jobFile);
		var factory = Kernel.Get<ISimulatorFactoryFactory>().Factory(ExecutionMode.Declaration, inputData, writer, null, null, false);

		factory.WriteModalResults = true;
		var sumData = new SummaryDataContainer(writer);

		var jobContainer = new JobContainer(sumData);
		jobContainer.AddRuns(factory);

		jobContainer.Execute(false);
		jobContainer.WaitFinished();
		Assert.IsTrue(jobContainer.GetProgress().All(x => x.Value.Success));
    }

	[TestCase(PEV_PrimaryBus_IMC, PEV_CompleteBus_IMC, IMCTechnology.None, IMCTechnology.None, TestName = "A - IMC PEV Complete Bus None/None")]
	[TestCase(PEV_PrimaryBus_IMC, PEV_CompleteBus_IMC, IMCTechnology.None, IMCTechnology.OverheadPantograph, TestName = "B - IMC PEV Complete Bus None/Pantograph")]
	[TestCase(PEV_PrimaryBus_IMC, PEV_CompleteBus_IMC, IMCTechnology.OverheadPantograph, IMCTechnology.OverheadPantograph, TestName = "C - IMC PEV Pantograph/Pantograph")]
	[TestCase(PEV_PrimaryBus_IMC, PEV_CompleteBus_IMC, IMCTechnology.OverheadPantograph, IMCTechnology.None, TestName = "D - IMC PEV Pantograph/None")]

	[TestCase(PEV_PrimaryBus_IMC, PEV_CompleteBus_IMC, IMCTechnology.None, IMCTechnology.GroundRail, TestName = "E - IMC PEV Complete Bus None/GroundRail")]
	[TestCase(PEV_PrimaryBus_IMC, PEV_CompleteBus_IMC, IMCTechnology.GroundRail, IMCTechnology.GroundRail, TestName = "F - IMC PEV GroundRail/GroundRail")]
	[TestCase(PEV_PrimaryBus_IMC, PEV_CompleteBus_IMC, IMCTechnology.GroundRail, IMCTechnology.None, TestName = "G - IMC PEV GroundRail/None")]
	public void RunJob_CompleteBus(string primaryJob, string completeJob, IMCTechnology primaryTech,
		IMCTechnology completeTech, MissionType? mission = null, LoadingType? loading = null)
	{
		var Kernel = new StandardKernel(new VectoNinjectModule());
		var _xmlReader = Kernel.Get<IXMLInputDataReader>();
        if (mission != null && loading != null) {
			Kernel.Rebind<IMissionFilter>().To<TestMissionFilter>().InSingletonScope();
			var missionFilter = Kernel.Get<IMissionFilter>() as TestMissionFilter;
			missionFilter.SetMissions((mission.Value, loading.Value));
		}
		// update bindings to create a mock object that provides the imc technology values for primary vehicle, VIF, and complete vehicle
		Kernel.UpdateBinding<IXMLDeclarationVehicleData>(XMLDeclarationPevPrimaryBusDataProviderV24.QUALIFIED_XSD_TYPE, 
			ctx => {
				var imcInputData = new Mock<XMLDeclarationPevPrimaryBusDataProviderV24>(
					(IXMLDeclarationJobInputData)ctx.Parameters.ToList()[0].GetValue(ctx, ctx.Request.Target),
					(XmlNode)ctx.Parameters.ToList()[1].GetValue(ctx, ctx.Request.Target),
					(string)ctx.Parameters.ToList()[2].GetValue(ctx, ctx.Request.Target)) { CallBase = true };
				imcInputData.Setup(v => v.InMotionCharging).Returns(() => new XMLIMCData() {
					Technology = primaryTech,
				});
				return imcInputData.Object;
			});
		Kernel.UpdateBinding<IXMLDeclarationVehicleData>(XMLDeclarationMultistage_PEV_Ex_PrimaryVehicleBusDataProviderV01.QUALIFIED_XSD_TYPE, ctx => {
			var imcInputData = new Mock<XMLDeclarationMultistage_PEV_Ex_PrimaryVehicleBusDataProviderV01>(
				(IXMLPrimaryVehicleBusJobInputData)ctx.Parameters.ToList()[0].GetValue(ctx, ctx.Request.Target),
				(XmlNode)ctx.Parameters.ToList()[1].GetValue(ctx, ctx.Request.Target), 
				(string)ctx.Parameters.ToList()[2].GetValue(ctx, ctx.Request.Target)) {CallBase = true};
			imcInputData.Setup(v => v.InMotionCharging).Returns(() => new XMLIMCData() {
				Technology = primaryTech,
			});
            return imcInputData.Object;
		});
		Kernel.UpdateBinding<IXMLDeclarationVehicleData>(XMLDeclarationPEVCompletedBusDataProviderV24.QUALIFIED_XSD_TYPE, ctx => {
			var imcInputData = new Mock<XMLDeclarationPEVCompletedBusDataProviderV24>(
				(IXMLDeclarationJobInputData)ctx.Parameters.ToList()[0].GetValue(ctx, ctx.Request.Target),
				(XmlNode)ctx.Parameters.ToList()[1].GetValue(ctx, ctx.Request.Target),
				(string)ctx.Parameters.ToList()[2].GetValue(ctx, ctx.Request.Target)) { CallBase = true };
			imcInputData.Setup(v => v.InMotionCharging).Returns(() => new XMLIMCData() {
				Technology = completeTech,
			});
			return imcInputData.Object;
		});
        // ---------
        var outFolder = Path.Combine(Path.GetDirectoryName(primaryJob), $"out_{primaryTech.ToString()}_{completeTech.ToString()}");
		if (!Directory.Exists(outFolder)) {
			Directory.CreateDirectory(outFolder);
		}

		var writer = new FileOutputWriter(Path.Combine(outFolder, Path.GetFileName(primaryJob)));
		var primary = _xmlReader.CreateDeclaration(primaryJob);
		var complete = _xmlReader.CreateDeclaration(completeJob);
		
		var inputData = new MockCompletedBusInputDataProvider(primary, complete, true);
		inputData.XMLInputReader = _xmlReader;
		var factory = Kernel.Get<ISimulatorFactoryFactory>().Factory(ExecutionMode.Declaration, inputData, writer, null, null, false);

		factory.WriteModalResults = true;
		factory.SerializeVectoRunData = true;
        var sumData = new SummaryDataContainer(writer);

		var jobContainer = new JobContainer(sumData);
		jobContainer.AddRuns(factory);

		jobContainer.Execute(false);
		jobContainer.WaitFinished();

		var runs = jobContainer.Runs;

		Assert.IsTrue(jobContainer.GetProgress().All(x => x.Value.Success));
    }

	public class MockCompletedBusInputDataProvider : IMultistagePrimaryAndStageInputDataProvider
	{
		public IXMLInputDataReader XMLInputReader { set; get; }

		private IDeclarationInputDataProvider _primaryVehicle;
		private IVehicleDeclarationInputData _completedVehicle;
        private bool _simulateResultingVif;


		internal MockCompletedBusInputDataProvider(IInputDataProvider primaryVehicle, IDeclarationInputDataProvider completedVehicleData)
		{
			_primaryVehicle = primaryVehicle as IDeclarationInputDataProvider;
			_completedVehicle = completedVehicleData.JobInputData.Vehicle;
			_simulateResultingVif = false;

		}
		internal MockCompletedBusInputDataProvider(IInputDataProvider primaryVehicle, IDeclarationInputDataProvider completedVehicleData, bool runSimulation) : this(primaryVehicle, completedVehicleData)
		{
			_simulateResultingVif = runSimulation;
		}

		#region Implementation of IInputDataProvider

		public DataSource DataSource { get; }

        #endregion

        #region Implementation of IMultistagePrimaryAndStageInputDataProvider

        public IDeclarationInputDataProvider PrimaryVehicle =>
    _primaryVehicle;

        public IVehicleDeclarationInputData StageInputData => _completedVehicle;

        public bool SimulateResultingVIF => _simulateResultingVif;

		public bool? Completed => null;

		#endregion
	}
}