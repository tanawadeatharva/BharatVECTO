using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents.Interfaces;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents
{
    public static class AirdragDataAdapterHelper
	{
		public static List<CrossWindCorrectionCurveReader.CrossWindCorrectionEntry> GetDeclarationAirResistanceCurve(
			string crosswindCorrectionParameters, SquareMeter aerodynamicDragAera, Meter vehicleHeight)
		{
			var startSpeed = Constants.SimulationSettings.CrosswindCorrection.MinVehicleSpeed;
			var maxSpeed = Constants.SimulationSettings.CrosswindCorrection.MaxVehicleSpeed;
			var speedStep = Constants.SimulationSettings.CrosswindCorrection.VehicleSpeedStep;

			var maxAlpha = Constants.SimulationSettings.CrosswindCorrection.MaxAlpha;
			var alphaStep = Constants.SimulationSettings.CrosswindCorrection.AlphaStep;

			var startHeightPercent = Constants.SimulationSettings.CrosswindCorrection.MinHeight;
			var maxHeightPercent = Constants.SimulationSettings.CrosswindCorrection.MaxHeight;
			var heightPercentStep = Constants.SimulationSettings.CrosswindCorrection.HeightStep;
			var heightShare = (double)heightPercentStep / maxHeightPercent;

			var values = DeclarationData.AirDrag.Lookup(crosswindCorrectionParameters);

			// first entry (0m/s) will get CdxA of second entry.
			var points = new List<CrossWindCorrectionCurveReader.CrossWindCorrectionEntry> {
					new CrossWindCorrectionCurveReader.CrossWindCorrectionEntry {
						Velocity = 0.SI<MeterPerSecond>(),
						EffectiveCrossSectionArea = 0.SI<SquareMeter>()
					}
				};

			for (var speed = startSpeed; speed <= maxSpeed; speed += speedStep)
			{
				var vVeh = speed.KMPHtoMeterPerSecond();

				var cdASum = 0.SI<SquareMeter>();

				for (var heightPercent = startHeightPercent;
					heightPercent < maxHeightPercent;
					heightPercent += heightPercentStep)
				{
					var height = heightPercent / 100.0 * vehicleHeight;
					var vWind = Physics.BaseWindSpeed *
								Math.Pow(height / Physics.BaseWindHeight, Physics.HellmannExponent);

					for (var alpha = 0; alpha <= maxAlpha; alpha += alphaStep)
					{
						var vAirX = vVeh + vWind * Math.Cos(alpha.ToRadian());
						var vAirY = vWind * Math.Sin(alpha.ToRadian());

						var beta = Math.Atan(vAirY / vAirX).ToDegree();

						// ΔCdxA = A1β + A2β² + A3β³
						var deltaCdA = values.A1 * beta + values.A2 * beta * beta + values.A3 * beta * beta * beta;

						// CdxA(β) = CdxA(0) + ΔCdxA(β)
						var cdA = aerodynamicDragAera + deltaCdA;

						var share = (alpha == 0 || alpha == maxAlpha ? alphaStep / 2.0 : alphaStep) / maxAlpha;

						// v_air = sqrt(v_airX²+vAirY²)
						// cdASum = CdxA(β) * v_air²/v_veh²
						cdASum += heightShare * share * cdA * (vAirX * vAirX + vAirY * vAirY) / (vVeh * vVeh);
					}
				}

				points.Add(
					new CrossWindCorrectionCurveReader.CrossWindCorrectionEntry
					{
						Velocity = vVeh,
						EffectiveCrossSectionArea = cdASum
					});
			}

			points[0].EffectiveCrossSectionArea = points[1].EffectiveCrossSectionArea;
			return points;
		}
		internal static AirdragData SetCommonAirdragData(IAirdragDeclarationInputData data)
		{
			var retVal = new AirdragData()
			{
				SavedInDeclarationMode = data.SavedInDeclarationMode,
				Manufacturer = data.Manufacturer,
				ModelName = data.Model,
				Date = data.Date,
				CertificationMethod = data.CertificationMethod,
				CertificationNumber = data.CertificationNumber,
				DigestValueInput = data.DigestValue != null ? data.DigestValue.DigestValue : "",
			};
			return retVal;
		}
	}

	public class AirdragDataAdapter : IAirdragDataAdapter
	{
		public List<CrossWindCorrectionCurveReader.CrossWindCorrectionEntry> GetDeclarationAirResistanceCurve(
			string crosswindCorrectionParameters, SquareMeter aerodynamicDragAera, Meter vehicleHeight)
		{
			return AirdragDataAdapterHelper.GetDeclarationAirResistanceCurve(crosswindCorrectionParameters,
				aerodynamicDragAera, vehicleHeight);
		}
		protected AirdragData DefaultAirdragData(Mission mission)
		{
			var aerodynamicDragArea = mission.DefaultCDxA + mission.Trailer.Sum(t => t.DeltaCdA).DefaultIfNull(0);

			return new AirdragData()
			{
				CertificationMethod = CertificationMethod.StandardValues,
				CrossWindCorrectionMode = CrossWindCorrectionMode.DeclarationModeCorrection,
				DeclaredAirdragArea = mission.DefaultCDxA,
				CrossWindCorrectionCurve = new CrosswindCorrectionCdxALookup(
					aerodynamicDragArea,
					GetDeclarationAirResistanceCurve(
						mission.CrossWindCorrectionParameters, aerodynamicDragArea, mission.VehicleHeight),
					CrossWindCorrectionMode.DeclarationModeCorrection)
			};
		}
		internal AirdragData SetCommonAirdragData(IAirdragDeclarationInputData data)
		{
			return AirdragDataAdapterHelper.SetCommonAirdragData(data);
		}

		public virtual AirdragData CreateAirdragData(
			IAirdragDeclarationInputData airdragInputData, Mission mission,
			Segment segment)
		{
			if (airdragInputData == null || airdragInputData.AirDragArea == null)
			{
				return DefaultAirdragData(mission);
			}

			var retVal = SetCommonAirdragData(airdragInputData);
			retVal.CrossWindCorrectionMode = CrossWindCorrectionMode.DeclarationModeCorrection;
			retVal.DeclaredAirdragArea = mission.MissionType == MissionType.Construction
				? mission.DefaultCDxA
				: airdragInputData.AirDragArea;

			var aerodynamicDragArea =
				retVal.DeclaredAirdragArea + mission.Trailer.Sum(t => t.DeltaCdA).DefaultIfNull(0);

			retVal.CrossWindCorrectionCurve =
				new CrosswindCorrectionCdxALookup(
					aerodynamicDragArea,
					GetDeclarationAirResistanceCurve(
						mission.CrossWindCorrectionParameters, aerodynamicDragArea, mission.VehicleHeight),
					CrossWindCorrectionMode.DeclarationModeCorrection);
			return retVal;
		}

		public virtual AirdragData CreateAirdragData(IVehicleDeclarationInputData completedVehicle, Mission mission)
		{
			if (!mission.BusParameter.AirDragMeasurementAllowed ||
				completedVehicle.Components.AirdragInputData?.AirDragArea == null) {
				return new AirdragData() {
					CertificationMethod = CertificationMethod.StandardValues,
					DeclaredAirdragArea = mission.DefaultCDxA,
					CrossWindCorrectionCurve = new CrosswindCorrectionCdxALookup(
						mission.DefaultCDxA,
						AirdragDataAdapterHelper.GetDeclarationAirResistanceCurve(
							mission.CrossWindCorrectionParameters, mission.DefaultCDxA, completedVehicle.Height + mission.BusParameter.DeltaHeight),
						CrossWindCorrectionMode.DeclarationModeCorrection),
					CrossWindCorrectionMode = CrossWindCorrectionMode.DeclarationModeCorrection
				};
			}

			var retVal = AirdragDataAdapterHelper.SetCommonAirdragData(completedVehicle.Components.AirdragInputData);
			retVal.CrossWindCorrectionMode = CrossWindCorrectionMode.DeclarationModeCorrection;
			var aerodynamicDragArea = completedVehicle.Components.AirdragInputData.AirDragArea;

			retVal.DeclaredAirdragArea = aerodynamicDragArea;
			retVal.CrossWindCorrectionCurve = new CrosswindCorrectionCdxALookup(
				aerodynamicDragArea,
				AirdragDataAdapterHelper.GetDeclarationAirResistanceCurve(
					mission.CrossWindCorrectionParameters,
					aerodynamicDragArea,
					completedVehicle.Height + mission.BusParameter.DeltaHeight),
				CrossWindCorrectionMode.DeclarationModeCorrection);

			return retVal;
		}

	}

	public class CompletedBusSpecificAirdragDataAdapter : AirdragDataAdapter { }

	public class SingleBusAirdragDataAdapter : CompletedBusSpecificAirdragDataAdapter { }

}
