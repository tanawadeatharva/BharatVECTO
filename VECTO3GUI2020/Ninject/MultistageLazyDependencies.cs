using System;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Factory;
using TUGraz.VectoCore.Utils;
using VECTO3GUI2020.Helper;
using VECTO3GUI2020.Util.XML;
using VECTO3GUI2020.ViewModel.Interfaces.JobEdit.Vehicle.Components;
using VECTO3GUI2020.ViewModel.MultiStage.Interfaces;

namespace VECTO3GUI2020.Ninject
{
    public class MultistageLazyDependencies : IMultistageDependencies
	{
		private readonly Lazy<IXMLWriterFactory> _xmlWriterFactory;
		private readonly Lazy<IDeclarationInjectFactory> _injectFactory;
		private readonly Lazy<IComponentViewModelFactory> _componentViewModelFactory;
		private readonly Lazy<IMultiStageViewModelFactory> _multistageViewModelFactory;
		private readonly Lazy<IDialogHelper> _dialogHelper;
		private readonly Lazy<IXMLComponentInputReader> _componentInputReader;
		public Lazy<IMultiStageViewModelFactory> MultistageViewModelFactory => _multistageViewModelFactory;
		public Lazy<IDialogHelper> DialogHelperLazy => _dialogHelper;
		public IDialogHelper DialogHelper => _dialogHelper.Value;
		public IDeclarationInjectFactory InjectFactory => _injectFactory.Value;
		public IComponentViewModelFactory ComponentViewModelFactory => _componentViewModelFactory.Value;
		public IXMLComponentInputReader ComponentInputReader => _componentInputReader.Value;
		public IXMLWriterFactory XMLWriterFactory => _xmlWriterFactory.Value;

		public MultistageLazyDependencies(
			Lazy<IDialogHelper> dialogHelper,
			Lazy<IDeclarationInjectFactory> injectFactory, 
			Lazy<IComponentViewModelFactory> componentViewModelFactory,
			Lazy<IXMLWriterFactory> xmlWriterFactory,
			Lazy<IMultiStageViewModelFactory> viewModelFactory,
			Lazy<IXMLComponentInputReader> componentInputReader)
		{
			_dialogHelper = dialogHelper;
			_componentViewModelFactory = componentViewModelFactory;
			_injectFactory = injectFactory;
			_xmlWriterFactory = xmlWriterFactory;
			_multistageViewModelFactory = viewModelFactory;
			_componentInputReader = componentInputReader;
		}
	}
}