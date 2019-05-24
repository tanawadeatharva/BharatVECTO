using System.Xml.Linq;
using Ninject;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.OutputData.XML.Engineering.Factory;
using TUGraz.VectoCore.OutputData.XML.Engineering.Interfaces;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.OutputData.XML.Engineering.Writer
{
	internal class XMLEngineeringDriverDataWriterV10 : AbstractXMLWriter, IXMLEngineeringDriverDataWriter
	{
		private XNamespace _componentDataNamespace;
		public const string NAMESPACE_URI = XMLDefinitions.ENGINEERING_DEFINITONS_NAMESPACE_V10;

		[Inject]
		public IEngineeringWriterFactory Factory { protected get; set; }

		public XMLEngineeringDriverDataWriterV10() : base("DriverModelEngineeringType") { }

		#region Implementation of IXMLEngineeringComponentWriter

		public override object[] WriteXML(IEngineeringInputDataProvider inputData)
		{
			var driverData = inputData.DriverInputData;
			var v10 = ComponentDataNamespace;

			var lacWriter = Factory.GetWriter(driverData.Lookahead, Writer, inputData.DataSource);
			var overspeedWriter = Factory.GetWriter(driverData.OverSpeedData, Writer, inputData.DataSource);
			var accWriter = Factory.GetWriter(driverData.AccelerationCurve, Writer, inputData.DataSource);
			var gearshiftWriter = Factory.GetWriter(driverData.GearshiftInputData, Writer, inputData.DataSource);
			return new object[] {
				new XElement(
					v10 + XMLNames.DriverModel_LookAheadCoasting,
					lacWriter.GetXMLTypeAttribute(),
					lacWriter.WriteXML(driverData.Lookahead)
				),
				new XElement(
					v10 + XMLNames.DriverModel_Overspeed,
					overspeedWriter.GetXMLTypeAttribute(),
					overspeedWriter.WriteXML(driverData.OverSpeedData)
				),
				new XElement(
					v10 + XMLNames.DriverModel_DriverAccelerationCurve,
					accWriter.GetXMLTypeAttribute(),
					accWriter.WriteXML(driverData.AccelerationCurve)
				),
				new XElement(
					v10 + XMLNames.DriverModel_ShiftStrategyParameters,
					gearshiftWriter.GetXMLTypeAttribute(),
					gearshiftWriter.WriteXML(driverData.GearshiftInputData)
				)
			};
		}

		#endregion

		#region Overrides of AbstractXMLWriter

		public override XNamespace ComponentDataNamespace
		{
			get { return _componentDataNamespace ?? (_componentDataNamespace = Writer.RegisterNamespace(NAMESPACE_URI)); }
		}

		#endregion
	}
}
