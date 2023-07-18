using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Data.ElectricComponents
{
	public class FuelCellData
	{
		
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
			[Required, SIRange(0, 1e8)] public KilogramPerSecond Resistance;
		}

    }
}