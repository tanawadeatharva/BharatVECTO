using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.CustomerInformationFile.CustomerInformationFile_0_9;

namespace TUGraz.VectoCore.InputData.Reader.Impl.MockupRunDataFactories
{
    public class DeclarationModeMockupTruckVectoRunDataFactory : DeclarationModeTruckVectoRunDataFactory
    {
		public DeclarationModeMockupTruckVectoRunDataFactory(IDeclarationInputDataProvider dataProvider,
			IDeclarationReport report) : base(dataProvider, report, false)
		{
			
		}

		#region Overrides of AbstractDeclarationVectoRunDataFactory

		protected override IDeclarationDataAdapter DataAdapter { get; }
		protected override IEnumerable<VectoRunData> GetNextRun()
		{
			throw new NotImplementedException();
		}


		protected override VectoRunData CreateVectoRunData(IVehicleDeclarationInputData vehicle, int modeIdx, Mission mission,
			KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading)
		{
			return new VectoRunData() {
				VehicleData = new VehicleData() {
					InputData = vehicle,
				},
				Mission = mission,
				
			};
		}


		protected override void Initialize()
		{
			_segment = GetSegment(InputDataProvider.JobInputData.Vehicle);

		}

		#endregion
	}
}
