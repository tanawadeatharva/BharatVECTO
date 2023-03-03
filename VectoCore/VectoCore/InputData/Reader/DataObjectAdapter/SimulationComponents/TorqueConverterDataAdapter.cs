using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;

namespace TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents
{
	public interface ITorqueConverterDataAdapter
	{
		TorqueConverterData CreateTorqueConverterData(GearboxType gearboxType,
			ITorqueConverterDeclarationInputData torqueConverter, double ratio,
			CombustionEngineData engineData);
	}
	public class TorqueConverterDataAdapter : ITorqueConverterDataAdapter
	{
		public virtual TorqueConverterData CreateTorqueConverterData(GearboxType gearboxType,
			ITorqueConverterDeclarationInputData torqueConverter, double ratio,
			CombustionEngineData engineData)
		{
			return TorqueConverterDataReader.Create(
				torqueConverter.TCData,
				DeclarationData.TorqueConverter.ReferenceRPM, DeclarationData.TorqueConverter.MaxInputSpeed,
				ExecutionMode.Declaration, ratio,
				DeclarationData.TorqueConverter.CLUpshiftMinAcceleration,
				DeclarationData.TorqueConverter.CCUpshiftMinAcceleration);
		}
	}

	public class GenericCompletedBusTorqueConverterDataAdapter : ITorqueConverterDataAdapter
	{
		private readonly GenericTorqueConverterData _genericTorqueConverterData = new GenericTorqueConverterData();
		#region Implementation of ITorqueConverterDataAdapter

		public TorqueConverterData CreateTorqueConverterData(GearboxType gearboxType,
			ITorqueConverterDeclarationInputData torqueConverter, double ratio, CombustionEngineData engineData)
		{
			if (torqueConverter != null && torqueConverter.TCData != null)
			{
				return TorqueConverterDataReader.Create(
					torqueConverter.TCData,
					DeclarationData.TorqueConverter.ReferenceRPM, DeclarationData.TorqueConverter.MaxInputSpeed,
					ExecutionMode.Engineering, ratio,
					DeclarationData.TorqueConverter.CLUpshiftMinAcceleration,
					DeclarationData.TorqueConverter.CCUpshiftMinAcceleration);
			}
			return _genericTorqueConverterData.CreateTorqueConverterData(gearboxType, ratio, engineData);
		}

		#endregion
	}
}