/*
* Copyright 2015, 2016 Graz University of Technology,
* Institute of Internal Combustion Engines and Thermodynamics,
* Institute of Technical Informatics
*
* Licensed under the EUPL (the "Licence");
* You may not use this work except in compliance with the Licence.
* You may obtain a copy of the Licence at:
*
* http://ec.europa.eu/idabc/eupl
*
* Unless required by applicable law or agreed to in writing, software 
* distributed under the Licence is distributed on an "AS IS" basis,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the Licence for the specific language governing permissions and 
* limitations under the Licence.
*/

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Exceptions;
using TUGraz.VectoCore.InputData.Impl;
using TUGraz.VectoCore.Models;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.JSON
{
	/// <summary>
	/// "Header": {
	///		"CreatedBy": "Raphael Luz IVT TU-Graz (85407225-fc3f-48a8-acda-c84a05df6837)",
	///		"Date": "29.07.2015 16:59:03",
	///		"AppVersion": "2.2",
	///		"FileVersion": 7
	/// },
	/// </summary>
	public abstract class JSONFile : LoggingObject
	{
		private string _basePath;

		protected JObject Header;
		protected JObject Body;

		protected JSONFile(JObject data, string filename)
		{
			Header = (JObject)data.GetEx(JsonKeys.JsonHeader);
			Body = (JObject)data.GetEx(JsonKeys.JsonBody);
			BasePath = filename;
		}

		public int FileVersion
		{
			get { return Header.GetEx(JsonKeys.JsonHeader_FileVersion).Value<int>(); }
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
					if (required) {
						throw new VectoException(string.Format("Invalid {0}: {1}", tableType, filename), e);
					}
					Log.Warn("Failed to read file {0} {1}", Path.Combine(BasePath, filename), tableType);
				}
			}
			if (required) {
				throw new VectoException("Invalid {0}: {1}", tableType, filename);
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
	/// A class which represents the json data format for serializing and deserializing the Job Data files.
	/// Fileformat: .vecto
	/// </summary>
	/// <code>
	/// {
	///   "Header": {
	///     "CreatedBy": " ()",
	///     "Date": "3/4/2015 12:31:06 PM",
	///     "AppVersion": "2.0.4-beta3",
	///     "FileVersion": 2
	///   },
	///   "Body": {
	///     "SavedInDeclMode": true,
	///     "VehicleFile": "../Components/12t Delivery Truck.vveh",
	///     "EngineFile": "../Components/12t Delivery Truck.veng",
	///     "GearboxFile": "../Components/12t Delivery Truck.vgbx",
	///     "Cycles": [
	///       "Long Haul",
	///       "Regional Delivery",
	///       "Urban Delivery"
	///     ],
	///     "Aux": [
	///       {
	///         "ID": "FAN",
	///         "Type": "Fan",
	///         "Path": "<NOFILE>",
	///         "Technology": ""
	///       },
	///       {
	///         "ID": "STP",
	///         "Type": "Steering pump",
	///         "Path": "<NOFILE>",
	///         "Technology": ""
	///       },
	///       {
	///         "ID": "AC",
	///         "Type": "HVAC",
	///         "Path": "<NOFILE>",
	///         "Technology": ""
	///       },
	///       {
	///         "ID": "ES",
	///         "Type": "Electric System",
	///         "Path": "<NOFILE>",
	///         "Technology": "",
	///         "TechList": []
	///       },
	///       {
	///         "ID": "PS",
	///         "Type": "Pneumatic System",
	///         "Path": "<NOFILE>",
	///         "Technology": ""
	///       }
	///     ],
	///     "VACC": "<NOFILE>",
	///     "EngineOnlyMode": true,
	///     "StartStop": {
	///       "Enabled": false,
	///       "MaxSpeed": 5.0,
	///       "MinTime": 5.0,
	///       "Delay": 5
	///     },
	///     "LAC": {
	///       "Enabled": true,
	///       "Dec": -0.5,
	///       "MinSpeed": 50.0
	///     },
	///     "OverSpeedEcoRoll": {
	///       "Mode": "OverSpeed",
	///       "MinSpeed": 50.0,
	///       "OverSpeed": 5.0,
	///       "UnderSpeed": 5.0
	///     }
	///   }
	/// }
	/// </code>
	public class JSONInputDataV2 : JSONFile, IEngineeringInputDataProvider, IEngineeringJobInputData, IDriverInputData,
		IAuxiliariesEngineeringInputData
	{
		protected IGearboxEngineeringInputData Gearbox;

		protected IAxleGearInputData AxleGear;

		protected IEngineEngineeringInputData Engine;

		protected IVehicleEngineeringInputData VehicleData;

		protected IRetarderInputData Retarder;

		private string _jobname;

		public JSONInputDataV2(JObject data, string filename) : base(data, filename)
		{
			_jobname = Path.GetFileNameWithoutExtension(filename);
			try {
				var gearboxFile = Body.GetEx(JsonKeys.Vehicle_GearboxFile).Value<string>();
				if (!EmptyOrInvalidFileName(gearboxFile)) {
					Gearbox = JSONInputDataFactory.ReadGearbox(Path.Combine(BasePath, gearboxFile));
				}
			} catch (Exception e) {
				throw new VectoException("Failed to read Gearbox file.", e);
			}
			try {
				var axleGear = Gearbox as IAxleGearInputData;
				if (axleGear != null) {
					AxleGear = axleGear;
				}
			} catch (Exception e) {
				throw new VectoException("Failed to read AxleGear file.", e);
			}
			try {
				Engine = JSONInputDataFactory.ReadEngine(
					Path.Combine(BasePath, Body.GetEx(JsonKeys.Vehicle_EngineFile).Value<string>()));
			} catch (Exception e) {
				throw new VectoException("Failed to read Engine file.", e);
			}
			try {
				var vehicleFile = Body.GetEx(JsonKeys.Vehicle_VehicleFile).Value<string>();
				if (!EmptyOrInvalidFileName(vehicleFile)) {
					VehicleData = JSONInputDataFactory.ReadJsonVehicle(
						Path.Combine(BasePath, vehicleFile));
				}
			} catch (Exception e) {
				throw new VectoException("Failed to read Vehicle file.", e);
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

		IDeclarationJobInputData IDeclarationInputDataProvider.JobInputData()
		{
			return JobInputData();
		}

		public virtual IVehicleEngineeringInputData VehicleInputData
		{
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
			get
			{
				if (AxleGear == null) {
					throw new InvalidFileFormatException("AxleGearData not found");
				}
				return AxleGear;
			}
		}

		IEngineDeclarationInputData IDeclarationInputDataProvider.EngineInputData
		{
			get { return EngineInputData; }
		}

		public virtual IEngineEngineeringInputData EngineInputData
		{
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

		IAuxiliariesDeclarationInputData IDeclarationInputDataProvider.AuxiliaryInputData()
		{
			return AuxiliaryInputData();
		}

		public virtual IRetarderInputData RetarderInputData
		{
			get
			{
				if (Retarder == null) {
					throw new InvalidFileFormatException("RetarderData not found");
				}
				return Retarder;
			}
		}

		public virtual IDriverInputData DriverInputData
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
							// todo: log?
							cycleData = null;
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

		public virtual IStartStopInputData StartStop
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

		public virtual ILookaheadCoastingInputData Lookahead
		{
			get
			{
				var lac = Body.GetEx(JsonKeys.DriverData_LookaheadCoasting);
				return new LookAheadCoastingInputData() {
					Enabled = lac.GetEx<bool>(JsonKeys.DriverData_Lookahead_Enabled),
					Deceleration = lac.GetEx<double>(JsonKeys.DriverData_Lookahead_Deceleration).SI<MeterPerSquareSecond>(),
					MinSpeed = lac.GetEx<double>(JsonKeys.DriverData_Lookahead_MinSpeed).KMPHtoMeterPerSecond(),
				};
			}
		}

		public virtual IOverSpeedEcoRollInputData OverSpeedEcoRoll
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
			get
			{
				var acceleration = Body[JsonKeys.DriverData_AccelerationCurve];
				if (acceleration == null || EmptyOrInvalidFileName(acceleration.Value<string>())) {
					throw new VectoException("AccelerationCurve (VACC) required");
				}
				var accelerationData = ReadTableData(acceleration.Value<string>(), "DriverAccelerationCurve", false);
				if (accelerationData != null) {
					return accelerationData;
				}
				try {
					var cycleDataRes =
						RessourceHelper.ReadStream(RessourceHelper.Namespace + "VACC." + acceleration.Value<string>() +
													Constants.FileExtensions.DriverAccelerationCurve);
					accelerationData = VectoCSVFile.ReadStream(cycleDataRes);
				} catch (Exception e) {
					throw new VectoException("Failed to read Driver Acceleration Curve", e);
				}
				return accelerationData;
			}
		}

		#endregion

		public virtual IList<IAuxiliaryEngineeringInputData> Auxiliaries
		{
			get { return AuxData().Cast<IAuxiliaryEngineeringInputData>().ToList(); }
		}

		IList<IAuxiliaryDeclarationInputData> IAuxiliariesDeclarationInputData.Auxiliaries
		{
			get { return AuxData().Cast<IAuxiliaryDeclarationInputData>().ToList(); }
		}

		private IList<AuxiliaryDataInputData> AuxData()
		{
			var retVal = new List<AuxiliaryDataInputData>();
			foreach (var aux in Body["Aux"] ?? Enumerable.Empty<JToken>()) {
				var auxData = new AuxiliaryDataInputData {
					ID = aux.GetEx<string>("ID"),
					Type = aux.GetEx<string>("Type"),
					Technology = aux.GetEx<string>("Technology")
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