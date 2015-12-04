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

using TUGraz.VectoCore.Models.SimulationComponent.Data.Engine;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Data
{
	public class CombustionEngineData : SimulationComponentData
	{
		public string ModelName { get; internal set; }

		public CubicMeter Displacement { get; internal set; }

		public PerSecond IdleSpeed { get; internal set; }

		public KilogramSquareMeter Inertia { get; internal set; }

		public KilogramPerWattSecond WHTCUrban { get; internal set; }

		public KilogramPerWattSecond WHTCRural { get; internal set; }

		public KilogramPerWattSecond WHTCMotorway { get; internal set; }

		public FuelConsumptionMap ConsumptionMap { get; internal set; }

		public EngineFullLoadCurve FullLoadCurve { get; internal set; }

		private double _whtcCorrectionFactor = 1;

		public double WHTCCorrectionFactor
		{
			get { return _whtcCorrectionFactor; }
			internal set { _whtcCorrectionFactor = value; }
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