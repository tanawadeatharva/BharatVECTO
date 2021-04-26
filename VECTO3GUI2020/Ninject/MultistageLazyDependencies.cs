using System;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Factory;
using TUGraz.VectoCore.Utils;
using VECTO3GUI2020.Helper;
using VECTO3GUI2020.ViewModel.Interfaces.JobEdit.Vehicle.Components;

namespace VECTO3GUI2020.Ninject
{
	public class MultistageLazyDependencies : IMultistageDependencies
	{

		private readonly Lazy<IDialogHelper> _dialogHelper;
		public Lazy<IDialogHelper> DialogHelperLazy => _dialogHelper;
		public IDialogHelper DialogHelper => _dialogHelper.Value;


		private readonly Lazy<IXMLInputDataReader> _inputDataReader;
		public Lazy<IXMLInputDataReader> InputDataReaderLazy => _inputDataReader;
		public IXMLInputDataReader InputDataReader => _inputDataReader.Value;

		//private readonly Lazy<XMLValidator> _xmlValidator = new Lazy<XMLValidator>(() => {return });
		//public Lazy<XMLValidator> XMLValidatorLazy => _xmlValidator;
		//public XMLValidator XMLValidator => _xmlValidator.Value;


		public IDeclarationInjectFactory InjectFactory => _injectFactory.Value;
		public IComponentViewModelFactory ComponentViewModelFactory => _componentViewModelFactory.Value;


		private readonly Lazy<IDeclarationInjectFactory> _injectFactory;
		private readonly Lazy<IComponentViewModelFactory> _componentViewModelFactory;
		public MultistageLazyDependencies(Lazy<IDialogHelper> dialogHelper, Lazy<IXMLInputDataReader> inputDataReader, Lazy<IDeclarationInjectFactory> injectFactory, Lazy<IComponentViewModelFactory> componentViewModelFactory)
		{
			_dialogHelper = dialogHelper;
			_inputDataReader = inputDataReader;
			_componentViewModelFactory = componentViewModelFactory;
			_injectFactory = injectFactory;
		}
	}
}