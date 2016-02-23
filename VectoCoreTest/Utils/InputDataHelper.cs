using System.IO;

namespace TUGraz.VectoCore.Tests.Utils
{
	public class InputDataHelper
	{
		public static MemoryStream InputDataAsStream(string header, string[] entries)
		{
			var cycleData = new MemoryStream();
			var writer = new StreamWriter(cycleData);
			writer.WriteLine(header);
			foreach (var entry in entries) {
				writer.WriteLine(entry);
			}
			writer.Flush();
			cycleData.Seek(0, SeekOrigin.Begin);
			return cycleData;
		}
	}
}