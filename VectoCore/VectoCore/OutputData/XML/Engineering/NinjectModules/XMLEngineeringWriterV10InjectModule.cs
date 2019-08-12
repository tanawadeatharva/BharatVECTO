using Ninject.Modules;
using TUGraz.VectoCore.OutputData.XML.Engineering.Interfaces;
using TUGraz.VectoCore.OutputData.XML.Engineering.Writer;
using TUGraz.VectoCore.OutputData.XML.Engineering.Writer.DriverData;
using TUGraz.VectoCore.OutputData.XML.Writer;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.OutputData.XML.NinjectModules
{
	public class XMLEngineeringWriterV10InjectModule : NinjectModule
	{
		#region Overrides of NinjectModule

		public override void Load()
		{
			Bind<IXMLEngineeringJobWriter>().To<XMLEngineeringJobWriterV10>().Named(XMLHelper.GetVersionFromNamespaceUri(XMLEngineeringJobWriterV10.NAMESPACE_URI));

			Bind<IXMLVehicleDataWriter>().To<XMLEngineeringVehicleDataWriterV10>()
										.Named(XMLHelper.GetVersionFromNamespaceUri(XMLEngineeringVehicleDataWriterV10.NAMESPACE_URI));

			Bind<IXMLEngineeringComponentWriter>().To<XMLEngineeringComponentsWriterV10>().Named(
				XMLHelper.GetVersionFromNamespaceUri(XMLEngineeringComponentsWriterV10.NAMESPACE_URI));

			Bind<IXMLEngineeringComponentsWriter>().To<XMLEngineeringComponentsWriterV10>()
										.Named(XMLHelper.GetVersionFromNamespaceUri(XMLEngineeringComponentsWriterV10.NAMESPACE_URI));

			Bind<IXMLEngineeringEngineWriter>().To<XMLEngineeringEngineWriterV10>().Named(
				XMLHelper.GetVersionFromNamespaceUri(XMLEngineeringEngineWriterV10.NAMESPACE_URI));

			Bind<IXMLEngineeringGearboxWriter>().To<XMLEngineeringGearboxWriterV10>().Named(
				XMLHelper.GetVersionFromNamespaceUri(XMLEngineeringGearboxWriterV10.NAMESPACE_URI));

			Bind<IXMLEngineeringGearWriter>().To<XMLEngineeringGearDataWriterV10>().Named(
				XMLHelper.GetVersionFromNamespaceUri(XMLEngineeringGearDataWriterV10.NAMESPACE_URI));

			Bind<IXMLEngineeringRetarderWriter>().To<XMLEngineeringRetarderWriterV10>()
												.Named(XMLHelper.GetVersionFromNamespaceUri(XMLEngineeringRetarderWriterV10.NAMESPACE_URI));

			Bind<IXMLEngineeringAxlegearWriter>().To<XMLEngineeringAxlegearWriterV10>()
												.Named(XMLHelper.GetVersionFromNamespaceUri(XMLEngineeringAxlegearWriterV10.NAMESPACE_URI));

			Bind<IXMLEngineeringAngledriveWriter>().To<XMLEngineeringAngledriveWriterV10>()
												.Named(XMLHelper.GetVersionFromNamespaceUri(XMLEngineeringAngledriveWriterV10.NAMESPACE_URI));

			Bind<IXMLEngineeringAxlesWriter>().To<XMLEngineeringAxlesWriterV10>()
											.Named(XMLHelper.GetVersionFromNamespaceUri(XMLEngineeringAxlesWriterV10.NAMESPACE_URI));

			Bind<IXMLEngineeringAxleWriter>().To<XMLEngineeringAxleWriterV10>()
											.Named(XMLHelper.GetVersionFromNamespaceUri(XMLEngineeringAxleWriterV10.NAMESPACE_URI));

			Bind<IXmlEngineeringTyreWriter>().To<XMLEngineeringTyreWriterV10>()
											.Named(XMLHelper.GetVersionFromNamespaceUri(XMLEngineeringTyreWriterV10.NAMESPACE_URI));

			Bind<IXMLEngineeringTorqueconverterWriter>().To<XMLEngineeringTorqueconverterWriterV10>()
														.Named(XMLHelper.GetVersionFromNamespaceUri(XMLEngineeringTorqueconverterWriterV10.NAMESPACE_URI));

			Bind<IXMLEngineeringAirdragWriter>().To<XMLEngineeringAirdragWriterV10>()
												.Named(XMLHelper.GetVersionFromNamespaceUri(XMLEngineeringAirdragWriterV10.NAMESPACE_URI));

			Bind<IXMLEngineeringDriverDataWriter>().To<XMLEngineeringDriverDataWriterV10>().Named(
				XMLHelper.GetVersionFromNamespaceUri(XMLEngineeringDriverDataWriterV10.NAMESPACE_URI));

			Bind<IXMLLookaheadDataWriter>().To<XMLEngineeringLookaheadDataWriterV10>().Named(
				XMLHelper.GetVersionFromNamespaceUri(XMLEngineeringLookaheadDataWriterV10.NAMESPACE_URI));

			Bind<IXMLOverspeedDataWriter>().To<XMLEngineeringOverspeedDataWriterV10>().Named(
				XMLHelper.GetVersionFromNamespaceUri(XMLEngineeringOverspeedDataWriterV10.NAMESPACE_URI));

			Bind<IXMLAccelerationDataWriter>().To<XMLAccelerationDataWriterV10>().Named(
				XMLHelper.GetVersionFromNamespaceUri(XMLAccelerationDataWriterV10.NAMESPACE_URI));

			Bind<IXMLGearshiftDataWriter>().To<XMLShiftParmeterDataWriterV10>().Named(
				XMLHelper.GetVersionFromNamespaceUri(XMLShiftParmeterDataWriterV10.NAMESPACE_URI));

			Bind<IXMLAuxiliariesWriter>().To<XMLEngineeringAuxiliariesWriterV10>().Named(
				XMLHelper.GetVersionFromNamespaceUri(XMLEngineeringAuxiliariesWriterV10.NAMESPACE_URI));

			Bind<IXMLAuxiliaryWriter>().To<XMLEngineeringAuxiliaryWriterV10>().Named(
				XMLHelper.GetVersionFromNamespaceUri(XMLEngineeringAuxiliaryWriterV10.NAMESPACE_URI));

			Bind<IXMLEngineeringADASWriter>().To<XMLEngineeringADASWriterV10>().Named(
				XMLHelper.GetVersionFromNamespaceUri(XMLEngineeringADASWriterV10.NAMESPACE_URI));
		}

		#endregion
	}
}
