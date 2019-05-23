using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.OutputData.XML.Engineering.Interfaces;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.OutputData.XML.Engineering.Writer
{
	internal class XMLShiftParmeterDataWriterV10 : AbstractXMLWriter, IXMLGearshiftDataWriter
	{
		public const string NAMESPACE_URI = XMLDefinitions.ENGINEERING_DEFINITONS_NAMESPACE_V10;

		private XNamespace _componentDataNamespace;
		public XMLShiftParmeterDataWriterV10() : base("ShiftStrategyParametersEngineeringType") { }

		#region Overrides of AbstractXMLWriter

		public override XNamespace ComponentDataNamespace
		{
			get { return _componentDataNamespace ?? (_componentDataNamespace = Writer.RegisterNamespace(NAMESPACE_URI)); }
		}

		#endregion

		#region Implementation of IXMLEngineeringComponentWriter

		public override object[] WriteXML(IDriverModelData inputData)
		{
			var gearshift = inputData as IGearshiftEngineeringInputData;
			var tns = ComponentDataNamespace;
			if (gearshift == null) {
				return new object[] { };
			}

			return new object[] {
				new XElement(
					tns + XMLNames.DriverModel_ShiftStrategyParameters_UpshiftMinAcceleration,
					gearshift.UpshiftMinAcceleration.Value()),
				new XElement(
					tns + XMLNames.DriverModel_ShiftStrategyParameters_DownshiftAfterUpshiftDelay,
					gearshift.DownshiftAfterUpshiftDelay.Value()),
				new XElement(
					tns + XMLNames.DriverModel_ShiftStrategyParameters_UpshiftAfterDownshiftDelay,
					gearshift.UpshiftAfterDownshiftDelay.Value()),
				new XElement(tns + XMLNames.DriverModel_ShiftStrategyParameters_TorqueReserve, gearshift.TorqueReserve),
				new XElement(
					tns + XMLNames.DriverModel_ShiftStrategyParameters_TimeBetweenGearshift,
					gearshift.MinTimeBetweenGearshift.Value()),
				new XElement(tns + XMLNames.DriverModel_ShiftStrategyParameters_StartSpeed, gearshift.StartSpeed.Value()),
				new XElement(
					tns + XMLNames.DriverModel_ShiftStrategyParameters_StartAcceleration, gearshift.StartAcceleration.Value()),
				new XElement(tns + XMLNames.DriverModel_ShiftStrategyParameters_StartTorqueReserve, gearshift.StartTorqueReserve)
			};
		}

		#endregion
	}
}
