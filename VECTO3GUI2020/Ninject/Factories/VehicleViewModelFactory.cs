using System;
using System.Reflection;
using System.Xml.Linq;
using Ninject.Extensions.Factory;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider.v24;
using TUGraz.VectoCore.Models.SimulationComponent;
using TUGraz.VectoCore.Utils;
using TUGraz.VectoCore.Utils.Ninject;
using VECTO3GUI2020.Annotations;
using VECTO3GUI2020.ViewModel.Interfaces.JobEdit.Vehicle;
using VECTO3GUI2020.ViewModel.MultiStage.Implementation;

namespace VECTO3GUI2020.Ninject.Factories
{

	public interface IVehicleViewModelFactory
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="consolidatedInputData">null if an exisiting step input should be used</param>
		/// <param name="stepInputData">null if a new step inputviewmodel should be created</param>
		/// <param name="exempted"></param>
		/// <returns></returns>
		IVehicleViewModel CreateVehicleViewModel(IVehicleDeclarationInputData consolidatedVehicleData, IVehicleDeclarationInputData vehicleInput);
	}

	public class VehicleViewModelFactoryModule : AbstractNinjectModule
	{
		#region Overrides of NinjectModule

		private const string scopeName = nameof(IVehicleViewModelFactory);
		public override void Load()
		{
			Bind<IVehicleViewModelFactory>().ToFactory(() => new CombineArgumentsToNameInstanceProvider(new[] {
				///From consolidated data
				new CombineArgumentsToNameInstanceProvider.MethodSettings() {
					combineToNameDelegate = (args) => {
						if (args.Length >= 2) {
							if (args[0] is IVehicleDeclarationInputData consolidatedVehicle) {
								return DefaultName(consolidatedVehicle.ExemptedVehicle);
							}

							if (args[1] is IVehicleDeclarationInputData vehicle) {
								return CombineToName(vehicle.DataSource.TypeVersion, vehicle.DataSource.Type);
							}
						}
						throw new ArgumentException($"arg[0] must be {nameof(IVehicleDeclarationInputData)}");
					},
					methods = new []{typeof(IVehicleViewModelFactory).GetMethod(nameof(IVehicleViewModelFactory.CreateVehicleViewModel))},
					takeArguments = 2,
					skipArguments = 0,
				},
			})).Named(scopeName);

			///Empty vehicle view models
			AddVehicleViewModelBinding<InterimStageBusVehicleViewModel_v2_8>(false);
			AddVehicleViewModelBinding<InterimStageBusVehicleViewModel_v2_8>(true);

			///Vehicle Viewmodels for existing files
			AddVehicleViewModelBinding<InterimStageBusVehicleViewModel_v2_8>(XMLDeclarationConventionalCompletedBusDataProviderV24.NAMESPACE_URI, XMLDeclarationConventionalCompletedBusDataProviderV24.XSD_TYPE);
			AddVehicleViewModelBinding<InterimStageBusVehicleViewModel_v2_8>(XMLDeclarationHevCompletedBusDataProviderV24.NAMESPACE_URI, XMLDeclarationHevCompletedBusDataProviderV24.XSD_TYPE);
			
			AddVehicleViewModelBinding<InterimStageBusVehicleViewModel_v2_8>(XMLDeclarationPEVCompletedBusDataProviderV24.NAMESPACE_URI, XMLDeclarationPEVCompletedBusDataProviderV24.XSD_TYPE);
			AddVehicleViewModelBinding<InterimStageBusVehicleViewModel_v2_8>(XMLDeclarationIepcCompletedBusDataProviderV24.NAMESPACE_URI, XMLDeclarationIepcCompletedBusDataProviderV24.XSD_TYPE);

            AddVehicleViewModelBinding<InterimStageBusVehicleViewModel_v2_8>(XMLDeclarationExemptedCompletedBusDataProviderV24.NAMESPACE_URI, XMLDeclarationExemptedCompletedBusDataProviderV24.XSD_TYPE); 

        }

        #endregion
		private void AddVehicleViewModelBinding<TConcrete>(XNamespace ns, string type) where TConcrete : IVehicleViewModel
		{
			Bind<IVehicleViewModel>().To<TConcrete>().WhenAnyAncestorNamed(scopeName).Named(CombineToName(ns, type));
		}

		private void AddVehicleViewModelBinding<TConcrete>(bool exempted) where TConcrete : IVehicleViewModel
		{
			Bind<IVehicleViewModel>().To<TConcrete>().WhenAnyAncestorNamed(scopeName).Named(DefaultName(exempted));
		}


		public static string DefaultName(bool exempted)
		{
			return exempted ? "exempted" : "default";
		}
        public static string CombineToName(XNamespace ns, string type)
		{
			return XMLHelper.CombineNamespace(ns, type);
		}
	}





}



