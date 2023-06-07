using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.Utils;

namespace Vecto3GUI2020Test.MockInput;

static internal class MockInput
{
	public static DataSource GetMockDataSource(string typeName, XNamespace version)
	{
		return new DataSource()
		{
			SourceFile = "Mocked",
			SourceType = DataSourceType.Missing,
			Type = typeName,
			TypeVersion = version.NamespaceName,
			SourceVersion = version.GetVersionFromNamespaceUri(),
		};
	}
}