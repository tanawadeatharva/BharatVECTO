using System.IO;
using System.Linq;
using System.Xml.Linq;
using Ninject;
using TUGraz.IVT.VectoXML;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.OutputData.XML.Engineering.Factory;
using TUGraz.VectoCore.OutputData.XML.Engineering.Interfaces;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.OutputData.XML.Engineering.Writer
{
	public class XMLEngineeringVehicleDataWriterV10 : AbstractXMLWriter, IXMLVehicleDataWriter
	{
		private XNamespace _componentDataNamespace;
		public const string NAMESPACE_URI = XMLDefinitions.ENGINEERING_DEFINITONS_NAMESPACE_V10;

		[Inject]
		public IEngineeringWriterFactory Factory { protected get; set; }

		public XMLEngineeringVehicleDataWriterV10() : base("VehicleEngineeringType") { }

		#region Overrides of AbstractXMLWriter

		public override XNamespace ComponentDataNamespace
		{
			get { return _componentDataNamespace ?? (_componentDataNamespace = Writer.RegisterNamespace(NAMESPACE_URI)); }
		}

		#endregion

		#region Implementation of IXMLEngineeringComponentWriter

		public override object[] WriteXML(IComponentInputData inputData)
		{
			var vehicle = inputData as IVehicleEngineeringInputData;
			var ns = ComponentDataNamespace;

			if (vehicle == null) {
				return null;
			}
			var pto = vehicle.Components.PTOTransmissionInputData;

			var componentsWriter = Factory.GetWriter(vehicle.Components, Writer, vehicle.DataSource);
			var retVal =  new object[] {
				GetXMLTypeAttribute(),
				GetDefaultComponentElements(vehicle),
				vehicle.VehicleCategory != VehicleCategory.Unknown ?  new XElement(ns + XMLNames.Vehicle_VehicleCategory, vehicle.VehicleCategory.ToXMLFormat()) : null,
				new XElement(ns + XMLNames.Vehicle_AxleConfiguration, vehicle.AxleConfiguration.GetName()),
				new XElement(ns + XMLNames.Vehicle_CurbMassChassis, vehicle.CurbMassChassis.Value().ToXMLFormat(0)),
				new XElement(ns + XMLNames.Vehicle_GrossVehicleMass, vehicle.GrossVehicleMassRating.Value().ToXMLFormat(0)),

				//new XElement(tns + XMLNames.Vehicle_AirDragArea, airdrag.AirDragArea.Value()),
				new XElement(ns + XMLNames.Vehicle_RetarderType, vehicle.Components.RetarderInputData.Type.ToXMLFormat()),
				vehicle.Components.RetarderInputData.Type.IsDedicatedComponent()
					? new XElement(ns + XMLNames.Vehicle_RetarderRatio, vehicle.Components.RetarderInputData.Ratio.ToXMLFormat(3))
					: null,
				new XElement(ns + XMLNames.Vehicle_AngledriveType, vehicle.Components.AngledriveInputData.Type.ToXMLFormat()),
				new XElement(ns + XMLNames.Vehicle_PTOType, pto.PTOTransmissionType),
				GetPTOData(ns, pto),
				CreateTorqueLimits(ns, vehicle),
				new XElement(ns + XMLNames.Vehicle_CurbMassExtra, vehicle.CurbMassExtra.Value()),
				new XElement(ns + XMLNames.Vehicle_Loading, vehicle.Loading.Value()),
				new XElement(
					ns + XMLNames.Vehicle_Components,
					componentsWriter.WriteXML(vehicle)
				),
				vehicle.ADAS == null ? null : 
				new XElement(ns + XMLNames.Vehicle_AdvancedDriverAssist,
							Factory.GetWriter(vehicle.ADAS, Writer, vehicle.ADAS.DataSource).WriteXML(vehicle)
				), 
			};
			return retVal;
		}

		#endregion

		protected virtual object[] GetPTOData(XNamespace tns, IPTOTransmissionInputData pto)
		{
			if (pto.PTOTransmissionType == "None") {
				return null;
			}

			var ptoLossMap = new XElement(tns + XMLNames.Vehicle_PTOIdleLossMap);
			ptoLossMap.Add(
				Writer.Configuration.SingleFile
					? EmbedDataTable(pto.PTOLossMap, AttributeMappings.PTOLossMap)
					: ExtCSVResource(pto.PTOLossMap, Path.Combine(Writer.Configuration.BasePath, Writer.RemoveInvalidFileCharacters("PTO_LossMap.vptol"))));
			var ptoCycle = new XElement(tns + XMLNames.Vehicle_PTOCycle);
			ptoCycle.Add(
				Writer.Configuration.SingleFile
					? EmbedDataTable(pto.PTOCycle, AttributeMappings.PTOCycleMap)
					: ExtCSVResource(pto.PTOCycle, Path.Combine(Writer.Configuration.BasePath, Writer.RemoveInvalidFileCharacters("PTO_cycle.vptoc"))));

			return new object[] { ptoLossMap, ptoCycle };
		}

		protected virtual XElement CreateTorqueLimits(XNamespace tns, IVehicleDeclarationInputData vehicle)
		{
			return vehicle.TorqueLimits.Count == 0
				? null
				: new XElement(
					tns + XMLNames.Vehicle_TorqueLimits,
					vehicle.TorqueLimits
							.OrderBy(x => x.Gear)
							.Select(
								entry => new XElement(
									tns + XMLNames.Vehicle_TorqueLimits_Entry,
									new XAttribute(XMLNames.Vehicle_TorqueLimits_Entry_Gear_Attr, entry.Gear),
									new XAttribute(XMLNames.Vehicle_TorqueLimits_Entry_MaxTorque_Attr, entry.MaxTorque.ToXMLFormat(0))
								)
							)
				);
		}
	}
}
