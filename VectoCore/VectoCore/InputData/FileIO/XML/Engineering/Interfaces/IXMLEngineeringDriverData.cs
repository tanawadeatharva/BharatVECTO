using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.InputData.FileIO.XML.Engineering.Reader;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Engineering.Interfaces
{
	public interface IXMLEngineeringDriverData : IDriverEngineeringInputData, IXMLResource
	{
		IXMLDriverDataReader Reader { set; }
	}
}
