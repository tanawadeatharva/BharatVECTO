using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using TUGraz.VectoCore.Exceptions;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.JSON
{
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
			if (filename == null || !filename.Any() || filename.Equals("<NOFILE>", StringComparison.InvariantCultureIgnoreCase)) {
				if (required) {
					throw new VectoException("Invalid {0}: {1}", tableType, filename);
				}
				return null;
			}
			return VectoCSVFile.Read(Path.Combine(BasePath, filename));
		}
	}

	public class JSONInputDataV2 : JSONFile, IInputDataProvider, IJobInputData, IDriverInputData
	{
		protected IGearboxInputData Gearbox;

		protected IAxleGearInputData AxleGear;

		protected IEngineInputData Engine;

		protected IVehicleInputData VehicleData;

		protected IRetarderInputData Retarder;

		private string _jobname;

		public JSONInputDataV2(JObject data, string filename) : base(data, filename)
		{
			_jobname = filename;
			Gearbox = JSONInputDataFactory.ReadGearbox(
				Path.Combine(BasePath, Body[JsonKeys.Vehicle_GearboxFile].Value<string>()));
			var axleGear = Gearbox as IAxleGearInputData;
			if (axleGear != null) {
				AxleGear = axleGear;
			}
			Engine = JSONInputDataFactory.ReadEngine(
				Path.Combine(BasePath, Body[JsonKeys.Vehicle_EngineFile].Value<string>()));
			VehicleData = JSONInputDataFactory.ReadJsonVehicle(
				Path.Combine(BasePath, Body[JsonKeys.Vehicle_VehicleFile].Value<string>()));
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

		public IList<IAuxiliaryInputData> AuxiliaryInputData()
		{
			return null;
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

		public IList<DataTable> Cycles
		{
			get
			{
				return
					Body[JsonKeys.Job_Cycles].Select(cycle => VectoCSVFile.Read(Path.Combine(BasePath, cycle.Value<string>())))
						.ToList();
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
				return new JSONStartStop() {
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
				return new JSONLookaheadCoasting() {
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
				return new JSONOverSpeedEcoRoll() {
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
			get { return ReadTableData(Body[JsonKeys.DriverData_AccelerationCurve].Value<string>(), "DriverAccelerationCurve"); }
		}

		#endregion
	}

	public class JSONStartStop : IStartStopInputData
	{
		public bool Enabled { get; internal set; }

		public MeterPerSecond MaxSpeed { get; internal set; }

		public Second MinTime { get; internal set; }

		public Second Delay { get; internal set; }
	}

	public class JSONLookaheadCoasting : ILookaheadCoastingInputData
	{
		public bool Enabled { get; internal set; }

		public MeterPerSquareSecond Deceleration { get; internal set; }

		public MeterPerSecond MinSpeed { get; internal set; }
	}

	public class JSONOverSpeedEcoRoll : IOverspeedEcoRollInputData
	{
		public DriverData.DriverMode Mode { get; internal set; }

		public MeterPerSecond MinSpeed { get; internal set; }

		public MeterPerSecond OverSpeed { get; internal set; }

		public MeterPerSecond UnderSpeed { get; internal set; }
	}
}