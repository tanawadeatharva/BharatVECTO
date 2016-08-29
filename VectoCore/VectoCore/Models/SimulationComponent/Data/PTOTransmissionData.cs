namespace TUGraz.VectoCore.Models.SimulationComponent.Data
{
	public class PTOTransmissionData : SimulationComponentData
	{
		public string TransmissionType;
		public ILossMap LossMap;
		public DrivingCycleData PTOCycle;
	}
}