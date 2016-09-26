/*
* This file is part of VECTO.
*
* Copyright © 2012-2016 European Union
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
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
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
		public CombustionEngineData EngineData { get; internal set; }

		[ValidateObject]
		public GearboxData GearboxData { get; internal set; }

		[ValidateObject]
		public AxleGearData AxleGearData { get; internal set; }

		[ValidateObject]
		public AngledriveData AngledriveData { get; internal set; }

		[Required, ValidateObject]
		public DrivingCycleData Cycle { get; internal set; }

		[ValidateObject]
		public IEnumerable<AuxData> Aux { get; internal set; }

		public AdvancedAuxData AdvancedAux { get; internal set; }

		// todo mk 2016-08-30: property is never used. Delete?
		[ValidateObject]
		public string AccelerationLimitingFile { get; internal set; }

		[ValidateObject]
		public RetarderData Retarder { get; internal set; }

		[ValidateObject]
		public PTOData PTO { get; internal set; }

		[ValidateObject]
		public DriverData DriverData { get; internal set; }

		public ExecutionMode ExecutionMode { get; internal set; }

		[Required, MinLength(1)]
		public string JobName { get; set; }

		public string ModFileSuffix { get; set; }

		[ValidateObject]
		public DeclarationReport Report { get; set; }

		[Required, ValidateObject]
		public LoadingType Loading { get; set; }

		[ValidateObject]
		public Mission Mission { get; set; }

		public class AuxData
		{
			// ReSharper disable once InconsistentNaming
			public string ID;

			[Required] public AuxiliaryType Type;

			public IList<string> Technology;

			public string[] TechList;

			[SIRange(0, 100 * Constants.Kilo)] public Watt PowerDemand;

			[Required] public AuxiliaryDemandType DemandType;

			[ValidateObject] public AuxiliaryData Data;
		}

		public class StartStopData
		{
			public bool Enabled;
			[Required, SIRange(0, 120 / Constants.MeterPerSecondToKMH)] public MeterPerSecond MaxSpeed;
			[Required, SIRange(0, 100)] public Second MinTime;
			[Required, SIRange(0, 100)] public Second Delay;
		}

		public static ValidationResult ValidateRunData(VectoRunData runData, ValidationContext validationContext)
		{
			var gearboxData = runData.GearboxData;
			var engineData = runData.EngineData;

			var maxSpeed = 95.KMPHtoMeterPerSecond();

			if (gearboxData != null) {
				var axleGearData = runData.AxleGearData;
				var angledriveData = runData.AngledriveData;
				var hasAngleDrive = angledriveData != null && angledriveData.Angledrive != null;
				var angledriveRatio = hasAngleDrive && angledriveData.Type == AngledriveType.SeparateAngledrive
					? angledriveData.Angledrive.Ratio
					: 1.0;
				var axlegearRatio = axleGearData != null ? axleGearData.AxleGear.Ratio : 1.0;
				var dynamicTyreRadius = runData.VehicleData != null ? runData.VehicleData.DynamicTyreRadius : 0.0.SI<Meter>();

				foreach (var gear in gearboxData.Gears) {
					for (var angularVelocity = engineData.IdleSpeed;
						angularVelocity < engineData.FullLoadCurve.RatedSpeed;
						angularVelocity += 2.0 / 3.0 * (engineData.FullLoadCurve.RatedSpeed - engineData.IdleSpeed) / 10.0) {
						var velocity = angularVelocity / gear.Value.Ratio / angledriveRatio / axlegearRatio * dynamicTyreRadius;

						if (velocity > maxSpeed) {
							continue;
						}


						for (var inTorque = engineData.FullLoadCurve.FullLoadStationaryTorque(angularVelocity) / 3;
							inTorque < engineData.FullLoadCurve.FullLoadStationaryTorque(angularVelocity);
							inTorque += 2.0 / 3.0 * engineData.FullLoadCurve.FullLoadStationaryTorque(angularVelocity) / 10.0) {
							if (gear.Value.MaxTorque != null && inTorque > gear.Value.MaxTorque) {
								continue;
							}

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
									string.Format("Interpolation of Angledrive-LossMap failed with torque={1} and angularSpeed={2}", gear.Key,
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
						}
					}
				}
			}

			return ValidationResult.Success;
		}
	}
}