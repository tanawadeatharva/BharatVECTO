using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Utils;
using VECTO3GUI.Helper;
using VECTO3GUI.ViewModel.Interfaces;

namespace VECTO3GUI.Model.TempDataObject
{
	public class AuxiliariesBusComponentData: IAuxiliariesBus, ITempDataObject<IAuxiliariesViewModel>
	{
		#region IAuxiliariesBus Interface

		public ObservableCollectionEx<AlternatorTechnologyModel> AlternatorTechnologies { get; set; }
		public List<AlternatorTechnologyModel> OriginAlternatorTechnologies { get; private set; }
		public bool DayrunninglightsLED { get; set; }
		public bool HeadlightsLED { get; set; }
		public bool PositionlightsLED { get; set; }
		public bool BrakelightsLED { get; set; }
		public bool InteriorLightsLED { get; set; }
		public BusHVACSystemConfiguration SystemConfiguration { get; set; }
		public ACCompressorType CompressorTypeDriver { get; set; }
		public ACCompressorType CompressorTypePassenger { get; set; }
		public Watt AuxHeaterPower { get; set; }
		public bool DoubleGlasing { get; set; }
		public bool HeatPump { get; set; }
		public bool AdjustableAuxiliaryHeater { get; set; }
		public bool SeparateAirDistributionDucts { get; set; }
		public ConsumerTechnology DoorDriveTechnology { get; set; }

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
			viewModel.AlternatorTechnologies = GetAlternatorTechnology();
			viewModel.DayrunninglightsLED = DayrunninglightsLED;
			viewModel.HeadlightsLED = HeadlightsLED;
			viewModel.PositionlightsLED = PositionlightsLED;
			viewModel.BrakelightsLED = BrakelightsLED;
			viewModel.InteriorLightsLED = InteriorLightsLED;
			viewModel.SystemConfiguration = SystemConfiguration;
			viewModel.CompressorTypeDriver = CompressorTypeDriver;
			viewModel.CompressorTypePassenger = CompressorTypePassenger;
			viewModel.AuxHeaterPower = AuxHeaterPower;
			viewModel.DoubleGlasing = DoubleGlasing;
			viewModel.HeatPump = HeatPump;
			viewModel.AdjustableAuxiliaryHeater = AdjustableAuxiliaryHeater;
			viewModel.SeparateAirDistributionDucts = SeparateAirDistributionDucts;
			viewModel.DoorDriveTechnology = viewModel.DoorDriveTechnology;
		}

		private ObservableCollectionEx<AlternatorTechnologyModel> GetAlternatorTechnology()
		{
			var res = new ObservableCollectionEx<AlternatorTechnologyModel>();
			for (int i = 0; i < OriginAlternatorTechnologies.Count; i++) {
				res.Add(new AlternatorTechnologyModel{AlternatorTechnology = OriginAlternatorTechnologies[i].AlternatorTechnology});
			}

			return res;
		}
		
		public void ClearValues(IAuxiliariesViewModel viewModel)
		{
			viewModel.AlternatorTechnologies = default(ObservableCollectionEx<AlternatorTechnologyModel>);
			viewModel.DayrunninglightsLED = default(bool);
			viewModel.HeadlightsLED = default(bool);
			viewModel.PositionlightsLED = default(bool);
			viewModel.BrakelightsLED = default(bool);
			viewModel.InteriorLightsLED = default(bool);
			viewModel.SystemConfiguration = default(BusHVACSystemConfiguration);
			viewModel.CompressorTypeDriver = default(ACCompressorType);
			viewModel.CompressorTypePassenger = default(ACCompressorType);
			viewModel.AuxHeaterPower = default(Watt);
			viewModel.DoubleGlasing = default(bool);
			viewModel.HeatPump = default(bool);
			viewModel.AdjustableAuxiliaryHeater = default(bool);
			viewModel.SeparateAirDistributionDucts = default(bool);
			viewModel.DoorDriveTechnology = ConsumerTechnology.Pneumatically;
		}
		
		private void SetValues(IAuxiliariesViewModel auxiliaries)
		{
			OriginAlternatorTechnologies = GetOriginAlternatorTechnologies(auxiliaries);
			AlternatorTechnologies = GetAlternatorTechnology();
			DayrunninglightsLED = auxiliaries.DayrunninglightsLED;
			HeadlightsLED = auxiliaries.HeadlightsLED;
			PositionlightsLED = auxiliaries.PositionlightsLED;
			BrakelightsLED = auxiliaries.BrakelightsLED;
			InteriorLightsLED = auxiliaries.InteriorLightsLED;
			SystemConfiguration = auxiliaries.SystemConfiguration;
			CompressorTypeDriver = auxiliaries.CompressorTypeDriver;
			CompressorTypePassenger = auxiliaries.CompressorTypePassenger;
			AuxHeaterPower = auxiliaries.AuxHeaterPower;
			DoubleGlasing = auxiliaries.DoubleGlasing;
			HeatPump = auxiliaries.HeatPump;
			AdjustableAuxiliaryHeater = auxiliaries.AdjustableAuxiliaryHeater;
			SeparateAirDistributionDucts = auxiliaries.SeparateAirDistributionDucts;
			DoorDriveTechnology = auxiliaries.DoorDriveTechnology;
		}

		private List<AlternatorTechnologyModel> GetOriginAlternatorTechnologies(IAuxiliariesViewModel auxiliaries)
		{
			if (auxiliaries.AlternatorTechnologies == null)
				return null;

			var result = new List<AlternatorTechnologyModel>();
			for (int i = 0; i < auxiliaries.AlternatorTechnologies.Count; i++) {
				result.Add(new AlternatorTechnologyModel {
					AlternatorTechnology =  auxiliaries.AlternatorTechnologies[i].AlternatorTechnology
				});
			}
			return result;
		}
	}
}
