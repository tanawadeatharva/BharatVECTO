using System;
using System.Collections.Generic;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.SimulationComponent.Data;

namespace TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents.Interfaces
{
    public interface IVehicleDataAdapter
    {
        VehicleData CreateVehicleData(IVehicleDeclarationInputData data, Segment segment, Mission mission,
            Kilogram loading, double? passengerCount, bool allowVocational);
        
		VehicleData CreateExemptedVehicleData(IVehicleDeclarationInputData data);

        VehicleData CreateVehicleData(IVehicleDeclarationInputData primaryVehicle,
            IVehicleDeclarationInputData completedVehicle, Segment segment, Mission mission,
            KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading);


        VehicleData CreateExemptedVehicleData(IVehicleDeclarationInputData primaryVehicle,
            IVehicleDeclarationInputData completedVehicle);
    }
}