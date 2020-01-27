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

		string Category { get;  }
		string BenefitName { get;  }

		double LowFloorH { get;  }
		double LowFloorV { get;  }
		double LowFloorC { get;  }

		double SemiLowFloorH { get;  }
		double SemiLowFloorV { get;  }
		double SemiLowFloorC { get;  }

		double RaisedFloorH { get;  }
		double RaisedFloorV { get;  }
		double RaisedFloorC { get;  }

		//bool OnVehicle { get; set; }
		bool ActiveVH { get;  }
		bool ActiveVV { get;  }
		bool ActiveVC { get;  }

		double H { get; }
		double VH { get; }
		double VV { get; }
		double VC { get; }
		double C { get; }

		bool IsEqualTo(ISSMTechnology source);
	}
}