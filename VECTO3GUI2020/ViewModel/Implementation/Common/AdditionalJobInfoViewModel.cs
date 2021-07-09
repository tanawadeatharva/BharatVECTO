using System;
using System.Collections.ObjectModel;
using System.Linq;
using TUGraz.VectoCommon.InputData;
using VECTO3GUI2020.ViewModel.Interfaces.Common;
using VECTO3GUI2020.ViewModel.MultiStage.Implementation;

namespace VECTO3GUI2020.ViewModel.Implementation.Common
{
	public interface IAdditionalJobInfoViewModel
	{
		void SetParent(IViewModelBase parent);
	}
	public class AdditionalJobInfoViewModelBase : ViewModelBase, IAdditionalJobInfoViewModel
	{
		private IViewModelBase _parent
			;

		#region Implementation of IAdditionalJobInfoViewModel

		public string Information { get; } = "Nothing to show here";





		public virtual void SetParent(IViewModelBase parent)
		{
			_parent = parent;
		}

		#endregion
	}

	public class AdditionalJobInfoViewModelMultiStage : AdditionalJobInfoViewModelBase
	{

		private IMultiStageJobViewModel _parent;
		private string _errorInfo;

		public AdditionalJobInfoViewModelMultiStage()
		{
			Title = "Multistage Job Info";
		}

		#region Overrides of AdditionalJobInfoViewModelBase

		public override void SetParent(IViewModelBase parent)
		{
			_parent = parent as IMultiStageJobViewModel;
			if (parent == null) {
				throw new ArgumentException(
					"this class should only be used to extend the functionality of a class that implements the IMultistageJobViewModel interface");
			}

			//Title += $"- {_parent.DataSource.}"
			if (_parent.CanBeSimulated) {
				return;
			}
			
			if(_parent.JobInputData?.ConsolidateManufacturingStage?.Vehicle?.VehicleDeclarationType !=
				VehicleDeclarationType.final && !_parent.Exempted)
			{
				ErrorInfo = "Job is not declared as \"final\"";
			}

			
			if (_parent.InvalidEntries != null && _parent.InvalidEntries.Count != 0) {
				ErrorInfo = "This Job cannot be Simulated! The following inputs are missing or invalid";
				foreach (var parentInvalidEntry in _parent.InvalidEntries)
				{
					InvalidEntries.Add(parentInvalidEntry);
				}
			}
		}

		public ObservableCollection<string> InvalidEntries { get; set; } = new ObservableCollection<string>();

		public string ErrorInfo
		{
			get => _errorInfo;
			set => SetProperty(ref _errorInfo, value);
		}

		#endregion
	}
}