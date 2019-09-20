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

using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCore.BusAuxiliaries.Interfaces {
	public class Signals : ISignals
	{

		// Backing variables
		private double _WHTCCorretion = 1;
		private bool _smartElectrics;

		public bool ClutchEngaged { get; set; }
		public Watt EngineDrivelinePower { get; set; }
		public NewtonMeter EngineDrivelineTorque { get; set; }
		public Watt EngineMotoringPower { get; set; }
		public PerSecond EngineSpeed { get; set; }

		public bool SmartElectrics
		{
			get {
				return _smartElectrics;
			}
			set {
				_smartElectrics = value;
			}
		}

		public bool SmartPneumatics { get; set; }
		public int TotalCycleTimeSeconds { get; set; }
		public double CurrentCycleTimeInSeconds { get; set; }
		public Watt PreExistingAuxPower { get; set; }
		public bool Idle { get; set; }
		public bool InNeutral { get; set; }

		public AdvancedAuxiliaryMessageType AuxiliaryEventReportingLevel { get; set; }

		public bool EngineStopped { get; set; }
		public bool DeclarationMode { get; set; }

		public double WHTC
		{
			set {
				_WHTCCorretion = value;
			}
			get {
				return _WHTCCorretion;
			}
		}

		public PerSecond EngineIdleSpeed { get; set; }
		public double PneumaticOverrunUtilisation { get; set; }
		public double StoredEnergyEfficiency { get; set; }
		public bool RunningCalc { get; set; }
		public Watt InternalEnginePower { get; set; }
	}
}
