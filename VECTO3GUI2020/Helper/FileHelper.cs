using System.IO;

namespace VECTO3GUI2020.Helper
{
	public static class FileHelper
	{
		public static void CreateDirectory(string fileName)
		{
			Path.GetDirectoryName(fileName);
			Directory.CreateDirectory(fileName);
		}
	}
}