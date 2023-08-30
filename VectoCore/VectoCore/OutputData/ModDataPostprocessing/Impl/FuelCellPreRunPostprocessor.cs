using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
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

		public double StateOfCharge => _infinityBat.StateOfCharge;

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


	public class FuelCellPreRunPostprocessor
	{
		public ModalDataContainer ModData;


		public FuelCellPreRunPostprocessor(IModalDataContainer modData)
		{
			ModData = modData as ModalDataContainer;
		}

		public FCCalcEntry[] CalculateFuelCellPowerDemand(FuelCellSystemData fcData, BatterySystemData batData)
		{
			//Infinity battery with virtual traces
			batData.ChargeSustainingBatterySystem = true;


			//try with initial SOC
			var entries = CalculateFuelCellPowerDemand(ModData.Distance, fcData, batData);








			return entries;
		}

		public double ShiftInitialSoc(double batMinSoc, double batMaxSoc, double initSoc, double minSocTrace,
			double maxSocTrace)
		{

			

			var initSoc_min = batMinSoc + (initSoc - minSocTrace);
			var initSoc_max = batMaxSoc - (maxSocTrace - initSoc);

			//TODO: add more checks and throw exceptions ;) 
			return (initSoc_min + initSoc_max) / 2;





		}

		public FCCalcEntry[] CalculateFuelCellPowerDemand(Meter windowSize, FuelCellSystemData fcData,
			BatterySystemData batData)
		{
			var tmpBatSystem = new BatterySystem(null, batData);

			var minSoc = tmpBatSystem.MinSoC;
			var maxSoc = tmpBatSystem.MaxSoC;
			var success = CalculateFuelCellPowerDemand(windowSize, fcData, batData, out var entries, batData.InitialSoC, out var traceMinSoc, out var traceMaxSoc);


			var energy_safety_margin = entries.Average(e => e.P_Bat_T.Value()).SI<Watt>() * 10.SI<Second>();

			var dSocSafety = (tmpBatSystem.TotalCapacity / (energy_safety_margin / tmpBatSystem.NominalVoltage)).Value();
			var minSocSafe = minSoc + dSocSafety;
			var maxSocSafe = maxSoc - dSocSafety;

			


			if (!success) {
				if (maxSocSafe - minSocSafe >= traceMaxSoc - traceMinSoc) {
					var shiftedSoc = ShiftInitialSoc(minSocSafe, maxSocSafe, batData.InitialSoC, traceMinSoc, traceMaxSoc);


				}


				throw new NotImplementedException();
			}


			


			return entries;
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
		public bool CalculateFuelCellPowerDemand(Meter windowSize, FuelCellSystemData fcData,
			BatterySystemData batData, out FCCalcEntry[] fcCalcEntries, double initSoc, out double minSoc_trace,out double maxSoc_trace)
		{
			var minFcPower = fcData.FuelCells.First().MinElectricPower;
			var maxFcPower = fcData.FuelCells.First().MaxElectricPower;

			
			
			batData.Batteries.ForEach(x => x.Item2.ChargeSustainingBattery = false);

			var infinityBatterySystem = new TracingInfinityBatterySystem(batData);
			infinityBatterySystem.Initialize(batData.InitialSoC);

			var processed = GetRawFuelCellPowerDemand(windowSize, minFcPower, maxFcPower, infinityBatterySystem);
			fcCalcEntries = processed;



			infinityBatterySystem.Initialize(batData.InitialSoC);
			var DeltaEnergyBat_int = 0.SI<WattSecond>();
			var timeFCCanIncrease = 0.SI<Second>();
			var infinityDummyContainer = new SimpleModDataContainer();
			var countIncrease = 0;

			minSoc_trace = batData.InitialSoC;
			maxSoc_trace = batData.InitialSoC;


			var batSystemMinSoc = infinityBatterySystem.MinSoC;
			var batSystemMaxSoc = infinityBatterySystem.MaxSoC;

			for (int i = 0; i < processed.Length; i++) {
				var entry = processed[i];
				// limit P_Bat_T to battery min/max
				var batPower = entry.P_Bat_T.LimitTo(infinityBatterySystem.MaxDischargePower(entry.dt), infinityBatterySystem.MaxChargePower(entry.dt));
				var batResonse = infinityBatterySystem.Request(entry.Time, entry.dt, batPower, false);
				infinityBatterySystem.CommitSimulationStep(entry.Time, entry.dt, infinityDummyContainer);
				DeltaEnergyBat_int += infinityDummyContainer.P_REES_int * entry.dt;
				entry.P_REESS_int = infinityDummyContainer.P_REES_int;
				entry.SoC = infinityDummyContainer.SoC;
				// check SoC min/max over cycle
				// check P_bat min/max (
				entry.CanIncreaseFCPower = entry.delta_Power_corr.IsGreater(0) &&
											entry.P_FC.IsGreater(0) &&
											entry.P_FC_corr.IsSmaller(maxFcPower);
				if (entry.CanIncreaseFCPower) {
					timeFCCanIncrease += entry.dt;
					countIncrease++;
				}

				minSoc_trace = VectoMath.Min(minSoc_trace, entry.SoC);
				maxSoc_trace = VectoMath.Max(maxSoc_trace, entry.SoC);

				if (maxSoc_trace - minSoc_trace > infinityBatterySystem.MaxSoC - infinityBatterySystem.MaxSoC) {
					return false;
				}
			}





			var fcIncrease = - DeltaEnergyBat_int / timeFCCanIncrease; //delta_P_fcs
			for (int i = 0; i < processed.Length; i++) {
				var entry = processed[i];
				entry.FCPowerFinal = VectoMath.Min(
					(entry.CanIncreaseFCPower
					? entry.P_FC_corr + fcIncrease / (1 - entry.P_Bat_loss / entry.P_Bat_T)
					: entry.P_FC_corr),
					
					maxFcPower);
			}


			var batSystem = new BatterySystem(null, batData);
			var dummyContainer = new SimpleModDataContainer();
			for (int i = 0; i < processed.Length; i++) {
				var entry = processed[i];

				var batPower = entry.P_Bat_T.LimitTo(batSystem.MaxDischargePower(entry.dt),
					batSystem.MaxChargePower(entry.dt));

				var batResponse = batSystem.Request(entry.Time, entry.dt, batPower, false);
				
				batSystem.CommitSimulationStep(entry.Time, entry.dt, dummyContainer);
				
			}


			using (var fs = new StreamWriter("fuelcell_data.txt")) {
				fs.WriteLine($"DeltaEnergyBat: {DeltaEnergyBat_int} / {DeltaEnergyBat_int.ConvertToKiloWattHour()}");
				fs.WriteLine($"timeFCCanIncrease: {timeFCCanIncrease}");
				fs.WriteLine("Time, dt, Distance, P_el_dem, P_FC_raw, P_FC, P_Bat_T, P_Bat_loss, P_FC_corr, " +
							"delta_Power_corr, SoC, CanIncreaseFCPower, FCPowerFinal");
				foreach (var entry in processed) {
					fs.WriteLine(entry.ToString());
				}
			}

			
			return true;
		}

		protected FCCalcEntry[] GetRawFuelCellPowerDemand(Meter windowSize, Watt minFcPower, Watt maxFcPower, TracingInfinityBatterySystem bat)
		{
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
			public Watt P_el_dem { get; set; }
			public Watt P_FC_raw { get; set; }
			public Watt P_FC { get; set; }

			public Watt P_Bat_T => P_FC + P_el_dem;
			public Watt P_Bat_loss { get; set; }

			public Watt P_FC_corr => P_FC.IsEqual(0) ? P_FC : P_FC + P_Bat_loss;

			public Watt delta_Power_corr => P_FC_corr + P_el_dem;
			public double SoC { get; set; }

			public bool CanIncreaseFCPower { get; set; }

			public Watt FCPowerFinal { get; set; }
			public Watt P_REESS_int { get; set; }

			#region Overrides of Object

			public override string ToString()
			{
				return $"{Time}, {dt}, {Distance}, {P_el_dem}, {P_FC_raw}, {P_FC}, {P_Bat_T}, {P_Bat_loss}, {P_FC_corr}, " +
						$"{delta_Power_corr}, {P_REESS_int}, {SoC}, {CanIncreaseFCPower}, {FCPowerFinal}";

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