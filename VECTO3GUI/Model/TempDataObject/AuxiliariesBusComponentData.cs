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
	public class AuxiliariesBusComponentData: IAuxiliariesBus
	{
		#region Properties

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

		public AuxiliariesBusComponentData(IAuxiliariesViewModel auxiliaries)
		{
			SetValues(auxiliaries);
		}

		public void UpdateCurrentValues(IAuxiliariesViewModel auxiliaries)
		{
			SetValues(auxiliaries);
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
