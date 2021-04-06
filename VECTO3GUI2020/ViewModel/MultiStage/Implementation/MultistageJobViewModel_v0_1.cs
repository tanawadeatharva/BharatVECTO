using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider;
using TUGraz.VectoCore.Utils;
using VECTO3GUI2020.ViewModel.Implementation.Common;
using VECTO3GUI2020.ViewModel.Interfaces;
using VECTO3GUI2020.ViewModel.Interfaces.Common;
using VECTO3GUI2020.ViewModel.Interfaces.Document;
using VECTO3GUI2020.ViewModel.Interfaces.JobEdit.Vehicle;
using VECTO3GUI2020.ViewModel.MultiStage.Interfaces;

namespace VECTO3GUI2020.ViewModel.MultiStage.Implementation
{
	public class MultiStageJobViewModel_v0_1 : ViewModelBase, IMultiStageJobViewModel 
	{
		#region QualifiedXSD

		public static readonly string QUALIFIED_XSD_TYPE = XMLDeclarationInputDataProviderMultistageV01.QUALIFIED_XSD_TYPE;

		public static readonly string INPUTPROVIDERTYPE =
			typeof(XMLDeclarationInputDataProviderMultistageV01).ToString();

		private IDeclarationMultistageJobInputData _jobInputData;
		#endregion

		private IManufacturingStageViewModel _manufacturingStageViewModel;
		public IManufacturingStageViewModel ManufacturingStageViewModel
		{
			get => _manufacturingStageViewModel;
			set => SetProperty(ref _manufacturingStageViewModel, value);
		}

		private IMultiStageViewModelFactory _vmFactory;


		public MultiStageJobViewModel_v0_1(IMultistageBusInputDataProvider inputData, IMultiStageViewModelFactory vmFactory)
		{
			_jobInputData = inputData.JobInputData;
			_vmFactory = vmFactory;


			
			var prevStageInputData = _jobInputData.ManufacturingStages.Last();

			_manufacturingStageViewModel =
				vmFactory.CreateManufacturingStageViewModel(prevStageInputData.GetType().ToString(),
					prevStageInputData);

			_jobInputData.ManufacturingStages.Add(_manufacturingStageViewModel as IManufacturingStageInputData);
			
		}



		public IPrimaryVehicleInformationInputDataProvider PrimaryVehicle => throw new NotImplementedException();

		public IList<IManufacturingStageInputData> ManufacturingStages => throw new NotImplementedException();
	}

	public interface IMultiStageJobViewModel : IDeclarationMultistageJobInputData
	{
		IManufacturingStageViewModel ManufacturingStageViewModel { get; }
	}
}