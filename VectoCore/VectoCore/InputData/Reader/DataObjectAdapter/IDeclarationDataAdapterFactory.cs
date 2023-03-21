using TUGraz.VectoCore.Utils.Ninject;

namespace TUGraz.VectoCore.InputData.Reader.DataObjectAdapter
{
	public interface IDeclarationDataAdapterFactory
	{
		IDeclarationDataAdapter CreateDataAdapter(
			VehicleTypeAndArchitectureStringHelperRundata.VehicleClassification vehicleClassification);
	}
}

