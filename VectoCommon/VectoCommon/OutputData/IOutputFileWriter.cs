using System;
using TUGraz.VectoCommon.InputData;

namespace TUGraz.VectoCommon.OutputData
{
	public interface IOutputFileWriter
	{
		void SaveEngine(IEngineEngineeringInputData eng, string filename);

		void SaveGearbox(IGearboxEngineeringInputData gbx, IAxleGearInputData axl, string filename);

		void SaveVehicle(IVehicleEngineeringInputData vehicle, IRetarderInputData retarder,
			IPTOTransmissionInputData pto, IAngledriveInputData angledrive, string filename);

		void SaveJob(IEngineeringInputDataProvider input, string filename);

		void ExportJob(IEngineeringInputDataProvider input, string filename, bool separateFiles);
	}
}