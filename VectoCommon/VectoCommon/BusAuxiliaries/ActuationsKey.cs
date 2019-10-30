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
using System.Diagnostics;

namespace TUGraz.VectoCommon.BusAuxiliaries
{
	[DebuggerDisplay("({ConsumerName}/{CycleName})")]
	public class ActuationsKey
	{
		// Properties
		public string ConsumerName { get; }

		public string CycleName { get; }

		// Constructor
		public ActuationsKey(string consumerName, string cycleName)
		{
			if (consumerName.Trim().Length == 0 | cycleName.Trim().Length == 0)
				throw new ArgumentException("ConsumerName and CycleName must be provided");
			ConsumerName = consumerName;
			CycleName = cycleName;
		}


		// Overrides to enable this class to be used as a dictionary key in the ActuationsMap.
		public override bool Equals(object obj)
		{
			var other = (ActuationsKey)obj;
			if (other == null) {
				return false;
			}
			return other.ConsumerName == ConsumerName && other.CycleName == CycleName;
		}

		public override int GetHashCode()
		{
			return string.Format("{0}#{1}", ConsumerName, CycleName).GetHashCode();
		}
	}
}
