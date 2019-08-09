using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.XML.Engineering.Interfaces;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Engineering
{
	public interface IXMLDriverDataReader
	{
		ILookaheadCoastingInputData LookAheadData { get; }

		IOverSpeedEcoRollEngineeringInputData OverspeedData { get; }

		IXMLDriverAcceleration AccelerationCurveData { get; }

		IGearshiftEngineeringInputData ShiftParameters { get; }
		IXMLEngineStopStartDriverData EngineStopStartData { get; }
	}

	public interface IXMLEngineStopStartDriverData
	{
		Second EngineOffStandStillActivationDelay { get; }

		Second MaxEngineOffTimespan { get; }

		double EngineStopStartUtilityFactor { get; }
	}
}