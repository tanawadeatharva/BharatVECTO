using System;
using Microsoft.Win32;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace TUGraz.VECTO
{
	class Program
	{
		static void Main(string[] args)
		{
			try {
				string version;
				if (args.Length > 0) {
					version = args[0];
					ValidateVersion(version);
				} else {
					version = GetHighestNETVersion();
				}
				Process.Start(new ProcessStartInfo($"{version}\\{Assembly.GetExecutingAssembly().GetName().Name}.exe") {
					WorkingDirectory = Directory.GetCurrentDirectory()
				});
			} catch (Exception e) {
				Console.WriteLine(e);
				File.AppendAllText("LOG.txt", e.ToString());
				throw;
			}
		}

		private static void ValidateVersion(string version)
		{
			var validVersions = new[] { "net45", "net48", "net60" };
			if (!validVersions.Contains(version))
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
