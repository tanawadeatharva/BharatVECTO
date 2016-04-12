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
using System.IO;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.Declaration
{
	public enum LoadingType
	{
		FullLoading,
		ReferenceLoad,
		EmptyLoading,
	}

	public static class LoadingTypeHelper
	{
		public static string GetShortName(this LoadingType loadingType)
		{
			return loadingType.ToString().Substring(0, 1);
		}
	}


	public class Mission
	{
		public MissionType MissionType { get; set; }
		public string CrossWindCorrection { get; set; }
		public double[] AxleWeightDistribution { get; set; }
		public double[] TrailerAxleWeightDistribution { get; set; }

		public Kilogram MassExtra { get; set; }

		public Kilogram MinLoad { get; set; }
		public Kilogram RefLoad { get; set; }
		public Kilogram MaxLoad { get; set; }

		public Dictionary<LoadingType, Kilogram> Loadings
		{
			get
			{
				return new Dictionary<LoadingType, Kilogram> {
					{ LoadingType.EmptyLoading, MinLoad },
					{ LoadingType.ReferenceLoad, RefLoad },
					{ LoadingType.FullLoading, MaxLoad }
				};
			}
		}

		public Stream CycleFile { get; set; }

		public bool UseCdA2 { get; set; }

		public class LoadingEntry
		{
			public Kilogram LoadingWeight;
			public string Name;
		}
	}
}