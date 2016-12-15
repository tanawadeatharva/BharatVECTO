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

using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Connector.Ports;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
	public abstract class AbstractGearbox<TStateType> :
			StatefulProviderComponent<TStateType, ITnOutPort, ITnInPort, ITnOutPort>, ITnOutPort, ITnInPort, IGearbox,
			IClutchInfo
		where TStateType : GearboxState, new()
	{
		/// <summary>
		/// The data and settings for the gearbox.
		/// </summary>
		[Required, ValidateObject] internal readonly GearboxData ModelData;

		protected AbstractGearbox(IVehicleContainer container, GearboxData gearboxModelData) : base(container)
		{
			ModelData = gearboxModelData;
		}

		#region ITnOutPort

		public abstract IResponse Request(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity,
			bool dryRun = false);

		public abstract IResponse Initialize(NewtonMeter outTorque, PerSecond outAngularVelocity);

		#endregion

		#region IGearboxCockpit

		public GearboxType GearboxType
		{
			get { return ModelData.Type; }
		}

		/// <summary>
		/// The current gear.
		/// </summary>
		public uint Gear { get; protected internal set; }

		[DebuggerHidden]
		public MeterPerSecond StartSpeed
		{
			get { return ModelData.StartSpeed; }
		}

		[DebuggerHidden]
		public MeterPerSquareSecond StartAcceleration
		{
			get { return ModelData.StartAcceleration; }
		}

		public NewtonMeter GearMaxTorque
		{
			get { return Gear == 0 || !ModelData.Gears.ContainsKey(Gear) ? null : ModelData.Gears[Gear].MaxTorque; }
		}

		public Watt GearboxLoss()
		{
			var ratio = ModelData.Gears[PreviousState.Gear].HasLockedGear
				? ModelData.Gears[PreviousState.Gear].Ratio
				: ModelData.Gears[PreviousState.Gear].TorqueConverterRatio;

			return (PreviousState.TransmissionTorqueLoss +
					PreviousState.InertiaTorqueLossOut / ratio) * PreviousState.InAngularVelocity;
		}

		public Second LastShift { get; protected set; }

		public GearData GetGearData(uint gear)
		{
			return ModelData.Gears[gear];
		}

		public abstract GearInfo NextGear { get; }

		public Second TractionInterruption
		{
			get { return ModelData.TractionInterruption; }
		}

		#endregion

		public abstract bool ClutchClosed(Second absTime);
	}

	public class GearboxState : SimpleComponentState
	{
		public NewtonMeter InertiaTorqueLossOut = 0.SI<NewtonMeter>();
		public NewtonMeter TransmissionTorqueLoss = 0.SI<NewtonMeter>();
		public uint Gear;
		public TransmissionLossMap.LossMapResult TorqueLossResult;
	}
}