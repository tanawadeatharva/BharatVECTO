using System;
using System.IO;
using System.Text.RegularExpressions;

namespace LicenceHeader
{
	/// <summary>
	/// Updates/Adds the EUPL license header to every .cs file in the projects: VectoCore, VectoConsole, VectoCoreTest
	/// </summary>
	internal class Program
	{
		private const string SolutionRootDirectory = @"..\\..\\..";
		private static readonly string[] ProjectDirectories = { "VectoCore", "VectoConsole", "VectoCoreTest" };

		private static void Main()
		{
			var licence = File.ReadAllText("header.txt");

			foreach (var dir in ProjectDirectories) {
				foreach (
					var file in Directory.GetFiles(Path.Combine(SolutionRootDirectory, dir), "*.cs", SearchOption.AllDirectories)) {
					if (file.Contains("\\obj\\") || file.Contains("\\bin\\")) {
						continue;
					}

					var re = new Regex("^.*?(?=using|namespace)");
					var content = File.ReadAllText(file);
					var updatedContent = re.Replace(content, licence);
					if (updatedContent != content) {
						File.WriteAllText(file, updatedContent);
					}
					Console.WriteLine(file);
				}
			}
		}
	}
}