/*
* Copyright 2015 Graz University of Technology
*
* Licensed under the EUPL (the "Licence");
* You may not use this work except in compliance with the Licence.
* You may obtain a copy of the Licence at:
*
* http://ec.europa.eu/idabc/eupl5
*
* Unless required by applicable law or agreed to in writing, software 
* distributed under the Licence is distributed on an "AS IS" basis,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the Licence for the specific language governing permissions and 
* limitations under the Licence.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.SimulationComponent;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Tests.Utils
{
	public class MockDrivingCycle : VectoSimulationComponent, IDrivingCycleInfo, IRoadLookAhead
	{
		private readonly IEnumerator<DrivingCycleData.DrivingCycleEntry> _left;
		private readonly IEnumerator<DrivingCycleData.DrivingCycleEntry> _right;

		public MockDrivingCycle(IVehicleContainer container, DrivingCycleData data) : base(container)
		{
			if (data != null) {
				_left = data.Entries.GetEnumerator();
				_right = data.Entries.GetEnumerator();
			} else {
				_left = Enumerable.Empty<DrivingCycleData.DrivingCycleEntry>().GetEnumerator();
				_right = Enumerable.Empty<DrivingCycleData.DrivingCycleEntry>().GetEnumerator();
			}
			_left.MoveNext();
			_right.MoveNext();
			_right.MoveNext();
		}


		public CycleData CycleData()
		{
			return new CycleData {
				AbsTime = 0.SI<Second>(),
				AbsDistance = 0.SI<Meter>(),
				LeftSample = _left.Current,
				RightSample = _right.Current
			};
		}

		protected override void DoWriteModalResults(IModalDataWriter writer)
		{
			writer[ModalResultField.dist] = 0.SI<Meter>();
			writer[ModalResultField.v_targ] = 0.KMPHtoMeterPerSecond();
			writer[ModalResultField.grad] = 0.SI<Scalar>();
			writer[ModalResultField.altitude] = 0.SI<Meter>();
		}

		protected override void DoCommitSimulationStep()
		{
			_left.MoveNext();
			_right.MoveNext();
		}

		public Meter CycleStartDistance
		{
			get { return 0.SI<Meter>(); }
		}

		public IReadOnlyList<DrivingCycleData.DrivingCycleEntry> LookAhead(Meter lookaheadDistance)
		{
			return new List<DrivingCycleData.DrivingCycleEntry>();
		}

		public IReadOnlyList<DrivingCycleData.DrivingCycleEntry> LookAhead(Second time)
		{
			return new List<DrivingCycleData.DrivingCycleEntry>();
		}
	}
}