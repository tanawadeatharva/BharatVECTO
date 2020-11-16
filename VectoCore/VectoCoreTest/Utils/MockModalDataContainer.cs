/*
* This file is part of VECTO.
*
* Copyright © 2012-2019 European Union
*
* Developed by Graz University of Technology,
*              Institute of Internal Combustion Engines and Thermodynamics,
*              Institute of Technical Informatics
*
* VECTO is licensed under the EUPL, Version 1.1 or - as soon they will be approved
* by the European Commission - subsequent versions of the EUPL (the "Licence");
* You may not use VECTO except in compliance with the Licence.
* You may obtain a copy of the Licence at:
*
* https://joinup.ec.europa.eu/community/eupl/og_page/eupl
*
* Unless required by applicable law or agreed to in writing, VECTO
* distributed under the Licence is distributed on an "AS IS" basis,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the Licence for the specific language governing permissions and
* limitations under the Licence.
*
* Authors:
*   Stefan Hausberger, hausberger@ivt.tugraz.at, IVT, Graz University of Technology
*   Christian Kreiner, christian.kreiner@tugraz.at, ITI, Graz University of Technology
*   Michael Krisper, michael.krisper@tugraz.at, ITI, Graz University of Technology
*   Raphael Luz, luz@ivt.tugraz.at, IVT, Graz University of Technology
*   Markus Quaritsch, markus.quaritsch@tugraz.at, IVT, Graz University of Technology
*   Martin Rexeis, rexeis@ivt.tugraz.at, IVT, Graz University of Technology
*/

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.VectoCore.Tests.Utils
{
	/// <summary>
	/// Fake Data Writer Class for Tests.
	/// </summary>
	internal class MockModalDataContainer : IModalDataContainer
	{
		protected Dictionary<IFuelProperties, Dictionary<ModalResultField, DataColumn>> FuelColumns =
			new Dictionary<IFuelProperties, Dictionary<ModalResultField, DataColumn>>();

		private Second _duration;
		private Meter _distance;


		public MockModalDataContainer()
		{
			Data = new ModalResults();
			foreach (var value in EnumHelper.GetValues<ModalResultField>()) {
				if (ModalDataContainer.FuelConsumptionSignals.Contains(value)) {
					continue;
				}

				var col = new DataColumn(value.GetName(), value.GetAttribute().DataType) { Caption = value.GetCaption() };
				col.ExtendedProperties[ModalResults.ExtendedPropertyNames.Decimals] = value.GetAttribute().Decimals;
				col.ExtendedProperties[ModalResults.ExtendedPropertyNames.OutputFactor] = value.GetAttribute().OutputFactor;
				col.ExtendedProperties[ModalResults.ExtendedPropertyNames.ShowUnit] = value.GetAttribute().ShowUnit;
				Data.Columns.Add(col);
			}

			CurrentRow = Data.NewRow();
			Auxiliaries = new Dictionary<string, DataColumn>();

			AddFuels(new IFuelProperties[] { VectoCore.Models.Declaration.FuelData.Diesel }.ToList());
		}

		protected void AddFuels(List<IFuelProperties> fuels)
		{
			foreach (var entry in fuels) {
				if (FuelColumns.ContainsKey(entry)) {
					throw new VectoException("Fuel {0} already added!", entry.FuelType.GetLabel());
				}

				FuelColumns[entry] = new Dictionary<ModalResultField, DataColumn>();
				foreach (var fcCol in ModalDataContainer.FuelConsumptionSignals) {
					var col = Data.Columns.Add(
						fuels.Count == 1 ? fcCol.GetName() : string.Format("{0}_{1}", fcCol.GetName(), entry.FuelType.GetLabel()),
						typeof(SI));
					col.ExtendedProperties[ModalResults.ExtendedPropertyNames.Decimals] =
						fcCol.GetAttribute().Decimals;
					col.ExtendedProperties[ModalResults.ExtendedPropertyNames.OutputFactor] =
						fcCol.GetAttribute().OutputFactor;
					col.ExtendedProperties[ModalResults.ExtendedPropertyNames.ShowUnit] =
						fcCol.GetAttribute().ShowUnit;
					FuelColumns[entry][fcCol] = col;
				}
			}
		}

		public ModalResults Data { get; set; }
		public DataRow CurrentRow { get; set; }

		public string ModFileName
		{
			get { return ""; }
		}

		public object this[ModalResultField key, IFuelProperties fuel]
		{
			get {
				if (!FuelColumns.ContainsKey(fuel) || !FuelColumns[fuel].ContainsKey(key)) {
					throw new VectoException("unknown fuel {0} for key {1}", fuel.GetLabel(), key.GetName());
				}

				return CurrentRow[FuelColumns[fuel][key]];
			}
			set {
				if (!FuelColumns.ContainsKey(fuel) || !FuelColumns[fuel].ContainsKey(key)) {
					throw new VectoException("unknown fuel {0} for key {1}", fuel.GetLabel(), key.GetName());
				}

				CurrentRow[FuelColumns[fuel][key]] = value;
			}
		}

		public object this[ModalResultField key, PowertrainPosition pos]
		{
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}

		public object this[string auxId]
		{
			get { return CurrentRow[Auxiliaries[auxId]]; }
			set { CurrentRow[Auxiliaries[auxId]] = value; }
		}

		public bool HasTorqueConverter { get; set; }

		public void CommitSimulationStep()
		{
			Data.Rows.Add(CurrentRow);
			CurrentRow = Data.NewRow();
		}

		IList<IFuelProperties> IModalDataContainer.FuelData { get { return FuelColumns.Keys.ToList(); } }

		public FuelData.Entry FuelData
		{
			get { return VectoCore.Models.Declaration.FuelData.Diesel; }
		}

		public VectoRun.Status RunStatus
		{
			get { return VectoRun.Status.Success; }
		}

		public string Error
		{
			get { return null; }
		}

		public string StackTrace
		{
			get { return null; }
		}

		public void Finish(VectoRun.Status runStatus, Exception exception = null) {}
		
		public bool WriteModalResults { get; set; }

		public IEnumerable<T> GetValues<T>(ModalResultField key)
		{
			return Data.Rows.Cast<DataRow>().Select(x => x.Field<T>((int)key));
		}

		public IEnumerable<T> GetValues<T>(DataColumn col)
		{
			return Data.Rows.Cast<DataRow>().Select(x => x.Field<T>(col));
		}

		public IEnumerable<T> GetValues<T>(Func<DataRow, T> selectorFunc)
		{
			throw new NotImplementedException();
		}

		public T TimeIntegral<T>(ModalResultField field, Func<SI, bool> filter = null) where T : SIBase<T>
		{
			throw new NotImplementedException();
		}

		public T TimeIntegral<T>(string field, Func<SI, bool> filter = null) where T : SIBase<T>
		{
			throw new NotImplementedException();
		}

		public Dictionary<string, DataColumn> Auxiliaries { get; set; }

		public void SetDataValue(string fieldName, object value)
		{
			throw new System.NotImplementedException();
		}

		public void AddAuxiliary(string id, string columnName = null)
		{
			var auxColName = columnName ?? ModalResultField.P_aux_ + id;
			if (!Data.Columns.Contains(auxColName)) {
				Auxiliaries[id] = Data.Columns.Add(auxColName, typeof(Watt));
			}
			Auxiliaries[id] = Data.Columns[auxColName];
		}

		public void Dispose()
		{
			throw new NotImplementedException();
		}

		public void FinishSimulation()
		{
			Data.Rows.Clear();
		}

		public string GetColumnName(IFuelProperties fuelData, ModalResultField mrf)
		{
			if (!FuelColumns.ContainsKey(fuelData) || !FuelColumns[fuelData].ContainsKey(mrf)) {
				throw new VectoException("unknown fuel {0} for key {1}", fuelData.GetLabel(), mrf.GetName());
			}

			return FuelColumns[fuelData][mrf].ColumnName;
		}

		public void Reset()
		{
			
		}

		public Second Duration
		{
			get { return _duration; }
		}

		public Meter Distance
		{
			get { return _distance; }
		}

		public Func<Second, Joule, Joule> AuxHeaterDemandCalc { get; set; }

		public KilogramPerWattSecond EngineLineCorrectionFactor(IFuelProperties fuel)
		{
			return 0.SI<KilogramPerWattSecond>();
		}

		public void CalculateAggregateValues()
		{
			
		}

		public void AddElectricMotor(PowertrainPosition pos)
		{
			
		}

		public KilogramPerWattSecond VehicleLineSlope(IFuelProperties fuel)
		{
			return 0.SI<KilogramPerWattSecond>();
		}

		public bool HasCombustionEngine { get; set; }
		public WattSecond TotalElectricMotorWorkDrive(PowertrainPosition emPos)
		{
			throw new NotImplementedException();
		}

		public WattSecond TotalElectricMotorWorkRecuperate(PowertrainPosition emPos)
		{
			throw new NotImplementedException();
		}

		public PerSecond ElectricMotorAverageSpeed(PowertrainPosition emPos)
		{
			throw new NotImplementedException();
		}

		public double ElectricMotorEfficiencyDrive(PowertrainPosition emPos)
		{
			throw new NotImplementedException();
		}

		public double ElectricMotorEfficiencyGenerate(PowertrainPosition emPos)
		{
			throw new NotImplementedException();
		}

		public WattSecond ElectricMotorOffLosses(PowertrainPosition emPos)
		{
			throw new NotImplementedException();
		}

		public WattSecond ElectricMotorLosses(PowertrainPosition emPos)
		{
			throw new NotImplementedException();
		}

		public WattSecond ElectricMotorTransmissionLosses(PowertrainPosition emPos)
		{
			throw new NotImplementedException();
		}

		public double BatteryStartSoC()
		{
			throw new NotImplementedException();
		}

		public double REESSEndSoC()
		{
			throw new NotImplementedException();
		}

		public WattSecond REESSLoss()
		{
			throw new NotImplementedException();
		}

		public WattSecond REESSEnergyEnd()
		{
			throw new NotImplementedException();
		}

		public string RunName { get; set; }
		public string CycleName { get; set; }
		public string RunSuffix { get; set; }

		public object this[ModalResultField key]
		{
			get { return CurrentRow[key.GetName()]; }
			set { CurrentRow[key.GetName()] = value; }
		}

		public void CommitSimulationStep(Second absTime, Second simulationInterval)
		{
			CurrentRow[ModalResultField.time.GetName()] = (absTime + simulationInterval / 2);
			CurrentRow[ModalResultField.simulationInterval.GetName()] = simulationInterval;
			CommitSimulationStep();
		}
	}
}