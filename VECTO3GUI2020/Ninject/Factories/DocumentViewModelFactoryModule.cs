using System;
using Ninject.Extensions.Factory;
using Ninject.Modules;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider.v24;
using TUGraz.VectoCore.Utils;
using TUGraz.VectoCore.Utils.Ninject;
using VECTO3GUI2020.Helper;
using VECTO3GUI2020.ViewModel.Implementation.Common;
using VECTO3GUI2020.ViewModel.Implementation.Document;
using VECTO3GUI2020.ViewModel.Interfaces.Document;
using VECTO3GUI2020.ViewModel.MultiStage.Implementation;
using VECTO3GUI2020.ViewModel.MultiStage.Interfaces;

namespace VECTO3GUI2020.Ninject.Factories
{
    public class DocumentViewModelFactoryModule : NinjectModule
    {
        #region Overrides of NinjectModule

        public const string DocumentViewModelFactoryScope = nameof(IDocumentViewModelFactory);
        public const string MultistepInputScope = nameof(IMultiStepInputViewModelFactory);
        public const string DeclarationViewModelScope = nameof(IDeclarationInputViewModelFactory);

        public const string NewDocumentViewModelScope = nameof(INewDocumentViewModelFactory);


        public const string PrimaryAndStageInputScope = nameof(IPrimaryAndStageInputViewModelFactory);

        public override void Load()
        {
            Bind<IDocumentViewModelFactory>().To<DocumentViewModelFactory>().Named(DocumentViewModelFactoryScope);

            #region MultistepViewModel

            Bind<IMultiStepInputViewModelFactory>().ToFactory().Named(MultistepInputScope);
            Bind<IDocumentViewModel>().To<MultiStageJobViewModel_v0_1>().WhenParentNamed(MultistepInputScope);
            #endregion

            #region DeclarationInputData

            Bind<IDeclarationInputViewModelFactory>().ToFactory(() => new CombineArgumentsToNameInstanceProvider(true,
                new[] {
                    new CombineArgumentsToNameInstanceProvider.MethodSettings() {
                        methods = new []{typeof(IDeclarationInputViewModelFactory).GetMethod(nameof(IDeclarationInputViewModelFactory.CreateDeclarationViewModel))},
                        takeArguments = 1,
                        combineToNameDelegate = (arguments) => {
                            if (arguments == null || arguments.Length < 1) {
                                throw new ArgumentException("Invalid number of arguments");
                            }

                            if (arguments[0] is IDeclarationInputDataProvider inputData) {
                                var name =  $"{inputData.JobInputData.Vehicle.DataSource.SourceVersion}:{inputData.JobInputData.Vehicle.DataSource.Type}";
                                return name;
                            }

                            throw new ArgumentException($"Invalid argument of type {arguments[0].GetType()}");
                        }
                    },
                }
            )
            ).Named(DeclarationViewModelScope);
            Bind<IDocumentViewModel>().To<SimulationOnlyDeclarationJob>().WhenParentNamed(DeclarationViewModelScope);

            var StageInputXMLVersions = new[] {
                new { XMLDeclarationConventionalCompletedBusDataProviderV24.NAMESPACE_URI, XMLDeclarationConventionalCompletedBusDataProviderV24.XSD_TYPE},
                new { AbstractXMLDeclarationCompletedBusDataProviderV24.NAMESPACE_URI, XMLDeclarationHevCompletedBusDataProviderV24.XSD_TYPE},
                new { AbstractXMLDeclarationCompletedBusDataProviderV24.NAMESPACE_URI, XMLDeclarationPEVCompletedBusDataProviderV24.XSD_TYPE},
                new { XMLDeclarationExemptedCompletedBusDataProviderV24.NAMESPACE_URI, XMLDeclarationExemptedCompletedBusDataProviderV24.XSD_TYPE},
                new { AbstractXMLDeclarationCompletedBusDataProviderV24.NAMESPACE_URI, XMLDeclarationIepcCompletedBusDataProviderV24.XSD_TYPE},
                };
            foreach (var version in StageInputXMLVersions)
            {
                var name = $"{version.NAMESPACE_URI.GetVersionFromNamespaceUri()}:{version.XSD_TYPE}";

                Bind<IDocumentViewModel>().To<StageInputViewModel>().WhenParentNamed(DeclarationViewModelScope)
                    .Named(name);
            }
            #endregion


            #region NewViewModels

            Bind<INewDocumentViewModelFactory>().ToFactory().Named(NewDocumentViewModelScope);
            Bind<IDocumentViewModel>().ToConstructor(
                constructorArgs => new StageInputViewModel(
                    constructorArgs.Inject<bool>(),
                    constructorArgs.Inject<IMultiStageViewModelFactory>(),
                    constructorArgs.Inject<IAdditionalJobInfoViewModel>()))
                .WhenParentNamed(NewDocumentViewModelScope)
                .NamedLikeFactoryMethod((INewDocumentViewModelFactory f) => f.GetCreateNewStepInputViewModel(false)); ;

            Bind<IDocumentViewModel>().ToConstructor(
                constructorArgs => new CreateVifViewModel(
                    constructorArgs.Inject<bool>(),
                    constructorArgs.Inject<IDialogHelper>(),
                    constructorArgs.Inject<IXMLInputDataReader>(),
                    constructorArgs.Inject<IAdditionalJobInfoViewModel>()))
                .WhenParentNamed(NewDocumentViewModelScope)
                .NamedLikeFactoryMethod((INewDocumentViewModelFactory f) => f.GetCreateNewVifViewModel(false));

            #endregion

            Bind<IPrimaryAndStageInputViewModelFactory>().ToFactory().Named(PrimaryAndStageInputScope);
            Bind<IDocumentViewModel>().To<CreateVifViewModel>().WhenParentNamed(PrimaryAndStageInputScope);
        }
        #endregion
    }
}
