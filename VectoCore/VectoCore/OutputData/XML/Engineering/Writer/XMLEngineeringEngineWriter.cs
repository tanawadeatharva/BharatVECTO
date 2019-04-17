using System.IO;
using System.Xml.Linq;
using TUGraz.IVT.VectoXML;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.OutputData.XML.Engineering.Interfaces;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.OutputData.XML.Engineering.Writer
{
	internal class XMLEngineeringEngineWriterV10 : AbstractComponentWriter<IEngineEngineeringInputData>,
		IXMLEngineeringEngineWriter
	{
		protected XNamespace _componentNamespace;
		public const string NAMESPACE_URI = XMLDefinitions.ENGINEERING_DEFINITONS_NAMESPACE_V10;


		public XMLEngineeringEngineWriterV10() : base("EngineDataEngineeringType") { }

		#region Overrides of AbstractXMLEngineeringEngineWriter

		protected override object[] DoWriteXML(IEngineEngineeringInputData data)
		{
			var ns = ComponentDataNamespace;
			return new object[] {
				GetXMLTypeAttribute(),
				GetDefaultComponentElements(data),
				new XElement(ns + XMLNames.Engine_Displacement, (data.Displacement.Value() * 1000 * 1000).ToXMLFormat(0)),
				new XElement(ns + XMLNames.Engine_IdlingSpeed, data.IdleSpeed.AsRPM.ToXMLFormat(0)),
				data.Inertia != null ? new XElement(ns + XMLNames.Engine_Inertia, data.Inertia.Value()) : null,
				new XElement(ns + XMLNames.Engine_FCCorrection, data.WHTCEngineering.ToXMLFormat(4)),
				new XElement(ns + XMLNames.Engine_FuelType, data.FuelType.ToXMLFormat()),
				new XElement(ns + XMLNames.Engine_FuelConsumptionMap, GetFuelConsumptionMap(ns, data)),
				new XElement(ns + XMLNames.Engine_FullLoadAndDragCurve, GetFullLoadDragCurve(ns, data))
			};
		}

		#endregion

		protected virtual object[] GetFullLoadDragCurve(XNamespace ns, IEngineEngineeringInputData data)
		{
			if (Writer.Configuration.SingleFile) {
				return EmbedDataTable(data.FullLoadCurve, AttributeMappings.EngineFullLoadCurveMapping);
			}

			var filename = Path.Combine(
				Writer.Configuration.BasePath, Writer.RemoveInvalidFileCharacters(string.Format("ENG_{0}.vfld", data.Model)));
			return ExtCSVResource(data.FullLoadCurve, filename);
		}

		protected virtual object[] GetFuelConsumptionMap(XNamespace ns, IEngineEngineeringInputData data)
		{
			if (Writer.Configuration.SingleFile) {
				return EmbedDataTable(data.FuelConsumptionMap, AttributeMappings.FuelConsumptionMapMapping);
			}

			var filename = Path.Combine(
				Writer.Configuration.BasePath, Writer.RemoveInvalidFileCharacters(string.Format("ENG_{0}.vmap", data.Model)));
			return ExtCSVResource(data.FuelConsumptionMap, filename);
		}

		#region Overrides of AbstractXMLWriter

		public override XNamespace ComponentDataNamespace
		{
			get { return _componentNamespace ?? (_componentNamespace = Writer.RegisterNamespace(NAMESPACE_URI)); }
		}

		#endregion
	}

	internal class XMLEngineeringEngineWriterV10TEST : XMLEngineeringEngineWriterV10
	{
		public new const string NAMESPACE_URI = XMLDefinitions.ENGINEERING_DEFINITONS_NAMESPACE_V10_TEST;

		protected override object[] DoWriteXML(IEngineEngineeringInputData data)
		{
			var v10 = Writer.RegisterNamespace(XMLEngineeringEngineWriterV10.NAMESPACE_URI);
			var v11 = ComponentDataNamespace;
			return new object[] {
				GetXMLTypeAttribute(),
				GetDefaultComponentElements(data),
				new XElement(v10 + XMLNames.Engine_Displacement, (data.Displacement.Value() * 1000 * 1000).ToXMLFormat(0)),
				new XElement(v10 + XMLNames.Engine_IdlingSpeed, data.IdleSpeed.AsRPM.ToXMLFormat(0)),
				data.Inertia != null ? new XElement(v10 + XMLNames.Engine_Inertia, data.Inertia.Value()) : null,
				new XElement(v10 + XMLNames.Engine_FCCorrection, data.WHTCEngineering.ToXMLFormat(4)),
				new XElement(v10 + XMLNames.Engine_FuelType, data.FuelType.ToXMLFormat()),
				new XElement(v10 + XMLNames.Engine_FuelConsumptionMap, GetFuelConsumptionMap(v10, data)),
				new XElement(v10 + XMLNames.Engine_FullLoadAndDragCurve, GetFullLoadDragCurve(v10, data)),
				new XElement(v11 + XMLNames.Engine_RatedPower, data.RatedPowerDeclared?.Value().ToXMLFormat(0) ?? "xxx kW"),
				new XElement(v11 + XMLNames.Engine_RatedSpeed, data.RatedSpeedDeclared?.AsRPM.ToXMLFormat(0) ?? "yyy rpm")
			};
		}

		public override XNamespace ComponentDataNamespace
		{
			get { return _componentNamespace ?? (_componentNamespace = Writer.RegisterNamespace(NAMESPACE_URI)); }
		}
	}
}
