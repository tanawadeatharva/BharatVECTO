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
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.OutputData.FileIO;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Tests.Models.Simulation
{
	[TestClass]
	public class FactoryTest
	{
		public const string DeclarationJobFile = @"Testdata\Jobs\12t Delivery Truck.vecto";

		public const string EngineeringJobFile = @"Testdata\Jobs\24t Coach.vecto";

		[TestMethod]
		public void CreateDeclarationSimulationRun()
		{
			var fileWriter = new FileOutputWriter(DeclarationJobFile);

			var inputData = JSONInputDataFactory.ReadJsonJob(DeclarationJobFile);
			var factory = new SimulatorFactory(ExecutionMode.Declaration, inputData, fileWriter);

			//factory.DataReader.SetJobFile(DeclarationJobFile);

			var run = factory.SimulationRuns().First();
			var vehicleContainer = (VehicleContainer)run.GetContainer();

			Assert.AreEqual(11, vehicleContainer.Components.Count);

			Assert.IsInstanceOfType(vehicleContainer.Gearbox, typeof(Gearbox), "gearbox not installed");
			Assert.IsInstanceOfType(vehicleContainer.Engine, typeof(CombustionEngine), "engine not installed");
			Assert.IsInstanceOfType(vehicleContainer.Vehicle, typeof(Vehicle), "vehicle not installed");

			var gearbox = vehicleContainer.Gearbox as Gearbox;
			Assert.IsNotNull(gearbox);


			// -- shiftpolygon downshift 

			Assert.AreEqual(600.RPMtoRad().Value(), gearbox.ModelData.Gears[1].ShiftPolygon.Downshift[0].AngularSpeed.Value(), 0.0001);
			Assert.AreEqual(0.0, gearbox.ModelData.Gears[1].ShiftPolygon.Downshift[0].Torque.Value(), 0.0001);

			Assert.AreEqual(600.RPMtoRad().Value(), gearbox.ModelData.Gears[1].ShiftPolygon.Downshift[1].AngularSpeed.Value(), 0.0001);
			Assert.AreEqual(266.8277, gearbox.ModelData.Gears[1].ShiftPolygon.Downshift[1].Torque.Value(), 0.1);

			Assert.AreEqual(1310.7646.RPMtoRad().Value(), gearbox.ModelData.Gears[1].ShiftPolygon.Downshift[2].AngularSpeed.Value(),
				0.1);
			Assert.AreEqual(899, gearbox.ModelData.Gears[1].ShiftPolygon.Downshift[2].Torque.Value(), 0.0001);

			// -- shiftpolygon upshift

			Assert.AreEqual(1531.5293.RPMtoRad().Value(), gearbox.ModelData.Gears[1].ShiftPolygon.Upshift[0].AngularSpeed.Value(), 0.1);
			Assert.AreEqual(0, gearbox.ModelData.Gears[1].ShiftPolygon.Upshift[0].Torque.Value(), 0.0001);

			Assert.AreEqual(1531.5293.RPMtoRad().Value(), gearbox.ModelData.Gears[1].ShiftPolygon.Upshift[1].AngularSpeed.Value(), 0.1);
			Assert.AreEqual(459.881, gearbox.ModelData.Gears[1].ShiftPolygon.Upshift[1].Torque.Value(), 0.1);

			Assert.AreEqual(2421.RPMtoRad().Value(), gearbox.ModelData.Gears[1].ShiftPolygon.Upshift[2].AngularSpeed.Value(), 0.1);
			Assert.AreEqual(899, gearbox.ModelData.Gears[1].ShiftPolygon.Upshift[2].Torque.Value(), 0.1);
		}

		[TestMethod]
		public void CreateEngineeringSimulationRun()
		{
			var fileWriter = new FileOutputWriter(EngineeringJobFile);

			var inputData = JSONInputDataFactory.ReadJsonJob(EngineeringJobFile);
			var factory = new SimulatorFactory(ExecutionMode.Engineering, inputData, fileWriter);

			var run = factory.SimulationRuns().First();

			var vehicleContainer = (VehicleContainer)run.GetContainer();
			Assert.AreEqual(11, vehicleContainer.Components.Count);

			Assert.IsInstanceOfType(vehicleContainer.Gearbox, typeof(Gearbox), "gearbox not installed");
			Assert.IsInstanceOfType(vehicleContainer.Engine, typeof(CombustionEngine), "engine not installed");
			Assert.IsInstanceOfType(vehicleContainer.Vehicle, typeof(Vehicle), "vehicle not installed");
		}
	}
}