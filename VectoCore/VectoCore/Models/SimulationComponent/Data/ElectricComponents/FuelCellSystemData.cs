using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
		public FuelCellMassFlowMap MassFlowMap { get; set; }
		public Watt MaxElectricPower { get; set; }
		public Watt MinElectricPower { get; set; }






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
			Entries = entries;
		}


		public class MassFlowMapEntry
		{
			[Required, SIRange(0, 1e8)] public Watt P_el_out;
			[Required, SIRange(0, 1e8)] public KilogramPerSecond H2;
		}

    }
}