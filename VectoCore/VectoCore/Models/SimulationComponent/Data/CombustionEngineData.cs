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
using System.ComponentModel.DataAnnotations;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Engine;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Data
{
	public class CombustionEngineData : SimulationComponentData
	{
		[Required, SIRange(1000 * 1E-6, 20000 * 1E-6)]
		public CubicMeter Displacement { get; internal set; }

		[Required, SIRange(400 * Constants.RPMToRad, 1000 * Constants.RPMToRad)]
		public PerSecond IdleSpeed { get; internal set; }

		[Required, SIRange(0, 10)]
		public KilogramSquareMeter Inertia { get; internal set; }

		[Required, Range(0.9, 2)]
		public double WHTCUrban { get; internal set; }

		[Required, Range(0.9, 2)]
		public double WHTCRural { get; internal set; }

		[Required, Range(0.9, 2)]
		public double WHTCMotorway { get; internal set; }

		[Required, ValidateObject]
		public FuelConsumptionMap ConsumptionMap { get; internal set; }

		[Required, ValidateObject]
		public EngineFullLoadCurve FullLoadCurve { get; internal set; }

		internal double WHTCCorrectionFactor = 1;

		public CombustionEngineData()
		{
			WHTCUrban = 1;
			WHTCMotorway = 1;
			WHTCRural = 1;
		}

		public CombustionEngineData Copy()
		{
			return new CombustionEngineData {
				Displacement = Displacement,
				IdleSpeed = IdleSpeed,
				Inertia = Inertia,
				WHTCUrban = WHTCUrban,
				WHTCRural = WHTCRural,
				WHTCMotorway = WHTCMotorway,
				ConsumptionMap = ConsumptionMap,
				FullLoadCurve = FullLoadCurve,
				WHTCCorrectionFactor = WHTCCorrectionFactor,
			};
		}

		#region Equality Member

		protected bool Equals(CombustionEngineData other)
		{
			return Equals(FullLoadCurve, other.FullLoadCurve) && string.Equals(ModelName, other.ModelName) &&
					Equals(Displacement, other.Displacement) && Equals(IdleSpeed, other.IdleSpeed) && Equals(Inertia, other.Inertia) &&
					Equals(WHTCUrban, other.WHTCUrban) && Equals(WHTCRural, other.WHTCRural) &&
					Equals(WHTCMotorway, other.WHTCMotorway) && Equals(ConsumptionMap, other.ConsumptionMap);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) {
				return false;
			}
			if (ReferenceEquals(this, obj)) {
				return true;
			}
			if (obj.GetType() != this.GetType()) {
				return false;
			}
			return Equals((CombustionEngineData)obj);
		}

		public override int GetHashCode()
		{
			unchecked {
				var hashCode = (FullLoadCurve != null ? FullLoadCurve.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (ModelName != null ? ModelName.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (Displacement != null ? Displacement.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (IdleSpeed != null ? IdleSpeed.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (Inertia != null ? Inertia.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (WHTCUrban.GetHashCode());
				hashCode = (hashCode * 397) ^ (WHTCRural.GetHashCode());
				hashCode = (hashCode * 397) ^ (WHTCMotorway.GetHashCode());
				hashCode = (hashCode * 397) ^ (ConsumptionMap != null ? ConsumptionMap.GetHashCode() : 0);
				return hashCode;
			}
		}

		#endregion
	}
}