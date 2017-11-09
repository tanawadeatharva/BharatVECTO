using System;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
    public class EPTPCombustionEngine : CombustionEngine
    {
        public EPTPCombustionEngine(IVehicleContainer container, CombustionEngineData modelData, bool pt1Disabled = false) : base(container, modelData, pt1Disabled) { }

        //protected override void DoWriteModalResults(IModalDataContainer container)
        //{
        //    ValidatePowerDemand(CurrentState.EngineTorque, CurrentState.DynamicFullLoadTorque,
        //        CurrentState.FullDragTorque);

        //    //var avgEngineSpeed = (PreviousState.EngineSpeed + CurrentState.EngineSpeed) / 2.0;
        //    var avgEngineSpeed = DataBus.CycleData.LeftSample.EngineSpeed;
        //    if (avgEngineSpeed.IsSmaller(EngineIdleSpeed,
        //        DataBus.ExecutionMode == ExecutionMode.Engineering ? 20.RPMtoRad() : 1e-3.RPMtoRad())) {
        //        Log.Warn("EngineSpeed below idling speed! n_eng_avg: {0}, n_idle: {1}", avgEngineSpeed,
        //            EngineIdleSpeed);
        //    }
        //    container[ModalResultField.P_eng_fcmap] = CurrentState.EngineTorque * avgEngineSpeed;
        //    container[ModalResultField.P_eng_out] = container[ModalResultField.P_eng_out] is DBNull
        //        ? CurrentState.EngineTorqueOut * avgEngineSpeed
        //        : container[ModalResultField.P_eng_out];
        //    container[ModalResultField.P_eng_inertia] = CurrentState.InertiaTorqueLoss * avgEngineSpeed;

        //    container[ModalResultField.n_eng_avg] = avgEngineSpeed;
        //    container[ModalResultField.T_eng_fcmap] = CurrentState.EngineTorque;

        //    container[ModalResultField.P_eng_full] = CurrentState.DynamicFullLoadTorque * avgEngineSpeed;
        //    container[ModalResultField.P_eng_full_stat] = CurrentState.StationaryFullLoadTorque * avgEngineSpeed;
        //    container[ModalResultField.P_eng_drag] = CurrentState.FullDragTorque * avgEngineSpeed;
        //    container[ModalResultField.Tq_full] = CurrentState.DynamicFullLoadTorque;
        //    container[ModalResultField.Tq_drag] = CurrentState.FullDragTorque;

        //    var result = ModelData.ConsumptionMap.GetFuelConsumption(CurrentState.EngineTorque, avgEngineSpeed,
        //        DataBus.ExecutionMode != ExecutionMode.Declaration);
        //    if (DataBus.ExecutionMode != ExecutionMode.Declaration && result.Extrapolated) {
        //        Log.Warn("FuelConsumptionMap was extrapolated: range for FC-Map is not sufficient: n: {0}, torque: {1}",
        //            avgEngineSpeed.Value(), CurrentState.EngineTorque.Value());
        //    }
        //    var pt1 = ModelData.FullLoadCurves[DataBus.Gear].PT1(avgEngineSpeed);
        //    if (DataBus.ExecutionMode == ExecutionMode.Declaration && pt1.Extrapolated) {
        //        Log.Error("requested rpm below minimum rpm in pt1 - extrapolating. n_eng_avg: {0}",
        //            avgEngineSpeed);
        //    }

        //    var fc = result.Value;
        //    var fcAux = fc;

        //    var fcWHTC = fcAux * ModelData.FuelConsumptionCorrectionFactor;
        //    var fcAAUX = fcWHTC;
        //    var advancedAux = EngineAux as BusAuxiliariesAdapter;
        //    if (advancedAux != null) {
        //        advancedAux.DoWriteModalResults(container);
        //        fcAAUX = advancedAux.AAuxFuelConsumption;
        //    }
        //    var fcFinal = fcAAUX;

        //    container[ModalResultField.FCMap] = fc;
        //    container[ModalResultField.FCAUXc] = fcAux;
        //    container[ModalResultField.FCWHTCc] = fcWHTC;
        //    container[ModalResultField.FCAAUX] = fcAAUX;
        //    container[ModalResultField.FCFinal] = fcFinal;
        //}

        protected override PerSecond GetEngineSpeed(PerSecond angularSpeed)
        {
            return DataBus.CycleData.LeftSample.EngineSpeed;
        }
    }
}