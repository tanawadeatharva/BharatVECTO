using TUGraz.VectoCommon.InputData;

namespace TUGraz.VectoCore.InputData.FileIO.JSON {
    public interface IJSONVehicleComponents  {

        IGearboxEngineeringInputData Gearbox { get;  }
        ITorqueConverterEngineeringInputData TorqueConverter { get;  }
        IAxleGearInputData AxleGear { get;  }
        IEngineEngineeringInputData Engine { get;  }
        IAuxiliariesEngineeringInputData EngineeringAuxiliaries { get;  }
        IAuxiliariesDeclarationInputData DeclarationAuxiliaries { get; }
    }
}