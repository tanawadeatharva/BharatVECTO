using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.XML.Engineering.Interfaces;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Engineering.Reader
{
	public interface IXMLDriverDataReader
	{
		ILookaheadCoastingInputData LookAheadData { get; }

		IOverSpeedEngineeringInputData OverspeedData { get; }

		IXMLDriverAcceleration AccelerationCurveData { get; }

		IGearshiftEngineeringInputData ShiftParameters { get; }
		IEngineStopStartEngineeringInputData EngineStopStartData { get; }
		IEcoRollEngineeringInputData EcoRollData { get; }

		IPCCEngineeringInputData PCCData { get; }
		
	}

}