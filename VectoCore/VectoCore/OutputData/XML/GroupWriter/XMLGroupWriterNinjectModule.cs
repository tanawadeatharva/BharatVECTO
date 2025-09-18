using System.Xml.Linq;
using Ninject.Extensions.Factory;
using TUGraz.VectoCore.OutputData.XML.GroupWriter.Declaration;
using TUGraz.VectoCore.OutputData.XML.GroupWriter.Declaration.Vehicle.CompletedBus;
using TUGraz.VectoCore.OutputData.XML.GroupWriter.Declaration.Vehicle.Components.Auxiliaries;
using TUGraz.VectoCore.Utils.Ninject;

namespace TUGraz.VectoCore.OutputData.XML.GroupWriter
{
	public class GroupWriterNinjectModule : AbstractNinjectModule
	{
		#region Overrides of NinjectModule

		private string GetName(string groupName, XNamespace xNamespace)
		{
			return UseFirstTwoArgumentsAsInstanceProvider.GetName(groupName, xNamespace.ToString());
		}
		public override void Load()
		{
			Bind<IGroupWriterFactory>().ToFactory(() => new UseFirstTwoArgumentsAsInstanceProvider(1, false)).InSingletonScope();

			///Vehicle V2_4	
			var v24 = XMLDeclarationNamespaces.V24;
			Bind<IVehicleDeclarationGroupWriter>().To<CompletedBusGeneralParametersWriterV2_4>().InSingletonScope().
				Named(GetName(GroupNames.Vehicle_CompletedBus_GeneralParametersSequenceGroup, v24));
			Bind<IVehicleDeclarationGroupWriter>().To<CompletedBusParametersWriterV2_4>().InSingletonScope().
				Named(GetName(GroupNames.Vehicle_CompletedBusParametersSequenceGroup, v24));
			Bind<IVehicleDeclarationGroupWriter>().To<CompletedBusDimensionsWriter_V2_4>().InSingletonScope().
				Named(GetName(GroupNames.Vehicle_CompletedBus_DimensionsSequenceGroup, v24));
			Bind<IVehicleDeclarationGroupWriter>().To<CompletedBusPassengerCountWriter_V2_4>().InSingletonScope().
				Named(GetName(GroupNames.Vehicle_CompletedBus_PassengerCountSequenceGroup, v24));

			///BusAuxiliaries_V2_4
			Bind<IBusAuxiliariesDeclarationGroupWriter>().To<BusAuxElectricSystemLightsGroupWriter_v2_4>().InSingletonScope().
				Named(GetName(GroupNames.BusAuxElectricSystemLightsGroup, v24));
			Bind<IBusAuxiliariesDeclarationGroupWriter>().To<BusAuxElectricSystemSupplyGroupWriter_v2_4>().InSingletonScope()
				.Named(GetName(GroupNames.BusAuxElectricSystemSupplySequenceGroup, v24));
			Bind<IBusAuxiliariesDeclarationGroupWriter>().To<BusAuxHVACConventionalGroupWriter_v2_4>().InSingletonScope()
				.Named(GetName(GroupNames.BusAuxHVACConventionalSequenceGroup, v24));
			Bind<IBusAuxiliariesDeclarationGroupWriter>().To<BusAuxHVACxEVGroupWriter_v2_4>().InSingletonScope()
				.Named(GetName(GroupNames.BusAuxHVACxEVSequenceGroup, v24));
			Bind<IBusAuxiliariesDeclarationGroupWriter>().To<BusAuxHVACHeatPumpWriter_v2_4>().InSingletonScope()
				.Named(GetName(GroupNames.BusAuxHVACHeatPumpSequenceGroup, v24));


		}

		#endregion
	}
}