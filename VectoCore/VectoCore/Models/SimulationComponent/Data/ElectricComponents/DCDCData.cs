using System;
using System.ComponentModel.DataAnnotations;

namespace TUGraz.VectoCore.Models.SimulationComponent.Data.ElectricComponents
{
    public class DCDCData
    {
        [Range(0,1)]
        public double DCDCEfficiency { get; internal set; }
    }
}
