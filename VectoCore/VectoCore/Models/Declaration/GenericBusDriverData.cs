using System;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Models.SimulationComponent.Data;

namespace TUGraz.VectoCore.Models.Declaration
{
	public class GenericBusDriverData
	{
		public DriverData CreateGenericBusDriverData(Segment completedSegment)
		{
			var lookAheadData = new DriverData.LACData
			{
				Enabled = DeclarationData.Driver.LookAhead.Enabled,
				//Deceleration = DeclarationData.Driver.LookAhead.Deceleration,
				MinSpeed = DeclarationData.Driver.LookAhead.MinimumSpeed,
				LookAheadDecisionFactor = new LACDecisionFactor(),
				LookAheadDistanceFactor = DeclarationData.Driver.LookAhead.LookAheadDistanceFactor
			};

			var overspeedData = new DriverData.OverSpeedData
			{
				Enabled = true,
				MinSpeed = DeclarationData.Driver.OverSpeed.MinSpeed,
				OverSpeed = DeclarationData.Driver.OverSpeed.AllowedOverSpeed,
			};

			var driver = new DriverData
			{
				AccelerationCurve = AccelerationCurveReader.ReadFromStream(completedSegment.AccelerationFile),
				LookAheadCoasting = lookAheadData,
				OverSpeed = overspeedData,
				EngineStopStart = new DriverData.EngineStopStartData
				{
					EngineOffStandStillActivationDelay = DeclarationData.Driver.EngineStopStart.ActivationDelay,
					MaxEngineOffTimespan = DeclarationData.Driver.EngineStopStart.MaxEngineOffTimespan,
					UtilityFactor = DeclarationData.Driver.EngineStopStart.UtilityFactor
				},
				EcoRoll = new DriverData.EcoRollData
				{
					UnderspeedThreshold = DeclarationData.Driver.EcoRoll.UnderspeedThreshold,
					MinSpeed = DeclarationData.Driver.EcoRoll.MinSpeed,
					ActivationPhaseDuration = DeclarationData.Driver.EcoRoll.ActivationDelay,
					AccelerationLowerLimit = DeclarationData.Driver.EcoRoll.AccelerationLowerLimit,
					AccelerationUpperLimit = DeclarationData.Driver.EcoRoll.AccelerationUpperLimit
				},
				PCC = new DriverData.PCCData
				{
					PCCEnableSpeed = DeclarationData.Driver.PCC.PCCEnableSpeed,
					MinSpeed = DeclarationData.Driver.PCC.MinSpeed,
					PreviewDistanceUseCase1 = DeclarationData.Driver.PCC.PreviewDistanceUseCase1,
					PreviewDistanceUseCase2 = DeclarationData.Driver.PCC.PreviewDistanceUseCase2,
					UnderSpeed = DeclarationData.Driver.PCC.Underspeed,
					OverspeedUseCase3 = DeclarationData.Driver.PCC.OverspeedUseCase3
				}
			};

			return driver;
		}
	}
}
