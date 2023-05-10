using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents.Interfaces;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.SimulationComponent.Data;

namespace TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents
{
    internal abstract class DriverDataAdapter : IDriverDataAdapter
	{
		protected DriverDataAdapter() { }

		#region Implementation of IDriverDataAdapter

		

		public virtual DriverData CreateDriverData(Segment segment)
		{
            var lookAheadData = new DriverData.LACData
            {
                Enabled = DeclarationData.Driver.LookAhead.Enabled,

                //Deceleration = DeclarationData.Driver.LookAhead.Deceleration,
                MinSpeed = DeclarationData.Driver.LookAhead.MinimumSpeed,
                LookAheadDecisionFactor = new LACDecisionFactor(),
                LookAheadDistanceFactor = DeclarationData.Driver.LookAhead.LookAheadDistanceFactor,
            };
            var overspeedData = new DriverData.OverSpeedData
            {
                Enabled = true,
                MinSpeed = DeclarationData.Driver.OverSpeed.MinSpeed,
                OverSpeed = DeclarationData.Driver.OverSpeed.AllowedOverSpeed,
            };

            var retVal = new DriverData
            {
                LookAheadCoasting = lookAheadData,
                OverSpeed = overspeedData,
                EngineStopStart = new DriverData.EngineStopStartData()
                {
                    EngineOffStandStillActivationDelay = DeclarationData.Driver.EngineStopStart.ActivationDelay,
                    MaxEngineOffTimespan = DeclarationData.Driver.EngineStopStart.MaxEngineOffTimespan,
                    UtilityFactorStandstill = DeclarationData.Driver.EngineStopStart.UtilityFactor,
                    UtilityFactorDriving = DeclarationData.Driver.EngineStopStart.UtilityFactor,
                },
                EcoRoll = new DriverData.EcoRollData()
                {
                    UnderspeedThreshold = DeclarationData.Driver.EcoRoll.UnderspeedThreshold,
                    MinSpeed = DeclarationData.Driver.EcoRoll.MinSpeed,
                    ActivationPhaseDuration = DeclarationData.Driver.EcoRoll.ActivationDelay,
                    AccelerationLowerLimit = DeclarationData.Driver.EcoRoll.AccelerationLowerLimit,
                    AccelerationUpperLimit = DeclarationData.Driver.EcoRoll.AccelerationUpperLimit,
                },
                PCC = new DriverData.PCCData()
                {
                    PCCEnableSpeed = DeclarationData.Driver.PCC.PCCEnableSpeed,
                    MinSpeed = DeclarationData.Driver.PCC.MinSpeed,
                    PreviewDistanceUseCase1 = DeclarationData.Driver.PCC.PreviewDistanceUseCase1,
                    PreviewDistanceUseCase2 = DeclarationData.Driver.PCC.PreviewDistanceUseCase2,
                    UnderSpeed = DeclarationData.Driver.PCC.Underspeed,
                    OverspeedUseCase3 = DeclarationData.Driver.PCC.OverspeedUseCase3
                }
            };

            retVal.AccelerationCurve = AccelerationCurveReader.ReadFromStream(segment.AccelerationFile);
            return retVal;
        }
        #endregion

    }

	internal sealed class LorryDriverDataAdapter : DriverDataAdapter
	{

		#region Overrides of DriverDataAdapter

		public override DriverData CreateDriverData(Segment segment)
		{
			return base.CreateDriverData(segment:segment);
		}

		#endregion
	}

    internal sealed class PrimaryBusDriverDataAdapter : DriverDataAdapter
	{
		#region Overrides of DriverDataAdapter

		public override DriverData CreateDriverData(Segment segment)
		{
			var retVal = base.CreateDriverData(segment);
			retVal.LookAheadCoasting.Enabled = true;
			return retVal;
        }

		#endregion
	}

	internal sealed class CompletedBusGenericDriverDataAdapter : DriverDataAdapter
	{
		public override DriverData CreateDriverData(Segment segment)
		{
			var retVal = base.CreateDriverData(segment);
			retVal.LookAheadCoasting.Enabled = false;
			retVal.OverSpeed.Enabled = false;
			return retVal;
		}
    }

	internal sealed class CompletedBusSpecificDriverDataAdapter : DriverDataAdapter
	{
		public override DriverData CreateDriverData(Segment segment)
		{
			var retVal = base.CreateDriverData(segment);
			retVal.LookAheadCoasting.Enabled = false;
			retVal.OverSpeed.Enabled = false;
			return retVal;
        }
	}
}

