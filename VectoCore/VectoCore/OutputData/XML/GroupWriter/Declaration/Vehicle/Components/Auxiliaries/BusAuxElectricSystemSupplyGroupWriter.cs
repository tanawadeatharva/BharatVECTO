using System;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;

namespace TUGraz.VectoCore.OutputData.XML.GroupWriter.Declaration.Vehicle.Components.Auxiliaries
{
    class BusAuxElectricSystemSupplyGroupWriter : GroupWriter, IBusAuxiliariesDeclarationGroupWriter
    {
		public BusAuxElectricSystemSupplyGroupWriter(XNamespace writerNamespace) : base(writerNamespace) { }

		#region Implementation of IBusAuxiliariesDeclarationGroupWriter

		public XElement[] GetGroupElements(IBusAuxiliariesDeclarationData aux)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
