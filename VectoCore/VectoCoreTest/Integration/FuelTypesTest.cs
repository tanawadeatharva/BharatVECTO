/*
* This file is part of VECTO.
*
* Copyright © 2012-2017 European Union
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
using NUnit.Framework;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.FileIO;

namespace TUGraz.VectoCore.Tests.Integration
{
	[TestFixture]
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
			0.0002199424, 0.0002199424, 26.311041667, 0.0006886, 9392.305166,
			TestName = "Diesel LH Low"),
		TestCase(FuelType.EthanolCI, null,
			@"TestData\Integration\DeclarationMode\Class2_RigidTruck_4x2\Class2_RigidTruck_DECL.vecto", 0,
			0.0002199424, 0.0002225401, 27.1412517, 0.000402797, 5652.9799242,
			TestName = "Ethanol/CI LH Low"),

		TestCase(FuelType.DieselCI, null,
			@"TestData\Integration\DeclarationMode\Class2_RigidTruck_4x2\Class2_RigidTruck_DECL.vecto", 1,
			0.0002547295, 0.0002547295, 30.4604632, 0.0007971, 10873.5324987,
			TestName = "Diesel LH Ref"),
		TestCase(FuelType.EthanolCI, null,
			@"TestData\Integration\DeclarationMode\Class2_RigidTruck_4x2\Class2_RigidTruck_DECL.vecto", 1,
			0.0002547295, 0.000257738, 31.421602924, 0.00046650, 6544.4914570,
			TestName = "Ethanol/CI LH Ref"),
		TestCase(FuelType.EthanolPI, null,
			@"TestData\Integration\DeclarationMode\Class2_RigidTruck_4x2\Class2_RigidTruck_DECL.vecto", 1,
			0.0002547295, 0.00025299078, 32.177003992, 0.0005312806, 7410.299665,
			TestName = "Ethanol/PI LH Ref"),
		TestCase(FuelType.PetrolPI, null,
			@"TestData\Integration\DeclarationMode\Class2_RigidTruck_4x2\Class2_RigidTruck_DECL.vecto", 1,
			0.0002547295, 0.00025472954, 34.04404719, 0.0007743778, 10567.9531310,
			TestName = "Petrol/PI LH Ref"),
		TestCase(FuelType.LPGPI, null,
			@"TestData\Integration\DeclarationMode\Class2_RigidTruck_4x2\Class2_RigidTruck_DECL.vecto", 1,
			0.0002547295, 0.0002547295, double.NaN, 0.00076928, 11713.8757597,
			TestName = "LPG/PI LH Ref"),
		TestCase(FuelType.NGPI, TankSystem.Liquefied,
			@"TestData\Integration\DeclarationMode\Class2_RigidTruck_4x2\Class2_RigidTruck_DECL.vecto", 1,
			0.0002547295, 0.00023397765, double.NaN, 0.00064811809, 11484.69123399,
			TestName = "LNG/PI LH Ref"),
		TestCase(FuelType.NGPI, TankSystem.Compressed,
			@"TestData\Integration\DeclarationMode\Class2_RigidTruck_4x2\Class2_RigidTruck_DECL.vecto", 1,
			0.0002547295, 0.0002393396, double.NaN, 0.000643823, 11484.69123399,
			TestName = "CNG/PI LH Ref"),
			]
		public void TestFuelTypesCO2(FuelType fuelType, TankSystem? tankSystem, string jobName, int runIdx, 
			double expectedFCMap, double expectedFCFinal, double expectedFCperkm, double expectedCo2, double expectedMJ)
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
			modContainer.FuelData = fuelData;
			((VehicleContainer)run.Run.GetContainer()).RunData.EngineData.FuelData = fuelData;

			run.Run.Run();

			// restore data table before assertions
			modContainer.Data = modData;
			
			Console.WriteLine("FC-Map g/m: {0}, FC-Final g/m {1}, FC-Final l/100km: {2}, CO2 g/m: {3}, Energy J/m: {4}", 
				modContainer.FCMapPerMeter().Value(),
				modContainer.FuelConsumptionFinal().Value(), 
				modContainer.FuelConsumptionFinalVolumePerMeter()?.ConvertToLiterPer100Kilometer().Value ?? double.NaN,
				modContainer.CO2PerMeter().Value(), 
				modContainer.EnergyPerMeter().Value());

			Assert.AreEqual(expectedFCMap, modContainer.FCMapPerMeter().Value(), 1e-6);
			Assert.AreEqual(expectedFCFinal, modContainer.FuelConsumptionFinal().Value(), 1e-3);
			Assert.AreEqual(expectedFCperkm, modContainer.FuelConsumptionFinalVolumePerMeter()?.ConvertToLiterPer100Kilometer().Value ?? double.NaN, 1e-6);
			
			Assert.AreEqual(expectedCo2, modContainer.CO2PerMeter().Value(), 1e-6);
			Assert.AreEqual(expectedMJ, modContainer.EnergyPerMeter().Value(), 1e-3);
		}
	}
}
