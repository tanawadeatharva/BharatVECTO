using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.InputData.FileIO.XML.Engineering.Interfaces;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Engineering
{
	public interface IXMLDriverDataReader
	{
		ILookaheadCoastingInputData LookAheadData { get; }

		IOverSpeedEcoRollEngineeringInputData OverspeedData { get; }

		IXMLDriverAcceleration AccelerationCurveData { get; }

		IGearshiftEngineeringInputData ShiftParameters { get; }
	}
}