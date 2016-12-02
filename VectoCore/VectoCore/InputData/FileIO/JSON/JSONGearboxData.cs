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
using System.Data;
using System.Linq;
using Newtonsoft.Json.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Impl;
using TUGraz.VectoCore.Models.Declaration;

namespace TUGraz.VectoCore.InputData.FileIO.JSON
{
	public class JSONGearboxDataV6 : JSONGearboxDataV5
	{
		public JSONGearboxDataV6(JObject data, string filename) : base(data, filename) {}

		public override GearboxType Type
		{
			get { return Body.GetEx<string>(JsonKeys.Gearbox_GearboxType).ParseEnum<GearboxType>(); }
		}

		public override IList<ITransmissionInputData> Gears
		{
			get
			{
				var resultGears = new List<ITransmissionInputData>();
				var gears = Body.GetEx(JsonKeys.Gearbox_Gears);
				for (var i = 1; i < gears.Count(); i++) {
					var gear = gears[i];

					resultGears.Add(CreateGear(i, gear));
				}
				return resultGears;
			}
		}

		public override TableData ShiftPolygon
		{
			get
			{
				return Body[JsonKeys.Gearbox_TorqueConverter] != null &&
						Body[JsonKeys.Gearbox_TorqueConverter]["ShiftPolygon"] != null
					? ReadTableData(Body.GetEx(JsonKeys.Gearbox_TorqueConverter)
						.GetEx<string>("ShiftPolygon"), "TorqueConverter Shift Polygon", false)
					: null;
			}
		}
	}

	/// <summary>
	///		Represents the Data containing all parameters of the gearbox
	/// </summary>
	/// {
	///  "Header": {
	///    "CreatedBy": "Raphael Luz IVT TU-Graz (85407225-fc3f-48a8-acda-c84a05df6837)",
	///    "Date": "29.07.2014 16:59:17",
	///    "AppVersion": "2.0.4-beta",
	///    "FileVersion": 4
	///  },
	///  "Body": {
	///    "SavedInDeclMode": false,
	///    "ModelName": "Generic 24t Coach",
	///		"GearboxType": "AMT",
	///    "Gears": [
	///      {
	///        "Ratio": 3.240355,
	///        "LossMap": "Axle.vtlm"
	///      },
	///      {
	///        "Ratio": 6.38,
	///        "LossMap": "Indirect GearData.vtlm",
	///      },
	///		...
	///		]
	/// }
	// ReSharper disable once InconsistentNaming
	public class JSONGearboxDataV5 : JSONFile, IGearboxEngineeringInputData, IAxleGearInputData,
		ITorqueConverterEngineeringInputData
	{
		public JSONGearboxDataV5(JObject data, string filename) : base(data, filename) {}

		#region IAxleGearInputData

		public virtual double Ratio
		{
			[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design",
				"CA1065:DoNotRaiseExceptionsInUnexpectedLocations")]
			get
			{
				var gears = Body.GetEx(JsonKeys.Gearbox_Gears);
				if (!gears.Any()) {
					throw new VectoSimulationException("At least one Gear-Entry must be defined in Gearbox!");
				}
				return gears[0].GetEx<double>(JsonKeys.Gearbox_Gear_Ratio);
			}
		}

		public TableData LossMap
		{
			[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design",
				"CA1065:DoNotRaiseExceptionsInUnexpectedLocations")]
			get
			{
				var gears = Body.GetEx(JsonKeys.Gearbox_Gears);
				if (!gears.Any()) {
					throw new VectoSimulationException("At least one Gear-Entry must be defined in Gearbox!");
				}
				var lossMap = gears[0][JsonKeys.Gearbox_Gear_LossMapFile];
				if (lossMap != null) {
					return ReadTableData(gears[0][JsonKeys.Gearbox_Gear_LossMapFile].Value<string>(), "AxleGear", required: false);
				}
				return null;
			}
		}

		public double Efficiency
		{
			[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design",
				"CA1065:DoNotRaiseExceptionsInUnexpectedLocations")]
			get
			{
				var gears = Body.GetEx(JsonKeys.Gearbox_Gears);
				if (!gears.Any()) {
					throw new VectoSimulationException("At least one Gear-Entry must be defined in Gearbox!");
				}
				return gears[0].GetEx<double>(JsonKeys.Gearbox_Gear_Efficiency);
			}
		}

		#endregion

		#region IGearboxInputData

		public virtual GearboxType Type
		{
			get
			{
				//return Body.GetEx<string>(JsonKeys.Gearbox_GearboxType).ParseEnum<GearboxType>(); 
				var typeString = Body.GetEx<string>(JsonKeys.Gearbox_GearboxType);
				if (!"AT".Equals(typeString)) {
					return typeString.ParseEnum<GearboxType>();
				}
				var gearRatio = Body.GetEx(JsonKeys.Gearbox_Gears)[1].GetEx<double>(JsonKeys.Gearbox_Gear_Ratio);
				var gearEfficiency = Body.GetEx(JsonKeys.Gearbox_Gears)[1][JsonKeys.Gearbox_Gear_Efficiency].Value<double>();
				if (gearRatio.IsEqual(1) && gearEfficiency.IsEqual(1)) {
					return GearboxType.ATPowerSplit;
				}
				return GearboxType.ATSerial;
			}
		}

		public virtual KilogramSquareMeter Inertia
		{
			get { return Body.GetEx<double>(JsonKeys.Gearbox_Inertia).SI<KilogramSquareMeter>(); }
		}

		public virtual TableData ShiftPolygon
		{
			get
			{
				return ReadTableData(Body.GetEx(JsonKeys.Gearbox_Gears)[1].GetEx<string>("ShiftPolygon"),
					"TorqueConverter Shift Polygon", false);
			}
		}

		public Second TractionInterruption
		{
			get { return Body.GetEx<double>(JsonKeys.Gearbox_TractionInterruption).SI<Second>(); }
		}

		public virtual IList<ITransmissionInputData> Gears
		{
			get
			{
				var resultGears = new List<ITransmissionInputData>();
				var gears = Body.GetEx(JsonKeys.Gearbox_Gears);
				var gearNr = 1;
				for (var i = 1; i < gears.Count(); i++, gearNr++) {
					var gear = gears[i];
					var torqueConverter = gear.GetEx<bool>(JsonKeys.Gearbox_Gear_TCactive);

					if (torqueConverter) {
						resultGears.Add(gears[i + 1].GetEx<bool>(JsonKeys.Gearbox_Gear_TCactive)
							? CreateGear(gearNr, gear)
							: CreateTorqueConverterGear(gearNr, gear, gears[++i]));
					} else {
						resultGears.Add(CreateGear(gearNr, gear));
					}
				}
				return resultGears;
			}
		}

		private TransmissionInputData CreateTorqueConverterGear(int gearNr, JToken gear, JToken nextGear)
		{
			var ratio = gear.GetEx<double>(JsonKeys.Gearbox_Gear_Ratio);
			var efficiency = gear[JsonKeys.Gearbox_Gear_Efficiency] != null
				? gear[JsonKeys.Gearbox_Gear_Efficiency].Value<double>()
				: double.NaN;
			var nextEfficiency = nextGear[JsonKeys.Gearbox_Gear_Efficiency] != null
				? nextGear[JsonKeys.Gearbox_Gear_Efficiency].Value<double>()
				: double.NaN;
			var nextRatio = nextGear.GetEx<double>(JsonKeys.Gearbox_Gear_Ratio);

			if (!((ratio.IsEqual(1) && efficiency.IsEqual(1)) || (!ratio.IsEqual(1) && ratio.IsEqual(nextRatio)))) {
				throw new VectoException(
					"TorqueConverter gear either has to have a ratio of 1 and efficiency of 1, or the ratios of the torque converter gear and the locked gear have to be the same");
			}
			return new TransmissionInputData {
				Gear = gearNr,
				Ratio = nextRatio,
				LossMap = nextGear[JsonKeys.Gearbox_Gear_LossMapFile] != null
					? ReadTableData(nextGear.GetEx<string>(JsonKeys.Gearbox_Gear_LossMapFile),
						string.Format("Gear {0} LossMap", gearNr))
					: null,
				Efficiency = nextEfficiency,
				MaxTorque = gear["MaxTorque"] != null ? gear["MaxTorque"].Value<double>().SI<NewtonMeter>() : null,
				ShiftPolygon = ReadTableData(gear.GetEx<string>(JsonKeys.Gearbox_Gear_ShiftPolygonFile),
					string.Format("Gear {0} shiftPolygon", gearNr), false),
			};
		}

		protected TransmissionInputData CreateGear(int gearNumber, JToken gear)
		{
			return new TransmissionInputData {
				Gear = gearNumber,
				Ratio = gear.GetEx<double>(JsonKeys.Gearbox_Gear_Ratio),
				MaxTorque =
					gear["MaxTorque"] != null && !string.IsNullOrEmpty(gear["MaxTorque"].ToString())
						? gear["MaxTorque"].Value<double>().SI<NewtonMeter>()
						: null,
				LossMap =
					gear[JsonKeys.Gearbox_Gear_LossMapFile] != null
						? ReadTableData(gear.GetEx<string>(JsonKeys.Gearbox_Gear_LossMapFile),
							string.Format("Gear {0} LossMap", gearNumber), false)
						: null,
				Efficiency = gear[JsonKeys.Gearbox_Gear_Efficiency] != null
					? gear[JsonKeys.Gearbox_Gear_Efficiency].Value<double>()
					: double.NaN,
				ShiftPolygon = gear[JsonKeys.Gearbox_Gear_ShiftPolygonFile] != null
					? ReadTableData(gear.GetEx<string>(JsonKeys.Gearbox_Gear_ShiftPolygonFile),
						string.Format("Gear {0} shiftPolygon", gearNumber), false)
					: null,
			};
		}

		public virtual Second MinTimeBetweenGearshift
		{
			get { return Body.GetEx<double>(JsonKeys.Gearbox_ShiftTime).SI<Second>(); }
		}

		public virtual double TorqueReserve
		{
			get { return Body.GetEx<double>(JsonKeys.Gearbox_TorqueReserve) / 100.0; }
		}

		public virtual MeterPerSecond StartSpeed
		{
			get { return Body.GetEx<double>(JsonKeys.Gearbox_StartSpeed).SI<MeterPerSecond>(); }
		}

		public virtual MeterPerSquareSecond StartAcceleration
		{
			get { return Body.GetEx<double>(JsonKeys.Gearbox_StartAcceleration).SI<MeterPerSquareSecond>(); }
		}

		public virtual double StartTorqueReserve
		{
			get { return Body.GetEx<double>(JsonKeys.Gearbox_StartTorqueReserve) / 100.0; }
		}

		public virtual ITorqueConverterEngineeringInputData TorqueConverter
		{
			get { return this; }
		}

		public Second DownshiftAferUpshiftDelay
		{
			get
			{
				return Body["DownshiftAferUpshiftDelay"] == null
					? DeclarationData.Gearbox.DownshiftAfterUpshiftDelay
					: Body.GetEx<double>("DownshiftAferUpshiftDelay").SI<Second>();
			}
		}

		public Second UpshiftAfterDownshiftDelay
		{
			get
			{
				return Body["UpshiftAfterDownshiftDelay"] == null
					? DeclarationData.Gearbox.UpshiftAfterDownshiftDelay
					: Body.GetEx<double>("UpshiftAfterDownshiftDelay").SI<Second>();
			}
		}

		public MeterPerSquareSecond UpshiftMinAcceleration
		{
			get
			{
				return Body["UpshiftMinAcceleration"] == null
					? DeclarationData.Gearbox.UpshiftMinAcceleration
					: Body.GetEx<double>("UpshiftMinAcceleration").SI<MeterPerSquareSecond>();
			}
		}

	    public Second PowershiftShiftTime
	    {
	        get
	        {
	            return Body["PowershiftShiftTime"] == null
	                ? 0.8.SI<Second>()
	                : Body.GetEx<double>("PowershiftShiftTime").SI<Second>();
	        }
	    }

	    public double PowerShiftInertiaFactor
	    {
	        get { return Body["PowershiftInertiaFactor"] == null ? 1 : Body.GetEx<double>("PowershiftInertiaFactor"); }
	    }

	    #endregion

		#region ITorqueConverterInputData

		// deprecated: AT transmission has to have a torque converter. 
		//public virtual bool Enabled
		//{
		//	get
		//	{
		//		return false;
		//		// TODO mk-2016-05-09: JSON ITorqueConverterInputData.Enabled always true --> as soon as TC is implemented, set to correct value!
		//	}
		//}

		public virtual PerSecond ReferenceRPM
		{
			get
			{
				return Body[JsonKeys.Gearbox_TorqueConverter] != null &&
						Body[JsonKeys.Gearbox_TorqueConverter][JsonKeys.Gearbox_TorqueConverter_ReferenceRPM] != null
					? Body.GetEx(JsonKeys.Gearbox_TorqueConverter)
						.GetEx<double>(JsonKeys.Gearbox_TorqueConverter_ReferenceRPM)
						.RPMtoRad()
					: null;
			}
		}

        public PerSecond MaxInputSpeed
        {
            get { return Body[JsonKeys.Gearbox_TorqueConverter] != null && Body[JsonKeys.Gearbox_TorqueConverter]["MaxTCSpeed"] != null ? Body.GetEx(JsonKeys.Gearbox_TorqueConverter).GetEx<double>("MaxTCSpeed").RPMtoRad() : 5000.RPMtoRad(); }
        }
		public virtual TableData TCData
		{
			get
			{
				return Body[JsonKeys.Gearbox_TorqueConverter] != null &&
						Body[JsonKeys.Gearbox_TorqueConverter][JsonKeys.Gearbox_TorqueConverter_TCMap] != null
					? ReadTableData(Body.GetEx(JsonKeys.Gearbox_TorqueConverter).GetEx<string>(JsonKeys.Gearbox_TorqueConverter_TCMap),
						"TorqueConverter Data", false)
					: null;
			}
		}

		KilogramSquareMeter ITorqueConverterEngineeringInputData.Inertia
		{
			get
			{
				return Body[JsonKeys.Gearbox_TorqueConverter] != null &&
						Body[JsonKeys.Gearbox_TorqueConverter][JsonKeys.Gearbox_TorqueConverter_Inertia] != null
					? Body.GetEx(JsonKeys.Gearbox_TorqueConverter)
						.GetEx<double>(JsonKeys.Gearbox_TorqueConverter_Inertia)
						.SI<KilogramSquareMeter>()
					: null;
			}
		}

		#endregion

		public string Vendor
		{
			get { return "N/A"; }
		}

		public string ModelName
		{
			get { return Body.GetEx<string>(JsonKeys.Gearbox_ModelName); }
		}

		public string Creator
		{
			get { return "N/A"; }
		}

		public string Date
		{
			get { return "N/A"; }
		}

		public string TypeId
		{
			get { return "N/A"; }
		}

		public string DigestValue
		{
			get { return ""; }
		}

		public IntegrityStatus IntegrityStatus
		{
			get { return IntegrityStatus.Unknown; }
		}
	}
}