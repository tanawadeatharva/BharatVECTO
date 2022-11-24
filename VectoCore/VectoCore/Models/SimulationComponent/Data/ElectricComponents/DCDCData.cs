using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TUGraz.VectoCore.Models.SimulationComponent.Data.ElectricComponents
{
    public class DCDCData
    {
        [Range(0,1)]
        public double DCDCEfficiency { get; internal set; }
    }
}
