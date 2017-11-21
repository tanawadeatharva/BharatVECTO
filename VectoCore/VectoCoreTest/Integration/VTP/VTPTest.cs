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

using NUnit.Framework;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.FileIO;

namespace TUGraz.VectoCore.Tests.Integration.VTP
{
    [TestFixture]
    public class VTPTest
    {
        [TestCase()]
        public void RunVTP()
        {
            var jobFile = @"TestData\Integration\VTPMode\GenericVehicle\class_5_generic_vehicle.vecto";

            var fileWriter = new FileOutputWriter(jobFile);
            var sumWriter = new SummaryDataContainer(fileWriter);
            var jobContainer = new JobContainer(sumWriter);
            var dataProvider = JSONInputDataFactory.ReadJsonJob(jobFile);
            var runsFactory = new SimulatorFactory(ExecutionMode.Engineering, dataProvider, fileWriter) {
                ModalResults1Hz = false,
                WriteModalResults = true,
                ActualModalData = false,
                Validate = false,
            };
            
            jobContainer.AddRuns(runsFactory);

			//var i = 0;
			//jobContainer.Runs[i].Run.Run();
			//Assert.IsTrue(jobContainer.Runs[i].Run.FinishedWithoutErrors);

			jobContainer.Execute();
			jobContainer.WaitFinished();

			Assert.AreEqual(true, jobContainer.AllCompleted);
            
        }

    }
}