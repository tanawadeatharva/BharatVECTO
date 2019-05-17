using System.Xml.Linq;
using Ninject;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.OutputData.XML.Engineering.Factory;
using TUGraz.VectoCore.OutputData.XML.Engineering.Interfaces;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.OutputData.XML.Engineering.Writer
{
	public interface IXMLEngineeringGearboxWriter : IXMLEngineeringComponentWriter { }

	internal class XMLEngineeringGearboxWriterV10 : AbstractComponentWriter<IGearboxEngineeringInputData>,
		IXMLEngineeringGearboxWriter
	{
		private XNamespace _componentNamespace;
		public const string NAMESPACE_URI = XMLDefinitions.ENGINEERING_DEFINITONS_NAMESPACE_V10;

		[Inject]
		public IEngineeringWriterFactory Factory { protected get; set; }

		public XMLEngineeringGearboxWriterV10() : base("GearboxDataEngineeringType") { }

		#region Overrides of AbstractComponentWriter<IGearboxEngineeringInputData>

		protected override object[] DoWriteXML(IGearboxEngineeringInputData data)
		{
			var tns = Writer.RegisterNamespace(NAMESPACE_URI);
			var gears = new XElement(tns + XMLNames.Gearbox_Gears);
			var i = 1;

			foreach (var gearData in data.Gears) {
				var gearWriter = Factory.GetWriter(gearData, Writer, gearData.DataSource);
				var gear = gearWriter.WriteXML(gearData, i++);

				gears.Add(gear);
			}

			return new object[] {
				GetXMLTypeAttribute(),
				new XAttribute(XMLNames.Component_ID_Attr, string.Format("GBX-{0}", data.Model)),

				GetDefaultComponentElements(data),
				new XElement(tns + XMLNames.Gearbox_TransmissionType, data.Type.ToXMLFormat()),
				new XElement(tns + XMLNames.Gearbox_Inertia, data.Inertia.Value()),
				new XElement(tns + XMLNames.Gearbox_TractionInterruption, data.TractionInterruption.Value()), gears
			};
		}

		#endregion

		#region Overrides of AbstractXMLWriter

		public override XNamespace ComponentDataNamespace
		{
			get { return _componentNamespace ?? (_componentNamespace = Writer.RegisterNamespace(NAMESPACE_URI)); }
		}

		#endregion

		#region Implementation of IXMLEngineeringComponentWriter

		#endregion
	}
}
