using System.Collections.Generic;
using TUGraz.VectoCommon.InputData;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Engineering.Interfaces
{
	public interface IXMLCyclesDataProvider
	{
		IList<ICycleData> Cycles { get; }
	}
}
