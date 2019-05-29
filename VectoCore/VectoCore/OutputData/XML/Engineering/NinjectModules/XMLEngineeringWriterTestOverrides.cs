using Ninject.Modules;
using TUGraz.VectoCore.OutputData.XML.Engineering.Interfaces;
using TUGraz.VectoCore.OutputData.XML.Engineering.Writer;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.OutputData.XML.NinjectModules
{
	public class XMLEngineeringWriterTestOverrides : NinjectModule
	{
		#region Overrides of NinjectModule

		public override void Load()
		{
			// testing derived xml data types

			Bind<IXMLEngineeringAxleWriter>().To<XMLEngineeringAxleWriterV10TEST>().Named(
				XMLHelper.GetVersionFromNamespaceUri(XMLEngineeringAxleWriterV10TEST.NAMESPACE_URI));

			Bind<IXMLEngineeringEngineWriter>().To<XMLEngineeringEngineWriterV10TEST>().Named(
				XMLHelper.GetVersionFromNamespaceUri(XMLEngineeringEngineWriterV10TEST.NAMESPACE_URI));

		}

		#endregion
	}
}
