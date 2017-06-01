using System.Collections.Generic;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.InputData.Reader;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Engine;

namespace TUGraz.IVT.VectoXML
{
	static internal class AttributeMappings
	{
		public static readonly Dictionary<string, string> FuelConsumptionMapMapping = new Dictionary<string, string> {
			{ FuelConsumptionMapReader.Fields.EngineSpeed, XMLNames.Engine_FuelConsumptionMap_EngineSpeed_Attr },
			{ FuelConsumptionMapReader.Fields.Torque, XMLNames.Engine_FuelConsumptionMap_Torque_Attr },
			{ FuelConsumptionMapReader.Fields.FuelConsumption, XMLNames.Engine_FuelConsumptionMap_FuelConsumption_Attr }
		};

		public static readonly Dictionary<string, string> EngineFullLoadCurveMapping = new Dictionary<string, string> {
			{ FullLoadCurveReader.Fields.EngineSpeed, XMLNames.Engine_EngineFullLoadCurve_EngineSpeed_Attr },
			{ FullLoadCurveReader.Fields.TorqueFullLoad, XMLNames.Engine_FullLoadCurve_MaxTorque_Attr },
			{ FullLoadCurveReader.Fields.TorqueDrag, XMLNames.Engine_FullLoadCurve_DragTorque_Attr },
			{ "PT1", "PT1" }
		};

		public static readonly Dictionary<string, string> ShiftPolygonMapping = new Dictionary<string, string> {
			{ ShiftPolygonReader.Fields.Torque, XMLNames.Gear_ShiftPolygon_EngineTorque_Attr },
			{ ShiftPolygonReader.Fields.AngularSpeedDown, XMLNames.Gear_ShiftPolygonMapping_DownshiftSpeed_Attr },
			{ ShiftPolygonReader.Fields.AngularSpeedUp, XMLNames.Gear_ShiftPolygonMapping_UpshiftSpeed_Attr }
		};

		public static readonly Dictionary<string, string> AuxMapMapping = new Dictionary<string, string>() {
			{ AuxiliaryDataReader.Fields.AuxSpeed, XMLNames.Aux_AuxMap_AuxiliarySpeed_Attr },
			{ AuxiliaryDataReader.Fields.MechPower, XMLNames.Aux_AuxMap_MechanicalPower_Attr },
			{ AuxiliaryDataReader.Fields.SupplyPower, XMLNames.Auxr_AuxMapMapping_SupplyPower_Attr }
		};

		public static readonly Dictionary<string, string> RetarderLossmapMapping = new Dictionary<string, string> {
			{ RetarderLossMapReader.Fields.RetarderSpeed, XMLNames.Retarder_RetarderLossmap_RetarderSpeed_Attr },
			{ RetarderLossMapReader.Fields.TorqueLoss, XMLNames.Retarder_RetarderLossmap_TorqueLoss_Attr }
		};

		public static readonly Dictionary<string, string> TorqueConverterDataMapping = new Dictionary<string, string>() {
			{ TorqueConverterDataReader.Fields.SpeedRatio, XMLNames.TorqueConverterData_SpeedRatio_Attr },
			{ TorqueConverterDataReader.Fields.TorqueRatio, XMLNames.TorqueConverterData_TorqueRatio_Attr },
			{ TorqueConverterDataReader.Fields.CharacteristicTorque, XMLNames.TorqueConverterDataMapping_InputTorqueRef_Attr }
		};

		public static readonly Dictionary<string, string> TransmissionLossmapMapping = new Dictionary<string, string>() {
			{ TransmissionLossMapReader.Fields.InputSpeed, XMLNames.TransmissionLossmap_InputSpeed_Attr },
			{ TransmissionLossMapReader.Fields.InputTorque, XMLNames.TransmissionLossmap_InputTorque_Attr },
			{ TransmissionLossMapReader.Fields.TorqeLoss, XMLNames.TransmissionLossmap_TorqueLoss_Attr }
		};

		public static readonly Dictionary<string, string> DriverAccelerationCurveMapping = new Dictionary<string, string>() {
			{ AccelerationCurveReader.Fields.Velocity, XMLNames.Vehicle_CrosswindCorrectionMap_VehicleSpeed_Attr },
			{ AccelerationCurveReader.Fields.Acceleration, XMLNames.Vehicle_AccelerationCurve_MaxAcceleration_Attr },
			{ AccelerationCurveReader.Fields.Deceleration, XMLNames.Vehicle_AccelerationCurve_MaxDeceleration_Attr }
		};

		public static readonly Dictionary<string, string> CoastingDFTargetSpeedLookupMapping =
			new Dictionary<string, string>() {
				{
					LACDecisionFactor.LACDecisionFactorVTarget.Fields.TargetVelocity,
					XMLNames.Driver_CoastingDFTargetSpeedLookupMapping_TargetVelocity_Attr
				}, {
					LACDecisionFactor.LACDecisionFactorVTarget.Fields.DecisionFactor,
					XMLNames.Driver_CoastingDFTargetSpeedLookupMapping_DecisionFactor_Attr
				}
			};

		public static readonly Dictionary<string, string> CoastingDFVelocityDropLookupMapping =
			new Dictionary<string, string>() {
				{
					LACDecisionFactor.LACDecisionFactorVdrop.Fields.VelocityDrop,
					XMLNames.Driver_CoastingDFVelocityDropLookupMapping_VelocityDrop_Attr
				}, {
					LACDecisionFactor.LACDecisionFactorVdrop.Fields.DecisionFactor,
					XMLNames.Driver_CoastingDFVelocityDropLookupMapping_DecisionFactorDrop_Attr
				}
			};

		public static readonly Dictionary<string, string> CrossWindCorrectionMapping = new Dictionary<string, string>() {
			{
				CrossWindCorrectionCurveReader.FieldsSpeedDependent.Velocity,
				XMLNames.Vehicle_CrosswindCorrectionMap_VehicleSpeed_Attr
			},
			{ CrossWindCorrectionCurveReader.FieldsSpeedDependent.Cd, XMLNames.Vehicle_CrosswindCorrectionMap_CdScalingFactor },
			{ CrossWindCorrectionCurveReader.FieldsCdxABeta.Beta, XMLNames.Vehicle_CrosswindCorrectionMap_Beta },
			{ CrossWindCorrectionCurveReader.FieldsCdxABeta.DeltaCdxA, XMLNames.Vehicle_CrosswindCorrectionMap_DeltaCdxA },
		};

		public static readonly Dictionary<string, string> PTOLossMap = new Dictionary<string, string> {
			{ PTOIdleLossMapReader.Fields.EngineSpeed, XMLNames.Vehicle_PTOIdleLossMap_EngineSpeed_Attr },
			{ PTOIdleLossMapReader.Fields.PTOTorque, XMLNames.Vehicle_PTOIdleLossMap_TorqueLoss_Attr },
		};

		public static readonly Dictionary<string, string> PTOCycleMap = new Dictionary<string, string>() {
			{ DrivingCycleDataReader.Fields.Time, XMLNames.Vehicle_PTOCycle_Time_Attr },
			{ DrivingCycleDataReader.Fields.EngineSpeed, XMLNames.Vehicle_PTOCycle_EngineSpeed_Attr },
			{ DrivingCycleDataReader.Fields.PTOTorque, XMLNames.Vehicle_PTOCycle_Torque_Attr },
		};
	}
}