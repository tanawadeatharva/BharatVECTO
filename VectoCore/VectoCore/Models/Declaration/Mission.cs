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
		// sort entries with increasing payload to get results ordered by increasing payload in declaration reports
		EmptyLoading,
		LowLoading,
		ReferenceLoad,
		FullLoading,
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

		public bool AirDragMeasurement { get; internal set; }
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

		//Completed Bus
		public int NumberOfAxles { get; internal set; }
		public bool IsArticulated { get; internal set; }
		public VehicleCode VehicleCode { get; internal set; }
		public RegistrationClass[] RegistrationClasses { get; internal set; }
		public bool? LowEntry { get; internal set; }
		public string VehicleParameterGroup { get; internal set; }
		public double PassengersHeavyUrban { get; internal set; }
		public double PassengersUrban { get; internal set; }
		public double PassengersSuburban { get; internal set; }
		public double PassengersInterurban { get; internal set; }
		public double PassengersCoach { get; internal set; }
		
		public bool?  BodyHeightLowerOrEqual { get; internal set; }
		public bool? PassengersSeatsLowerOrEqual { get; internal set; }
		public AxleLoadDistribution AxleLoadDistribution { get;  internal set; }
		public VehicleEquipment VehicleEquipment { get; internal set; }
	}

	public class VehicleEquipment
	{
		public double? ExternalDisplays { get; internal set; }
		public double? InternalDisplays { get; internal set; }
		public double? Fridge { get; internal set; }
		public double? KitchenStandard { get; internal set; }
	}

	public class AxleLoadDistribution
	{
		public double Axle01 { get; private set; }
		public double Axle02 { get; private set; }
		public double Axle03 { get; private set; }
		public double Axle04 { get; private set; }


		public AxleLoadDistribution(string loadDistribution)
		{
			SetAxleLoadDistribution(loadDistribution);
		}
		
		private void SetAxleLoadDistribution(string loadDistribution)
		{
			var splitResult = loadDistribution.Split('/');
			Axle01 = splitResult[0].ToDouble();
			Axle02 = splitResult[1].ToDouble();
			Axle03 = splitResult[2].ToDouble();
			Axle04 = splitResult[3].ToDouble();
		}
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

	public static class TrailerTypeHelper
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