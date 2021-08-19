using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Castle.Components.DictionaryAdapter;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;

namespace TUGraz.VectoCore.OutputData.XML.GroupWriter.Declaration.Vehicle.CompletedBus
{
    class CompletedBusPassengerCountWriter_V2_10_2 : GroupWriter, IVehicleDeclarationGroupWriter
	{
		public CompletedBusPassengerCountWriter_V2_10_2(XNamespace writerNamespace) : base(writerNamespace) { }


		#region Implementation of IGroupWriter


		public XElement[] GetGroupElements(IVehicleDeclarationInputData vehicle)
		{
			return new XElement[] {
				new XElement(_writerNamespace + XMLNames.Bus_NumberPassengerSeatsLowerDeck,
					vehicle.NumberPassengerSeatsLowerDeck),

				new XElement(_writerNamespace + XMLNames.Bus_NumberPassengersStandingLowerDeck,
					vehicle.NumberPassengersStandingLowerDeck),

				new XElement(_writerNamespace + XMLNames.Bus_NumberPassengerSeatsUpperDeck,
					vehicle.NumberPassengerSeatsUpperDeck),

				new XElement(_writerNamespace + XMLNames.Bus_NumberPassengersStandingUpperDeck,
					vehicle.NumberPassengersStandingUpperDeck),
			};
		}

		#endregion


	}

}
