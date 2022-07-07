using Microsoft.Win32;
using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace TUGraz.VECTO
{
	class StarterHelper
	{
		public static void StartVECTO(string[] args, params string[] validVersions)
		{
			var path = "No path found.";
			string version = "No version found.";
			if (validVersions is null) {
				validVersions = new[] { "net45", "net48", "net60" };
			}
			try {
				if (args.Length > 0) {
					version = args[0].ToLower();
				} else {
					version = StarterHelper.GetHighestNETVersion();
				}

				path = $"{version}\\{Assembly.GetExecutingAssembly().GetName().Name}.exe";
				Process.Start(new ProcessStartInfo(path) {
					WorkingDirectory = Directory.GetCurrentDirectory()
				});
				ValidateVersion(version, validVersions);
			} catch (Exception e) {
				var message = $"Error during starting VECTO.\nDetected .NET version: {version}\nTried to open path: {path}\n{e.Message}";
				File.AppendAllText("LOG.txt", $"{DateTime.Now} {message}\n");
				Console.WriteLine(message);
				MessageBox.Show(message);
			}
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

			if (SupportsNet48()) {
				return "net48";
			}

			return "net45";
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
