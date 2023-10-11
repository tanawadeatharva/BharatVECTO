using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading;
using NLog.Filters;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.OutputData.ModDataPostprocessing.Impl.FuelCell;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Data.ElectricComponents
{
    public class FuelCellSystemData
	{
		/// <summary>
		/// Battery string id for pre processing battery
		/// </summary>
		public const int FuelCellBatID = 0xFCB;

		public IList<FuelCellStringData> FuelCellStrings { get; set; }

		public FuelCellPowerMap FuelCellPowerMap { get; set; }
		public FuelCellSystemShareMap FuelCellShareMap { get; set; }

		public Watt ChargingPower(Meter mileageCounterDistance)
		{
			return FuelCellPowerMap.Lookup(mileageCounterDistance);
		}

		public IFuelCellPreRunInfo PreRunPostProcessing { get; set; }

		private void CheckFcCount() {
			if (FuelCellStrings.Count > 2) {
				throw new VectoException("Only two strings are supported");
			}

			if (FuelCellStrings.Any(fcs => fcs.FcCount > 3)) {
				throw new VectoException("Max 3 fuel cells per string are allowed");
			}


			if (FuelCellStrings.Count < 1) {
				throw new VectoException("No fuel-cells provided");
			}
        }
		public Watt MinElectricPower {
			get {
				CheckFcCount();
				return FuelCellStrings.Min(fcs => fcs.MinPower);
			}
		}

		public Watt MaxElectricPower
		{
			get
			{
				CheckFcCount();
				return FuelCellStrings.Sum(fcs => fcs.MaxPower);
			}
		}

	}

	public class FuelCellData
	{
		public class FuelCellId
		{
			public int Id { get; set; }
			public int SubId { get; set; }

			#region Overrides of Object

			public override string ToString()
			{
				//Id is assigned based on the component
				//Sub id is used when the same fuel cell is used multiple time (->count)
				return $"{Id}.{SubId}";
			}

			#endregion
		}
		public FuelCellMassFlowMap MassFlowMap { get; set; }
		public Watt MaxElectricPower { get; set; }
		public Watt MinElectricPower { get; set; }

		public Watt MinEffPower => MassFlowMap.MinEffPower;

		///// <summary>
		///// id -> each different fuelcell string, subId -> if the same fuelcell component is used multiple times
  //      /// </summary>
  //      public FuelCellId Id { get; set; }
	}

	public class FuelCellStringData
	{
		private readonly FuelCellData _fcData;

		public FuelCellData FcData => _fcData;

		private readonly int _count;

		public FuelCellStringData(FuelCellData fcData, int count)
		{

			if (count < 1) {
				throw new ArgumentException("At least one fuel cell has to be provided per string");
			}

			_fcData = fcData;
			_count = count;
		}

		public int FcCount => _count;

		public FuelCellStringMassFlowMap MassFlowMap { get; set; }



		public Watt MaxPower => FcData.MaxElectricPower * _count;
		public Watt MinPower => FcData.MinElectricPower;
	}


	/// <summary>
	/// Determines the power delivery of the fuel cell given the current distance
	/// </summary>
	public class FuelCellPowerMap
	{
		private Watt _powerDelivery;

		private readonly List<FuelCellPowerMapEntry> _entries;
		private Meter[] _distanceEntries;

		public Watt InitPower => _entries.MinBy(e => e.Power).Power;

		/// <summary>
		/// Constant for now, remove when this is replaced with the actual implementation 
		/// </summary>
		/// <param name="powerDelivery">Test purpose (overrides lookup with constant power delivery</param>
		public FuelCellPowerMap(FuelCellPowerMapEntry[] entries,Watt powerDelivery = null)
		{
			_powerDelivery = powerDelivery;
			_entries = entries.OrderBy(e => e.Distance).ToList();

			_distanceEntries = _entries.Select(e => e.Distance).ToArray();

		}

		public FuelCellPowerMap(FuelCellPreRunPostprocessor.FCCalcEntry[] entries) : this(entries.Select(e => new FuelCellPowerMapEntry() {
				Distance = e.s,
				Power = e.FCPowerFinal,
			}).ToArray())
		{

		}

		public Watt Lookup(Meter distance)
		{
			if (_powerDelivery != null) {
				return _powerDelivery;
			}

			//Just interpolate for now ?

			var closest = FindClosestIndex(distance, out var interval);

			//var closest = FindIndex(distance, out var interval);

			if (distance.IsEqual(_entries[interval.end].Distance, 1E-06.SI<Meter>())) {
				return _entries[interval.end].Power;
			}

			if (distance.IsEqual(_entries[interval.start].Distance, 1E-06.SI<Meter>()))
			{
				return _entries[interval.start].Power;
			}


            return VectoMath.Interpolate(_entries[interval.start].Distance, _entries[interval.end].Distance, _entries[interval.start].Power,
				_entries[interval.end].Power, distance);
        }


		protected int FindClosestIndex(Meter distance, out (int start, int end) interval)
		{

			return SearchAlgorithm.FindClosest(_distanceEntries, distance, t => t.a.Value() - t.b.Value(), out interval);
		}


		//public Watt Lookup(Meter distance)
		//{
		//	if (_powerDelivery != null)
		//	{
		//		return _powerDelivery;
		//	}

		//	//Just interpolate for now ?
		//	var idx = FindIndex(distance);
		//	return VectoMath.Interpolate(_entries[idx - 1].Distance, _entries[idx].Distance, _entries[idx - 1].Power,
		//		_entries[idx].Power, distance);
		//}

		protected int FindIndex(Meter distance, out (int start, int end) interval)
		{

			for (var index = 1; index < _entries.Count; index++)
			{
				if (distance.IsGreaterOrEqual(_entries[index - 1].Distance) && distance.IsSmallerOrEqual(_entries[index].Distance)) {
					interval = (index, index - 1);
					return index;
				}
			}
			throw new VectoException("Distance Request {0} exceeds fuel cell model data. min: {1} max: {2}", distance, _entries.First().Distance, _entries.Last().Distance);
		}















        public class FuelCellPowerMapEntry
		{
			[Required, SIRange(0, 1e8)] public Meter Distance;
			[Required, SIRange(0, 1e8)] public Watt Power;
		}

	}

	public class FuelCellMassFlowMap
	{


		public Watt MinPower => Entries.MinBy(e => e.P_el_out).P_el_out;
		public Watt MaxPower => Entries.MaxBy(e => e.P_el_out).P_el_out;

		protected internal readonly MassFlowMapEntry[] Entries;

		public FuelCellMassFlowMap(MassFlowMapEntry[] entries)
		{
			Entries = entries.OrderBy(e => e.P_el_out).ToArray();
		}
		/// <summary>
		/// Looks up the fuel consumption for the given power
		/// </summary>
		/// <param name="power"></param>
		/// <param name="useMinEfficiency">If set to true, the maximum efficiency is used if <see cref="power"/> is smaller than <see cref="MinEffPower"/></param>
		/// <returns></returns>
		/// <exception cref="VectoException"></exception>
		public KilogramPerSecond Lookup(Watt power, bool useMinEfficiency = true)
		{
			if (power < 0) {
				throw new VectoException($"{nameof(power)} power must be > 0");
			}
			if (useMinEfficiency && power < MinEffPower) {
				return LookupPowerInMap(MinEffPower) * (power / MinEffPower);




			} else {
				return LookupPowerInMap(power);
			}
		}

		private KilogramPerSecond LookupPowerInMap(Watt power)
		{
			var idx = FindIndex(power);
			return VectoMath.Interpolate(Entries[idx - 1].P_el_out, Entries[idx].P_el_out, Entries[idx - 1].H2,
				Entries[idx].H2, power);
        }

		protected int FindIndex(Watt power)
		{
			//TODO switch to binary search
			for (var index = 1; index < Entries.Length; index++)
			{
				if (power.IsGreaterOrEqual(Entries[index - 1].P_el_out) && power.IsSmallerOrEqual(Entries[index].P_el_out))
				{
					return index;
				}
			}
			
			throw new VectoException("Power Request {0} exceeds fuel cell model data. min: {1} max: {2}", power, Entries.First().P_el_out, Entries.Last().P_el_out);
		}

		public List<Watt> MeasuredPoints => Entries.Select(e => e.P_el_out).ToList();

		/// <summary>
		/// Point with the maximal efficiency, usually at low powers, used to split the power between the fuel cells in a string
		/// </summary>
		public Watt MinEffPower => Entries.MaxBy(e => e.P_el_out / e.H2)?.P_el_out;



        public class MassFlowMapEntry
		{
			[Required, SIRange(0, 1e8)] public Watt P_el_out;
			[Required, SIRange(0, 1e8)] public KilogramPerSecond H2;
		}
	}

	public class FuelCellStringMassFlowMap
	{
		public Watt MinPower => _fuelCellComponentMap.MinPower;
		public Watt MaxPower => _fuelCellComponentMap.MaxPower * _fcCount;


		public Watt MinPowerEff => _fuelCellComponentMap.MinEffPower;

		public FuelCellMassFlowMap _fuelCellComponentMap;
		private readonly int _fcCount;

		private List<Watt> _measuredPoints = null;
		public IList<Watt> MeasuredPoints {
			get { return _measuredPoints ?? (_measuredPoints = GetMeasuredPoints()); }
		}

		private List<Watt> GetMeasuredPoints()
		{
			var measuredPoints = new List<Watt>();
			var minEffPower = _fuelCellComponentMap.MinEffPower;

			if (_fcCount == 1) {
				return _fuelCellComponentMap.MeasuredPoints;
			}

			for (int i = 1; i <= _fcCount; i++) {
				//Get points before switching
				if (i == 1) {
					measuredPoints.AddRange(_fuelCellComponentMap.MeasuredPoints.Where(p => p < 2 * minEffPower));


					continue;
				}

				//use all operating points up to _fcCount * MaxPower
				if (i == _fcCount) {
					measuredPoints.AddRange(_fuelCellComponentMap.MeasuredPoints.Where(p => p >= minEffPower)
						.Select(p => (i) * p));


					continue;
				}

				measuredPoints.AddRange(_fuelCellComponentMap.MeasuredPoints
					.Where(p => p >= minEffPower && i * p < (i + 1) * minEffPower).Select(p => p * i));
				//measuredPoints.AddRange(_fuelCellComponentMap.MeasuredPoints.Where(p => p.));
			}


			return measuredPoints;
		}


		public FuelCellStringMassFlowMap(FuelCellMassFlowMap fcMap, int fcCount)
		{
			_fcCount = fcCount;
			_fuelCellComponentMap = fcMap;
			if (fcCount < 1) {
				throw new ArgumentException("At string must consist of at least one fuel cell");
			}
		}

		internal int GetActiveFuelCellCount(Watt p)
		{
			var minFcCount = (int)Math.Ceiling(p / _fuelCellComponentMap.MaxPower);

			var fcEffCount = (int)VectoMath.LimitTo(Math.Floor((p / _fuelCellComponentMap.MinEffPower).Value()), 1, _fcCount);

			return VectoMath.Max(minFcCount, fcEffCount);

		}

		public KilogramPerSecond Lookup(Watt power)
		{
			var activeFc = GetActiveFuelCellCount(power);
			var totalFc = 0.SI<KilogramPerSecond>();

			for (int i = 0; i < activeFc; i++) {
				totalFc += _fuelCellComponentMap.Lookup(power / activeFc);
			}

			return totalFc;
		}
		
	}

	public class FuelCellSystemMassFlowMap
	{
		private readonly FuelCellStringMassFlowMap _fcString1;
		private readonly FuelCellStringMassFlowMap _fcString2;

		public FuelCellSystemMassFlowMap(FuelCellStringMassFlowMap fcString1, FuelCellStringMassFlowMap fcString2 = null)
		{
			_fcString1 = fcString1 ?? throw new ArgumentNullException(nameof(fcString1));
			_fcString2 = fcString2;
		}

		public Watt MaxPower => _fcString1.MaxPower + (_fcString2?.MaxPower ?? 0.SI<Watt>());
		public Watt MinPower => VectoMath.Min(_fcString1.MinPower, _fcString2?.MinPower ?? _fcString1.MinPower);

		public FuelCellStringMassFlowMap FuelCell1 => _fcString1;
		public FuelCellStringMassFlowMap FuelCell2 => _fcString2;

        public KilogramPerSecond Lookup(FuelCellSystemShareMap.FuelCellShare share, Watt power)
		{
			
			if (_fcString2 == null && !share.ShareA.IsEqual(1)) {
				throw new ArgumentException(
					"Invalid fuel cell share, if only one fuel cell string is present the share must always be equal to 1");
			}

			var fuelConsumption1 = share.ShareA.IsEqual(0) ? 0.SI<KilogramPerSecond>() : (_fcString1.Lookup(share.ShareA * power));
			var fuelConsumption2 = share.ShareB.IsEqual(0) ? 0.SI<KilogramPerSecond>() : _fcString2?.Lookup(share.ShareB * power) ?? 0.SI<KilogramPerSecond>();
			return fuelConsumption1 + fuelConsumption2;
		}
	}
	/// <summary>
	/// Holds the shares between the fuel cell strings.
	/// </summary>
	public class FuelCellSystemShareMap
	{
		private readonly FuelCellSystemMassFlowMap _fcSystemMassFlowMap;

		public class FuelCellShareEntry
		{
			private readonly Watt _power;

			public Watt Power => _power;

			private readonly KilogramPerSecond _fuelConsumption;

			public KilogramPerSecond FuelConsumption => _fuelConsumption;

			private readonly FuelCellShare _share;

			public FuelCellShare Share => _share;

			public FuelCellShareEntry(Watt power, KilogramPerSecond fuelConsumption, FuelCellShare share)
			{
				_power = power;
				_fuelConsumption = fuelConsumption;
				_share = share;
			}

			

		}

		public class FuelCellShare
		{
			public FuelCellShare(double a) {
				ShareA = a;
			}

			public double ShareA { get; }
			public double ShareB => 1 - ShareA;
		}




		public FuelCellSystemShareMap(FuelCellSystemMassFlowMap fcSystemMassFlowMap)
		{
			_fcSystemMassFlowMap = fcSystemMassFlowMap;
		}



		/// <summary>
		/// 
		/// </summary>
		/// <param name="power"></param>
		/// <param name="previous">If multiple shares lead to the same fuel consumption, a similar share than the previous one is selected</param>
		/// <returns></returns>
		public FuelCellShareEntry Lookup(Watt power, FuelCellShare previous = null)
		{
			var shares = GetSharesWithLowestFuelConsumption(power);
			var p = previous;

			return previous != null 
				? shares.OrderBy(s => Math.Sqrt(Math.Pow(s.Share.ShareA - p.ShareA, 2))).First()
				: shares.First();
		}


		internal IEnumerable<FuelCellShare> GetValidShares(Watt power)
		{
			if (_fcSystemMassFlowMap.FuelCell2 == null) {
				var list = new List<FuelCellShare> { new FuelCellShare(1) };
				return list;
			}

			var fc1 = _fcSystemMassFlowMap.FuelCell1.MeasuredPoints;
			var fc2 = _fcSystemMassFlowMap.FuelCell2.MeasuredPoints;

			var allMeasured = fc1.Concat(fc2).Distinct();


			var shares = allMeasured.Select(p => p / power).ToDouble();
			shares.Add(0); //manually add the configuration where 1 fuel cell supplies the whole power
            //fc1 is lead, a1 represents share of fuel cell 1 of the total power demand
			var a1 = FilterInvalid(shares, fc1, fc2, power);

			//fc2 is lead, b2 represents share of fuel cell 2 of the total power demand
			var b1 = FilterInvalid(shares, fc2, fc1, power);
			var a2 = b1.Select(b => 1 - b);


			return a1.Concat(a2).Select(a => Math.Round(Convert.ToDecimal(a), 9)).Distinct().Select(a => new FuelCellShare(Convert.ToDouble(a)));
		}

		private IEnumerable<double> FilterInvalid(IList<double> leadShare, IList<Watt> leadFc, IList<Watt> followFc, Watt power)
		{
			var leadMaxPower = leadFc.Max();

			var followMaxPower = followFc.Max();

			var shares = leadShare.Where(l => l.IsGreaterOrEqual(0) && l.IsSmallerOrEqual(1));

			return shares
				.Where(
					l => (( (l * power).IsSmallerOrEqual(leadMaxPower)) || l == 0) && 

							((((1 - l) * power).IsSmallerOrEqual(followMaxPower)) || l.IsEqual(1)));

		}

		public IEnumerable<FuelCellShareEntry> GetSharesWithLowestFuelConsumption(Watt power)
		{

			var validShares = GetValidShares(power).ToList();

			var validSharesFuelConsumption = validShares.Select(s => new FuelCellShareEntry(power, fuelConsumption:_fcSystemMassFlowMap.Lookup(s, power), s));


			var orderedShares = validSharesFuelConsumption
				.OrderBy(s => s.FuelConsumption);
			var minFc = orderedShares.First().FuelConsumption;
			foreach (var share in orderedShares) {
				if (share.FuelConsumption.IsEqual(minFc)) {
					yield return share;
				} else {
					yield break;
				}
			}
		}




	}

}