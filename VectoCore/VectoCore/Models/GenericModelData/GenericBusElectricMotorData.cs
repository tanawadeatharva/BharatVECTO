using System;
using System.Collections.Generic;
using System.Data;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.ElectricMotor;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.GenericModelData
{
	public class GenericBusElectricMotorData : GenericBusEMBase
	{
		public GenericBusElectricMotorData()
		{
			GenericEfficiencyMap_ASM =
				$"{DeclarationData.DeclarationDataResourcePrefix}.GenericBusData.EfficiencyMap_ASM_normalized.vmap";
			GenericEfficiencyMap_PSM =
				$"{DeclarationData.DeclarationDataResourcePrefix}.GenericBusData.EfficiencyMap_PSM_normalized.vmap";
		}


		public ElectricMotorData CreateGenericElectricMotorData(ElectricMachineEntry<IElectricMotorDeclarationInputData> electricMachineEntry)
		{
			var electricMachineType = electricMachineEntry.ElectricMachine.ElectricMachineType;
			
			var electricMotorData = new ElectricMotorData {
				RatioPerGear = electricMachineEntry.RatioPerGear,
				EMDragCurve = ElectricMotorDragCurveReader.Create(electricMachineEntry.ElectricMachine.DragCurve,
					electricMachineEntry.Count),
				EfficiencyData = GetVoltageLevels(electricMachineEntry, electricMachineType),
				Inertia = electricMachineEntry.ElectricMachine.Inertia * electricMachineEntry.Count,//??
				RatioADC = electricMachineEntry.RatioADC
				//electricMotorData.OverloadRegenerationFactor
				//electricMotorData.Overload
				//electricMotorData.TransmissionLossMap
			};

			return electricMotorData;
		}


		private VoltageLevelData GetVoltageLevels(ElectricMachineEntry<IElectricMotorDeclarationInputData> electricMachineEntry, 
			ElectricMachineType electricMachineType)
		{
			var voltageLevels = electricMachineEntry.ElectricMachine.VoltageLevels;
			var count = electricMachineEntry.Count;
			var normalizedMap = GetNormalizedEfficiencyMap(electricMachineType);


			 return new VoltageLevelData {
				VoltageLevels = GetElectricMotorVoltageLevelData(voltageLevels, count, normalizedMap)
			};
		}
		

		private List<ElectricMotorVoltageLevelData> GetElectricMotorVoltageLevelData(IList<IElectricMotorVoltageLevel> voltageLevels, int count, TableData normalizedMap)
		{
			var result = new List<ElectricMotorVoltageLevelData>();

			foreach (var voltageLevel in voltageLevels) {

				var ratedPoint = GenericRatedPointHelper.GetRatedPointOfFullLoadCurveAtEM(voltageLevels[0].FullLoadCurve);
				var efficiencyMap = DeNormalizeData(normalizedMap, ratedPoint);
				
				var electricMotorVoltageLevel = new ElectricMotorVoltageLevelData {
					Voltage = voltageLevel.VoltageLevel,
					FullLoadCurve = GetElectricMotorFullLoadCurve(voltageLevel.FullLoadCurve),
					EfficiencyMap = ElectricMotorMapReader.Create(efficiencyMap, count)
				};

				result.Add(electricMotorVoltageLevel);
			}
			
			return result;
		}


		private ElectricMotorFullLoadCurve GetElectricMotorFullLoadCurve(TableData fullLoadCurve)
		{
			var entries = new List<ElectricMotorFullLoadCurve.FullLoadEntry>();

			foreach (DataRow row in fullLoadCurve.Rows) {
				entries.Add(new ElectricMotorFullLoadCurve.FullLoadEntry {
					MotorSpeed = row.ParseDouble("outShaftSpeed").SI<PerSecond>(),
					FullGenerationTorque = row.ParseDouble("minTorque").SI<NewtonMeter>(),
					FullDriveTorque = row.ParseDouble("maxTorque").SI<NewtonMeter>()
				});
			}

			return new ElectricMotorFullLoadCurve(entries);
		}
		

		private DataTable DeNormalizeData(TableData normalizedMap, RatedPoint ratedPoint)
		{
			var result = new DataTable();
			result.Columns.Add(ElectricMotorMapReader.Fields.MotorSpeed);
			result.Columns.Add(ElectricMotorMapReader.Fields.Torque);
			result.Columns.Add(ElectricMotorMapReader.Fields.PowerElectrical);
			
			foreach (DataRow row in normalizedMap.Rows) {
				var motorSpeed = row.ParseDouble(MotorSpeedNorm) * ratedPoint.NRated;
				var torque = row.ParseDouble(TorqueNorm) * ratedPoint.TRated;
				var powerElectrical = row.ParseDouble(PowerElectricalNorm) * ratedPoint.PRated;

				var newRow = result.NewRow();
				newRow[ElectricMotorMapReader.Fields.MotorSpeed] = Math.Round(motorSpeed.Value(), 2, MidpointRounding.AwayFromZero).ToXMLFormat(2);
				newRow[ElectricMotorMapReader.Fields.Torque] = Math.Round(torque.Value(), 2, MidpointRounding.AwayFromZero).ToXMLFormat(2);
				newRow[ElectricMotorMapReader.Fields.PowerElectrical] = Math.Round(powerElectrical.Value(), 2, MidpointRounding.AwayFromZero).ToXMLFormat(2);
				result.Rows.Add(newRow);
			}
			
			return result;
		}
	}
}
