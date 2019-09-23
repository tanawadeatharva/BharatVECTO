// Copyright 2017 European Union.
// Licensed under the EUPL (the 'Licence');
// 
// * You may not use this work except in compliance with the Licence.
// * You may obtain a copy of the Licence at: http://ec.europa.eu/idabc/eupl
// * Unless required by applicable law or agreed to in writing,
// software distributed under the Licence is distributed on an "AS IS" basis,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// 
// See the LICENSE.txt for the specific language governing permissions and limitations.

using System;
using System.Collections.Generic;
using System.Globalization;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.BusAuxiliaries.Interfaces;

namespace TUGraz.VectoCore.BusAuxiliaries.Legacy {
	public class cMAP : IFuelConsumptionMap
	{
		private List<float> LnU;
		private List<float> LTq;
		private List<float> lFC;

		private string sFilePath;
		private int iMapDim;

		private cDelaunayMap FuelMap;

		private void ResetMe()
		{
			lFC = null;
			LTq = null;
			LnU = null;
			iMapDim = -1;
			FuelMap = new cDelaunayMap();
		}

		public bool ReadFile(bool ShowMsg = true)
		{
			cFile_V3 file;
			string[] line;
			float nU;
			string MsgSrc;


			MsgSrc = "Main/ReadInp/MAP";

			// Reset
			ResetMe();

			// Stop if there's no file
			if (sFilePath == "" || !System.IO.File.Exists(sFilePath))
				return false;

			// Open file
			file = new cFile_V3();
			if (!file.OpenRead(sFilePath)) {
				file = null/* TODO Change to default(_) if this is not a reference type */;
				return false;
			}

			// Skip Header
			file.ReadLine();

			// Initi Lists (before version check so ReadOldFormat works)
			lFC = new System.Collections.Generic.List<float>();
			LTq = new System.Collections.Generic.List<float>();
			LnU = new System.Collections.Generic.List<float>();

			try {
				while (!file.EndOfFile) {

					// Line read
					line = file.ReadLine();

					// Line counter up (was reset in ResetMe)
					iMapDim += 1;

					// Revolutions
					nU = float.Parse(line[0], CultureInfo.InvariantCulture);

					LnU.Add(nU);

					// Power
					LTq.Add(float.Parse(line[1], CultureInfo.InvariantCulture));

					// FC
					// Check sign
					if (System.Convert.ToSingle(line[2]) < 0) {
						file.Close();

						return false;
					}

					lFC.Add(System.Convert.ToSingle(line[2]));
				}
			} catch (Exception ex) {
				goto lbEr;
			}

			// Close file
			file.Close();

			file = null/* TODO Change to default(_) if this is not a reference type */;

			return true;


			// ERROR-label for clean Abort
			lbEr:
			;
			file.Close();
			file = null/* TODO Change to default(_) if this is not a reference type */;

			return false;
		}

		public bool Triangulate()
		{
			int i;

			string MsgSrc;

			MsgSrc = "MAP/Norm";

			// FC Delauney
			for (i = 0; i <= iMapDim; i++)
				FuelMap.AddPoints(LnU[i], LTq[i], lFC[i]);

			return FuelMap.Triangulate();
		}


		public float fFCdelaunay_Intp(float nU, float Tq)
		{
			float val;

			val = System.Convert.ToSingle(FuelMap.Intpol(nU, Tq));

			if (FuelMap.ExtrapolError)
				return -10000;
			else
				return val;
		}


		public string FilePath
		{
			get {
				return sFilePath;
			}
			set {
				sFilePath = value;
			}
		}

		public int MapDim
		{
			get {
				return iMapDim;
			}
		}

		public List<float> Tq
		{
			get {
				return LTq;
			}
		}

		public List<float> FC
		{
			get {
				return lFC;
			}
		}

		public List<float> nU
		{
			get {
				return LnU;
			}
		}


		public KilogramPerSecond GetFuelConsumption(NewtonMeter torque, PerSecond angularVelocity)
		{
			return (fFCdelaunay_Intp(System.Convert.ToSingle(angularVelocity.AsRPM), System.Convert.ToSingle(torque.Value())) / 3600.0 / 1000.0).SI<KilogramPerSecond>();
		}
	}
}
