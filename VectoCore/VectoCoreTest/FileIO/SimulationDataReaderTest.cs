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

using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.InputData.Reader.Impl;
using TUGraz.VectoCore.Models.Declaration;

namespace TUGraz.VectoCore.Tests.FileIO
{
	[TestClass]
	public class SimulationDataReaderTest
	{
		protected const string DeclarationJob = @"TestData\Jobs\12t Delivery Truck.vecto";
		protected const double Tolerance = 0.0001;

		[TestMethod]
		public void ReadDeclarationJobFile()
		{
			var dataProvider = JSONInputDataFactory.ReadJsonJob(DeclarationJob);
			var declarationProvider = dataProvider as IDeclarationInputDataProvider;
			if (declarationProvider == null) {
				throw new VectoException("Failed to cas to Engineering InputDataProvider");
			}
			var reader = new DeclarationModeVectoRunDataFactory(declarationProvider, null);
			//reader.SetJobFile(DeclarationJob);

			var runData = reader.NextRun().First();

			Assert.AreEqual(ExecutionMode.Declaration, runData.ExecutionMode);

			Assert.AreEqual(Path.GetFileNameWithoutExtension(DeclarationJob), runData.JobName);

			// curbweight + bodyCurbWeight + trailerCurbWeight (for Long Haul only)
			Assert.AreEqual(5850 + 1900 + 3400, runData.VehicleData.CurbWeight.Value());

			Assert.AreEqual(11900, runData.VehicleData.GrossVehicleWeight.Value());
			Assert.AreEqual(AxleConfiguration.AxleConfig_4x2, runData.VehicleData.AxleConfiguration);
			Assert.AreEqual(0.4069297458, runData.VehicleData.DynamicTyreRadius.Value(), Tolerance);

			Assert.AreEqual(VehicleClass.Class2, runData.VehicleData.VehicleClass);
			Assert.AreEqual(3, runData.VehicleData.AxleData.Count);
			Assert.AreEqual(6, runData.VehicleData.AxleData[0].Inertia.Value(), Tolerance);

			Assert.AreEqual(true, runData.DriverData.LookAheadCoasting.Enabled);
			//Assert.AreEqual(DeclarationData.Driver.LookAhead.MinimumSpeed.Value(),
			//	runData.DriverData.LookAheadCoasting.MinSpeed.Value(), Tolerance);
			//Assert.AreEqual(DeclarationData.Driver.LookAhead.Deceleration.Value(),
			//	runData.DriverData.LookAheadCoasting.Deceleration.Value(), Tolerance);

			Assert.AreNotEqual(DriverMode.Off, runData.DriverData.OverSpeedEcoRoll.Mode);
			Assert.AreEqual(DeclarationData.Driver.OverSpeedEcoRoll.MinSpeed.Value(),
				runData.DriverData.OverSpeedEcoRoll.MinSpeed.Value(), Tolerance);
			Assert.AreEqual(DeclarationData.Driver.OverSpeedEcoRoll.OverSpeed.Value(),
				runData.DriverData.OverSpeedEcoRoll.OverSpeed.Value(), Tolerance);
			Assert.AreEqual(DeclarationData.Driver.OverSpeedEcoRoll.UnderSpeed.Value(),
				runData.DriverData.OverSpeedEcoRoll.UnderSpeed.Value(), Tolerance);

			//Assert.AreEqual(false, runData.DriverData.StartStop.Enabled);
			Assert.AreEqual(DeclarationData.Driver.StartStop.Delay.Value(), runData.DriverData.StartStop.Delay.Value(), Tolerance);
			Assert.AreEqual(DeclarationData.Driver.StartStop.MaxSpeed.Value(), runData.DriverData.StartStop.MaxSpeed.Value(),
				Tolerance);
			Assert.AreEqual(DeclarationData.Driver.StartStop.MinTime.Value(), runData.DriverData.StartStop.MinTime.Value(),
				Tolerance);

			Assert.AreEqual(3.7890, runData.EngineData.Inertia.Value());

			var downshiftSpeeds = new[] { 660, 660, 1750.70139 };
			var downshiftTorque = new[] { -163.9, 208.116856, 988.9 };

			Assert.AreEqual(downshiftSpeeds.Length, runData.GearboxData.Gears[2].ShiftPolygon.Downshift.Count);
			for (var i = 0; i < downshiftSpeeds.Length; i++) {
				Assert.AreEqual(downshiftSpeeds[i].RPMtoRad().Value(),
					runData.GearboxData.Gears[2].ShiftPolygon.Downshift[i].AngularSpeed.Value(), Tolerance);
				Assert.AreEqual(downshiftTorque[i], runData.GearboxData.Gears[2].ShiftPolygon.Downshift[i].Torque.Value(), Tolerance);
			}

			var upshiftSpeed = new[] { 1891.2419, 1891.2419, 5798.4116 };
			var upshiftTorque = new[] { -163.9, 245.3663, 988.9 };

			Assert.AreEqual(upshiftSpeed.Length, runData.GearboxData.Gears[2].ShiftPolygon.Downshift.Count);
			for (var i = 0; i < downshiftSpeeds.Length; i++) {
				Assert.AreEqual(upshiftSpeed[i].RPMtoRad().Value(),
					runData.GearboxData.Gears[1].ShiftPolygon.Upshift[i].AngularSpeed.Value(), Tolerance);
				Assert.AreEqual(upshiftTorque[i], runData.GearboxData.Gears[1].ShiftPolygon.Upshift[i].Torque.Value(), Tolerance);
			}
			//Assert.AreEqual(, runData.DriverData.LookAheadCoasting.);
		}
	}
}