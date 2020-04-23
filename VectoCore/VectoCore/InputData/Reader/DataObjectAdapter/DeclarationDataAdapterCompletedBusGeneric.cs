using System;
using System.Collections.Generic;
using System.Linq;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Models.BusAuxiliaries;
using TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.Electrics;
using TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.HVAC;
using TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.Pneumatics;
using TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules.Electrics;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.GenericModelData;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.Reader.DataObjectAdapter
{
	public class DeclarationDataAdapterCompletedBusGeneric : DeclarationDataAdapterPrimaryBus
	{
		private readonly GenericTransmissionComponentData _genericPowertrainData = new GenericTransmissionComponentData();
		private readonly GenericBusRetarderData _genericRetarderData = new GenericBusRetarderData();
		private readonly GenericTorqueConverterData _genericTorqueConverterData = new GenericTorqueConverterData();

		public const double GearEfficiencyDirectGear = 0.98;
		public const double GearEfficiencyIndirectGear = 0.96;


		

		// The model parameters for the completed bus with generic power train and generic body is basically the same as the primary bus
		// only powertrain components are different

		
		public CombustionEngineData CreateEngineData(IVehicleDeclarationInputData primaryVehicle, int modeIdx)
		{
			return GenericBusEngineData.Instance.CreateGenericBusEngineData(primaryVehicle, modeIdx);
		}

		#region Overrides of DeclarationDataAdapterHeavyLorry

		public override AxleGearData CreateAxleGearData(IAxleGearInputData axlegearData)
		{
			return _genericPowertrainData.CreateGenericBusAxlegearData(axlegearData);
		}

		public override AngledriveData CreateAngledriveData(IAngledriveInputData data)
		{
			return _genericPowertrainData.CreateGenericBusAngledriveData(data);
		}


		public override RetarderData CreateRetarderData(IRetarderInputData retarder)
		{
			return _genericRetarderData.CreateGenericBusRetarderData(retarder);
		}

		#endregion

		#region Overrides of AbstractSimulationDataAdapter

		protected override TransmissionLossMap CreateGearLossMap(
			ITransmissionInputData gear, uint i, bool useEfficiencyFallback, VehicleCategory vehicleCategory)
		{
			return TransmissionLossMapReader.Create(
				gear.Ratio.IsEqual(1) ? GearEfficiencyDirectGear :GearEfficiencyIndirectGear, gear.Ratio, $"Gear {i + 1}");
		}

		protected override void CretateTCFirstGearATPowerSplit(GearData gearData, uint i, ShiftPolygon shiftPolygon)
		{
			gearData.TorqueConverterRatio = 1;
			gearData.TorqueConverterGearLossMap = TransmissionLossMapReader.Create(GearEfficiencyIndirectGear, 1, string.Format("TCGear {0}", i + 1));
			gearData.TorqueConverterShiftPolygon = shiftPolygon;
		}

		#endregion

		#region Overrides of DeclarationDataAdapterHeavyLorry

		protected override TorqueConverterData CreateTorqueConverterData(ITorqueConverterDeclarationInputData torqueConverter, double ratio, CombustionEngineData engineData)
		{
			if (torqueConverter != null) {
				return TorqueConverterDataReader.Create(
					torqueConverter.TCData,
					DeclarationData.TorqueConverter.ReferenceRPM, DeclarationData.TorqueConverter.MaxInputSpeed,
					ExecutionMode.Engineering, ratio,
					DeclarationData.TorqueConverter.CLUpshiftMinAcceleration,
					DeclarationData.TorqueConverter.CCUpshiftMinAcceleration);
			}
			return _genericTorqueConverterData.CreateTorqueConverterData(ratio, engineData);
		}

		#endregion

		
	}
}
