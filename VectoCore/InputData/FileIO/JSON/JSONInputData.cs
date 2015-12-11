using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using NLog;
using TUGraz.VectoCore.Exceptions;
using TUGraz.VectoCore.InputData.Impl;
using TUGraz.VectoCore.Models.Declaration;
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
	public abstract class JSONFile
	{
		private string _basePath;

		protected JObject Header;
		protected JObject Body;

		protected JSONFile(JObject data, string filename)
		{
			Header = (JObject)data[JsonKeys.JsonHeader];
			Body = (JObject)data[JsonKeys.JsonBody];
			BasePath = filename;
		}

		public int FileVersion
		{
			get { return Header[JsonKeys.JsonHeader_FileVersion].Value<int>(); }
		}

		public bool SavedInDeclarationMode
		{
			get { return Body[JsonKeys.SavedInDeclMode].Value<bool>(); }
		}

		internal string BasePath
		{
			get { return _basePath; }
			set { _basePath = Path.GetDirectoryName(Path.GetFullPath(value)); }
		}

		protected DataTable ReadTableData(string filename, string tableType, bool required = true)
		{
			if (!EmptyOrInvalidFileName(filename)) {
				return VectoCSVFile.Read(Path.Combine(BasePath, filename), true);
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
	public class JSONInputDataV2 : JSONFile, IInputDataProvider, IJobInputData, IDriverInputData, IAuxiliariesInputData
	{
		protected IGearboxInputData Gearbox;

		protected IAxleGearInputData AxleGear;

		protected IEngineInputData Engine;

		protected IVehicleInputData VehicleData;

		protected IRetarderInputData Retarder;

		private string _jobname;

		public JSONInputDataV2(JObject data, string filename) : base(data, filename)
		{
			_jobname = Path.GetFileName(filename);
			var gearboxFile = Body[JsonKeys.Vehicle_GearboxFile].Value<string>();
			if (!EmptyOrInvalidFileName(gearboxFile)) {
				Gearbox = JSONInputDataFactory.ReadGearbox(Path.Combine(BasePath, gearboxFile));
			}
			var axleGear = Gearbox as IAxleGearInputData;
			if (axleGear != null) {
				AxleGear = axleGear;
			}
			Engine = JSONInputDataFactory.ReadEngine(
				Path.Combine(BasePath, Body[JsonKeys.Vehicle_EngineFile].Value<string>()));
			var vehicleFile = Body[JsonKeys.Vehicle_VehicleFile].Value<string>();
			if (!EmptyOrInvalidFileName(vehicleFile)) {
				VehicleData = JSONInputDataFactory.ReadJsonVehicle(
					Path.Combine(BasePath, vehicleFile));
			}

			var retarder = VehicleData as IRetarderInputData;
			if (retarder != null) {
				Retarder = retarder;
			}
		}

		#region IInputDataProvider

		public IJobInputData JobInputData()
		{
			return this;
		}

		public IVehicleInputData VehicleInputData
		{
			get { return VehicleData; }
		}

		public IGearboxInputData GearboxInputData
		{
			get { return Gearbox; }
		}

		public IAxleGearInputData AxleGearInputData
		{
			get { return AxleGear; }
		}

		public IEngineInputData EngineInputData
		{
			get { return Engine; }
		}

		public IAuxiliariesInputData AuxiliaryInputData()
		{
			return this;
		}

		public IRetarderInputData RetarderInputData
		{
			get { return Retarder; }
		}

		public IDriverInputData DriverInputData
		{
			get { return this; }
		}

		#endregion

		#region IJobInputData

		public IVehicleInputData Vehicle
		{
			get { return VehicleData; }
		}

		public IList<ICycleData> Cycles
		{
			get
			{
				var retVal = new List<ICycleData>();
				foreach (var cycle in Body[JsonKeys.Job_Cycles]) {
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

		public bool EngineOnlyMode
		{
			get { return Body[JsonKeys.Job_EngineOnlyMode].Value<bool>(); }
		}

		public string JobName
		{
			get { return _jobname; }
		}

		#endregion

		#region DriverInputData

		public IStartStopInputData StartStop
		{
			get
			{
				return new StartStopInputData() {
					Enabled = Body[JsonKeys.DriverData_StartStop][JsonKeys.DriverData_StartStop_Enabled].Value<bool>(),
					Delay = Body[JsonKeys.DriverData_StartStop][JsonKeys.DriverData_StartStop_Delay].Value<double>().SI<Second>(),
					MaxSpeed =
						Body[JsonKeys.DriverData_StartStop][JsonKeys.DriverData_StartStop_MaxSpeed].Value<double>().KMPHtoMeterPerSecond(),
					MinTime = Body[JsonKeys.DriverData_StartStop][JsonKeys.DriverData_StartStop_MinTime].Value<double>().SI<Second>(),
				};
			}
		}

		public ILookaheadCoastingInputData Lookahead
		{
			get
			{
				return new LookAheadCoastingInputData() {
					Enabled = Body[JsonKeys.DriverData_LookaheadCoasting][JsonKeys.DriverData_Lookahead_Enabled].Value<bool>(),
					Deceleration =
						Body[JsonKeys.DriverData_LookaheadCoasting][JsonKeys.DriverData_Lookahead_Deceleration].Value<double>()
							.SI<MeterPerSquareSecond>(),
					MinSpeed =
						Body[JsonKeys.DriverData_LookaheadCoasting][JsonKeys.DriverData_Lookahead_MinSpeed].Value<double>()
							.KMPHtoMeterPerSecond(),
				};
			}
		}

		public IOverspeedEcoRollInputData OverspeedEcoRoll
		{
			get
			{
				return new OverSpeedEcoRollInputData() {
					Mode =
						DriverData.ParseDriverMode(
							Body[JsonKeys.DriverData_OverspeedEcoRoll][JsonKeys.DriverData_OverspeedEcoRoll_Mode].Value<string>()),
					MinSpeed =
						Body[JsonKeys.DriverData_OverspeedEcoRoll][JsonKeys.DriverData_OverspeedEcoRoll_MinSpeed].Value<double>()
							.KMPHtoMeterPerSecond(),
					OverSpeed =
						Body[JsonKeys.DriverData_OverspeedEcoRoll][JsonKeys.DriverData_OverspeedEcoRoll_OverSpeed].Value<double>()
							.KMPHtoMeterPerSecond(),
					UnderSpeed =
						Body[JsonKeys.DriverData_OverspeedEcoRoll][JsonKeys.DriverData_OverspeedEcoRoll_UnderSpeed].Value<double>()
							.KMPHtoMeterPerSecond()
				};
			}
		}

		public DataTable AccelerationCurve
		{
			get
			{
				var accelerationFile = Body[JsonKeys.DriverData_AccelerationCurve].Value<string>();
				return EmptyOrInvalidFileName(accelerationFile) ? null : ReadTableData(accelerationFile, "DriverAccelerationCurve");
			}
		}

		#endregion

		public IList<IAuxiliaryInputData> Auxiliaries
		{
			get
			{
				var retVal = new List<IAuxiliaryInputData>();
				foreach (var aux in Body["Aux"]) {
					var auxData = new AuxiliaryDataInputData {
						ID = aux["ID"].Value<string>(),
						Type = aux["Type"].Value<string>(),
						Technology = aux["Technology"].Value<string>()
					};
					var auxFile = aux["Path"].Value<string>();
					if (EmptyOrInvalidFileName(auxFile)) {
						continue;
					}
					var stream = new StreamReader(Path.Combine(BasePath, auxFile));
					stream.ReadLine(); // skip header "Transmission ration to engine rpm [-]"
					auxData.TransmissionRatio = stream.ReadLine().IndulgentParse();
					stream.ReadLine(); // skip header "Efficiency to engine [-]"
					auxData.EfficiencyToEngine = stream.ReadLine().IndulgentParse();
					stream.ReadLine(); // skip header "Efficiency auxiliary to supply [-]"
					auxData.EfficiencyToSupply = stream.ReadLine().IndulgentParse();
					auxData.DemandMap = VectoCSVFile.ReadStream(new MemoryStream(Encoding.UTF8.GetBytes(stream.ReadToEnd())));
					retVal.Add(auxData);
				}
				return retVal;
			}
		}
	}
}