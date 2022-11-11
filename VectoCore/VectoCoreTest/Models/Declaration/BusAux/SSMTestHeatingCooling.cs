using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Moq;
using NUnit.Framework;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Impl;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.CompletedBus.Specific;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents;
using TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.HVAC;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data;

namespace TUGraz.VectoCore.Tests.Models.Declaration.BusAux;

[TestFixture, Parallelizable(ParallelScope.All)]
public class SSMTestHeatingCooling
{

	[
	// only mechanical heatpumps
	TestCase(BusHVACSystemConfiguration.Configuration1  , 1  , HeatPumpType.none                 , HeatPumpType.none                 , 299.88    , 0.0)       ,
	TestCase(BusHVACSystemConfiguration.Configuration1  , 6  , HeatPumpType.none                 , HeatPumpType.none                 , 299.88    , 0.0)       ,
	TestCase(BusHVACSystemConfiguration.Configuration1  , 8  , HeatPumpType.none                 , HeatPumpType.none                 , 287.8848  , 0.0)       ,
	TestCase(BusHVACSystemConfiguration.Configuration1  , 11 , HeatPumpType.none                 , HeatPumpType.none                 , 287.8848  , 0.0)       ,

	TestCase(BusHVACSystemConfiguration.Configuration2  , 1  , HeatPumpType.non_R_744_2_stage    , HeatPumpType.none                 , 299.88    , 0.0)       ,
	TestCase(BusHVACSystemConfiguration.Configuration2  , 6  , HeatPumpType.non_R_744_2_stage    , HeatPumpType.none                 , 299.88    , 0.0)       ,
	TestCase(BusHVACSystemConfiguration.Configuration2  , 8  , HeatPumpType.non_R_744_2_stage    , HeatPumpType.none                 , 287.8848  , 1.0156)    ,
	TestCase(BusHVACSystemConfiguration.Configuration2  , 11 , HeatPumpType.non_R_744_2_stage    , HeatPumpType.none                 , 287.8848  , 1351.1654) ,

	TestCase(BusHVACSystemConfiguration.Configuration3  , 1  , HeatPumpType.none                 , HeatPumpType.none                 , 428.400   , 0.0)       ,
	TestCase(BusHVACSystemConfiguration.Configuration3  , 6  , HeatPumpType.none                 , HeatPumpType.none                 , 856.800   , 0.0)       ,
	TestCase(BusHVACSystemConfiguration.Configuration3  , 8  , HeatPumpType.none                 , HeatPumpType.none                 , 822.5280  , 0.0)       ,
	TestCase(BusHVACSystemConfiguration.Configuration3  , 11 , HeatPumpType.none                 , HeatPumpType.none                 , 822.5280  , 0.0)       ,

	TestCase(BusHVACSystemConfiguration.Configuration4  , 1  , HeatPumpType.non_R_744_2_stage    , HeatPumpType.none                 , 428.400   , 0.0)       ,
	TestCase(BusHVACSystemConfiguration.Configuration4  , 6  , HeatPumpType.non_R_744_2_stage    , HeatPumpType.none                 , 856.80    , 0.0)       ,
	TestCase(BusHVACSystemConfiguration.Configuration4  , 8  , HeatPumpType.non_R_744_2_stage    , HeatPumpType.none                 , 822.5280  , 633.1093)  ,
	TestCase(BusHVACSystemConfiguration.Configuration4  , 11 , HeatPumpType.non_R_744_2_stage    , HeatPumpType.none                 , 822.5280  , 2000.0)    ,

	TestCase(BusHVACSystemConfiguration.Configuration5  , 1  , HeatPumpType.none                 , HeatPumpType.non_R_744_3_stage    , 428.400   , 0.0)       ,
	TestCase(BusHVACSystemConfiguration.Configuration5  , 6  , HeatPumpType.none                 , HeatPumpType.non_R_744_3_stage    , 856.80    , 0.0)       ,
	TestCase(BusHVACSystemConfiguration.Configuration5  , 8  , HeatPumpType.none                 , HeatPumpType.non_R_744_3_stage    , 822.528   , 612.7957)  ,
	TestCase(BusHVACSystemConfiguration.Configuration5  , 11 , HeatPumpType.none                 , HeatPumpType.non_R_744_3_stage    , 822.528   , 2965.1077) ,

	TestCase(BusHVACSystemConfiguration.Configuration6  , 1  , HeatPumpType.none                 , HeatPumpType.non_R_744_3_stage    , 428.400   , 0.0)       ,
	TestCase(BusHVACSystemConfiguration.Configuration6  , 6  , HeatPumpType.none                 , HeatPumpType.non_R_744_3_stage    , 856.80    , 0.0)       ,
	TestCase(BusHVACSystemConfiguration.Configuration6  , 8  , HeatPumpType.none                 , HeatPumpType.non_R_744_3_stage    , 822.528   , 612.7957)  ,
	TestCase(BusHVACSystemConfiguration.Configuration6  , 11 , HeatPumpType.none                 , HeatPumpType.non_R_744_3_stage    , 822.528   , 2965.1077) ,

	TestCase(BusHVACSystemConfiguration.Configuration7  , 1  , HeatPumpType.non_R_744_2_stage    , HeatPumpType.non_R_744_3_stage    , 428.400   , 0.0)       ,
	TestCase(BusHVACSystemConfiguration.Configuration7  , 6  , HeatPumpType.non_R_744_2_stage    , HeatPumpType.non_R_744_3_stage    , 856.80    , 0.0)       ,
	TestCase(BusHVACSystemConfiguration.Configuration7  , 8  , HeatPumpType.non_R_744_2_stage    , HeatPumpType.non_R_744_3_stage    , 822.528   , 616.1931)  ,
	TestCase(BusHVACSystemConfiguration.Configuration7  , 11 , HeatPumpType.non_R_744_2_stage    , HeatPumpType.non_R_744_3_stage    , 822.528   , 2984.8347) ,

	TestCase(BusHVACSystemConfiguration.Configuration8  , 1  , HeatPumpType.none                 , HeatPumpType.non_R_744_3_stage    , 428.400   , 0.0)       ,
	TestCase(BusHVACSystemConfiguration.Configuration8  , 6  , HeatPumpType.none                 , HeatPumpType.non_R_744_3_stage    , 856.80    , 0.0)       ,
	TestCase(BusHVACSystemConfiguration.Configuration8  , 8  , HeatPumpType.none                 , HeatPumpType.non_R_744_3_stage    , 822.528   , 612.7957)  ,
	TestCase(BusHVACSystemConfiguration.Configuration8  , 11 , HeatPumpType.none                 , HeatPumpType.non_R_744_3_stage    , 822.528   , 2965.1077) ,

	TestCase(BusHVACSystemConfiguration.Configuration9  , 1  , HeatPumpType.non_R_744_2_stage    , HeatPumpType.non_R_744_3_stage    , 428.400   , 0.0)       ,
	TestCase(BusHVACSystemConfiguration.Configuration9  , 6  , HeatPumpType.non_R_744_2_stage    , HeatPumpType.non_R_744_3_stage    , 856.80    , 0.0)       ,
	TestCase(BusHVACSystemConfiguration.Configuration9  , 8  , HeatPumpType.non_R_744_2_stage    , HeatPumpType.non_R_744_3_stage    , 822.528   , 616.1931)  ,
	TestCase(BusHVACSystemConfiguration.Configuration9  , 11 , HeatPumpType.non_R_744_2_stage    , HeatPumpType.non_R_744_3_stage    , 822.528   , 2984.8347) ,

	TestCase(BusHVACSystemConfiguration.Configuration10 , 1  , HeatPumpType.none                 , HeatPumpType.non_R_744_3_stage    , 428.400   , 0.0)       ,
	TestCase(BusHVACSystemConfiguration.Configuration10 , 6  , HeatPumpType.none                 , HeatPumpType.non_R_744_3_stage    , 856.80    , 0.0)       ,
	TestCase(BusHVACSystemConfiguration.Configuration10 , 8  , HeatPumpType.none                 , HeatPumpType.non_R_744_3_stage    , 822.528   , 612.7957)  ,
	TestCase(BusHVACSystemConfiguration.Configuration10 , 11 , HeatPumpType.none                 , HeatPumpType.non_R_744_3_stage    , 822.528   , 2965.1077) ,

	// only electrical heatpumps
	TestCase(BusHVACSystemConfiguration.Configuration2  , 1  , HeatPumpType.non_R_744_continuous , HeatPumpType.none                 , 299.88    , 0.0)       ,
	TestCase(BusHVACSystemConfiguration.Configuration2  , 6  , HeatPumpType.non_R_744_continuous , HeatPumpType.none                 , 299.88    , 0.0)       ,
	TestCase(BusHVACSystemConfiguration.Configuration2  , 8  , HeatPumpType.non_R_744_continuous , HeatPumpType.none                 , 288.8324  , 0)         ,
	TestCase(BusHVACSystemConfiguration.Configuration2  , 11 , HeatPumpType.non_R_744_continuous , HeatPumpType.none                 , 1520.7    , 0)         ,

	TestCase(BusHVACSystemConfiguration.Configuration4  , 1  , HeatPumpType.non_R_744_continuous , HeatPumpType.none                 , 428.400   , 0.0)       ,
	TestCase(BusHVACSystemConfiguration.Configuration4  , 6  , HeatPumpType.non_R_744_continuous , HeatPumpType.none                 , 856.80    , 0.0)       ,
	TestCase(BusHVACSystemConfiguration.Configuration4  , 8  , HeatPumpType.non_R_744_continuous , HeatPumpType.none                 , 1413.2125 , 0)         ,
	TestCase(BusHVACSystemConfiguration.Configuration4  , 11 , HeatPumpType.non_R_744_continuous , HeatPumpType.none                 , 2647.3455 , 0)         ,

	TestCase(BusHVACSystemConfiguration.Configuration5  , 1  , HeatPumpType.none                 , HeatPumpType.non_R_744_continuous , 428.400   , 0.0)       ,
	TestCase(BusHVACSystemConfiguration.Configuration5  , 6  , HeatPumpType.none                 , HeatPumpType.non_R_744_continuous , 856.80    , 0.0)       ,
	TestCase(BusHVACSystemConfiguration.Configuration5  , 8  , HeatPumpType.none                 , HeatPumpType.non_R_744_continuous , 1413.2125 , 0)         ,
	TestCase(BusHVACSystemConfiguration.Configuration5  , 11 , HeatPumpType.none                 , HeatPumpType.non_R_744_continuous , 3636.1338 , 0)         ,

	TestCase(BusHVACSystemConfiguration.Configuration6  , 1  , HeatPumpType.none                 , HeatPumpType.non_R_744_continuous , 428.400   , 0.0)       ,
	TestCase(BusHVACSystemConfiguration.Configuration6  , 6  , HeatPumpType.none                 , HeatPumpType.non_R_744_continuous , 856.80    , 0.0)       ,
	TestCase(BusHVACSystemConfiguration.Configuration6  , 8  , HeatPumpType.none                 , HeatPumpType.non_R_744_continuous , 1413.2125 , 0)         ,
	TestCase(BusHVACSystemConfiguration.Configuration6  , 11 , HeatPumpType.none                 , HeatPumpType.non_R_744_continuous , 3636.1338 , 0)         ,

	TestCase(BusHVACSystemConfiguration.Configuration7  , 1  , HeatPumpType.non_R_744_continuous , HeatPumpType.non_R_744_continuous , 428.400   , 0.0)       ,
	TestCase(BusHVACSystemConfiguration.Configuration7  , 6  , HeatPumpType.non_R_744_continuous , HeatPumpType.non_R_744_continuous , 856.80    , 0.0)       ,
	TestCase(BusHVACSystemConfiguration.Configuration7  , 8  , HeatPumpType.non_R_744_continuous , HeatPumpType.non_R_744_continuous , 1413.2125 , 0)         ,
	TestCase(BusHVACSystemConfiguration.Configuration7  , 11 , HeatPumpType.non_R_744_continuous , HeatPumpType.non_R_744_continuous , 3636.1338 , 0)         ,

	TestCase(BusHVACSystemConfiguration.Configuration8  , 1  , HeatPumpType.none                 , HeatPumpType.non_R_744_continuous , 428.400   , 0.0)       ,
	TestCase(BusHVACSystemConfiguration.Configuration8  , 6  , HeatPumpType.none                 , HeatPumpType.non_R_744_continuous , 856.80    , 0.0)       ,
	TestCase(BusHVACSystemConfiguration.Configuration8  , 8  , HeatPumpType.none                 , HeatPumpType.non_R_744_continuous , 1413.2125 , 0)         ,
	TestCase(BusHVACSystemConfiguration.Configuration8  , 11 , HeatPumpType.none                 , HeatPumpType.non_R_744_continuous , 3636.1338 , 0)         ,

	TestCase(BusHVACSystemConfiguration.Configuration9  , 1  , HeatPumpType.non_R_744_continuous , HeatPumpType.non_R_744_continuous , 428.400   , 0.0)       ,
	TestCase(BusHVACSystemConfiguration.Configuration9  , 6  , HeatPumpType.non_R_744_continuous , HeatPumpType.non_R_744_continuous , 856.80    , 0.0)       ,
	TestCase(BusHVACSystemConfiguration.Configuration9  , 8  , HeatPumpType.non_R_744_continuous , HeatPumpType.non_R_744_continuous , 1413.2125 , 0)         ,
	TestCase(BusHVACSystemConfiguration.Configuration9  , 11 , HeatPumpType.non_R_744_continuous , HeatPumpType.non_R_744_continuous , 3636.1338 , 0)         ,

	TestCase(BusHVACSystemConfiguration.Configuration10 , 1  , HeatPumpType.none                 , HeatPumpType.non_R_744_continuous , 428.400   , 0.0)       ,
	TestCase(BusHVACSystemConfiguration.Configuration10 , 6  , HeatPumpType.none                 , HeatPumpType.non_R_744_continuous , 856.80    , 0.0)       ,
	TestCase(BusHVACSystemConfiguration.Configuration10 , 8  , HeatPumpType.none                 , HeatPumpType.non_R_744_continuous , 1413.2125 , 0)         ,
	TestCase(BusHVACSystemConfiguration.Configuration10 , 11 , HeatPumpType.none                 , HeatPumpType.non_R_744_continuous , 3636.1338 , 0)         ,

	// mixed electrical and mechanical heatpumps
	TestCase(BusHVACSystemConfiguration.Configuration7  , 1  , HeatPumpType.non_R_744_continuous , HeatPumpType.non_R_744_3_stage    , 428.400   , 0.0)       ,
	TestCase(BusHVACSystemConfiguration.Configuration7  , 6  , HeatPumpType.non_R_744_continuous , HeatPumpType.non_R_744_3_stage    , 856.80    , 0.0)       ,
	TestCase(BusHVACSystemConfiguration.Configuration7  , 8  , HeatPumpType.non_R_744_continuous , HeatPumpType.non_R_744_3_stage    , 883.4159  , 547.9913)  ,
	TestCase(BusHVACSystemConfiguration.Configuration7  , 11 , HeatPumpType.non_R_744_continuous , HeatPumpType.non_R_744_3_stage    , 1116.3204 , 2644.1315) ,

	TestCase(BusHVACSystemConfiguration.Configuration7  , 1  , HeatPumpType.non_R_744_2_stage    , HeatPumpType.non_R_744_continuous , 428.400   , 0.0)       ,
	TestCase(BusHVACSystemConfiguration.Configuration7  , 6  , HeatPumpType.non_R_744_2_stage    , HeatPumpType.non_R_744_continuous , 856.80    , 0.0)       ,
	TestCase(BusHVACSystemConfiguration.Configuration7  , 8  , HeatPumpType.non_R_744_2_stage    , HeatPumpType.non_R_744_continuous , 1360.3368 , 59.7565)   ,
	TestCase(BusHVACSystemConfiguration.Configuration7  , 11 , HeatPumpType.non_R_744_2_stage    , HeatPumpType.non_R_744_continuous , 3393.4694 , 285.6602)  ,

	TestCase(BusHVACSystemConfiguration.Configuration9  , 1  , HeatPumpType.non_R_744_continuous , HeatPumpType.non_R_744_3_stage    , 428.400   , 0.0)       ,
	TestCase(BusHVACSystemConfiguration.Configuration9  , 6  , HeatPumpType.non_R_744_continuous , HeatPumpType.non_R_744_3_stage    , 856.80    , 0.0)       ,
	TestCase(BusHVACSystemConfiguration.Configuration9  , 8  , HeatPumpType.non_R_744_continuous , HeatPumpType.non_R_744_3_stage    , 883.4159  , 547.9913)  ,
	TestCase(BusHVACSystemConfiguration.Configuration9  , 11 , HeatPumpType.non_R_744_continuous , HeatPumpType.non_R_744_3_stage    , 1116.3204 , 2644.1315) ,

	TestCase(BusHVACSystemConfiguration.Configuration9  , 1  , HeatPumpType.non_R_744_2_stage    , HeatPumpType.non_R_744_continuous , 428.400   , 0.0)       ,
	TestCase(BusHVACSystemConfiguration.Configuration9  , 6  , HeatPumpType.non_R_744_2_stage    , HeatPumpType.non_R_744_continuous , 856.80    , 0.0)       ,
	TestCase(BusHVACSystemConfiguration.Configuration9  , 8  , HeatPumpType.non_R_744_2_stage    , HeatPumpType.non_R_744_continuous , 1360.3368 , 59.7565)   ,
	TestCase(BusHVACSystemConfiguration.Configuration9  , 11 , HeatPumpType.non_R_744_2_stage    , HeatPumpType.non_R_744_continuous , 3393.4694 , 285.6602)  ,

    ]
	public void SSMTest_Cooling_SingleEnvironment(BusHVACSystemConfiguration cfg, int envId, HeatPumpType driverHeatpump,
		HeatPumpType passengerHeatpump,  double expectedElPwrW, double expectedMechPwrW)
	{
		var auxData = GetAuxParametersConventionalSingleDeckForTest(cfg, driverHeatpump, passengerHeatpump);

		// set list of environmental conditions to a single entry
		var ssmInputs = auxData.SSMInputsCooling as SSMInputs;
		Assert.NotNull(ssmInputs);
		var envAll = ssmInputs.EnvironmentalConditions;
		var selectedEnv = envAll.EnvironmentalConditionsMap.GetEnvironmentalConditions().Where(x => x.ID == envId).FirstOrDefault();
		Assert.NotNull(selectedEnv);
		var newEnv = new EnvironmentalConditionMapEntry(selectedEnv.ID, selectedEnv.Temperature, selectedEnv.Solar, 1.0,
			selectedEnv.HeatPumpCoP.ToDictionary(x => x.Key, x => x.Value), 
			selectedEnv.HeaterEfficiency.ToDictionary(x => x.Key, x => x.Value));
		ssmInputs.EnvironmentalConditionsMap = new EnvironmentalConditionsMap(new[] { newEnv });
		// ---

		Assert.AreEqual(cfg, ssmInputs.HVACSystemConfiguration);

		var ssm = new SSMTOOL(auxData.SSMInputsCooling);

		Console.WriteLine($"{ssm.ElectricalWAdjusted.Value().ToGUIFormat(4)}, {ssm.MechanicalWBaseAdjusted.Value().ToGUIFormat(4)}");
		
		Assert.AreEqual(expectedElPwrW, ssm.ElectricalWAdjusted.Value(), 1e-3, "expected Electric Power");
		Assert.AreEqual(expectedMechPwrW, ssm.MechanicalWBaseAdjusted.Value(), 1e-3, "expected Mechanical Power");

	}


	[
		// only mechanical heatpumps
		// electric: only ventilation, mechanic: heatpump
		TestCase(BusHVACSystemConfiguration.Configuration1  , HeatPumpType.none              , HeatPumpType.none              , 295.3949 , 0.0)       ,
		TestCase(BusHVACSystemConfiguration.Configuration2  , HeatPumpType.non_R_744_2_stage , HeatPumpType.none              , 295.3949 , 109.29748) ,
		TestCase(BusHVACSystemConfiguration.Configuration3  , HeatPumpType.none              , HeatPumpType.none              , 664.4004 , 0.0)       ,
		TestCase(BusHVACSystemConfiguration.Configuration4  , HeatPumpType.non_R_744_2_stage , HeatPumpType.none              , 664.4004 , 398.5246)  ,
		TestCase(BusHVACSystemConfiguration.Configuration5  , HeatPumpType.none              , HeatPumpType.non_R_744_3_stage , 664.4004 , 429.8231)  ,
		TestCase(BusHVACSystemConfiguration.Configuration6  , HeatPumpType.none              , HeatPumpType.non_R_744_3_stage , 664.4004 , 429.8231)  ,
		TestCase(BusHVACSystemConfiguration.Configuration7  , HeatPumpType.non_R_744_2_stage , HeatPumpType.non_R_744_3_stage , 664.4004 , 432.3237)  ,
		TestCase(BusHVACSystemConfiguration.Configuration8  , HeatPumpType.none              , HeatPumpType.non_R_744_3_stage , 664.4004 , 429.8231)  ,
		TestCase(BusHVACSystemConfiguration.Configuration9  , HeatPumpType.non_R_744_2_stage , HeatPumpType.non_R_744_3_stage , 664.4004 , 432.3237)  ,
		TestCase(BusHVACSystemConfiguration.Configuration10 , HeatPumpType.none              , HeatPumpType.non_R_744_3_stage , 664.4004 , 429.8231)  ,

		// only electrical heatpumps
		// electric: ventilation + heatpump
		//TestCase(BusHVACSystemConfiguration.Configuration1, HeatPumpType.none, HeatPumpType.none, 0, 0.0),
		TestCase(BusHVACSystemConfiguration.Configuration2, HeatPumpType.non_R_744_continuous, HeatPumpType.none, 395.9945, 0.0),
		//TestCase(BusHVACSystemConfiguration.Configuration3, HeatPumpType.none, HeatPumpType.none, 0.0, 0.0),
		TestCase(BusHVACSystemConfiguration.Configuration4, HeatPumpType.non_R_744_continuous, HeatPumpType.none, 1033.8823, 0.0),

		TestCase(BusHVACSystemConfiguration.Configuration5  , HeatPumpType.none                 , HeatPumpType.non_R_744_continuous , 1076.1036 , 0) ,
		TestCase(BusHVACSystemConfiguration.Configuration6  , HeatPumpType.none                 , HeatPumpType.non_R_744_continuous , 1076.1036 , 0) ,
		TestCase(BusHVACSystemConfiguration.Configuration7  , HeatPumpType.non_R_744_continuous , HeatPumpType.non_R_744_continuous , 1076.1036 , 0) ,
		TestCase(BusHVACSystemConfiguration.Configuration8  , HeatPumpType.none                 , HeatPumpType.non_R_744_continuous , 1076.1036 , 0) ,
		TestCase(BusHVACSystemConfiguration.Configuration9  , HeatPumpType.non_R_744_continuous , HeatPumpType.non_R_744_continuous , 1076.1036 , 0) ,
		TestCase(BusHVACSystemConfiguration.Configuration10 , HeatPumpType.none                 , HeatPumpType.non_R_744_continuous , 1076.1036 , 0) ,

		// mixed electrical and mechanical heatpumps
		TestCase(BusHVACSystemConfiguration.Configuration7 , HeatPumpType.non_R_744_continuous , HeatPumpType.non_R_744_3_stage    , 707.0598  , 383.9348) ,
		TestCase(BusHVACSystemConfiguration.Configuration7 , HeatPumpType.non_R_744_2_stage    , HeatPumpType.non_R_744_continuous , 1039.7276 , 41.703)   ,
		TestCase(BusHVACSystemConfiguration.Configuration9 , HeatPumpType.non_R_744_continuous , HeatPumpType.non_R_744_3_stage    , 707.0598  , 383.9348) ,
		TestCase(BusHVACSystemConfiguration.Configuration9 , HeatPumpType.non_R_744_2_stage    , HeatPumpType.non_R_744_continuous , 1039.7276 , 41.703)   ,

	]
	public void SSMTest_Cooling_AvgAllEnvironments(BusHVACSystemConfiguration cfg, HeatPumpType driverHeatpump,
		HeatPumpType passengerHeatpump, double expectedElPwrW, double expectedMechPwrW)
	{
		var auxData = GetAuxParametersConventionalSingleDeckForTest(cfg, driverHeatpump, passengerHeatpump);

		var ssmInputs = auxData.SSMInputsCooling as SSMInputs;
		Assert.NotNull(ssmInputs);
		Assert.AreEqual(cfg, ssmInputs.HVACSystemConfiguration);

		var ssm = new SSMTOOL(auxData.SSMInputsCooling);

		Console.WriteLine($"{ssm.ElectricalWAdjusted.Value().ToGUIFormat(4)}, {ssm.MechanicalWBaseAdjusted.Value().ToGUIFormat(4)}");

		Assert.AreEqual(expectedElPwrW, ssm.ElectricalWAdjusted.Value(), 1e-3, "expected electrical power");
		Assert.AreEqual(expectedMechPwrW, ssm.MechanicalWBaseAdjusted.Value(), 1e-3, "expected mechanical power");

	}

	private IAuxiliaryConfig GetAuxParametersConventionalSingleDeckForTest(BusHVACSystemConfiguration hvacConfig, HeatPumpType driverHeatpump,
		HeatPumpType passengerHeatpump)
	{
		return CreateBusAuxInputParameters(MissionType.Urban, 
			VehicleClass.Class31a, 
			VectoSimulationJobType.ConventionalVehicle, 
			VehicleCode.CA, 
			RegistrationClass.II, 
			AxleConfiguration.AxleConfig_4x2, 
			false, 
			false, 
			12.SI<Meter>(), 
			3.SI<Meter>(), 
			2.55.SI<Meter>(), 
			40, 
			0, 
			HeatPumpType.none, 
			driverHeatpump, 
			HeatPumpType.none, 
			passengerHeatpump, 
			30000.SI<Watt>(), 
			false,
			false,
			false, 
			hvacConfig, 
			false, 
			false,
			false, 
			false, 
			false,
            steeringpumps: new[] { "Dual displacement" }, 
			fanTech: "Crankshaft mounted - Discrete step clutch", 
			alternatorTech: AlternatorType.Conventional, entranceHeight: 0.3.SI<Meter>());

	}

	private IAuxiliaryConfig CreateBusAuxInputParameters(MissionType missionType, VehicleClass vehicleClass,
		VectoSimulationJobType vehicleType,
		VehicleCode vehicleCode, RegistrationClass registrationClass,
		AxleConfiguration axleconfiguration, bool articulated,
		bool lowEntry, Meter length,
		Meter height, Meter width, int numPassengersLowerdeck, int numPassengersUpperdeck,
		HeatPumpType hpHeatingDriver, HeatPumpType hpCoolingDriver, HeatPumpType hpHeatingPassenger,
		HeatPumpType hpCoolingPassenger, Watt auxHeaterPower,
		bool airElectricHeater, bool waterElectricHeater, bool otherElectricHeater,
		BusHVACSystemConfiguration hvacConfig, bool doubleGlazing, bool adjustableAuxHeater,
		bool separateAirdistributionDicts, bool adjustableCoolantThermostat, bool engineWasteGasHeatExchanger,
		string[] steeringpumps, string fanTech, AlternatorType alternatorTech, Meter entranceHeight)
	{
		var dao = new SpecificCompletedBusAuxiliaryDataAdapter(new PrimaryBusAuxiliaryDataAdapter());

		var segment = DeclarationData.CompletedBusSegments.Lookup(axleconfiguration.NumAxles(),
			vehicleCode, registrationClass, numPassengersLowerdeck, height, lowEntry);
		var mission = segment.Missions.FirstOrDefault();

		var primaryVehicle = new Mock<IVehicleDeclarationInputData>();
		primaryVehicle.Setup(p => p.VehicleType).Returns(vehicleType);
		primaryVehicle.Setup(p => p.AxleConfiguration).Returns(axleconfiguration);
		primaryVehicle.Setup(p => p.Articulated).Returns(articulated);

		var primaryComponents = new Mock<IVehicleComponentsDeclaration>();
		var primaryBusAux = new Mock<IBusAuxiliariesDeclarationData>();
		var primaryBusAuxPS_S = new Mock<IPneumaticSupplyDeclarationData>();
		var primaryBusAuxPS_C = new Mock<IPneumaticConsumersDeclarationData>();
		var primaryBusAuxHVAC = new Mock<IHVACBusAuxiliariesDeclarationData>();
		var primaryBusAuxES = new Mock<IElectricSupplyDeclarationData>();
		primaryVehicle.Setup(p => p.Components).Returns(primaryComponents.Object);
		primaryComponents.Setup(p => p.BusAuxiliaries).Returns(primaryBusAux.Object);
		primaryBusAux.Setup(p => p.PneumaticSupply).Returns(primaryBusAuxPS_S.Object);
		primaryBusAux.Setup(p => p.PneumaticConsumers).Returns(primaryBusAuxPS_C.Object);
		primaryBusAux.Setup(p => p.HVACAux).Returns(primaryBusAuxHVAC.Object);
		primaryBusAux.Setup(p => p.ElectricSupply).Returns(primaryBusAuxES.Object);

		primaryBusAux.Setup(p => p.FanTechnology).Returns(fanTech);
		primaryBusAux.Setup(p => p.SteeringPumpTechnology).Returns(steeringpumps.ToList);

		primaryBusAuxPS_S.Setup(p => p.CompressorDrive).Returns(CompressorDrive.electrically);
		primaryBusAuxPS_S.Setup(p => p.CompressorSize).Returns("Medium Supply 2-stage");
		primaryBusAuxPS_S.Setup(p => p.SmartAirCompression).Returns(false);
		primaryBusAuxPS_S.Setup(p => p.SmartRegeneration).Returns(false);

		primaryBusAuxES.Setup(p => p.AlternatorTechnology).Returns(alternatorTech);
		primaryBusAuxES.Setup(p => p.Alternators).Returns(new[] { new AlternatorInputData(28.3.SI<Volt>(), 50.SI<Ampere>()) }.Cast<IAlternatorDeclarationInputData>().ToList());
		primaryBusAuxES.Setup(p => p.ElectricStorage).Returns(new List<IBusAuxElectricStorageDeclarationInputData>());

		primaryBusAuxPS_C.Setup(p => p.AdBlueDosing).Returns(ConsumerTechnology.Mechanically);
		primaryBusAuxPS_C.Setup(p => p.AirsuspensionControl).Returns(ConsumerTechnology.Electrically);
		primaryBusAuxHVAC.Setup(p => p.DoubleGlazing).Returns(doubleGlazing);
		primaryBusAuxHVAC.Setup(p => p.AdjustableAuxiliaryHeater).Returns(adjustableAuxHeater);
		primaryBusAuxHVAC.Setup(p => p.SeparateAirDistributionDucts).Returns(separateAirdistributionDicts);
		primaryBusAuxHVAC.Setup(p => p.AdjustableCoolantThermostat).Returns(adjustableCoolantThermostat);
		primaryBusAuxHVAC.Setup(p => p.EngineWasteGasHeatExchanger).Returns(engineWasteGasHeatExchanger);

		var completedVehicle = new Mock<IVehicleDeclarationInputData>();
		completedVehicle.Setup(c => c.VehicleCode).Returns(vehicleCode);
		completedVehicle.Setup(c => c.Length).Returns(length);
		completedVehicle.Setup(c => c.Height).Returns(height);
		completedVehicle.Setup(c => c.Width).Returns(width);
		completedVehicle.Setup(c => c.NumberPassengerSeatsLowerDeck).Returns(numPassengersLowerdeck);
		completedVehicle.Setup(c => c.NumberPassengersStandingLowerDeck).Returns(0);
		completedVehicle.Setup(c => c.NumberPassengerSeatsUpperDeck).Returns(numPassengersUpperdeck);
		completedVehicle.Setup(c => c.NumberPassengersStandingUpperDeck).Returns(0);
		completedVehicle.Setup(c => c.RegisteredClass).Returns(registrationClass);
		completedVehicle.Setup(c => c.LowEntry).Returns(lowEntry);
		completedVehicle.Setup(c => c.EntranceHeight).Returns(entranceHeight);

		var completedComponents = new Mock<IVehicleComponentsDeclaration>();
		var completedBusAux = new Mock<IBusAuxiliariesDeclarationData>();
		var completedHVACAux = new Mock<IHVACBusAuxiliariesDeclarationData>();
		completedBusAux.Setup(c => c.HVACAux).Returns(completedHVACAux.Object);
		completedComponents.Setup(c => c.BusAuxiliaries).Returns(completedBusAux.Object);
		completedVehicle.Setup(c => c.Components).Returns(completedComponents.Object);

		completedHVACAux.Setup(c => c.SystemConfiguration).Returns(hvacConfig);
		completedHVACAux.Setup(c => c.AuxHeaterPower).Returns(auxHeaterPower);
		completedHVACAux.Setup(c => c.SeparateAirDistributionDucts).Returns(mission.BusParameter.SeparateAirDistributionDuctsHVACCfg.Contains(hvacConfig));
		completedHVACAux.Setup(c => c.HeatPumpTypeHeatingDriverCompartment).Returns(hpHeatingDriver);
		completedHVACAux.Setup(c => c.HeatPumpTypeHeatingPassengerCompartment).Returns(hpHeatingPassenger);
		completedHVACAux.Setup(c => c.HeatPumpTypeCoolingDriverCompartment).Returns(hpCoolingDriver);
		completedHVACAux.Setup(c => c.HeatPumpTypeCoolingPassengerCompartment).Returns(hpCoolingPassenger);
		completedHVACAux.Setup(c => c.AirElectricHeater).Returns(airElectricHeater);
		completedHVACAux.Setup(c => c.WaterElectricHeater).Returns(waterElectricHeater);
		completedHVACAux.Setup(c => c.OtherHeatingTechnology).Returns(otherElectricHeater);


		var runData = new VectoRunData() {
			Mission = mission,
			Loading = LoadingType.ReferenceLoad,
			VehicleData = new VehicleData() {
				VehicleClass = vehicleClass,
			},
			Retarder = new RetarderData() {
				Type = RetarderType.None
			}
		};

		var retVal = dao.CreateBusAuxiliariesData(mission, primaryVehicle.Object, completedVehicle.Object, runData);
		
		return retVal;
	}
}