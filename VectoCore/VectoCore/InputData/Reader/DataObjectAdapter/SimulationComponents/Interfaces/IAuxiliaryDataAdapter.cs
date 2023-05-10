using System.Collections.Generic;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.BusAuxiliaries;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;

namespace TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents.Interfaces
{
    public interface IAuxiliaryDataAdapter
    {
        IList<VectoRunData.AuxData> CreateAuxiliaryData(IAuxiliariesDeclarationInputData auxInputData,
            IBusAuxiliariesDeclarationData busAuxData, MissionType mission, VehicleClass hvdClass, Meter vehicleLength,
            int? numSteeredAxles, VectoSimulationJobType jobType);
    }
	public interface ICompletedBusAuxiliaryDataAdapter : IPrimaryBusAuxiliaryDataAdapter
	{
		IAuxiliaryConfig CreateBusAuxiliariesData(Mission mission, IVehicleDeclarationInputData primaryVehicle,
			IVehicleDeclarationInputData completedVehicle, VectoRunData runData);
	}

	public interface IPrimaryBusAuxiliaryDataAdapter : IAuxiliaryDataAdapter
	{
		AuxiliaryConfig CreateBusAuxiliariesData(Mission mission, IVehicleDeclarationInputData primaryVehicle, VectoRunData runData);

	}

}