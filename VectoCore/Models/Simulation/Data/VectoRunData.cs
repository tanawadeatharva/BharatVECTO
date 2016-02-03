/*
* Copyright 2015 European Union
*
* Licensed under the EUPL (the "Licence");
* You may not use this work except in compliance with the Licence.
* You may obtain a copy of the Licence at:
*
* http://ec.europa.eu/idabc/eupl5
*
* Unless required by applicable law or agreed to in writing, software 
* distributed under the Licence is distributed on an "AS IS" basis,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the Licence for the specific language governing permissions and 
* limitations under the Licence.
*/

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Exceptions;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.PDF;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.Simulation.Data
{
	[DataContract]
	[CustomValidation(typeof(VectoRunData), "ValidateRunData")]
	public class VectoRunData : SimulationComponentData
	{
		[Required, ValidateObject]
		public VehicleData VehicleData { get; internal set; }

		[Required, ValidateObject]
		public CombustionEngineData EngineData { get; internal set; }

		[Required, ValidateObject]
		public GearboxData GearboxData { get; internal set; }

		[Required, ValidateObject]
		public AxleGearData AxleGearData { get; internal set; }

		[Required, ValidateObject]
		public DrivingCycleData Cycle { get; internal set; }

		[Required, ValidateObject]
		public IEnumerable<AuxData> Aux { get; internal set; }

		[Required, ValidateObject]
		public string AccelerationLimitingFile { get; internal set; }

		[Required, ValidateObject]
		public RetarderData Retarder { get; internal set; }

		[Required, ValidateObject]
		public DriverData DriverData { get; internal set; }

		public bool IsEngineOnly { get; internal set; }

		[Required, MinLength(1)]
		public string JobName { get; set; }

		[Required]
		public string ModFileSuffix { get; set; }

		[Required, ValidateObject]
		public DeclarationReport Report { get; set; }

		[Required, ValidateObject]
		public LoadingType Loading { get; set; }

		[Required, ValidateObject]
		public Mission Mission { get; set; }

		public class AuxData
		{
			[Required] public string ID;

			[Required] public AuxiliaryType Type;

			public string Technology;

			[Required] public string[] TechList;

			[Required, SIRange(0, 100 * Constants.Kilo)] public Watt PowerDemand;

			[Required] public AuxiliaryDemandType DemandType;

			[Required, ValidateObject] public AuxiliaryData Data;
		}

		public class StartStopData
		{
			public bool Enabled;
			[Required, SIRange(0, 120 / Constants.MeterPerSecondToKMH)] public MeterPerSecond MaxSpeed;
			[Required, SIRange(0, 100)] public Second MinTime;
			[Required, SIRange(0, 100)] public Second Delay;
		}

		public ValidationResult ValidateRunData(VectoRunData runData, ValidationContext validationContext)
		{
			var gearboxData = runData.GearboxData;
			var engineData = runData.EngineData;
			var axleGearData = runData.AxleGearData;

			if (gearboxData != null) {
				foreach (var gear in gearboxData.Gears) {
					for (var angularVelocity = engineData.IdleSpeed;
						angularVelocity < engineData.FullLoadCurve.RatedSpeed;
						angularVelocity += 2.0 / 3.0 * (engineData.FullLoadCurve.RatedSpeed - engineData.IdleSpeed) / 10.0) {
						for (var inTorque = engineData.FullLoadCurve.FullLoadStationaryTorque(angularVelocity) / 3;
							inTorque < engineData.FullLoadCurve.FullLoadStationaryTorque(angularVelocity);
							inTorque += 2.0 / 3.0 * engineData.FullLoadCurve.FullLoadStationaryTorque(angularVelocity) / 10.0) {
							NewtonMeter axleTorque;
							try {
								axleTorque = gear.Value.LossMap.GetOutTorque(angularVelocity, inTorque);
							} catch (VectoException) {
								return
									new ValidationResult(
										string.Format("Interpolation of Gear-{0}-LossMap failed with torque={1} and angularSpeed={2}", gear.Key,
											inTorque, angularVelocity.ConvertTo().Rounds.Per.Minute));
							}

							if (axleGearData != null) {
								var axleAngularVelocity = angularVelocity / gear.Value.Ratio;
								try {
									axleGearData.AxleGear.LossMap.GetOutTorque(axleAngularVelocity, axleTorque);
								} catch (VectoException) {
									return
										new ValidationResult(
											string.Format("Interpolation of AxleGear-LossMap failed with torque={0} and angularSpeed={1}", axleTorque,
												axleAngularVelocity.ConvertTo().Rounds.Per.Minute));
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