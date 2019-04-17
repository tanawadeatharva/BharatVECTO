using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.OutputData.XML.Engineering.Interfaces;

namespace TUGraz.VectoCore.OutputData.XML.Factory
{
	public interface IEngineeringWriterInjectFactory
	{
		IXMLEngineeringJobWriter CreateJobWriter(string version, IXMLEngineeringWriter writer, IEngineeringInputDataProvider inputData);
	}
}
