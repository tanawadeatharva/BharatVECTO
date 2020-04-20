using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Utils;
using VECTO3GUI.ViewModel.Interfaces;

namespace VECTO3GUI.Model.TempDataObject
{
	public class AuxiliariesBusComponentData: IAuxiliariesBus, ITempDataObject<IAuxiliariesViewModel>
	{
		#region IAuxiliariesBus Interface

		public ObservableCollection<string> AlternatorTechnologies { get; set; }
		public bool DayrunninglightsLED { get; set; }
		public bool HeadlightsLED { get; set; }
		public bool PositionlightsLED { get; set; }
		public bool BrakelightsLED { get; set; }
		public bool InteriorLightsLED { get; set; }
		public ConsumerTechnology DoorDriveTechnology { get; set; }
		public BusHVACSystemConfiguration SystemConfiguration { get; set; }
		public ACCompressorType CompressorTypeDriver { get; set; }
		public ACCompressorType CompressorTypePassenger { get; set; }
		public Watt AuxHeaterPower { get; set; }
		public bool DoubleGlasing { get; set; }
		public bool HeatPump { get; set; }
		public bool AdjustableAuxiliaryHeater { get; set; }
		public bool SeparateAirDistributionDucts { get; set; }

		#endregion

		public AuxiliariesBusComponentData(IAuxiliariesViewModel viewModel, bool defaultValues)
		{
			if (defaultValues)
				ClearValues(viewModel);
		}

		public AuxiliariesBusComponentData(IAuxiliariesViewModel viewModel)
		{
			SetValues(viewModel);
		}

		public void UpdateCurrentValues(IAuxiliariesViewModel viewModel)
		{
			SetValues(viewModel);
		}

		public void ResetToComponentValues(IAuxiliariesViewModel viewModel)
		{
			viewModel.AlternatorTechnologies = AlternatorTechnologies;
			viewModel.DayrunninglightsLED = DayrunninglightsLED;
			viewModel.HeadlightsLED = HeadlightsLED;
			viewModel.PositionlightsLED = PositionlightsLED;
			viewModel.BrakelightsLED = BrakelightsLED;
			viewModel.InteriorLightsLED = InteriorLightsLED;
			viewModel.DoorDriveTechnology = DoorDriveTechnology;
			viewModel.SystemConfiguration = SystemConfiguration;
			viewModel.CompressorTypeDriver = CompressorTypeDriver;
			viewModel.CompressorTypePassenger = CompressorTypePassenger;
			viewModel.AuxHeaterPower = AuxHeaterPower;
			viewModel.DoubleGlasing = DoubleGlasing;
			viewModel.HeatPump = HeatPump;
			viewModel.AdjustableAuxiliaryHeater = AdjustableAuxiliaryHeater;
			viewModel.SeparateAirDistributionDucts = SeparateAirDistributionDucts;
		}

		public void ClearValues(IAuxiliariesViewModel viewModel)
		{
			viewModel.AlternatorTechnologies = default(ObservableCollection<string>);
			viewModel.DayrunninglightsLED = default(bool);
			viewModel.HeadlightsLED = default(bool);
			viewModel.PositionlightsLED = default(bool);
			viewModel.BrakelightsLED = default(bool);
			viewModel.InteriorLightsLED = default(bool);
			viewModel.DoorDriveTechnology = default(ConsumerTechnology);
			viewModel.SystemConfiguration = default(BusHVACSystemConfiguration);
			viewModel.CompressorTypeDriver = default(ACCompressorType);
			viewModel.CompressorTypePassenger = default(ACCompressorType);
			viewModel.AuxHeaterPower = default(Watt);
			viewModel.DoubleGlasing = default(bool);
			viewModel.HeatPump = default(bool);
			viewModel.AdjustableAuxiliaryHeater = default(bool);
			viewModel.SeparateAirDistributionDucts = default(bool);
		}
		
		private void SetValues(IAuxiliariesViewModel auxiliaries)
		{
			AlternatorTechnologies = new ObservableCollection<string>(auxiliaries.AlternatorTechnologies);
			DayrunninglightsLED = auxiliaries.DayrunninglightsLED;
			HeadlightsLED = auxiliaries.HeadlightsLED;
			PositionlightsLED = auxiliaries.PositionlightsLED;
			BrakelightsLED = auxiliaries.BrakelightsLED;
			InteriorLightsLED = auxiliaries.InteriorLightsLED;
			DoorDriveTechnology = auxiliaries.DoorDriveTechnology;
			SystemConfiguration = auxiliaries.SystemConfiguration;
			CompressorTypeDriver = auxiliaries.CompressorTypeDriver;
			CompressorTypePassenger = auxiliaries.CompressorTypePassenger;
			AuxHeaterPower = auxiliaries.AuxHeaterPower;
			DoubleGlasing = auxiliaries.DoubleGlasing;
			HeatPump = auxiliaries.HeatPump;
			AdjustableAuxiliaryHeater = auxiliaries.AdjustableAuxiliaryHeater;
			SeparateAirDistributionDucts = auxiliaries.SeparateAirDistributionDucts;
		}

	}
}
