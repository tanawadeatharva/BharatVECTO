/*
* This file is part of VECTO.
*
* Copyright © 2012-2019 European Union
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

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Newtonsoft.Json.Linq;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter;
using TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.Electrics;
using TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.HVAC;
using TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.Pneumatics;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Engine;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.Utils;


namespace TUGraz.VectoCore.Models.Declaration
{
	public static class DeclarationData
	{
		/// <summary>
		/// The standard acceleration for gravity on earth.
		/// http://physics.nist.gov/Pubs/SP330/sp330.pdf (page 52)
		/// </summary>
		public static readonly MeterPerSquareSecond GravityAccelleration = 9.80665.SI<MeterPerSquareSecond>();

		public static readonly KilogramPerCubicMeter AirDensity = 1.188.SI<KilogramPerCubicMeter>();


		public const string DeclarationDataResourcePrefix = "TUGraz.VectoCore.Resources.Declaration";

		public static readonly Watt MinEnginePowerForEMS = 300e3.SI<Watt>();

		public static readonly TruckSegments TruckSegments = new TruckSegments();
		public static readonly PrimaryBusSegments PrimaryBusSegments = new PrimaryBusSegments();
		public static readonly CompletedBusSegments CompletedBusSegments = new CompletedBusSegments();
		public static readonly Wheels Wheels = new Wheels();
		public static readonly PT1 PT1 = new PT1();
		public static readonly FuelData FuelData = FuelData.Instance();
		public static readonly ElectricSystem ElectricSystem = new ElectricSystem();
		public static readonly Fan Fan = new Fan();


		public static readonly HeatingVentilationAirConditioning HeatingVentilationAirConditioning =
			new HeatingVentilationAirConditioning();

		public static readonly PneumaticSystem PneumaticSystem = new PneumaticSystem();
		public static readonly SteeringPump SteeringPump = new SteeringPump();
		public static readonly SteeringPumpBus SteeringPumpBus = new SteeringPumpBus();
		public static readonly WHTCCorrection WHTCCorrection = new WHTCCorrection();
		public static readonly AirDrag AirDrag = new AirDrag();
		public static readonly StandardBodies StandardBodies = new StandardBodies();
		public static readonly Payloads Payloads = new Payloads();

		public static readonly PTOTransmission PTOTransmission = new PTOTransmission();

		//public static MeterPerSecond CycleSpeedLimit;
		public const double LossMapExtrapolationFactor = 6;

		public static readonly ADASCombinations ADASCombinations = new ADASCombinations();

		public static readonly WeightingGroups WeightingGroup = new WeightingGroups();
		public static readonly WeightingFactors WeightingFactors = new WeightingFactors();

		public const double AlternaterEfficiency = 0.7;

		/// <summary>
		/// Formula for calculating the payload for a given gross vehicle weight.
		/// (so called "pc-formula", Whitebook Apr 2016, Part 1, p.187)
		/// </summary>
		public static Kilogram GetPayloadForGrossVehicleWeight(Kilogram grossVehicleWeight, string equationName)
		{
			if (equationName.ToLower().StartsWith("pc10"))
			{
				return Payloads.Lookup10Percent(grossVehicleWeight);
			}

			if (equationName.ToLower().StartsWith("pc75"))
			{
				return Payloads.Lookup75Percent(grossVehicleWeight);
			}

			return Payloads.Lookup50Percent(grossVehicleWeight);
		}

		/// <summary>
		/// Returns the payload for a trailer. This is 75% of (GVW-CurbWeight).
		/// </summary>
		public static Kilogram GetPayloadForTrailerWeight(Kilogram grossVehicleWeight, Kilogram curbWeight,
			bool lowLoading)
		{
			return
				(Math.Round(
					(Payloads.LookupTrailer(grossVehicleWeight, curbWeight) / (lowLoading ? 7.5 : 1)).LimitTo(
						0.SI<Kilogram>(),
						grossVehicleWeight - curbWeight).Value() / 100, 0) * 100).SI<Kilogram>();
		}

		public static int PoweredAxle()
		{
			return 1;
		}

		public static class FactorMethodBus
		{
			#region Constans
			private static string GenericEngineCM_Normed_CI =
				$"{DeclarationDataResourcePrefix}.GenericBusData.EngineConsumptionMap_CI_Normed.vmap";

			private static string GenericEngineCM_Normed_PI =
				$"{DeclarationDataResourcePrefix}.GenericBusData.EngineConsumptionMap_PI_Normed.vmap";
			
			public static string GenericTorqueConvert =
				$"{DeclarationDataResourcePrefix}.GenericBusData.GenericTorqueConverter.csv";
			#endregion


			#region Create Engine Data

			public static CombustionEngineData CreateBusEngineData(IVehicleDeclarationInputData pifVehicle)
			{
				var enginePif = pifVehicle.Components.EngineInputData;
				var gearbox = pifVehicle.Components.GearboxInputData;

				var engine = new CombustionEngineData();

				var fullLoadCurves = new Dictionary<uint, EngineFullLoadCurve>
				{
					[0] = FullLoadCurveReader.Create(enginePif.EngineModes.First().FullLoadCurve, true)
				};
				// TODO: MQ 20200401 add full-load curves per gear, limited by max torque (gbx or vehicle)
				//   see DeclarationDataAdapterHeavyLorry ln 235ff.

				engine.FullLoadCurves = fullLoadCurves;

				engine.IdleSpeed = enginePif.EngineModes[0].IdleSpeed;
				engine.Displacement = enginePif.Displacement;

				var fuel = GetCombustionEngineFuelData(enginePif.EngineModes.First().Fuels.First().FuelType,
					pifVehicle.DualFuelVehicle);


				engine.WHRType = WHRType.None;

				engine.Fuels = new List<CombustionEngineFuelData> { fuel };

				engine.Inertia = Engine.EngineInertia(engine.Displacement, gearbox.Type);
				return engine;
			}


			private static CombustionEngineFuelData GetCombustionEngineFuelData(FuelType fuelType, bool isDualFuel)
			{
				var ressourceId = string.Empty;

				if (isDualFuel)
				{
					ressourceId = GenericEngineCM_Normed_CI;
				}
				else
				{
					switch (fuelType)
					{
						case FuelType.DieselCI:
						case FuelType.EthanolCI:
						case FuelType.NGCI:
							ressourceId = GenericEngineCM_Normed_CI;
							break;
						case FuelType.EthanolPI:
						case FuelType.LPGPI:
						case FuelType.PetrolPI:
						case FuelType.NGPI:
							ressourceId = GenericEngineCM_Normed_PI;
							break;
					}
				}
				//ToDo ConsumptionMap
				//var denormalizedData = DenormalizeData(ressourceId);
				//var fcMap = FuelConsumptionMapReader.Create(denormalizedData);

				var fuel = new CombustionEngineFuelData
				{
					WHTCUrban = 1,
					WHTCRural = 1,
					WHTCMotorway = 1,
					ColdHotCorrectionFactor = 1,
					CorrectionFactorRegPer = 1,
					//ConsumptionMap = fcMap
				};

				return fuel;

			}

			private static DataTable DenormalizeData(string ressourceId)
			{
				// TODO: use vehicle specific values. n_idle  from PIF, nRated calculated from
				// full-load curve (fullLoadCurves[0].RatedSpeed), m_rated => max torque (fullLoadCurves[0].MaxTorque)
				var nIdle = 600.0;
				var nRated = 1800.0;
				var mRated = 1750.0;

				var normedData = VectoCSVFile.ReadStream(RessourceHelper.ReadStream(ressourceId), source: ressourceId);

				var result = new DataTable();
				result.Columns.Add(FuelConsumptionMapReader.Fields.EngineSpeed);
				result.Columns.Add(FuelConsumptionMapReader.Fields.Torque);
				result.Columns.Add(FuelConsumptionMapReader.Fields.FuelConsumption);

				foreach (DataRow row in normedData.Rows)
				{
					var engineSpeed = DenormalizeEngineSpeed((string)row[FuelConsumptionMapReader.Fields.EngineSpeed],
						nIdle, nRated);
					var torque = DenormalizeTorque((string)row[FuelConsumptionMapReader.Fields.Torque], mRated);
					var fc = DenormalizeFC((string)row[FuelConsumptionMapReader.Fields.FuelConsumption], mRated);

					var newRow = result.NewRow();
					newRow[FuelConsumptionMapReader.Fields.EngineSpeed] = engineSpeed;
					newRow[FuelConsumptionMapReader.Fields.Torque] = torque;
					newRow[FuelConsumptionMapReader.Fields.FuelConsumption] = fc;
					result.Rows.Add(newRow);
				}

				return result;
			}

			private static double DenormalizeFC(string fc, double mRated)
			{
				return fc.ToDouble() * mRated;
			}

			private static double DenormalizeTorque(string torque, double mRated)
			{
				return torque.ToDouble() * mRated;
			}

			private static double DenormalizeEngineSpeed(string engineSpeed, double nIdle, double nRated)
			{
				return engineSpeed.ToDouble() * (nRated - nIdle) + nIdle;
			}
			
			#endregion

			#region Create Axlegear Data
			public static AxleGearData CreateAxlegearData(IAxleGearInputData axlegearData)
			{
				var axleGear = new AxleGearData
				{
					LineType = axlegearData.LineType,
					InputData = axlegearData
				};

				var ratio = axlegearData.Ratio;

				var outputLossMap = CreateAxlegearOutputLossMap(ratio);
				var inputLossMap = CalculateAxleInputLossMap(outputLossMap, ratio);

				var transmissionData = new TransmissionData
				{
					Ratio = axlegearData.Ratio,
					LossMap = TransmissionLossMapReader.Create(inputLossMap, ratio, "Axlegear")
				};
					
				axleGear.AxleGear = transmissionData;

				return axleGear;
			}
			
			private static DataTable CreateAxlegearOutputLossMap(double axleRatio)
			{
				var torques = new [] {
					Constants.GenericLossMapSettings.OutputTorqueEnd * -1.0,
					Constants.GenericLossMapSettings.OutputTorqueStart *-1.0,
					Constants.GenericLossMapSettings.OutputTorqueStart,
					Constants.GenericLossMapSettings.OutputTorqueEnd
				};

				var outStart = Constants.GenericLossMapSettings.OutputSpeedStart;
				var outEnd = Constants.GenericLossMapSettings.OutputSpeedEnd;

				var outputSpeeds = new [] {
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

				for (int i = 0; i < 12; i++) {
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

			private static double CalculateOutputTorqueLoss(double td0_, double td150_, double td_n, 
				double outputspeed, double ouputTorque, double efficiency)
			{
				if (ouputTorque < 0)
					ouputTorque = ouputTorque * -1.0;
				
				return td0_+ td150_ * outputspeed/td_n + ouputTorque/ efficiency - ouputTorque;
			}

			private static DataTable CalculateAxleInputLossMap(DataTable outputLossMap, double axleRatio)
			{
				var inputLossMap = new DataTable();

				inputLossMap.Columns.Add(TransmissionLossMapReader.Fields.InputSpeed);
				inputLossMap.Columns.Add(TransmissionLossMapReader.Fields.InputTorque);
				inputLossMap.Columns.Add(TransmissionLossMapReader.Fields.TorqeLoss);

				foreach (DataRow row in outputLossMap.Rows) {
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
			private static double GetInputSpeed(double outputSpeed, double iAxle)
			{
				return outputSpeed * iAxle;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			private static double GetInputTorque(double outputTorque, double outputLoss, double iAxle)
			{
				return (outputTorque + outputLoss) / iAxle;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			private static double GetInputTorqueLoss(double outputLoss, double iAxle)
			{
				return outputLoss / iAxle;
			}

			#endregion

			#region Create Angledrive Data

			public static AngledriveData CreateAngledriveData(IAngledriveInputData angledriveInputData, double axleRatio)
			{
				if (angledriveInputData.DataSource.SourceFile == null)
					return null;

				var angledriveData = new AngledriveData
				{
					Type = angledriveInputData.Type,
					InputData = angledriveInputData
				};

				var axleGearOutputLossMap = CreateAxlegearOutputLossMap(axleRatio);
				var axleGearInputLossMap = CalculateAxleInputLossMap(axleGearOutputLossMap, axleRatio);

				var transmissionAngleDrive = new TransmissionData
				{
					Ratio = angledriveInputData.Ratio,
					LossMap = GetAngleDriveLossMap(axleGearInputLossMap, angledriveInputData.Ratio)
				};

				angledriveData.Angledrive = transmissionAngleDrive;

				return angledriveData;
			}

			private static TransmissionLossMap GetAngleDriveLossMap(DataTable axleGearInputTable, double ratio)
			{
				var angleDriveLossMap = new DataTable();
				angleDriveLossMap.Columns.Add(TransmissionLossMapReader.Fields.InputSpeed);
				angleDriveLossMap.Columns.Add(TransmissionLossMapReader.Fields.InputTorque);
				angleDriveLossMap.Columns.Add(TransmissionLossMapReader.Fields.TorqeLoss);

				var angleDriveFactor = Constants.GenericLossMapSettings.FactorAngleDrive;
				foreach (DataRow row in axleGearInputTable.Rows) {
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

			#endregion

			#region Create Gearbox Data

			public static GearboxData CreateGearboxData(IVehicleDeclarationInputData pifVehicle, VectoRunData runData,
				IShiftPolygonCalculator shiftPolygonCalc)
			{
				return DeclarationDataAdapterHeavyLorry.DoCreateGearboxData(pifVehicle, runData, shiftPolygonCalc);
				//var gearbox = new GearboxData
				//{
				//	Inertia = 0.SI<KilogramSquareMeter>(),
				//	TractionInterruption =
				//		GearBoxTypeHelper.TractionInterruption(pifVehicle.Components.GearboxInputData.Type),
				//	Gears = GetGearData(pifVehicle.Components.GearboxInputData,
				//		pifVehicle.TorqueLimits.ToDictionary(e => e.Gear), fullLoadMaxTorque)
				//};

				//return gearbox;
			}

			
			#endregion

			# region SSMInputs Methods 

			public static void SetBoundaryConditions(SSMInputs input)
			{
				input.GFactor = Constants.BusAuxiliaries.SteadyStateModel.GFactor;
				input.HeatingBoundaryTemperature = Constants.BusAuxiliaries.SteadyStateModel.HeatingBoundaryTemperature;
				input.CoolingBoundaryTemperature = Constants.BusAuxiliaries.SteadyStateModel.CoolingBoundaryTemperature;
				input.SpecificVentilationPower = Constants.BusAuxiliaries.SteadyStateModel.SpecificVentilationPower;
				input.AuxHeaterEfficiency = Constants.BusAuxiliaries.SteadyStateModel.AuxHeaterEfficiency;
				input.MaxPossibleBenefitFromTechnologyList =
					Constants.BusAuxiliaries.SteadyStateModel.MaxPossibleBenefitFromTechnologyList;
			}

			public static void SetEnvironmentalConditions(SSMInputs input)
			{
				input.DefaultConditions = new EnvironmentalConditionMapEntry(
					Constants.BusAuxiliaries.SteadyStateModel.DefaultTemperature,
					Constants.BusAuxiliaries.SteadyStateModel.DefaultSolar, 1.0);
				input.EnvironmentalConditionsMap = DeclarationData.BusAuxiliaries.DefaultEnvironmentalConditions;
			}

			#endregion

			#region Create Retarder

			public static RetarderData CreateRetarderData(IRetarderInputData retarderInput)
			{
				if (retarderInput == null)
					return null;

				var retarder = new RetarderData
				{
					Ratio = retarderInput.Ratio,
					Type = retarderInput.Type,
					LossMap = GenerateGenericLossMap(retarderInput.Ratio)
				};

				return retarder;
			}

			private static RetarderLossMap GenerateGenericLossMap(double stepUpRatio)
			{
				var retarderSpeeds = new double [] {
					0, 200 , 400, 600, 900, 1200,
					1600, 2000, 2500, 3000, 3500, 4000,
					4500, 5000
				};

				var genericRetarderLosses = GetHydrodynamicRetardersLoss(retarderSpeeds, stepUpRatio);
				//var genericRetarderLosses = GetMagneticRetarderLoss(retarderSpeeds, stepUpRatio);

				var torqueLoss = new DataTable();
				torqueLoss.Columns.Add(RetarderLossMapReader.Fields.RetarderSpeed);
				torqueLoss.Columns.Add(RetarderLossMapReader.Fields.TorqueLoss);
				
				for (int i = 0; i < genericRetarderLosses.Length; i++) {
					var newRow = torqueLoss.NewRow();
					newRow[RetarderLossMapReader.Fields.RetarderSpeed] = retarderSpeeds[i];
					newRow[RetarderLossMapReader.Fields.TorqueLoss] = genericRetarderLosses[i] * 
						Constants.GenericLossMapSettings.RetarderGenericFactor;
					torqueLoss.Rows.Add(newRow);
				}

				return RetarderLossMapReader.Create(torqueLoss);
			}
			
			private static double[] GetHydrodynamicRetardersLoss(double[] retarderSpeeds, double stepUpRatio)
			{
				var losses = new double[retarderSpeeds.Length];

				for (int i = 0; i < retarderSpeeds.Length; i++) {
					losses[i] = 10.0 / stepUpRatio + 2.0 / Math.Pow(stepUpRatio, 3) * Math.Pow(retarderSpeeds[i] / 1000.0, 2);
				}

				return losses;
			}

			private static double[] GetMagneticRetarderLoss(double[] retarderSpeeds, double stepUpRatio)
			{
				var losses = new double[retarderSpeeds.Length];

				for (int i = 0; i < retarderSpeeds.Length; i++) {
					losses[i] = 15.0 / stepUpRatio + 2.0 / Math.Pow(stepUpRatio, 4) * Math.Pow(retarderSpeeds[i] / 1000.0, 3);
				}

				return losses;
			}

			#endregion

			#region Common Getters

			public static Meter GetDynamicTyreRadius(IVehicleDeclarationInputData primaryVehicle)
			{
				var axleWheels = primaryVehicle.Components.AxleWheels.AxlesDeclaration;
				Meter dynamicTyreRadius = null;

				for (int i = 0; i < axleWheels.Count; i++)
				{
					if (axleWheels[i].AxleType == AxleType.VehicleDriven)
					{
						dynamicTyreRadius = DeclarationData.Wheels.Lookup(axleWheels[i].Tyre.Dimension.RemoveWhitespace()).DynamicTyreRadius;
						break;
					}
				}

				return dynamicTyreRadius;
			}

			#endregion

			#region Create Driver Data

			public static DriverData CreateDriverData(Segment completedSegment)
			{
				var lookAheadData = new DriverData.LACData
				{
					Enabled = Driver.LookAhead.Enabled,
					//Deceleration = DeclarationData.Driver.LookAhead.Deceleration,
					MinSpeed = Driver.LookAhead.MinimumSpeed,
					LookAheadDecisionFactor = new LACDecisionFactor(),
					LookAheadDistanceFactor = Driver.LookAhead.LookAheadDistanceFactor
				};

				var overspeedData = new DriverData.OverSpeedData
				{
					Enabled = true,
					MinSpeed = Driver.OverSpeed.MinSpeed,
					OverSpeed = Driver.OverSpeed.AllowedOverSpeed,
				};

				var driver = new DriverData
				{
					AccelerationCurve = AccelerationCurveReader.ReadFromStream(completedSegment.AccelerationFile),
					LookAheadCoasting = lookAheadData,
					OverSpeed = overspeedData,
					EngineStopStart = new DriverData.EngineStopStartData
					{
						EngineOffStandStillActivationDelay = Driver.EngineStopStart.ActivationDelay,
						MaxEngineOffTimespan = Driver.EngineStopStart.MaxEngineOffTimespan,
						UtilityFactor = Driver.EngineStopStart.UtilityFactor
					},
					EcoRoll = new DriverData.EcoRollData
					{
						UnderspeedThreshold = Driver.EcoRoll.UnderspeedThreshold,
						MinSpeed = Driver.EcoRoll.MinSpeed,
						ActivationPhaseDuration = Driver.EcoRoll.ActivationDelay,
						AccelerationLowerLimit = Driver.EcoRoll.AccelerationLowerLimit,
						AccelerationUpperLimit = Driver.EcoRoll.AccelerationUpperLimit
					},
					PCC = new DriverData.PCCData
					{
						PCCEnableSpeed = Driver.PCC.PCCEnableSpeed,
						MinSpeed = Driver.PCC.MinSpeed,
						PreviewDistanceUseCase1 = Driver.PCC.PreviewDistanceUseCase1,
						PreviewDistanceUseCase2 = Driver.PCC.PreviewDistanceUseCase2,
						UnderSpeed = Driver.PCC.Underspeed,
						OverspeedUseCase3 = Driver.PCC.OverspeedUseCase3
					}
				};


				return driver;
			}

			#endregion

		}





		public static class BusAuxiliaries
		{
			//private static ISSMInputs ssmInputs = null;

			private static IEnvironmentalConditionsMap envMap = null;

			//private static AuxiliaryConfig busAuxConfig = null;
			private static ElectricalConsumerList elUserConfig;

			private static IActuationsMap actuationsMap;
			//private static PneumaticsAuxilliariesConfig pneumaticAuxConfig;
			private static List<SSMTechnology> ssmTechnologies;


			//public static ISSMInputs SSMDefaultValues
			//{
			//	get {
			//		return ssmInputs ?? (ssmInputs = SSMInputData.ReadStream(
			//					RessourceHelper.ReadStream(DeclarationDataResourcePrefix + ".Buses.SSMDefaults.AHSM"),
			//					DefaultEnvironmentalConditions));
			//	}
			//}

			public static BusAlternatorTechnologies AlternatorTechnologies = new BusAlternatorTechnologies();
			private static HVACCoolingPower hvacMaxCoolingPower;

			public static List<SSMTechnology> SSMTechnologyList
			{
				get {
					return ssmTechnologies ?? (ssmTechnologies = SSMTechnologiesReader.ReadFromStream(
								RessourceHelper.ReadStream(DeclarationDataResourcePrefix + ".Buses.SSMTechList.csv")));
				}
			}

			public static IEnvironmentalConditionsMap DefaultEnvironmentalConditions
			{
				get {
					return envMap ?? (envMap = EnvironmentalContidionsMapReader.ReadStream(
								RessourceHelper.ReadStream(DeclarationDataResourcePrefix + ".Buses.DefaultClimatic.aenv")));
				}
			}

			public static ElectricalConsumerList DefaultElectricConsumerList
			{
				get {
					return elUserConfig ?? (elUserConfig = ElectricConsumerReader.ReadStream(
								RessourceHelper.ReadStream(DeclarationDataResourcePrefix + ".Buses.ElectricConsumers.csv")));
				}
			}


			public static IActuationsMap ActuationsMap
			{
				get {
					return actuationsMap ?? (actuationsMap = ActuationsMapReader.ReadStream(
									RessourceHelper.ReadStream(DeclarationDataResourcePrefix + ".Buses.DefaultActuationsMap.apac")));
				}
			}

			public static HVACCoolingPower HVACMaxCoolingPower
			{
				get { return hvacMaxCoolingPower ?? (hvacMaxCoolingPower = new HVACCoolingPower()); }
			}

			public static PerSecond VentilationRate(BusHVACSystemConfiguration hvacSystemConfig, bool heating)
			{

				switch (hvacSystemConfig) {
					
					case BusHVACSystemConfiguration.Configuration1: 
					case BusHVACSystemConfiguration.Configuration2:
						return Constants.BusAuxiliaries.SteadyStateModel.LowVentilation;
						
					case BusHVACSystemConfiguration.Configuration3: 
					case BusHVACSystemConfiguration.Configuration4: 
					case BusHVACSystemConfiguration.Configuration5: 
					case BusHVACSystemConfiguration.Configuration6: 
					case BusHVACSystemConfiguration.Configuration7: 
					case BusHVACSystemConfiguration.Configuration8: 
					case BusHVACSystemConfiguration.Configuration9:
						return heating
							? Constants.BusAuxiliaries.SteadyStateModel.HighVentilationHeating
							: Constants.BusAuxiliaries.SteadyStateModel.HighVentilation;
						
					default: throw new ArgumentOutOfRangeException(nameof(hvacSystemConfig), hvacSystemConfig, null);
				}
			}

			public static SquareMeter CalculateBusFloorSurfaceArea(Meter busLength, Meter busWidth)
			{
				return (busLength - Constants.BusParameters.DriverCompartmentLength) * CorrectedBusWidth(busWidth);
			}

			public static Meter CorrectedBusWidth(Meter busWidth)
			{
				return busWidth.IsBetween(Constants.BusParameters.VehicleWidthLow, Constants.BusParameters.VehicleWidthHigh)
					? Constants.BusParameters.VehicleWidthHigh
					: busWidth;
			}
			 

			public static Meter CalculateInternalLength(Meter vehicleLength, bool doubleDecker, FloorType floorType, double numPassLowFloor)
				{
				if (floorType == FloorType.LowFloor) {
					return doubleDecker ? 2 * vehicleLength : vehicleLength;
				}

				if (floorType == FloorType.HighFloor) {
					if (doubleDecker) {
						return numPassLowFloor > 6 ? 1.5 * vehicleLength : vehicleLength + 2.4.SI<Meter>();
					}

					return vehicleLength;
				}
				throw new VectoException("Internal Length for floorType {0} {1} not defined", floorType.ToString(), doubleDecker ? "DD" : "SD");
			}

			public static Meter CalculateLengthInteriorLights(
				Meter vehicleLength, bool doubleDecker, FloorType floorType, double numPassLowFloor)
			{
				return CalculateInternalLength(vehicleLength, doubleDecker, floorType, numPassLowFloor);
			}

			public static Meter CalculateInternalHeight(FloorType floorType, bool doubleDecker, Meter vehicleHeight)
			{
				if (doubleDecker) {
					return Constants.BusParameters.InternalHeightDoubleDecker;
				}

				switch (floorType) {
					case FloorType.LowFloor:
						return vehicleHeight;
					case FloorType.HighFloor:
						return vehicleHeight - Constants.BusParameters.HeightLuggageCompartment;
				}

				throw new VectoException("Internal height for vehicle floor type '{0}' {1} not defined", floorType.ToString(), doubleDecker ? "double decker" : "single decker");
			}

			public static Meter WindowHeight(bool doubleDecker)
			{
				return doubleDecker
					? Constants.BusParameters.WindowHeightDoubleDecker
					: Constants.BusParameters.WindowHeightSingleDecker;
			}

			public static SquareMeter FrontAndRearWindowArea(bool doubleDecker)
			{
				return doubleDecker
					? Constants.BusParameters.FrontAndRearWindowAreaDoubleDecker
					: Constants.BusParameters.FrontAndRearWindowAreaSingleDecker;
			}

			public static WattPerKelvinSquareMeter UValue(FloorType floorType)
			{
				switch (floorType) {
					case FloorType.LowFloor:
						return 4.SI<WattPerKelvinSquareMeter>();
					case FloorType.SemiLowFloor:
						return 3.5.SI<WattPerKelvinSquareMeter>();
					case FloorType.HighFloor:
						return 3.SI<WattPerKelvinSquareMeter>();
					default: throw new ArgumentOutOfRangeException(nameof(floorType), floorType, null);
				}
			}

			public static double CalculateCOP(Watt coolingPwrDriver, ACCompressorType comprTypeDriver, Watt coolingPwrPass, ACCompressorType comprTypePass, FloorType floorType)
			{
				if (coolingPwrDriver.IsGreater(0) && comprTypeDriver == ACCompressorType.None) {
					comprTypeDriver = comprTypePass;
				}
				return (coolingPwrDriver * comprTypeDriver.COP(floorType) + coolingPwrPass * comprTypePass.COP(floorType)) /
						(coolingPwrDriver + coolingPwrPass);
			}
		}

		public static class Driver
		{
			public static class LookAhead
			{
				public const bool Enabled = true;

				public const double DecisionFactorCoastingOffset = 2.5;
				public const double DecisionFactorCoastingScaling = 1.5;
				public const double LookAheadDistanceFactor = 10;
				public static readonly MeterPerSecond MinimumSpeed = 50.KMPHtoMeterPerSecond();
			}

			public static class OverSpeed
			{
				public static readonly MeterPerSecond MinSpeed = 50.KMPHtoMeterPerSecond();
				public static readonly MeterPerSecond AllowedOverSpeed = 2.5.KMPHtoMeterPerSecond();
			}

			public static class EngineStopStart
			{
				public static readonly Second ActivationDelay = 2.SI<Second>();
				public static readonly Second MaxEngineOffTimespan = 120.SI<Second>();
				public const double UtilityFactor = 0.8;
			}

			public static class EcoRoll
			{
				public static readonly MeterPerSecond MinSpeed = 60.KMPHtoMeterPerSecond();
				public static readonly Second ActivationDelay = 2.SI<Second>();
				public static readonly MeterPerSecond UnderspeedThreshold = 0.KMPHtoMeterPerSecond();

				public static readonly MeterPerSquareSecond AccelerationLowerLimit = 0.SI<MeterPerSquareSecond>();
				public static readonly MeterPerSquareSecond AccelerationUpperLimit = 0.1.SI<MeterPerSquareSecond>();
			}

			public static class PCC
			{
				public static readonly MeterPerSecond PCCEnableSpeed = 80.KMPHtoMeterPerSecond();
				public static readonly MeterPerSecond MinSpeed = 50.KMPHtoMeterPerSecond();
				public static readonly Meter PreviewDistanceUseCase1 = 1500.SI<Meter>();
				public static readonly Meter PreviewDistanceUseCase2 = 1000.SI<Meter>();
				public static readonly MeterPerSecond Underspeed = 8.KMPHtoMeterPerSecond();
				public static readonly MeterPerSecond OverspeedUseCase3 = 5.KMPHtoMeterPerSecond();
			}
		}

		public static class Trailer
		{
			public static readonly double RollResistanceCoefficient = 0.0055;
			public const double TyreTestLoad = 37500;

			public const bool TwinTyres = false;

			//public const string WheelsType = "385/65 R 22.5";
		}

		public static class Engine
		{
			public static readonly KilogramSquareMeter ClutchInertia = 1.3.SI<KilogramSquareMeter>();
			public static readonly KilogramSquareMeter TorqueConverterInertia = 1.2.SI<KilogramSquareMeter>();

			public static readonly KilogramSquareMeter EngineBaseInertia = 0.41.SI<KilogramSquareMeter>();
			public static readonly SI EngineDisplacementInertia = (0.27 * 1000).SI(Unit.SI.Kilo.Gramm.Per.Meter); // [kg/m]
			public static readonly Second DefaultEngineStartTime = 1.SI<Second>();

			public const double TorqueLimitGearboxFactor = 0.9;
			public const double TorqueLimitVehicleFactor = 0.95;

			public static KilogramSquareMeter EngineInertia(CubicMeter displacement, GearboxType gbxType)
			{
				// VB Code:    Return 1.3 + 0.41 + 0.27 * (Displ / 1000)
				return (gbxType.AutomaticTransmission() ? TorqueConverterInertia : ClutchInertia) + EngineBaseInertia +
						EngineDisplacementInertia * displacement;
			}
		}

		public static class GearboxTCU
		{
			public const double TorqueReserve = 0;
			public const double TorqueReserveStart = 0.2;

			public static readonly MeterPerSecond StartSpeed = 8.KMPHtoMeterPerSecond();
			public static readonly MeterPerSquareSecond StartAcceleration = 0.8.SI<MeterPerSquareSecond>();

			public static readonly Second GearResidenceTime = 5.SI<Second>();
			public static readonly Watt CurrentCardanPowerThresholdPropulsion = 5000.SI<Watt>();
			public static readonly Watt AverageCardanPowerThresholdPropulsion = 1000.SI<Watt>();
			public static readonly Second LookBackInterval = 4.SI<Second>();
			public static readonly Second DriverAccelerationLookBackInterval = 2.SI<Second>();
			public const double EngineSpeedHighDriveOffFactor = 1.05;
			public const double DnT99L_highMin1 = 0.4;
			public const double DnT99L_highMin2 = 0.5;

			public const int AllowedGearRangeUp = 3;
			public const int AllowedGearRangeDown = 3;

			public const double TargetSpeedDeviationFactor = 0.1;

			public static double RatingFactorCurrentGear = 0.97;
			public static double RatingFactorCurrentGearAT = 0.97;

			public static readonly MeterPerSquareSecond DriverAccelerationThresholdLow = 0.1.SI<MeterPerSquareSecond>();
			public static double VelocityDropFactor = 1.0;
			public static double AccelerationFactor = 0.5;

			public static double RatioEarlyUpshiftFC = 24;
			public static double RatioEarlyDownshiftFC = 24;

			public static int AllowedGearRangeFCAMT = 2;
			public static int AllowedGearRangeFCAT = 1;
			public static int AllowedGearRangeFCATSkipGear = 2;
			public static int ATSkipGearsThreshold = 6;

			public static PerSecond MinEngineSpeedPostUpshift = 0.RPMtoRad();

			public static Second ATLookAheadTime = Gearbox.PowershiftShiftTime;

			public static double[] LoadStageThresholdsUp = { 19.7, 36.34, 53.01, 69.68, 86.35 };
			public static double[] LoadStageThresoldsDown = { 13.7, 30.34, 47.01, 63.68, 80.35 };

			public static double[][] ShiftSpeedsTCToLocked = {
				new[] {  90.0, 120, 165,  90, 120, 165 },
				new[] {  90.0, 120, 165,  90, 120, 165 },
				new[] {  90.0, 120, 165,  90, 120, 165 },
				new[] {  90.0, 120, 165, 110, 140, 185 },
				new[] { 100.0, 130, 175, 120, 150, 195 },
				new[] { 110.0, 140, 185, 130, 160, 205 },
			};

			public const double DownhillSlope = -5;
			public const double UphillSlope = 5;

			public static string DefaultShiftStrategy = "";
			public const double DragMarginFactor = 0.7;


			// TODO: MQ 2019-11-26 remove, once the parameters are fixed! make fields above read-only or const
			static GearboxTCU()
			{
				//#if RELEASE_CANDIDATE
				var expectedFile = @"Declaration\EffShiftParameters.vtcu";
				if (!File.Exists(expectedFile))
				{
					return;
				}

				var tcuData = JSONInputDataFactory.ReadShiftParameters(expectedFile, true);
				if (tcuData.RatingFactorCurrentGear.HasValue)
				{
					RatingFactorCurrentGear = tcuData.RatingFactorCurrentGear.Value;
					RatingFactorCurrentGearAT = tcuData.RatingFactorCurrentGear.Value;
				}
				if (tcuData.RatioEarlyDownshiftFC.HasValue)
				{
					RatioEarlyDownshiftFC = tcuData.RatioEarlyDownshiftFC.Value;
				}
				if (tcuData.RatioEarlyUpshiftFC.HasValue)
				{
					RatioEarlyUpshiftFC = tcuData.RatioEarlyUpshiftFC.Value;
				}
				if (tcuData.AllowedGearRangeFC.HasValue)
				{
					AllowedGearRangeFCAMT = tcuData.AllowedGearRangeFC.Value;
					AllowedGearRangeFCAT = tcuData.AllowedGearRangeFC.Value;
				}
				if (tcuData.VeloictyDropFactor.HasValue)
				{
					VelocityDropFactor = tcuData.VeloictyDropFactor.Value;
				}
				if (tcuData.AccelerationFactor.HasValue)
				{
					AccelerationFactor = tcuData.AccelerationFactor.Value;
				}
				if (tcuData.ATLookAheadTime != null)
				{
					ATLookAheadTime = tcuData.ATLookAheadTime;
				}
				if (tcuData.LoadStageThresholdsDown != null && LoadStageThresoldsDown.Length > 0)
				{
					LoadStageThresoldsDown = tcuData.LoadStageThresholdsDown.ToArray();
				}
				if (tcuData.LoadStageThresholdsUp != null && LoadStageThresholdsUp.Length > 0)
				{
					LoadStageThresholdsUp = tcuData.LoadStageThresholdsUp.ToArray();
				}
				if (tcuData.ShiftSpeedsTCToLocked != null && ShiftSpeedsTCToLocked.Length > 0)
				{
					ShiftSpeedsTCToLocked = tcuData.ShiftSpeedsTCToLocked;
				}
				if (tcuData.MinEngineSpeedPostUpshift != null)
				{
					MinEngineSpeedPostUpshift = tcuData.MinEngineSpeedPostUpshift;
				}
				var tmp = tcuData as JSONFile;
				if (tmp != null && tmp.Body["ShiftStrategy"] != null)
				{
					DefaultShiftStrategy = tmp.Body["ShiftStrategy"].Value<string>();
				}
				//#endif
			}
		}

		public static class Gearbox
		{
			public static readonly KilogramSquareMeter Inertia = 0.SI<KilogramSquareMeter>();

			public static readonly MeterPerSecond TruckMaxAllowedSpeed = 85.KMPHtoMeterPerSecond();
			public const double ShiftPolygonRPMMargin = 7; // %
			private const double ShiftPolygonEngineFldMargin = 0.98;

			public static readonly Second MinTimeBetweenGearshifts = 2.SI<Second>();
			public static readonly Second DownshiftAfterUpshiftDelay = 6.SI<Second>();
			public static readonly Second UpshiftAfterDownshiftDelay = 6.SI<Second>();

			public static readonly MeterPerSquareSecond UpshiftMinAcceleration = 0.1.SI<MeterPerSquareSecond>();

			//public static readonly PerSecond TorqueConverterSpeedLimit = 1600.RPMtoRad();
			public static double TorqueConverterSecondGearThreshold(VehicleCategory category)
			{
				return category.IsTruck() ? 1.8 : 1.85;
			}

			public static readonly Second PowershiftShiftTime = 0.8.SI<Second>();

			/// <summary>
			/// computes the shift polygons for a single gear according to the whitebook 2016
			/// </summary>
			/// <param name="type"></param>
			/// <param name="gearIdx">index of the gear to compute the shift polygons for  -- gear number - 1!</param>
			/// <param name="fullLoadCurve">engine full load curve, potentially limited by the gearbox</param>
			/// <param name="gears">list of gears</param>
			/// <param name="engine">engine data</param>
			/// <param name="axlegearRatio"></param>
			/// <param name="dynamicTyreRadius"></param>
			/// <returns></returns>
			public static ShiftPolygon ComputeShiftPolygon(
				GearboxType type, int gearIdx, EngineFullLoadCurve fullLoadCurve,
				IList<ITransmissionInputData> gears, CombustionEngineData engine, double axlegearRatio, Meter dynamicTyreRadius)
			{
				switch (type)
				{
					case GearboxType.AMT:

					//return ComputeEfficiencyShiftPolygon(gearIdx, fullLoadCurve, gears, engine, axlegearRatio, dynamicTyreRadius);
					case GearboxType.MT:
						return ComputeManualTransmissionShiftPolygon(
							gearIdx, fullLoadCurve, gears, engine, axlegearRatio, dynamicTyreRadius);
					case GearboxType.ATSerial:
					case GearboxType.ATPowerSplit:
						return TorqueConverter.ComputeShiftPolygon(fullLoadCurve, gearIdx == 0, gearIdx >= gears.Count - 1);
					case GearboxType.DrivingCycle: break;
					default: throw new ArgumentOutOfRangeException(nameof(type), type, null);
				}

				return type.AutomaticTransmission()
					? TorqueConverter.ComputeShiftPolygon(fullLoadCurve, gearIdx == 0, gearIdx >= gears.Count - 1)

					// That's the same for all gears, so call the same method...
					: ComputeManualTransmissionShiftPolygon(gearIdx, fullLoadCurve, gears, engine, axlegearRatio, dynamicTyreRadius);
			}

			public static ShiftPolygon ComputeEfficiencyShiftPolygon(
				int gearIdx, EngineFullLoadCurve fullLoadCurve, IList<ITransmissionInputData> gears, CombustionEngineData engine,
				double axlegearRatio, Meter dynamicTyreRadius)
			{
				if (gears.Count < 2)
				{
					throw new VectoException("ComputeShiftPolygon needs at least 2 gears. {0} gears given.", gears.Count);
				}

				var p2 = new Point(engine.IdleSpeed.Value() * 1.1, 0);
				var p3 = new Point(fullLoadCurve.NTq99lSpeed.Value(), 0);
				var p5 = new Point(fullLoadCurve.NP98hSpeed.Value(), fullLoadCurve.MaxTorque.Value() * 1.1);

				var downShift = new List<ShiftPolygon.ShiftPolygonEntry>();

				if (gearIdx > 0)
				{
					var downShiftPoints = fullLoadCurve
						.FullLoadEntries.Where(fldEntry => fldEntry.EngineSpeed >= p2.X && fldEntry.EngineSpeed <= p3.X)
						.Select(
							fldEntry =>
								new Point(fldEntry.EngineSpeed.Value(), fldEntry.TorqueFullLoad.Value() * ShiftPolygonEngineFldMargin))
						.ToList();
					downShift.Add(new ShiftPolygon.ShiftPolygonEntry(fullLoadCurve.MaxDragTorque * 1.1, p2.X.SI<PerSecond>()));
					if (downShiftPoints.Count == 0)
					{
						// coarse grid points in FLD
						downShift.Add(
							new ShiftPolygon.ShiftPolygonEntry(
								fullLoadCurve.FullLoadStationaryTorque(p2.X.SI<PerSecond>()) * ShiftPolygonEngineFldMargin,
								p2.X.SI<PerSecond>()));
						downShift.Add(
							new ShiftPolygon.ShiftPolygonEntry(
								fullLoadCurve.FullLoadStationaryTorque(p3.X.SI<PerSecond>()) * ShiftPolygonEngineFldMargin,
								p3.X.SI<PerSecond>()));
					}
					else
					{
						if (downShiftPoints.Min(x => x.X) > p2.X)
						{
							downShift.Add(
								new ShiftPolygon.ShiftPolygonEntry(
									fullLoadCurve.FullLoadStationaryTorque(p2.X.SI<PerSecond>()) * ShiftPolygonEngineFldMargin,
									p2.X.SI<PerSecond>()));
						}

						downShift.AddRange(
							downShiftPoints.Select(
								x => new ShiftPolygon.ShiftPolygonEntry(
									x.Y.SI<NewtonMeter>() * ShiftPolygonEngineFldMargin, x.X.SI<PerSecond>())));
						if (downShiftPoints.Max(x => x.X) < p3.X)
						{
							downShift.Add(
								new ShiftPolygon.ShiftPolygonEntry(
									fullLoadCurve.FullLoadStationaryTorque(p3.X.SI<PerSecond>()) * ShiftPolygonEngineFldMargin,
									p3.X.SI<PerSecond>()));
						}
					}
					downShift.Add(new ShiftPolygon.ShiftPolygonEntry(fullLoadCurve.MaxTorque * 1.1, p3.X.SI<PerSecond>()));
				}
				var upShift = new List<ShiftPolygon.ShiftPolygonEntry>();
				if (gearIdx >= gears.Count - 1)
				{
					return new ShiftPolygon(downShift, upShift);
				}

				upShift.Add(new ShiftPolygon.ShiftPolygonEntry(fullLoadCurve.MaxDragTorque * 1.1, p5.X.SI<PerSecond>()));
				upShift.Add(new ShiftPolygon.ShiftPolygonEntry(p5.Y.SI<NewtonMeter>(), p5.X.SI<PerSecond>()));
				return new ShiftPolygon(downShift, upShift);
			}


			public static ShiftPolygon ComputeManualTransmissionShiftPolygon(
				int gearIdx, EngineFullLoadCurve fullLoadCurve,
				IList<ITransmissionInputData> gears, CombustionEngineData engine, double axlegearRatio, Meter dynamicTyreRadius)
			{
				if (gears.Count < 2)
				{
					throw new VectoException("ComputeShiftPolygon needs at least 2 gears. {0} gears given.", gears.Count);
				}

				// ReSharper disable once InconsistentNaming
				var engineSpeed85kmhLastGear = ComputeEngineSpeed85kmh(gears[gears.Count - 1], axlegearRatio, dynamicTyreRadius);

				var nVHigh = VectoMath.Min(engineSpeed85kmhLastGear, engine.FullLoadCurves[0].RatedSpeed);

				var diffRatio = gears[gears.Count - 2].Ratio / gears[gears.Count - 1].Ratio - 1;

				var maxDragTorque = fullLoadCurve.MaxDragTorque * 1.1;

				var p1 = new Point(engine.IdleSpeed.Value() / 2, 0);
				var p2 = new Point(engine.IdleSpeed.Value() * 1.1, 0);
				var p3 = new Point(
					nVHigh.Value() * 0.9,
					fullLoadCurve.FullLoadStationaryTorque(nVHigh * 0.9).Value());

				var p4 = new Point((nVHigh * (1 + diffRatio / 3)).Value(), 0);
				var p5 = new Point(fullLoadCurve.N95hSpeed.Value(), fullLoadCurve.MaxTorque.Value());

				var p6 = new Point(p2.X, VectoMath.Interpolate(p1, p3, p2.X));
				var p7 = new Point(p4.X, VectoMath.Interpolate(p2, p5, p4.X));

				var fldMargin = ShiftPolygonFldMargin(fullLoadCurve.FullLoadEntries, (p3.X * 0.95).SI<PerSecond>());
				var downshiftCorr = MoveDownshiftBelowFld(Edge.Create(p6, p3), fldMargin, 1.1 * fullLoadCurve.MaxTorque);

				var downShift = new List<ShiftPolygon.ShiftPolygonEntry>();
				if (gearIdx > 0)
				{
					downShift =
						new[] { p2, downshiftCorr.P1, downshiftCorr.P2 }.Select(
																			point => new ShiftPolygon.ShiftPolygonEntry(point.Y.SI<NewtonMeter>(), point.X.SI<PerSecond>()))
																		.ToList();

					downShift[0].Torque = maxDragTorque;
				}
				var upShift = new List<ShiftPolygon.ShiftPolygonEntry>();
				if (gearIdx >= gears.Count - 1)
				{
					return new ShiftPolygon(downShift, upShift);
				}

				var gearRatio = gears[gearIdx].Ratio / gears[gearIdx + 1].Ratio;
				var rpmMarginFactor = 1 + ShiftPolygonRPMMargin / 100.0;

				// ReSharper disable InconsistentNaming
				var p2p = new Point(p2.X * gearRatio * rpmMarginFactor, p2.Y / gearRatio);
				var p3p = new Point(p3.X * gearRatio * rpmMarginFactor, p3.Y / gearRatio);
				var p6p = new Point(p6.X * gearRatio * rpmMarginFactor, p6.Y / gearRatio);
				var edgeP6pP3p = new Edge(p6p, p3p);
				var p3pExt = new Point((1.1 * p5.Y - edgeP6pP3p.OffsetXY) / edgeP6pP3p.SlopeXY, 1.1 * p5.Y);

				// ReSharper restore InconsistentNaming

				var upShiftPts = IntersectTakeHigherShiftLine(new[] { p4, p7, p5 }, new[] { p2p, p6p, p3pExt });
				if (gears[gearIdx].MaxInputSpeed != null)
				{
					var maxSpeed = gears[gearIdx].MaxInputSpeed.Value();
					upShiftPts = IntersectTakeLowerShiftLine(
						upShiftPts,
						new[] { new Point(maxSpeed, 0), new Point(maxSpeed, upShiftPts.Max(pt => pt.Y)) });
				}
				upShift =
					upShiftPts.Select(point => new ShiftPolygon.ShiftPolygonEntry(point.Y.SI<NewtonMeter>(), point.X.SI<PerSecond>()))
							.ToList();
				upShift[0].Torque = maxDragTorque;
				return new ShiftPolygon(downShift, upShift);
			}

			/// <summary>
			/// ensures the original downshift line is below the (already reduced) full-load curve
			/// </summary>
			/// <param name="shiftLine">second part of the shift polygon (slope)</param>
			/// <param name="fldMargin">reduced full-load curve</param>
			/// <param name="maxTorque">max torque</param>
			/// <returns>returns a corrected shift polygon segment (slope) that is below the full load curve and reaches given maxTorque. the returned segment has the same slope</returns>
			internal static Edge MoveDownshiftBelowFld(Edge shiftLine, IEnumerable<Point> fldMargin, NewtonMeter maxTorque)
			{
				var slope = shiftLine.SlopeXY;
				var d = shiftLine.P2.Y - slope * shiftLine.P2.X;

				d = fldMargin.Select(point => point.Y - slope * point.X).Concat(new[] { d }).Min();
				var p6Corr = new Point(shiftLine.P1.X, shiftLine.P1.X * slope + d);
				var p3Corr = new Point((maxTorque.Value() - d) / slope, maxTorque.Value());
				return Edge.Create(p6Corr, p3Corr);
			}

			/// <summary>
			/// reduce the torque of the full load curve up to the given rpms
			/// </summary>
			/// <param name="fullLoadCurve"></param>
			/// <param name="rpmLimit"></param>
			/// <returns></returns>
			internal static IEnumerable<Point> ShiftPolygonFldMargin(
				List<EngineFullLoadCurve.FullLoadCurveEntry> fullLoadCurve,
				PerSecond rpmLimit)
			{
				return fullLoadCurve.TakeWhile(fldEntry => fldEntry.EngineSpeed < rpmLimit)
									.Select(
										fldEntry =>
											new Point(fldEntry.EngineSpeed.Value(), fldEntry.TorqueFullLoad.Value() * ShiftPolygonEngineFldMargin))
									.ToList();
			}

			// ReSharper disable once InconsistentNaming
			private static PerSecond ComputeEngineSpeed85kmh(
				ITransmissionInputData gear, double axleRatio,
				Meter dynamicTyreRadius)
			{
				var engineSpeed = TruckMaxAllowedSpeed / dynamicTyreRadius * axleRatio * gear.Ratio;
				return engineSpeed;
			}

			internal static Point[] IntersectTakeHigherShiftLine(Point[] orig, Point[] transformedDownshift)
			{
				var intersections = Intersect(orig, transformedDownshift);

				// add all points (i.e. intersecting points and both line segments) to a single list
				var pointSet = new List<Point>(orig);
				pointSet.AddRange(transformedDownshift);
				pointSet.AddRange(intersections);
				pointSet.AddRange(ProjectPointsToLineSegments(orig, transformedDownshift));
				pointSet.AddRange(ProjectPointsToLineSegments(transformedDownshift, orig));

				// line sweeping from max_X to 0: select point with lowest Y coordinate, abort if a point has Y = 0
				var shiftPolygon = new List<Point>();
				foreach (var xCoord in pointSet.Select(pt => pt.X).Distinct().OrderBy(x => x).Reverse())
				{
					var coord = xCoord;
					var xPoints = pointSet.Where(pt => pt.X.IsEqual(coord) && !pt.Y.IsEqual(0)).ToList();
					shiftPolygon.Add(xPoints.MinBy(pt => pt.Y));
					var tmp = pointSet.Where(pt => pt.X.IsEqual(coord)).Where(pt => pt.Y.IsEqual(0)).ToList();
					if (!tmp.Any())
					{
						continue;
					}

					shiftPolygon.Add(tmp.First());
					break;
				}

				// find and remove colinear points
				var toRemove = new List<Point>();
				for (var i = 0; i < shiftPolygon.Count - 2; i++)
				{
					var edge = new Edge(shiftPolygon[i], shiftPolygon[i + 2]);
					if (edge.ContainsXY(shiftPolygon[i + 1]))
					{
						toRemove.Add(shiftPolygon[i + 1]);
					}
				}
				foreach (var point in toRemove)
				{
					shiftPolygon.Remove(point);
				}

				// order points first by x coordinate and the by Y coordinate ASC
				return shiftPolygon.OrderBy(pt => pt.X).ThenBy(pt => pt.Y).ToArray();
			}

			internal static Point[] IntersectTakeLowerShiftLine(Point[] upShiftPts, Point[] upperLimit)
			{
				var intersections = Intersect(upShiftPts, upperLimit);
				if (!intersections.Any())
				{
					return upShiftPts[0].X < upperLimit[0].X ? upShiftPts : upperLimit;
				}

				var pointSet = new List<Point>(upShiftPts);
				pointSet.AddRange(upperLimit);
				pointSet.AddRange(intersections);
				pointSet.AddRange(ProjectPointsToLineSegments(upShiftPts, upperLimit, true));
				pointSet.AddRange(ProjectPointsToLineSegments(upperLimit, upShiftPts, true));

				var shiftPolygon = new List<Point>();
				foreach (var yCoord in pointSet.Select(pt => pt.Y).Distinct().OrderBy(y => y).Reverse())
				{
					var yPoints = pointSet.Where(pt => pt.Y.IsEqual(yCoord)).ToList();
					shiftPolygon.Add(yPoints.MinBy(pt => pt.X));
				}

				// find and remove colinear points
				var toRemove = new List<Point>();
				for (var i = 0; i < shiftPolygon.Count - 2; i++)
				{
					var edge = new Edge(shiftPolygon[i], shiftPolygon[i + 2]);
					if (edge.ContainsXY(shiftPolygon[i + 1]))
					{
						toRemove.Add(shiftPolygon[i + 1]);
					}
				}
				foreach (var point in toRemove)
				{
					shiftPolygon.Remove(point);
				}

				// order points first by x coordinate and the by Y coordinate ASC
				return shiftPolygon.OrderBy(pt => pt.X).ThenBy(pt => pt.Y).ToArray();
			}

			private static Point[] Intersect(Point[] orig, Point[] transformedDownshift)
			{
				var intersections = new List<Point>();

				// compute all intersection points between both line segments
				// ReSharper disable once LoopCanBeConvertedToQuery
				foreach (var origLine in orig.Pairwise(Edge.Create))
				{
					// ReSharper disable once LoopCanBeConvertedToQuery
					foreach (var transformedLine in transformedDownshift.Pairwise(Edge.Create))
					{
						var isect = VectoMath.Intersect(origLine, transformedLine);
						if (isect != null)
						{
							intersections.Add(isect);
						}
					}
				}

				return intersections.ToArray();
			}

			private static IEnumerable<Point> ProjectPointsToLineSegments(
				IEnumerable<Point> lineSegments, Point[] points,
				bool projectToVertical = false)
			{
				var pointSet = new List<Point>();
				foreach (var segment in lineSegments.Pairwise(Edge.Create))
				{
					if (segment.P1.X.IsEqual(segment.P2.X))
					{
						if (projectToVertical)
						{
							pointSet.AddRange(
								points.Select(point => new Point(segment.P1.X, point.Y))
									.Where(pt => pt.Y.IsBetween(segment.P1.Y, segment.P2.Y)));
						}
						continue;
					}

					var k = segment.SlopeXY;
					var d = segment.P1.Y - segment.P1.X * k;
					pointSet.AddRange(points.Select(point => new Point(point.X, point.X * k + d)));
				}

				return pointSet;
			}
		}

		public static class TorqueConverter
		{
			public static readonly PerSecond ReferenceRPM = 1000.RPMtoRad();
			public static readonly PerSecond MaxInputSpeed = 5000.RPMtoRad();
			public static readonly MeterPerSquareSecond CLUpshiftMinAcceleration = 0.1.SI<MeterPerSquareSecond>();
			public static readonly MeterPerSquareSecond CCUpshiftMinAcceleration = 0.1.SI<MeterPerSquareSecond>();

			private static readonly PerSecond DownshiftPRM = 700.RPMtoRad();
			private static readonly PerSecond UpshiftLowRPM = 900.RPMtoRad();
			private static readonly PerSecond UpshiftHighRPM = 1150.RPMtoRad();

			public static ShiftPolygon ComputeShiftPolygon(
				EngineFullLoadCurve fullLoadCurve, bool first = false,
				bool last = false)
			{
				var maxDragTorque = fullLoadCurve.MaxDragTorque * 1.1;
				var maxTorque = fullLoadCurve.MaxTorque * 1.1;
				var p0 = new Point(UpshiftLowRPM.Value(), maxDragTorque.Value());
				var p1 = new Point(UpshiftLowRPM.Value(), 0);
				var p2 = new Point(UpshiftHighRPM.Value(), fullLoadCurve.FullLoadStationaryTorque(UpshiftHighRPM).Value());
				var edge = new Edge(p1, p2);
				var p2corr = new Point((maxTorque.Value() - edge.OffsetXY) / edge.SlopeXY, maxTorque.Value());

				var downshift = new[] {
					new ShiftPolygon.ShiftPolygonEntry(maxDragTorque, DownshiftPRM),
					new ShiftPolygon.ShiftPolygonEntry(maxTorque, DownshiftPRM)
				};
				var upshift = new[] { p0, p1, p2corr }.Select(
					pt => new ShiftPolygon.ShiftPolygonEntry(pt.Y.SI<NewtonMeter>(), pt.X.SI<PerSecond>()));

				return new ShiftPolygon(
					first ? new List<ShiftPolygon.ShiftPolygonEntry>() : downshift.ToList(),
					last ? new List<ShiftPolygon.ShiftPolygonEntry>() : upshift.ToList());
			}

			public static IEnumerable<TorqueConverterEntry> GetTorqueConverterDragCurve(double ratio, TorqueConverterEntry first, TorqueConverterEntry last)
			{
				var characteristicTorque = new[] {
					new TorqueConverterEntry() {
						SpeedRatio = 1,
						TorqueRatio = last.TorqueRatio * 0.99,
						Torque = 0.SI<NewtonMeter>()
					},
					new TorqueConverterEntry(
					) {
						SpeedRatio = 5,
						TorqueRatio = 0.9,
						Torque =  -4 * first.Torque
					},
				};
				foreach (var torqueConverterEntry in characteristicTorque)
				{
					torqueConverterEntry.SpeedRatio = torqueConverterEntry.SpeedRatio * ratio;
					torqueConverterEntry.TorqueRatio = torqueConverterEntry.TorqueRatio / ratio;
				}

				return characteristicTorque.Where(x => x.SpeedRatio >= ratio).ToArray();
			}
		}

		public static class PTO
		{
			public const string DefaultPTOTechnology =
				"only the drive shaft of the PTO - shift claw, synchronizer, sliding gearwheel";

			public const string DefaultPTOIdleLosses =
				DeclarationDataResourcePrefix + ".MissionCycles.MunicipalUtility_PTO_generic.vptol";

			public const string DefaultPTOActivationCycle =
				DeclarationDataResourcePrefix + ".MissionCycles.MunicipalUtility_PTO_generic.vptoc";
		}

		public static class VTPMode
		{
			public static readonly Meter RunInThreshold = 15000.SI(Unit.SI.Kilo.Meter).Cast<Meter>();
			public const double EvolutionCoefficient = 0.98;

			public const MissionType SelectedMissionHeavyLorry = MissionType.LongHaul;
			public const MissionType SelectedMissionMediumLorry = MissionType.RegionalDelivery;
			public const LoadingType SelectedLoading = LoadingType.ReferenceLoad;

			// verification of input data

			public const double WheelSpeedDifferenceFactor = 1.4;
			public const double WheelTorqueDifferenceFactor = 3;

			public static readonly PerSecond WheelSpeedZeroTolerance = 0.1.RPMtoRad();
			public static readonly PerSecond MaxWheelSpeedDifferenceStandstill = 1.RPMtoRad();

			public static readonly PerSecond MinFanSpeed = 20.RPMtoRad();
			public static readonly PerSecond MaxFanSpeed = 4000.RPMtoRad();

			public static readonly Second SamplingInterval = 0.5.SI<Second>();

			public static readonly WattSecond MinPosWorkAtWheelsForFC = 1.SI(Unit.SI.Kilo.Watt.Hour).Cast<WattSecond>();

			public static readonly SpecificFuelConsumption LowerFCThreshold =
				180.SI(Unit.SI.Gramm.Per.Kilo.Watt.Hour).Cast<SpecificFuelConsumption>();

			public static readonly SpecificFuelConsumption UpperFCThreshold =
				600.SI(Unit.SI.Gramm.Per.Kilo.Watt.Hour).Cast<SpecificFuelConsumption>();

			public static readonly Second FCAccumulationWindow = 10.SI(Unit.SI.Minute).Cast<Second>();
			public static readonly double[] FanParameters = { 7.320, 1200.0, 810 };
		}

		public static class Vehicle
		{
			public const bool DualFuelVehicleDefault = false;
			public const bool HybridElectricHDVDefault = false;
			public const bool ZeroEmissionVehicleDefault = false;
			public const TankSystem TankSystemDefault = TankSystem.Compressed;
			public const bool SleeperCabDefault = true;
			public const bool VocationalVehicleDefault = false;

			public static class ADAS
			{
				public const PredictiveCruiseControlType PredictiveCruiseControlDefault = PredictiveCruiseControlType.None;
				public const EcoRollType EcoRoll = EcoRollType.None;
				public const bool EngineStopStartDefault = false;
			}
		}
	}
}
