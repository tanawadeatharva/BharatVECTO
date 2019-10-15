using TUGraz.VectoCore.Models.Declaration;

namespace TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules.HVAC {
	public interface ITechListBenefitLine
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

		void CloneFrom(ITechListBenefitLine source);

		bool IsEqualTo(ITechListBenefitLine source);
	}
}