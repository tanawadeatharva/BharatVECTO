using System.Collections.Generic;
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
		TestCase(BusHVACSystemConfiguration.Configuration1, HeatPumpType.none, HeatPumpType.none, 1, 0, 0.0),
		TestCase(BusHVACSystemConfiguration.Configuration1, HeatPumpType.none, HeatPumpType.none, 6, 0, 0.0),
		TestCase(BusHVACSystemConfiguration.Configuration1, HeatPumpType.none, HeatPumpType.none, 8, 0, 0.0),
		TestCase(BusHVACSystemConfiguration.Configuration1, HeatPumpType.none, HeatPumpType.none, 11, 0, 0.0),

		TestCase(BusHVACSystemConfiguration.Configuration2, HeatPumpType.non_R_744_2_stage, HeatPumpType.none, 1, 0, 0.0),
		TestCase(BusHVACSystemConfiguration.Configuration2, HeatPumpType.non_R_744_2_stage, HeatPumpType.none, 6, 299.88, 0.0),
		TestCase(BusHVACSystemConfiguration.Configuration2, HeatPumpType.non_R_744_2_stage, HeatPumpType.none, 8, 287.8848, 1.0156),
		TestCase(BusHVACSystemConfiguration.Configuration2, HeatPumpType.non_R_744_2_stage, HeatPumpType.none, 11, 287.8848, 1351.1654),

		TestCase(BusHVACSystemConfiguration.Configuration3, HeatPumpType.none, HeatPumpType.none, 1, 0, 0.0),
		TestCase(BusHVACSystemConfiguration.Configuration3, HeatPumpType.none, HeatPumpType.none, 6, 0, 0.0),
		TestCase(BusHVACSystemConfiguration.Configuration3, HeatPumpType.none, HeatPumpType.none, 8, 0, 0.0),
		TestCase(BusHVACSystemConfiguration.Configuration3, HeatPumpType.none, HeatPumpType.none, 11, 0, 0.0),

		TestCase(BusHVACSystemConfiguration.Configuration4, HeatPumpType.non_R_744_2_stage, HeatPumpType.none, 1, 0, 0.0),
		TestCase(BusHVACSystemConfiguration.Configuration4, HeatPumpType.non_R_744_2_stage, HeatPumpType.none, 6, 856.80, 0.0),
		TestCase(BusHVACSystemConfiguration.Configuration4, HeatPumpType.non_R_744_2_stage, HeatPumpType.none, 8, 822.5280, 633.1093),
		TestCase(BusHVACSystemConfiguration.Configuration4, HeatPumpType.non_R_744_2_stage, HeatPumpType.none, 11, 822.5280, 2000.0),

		TestCase(BusHVACSystemConfiguration.Configuration6, HeatPumpType.none, HeatPumpType.non_R_744_2_stage, 6, 856.80, 0.0)

    ]
	public void SSMTest_Cooling_SingleEnvironment(BusHVACSystemConfiguration cfg, HeatPumpType driverHeatpump,
		HeatPumpType passengerHeatpump, int envId, double expectedElPwrW, double expectedMechPwrW)
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

		var ssm = new SSMTOOL(auxData.SSMInputsCooling);
		
		Assert.AreEqual(expectedElPwrW, ssm.ElectricalWAdjusted.Value(), 1e-3);
		Assert.AreEqual(expectedMechPwrW, ssm.MechanicalWBaseAdjusted.Value(), 1e-3);

	}


	[
		// electric: only ventilation, mechanic: heatpump
		TestCase(BusHVACSystemConfiguration.Configuration1, HeatPumpType.none, HeatPumpType.none, 293.8056, 0.0),  // correct? I'd expect P_el = 293 as well! (ventilation low)
		TestCase(BusHVACSystemConfiguration.Configuration2, HeatPumpType.non_R_744_2_stage, HeatPumpType.none, 293.8056, 109.29748),
		TestCase(BusHVACSystemConfiguration.Configuration3, HeatPumpType.none, HeatPumpType.none, 662.12989, 0.0), // correct? I'd expect p_el = 622 as well! (ventilation high)
		TestCase(BusHVACSystemConfiguration.Configuration4, HeatPumpType.non_R_744_2_stage, HeatPumpType.none, 662.12989, 398.5246),

		TestCase(BusHVACSystemConfiguration.Configuration6, HeatPumpType.none, HeatPumpType.non_R_744_2_stage, 662.1298, 444.7991),

		// electric: ventilation + heatpump
		//TestCase(BusHVACSystemConfiguration.Configuration1, HeatPumpType.none, HeatPumpType.none, 0, 0.0),
		TestCase(BusHVACSystemConfiguration.Configuration2, HeatPumpType.non_R_744_continuous, HeatPumpType.none, 394.405, 0.0),
		//TestCase(BusHVACSystemConfiguration.Configuration3, HeatPumpType.none, HeatPumpType.none, 0.0, 0.0),
		TestCase(BusHVACSystemConfiguration.Configuration4, HeatPumpType.non_R_744_continuous, HeatPumpType.none, 1031.611, 0.0),

	]
	public void SSMTest_Cooling_AvgAllEnvironments(BusHVACSystemConfiguration cfg, HeatPumpType driverHeatpump,
		HeatPumpType passengerHeatpump, double expectedElPwrW, double expectedMechPwrW)
	{
		var auxData = GetAuxParametersConventionalSingleDeckForTest(cfg, driverHeatpump, passengerHeatpump);

		var ssm = new SSMTOOL(auxData.SSMInputsCooling);

		Assert.AreEqual(expectedElPwrW, ssm.ElectricalWAdjusted.Value(), 1e-3);
		Assert.AreEqual(expectedMechPwrW, ssm.MechanicalWBaseAdjusted.Value(), 1e-3);

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