using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.Components.DictionaryAdapter;
using Castle.Core.Internal;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Utils;
using VECTO3GUI2020.Helper;
using VECTO3GUI2020.ViewModel.Implementation.Common;
using VECTO3GUI2020.ViewModel.Interfaces;
using VECTO3GUI2020.ViewModel.Interfaces.Common;

namespace VECTO3GUI2020
{
    public class TestViewModel : ViewModelBase, IMainViewModel
    {
		private ConvertedSI _convertedSI;

		public ConvertedSI ConvertedSI
		{
			get => _convertedSI;
			set => SetProperty(ref _convertedSI, value);

		}

		private ConvertedSI _convertedSI1;

		public ConvertedSI ConvertedSI1
		{
			get => _convertedSI1;
			set => SetProperty(ref _convertedSI1, value);

		}


		private Meter _meter;
		//private HeatPumpMode? _heatpumpMode;
		private string _testString;

		public Meter Meter
		{
			get => _meter;
			set => SetProperty(ref _meter, value);
		}

		//public HeatPumpMode? HeatPumpMode
		//{
		//	get => _heatpumpMode;
		//	set => SetProperty(ref _heatpumpMode, value);
		//}

		//private IList<HeatPumpMode> allowedValues = new List<HeatPumpMode>(){
		//	TUGraz.VectoCommon.BusAuxiliaries.HeatPumpMode.cooling,
		//	TUGraz.VectoCommon.BusAuxiliaries.HeatPumpMode.heating_and_cooling
		//};

		//private HeatPumpMode? _heatpumpMode2;
		private bool _enabled;
		public bool enabled { get => _enabled; set => SetProperty(ref _enabled, value); }
		//public HeatPumpMode? HeatPumpMode2
		//{
		//	get => _heatpumpMode2;
		//	set => SetProperty(ref _heatpumpMode2, value);
		//}

		//public ObservableCollection<Enum> HeatPumpModeListItems
		//{
		//	get => new ObservableCollection<Enum>(allowedValues.Cast<Enum>());
		//}

		public String TestString
		{
			get => _testString;
			set
			{
				if (value.IsNullOrEmpty()) {
					throw new VectoEmptyFieldException();
				}
				SetProperty(ref _testString, value);
			}
		}


		public TestViewModel()
		{
			_enabled = true;

		}
    }
}
