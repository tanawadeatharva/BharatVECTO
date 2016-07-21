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
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using iTextSharp.text.pdf.codec;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox
{
	public class TorqueConverterData
	{
		public List<TorqueRatioCurveEntry> TorqueRatio;
		public List<CharacteristicTorqueEntry> CharacteristicTorque;

		protected internal TorqueConverterData(List<TorqueRatioCurveEntry> torqueRatio,
			List<CharacteristicTorqueEntry> characteristicTorque)
		{
			TorqueRatio = torqueRatio;
			CharacteristicTorque = characteristicTorque;
		}

		public void GetInputTorqueAndAngularSpeed(NewtonMeter torqueOut, PerSecond angularSpeedOut, out NewtonMeter torqueIn,
			out PerSecond angularSpeedIn)
		{
			var solutions = new List<double>();
			var mpNorm = 1000.RPMtoRad().Value();

			foreach (
				var muEdge in TorqueRatio.Pairwise((item1, item2) => Edge.Create(new Point(item1.SpeedRatio, item1.TorqueRatio),
					new Point(item2.SpeedRatio, item2.TorqueRatio)))) {
				foreach (
					var mpEdge in
						CharacteristicTorque.Pairwise((item1, item2) => Edge.Create(new Point(item1.SpeedRatio, item1.Torque.Value()),
							new Point(item2.SpeedRatio, item2.Torque.Value())))) {
					var a = muEdge.OffsetXY * mpEdge.OffsetXY / (mpNorm * mpNorm);
					var b = angularSpeedOut.Value() * (muEdge.SlopeXY * mpEdge.OffsetXY + mpEdge.SlopeXY * muEdge.OffsetXY) / mpNorm;
					var c = angularSpeedOut.Value() * angularSpeedOut.Value() * mpEdge.SlopeXY * muEdge.SlopeXY / mpNorm -
							torqueOut.Value();
					var sol = VectoMath.QuadraticEquationSolver(a, b, c);

					var edge1 = muEdge;
					var edge2 = mpEdge;
					var selected =
						sol.Where(x => x > 0 && angularSpeedOut.Value() / x >= edge1.P1.X && angularSpeedOut.Value() / x < edge1.P2.X
										&& angularSpeedOut.Value() / x >= edge2.P1.X && angularSpeedOut.Value() / x < edge2.P2.X);
					solutions.AddRange(selected);
				}
			}

			angularSpeedIn = solutions.Min().SI<PerSecond>();
			torqueIn = 0.SI<NewtonMeter>();
		}
	}


	public class CharacteristicTorqueEntry
	{
		public double SpeedRatio;
		public NewtonMeter Torque;
	}

	public class TorqueRatioCurveEntry
	{
		public double SpeedRatio;
		public double TorqueRatio;
	}
}