using System;
using System.Collections.Generic;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data;

namespace TUGraz.VectoCore.InputData.Reader.DataObjectAdapter {
	public class DeclarationDataAdapterPrimaryBus : DeclarationDataAdapterTruck
	{
		public AirdragData CreateAirdragData(Mission mission)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<VectoRunData.AuxData> CreateAuxiliaryData()
		{
			throw new NotImplementedException();
		}

		#region Overrides of DeclarationDataAdapterTruck

		public override VehicleData CreateVehicleData(IVehicleDeclarationInputData data, Mission mission, Kilogram loading)
		{
			var retVal = base.CreateVehicleData(data, mission, loading);
			retVal.CurbWeight = mission.CurbMass;
			return retVal;
		}

		#endregion
	}
}