using Ninject.Modules;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Reader;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Reader.Impl;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration.NinjectModules
{
	public class XMLDeclarationInputDataV21InjectModule : NinjectModule
	{
		#region Overrides of NinjectModule

		public override void Load()
		{
			Bind<IXMLDeclarationVehicleData>().To<XMLDeclarationVehicleDataProviderV21>().Named(
				XMLDeclarationVehicleDataProviderV21.QUALIFIED_XSD_TYPE);

			Bind<IXMLDeclarationVehicleData>().To<XMLDeclarationExemptedVehicleDataProviderV22>()
											.Named(XMLDeclarationExemptedVehicleDataProviderV22.QUALIFIED_XSD_TYPE);

			Bind<IXMLEngineDeclarationInputData>().To<XMLDeclarationEngineDataProviderV21>().Named(
				XMLDeclarationEngineDataProviderV21.QUALIFIED_XSD_TYPE);

			Bind<IXMLAdvancedDriverAssistantSystemDeclarationInputData>()
				.To<XMLDeclarationADASDataProviderV21>().Named(
					XMLDeclarationADASDataProviderV21.QUALIFIED_XSD_TYPE);

			// ---------------------------------------------------------------------------------------

			Bind<IXMLJobDataReader>().To<XMLJobDataReaderV21>()
									.Named(XMLJobDataReaderV21.QUALIFIED_XSD_TYPE);

			Bind<IXMLADASReader>().To<XMLADASReaderV21>()
								.Named(XMLADASReaderV21.QUALIFIED_XSD_TYPE);
		}

		#endregion
	}
}
