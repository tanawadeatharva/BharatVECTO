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

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Tests.Utils;

namespace TUGraz.VectoCore.Tests.Models.SimulationComponentData
{
	[TestClass]
	public class AuxiliaryTypeHelperTest
	{
		[TestMethod]
		public void TestParseAuxiliaryType()
		{
			Assert.AreEqual(AuxiliaryType.Fan, AuxiliaryTypeHelper.Parse("Fan"));
			Assert.AreEqual(AuxiliaryType.SteeringPump, AuxiliaryTypeHelper.Parse("Steering pump"));
			Assert.AreEqual(AuxiliaryType.ElectricSystem, AuxiliaryTypeHelper.Parse("Electric System"));
			Assert.AreEqual(AuxiliaryType.HeatingVentilationAirCondition, AuxiliaryTypeHelper.Parse("HVAC"));
			Assert.AreEqual(AuxiliaryType.PneumaticSystem, AuxiliaryTypeHelper.Parse("Pneumatic System"));
			AssertHelper.Exception<ArgumentOutOfRangeException>(() => { AuxiliaryTypeHelper.Parse("Foo Bar Blupp"); });
		}

		[TestMethod]
		public void TestToString()
		{
			Assert.AreEqual(Constants.Auxiliaries.Names.Fan, AuxiliaryTypeHelper.ToString(AuxiliaryType.Fan));
			Assert.AreEqual(Constants.Auxiliaries.Names.SteeringPump, AuxiliaryTypeHelper.ToString(AuxiliaryType.SteeringPump));
			Assert.AreEqual(Constants.Auxiliaries.Names.HeatingVentilationAirCondition,
				AuxiliaryTypeHelper.ToString(AuxiliaryType.HeatingVentilationAirCondition));
			Assert.AreEqual(Constants.Auxiliaries.Names.PneumaticSystem,
				AuxiliaryTypeHelper.ToString(AuxiliaryType.PneumaticSystem));
			Assert.AreEqual(Constants.Auxiliaries.Names.ElectricSystem,
				AuxiliaryTypeHelper.ToString(AuxiliaryType.ElectricSystem));
		}
	}
}