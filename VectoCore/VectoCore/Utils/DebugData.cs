/*
* This file is part of VECTO.
*
* Copyright © 2012-2019 European Union
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
using System.Diagnostics;

namespace TUGraz.VectoCore.Utils
{
	public class DebugData
	{
		internal readonly Queue<dynamic> Data;
		private const int defaultCapacity = 16;

		public DebugData()
		{
#if DEBUG
			Data = new Queue<dynamic>(defaultCapacity);
#else
			Data = new Queue<dynamic>(0);
#endif
		}

		[Conditional("DEBUG")]
		public void Trim(int maxCount = defaultCapacity)
		{
			while (Data.Count > maxCount) {
				Data.Dequeue();
			}
		}
		
		[Conditional("DEBUG")]
		public void Add(object value) => Data.Enqueue(value);

		[Conditional("DEBUG")]
		public void Add(object value1, object value2) => Data.Enqueue((value1, value2));

		[Conditional("DEBUG")]
		public void Add(object value1, object value2, object value3) => Data.Enqueue((value1, value2, value3));
	}
}