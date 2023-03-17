using System.IO;
using System.Linq;
using NUnit.Framework;
using TUGraz.VectoCommon.Utils;

namespace Vecto3GUI2020Test.Utils;

public static class AssertHelper
{
	public static void FileExists(string fileName)
	{
		if (!File.Exists(fileName)) {
			Assert.Fail($"File {fileName} not found!");
		}
	}


	public static void AssertNoErrorDialogs(this MockDialogHelper md)
	{
		Assert.AreEqual(0, md.NrErrors, string.Join("\n", md.Dialogs.Select(d => d.Message)));
	}
}