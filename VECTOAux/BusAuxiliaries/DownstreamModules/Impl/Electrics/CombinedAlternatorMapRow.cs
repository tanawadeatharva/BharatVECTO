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

namespace Electrics
{
	// This class is reflective of the stored entries for the combined alternator
	// And is used by the Combined Alternator Form and any related classes.

	public class CombinedAlternatorMapRow : ICombinedAlternatorMapRow
	{
		public string AlternatorName { get; set; }
		public double RPM { get; set; }
		public double Amps { get; set; }
		public double Efficiency { get; set; }
		public double PulleyRatio { get; set; }

		// Constructors
		public CombinedAlternatorMapRow()
		{
		}

		public CombinedAlternatorMapRow(string alternatorName, double rpm, double amps, double efficiency, double pulleyRatio)
		{

			// Sanity Check
			if (alternatorName.Trim().Length == 0)
				throw new ArgumentException("Alternator name cannot be zero length");
			if (efficiency < 0 | efficiency > 100)
				throw new ArgumentException("Alternator Efficiency must be between 0 and 100");
			if (pulleyRatio <= 0)
				throw new ArgumentException("Alternator Pully ratio must be a positive number");

			// Assignments
			AlternatorName = alternatorName;
			RPM = rpm;
			Amps = amps;
			Efficiency = efficiency;
			PulleyRatio = pulleyRatio;
		}
	}
}
