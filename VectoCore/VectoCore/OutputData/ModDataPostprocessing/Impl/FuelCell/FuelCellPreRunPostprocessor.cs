using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Cryptography.X509Certificates;
using NLog.LayoutRenderers;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.ElectricComponents;
using TUGraz.VectoCore.Models.SimulationComponent.Data.ElectricComponents.Battery;
using TUGraz.VectoCore.Models.SimulationComponent.Data.ShiftStrategy;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.OutputData.ModDataPostprocessing.Impl.FuelCell
{
	public interface IFuelCellPreRunInfo
    {
        Meter WindowSize { get; }
        int BinarySearchIterations { get; }
        double StartSoC { get; }

		FuelCellPreRunPostprocessor.SearchResult[] AcceptedSearchResults { get; }
        FuelCellPreRunPostprocessor.SearchResult[] RejectedSearchResults { get; }
    }

    public partial class FuelCellPreRunPostprocessor : IFuelCellPreRunInfo
    {
		#region
		
		public IOutputDataWriter Writer { get; set; }
		#endregion
		//public ModalDataContainer ModData;

		#region PreRunInfo

		private string RunName;
		private PreRunEntry[] _preRunResults;
		private Meter _totalDistance;
        public Meter TotalDistance
		{
			get => _totalDistance ?? (_totalDistance =
				_preRunResults.Last().s - (_preRunResults.First().s - _preRunResults.First().ds));
		}
		private Second _totalDuration;
        public Second TotalDuration { get => _totalDuration ??
											(_totalDuration = _preRunResults.Last().t - _preRunResults.First().t + _preRunResults.First().dt / 2 + _preRunResults.Last().dt / 2); }

        #endregion


        #region BinarySearch
        public SearchResult[] AcceptedSearchResults { get; private set; }
        public SearchResult[] RejectedSearchResults { get; private set; }
		public int BinarySearchIterations { get; private set; }
        #endregion



        public double StartSoC { get; private set; }

        public Meter WindowSize { get; private set; }
        
        private static object fileLock = new object();



		public class SearchResult
        {
			public SearchResult(bool success, double initSoc, Meter distance, FCCalcEntry[] entries)
			{
				_success = success;
				_initSoc = initSoc;
				_distance = distance;
				_entries = entries;
			}

			private readonly FCCalcEntry[] _entries;
			public FCCalcEntry[] Entries => _entries;

			private readonly double _initSoc;
			public double InitSoc => _initSoc;

			private readonly bool _success;
			public bool Success => _success;

			private readonly Meter _distance;
			public Meter Distance => _distance;
		}


        public FuelCellPreRunPostprocessor(IModalDataContainer modData)
		{
			//Todo: Remove cast
			FillPreRunResults(modData);
			RunName = modData?.RunName ?? "";
			//Debug.Assert(modData.Distance == TotalDistance);
			//Debug.Assert(modData.Duration == TotalDuration);
		}

		private void FillPreRunResults(IModalDataContainer modData)
		{
			var data = modData?.Data;
			var timeCol = data?.Columns[ModalResultField.time.GetShortCaption()];
			var dtCol = data?.Columns[ModalResultField.simulationInterval.GetName()];
			var distCol = data?.Columns[ModalResultField.dist.GetShortCaption()];
			var dsCol = data?.Columns[ModalResultField.simulationDistance.GetName()];
			var esTCol = data?.Columns[ModalResultField.P_terminal_ES.GetName()];

			_preRunResults = data?.AsEnumerable().Select(m => new PreRunEntry() {
				P_es_T = (Watt)m[esTCol],
				t = (Second)m[timeCol],
				dt = (Second)m[dtCol],
				s = (Meter)m[distCol],
                ds = (Meter)m[dsCol],
			}).ToArray();
		}


		/// <summary>
		/// Entry
		/// </summary>
		/// <param name="fcData"></param>
		/// <param name="batData"></param>
		/// <returns></returns>
		public SearchResult CalculateFuelCellPowerDemand(FuelCellSystemData fcData, BatterySystemData batData)
		{

			batData = batData.Clone();
		
            var tmpBatSystem = new BatterySystem(null, batData);
			tmpBatSystem.Initialize(batData.InitialSoC);

			var rawFcCalcEntries = GetRawFuelCellPowerDemand(TotalDistance, fcData.MinElectricPower, fcData.MaxElectricPower, tmpBatSystem,
				_preRunResults);

			ApplyBatterySafetyMargin(batData, rawFcCalcEntries);

			if (TryWithFullDistance(fcData, ref batData, out var result)) {
				StartSoC = result.InitSoc;
				WindowSize = TotalDistance;
				BinarySearchIterations = 0;
				return result;
			}
			return BinarySearchFuelCellPowerDemand(fcData, batData);
		}

		private SearchResult BinarySearchFuelCellPowerDemand(FuelCellSystemData fcData, BatterySystemData batData)
		{
			double searchThreshold = 0.05;
			var minWindowSize = 5.SI<Meter>();
			//Start binary search
			var accepted = new List<SearchResult>();
			// we can add the full distance already beforehand because the search starts only if the full distance is not 
			// feasible.
			var rejected = new List<SearchResult>() {
				new SearchResult(false, batData.InitialSoC, TotalDistance, Array.Empty<FCCalcEntry>())
			};
			var iterationCount = 0;
			try {
				SearchAlgorithm.BinarySearch(0.SI<Meter>(), TotalDistance,
					evaluateFunction:
					distance => {
						CalculateFuelCellPowerDemandForWindowSize(distance, fcData, batData, out var result);
						return result;
					},
					acceptFunction:
					(distance, result) => {
						var searchResult = (SearchResult)result;
						if (searchResult.Success) {
							accepted.Add(searchResult);
						} else {
							rejected.Add(searchResult);
						}

						return searchResult.Success;
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
						var deviation = VectoMath.Abs((lastRejected.Distance - lastAccepted.Distance)) /
							(lastRejected.Distance + lastAccepted.Distance) * 2;
						if (deviation < searchThreshold) {

						}

						return deviation < searchThreshold;
					},
					ref iterationCount,
					searcher: this
				);
			} finally {
				BinarySearchIterations = iterationCount;
				WindowSize = accepted.LastOrDefault()?.Distance ?? TotalDistance;
				RejectedSearchResults = rejected.ToArray();
				AcceptedSearchResults = accepted.ToArray();
				StartSoC = accepted.LastOrDefault()?.InitSoc ?? 0.0;
			}

			return accepted.Last();
		}

		private bool TryWithFullDistance(FuelCellSystemData fcData, ref BatterySystemData batData,
			out SearchResult result)
		{
			result = null;
			var fcSufficient = CheckFCPower(fcData, batData, out var msg);
			if (!fcSufficient) {
				throw new VectoException($"FuelCell power not sufficient {msg}");
			}

			//try with window size = full distance
			return CalculateFuelCellPowerDemandForWindowSize(TotalDistance, fcData, batData, out result);
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
            BatterySystemData batData, out SearchResult result)
        {
            ///For a given window size as first step we calculate the fuel cell power, try to simulate it with a tracing infinity battery and check if the SoC limits are violated
			var minFcPower = fcData.MinElectricPower;
			var maxFcPower = fcData.MaxElectricPower;

			batData = batData.Clone();
			//Calculate Raw Fuel CellDemand
			var batSystem = new BatterySystem(null, batData);
			var initSoc = batData.InitialSoC;
			batSystem.Initialize(initSoc);
			var fcCalcEntries = GetRawFuelCellPowerDemand(windowSize: windowSize, minFcPower: minFcPower,
				maxFcPower: maxFcPower, bat: batSystem, preRunResults: _preRunResults);

			var processedFcCalcEntries = new List<FCCalcEntry>(_preRunResults.Length); //Holds the processed fcCalcEntries to avoid multiple enumerations
			//var socLimits = ApplyBatterySafetyMargin(batData, fcCalcEntries, batSystem);
			var usableRange = batSystem.MaxSoC - batSystem.MinSoC;


			//Determine battery energy losses ---------------------------------------------------------------------------------
			//and check for SoC violations
            var infinityBatterySystem = new TracingInfinityBatterySystem(batData.Clone());
			infinityBatterySystem.Initialize(batData.InitialSoC);

			var deltaEnergyBatInt = 0.SI<WattSecond>();
			var timeFcCanChange = 0.SI<Second>();

			var infinityDummyContainer = new SimpleModDataContainer();




            var correctedPreRunResults = new List<PreRunEntry>();
			var preRunResultsAreCorrected = false;


			foreach (var entry in fcCalcEntries) {
				var correctedPreRunEntry = new PreRunEntry(entry.preRunEntry);
				correctedPreRunResults.Add(correctedPreRunEntry);

				var SoC = RequestFromBattery(entry, infinityBatterySystem, infinityDummyContainer, maxFcPower, minFcPower, ref deltaEnergyBatInt, ref timeFcCanChange);

				processedFcCalcEntries.Add(entry);
				if ((SoC - infinityBatterySystem.MinSoC).IsGreater(usableRange))
				{
					//SoC range violated

					//Recuperating
					if (entry.P_el_dem.IsGreater(0))
					{
						//Stop recuperating
						correctedPreRunEntry.P_es_T = 0.SI<Watt>();
						preRunResultsAreCorrected = true;
					}
					else
					{
						//we are not recuperating but the used SoC range is to large,
						//Window is too large.
						result = new SearchResult(
							success: false,
							initSoc: SoC,
							distance: windowSize,
							entries: processedFcCalcEntries.ToArray());
						WriteEntriesToFile(windowSize, processedFcCalcEntries.ToArray(), initSoc, deltaEnergyBatInt,
							timeFcCanChange, false);
						//Return false to decrease window size
						return false;
					}
				}
			}

			if (preRunResultsAreCorrected) {
				//Throw away old results
                deltaEnergyBatInt = 0.SI<WattSecond>();
				timeFcCanChange = 0.SI<Second>();
				processedFcCalcEntries.Clear(); 
				infinityBatterySystem = new TracingInfinityBatterySystem(batData);
				infinityBatterySystem.Initialize(StartSoC);

				//Calculate updated fc demand
				fcCalcEntries = GetRawFuelCellPowerDemand(windowSize, minFcPower, maxFcPower, batSystem,
					correctedPreRunResults);


				infinityDummyContainer = new SimpleModDataContainer();
				foreach (var entry in fcCalcEntries) {


					var SoC = RequestFromBattery(entry, infinityBatterySystem, infinityDummyContainer, maxFcPower,
						minFcPower, ref deltaEnergyBatInt, ref timeFcCanChange);
					processedFcCalcEntries.Add(entry);
					if ((SoC - infinityBatterySystem.MinSoC).IsGreater(usableRange)) {
						throw new VectoException("Violation of usable SoC should be covered");
						if (entry.P_el_dem.IsGreater(0)) {
							throw new VectoException(
								"Parts where we are recuperating with full battery should not be included");
						} else {
							
						}
					}
				}
			}



			var remainingTime = timeFcCanChange;
            //compensate delta P_bat_int
            foreach (var entry in processedFcCalcEntries)
			{
				var fcPower = entry.P_FC;
				//var fcPower = entry.P_FC_corr;

				if (entry.CanChangeFCPower)
				{
					if (deltaEnergyBatInt.IsGreater(0)) {
						WriteEntriesToFile(windowSize, processedFcCalcEntries.ToArray(), initSoc, deltaEnergyBatInt, timeFcCanChange, false);
						throw new NotImplementedException("Positive delta bat_int is not implemented");
					}

					Watt batChangeTarget = -deltaEnergyBatInt / remainingTime;

					var fcChangeTarget = batChangeTarget;

					var fcPowerTarget = fcPower + fcChangeTarget;
					var fcPowerActual = fcPowerTarget
										- VectoMath.Max(fcPowerTarget - maxFcPower, 0.SI<Watt>())
										- VectoMath.Min(fcPowerTarget - minFcPower, 0.SI<Watt>());



					var fcChangeActual = fcChangeTarget - (fcPowerTarget - fcPowerActual);
					var batChangeActual = fcChangeActual;

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



			//Check infintiy battery again for violated SoC limits
			infinityBatterySystem = new TracingInfinityBatterySystem(batData);
			infinityBatterySystem.Initialize(initSoc);
			infinityDummyContainer = new SimpleModDataContainer();
			foreach (var entry in processedFcCalcEntries) {
				//Should be limited?
				var response = infinityBatterySystem.Request(entry.t, entry.dt, entry.P_Bat_T_Final, false);
				if (!(response is RESSResponseSuccess))
				{
					throw new VectoException("Unexpected Response");
				}
                infinityBatterySystem.CommitSimulationStep(entry.t, entry.dt, infinityDummyContainer);
			}


			var minSocTrace = infinityBatterySystem.MinSocTrace;
			var maxSocTrace = infinityBatterySystem.MaxSocTrace;


			if (TryShiftInitialSoC(
					batMinSoc: batSystem.MinSoC,
					batMaxSoc: batSystem.MaxSoC,
					initSoc, minSocTrace, maxSocTrace, out var shiftedSoc)) {

				initSoc = shiftedSoc;

				if(CalculateWithRealBattery(batData, initSoc, processedFcCalcEntries.ToArray()))
				{
					WriteEntriesToFile(windowSize, processedFcCalcEntries.ToArray(), initSoc, deltaEnergyBatInt, timeFcCanChange, true);
					result = new SearchResult(
						distance: windowSize,
						entries: processedFcCalcEntries.ToArray(),
						initSoc: initSoc,
						success: true);
					return true;
				}
			}

			WriteEntriesToFile(windowSize, processedFcCalcEntries.ToArray(), initSoc, deltaEnergyBatInt, timeFcCanChange, false);
			result = new SearchResult(
				distance: windowSize,
				entries: processedFcCalcEntries.ToArray(),
				initSoc: initSoc,
				success: false);

			return false;
		}

		private static double RequestFromBattery(FCCalcEntry entry, TracingInfinityBatterySystem infinityBatterySystem,
			SimpleModDataContainer infinityDummyContainer, Watt maxFcPower, Watt minFcPower, ref WattSecond deltaEnergyBatInt,
			ref Second timeFcCanChange)
		{
			//Since we are using the infinity battery, the limit that is applied is only influenced by the properties of the battery but not by the SoC
            var batPower = entry.P_Bat_T.LimitTo
			(infinityBatterySystem.MaxDischargePower(entry.dt),
				infinityBatterySystem.MaxChargePower(entry.dt));

			//Request determined battery power from infinity battery system
			var batResponse = infinityBatterySystem.Request(entry.t, entry.dt, batPower, false);
			if (!(batResponse is RESSResponseSuccess)) {
				throw new VectoException("Unexpected Response");
			}

			infinityBatterySystem.CommitSimulationStep(entry.t, entry.dt, infinityDummyContainer);

			deltaEnergyBatInt += infinityDummyContainer.P_REES_int * entry.dt;

			entry.P_REESS_int = infinityDummyContainer.P_REES_int;
			var SoC = infinityBatterySystem.StateOfCharge; //Current virtual soc
			entry.SoC = SoC;


			// check SoC min/max over cycle
			// check P_bat min/max (
			entry.CanChangeFCPower = !entry.P_FC.IsEqual(0) //Fuel cell is switched off
									&& (entry.P_FC_corr.IsSmaller(maxFcPower) ||
										entry.P_FC_corr.IsGreater(minFcPower)); //power is within limits
			if (entry.CanChangeFCPower) {
				timeFcCanChange += entry.dt;
			}

			return SoC;
		}


		private BatterySystemData ApplyBatterySafetyMargin(BatterySystemData batData, IEnumerable<FCCalcEntry> fcCalcEntries)
		{
			var tmpBatSystem = new BatterySystem(null, batData);
			var first = fcCalcEntries.First();
			Debug.Assert(first.WindowSize == TotalDistance); 
			//When window size equals TotalDistance, P_FC_raw == average P_el_dem
			var energy_safety_margin = first.P_FC_raw * 10.SI<Second>(); 
			var dSocSafety =
				Math.Abs((energy_safety_margin / tmpBatSystem.NominalVoltage / tmpBatSystem.TotalCapacity).Value());
			var minSocSafe = tmpBatSystem.MinSoC + dSocSafety;
			var maxSocSafe = tmpBatSystem.MaxSoC - dSocSafety;

			if ((maxSocSafe - minSocSafe).IsSmallerOrEqual(0)) {
				throw new VectoException(
					"Battery is too small! Usable SOC range would be negative when safety margins are considered.");
			}

			batData.Batteries.ForEach(tuple => {
				tuple.Item2.MinSOC = minSocSafe;
				tuple.Item2.MaxSOC = maxSocSafe;
			});
			tmpBatSystem = new BatterySystem(null, batData);
			tmpBatSystem.Initialize(batData.InitialSoC);
			
			Debug.Assert(tmpBatSystem.MinSoC.IsEqual(minSocSafe) && tmpBatSystem.MaxSoC.IsEqual(maxSocSafe), "Invalid Soc limits");
			return batData;
		}


		public bool CalculateWithRealBattery(BatterySystemData batData, double SoC, FCCalcEntry[] entries)
        {
            batData = batData.Clone();
            batData.ChargeSustainingBatterySystem = false;
            var batSystem = new BatterySystem(null, batData);
            batSystem.Initialize(SoC);
            var dummyContainer = new SimpleModDataContainer();

            var infBattery = new TracingInfinityBatterySystem(batData);
			infBattery.Initialize((batSystem.MaxSoC + batSystem.MinSoC) / 2);



            for (int i = 0; i < entries.Length; i++)
            {
                var entry = entries[i];
                //var batPower = entry.P_Bat_T_Final;
				var maxPower = infBattery.MaxChargePower(entry.dt);
				var minPower = infBattery.MaxDischargePower(entry.dt);
                var batPower = entry.P_Bat_T_Final.LimitTo(minPower,maxPower);
                //Limit batPower
                var batResponse = batSystem.Request(entry.t, entry.dt, batPower, false);
                if (!(batResponse is RESSResponseSuccess))
                {
					return false;
                }

				entry.Real_SoC = batResponse.StateOfCharge;
                batSystem.CommitSimulationStep(entry.t, entry.dt, dummyContainer);
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
		[Obsolete]
		public bool CalculateFuelCellPowerDemandForSoC(Meter windowSize, FuelCellSystemData fcData,
            BatterySystemData batData, out FCCalcEntry[] fcCalcEntries, double initSoc, out double minSoc_trace, out double maxSoc_trace)
		{
			throw new NotImplementedException();
            //Use Enumerators instead of converting everything to an array
			var minFcPower = fcData.MinElectricPower;
			var maxFcPower = fcData.MaxElectricPower;

            batData.ChargeSustainingBatterySystem = true;
			batData.InitialSoC = initSoc;

			//Calculate raw fuel cell demand ----------------------------------------------------------------------------------
			var batterySystem = new BatterySystem(null, batData);
			batterySystem.Initialize(batData.InitialSoC);
			fcCalcEntries = GetRawFuelCellPowerDemand(windowSize: windowSize, minFcPower: minFcPower, maxFcPower: maxFcPower, bat: batterySystem, preRunResults: _preRunResults).ToArray();


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

				var batResonse = infinityBatterySystem.Request(entry.t, entry.dt, batPower, false);
				infinityBatterySystem.CommitSimulationStep(entry.t, entry.dt, infinityDummyContainer);
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
                        WriteEntriesToFile(windowSize, fcCalcEntries, initSoc, deltaEnergyBatInt, timeFcCanChange, false);
					} else {
						return false;
					}
				}
			}
            //Calculate P_FC_raw again

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

				

				var batResponse = batSystem.Request(entry.t, entry.dt, batPower, false);




				if (!(batResponse is RESSResponseSuccess)) {
                    WriteEntriesToFile(windowSize, fcCalcEntries, initSoc, deltaEnergyBatInt, timeFcCanChange, false);
					return false;
				}
				batSystem.CommitSimulationStep(entry.t, entry.dt, dummyContainer);
				//realBatTraceMax = VectoMath.Max(realBatTraceMax, dummyContainer.SoC);
				//realBatTraceMin = VectoMath.Min(realBatTraceMin, dummyContainer.SoC);

			}

			var success = (realBatTraceMax - realBatTraceMin).IsSmallerOrEqual(batMaxSoc - batMinSoc)
						&& !(maxSoc_trace > batMaxSoc || minSoc_trace < batMinSoc);

			WriteEntriesToFile(windowSize, fcCalcEntries, initSoc, deltaEnergyBatInt, timeFcCanChange, success);
			return success;
		}


		[Conditional("TRACE_FC")]
		private void WriteEntriesToFile(Meter windowSize, FCCalcEntry[] fcCalcEntries, double initSoc,
            WattSecond deltaEnergyBatInt, Second timeFcCanChange, bool success)
        {

			lock (fileLock)
            {
                using (var fs = new StreamWriter(Path.Combine(Path.GetDirectoryName(Writer?.JobFile) ?? "", $"fuelcell_data_{RunName}_{Math.Round(windowSize.Value(), 0)}_soc_{initSoc}_{(success ? "success" : "")}.csv")))
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
			var minFcPower = 0.SI<Watt>(); //values below min power will be handled with time slicing
			var maxFcPower = fcData.MaxElectricPower;

            var tmpBatSystem = new BatterySystem(null, batData);
            tmpBatSystem.Initialize(batData.InitialSoC);

			var avgPowerDemand = GetRawFuelCellPowerDemand(TotalDistance, minFcPower, maxFcPower, tmpBatSystem, _preRunResults).First().P_FC_raw;

			if (avgPowerDemand > maxFcPower)
            {
                msg = $"average el demand {avgPowerDemand} > max fuel cell power {maxFcPower}";
                return false;
			}

            return true;
		}


        


        public IEnumerable<FCCalcEntry> GetRawFuelCellPowerDemand(Meter windowSize, Watt minFcPower, Watt maxFcPower, BatterySystem bat, IEnumerable<PreRunEntry> preRunResults)
		{
			foreach (var fcCalcEntry in FcCalcEntries(windowSize, minFcPower, maxFcPower, bat, preRunResults, TotalDistance, TotalDuration)) {
				yield return fcCalcEntry;
			}
		}

		public static IEnumerable<FCCalcEntry> FcCalcEntries(Meter windowSize, Watt minFcPower, Watt maxFcPower, BatterySystem bat,
			IEnumerable<PreRunEntry> preRunResults, Meter totalDistance, Second totalDuration)
		{
			Debug.Assert(minFcPower < maxFcPower, "min power must be smaller than max power");
			if (windowSize.IsGreater(totalDistance)) {
				throw new VectoException("Window size must not exceed cycle distance!");
			}

			var preRunEntries = preRunResults as PreRunEntry[] ?? preRunResults.ToArray();
			if (windowSize.IsEqual(totalDistance)) {
				var P_FC_raw = preRunEntries.TimeIntegral<PreRunEntry, Second, Watt, WattSecond>((e) => e.dt, (e) => e.P_es_T) /
								totalDuration;
				foreach (var r in preRunEntries) {
					var entry = new FCCalcEntry() {
						WindowSize = windowSize,
						preRunEntry = r,
						P_FC_raw = -P_FC_raw,
						P_el_dem = r.P_es_T,
					};
					entry.P_FC = entry.P_FC_raw > minFcPower
						? VectoMath.Min(entry.P_FC_raw, maxFcPower)
						: entry.P_FC_raw.IsSmaller(0)
							? 0.SI<Watt>()
							: minFcPower;
					var batResonse = bat.Request(entry.t, entry.dt, entry.P_Bat_T, true);
					entry.P_Bat_loss = batResonse.LossPower;

					yield return entry;
				}

				yield break;
			}

			var wIt = new GeneralizedModDataWindowIterator<PreRunEntry, Meter>(preRunEntries, windowSize,
				entry => entry.s, entry => entry.ds);


			for (; !wIt.EndReached; wIt.MoveNext()) {
				var entry = new FCCalcEntry() {
					preRunEntry = preRunEntries[wIt.Position],
					WindowSize = windowSize,
				};


				var time = 0.SI<Second>();
				var emEnergy = 0.SI<WattSecond>();
				for (; !wIt.WindowEndReached; wIt.NextEntry()) {
					var dt = preRunEntries[wIt.CurrentIndex].dt;
					time += dt;

					emEnergy += dt * preRunEntries[wIt.CurrentIndex].P_es_T;
				}

				entry.P_FC_raw = -emEnergy / time;
				entry.P_FC = entry.P_FC_raw > minFcPower
					? VectoMath.Min(entry.P_FC_raw, maxFcPower)
					: entry.P_FC_raw.IsSmaller(0)
						? 0.SI<Watt>()
						: minFcPower;
				entry.P_el_dem = preRunEntries[wIt.Position].P_es_T;
				var batResonse = bat.Request(entry.t, entry.dt, entry.P_Bat_T, true);
				entry.P_Bat_loss = batResonse.LossPower;
				yield return entry;
			}

			yield break;
		}
	}
}