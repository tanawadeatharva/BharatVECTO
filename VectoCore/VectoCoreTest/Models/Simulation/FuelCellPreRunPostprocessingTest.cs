using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.VisualBasic.FileIO;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using Moq;
using NUnit.Framework;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.Simulation.Impl.SimulatorFactory;
using TUGraz.VectoCore.Models.SimulationComponent;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.FileIO;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Tests.Models.Simulation;


[TestFixture]
public class FuelCellPreRunPostprocessing
{

	[TestCase()]
	public void FuelCellPreRunPostprocessingTest()
	{

	}

	[TestCase()]
	public void FuelCellPostProcessing_DistanceWindow()
	{
		string jobFile = "TestData/H2_FCV/GenericVehicleE2 - FCHV/FCHV_singleFc.vecto";
		int cycleIdx = 0;

        var inputProvider = JSONInputDataFactory.ReadJsonJob(jobFile);

		var writer = new FileOutputWriter(jobFile);
		var factory = SimulatorFactory.CreateSimulatorFactory(ExecutionMode.Engineering, inputProvider, writer);
		factory.Validate = false;
		factory.WriteModalResults = true;

		var sumContainer = new SummaryDataContainer(writer);
		var jobContainer = new JobContainer(sumContainer);

		factory.SumData = sumContainer;

		var run = factory.SimulationRuns().ToArray()[cycleIdx];
		run.GetContainer().RunData.IterativeRunStrategy = null;

		Assert.NotNull(run);

		var pt = run.GetContainer();

		Assert.NotNull(pt);

		run.Run();
		Assert.IsTrue(run.FinishedWithoutErrors);
    }


	protected VectoRunData GetVectoRunData()
	{
		var mockCycle = new Mock<IDrivingCycleData>();
		mockCycle.Setup(x => x.CycleType).Returns(CycleType.DistanceBased);

		return new VectoRunData() {
			Cycle = mockCycle.Object,
		};
    }


	public class ModalResultsTest : ModalResults
	{
		private const string Delimiter = ",";
		private const string Comment = "#";
		private static readonly Regex HeaderFilter = new Regex(@"\[.*?\]|\<|\>", RegexOptions.Compiled);

        public ModalResultsTest(string modDataFile)
		{
			var xml = Path.ChangeExtension(modDataFile, ".vmod.xml");
			if (File.Exists(xml)) {
				ReadXml(XmlTextReader.Create(xml));
				return;
			}


            TableName = $"ModData - {modDataFile}";
			using (var fs = File.OpenText(modDataFile)) {
				var p = new TextFieldParser(fs) {
					TextFieldType = FieldType.Delimited,
					Delimiters = new[] { Delimiter },
					CommentTokens = new[] { Comment },
					HasFieldsEnclosedInQuotes = true,
					TrimWhiteSpace = true
				};

				var hdrFields = p.ReadFields();
				if (hdrFields == null) {
					throw new CSVReadException("CSV Read Error: File was empty.");
				}

				var colsWithoutComment = hdrFields
					.Select(l => l.Contains(Comment) ? l.Substring(0, l.IndexOf(Comment, StringComparison.Ordinal)) : l)
					.ToArray();

				var columns = colsWithoutComment
					.Select(l =>  HeaderFilter.Replace(l, ""))
					.Select(l => l.Trim())
					.Where(col => !double.TryParse(col, NumberStyles.Any, CultureInfo.InvariantCulture, out _))
					.Distinct()
					.ToList();

				var firstLineIsData = columns.Count == 0;

				if (firstLineIsData) {
					LogManager.GetLogger(typeof(VectoCSVFile).FullName)
						.Warn("No valid Data Header found. Interpreting the first line as data line.");
					// set the validColumns to: {"0", "1", "2", "3", ...} for all columns in first line.
					columns = colsWithoutComment.Select((_, i) => i.ToString()).ToList();
				}

				var mrfCols = new List<object>();
				var colNames = EnumHelper.GetValues<ModalResultField>().Select(x => Tuple.Create(x, HeaderFilter.Replace(x.GetCaption(), "").Trim())).ToArray();
				//var missing = new List<string>();
                foreach (var column in columns) {
					var mrf = colNames.FirstOrDefault(x => x.Item2.Equals(column, StringComparison.CurrentCultureIgnoreCase));
					if (mrf != null) {
						mrfCols.Add(mrf.Item1);
					} else {
						mrfCols.Add(column);
						//	missing.Add(column);
					}
				}

				foreach (object col in mrfCols) {
					if (col is ModalResultField field) {
						CreateColumns(new [] {field});
					} else {
						Columns.Add(col.ToString());
					}
				}
				//missing.ForEach(col => Columns.Add(col));

				var lineNumber = 1;
				while (!p.EndOfData) {
					object[] cells = { };
					if (firstLineIsData) {
						cells = colsWithoutComment;
					} else {
						var fields = p.ReadFields();
						if (fields != null) {
							cells = fields.Select(l =>
									l.Contains(Comment)
										? l.Substring(0, l.IndexOf(Comment, StringComparison.Ordinal))
										: l)
								.Select(s => s.Trim())
								.ToArray();

							cells = cells.Select((t, i) => ConvertToValue(t, mrfCols[i])).ToArray();
						}
					}

					firstLineIsData = false;
					if (false && Columns.Count != cells.Length) {
						throw new CSVReadException(
							$"Line {lineNumber}: The number of values is not correct. " +
							$"Expected {Columns.Count} Columns, Got {cells.Length} Columns");
					}

					try {
						// ReSharper disable once CoVariantArrayConversion
						Rows.Add(cells);
					} catch (InvalidCastException e) {
						throw new CSVReadException(
							$"Line {lineNumber}: The data format of a value is not correct. {e.Message}", e);
					}

					lineNumber++;
				}
			}
		}

		private object ConvertToValue(object cell, object mrfCol)
		{
			var unitRegex = new Regex(@"[^\]]*\[([^]]+)\]", RegexOptions.Compiled);
			if (mrfCol is ModalResultField mrf) {
				var unit = unitRegex.Replace(mrf.GetCaption(), "$1");
				switch (unit) {
					case "s": return cell.ToString().ToDouble().SI<Second>();
					case "km/h": return cell.ToString().ToDouble().KMPHtoMeterPerSecond();
					case "m": return cell.ToString().ToDouble().SI<Meter>();
					case "m/s^2": return cell.ToString().ToDouble().SI<MeterPerSquareSecond>();
					case "%": return (cell.ToString().ToDouble() / 100).SI<Scalar>();
					case "1/min": return cell.ToString().ToDouble().RPMtoRad();
					case "Nm": return cell.ToString().ToDouble().SI<NewtonMeter>();
					case "kW": return (cell.ToString().ToDouble() * 1000).SI<Watt>();
					case "g/h": return cell.ToString().ToDouble().SI(Unit.SI.Gramm.Per.Hour).Cast<KilogramPerSecond>();
					case "V": return cell.ToString().ToDouble().SI<Volt>();
					case "A": return cell.ToString().ToDouble().SI<Ampere>();
					default:
						return cell;
						//return cell.ToString().ToDouble().SI<Scalar>();
				}
			}

			//if (mrfCol is string colName) {
				return cell;
			//}
		}
	}
}