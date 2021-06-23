using System.CodeDom;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.InputData.FileIO.XML;

namespace Vecto3GUI2020Test
{
	public class TestHelper
	{
		private IXMLInputDataReader _inputDataReader;

		public TestHelper(IXMLInputDataReader inputDataReader)
		{
			_inputDataReader = inputDataReader;
		}




		public IInputDataProvider GetInputDataProvider(string fileName)
		{
			return _inputDataReader.Create(fileName);
		}
	}
}