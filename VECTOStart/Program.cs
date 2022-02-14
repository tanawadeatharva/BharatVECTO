using System;
using Microsoft.Win32;
using System.Diagnostics;

namespace TUGraz.VECTO
{
	class Program
	{
		static void Main()
		{
			var version = GetHighestNETVersion();
			try {
				Process.Start(new ProcessStartInfo($"{version}\\VECTO.exe") { CreateNoWindow = true });
			} catch (Exception e) {
				Console.WriteLine($"Could not start VECTO with {version}: {e.Message}");
				Console.ReadKey();
			}
		}

		private static string GetHighestNETVersion()
		{
			if (SupportsNet50()) {
				return "net5.0";
			} 
			
			if (SupportsNET48()) {
				return "net48";
			}

			return "net45";
		}

		private static bool SupportsNet50()
		{
			var p = Process.Start(new ProcessStartInfo("dotnet", "--list-runtimes") {
				CreateNoWindow = true,
				UseShellExecute = false,
				RedirectStandardError = true,
				RedirectStandardOutput = true
			}
			);

			p.WaitForExit();
			var output = p.StandardOutput.ReadToEnd();
			return output.Contains("Microsoft.WindowsDesktop.App 5.0");
		}

		private static bool SupportsNET48()
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
