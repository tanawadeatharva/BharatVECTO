using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;

namespace TUGraz.VectoCore.Models.SimulationComponent.Data.ElectricComponents
{
	public class FuelCellSystemData
	{
		public WattPerSecond GradientPowerChange { get; set; }

		public Second OnOffHysteresis { get; set; }


		public IList<FuelCellData> FuelCells { get; set; }

		public FuelCellPowerMap FuelCellPowerMap { get; set; }

		public Watt ChargingPower(Meter mileageCounterDistance)
		{
			return FuelCellPowerMap.Lookup(mileageCounterDistance);
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

		/// <summary>
		/// Constant for now, remove when this is replaced with the actual implementation 
		/// </summary>
		/// <param name="powerDelivery"></param>
		public FuelCellPowerMap(Watt powerDelivery)
		{
			_powerDelivery = powerDelivery;
		}

		public Watt Lookup(Meter distance)
		{
			return _powerDelivery;
		}


	}

	public class FuelCellMassFlowMap
	{


		protected internal MassFlowMapEntry[] Entries;

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

			for (var index = 1; index < Entries.Length; index++)
			{
				if (power.IsGreaterOrEqual(Entries[index - 1].P_el_out) && power.IsSmallerOrEqual(Entries[index].P_el_out))
				{
					return index;
				}
			}
			throw new VectoException("Power Request {0} exceeds fuel cell model data. min: {1} max: {2}", power, Entries.First().P_el_out, Entries.Last().P_el_out);
		}



        public class MassFlowMapEntry
		{
			[Required, SIRange(0, 1e8)] public Watt P_el_out;
			[Required, SIRange(0, 1e8)] public KilogramPerSecond H2;
		}
	}
}