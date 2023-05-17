using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;

namespace VECTO3GUI2020.Util.XML.Components
{
    public interface IXMLComponentsWriter
    {
        XElement GetComponents();
		IXMLComponentsWriter Init(IVehicleComponentsDeclaration inputData);
	}
}