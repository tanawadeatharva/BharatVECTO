using System.Collections.Generic;

namespace TUGraz.VectoCommon.BusAuxiliaries {

	public interface ISSMTechnologies
	{
		IReadOnlyList<ISSMTechnology> Items { get; }

		string Source { get; }
	}

	public interface ISSMTechnology
	{
		FloorType BusFloorType { set; }

		string Category { get; set; }
		string BenefitName { get; set; }

		double LowFloorH { get; set; }
		double LowFloorV { get; set; }
		double LowFloorC { get; set; }

		double SemiLowFloorH { get; set; }
		double SemiLowFloorV { get; set; }
		double SemiLowFloorC { get; set; }

		double RaisedFloorH { get; set; }
		double RaisedFloorV { get; set; }
		double RaisedFloorC { get; set; }

		bool OnVehicle { get; set; }
		bool ActiveVH { get; set; }
		bool ActiveVV { get; set; }
		bool ActiveVC { get; set; }

		double H { get; }
		double VH { get; }
		double VV { get; }
		double VC { get; }
		double C { get; }

		bool IsEqualTo(ISSMTechnology source);
	}
}