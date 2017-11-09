/*
* This file is part of VECTO.
*
* Copyright © 2012-2017 European Union
*
* Developed by Graz University of Technology,
*              Institute of Internal Combustion Engines and Thermodynamics,
*              Institute of Technical Informatics
*
* VECTO is licensed under the EUPL, Version 1.1 or - as soon they will be approved
* by the European Commission - subsequent versions of the EUPL (the "Licence");
* You may not use VECTO except in compliance with the Licence.
* You may obtain a copy of the Licence at:
*
* https://joinup.ec.europa.eu/community/eupl/og_page/eupl
*
* Unless required by applicable law or agreed to in writing, VECTO
* distributed under the Licence is distributed on an "AS IS" basis,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the Licence for the specific language governing permissions and
* limitations under the Licence.
*
* Authors:
*   Stefan Hausberger, hausberger@ivt.tugraz.at, IVT, Graz University of Technology
*   Christian Kreiner, christian.kreiner@tugraz.at, ITI, Graz University of Technology
*   Michael Krisper, michael.krisper@tugraz.at, ITI, Graz University of Technology
*   Raphael Luz, luz@ivt.tugraz.at, IVT, Graz University of Technology
*   Markus Quaritsch, markus.quaritsch@tugraz.at, IVT, Graz University of Technology
*   Martin Rexeis, rexeis@ivt.tugraz.at, IVT, Graz University of Technology
*/

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Xml.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.OutputData;
using DriverData = TUGraz.VectoCore.Models.SimulationComponent.Data.DriverData;

namespace TUGraz.VectoCore.Models.Simulation.Data
{
	[CustomValidation(typeof(VectoRunData), "ValidateRunData")]
	public class VectoRunData : SimulationComponentData
	{
		[ValidateObject]
		public VehicleData VehicleData { get; internal set; }

		[ValidateObject]
		public AirdragData AirdragData { get; internal set; }

		[ValidateObject]
		public CombustionEngineData EngineData { get; internal set; }

		[ValidateObject]
		public GearboxData GearboxData { get; internal set; }

		[ValidateObject]
		public AxleGearData AxleGearData { get; internal set; }

		[ValidateObject]
		public AngledriveData AngledriveData { get; internal set; }

		[Required, ValidateObject]
		public IDrivingCycleData Cycle { get; internal set; }

		[ValidateObject]
		public IEnumerable<AuxData> Aux { get; internal set; }

		public AdvancedAuxData AdvancedAux { get; internal set; }

		[ValidateObject]
		public RetarderData Retarder { get; internal set; }

		[ValidateObject]
		public PTOData PTO { get; internal set; }

		[ValidateObject]
		public DriverData DriverData { get; internal set; }

		public ExecutionMode ExecutionMode { get; internal set; }

		[Required, MinLength(1)]
		public string JobName { get; internal set; }

		public string ModFileSuffix { get; internal set; }

		[ValidateObject]
		public IDeclarationReport Report { get; internal set; }

		[Required, ValidateObject]
		public LoadingType Loading { get; internal set; }

		[ValidateObject]
		public Mission Mission { get; internal set; }

		public XElement InputDataHash { get; internal set; }

		public int JobRunId { get; internal set; }

        public double[] AuxFanParameters { get; internal set; }

        public class AuxData
		{
			// ReSharper disable once InconsistentNaming
			public string ID;

			public IList<string> Technology;

			[SIRange(0, 100 * Constants.Kilo)] public Watt PowerDemand;

			[Required] public AuxiliaryDemandType DemandType;

			[ValidateObject] public AuxiliaryData Data;
		}

		public static ValidationResult ValidateRunData(VectoRunData runData, ValidationContext validationContext)
		{
			var gearboxData = runData.GearboxData;
			var engineData = runData.EngineData;

			if (gearboxData != null) {
				var validationResult = CheckPowertrainLossMapsSize(runData, gearboxData, engineData);
				if (validationResult != null) {
					return validationResult;
				}
			}

			if (runData.Cycle != null && runData.Cycle.Entries.Any(e => e.PTOActive)) {
				if (runData.PTO == null || runData.PTO.PTOCycle == null) {
					return new ValidationResult("PTOCycle is used in DrivingCycle, but is not defined in Vehicle-Data.");
				}
			}

			return ValidationResult.Success;
		}

		private static ValidationResult CheckPowertrainLossMapsSize(VectoRunData runData, GearboxData gearboxData,
			CombustionEngineData engineData)
		{
			var maxSpeed = 95.KMPHtoMeterPerSecond();
			var axleGearData = runData.AxleGearData;
			var angledriveData = runData.AngledriveData;
			var hasAngleDrive = angledriveData != null && angledriveData.Angledrive != null;
			var angledriveRatio = hasAngleDrive && angledriveData.Type == AngledriveType.SeparateAngledrive
				? angledriveData.Angledrive.Ratio
				: 1.0;
			var axlegearRatio = axleGearData != null ? axleGearData.AxleGear.Ratio : 1.0;
			var dynamicTyreRadius = runData.VehicleData != null ? runData.VehicleData.DynamicTyreRadius : 0.0.SI<Meter>();

			if (gearboxData.Gears.Count + 1 != engineData.FullLoadCurves.Count) {
				return
					new ValidationResult(
						string.Format("number of full-load curves in engine does not match gear count. engine fld: {0}, gears: {1}",
							engineData.FullLoadCurves.Count, gearboxData.Gears.Count));
			}

			foreach (var gear in gearboxData.Gears) {
				for (var angularVelocity = engineData.IdleSpeed;
					angularVelocity < engineData.FullLoadCurves[gear.Key].RatedSpeed;
					angularVelocity += 2.0 / 3.0 * (engineData.FullLoadCurves[gear.Key].RatedSpeed - engineData.IdleSpeed) / 10.0) {
					if (!gear.Value.HasLockedGear) {
						continue;
					}

					var velocity = angularVelocity / gear.Value.Ratio / angledriveRatio / axlegearRatio * dynamicTyreRadius;

					if (velocity > maxSpeed) {
						continue;
					}

					for (var inTorque = engineData.FullLoadCurves[gear.Key].FullLoadStationaryTorque(angularVelocity) / 3;
						inTorque < engineData.FullLoadCurves[gear.Key].FullLoadStationaryTorque(angularVelocity);
						inTorque += 2.0 / 3.0 * engineData.FullLoadCurves[gear.Key].FullLoadStationaryTorque(angularVelocity) / 10.0) {
						var validateRunData = CheckLossMapsEntries(gear, angularVelocity, inTorque, angledriveData, axleGearData, velocity);
						if (validateRunData != null) {
							return validateRunData;
						}
					}
				}
			}
			return null;
		}

		private static ValidationResult CheckLossMapsEntries(KeyValuePair<uint, GearData> gear, PerSecond angularVelocity,
			NewtonMeter inTorque, AngledriveData angledriveData, AxleGearData axleGearData, SI velocity)
		{
			var hasAngleDrive = angledriveData != null && angledriveData.Angledrive != null;
			var angledriveRatio = hasAngleDrive && angledriveData.Type == AngledriveType.SeparateAngledrive
				? angledriveData.Angledrive.Ratio
				: 1.0;
			NewtonMeter angledriveTorque;
			try {
				angledriveTorque = gear.Value.LossMap.GetOutTorque(angularVelocity, inTorque);
			} catch (VectoException) {
				return new ValidationResult(
					string.Format("Interpolation of Gear-{0}-LossMap failed with torque={1} and angularSpeed={2}", gear.Key,
						inTorque, angularVelocity.ConvertTo().Rounds.Per.Minute));
			}
			var axlegearTorque = angledriveTorque;
			try {
				if (hasAngleDrive) {
					axlegearTorque = angledriveData.Angledrive.LossMap.GetOutTorque(angularVelocity / gear.Value.Ratio,
						angledriveTorque);
				}
			} catch (VectoException) {
				return new ValidationResult(
					string.Format("Interpolation of Angledrive-LossMap failed with torque={0} and angularSpeed={1}",
						angledriveTorque, (angularVelocity / gear.Value.Ratio).ConvertTo().Rounds.Per.Minute));
			}

			if (axleGearData != null) {
				var axleAngularVelocity = angularVelocity / gear.Value.Ratio / angledriveRatio;
				try {
					axleGearData.AxleGear.LossMap.GetOutTorque(axleAngularVelocity, axlegearTorque);
				} catch (VectoException) {
					return
						new ValidationResult(
							string.Format(
								"Interpolation of AxleGear-LossMap failed with torque={0} and angularSpeed={1} (gear={2}, velocity={3})",
								axlegearTorque, axleAngularVelocity.ConvertTo().Rounds.Per.Minute, gear.Key, velocity));
				}
			}
			return null;
		}
	}
}