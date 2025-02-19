using System;
using System.Reflection;
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
using TUGraz.VectoCore.Models.SimulationComponent.Data.ElectricComponents.Battery;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Impl.Gearbox;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.Common;
using TUGraz.VectoCore.Utils.Ninject;

namespace TUGraz.VectoCore.Models.Simulation
{

	public class PowertrainComponentNinjectModule : VectoNinjectModule
	{
		
		private ComponentBindingNameHelper _realPowertrain = new ComponentBindingNameHelper("RealPowertrain");
		private ComponentBindingNameHelper _testPowertrain = new ComponentBindingNameHelper("TestPowertrain");

        public override void Load()
		{
			#region Setup Factories for powertrain components

			Bind<IPowertrainComponentFactory>().ToFactory(() => GetMethodSettings(_realPowertrain))
				.WhenInjectedExactlyInto<PowertrainBuilder>().InSingletonScope();

			Bind<IPowertrainComponentFactory>().ToFactory(() => GetMethodSettings(_testPowertrain))
				.WhenInjectedExactlyInto<SimplePowertrainBuilder>().InSingletonScope();

			#endregion

			#region Real Powertrain

			Bind<IVehicleContainer>().To<VehicleContainer>().Named(_realPowertrain.Prefix);
			Bind<IExemptedVehicleContainer>().To<ExemptedVehicleContainer>().Named(_realPowertrain.Prefix);
			Bind<ISimpleVehicleContainer>().To<SimplePowertrainContainer>().Named(_realPowertrain.Prefix);

            Bind<IVehicle>().To<Vehicle>().Named(_realPowertrain.Prefix);
			Bind<IWheels>().To<Wheels>().Named(_realPowertrain.Prefix);
			Bind<IDrivingCycle>().To<DistanceBasedDrivingCycle>().Named(_realPowertrain.Prefix);
			Bind<IDriverDemandInProvider>().To<MeasuredSpeedDrivingCycle>().Named(_realPowertrain.Prefix);
			Bind<IDriver>().To<Driver>().Named(_realPowertrain.Prefix);
			Bind<IDriverStrategy>().To<DefaultDriverStrategy>().Named(_realPowertrain.Prefix);
			Bind<IBrakes>().To<Brakes>().Named(_realPowertrain.Prefix);
			Bind<IAxlegear>().To<AxleGear>().Named(_realPowertrain.Prefix);
			Bind<IAngledrive>().To<Angledrive>().Named(_realPowertrain.Prefix);
			Bind<IRetarder>().To<Retarder>().Named(_realPowertrain.Prefix);
			Bind<IClutch>().To<Clutch>().Named(_realPowertrain.Prefix);
			Bind<IClutchInfo>().To<ATClutchInfo>().Named(_realPowertrain.Prefix);
			Bind<ICombustionEngine>().To<StopStartCombustionEngine>().Named(_realPowertrain.Prefix);
			Bind<IWHRCharger>().To<WHRCharger>().Named(_realPowertrain.Prefix);
			Bind<IDCDCConverter>().To<DCDCConverter>().Named(_realPowertrain.Prefix);
			Bind<IBusAuxiliariesAdapter>().To<BusAuxiliariesAdapter>().Named(_realPowertrain.Prefix);
			Bind<IElectricSystem>().To<ElectricSystem>().Named(_realPowertrain.Prefix);
			Bind<IElectricChargerPort>().To<GensetChargerAdapter>().Named(_realPowertrain.Prefix);
			Bind<IElectricMotor>().To<ElectricMotor>().Named(_realPowertrain.ElectricMotorName(false));
			Bind<IElectricMotor>().To<IEPC>().Named(_realPowertrain.ElectricMotorName(true));
			Bind<ISimpleBattery>().To<SimpleBattery>().Named(_realPowertrain.SimpleBatteryName(true));
			Bind<ISimpleBattery>().To<NoBattery>().Named(_realPowertrain.SimpleBatteryName(false));
			Bind<IBusAuxiliariesAdapter>().To<BusAuxiliariesAdapter>().Named(_realPowertrain.ElectricMotorName(true));
			Bind<IElectricEnergyStorage>().To<SuperCap>().Named(_realPowertrain.REESSName(REESSType.SuperCap));
			Bind<IElectricEnergyStorage>().To<BatterySystem>().Named(_realPowertrain.REESSName(REESSType.Battery));

			Bind<IEngineInfo>().To<DummyEngineInfo>().Named(_realPowertrain.Prefix);
			Bind<IAxlegearInfo>().To<DummyAxleGearInfo>().Named(_realPowertrain.Prefix);
			Bind<IGearboxInfo>().To<DummyGearboxInfo>().Named(_realPowertrain.GearboxInfoName(false));
			Bind<IGearboxInfo>().To<EngineOnlyGearboxInfo>().Named(_realPowertrain.GearboxInfoName(true));
			Bind<IMileageCounter>().To<ZeroMileageCounter>().Named(_realPowertrain.Prefix);
			Bind<IDriverInfo>().To<DummyDriverInfo>().Named(_realPowertrain.Prefix);


            // MQ: 20250219 - original code for creating gearbox
            //switch (container.RunData.GearboxData.Type) {
            //	case GearboxType.AMT:
            //		return isMeasuredSpeedHybrid
            //			? ComponentFactory.CreateMeasuredSpeedHybridsGearbox(container, strategy)
            //			: ComponentFactory.CreateAMTGearbox(container, strategy);
            //	case GearboxType.MT:
            //		return isMeasuredSpeedHybrid
            //			? ComponentFactory.CreateMeasuredSpeedHybridsGearbox(container, strategy)
            //			: ComponentFactory.CreateMTGearbox(container, strategy);
            //	case GearboxType.ATPowerSplit:
            //	case GearboxType.ATSerial:
            //		ComponentFactory.CreateATClutchInfo(container);
            //		return ComponentFactory.CreateAPTGearbox(container, strategy);
            //	case GearboxType.APTN:
            //		return ComponentFactory.CreateAPTNGearbox(container, strategy);
            //	case GearboxType.IHPC:
            //		return ComponentFactory.CreateAPTNGearbox(container, strategy);
            //	default:
            //		throw new ArgumentOutOfRangeException("Unknown Gearbox Type",
            //			container.RunData.GearboxData.Type.ToString());
            //}

            Bind<IGearbox>().To<AMTGearbox>().Named(_realPowertrain.GearboxName(GearboxType.AMT, false));
			Bind<IGearbox>().To<MTGearbox>().Named(_realPowertrain.GearboxName(GearboxType.MT, false));
			Bind<IGearbox>().To<APTGearbox>().Named(_realPowertrain.GearboxName(GearboxType.ATPowerSplit, false));
			Bind<IGearbox>().To<APTGearbox>().Named(_realPowertrain.GearboxName(GearboxType.ATSerial, false));
			Bind<IGearbox>().To<APTNGearbox>().Named(_realPowertrain.GearboxName(GearboxType.APTN, false));
			Bind<IGearbox>().To<APTNGearbox>().Named(_realPowertrain.GearboxName(GearboxType.IHPC, false));

            Bind<IGearbox>().To<MeasuredSpeedHybridsGearbox>().Named(_realPowertrain.GearboxName(GearboxType.AMT, true));
			Bind<IGearbox>().To<MeasuredSpeedHybridsGearbox>().Named(_realPowertrain.GearboxName(GearboxType.MT, true));

            #endregion

            #region Test Powertrain

            Bind<IVehicleContainer>().To<VehicleContainer>().Named(_testPowertrain.Prefix);
			Bind<IExemptedVehicleContainer>().To<ExemptedVehicleContainer>().Named(_testPowertrain.Prefix);
			Bind<ISimpleVehicleContainer>().To<SimplePowertrainContainer>().Named(_testPowertrain.Prefix);

			Bind<IVehicle>().To<TestPowertrainVehicle>().Named(_testPowertrain.Prefix);
			Bind<ICombustionEngine>().To<TestpowertrainCombustionEngine>().Named(_testPowertrain.Prefix);
			Bind<IElectricSystem>().To<TestpowertrainElectricSystem>().Named(_testPowertrain.Prefix);
			Bind<IElectricChargerPort>().To<TestpowertrainGensetChargerAdapter>().Named(_testPowertrain.Prefix);
			Bind<IElectricMotor>().To<TestpowertrainElectricMotor>().Named(_testPowertrain.ElectricMotorName(false));
			Bind<IElectricMotor>().To<TestpowertrainIEPC>().Named(_testPowertrain.ElectricMotorName(true));

            Bind<IWheels>().To<Wheels>().Named(_testPowertrain.Prefix);
			Bind<IDrivingCycle>().To<DistanceBasedDrivingCycle>().Named(_testPowertrain.Prefix);
			Bind<IDriverDemandInProvider>().To<MeasuredSpeedDrivingCycle>().Named(_testPowertrain.Prefix);
			Bind<IDriver>().To<Driver>().Named(_testPowertrain.Prefix);
			Bind<IDriverStrategy>().To<DefaultDriverStrategy>().Named(_testPowertrain.Prefix);
			Bind<IBrakes>().To<Brakes>().Named(_testPowertrain.Prefix);
			Bind<IAxlegear>().To<AxleGear>().Named(_testPowertrain.Prefix);
			Bind<IAngledrive>().To<Angledrive>().Named(_testPowertrain.Prefix);
			Bind<IRetarder>().To<Retarder>().Named(_testPowertrain.Prefix);
			Bind<IClutch>().To<Clutch>().Named(_testPowertrain.Prefix);
			Bind<IClutchInfo>().To<ATClutchInfo>().Named(_testPowertrain.Prefix);
            Bind<IWHRCharger>().To<WHRCharger>().Named(_testPowertrain.Prefix);
			Bind<IDCDCConverter>().To<DCDCConverter>().Named(_testPowertrain.Prefix);
			Bind<IBusAuxiliariesAdapter>().To<BusAuxiliariesAdapter>().Named(_testPowertrain.Prefix);
			Bind<ISimpleBattery>().To<SimpleBattery>().Named(_testPowertrain.SimpleBatteryName(true));
			Bind<ISimpleBattery>().To<NoBattery>().Named(_testPowertrain.SimpleBatteryName(false));
			Bind<IElectricEnergyStorage>().To<SuperCap>().Named(_testPowertrain.REESSName(REESSType.SuperCap));
			Bind<IElectricEnergyStorage>().To<BatterySystem>().Named(_testPowertrain.REESSName(REESSType.Battery));

			Bind<IEngineInfo>().To<DummyEngineInfo>().Named(_testPowertrain.Prefix);
			Bind<IAxlegearInfo>().To<DummyAxleGearInfo>().Named(_testPowertrain.Prefix);
			Bind<IGearboxInfo>().To<DummyGearboxInfo>().Named(_testPowertrain.GearboxInfoName(false));
			Bind<IGearboxInfo>().To<EngineOnlyGearboxInfo>().Named(_testPowertrain.GearboxInfoName(true));
			Bind<IMileageCounter>().To<ZeroMileageCounter>().Named(_testPowertrain.Prefix);
			Bind<IDriverInfo>().To<DummyDriverInfo>().Named(_testPowertrain.Prefix);
            #endregion

        }

		private IInstanceProvider GetMethodSettings(ComponentBindingNameHelper namingHelper)
		{
			return new CombineArgumentsToNameInstanceProvider(
				// method settings for creating name bindings for gearbox
				new CombineArgumentsToNameInstanceProvider.MethodSettings() {
					combineToNameDelegate = namingHelper.CreateGearboxName,
					skipArguments = 2,
					takeArguments = 2,
					methods = new[] {
						typeof(IPowertrainComponentFactory).GetMethod(nameof(IPowertrainComponentFactory
							.CreateGearbox)),
					}
				},
				// method settings for creating name bindings for gearboxInfo
				new CombineArgumentsToNameInstanceProvider.MethodSettings() {
					combineToNameDelegate = namingHelper.CreateGearboxInfoName,
					skipArguments = 1,
					takeArguments = 1,
					methods = new[] {
						typeof(IPowertrainComponentFactory).GetMethod(nameof(IPowertrainComponentFactory
							.CreateDummyGearboxInfo)),
					}
				},
                // method settings for creating electric motor/IEPC
                new CombineArgumentsToNameInstanceProvider.MethodSettings() {
					combineToNameDelegate = namingHelper.CreateElectricMotorName,
					skipArguments = 1,
					takeArguments = 1,
					methods = new[] {
						typeof(IPowertrainComponentFactory).GetMethod(nameof(IPowertrainComponentFactory
							.CreateElectricMotor)),
					}
				},
				// method settings for creating SimpleBattery (bus aux)
				new CombineArgumentsToNameInstanceProvider.MethodSettings() {
					combineToNameDelegate = namingHelper.CreateSimpleBatteryName,
					skipArguments = 1,
					takeArguments = 1,
					methods = new[] {
						typeof(IPowertrainComponentFactory).GetMethod(nameof(IPowertrainComponentFactory
								.CreateSimpleBattery)),
					}
				},
				// method settings for creating REESS (battery/supercap)
				new CombineArgumentsToNameInstanceProvider.MethodSettings() {
					combineToNameDelegate = namingHelper.CreateREESSName,
					skipArguments = 1,
					takeArguments = 1,
					methods = new[] {
						typeof(IPowertrainComponentFactory).GetMethod(nameof(IPowertrainComponentFactory
								.CreateREESS),
							new Type[] { typeof(REESSType), typeof(IVehicleContainer), typeof(SuperCapData) }),
						typeof(IPowertrainComponentFactory).GetMethod(nameof(IPowertrainComponentFactory
								.CreateREESS),
							new Type[] { typeof(REESSType), typeof(IVehicleContainer), typeof(BatterySystemData) }),
					}
				},
                // fallback name binding for all other methods
                new CombineArgumentsToNameInstanceProvider.MethodSettings() {
					combineToNameDelegate = namingHelper.CreateComponentName,
					skipArguments = 0,
					takeArguments = 0,
					methods = null
				});
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

		public string GearboxName(GearboxType gbxType, bool measuredSpeed)
		{
			return _prefix + gbxType.ToString() + (measuredSpeed ? "_MS" : "");
        }

		public string CreateGearboxName(object[] arguments)
		{
			if (arguments.Length == 2 && arguments[0] is GearboxType gbxType && arguments[1] is bool measuredSpeed) {
				return GearboxName(gbxType, measuredSpeed);
			}

			throw new ArgumentException($"exactly two arguments expected for CreateGearboxName: {typeof(GearboxType).Name}, bool");
		}

		public string CreateComponentName(object[] arguments)
		{
			return Prefix;
		}

		public string CreateElectricMotorName(object[] arguments)
		{
			if (arguments.Length == 1 && arguments[0] is bool isIEPC) {
				return ElectricMotorName(isIEPC);
			}
			throw new ArgumentException($"exactly one argument expected for CreateElectricMotorName: bool");

        }

		public string ElectricMotorName(bool isIepc)
		{
			return _prefix + (isIepc ? "_IEPC" : "_EM");
		}

		public string CreateREESSName(object[] arguments)
		{
			if (arguments.Length == 1 && arguments[0] is REESSType reessType) {
				return REESSName(reessType);
			}
			throw new ArgumentException($"exactly one argument expected for CreateElectricMotorName: {typeof(REESSType).Name}");
        }

		public string REESSName(REESSType reessType)
		{
			return _prefix + reessType.ToString();
		}

		public string CreateSimpleBatteryName(object[] arguments)
		{
			if (arguments.Length == 1 && arguments[0] is bool smartAlternator) {
				return SimpleBatteryName(smartAlternator);
			}
			throw new ArgumentException($"exactly one argument expected for CreateElectricMotorName: bool");
        }

		public string SimpleBatteryName(bool smartAlternator)
		{
			return _prefix + (smartAlternator ? "_smart" : "");
		}

		public string CreateGearboxInfoName(object[] arguments)
		{
			if (arguments.Length == 1 && arguments[0] is bool engineOnly) {
				return GearboxInfoName(engineOnly);
			}
			throw new ArgumentException($"exactly one argument expected for CreateGearboxInfoName: bool");
        }

		public string GearboxInfoName(bool engineOnly)
		{
			return _prefix + (engineOnly ? "_EngOnly" : "_Full");
		}
	}

}