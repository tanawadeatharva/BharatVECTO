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
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.VectoCore.Models.SimulationComponent
{
	/// <summary>
	/// Base class for all vecto simulation components.
	/// </summary>
	public abstract class VectoSimulationComponent : LoggingObject
	{
		[NonSerialized] protected IDataBus DataBus;

		/// <summary>
		/// Constructor. Registers the component in the cockpit.
		/// </summary>
		/// <param name="dataBus">The vehicle container</param>
		protected VectoSimulationComponent(IVehicleContainer dataBus)
		{
			DataBus = dataBus;
			dataBus.AddComponent(this);
		}

		public virtual void CommitSimulationStep(IModalDataContainer container)
		{
			if (container != null) {
				DoWriteModalResults(container);
			}
			DoCommitSimulationStep();
		}

		protected abstract void DoWriteModalResults(IModalDataContainer container);

		/// <summary>
		/// Commits the simulation step.
		/// Writes the moddata into the data writer.
		/// Commits the internal state of the object if needed.
		/// </summary>
		protected abstract void DoCommitSimulationStep();
	}

	public abstract class StatefulVectoSimulationComponent<TStateType> : VectoSimulationComponent where TStateType : new()
	{
		protected internal TStateType CurrentState;
		protected internal TStateType PreviousState;

		protected StatefulVectoSimulationComponent(IVehicleContainer container)
			: base(container)
		{
			CurrentState = new TStateType();
			PreviousState = new TStateType();
		}

		protected void AdvanceState()
		{
			PreviousState = CurrentState;
			CurrentState = new TStateType();
		}
	}

	public abstract class StatefulProviderComponent<TStateType, TProviderOutPort, TProviderInPort, TOutPort> :
			StatefulVectoSimulationComponent<TStateType>
		where TStateType : new()
		where TProviderOutPort : class
		where TProviderInPort : class
	{
		protected TOutPort NextComponent;

		protected StatefulProviderComponent(IVehicleContainer container) : base(container) {}

		public TProviderOutPort OutPort()
		{
			return this as TProviderOutPort;
		}

		public TProviderInPort InPort()
		{
			return this as TProviderInPort;
		}

		public virtual void Connect(TOutPort other)
		{
			NextComponent = other;
		}

		protected override void DoCommitSimulationStep()
		{
			AdvanceState();
		}
	}

	public class SimpleComponentState
	{
		public NewtonMeter OutTorque = 0.SI<NewtonMeter>();
		public NewtonMeter InTorque = 0.SI<NewtonMeter>();

		public PerSecond OutAngularVelocity = 0.SI<PerSecond>();
		public PerSecond InAngularVelocity = 0.SI<PerSecond>();

		public void SetState(NewtonMeter inTorque, PerSecond inAngularVelocity, NewtonMeter outTorque,
			PerSecond outAngularVelocity)
		{
			InTorque = inTorque;
			InAngularVelocity = inAngularVelocity;
			OutTorque = outTorque;
			OutAngularVelocity = outAngularVelocity;
		}
	}
}