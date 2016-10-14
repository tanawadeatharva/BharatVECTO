namespace TUGraz.VectoCore.Models.SimulationComponent.Data
{
	public class PTOData : SimulationComponentData
	{
		public string TransmissionType;
		public ILossMap LossMap;
		public DrivingCycleData PTOCycle;
	}
}