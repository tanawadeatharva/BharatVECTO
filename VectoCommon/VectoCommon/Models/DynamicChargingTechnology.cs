using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TUGraz.VectoCommon.Models
{
    public enum DynamicChargingTechnology
    {
        None,
        OverheadPantograph,
        OverheadTrolley,
        GroundRail,
        Wireless
    }

    public static class DynamicChargingTechnologyHelper
    {
        public static DynamicChargingTechnology Parse(string technology)
        {
            switch (technology)
            {
                case "None": return DynamicChargingTechnology.None;
                case "Overhead pantograph": return DynamicChargingTechnology.OverheadPantograph;
                case "Overhead trolley": return DynamicChargingTechnology.OverheadTrolley;
                case "Ground rail": return DynamicChargingTechnology.GroundRail;
                case "Wireless": return DynamicChargingTechnology.Wireless;
                default: return DynamicChargingTechnology.None;
            }
        }

        public static string ToXMLFormat(this DynamicChargingTechnology technology)
        {
            switch (technology)
            {
                case DynamicChargingTechnology.None: return "None";
                case DynamicChargingTechnology.OverheadPantograph: return "Overhead pantograph";
                case DynamicChargingTechnology.OverheadTrolley: return "Overhead trolley";
                case DynamicChargingTechnology.GroundRail: return "Ground rail";
                case DynamicChargingTechnology.Wireless: return "Wireless";
                default: throw new ArgumentOutOfRangeException($"Unknown Dynamic Charging Technology: {technology}");
            }
        }
    }

}
