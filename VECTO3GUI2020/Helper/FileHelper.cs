using System.IO;
using Castle.Core.Internal;

namespace VECTO3GUI2020.Helper
{
	public static class FileHelper
	{
		public static void CreateDirectory(string fileName)
		{
			var dirName = Path.GetDirectoryName(fileName);
			if (!dirName.IsNullOrEmpty()) {
				Directory.CreateDirectory(dirName);
			}
		}
	}
}