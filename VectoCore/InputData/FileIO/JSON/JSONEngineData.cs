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

using System.Data;
using Newtonsoft.Json.Linq;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.JSON
{
	/// <summary>
	///     Represents the CombustionEngineData. Fileformat: .veng
	/// </summary>
	/// <code>
	/// {
	///  "Header": {
	///    "CreatedBy": " ()",
	///    "Date": "3/4/2015 12:26:24 PM",
	///    "AppVersion": "2.0.4-beta3",
	///    "FileVersion": 2
	///  },
	///  "Body": {
	///    "SavedInDeclMode": false,
	///    "ModelName": "Generic 24t Coach",
	///    "Displacement": 12730.0,
	///    "IdlingSpeed": 560.0,
	///    "Inertia": 3.8,
	///    "FullLoadCurves": [
	///      {
	///        "Path": "24t Coach.vfld",
	///        "Gears": "0 - 99"
	///      }
	///    ],
	///    "FuelMap": "24t Coach.vmap",
	///    "WHTC-Urban": 0.0,
	///    "WHTC-Rural": 0.0,
	///    "WHTC-Motorway": 0.0
	///  }
	/// }
	/// </code>
	public class JSONEngineDataV3 : JSONFile, IEngineEngineeringInputData
	{
		public JSONEngineDataV3(JObject data, string fileName) : base(data, fileName) {}

		public virtual CubicMeter Displacement
		{
			get { return Body.GetEx<double>(JsonKeys.Engine_Displacement).SI().Cubic.Centi.Meter.Cast<CubicMeter>(); }
			// convert vom ccm to m^3}
		}

		public virtual PerSecond IdleSpeed
		{
			get { return Body.GetEx<double>(JsonKeys.Engine_IdleSpeed).RPMtoRad(); }
		}

		public virtual DataTable FuelConsumptionMap
		{
			get { return ReadTableData(Body.GetEx<string>(JsonKeys.Engine_FuelConsumptionMap), "FuelConsumptionMap"); }
		}

		public virtual DataTable FullLoadCurve
		{
			get { return ReadTableData(Body.GetEx<string>(JsonKeys.Engine_FullLoadCurveFile), "FullLoadCurve"); }
		}

		public virtual KilogramSquareMeter Inertia
		{
			get { return Body.GetEx<double>(JsonKeys.Engine_Inertia).SI<KilogramSquareMeter>(); }
		}

		public virtual KilogramPerWattSecond WHTCMotorway
		{
			get
			{
				return Body.GetEx<double>(JsonKeys.Engine_WHTC_Motorway).SI().Gramm.Per.Kilo.Watt.Hour.Cast<KilogramPerWattSecond>();
			}
		}

		public virtual KilogramPerWattSecond WHTCRural
		{
			get
			{
				return Body.GetEx<double>(JsonKeys.Engine_WHTC_Rural).SI().Gramm.Per.Kilo.Watt.Hour.Cast<KilogramPerWattSecond>();
			}
		}

		public virtual KilogramPerWattSecond WHTCUrban
		{
			get
			{
				return Body.GetEx<double>(JsonKeys.Engine_WHTC_Urban).SI().Gramm.Per.Kilo.Watt.Hour.Cast<KilogramPerWattSecond>();
			}
		}

		public string Vendor
		{
			get { return "N/A"; }
		}

		public string MakeAndModel
		{
			get { return Body.GetEx<string>(JsonKeys.Engine_ModelName); }
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
			get { return "N/A"; }
		}

		public IntegrityStatus IntegrityStatus
		{
			get { return IntegrityStatus.Unknown; }
		}
	}
}