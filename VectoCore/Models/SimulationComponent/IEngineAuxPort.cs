using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent
{
	public interface IEngineAuxInProvider
	{
		IEngineAuxPort Port();
	}

	public interface IEngineAuxOutProvider
	{
		void Connect(IEngineAuxPort aux);
	}

	public interface IEngineAuxPort
	{
		NewtonMeter Initialize(NewtonMeter torque, PerSecond angularSpeed);

		NewtonMeter PowerDemand(Second absTime, Second dt, NewtonMeter torque, PerSecond angularSpeed, bool dryRun = false);
	}
}