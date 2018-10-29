using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Connector.Ports;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl {
	public class TorqueConverterWrapper
	{
		protected internal CycleTorqueConverter CycleTorqueConverter;
		protected internal TorqueConverter TorqueConverter;
		protected bool UseCycleTorqueConverter;
		
		public TorqueConverterWrapper(bool useCycle, CycleTorqueConverter cycleTorqueConverter, TorqueConverter torqueConverter)
		{
			UseCycleTorqueConverter = useCycle;
			if (useCycle) {
				CycleTorqueConverter = cycleTorqueConverter;
			} else {
				TorqueConverter = torqueConverter;
			}
		}

		public ITnOutPort NextComponent
		{
			set {
				if (UseCycleTorqueConverter) {
					CycleTorqueConverter.NextComponent = value;
				} else {
					TorqueConverter.NextComponent = value;
				}
			}
		}

		public IResponse Initialize(NewtonMeter outTorque, PerSecond outAngularVelocity, PerSecond inAngularVelocity)
		{
			return UseCycleTorqueConverter
				? CycleTorqueConverter.Initialize(outTorque, outAngularVelocity, inAngularVelocity)
				: TorqueConverter.Initialize(outTorque, outAngularVelocity);
		}

		public IResponse Request(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, PerSecond inAngularVelocity, bool dryRun = false)
		{
			return UseCycleTorqueConverter
				? CycleTorqueConverter.Request(absTime, dt, outTorque, outAngularVelocity, inAngularVelocity, dryRun)
				: TorqueConverter.Request(absTime, dt, outTorque, outAngularVelocity, dryRun);
		}

		public void Locked(NewtonMeter outTorque, PerSecond outAngularVelocity, NewtonMeter inTorque, PerSecond inAngularVelocity)
		{
			if (UseCycleTorqueConverter) {
				CycleTorqueConverter.Locked(outTorque, outAngularVelocity, inTorque, inAngularVelocity);
			} else {
				TorqueConverter.Locked(outTorque, outAngularVelocity, inTorque, inAngularVelocity);
			}

		}
	}
}