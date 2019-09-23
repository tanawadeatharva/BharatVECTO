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

namespace TUGraz.VectoCore.BusAuxiliaries.Interfaces.DownstreamModules.PneumaticSystem
{
	public class ActuationsKey
	{
		private string _consumerName;
		private string _cycleName;

		// Properties
		public string ConsumerName
		{
			get {
				return _consumerName;
			}
		}

		public string CycleName
		{
			get {
				return _cycleName;
			}
		}

		// Constructor
		public ActuationsKey(string consumerName, string cycleName)
		{
			if (consumerName.Trim().Length == 0 | cycleName.Trim().Length == 0)
				throw new ArgumentException("ConsumerName and CycleName must be provided");
			_consumerName = consumerName;
			_cycleName = cycleName;
		}


		// Overrides to enable this class to be used as a dictionary key in the ActuationsMap.
		public override bool Equals(object obj)
		{
			var other = (ActuationsKey)obj;

			return other.ConsumerName == this.ConsumerName && other.CycleName == this.CycleName;
		}

		public override int GetHashCode()
		{
			return 0;
		}
	}
}
