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

using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.InputData.Impl;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.JSON
{
	public abstract class JSONFile : LoggingObject
	{
		public const string MissingFileSuffix = "   -- (MISSING!)";

		private readonly string _sourceFile;

		protected readonly JObject Body;

		protected JSONFile(JObject data, string filename, bool tolerateMissing = false)
		{
			//var header = (JObject)data.GetEx(JsonKeys.JsonHeader);
			Body = (JObject)data.GetEx(JsonKeys.JsonBody);
			_sourceFile = Path.GetFullPath(filename);
			TolerateMissing = tolerateMissing;
		}

		protected bool TolerateMissing { get; set; }

		public DataSourceType SourceType
		{
			get { return DataSourceType.JSONFile; }
		}

		public string Source
		{
			get { return _sourceFile; }
		}

		public bool SavedInDeclarationMode
		{
			get { return Body.GetEx(JsonKeys.SavedInDeclMode).Value<bool>(); }
		}

		internal string BasePath
		{
			get { return Path.GetDirectoryName(_sourceFile); }
		}

		protected TableData ReadTableData(string filename, string tableType, bool required = true)
		{
			if (!EmptyOrInvalidFileName(filename) && File.Exists(Path.Combine(BasePath, filename))) {
				try {
					return VectoCSVFile.Read(Path.Combine(BasePath, filename), true);
				} catch (Exception e) {
					Log.Warn("Failed to read file {0} {1}", Path.Combine(BasePath, filename), tableType);
					throw new VectoException("Failed to read file for {0}: {1}", e, tableType, filename);
				}
			}
			if (required) {
				throw new VectoException("Invalid filename for {0}: {1}", tableType, filename);
			}
			return null;
		}

		internal static bool EmptyOrInvalidFileName(string filename)
		{
			return filename == null || !filename.Any() ||
					filename.Equals("<NOFILE>", StringComparison.InvariantCultureIgnoreCase)
					|| filename.Equals("-");
		}

		public static JObject GetDummyJSONStructure()
		{
			return JObject.FromObject(new Dictionary<string, object>() {
				{ JsonKeys.JsonHeader, new object() },
				{ JsonKeys.JsonBody, new object() }
			});
		}
	}

	/// <summary>
	/// Class for reading json data of vecto-job-file.
	/// Fileformat: .vecto
	/// </summary>
	public class JSONInputDataV2 : JSONFile, IEngineeringInputDataProvider, IDeclarationInputDataProvider,
		IEngineeringJobInputData, IDriverEngineeringInputData, IAuxiliariesEngineeringInputData,
		IAuxiliariesDeclarationInputData
	{
		public IGearboxEngineeringInputData Gearbox { get; internal set; }
		public IAxleGearInputData AxleGear { get; internal set; }
		public ITorqueConverterEngineeringInputData TorqueConverter { get; internal set; }
		public IEngineEngineeringInputData Engine { get; internal set; }


		protected readonly IVehicleEngineeringInputData VehicleData;

		private readonly string _jobname;


		public JSONInputDataV2(JObject data, string filename, bool tolerateMissing = false)
			: base(data, filename, tolerateMissing)
		{
			_jobname = Path.GetFileNameWithoutExtension(filename);

			Engine = ReadEngine();

			if (Body.GetEx(JsonKeys.Job_EngineOnlyMode).Value<bool>()) {
				return;
			}

			Gearbox = ReadGearbox();
			AxleGear = Gearbox as IAxleGearInputData;
			TorqueConverter = Gearbox as ITorqueConverterEngineeringInputData;

			VehicleData = ReadVehicle();
		}

		private IVehicleEngineeringInputData ReadVehicle()
		{
			try {
				var vehicleFile = Body.GetEx(JsonKeys.Vehicle_VehicleFile).Value<string>();
				return JSONInputDataFactory.ReadJsonVehicle(
					Path.Combine(BasePath, vehicleFile), this);
			} catch (Exception e) {
				if (!TolerateMissing) {
					throw new VectoException("JobFile: Failed to read Vehicle file '{0}': {1}", e, Body[JsonKeys.Vehicle_VehicleFile],
						e.Message);
				}
				return new JSONVehicleDataV7(GetDummyJSONStructure(),
					Path.Combine(BasePath, Body.GetEx(JsonKeys.Vehicle_VehicleFile).Value<string>()) + MissingFileSuffix, this);
			}
		}

		private IGearboxEngineeringInputData ReadGearbox()
		{
			try {
				var gearboxFile = Body.GetEx(JsonKeys.Vehicle_GearboxFile).Value<string>();

				return JSONInputDataFactory.ReadGearbox(Path.Combine(BasePath, gearboxFile));
			} catch (Exception e) {
				if (!TolerateMissing) {
					throw new VectoException("JobFile: Failed to read Gearbox file '{0}': {1}", e, Body[JsonKeys.Vehicle_GearboxFile],
						e.Message);
				}
				return new JSONGearboxDataV6(GetDummyJSONStructure(),
					Path.Combine(BasePath, Body.GetEx(JsonKeys.Vehicle_GearboxFile).Value<string>()) + MissingFileSuffix);
			}
		}

		private IEngineEngineeringInputData ReadEngine()
		{
			try {
				return JSONInputDataFactory.ReadEngine(
					Path.Combine(BasePath, Body.GetEx(JsonKeys.Vehicle_EngineFile).Value<string>()));
			} catch (Exception e) {
				if (!TolerateMissing) {
					throw new VectoException("JobFile: Failed to read Engine file '{0}': {1}", e, Body[JsonKeys.Vehicle_EngineFile],
						e.Message);
				}

				return
					new JSONEngineDataV3(GetDummyJSONStructure(),
						Path.Combine(BasePath, Body.GetEx(JsonKeys.Vehicle_EngineFile).Value<string>()) + MissingFileSuffix);
			}
		}

		#region IInputDataProvider

		IVehicleDeclarationInputData IDeclarationJobInputData.Vehicle
		{
			get { return VehicleInputData; }
		}

		public virtual IEngineeringJobInputData JobInputData
		{
			get { return this; }
		}

		public XElement XMLHash
		{
			get { return new XElement(XMLNames.DI_Signature); }
		}

		IDeclarationJobInputData IDeclarationInputDataProvider.JobInputData
		{
			get { return this; }
		}

		public virtual IVehicleEngineeringInputData VehicleInputData
		{
			[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design",
				"CA1065:DoNotRaiseExceptionsInUnexpectedLocations")]
			get {
				if (VehicleData == null) {
					throw new InvalidFileFormatException("VehicleData not found ");
				}
				return VehicleData;
			}
		}

		public virtual IEngineEngineeringInputData EngineOnly
		{
			[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design",
				"CA1065:DoNotRaiseExceptionsInUnexpectedLocations")]
			get {
				if (Engine == null) {
					throw new InvalidFileFormatException("EngineData not found");
				}
				return Engine;
			}
		}

		IDriverEngineeringInputData IEngineeringInputDataProvider.DriverInputData
		{
			get { return this; }
		}

		#endregion

		#region IJobInputData

		public virtual IVehicleEngineeringInputData Vehicle
		{
			get { return VehicleData; }
		}

		public virtual IList<ICycleData> Cycles
		{
			[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design",
				"CA1065:DoNotRaiseExceptionsInUnexpectedLocations")]
			get {
				var retVal = new List<ICycleData>();
				if (Body[JsonKeys.Job_Cycles] == null) {
					return retVal;
				}
				foreach (var cycle in Body.GetEx(JsonKeys.Job_Cycles)) {
					//.Select(cycle => 
					var cycleFile = Path.Combine(BasePath, cycle.Value<string>());
					TableData cycleData;
					if (File.Exists(cycleFile)) {
						cycleData = VectoCSVFile.Read(cycleFile);
					} else {
						try {
							var resourceName = DeclarationData.DeclarationDataResourcePrefix + ".MissionCycles." +
												cycle.Value<string>() + Constants.FileExtensions.CycleFile;
							cycleData = VectoCSVFile.ReadStream(RessourceHelper.ReadStream(resourceName), source: resourceName);
						} catch (Exception e) {
							Log.Debug("Driving Cycle could not be read: " + cycleFile);
							if (!TolerateMissing) {
								throw new VectoException("Driving Cycle could not be read: " + cycleFile, e);
							}
							cycleData = new TableData(cycleFile + MissingFileSuffix, DataSourceType.Missing);
						}
					}
					retVal.Add(new CycleInputData() {
						Name = Path.GetFileNameWithoutExtension(cycle.Value<string>()),
						CycleData = cycleData
					});
				}
				return retVal;
			}
		}

		public virtual bool EngineOnlyMode
		{
			get { return Body.GetEx(JsonKeys.Job_EngineOnlyMode).Value<bool>(); }
		}

		public virtual string JobName
		{
			get { return _jobname; }
		}

		#endregion

		#region DriverInputData

		public virtual ILookaheadCoastingInputData Lookahead
		{
			get {
				if (Body[JsonKeys.DriverData_LookaheadCoasting] == null) {
					return null;
				}

				var lac = Body.GetEx(JsonKeys.DriverData_LookaheadCoasting);
				var distanceScalingFactor = lac["PreviewDistanceFactor"] != null
					? lac.GetEx<double>("PreviewDistanceFactor")
					: DeclarationData.Driver.LookAhead.LookAheadDistanceFactor;
				var lacDfOffset = lac["DF_offset"] != null
					? lac.GetEx<double>("DF_offset")
					: DeclarationData.Driver.LookAhead.DecisionFactorCoastingOffset;
				var lacDfScaling = lac["DF_scaling"] != null
					? lac.GetEx<double>("DF_scaling")
					: DeclarationData.Driver.LookAhead.DecisionFactorCoastingScaling;
				var speedDependentLookup = GetSpeedDependentLookupTable(lac);
				var velocityDropLookup = GetVelocityDropLookupTable(lac);
				var minSpeed = lac["MinSpeed"] != null
					? lac.GetEx<double>(JsonKeys.DriverData_Lookahead_MinSpeed).KMPHtoMeterPerSecond()
					: DeclarationData.Driver.LookAhead.MinimumSpeed;
				return new LookAheadCoastingInputData() {
					Enabled = lac.GetEx<bool>(JsonKeys.DriverData_Lookahead_Enabled),
					//Deceleration = lac.GetEx<double>(JsonKeys.DriverData_Lookahead_Deceleration).SI<MeterPerSquareSecond>(),
					MinSpeed = minSpeed,
					LookaheadDistanceFactor = distanceScalingFactor,
					CoastingDecisionFactorOffset = lacDfOffset,
					CoastingDecisionFactorScaling = lacDfScaling,
					CoastingDecisionFactorTargetSpeedLookup = speedDependentLookup,
					CoastingDecisionFactorVelocityDropLookup = velocityDropLookup
				};
			}
		}

		private TableData GetVelocityDropLookupTable(JToken lac)
		{
			if (lac["Df_velocityDropLookup"] == null || string.IsNullOrWhiteSpace(lac["Df_velocityDropLookup"].Value<string>())) {
				return null;
			}
			try {
				return ReadTableData(lac.GetEx<string>("Df_velocityDropLookup"),
					"Lookahead Coasting Decisionfactor - Velocity drop");
			} catch (Exception) {
				if (TolerateMissing) {
					return
						new TableData(Path.Combine(BasePath, lac["Df_velocityDropLookup"].Value<string>()) + MissingFileSuffix,
							DataSourceType.Missing);
				}
			}
			return null;
		}

		private TableData GetSpeedDependentLookupTable(JToken lac)
		{
			if (lac["DF_targetSpeedLookup"] == null || string.IsNullOrWhiteSpace(lac["DF_targetSpeedLookup"].Value<string>())) {
				return null;
			}
			try {
				return ReadTableData(lac.GetEx<string>("DF_targetSpeedLookup"),
					"Lookahead Coasting Decisionfactor - Target speed");
			} catch (Exception) {
				if (TolerateMissing) {
					return
						new TableData(Path.Combine(BasePath, lac["DF_targetSpeedLookup"].Value<string>()) + MissingFileSuffix,
							DataSourceType.Missing);
				}
			}
			return null;
		}

		public virtual IOverSpeedEcoRollEngineeringInputData OverSpeedEcoRoll
		{
			get {
				var overspeed = Body.GetEx(JsonKeys.DriverData_OverspeedEcoRoll);
				return new OverSpeedEcoRollInputData() {
					Mode = DriverData.ParseDriverMode(overspeed.GetEx<string>(JsonKeys.DriverData_OverspeedEcoRoll_Mode)),
					MinSpeed = overspeed.GetEx<double>(JsonKeys.DriverData_OverspeedEcoRoll_MinSpeed).KMPHtoMeterPerSecond(),
					OverSpeed = overspeed.GetEx<double>(JsonKeys.DriverData_OverspeedEcoRoll_OverSpeed).KMPHtoMeterPerSecond(),
					UnderSpeed =
						overspeed.GetEx<double>(JsonKeys.DriverData_OverspeedEcoRoll_UnderSpeed).KMPHtoMeterPerSecond()
				};
			}
		}

		public virtual TableData AccelerationCurve
		{
			[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design",
				"CA1065:DoNotRaiseExceptionsInUnexpectedLocations")]
			get {
				var acceleration = Body[JsonKeys.DriverData_AccelerationCurve];
				if (acceleration == null || EmptyOrInvalidFileName(acceleration.Value<string>())) {
					return null;
					//					throw new VectoException("AccelerationCurve (VACC) required");
				}
				try {
					return ReadTableData(acceleration.Value<string>(), "DriverAccelerationCurve");
				} catch (VectoException e) {
					Log.Warn("Could not find file for acceleration curve. Trying lookup in declaration data.");
					try {
						var resourceName = DeclarationData.DeclarationDataResourcePrefix + ".VACC." +
											acceleration.Value<string>() +
											Constants.FileExtensions.DriverAccelerationCurve;
						return VectoCSVFile.ReadStream(RessourceHelper.ReadStream(resourceName), source: resourceName);
					} catch (Exception) {
						if (!TolerateMissing) {
							throw new VectoException("Failed to read Driver Acceleration Curve: " + e.Message, e);
						}
						return new TableData(Path.Combine(BasePath, acceleration.Value<string>()) + MissingFileSuffix,
							DataSourceType.Missing);
					}
				}
			}
		}

		#endregion

		#region IAuxiliariesEngineeringInputData

		IList<IAuxiliaryEngineeringInputData> IAuxiliariesEngineeringInputData.Auxiliaries
		{
			get { return AuxData().Cast<IAuxiliaryEngineeringInputData>().ToList(); }
		}

		IList<IAuxiliaryDeclarationInputData> IAuxiliariesDeclarationInputData.Auxiliaries
		{
			get { return AuxData().Cast<IAuxiliaryDeclarationInputData>().ToList(); }
		}

		protected virtual IList<AuxiliaryDataInputData> AuxData()
		{
			var retVal = new List<AuxiliaryDataInputData>();
			foreach (var aux in Body["Aux"] ?? Enumerable.Empty<JToken>()) {
				var type = AuxiliaryTypeHelper.Parse(aux.GetEx<string>("Type"));

				var auxData = new AuxiliaryDataInputData {
					ID = aux.GetEx<string>("ID"),
					Type = type,
					Technology = new List<string>(),
				};
				var tech = aux.GetEx<string>("Technology");

				if (auxData.Type == AuxiliaryType.ElectricSystem) {
					if (aux["TechList"] == null || aux["TechList"].Any()) {
						auxData.Technology.Add("Standard technology");
					} else {
						auxData.Technology.Add("Standard technology - LED headlights, all");
					}
				}

				if (auxData.Type == AuxiliaryType.SteeringPump) {
					auxData.Technology.Add(tech);
				}

				if (auxData.Type == AuxiliaryType.Fan) {
					auxData.Technology.Add(MapLegacyFanTechnologies(tech));
				}

				var auxFile = aux["Path"];
				retVal.Add(auxData);

				if (auxFile == null || EmptyOrInvalidFileName(auxFile.Value<string>())) {
					continue;
				}

				AuxiliaryFileHelper.FillAuxiliaryDataInputData(auxData, Path.Combine(BasePath, auxFile.Value<string>()));
			}
			return retVal;
		}

		private static string MapLegacyFanTechnologies(string tech)
		{
			string newTech;
			switch (tech) {
				case "Crankshaft mounted - Electronically controlled visco clutch (Default)":
					newTech = "Crankshaft mounted - Electronically controlled visco clutch";
					break;
				case "Crankshaft mounted - On/Off clutch":
					newTech = "Crankshaft mounted - On/off clutch";
					break;
				case "Belt driven or driven via transm. - On/Off clutch":
					newTech = "Belt driven or driven via transm. - On/off clutch";
					break;
				default:
					newTech = tech;
					break;
			}
			return newTech;
		}

		#endregion

		#region AdvancedAuxiliaries

		public AuxiliaryModel AuxiliaryAssembly
		{
			get {
				return AuxiliaryModelHelper.Parse(Body["AuxiliaryAssembly"] == null ? "" : Body["AuxiliaryAssembly"].ToString());
			}
		}

		public string AuxiliaryVersion
		{
			get { return Body["AuxiliaryVersion"] != null ? Body["AuxiliaryVersion"].Value<string>() : "<CLASSIC>"; }
		}

		public string AdvancedAuxiliaryFilePath
		{
			get {
				return Body["AdvancedAuxiliaryFilePath"] != null
					? Path.Combine(Path.GetFullPath(BasePath), Body["AdvancedAuxiliaryFilePath"].Value<string>())
					: "";
			}
		}

		#endregion
	}

	public class JSONInputDataV3 : JSONInputDataV2
	{
		public JSONInputDataV3(JObject data, string filename, bool tolerateMissing = false)
			: base(data, filename, tolerateMissing) {}

		protected override IList<AuxiliaryDataInputData> AuxData()
		{
			var retVal = new List<AuxiliaryDataInputData>();
			if (Body["Padd"] != null) {
				retVal.Add(new AuxiliaryDataInputData() {
					ID = "ConstantAux",
					AuxiliaryType = AuxiliaryDemandType.Constant,
					ConstantPowerDemand = Body.GetEx<double>("Padd").SI<Watt>()
				});
			}
			foreach (var aux in Body["Aux"] ?? Enumerable.Empty<JToken>()) {
				try {
					aux.GetEx("Technology").ToObject<List<string>>();
				} catch (Exception) {
					throw new VectoException(
						"Aux: Technology for aux '{0}' list could not be read. Maybe it is a single string instead of a list of strings?",
						aux.GetEx<string>("ID"));
				}

				var type = AuxiliaryTypeHelper.Parse(aux.GetEx<string>("Type"));

				var auxData = new AuxiliaryDataInputData {
					ID = aux.GetEx<string>("ID"),
					Type = type,
					Technology = aux.GetEx("Technology").ToObject<List<string>>()
				};

				var auxFile = aux["Path"];
				retVal.Add(auxData);

				if (auxFile == null || EmptyOrInvalidFileName(auxFile.Value<string>())) {
					continue;
				}
				AuxiliaryFileHelper.FillAuxiliaryDataInputData(auxData, Path.Combine(BasePath, auxFile.Value<string>()));
			}
			return retVal;
		}
	}


	public class JSONInputDataV4 : JSONInputDataV3
	{
		public JSONInputDataV4(JObject data, string filename, bool tolerateMissing = false)
			: base(data, filename, tolerateMissing) {}
	}
}
