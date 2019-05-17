using TUGraz.VectoCommon.InputData;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Engineering.Interfaces
{
	public interface IXMLEngineeringDriverData : IDriverEngineeringInputData, IXMLResource
	{
		IXMLDriverDataReader Reader { set; }
	}
}
