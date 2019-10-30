using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces;
using TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules.Electrics;
using TUGraz.VectoCore.Models.BusAuxiliaries.Util;

namespace TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.Electrics
{
	public class CombinedAlternator : IAlternatorMap, ICombinedAlternator
	{
		public List<IAlternator> Alternators { get; set; } = new List<IAlternator>();
		
		private ICombinedAlternatorSignals altSignals;
		private ISignals Signals;
		private double AverageAlternatorsEfficiency;

		// Constructors
		public CombinedAlternator(IList<ICombinedAlternatorMapRow> alternatorData, string source, ISignals signals = null)
		{
			Source = source;
			Signals = signals;

			altSignals = new CombinedAlternatorSignals();

			Initialise(alternatorData);

			// Calculate alternators average which is used only in the pre-run
			var efficiencySum = 0.0;

			foreach (var alt in Alternators) {
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
			AverageAlternatorsEfficiency = efficiencyAverage / 100.0;
		}

		public string Source { get; }

		// Interface Implementation
		public double GetEfficiency(PerSecond CrankRPM, Ampere Amps)
		{
			altSignals.CrankRPM = CrankRPM;
			altSignals.CurrentDemandAmps = (Amps.Value() / (double)Alternators.Count).SI<Ampere>();

			;

			//if (Signals == null/* || Signals.RunningCalc*/) { 
				// If running calc cycle get efficiency from interpolation function
				var alternatorMapValues = Alternators.Average(a => a.Efficiency) / 100.0;
			//} else {
			//	// If running Pre calc cycle get an average of inputs
			
			//	alternatorMapValues = AverageAlternatorsEfficiency;
			//}

			if (alternatorMapValues <= 0) {
				alternatorMapValues = 0.01;
			}

			return alternatorMapValues;
		}

		protected void Initialise(IList<ICombinedAlternatorMapRow> map)
		{

			// From the map we construct this CombinedAlternator object and original CombinedAlternator Object

			Alternators.Clear();
			
			foreach (var alt in map.GroupBy(g => g.AlternatorName)) {
				Alternators.Add(new Alternator(altSignals, alt.ToList()));
			}
		}

		

		//event Interfaces.AuxiliaryEventEventHandler IAuxiliaryEvent.AuxiliaryEvent
		//{
		//	add {
		//		//throw new NotImplementedException();
		//	}

		//	remove {
		//		//throw new NotImplementedException();
		//	}
		//}

		
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

			var altToRemove = Alternators.First(w => w.AlternatorName == alternatorName);
			var numAlternators = Alternators.Count;

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
			foreach (var alt in Alternators.OrderBy(o => o.AlternatorName)) {
				// 2000 - IE Alt1,2000,10,50,3
				for (var row = 1; row <= 3; row++) {
					var amps = alt.InputTable2000[row].Amps;
					var eff = alt.InputTable2000[row].Eff;
					sb.Append(alt.AlternatorName + ",2000," + amps.Value().ToString("0.000") + "," + eff.ToString("0.000") + "," + alt.PulleyRatio.ToString("0.000"));
					sb.AppendLine("");
				}

				// 4000 - IE Alt1,2000,10,50,3
				for (var row = 1; row <= 3; row++) {
					var amps = alt.InputTable4000[row].Amps;
					var eff = alt.InputTable4000[row].Eff;
					sb.Append(alt.AlternatorName + ",4000," + amps.Value().ToString("0.000") + "," + eff.ToString("0.000") + "," + alt.PulleyRatio.ToString("0.000"));
					sb.AppendLine("");
				}

				// 6000 - IE Alt1,2000,10,50,3
				for (var row = 1; row <= 3; row++) {
					var amps = alt.InputTable6000[row].Amps;
					var eff = alt.InputTable6000[row].Eff;
					sb.Append(alt.AlternatorName + ",6000," + amps.Value().ToString("0.000") + "," + eff.ToString("0.000") + "," + alt.PulleyRatio.ToString("0.000"));
					sb.AppendLine("");
				}
			}

			// Add Model Source
			sb.AppendLine("[MODELSOURCE]");
			sb.Append(ToString());

			// Write the stream cotnents to a new file named "AllTxtFiles.txt" 
			using (var outfile = new StreamWriter(aaltPath)) {
				outfile.Write(sb.ToString());
			}

			return true;
		}

		


		// Can be used to send messages to Vecto.
		//public event AuxiliaryEventEventHandler AuxiliaryEvent;

		public delegate void AuxiliaryEventEventHandler(ref object sender, string message, AdvancedAuxiliaryMessageType messageType);

		// This is used to generate a diagnostics output which enables the user to 
		// Determine if they beleive the resulting map is what is expected
		// Basically it is a check against the model/Spreadsheet
		public override string ToString()
		{
			var sb = new StringBuilder();
			string a1, a2, a3, e1, e2, e3;

			const string vbTab = "\t";
			foreach (Alternator alt in Alternators.OrderBy(o => o.AlternatorName)) {
				sb.AppendLine("");
				sb.AppendFormat("** {0} ** , PulleyRatio {1}", alt.AlternatorName, alt.PulleyRatio);
				sb.AppendLine("");
				sb.AppendLine("******************************************************************");
				sb.AppendLine("");

				var i = 1;
				sb.AppendLine("Table 1 (2000)" + vbTab + "Table 2 (4000)" + vbTab + "Table 3 (6000)");
				sb.AppendLine("Amps" + vbTab + "Eff" + vbTab + "Amps" + vbTab + "Eff" + vbTab + "Amps" + vbTab + "Eff" + vbTab);
				sb.AppendLine("");
				for (i = 1; i <= 3; i++) {
					a1 = alt.InputTable2000[i].Amps.Value().ToString("0");
					e1 = alt.InputTable2000[i].Eff.ToString("0.000");
					a2 = alt.InputTable4000[i].Amps.Value().ToString("0");
					e2 = alt.InputTable4000[i].Eff.ToString("0.000");
					a3 = alt.InputTable6000[i].Amps.Value().ToString("0");
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

			foreach (var alt in this.Alternators) {

				// Can we find the same alternatorName in other
				if (other.Alternators.Count(f => f.AlternatorName == alt.AlternatorName) != 1)
					return false;

				// get the alternator to compare and compare it.
				if (!alt.IsEqualTo(other.Alternators.First(f => f.AlternatorName == alt.AlternatorName)))
					return false;
			}

			return true;
		}

		public override bool Equals(object other)
		{
			var myOther = other as CombinedAlternator;
			return myOther != null && IsEqualTo(myOther);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
