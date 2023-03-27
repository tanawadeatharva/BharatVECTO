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

	public static void FilesExist(params string[] files)
	{
		foreach (var file in files) {
			FileExists(file);
		}
	}

	public static void AssertNoErrorDialogs(this MockDialogHelper md)
	{
		Assert.AreEqual(0, md.NrErrors, string.Join("\n", md.Dialogs.Select(d => d.Message)));
	}

	public static void AssertErrorMessage(this MockDialogHelper md, string searchString)
	{
		Assert.That(md.Dialogs.Any(dialog => dialog.Message.Contains(searchString)));
	}
}