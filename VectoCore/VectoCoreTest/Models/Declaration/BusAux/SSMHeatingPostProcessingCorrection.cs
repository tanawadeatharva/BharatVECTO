using System;
using System.Linq;
using Moq;
using NUnit.Framework;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.VectoCore.Tests.Models.Declaration.BusAux;

[TestFixture, Parallelizable(ParallelScope.All)]
public class SSMHeatingPostProcessingCorrection
{
	private const BusHVACSystemConfiguration CFG1 = BusHVACSystemConfiguration.Configuration1;
	private const BusHVACSystemConfiguration CFG2 = BusHVACSystemConfiguration.Configuration2;
	private const BusHVACSystemConfiguration CFG3 = BusHVACSystemConfiguration.Configuration3;
	private const BusHVACSystemConfiguration CFG4 = BusHVACSystemConfiguration.Configuration4;
	private const BusHVACSystemConfiguration CFG5 = BusHVACSystemConfiguration.Configuration5;
	private const BusHVACSystemConfiguration CFG6 = BusHVACSystemConfiguration.Configuration6;
	private const BusHVACSystemConfiguration CFG7 = BusHVACSystemConfiguration.Configuration7;
	private const BusHVACSystemConfiguration CFG8 = BusHVACSystemConfiguration.Configuration8;
	private const BusHVACSystemConfiguration CFG9 = BusHVACSystemConfiguration.Configuration9;
	private const BusHVACSystemConfiguration CFG10 = BusHVACSystemConfiguration.Configuration10;

	private const HeatPumpType HeatPumpNone = HeatPumpType.none;
	private const HeatPumpType HeatPumpR744 = HeatPumpType.R_744;
	private const HeatPumpType HeatPump2Stage = HeatPumpType.non_R_744_2_stage;
	private const HeatPumpType HeatPump3Stage = HeatPumpType.non_R_744_3_stage;
	private const HeatPumpType HeatPumpCont = HeatPumpType.non_R_744_continuous;
	private const HeaterType NoElHtr = HeaterType.None;
	private const HeaterType AirElHtr = HeaterType.AirElectricHeater;
	private const HeaterType WaterElHtr = HeaterType.WaterElectricHeater;
	private const HeaterType OthrElHtr = HeaterType.OtherElectricHeating;

	private const double AuxHtrPwr0 = 0.0;
	private const double AuxHtrPwr30 = 30.0e3;

	private const string FuelMap = "engine speed, torque, fuel consumption\n" +
										"500,-31,0\n" +
										"500,0,500\n" +
										"500,1000,24000\n" +
										"2500,-109,0\n" +
										"2500,0,3800\n" +
										"2500,1000,32000\n";

	protected static readonly FuelData.Entry Fuel = FuelData.Diesel;

	[
		TestCase("a", CFG1, HeatPumpNone, HeatPumpNone, 0, AuxHtrPwr0, 0, 0),
		TestCase("b", CFG1, HeatPumpNone, HeatPumpNone, 0, AuxHtrPwr30, 6685488.8606, 0.1565688),
		TestCase("c", CFG1, HeatPumpNone, HeatPumpNone, 4, AuxHtrPwr30, 205671.4565, 0.0048166),
	]
	public void TestModDataPostprocessing_Conventional(string sort, BusHVACSystemConfiguration cfg, HeatPumpType driverHpHeating, HeatPumpType passengerHpHeating, 
		double fuelConsumptionKg, double auxhHeaterPwr, double expectedAuxHeatingDemandJ, double expectedFcAuxHeaterKg)
	{
		var correction = new ModalDataPostprocessingCorrection();
		var busAux = GetAuxParametersConventionalSingleDeckForTest(cfg, driverHeatPumpHeating: driverHpHeating, passengerHeatPumpHeating: passengerHpHeating, auxHeaterPower: auxhHeaterPwr.SI<Watt>());
		var runData = GetVectoRunDataConventional(busAux);
		var mockModData = GetConventionalMockModData(runData, fuelConsumptionKg.SI<Kilogram>());

		var corrected = correction.ApplyCorrection(mockModData, runData);

		Assert.NotNull(corrected);
		var fcCorr = corrected.FuelConsumptionCorrection(Fuel) as FuelConsumptionCorrection;
		Assert.NotNull(fcCorr);

		Console.WriteLine($"{corrected.AuxHeaterDemand.Value()}, {fcCorr.FcAuxHtr.Value()}");

		Assert.AreEqual(expectedAuxHeatingDemandJ, corrected.AuxHeaterDemand.Value(), 1e-3, "expectedAuxHeatingDemand");
		Assert.AreEqual(expectedFcAuxHeaterKg, fcCorr.FcAuxHtr.Value(), 1e-6, "expectedFcAuxHeater");
	}

	protected VectoRunData GetVectoRunDataConventional(IAuxiliaryConfig busAux)
	{
		
		var runData = new VectoRunData() {
			DriverData = new DriverData() {
				EngineStopStart = new DriverData.EngineStopStartData() {
					UtilityFactorDriving = 0.8,
					UtilityFactorStandstill = 0.8
				}
			},
			EngineData = new CombustionEngineData() {
				WHRType = WHRType.None,
				IdleSpeed = 600.RPMtoRad(),
				Fuels = new[] { new CombustionEngineFuelData() {
					FuelData =Fuel,
					ConsumptionMap = FuelConsumptionMapReader.ReadFromStream(FuelMap.ToStream()),
				}}.ToList(),
			},
			BusAuxiliaries = busAux
		};
		return runData;
	}

	protected IModalDataContainer GetConventionalMockModData(VectoRunData runData, Kilogram totalFuelConsumption)
	{
		var mockContainer = new Mock<IVehicleContainer>();
		var eng = new Mock<IEngineInfo>();

		eng.Setup(e => e.EngineIdleSpeed).Returns(runData.EngineData.IdleSpeed);

		//mockContainer.Setup(c => c.ModalData).Returns(m.Object);
		mockContainer.Setup(c => c.EngineInfo).Returns(eng.Object);

		var busAux = new BusAuxiliariesAdapter(mockContainer.Object, runData.BusAuxiliaries);

		var m = new Mock<IModalDataContainer>();
		m.Setup(x => x.Duration).Returns(3600.SI<Second>());
		m.Setup(x => x.Distance).Returns(30000.SI<Meter>());
		m.Setup(x => x.TimeIntegral<WattSecond>(ModalResultField.P_WHR_el_corr, null)).Returns(0.SI<WattSecond>());
		m.Setup(x => x.TimeIntegral<WattSecond>(ModalResultField.P_WHR_mech_corr, null)).Returns(0.SI<WattSecond>());
		m.Setup(x => x.TimeIntegral<WattSecond>(ModalResultField.P_busAux_ES_consumer_sum, null))
			.Returns(0.SI<WattSecond>());
		m.Setup(x => x.TimeIntegral<WattSecond>(ModalResultField.P_busAux_ES_generated, null))
			.Returns(0.SI<WattSecond>());
		m.Setup(x => x.TimeIntegral<WattSecond>(ModalResultField.P_ice_start, null)).Returns(0.SI<WattSecond>());
		m.Setup(x => x.GetValues<NormLiter>(ModalResultField.Nl_busAux_PS_generated)).Returns(new[] { 2470.5180.SI<NormLiter>() });
		m.Setup(x => x.FuelData).Returns(new IFuelProperties[] { Fuel });
		m.Setup(x => x.TimeIntegral<Kilogram>(It.IsIn(Fuel.FuelType.GetLabel()), null)).Returns<string, Func<SI, bool>>((f,_) => totalFuelConsumption);
		m.Setup(x => x.GetColumnName(It.IsNotNull<IFuelProperties>(), It.IsNotNull<ModalResultField>()))
			.Returns<IFuelProperties, ModalResultField>((f, m) => f.GetLabel());
		m.Setup(x => x.EngineLineCorrectionFactor(It.IsIn(Fuel)))
			.Returns(200.SI(Unit.SI.Kilo.Gramm.Per.Kilo.Watt.Hour).Cast<KilogramPerWattSecond>());
        m.Setup(x => x.AuxHeaterDemandCalc).Returns(busAux.AuxHeaterDemandCalculation);

		return m.Object;
	}


	protected IAuxiliaryConfig GetAuxParametersConventionalSingleDeckForTest(BusHVACSystemConfiguration hvacConfig, HeatPumpType? driverHeatpumpCooling = null,
		HeatPumpType? passengerHeatpumpCooling = null, HeatPumpType? driverHeatPumpHeating = null, HeatPumpType? passengerHeatPumpHeating = null,
		Watt auxHeaterPower = null, HeaterType? electricHeater = null, int passengerCount = 40, double length = 12, double height = 3)
	{
		return SSMBusAuxModelParameters.CreateBusAuxInputParameters(MissionType.Urban,
			VehicleClass.Class31a,
			VectoSimulationJobType.ConventionalVehicle,
			VehicleCode.CA,
			RegistrationClass.II,
			AxleConfiguration.AxleConfig_4x2,
			articulated: false,
			lowEntry: false,
			length: length.SI<Meter>(),
			height: height.SI<Meter>(),
			width: 2.55.SI<Meter>(),
			numPassengersLowerdeck: passengerCount,
			numPassengersUpperdeck: 0,
			hpHeatingDriver: driverHeatPumpHeating ?? HeatPumpType.none,
			hpCoolingDriver: driverHeatpumpCooling ?? HeatPumpType.none,
			hpHeatingPassenger: passengerHeatPumpHeating ?? HeatPumpType.none,
			hpCoolingPassenger: passengerHeatpumpCooling ?? HeatPumpType.none,
			auxHeaterPower: auxHeaterPower ?? 0.SI<Watt>(),
			airElectricHeater: electricHeater != null && (electricHeater & HeaterType.AirElectricHeater) != 0,
			waterElectricHeater: electricHeater != null && (electricHeater & HeaterType.WaterElectricHeater) != 0,
			otherElectricHeater: electricHeater != null && (electricHeater & HeaterType.OtherElectricHeating) != 0,
			hvacConfig,
			doubleGlazing: false,
			adjustableAuxHeater: false,
			separateAirdistributionDicts: false,
			adjustableCoolantThermostat: false,
			engineWasteGasHeatExchanger: false,
			steeringpumps: new[] { "Dual displacement" },
			fanTech: "Crankshaft mounted - Discrete step clutch",
			alternatorTech: AlternatorType.Conventional, entranceHeight: 0.3.SI<Meter>());

	}
}