using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using DownstreamModules.Electrics;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.BusAuxiliaries.Interfaces;
using TUGraz.VectoCore.BusAuxiliaries.Interfaces.DownstreamModules.Electrics;


namespace Electrics
{
	public class CombinedAlternator : IAlternatorMap, ICombinedAlternator
	{
		private List<ICombinedAlternatorMapRow> map = new List<ICombinedAlternatorMapRow>();
		public List<IAlternator> Alternators { get; set; } = new List<IAlternator>();
		private List<IAlternator> OriginalAlternators = new List<IAlternator>();
		private string FilePath;
		private ICombinedAlternatorSignals altSignals;
		private ISignals Signals;
		private AlternatorMapValues AverageAlternatorsEfficiency;

		// Interface Implementation
		public AlternatorMapValues GetEfficiency(double CrankRPM, Ampere Amps)
		{
			altSignals.CrankRPM = CrankRPM;
			altSignals.CurrentDemandAmps = (Amps.Value() / (double)Alternators.Count).SI<Ampere>();

			AlternatorMapValues alternatorMapValues; /* TODO Change to default(_) if this is not a reference type */;

			if (Signals == null || Signals.RunningCalc)
				// If running calc cycle get efficiency from interpolation function
				alternatorMapValues = new AlternatorMapValues(Convert.ToSingle(Alternators.Average(a => a.Efficiency) / (double)100));
			else
				// If running Pre calc cycle get an average of inputs
				alternatorMapValues = AverageAlternatorsEfficiency;

			if (alternatorMapValues.Efficiency <= 0)
				alternatorMapValues = new AlternatorMapValues(0.01);

			return alternatorMapValues;
		}

		public bool Initialise()
		{

			// From the map we construct this CombinedAlternator object and original CombinedAlternator Object

			Alternators.Clear();
			OriginalAlternators.Clear();


			foreach (var alt in map.GroupBy(g => g.AlternatorName)) {
				var altName = alt.First().AlternatorName;
				var pulleyRatio = alt.First().PulleyRatio;


				IAlternator alternator = new Alternator(altSignals, alt.ToList());

				Alternators.Add(alternator);
			}

			return true;
		}

		// Constructors
		public CombinedAlternator(string filePath, ISignals signals = null/* TODO Change to default(_) if this is not a reference type */)
		{
			string feedback = string.Empty;
			this.Signals = signals;

			if (!FilePathUtils.ValidateFilePath(filePath, ".aalt", ref feedback))
				throw new ArgumentException(string.Format("Combined Alternator requires a valid .AALT filename. : {0}", feedback));
			else
				this.FilePath = filePath;


			this.altSignals = new CombinedAlternatorSignals();


			// IF file exists then read it otherwise create a default.

			if (File.Exists(filePath) && InitialiseMap(filePath))
				Initialise();
			else {
				// Create Default Map
				CreateDefaultMap();
				Initialise();
			}

			// Calculate alternators average which is used only in the pre-run
			var efficiencySum = 0.0;
			
			foreach (IAlternator alt in Alternators) {
				efficiencySum += alt.InputTable2000.ElementAt(1).Eff;
				efficiencySum += alt.InputTable2000.ElementAt(2).Eff;
				efficiencySum += alt.InputTable2000.ElementAt(3).Eff;

				efficiencySum += alt.InputTable4000.ElementAt(1).Eff;
				efficiencySum += alt.InputTable4000.ElementAt(2).Eff;
				efficiencySum += alt.InputTable4000.ElementAt(3).Eff;

				efficiencySum += alt.InputTable6000.ElementAt(1).Eff;
				efficiencySum += alt.InputTable6000.ElementAt(2).Eff;
				efficiencySum += alt.InputTable6000.ElementAt(3).Eff;
			}

			var efficiencyAverage = efficiencySum / (Alternators.Count * 9);
			AverageAlternatorsEfficiency = new AlternatorMapValues(efficiencyAverage / 100);
		}

		event TUGraz.VectoCore.BusAuxiliaries.Interfaces.AuxiliaryEventEventHandler IAuxiliaryEvent.AuxiliaryEvent
		{
			add {
				throw new NotImplementedException();
			}

			remove {
				throw new NotImplementedException();
			}
		}

		// Helpers
		private void CreateDefaultMap()
		{
			map.Clear();

			map.Add(new CombinedAlternatorMapRow("Alt1", 2000, 10, 62, 3.6));
			map.Add(new CombinedAlternatorMapRow("Alt1", 2000, 27, 70, 3.6));
			map.Add(new CombinedAlternatorMapRow("Alt1", 2000, 53, 30, 3.6));

			map.Add(new CombinedAlternatorMapRow("Alt1", 4000, 10, 64, 3.6));
			map.Add(new CombinedAlternatorMapRow("Alt1", 4000, 63, 74, 3.6));
			map.Add(new CombinedAlternatorMapRow("Alt1", 4000, 125, 68, 3.6));

			map.Add(new CombinedAlternatorMapRow("Alt1", 6000, 10, 53, 3.6));
			map.Add(new CombinedAlternatorMapRow("Alt1", 6000, 68, 70, 3.6));
			map.Add(new CombinedAlternatorMapRow("Alt1", 6000, 136, 62, 3.6));

			map.Add(new CombinedAlternatorMapRow("Alt2", 2000, 10, 62, 3));
			map.Add(new CombinedAlternatorMapRow("Alt2", 2000, 27, 70, 3));
			map.Add(new CombinedAlternatorMapRow("Alt2", 2000, 53, 30, 3));

			map.Add(new CombinedAlternatorMapRow("Alt2", 4000, 10, 64, 3));
			map.Add(new CombinedAlternatorMapRow("Alt2", 4000, 63, 74, 3));
			map.Add(new CombinedAlternatorMapRow("Alt2", 4000, 125, 68, 3));

			map.Add(new CombinedAlternatorMapRow("Alt2", 6000, 10, 53, 3));
			map.Add(new CombinedAlternatorMapRow("Alt2", 6000, 68, 70, 3));
			map.Add(new CombinedAlternatorMapRow("Alt2", 6000, 136, 62, 3));
		}

		// Grid Management
		private bool AddNewAlternator(List<ICombinedAlternatorMapRow> list, ref string feeback)
		{
			var altName = list.First().AlternatorName;
			var pulleyRatio = list.First().PulleyRatio;

			// Check alt does not already exist in list
			if (Alternators.Any(w => w.AlternatorName == altName)) {
				feeback = "This alternator already exists in in the list, operation not completed.";
				return false;
			}

			IAlternator alternator = new Alternator(altSignals, list.ToList());

			Alternators.Add(alternator);


			return true;
		}

		public bool AddAlternator(List<ICombinedAlternatorMapRow> rows, ref string feedback)
		{
			if (!AddNewAlternator(rows, ref feedback)) {
				feedback = string.Format("Unable to add new alternator : {0}", feedback);
				return false;
			}

			return true;
		}

		public bool DeleteAlternator(string alternatorName, ref string feedback, bool CountValidation)
		{

			// Is this the last alternator, if so deny the user the right to remove it.
			if (CountValidation && Alternators.Count < 2) {
				feedback = "There must be at least one alternator remaining, operation aborted.";
				return false;
			}

			if (Alternators.All(w => w.AlternatorName != alternatorName)) {
				feedback = "This alternator does not exist";
				return false;
			}

			IAlternator altToRemove = Alternators.First(w => w.AlternatorName == alternatorName);
			int numAlternators = Alternators.Count;

			Alternators.Remove(altToRemove);

			if (Alternators.Count == numAlternators - 1) {
				return true;
			}

			feedback = string.Format("The alternator {0} could not be removed : {1}", alternatorName, feedback);
			return false;
		}
		

		public bool Save(string aaltPath)
		{
			var sb = new StringBuilder();
			
			// write headers  
			sb.AppendLine("[AlternatorName],[RPM],[Amps],[Efficiency],[PulleyRatio]");

			// write details
			foreach (IAlternator alt in Alternators.OrderBy(o => o.AlternatorName)) {
				// 2000 - IE Alt1,2000,10,50,3
				for (var row = 1; row <= 3; row++) {
					var amps = alt.InputTable2000[row].Amps;
					var eff = alt.InputTable2000[row].Eff;
					sb.Append(alt.AlternatorName + ",2000," + amps.ToString("0.000") + "," + eff.ToString("0.000") + "," + alt.PulleyRatio.ToString("0.000"));
					sb.AppendLine("");
				}

				// 4000 - IE Alt1,2000,10,50,3
				for (var row = 1; row <= 3; row++) {
					var amps = alt.InputTable4000[row].Amps;
					var eff = alt.InputTable4000[row].Eff;
					sb.Append(alt.AlternatorName + ",4000," + amps.ToString("0.000") + "," + eff.ToString("0.000") + "," + alt.PulleyRatio.ToString("0.000"));
					sb.AppendLine("");
				}

				// 6000 - IE Alt1,2000,10,50,3
				for (var row = 1; row <= 3; row++) {
					var amps = alt.InputTable6000[row].Amps;
					var eff = alt.InputTable6000[row].Eff;
					sb.Append(alt.AlternatorName + ",6000," + amps.ToString("0.000") + "," + eff.ToString("0.000") + "," + alt.PulleyRatio.ToString("0.000"));
					sb.AppendLine("");
				}
			}

			// Add Model Source
			sb.AppendLine("[MODELSOURCE]");
			sb.Append(ToString());

			// Write the stream cotnents to a new file named "AllTxtFiles.txt" 
			using (StreamWriter outfile = new StreamWriter(aaltPath)) {
				outfile.Write(sb.ToString());
			}

			return true;
		}

		private bool Load()
		{
			if (!InitialiseMap(FilePath))
				return false;


			return true;
		}

		// Initialises the map, only valid when loadingUI for first time in edit mode or always in operational mode.
		private bool InitialiseMap(string filePath)
		{
			bool returnValue = false;
			string[] elements;

			if (File.Exists(filePath)) {
				using (StreamReader sr = new StreamReader(filePath)) {
					// get array og lines fron csv
					string[] lines = sr.ReadToEnd().Split(new[] { Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries);

					// Must have at least 2 entries in map to make it usable [dont forget the header row]
					if ((lines.Count() < 10))
						throw new ArgumentException("Insufficient rows in csv to build a usable map");

					map = new List<ICombinedAlternatorMapRow>();

					bool firstline = true;

					foreach (string line in lines) {
						if (!firstline) {

							// Advanced Alternator Source Check.
							if (line.Contains("[MODELSOURCE"))
								break;

							// split the line
							elements = line.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
							// 3 entries per line required
							if ((elements.Length != 5))
								throw new ArgumentException("Incorrect number of values in csv file");
							// add values to map

							map.Add(new CombinedAlternatorMapRow(elements[0], float.Parse(elements[1], CultureInfo.InvariantCulture), float.Parse(elements[2], CultureInfo.InvariantCulture), float.Parse(elements[3], CultureInfo.InvariantCulture), float.Parse(elements[4], CultureInfo.InvariantCulture)));
						} else
							firstline = false;
					}
				}
				return true;
			} else
				throw new ArgumentException("Supplied input file does not exist");

			return returnValue;
		}

		// Can be used to send messages to Vecto.
		public event AuxiliaryEventEventHandler AuxiliaryEvent;

		public delegate void AuxiliaryEventEventHandler(ref object sender, string message, AdvancedAuxiliaryMessageType messageType);

		// This is used to generate a diagnostics output which enables the user to 
		// Determine if they beleive the resulting map is what is expected
		// Basically it is a check against the model/Spreadsheet
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			string a1, a2, a3, e1, e2, e3;

			const string vbTab = "\t";
			foreach (Alternator alt in Alternators.OrderBy(o => o.AlternatorName)) {
				sb.AppendLine("");
				sb.AppendFormat("** {0} ** , PulleyRatio {1}", alt.AlternatorName, alt.PulleyRatio);
				sb.AppendLine("");
				sb.AppendLine("******************************************************************");
				sb.AppendLine("");

				int i = 1;
				sb.AppendLine("Table 1 (2000)" + vbTab + "Table 2 (4000)" + vbTab + "Table 3 (6000)");
				sb.AppendLine("Amps" + vbTab + "Eff" + vbTab + "Amps" + vbTab + "Eff" + vbTab + "Amps" + vbTab + "Eff" + vbTab);
				sb.AppendLine("");
				for (i = 1; i <= 3; i++) {
					a1 = alt.InputTable2000[i].Amps.ToString("0");
					e1 = alt.InputTable2000[i].Eff.ToString("0.000");
					a2 = alt.InputTable4000[i].Amps.ToString("0");
					e2 = alt.InputTable4000[i].Eff.ToString("0.000");
					a3 = alt.InputTable6000[i].Amps.ToString("0");
					e3 = alt.InputTable6000[i].Eff.ToString("0.000");
					sb.AppendLine(a1 + vbTab + e1 + vbTab + a2 + vbTab + e2 + vbTab + a3 + vbTab + e3 + vbTab);
				}
			}

			// sb.AppendLine("")
			// sb.AppendLine("********* COMBINED EFFICIENCY VALUES **************")
			// sb.AppendLine("")
			// sb.AppendLine(vbTab + "RPM VALUES")
			// sb.AppendLine("AMPS" + vbTab + "500" + vbTab + "1500" + vbTab + "2500" + vbTab + "3500" + vbTab + "4500" + vbTab + "5500" + vbTab + "6500" + vbTab + "7500")
			// For a As Single = 1 To Alternators.Count * 50

			// sb.Append(a.ToString("0") + vbTab)
			// For Each r As Single In {500, 1500, 2500, 3500, 4500, 5500, 6500, 7500}

			// Dim eff As Single = GetEfficiency(r, a).Efficiency

			// sb.Append(eff.ToString("0.000") + vbTab)

			// Next
			// sb.AppendLine("")

			// Next


			return sb.ToString();
		}


		// Equality
		public bool IsEqualTo(ICombinedAlternator other)
		{

			// Count Check.
			if (this.Alternators.Count != other.Alternators.Count)
				return false;

			foreach (IAlternator alt in this.Alternators) {

				// Can we find the same alternatorName in other
				if (other.Alternators.Where(f => f.AlternatorName == alt.AlternatorName).Count() != 1)
					return false;

				// get the alternator to compare and compare it.
				if (!alt.IsEqualTo(other.Alternators.First(f => f.AlternatorName == alt.AlternatorName)))
					return false;
			}

			return true;
		}
	}
}
