using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Castle.Components.DictionaryAdapter;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;

namespace TUGraz.VectoCore.OutputData.XML.GroupWriter.Declaration.Vehicle.Components.Auxiliaries
{
    class BusAuxElectricSystemSupplyGroupWriter_v2_10_2 : GroupWriter, IBusAuxiliariesDeclarationGroupWriter
    {
		public BusAuxElectricSystemSupplyGroupWriter_v2_10_2(XNamespace writerNamespace) : base(writerNamespace) { }

		#region Implementation of IBusAuxiliariesDeclarationGroupWriter

		public XElement[] GetGroupElements(IBusAuxiliariesDeclarationData aux)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
