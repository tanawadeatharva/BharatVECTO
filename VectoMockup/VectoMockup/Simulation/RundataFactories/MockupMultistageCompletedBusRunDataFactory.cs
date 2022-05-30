using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.Impl;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.VectoMockup.Simulation.RundataFactories
{
    internal class MockupMultistageCompletedBusRunDataFactory : DeclarationModeCompletedMultistageBusVectoRunDataFactory
    {
		public MockupMultistageCompletedBusRunDataFactory(IMultistageBusInputDataProvider dataProvider,
			IDeclarationReport report) : base(dataProvider, report)
		{

		}

		#region Overrides of DeclarationModeCompletedMultistageBusVectoRunDataFactory

		protected override void Initialize()
		{

			_segmentCompletedBus = GetCompletedSegment(CompletedVehicle, PrimaryVehicle.AxleConfiguration);
			
			//base.Initialize();
		}

		protected override IEnumerable<VectoRunData> VectoRunDataHeavyBusCompleted()
		{

			return base.VectoRunDataHeavyBusCompleted();
		}

		protected override VectoRunData CreateVectoRunDataSpecific(Mission mission, KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading, int modeIdx)
		{
			return base.CreateVectoRunDataSpecific(mission, loading, modeIdx);
		}

		protected override VectoRunData CreateVectoRunDataGeneric(Mission mission, KeyValuePair<LoadingType, Tuple<Kilogram, double?>> loading, Segment primarySegment, int modeIdx)
		{
			return base.CreateVectoRunDataGeneric(mission, loading, primarySegment, modeIdx);
		}

		#endregion
	}
}
