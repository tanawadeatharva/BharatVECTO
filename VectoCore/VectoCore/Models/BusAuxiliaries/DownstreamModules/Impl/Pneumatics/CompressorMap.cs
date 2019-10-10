// Copyright 2017 European Union.
// Licensed under the EUPL (the 'Licence');
// 
// * You may not use this work except in compliance with the Licence.
// * You may obtain a copy of the Licence at: http://ec.europa.eu/idabc/eupl
// * Unless required by applicable law or agreed to in writing,
// software distributed under the Licence is distributed on an "AS IS" basis,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// 
// See the LICENSE.txt for the specific language governing permissions and limitations.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces;
using TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules.HVAC;
using TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules.PneumaticSystem;

namespace TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.Pneumatics
{
	/// <summary>
	/// 	''' Compressor Flow Rate and Power Map
	/// 	''' </summary>
	/// 	''' <remarks></remarks>
	public class CompressorMap : ICompressorMap //, IAuxiliaryEvent
	{
		private readonly string filePath;
		private double _averagePowerDemandPerCompressorUnitFlowRateLitresperSec;
		private bool _MapBoundariesExceeded;

		/// <summary>
		/// 		''' Dictionary of values keyed by the rpm valaues in the csv file
		/// 		''' Values are held as a tuple as follows
		/// 		''' Item1 = flow rate
		/// 		''' Item2 - power [compressor on] 
		/// 		''' Item3 - power [compressor off]
		/// 		''' </summary>
		/// 		''' <remarks></remarks>
		private Dictionary<int, CompressorMapValues> map;

		// Returns the AveragePowerDemand  per unit flow rate in seconds.
		public double GetAveragePowerDemandPerCompressorUnitFlowRate()
		{
			return _averagePowerDemandPerCompressorUnitFlowRateLitresperSec;
		}


		/// <summary>
		/// 		''' Creates a new instance of the CompressorMap class
		/// 		''' </summary>
		/// 		''' <param name="path">full path to csv data file</param>
		/// 		''' <remarks></remarks>
		public CompressorMap(string path)
		{
			filePath = path;
		}

		/// <summary>
		/// 		''' Initilaises the map from the supplied csv data
		/// 		''' </summary>
		/// 		''' <remarks></remarks>
		public bool Initialise()
		{
			if (File.Exists(filePath)) {
				using (var sr = new StreamReader(filePath)) {
					// get array of lines from csv
					var lines = sr.ReadToEnd().Split(new[] { Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries);

					// Must have at least 2 entries in map to make it usable [dont forget the header row]
					if (lines.Length < 3)
						throw new ArgumentException("Insufficient rows in csv to build a usable map");

					map = new Dictionary<int, CompressorMapValues>();
					var firstline = true;

					foreach (var line in lines) {
						if (!firstline) {
							// split the line
							var elements = line.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
							// 4 entries per line required
							if ((elements.Length != 4))
								throw new ArgumentException("Incorrect number of values in csv file");
							// add values to map
							try {
								map.Add(int.Parse(elements[0]), new CompressorMapValues(double.Parse(elements[1], CultureInfo.InvariantCulture).SI<NormLiterPerSecond>(), double.Parse(elements[2], CultureInfo.InvariantCulture).SI<Watt>(), double.Parse(elements[3], CultureInfo.InvariantCulture).SI<Watt>()));
							} catch (FormatException fe) {
								throw new InvalidCastException(string.Format("Compresor Map: line '{0}", line), fe);
							}
						} else
							firstline = false;
					}
				}

				// *********************************************************************
				// Calculate the Average Power Demand Per Compressor Unit FlowRate / per second.
				double powerDividedByFlowRateSum = 0;
				foreach (var speed in map)
					powerDividedByFlowRateSum += (speed.Value.PowerCompressorOn - speed.Value.PowerCompressorOff).Value() / (double)speed.Value.FlowRate.Value();

				// Map in Litres Per Minute, so * 60 to get per second, calculated only once at initialisation.
				_averagePowerDemandPerCompressorUnitFlowRateLitresperSec = (powerDividedByFlowRateSum / map.Count) * 60;
			} else
				throw new ArgumentException("supplied input file does not exist");

			// If we get here then all should be well and we can return a True value of success.
			return true;
		}

		/// <summary>
		/// 		''' Returns compressor flow rate at the given rotation speed
		/// 		''' </summary>
		/// 		''' <param name="rpm">compressor rotation speed</param>
		/// 		''' <returns></returns>
		/// 		''' <remarks>Single</remarks>
		public NormLiterPerSecond GetFlowRate(double rpm)
		{
			var val = InterpolatedTuple(rpm);
			return val.FlowRate;
		}

		/// <summary>
		/// 		''' Returns mechanical power at rpm when compressor is on
		/// 		''' </summary>
		/// 		''' <param name="rpm">compressor rotation speed</param>
		/// 		''' <returns></returns>
		/// 		''' <remarks>Single</remarks>
		public Watt GetPowerCompressorOn(double rpm)
		{
			var val = InterpolatedTuple(rpm);
			return val.PowerCompressorOn;
		}

		/// <summary>
		/// 		''' Returns mechanical power at rpm when compressor is off
		/// 		''' </summary>
		/// 		''' <param name="rpm">compressor rotation speed</param>
		/// 		''' <returns></returns>
		/// 		''' <remarks>Single</remarks>
		public Watt GetPowerCompressorOff(double rpm)
		{
			var val = InterpolatedTuple(rpm);
			return val.PowerCompressorOff;
		}

		/// <summary>
		/// 		''' Returns an instance of CompressorMapValues containing the values at a key, or interpolated values
		/// 		''' </summary>
		/// 		''' <returns>CompressorMapValues</returns>
		/// 		''' <remarks>Throws exception if rpm are outside map</remarks>
		private CompressorMapValues InterpolatedTuple(double rpm)
		{
			// check the rpm is within the map
			var min = map.Keys.Min();
			var max = map.Keys.Max();

			if (rpm < min || rpm > max) {
				if (!_MapBoundariesExceeded) {
					OnMessage(this, string.Format("Compresser : limited RPM of '{2}' to extent of map - map range is {0} to {1}", min, max, rpm), AdvancedAuxiliaryMessageType.Warning);
					_MapBoundariesExceeded = true;
				}

				// Limiting as agreed.
				if (rpm > max)
					rpm = max;
				if (rpm < min)
					rpm = min;
			}

			// If supplied rpm is a key, we can just return the appropriate tuple
			var intRpm = System.Convert.ToInt32(rpm);
			if (rpm.IsEqual(intRpm) && map.ContainsKey(intRpm))
				return map[intRpm];

			// Not a key value, interpolate
			// get the entries before and after the supplied rpm
			var pre = (from m in map
														  where m.Key < rpm
														  select m).Last();
			var post = (from m in map
														   where m.Key > rpm
														   select m).First();

			// get the delta values for rpm and the map values
			double dRpm = post.Key - pre.Key;
			var dFlowRate = post.Value.FlowRate - pre.Value.FlowRate;
			var dPowerOn = post.Value.PowerCompressorOn - pre.Value.PowerCompressorOn;
			var dPowerOff = post.Value.PowerCompressorOff - pre.Value.PowerCompressorOff;

			// calculate the slopes
			var flowSlope = dFlowRate.Value() / dRpm;
			var powerOnSlope = dPowerOn.Value() / dRpm;
			var powerOffSlope = dPowerOff.Value() / dRpm;

			// calculate the new values
			var flowRate = (((rpm - pre.Key) * flowSlope).SI<NormLiterPerSecond>() + pre.Value.FlowRate);
			var powerCompressorOn = (((rpm - pre.Key) * powerOnSlope).SI<Watt>() + pre.Value.PowerCompressorOn);
			var powerCompressorOff = (((rpm - pre.Key) * powerOffSlope).SI<Watt>() + pre.Value.PowerCompressorOff);

			// Build and return a new CompressorMapValues instance
			return new CompressorMapValues(flowRate, powerCompressorOn, powerCompressorOff);
		}

		/// <summary>
		/// 		''' Encapsulates compressor map values
		/// 		''' Flow Rate
		/// 		''' Power - Compressor On
		/// 		''' Power - Compressor Off
		/// 		''' </summary>
		/// 		''' <remarks></remarks>
		/// 		'''

		private struct CompressorMapValues
		{
			/// <summary>
			/// 			''' Compressor flowrate
			/// 			''' </summary>
			/// 			''' <remarks></remarks>
			public readonly NormLiterPerSecond FlowRate;

			/// <summary>
			/// 			''' Power, compressor on
			/// 			''' </summary>
			/// 			''' <remarks></remarks>
			public readonly Watt PowerCompressorOn;

			/// <summary>
			/// 			''' Power compressor off
			/// 			''' </summary>
			/// 			''' <remarks></remarks>
			public readonly Watt PowerCompressorOff;

			/// <summary>
			/// 			''' Creates a new instance of CompressorMapValues
			/// 			''' </summary>
			/// 			''' <param name="flowRate">flow rate</param>
			/// 			''' <param name="powerCompressorOn">power - compressor on</param>
			/// 			''' <param name="powerCompressorOff">power - compressor off</param>
			/// 			''' <remarks></remarks>
			public CompressorMapValues(NormLiterPerSecond flowRate, Watt powerCompressorOn, Watt powerCompressorOff)
			{
				this.FlowRate = flowRate;
				this.PowerCompressorOn = powerCompressorOn;
				this.PowerCompressorOff = powerCompressorOff;
			}
		}


		public event MessageEventHandler Message;

		//public delegate void MessageEventHandler(ref object sender, string message, AdvancedAuxiliaryMessageType messageType);

		private void OnMessage(object sender, string message, AdvancedAuxiliaryMessageType messageType)
		{
			if (message != null) {
				object compressorMap = this;
				Message?.Invoke(ref compressorMap, message, messageType);
			}

		}

		#region Implementation of IAuxiliaryEvent

		//public event AuxiliaryEventEventHandler AuxiliaryEvent;

		#endregion
	}
}
