using System;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Factory;
using TUGraz.VectoCore.Utils;
using VECTO3GUI2020.Helper;
using VECTO3GUI2020.Util.XML.Interfaces;
using VECTO3GUI2020.ViewModel.Interfaces.JobEdit.Vehicle.Components;
using VECTO3GUI2020.ViewModel.MultiStage.Interfaces;

namespace VECTO3GUI2020.Ninject
{
	public class MultistageLazyDependencies : IMultistageDependencies
	{

		private readonly Lazy<IDialogHelper> _dialogHelper;
		public Lazy<IDialogHelper> DialogHelperLazy => _dialogHelper;
		public IDialogHelper DialogHelper => _dialogHelper.Value;


		//private readonly Lazy<XMLValidator> _xmlValidator = new Lazy<XMLValidator>(() => {return });
		//public Lazy<XMLValidator> XMLValidatorLazy => _xmlValidator;
		//public XMLValidator XMLValidator => _xmlValidator.Value;


		public IDeclarationInjectFactory InjectFactory => _injectFactory.Value;
		public IComponentViewModelFactory ComponentViewModelFactory => _componentViewModelFactory.Value;
		public IXMLWriterFactory XMLWriterFactory => _xmlWriterFactory.Value;

		private Lazy<IXMLWriterFactory> _xmlWriterFactory;


		private readonly Lazy<IDeclarationInjectFactory> _injectFactory;
		private readonly Lazy<IComponentViewModelFactory> _componentViewModelFactory;
		public MultistageLazyDependencies(
			Lazy<IDialogHelper> dialogHelper,
			Lazy<IDeclarationInjectFactory> injectFactory, 
			Lazy<IComponentViewModelFactory> componentViewModelFactory,
			Lazy<IXMLWriterFactory> xmlWriterFactory,
			Lazy<IMultiStageViewModelFactory> viewModelFactory)
		{
			_dialogHelper = dialogHelper;
			_componentViewModelFactory = componentViewModelFactory;
			_injectFactory = injectFactory;
			_xmlWriterFactory = xmlWriterFactory;

		}
	}
}