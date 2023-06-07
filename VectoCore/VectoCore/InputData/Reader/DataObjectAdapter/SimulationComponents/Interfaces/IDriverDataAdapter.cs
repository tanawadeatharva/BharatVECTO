using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.SimulationComponent.Data;

namespace TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents.Interfaces
{
    public interface IDriverDataAdapter
    {
        DriverData CreateDriverData(Segment segment);
	}

	public interface IDriverDataAdapterBus
	{
		DriverData CreateBusDriverData(Segment segment, VectoSimulationJobType jobType, ArchitectureID arch, CompressorDrive compressorDrive);
	}
}