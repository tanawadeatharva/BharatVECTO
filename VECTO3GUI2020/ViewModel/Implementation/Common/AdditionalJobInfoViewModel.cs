using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Media.TextFormatting;
using TUGraz.VectoCommon.InputData;
using VECTO3GUI2020.ViewModel.Interfaces.Common;
using VECTO3GUI2020.ViewModel.MultiStage.Implementation;

namespace VECTO3GUI2020.ViewModel.Implementation.Common
{
	public interface IAdditionalJobInfoViewModel
	{
		void SetParent(IViewModelBase parent);
	}
	public abstract class AdditionalJobInfoViewModelBase : ViewModelBase, IAdditionalJobInfoViewModel
	{

		#region Implementation of IAdditionalJobInfoViewModel

		public string Information { get; } = "Nothing to show here";

		public abstract void SetParent(IViewModelBase parent);


		public AdditionalJobInfoViewModelBase()
		{
			SizeToContent = SizeToContent.WidthAndHeight;
			MinHeight = 200;
			MinWidth = 300;
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

	public class AdditionalJobInfoViewModelNewVif : AdditionalJobInfoViewModelBase
	{
		private CreateVifViewModel _parent;
		public ObservableCollection<string> InvalidEntries { get; set; } = new ObservableCollection<string>();

		#region Overrides of AdditionalJobInfoViewModelBase

		public override void SetParent(IViewModelBase parent)
		{
			_parent = parent as CreateVifViewModel;
            (_parent as INotifyPropertyChanged).PropertyChanged += AdditionalJobInfoViewModelNewVif_PropertyChanged;
			Debug.Assert(_parent != null);
		}

        private void AdditionalJobInfoViewModelNewVif_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
			if (e.PropertyName == nameof(_parent.CanBeSimulated)) {



			}
		}



        #endregion
    }

	public class AdditionalJobInfoViewModelStageInput : AdditionalJobInfoViewModelBase
	{
		private IStageViewModelBase stageViewModel;
		#region Overrides of AdditionalJobInfoViewModelBase

		public override void SetParent(IViewModelBase parent)
		{
			stageViewModel = parent as IStageViewModelBase;
			Debug.Assert(stageViewModel != null);
		}
		#endregion
	}
}