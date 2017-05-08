using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TUGraz.VectoHashing;

namespace HashingCmd
{
	class Program
	{
		public delegate void HashingAction(VectoHash h);

		private const string Usage = @"
hashingcmd.exe -v <file.xml>

";

		private const string Help = @"
hashingcmd.exe

-h:    help
-v:    verify hashed file
-s:    create hashed file file
-c:    compute hash and write to stdout
-r:    read hash from file and write to stdout
";

		static Dictionary<string, HashingAction> actions = new Dictionary<string, HashingAction>();

		static int Main(string[] args)
		{
			try {
				if (args.Contains("-h")) {
					ShowVersionInformation();
					Console.Write(Help);
					return 0;
				}
				actions["-v"] = VerifyHashAction;
				actions["-c"] = ComputeHashAction;
				actions["-r"] = ReadHashAction;
				actions["-s"] = CreateHashedFileAction;

				var fileList = args.Except(actions.Keys);
				foreach (var file in fileList) {
					WriteLine("processing " + Path.GetFileName(file));
					foreach (var arg in args) {
						if (actions.ContainsKey(arg)) {
							try {
								var h = VectoHash.Load(file);
								actions[arg](h);
							} catch (Exception e) {
								Console.ForegroundColor = ConsoleColor.Red;
								Console.Error.WriteLine(e.Message);
								Console.ResetColor();
							}
						}
					}
				}
			} catch (Exception e) {
				Console.ForegroundColor = ConsoleColor.Red;
				Console.Error.WriteLine(e.Message);
				Console.ResetColor();

				//Console.Error.WriteLine("Please see log-file for further details (logs/log.txt)");
				Environment.ExitCode = Environment.ExitCode != 0 ? Environment.ExitCode : 1;
			}
#if DEBUG
			Console.WriteLine("done.");
			Console.ReadKey();
#endif
			return Environment.ExitCode;
		}

		private static void CreateHashedFileAction(VectoHash h)
		{
			h.AddHash();
		}

		private static void ReadHashAction(VectoHash h)
		{
			var components = h.GetContainigComponents();
			foreach (var component in components) {
				var readHash = h.ReadHash();
				WriteLine("  " + component.XMLElementName() + "\t ... " + readHash);
			}
		}

		private static void ComputeHashAction(VectoHash h)
		{
			var hash = h.ComputeHash();
			WriteLine("computed hash:  <" + hash + ">");
		}

		private static void VerifyHashAction(VectoHash h)
		{
			WriteLine("validating hashes");

			var components = h.GetContainigComponents();
			foreach (var component in components) {
				var result = h.ValidateHash(component);

				WriteLine("  " + component.XMLElementName() + "\t ... " + (result ? "valid" : "invalid"),
					result ? ConsoleColor.Green : ConsoleColor.Red);
			}
		}

		private static void WriteLine(string message, ConsoleColor foregroundColor = ConsoleColor.Gray)
		{
			Console.ForegroundColor = foregroundColor;
			Console.WriteLine(message);
			Console.ResetColor();
		}

		private static void ShowVersionInformation()
		{
			var hashingLib = AssemblyName.GetAssemblyName("VectoHashing.dll");
			WriteLine(string.Format(@"HashingLibrary: {0}", hashingLib.Version));
		}
	}
}