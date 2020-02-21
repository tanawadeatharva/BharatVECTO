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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCore.Models.Declaration
{
	public enum LoadingType
	{
		FullLoading,
		ReferenceLoad,
		LowLoading,
		EmptyLoading,
	}

	public class Mission
	{
		public Kilogram CurbMass { get; internal set; }
		public MissionType MissionType { get; internal set; }
		public string CrossWindCorrectionParameters { get; internal set; }
		public double[] AxleWeightDistribution { get; internal set; }

		public Kilogram BodyCurbWeight { get; internal set; }

		public Stream CycleFile { get; internal set; }

		public IList<MissionTrailer> Trailer { get; internal set; }

		public Kilogram MinLoad { get; internal set; }
		public Kilogram LowLoad { get; internal set; }
		public Kilogram RefLoad { get; internal set; }
		public Kilogram MaxLoad { get; internal set; }

		public Kilogram MaxPayload { get; internal set; }

		public Meter VehicleHeight { get; internal set; }

		public SquareMeter DefaultCDxA { get; internal set; }

		public CubicMeter TotalCargoVolume { get; internal set; }
		
		public Dictionary<LoadingType, Kilogram> Loadings
		{
			get {
				return new Dictionary<LoadingType, Kilogram>
				{
					{LoadingType.EmptyLoading, MinLoad },
					{ LoadingType.LowLoading, LowLoad },
					{ LoadingType.ReferenceLoad, RefLoad },
					{LoadingType.FullLoading, MaxLoad }
				}.Where(x => x.Value != null).ToDictionary(x => x.Key, x => x.Value);
			}
		}

		public BusParameters BusParameter { get; internal set; }
	}

	public class BusParameters
	{

		public Meter VehicleWidth { get; internal set; }

		public Meter VehicleLength { get; internal set; }

		public Meter BodyHeight { get; internal set; }

		public double NumberPassengersLowerDeck { get; internal set; }

		public double NumberPassengersUpperDeck { get; internal set; }

		public bool DoubleDecker { get; internal set; }

		public FloorType FloorType { get; internal set; }

		// #### HVAC Model Parameters

		public BusHVACSystemConfiguration HVACConfiguration { get; internal set; }

		public Watt HVACAuxHeaterPower { get; internal set; }

		public ACCompressorType HVACCompressorType { get; internal set; }

		public bool HVACDoubleGlasing { get; internal set; }

		public bool HVACHeatpump { get; internal set; }
		public bool HVACAdjustableAuxHeater { get; internal set; }

		public bool HVACSeparateAirDistributionDucts { get; internal set; }
		public PerSquareMeter PassengerDensity { get;  internal set; }
		public VehicleClass BusGroup { get; internal set; }
	}

	public class MissionTrailer
	{
		public TrailerType TrailerType { get; internal set; }
		public Kilogram TrailerCurbWeight { get; internal set; }
		public Kilogram TrailerGrossVehicleWeight { get; internal set; }
		public List<Wheels.Entry> TrailerWheels { get; internal set; }
		public double TrailerAxleWeightShare { get; internal set; }
		public SquareMeter DeltaCdA { get; internal set; }
		public CubicMeter CargoVolume { get; internal set; }
	}

	public enum TrailerType
	{
		//None,
		T1,
		T2,
		ST1,
		Dolly,
		STT1,
		STT2
	}

	public static class TrailterTypeHelper
	{
		public static TrailerType Parse(string trailer)
		{
			if ("d".Equals(trailer, StringComparison.InvariantCultureIgnoreCase)) {
				return TrailerType.Dolly;
			}
			return trailer.ParseEnum<TrailerType>();
		}
	}
}