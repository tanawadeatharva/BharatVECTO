using System;
using System.Data;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;

namespace TUGraz.VectoCore.Models.Declaration
{
	public class GenericBusAngledriveData
	{
		public AngledriveData CreateGenericBusAngledriveData(IAngledriveInputData angledriveInputData, double axleRatio,
			DataTable axleGearInputLossMap)
		{
			if (angledriveInputData.DataSource.SourceFile == null)
				return null;

			var angledriveData = new AngledriveData
			{
				Type = angledriveInputData.Type,
				InputData = angledriveInputData
			};
			
			var transmissionAngleDrive = new TransmissionData
			{
				Ratio = angledriveInputData.Ratio,
				LossMap = GetAngleDriveLossMap(axleGearInputLossMap, angledriveInputData.Ratio)
			};

			angledriveData.Angledrive = transmissionAngleDrive;

			return angledriveData;
		}

		private TransmissionLossMap GetAngleDriveLossMap(DataTable axleGearInputTable, double ratio)
		{
			var angleDriveLossMap = new DataTable();
			angleDriveLossMap.Columns.Add(TransmissionLossMapReader.Fields.InputSpeed);
			angleDriveLossMap.Columns.Add(TransmissionLossMapReader.Fields.InputTorque);
			angleDriveLossMap.Columns.Add(TransmissionLossMapReader.Fields.TorqeLoss);

			var angleDriveFactor = Constants.GenericLossMapSettings.FactorAngleDrive;
			foreach (DataRow row in axleGearInputTable.Rows)
			{
				var inputSpeed = row[0].ToString().ToDouble();
				var inputTorque = row[1].ToString().ToDouble();
				var inputTorqueLoss = row[2].ToString().ToDouble() * angleDriveFactor;

				var newRow = angleDriveLossMap.NewRow();
				newRow[0] = inputSpeed;
				newRow[1] = inputTorque;
				newRow[2] = inputTorqueLoss;
				angleDriveLossMap.Rows.Add(newRow);
			}

			return TransmissionLossMapReader.Create(angleDriveFactor, ratio, "AngleDrive");
		}
	}
}
