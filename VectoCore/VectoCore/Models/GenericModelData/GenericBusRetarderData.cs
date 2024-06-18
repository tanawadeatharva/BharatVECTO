using System;
using System.Collections.Generic;
using System.Data;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Models.SimulationComponent.Data;

namespace TUGraz.VectoCore.Models.Declaration
{
	public class GenericBusRetarderData
	{
		public RetarderData CreateGenericBusRetarderData(IRetarderInputData retarderInput, PerSecond engineSpeed, double gearboxRatio) =>
			new RetarderData {
				Type = retarderInput?.Type ?? RetarderType.None,
				Ratio = retarderInput?.Type.IsDedicatedComponent() ?? false ? retarderInput.Ratio : 1.0,
				LossMap = retarderInput?.Type.IsDedicatedComponent() ?? false 
					? GenerateGenericLossMap(retarderInput, engineSpeed.AsRPM, gearboxRatio) : null
			};

		public RetarderData CreateGenericBusRetarderData(IRetarderInputData retarderInput) =>
			new RetarderData {
				Type = retarderInput?.Type ?? RetarderType.None,
				Ratio = retarderInput?.Type.IsDedicatedComponent() ?? false ? retarderInput.Ratio : 1.0,
				LossMap = retarderInput?.Type.IsDedicatedComponent() ?? false 
					? GenerateGenericLossMap(retarderInput) : null
			};

		private RetarderLossMap GenerateGenericLossMap(IRetarderInputData retarderData, double engineSpeed = 0, double gearboxRatio = 0)
		{
			var retarderSpeeds = GenerateRetarderSpeeds(retarderData, engineSpeed, gearboxRatio);

			var genericRetarderLosses = GetHydrodynamicRetardersLoss(retarderSpeeds, retarderData.Ratio);
			//var genericRetarderLosses = GetMagneticRetarderLoss(retarderSpeeds, stepUpRatio);

			var torqueLoss = new DataTable();
			torqueLoss.Columns.Add(RetarderLossMapReader.Fields.RetarderSpeed);
			torqueLoss.Columns.Add(RetarderLossMapReader.Fields.TorqueLoss);

			for (int i = 0; i < genericRetarderLosses.Length; i++)
			{
				var newRow = torqueLoss.NewRow();
				newRow[RetarderLossMapReader.Fields.RetarderSpeed] = Math.Round(retarderSpeeds[i], 2, MidpointRounding.AwayFromZero).ToXMLFormat(2);
				newRow[RetarderLossMapReader.Fields.TorqueLoss] = Math.Round(genericRetarderLosses[i] *
					Constants.GenericLossMapSettings.RetarderGenericFactor, 2, MidpointRounding.AwayFromZero).ToXMLFormat();
				torqueLoss.Rows.Add(newRow);
			}

			return RetarderLossMapReader.Create(torqueLoss);
		}

		private double[] GenerateRetarderSpeeds(IRetarderInputData retarderData, double engineSpeedRPM, double gearboxRatio)
		{
			var SMALL_STEP = 200;
			var LARGE_STEP = 500;
			var LARGE_STEP_THRESHOLD = 1000;

			var defaultEngineSpeed = 5000;
			var maxRetarderSpeed = retarderData.Ratio * defaultEngineSpeed;
			if (engineSpeedRPM != 0 && gearboxRatio != 0)
			{
				maxRetarderSpeed = retarderData.Ratio * engineSpeedRPM;

				if (retarderData.Type == RetarderType.TransmissionOutputRetarder)
				{
					maxRetarderSpeed = retarderData.Ratio * (engineSpeedRPM / gearboxRatio);
				}
			}

			var step = SMALL_STEP;
			var retarderSpeeds = new List<double>();
			for (int i = 0; i < maxRetarderSpeed + step; i += step)
			{
				retarderSpeeds.Add(i);
				var currentMaxSpeed = retarderSpeeds.MaxBy(v => v);
				if (currentMaxSpeed >= LARGE_STEP_THRESHOLD)
				{
					step = LARGE_STEP;
				}
			}

			return retarderSpeeds.ToArray();
		}

		private double[] GetHydrodynamicRetardersLoss(double[] retarderSpeeds, double stepUpRatio)
		{
			var losses = new double[retarderSpeeds.Length];

			for (int i = 0; i < retarderSpeeds.Length; i++)
			{
				losses[i] = 10.0 / stepUpRatio + 2.0 / Math.Pow(stepUpRatio, 3) * Math.Pow(retarderSpeeds[i] / 1000.0, 2);
			}

			return losses;
		}

		private double[] GetMagneticRetarderLoss(double[] retarderSpeeds, double stepUpRatio)
		{
			var losses = new double[retarderSpeeds.Length];

			for (int i = 0; i < retarderSpeeds.Length; i++)
			{
				losses[i] = 15.0 / stepUpRatio + 2.0 / Math.Pow(stepUpRatio, 4) * Math.Pow(retarderSpeeds[i] / 1000.0, 3);
			}

			return losses;
		}
	}
}
