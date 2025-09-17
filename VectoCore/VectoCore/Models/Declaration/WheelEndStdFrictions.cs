using System.Data;
using System.Linq;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.Declaration
{
    public sealed class WheelEndStdFrictions : LookupData<VehicleClass, double>
    {
        private const string VEHICLE_CLASS_FIELD = "class";
        private const string FRICTION_FIELD = "friction";
        
        protected override string ResourceId => DeclarationData.DeclarationDataResourcePrefix + ".WheelEndFriction.csv";

        protected override string ErrorMessage => "ERROR: Could not find the WheelEnd friction for vehicle. Class: {0}";

        protected override void ParseData(DataTable table)
		{
            Entries.Clear();

			foreach (DataRow row in table.AsEnumerable()) {
                Entries.Add(VehicleClassHelper.Parse(row.Field<string>(VEHICLE_CLASS_FIELD)), row.ParseDouble(FRICTION_FIELD));
            }
		}

        public NewtonMeter Find(VehicleClass vehicleClass)
        {
            return ContainsKey(vehicleClass) ? Lookup(vehicleClass).SI<NewtonMeter>() : null;
        }

    }
}
