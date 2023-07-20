using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Data.ElectricComponents
{
	public class FuelCellSystemData
	{
		public WattPerSecond GradientPowerChange { get; set; }

		public Second OnOffHysteresis { get; set; }


		public IList<FuelCellData> FuelCells { get; set; }
	}

	public class FuelCellData
	{
		public FuelCellMassFlowMap MassFlowMap { get; set; }
		public Watt MaxElectricPower { get; set; }
		public Watt MinElectricPower { get; set; }






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