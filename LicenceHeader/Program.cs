/*
* Copyright 2015 European Union
*
* Licensed under the EUPL (the "Licence");
* You may not use this work except in compliance with the Licence.
* You may obtain a copy of the Licence at:
*
* http://ec.europa.eu/idabc/eupl5
*
* Unless required by applicable law or agreed to in writing, software 
* distributed under the Licence is distributed on an "AS IS" basis,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the Licence for the specific language governing permissions and 
* limitations under the Licence.
*/

using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace LicenceHeader
{
	/// <summary>
	/// Updates/Adds the EUPL license header to every .cs file in the solution directory
	/// </summary>
	internal class Program
	{
		private const string SolutionRootDirectory = @"..\\..\\..";

		private static void Main()
		{
			var licence = File.ReadAllText("header.txt");
			var count = 0;

			foreach (var file in Directory.GetFiles(SolutionRootDirectory, "*.cs", SearchOption.AllDirectories)) {
				if (file.Contains("\\obj\\") || file.Contains("\\bin\\")) {
					continue;
				}

				var re = new Regex("^.*?(?=using|namespace)");
				var content = File.ReadAllText(file, Encoding.Default);
				var updatedContent = re.Replace(content, licence);
				if (updatedContent != content) {
					File.WriteAllText(file, updatedContent, Encoding.Default);
					Console.WriteLine("Updated " + file);
					count++;
				}
			}
			Console.WriteLine("Finished. Updated {0} files.", count);
			Console.ReadKey();
		}
	}
}