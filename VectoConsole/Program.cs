/*
* This file is part of VECTO.
*
* Copyright © 2012-2016 European Union
*
* Developed by Graz University of Technology,
*              Institute of Internal Combustion Engines and Thermodynamics,
*              Institute of Technical Informatics
*
* VECTO is licensed under the EUPL, Version 1.1 or - as soon they will be approved
* by the European Commission - subsequent versions of the EUPL (the "Licence");
* You may not use VECTO except in compliance with the Licence.
* You may obtain a copy of the Licence at:
*
* https://joinup.ec.europa.eu/community/eupl/og_page/eupl
*
* Unless required by applicable law or agreed to in writing, VECTO
* distributed under the Licence is distributed on an "AS IS" basis,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the Licence for the specific language governing permissions and
* limitations under the Licence.
*
* Authors:
*   Stefan Hausberger, hausberger@ivt.tugraz.at, IVT, Graz University of Technology
*   Christian Kreiner, christian.kreiner@tugraz.at, ITI, Graz University of Technology
*   Michael Krisper, michael.krisper@tugraz.at, ITI, Graz University of Technology
*   Raphael Luz, luz@ivt.tugraz.at, IVT, Graz University of Technology
*   Markus Quaritsch, markus.quaritsch@tugraz.at, IVT, Graz University of Technology
*   Martin Rexeis, rexeis@ivt.tugraz.at, IVT, Graz University of Technology
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Xml;
using NLog;
using NLog.Config;
using NLog.Targets;
using TUGraz.VectoAPI.InputData;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.FileIO;

namespace VectoConsole
{
	internal static class Program
	{
		public static List<string> WarningMessages = new List<string>();

		private static int _numLines;
		private static int ProgessCounter { get; set; }

		private const string Usage = @"Usage: vectocmd.exe [-h] [-v] FILE1.vecto [FILE2.vecto ...]";

		private const string Help = @"
Commandline Interface for Vecto.

Synopsis:
    vectocmd.exe [-h] [-v] FILE1.(vecto|xml) [FILE2.(vecto|xml) ...]

Description:
    FILE1.vecto [FILE2.vecto ...]: A list of vecto-job files (with the 
       extension: .vecto). At least one file must be given. Delimited by 
       whitespace.

    -t: output information about execution times
    -mod: write mod-data in addition to sum-data
    -eng: switch to engineering mode (implies -mod)
    -v: Shows verbose information (errors and warnings will be displayed)
	-vv: Shows more verbose information (infos will be displayed)
	-vvv: Shows debug messages (slow!)
	-vvvv: Shows all verbose information (everything, slow!)
    -V: show version information
    -h: Displays this help.

Examples:
    vecto.exe ""12t Delivery Truck.vecto"" 40t_Long_Haul_Truck.vecto
    vecto.exe 24tCoach.vecto 40t_Long_Haul_Truck.vecto
    vecto.exe -v 24tCoach.vecto
    vecto.exe -v jobs\40t_Long_Haul_Truck.vecto
	vecto.exe -h
";

		private static JobContainer _jobContainer;

		private static int Main(string[] args)
		{
			try {
				// on -h display help and terminate.
				if (args.Contains("-h")) {
					ShowVersionInformation();
					Console.Write(Help);
					return 0;
				}

				// on -v: activate verbose console logger
				var logLevel = LogLevel.Fatal;

				// Fatal > Error > Warn > Info > Debug > Trace
				var debugEnabled = false;

				if (args.Contains("-v")) {
					// display errors, warnings
					logLevel = LogLevel.Warn;
					debugEnabled = true;
				} else if (args.Contains("-vv")) {
					// also display info
					logLevel = LogLevel.Info;
					debugEnabled = true;
				} else if (args.Contains("-vvv")) {
					// display debug messages
					logLevel = LogLevel.Debug;
					debugEnabled = true;
				} else if (args.Contains("-vvvv")) {
					// display everything!
					logLevel = LogLevel.Trace;
					debugEnabled = true;
				}

				var config = LogManager.Configuration;
				config.LoggingRules.Add(new LoggingRule("*", logLevel, config.FindTargetByName("LogFile")));

				if (logLevel > LogLevel.Warn) {
					var methodCallTarget = new MethodCallTarget {
						ClassName = "VectoConsole.Program, vectocmd",
						MethodName = "LogWarning",
						Name = "WarningLogger"
					};
					methodCallTarget.Parameters.Add(new MethodCallParameter("${level}"));
					methodCallTarget.Parameters.Add(new MethodCallParameter("${message}"));
					config.LoggingRules.Add(new LoggingRule("*", LogLevel.Warn, methodCallTarget));
				}
				LogManager.Configuration = config;

				// todo mk 2016-03-02: trace listener still needed?
				Trace.Listeners.Add(new ConsoleTraceListener(true));

				if (args.Contains("-V") || debugEnabled) {
					ShowVersionInformation();
				}

				var fileList = args.Except(new[] { "-v", "-vv", "-vvv", "-vvvv", "-V", "-mod", "-eng", "-t" }).ToArray();
				var jobFiles =
					fileList.Where(
						f =>
							Path.GetExtension(f) == Constants.FileExtensions.VectoJobFile ||
							Path.GetExtension(f) == Constants.FileExtensions.VectoXMLDeclarationFile).ToList();
				//var xmlFiles = fileList.Where(f => );

				// if no other arguments given: display usage and terminate
				if (!args.Any()) {
					Console.Write(Usage);
					return 1;
				}


				var stopWatch = new Stopwatch();
				var timings = new Dictionary<string, double>();

				// process the file list and start simulation
				var fileWriter = new FileOutputWriter(fileList.First());
				var sumWriter = new SummaryDataContainer(fileWriter);
				_jobContainer = new JobContainer(sumWriter);

				var mode = ExecutionMode.Declaration;
				if (args.Contains("-eng")) {
					mode = ExecutionMode.Engineering;
					Console.ForegroundColor = ConsoleColor.White;
					Console.WriteLine(@"Switching to Engineering Mode. Make sure the job-file is saved in engineering mode!");
					Console.ResetColor();
				}

				stopWatch.Start();


				if (!jobFiles.Any()) {
					Console.ForegroundColor = ConsoleColor.Red;
					Console.WriteLine(@"No Job files found. Please restart the application with a valid '.vecto' file.");
					Console.ResetColor();
					return 1;
				}

				foreach (var file in jobFiles) {
					Console.WriteLine(@"Reading job: " + file);
					if (Path.GetExtension(file) == Constants.FileExtensions.VectoJobFile) {
						var dataProvider = JSONInputDataFactory.ReadJsonJob(file);
						var runsFactory = new SimulatorFactory(mode, dataProvider, fileWriter);

						if (args.Contains("-mod")) {
							runsFactory.WriteModalResults = true;
						}
						_jobContainer.AddRuns(runsFactory);
					}
					if (Path.GetExtension(file) == Constants.FileExtensions.VectoXMLDeclarationFile) {
						var dataProvider = new XMLInputDataProvider(new XmlTextReader(file), true);
						var runsFactory = new SimulatorFactory(ExecutionMode.Declaration, dataProvider, fileWriter);
						if (args.Contains("-mod")) {
							runsFactory.WriteModalResults = true;
						}
						_jobContainer.AddRuns(runsFactory);
					}
				}

				Console.WriteLine();
				Console.ForegroundColor = ConsoleColor.White;
				Console.WriteLine(@"Detected cycles:");
				Console.ResetColor();
				foreach (var cycle in _jobContainer.GetCycleTypes()) {
					Console.WriteLine(@"  {0}: {1}", cycle.Name, cycle.CycleType);
				}
				Console.WriteLine();

				stopWatch.Stop();
				timings.Add("Reading input files", stopWatch.Elapsed.TotalMilliseconds);
				stopWatch.Reset();

				Console.ForegroundColor = ConsoleColor.White;
				Console.WriteLine(@"Starting simulation runs");
				if (debugEnabled) {
					Console.ForegroundColor = ConsoleColor.Yellow;
					Console.WriteLine(@"Debug-Output is enabled, executing simulation runs sequentially");
				}
				Console.ResetColor();
				Console.WriteLine();

				DisplayWarnings();
				Console.WriteLine();
				stopWatch.Start();
				_jobContainer.Execute(!debugEnabled);

				Console.CancelKeyPress += (sender, e) => {
					if (e.SpecialKey == ConsoleSpecialKey.ControlC) {
						e.Cancel = true;
						_jobContainer.CancelCurrent();
					}
				};

				while (!_jobContainer.AllCompleted) {
					PrintProgress(_jobContainer.GetProgress());
					Thread.Sleep(250);
				}
				stopWatch.Stop();
				timings.Add("Simulation runs", stopWatch.Elapsed.TotalMilliseconds);


				PrintProgress(_jobContainer.GetProgress(), args.Contains("-t"));

				if (args.Contains("-t")) {
					PrintTimings(timings);
				}

				DisplayWarnings();
			} catch (Exception e) {
				Console.ForegroundColor = ConsoleColor.Red;
				Console.Error.WriteLine(e.Message);
				Console.ResetColor();

				Console.Error.WriteLine("Please see log-file for further details (logs/log.txt)");

				Environment.ExitCode = Environment.ExitCode != 0 ? Environment.ExitCode : 1;
			}

			Console.ReadKey();

			return Environment.ExitCode;
		}

		private static void DisplayWarnings()
		{
			if (WarningMessages.Any()) {
				Console.ForegroundColor = ConsoleColor.Yellow;
				foreach (var message in WarningMessages) {
					Console.Error.WriteLine(message);
				}
				Console.ResetColor();
			}
			WarningMessages.Clear();
		}

		public static void LogWarning(string level, string message)
		{
			if (level == "Warn") {
				WarningMessages.Add(message);
			}
		}

		private static void ShowVersionInformation()
		{
			var vectodll = AssemblyName.GetAssemblyName("VectoCore.dll");
			Console.WriteLine(@"VectoConsole: {0}", Assembly.GetExecutingAssembly().GetName().Version);
			Console.WriteLine(@"VectoCore: {0}", vectodll.Version);
		}

		private static void PrintProgress(Dictionary<uint, JobContainer.ProgressEntry> progessData,
			bool showTiming = true)
		{
			Console.SetCursorPosition(0, Console.CursorTop - _numLines);
			_numLines = 0;
			var sumProgress = 0.0;
			foreach (var progressEntry in progessData) {
				if (progressEntry.Value.Success) {
					Console.ForegroundColor = ConsoleColor.Green;
				} else if (progressEntry.Value.Error != null) {
					Console.ForegroundColor = ConsoleColor.Red;
				}
				var timingString = "";
				if (showTiming && progressEntry.Value.ExecTime > 0) {
					timingString = string.Format("{0,9:F2}s", progressEntry.Value.ExecTime / 1000.0);
				}
				var runName = string.Format("{0} {1} {2}", progressEntry.Value.RunName, progressEntry.Value.CycleName,
					progressEntry.Value.RunSuffix);
				Console.WriteLine(@"{0,-60} {1,8:P}{2}", runName, progressEntry.Value.Progress, timingString);
				Console.ResetColor();
				sumProgress += progressEntry.Value.Progress;
				_numLines++;
			}
			sumProgress /= _numLines;
			var spinner = "/-\\|"[ProgessCounter++ % 4];
			var bar = new string('#', (int)(sumProgress * 100.0 / 2));
			Console.WriteLine(@"   {2}   [{1,-50}]  [{0,7:P}]", sumProgress, bar, spinner);

			if (WarningMessages.Any()) {
				Console.ForegroundColor = ConsoleColor.Yellow;
				Console.WriteLine(@"Warnings: {0,5}", WarningMessages.Count);
				Console.ResetColor();
			}

			_numLines += 2;
		}

		private static void PrintTimings(Dictionary<string, double> timings)
		{
			Console.WriteLine();
			Console.WriteLine(@"---- timing information ----");
			foreach (var timing in timings) {
				Console.WriteLine(@"{0,-20}: {1:F2}s", timing.Key, timing.Value / 1000);
			}
		}
	}
}