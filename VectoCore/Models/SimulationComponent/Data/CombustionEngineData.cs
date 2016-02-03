/*
* Copyright 2015 European Union
*
* Licensed under the EUPL (the "Licence");
* You may not use this work except in compliance with the Licence.
* You may obtain a copy of the Licence at:
*
* http://ec.europa.eu/idabc/eupl5
*
* Unless required by applicable law or agreed to in writing, software 
* distributed under the Licence is distributed on an "AS IS" basis,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the Licence for the specific language governing permissions and 
* limitations under the Licence.
*/

using System;
using System.ComponentModel.DataAnnotations;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Engine;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Data
{
	public class CombustionEngineData : SimulationComponentData
	{
		[Required, SIRange(1000 / (Constants.Kilo * Constants.Kilo), 20000 / (Constants.Kilo * Constants.Kilo))]
		public CubicMeter Displacement { get; internal set; }

		[Required, SIRange(400 * Constants.RPMToRad, 1000 * Constants.RPMToRad)]
		public PerSecond IdleSpeed { get; internal set; }

		[Required, SIRange(0, 10)]
		public KilogramSquareMeter Inertia { get; internal set; }

		[Required, SIRange(double.Epsilon, 1000 / (Constants.Kilo * Constants.Kilo * Constants.SecondsPerHour))]
		public KilogramPerWattSecond WHTCUrban { get; internal set; }

		[Required, SIRange(double.Epsilon, 1000 / (Constants.Kilo * Constants.Kilo * Constants.SecondsPerHour))]
		public KilogramPerWattSecond WHTCRural { get; internal set; }

		[Required, SIRange(double.Epsilon, 1000 / (Constants.Kilo * Constants.Kilo * Constants.SecondsPerHour))]
		public KilogramPerWattSecond WHTCMotorway { get; internal set; }

		[Required, ValidateObject]
		public FuelConsumptionMap ConsumptionMap { get; internal set; }

		[Required, ValidateObject]
		public EngineFullLoadCurve FullLoadCurve { get; internal set; }

		internal double WHTCCorrectionFactor = 1;

		#region Equality Member

		protected bool Equals(CombustionEngineData other)
		{
			return Equals(FullLoadCurve, other.FullLoadCurve) && string.Equals(MakeAndModel, other.MakeAndModel) &&
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
				hashCode = (hashCode * 397) ^ (MakeAndModel != null ? MakeAndModel.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (Displacement != null ? Displacement.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (IdleSpeed != null ? IdleSpeed.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (Inertia != null ? Inertia.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (WHTCUrban != null ? WHTCUrban.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (WHTCRural != null ? WHTCRural.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (WHTCMotorway != null ? WHTCMotorway.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (ConsumptionMap != null ? ConsumptionMap.GetHashCode() : 0);
				return hashCode;
			}
		}

		#endregion
	}
}