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

using System.Data;
using Newtonsoft.Json.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Utils;

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

		public virtual double WHTCEngineering
		{
			get { return 1; }
		}

		public virtual double WHTCMotorway
		{
			get { return Body.GetEx<double>(JsonKeys.Engine_WHTC_Motorway); }
		}

		public virtual double WHTCRural
		{
			get { return Body.GetEx<double>(JsonKeys.Engine_WHTC_Rural); }
		}

		public virtual double WHTCUrban
		{
			get { return Body.GetEx<double>(JsonKeys.Engine_WHTC_Urban); }
		}

		public string Vendor
		{
			get { return "N/A"; }
		}

		public string ModelName
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