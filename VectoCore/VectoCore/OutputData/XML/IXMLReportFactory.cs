using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.InputData;

namespace TUGraz.VectoCore.OutputData.XML
{
    public interface IXMLDeclarationReportFactory
    {
        //Creates a XMLDeclarationReport based on the type of the first argument
		IDeclarationReport CreateReport(IInputDataProvider input, IOutputDataWriter outputWriter);
		IVTPReport CreateVTPReport(IVTPDeclarationInputDataProvider input, IOutputDataWriter outputWriter);

		IVTPReport CreateVTPReport(IInputDataProvider input, IOutputDataWriter outputWriter);
	}
}
