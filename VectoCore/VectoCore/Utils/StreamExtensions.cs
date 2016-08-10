using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TUGraz.VectoCore.Utils
{
	internal static class StreamExtensions
	{
		public static IEnumerable<string> ReadLines(this Stream stream)
		{
			using (var reader = new StreamReader(stream, Encoding.UTF8)) {
				while (!reader.EndOfStream) {
					yield return reader.ReadLine();
				}
			}
		}
	}
}