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
using System.IO;
using System.Linq;
using NUnit.Framework;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.FileIO;
using TUGraz.VectoCore.Tests.Utils;

namespace TUGraz.VectoCore.Tests.Integration
{
	[TestFixture]
	[Parallelizable(ParallelScope.All)]
	public class FuelTypesTest
	{

		[OneTimeSetUp]
		public void RunBeforeAnyTests()
		{
			Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);
		}


		[Category("LongRunning")]
		[Category("Integration")]
		[TestCase(FuelType.DieselCI, null,
			@"TestData\Integration\DeclarationMode\Class2_RigidTruck_4x2\Class2_RigidTruck_DECL.vecto", 0,
			0.000219912706926502, 0.000219912706926502, 26.3053477184811, 0.000688326772679951, 9390.27258576163,
			TestName = "Diesel LH Low"),
		TestCase(FuelType.EthanolCI, null,
			@"TestData\Integration\DeclarationMode\Class2_RigidTruck_4x2\Class2_RigidTruck_DECL.vecto", 0,
			0.000219912706926502, 0.000222510101102798, 27.1353781832681, 0.000402743282996065, 5651.75656801107,
			TestName = "Ethanol/CI LH Low"),

		TestCase(FuelType.DieselCI, null,
			@"TestData\Integration\DeclarationMode\Class2_RigidTruck_4x2\Class2_RigidTruck_DECL.vecto", 1,
			0.000254681238692732, 0.000254681238692732, 30.464263001523, 0.000797152277108253, 10874.8888921797,
			TestName = "Diesel LH Ref"),
		TestCase(FuelType.EthanolCI, null,
			@"TestData\Integration\DeclarationMode\Class2_RigidTruck_4x2\Class2_RigidTruck_DECL.vecto", 1,
			0.000254681238692732, 0.000257689284819024, 31.4255225389053, 0.000466417605522433, 6545.3078344032,
			TestName = "Ethanol/CI LH Ref"),
		TestCase(FuelType.EthanolPI, null,
			@"TestData\Integration\DeclarationMode\Class2_RigidTruck_4x2\Class2_RigidTruck_DECL.vecto", 1,
			0.000254681238692732, 0.000252942800203362, 32.1810178375779, 0.000531179880427061, 7411.22404595852,
			TestName = "Ethanol/PI LH Ref"),
		TestCase(FuelType.PetrolPI, null,
			@"TestData\Integration\DeclarationMode\Class2_RigidTruck_4x2\Class2_RigidTruck_DECL.vecto", 1,
			0.000254681238692732, 0.000254681238692732, 34.0482939428787, 0.000774230965625907, 10569.2714057484,
			TestName = "Petrol/PI LH Ref"),
		TestCase(FuelType.LPGPI, null,
			@"TestData\Integration\DeclarationMode\Class2_RigidTruck_4x2\Class2_RigidTruck_DECL.vecto", 1,
			0.000254681238692732, 0.000254681238692732, null, 0.000769137340852052, 11715.3369798657,
			TestName = "LPG/PI LH Ref"),
		TestCase(FuelType.NGPI, TankSystem.Liquefied,
			@"TestData\Integration\DeclarationMode\Class2_RigidTruck_4x2\Class2_RigidTruck_DECL.vecto", 1,
			0.000254681238692732, 0.000233933276273773, null, 0.000647995175278351, 11486.1238650422,
			TestName = "LNG/PI LH Ref"),
		TestCase(FuelType.NGPI, TankSystem.Compressed,
			@"TestData\Integration\DeclarationMode\Class2_RigidTruck_4x2\Class2_RigidTruck_DECL.vecto", 1,
			0.000254681238692732, 0.000239294247188382, null, 0.000643701524936747, 11486.1238650423,
			TestName = "CNG/PI LH Ref"),
			]
		public void TestFuelTypesCO2(FuelType fuelType, TankSystem? tankSystem, string jobName, int runIdx, 
			double expectedFCMap, double expectedFCFinal, double? expectedFCperkm, double expectedCo2, double expectedMJ)
		{
			// the same engine fc-map is used for different fuel types, thus the difference in expected results
			var fileWriter = new FileOutputWriter(jobName);
			var sumData = new SummaryDataContainer(fileWriter);

			var jobContainer = new JobContainer(sumData);
			var inputData = JSONInputDataFactory.ReadJsonJob(jobName);

			var runsFactory = new SimulatorFactory(ExecutionMode.Declaration, inputData, fileWriter) {
				WriteModalResults = true,
				Validate = false
			};

			jobContainer.AddRuns(runsFactory);

			// store a reference to the data table here because it is going to be deleted
			var run = jobContainer.Runs[runIdx];
			var modContainer = (ModalDataContainer)run.Run.GetContainer().ModalData;
			var modData = modContainer.Data;

			var fuelData = FuelData.Instance().Lookup(fuelType, tankSystem);
			
			// change the fuel entry in mod data and engine data
			var origFuel = modContainer.FuelColumns.Keys.First();
			modContainer.FuelColumns[fuelData] = modContainer.FuelColumns[origFuel];
			if (fuelData.FuelType != origFuel.FuelType) {
				modContainer.FuelColumns.Remove(origFuel);
			}
			((VehicleContainer)run.Run.GetContainer()).RunData.EngineData.Fuels.First().FuelData = fuelData;

			run.Run.Run();

			// restore data table before assertions
			modContainer.Data = modData;
			var fuel = modContainer.FuelData.First();
			var distance = modContainer.Distance;
			var fcFinal = modContainer.FuelConsumptionPerMeter(ModalResultField.FCFinal, fuel);
			var fcVolumePerMeter = fuel.FuelDensity == null ? null :
				(fcFinal / fuel.FuelDensity)
				.Cast<VolumePerMeter>();
			var co2PerMeter = distance == null ? null : fcFinal * fuel.CO2PerFuelWeight;
			var energyPerMeter = distance == null ? null : fcFinal * fuel.LowerHeatingValueVecto;

//			Console.WriteLine("FC-Map g/m: {0}, FC-Final g/m {1}, FC-Final l/100km: {2}, CO2 g/m: {3}, Energy J/m: {4}",
			Console.WriteLine("{0}, {1}, {2}, {3}, {4}",
				modContainer.FuelConsumptionPerMeter(ModalResultField.FCMap, fuel).Value(),
				fcFinal.Value(), fcVolumePerMeter?.ConvertToLiterPer100Kilometer().Value ?? double.NaN,
				co2PerMeter?.Value(), energyPerMeter?.Value());

			AssertHelper.AreRelativeEqual(expectedFCMap, modContainer.FuelConsumptionPerMeter(ModalResultField.FCMap, fuel), 1e-3);
			AssertHelper.AreRelativeEqual(expectedFCFinal, fcFinal, 1e-3);
			AssertHelper.AreRelativeEqual(expectedFCperkm, fcVolumePerMeter?.ConvertToLiterPer100Kilometer().Value.SI(), 1e-3);

			AssertHelper.AreRelativeEqual(expectedCo2, co2PerMeter, 1e-3);
			AssertHelper.AreRelativeEqual(expectedMJ, energyPerMeter, 1e-3);
		}
	}
}
