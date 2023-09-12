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
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.ElectricComponents;
using TUGraz.VectoCore.Models.SimulationComponent.Data.ElectricComponents.Battery;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.OutputData.ModDataPostprocessing.Impl.FuelCell
{
	public interface IFuelCellPostProcessingInfo
    {
        Meter WindowSize { get; }
        int BinarySearchIterations { get; }
        double SoC { get; }


        FuelCellPreRunPostprocessor.SearchResult[] AcceptedSearchResults { get; }
        FuelCellPreRunPostprocessor.SearchResult[] RejectedSearchResults { get; }
    }

    public partial class FuelCellPreRunPostprocessor : IFuelCellPostProcessingInfo
    {
        public DebugData _debug = new DebugData();

        public ModalDataContainer ModData;

        public SearchResult[] AcceptedSearchResults { get; private set; }
        public SearchResult[] RejectedSearchResults { get; private set; }


        public int BinarySearchIterations { get; private set; }

        public double SoC { get; private set; }

        public Meter WindowSize { get; private set; }
        
        private static object fileLock = new object();
		private readonly string _outputPath;

		public class SearchResult
        {
            public FCCalcEntry[] entries;
            public double initSOC;
            public bool success;
            public Meter distance;
        }

        public FuelCellPreRunPostprocessor(IModalDataContainer modData, string outputPath = null)
		{
			_outputPath = outputPath.IsNullOrEmpty() ? Directory.GetCurrentDirectory() : outputPath;
            ModData = modData as ModalDataContainer;
        }

        public SearchResult CalculateFuelCellPowerDemand(FuelCellSystemData fcData, BatterySystemData batData)
        {

            batData = batData.Clone();
            var fcSufficient = CheckFCPower(fcData, batData, out var msg);
            if (!fcSufficient)
            {
                throw new VectoException($"FuelCell power not sufficient {msg}");
            }



            //try with window size = full distance

            var success = CalculateFuelCellPowerDemandForWindowSize(ModData.Distance, fcData, batData, out var entries, out var soC);
            //TODO: return determined init SOC
            if (success)
            {
                return new SearchResult()
                {
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
            try
            {
                SearchAlgorithm.BinarySearch(0.SI<Meter>(), ModData.Distance,
                    evaluateFunction:
                    distance =>
                    {
                        var calcSuccess = CalculateFuelCellPowerDemandForWindowSize(distance, fcData, batData,
                            out var calcEntries, out var SoC);
                        return new SearchResult()
                        {
                            distance = distance,
                            entries = calcEntries,
                            initSOC = SoC,
                            success = calcSuccess
                        };
                    },
                    acceptFunction:
                    (distance, result) =>
                    {
                        var searchResult = (SearchResult)result;
                        if (searchResult.success)
                        {
                            accepted.Add(searchResult);
                        }
                        else
                        {
                            rejected.Add(searchResult);
                        }

                        return searchResult.success;

                    },
                    abortCriterion: (d, o) =>
                    {
                        if (d < minWindowSize)
                        {
                            throw new VectoSearchAbortedException(
                                $"Window size < {minWindowSize}, battery is too small");
                        }

                        if (!accepted.Any() || !rejected.Any())
                        {
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
            }
            finally
            {
                BinarySearchIterations = iterationCount;
                WindowSize = accepted.LastOrDefault()?.distance ?? ModData.Distance;
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
            if ((batMaxSoc - batMinSoc).IsSmaller(maxSocTrace - minSocTrace, 1E-03))
            {
                return false;
            }
			//check difference
            var initSoc_min = batMinSoc + (initSoc - minSocTrace);
            var initSoc_max = batMaxSoc - (maxSocTrace - initSoc);

            shiftedSoc = (initSoc_min + initSoc_max) / 2;

			if (shiftedSoc.IsSmaller(batMinSoc) || shiftedSoc.IsGreater(batMaxSoc)) {
				throw new VectoException($"Calculated SoC {shiftedSoc} is out of range!");
			} 

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

            var dSocSafety = Math.Abs((energy_safety_margin / tmpBatSystem.NominalVoltage / tmpBatSystem.TotalCapacity).Value());
            var minSocSafe = minSoc + dSocSafety;
            var maxSocSafe = maxSoc - dSocSafety;



            if (!success)
            {
                if (TryShiftInitialSoC(minSocSafe, maxSocSafe, batData.InitialSoC, traceMinSoc, traceMaxSoc, out SoC))
                {

                  
                    if (!CalculateFuelCellPowerDemandForSoC(windowSize, fcData, batData, out entries, SoC,
                            out var _, out var _)) {
						return false;

					}
                }
                else
                {
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

            var infBattery = new TracingInfinityBatterySystem(batData);
			infBattery.Initialize(SoC);

			var maxPower = infBattery.MaxChargePower(1.SI<Second>());
			var minPower = infBattery.MaxDischargePower(1.SI<Second>());

            for (int i = 0; i < entries.Length; i++)
            {
                var entry = entries[i];

                //var batPower = entry.P_Bat_T_Final;


                var batPower = entry.P_Bat_T_Final.LimitTo(minPower,maxPower);
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
            BatterySystemData batData, out FCCalcEntry[] fcCalcEntries, double initSoc, out double minSoc_trace, out double maxSoc_trace)
		{
			var minFcPower = fcData.MinPower;
			var maxFcPower = fcData.MaxPower;

			batData.ChargeSustainingBatterySystem = true;
			batData.InitialSoC = initSoc;


			//Calculate raw fuel cell demand ----------------------------------------------------------------------------------
			var batterySystem = new BatterySystem(null, batData);
			batterySystem.Initialize(batData.InitialSoC);
			fcCalcEntries = GetRawFuelCellPowerDemand(windowSize: windowSize, minFcPower: minFcPower, maxFcPower: maxFcPower, bat: batterySystem);


			//Use battery with smaller usable SoC range
			var energy_safety_margin = fcCalcEntries.Average(e => e.P_Bat_T.Value()).SI<Watt>() * 10.SI<Second>();
			var dSocSafety = Math.Abs((energy_safety_margin / batterySystem.NominalVoltage / batterySystem.TotalCapacity).Value());
			var minSocSafe = batterySystem.MinSoC + dSocSafety;
			var maxSocSafe = batterySystem.MaxSoC - dSocSafety;

			if ((maxSocSafe - minSocSafe).IsSmallerOrEqual(0)) {
				throw new VectoException(
					"Battery is too small! Usable SOC range would be negative when safety margins are considered.");
			}
			batData.Batteries.ForEach(tuple =>
			{
				tuple.Item2.MinSOC = minSocSafe;
				tuple.Item2.MaxSOC = maxSocSafe;
			});
			var batMaxSoc = batterySystem.MaxSoC;
			var batMinSoc = batterySystem.MinSoC;
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

			var usableSoCRange = batMaxSoc - batMinSoc;
			for (var i = 0; i < fcCalcEntries.Length; i++)
			{
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
				if (entry.CanChangeFCPower)
				{
					timeFcCanChange += entry.dt;
					countChange++;
				}

				minSoc_trace = VectoMath.Min(minSoc_trace, infinityDummyContainer.SoC);
				maxSoc_trace = VectoMath.Max(maxSoc_trace, infinityDummyContainer.SoC);

				var currentSoc = infinityBatterySystem.StateOfCharge;

				if ((currentSoc - minSoc_trace).IsGreater(usableSoCRange)) {
                    //SoC range violated



                    //Recuperating
					if (entry.P_el_dem.IsGreater(0)) {
						entry.P_el_dem_corr = 0.SI<Watt>();
					} else {
						return false;
					}
				}
			}

		
			if ((maxSoc_trace - minSoc_trace).IsGreater(batMaxSoc - batMinSoc))
			{
				WriteEntriesToFile(windowSize, fcCalcEntries, initSoc, deltaEnergyBatInt, timeFcCanChange, false);
				return false;
			}



			infinityBatterySystem.Initialize(initSoc);
			infinityDummyContainer = new SimpleModDataContainer();

			var remainingTime = timeFcCanChange;

			foreach (var entry in fcCalcEntries)
			{
				var fcPower = entry.P_FC;
				//var fcPower = entry.P_FC_corr;


				if (entry.CanChangeFCPower)
				{
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
						throw new VectoException("Fuel cell limits violated");
					}

				}
				else
				{
					entry.FCPowerFinal = fcPower;
					entry.delta_P_FCS = 0.SI<Watt>();
				}


			}

			batData.ChargeSustainingBatterySystem = false;
			var batSystem = new BatterySystem(null, batData);
			infinityBatterySystem.Initialize(initSoc);
			batSystem.Initialize(initSoc);

			var dummyContainer = new SimpleModDataContainer();


			var realBatTraceMin = initSoc;
			var realBatTraceMax = initSoc;
			foreach (var entry in fcCalcEntries)
			{
				//var batPower = entry.P_Bat_T.LimitTo(batSystem.MaxDischargePower(entry.dt),
				//	batSystem.MaxChargePower(entry.dt));

				//var batPower = entry.P_Bat_T_Final.LimitTo(batSystem.MaxDischargePower(entry.dt),
				//	batSystem.MaxChargePower(entry.dt));

				//Limit to max charge and maxdischarge power of the infinity battery,
                
				var batPower = VectoMath.LimitTo(entry.P_Bat_T_Final,
					infinityBatterySystem.MaxDischargePower(entry.dt),infinityBatterySystem.MaxChargePower(entry.dt));

				

				var batResponse = batSystem.Request(entry.Time, entry.dt, batPower, false);




				if (!(batResponse is RESSResponseSuccess)) {
                    WriteEntriesToFile(windowSize, fcCalcEntries, initSoc, deltaEnergyBatInt, timeFcCanChange, false);
					return false;
				}
				batSystem.CommitSimulationStep(entry.Time, entry.dt, dummyContainer);
				//realBatTraceMax = VectoMath.Max(realBatTraceMax, dummyContainer.SoC);
				//realBatTraceMin = VectoMath.Min(realBatTraceMin, dummyContainer.SoC);

			}



			var success = (realBatTraceMax - realBatTraceMin).IsSmallerOrEqual(batMaxSoc - batMinSoc)
						&& !(maxSoc_trace > batMaxSoc || minSoc_trace < batMinSoc);

			WriteEntriesToFile(windowSize, fcCalcEntries, initSoc, deltaEnergyBatInt, timeFcCanChange, success);
			return success;
		}

		private void WriteEntriesToFile(Meter windowSize, FCCalcEntry[] fcCalcEntries, double initSoc,
            WattSecond deltaEnergyBatInt, Second timeFcCanChange, bool success)
        {
            

            lock (fileLock)
            {
                using (var fs = new StreamWriter(
                            $"fuelcell_data_{ModData.RunName}_{Math.Round(windowSize.Value(), 0)}_soc_{initSoc}_{(success ? "success" : "")}.csv"))
                {
                    fs.WriteLine($"DeltaEnergyBat: {deltaEnergyBatInt} / {deltaEnergyBatInt.ConvertToKiloWattHour()}");
                    fs.WriteLine($"timeFCCanIncrease: {timeFcCanChange}");
                    fs.WriteLine(FCCalcEntry.Header);
                    foreach (var entry in fcCalcEntries)
                    {
                        fs.WriteLine(entry.ToString());
                    }
                }
            }
        }

        /// <summary>
        /// Check if fuelcell can supply enough power over the cycle.
        /// </summary>
        /// <returns></returns>
        public bool CheckFCPower(FuelCellSystemData fcData, BatterySystemData batData, out string msg)
        {
            msg = "";
            var minFcPower = fcData.FuelCells.Single().MinElectricPower;
            var maxFcPower = fcData.FuelCells.Single().MaxElectricPower;

            var tmpBatSystem = new BatterySystem(null, batData);
            tmpBatSystem.Initialize(batData.InitialSoC);


            var entries = GetRawFuelCellPowerDemand(ModData.Distance, minFcPower, maxFcPower, tmpBatSystem);
            if (entries.First().P_FC_raw > maxFcPower)
            {
                msg = $"average el demand {entries.First().P_FC_raw} > max fuel cell power {maxFcPower}";
                return false;

            }

            return true;

            //var tracingInfinityBat = new TracingInfinityBatterySystem(batData.Clone());
            //tracingInfinityBat.Initialize(batData.InitialSoC);
            //var minSocTrace = batData.InitialSoC;
            //var simpleModData = new SimpleModDataContainer();

            //var usableSocRange = tmpBatSystem.MaxSoC - tmpBatSystem.MinSoC;
            //         foreach (var entry in entries) {
            //	entry.P_FC = maxFcPower; // Use max power all the time

            //	var response = tracingInfinityBat.Request(entry.Time, entry.dt, entry.P_Bat_T, false);
            //	if (response is RESSOverloadResponse ovl) {
            //		entry.P_FC = response.MaxChargePower - entry.P_el_dem; //In case we are recuperating
            //		response = tracingInfinityBat.Request(entry.Time, entry.dt, entry.P_Bat_T, false);
            //             }

            //             tracingInfinityBat.CommitSimulationStep(entry.Time, entry.dt, simpleModData);

            //	minSocTrace = VectoMath.Min(minSocTrace, tracingInfinityBat.StateOfCharge);
            //	var delta_soc_discharge = batData.InitialSoC - minSocTrace;
            //	if (delta_soc_discharge > usableSocRange) {
            //		msg = $"Battery gets empty";
            //                 return false;
            //	}
        }

        protected FCCalcEntry[] GetRawFuelCellPowerDemand(Meter windowSize, Watt minFcPower, Watt maxFcPower, BatterySystem bat)
        {
            Debug.Assert(minFcPower < maxFcPower, "min power must be smaller than max power");



            if (windowSize.IsGreater(ModData.Distance))
            {
                throw new VectoException("Window size must not exceed cycle distance!");
            }
            var data = ModData.Data;
            var timeCol = data.Columns[ModalResultField.time.GetShortCaption()];
            var dtCol = data.Columns[ModalResultField.simulationInterval.GetName()];
            var distCol = data.Columns[ModalResultField.dist.GetShortCaption()];
            var esTCol = data.Columns[ModalResultField.P_terminal_ES.GetName()];

            if (windowSize.IsEqual(ModData.Distance))
            {
                var P_FC_raw = ModData.TimeIntegral<WattSecond>(ModalResultField.P_terminal_ES) / ModData.Duration;
                var retVal = ModData.Data.AsEnumerable().Select(r =>
                {
                    var entry = new FCCalcEntry()
                    {
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
            for (; !wIt.CycleEndReached; wIt.MoveNext())
            {
                var entry = new FCCalcEntry()
                {
                    Time = (Second)data.Rows[wIt.Position][timeCol],
                    dt = (Second)data.Rows[wIt.Position][dtCol],
                    Distance = (Meter)data.Rows[wIt.Position][distCol]
                };
                var time = 0.SI<Second>();
                var emEnergy = 0.SI<WattSecond>();
                for (; !wIt.WindowEndReached; wIt.NextEntry())
                {
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
	}
}