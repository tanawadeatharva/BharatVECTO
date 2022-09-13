using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.ElectricMotor;
using TUGraz.VectoCore.Utils;


namespace TUGraz.VectoCore.Models.GenericModelData
{
	public class GenericBusIEPCData
	{
		#region Constant

		public const double GearEfficiency = 0.95;

		public const string MotorSpeedNorm = "n_norm";
		public const string TorqueNorm = "T_norm";
		public const string PowerElectricalNorm = "Pel_norm";

		private static string GenericEfficiencyMap_IEPC_ASM =
			$"{DeclarationData.DeclarationDataResourcePrefix}.GenericBusData.EfficiencyMap_IEPC_ASM_normalized.vmap";

		private static string GenericEfficiencyMap_IEPC_PSM =
			$"{DeclarationData.DeclarationDataResourcePrefix}.GenericBusData.EfficiencyMap_IEPC_PSM_normalized.vmap";

		#endregion

		private double axleEfficiency;
		private Dictionary<int, double> gearRatios;
		private double axleRatio;
		private KeyValuePair<int, double> gearRatioAtMeasurement;


		public IEPCElectricMotorData GetIEPCElectricMotorData(IIEPCDeclarationInputData iepcData, IAxleGearInputData axleGearData)
		{
			InitData(iepcData, axleGearData);

			var count = iepcData.DesignTypeWheelMotor && iepcData.NrOfDesignTypeWheelMotorMeasured == 1 ? 2 : 1; //? also for declaration mode valid

			var iepcEM = new IEPCElectricMotorData {
				IEPCDragCurves = GetIEPCDragCurves(iepcData, count),
				EfficiencyData = GetIEPCVoltageLevelData(iepcData.VoltageLevels, count, iepcData.ElectricMachineType),
				Inertia = iepcData.Inertia * count,
				RatioPerGear = gearRatios.Select(x => x.Value).ToArray()
				//iepcEM.OverloadRegenerationFactor
				//iepcEM.RatioADC
				//iepcEM.TransmissionLossMap
				//iepcEM.EMDragCurve
				//iepcEM.Overload
			};
			
			return iepcEM;
		}

		private void InitData(IIEPCDeclarationInputData iepcData, IAxleGearInputData axleGear)
		{
			if (!iepcData.DifferentialIncluded) {
				axleEfficiency = 1;
				axleRatio = 1;
			} else {
				axleEfficiency = axleGear.Efficiency;
				axleRatio = axleGear.Ratio;
			}

			gearRatios = new Dictionary<int, double>();
			foreach (var gear in iepcData.Gears.OrderBy(x => x.GearNumber)) {
				gearRatios.Add(gear.GearNumber, gear.Ratio);
			}

			gearRatioAtMeasurement = GetGearRatioAtMeasurement();
		}

		private VoltageLevelData GetIEPCVoltageLevelData(IList<IElectricMotorVoltageLevel> voltageLevels, int count, ElectricMachineType electricMachineType)
		{
			return new VoltageLevelData {
				VoltageLevels = GetElectricMotorVoltageLevelData(voltageLevels, count, electricMachineType)
			};
		}

		private List<ElectricMotorVoltageLevelData> GetElectricMotorVoltageLevelData(IList<IElectricMotorVoltageLevel> voltageLevels, int count, ElectricMachineType electricMachineType)
		{
			var result = new List<ElectricMotorVoltageLevelData>();
			foreach (var entry in voltageLevels.OrderBy(x => x.VoltageLevel)) {

				var iepcVoltageLevel = new IEPCVoltageLevelData();
				iepcVoltageLevel.EfficiencyMaps = GetEfficiencyMaps(entry, count, electricMachineType);
				iepcVoltageLevel.Voltage = entry.VoltageLevel;
				iepcVoltageLevel.FullLoadCurve = GetElectricMotorFullLoadCurve(entry, count);
			}

			return result;
		}

		private ElectricMotorFullLoadCurve GetElectricMotorFullLoadCurve(IElectricMotorVoltageLevel voltageLevel,
			int count)
		{
			return IEPCFullLoadCurveReader.Create(voltageLevel.FullLoadCurve, count, gearRatioAtMeasurement.Value);
		}

		
		private Dictionary<uint, EfficiencyMap> GetEfficiencyMaps(IElectricMotorVoltageLevel voltageLevel, int count, ElectricMachineType electricMachineType)
		{
			var result = new Dictionary<uint, EfficiencyMap>();
			
			foreach (var gearEntry in gearRatios) {

				var gearRatio = gearEntry.Value;

				var ratedPoint = GenericRatedPointHelper.GetRatedPointOfFullLoadCurveAtIEPC(voltageLevel.FullLoadCurve,
					axleRatio, gearRatio, GearEfficiency, axleEfficiency);

				var deNormalizedMap = DeNormalizeData(GetNormalizedEfficiencyMap(electricMachineType), ratedPoint, gearRatio);
				result.Add((uint) gearEntry.Key, IEPCMapReader.Create(deNormalizedMap, count, gearRatio));
			}

			return result;
		}
		

		private Dictionary<uint, DragCurve> GetIEPCDragCurves(IIEPCDeclarationInputData iepcData, int count)
		{
			var result = new Dictionary<uint, DragCurve>();

			foreach (var dragCurve in iepcData.DragCurves) {
		
				if(!dragCurve.Gear.HasValue)
					continue;

				var ratio = iepcData.Gears.First(x => x.GearNumber == dragCurve.Gear.Value).Ratio;
				result.Add((uint)dragCurve.Gear.Value, IEPCDragCurveReader.Create(dragCurve.DragCurve, count, ratio));
			}
			
			return result;
		}


		private KeyValuePair<int, double> GetGearRatioAtMeasurement()
		{
			var gear = gearRatios.Select(x => new {
				Gear = x.Key,
				Ratio = x.Value,
				Distance = Math.Abs(x.Value - 1)
			}).OrderBy(x => x.Distance).GroupBy(x => x.Distance).First().MaxBy(x => x.Ratio);

			return new KeyValuePair<int, double>(gear.Gear, gear.Ratio);
		}
		

		private DataTable DeNormalizeData(TableData normalizedMap, RatedPoint ratedPoint, double gearRatio)
		{

			var result = new DataTable();
			result.Columns.Add(ElectricMotorMapReader.Fields.MotorSpeed);
			result.Columns.Add(ElectricMotorMapReader.Fields.Torque);
			result.Columns.Add(ElectricMotorMapReader.Fields.PowerElectrical);


			foreach (DataRow row in normalizedMap.Rows) {
				var torqueNormValue = row.ParseDouble(TorqueNorm);
				var motorSpeed = row.ParseDouble(MotorSpeedNorm) * ratedPoint.NRated / gearRatio / axleRatio;
				var torque = torqueNormValue * ratedPoint.TRated * gearRatio * axleRatio * 
							( torqueNormValue > 0 ? GearEfficiency * axleEfficiency : 1 / GearEfficiency * axleEfficiency);
				var powerElectrical = row.ParseDouble(PowerElectricalNorm) * ratedPoint.PRated;


				var newRow = result.NewRow();
				newRow[ElectricMotorMapReader.Fields.MotorSpeed] = Math.Round(motorSpeed.Value(), 2, MidpointRounding.AwayFromZero).ToXMLFormat(2);
				newRow[ElectricMotorMapReader.Fields.Torque] = Math.Round(torque.Value(), 2, MidpointRounding.AwayFromZero).ToXMLFormat(2);
				newRow[ElectricMotorMapReader.Fields.PowerElectrical] = Math.Round(powerElectrical.Value(), 2, MidpointRounding.AwayFromZero).ToXMLFormat(2);
				result.Rows.Add(newRow);

			}

			return result;
		}


		private TableData GetNormalizedEfficiencyMap(ElectricMachineType electricMachineType)
		{
			switch (electricMachineType)
			{
				case ElectricMachineType.ASM:
				case ElectricMachineType.ESM:
				case ElectricMachineType.RM:
					return ReadCsvResource(GenericEfficiencyMap_IEPC_ASM);
				case ElectricMachineType.PSM:
					return ReadCsvResource(GenericEfficiencyMap_IEPC_PSM);
				default:
					return null;
			}
		}


		private static TableData ReadCsvResource(string ressourceId)
		{
			var tmp = ressourceId.Replace(DeclarationData.DeclarationDataResourcePrefix + ".", "");
			var parts = tmp.Split('.');
			var fileName = Path.Combine("Declaration", string.Join(".", parts[parts.Length - 2], parts[parts.Length - 1]));
			if (File.Exists(fileName))
			{
				return VectoCSVFile.Read(fileName);
			}

			return VectoCSVFile.ReadStream(RessourceHelper.ReadStream(ressourceId), source: ressourceId);
		}
	}
}
