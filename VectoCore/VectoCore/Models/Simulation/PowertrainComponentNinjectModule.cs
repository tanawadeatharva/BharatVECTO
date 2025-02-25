using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using Ninject.Extensions.Factory;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.Electrics;
using TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules.Electrics;
using TUGraz.VectoCore.Models.Connector.Ports;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.ElectricComponents.Battery;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Impl.Gearbox;
using TUGraz.VectoCore.Models.SimulationComponent.Strategies;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.Common;
using TUGraz.VectoCore.Utils.Ninject;

namespace TUGraz.VectoCore.Models.Simulation
{

    public class PowertrainComponentNinjectModule : VectoNinjectModule
	{
		
		private ComponentBindingNameHelper _realPowertrain = new ComponentBindingNameHelper("RealPowertrain");
		private ComponentBindingNameHelper _testPowertrain = new ComponentBindingNameHelper("TestPowertrain");

		const GearboxType MTGearbox = GearboxType.MT;
		const GearboxType AMTGearbox = GearboxType.AMT;
		const GearboxType APTSGearbox = GearboxType.ATSerial;
		const GearboxType APTPGearbox = GearboxType.ATPowerSplit;
		const GearboxType APTNGearbox = GearboxType.APTN;
		const GearboxType IHPCGearbox = GearboxType.IHPC;
		const GearboxType IEPCGearbox = GearboxType.IEPC;
		const GearboxType Drivingcycle = GearboxType.DrivingCycle;

        private Dictionary<Tuple<CycleType, VectoSimulationJobType>, Dictionary<GearboxType, Type>> GearboxConfig() 
		{
			var mtGearboxT = typeof(MTGearbox);
			var amtGearboxT = typeof(AMTGearbox);
			var aptGearboxT = typeof(APTGearbox);
			var aptnGearboxT = typeof(APTNGearbox);
			var pevGearboxT = typeof(PEVGearbox);
			var iepcGearboxT = typeof(IEPCGearbox);
			var measSpdPHEVGbxT = typeof(MeasuredSpeedHybridsGearbox);
			var measSpdPHEVGearGbxT = typeof(MeasuredSpeedHybridsCycleGearbox);
			var bevCycleGbxT = typeof(BEVCycleGearbox);
			var cycleGearboxT = typeof(CycleGearbox);

            #region distance based
            var GbxClass_Distance_Conv = new Dictionary<GearboxType, Type>() {
				{MTGearbox, mtGearboxT},
				{AMTGearbox, amtGearboxT},
				{APTSGearbox, aptGearboxT},
				{APTPGearbox, aptGearboxT},
			};
			var GbxClass_Distance_PHEV = new Dictionary<GearboxType, Type>() {
				{IHPCGearbox, aptnGearboxT}
			};
			var GbxClass_Distance_IHPC = new Dictionary<GearboxType, Type>() {
				{IHPCGearbox, aptnGearboxT}
			};
			var GbxClass_Distance_SHEV = new Dictionary<GearboxType, Type>() {
				{AMTGearbox, pevGearboxT},
				{APTSGearbox, pevGearboxT},
				{APTPGearbox, pevGearboxT},
				{APTNGearbox, aptnGearboxT},
			};
			var GbxClass_Distance_SIEPC = new Dictionary<GearboxType, Type>() {
				{IEPCGearbox, iepcGearboxT},
			};
			var GbxClass_Distance_PEV = new Dictionary<GearboxType, Type>() {
				{AMTGearbox, pevGearboxT},
				{APTSGearbox, pevGearboxT},
				{APTPGearbox, pevGearboxT},
				{APTNGearbox, aptnGearboxT},
				{IHPCGearbox, pevGearboxT}
			};
	        var GbxClass_Distance_EIEPC = new Dictionary<GearboxType, Type>() {
				{IEPCGearbox, iepcGearboxT},
			};
            #endregion

            #region measured speed
			var GbxClass_MeasSpd_Conv = new Dictionary<GearboxType, Type>() {
				{MTGearbox, mtGearboxT},
				{AMTGearbox, amtGearboxT},
				{APTSGearbox, aptGearboxT},
				{APTPGearbox, aptGearboxT},
			};
			var GbxClass_MeasSpd_PHEV = new Dictionary<GearboxType, Type>() {
				{AMTGearbox, measSpdPHEVGbxT},
				{APTSGearbox, aptGearboxT},
				{APTPGearbox, aptGearboxT},
			};
			var GbxClass_MeasSpd_IHPC = new Dictionary<GearboxType, Type>() {
				{IHPCGearbox, aptnGearboxT}
			};
			var GbxClass_MeasSpd_PEV = new Dictionary<GearboxType, Type>() {
				{AMTGearbox, pevGearboxT},
				{APTSGearbox, pevGearboxT},
				{APTPGearbox, pevGearboxT},
				{APTNGearbox, aptnGearboxT},
			};
			var GbxClass_MeasSpd_EIEPC = new Dictionary<GearboxType, Type>() {
				{IEPCGearbox, iepcGearboxT},
			};
            #endregion

            #region measured speed gear
			var GbxClass_MeasSpdGear_Conv = new Dictionary<GearboxType, Type>() {
				{MTGearbox, cycleGearboxT},
				{AMTGearbox, cycleGearboxT},
				{APTSGearbox, cycleGearboxT},
				{APTPGearbox, cycleGearboxT},
			};
			var GbxClass_MeasSpdGear_PHEV = new Dictionary<GearboxType, Type>() {
				{AMTGearbox, measSpdPHEVGearGbxT},
				{APTSGearbox, measSpdPHEVGearGbxT},
				{APTPGearbox, measSpdPHEVGearGbxT},
			};
			var GbxClass_MeasSpdGear_IHPC = new Dictionary<GearboxType, Type>() {
				{IHPCGearbox, measSpdPHEVGearGbxT}
			};
            var GbxClass_MeasSpdGear_PEV = new Dictionary<GearboxType, Type>() {
				{AMTGearbox, bevCycleGbxT},
				{APTSGearbox, bevCycleGbxT},
				{APTPGearbox, bevCycleGbxT},
				{APTNGearbox, bevCycleGbxT},
			};
			var GbxClass_MeasSpdGear_EIEPC = new Dictionary<GearboxType, Type>() {
				{IEPCGearbox, bevCycleGbxT},
			};
            #endregion

			#region PWheel
			var GbxClass_PWheel_Conv = new Dictionary<GearboxType, Type>() {
				{MTGearbox, cycleGearboxT},
				{AMTGearbox, cycleGearboxT},
				{APTSGearbox, cycleGearboxT},
				{APTPGearbox, cycleGearboxT},
			};
			var GbxClass_PWheel_PEV = new Dictionary<GearboxType, Type>() {
				{AMTGearbox, bevCycleGbxT},
				{APTSGearbox, bevCycleGbxT},
				{APTPGearbox, bevCycleGbxT},
				{APTNGearbox, bevCycleGbxT},
			};
			var GbxClass_PWheel_EIEPC = new Dictionary<GearboxType, Type>() {
				{IEPCGearbox, bevCycleGbxT},
			};
            #endregion

			#region PWheel
			var GbxClass_VTP_Conv = new Dictionary<GearboxType, Type>() {
				{MTGearbox, cycleGearboxT},
				{AMTGearbox, cycleGearboxT},
				{APTSGearbox, cycleGearboxT},
				{APTPGearbox, cycleGearboxT},
			};
			#endregion

            return new Dictionary<Tuple<CycleType, VectoSimulationJobType>, Dictionary<GearboxType, Type>>() {
					// distance based cycles
					{ Tuple.Create(CycleType.DistanceBased, VectoSimulationJobType.ConventionalVehicle) , GbxClass_Distance_Conv},
					{ Tuple.Create(CycleType.DistanceBased, VectoSimulationJobType.ParallelHybridVehicle) , GbxClass_Distance_PHEV},
					{ Tuple.Create(CycleType.DistanceBased, VectoSimulationJobType.IHPC) , GbxClass_Distance_IHPC},
					{ Tuple.Create(CycleType.DistanceBased, VectoSimulationJobType.SerialHybridVehicle) , GbxClass_Distance_SHEV},
					{ Tuple.Create(CycleType.DistanceBased, VectoSimulationJobType.IEPC_S) , GbxClass_Distance_SIEPC},
					{ Tuple.Create(CycleType.DistanceBased, VectoSimulationJobType.BatteryElectricVehicle) , GbxClass_Distance_PEV},
					{ Tuple.Create(CycleType.DistanceBased, VectoSimulationJobType.IEPC_E) , GbxClass_Distance_EIEPC},
					// measured speed cycles
					{ Tuple.Create(CycleType.MeasuredSpeed, VectoSimulationJobType.ConventionalVehicle) , GbxClass_MeasSpd_Conv},
					{ Tuple.Create(CycleType.MeasuredSpeed, VectoSimulationJobType.ParallelHybridVehicle) , GbxClass_MeasSpd_PHEV},
					{ Tuple.Create(CycleType.MeasuredSpeed, VectoSimulationJobType.IHPC) , GbxClass_MeasSpd_IHPC},
					{ Tuple.Create(CycleType.MeasuredSpeed, VectoSimulationJobType.BatteryElectricVehicle) , GbxClass_MeasSpd_PEV},
					{ Tuple.Create(CycleType.MeasuredSpeed, VectoSimulationJobType.IEPC_E) , GbxClass_MeasSpd_EIEPC},
					// measured speed with gear cycles
					{ Tuple.Create(CycleType.MeasuredSpeedGear, VectoSimulationJobType.ConventionalVehicle) , GbxClass_MeasSpdGear_Conv},
					{ Tuple.Create(CycleType.MeasuredSpeedGear, VectoSimulationJobType.ParallelHybridVehicle) , GbxClass_MeasSpdGear_PHEV},
					{ Tuple.Create(CycleType.MeasuredSpeedGear, VectoSimulationJobType.IHPC) , GbxClass_MeasSpdGear_IHPC},
					{ Tuple.Create(CycleType.MeasuredSpeedGear, VectoSimulationJobType.BatteryElectricVehicle) , GbxClass_MeasSpdGear_PEV},
					{ Tuple.Create(CycleType.MeasuredSpeedGear, VectoSimulationJobType.IEPC_E) , GbxClass_MeasSpdGear_EIEPC},
					// pwheel cycles
					{ Tuple.Create(CycleType.PWheel, VectoSimulationJobType.ConventionalVehicle) , GbxClass_PWheel_Conv},
					{ Tuple.Create(CycleType.PWheel, VectoSimulationJobType.BatteryElectricVehicle) , GbxClass_PWheel_PEV},
					{ Tuple.Create(CycleType.PWheel, VectoSimulationJobType.IEPC_E) , GbxClass_PWheel_EIEPC},
					// VTP cycles
					{ Tuple.Create(CycleType.VTP, VectoSimulationJobType.ConventionalVehicle) , GbxClass_VTP_Conv},
				};
		}

		private Dictionary<Tuple<CycleType, VectoSimulationJobType>, Dictionary<GearboxType, Type>> GearboxTestpowertrainConfig()
		{
			var amtGearboxT = typeof(TestPowertrainGearbox);
			var aptGearboxT = typeof(TestPowertrainAPTGearbox);
			var aptnGearboxT = typeof(TestPowertrainAPTNGearbox);


			#region distance based
			var GbxClass_Distance_Conv = new Dictionary<GearboxType, Type>() {
				{MTGearbox, amtGearboxT},
				{AMTGearbox, amtGearboxT},
				{APTSGearbox, aptGearboxT},
				{APTPGearbox, aptGearboxT},
			};
			var GbxClass_Distance_PHEV = new Dictionary<GearboxType, Type>() {
				{AMTGearbox, amtGearboxT},
				{APTSGearbox, aptGearboxT},
				{APTPGearbox, aptGearboxT},
				{IHPCGearbox, aptnGearboxT}
			};
			var GbxClass_Distance_SHEV = new Dictionary<GearboxType, Type>() {
				{AMTGearbox, amtGearboxT},
				{APTSGearbox, amtGearboxT},
				{APTPGearbox, aptnGearboxT},
				{APTNGearbox, aptnGearboxT},
			};
			//var GbxClass_Distance_SIEPC = new Dictionary<GearboxType, Type>() {
			//	{IEPCGearbox, iepcGearboxT},
			//};
			//var GbxClass_Distance_PEV = new Dictionary<GearboxType, Type>() {
			//	{AMTGearbox, pevGearboxT},
			//	{APTSGearbox, pevGearboxT},
			//	{APTPGearbox, pevGearboxT},
			//	{APTNGearbox, aptnGearboxT},
			//	{IHPCGearbox, pevGearboxT}
			//};
			//var GbxClass_Distance_EIEPC = new Dictionary<GearboxType, Type>() {
			//	{IEPCGearbox, iepcGearboxT},
			//};
			#endregion

            return new Dictionary<Tuple<CycleType, VectoSimulationJobType>, Dictionary<GearboxType, Type>>() {
				// distance based cycles
				{ Tuple.Create(CycleType.DistanceBased, VectoSimulationJobType.ConventionalVehicle) , GbxClass_Distance_Conv},
				{ Tuple.Create(CycleType.DistanceBased, VectoSimulationJobType.ParallelHybridVehicle) , GbxClass_Distance_PHEV},
				{ Tuple.Create(CycleType.DistanceBased, VectoSimulationJobType.SerialHybridVehicle) , GbxClass_Distance_SHEV},
				//{ Tuple.Create(CycleType.DistanceBased, VectoSimulationJobType.IEPC_S) , GbxClass_Distance_SIEPC},

            };
		}

        public override void Load()
		{
			#region Setup Factories for powertrain components

			Bind<IPowertrainComponentFactory>().ToFactory(() => GetMethodSettings(_realPowertrain))
				.WhenInjectedExactlyInto<PowertrainBuilder>().InSingletonScope();

			Bind<IPowertrainComponentFactory>().ToFactory(() => GetMethodSettings(_testPowertrain))
				.WhenInjectedExactlyInto<SimplePowertrainBuilder>().InSingletonScope();

			#endregion

			#region Real Powertrain

			AddCommonMappings(_realPowertrain);

            Bind<IVehicle>().To<Vehicle>().Named(_realPowertrain.Prefix);
			Bind<ICombustionEngine>().To<StopStartCombustionEngine>().Named(_realPowertrain.ICEName(false));
			Bind<ICombustionEngine>().To<EngineOnlyCombustionEngine>().Named(_realPowertrain.ICEName(true));
			
			Bind<IElectricSystem>().To<ElectricSystem>().Named(_realPowertrain.Prefix);
			Bind<IElectricChargerPort>().To<GensetChargerAdapter>().Named(_realPowertrain.Prefix);
			Bind<IElectricMotor>().To<ElectricMotor>().Named(_realPowertrain.ElectricMotorName(false));
			Bind<IElectricMotor>().To<IEPC>().Named(_realPowertrain.ElectricMotorName(true));

			foreach (var entry in GearboxConfig()) {
				foreach (var gbxType in entry.Value) {
					Bind<IGearbox>().To(gbxType.Value).Named(_realPowertrain.GearboxName(entry.Key.Item2, entry.Key.Item1, gbxType.Key));
				}
			}

			Bind<IHybridController>().To<HybridController>().WhenInjectedInto<IPowertrainBuilder>(); // fallback binding
			Bind<IHybridController>().To<MeasuredSpeedGearHybridController>().Named(_realPowertrain.HybridControllerName(CycleType.MeasuredSpeedGear));
			
			Bind<ISerialHybridController>().To<SerialHybridController>().WhenInjectedInto<IPowertrainBuilder>(); // fallback binding

            // CycleType == MeasuredSpeedGear
            //var strategy = (data.GearboxData.Type.ManualTransmission() || data.GearboxData.Type == GearboxType.IHPC) 
            //	? (IHybridControlStrategy) new MeasuredSpeedGearHybridStrategy(data, container)
            //	: (IHybridControlStrategy) new MeasuredSpeedGearATHybridStrategy(data, container);

            Bind<IHybridControlStrategy>().To<MeasuredSpeedGearHybridStrategy>()
				.Named(_realPowertrain.HybridStrategyName(VectoSimulationJobType.ParallelHybridVehicle, CycleType.MeasuredSpeedGear, false));
			Bind<IHybridControlStrategy>().To<MeasuredSpeedGearATHybridStrategy>()
				.Named(_realPowertrain.HybridStrategyName(VectoSimulationJobType.ParallelHybridVehicle, CycleType.MeasuredSpeedGear, true));
			Bind<IHybridControlStrategy>().To<MeasuredSpeedGearHybridStrategy>()
				.Named(_realPowertrain.HybridStrategyName(VectoSimulationJobType.IHPC, CycleType.MeasuredSpeedGear, false));
			Bind<IHybridControlStrategy>().To<MeasuredSpeedGearATHybridStrategy>()
				.Named(_realPowertrain.HybridStrategyName(VectoSimulationJobType.IHPC, CycleType.MeasuredSpeedGear, true));

            // CycleType == MeasuredSpeed
            //var strategy = (data.GearboxData.Type.ManualTransmission() || data.GearboxData.Type == GearboxType.IHPC)
            //	? (IHybridControlStrategy) new MeasuredSpeedHybridStrategy(data, container)
            //	: (IHybridControlStrategy) new MeasuredSpeedHybridStrategyAT(data, container);

            Bind<IHybridControlStrategy>().To<MeasuredSpeedHybridStrategy>()
				.Named(_realPowertrain.HybridStrategyName(VectoSimulationJobType.ParallelHybridVehicle,CycleType.MeasuredSpeed, false));
			Bind<IHybridControlStrategy>().To<MeasuredSpeedHybridStrategyAT>()
				.Named(_realPowertrain.HybridStrategyName(VectoSimulationJobType.ParallelHybridVehicle, CycleType.MeasuredSpeed, true));
			Bind<IHybridControlStrategy>().To<MeasuredSpeedHybridStrategy>()
				.Named(_realPowertrain.HybridStrategyName(VectoSimulationJobType.IHPC, CycleType.MeasuredSpeed, false));
			Bind<IHybridControlStrategy>().To<MeasuredSpeedHybridStrategyAT>()
				.Named(_realPowertrain.HybridStrategyName(VectoSimulationJobType.IHPC, CycleType.MeasuredSpeed, true));

            //if (data.GearboxData.Type.ManualTransmission() || data.GearboxData.Type == GearboxType.IHPC) {
            //	ctl = ComponentFactory.CreateHybridController(data.Cycle.CycleType, container, new HybridStrategy(data, container), es);
            //	clutch = new SwitchableClutch(container, data.EngineData);
            //} else {
            //	ctl = ComponentFactory.CreateHybridController(data.Cycle.CycleType, container, new HybridStrategyAT(data, container), es);
            //	ComponentFactory.CreateATClutchInfo(container);
            //}
            Bind<IHybridControlStrategy>().To<HybridStrategy>()
				.Named(_realPowertrain.HybridStrategyName(VectoSimulationJobType.ParallelHybridVehicle, CycleType.DistanceBased, false));
			Bind<IHybridControlStrategy>().To<HybridStrategyAT>()
				.Named(_realPowertrain.HybridStrategyName(VectoSimulationJobType.ParallelHybridVehicle, CycleType.DistanceBased, true));
			Bind<IHybridControlStrategy>().To<HybridStrategy>()
				.Named(_realPowertrain.HybridStrategyName(VectoSimulationJobType.IHPC, CycleType.DistanceBased, false));
			Bind<IHybridControlStrategy>().To<HybridStrategyAT>()
				.Named(_realPowertrain.HybridStrategyName(VectoSimulationJobType.IHPC, CycleType.DistanceBased, true));

			Bind<IHybridControlStrategy>().To<SerialHybridStrategy>()
				.Named(_realPowertrain.HybridStrategyName(VectoSimulationJobType.SerialHybridVehicle, CycleType.DistanceBased, false));
			Bind<IHybridControlStrategy>().To<SerialHybridStrategyAT>()
				.Named(_realPowertrain.HybridStrategyName(VectoSimulationJobType.SerialHybridVehicle, CycleType.DistanceBased, true));
			Bind<IHybridControlStrategy>().To<SerialHybridStrategy>()
				.Named(_realPowertrain.HybridStrategyName(VectoSimulationJobType.IEPC_S, CycleType.DistanceBased, false));


            Bind<IElectricMotorControl>().To<BatteryElectricMotorController>()
				.Named(_realPowertrain.ElectricMotorControllerName(CycleType.DistanceBased));
			Bind<IElectricMotorControl>().To<BatteryElectricMotorController>()
				.Named(_realPowertrain.ElectricMotorControllerName(CycleType.MeasuredSpeed));
			Bind<IElectricMotorControl>().To<BatteryElectricMotorController>()
				.Named(_realPowertrain.ElectricMotorControllerName(CycleType.MeasuredSpeedGear));
            Bind<IElectricMotorControl>().To<PWheelBatteryElectricMotorController>()
				.Named(_realPowertrain.ElectricMotorControllerName(CycleType.PWheel));

            #endregion

            #region Test Powertrain

            Bind<IClutch>().To<Clutch>(); // fallback binding

            AddCommonMappings(_testPowertrain);

            Bind<IVehicle>().To<TestPowertrainVehicle>().Named(_testPowertrain.Prefix);
			Bind<ICombustionEngine>().To<TestpowertrainCombustionEngine>().Named(_testPowertrain.ICEName(false));
			Bind<IElectricSystem>().To<TestpowertrainElectricSystem>().Named(_testPowertrain.Prefix);
			Bind<IElectricChargerPort>().To<TestpowertrainGensetChargerAdapter>().Named(_testPowertrain.Prefix);
			Bind<IElectricMotor>().To<TestpowertrainElectricMotor>().Named(_testPowertrain.ElectricMotorName(false));
			Bind<IElectricMotor>().To<TestpowertrainIEPC>().Named(_testPowertrain.ElectricMotorName(true));

			Bind<IHybridController>().To<SimpleHybridController>().WhenInjectedInto<ISimplePowertrainBuilder>(); // no differentiation for testpowertrain

            //if (data.GearboxData.Type.AutomaticTransmission()) {
            //	gearbox = new TestPowertrainAPTNGearbox(container, ctl.ShiftStrategy);
            //} else {
            //	gearbox = new TestPowertrainGearbox(container, ctl.ShiftStrategy);
            //}
            //Bind<IGearbox>().To<TestPowertrainGearbox>(); // fallback binding

			foreach (var entry in GearboxTestpowertrainConfig()) {
				foreach (var gbxType in entry.Value) {
					Bind<IGearbox>().To(gbxType.Value).Named(_testPowertrain.GearboxName(entry.Key.Item2, entry.Key.Item1, gbxType.Key));
				}
			}

            //Bind<IGearbox>().To<TestPowertrainAPTGearbox>().Named(_testPowertrain.GearboxName(CycleType.DistanceBased, GearboxType.ATPowerSplit));
            //Bind<IGearbox>().To<TestPowertrainAPTGearbox>().Named(_testPowertrain.GearboxName(CycleType.DistanceBased, GearboxType.ATSerial));
            //Bind<IGearbox>().To<TestPowertrainAPTGearbox>().Named(_testPowertrain.GearboxName(CycleType.DistanceBased, GearboxType.APTN));



            #endregion

        }

        private void AddCommonMappings(ComponentBindingNameHelper namingHelper)
		{
			Bind<IVehicleContainer>().To<VehicleContainer>().Named(namingHelper.Prefix);
			Bind<IExemptedVehicleContainer>().To<ExemptedVehicleContainer>().Named(namingHelper.Prefix);
			Bind<ISimpleVehicleContainer>().To<SimplePowertrainContainer>().Named(namingHelper.Prefix);

            Bind<IWheels>().To<Wheels>().Named(namingHelper.Prefix);
            Bind<IDistanceBasedDrivingCycle>().To<DistanceBasedDrivingCycle>().Named(namingHelper.Prefix);
            Bind<IMeasuredSpeedDrivingCycle>().To<MeasuredSpeedDrivingCycle>().Named(namingHelper.Prefix);
            Bind<IDriver>().To<Driver>().Named(namingHelper.Prefix);
            Bind<IDriverStrategy>().To<DefaultDriverStrategy>().Named(namingHelper.Prefix);
            Bind<IBrakes>().To<Brakes>().Named(namingHelper.Prefix);
            Bind<IAxlegear>().To<AxleGear>().Named(namingHelper.Prefix);
            Bind<IAngledrive>().To<Angledrive>().Named(namingHelper.Prefix);
            Bind<IRetarder>().To<Retarder>().Named(namingHelper.Prefix);

			// concrete bindings for Clutch
			Bind<IClutch>().To<SwitchableClutch>()
				.Named(namingHelper.ClutchName(VectoSimulationJobType.ParallelHybridVehicle));
			Bind<IClutch>().To<SwitchableClutch>()
				.Named(namingHelper.ClutchName(VectoSimulationJobType.IHPC));

            Bind<IClutchInfo>().To<ATClutchInfo>().Named(namingHelper.Prefix);
            Bind<IWHRCharger>().To<WHRCharger>().Named(namingHelper.Prefix);
            Bind<IDCDCConverter>().To<DCDCConverter>().Named(namingHelper.Prefix);
            Bind<IBusAuxiliariesAdapter>().To<BusAuxiliariesAdapter>().Named(namingHelper.Prefix);
            Bind<ISimpleBattery>().To<SimpleBattery>().Named(namingHelper.SimpleBatteryName(true));
            Bind<ISimpleBattery>().To<NoBattery>().Named(namingHelper.SimpleBatteryName(false));
            Bind<IElectricEnergyStorage>().To<SuperCap>().Named(namingHelper.REESSName(REESSType.SuperCap));
            Bind<IElectricEnergyStorage>().To<BatterySystem>().Named(namingHelper.REESSName(REESSType.Battery));

            Bind<IEngineInfo>().To<DummyEngineInfo>().Named(namingHelper.Prefix);
            Bind<IAxlegearInfo>().To<DummyAxleGearInfo>().Named(namingHelper.Prefix);
            Bind<IGearboxInfo>().To<DummyGearboxInfo>().Named(namingHelper.GearboxInfoName(false));
            Bind<IGearboxInfo>().To<EngineOnlyGearboxInfo>().Named(namingHelper.GearboxInfoName(true));
            Bind<IMileageCounter>().To<ZeroMileageCounter>().Named(namingHelper.Prefix);
            Bind<IDriverInfo>().To<DummyDriverInfo>().Named(namingHelper.Prefix);

            Bind<IPowertrainDrivingCycle>().To<PowertrainDrivingCycle>().Named(namingHelper.Prefix);
			Bind<IEngineAuxiliary>().To<EngineAuxiliary>().Named(namingHelper.Prefix);
		}

		
		private IInstanceProvider GetMethodSettings(ComponentBindingNameHelper namingHelper)
		{
			return new CombineArgumentsToNameInstanceProvider(true,
				// method settings for creating name bindings for combustion engine
				GetMethodSettings(namingHelper.CreatICEName, 1, typeof(IPowertrainComponentFactory).GetMethod(
					nameof(IPowertrainComponentFactory
						.CreateCombustionEngine))),
				// method settings for creating name bindings for gearbox
				GetMethodSettings(namingHelper.CreateGearboxName, 3, typeof(IPowertrainComponentFactory).GetMethod(
					nameof(IPowertrainComponentFactory
						.CreateGearbox))),
				// method settings for creating name bindings for gearboxInfo
				GetMethodSettings(namingHelper.CreateGearboxInfoName, 1, typeof(IPowertrainComponentFactory).GetMethod(
					nameof(IPowertrainComponentFactory
						.CreateDummyGearboxInfo))),
				// method settings for creating name bindings for clutch
				GetMethodSettings(namingHelper.CreateClutchName, 1, typeof(IPowertrainComponentFactory).GetMethod(
					nameof(IPowertrainComponentFactory
						.CreateClutch))),
				// method settings for creating electric motor/IEPC
				GetMethodSettings(namingHelper.CreateElectricMotorName, 1,
					typeof(IPowertrainComponentFactory).GetMethod(nameof(IPowertrainComponentFactory
						.CreateElectricMotor))),
				// method settings for creating electric motor controller
				GetMethodSettings(namingHelper.CreateElectricMotorControllerName, 1,
					typeof(IPowertrainComponentFactory).GetMethod(nameof(IPowertrainComponentFactory
						.CreateElectricMotorController))),
				// method settings for creating hybrid controller
				GetMethodSettings(namingHelper.CreateHybridControllerName, 1,
					typeof(IPowertrainComponentFactory).GetMethod(nameof(IPowertrainComponentFactory
						.CreateHybridController)),
					typeof(IPowertrainComponentFactory).GetMethod(nameof(IPowertrainComponentFactory
						.CreateSerialHybridController))),
				// method settings for creating hybrid strategy
				GetMethodSettings(namingHelper.CreateHybridStrategyName, 3,
					typeof(IPowertrainComponentFactory).GetMethod(nameof(IPowertrainComponentFactory
						.CreateHybridStrategy))),
				// method settings for creating SimpleBattery (bus aux)
				GetMethodSettings(namingHelper.CreateSimpleBatteryName, 1,
					typeof(IPowertrainComponentFactory).GetMethod(nameof(IPowertrainComponentFactory
						.CreateSimpleBattery))),
				// method settings for creating REESS (battery/supercap)
				GetMethodSettings(namingHelper.CreateREESSName, 1, typeof(IPowertrainComponentFactory).GetMethod(
						nameof(IPowertrainComponentFactory
							.CreateREESS),
						new Type[] { typeof(REESSType), typeof(IVehicleContainer), typeof(SuperCapData) }),
					typeof(IPowertrainComponentFactory).GetMethod(nameof(IPowertrainComponentFactory
							.CreateREESS),
						new Type[] { typeof(REESSType), typeof(IVehicleContainer), typeof(BatterySystemData) })),
				// fallback name binding for all other methods
				GetMethodSettings(namingHelper.CreateComponentName, 0, null));
		}

		private CombineArgumentsToNameInstanceProvider.MethodSettings GetMethodSettings(CombineArgumentsToNameInstanceProvider.CombineToName nameCreator, int paramCount, params MethodInfo[] methods)
		{
			return new CombineArgumentsToNameInstanceProvider.MethodSettings() {
				combineToNameDelegate = nameCreator,
				skipArguments = paramCount,
				takeArguments = paramCount,
				methods = methods
			};

		}
	}

	public class ComponentBindingNameHelper
	{
		private readonly string _prefix;

		public ComponentBindingNameHelper(string prefix)
		{
			_prefix = prefix;
		}

		public string Prefix => _prefix;

		public string CreateComponentName(object[] arguments)
		{
			return Prefix;
		}

		public string CreateGearboxName(object[] arguments) => CheckArguments<VectoSimulationJobType, CycleType, GearboxType>(arguments, GearboxName);

		public string GearboxName(VectoSimulationJobType jobType, CycleType cycle, GearboxType gbxType) => $"{_prefix}_{jobType.ToString()}_{cycle.ToString()}_{gbxType.ToString()}";
		
		public string CreateElectricMotorName(object[] arguments) => CheckArguments<bool>(arguments, ElectricMotorName);

		public string ElectricMotorName(bool isIepc) => $"{_prefix}_{(isIepc ? "_IEPC" : "_EM")}";

		public string CreateREESSName(object[] arguments) => CheckArguments<REESSType>(arguments, REESSName);

		public string REESSName(REESSType reessType) => $"{_prefix}_{reessType.ToString()}";

		public string CreateSimpleBatteryName(object[] arguments) => CheckArguments<bool>(arguments, SimpleBatteryName);

		public string SimpleBatteryName(bool smartAlternator) => $"{_prefix}_{(smartAlternator ? "smart" : "conv")}";

		public string CreateGearboxInfoName(object[] arguments) => CheckArguments<bool>(arguments, GearboxInfoName);

		public string GearboxInfoName(bool engineOnly) => $"{_prefix}_{(engineOnly ? "EngOnly" : "Full")}";

		public string CreatICEName(object[] arguments) => CheckArguments<bool>(arguments, ICEName);

		public string ICEName(bool engineOnly) => $"{_prefix}_{(engineOnly ? "EngOnly" : "Full")}";

		public string CreateHybridStrategyName(object[] arguments) => CheckArguments<VectoSimulationJobType,CycleType, bool>(arguments, HybridStrategyName);

        public string HybridStrategyName(VectoSimulationJobType jobType, CycleType cycleType, bool atTransmission) => $"{_prefix}_{jobType}_{cycleType}_{(atTransmission ? "AT" : "AMT")}";

		public string CreateHybridControllerName(object[] arguments) => CheckArguments<CycleType>(arguments, HybridControllerName);

        public string HybridControllerName(CycleType cycleType) => $"{_prefix}_{cycleType}";

		public string CreateClutchName(object[] arguments) => CheckArguments<VectoSimulationJobType>(arguments, ClutchName);

		public string ClutchName(VectoSimulationJobType jobType) => $"{_prefix}_{jobType.ToString()}";

		public string CreateElectricMotorControllerName(object[] arguments) => CheckArguments<CycleType>(arguments, ElectricMotorControllerName);

		public string ElectricMotorControllerName(CycleType cycle) => $"{_prefix}_{cycle}";

        // -----

        protected string CheckArguments<T1>(object[] arguments, Func<T1, string> func, [CallerMemberName] string callerName = "")
		{
			if (arguments.Length == 1 && arguments[0] is T1 p1) {
				return func(p1);
			}
			throw new ArgumentException($"exactly one argument expected for {callerName}: {typeof(T1).Name}");
		}

		protected string CheckArguments<T1, T2>(object[] arguments, Func<T1, T2, string> func, [CallerMemberName] string callerName = "")
		{
			if (arguments.Length == 2 && arguments[0] is T1 p1 && arguments[1] is T2 p2) {
				return func(p1, p2);
			}
			throw new ArgumentException($"exactly two arguments expected for {callerName}: {typeof(T1).Name}, {typeof(T2).Name}");
		}

		protected string CheckArguments<T1, T2, T3>(object[] arguments, Func<T1, T2, T3, string> func, [CallerMemberName] string callerName = "")
		{
			if (arguments.Length == 3 && arguments[0] is T1 p1 && arguments[1] is T2 p2 && arguments[2] is T3 p3) {
				return func(p1, p2, p3);
			}
			throw new ArgumentException($"exactly three arguments expected for {callerName}: {typeof(T1).Name}, {typeof(T2).Name}, {typeof(T3).Name}");
		}

	}

}