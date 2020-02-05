using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ninject.Modules;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Reader;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Reader.Impl;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration.NinjectModules
{
	public class XMLDeclarationPrimaryVehicleBusV01InjectModule : NinjectModule
	{
		#region Overrides of NinjectModule
		
		public override void Load()
		{

			Bind<IXMLPrimaryVehicleBusInputData>().To<XMLPrimaryVehicleBusInputDataV01>()
				.Named(XMLPrimaryVehicleBusInputDataV01.QUALIFIED_XSD_TYPE);

			Bind<IXMLPrimaryVehicleBusJobInputData>().To<XMLDeclarationPrimaryVehicleBusJobInputDataProviderV01>()
				.Named(XMLDeclarationPrimaryVehicleBusJobInputDataProviderV01.QUALIFIED_XSD_TYPE);

			Bind<IXMLJobDataReader>().To<XMLJobDataPrimaryVehicleReaderV01>()
				.Named(XMLJobDataPrimaryVehicleReaderV01.QUALIFIED_XSD_TYPE);
			
			Bind<IXMLDeclarationVehicleData>().To<XMLDeclarationPrimaryVehicleBusDataProviderV01>()
				.Named(XMLDeclarationPrimaryVehicleBusDataProviderV01.QUALIFIED_XSD_TYPE);
			
			Bind<IXMLDeclarationPrimaryVehicleBusInputDataReader>().To<XMLPrimaryVehicleBusInputReaderV01>()
				.Named(XMLPrimaryVehicleBusInputReaderV01.QUALIFIED_XSD_TYPE);
			
			Bind<IXMLComponentReader>().To<XMLPrimaryVehicleBusComponentReaderV01>()
				.Named(XMLPrimaryVehicleBusComponentReaderV01.QUALIFIED_XSD_TYPE);
			
			Bind<IXMLVehicleComponentsDeclaration>().To<XMLDeclarationComponentsPrimaryVehicleBusDataProviderV01>()
				.Named(XMLDeclarationComponentsPrimaryVehicleBusDataProviderV01.QUALIFIED_XSD_TYPE);

			Bind<IXMLEngineDeclarationInputData>().To<XMLDeclarationPrimaryVehicleBusEngineDataProviderV01>()
				.Named(XMLDeclarationPrimaryVehicleBusEngineDataProviderV01.QUALIFIED_XSD_TYPE);
			
			Bind<IXMLGearboxDeclarationInputData>().To<XMLDeclarationPrimaryVehicleBusGearboxDataProviderV01>()
				.Named(XMLDeclarationPrimaryVehicleBusGearboxDataProviderV01.QUALIFIED_XSD_TYPE);

			Bind<IXMLGearboxReader>().To<XMLPrimaryVehicleBusComponentReaderV01>()
				.Named(XMLPrimaryVehicleBusComponentReaderV01.GEARBOX_READER_QUALIFIED_XSD_TYPE);

			Bind<IXMLGearData>().To<XMLPrimaryVehicleBusTransmissionDataV01>()
				.Named(XMLPrimaryVehicleBusTransmissionDataV01.QUALIFIED_XSD_TYPE);
			
			Bind<IXMLAngledriveInputData>().To<XMLDeclarationPrimaryVehicleBusAngledriveDataProviderV01>()
				.Named(XMLDeclarationPrimaryVehicleBusAngledriveDataProviderV01.QUALIFIED_XSD_TYPE);

			Bind<IXMLAxleGearInputData>().To<XMLDeclarationPrimaryVehicleBusAxlegearDataProviderV01>().Named(
				XMLDeclarationPrimaryVehicleBusAxlegearDataProviderV01.QUALIFIED_XSD_TYPE);

			Bind<IXMLApplicationInformationData>().To<XMLDeclarationPrimaryVehicleBusApplicationInformationDataProviderV01>()
				.Named(XMLDeclarationPrimaryVehicleBusApplicationInformationDataProviderV01.QUALIFIED_XSD_TYPE);

			Bind<IXMLResultsInputData>().To<XMLDeclarationPrimaryVehicleBusResultsInputDataProviderV01>()
				.Named(XMLDeclarationPrimaryVehicleBusResultsInputDataProviderV01.QUALIFIED_XSD_TYPE);


		}

		#endregion

	}
}
