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

using System;
using System.Data;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.Declaration
{
	public class TorqueConverter : LookupData<double, TorqueConverter.TorqueConverterEntry>
	{
		protected const string ResourceId = "TUGraz.VectoCore.Resources.Declaration.DefaultTC.vtcc";


		public TorqueConverter()
		{
			ParseData(ReadCsvResource(ResourceId));
		}


		[Obsolete("Default Lookup not availabel. Use LookupMu or LookupTorque instead.", true)]
		protected new TorqueConverterEntry Lookup(double key)
		{
			throw new InvalidOperationException(
				"Default Lookup not available. Use TorqueConverter.LookupMu() or TorqueConverter.LookupTorque() instead.");
		}


		public NewtonMeter LookupTorque(double nu, PerSecond angularSpeedIn, PerSecond referenceSpeed)
		{
			var sec = Data.GetSection(kv => kv.Key < nu);

			if (nu < sec.Item1.Key || sec.Item2.Key < nu) {
				Log.Warn(string.Format("TCextrapol: nu = {0} [n_out/n_in]", nu));
			}

			var torque = VectoMath.Interpolate(sec.Item1.Key, sec.Item2.Key, sec.Item1.Value.Torque, sec.Item2.Value.Torque, nu);
			return torque * Math.Pow((angularSpeedIn / referenceSpeed).Cast<Scalar>(), 2);
		}

		public double LookupMu(double nu)
		{
			var sec = Data.GetSection(kv => kv.Key < nu);

			if (nu < sec.Item1.Key || sec.Item2.Key < nu) {
				Log.Warn(string.Format("TCextrapol: nu = {0} [n_out/n_in]", nu));
			}

			return VectoMath.Interpolate(sec.Item1.Key, sec.Item2.Key, sec.Item1.Value.Mu, sec.Item2.Value.Mu, nu);
		}


		protected override void ParseData(DataTable table)
		{
			Data.Clear();
			foreach (DataRow row in table.Rows) {
				Data[row.ParseDouble("nue")] = new TorqueConverterEntry {
					Mu = row.ParseDouble("mue"),
					Torque = row.ParseDouble("MP1000 (1000/rpm)^2*Nm").SI<NewtonMeter>()
				};
			}
		}

		public class TorqueConverterEntry
		{
			public double Mu { get; set; }
			public NewtonMeter Torque { get; set; }
		}
	}
}