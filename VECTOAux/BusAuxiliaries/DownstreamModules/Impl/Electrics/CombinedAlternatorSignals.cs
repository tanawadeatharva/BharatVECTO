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

namespace Electrics
{
	// Used by the CombinedAlternator class and any other related classes.
	public class CombinedAlternatorSignals : ICombinedAlternatorSignals
	{
		public double CrankRPM { get; set; }

		public Ampere CurrentDemandAmps { get; set; }

		// Number of alternators in the Combined Alternator
		public int NumberOfAlternators { get; set; }
	}
}
