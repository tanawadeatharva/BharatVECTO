using Microsoft.Win32;
using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace TUGraz.VECTO
{
	class StarterHelper
	{
		public static void StartVECTO(string[] cmdArguments, params string[] validVersions)
		{
			StartVECTO(cmdArguments, false, validVersions);
		}

        public static void StartVECTO(string[] cmdArguments, bool consoleApp, params string[] validVersions)
		{
			var path = "No path found.";
			string version = "No version found.";
			if (validVersions is null || validVersions.Length == 0) {
				validVersions = new[] { "net48", "net60" };
			}

			//var versionArgument = cmdArguments.FirstOrDefault(arg => arg.StartsWith("net"));
			try {
				version = GetHighestNETVersion();
				

				path = $"{version}\\{Assembly.GetExecutingAssembly().GetName().Name}.exe";

				string argumentsString = "";
				if (cmdArguments.Length > 0) {
					foreach (var cmdArgument in cmdArguments) {
						argumentsString += "\"" +  SanitizeInput(cmdArgument) + "\" ";
					}
				}

				var processInfo = new ProcessStartInfo(path) {
					WorkingDirectory = Directory.GetCurrentDirectory(),
					Arguments = argumentsString,
				};
				

				if (consoleApp) {
					processInfo.UseShellExecute = consoleApp ? false : processInfo.UseShellExecute;
                    Process.Start(processInfo)?.WaitForExit();
                } else {
					Process.Start(processInfo);
				}
				
				ValidateVersion(version, validVersions);
			} catch (Exception e) {
				var message = $"Error during starting VECTO.\nDetected .NET version: {version}\nTried to open path: {path}\n{e.Message}";
				File.AppendAllText("LOG.txt", $"{DateTime.Now} {message}\n");
				Console.WriteLine(message);
				throw new Exception(message);
			}
		}

        public static string SanitizeInput(string input)
        {
            var disallowedChars = new char[] { '&', ';', '|', '$' };

            foreach (var c in disallowedChars)
            {
                input = input.Replace(c.ToString(), string.Empty);
            }

            return input;
        }

        private static void ValidateVersion(string version, params string[] validVersions)
		{
			if (!((IList)validVersions).Contains(version))
				throw new Exception($"Invalid .NET Version supplied. Only the following values are valid: {string.Join(", ", validVersions)}");
		}
		
		private static string GetHighestNETVersion()
		{
			if (SupportsNet60()) {
				return "net60";
			}

			return "net48";
			//if (SupportsNet48()) {
			//	return "net48";
			//}

			//return "net48";
		}

		private static bool SupportsNet60()
		{
			try {
				var p = Process.Start(new ProcessStartInfo("dotnet", "--list-runtimes") {
					CreateNoWindow = true,
					UseShellExecute = false,
					RedirectStandardError = true,
					RedirectStandardOutput = true
				});

				p.WaitForExit();
				var output = p.StandardOutput.ReadToEnd();
				return output.Contains("Microsoft.WindowsDesktop.App 6");
			} catch (Exception e) {
				Console.WriteLine(e);
				File.AppendAllText("LOG.txt", $"{DateTime.Now} {e.Message}\n");
			}

			return false;
		}

		private static bool SupportsNet48()
		{
			const string subkey = @"SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full\";
			using (var ndpKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32).OpenSubKey(subkey)) {
				if (ndpKey != null && ndpKey.GetValue("Release") != null) {
					var releaseKey = (int)ndpKey.GetValue("Release");
					return releaseKey >= 528040;
				}

				return false;
			}
		}

	}
}
