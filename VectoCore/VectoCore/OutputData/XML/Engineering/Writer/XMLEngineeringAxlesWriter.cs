using System.Collections.Generic;
using System.Xml.Linq;
using Ninject;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.OutputData.XML.Engineering.Factory;
using TUGraz.VectoCore.OutputData.XML.Engineering.Interfaces;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.OutputData.XML.Engineering.Writer
{
	internal class XMLEngineeringAxlesWriterV10 : AbstractXMLWriter, IXMLEngineeringAxlesWriter
	{
		private XNamespace _componentDataNamespace;
		private object[] _axles;
		public const string NAMESPACE_URI = XMLDefinitions.ENGINEERING_DEFINITONS_NAMESPACE_V10;

		[Inject]
		public IEngineeringWriterFactory Factory { protected get; set; }

		public XMLEngineeringAxlesWriterV10() : base("AxleComponentEngineeringType") { }

		#region Overrides of AbstractXMLWriter

		#region Overrides of AbstractXMLWriter

		public override object[] WriteXML(IComponentInputData vehicle)
		{
			return _axles ?? (_axles = CreateAxles(vehicle));
		}

		private object[] CreateAxles(IComponentInputData inputData)
		{
			var vehicle = inputData as IVehicleEngineeringInputData;
			if (vehicle == null) {
				return null;
			}
			var retVal = new List<object>();
			var tns = ComponentDataNamespace;
			var idx = 1;
			foreach (var axle in vehicle.Components.AxleWheels.AxlesEngineering) {
				var writer = Factory.GetWriter(axle, Writer, axle.DataSource);
				retVal.Add(
					new XElement(
						tns + XMLNames.AxleWheels_Axles_Axle,
						writer.GetXMLTypeAttribute(),
						writer.WriteXML(axle, idx++, vehicle.DynamicTyreRadius)
					));
			}

			return new object[] { new XElement(tns + XMLNames.AxleWheels_Axles, retVal.ToArray()) };
		}

		#endregion

		public override XNamespace ComponentDataNamespace
		{
			get { return _componentDataNamespace ?? (_componentDataNamespace = Writer.RegisterNamespace(NAMESPACE_URI)); }
		}

		#endregion

		#region Implementation of IXMLEngineeringComponentWriter

		#endregion
	}
}
