using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider;
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
		public WattPerSecond GradientPowerChange { get; set; }



		public IList<FuelCellData> FuelCells { get; set; }

		public FuelCellPowerMap FuelCellPowerMap { get; set; }

		public Watt ChargingPower(Meter mileageCounterDistance)
		{
			return FuelCellPowerMap.Lookup(mileageCounterDistance);
		}

		public IFuelCellPreRunInfo PreRunPostProcessing { get; set; }

		private void CheckFcCount()
		{
			if (FuelCells.Count > 1) {
				throw new VectoException("Multiple fuel-cells are currently not supported");
			}

			if (FuelCells.Count < 1) {
				throw new VectoException("No fuel-cells provided");
			}
        }
		public Watt MinPower
		{
			get
			{
				CheckFcCount();
				return FuelCells.First().MinElectricPower;
			}
		}

		public Watt MaxPower
		{
			get
			{
				CheckFcCount();
				return FuelCells.First().MaxElectricPower;
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
        /// <summary>
		/// id -> each different fuelcell component, subId -> if the same fuelcell component is used multiple times
        /// </summary>
        public FuelCellId Id { get; set; }
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

		public KilogramPerSecond Lookup(Watt power)
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
		public Watt MinPowerEff => Entries.MaxBy(e => e.P_el_out / e.H2)?.P_el_out;



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


		public Watt MinPowerEff => _fuelCellComponentMap.MinPowerEff;

		public FuelCellMassFlowMap _fuelCellComponentMap;
		private readonly int _fcCount;

		

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
			return (int)VectoMath.LimitTo(Math.Floor((p / _fuelCellComponentMap.MinPowerEff).Value()), 1, _fcCount);
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

}