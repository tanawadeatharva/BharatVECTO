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
using TUGraz.VectoCommon.BusAuxiliaries;

namespace TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.Pneumatics
{
	public class ActuationsMap : IActuationsMap
	{
		private Dictionary<ActuationsKey, int> _map;

		public ActuationsMap(Dictionary<ActuationsKey, int> map, string source)
		{
			_map = map;
			Source = source;
		}

		public int GetNumActuations(ActuationsKey key)
		{
			if (_map == null || !_map.ContainsKey(key))
				throw new ArgumentException(string.Format("Pneumatic Actuations map does not contain the key '{0} / {1}'.", key.ConsumerName, key.CycleName));

			return _map[key];
		}

		public string Source { get; }


		
	}
}
