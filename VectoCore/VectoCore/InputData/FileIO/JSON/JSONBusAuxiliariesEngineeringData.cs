using System.IO;
using Newtonsoft.Json.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Reader.Impl;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.JSON
{
	public class JSONBusAuxiliariesEngineeringDataV1 : JSONFile, IBusAuxiliariesEngineeringData, IBusAuxPneumaticSystemEngineeringData, IBusAuxElectricSystemEngineeringData, IBusAuxHVACData
	{
		private JToken _pneumatic;
		private JToken _electric;
		private JToken _hvac;


		public JSONBusAuxiliariesEngineeringDataV1(JObject data, string filename, bool tolerateMissing = false) : base(
			data, filename, tolerateMissing)
		{
			_pneumatic = Body["PneumaticSystem"];
			_electric = Body["ElectricSystem"];
			_hvac = Body["HVAC"];
		}
		
		public IBusAuxPneumaticSystemEngineeringData PneumaticSystem
		{
			get { return this; }
		}
		
		public IBusAuxElectricSystemEngineeringData ElectricSystem
		{
			get { return this; }
		}
		
		public IBusAuxHVACData HVACData { get { return this; } }


		#region Implementation of  IBusAuxPneumaticSystemEngineeringData

		public TableData CompressorMap
		{
			get { return VectoCSVFile.Read(Path.Combine(BasePath, _pneumatic.GetEx<string>("CompressorMap"))); }
		}

		public NormLiterPerSecond AverageAirConsumed
		{
			get { return _pneumatic.GetEx<double>("AverageAirDemand").SI<NormLiterPerSecond>(); }
		}

		public bool SmartAirCompression
		{
			get { return _pneumatic.GetEx<bool>("SmartAirCompression"); }
		}

		public double GearRatio
		{
			get { return _pneumatic.GetEx<double>("GearRatio"); }
		}

		#endregion

		#region Implementation of IBusAuxElectricSystemEngineeringData

		public double AlternatorEfficiency
		{
			get { return _electric.GetEx<double>("AlternatorEfficiency"); }
		}

		public double DCDCConverterEfficiency
		{
			get
			{
				return _electric["DCDCConverterEfficiency"] == null ? 1 : _electric.GetEx<double>("DCDCConverterEfficiency");
			}
		}

		public Ampere CurrentDemand
		{
			get { return _electric.GetEx<double>("CurrentDemand").SI<Ampere>(); }
		}

		public Ampere CurrentDemandEngineOffDriving { get { return _electric.GetEx<double>("CurrentDemandEngineOffDriving").SI<Ampere>(); } }

		public Ampere CurrentDemandEngineOffStandstill { get { return _electric.GetEx<double>("CurrentDemandEngineOffStandstill").SI<Ampere>(); } }

		public AlternatorType AlternatorType
		{
			get
			{
				return _electric["AlternatorType"] == null ? AlternatorType.Conventional :  _electric.GetEx<string>("AlternatorType").ParseEnum<AlternatorType>();
			}
		}

		public WattSecond ElectricStorageCapacity
		{
			get
			{
				if (_electric["ElectricStorageCapacity"] == null) {
					return null;
				}

				return _electric.GetEx<double>("ElectricStorageCapacity").SI(Unit.SI.Watt.Hour).Cast<WattSecond>();
			}
		}

		public Watt MaxAlternatorPower
		{
			get { return _electric.GetEx<double>("MaxAlternatorPower").SI<Watt>(); }
		}

		public bool ESSupplyFromHEVREESS
		{
			get
			{
				return _electric["ESSupplyFromHEVREESS"] == null ? false : _electric.GetEx<bool>("ESSupplyFromHEVREESS");
			}
		}

		public double ElectricStorageEfficiency
		{
			get
			{
				return _electric["BatteryEfficiency"] == null ? 1 : _electric.GetEx<double>("BatteryEfficiency");
			}
		}

		#endregion

		#region Implementation of IBusAuxHVACData

		public Watt ElectricalPowerDemand
		{
			get { return _hvac.GetEx<double>("ElectricPowerDemand").SI<Watt>(); }
		}

		public Watt MechanicalPowerDemand
		{
			get { return _hvac.GetEx<double>("MechanicalPowerDemand").SI<Watt>(); }
		}

		public Joule AverageHeatingDemand
		{
			get { return _hvac.GetEx<double>("AverageHeatingDemand").SI(Unit.SI.Mega.Joule).Cast<Joule>(); }
		}

		public Watt AuxHeaterPower
		{
			get
			{
				return _hvac.GetEx<double>("AuxHeaterPower").SI<Watt>();
			}
		}

		#endregion
	}
}