using NUnit.Framework;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Declaration;

namespace TUGraz.VectoCore.Tests.Models.Declaration
{
	[TestFixture]
	public class DeclarationSegmentHeavyBusesTest
	{

		[
		TestCase(VehicleCategory.HeavyBusPrimaryVehicle, AxleConfiguration.AxleConfig_4x2, FloorType.LowFloor, false, false, VehicleClass.ClassPB41),
		TestCase(VehicleCategory.HeavyBusPrimaryVehicle, AxleConfiguration.AxleConfig_4x2, FloorType.HighFloor, true, false, VehicleClass.ClassPB41),
		TestCase(VehicleCategory.HeavyBusPrimaryVehicle, AxleConfiguration.AxleConfig_4x2, FloorType.LowFloor, false, true, VehicleClass.ClassPB41),
		TestCase(VehicleCategory.HeavyBusPrimaryVehicle, AxleConfiguration.AxleConfig_4x2, FloorType.HighFloor, true, true, VehicleClass.ClassPB41),

		TestCase(VehicleCategory.HeavyBusPrimaryVehicle, AxleConfiguration.AxleConfig_6x2, FloorType.LowFloor, false, false, VehicleClass.ClassPB42),
		TestCase(VehicleCategory.HeavyBusPrimaryVehicle, AxleConfiguration.AxleConfig_6x2, FloorType.HighFloor, true, false, VehicleClass.ClassPB42),
		TestCase(VehicleCategory.HeavyBusPrimaryVehicle, AxleConfiguration.AxleConfig_6x4, FloorType.LowFloor, false, false, VehicleClass.ClassPB42),
		TestCase(VehicleCategory.HeavyBusPrimaryVehicle, AxleConfiguration.AxleConfig_6x4, FloorType.HighFloor, true, false, VehicleClass.ClassPB42),

		TestCase(VehicleCategory.HeavyBusPrimaryVehicle, AxleConfiguration.AxleConfig_6x2, FloorType.LowFloor, false, true, VehicleClass.ClassPB43),
		TestCase(VehicleCategory.HeavyBusPrimaryVehicle, AxleConfiguration.AxleConfig_6x2, FloorType.HighFloor, true, true, VehicleClass.ClassPB43),
		TestCase(VehicleCategory.HeavyBusPrimaryVehicle, AxleConfiguration.AxleConfig_6x4, FloorType.LowFloor, false, true, VehicleClass.ClassPB43),
		TestCase(VehicleCategory.HeavyBusPrimaryVehicle, AxleConfiguration.AxleConfig_6x4, FloorType.HighFloor, true, true, VehicleClass.ClassPB43),

		TestCase(VehicleCategory.HeavyBusPrimaryVehicle, AxleConfiguration.AxleConfig_8x2, FloorType.LowFloor, false, false, VehicleClass.ClassPB44),
		TestCase(VehicleCategory.HeavyBusPrimaryVehicle, AxleConfiguration.AxleConfig_8x2, FloorType.HighFloor, true, false, VehicleClass.ClassPB44),
		TestCase(VehicleCategory.HeavyBusPrimaryVehicle, AxleConfiguration.AxleConfig_8x4, FloorType.LowFloor, false, false, VehicleClass.ClassPB44),
		TestCase(VehicleCategory.HeavyBusPrimaryVehicle, AxleConfiguration.AxleConfig_8x4, FloorType.HighFloor, true, false, VehicleClass.ClassPB44),

		TestCase(VehicleCategory.HeavyBusPrimaryVehicle, AxleConfiguration.AxleConfig_8x2, FloorType.LowFloor, false, true, VehicleClass.ClassPB45),
		TestCase(VehicleCategory.HeavyBusPrimaryVehicle, AxleConfiguration.AxleConfig_8x2, FloorType.HighFloor, true, true, VehicleClass.ClassPB45),
		TestCase(VehicleCategory.HeavyBusPrimaryVehicle, AxleConfiguration.AxleConfig_8x4, FloorType.LowFloor, false, true, VehicleClass.ClassPB45),
		TestCase(VehicleCategory.HeavyBusPrimaryVehicle, AxleConfiguration.AxleConfig_8x4, FloorType.HighFloor, true, true, VehicleClass.ClassPB45),
			]
		public void SegmentLookupTest(VehicleCategory category, AxleConfiguration axleConfiguration, FloorType floorType, bool doubleDecker, bool articulated, VehicleClass expectedClass)
		{
			var segment = DeclarationData.BusSegments.Lookup(category, axleConfiguration, articulated, floorType, doubleDecker, true);

			Assert.AreEqual(10, segment.Missions.Length);
			Assert.AreEqual(expectedClass, segment.VehicleClass);
		}
	}
}
