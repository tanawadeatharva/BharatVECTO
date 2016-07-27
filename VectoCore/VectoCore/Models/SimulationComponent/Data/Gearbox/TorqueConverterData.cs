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
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox
{
	public class TorqueConverterData
	{
		protected List<TorqueConverterEntry> TorqueConverterEntries;

		public PerSecond ReferenceSpeed { get; protected internal set; }

		public KilogramSquareMeter Inertia { get; protected internal set; }

		protected internal TorqueConverterData(List<TorqueConverterEntry> torqueConverterEntries, PerSecond referenceSpeed)
		{
			TorqueConverterEntries = torqueConverterEntries;
			ReferenceSpeed = referenceSpeed;
		}


		public void GetInputTorqueAndAngularSpeed(NewtonMeter torqueOut, PerSecond angularSpeedOut, out NewtonMeter torqueIn,
			out PerSecond angularSpeedIn)
		{
			var solutions = new List<double>();
			var mpNorm = ReferenceSpeed.Value();

			// Find analytic solution for torque converter operating point
			// mu = f(nu) = f(n_out / n_in) = T_out / T_in
			// MP1000 = f(nu) = f(n_out / n_in)
			// Tq_in = MP1000(nu) * (n_in/1000)^2 = T_out / mu
			//
			// mu(nu) and MP1000(nu) are provided as piecewise linear functions (y = k*x+d)
			// solving the equation above for n_in results in a quadratic equation
			foreach (var segment in TorqueConverterEntries.Pairwise(Tuple.Create)) {
				var mpEdge = Edge.Create(new Point(segment.Item1.SpeedRatio, segment.Item1.Torque.Value()),
					new Point(segment.Item2.SpeedRatio, segment.Item2.Torque.Value()));
				var muEdge = Edge.Create(new Point(segment.Item1.SpeedRatio, segment.Item1.TorqueRatio),
					new Point(segment.Item2.SpeedRatio, segment.Item2.TorqueRatio));

				var a = muEdge.OffsetXY * mpEdge.OffsetXY / (mpNorm * mpNorm);
				var b = angularSpeedOut.Value() * (muEdge.SlopeXY * mpEdge.OffsetXY + mpEdge.SlopeXY * muEdge.OffsetXY) /
						(mpNorm * mpNorm);
				var c = angularSpeedOut.Value() * angularSpeedOut.Value() * mpEdge.SlopeXY * muEdge.SlopeXY / (mpNorm * mpNorm) -
						torqueOut.Value();
				var sol = VectoMath.QuadraticEquationSolver(a, b, c);

				var selected = sol.Where(x => x > 0
											&& angularSpeedOut.Value() / x >= muEdge.P1.X && angularSpeedOut.Value() / x < muEdge.P2.X
											&& angularSpeedOut.Value() / x >= mpEdge.P1.X && angularSpeedOut.Value() / x < mpEdge.P2.X);
				solutions.AddRange(selected);
			}
			if (solutions.Count == 0) {
				throw new VectoException("No solution for input torque/input speed found! n_out: {0}, tq_out: {1}", angularSpeedOut,
					torqueOut);
			}

			angularSpeedIn = solutions.Min().SI<PerSecond>();
			var mu = MuLookup(angularSpeedOut / angularSpeedIn);
			torqueIn = torqueOut / mu;
		}

		private double MuLookup(double nu)
		{
			int index;
			TorqueConverterEntries.GetSection(x => x.SpeedRatio > nu, out index);
			var muEdge =
				Edge.Create(new Point(TorqueConverterEntries[index].SpeedRatio, TorqueConverterEntries[index].TorqueRatio),
					new Point(TorqueConverterEntries[index + 1].SpeedRatio, TorqueConverterEntries[index + 1].TorqueRatio));
			return muEdge.SlopeXY * nu + muEdge.OffsetXY;
		}
	}


	public class TorqueConverterEntry
	{
		public double SpeedRatio;
		public NewtonMeter Torque;
		public double TorqueRatio;
	}
}