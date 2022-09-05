using System;
using System.Collections.Generic;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.VectoCore.InputData.Reader.Impl.DeclarationMode.PrimaryBusRunDataFactory
{
	public abstract class DeclarationModePrimaryBusRunDataFactory
	{


		public abstract class PrimaryBusBase : IVectoRunDataFactory
		{
			#region Implementation of IVectoRunDataFactory

			public IDeclarationDataAdapter DataAdapter { get; }
			public IDeclarationInputDataProvider DataProvider { get; }

			public IDeclarationReport Report { get; }
       
            protected PrimaryBusBase(IDeclarationInputDataProvider dataProvider, IDeclarationReport report,
				IDeclarationDataAdapter declarationDataAdapter)
            {
				DataAdapter = declarationDataAdapter;
				DataProvider = dataProvider;
				Report = report;
			}



			public IEnumerable<VectoRunData> NextRun()
			{
				throw new NotImplementedException();
			}

			#endregion
		}

		public class Conventional : PrimaryBusBase
		{
			public Conventional(IDeclarationInputDataProvider dataProvider, IDeclarationReport report,
				IDeclarationDataAdapter declarationDataAdapter) : base(dataProvider, report, declarationDataAdapter) { }
		}

		public class HEV_S2 : PrimaryBusBase
		{
			public HEV_S2(IDeclarationInputDataProvider dataProvider, IDeclarationReport report,
				IDeclarationDataAdapter declarationDataAdapter) : base(dataProvider, report, declarationDataAdapter) { }
		}

		public class HEV_S3 : PrimaryBusBase
		{
			public HEV_S3(IDeclarationInputDataProvider dataProvider, IDeclarationReport report,
				IDeclarationDataAdapter declarationDataAdapter) : base(dataProvider, report, declarationDataAdapter) { }
		}

		public class HEV_S4 : PrimaryBusBase
		{
			public HEV_S4(IDeclarationInputDataProvider dataProvider, IDeclarationReport report, IDeclarationDataAdapter declarationDataAdapter) : base(dataProvider, report, declarationDataAdapter) { }
		}

		public class HEV_S_IEPC : PrimaryBusBase
		{
			public HEV_S_IEPC(IDeclarationInputDataProvider dataProvider, IDeclarationReport report, IDeclarationDataAdapter declarationDataAdapter) : base(dataProvider, report, declarationDataAdapter) { }
		}

		public class HEV_P1 : PrimaryBusBase
		{
			public HEV_P1(IDeclarationInputDataProvider dataProvider, IDeclarationReport report, IDeclarationDataAdapter declarationDataAdapter) : base(dataProvider, report, declarationDataAdapter) { }
		}

		public class HEV_P2 : PrimaryBusBase
		{
			public HEV_P2(IDeclarationInputDataProvider dataProvider, IDeclarationReport report, IDeclarationDataAdapter declarationDataAdapter) : base(dataProvider, report, declarationDataAdapter) { }
		}

		public class HEV_P2_5 : PrimaryBusBase
		{
			public HEV_P2_5(IDeclarationInputDataProvider dataProvider, IDeclarationReport report, IDeclarationDataAdapter declarationDataAdapter) : base(dataProvider, report, declarationDataAdapter) { }
		}

		public class HEV_P3 : PrimaryBusBase
		{
			public HEV_P3(IDeclarationInputDataProvider dataProvider, IDeclarationReport report, IDeclarationDataAdapter declarationDataAdapter) : base(dataProvider, report, declarationDataAdapter) { }
		}

		public class HEV_P4 : PrimaryBusBase
		{
			public HEV_P4(IDeclarationInputDataProvider dataProvider, IDeclarationReport report, IDeclarationDataAdapter declarationDataAdapter) : base(dataProvider, report, declarationDataAdapter) { }
		}


		public class PEV_E2 : PrimaryBusBase
		{
			public PEV_E2(IDeclarationInputDataProvider dataProvider, IDeclarationReport report, IDeclarationDataAdapter declarationDataAdapter) : base(dataProvider, report, declarationDataAdapter) { }
		}

		public class PEV_E3 : PrimaryBusBase
		{
			public PEV_E3(IDeclarationInputDataProvider dataProvider, IDeclarationReport report, IDeclarationDataAdapter declarationDataAdapter) : base(dataProvider, report, declarationDataAdapter) { }
		}

		public class PEV_E4 : PrimaryBusBase
		{
			public PEV_E4(IDeclarationInputDataProvider dataProvider, IDeclarationReport report, IDeclarationDataAdapter declarationDataAdapter) : base(dataProvider, report, declarationDataAdapter) { }
		}

		public class PEV_E_IEPC : PrimaryBusBase
		{
			public PEV_E_IEPC(IDeclarationInputDataProvider dataProvider, IDeclarationReport report, IDeclarationDataAdapter declarationDataAdapter) : base(dataProvider, report, declarationDataAdapter) { }
		}

		public class Exempted : PrimaryBusBase
		{
			public Exempted(IDeclarationInputDataProvider dataProvider, IDeclarationReport report, IDeclarationDataAdapter declarationDataAdapter) : base(dataProvider, report, declarationDataAdapter) { }
		}
	}
}