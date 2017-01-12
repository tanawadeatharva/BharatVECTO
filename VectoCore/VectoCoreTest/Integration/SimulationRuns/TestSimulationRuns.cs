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
using NUnit.Framework;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.OutputData.FileIO;
using TUGraz.VectoCore.Tests.Models.Simulation;

namespace TUGraz.VectoCore.Tests.Integration.SimulationRuns
{
	[TestFixture]
	public class TestSimulationRuns
	{
		[Category("UserBugs"),
		TestCase("Kies-20161115"),
		TestCase("Mandl-20161115"),
		TestCase("Silberholz-20161121"),
		]
		public static void RunJob_Eng(string jobName)
		{
			var writer = new FileOutputWriter(jobName);
			var inputData = JSONInputDataFactory.ReadJsonJob("TestData\\Bugs\\" + jobName + "\\job.vecto");
			var factory = new SimulatorFactory(ExecutionMode.Engineering, inputData, writer) { WriteModalResults = true };
			var jobContainer = new JobContainer(new MockSumWriter());
			jobContainer.AddRuns(factory);
			jobContainer.Execute();
			jobContainer.WaitFinished();
			Assert.IsTrue(jobContainer.Runs.All(r => r.Success), string.Concat(jobContainer.Runs.Select(r => r.ExecException)));
		}
	}
}