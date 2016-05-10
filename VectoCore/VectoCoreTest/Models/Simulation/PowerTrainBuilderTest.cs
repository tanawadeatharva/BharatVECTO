/*
* This file is part of VECTO.
*
* Copyright © 2012-2016 European Union
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

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.InputData.Reader.Impl;
using TUGraz.VectoCore.Models.Connector.Ports;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.Tests.Utils;

namespace TUGraz.VectoCore.Tests.Models.Simulation
{
	[TestClass]
	public class PowerTrainBuilderTest
	{
		public const string JobFile = @"TestData\Jobs\24t Coach.vecto";

		[TestMethod]
		public void BuildFullPowerTrainTest()
		{
			var dataProvider = JSONInputDataFactory.ReadJsonJob(JobFile);
			var engineeringProvider = dataProvider as IEngineeringInputDataProvider;
			if (engineeringProvider == null) {
				throw new VectoException("Failed to cas to Engineering InputDataProvider");
			}
			var reader = new EngineeringModeVectoRunDataFactory(engineeringProvider);
			var runData = reader.NextRun().First();

			var writer = new MockModalDataContainer();
			var builder = new PowertrainBuilder(writer);

			var powerTrain = builder.Build(runData);

			Assert.IsInstanceOfType(powerTrain, typeof(IVehicleContainer));
			Assert.AreEqual(11, powerTrain.SimulationComponents().Count);

			Assert.IsInstanceOfType(powerTrain.Engine, typeof(CombustionEngine));
			Assert.IsInstanceOfType(powerTrain.Gearbox, typeof(Gearbox));
			Assert.IsInstanceOfType(powerTrain.Cycle, typeof(ISimulationOutPort));
			Assert.IsInstanceOfType(powerTrain.Vehicle, typeof(Vehicle));
		}
	}
}