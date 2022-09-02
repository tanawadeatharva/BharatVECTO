using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.VectoCore.InputData.Reader.Impl.DeclarationMode.HeavyLorryRunDataFactory
{
	public abstract partial class DeclarationModeHeavyLorryRunDataFactory
	{
		public class HEV_S3 : LorryBase
		{
			public HEV_S3(IDeclarationInputDataProvider dataProvider, IDeclarationReport report,
				IDeclarationDataAdapter declarationDataAdapter) : base(dataProvider, report, declarationDataAdapter) { }
		}
	}
}