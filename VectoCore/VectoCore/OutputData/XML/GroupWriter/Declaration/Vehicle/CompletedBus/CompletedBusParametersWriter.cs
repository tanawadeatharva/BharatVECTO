using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Castle.Components.DictionaryAdapter;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;

namespace TUGraz.VectoCore.OutputData.XML.GroupWriter.Declaration.Vehicle.CompletedBus
{
    class CompletedBusParametersWriterV2_10_2 : GroupWriter, IVehicleDeclarationGroupWriter
	{
		public CompletedBusParametersWriterV2_10_2(XNamespace writerNamespace) : base(writerNamespace) { }


		public static XElement[] GetGroupElements(IVehicleDeclarationInputData vehicle, XNamespace writerNamespace)
		{
			var elements = new List<XElement>();

			if (vehicle.Model != null) {
				elements.Add(new XElement(writerNamespace + XMLNames.Component_Model,
					vehicle.Model));
			}

			if (vehicle.LegislativeClass != null) {
				elements.Add(new XElement(writerNamespace + XMLNames.Vehicle_LegislativeCategory,
					vehicle.LegislativeClass.ToXMLFormat()));
			}

			if (vehicle.CurbMassChassis != null) {
				elements.Add(new XElement(writerNamespace + XMLNames.CorrectedActualMass,
					vehicle.CurbMassChassis.ToXMLFormat(0)));
			}

			if (vehicle.GrossVehicleMassRating != null) {
				elements.Add(new XElement(writerNamespace + XMLNames.Vehicle_TPMLM,
					vehicle.GrossVehicleMassRating.ToXMLFormat(0)));
			}

			if (vehicle.AirdragModifiedMultistage != null) {
				elements.Add(new XElement(writerNamespace + XMLNames.Bus_AirdragModifiedMultistage,
					vehicle.AirdragModifiedMultistage));
			}

			if (vehicle.RegisteredClass != null) {
				elements.Add(new XElement(writerNamespace + XMLNames.Vehicle_RegisteredClass,
					vehicle.RegisteredClass.ToXMLFormat()));
			}

			return elements.ToArray();
		}


		#region Implementation of IVehicleDeclarationGroupWriter

		public XElement[] GetGroupElements(IVehicleDeclarationInputData vehicle)
		{
			return GetGroupElements(vehicle, _writerNamespace);
		}

		#endregion
	}
}
