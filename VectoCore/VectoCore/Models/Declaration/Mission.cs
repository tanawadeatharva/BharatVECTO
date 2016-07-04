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
using System.IO;
using TUGraz.VectoCommon.Utils;

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
			switch (loadingType) {
				case LoadingType.FullLoading:
					return "F";
				case LoadingType.ReferenceLoad:
					return "R";
				case LoadingType.EmptyLoading:
					return "E";
				default:
					throw new ArgumentOutOfRangeException("loadingType", loadingType, null);
			}
		}
	}

	public class Mission
	{
		public MissionType MissionType;
		public string CrossWindCorrection;
		public double[] AxleWeightDistribution;
		public double[] TrailerAxleWeightDistribution;

		public Kilogram CurbWeight;
		public Kilogram BodyCurbWeight;
		public Kilogram BodyGrossVehicleWeight;
		public TrailerType TrailerType;
		public Kilogram TrailerCurbWeight;
		public Kilogram TrailerGrossVehicleWeight;
		public Stream CycleFile;
		public SquareMeter DeltaCdA;

		public Kilogram MinLoad;
		public Kilogram RefLoad;
		public Kilogram MaxLoad;

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
	}

	public enum TrailerType
	{
		None,
		T1,
		T2
	}
}