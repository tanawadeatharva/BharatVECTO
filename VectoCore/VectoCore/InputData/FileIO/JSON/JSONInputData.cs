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

using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
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
		private string _basePath;

		protected readonly JObject Header;
		protected readonly JObject Body;

		protected JSONFile(JObject data, string filename)
		{
			Header = (JObject)data.GetEx(JsonKeys.JsonHeader);
			Body = (JObject)data.GetEx(JsonKeys.JsonBody);
			BasePath = filename;
		}

		public bool SavedInDeclarationMode
		{
			get { return Body.GetEx(JsonKeys.SavedInDeclMode).Value<bool>(); }
		}

		internal string BasePath
		{
			get { return _basePath; }
			set { _basePath = Path.GetDirectoryName(Path.GetFullPath(value)); }
		}

		protected DataTable ReadTableData(string filename, string tableType, bool required = true)
		{
			if (!EmptyOrInvalidFileName(filename)) {
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
	}

	/// <summary>
	/// Class for reading json data of vecto-job-file.
	/// Fileformat: .vecto
	/// </summary>
	public class JSONInputDataV2 : JSONFile, IEngineeringInputDataProvider, IDeclarationInputDataProvider,
		IEngineeringJobInputData, IDriverEngineeringInputData, IAuxiliariesEngineeringInputData,
		IAuxiliariesDeclarationInputData
	{
		protected readonly IGearboxEngineeringInputData Gearbox;
		protected readonly IAxleGearInputData AxleGear;
		protected readonly ITorqueConverterEngineeringInputData TorqueConverter;
		protected readonly IAngularGearInputData AngularGear;
		protected readonly IEngineEngineeringInputData Engine;
		protected readonly IVehicleEngineeringInputData VehicleData;
		protected readonly IRetarderInputData Retarder;

		private readonly string _jobname;

		public JSONInputDataV2(JObject data, string filename) : base(data, filename)
		{
			_jobname = Path.GetFileNameWithoutExtension(filename);
			try {
				var gearboxFile = Body.GetEx(JsonKeys.Vehicle_GearboxFile).Value<string>();
				if (!EmptyOrInvalidFileName(gearboxFile)) {
					Gearbox = JSONInputDataFactory.ReadGearbox(Path.Combine(BasePath, gearboxFile));
				}
				AxleGear = Gearbox as IAxleGearInputData;
				TorqueConverter = Gearbox as ITorqueConverterEngineeringInputData;
			} catch (Exception e) {
				throw new VectoException("JobFile: Failed to read Gearbox file '{0}': {1}", e, Body[JsonKeys.Vehicle_GearboxFile],
					e.Message);
			}

			try {
				Engine = JSONInputDataFactory.ReadEngine(
					Path.Combine(BasePath, Body.GetEx(JsonKeys.Vehicle_EngineFile).Value<string>()));
			} catch (Exception e) {
				throw new VectoException("JobFile: Failed to read Engine file '{0}': {1}", e, Body[JsonKeys.Vehicle_EngineFile],
					e.Message);
			}

			try {
				var vehicleFile = Body.GetEx(JsonKeys.Vehicle_VehicleFile).Value<string>();
				if (!EmptyOrInvalidFileName(vehicleFile)) {
					VehicleData = JSONInputDataFactory.ReadJsonVehicle(
						Path.Combine(BasePath, vehicleFile));

					AngularGear = VehicleData as IAngularGearInputData;
				}
			} catch (Exception e) {
				throw new VectoException("JobFile: Failed to read Vehicle file '{0}': {1}", e, Body[JsonKeys.Vehicle_VehicleFile],
					e.Message);
			}
			var retarder = VehicleData as IRetarderInputData;
			if (retarder != null) {
				Retarder = retarder;
			}
		}

		#region IInputDataProvider

		public virtual IEngineeringJobInputData JobInputData()
		{
			return this;
		}

		IVehicleDeclarationInputData IDeclarationInputDataProvider.VehicleInputData
		{
			get { return VehicleInputData; }
		}

		IGearboxDeclarationInputData IDeclarationInputDataProvider.GearboxInputData
		{
			get { return GearboxInputData; }
		}

		ITorqueConverterDeclarationInputData IDeclarationInputDataProvider.TorqueConverterInputData
		{
			get { return TorqueConverterInputData; }
		}

		public ITorqueConverterEngineeringInputData TorqueConverterInputData
		{
			get
			{
				if (TorqueConverter == null) {
					throw new InvalidFileFormatException("TorqueConverterData not found");
				}
				return TorqueConverter;
			}
		}

		IDeclarationJobInputData IDeclarationInputDataProvider.JobInputData()
		{
			return JobInputData();
		}

		public virtual IVehicleEngineeringInputData VehicleInputData
		{
			[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design",
				"CA1065:DoNotRaiseExceptionsInUnexpectedLocations")]
			get
			{
				if (VehicleData == null) {
					throw new InvalidFileFormatException("VehicleData not found ");
				}
				return VehicleData;
			}
		}

		public virtual IGearboxEngineeringInputData GearboxInputData
		{
			[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design",
				"CA1065:DoNotRaiseExceptionsInUnexpectedLocations")]
			get
			{
				if (Gearbox == null) {
					throw new InvalidFileFormatException("GearboxData not found");
				}
				return Gearbox;
			}
		}

		public virtual IAxleGearInputData AxleGearInputData
		{
			[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design",
				"CA1065:DoNotRaiseExceptionsInUnexpectedLocations")]
			get
			{
				if (AxleGear == null) {
					throw new InvalidFileFormatException("AxleGearData not found");
				}
				return AxleGear;
			}
		}

		public IAngularGearInputData AngularGearInputData
		{
			get { return AngularGear; }
		}

		IEngineDeclarationInputData IDeclarationInputDataProvider.EngineInputData
		{
			get { return EngineInputData; }
		}

		public virtual IEngineEngineeringInputData EngineInputData
		{
			[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design",
				"CA1065:DoNotRaiseExceptionsInUnexpectedLocations")]
			get
			{
				if (Engine == null) {
					throw new InvalidFileFormatException("EngineData not found");
				}
				return Engine;
			}
		}

		public virtual IAuxiliariesEngineeringInputData AuxiliaryInputData()
		{
			return this;
		}

		IDriverEngineeringInputData IEngineeringInputDataProvider.DriverInputData
		{
			get { return this; }
		}

		IAuxiliariesDeclarationInputData IDeclarationInputDataProvider.AuxiliaryInputData()
		{
			return this;
		}

		public virtual IRetarderInputData RetarderInputData
		{
			[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design",
				"CA1065:DoNotRaiseExceptionsInUnexpectedLocations")]
			get
			{
				if (Retarder == null) {
					throw new InvalidFileFormatException("RetarderData not found");
				}
				return Retarder;
			}
		}

		public virtual IDriverDeclarationInputData DriverInputData
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
			get
			{
				var retVal = new List<ICycleData>();
				foreach (var cycle in Body.GetEx(JsonKeys.Job_Cycles)) {
					//.Select(cycle => 
					var cycleFile = Path.Combine(BasePath, cycle.Value<string>());
					DataTable cycleData;
					if (File.Exists(cycleFile)) {
						cycleData = VectoCSVFile.Read(cycleFile);
					} else {
						try {
							var cycleDataRes =
								RessourceHelper.ReadStream(RessourceHelper.Namespace + "MissionCycles." + cycle.Value<string>() + ".vdri");
							cycleData = VectoCSVFile.ReadStream(cycleDataRes);
						} catch {
							Log.Debug("Driving Cycle could not be read: " + cycleFile);
							throw new VectoException("Driving Cycle could not be read: " + cycleFile);
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

		IVehicleDeclarationInputData IDeclarationJobInputData.Vehicle
		{
			get { return Vehicle; }
		}

		public virtual string JobName
		{
			get { return _jobname; }
		}

		#endregion

		#region DriverInputData

		public virtual IStartStopEngineeringInputData StartStop
		{
			get
			{
				var startStop = Body.GetEx(JsonKeys.DriverData_StartStop);
				return new StartStopInputData {
					Enabled = startStop.GetEx<bool>(JsonKeys.DriverData_StartStop_Enabled),
					Delay = startStop.GetEx<double>(JsonKeys.DriverData_StartStop_Delay).SI<Second>(),
					MaxSpeed = startStop.GetEx<double>(JsonKeys.DriverData_StartStop_MaxSpeed).KMPHtoMeterPerSecond(),
					MinTime = startStop.GetEx<double>(JsonKeys.DriverData_StartStop_MinTime).SI<Second>(),
				};
			}
		}

		IOverSpeedEcoRollDeclarationInputData IDriverDeclarationInputData.OverSpeedEcoRoll
		{
			get
			{
				var overspeed = Body.GetEx(JsonKeys.DriverData_OverspeedEcoRoll);
				return new OverSpeedEcoRollInputData() {
					Mode = DriverData.ParseDriverMode(overspeed.GetEx<string>(JsonKeys.DriverData_OverspeedEcoRoll_Mode))
				};
			}
		}

		public virtual ILookaheadCoastingInputData Lookahead
		{
			get
			{
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
				var speedDependentLookup = lac["DF_targetSpeedLookup"] != null
					? ReadTableData(lac.GetEx<string>("DF_targetSpeedLookup"), "Lookahead Coasting Decisionfactor - Target speed",
						false)
					: null;
				var velocityDropLookup = lac["Df_velocityDropLookup"] != null
					? ReadTableData(lac.GetEx<string>("Df_velocityDropLookup"),
						"Lookahead Coasting Decisionfactor - Velocity drop", false)
					: null;
				return new LookAheadCoastingInputData() {
					Enabled = lac.GetEx<bool>(JsonKeys.DriverData_Lookahead_Enabled),
					//Deceleration = lac.GetEx<double>(JsonKeys.DriverData_Lookahead_Deceleration).SI<MeterPerSquareSecond>(),
					//MinSpeed = lac.GetEx<double>(JsonKeys.DriverData_Lookahead_MinSpeed).KMPHtoMeterPerSecond(),
					LookaheadDistanceFactor = distanceScalingFactor,
					CoastingDecisionFactorOffset = lacDfOffset,
					CoastingDecisionFactorScaling = lacDfScaling,
					CoastingDecisionFactorTargetSpeedLookup = speedDependentLookup,
					CoastingDecisionFactorVelocityDropLookup = velocityDropLookup
				};
			}
		}

		IStartStopDeclarationInputData IDriverDeclarationInputData.StartStop
		{
			get { return StartStop; }
		}

		public virtual IOverSpeedEcoRollEngineeringInputData OverSpeedEcoRoll
		{
			get
			{
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

		public virtual DataTable AccelerationCurve
		{
			[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design",
				"CA1065:DoNotRaiseExceptionsInUnexpectedLocations")]
			get
			{
				var acceleration = Body[JsonKeys.DriverData_AccelerationCurve];
				if (acceleration == null || EmptyOrInvalidFileName(acceleration.Value<string>())) {
					throw new VectoException("AccelerationCurve (VACC) required");
				}
				try {
					return ReadTableData(acceleration.Value<string>(), "DriverAccelerationCurve", false);
				} catch (VectoException e) {
					Log.Warn("Could not find file for acceleration curve. Trying lookup in declaration data.");
					try {
						var cycleDataRes = RessourceHelper.ReadStream(RessourceHelper.Namespace + "VACC." + acceleration.Value<string>() +
																	Constants.FileExtensions.DriverAccelerationCurve);
						return VectoCSVFile.ReadStream(cycleDataRes);
					} catch (Exception) {
						throw new VectoException("Failed to read Driver Acceleration Curve: " + e.Message, e);
					}
				}
			}
		}

		#endregion

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
					switch (tech) {
						case "Crankshaft mounted - Electronically controlled visco clutch (Default)":
							auxData.Technology.Add("Crankshaft mounted - Electronically controlled visco clutch");
							break;
						case "Crankshaft mounted - On/Off clutch":
							auxData.Technology.Add("Crankshaft mounted - On/off clutch");
							break;
						case "Belt driven or driven via transm. - On/Off clutch":
							auxData.Technology.Add("Belt driven or driven via transm. - On/off clutch");
							break;
						default:
							auxData.Technology.Add(tech);
							break;
					}
				}

				var auxFile = aux["Path"];
				retVal.Add(auxData);

				if (auxFile == null || EmptyOrInvalidFileName(auxFile.Value<string>())) {
					continue;
				}
				var stream = new StreamReader(Path.Combine(BasePath, auxFile.Value<string>()));
				stream.ReadLine(); // skip header "Transmission ration to engine rpm [-]"
				auxData.TransmissionRatio = stream.ReadLine().IndulgentParse();
				stream.ReadLine(); // skip header "Efficiency to engine [-]"
				auxData.EfficiencyToEngine = stream.ReadLine().IndulgentParse();
				stream.ReadLine(); // skip header "Efficiency auxiliary to supply [-]"
				auxData.EfficiencyToSupply = stream.ReadLine().IndulgentParse();
				auxData.DemandMap = VectoCSVFile.ReadStream(new MemoryStream(Encoding.UTF8.GetBytes(stream.ReadToEnd())));
			}
			return retVal;
		}

		#region AdvancedAuxiliaries

		public AuxiliaryModel AuxiliaryAssembly
		{
			get
			{
				return AuxiliaryModelHelper.Parse(Body["AuxiliaryAssembly"] == null ? "" : Body["AuxiliaryAssembly"].ToString());
			}
		}

		public string AuxiliaryVersion
		{
			get { return Body["AuxiliaryVersion"] != null ? Body["AuxiliaryVersion"].Value<string>() : "<CLASSIC>"; }
		}

		public string AdvancedAuxiliaryFilePath
		{
			get
			{
				return Body["AdvancedAuxiliaryFilePath"] != null
					? Path.Combine(Path.GetFullPath(BasePath), Body["AdvancedAuxiliaryFilePath"].Value<string>())
					: "";
			}
		}

		#endregion
	}

	public class JSONInputDataV3 : JSONInputDataV2
	{
		public JSONInputDataV3(JObject data, string filename) : base(data, filename) {}

		protected override IList<AuxiliaryDataInputData> AuxData()
		{
			var retVal = new List<AuxiliaryDataInputData>();
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
				var stream = new StreamReader(Path.Combine(BasePath, auxFile.Value<string>()));
				stream.ReadLine(); // skip header "Transmission ration to engine rpm [-]"
				auxData.TransmissionRatio = stream.ReadLine().IndulgentParse();
				stream.ReadLine(); // skip header "Efficiency to engine [-]"
				auxData.EfficiencyToEngine = stream.ReadLine().IndulgentParse();
				stream.ReadLine(); // skip header "Efficiency auxiliary to supply [-]"
				auxData.EfficiencyToSupply = stream.ReadLine().IndulgentParse();
				auxData.DemandMap = VectoCSVFile.ReadStream(new MemoryStream(Encoding.UTF8.GetBytes(stream.ReadToEnd())));
			}
			return retVal;
		}
	}
}