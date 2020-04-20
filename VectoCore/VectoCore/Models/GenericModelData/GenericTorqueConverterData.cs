using System;
using System.Data;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.Declaration
{
	public class GenericTorqueConverterData
	{
		public TorqueConverterData CreateTorqueConverterData(double ratio, CombustionEngineData engineData)
		{
			var genericTCData = CreateGenericTorqueConverterCharacteristics(ratio, engineData.FullLoadCurves[0].RatedSpeed, engineData.FullLoadCurves[0].MaxTorque);

			return TorqueConverterDataReader.Create(
				genericTCData,
				DeclarationData.TorqueConverter.ReferenceRPM, DeclarationData.TorqueConverter.MaxInputSpeed,
				ExecutionMode.Declaration, ratio,
				DeclarationData.TorqueConverter.CLUpshiftMinAcceleration,
				DeclarationData.TorqueConverter.CCUpshiftMinAcceleration);
		}

		private DataTable CreateGenericTorqueConverterCharacteristics(double ratio, PerSecond ratedSpeed, NewtonMeter maxTorque)
		{
			var engRatedSpeed = ratedSpeed.AsRPM;
			var engMaxTorque = maxTorque.Value();

			const double speedAtStallPtFactor = 0.7;
			const double torqueAtStallPtFactor = 0.8;

			const double refTorqueIntermediatePtFactor = 0.8;
			const double refTorqueCouplingPtFactor = 0.5;
			const double refTorqueOverrunPtFactor = 0;


			const double speedRAtioOverrunPt = 1.0;
			const double speedRatioCouplingPt = 0.9;
			const double speedRatioIntermediatePt = 0.6;
			const double speedRatioStallPt = 0.0;

			const double torqueRatioStallPt = 1.8;
			const double torqueRatioCouplingPt = 0.95;
			const double torqueRatioOverrunPt = 0.94;

			const double tcRefSpeed = 1000;

			var retVal = new DataTable();
			var colSpeedRatio = retVal.Columns.Add(TorqueConverterDataReader.Fields.SpeedRatio);
			var colTqRatio = retVal.Columns.Add(TorqueConverterDataReader.Fields.TorqueRatio);
			var colRefTq = retVal.Columns.Add(TorqueConverterDataReader.Fields.CharacteristicTorque);

			

			var refTorqueStallPt = torqueAtStallPtFactor * engMaxTorque *
									Math.Pow(tcRefSpeed / (speedAtStallPtFactor * engRatedSpeed), 2);

			var torqueRatioIntermediatePt = VectoMath.Interpolate(
				speedRatioStallPt, speedRatioCouplingPt, torqueRatioStallPt, torqueRatioCouplingPt, speedRatioIntermediatePt);

			// stall point
			var stallPt = retVal.NewRow();
			stallPt[colSpeedRatio] = Math.Round(speedRatioStallPt * ratio, 4, MidpointRounding.AwayFromZero);
			stallPt[colTqRatio] = Math.Round(torqueRatioStallPt / speedRAtioOverrunPt / ratio, 2, MidpointRounding.AwayFromZero);
			stallPt[colRefTq] = Math.Round(refTorqueStallPt;
			retVal.Rows.Add(stallPt);

			// intermediate point
			var intermediatePt = retVal.NewRow();
			intermediatePt[colSpeedRatio] = Math.Round(speedRatioIntermediatePt * ratio, 4, MidpointRounding.AwayFromZero);
			intermediatePt[colTqRatio] = Math.Round(torqueRatioIntermediatePt / ratio, 2, MidpointRounding.AwayFromZero);
			intermediatePt[colRefTq] = Math.Round(refTorqueStallPt * refTorqueIntermediatePtFactor, 2, MidpointRounding.AwayFromZero);
			retVal.Rows.Add(intermediatePt);

			//coupling point
			var couplingPt = retVal.NewRow();
			couplingPt[colSpeedRatio] = Math.Round(speedRatioCouplingPt * speedRAtioOverrunPt * ratio, 4, MidpointRounding.AwayFromZero);
			couplingPt[colTqRatio] = Math.Round(torqueRatioCouplingPt / speedRAtioOverrunPt / ratio, 2, MidpointRounding.AwayFromZero);
			couplingPt[colRefTq] = Math.Round(refTorqueStallPt * refTorqueCouplingPtFactor, 2, MidpointRounding.AwayFromZero);
			retVal.Rows.Add(couplingPt);

			// overrun point
			var overrunPt = retVal.NewRow();
			overrunPt[colSpeedRatio] = Math.Round(speedRAtioOverrunPt * ratio, 4, MidpointRounding.AwayFromZero);
			overrunPt[colTqRatio] = Math.Round(torqueRatioOverrunPt / speedRAtioOverrunPt / ratio, 2, MidpointRounding.AwayFromZero);
			overrunPt[colRefTq] = Math.Round(refTorqueStallPt * refTorqueOverrunPtFactor, 2, MidpointRounding.AwayFromZero);
			retVal.Rows.Add(overrunPt);

			return retVal;
		}
	}
}
