using System;
using System.Data;
using System.Runtime.CompilerServices;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;

namespace TUGraz.VectoCore.Models.Declaration
{
	public class GenericBusAxelgearData
	{
		public DataTable AxleGearInputLossMap { get; private set; }

		public AxleGearData CreateGenericBusAxlegearData(IAxleGearInputData axlegearData)
		{
			var axleGear = new AxleGearData
			{
				LineType = axlegearData.LineType,
				InputData = axlegearData
			};

			var ratio = axlegearData.Ratio;

			var outputLossMap = CreateAxlegearOutputLossMap(ratio);
			AxleGearInputLossMap = CalculateAxleInputLossMap(outputLossMap, ratio);

			var transmissionData = new TransmissionData
			{
				Ratio = axlegearData.Ratio,
				LossMap = TransmissionLossMapReader.Create(AxleGearInputLossMap, ratio, "Axlegear")
			};

			axleGear.AxleGear = transmissionData;

			return axleGear;
		}

		private DataTable CreateAxlegearOutputLossMap(double axleRatio)
		{
			var torques = new[] {
					Constants.GenericLossMapSettings.OutputTorqueEnd * -1.0,
					Constants.GenericLossMapSettings.OutputTorqueStart *-1.0,
					Constants.GenericLossMapSettings.OutputTorqueStart,
					Constants.GenericLossMapSettings.OutputTorqueEnd
				};

			var outStart = Constants.GenericLossMapSettings.OutputSpeedStart;
			var outEnd = Constants.GenericLossMapSettings.OutputSpeedEnd;

			var outputSpeeds = new[] {
					0, 0, 0, 0,
					outStart, outStart,outStart, outStart,
					outEnd, outEnd, outEnd, outEnd
				};

			var td0 = Constants.GenericLossMapSettings.T0 +
					  axleRatio * Constants.GenericLossMapSettings.T1;

			var td0_ = td0 * 0.5;
			var td150_ = td0 * 0.5;
			var td_n = Constants.GenericLossMapSettings.Td_n;
			var efficiency = Constants.GenericLossMapSettings.Efficiency;

			var torqueIndex = 0;


			var lossMap = new DataTable();
			lossMap.Columns.Add("output speed");
			lossMap.Columns.Add("output torque");
			lossMap.Columns.Add("output torque loss");

			for (int i = 0; i < 12; i++)
			{
				if (i % 4 == 0)
					torqueIndex = 0;

				var calculationSpeed = outputSpeeds[i].IsEqual(0)
					? outputSpeeds[4]
					: outputSpeeds[i];

				var torque = torques[torqueIndex++];

				var newRow = lossMap.NewRow();
				newRow[lossMap.Columns[0]] = outputSpeeds[i];
				newRow[lossMap.Columns[1]] = torque;
				newRow[lossMap.Columns[2]] =
					CalculateOutputTorqueLoss(td0_, td150_, td_n, calculationSpeed, torque, efficiency);

				lossMap.Rows.Add(newRow);

			}

			return lossMap;
		}

		private double CalculateOutputTorqueLoss(double td0_, double td150_, double td_n,
			double outputspeed, double ouputTorque, double efficiency)
		{
			if (ouputTorque < 0)
				ouputTorque = ouputTorque * -1.0;

			return td0_ + td150_ * outputspeed / td_n + ouputTorque / efficiency - ouputTorque;
		}

		private DataTable CalculateAxleInputLossMap(DataTable outputLossMap, double axleRatio)
		{
			var inputLossMap = new DataTable();

			inputLossMap.Columns.Add(TransmissionLossMapReader.Fields.InputSpeed);
			inputLossMap.Columns.Add(TransmissionLossMapReader.Fields.InputTorque);
			inputLossMap.Columns.Add(TransmissionLossMapReader.Fields.TorqeLoss);

			foreach (DataRow row in outputLossMap.Rows)
			{
				var outputSpeed = row[0].ToString().ToDouble();
				var outputTorque = row[1].ToString().ToDouble();
				var outputLoss = row[2].ToString().ToDouble();

				var newRow = inputLossMap.NewRow();
				newRow[0] = GetInputSpeed(outputSpeed, axleRatio);
				newRow[1] = GetInputTorque(outputTorque, outputLoss, axleRatio);
				newRow[2] = GetInputTorqueLoss(outputLoss, axleRatio);
				inputLossMap.Rows.Add(newRow);
			}

			return inputLossMap;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private double GetInputSpeed(double outputSpeed, double iAxle)
		{
			return outputSpeed * iAxle;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private double GetInputTorque(double outputTorque, double outputLoss, double iAxle)
		{
			return (outputTorque + outputLoss) / iAxle;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private double GetInputTorqueLoss(double outputLoss, double iAxle)
		{
			return outputLoss / iAxle;
		}
	}
}
