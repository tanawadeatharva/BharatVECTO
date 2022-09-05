using System;
using System.Collections.Generic;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.VectoCore.InputData.Reader.Impl.DeclarationMode.CompletedBusRunDataFactory
{
	public abstract class DeclarationModeCompletedBusRunDataFactory
	{
		public abstract class CompletedBusBase : IVectoRunDataFactory
		{
			public IDeclarationDataAdapter GenericDataAdapter { get; }
			public IDeclarationDataAdapter SpecificDataAdapter { get; }
			public IMultistageVIFInputData DataProvider { get; }

			public CompletedBusBase(IMultistageVIFInputData dataProvider, 
				ISpecificCompletedBusDataAdapter specificDataAdapter, 
				IGenericCompletedBusDataAdapter genericDataAdapter)
			{
				SpecificDataAdapter = specificDataAdapter;
				GenericDataAdapter = genericDataAdapter;
				DataProvider = dataProvider;
			}



			#region Implementation of IVectoRunDataFactory
			public IEnumerable<VectoRunData> NextRun()
			{
				throw new NotImplementedException();
			}

			#endregion
		}


		public class Conventional :
			CompletedBusBase
		{
			public Conventional(IMultistageVIFInputData dataProvider,
				ISpecificCompletedBusDataAdapter specificDataAdapter,
				IGenericCompletedBusDataAdapter genericDataAdapter) : base(dataProvider, specificDataAdapter,
				genericDataAdapter) { }
		}
		public class HEV_S2 : CompletedBusBase
		{
			public HEV_S2(IMultistageVIFInputData dataProvider, ISpecificCompletedBusDataAdapter specificDataAdapter, IGenericCompletedBusDataAdapter genericDataAdapter) : base(dataProvider, specificDataAdapter, genericDataAdapter) { }
		}
		public class HEV_S3 : CompletedBusBase
		{
			public HEV_S3(IMultistageVIFInputData dataProvider, ISpecificCompletedBusDataAdapter specificDataAdapter, IGenericCompletedBusDataAdapter genericDataAdapter) : base(dataProvider, specificDataAdapter, genericDataAdapter) { }
		}
		public class HEV_S4 : CompletedBusBase
		{
			public HEV_S4(IMultistageVIFInputData dataProvider, ISpecificCompletedBusDataAdapter specificDataAdapter, IGenericCompletedBusDataAdapter genericDataAdapter) : base(dataProvider, specificDataAdapter, genericDataAdapter) { }
		}
		public class HEV_S_IEPC : CompletedBusBase
		{
			public HEV_S_IEPC(IMultistageVIFInputData dataProvider, ISpecificCompletedBusDataAdapter specificDataAdapter, IGenericCompletedBusDataAdapter genericDataAdapter) : base(dataProvider, specificDataAdapter, genericDataAdapter) { }
		}
		public class HEV_P1 : CompletedBusBase
		{
			public HEV_P1(IMultistageVIFInputData dataProvider, ISpecificCompletedBusDataAdapter specificDataAdapter, IGenericCompletedBusDataAdapter genericDataAdapter) : base(dataProvider, specificDataAdapter, genericDataAdapter) { }
		}
		public class HEV_P2 : CompletedBusBase
		{
			public HEV_P2(IMultistageVIFInputData dataProvider, ISpecificCompletedBusDataAdapter specificDataAdapter, IGenericCompletedBusDataAdapter genericDataAdapter) : base(dataProvider, specificDataAdapter, genericDataAdapter) { }
		}
		public class HEV_P2_5 : CompletedBusBase
		{
			public HEV_P2_5(IMultistageVIFInputData dataProvider, ISpecificCompletedBusDataAdapter specificDataAdapter, IGenericCompletedBusDataAdapter genericDataAdapter) : base(dataProvider, specificDataAdapter, genericDataAdapter) { }
		}
		public class HEV_P3 : CompletedBusBase
		{
			public HEV_P3(IMultistageVIFInputData dataProvider, ISpecificCompletedBusDataAdapter specificDataAdapter, IGenericCompletedBusDataAdapter genericDataAdapter) : base(dataProvider, specificDataAdapter, genericDataAdapter) { }
		}
		public class HEV_P4 : CompletedBusBase
		{
			public HEV_P4(IMultistageVIFInputData dataProvider, ISpecificCompletedBusDataAdapter specificDataAdapter, IGenericCompletedBusDataAdapter genericDataAdapter) : base(dataProvider, specificDataAdapter, genericDataAdapter) { }
		}
		public class PEV_E1 : CompletedBusBase
		{
			public PEV_E1(IMultistageVIFInputData dataProvider, ISpecificCompletedBusDataAdapter specificDataAdapter, IGenericCompletedBusDataAdapter genericDataAdapter) : base(dataProvider, specificDataAdapter, genericDataAdapter) { }
		}
		public class PEV_E2 : CompletedBusBase
		{
			public PEV_E2(IMultistageVIFInputData dataProvider, ISpecificCompletedBusDataAdapter specificDataAdapter, IGenericCompletedBusDataAdapter genericDataAdapter) : base(dataProvider, specificDataAdapter, genericDataAdapter) { }
		}
		public class PEV_E3 : CompletedBusBase
		{
			public PEV_E3(IMultistageVIFInputData dataProvider, ISpecificCompletedBusDataAdapter specificDataAdapter, IGenericCompletedBusDataAdapter genericDataAdapter) : base(dataProvider, specificDataAdapter, genericDataAdapter) { }
		}
		public class PEV_E4 : CompletedBusBase
		{
			public PEV_E4(IMultistageVIFInputData dataProvider, ISpecificCompletedBusDataAdapter specificDataAdapter, IGenericCompletedBusDataAdapter genericDataAdapter) : base(dataProvider, specificDataAdapter, genericDataAdapter) { }
		}
		public class PEV_E_IEPC : CompletedBusBase
		{
			public PEV_E_IEPC(IMultistageVIFInputData dataProvider, ISpecificCompletedBusDataAdapter specificDataAdapter, IGenericCompletedBusDataAdapter genericDataAdapter) : base(dataProvider, specificDataAdapter, genericDataAdapter) { }
		}
	}
}