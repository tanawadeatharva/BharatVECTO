using System;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Factory;
using TUGraz.VectoCore.Utils;
using VECTO3GUI2020.Helper;
using VECTO3GUI2020.ViewModel.Interfaces.JobEdit.Vehicle.Components;

namespace VECTO3GUI2020.Ninject
{
	public interface IMultistageDependencies
	{
		Lazy<IDialogHelper> DialogHelperLazy { get; }
		Lazy<IXMLInputDataReader> InputDataReaderLazy { get; }

		IDialogHelper DialogHelper { get; }
		IXMLInputDataReader InputDataReader { get; }
		IDeclarationInjectFactory InjectFactory { get; }
		IComponentViewModelFactory ComponentViewModelFactory { get; }
	}
}