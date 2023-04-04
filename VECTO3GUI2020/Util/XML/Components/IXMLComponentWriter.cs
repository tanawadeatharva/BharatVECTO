using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;

namespace VECTO3GUI2020.Util.XML.Components
{
    public interface IXMLComponentWriter
    {
        XElement GetElement();
		IXMLComponentWriter Init(IComponentInputData inputData);
	}
}
