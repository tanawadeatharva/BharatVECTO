using System;
using System.Xml.Linq;
using Ninject.Extensions.Factory;
using Ninject.Modules;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.Utils;
using TUGraz.VectoCore.Utils.Ninject;
using VECTO3GUI2020.Ninject.Util;
using VECTO3GUI2020.Util.XML.Implementation;
using VECTO3GUI2020.Util.XML.Implementation.ComponentWriter;
using VECTO3GUI2020.Util.XML.Implementation.DocumentWriter;
using VECTO3GUI2020.Util.XML.Interfaces;

namespace VECTO3GUI2020.Ninject.Factories
{
    public class XMLWriterFactoryModule : NinjectModule
	{
		private const string scope = nameof(IXMLWriterFactoryInternal);
        public override void Load()
		{
			Bind<IXMLWriterFactory>().To<XMLWriterFactory>();
			Bind<IXMLWriterFactoryInternal>().ToFactory(() => new CombineArgumentsToNameInstanceProvider(false, new [] {
				new CombineArgumentsToNameInstanceProvider.MethodSettings() {
					combineToNameDelegate = ((args) => {
						if (args[0] is DataSource dataSource) {
							return GetName(dataSource.TypeVersion, dataSource.Type);
						}
						throw new ArgumentException();
					}),
					methods = new []{typeof(IXMLWriterFactoryInternal).GetMethod(nameof(IXMLWriterFactoryInternal.CreateWriter))},
					skipArguments = 1,
					takeArguments = 1,
				}
			})).Named(scope);

			//Array.ForEach(
			//    XMLVehicleWriter_v2_10.SUPPORTEDVERSIONS,
			//    sv =>
			//        Bind<IXMLVehicleWriter>().To<XMLVehicleWriter_v2_10>().Named(sv));
			Array.ForEach(XMLVehicleWriter_v2_10.SUPPORTEDVERSIONS,
				sv => Bind<IXMLVehicleWriter>().To<XMLVehicleWriter_v2_10>().WhenParentNamed(scope).Named(GetName(sv.version, sv.type)));



			#region

			//Array.ForEach(
			//    XMLDeclarationJobWriter_v1_0.SUPPORTED_VERSIONS,
			//    sv =>
			//        Bind<IXMLDeclarationJobWriter>().To<XMLDeclarationJobWriter_v1_0>().Named(sv));

			//Array.ForEach(
			//    XMLDeclarationJobWriter_v2_0.SUPPORTED_VERSIONS,
			//    sv =>
			//        Bind<IXMLDeclarationJobWriter>().To<XMLDeclarationJobWriter_v2_0>().Named(sv));

			//Array.ForEach(
			//    XMLPTOWriter_v1_0.SUPPORTED_VERSIONS,
			//    sv => Bind<IXMLComponentWriter>().To<XMLPTOWriter_v1_0>().Named(sv));

			//Array.ForEach(
			//    XMLPTOWriter_v2_0.SUPPORTED_VERSIONS,
			//    sv => Bind<IXMLComponentWriter>().To<XMLPTOWriter_v2_0>().Named(sv));

			//Array.ForEach(
			//    XMLVehicleWriter_v1_0.SUPPORTEDVERSIONS,
			//    (sv) =>
			//        Bind<IXMLVehicleWriter>().To<XMLVehicleWriter_v1_0>().Named(sv));

			//Array.ForEach(
			//    XMLVehicleWriter_v2_0.SUPPORTEDVERSIONS,
			//    sv =>
			//        Bind<IXMLVehicleWriter>().To<XMLVehicleWriter_v2_0>().Named(sv));


			//Array.ForEach(
			//    XMLComponentsWriter_v1_0.SUPPORTED_VERSIONS,
			//    sv => Bind<IXMLComponentsWriter>().To<XMLComponentsWriter_v1_0>().Named(sv));

			//Array.ForEach(
			//    XMLComponentsWriter_v2_0.SUPPORTED_VERSIONS,
			//    sv => Bind<IXMLComponentsWriter>().To<XMLComponentsWriter_v2_0>().Named(sv));

			//Array.ForEach(
			//    XMLEngineWriter_v1_0.SUPPORTED_VERSIONS,
			//    sv => Bind<IXMLComponentWriter>().To<XMLEngineWriter_v1_0>().Named(sv));

			//Array.ForEach(
			//    XMLEngineWriter_v2_0.SUPPORTED_VERSIONS,
			//    sv => Bind<IXMLComponentWriter>().To<XMLEngineWriter_v2_0>().Named(sv));

			//Array.ForEach(
			//    XMLGearboxWriter_v2_0.SUPPORTED_VERSIONS,
			//    sv => Bind<IXMLComponentWriter>().To<XMLGearboxWriter_v2_0>().Named(sv));

			//Array.ForEach(
			//    XMLRetarderWriter_v2_0.SUPPORTED_VERSIONS,
			//    sv => Bind<IXMLComponentWriter>().To<XMLRetarderWriter_v2_0>().Named(sv));

			//Array.ForEach(
			//    XMLAxleGearWriter_v2_0.SUPPORTED_VERSIONS,
			//    sv => Bind<IXMLComponentWriter>().To<XMLAxleGearWriter_v2_0>().Named(sv));

			//Array.ForEach(
			//    XMLAxleWheelsWriter_v2_0.SUPPORTED_VERSIONS,
			//    sv => Bind<IXMLComponentWriter>().To<XMLAxleWheelsWriter_v2_0>().Named(sv));

			//Array.ForEach(
			//    XMLAxleWheelWriter_v2_0.SUPPORTED_VERSIONS,
			//    sv => Bind<IXMLComponentWriter>().To<XMLAxleWheelWriter_v2_0>().Named(sv));

			//Array.ForEach(
			//    XMLTyreWriter_v2_0.SUPPORTED_VERSIONS,
			//    sv => Bind<IXMLComponentWriter>().To<XMLTyreWriter_v2_0>().Named(sv));

			//Array.ForEach(
			//    XMLTyreWriter_v2_3.SUPPORTED_VERSIONS,
			//    sv => Bind<IXMLComponentWriter>().To<XMLTyreWriter_v2_3>().Named(sv));

			//Array.ForEach(
			//    XMLAuxiliariesWriter_v2_0.SUPPORTED_VERSIONS,
			//    sv => Bind<IXMLComponentWriter>().To<XMLAuxiliariesWriter_v2_0>().Named(sv));
			//Array.ForEach(
			//    XMLAirDragWriter_v2_0.SUPPORTED_VERSIONS,
			//    sv => Bind<IXMLComponentWriter>().To<XMLAirDragWriter_v2_0>().Named(sv));

			//Bind<IXMLBusAuxiliariesWriter>().To<XMLBusAuxiliariesWriterMultistage>();

			#endregion

		}

        public string GetName(XNamespace version, string xsdType)
		{
			return XMLHelper.CombineNamespace(version, xsdType);
		}
	}
}
