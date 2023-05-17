using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.Models.SimulationComponent.Data;

namespace TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents.Interfaces
{
    public interface IAngledriveDataAdapter
    {
        AngledriveData CreateAngledriveData(IAngledriveInputData data, bool useEfficiencyFallback);
        AngledriveData CreateAngledriveData(IAngledriveInputData data);
    }
}