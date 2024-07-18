using Castle.Core.Internal;
using ErrorOr;
using ShellProgressBar;
using System;
using System.Reflection;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Utils;
using XMLConverterLibrary;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace XMLConverterCmdLine
{
	internal class Program
	{
		private static List<IXMLConverter> _xmlConverters;
		private static string _targetVersion;
		private static bool _verbose;

		private const int SUCCESS_CODE = 0;
		private const int FAILURE_CODE = 1;

		private const string EXECUTABLE = "XMLConverterCmdLine.exe";
		private const string HELP_OPTION = "-h";
		private const string LIST_OPTION = "-l";
		private const string TARGET_OPTION = "--version";
		private const string VERBOSE_OPTION = "-v";

		private static readonly string Usage = $@"
Usage:
	{EXECUTABLE} [{HELP_OPTION}] [{LIST_OPTION}] [{VERBOSE_OPTION}] [{TARGET_OPTION}='vX.Y'] INPUT.xml
	{EXECUTABLE} [{HELP_OPTION}] [{LIST_OPTION}] [{VERBOSE_OPTION}] [{TARGET_OPTION}='vX.Y'] INPUT_FOLDER

";

		private static readonly string Help = $@"
Command-line Interface for the XML Converter tool.

Synopsis:
	{EXECUTABLE} [{HELP_OPTION}] [{LIST_OPTION}] [{VERBOSE_OPTION}] [{TARGET_OPTION}='vX.Y'] INPUT.xml
	{EXECUTABLE} [{HELP_OPTION}] [{LIST_OPTION}] [{VERBOSE_OPTION}] [{TARGET_OPTION}='vX.Y'] INPUT_FOLDER

Description:
	This tool converts XML VECTO files from one version to another.
	For jobs/vehicles 'version' refers to the version of the vehicle which is defined 
	in namespace urn:tugraz:ivt:VectoAPI:DeclarationDefinitions:vX.Y.
	The converted XML file will be placed inside a folder named '{XMLFileWriter.OUTPUT_FOLDER}', 
	located inside the parent folder of the original XML file.

	INPUT.xml: An XML VECTO file (e.g. a job file). 
	INPUT_FOLDER: A folder containing VECTO files in XML format.
	
	{TARGET_OPTION}: Target version
	{LIST_OPTION}: List available conversions
	{HELP_OPTION}: Displays this help text
	{VERBOSE_OPTION}: Verbose mode, displays a message for each XML file successfully converted
	
Examples:
	{EXECUTABLE} {TARGET_OPTION}='v2.4' my_job.xml
	{EXECUTABLE} {VERBOSE_OPTION} {TARGET_OPTION}='v2.4' my_folder_containing_xml_jobs
	{EXECUTABLE} {LIST_OPTION}
	{EXECUTABLE} {HELP_OPTION}
";

		static Program() 
		{
			var jobConverter = new XMLConverter(
					new XMLFileReader(),
					new XMLJobConverterFactory(),
					new XMLFileWriter()
				);

			_xmlConverters = new List<IXMLConverter>() { jobConverter };
			
			_targetVersion = "";
			_verbose = false;
		}

		static int Main(string[] args)
		{
			try
			{
				if (args.Length == 0)
				{
					WriteErrorLine(Usage, ConsoleColor.Gray);
					return FAILURE_CODE;
				}

				if (args.AsEnumerable().Contains(HELP_OPTION))
				{
					ShowVersionInformation();
					WriteLine(Help);
					return SUCCESS_CODE;
				}

				if (args.AsEnumerable().Contains(LIST_OPTION))
				{
					PrintAvailableConversions();
					return SUCCESS_CODE;
				}

				if (args.AsEnumerable().Contains(VERBOSE_OPTION))
				{
					_verbose = true;
				}

				if (args.AsEnumerable().Count(x => x.StartsWith($"{TARGET_OPTION}=")) == 0)
				{
					WriteErrorLine($"No target version specified! Use {TARGET_OPTION}='vX.Y' to specify target version.", ConsoleColor.Red);
					return FAILURE_CODE;
				}

				if (!ParseTargetVersion(args))
				{
					WriteErrorLine($"'{_targetVersion}' is not a valid target version!");
					return FAILURE_CODE;
				}
				
				var parseInputFileResult = ParseInputFile(args);
				if (parseInputFileResult.IsError)
				{
					WriteErrorLine(string.Join(Environment.NewLine, parseInputFileResult.Errors.Select(x => x.Description)));
					return FAILURE_CODE;
				}

				var resultMsgs = ExecuteConversions(parseInputFileResult.Value);

				resultMsgs.ForEach(x => { Console.WriteLine(x); });
			}
			catch (Exception e)
			{
				WriteErrorLine(e.Message);
				Environment.ExitCode = (Environment.ExitCode != SUCCESS_CODE) ? Environment.ExitCode : FAILURE_CODE;
			}

			return Environment.ExitCode;
		}

		private static List<string> ExecuteConversions(string file)
		{
			var converter = _xmlConverters.Where(x => (x.DocumentType == XmlDocumentType.DeclarationJobData)
				&& x.SupportedConversions.Any(y => y.Item2 == _targetVersion)).First();

			var resultMsgs = new List<string>();

			var xmlFiles = (File.GetAttributes(file).HasFlag(FileAttributes.Directory)
				? Directory.GetFiles(file, "*.xml", SearchOption.AllDirectories)
				: new string[] { file })
					.ToList()
					.Where(x => !x.Contains(XMLFileWriter.OUTPUT_FOLDER)); 

			ProgressBar progressBar = new ProgressBar(xmlFiles.Count(), "files processed", new ProgressBarOptions { ProgressBarOnBottom = true });

			foreach (var item in xmlFiles)
			{
				var conversionResult = converter.Convert(item, _targetVersion);

				progressBar.Tick();

				if (conversionResult.IsError)
				{
					resultMsgs.Add($"{item}: ERROR: " + string.Join(Environment.NewLine, conversionResult.Errors.Select(x => x.Description)));
				}
				else
				{
					if (_verbose)
					{
						resultMsgs.Add($"{item}: converted!");
					}
				}
			}

			progressBar.Dispose();

            return resultMsgs;
		}

		private static bool ParseTargetVersion(string[] args)
		{
			var target = args.Find(x => x.StartsWith(TARGET_OPTION));

			_targetVersion = target.Substring(target.IndexOf('=') + 1).Trim().Trim('\'');

			var validVersions = _xmlConverters
				.Select(x => x.SupportedConversions.Select(y => y.Item2)).Aggregate((c, d) => c.Concat(d)).Distinct();
				
			return validVersions.Contains(_targetVersion);
		}

		private static ErrorOr<string> ParseInputFile(string[] args)
		{
			var fileList = args.Where(x => !x.StartsWith(TARGET_OPTION) && (x != VERBOSE_OPTION)).ToList();

			if (fileList.Count == 0)
			{
				return Error.NotFound(description: $"No file or folder specified for conversion!");
			}

			var file = fileList.First();

			if (!File.Exists(file) && !Directory.Exists(file))
			{
				return Error.NotFound(description: $"File/folder '{file}' does not exist!");
			}

			bool isFolder = File.GetAttributes(file).HasFlag(FileAttributes.Directory);

			if (!isFolder && Path.GetExtension(file) != ".xml") 
			{
				return Error.Validation(description: $"File '{file}' is not XML!");
			}

			return file;
		}

		private static void PrintAvailableConversions()
		{
			Console.WriteLine("");
			Console.WriteLine("Available Conversions");
			Console.WriteLine("---------------------");

			_xmlConverters.ForEach(x => 
			{
				Console.WriteLine($"Type: {GetUserFriendlyTypeDescription(x.DocumentType)}");

				foreach (var item in x.SupportedConversions)
				{
					Console.WriteLine($"\t{item.Item1} -> {item.Item2}");
				}

				Console.WriteLine("");
			});
		}

		private static string GetUserFriendlyTypeDescription(XmlDocumentType type)
		{
			if (type == XmlDocumentType.DeclarationJobData)
			{
				var line1 = $"VECTO Job XML [root element: 'VectoInputDeclaration', child element: 'Vehicle']";
				var line2 = $"The 'Vehicle' element may also contain sub-elements whose type is defined in ";
				var line3 = $"{XMLDefinitions.DECLARATION_NAMESPACE} v2.2, v2.3, and v2.5.";
				var line4 = $"\nThe following conversions are supported for 'Vehicle' defined in {XMLDefinitions.DECLARATION_NAMESPACE}:";

				return $"{line1}\n{line2}\n{line3}\n{line4}";
			}
			else
			{
				return type.ToString();
			}
		}

		private static void ShowVersionInformation()
		{
			WriteLine($@"XMLConverterCmdLine: {Assembly.GetExecutingAssembly().GetName().Version}");
			WriteLine($@"VectoCore: {VectoSimulationCore.VersionNumber}");
		}

		private static void WriteErrorLine(string message, ConsoleColor foregroundColor = ConsoleColor.Red)
		{
			Console.ForegroundColor = foregroundColor;
			Console.Error.WriteLine(message);
			Console.ResetColor();
		}

		private static void WriteLine(string message, ConsoleColor foregroundColor = ConsoleColor.Gray)
		{
			Console.ForegroundColor = foregroundColor;
			Console.WriteLine(message);
			Console.ResetColor();
		}

	}
}
