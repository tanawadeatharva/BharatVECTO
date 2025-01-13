using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TUGraz.VectoCommon.Models
{
    public enum HydrogenStorageTechnology
    {
        Unknown,
        Compressed,
        Liquid,
        CryoCompressed
    }

    public static class HydrogenStorageTechnologyHelper
    {
        public static HydrogenStorageTechnology Parse(string technology)
        {
            switch (technology) 
            {
                case "Compressed": return HydrogenStorageTechnology.Compressed;
                case "Liquid": return HydrogenStorageTechnology.Liquid;
                case "Cryo-compressed": return HydrogenStorageTechnology.CryoCompressed;
                default: return HydrogenStorageTechnology.Unknown;
            }
        }

        public static string ToXMLFormat(this HydrogenStorageTechnology technology)
        {
            switch (technology)
            {
                case HydrogenStorageTechnology.Compressed: return "Compressed";
                case HydrogenStorageTechnology.Liquid: return "Liquid";
                case HydrogenStorageTechnology.CryoCompressed: return "Cryo-compressed";
                default: throw new ArgumentOutOfRangeException($"Unknown Hydrogen Storage Technology: {technology}");
            }
        }
    }

}
