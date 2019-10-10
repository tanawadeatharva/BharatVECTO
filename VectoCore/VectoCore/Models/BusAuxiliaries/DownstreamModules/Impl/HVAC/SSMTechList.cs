using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules.HVAC;

namespace TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.HVAC
{
	// Used By SSMTOOL Class.
	public class SSMTechList : ISSMTechList
	{

		// Private Fields
		private string filePath;
		private ISSMGenInputs _ssmInputs;
		//private bool _dirty;

		public List<ITechListBenefitLine> TechLines { get; set; }

		// Constructors
		public SSMTechList(string filepath, ISSMGenInputs genInputs, bool initialiseDefaults = false)
		{
			TechLines = new List<ITechListBenefitLine>();

			filePath = filepath;

			_ssmInputs = genInputs;

			if (initialiseDefaults)
				SetDefaults();
		}


		public void SetSSMGeneralInputs(ISSMGenInputs genInputs)
		{
			_ssmInputs = genInputs;
		}

		// Initialisation Methods
		public bool Initialise(string filepath)
		{
			filePath = filepath;

			return Initialise();
		}

		public bool Initialise()
		{
			var returnStatus = true;

			if (File.Exists(filePath)) {
				using (var sr = new StreamReader(filePath)) {
					// get array og lines fron csv
					var lines = sr.ReadToEnd().Split(new [] { Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries);

					// Must have at least 2 entries in map to make it usable [dont forget the header row]
					if (lines.Length < 1) {
						return false;
					}

					var firstline = true;

					foreach (var line in lines) {
						if (!firstline) {

							// split the line
							var elements = line.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
							// 3 entries per line required
							if (elements.Length != 17)
								throw new ArgumentException("Incorrect number of values in csv file");
							// add values to map

							// 00. Category,
							// 01. BenefitName,
							// 02. Units,
							// 03. LowH,
							// 04. LowV,
							// 05. LowC,
							// 06. SemiLowH,
							// 07. SemiLowV,
							// 08. SemiLowC,
							// 09. RaisedH,
							// 10. RaisedV,
							// 11. RaisedC,
							// 12. OnVehicle,
							// 13. LineType,
							// 14. AvtiveVH,
							// 15. ActiveVV,
							// 16. ActiveVC


							// Bus
							try {
								var tbline = new TechListBenefitLine(_ssmInputs, elements[2], elements[0], elements[1], double.Parse(elements[3], CultureInfo.InvariantCulture), double.Parse(elements[4], CultureInfo.InvariantCulture), double.Parse(elements[5], CultureInfo.InvariantCulture), double.Parse(elements[6], CultureInfo.InvariantCulture), double.Parse(elements[7], CultureInfo.InvariantCulture), double.Parse(elements[8], CultureInfo.InvariantCulture), double.Parse(elements[9], CultureInfo.InvariantCulture), double.Parse(elements[10], CultureInfo.InvariantCulture), double.Parse(elements[11], CultureInfo.InvariantCulture), bool.Parse(elements[12]), elements[13].ParseEnum<TechLineType>(), bool.Parse(elements[14]), bool.Parse(elements[15]), bool.Parse(elements[16]));

								TechLines.Add(tbline);
							} catch (Exception ) {

								// Indicate problems
								returnStatus = false;
							}
						} else {
							firstline = false;
						}
					}
				}
			} else {
				returnStatus = false;
			}

			return returnStatus;
		}

		// Public Properties - Outputs Of Class
		public double CValueVariation
		{
			get {
				double a;

				a = TechLines.Where(x => x.Units.ToLower() == "fraction").Sum(s => s.C);

				return a;
			}
		}

		public double CValueVariationKW
		{
			get {
				double a;

				a = TechLines.Where(x => x.Units.ToLower() == "kw").Sum(s => s.C);

				return a;
			}
		}

		public double HValueVariation
		{
			get {

				// Dim a,b As double
				return TechLines.Where(x => x.Units == "fraction").Sum(s => s.H);
			}
		}

		public double HValueVariationKW
		{
			get {
				return TechLines.Where(x => x.Units.ToLower() == "kw").Sum(s => s.H);
			}
		}

		public double VCValueVariation
		{
			get {
				return TechLines.Where(x => x.Units.ToLower() == "fraction").Sum(s => s.VC); // -  VCValueVariationKW
			}
		}

		public double VCValueVariationKW
		{
			get {
				return TechLines.Where(x => x.Units.ToLower() == "kw").Sum(s => s.VC);
			}
		}

		public double VHValueVariation
		{
			get {
				// Dim a,b As double

				return TechLines.Where(x => x.Units.ToLower() == "fraction").Sum(s => s.VH);
			}
		}

		public double VHValueVariationKW
		{
			get {
				return TechLines.Where(x => x.Units.ToLower() == "kw").Sum(s => s.VH);
			}
		}

		public double VVValueVariation
		{
			get {
				return TechLines.Where(x => x.Units.ToLower() == "fraction").Sum(s => s.VV);
			}
		}

		public double VVValueVariationKW
		{
			get {
				return TechLines.Where(x => x.Units.ToLower() == "kw").Sum(s => s.VV); // - VVValueVariationKW
			}
		}

		// Member Management
		public bool Add(ITechListBenefitLine item, ref string feedback)
		{
			var initialCount = TechLines.Count;

			if (TechLines.Any(w => w.Category == item.Category && w.BenefitName == item.BenefitName)) {
				// Failure
				feedback = "Item already exists.";
				return false;
			}


			try {
				TechLines.Add(item);

				if (TechLines.Count == initialCount + 1) {

					// Success
					feedback = "OK";
					//_dirty = true;
					return true;
				} else {

					// Failure
					feedback = "The system was unable to add the new tech benefit list item.";
					return false;
				}
			} catch (Exception ) {
				feedback = "The system threw an exception and was unable to add the new tech benefit list item.";
				return false;
			}
		}

		public void Clear()
		{
			if (TechLines.Count > 0)
				//_dirty = true;

			TechLines.Clear();
		}

		public bool Delete(ITechListBenefitLine item, ref string feedback)
		{
			var currentCount = TechLines.Count;

			if ((TechLines.Count(c => c.Category == item.Category && c.BenefitName == item.BenefitName) == 1)) {
				try {
					TechLines.RemoveAt(TechLines.FindIndex(c => c.Category == item.Category && c.BenefitName == item.BenefitName));

					if (TechLines.Count == currentCount - 1) {
						// This succeeded
						//_dirty = true;
						return true;
					} else {
						// No Exception, but this failed for some reason.
						feedback = "The system was unable to remove the item from the list.";
						return false;
					}
				} catch (Exception) {
					feedback = "An exception occured, the removal failed.";
					return false;
				}
			} else {
				feedback = "the item was not found in the list.";
				return false;
			}
		}

		public bool Modify(ITechListBenefitLine originalItem, ITechListBenefitLine newItem, ref string feedback)
		{
			var fi = TechLines.Find(f => f.Category == originalItem.Category && f.BenefitName == originalItem.BenefitName);

			if (fi != null) {
				try {
					var originalUnits = fi.Units;
					fi.CloneFrom(newItem);

					// The lines below are to assist in testing. The KW units are being excluded, but for benchmarking against the spreadsheet model
					// Two KW entries are left in. There is no provision for adding KW units in so we check if the original entry was KW and 
					// force it back to KW if it was already so. There shoud be no need to remove this as newly created lists will not match this
					// Phenomenon.
					if ((originalUnits.ToLower() == "kw")) {
						fi.Units = originalUnits;
						newItem.Units = originalUnits;
					}

					if (newItem == fi) {
						// This succeeded
						//_dirty = true;
						return true;
					} else {
						// No Exception, but this failed for some reason.
						feedback = "The system was unable to remove the item from the list.";
						return false;
					}
				} catch (Exception ) {
					feedback = "An exception occured, the update failed.";
					return false;
				}
			} else {
				feedback = "the item was not found so cannot be modified.";
				return false;
			}
		}


		private void SetDefaults()
		{
			ITechListBenefitLine techLine1 = new TechListBenefitLine(_ssmInputs);
			{
				var withBlock = techLine1;
				withBlock.Category = "Cooling";
				withBlock.BenefitName = "Separate air distribution ducts";
				withBlock.LowFloorH = 0;
				withBlock.LowFloorC = 0.04;
				withBlock.LowFloorV = 0.04;
				withBlock.SemiLowFloorH = 0;
				withBlock.SemiLowFloorC = 0.04;
				withBlock.SemiLowFloorV = 0.04;
				withBlock.RaisedFloorH = 0;
				withBlock.RaisedFloorC = 0.04;
				withBlock.RaisedFloorV = 0.04;
				withBlock.ActiveVH = false;
				withBlock.ActiveVV = false;
				withBlock.ActiveVC = true;
				withBlock.OnVehicle = false;
				withBlock.Units = "fraction";
			}

			ITechListBenefitLine techLine2 = new TechListBenefitLine(_ssmInputs);
			{
				var withBlock = techLine2;
				withBlock.Category = "Heating";
				withBlock.BenefitName = "Adjustable auxiliary heater";
				withBlock.LowFloorH = 0.02;
				withBlock.LowFloorC = 0;
				withBlock.LowFloorV = 0.02;
				withBlock.SemiLowFloorH = 0.02;
				withBlock.SemiLowFloorC = 0;
				withBlock.SemiLowFloorV = 0.02;
				withBlock.RaisedFloorH = 0.02;
				withBlock.RaisedFloorC = 0;
				withBlock.RaisedFloorV = 0.02;
				withBlock.ActiveVH = true;
				withBlock.ActiveVV = false;
				withBlock.ActiveVC = false;
				withBlock.OnVehicle = false;
				withBlock.Units = "fraction";
			}

			ITechListBenefitLine techLine3 = new TechListBenefitLine(_ssmInputs);
			{
				var withBlock = techLine3;
				withBlock.Category = "Heating";
				withBlock.BenefitName = "Adjustable coolant thermostat";
				withBlock.LowFloorH = 0.02;
				withBlock.LowFloorC = 0;
				withBlock.LowFloorV = 0.02;
				withBlock.SemiLowFloorH = 0.02;
				withBlock.SemiLowFloorC = 0;
				withBlock.SemiLowFloorV = 0.02;
				withBlock.RaisedFloorH = 0.02;
				withBlock.RaisedFloorC = 0;
				withBlock.RaisedFloorV = 0.02;
				withBlock.ActiveVH = true;
				withBlock.ActiveVV = false;
				withBlock.ActiveVC = false;
				withBlock.OnVehicle = false;
				withBlock.Units = "fraction";
			}

			ITechListBenefitLine techLine4 = new TechListBenefitLine(_ssmInputs);
			{
				var withBlock = techLine4;
				withBlock.Category = "Heating";
				withBlock.BenefitName = "Engine waste gas heat exchanger";
				withBlock.LowFloorH = 0.04;
				withBlock.LowFloorC = 0;
				withBlock.LowFloorV = 0.04;
				withBlock.SemiLowFloorH = 0;
				withBlock.SemiLowFloorC = 0;
				withBlock.SemiLowFloorV = 0;
				withBlock.RaisedFloorH = 0;
				withBlock.RaisedFloorC = 0;
				withBlock.RaisedFloorV = 0;
				withBlock.ActiveVH = true;
				withBlock.ActiveVV = false;
				withBlock.ActiveVC = false;
				withBlock.OnVehicle = false;
				withBlock.Units = "fraction";
			}

			ITechListBenefitLine techLine5 = new TechListBenefitLine(_ssmInputs);
			{
				var withBlock = techLine5;
				withBlock.Category = "Heating";
				withBlock.BenefitName = "Heat pump systems";
				withBlock.LowFloorH = 0.06;
				withBlock.LowFloorC = 0;
				withBlock.LowFloorV = 0.06;
				withBlock.SemiLowFloorH = 0.04;
				withBlock.SemiLowFloorC = 0;
				withBlock.SemiLowFloorV = 0.04;
				withBlock.RaisedFloorH = 0.04;
				withBlock.RaisedFloorC = 0;
				withBlock.RaisedFloorV = 0.04;
				withBlock.ActiveVH = true;
				withBlock.ActiveVV = false;
				withBlock.ActiveVC = false;
				withBlock.OnVehicle = false;
				withBlock.Units = "fraction";
			}

			ITechListBenefitLine techLine6 = new TechListBenefitLine(_ssmInputs);
			{
				var withBlock = techLine6;
				withBlock.Category = "Insulation";
				withBlock.BenefitName = "Double-glazing";
				withBlock.LowFloorH = 0.04;
				withBlock.LowFloorC = 0.04;
				withBlock.LowFloorV = 0.04;
				withBlock.SemiLowFloorH = 0.04;
				withBlock.SemiLowFloorC = 0.04;
				withBlock.SemiLowFloorV = 0.04;
				withBlock.RaisedFloorH = 0.04;
				withBlock.RaisedFloorC = 0.04;
				withBlock.RaisedFloorV = 0.04;
				withBlock.ActiveVH = true;
				withBlock.ActiveVV = true;
				withBlock.ActiveVC = true;
				withBlock.OnVehicle = false;
				withBlock.Units = "fraction";
			}

			ITechListBenefitLine techLine7 = new TechListBenefitLine(_ssmInputs);
			{
				var withBlock = techLine7;
				withBlock.Category = "Insulation";
				withBlock.BenefitName = "Tinted windows";
				withBlock.LowFloorH = 0;
				withBlock.LowFloorC = 0;
				withBlock.LowFloorV = 0;
				withBlock.SemiLowFloorH = 0;
				withBlock.SemiLowFloorC = 0;
				withBlock.SemiLowFloorV = 0;
				withBlock.RaisedFloorH = 0;
				withBlock.RaisedFloorC = 0;
				withBlock.RaisedFloorV = 0;
				withBlock.ActiveVH = false;
				withBlock.ActiveVV = false;
				withBlock.ActiveVC = false;
				withBlock.OnVehicle = false;
				withBlock.Units = "fraction";
			}

			ITechListBenefitLine techLine8 = new TechListBenefitLine(_ssmInputs);
			{
				var withBlock = techLine8;
				withBlock.Category = "Ventilation";
				withBlock.BenefitName = "Fan control strategy (serial/parallel)";
				withBlock.LowFloorH = 0.02;
				withBlock.LowFloorC = 0.02;
				withBlock.LowFloorV = 0.02;
				withBlock.SemiLowFloorH = 0.02;
				withBlock.SemiLowFloorC = 0.02;
				withBlock.SemiLowFloorV = 0.02;
				withBlock.RaisedFloorH = 0.02;
				withBlock.RaisedFloorC = 0.02;
				withBlock.RaisedFloorV = 0.02;
				withBlock.ActiveVH = true;
				withBlock.ActiveVV = true;
				withBlock.ActiveVC = true;
				withBlock.OnVehicle = false;
				withBlock.Units = "fraction";
				withBlock.LineType = TechLineType.HVCActiveSelection;
			}

			var feedback = string.Empty;
			Add(techLine1, ref feedback);
			Add(techLine2, ref feedback);
			Add(techLine3, ref feedback);
			Add(techLine4, ref feedback);
			Add(techLine5, ref feedback);
			Add(techLine6, ref feedback);
			Add(techLine7, ref feedback);
			Add(techLine8, ref feedback);
		}
	}
}
