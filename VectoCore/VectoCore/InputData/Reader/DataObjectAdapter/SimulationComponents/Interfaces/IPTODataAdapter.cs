using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.Models.SimulationComponent.Data;

namespace TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents.Interfaces
{
    public interface IPTODataAdapter
    {
        PTOData CreatePTOTransmissionData(IPTOTransmissionInputData pto, IGearboxDeclarationInputData gbx);
        PTOData CreateDefaultPTOData(IPTOTransmissionInputData pto, IGearboxDeclarationInputData gbx);
    }
}