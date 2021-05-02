using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
		private HeatPumpMode? _heatpumpMode;
		private string _testString;

		public Meter Meter
		{
			get => _meter;
			set => SetProperty(ref _meter, value);
		}

		public HeatPumpMode? HeatPumpMode
		{
			get => _heatpumpMode;
			set => SetProperty(ref _heatpumpMode, value);
		}

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
			
		}
    }
}
