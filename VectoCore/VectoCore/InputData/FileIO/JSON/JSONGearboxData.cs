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

namespace TUGraz.VectoCore.InputData.FileIO.JSON
{
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
	public class JSONGearboxDataV5 : JSONFile, IGearboxEngineeringInputData, IAxleGearInputData, ITorqueConverterInputData
	{
		public JSONGearboxDataV5(JObject data, string filename) : base(data, filename) {}

		#region IAxleGearInputData

		public virtual double Ratio
		{
			[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations")]
			get
			{
				var gears = Body.GetEx(JsonKeys.Gearbox_Gears);
				if (!gears.Any()) {
					throw new VectoSimulationException("At least one Gear-Entry must be defined in Gearbox!");
				}
				return gears[0].GetEx<double>(JsonKeys.Gearbox_Gear_Ratio);
			}
		}

		public DataTable LossMap
		{
			[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations")]
			get
			{
				var gears = Body.GetEx(JsonKeys.Gearbox_Gears);
				if (!gears.Any()) {
					throw new VectoSimulationException("At least one Gear-Entry must be defined in Gearbox!");
				}
				return ReadTableData(
					gears[0].GetEx<string>(JsonKeys.Gearbox_Gear_LossMapFile), "AxleGear");
			}
		}

		#endregion

		#region IGearboxInputData

		public virtual GearboxType Type
		{
			get { return Body.GetEx<string>(JsonKeys.Gearbox_GearboxType).ParseEnum<GearboxType>(); }
		}


		public virtual KilogramSquareMeter Inertia
		{
			get { return Body.GetEx<double>(JsonKeys.Gearbox_Inertia).SI<KilogramSquareMeter>(); }
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
				for (var i = 1; i < gears.Count(); i++) {
					var gear = gears[i];
					var inputData = new TransmissionInputData {
							Gear = i,
							Ratio = gear.GetEx<double>(JsonKeys.Gearbox_Gear_Ratio),
							FullLoadCurve = ReadTableData(gear.GetEx<string>(JsonKeys.Gearbox_Gear_FullLoadCurveFile), string.Format("Gear {0} FLD", i), false),
							LossMap = ReadTableData(gear.GetEx<string>(JsonKeys.Gearbox_Gear_LossMapFile), string.Format("Gear {0} LossMap", i)),
							ShiftPolygon = ReadTableData(gear.GetEx<string>(JsonKeys.Gearbox_Gear_ShiftPolygonFile), string.Format("Gear {0} shiftPolygon", i), false),
							TorqueConverterActive = gear.GetEx<bool>(JsonKeys.Gearbox_Gear_TCactive)};
					resultGears.Add(inputData);
				}
				return resultGears;
			}
		}

		public virtual bool SkipGears
		{
			get { return Body.GetEx<bool>(JsonKeys.Gearbox_SkipGears); }
		}

		public virtual Second ShiftTime
		{
			get { return Body.GetEx<double>(JsonKeys.Gearbox_ShiftTime).SI<Second>(); }
		}

		public virtual bool EarlyShiftUp
		{
			get { return Body.GetEx<bool>(JsonKeys.Gearbox_EarlyShiftUp); }
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

		public virtual ITorqueConverterInputData TorqueConverter
		{
			get { return this; }
		}

		#endregion

		#region ITorqueConverterInputData

		public virtual bool Enabled
		{
			get { return false; // TODO mk-2016-05-09: JSON ITorqueConverterInputData.Enabled always true --> as soon as TC is implemented, set to correct value!
			}
		}

		public virtual PerSecond ReferenceRPM
		{
			get
			{
				return
					Body.GetEx(JsonKeys.Gearbox_TorqueConverter)
						.GetEx<double>(JsonKeys.Gearbox_TorqueConverter_ReferenceRPM)
						.RPMtoRad();
			}
		}

		public virtual DataTable TCData
		{
			get
			{
				return
					ReadTableData(Body.GetEx(JsonKeys.Gearbox_TorqueConverter).GetEx<string>(JsonKeys.Gearbox_TorqueConverter_TCMap),
						"TorqueConverter Data");
			}
		}

		KilogramSquareMeter ITorqueConverterInputData.Inertia
		{
			get
			{
				return
					Body.GetEx(JsonKeys.Gearbox_TorqueConverter)
						.GetEx<double>(JsonKeys.Gearbox_TorqueConverter_Inertia)
						.SI<KilogramSquareMeter>();
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