using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Cryptography.X509Certificates;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.SimulationComponent;
using TUGraz.VectoCore.Models.SimulationComponent.Data.ElectricComponents;
using TUGraz.VectoCore.Models.SimulationComponent.Data.ElectricComponents.Battery;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.OutputData.ModDataPostprocessing.Impl
{
	public class TracingInfinityBatterySystem : StatefulVectoSimulationComponent<TracingInfinityBatterySystem.TracingInfinityBatteryState>, IElectricEnergyStoragePort, IRESSInfo
	{

		public class TracingInfinityBatteryState
		{
			public double StateOfCharge;
			public double MinSoc;
			public double MaxSoc;

			public Volt InternalVoltage;
			public Ampere Current;
		}

		private BatterySystem _infinityBat;
#if DEBUG
		public IList<double> SoCTrace = new List<double>();
#endif

        public TracingInfinityBatterySystem(BatterySystemData batData) : base(null)
		{
			var tmp_batData = batData.Clone();
			tmp_batData.ChargeSustainingBatterySystem = true;
			_infinityBat = new BatterySystem(null, tmp_batData); ;
		}


        #region Implementation of IElectricEnergyStoragePort

		public static double CalculateDeltaSOC(BatterySystem batterySystem, IRESSResponse response, out Ampere current)
		{
			var totalCapacity = batterySystem.Capacity;




			current = (response.PowerDemand - response.LossPower) / response.InternalVoltage;
			var energy = current * response.SimulationInterval;




			var energy_in_bat = totalCapacity * batterySystem.StateOfCharge;


			var remaining_energy = energy_in_bat + energy;



			var remaining_soc = remaining_energy / batterySystem.Capacity;
			var deltaSoc = remaining_soc - batterySystem.StateOfCharge;


			return deltaSoc;
		}


		public void Initialize(double initialSoC)
		{

			_infinityBat.Initialize(initialSoC);
			PreviousState.StateOfCharge = initialSoC;
			SoCTrace.Clear();
			SoCTrace.Add(initialSoC);
        }

		public IRESSResponse Request(Second absTime, Second dt, Watt powerDemand, bool dryRun)
		{
			var response = _infinityBat.Request(absTime, dt, powerDemand, dryRun);

            if (response is RESSResponseSuccess responseSuccess) {


				var dSoc = CalculateDeltaSOC(batterySystem: _infinityBat, response: responseSuccess, out var current);


				CurrentState.Current = current;
				CurrentState.InternalVoltage = responseSuccess.InternalVoltage;
				CurrentState.StateOfCharge = PreviousState.StateOfCharge + dSoc;
				SoCTrace.Add(CurrentState.StateOfCharge);
			}


			return response;
		}

		#endregion

		#region Overrides of VectoSimulationComponent

		protected override void DoWriteModalResults(Second time, Second simulationInterval, IModalDataContainer container)
		{
			container[ModalResultField.P_reess_int] = CurrentState.InternalVoltage * CurrentState.Current;

			container[ModalResultField.REESSStateOfCharge] = CurrentState.StateOfCharge.SI<Scalar>();
		}

		protected override void DoCommitSimulationStep(Second time, Second simulationInterval)
		{
			AdvanceState();

		}

		protected override bool DoUpdateFrom(object other)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region Implementation of IRESSInfo

		public Volt InternalVoltage => _infinityBat.InternalVoltage;

		public double StateOfCharge => PreviousState.StateOfCharge;

		public WattSecond StoredEnergy => _infinityBat.StoredEnergy;

		public Watt MaxChargePower(Second dt)
		{
			return _infinityBat.MaxChargePower(dt);
		}

		public Watt MaxDischargePower(Second dt)
		{
			return _infinityBat.MaxDischargePower(dt);
		}

		public double MinSoC => _infinityBat.MinSoC;

		public double MaxSoC => _infinityBat.MaxSoC;

		public AmpereSecond Capacity => _infinityBat.Capacity;

		public Volt NominalVoltage => _infinityBat.NominalVoltage;

		#endregion
	}

	public interface IFuelCellPostProcessingInfo
	{
		Meter WindowSize { get; }
		int BinarySearchIterations { get;}
		double SoC { get;  }


		FuelCellPreRunPostprocessor.SearchResult[] AcceptedSearchResults { get; }
		FuelCellPreRunPostprocessor.SearchResult[] RejectedSearchResults { get; }
	}

	public class FuelCellPreRunPostprocessor : IFuelCellPostProcessingInfo
	{
		public DebugData _debug = new DebugData();

		public ModalDataContainer ModData;

		public SearchResult[] AcceptedSearchResults { get; private set; }
		public SearchResult[] RejectedSearchResults { get; private set; }


		public int BinarySearchIterations { get; private set; }

		public double SoC { get; private set; }

		public Meter WindowSize { get; private set; }

		private static object fileLock = new object();

		public class SearchResult
		{
			public FCCalcEntry[] entries;
			public double initSOC;
			public bool success;
			public Meter distance;
		}

		public FuelCellPreRunPostprocessor(IModalDataContainer modData)
		{
			ModData = modData as ModalDataContainer;
		}

		public SearchResult CalculateFuelCellPowerDemand(FuelCellSystemData fcData, BatterySystemData batData)
		{
			var fcSufficient = CheckFCPower(fcData, batData);
			if (!fcSufficient) {
				throw new Exception($"FuelCell power not sufficient");
            }



			//try with window size = full distance
			
			var success = CalculateFuelCellPowerDemandForWindowSize(ModData.Distance, fcData, batData, out var entries, out var soC);
			//TODO: return determined init SOC
			if (success) {
				return new SearchResult() {
					entries = entries,
					distance = ModData.Distance,
					initSOC = soC,
				};
			}

			double searchThreshold = 0.02;
			var minWindowSize = 5.SI<Meter>();
            //Start binary search
            var accepted = new List<SearchResult>();
			// we can add the full distance already beforehand because the search starts only if the full distance is not 
			// feasible.
            var rejected = new List<SearchResult>() { new SearchResult() {
				distance = ModData.Distance,
				success = false,
			} };
			var iterationCount = 0;
			try {
				SearchAlgorithm.BinarySearch(0.SI<Meter>(), ModData.Distance,
					evaluateFunction:
					distance => {
						var calcSuccess = CalculateFuelCellPowerDemandForWindowSize(distance, fcData, batData,
							out var calcEntries, out var SoC);
						return new SearchResult() {
							distance = distance,
							entries = calcEntries,
							initSOC = SoC,
							success = calcSuccess
						};
					},
					acceptFunction:
					(distance, result) => {
						var searchResult = (SearchResult)result;
						if (searchResult.success) {
							accepted.Add(searchResult);
						} else {
							rejected.Add(searchResult);
						}

						return searchResult.success;

					},
					abortCriterion: (d, o) => {
						if (d < minWindowSize) {
							throw new VectoSearchAbortedException(
								$"Window size < {minWindowSize}, battery is too small");
						}

						if (!accepted.Any() || !rejected.Any()) {
							return false;
						}

						var lastAccepted = accepted.Last();
						var lastRejected = rejected.Last();
						var deviation = Math.Abs((lastRejected.distance - lastAccepted.distance) /
							(lastRejected.distance + lastAccepted.distance) * 2);


						return deviation < searchThreshold;
					},
					ref iterationCount,
					searcher: this
				);
			} finally {
				BinarySearchIterations = iterationCount;
				WindowSize = accepted.LastOrDefault()?.distance;
				RejectedSearchResults = rejected.ToArray();
				AcceptedSearchResults = accepted.ToArray();
				SoC = accepted.LastOrDefault()?.initSOC ?? 0.0;
			}
			return accepted.Last();
        }

		public bool TryShiftInitialSoC(double batMinSoc, double batMaxSoc, double initSoc, double minSocTrace,
			double maxSocTrace, out double shiftedSoc)
		{
			shiftedSoc = -1;
			if (batMaxSoc - batMaxSoc < maxSocTrace - minSocTrace) {
				return false;
			} else {
				throw new NotFiniteNumberException("Remove, just interested if this happens at least sometimes");
			}
			//check difference
			var initSoc_min = batMinSoc + (initSoc - minSocTrace);
			var initSoc_max = batMaxSoc - (maxSocTrace - initSoc);

			//TODO: add more checks and throw exceptions ;) 
			shiftedSoc = (initSoc_min + initSoc_max) / 2;

			return true;
		}



		public bool CalculateFuelCellPowerDemandForWindowSize(Meter windowSize, FuelCellSystemData fcData,
			BatterySystemData batData, out FCCalcEntry[] entries, out double SoC)
		{
			batData = batData.Clone();
			var tmpBatSystem = new BatterySystem(null, batData);
			SoC = batData.InitialSoC;


			var minSoc = tmpBatSystem.MinSoC;
			var maxSoc = tmpBatSystem.MaxSoC;

			//Use safety margins here
			var success = CalculateFuelCellPowerDemandForSoC(windowSize, fcData, batData, out entries, SoC, out var traceMinSoc, out var traceMaxSoc);
			_debug.Add($"min_trace {traceMinSoc} max_trace {traceMaxSoc} range {traceMaxSoc - traceMinSoc}");

			var energy_safety_margin = entries.Average(e => e.P_Bat_T.Value()).SI<Watt>() * 10.SI<Second>();

			var dSocSafety = Math.Abs(((energy_safety_margin / tmpBatSystem.NominalVoltage) / tmpBatSystem.TotalCapacity ).Value());
			var minSocSafe = minSoc + dSocSafety;
			var maxSocSafe = maxSoc - dSocSafety;



			if (!success) {
				if (TryShiftInitialSoC(minSocSafe, maxSocSafe, batData.InitialSoC, traceMinSoc, traceMaxSoc, out SoC)) {

					throw new Exception("SOC Shifted");
					if (!CalculateFuelCellPowerDemandForSoC(windowSize, fcData, batData, out entries, SoC,
							out var _, out var _)) {
						throw new Exception();

					};
				} else {
					return false;
				}
			}
			return CalculateWithRealBattery(batData, SoC, entries);
		}

		public bool CalculateWithRealBattery(BatterySystemData batData, double SoC, FCCalcEntry[] entries)
		{
			batData = batData.Clone();
			batData.ChargeSustainingBatterySystem = false;
			var batSystem = new BatterySystem(null, batData);
			batSystem.Initialize(SoC);
			var dummyContainer = new SimpleModDataContainer();
			for (int i = 0; i < entries.Length; i++)
			{
				var entry = entries[i];

				//var batPower = entry.P_Bat_T_Final;


				var batPower = entry.P_Bat_T_Final.LimitTo(batSystem.MaxDischargePower(entry.dt),
					batSystem.MaxChargePower(entry.dt));
                //Limit batPower
                var batResponse = batSystem.Request(entry.Time, entry.dt, batPower, false);
				if (!(batResponse is RESSResponseSuccess))
				{
					return false;
				}
				batSystem.CommitSimulationStep(entry.Time, entry.dt, dummyContainer);
			}

			return true;
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="windowSize"></param>
		/// <param name="fcData"></param>
		/// <param name="batData"></param>
		/// <param name="fcCalcEntries"></param>
		/// <param name="minSoc_trace">the lowest SoC of the tracing infinity battery</param>
		/// <param name="maxSoc_trace">the highest SoC of the tracing infinity battery</param>
		/// <returns></returns>
		public bool CalculateFuelCellPowerDemandForSoC(Meter windowSize, FuelCellSystemData fcData,
			BatterySystemData batData, out FCCalcEntry[] fcCalcEntries, double initSoc, out double minSoc_trace,out double maxSoc_trace)
		{
			var minFcPower = fcData.MinPower;
			var maxFcPower = fcData.MaxPower;

			batData.ChargeSustainingBatterySystem = true;
			batData.InitialSoC = initSoc;

            //Debug.Assert(!batData.ChargeSustainingBatterySystem, "Provide real battery");


			

			//Calculate raw fuel cell demand ----------------------------------------------------------------------------------
			var batterySystem = new BatterySystem(null, batData);
			var batMaxSoc = batterySystem.MaxSoC;
			var batMinSoc = batterySystem.MinSoC;

			batterySystem.Initialize(batData.InitialSoC);
			fcCalcEntries = GetRawFuelCellPowerDemand(windowSize: windowSize, minFcPower: minFcPower, maxFcPower: maxFcPower, bat: batterySystem);
			//-----------------------------------------------------------------------------------------------------------------



			//Determine battery energy losses ---------------------------------------------------------------------------------
			var infinityBatterySystem = new TracingInfinityBatterySystem(batData.Clone());
			infinityBatterySystem.Initialize(batData.InitialSoC);

			var deltaEnergyBatInt = 0.SI<WattSecond>();
			var timeFcCanChange = 0.SI<Second>();

			var infinityDummyContainer = new SimpleModDataContainer();
			
			var countChange = 0;

			minSoc_trace = batData.InitialSoC;
			maxSoc_trace = batData.InitialSoC;

			for (var i = 0; i < fcCalcEntries.Length; i++) {
				var entry = fcCalcEntries[i];
				// limit P_Bat_T to battery min/max


				var batPower = entry.P_Bat_T.LimitTo(
					infinityBatterySystem.MaxDischargePower(entry.dt),
					infinityBatterySystem.MaxChargePower(entry.dt));


				var batResonse = infinityBatterySystem.Request(entry.Time, entry.dt, batPower, false);
				infinityBatterySystem.CommitSimulationStep(entry.Time, entry.dt, infinityDummyContainer);

				///Column N in excel
				deltaEnergyBatInt += infinityDummyContainer.P_REES_int * entry.dt;

				entry.P_REESS_int = infinityDummyContainer.P_REES_int;
				entry.SoC = infinityDummyContainer.SoC;
				// check SoC min/max over cycle
				// check P_bat min/max (
				entry.CanChangeFCPower = !entry.P_FC.IsEqual(0) &&
											(entry.P_FC_corr.IsSmaller(maxFcPower) || entry.P_FC_corr.IsGreater(minFcPower));
				if (entry.CanChangeFCPower) {
					timeFcCanChange += entry.dt;
					countChange++;
				}

				minSoc_trace = VectoMath.Min(minSoc_trace, infinityDummyContainer.SoC);
				maxSoc_trace = VectoMath.Max(maxSoc_trace, infinityDummyContainer.SoC);

				//Add again
				//if (maxSoc_trace - minSoc_trace > batMaxSoc - batMinSoc) {
				//	return false;
				//}
			}




			infinityBatterySystem.Initialize(initSoc);
			infinityDummyContainer = new SimpleModDataContainer();

			var remainingTime = timeFcCanChange;
			
			foreach (var entry in fcCalcEntries) {
				var fcPower = entry.P_FC; 
				//var fcPower = entry.P_FC_corr;


				if (entry.CanChangeFCPower) {
					Watt batChangeTarget = entry.CanChangeFCPower ? -deltaEnergyBatInt / remainingTime : 0.SI<Watt>();

					var fcChangeTarget = batChangeTarget / (1 - entry.P_Bat_loss / entry.P_Bat_T);

					var fcPowerTarget = fcPower + fcChangeTarget;
					var fcPowerActual = fcPowerTarget
											- VectoMath.Max(fcPowerTarget - maxFcPower, 0.SI<Watt>())
											- VectoMath.Min(fcPowerTarget - minFcPower, 0.SI<Watt>());



					var fcChangeActual = fcChangeTarget - (fcPowerTarget - fcPowerActual);
                    var batChangeActual = fcChangeActual * (1 - entry.P_Bat_loss / entry.P_Bat_T);


					


					deltaEnergyBatInt += batChangeActual * entry.dt;
					remainingTime -= entry.dt;



					entry.delta_P_FCS = fcChangeActual;
					entry.FCPowerFinal = fcPowerActual;
                    if (!entry.FCPowerFinal.IsBetween(minFcPower, maxFcPower))
					{

						//WriteEntriesToFile(windowSize, fcCalcEntries, initSoc, deltaEnergyBatInt, timeFcCanChange);
                        //throw new VectoException("Fuel cell limits violated");
                    }
            
                } else {
					entry.FCPowerFinal = entry.P_FC_corr;
					entry.delta_P_FCS = 0.SI<Watt>();
				}


			}

			batData.ChargeSustainingBatterySystem = false;
			var batSystem = new BatterySystem(null, batData);
			batSystem.Initialize(initSoc);

			var dummyContainer = new SimpleModDataContainer();


			var realBatTraceMin = initSoc;
			var realBatTraceMax = initSoc;
			foreach (var entry in fcCalcEntries) {
                //var batPower = entry.P_Bat_T.LimitTo(batSystem.MaxDischargePower(entry.dt),
                //	batSystem.MaxChargePower(entry.dt));

				var batPower = entry.P_Bat_T_Final.LimitTo(batSystem.MaxDischargePower(entry.dt),
					batSystem.MaxChargePower(entry.dt));


                var batResponse = batSystem.Request(entry.Time, entry.dt, batPower, false);
				if (!(batResponse is RESSResponseSuccess)) {

				}
				batSystem.CommitSimulationStep(entry.Time, entry.dt, dummyContainer);
				realBatTraceMax = VectoMath.Max(realBatTraceMax, dummyContainer.SoC);
				realBatTraceMin = VectoMath.Min(realBatTraceMin, dummyContainer.SoC);


			}





			WriteEntriesToFile(windowSize, fcCalcEntries, initSoc, deltaEnergyBatInt, timeFcCanChange);


			if ((realBatTraceMax - realBatTraceMin) <= (batMaxSoc - batMinSoc)) {
				return true;
			}


            if (maxSoc_trace > batMaxSoc || minSoc_trace < batMinSoc) {
				return false;
			}

			return true;
		}

		private void WriteEntriesToFile(Meter windowSize, FCCalcEntry[] fcCalcEntries, double initSoc,
			WattSecond deltaEnergyBatInt, Second timeFcCanChange)
		{

			lock (fileLock) {
				using (var fs = new StreamWriter(
							$"fuelcell_data_{ModData.RunName}_{Math.Round(windowSize.Value(), 0)}_soc_{initSoc}.csv")) {
					fs.WriteLine($"DeltaEnergyBat: {deltaEnergyBatInt} / {deltaEnergyBatInt.ConvertToKiloWattHour()}");
					fs.WriteLine($"timeFCCanIncrease: {timeFcCanChange}");
					fs.WriteLine(FCCalcEntry.Header);
					foreach (var entry in fcCalcEntries) {
						fs.WriteLine(entry.ToString());
					}
				}
			}
		}

		/// <summary>
		/// Check if fuelcell can supply enough power over the cycle.
		/// </summary>
		/// <returns></returns>
		public bool CheckFCPower(FuelCellSystemData fcData, BatterySystemData batData)
		{
			var minFcPower = fcData.FuelCells.Single().MinElectricPower;
			var maxFcPower = fcData.FuelCells.Single().MaxElectricPower;

			var tmpBatSystem = new BatterySystem(null, batData);
			tmpBatSystem.Initialize(batData.InitialSoC);


            var entries = GetRawFuelCellPowerDemand(ModData.Distance, minFcPower, maxFcPower, tmpBatSystem);
			if (entries.First().P_FC_raw > maxFcPower) {
				return false;
			}

			var tracingInfinityBat = new TracingInfinityBatterySystem(batData.Clone());
			tracingInfinityBat.Initialize(batData.InitialSoC);
			var minSocTrace = batData.InitialSoC;
			var simpleModData = new SimpleModDataContainer();

			var usableSocRange = tmpBatSystem.MaxSoC - tmpBatSystem.MinSoC;
            foreach (var entry in entries) {
				entry.P_FC = maxFcPower; // Use max power all the time

				var response = tracingInfinityBat.Request(entry.Time, entry.dt, entry.P_Bat_T, false);
				if (response is RESSOverloadResponse ovl) {
					entry.P_FC = response.MaxChargePower - entry.P_el_dem; //In case we are recuperating
					response = tracingInfinityBat.Request(entry.Time, entry.dt, entry.P_Bat_T, false);
                }

                tracingInfinityBat.CommitSimulationStep(entry.Time, entry.dt, simpleModData);

				minSocTrace = VectoMath.Min(minSocTrace, tracingInfinityBat.StateOfCharge);
				var delta_soc_discharge = batData.InitialSoC - minSocTrace;
				if (delta_soc_discharge > usableSocRange) {
					return false;
				}
			}











			return true;
		}

		protected FCCalcEntry[] GetRawFuelCellPowerDemand(Meter windowSize, Watt minFcPower, Watt maxFcPower, BatterySystem bat)
		{
			Debug.Assert(minFcPower < maxFcPower, "min power must be smaller than max power");
			
			
			
			if (windowSize.IsGreater(ModData.Distance)) {
				throw new VectoException("Window size must not exceed cycle distance!");
			}
			var data = ModData.Data;
            var timeCol = data.Columns[ModalResultField.time.GetShortCaption()];
			var dtCol = data.Columns[ModalResultField.simulationInterval.GetName()];
			var distCol = data.Columns[ModalResultField.dist.GetShortCaption()];
			var esTCol = data.Columns[ModalResultField.P_terminal_ES.GetName()];

			if (windowSize.IsEqual(ModData.Distance)) {
				var P_FC_raw = ModData.TimeIntegral<WattSecond>(ModalResultField.P_terminal_ES) / ModData.Duration;
				var retVal = ModData.Data.AsEnumerable().Select(r => {
					var entry = new FCCalcEntry() {
						Time = (Second)r[timeCol],
						dt = (Second)r[dtCol],
						Distance = (Meter)r[distCol],
						P_FC_raw = -P_FC_raw,
						P_el_dem = (Watt)r[esTCol],
					};
					entry.P_FC = entry.P_FC_raw > minFcPower
						? VectoMath.Min(entry.P_FC_raw, maxFcPower)
						: entry.P_FC_raw.IsSmaller(0)
							? 0.SI<Watt>()
							: minFcPower;
                    var batResonse = bat.Request(entry.Time, entry.dt, entry.P_Bat_T, true);
					entry.P_Bat_loss = batResonse.LossPower;

                    return entry;
				}).ToArray();
				return retVal;
			}

            var wIt = new ModDataWindowIterator(ModData, windowSize);

			var processed = new FCCalcEntry[ModData.Data.Rows.Count];
			for (; !wIt.CycleEndReached; wIt.MoveNext()) {
				var entry = new FCCalcEntry() {
					Time = (Second)data.Rows[wIt.Position][timeCol],
					dt = (Second)data.Rows[wIt.Position][dtCol],
					Distance = (Meter)data.Rows[wIt.Position][distCol]
				};
				var time = 0.SI<Second>();
				var emEnergy = 0.SI<WattSecond>();
				for (; !wIt.WindowEndReached; wIt.NextEntry()) {
					time += (Second)data.Rows[wIt.Current][dtCol];
					emEnergy += (Second)data.Rows[wIt.Current][dtCol] * (Watt)data.Rows[wIt.Current][esTCol];
				}

				entry.P_FC_raw = -emEnergy / time;
				entry.P_FC = entry.P_FC_raw > minFcPower
					? VectoMath.Min(entry.P_FC_raw, maxFcPower)
					: entry.P_FC_raw.IsSmaller(0)
						? 0.SI<Watt>()
						: minFcPower;
				entry.P_el_dem = (Watt)data.Rows[wIt.Position][esTCol];
				var batResonse = bat.Request(entry.Time, entry.dt, entry.P_Bat_T, true);
				entry.P_Bat_loss = batResonse.LossPower;
				processed[wIt.Position] = entry;
			}

			return processed;
		}

		public class FCCalcEntry
		{
			public Second Time { get; set; }
			public Second dt { get; set; }
			public Meter Distance { get; set; }


			/// <summary>
			/// P_ES_t, electric demand from powertrain + aux.
			/// </summary>
			public Watt P_el_dem { get; set; }



			/// <summary>
			/// P_ES_t, electric demand from powertrain + aux. windowed average
			/// </summary>
			public Watt P_el_dem_w { get; set; }


			/// <summary>
			/// Fuel Cell power without any corrections
			/// </summary>
			public Watt P_FC_raw { get; set; }

			/// <summary>
			/// Fuel Cell power limited to Min/Max power, zero => fuel cell is off
			/// </summary>
			public Watt P_FC { get; set; }


			/// <summary>
			/// Remaining power that should be provided by battery
			/// </summary>
			public Watt P_Bat_T => P_FC + P_el_dem;


			/// <summary>
			/// Battery losses from P_Bat_T
			/// </summary>
			public Watt P_Bat_loss { get; set; }


			/// <summary>
			/// Power provided by fuel cell including battery losses
			/// </summary>
			public Watt P_FC_corr => P_FC.IsEqual(0) ? P_FC : P_FC + P_Bat_loss;

			/// <summary>
			/// Fuel Cell power demand
			/// </summary>
			public Watt delta_Power_corr => P_FC_corr + P_el_dem;

			public double SoC { get; set; }

			public bool CanChangeFCPower { get; set; }



			public Watt FCPowerFinal { get; set; }

			/// <summary>
			/// 
			/// </summary>
			public Watt P_Bat_T_Final => FCPowerFinal + P_el_dem;


			public Watt P_REESS_int { get; set; }


			/// <summary>
			/// 
			/// </summary>
			public Watt delta_P_FCS { get; set; }


			public static string Header => "Time," +
											" dt, " +
											"Distance, " +
											"P_el_dem," +
											" " +
											"P_FC_raw, " +
											"P_FC, " +
											"P_Bat_T, " +
											"P_Bat_loss, " +
											"P_FC_corr, " +
											"delta_Power_corr, " +
											"P_REESS_int, " +
											"SoC, " +
											"CanChangeFCPower, " +
											"FCPowerFinal," +
											"delta_P_FCS";
            #region Overrides of Object

            public override string ToString()
			{
				return  $"{Time.ToXMLFormat()}," +
						$"{dt.ToXMLFormat()}, " +
						$"{Distance.ToXMLFormat()}, " +
						$"{P_el_dem.ToXMLFormat()}, " +
						$"{P_FC_raw.ToXMLFormat()}, " +
						$"{P_FC.ToXMLFormat()}," +
						$"{P_Bat_T.ToXMLFormat()}, " +
						$"{P_Bat_loss.ToXMLFormat()}, " +
						$"{P_FC_corr.ToXMLFormat()}, " +
						$"{delta_Power_corr.ToXMLFormat()}, " +
						$"{P_REESS_int.ToXMLFormat()}, " +
						$"{SoC.ToXMLFormat()}, " +
						$"{CanChangeFCPower}, " +
						$"{FCPowerFinal.ToXMLFormat()}, " +
						$"{delta_P_FCS.ToXMLFormat()}";
			}

			#endregion
		}
	}


	public class ModDataWindowIterator
	{
		public ModalDataContainer ModData { get; set; }

		protected DataColumn DistanceColumn { get; }

		//protected ModDataAsRingBuffer RingBuffer { get; }

		protected Meter WindowDistance { get; }

		protected int NumRows;
		public int Position { get; protected set; }
		protected internal int Start { get; protected set; }
		protected internal int End { get; protected set; }
		public int Current { get; protected set; }

		protected bool CycleStart = true;

		public bool CycleEndReached { get; protected set; } = false;

		public bool WindowEndReached { get; protected set; } = false;

		public Meter CycleStartDistance { get; set; }

        public ModDataWindowIterator(ModalDataContainer results, Meter windowDistance)
		{
			ModData = results;
			NumRows = ModData.Data.Rows.Count;
			
			WindowDistance = windowDistance;

			DistanceColumn = results.Data.Columns[ModalResultField.dist.GetShortCaption()];
			CycleStartDistance = (Meter)results.Data.Rows[0][DistanceColumn] - (Meter)results.Data.Rows[0][ModalResultField.simulationDistance.GetName()];


            if (windowDistance.IsGreaterOrEqual(ModData.Distance)) {
				throw new VectoException("Window size must be smaller than total cycle distance");
			}
			
			SetIteratorPositions(0);
		}


		public virtual void MoveNext()
		{
			if (CycleEndReached) {
				//return;
				throw new VectoException("Failed to move iterator forward - iterated all elements");

			}

			CycleStart = false;

			var newStart = GetNextRow(Position);
			SetIteratorPositions(newStart);
			//var nextWindow = GetNextRow(Position);
			if (Position == 0) {
				CycleEndReached = true;
			}
		}

		public virtual void NextEntry()
		{
			if (WindowEndReached) {
				return;
			}
			Current = GetNextRow(Current);
			if (Current == GetNextRow(End)) {
				WindowEndReached = true;
			}
		}

		public override string ToString()
		{
			
            return
				$"ModDataWindow: wndSize: {WindowDistance}, Position: {Position} ({ModData.Data.Rows[Position][DistanceColumn]} " +
				$"Start: {Start} ({ModData.Data.Rows[Start][DistanceColumn]}) " +
				$"End: {End} ({ModData.Data.Rows[End][DistanceColumn]}) " +
				$"ActualWindowSize: {ActualWindowSize}";
		}

		public virtual Meter ActualWindowSize
		{
			get
			{
				var startDistance = (Meter)ModData.Data.Rows[Start][DistanceColumn];
				var endDistance = (Meter)ModData.Data.Rows[End][DistanceColumn];
				var diff = endDistance - startDistance;

                return diff < 0 ? diff + ModData.Distance : diff;
			}
		}

		protected void SetIteratorPositions(int position)
		{
			Position = position;
			
            var currentDistance = (Meter)ModData.Data.Rows[Position][DistanceColumn];
			var startDistance = GetDistanceInCycle(currentDistance - WindowDistance / 2.0);
			var endDistance = GetDistanceInCycle(currentDistance + WindowDistance / 2.0);

            if (CycleStart) {
				End = MoveIteratorForward(Position, r => (Meter)(ModData.Data.Rows[r][DistanceColumn]) <= endDistance);
				Start = MoveIteratorBackward(GetPreviousRow(Position), r => (Meter)(ModData.Data.Rows[r][DistanceColumn]) <= startDistance);
			} else {
				if (endDistance < (Meter)ModData.Data.Rows[End][DistanceColumn]) {
					End = MoveIteratorForward(0, r => (Meter)(ModData.Data.Rows[r][DistanceColumn]) <= endDistance);
				} else {
					End = MoveIteratorForward(End, r => {
						if (endDistance.IsEqual(ModData.Distance) && r == 0) {
							return false;
						}
						return (Meter)(ModData.Data.Rows[r][DistanceColumn]) <= endDistance;
					});
				}

				if (startDistance < (Meter)ModData.Data.Rows[Start][DistanceColumn]) {
					Start = MoveIteratorForward(0, r => (Meter)(ModData.Data.Rows[r][DistanceColumn]) <= startDistance);
				} else {
					Start = GetNextRow(MoveIteratorForward((Start),
						r => (Meter)(ModData.Data.Rows[r][DistanceColumn]) <= startDistance));
				}
            }

			Current = Start;
			WindowEndReached = false;
		}

		protected virtual SIBase<Meter> GetDistanceInCycle(Meter relativeDistance)
		{
			var cycleDistance = ModData.Distance;

            while (relativeDistance.IsGreater(CycleStartDistance + cycleDistance)) {
				relativeDistance -= cycleDistance;
			}

			while (relativeDistance.IsSmaller(CycleStartDistance)) {
				relativeDistance += cycleDistance;
			}
			return relativeDistance;
		}

		protected virtual int MoveIteratorForward(int startIdx, Func<int, bool> action)
		{
			var retVal = startIdx;
			var initialValue = action(startIdx);
			do {
				retVal = GetNextRow(retVal);
				if (action(retVal) != initialValue) {
					return GetPreviousRow(retVal);
				}
			} while (retVal != startIdx);

			return retVal;
		}


		protected virtual int MoveIteratorBackward(int startIdx, Func<int, bool> action)
		{
			var retVal = startIdx;
			var initialValue = action(startIdx);
			do {
				retVal = GetPreviousRow(retVal);
				if (action(retVal) != initialValue) {
					return GetNextRow(retVal);
				}

				if (retVal == startIdx) {
					throw new VectoException("Failed to move iterator forward - iterated all elements");
				}
			} while (true);
		}

        protected virtual int GetNextRow(int rowIdx)
		{
			return (rowIdx + 1) % NumRows;

		}

		protected virtual int GetPreviousRow(int rowIdx)
		{
			var retVal = (rowIdx - 1) % NumRows;
			if (retVal < 0) {
				retVal += NumRows;
			}
			return retVal;
		}

    }

}