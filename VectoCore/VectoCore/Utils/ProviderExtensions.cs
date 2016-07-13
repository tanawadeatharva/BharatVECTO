using System;
using System.Collections.Generic;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.Models.Connector.Ports;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;

namespace TUGraz.VectoCore.Utils
{
	public static class ProviderExtensions
	{
		public static void AddAuxiliaries(this CombustionEngine engine, VehicleContainer container,
			VectoRunData data)
		{
			// aux --> engine
			if (data.AdvancedAux != null && data.AdvancedAux.AuxiliaryAssembly == AuxiliaryModel.Advanced) {
				engine.Connect(PowertrainBuilder.CreateAdvancedAuxiliaries(data, container).Port());
			} else {
				if (data.Aux != null) {
					engine.Connect(PowertrainBuilder.CreateAuxiliaries(data, container).Port());
				}
			}
		}

		public static IDriver AddComponent(this IDrivingCycleInProvider prev, IDriver next)
		{
			prev.InPort().Connect(next.OutPort());
			return next;
		}

		public static IVehicle AddComponent(this IDriverDemandInProvider prev, IVehicle next)
		{
			prev.InPort().Connect(next.OutPort());
			return next;
		}

		public static IWheels AddComponent(this IFvInProvider prev, IWheels next)
		{
			prev.InPort().Connect(next.OutPort());
			return next;
		}

		public static IPowerTrainComponent AddComponent(this ITnInProvider prev, IPowerTrainComponent next)
		{
			prev.InPort().Connect(next.OutPort());
			return next;
		}

		public static IPowerTrainComponent AddComponent(this IPowerTrainComponent prev, IPowerTrainComponent next)
		{
			prev.InPort().Connect(next.OutPort());
			return next;
		}

		public static CombustionEngine AddComponent(this IPowerTrainComponent prev, CombustionEngine next)
		{
			prev.InPort().Connect(next.OutPort());

			var clutch = prev as IClutch;
			if (clutch != null)
				next.IdleController.RequestPort = clutch.IdleControlPort;
			return next;
		}

		public static IPowerTrainComponent AddRetarderAndGearbox(this IPowerTrainComponent prev, RetarderData data,
			IGearbox gearbox,
			IVehicleContainer container)
		{
			switch (data.Type) {
				case RetarderType.Primary:
					return prev.AddComponent(new Retarder(container, data.LossMap, data.Ratio)).AddComponent(gearbox);
				case RetarderType.Secondary:
					return prev.AddComponent(gearbox).AddComponent(new Retarder(container, data.LossMap, data.Ratio));
				case RetarderType.None:
					return prev.AddComponent(new DummyRetarder(container)).AddComponent(gearbox);
				case RetarderType.LossesIncludedInTransmission:
					return prev.AddComponent(new DummyRetarder(container)).AddComponent(gearbox);
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}
}