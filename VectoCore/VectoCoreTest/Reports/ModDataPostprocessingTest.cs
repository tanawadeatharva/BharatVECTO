using System;
using System.Collections.Generic;
using NUnit.Framework;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Engine;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.Tests.Utils;

namespace TUGraz.VectoCore.Tests.Reports
{
	[TestFixture]
	[Parallelizable]
	public class ModDataPostprocessingTest
	{

		[TestCase()]
		public void TestAuxESSStandstill_ModDataCorrection()
		{
			var runData = GetRunData();
			var modData = new ModalDataContainer(runData, null, null);

			var fuel = runData.EngineData.Fuels[0];

			var absTime = 0.SI<Second>();
			var rnd = new Random(210629);
			var dt = 0.SI<Second>();

			var i = 0;
			// constant driving
			var n_ice = 800.RPMtoRad();
			var t_ice = 400.SI<NewtonMeter>();
			for (; i < 100; i++) {
				dt = (rnd.NextDouble() * 0.2 + 0.4).SI<Second>();

				modData[ModalResultField.time] = absTime;
				modData[ModalResultField.simulationInterval] = dt;

				modData[ModalResultField.v_act] = 50.KMPHtoMeterPerSecond();
				modData[ModalResultField.ICEOn] = true;

				modData[ModalResultField.n_ice_avg] = n_ice;
				modData[ModalResultField.P_ice_fcmap] = n_ice * t_ice;
				var fc = fuel.ConsumptionMap.GetFuelConsumption(t_ice, n_ice).Value *
						fuel.FuelConsumptionCorrectionFactor;
				modData[ModalResultField.FCWHTCc] = fc;
				modData[ModalResultField.FCFinal] = fc;

				modData[ModalResultField.P_aux_ESS_mech_ice_off] = 0.SI<Watt>();
				modData[ModalResultField.P_aux_ESS_mech_ice_on] = 0.SI<Watt>();

				// WHR
				modData[ModalResultField.P_WHR_el_corr] = 0.SI<Watt>();
				modData[ModalResultField.P_WHR_mech_corr] = 0.SI<Watt>();
				
				modData.CommitSimulationStep();
				absTime += dt;
			}

			// standstill
			for (; i < 200; i++) {
				dt = (rnd.NextDouble() * 0.2 + 0.4).SI<Second>();

				modData[ModalResultField.time] = absTime;
				modData[ModalResultField.simulationInterval] = dt;

				modData[ModalResultField.v_act] = 0.KMPHtoMeterPerSecond();
				modData[ModalResultField.ICEOn] = false;

				modData[ModalResultField.n_ice_avg] = 0.RPMtoRad();
				modData[ModalResultField.P_ice_fcmap] = 0.SI<Watt>();
				modData[ModalResultField.FCWHTCc] = 0.SI<KilogramPerSecond>();
				modData[ModalResultField.FCFinal] = 0.SI<KilogramPerSecond>();

				modData[ModalResultField.P_aux_ESS_mech_ice_off] = 300.SI<Watt>();
				modData[ModalResultField.P_aux_ESS_mech_ice_on] = 900.SI<Watt>();

				// WHR
				modData[ModalResultField.P_WHR_el_corr] = 0.SI<Watt>();
				modData[ModalResultField.P_WHR_mech_corr] = 0.SI<Watt>();

				modData.CommitSimulationStep();
				absTime += dt;
			}

			// constant driving different speed
			modData[ModalResultField.P_ice_start] = 0000.SI<Watt>();
			n_ice = 1200.RPMtoRad();
			for (; i < 300; i++) {
				dt = (rnd.NextDouble() * 0.2 + 0.4).SI<Second>();

				modData[ModalResultField.time] = absTime;
				modData[ModalResultField.simulationInterval] = dt;

				modData[ModalResultField.v_act] = 50.KMPHtoMeterPerSecond();
				modData[ModalResultField.ICEOn] = true;

				modData[ModalResultField.n_ice_avg] = n_ice;
				modData[ModalResultField.P_ice_fcmap] = n_ice * t_ice;
				var fc = fuel.ConsumptionMap.GetFuelConsumption(400.SI<NewtonMeter>(), n_ice).Value *
					fuel.FuelConsumptionCorrectionFactor;
				modData[ModalResultField.FCWHTCc] = fc;
				modData[ModalResultField.FCFinal] = fc;


				modData[ModalResultField.P_aux_ESS_mech_ice_off] = 0.SI<Watt>();
				modData[ModalResultField.P_aux_ESS_mech_ice_on] = 0.SI<Watt>();

				// WHR
				modData[ModalResultField.P_WHR_el_corr] = 0.SI<Watt>();
				modData[ModalResultField.P_WHR_mech_corr] = 0.SI<Watt>();

				modData.CommitSimulationStep();
				absTime += dt;
			}

			var corr = modData.CorrectedModalData as CorrectedModalData;

			Assert.AreEqual(2.625452e-8, modData.EngineLineCorrectionFactor(fuel.FuelData).Value(), 1e-12);
			Assert.AreEqual(0, corr.WorkWHREl.Value());
			Assert.AreEqual(0, corr.WorkWHRElMech.Value());
			Assert.AreEqual(0, corr.WorkWHRMech.Value());

			Assert.AreEqual(46054.609, corr.EnergyAuxICEOnStandstill.Value(), 1e-3);
			Assert.AreEqual(15351.536, corr.EnergyAuxICEOffStandstill.Value(), 1e-3);

			Assert.AreEqual(0, corr.EnergyAuxICEOnDriving.Value(), 1e-3);
			Assert.AreEqual(0, corr.EnergyAuxICEOffDriving.Value(), 1e-3);


			var f = corr.FuelCorrection[fuel.FuelData.FuelType] as FuelConsumptionCorrection;

			var fcEssStandStillOff = 0.0002821330689;
			var fcEssStandStillOn = 0.0048594804294;
			Assert.AreEqual(fcEssStandStillOff, f.FcESS_AuxStandstill_ICEOff.Value(), 1e-12);
			Assert.AreEqual(fcEssStandStillOn, f.FcESS_AuxStandstill_ICEOn.Value(), 1e-12);
			Assert.AreEqual(0, f.FcESS_AuxDriving_ICEOff.Value(), 1e-12);
			Assert.AreEqual(0, f.FcESS_AuxDriving_ICEOn.Value(), 1e-12);


			var fcModSum = 0.195725;
			var fcIdle =
				fuel.ConsumptionMap.GetFuelConsumption(0.SI<NewtonMeter>(), runData.EngineData.IdleSpeed).Value
					.Value() * fuel.FuelConsumptionCorrectionFactor;
			Assert.AreEqual(fcModSum, modData.TotalFuelConsumption(ModalResultField.FCWHTCc, fuel.FuelData).Value(), 1e-6);
			Assert.AreEqual(fcModSum + fcEssStandStillOff + fcEssStandStillOn , f.FcEssCorr.Value(), 1e-6);

		}



		[TestCase()]
		public void TestAuxESSDriving_ModDataCorrection()
		{
			var runData = GetRunData();
			var modData = new ModalDataContainer(runData, null, null);

			var fuel = runData.EngineData.Fuels[0];

			var absTime = 0.SI<Second>();
			var rnd = new Random(210629);
			var dt = 0.SI<Second>();

			var i = 0;
			// constant driving
			var n_ice = 800.RPMtoRad();
			var t_ice = 400.SI<NewtonMeter>();
			for (; i < 100; i++) {
				dt = (rnd.NextDouble() * 0.2 + 0.4).SI<Second>();

				modData[ModalResultField.time] = absTime;
				modData[ModalResultField.simulationInterval] = dt;

				modData[ModalResultField.v_act] = 50.KMPHtoMeterPerSecond();
				modData[ModalResultField.ICEOn] = true;

				modData[ModalResultField.n_ice_avg] = n_ice;
				modData[ModalResultField.P_ice_fcmap] = n_ice * t_ice;
				var fc = fuel.ConsumptionMap.GetFuelConsumption(t_ice, n_ice).Value *
						fuel.FuelConsumptionCorrectionFactor;
				modData[ModalResultField.FCWHTCc] = fc;
				modData[ModalResultField.FCFinal] = fc;

				modData[ModalResultField.P_aux_ESS_mech_ice_off] = 0.SI<Watt>();
				modData[ModalResultField.P_aux_ESS_mech_ice_on] = 0.SI<Watt>();

				// WHR
				modData[ModalResultField.P_WHR_el_corr] = 0.SI<Watt>();
				modData[ModalResultField.P_WHR_mech_corr] = 0.SI<Watt>();

				modData.CommitSimulationStep();
				absTime += dt;
			}

			// driving ICE Off
			for (; i < 200; i++) {
				dt = (rnd.NextDouble() * 0.2 + 0.4).SI<Second>();

				modData[ModalResultField.time] = absTime;
				modData[ModalResultField.simulationInterval] = dt;

				modData[ModalResultField.v_act] = 50.KMPHtoMeterPerSecond();
				modData[ModalResultField.ICEOn] = false;

				modData[ModalResultField.n_ice_avg] = 0.RPMtoRad();
				modData[ModalResultField.P_ice_fcmap] = 0.SI<Watt>();
				modData[ModalResultField.FCWHTCc] = 0.SI<KilogramPerSecond>();
				modData[ModalResultField.FCFinal] = 0.SI<KilogramPerSecond>();

				modData[ModalResultField.P_aux_ESS_mech_ice_off] = 300.SI<Watt>();
				modData[ModalResultField.P_aux_ESS_mech_ice_on] = 900.SI<Watt>();

				// WHR
				modData[ModalResultField.P_WHR_el_corr] = 0.SI<Watt>();
				modData[ModalResultField.P_WHR_mech_corr] = 0.SI<Watt>();

				modData.CommitSimulationStep();
				absTime += dt;
			}

			// constant driving different speed
			modData[ModalResultField.P_ice_start] = 0000.SI<Watt>();
			n_ice = 1200.RPMtoRad();
			for (; i < 300; i++) {
				dt = (rnd.NextDouble() * 0.2 + 0.4).SI<Second>();

				modData[ModalResultField.time] = absTime;
				modData[ModalResultField.simulationInterval] = dt;

				modData[ModalResultField.v_act] = 50.KMPHtoMeterPerSecond();
				modData[ModalResultField.ICEOn] = true;

				modData[ModalResultField.n_ice_avg] = n_ice;
				modData[ModalResultField.P_ice_fcmap] = n_ice * t_ice;
				var fc = fuel.ConsumptionMap.GetFuelConsumption(400.SI<NewtonMeter>(), n_ice).Value *
					fuel.FuelConsumptionCorrectionFactor;
				modData[ModalResultField.FCWHTCc] = fc;
				modData[ModalResultField.FCFinal] = fc;


				modData[ModalResultField.P_aux_ESS_mech_ice_off] = 0.SI<Watt>();
				modData[ModalResultField.P_aux_ESS_mech_ice_on] = 0.SI<Watt>();

				// WHR
				modData[ModalResultField.P_WHR_el_corr] = 0.SI<Watt>();
				modData[ModalResultField.P_WHR_mech_corr] = 0.SI<Watt>();

				modData.CommitSimulationStep();
				absTime += dt;
			}

			var corr = modData.CorrectedModalData as CorrectedModalData;

			Assert.AreEqual(2.625452e-8, modData.EngineLineCorrectionFactor(fuel.FuelData).Value(), 1e-12);
			Assert.AreEqual(0, corr.WorkWHREl.Value());
			Assert.AreEqual(0, corr.WorkWHRElMech.Value());
			Assert.AreEqual(0, corr.WorkWHRMech.Value());

			Assert.AreEqual(0, corr.EnergyAuxICEOnStandstill.Value(), 1e-3);
			Assert.AreEqual(0, corr.EnergyAuxICEOffStandstill.Value(), 1e-3);

			Assert.AreEqual(46054.609, corr.EnergyAuxICEOnDriving.Value(), 1e-3);
			Assert.AreEqual(15351.536, corr.EnergyAuxICEOffDriving.Value(), 1e-3);

			var f = corr.FuelCorrection[fuel.FuelData.FuelType] as FuelConsumptionCorrection;

			var fcEssDrivingOff = 0.0003224377930;
			var fcEssDrivingOn = 0.0032396536196;
			Assert.AreEqual(0, f.FcESS_AuxStandstill_ICEOff.Value(), 1e-12);
			Assert.AreEqual(0, f.FcESS_AuxStandstill_ICEOn.Value(), 1e-12);
			Assert.AreEqual(fcEssDrivingOff, f.FcESS_AuxDriving_ICEOff.Value(), 1e-12);
			Assert.AreEqual(fcEssDrivingOn, f.FcESS_AuxDriving_ICEOn.Value(), 1e-12);

			var fcModSum = 0.195725;
			var fcIdle =
				fuel.ConsumptionMap.GetFuelConsumption(0.SI<NewtonMeter>(), runData.EngineData.IdleSpeed).Value
					.Value() * fuel.FuelConsumptionCorrectionFactor;
			Assert.AreEqual(fcModSum, modData.TotalFuelConsumption(ModalResultField.FCWHTCc, fuel.FuelData).Value(), 1e-6);
			Assert.AreEqual(fcModSum + fcEssDrivingOff + fcEssDrivingOn, f.FcEssCorr.Value(), 1e-6);
		}

		[TestCase()]
		public void TestBusAuxPSESSStandstill_ModDataCorrection()
		{
			Assert.Fail();

		}

		[TestCase()]
		public void TestBusAuxPSESSDriving_ModDataCorrection()
		{
			Assert.Fail();

		}

		[TestCase()]
		public void TestBusAuxSmartES_ModDataCorrection()
		{
			Assert.Fail();

		}

		[TestCase()]
		public void TestBusAuxDCDCMissing_ModDataCorrection()
		{
			Assert.Fail();

		}

		[TestCase()]
		public void TestBusAuxSmartPS_ModDataCorrection()
		{
			Assert.Fail();

		}

		[TestCase()]
		public void TestBusAuxPSDemand_ModDataCorrection()
		{
			Assert.Fail();

		}

		[TestCase()]
		public void TestWHRElAlternator_ModDataCorrection()
		{
			Assert.Fail();

		}

		[TestCase()]
		public void TestWHRElNoAlternator_ModDataCorrection()
		{
			Assert.Fail();

		}

		[TestCase]
		public void TestWHRMech_ModDataCorrection()
		{
			Assert.Fail();

		}


		private VectoRunData GetRunData()
		{
			var fcMapHeader = "engine speed [rpm],torque [Nm],fuel consumption [g/h]";
			var fcMapEntries = new[] {
				"600,-107,0",
				"600,0,1042",
				"600,916,10963",
				"2000,-215,0",
				"2000,0,6519",
				"2000,966,40437"
			};

			return new VectoRunData() {
				DriverData = new DriverData() {
					EngineStopStart = new DriverData.EngineStopStartData() {
						UtilityFactorDriving = 0.8,
						UtilityFactorStandstill = 0.7
					}
				},
				EngineData = new CombustionEngineData() {
					IdleSpeed = 600.RPMtoRad(),
					Fuels = new List<CombustionEngineFuelData>() {
						new CombustionEngineFuelData() {
							FuelData = FuelData.Diesel,
							FuelConsumptionCorrectionFactor = 1.012,
							ConsumptionMap = FuelConsumptionMapReader.ReadFromStream(InputDataHelper.InputDataAsStream(fcMapHeader, fcMapEntries))
						}
					}
				}
			};
		}
	}
}