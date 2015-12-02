using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using TUGraz.VectoCore.Exceptions;
using TUGraz.VectoCore.Models.Declaration;
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
			Header = (JObject)data["Header"];
			Body = (JObject)data["Body"];
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
	}

	public class JSONInputDataV2 : JSONFile, IInputDataProvider, IJobInputData, IDriverInputData
	{
		protected IGearboxInputData Gearbox;

		protected IAxleGearInputData AxleGear;

		protected IEngineInputData Engine;

		protected IVehicleInputData VehicleData;

		protected IRetarderInputData Retarder;


		public JSONInputDataV2(JObject data, string filename) : base(data, filename)
		{
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

		public IEnumerable<IAuxiliaryInputData> AuxiliaryInputData()
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
				return Body["Cycles"].Select(cycle => VectoCSVFile.Read(Path.Combine(BasePath, cycle.Value<string>()))).ToList();
			}
		}

		public bool EngineOnlyMode
		{
			get { return Body["EngineOnlyMode"].Value<bool>(); }
		}

		#endregion
	}

	public class JSONVehicleDataV7 : JSONFile, IVehicleInputData, IRetarderInputData
	{
		public JSONVehicleDataV7(JObject data, string fileName) : base(data, fileName) {}

		#region IVehicleInputData

		public VehicleCategory VehicleCategory
		{
			get
			{
				return
					(VehicleCategory)Enum.Parse(typeof(VehicleCategory), Body[JsonKeys.Vehicle_VehicleCategory].Value<string>(), true);
			}
		}

		public Kilogram CurbWeight
		{
			get { return Body[JsonKeys.Vehicle_CurbWeight].Value<double>().SI<Kilogram>(); }
		}

		public Kilogram GrossVehicleMassRating
		{
			get { return Body[JsonKeys.Vehicle_GrossVehicleMassRating].Value<double>().SI<Ton>().Cast<Kilogram>(); }
		}

		public SquareMeter DragCoefficient
		{
			get { return Body["CdA"].Value<double>().SI<SquareMeter>(); }
		}

		public SquareMeter DragCoefficientRigidTruck
		{
			get { return Body["CdA2"].Value<double>().SI<SquareMeter>(); }
		}

		public string Rim
		{
			get { return Body["Rim"].Value<string>(); }
		}

		public AxleConfiguration AxleConfiguration
		{
			get { return AxleConfigurationHelper.Parse(Body["AxleConfig"]["Type"].Value<string>()); }
		}

		public IList<IAxleInputData> Axles
		{
			get
			{
				return Body["AxleConfig"]["Axles"].Select(axle => new JSONAxleInputData() {
					Inertia = axle["Inertia"].Value<double>().SI<KilogramSquareMeter>(),
					Wheels = axle["Wheels"].Value<string>(),
					TwinTyres = axle["TwinTyres"].Value<bool>(),
					RollResistanceCoefficient = axle["RRCISO"].Value<double>(),
					TyreTestLoad = axle["FzISO"].Value<double>().SI<Newton>()
				}).Cast<IAxleInputData>().ToList();
			}
		}

		#endregion

		#region IRetarderInputData

		public RetarderData.RetarderType Type
		{
			get
			{
				return
					(RetarderData.RetarderType)
						Enum.Parse(typeof(RetarderData.RetarderType), Body["Retarder"]["Type"].Value<string>(), true);
			}
		}

		public double Ratio
		{
			get { return Body["Retarder"]["Ratio"].Value<double>(); }
		}

		public DataTable LossMap
		{
			get
			{
				var filename = Body["Retarder"]["File"].Value<string>();
				if (filename == null || !filename.Any() || filename.Equals("<NOFILE>", StringComparison.InvariantCultureIgnoreCase)) {
					throw new VectoException("Invalid Lossmap: {0}", filename);
				}
				return VectoCSVFile.Read(Path.Combine(BasePath, filename));
			}
		}

		#endregion
	}

	public class JSONEngineDataV3 : JSONFile, IEngineInputData
	{
		public JSONEngineDataV3(JObject data, string fileName) : base(data, fileName) {}

		public string ModelName
		{
			get { return Body["ModelName"].Value<string>(); }
		}

		public CubicMeter Displacement
		{
			get { return Body["Displacement"].Value<double>().SI().Cubic.Centi.Meter.Cast<CubicMeter>(); }
		}

		public RoundsPerMinute IdleSpeed
		{
			get { return Body["IdlingSpeed"].Value<double>().SI<RoundsPerMinute>(); }
		}

		public DataTable FullLoadCurve
		{
			get
			{
				var filename = Body["FullLoadCurve"].Value<string>();
				if (filename == null || !filename.Any() || filename.Equals("<NOFILE>", StringComparison.InvariantCultureIgnoreCase)) {
					throw new VectoException("Invalid FullLoadCurve: {0}", filename);
				}
				return VectoCSVFile.Read(Path.Combine(BasePath, filename));
			}
		}

		public KilogramSquareMeter Inertia
		{
			get { return Body["Inertia"].Value<double>().SI<KilogramSquareMeter>(); }
		}

		public KilogramPerWattSecond WHTCMotorway
		{
			get { return Body["WHTC-Motorway"].Value<double>().SI<KilogramPerWattSecond>(); }
		}

		public KilogramPerWattSecond WHTCRural
		{
			get { return Body["WHTC-Rural"].Value<double>().SI<KilogramPerWattSecond>(); }
		}

		public KilogramPerWattSecond WHTCUrban
		{
			get { return Body["WHTC-Urban"].Value<double>().SI<KilogramPerWattSecond>(); }
		}
	}

	public class JSONGearboxDataV5 : JSONFile, IGearboxInputData, IAxleGearInputData, ITorqueConverterInputData
	{
		public JSONGearboxDataV5(JObject data, string filename) : base(data, filename) {}

		#region IAxleGearInputData

		public double Ratio
		{
			get { return Body["Gears"][0]["Ratio"].Value<double>(); }
		}

		public DataTable LossMap
		{
			get
			{
				var filename = Body["Gears"][0]["LossMap"].Value<string>();
				if (filename == null || !filename.Any() || filename.Equals("<NOFILE>", StringComparison.InvariantCultureIgnoreCase)) {
					throw new VectoException("Invalid AxleGear LossMap: {0}", filename);
				}
				return VectoCSVFile.Read(Path.Combine(BasePath, filename));
			}
		}

		#endregion

		#region IGearboxInputData

		public string ModelName
		{
			get { return Body["ModelName"].Value<string>(); }
		}

		public GearboxType Type
		{
			get { return Body["GearboxType"].Value<string>().Parse<GearboxType>(); }
		}


		public KilogramSquareMeter Inertia
		{
			get { return Body["Inertia"].Value<double>().SI<KilogramSquareMeter>(); }
		}

		public Second TractionInterruption
		{
			get { return Body["TracInt"].Value<double>().SI<Second>(); }
		}

		public IList<ITransmissionInputData> Gears
		{
			get
			{
				var retVal = new List<ITransmissionInputData>();
				var i = 0;
				foreach (var gear in Body["Gears"]) {
					if (i++ == 0) {
						continue;
					}
					var lossMapFile = gear["LossMap"].Value<string>();
					if (lossMapFile == null || !lossMapFile.Any() ||
						lossMapFile.Equals("<NOFILE>", StringComparison.InvariantCultureIgnoreCase)) {
						throw new VectoException("Invalid AxleGear LossMap: {0}", lossMapFile);
					}
					var lossMap = VectoCSVFile.Read(Path.Combine(BasePath, lossMapFile));

					var fullLoadCurveFile = gear["FullLoadCurve"].Value<string>();
					DataTable fullLoadCurve = null;
					if (fullLoadCurveFile != null && fullLoadCurveFile.Any()) {
						fullLoadCurve = VectoCSVFile.Read(Path.Combine(BasePath, fullLoadCurveFile));
					}

					var shiftPolygonFile = gear["ShiftPolygon"].Value<string>();
					DataTable shiftPolygon = null;
					if (shiftPolygonFile != null && shiftPolygonFile.Any() && !shiftPolygonFile.Equals("-")) {
						fullLoadCurve = VectoCSVFile.Read(Path.Combine(BasePath, shiftPolygonFile));
					}

					retVal.Add(new JSONTransmissionInputData() {
						Gear = i,
						Ratio = gear["Ratio"].Value<double>(),
						FullLoadCurve = fullLoadCurve,
						LossMap = lossMap,
						ShiftPolygon = shiftPolygon,
						TorqueConverterActive = gear["TCactive"].Value<bool>()
					});
				}
				return retVal;
			}
		}

		public bool SkipGears
		{
			get { return Body["SkipGears"].Value<bool>(); }
		}

		public Second ShiftTime
		{
			get { return Body["ShiftTime"].Value<double>().SI<Second>(); }
		}

		public bool EarlyShiftUp
		{
			get { return Body["EarlyShiftUp"].Value<bool>(); }
		}

		public double TorqueReserve
		{
			get { return Body["TqReserve"].Value<double>() / 100.0; }
		}

		public MeterPerSecond StartSpeed
		{
			get { return Body["StartSpeed"].Value<double>().SI<MeterPerSecond>(); }
		}

		public MeterPerSquareSecond StartAcceleration
		{
			get { return Body["StartAcc"].Value<double>().SI<MeterPerSquareSecond>(); }
		}

		public double StartTorqueReserve
		{
			get { return Body["StartTqReserve"].Value<double>() / 100.0; }
		}

		public ITorqueConverterInputData TorqueConverter
		{
			get { return this; }
		}

		#endregion

		#region ITorqueConverterInputData

		public RoundsPerMinute ReferenceRPM
		{
			get { return Body["TorqueConverter"]["RefRPM"].Value<double>().SI<RoundsPerMinute>(); }
		}

		public DataTable TCData
		{
			get
			{
				var filename = Body["TorqueConverter"]["File"].Value<string>();
				if (filename == null || !filename.Any() || filename.Equals("<NOFILE>", StringComparison.InvariantCultureIgnoreCase)) {
					throw new VectoException("Invalid TroqueConverter Curve: {0}", filename);
				}
				return VectoCSVFile.Read(Path.Combine(BasePath, filename));
			}
		}

		KilogramSquareMeter ITorqueConverterInputData.Inertia
		{
			get { return Body["TorqueConverter"]["Inertia"].Value<double>().SI<KilogramSquareMeter>(); }
		}

		#endregion
	}

	public class JSONTransmissionInputData : ITransmissionInputData
	{
		public int Gear { get; internal set; }

		public double Ratio { get; internal set; }

		public DataTable LossMap { get; internal set; }

		public DataTable FullLoadCurve { get; internal set; }

		public DataTable ShiftPolygon { get; internal set; }

		public bool TorqueConverterActive { get; internal set; }
	}

	public class JSONAxleInputData : IAxleInputData
	{
		public string Wheels { get; internal set; }

		public bool TwinTyres { get; internal set; }

		public double RollResistanceCoefficient { get; internal set; }

		public Newton TyreTestLoad { get; internal set; }

		public double AxleWeightShare { get; internal set; }

		public KilogramSquareMeter Inertia { get; internal set; }
	}
}