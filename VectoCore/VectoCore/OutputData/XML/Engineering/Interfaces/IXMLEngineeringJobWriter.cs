using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.OutputData.XML.Engineering.Interfaces;

namespace TUGraz.VectoCore.OutputData.XML
{
	public interface IXMLEngineeringJobWriter
	{
		IEngineeringInputDataProvider InputData { set; }
		IXMLEngineeringWriter Writer { set; }

		object[] WriteXML();
	}
}
