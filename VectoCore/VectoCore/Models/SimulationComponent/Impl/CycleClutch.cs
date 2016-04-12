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

using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.Connector.Ports;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
	/// <summary>
	/// Clutch which mediates angularSpeed from powertrain (depending on vehicle speed) with engineSpeed directly set from driving cycle.
	/// Can be thought as: Clutch which is always slipping.
	/// </summary>
	public class CycleClutch : Clutch
	{
		public CycleClutch(IVehicleContainer container) : base(container) {}

		public override IResponse Request(Second absTime, Second dt, NewtonMeter torque, PerSecond angularVelocity,
			bool dryRun = false)
		{
			var angularVelocityIn = DataBus.CycleData.LeftSample.AngularVelocity;

			if (angularVelocity != null) {
				// engaged - act like transmission torque converter (convert torque for angularVelocity to torque for angularVelocityIn) 
				// convert requested power to equivalent torque with angularVelocityIn
				var torqueIn = torque * angularVelocity / angularVelocityIn;

				var retVal = NextComponent.Request(absTime, dt, torqueIn, angularVelocityIn, dryRun);
				retVal.ClutchPowerRequest = torque * angularVelocity;
				return retVal;
			} else {
				// disengaged -> clutch open
				var retVal = NextComponent.Request(absTime, dt, torque, angularVelocityIn, dryRun);
				return retVal;
			}
		}

		public override IResponse Initialize(NewtonMeter torque, PerSecond angularVelocity)
		{
			var angularVelocityIn = DataBus.CycleData.LeftSample.AngularVelocity;
			var torqueIn = torque * angularVelocity / angularVelocityIn;

			var retVal = NextComponent.Initialize(torqueIn, angularVelocityIn);
			retVal.ClutchPowerRequest = torque * angularVelocity;
			return retVal;
		}
	}
}