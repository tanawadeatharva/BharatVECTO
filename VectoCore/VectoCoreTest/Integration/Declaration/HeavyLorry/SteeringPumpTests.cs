using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Moq;
using Ninject;
using NUnit.Framework;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.Simulation.Impl.SimulatorFactory;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.OutputData.FileIO;

namespace TUGraz.VectoCore.Tests.Integration.Declaration.HeavyLorry;

[TestFixture]
public class SteeringPumpTests
{
	private const string BASE_DIR = @"TestData\Integration\DeclarationMode\V24_DeclarationMode\";
	private StandardKernel _kernel;
	private IXMLInputDataReader _xmlReader;
	
	
	
	
	[OneTimeSetUp]
	public void OneTimeSetup()
	{
		_kernel = new StandardKernel(new VectoNinjectModule());
		_xmlReader = _kernel.Get<IXMLInputDataReader>();
	}


	[TestCase(@"HeavyLorry\Conventional_heavyLorry_AMT.xml")]
	public void MechanicalSPTest(string jobFile)
	{
		var runsFactory = GetRunsFactory(jobFile);
		runsFactory.SerializeVectoRunData = true;
		foreach (var simulationRun in runsFactory.SimulationRuns()) {
			var container = simulationRun.GetContainer() as VehicleContainer;
		
			Assert.IsTrue(HasEngineAuxiliary("STP", container));
			Assert.IsTrue(HasEngineAuxiliary("FAN", container));
			Assert.IsTrue(HasEngineAuxiliary("ES", container));
			Assert.IsTrue(HasEngineAuxiliary("PS", container));
			Assert.IsTrue(HasEngineAuxiliary("AC", container));
		}
	}


	private bool HasEngineAuxiliary(string aux, VehicleContainer container)
	{
		try {
			dynamic engine = container.EngineInfo;
			var engineAux = engine.EngineAux as EngineAuxiliary;
			Assert.NotNull(engineAux);
			var auxField =
				typeof(EngineAuxiliary).GetField("Auxiliaries", BindingFlags.NonPublic | BindingFlags.Instance);
			var powerDemands =
				auxField.GetValue(engineAux) as Dictionary<string, Func<PerSecond, Second, Second, bool, Watt>>;
			return powerDemands.ContainsKey(aux);
		} catch (Exception ex) {
			return false;
		}
	}
	
	private bool HasElectricAuxiliary(string aux, VehicleContainer container)
	{
		try {
			dynamic engine = container.ElectricSystemInfo;
			var engineAux = engine.EngineAux as EngineAuxiliary;
			Assert.NotNull(engineAux);
			var auxField =
				typeof(EngineAuxiliary).GetField("Auxiliaries", BindingFlags.NonPublic | BindingFlags.Instance);
			var powerDemands =
				auxField.GetValue(engineAux) as Dictionary<string, Func<PerSecond, Second, Second, bool, Watt>>;
			return powerDemands.ContainsKey(aux);
		} catch (Exception ex) {
			return false;
		}
	}

	private ISimulatorFactory GetRunsFactory(string jobFile)
	{
		var filePath = Path.Combine(BASE_DIR, jobFile);
		var dataProvider = _xmlReader.CreateDeclaration(filePath);
		// Mock<IInputDataProvider> moq =
		// 	new Mock<IInputDataProvider>(() => dataProvider, MockBehavior.Strict);
		var fileWriter = new FileOutputWriter(filePath);
		var runsFactory = SimulatorFactory.CreateSimulatorFactory(ExecutionMode.Declaration, dataProvider, fileWriter);
		return runsFactory;
	}
}


