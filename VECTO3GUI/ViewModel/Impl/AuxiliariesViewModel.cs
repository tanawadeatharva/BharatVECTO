using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Documents;
using Castle.Core.Internal;
using Ninject;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Declaration;
using VECTO3GUI.Util;
using VECTO3GUI.ViewModel.Adapter;
using VECTO3GUI.ViewModel.Interfaces;
using VECTO3GUI.Model;
using Component = VECTO3GUI.Util.Component;

namespace VECTO3GUI.ViewModel.Impl
{
	public class AuxiliariesViewModel : AbstractViewModel, IAuxiliariesViewModel
	{
		[Inject]
		public IAdapterFactory AdapterFactory { set; protected get; }

		#region Members

		private string _pneumaticSystemTechnology;
		private string _fanTechnology;
		private string _electricSystemTechnology;
		private string _hvacTechnology;
		private readonly ObservableCollection<SteeringPumpEntry> _steeringPumpTechnologies = new ObservableCollection<SteeringPumpEntry>();
		private IAxlesViewModel axlesViewModel;
		private IBusAuxiliariesDeclarationData _busAuxiliaries;

		private ObservableCollection<string> _alternatorTechnologies;
		private bool _dayRunningLightsLED;
		private bool _headlightyLED;
		private bool _positionlightsLED;
		private bool _breaklightsLED;
		private bool _interiorLightsLED;
		private ConsumerTechnology _doorDriveTechnology;
		private BusHVACSystemConfiguration _systemConfiguration;
		private ACCompressorType _compressorTypeDriver;
		private ACCompressorType _compressorTypePassenger;
		private Watt _auxHeaterPower;
		private bool _doubleGlasing;
		private bool _heatPump;
		private bool _adjustableAuxiliaryHeater;
		private bool _separateAirDistributionDucts;

		#endregion


		#region Implementation of IAuxiliariesViewModel

		public IAuxiliariesDeclarationInputData ModelData { get { return AdapterFactory.AuxiliariesDeclarationAdapter(this); } }

		public string PneumaticSystemTechnology
		{
			get { return _pneumaticSystemTechnology; }
			set { SetProperty(ref _pneumaticSystemTechnology, value); }
		}

		public AllowedEntry<string>[] AllowedPneumaticSystemTechnologies
		{
			get { return DeclarationData.PneumaticSystem.GetTechnologies().Select(t => AllowedEntry.Create(t, t)).ToArray(); }
		}

		public string FanTechnology
		{
			get { return _fanTechnology; }
			set { SetProperty(ref _fanTechnology, value); }
		}

		public AllowedEntry<string>[] AllowedFanTechnologies { get { return DeclarationData.Fan.GetTechnologies().Select(t => AllowedEntry.Create(t, t)).ToArray(); } }

		public ObservableCollection<SteeringPumpEntry> SteeringPumpTechnologies
		{
			get
			{
				if (axlesViewModel == null)
				{
					ConnectAxleViewModel();
				}

				return _steeringPumpTechnologies;
			}
		}

		public AllowedEntry<string>[] AllowedSteeringPumpTechnologies { get { return DeclarationData.SteeringPump.GetTechnologies().Select(t => AllowedEntry.Create(t, t)).ToArray(); } }

		public string ElectricSystemTechnology
		{
			get { return _electricSystemTechnology; }
			set { SetProperty(ref _electricSystemTechnology, value); }
		}

		public AllowedEntry<string>[] AllowedElectricSystemTechnologies { get { return DeclarationData.ElectricSystem.GetTechnologies().Select(t => AllowedEntry.Create(t, t)).ToArray(); } }

		public string HVACTechnology
		{
			get { return _hvacTechnology; }
			set { SetProperty(ref _hvacTechnology, value); }
		}

		public AllowedEntry<string>[] AllowedHVACTechnologies { get { return DeclarationData.HeatingVentilationAirConditioning.GetTechnologies().Select(t => AllowedEntry.Create(t, t)).ToArray(); } }
		
		public bool DayrunninglightsLED
		{
			get { return _dayRunningLightsLED; }
			set { SetProperty(ref _dayRunningLightsLED, value); }
		}
		public bool HeadlightsLED
		{
			get { return _headlightyLED; }
			set { SetProperty(ref _headlightyLED, value); }
		}
		public bool PositionlightsLED
		{
			get { return _positionlightsLED; }
			set { SetProperty(ref _positionlightsLED, value); }
		}
		public bool BrakelightsLED
		{
			get { return _breaklightsLED; }
			set { SetProperty(ref _breaklightsLED, value); }
		}
		public bool InteriorLightsLED
		{
			get { return _interiorLightsLED; }
			set { SetProperty(ref _interiorLightsLED, value); }
		}

		public ObservableCollection<string> AlternatorTechnologies
		{
			get { return _alternatorTechnologies; }
			set { SetProperty(ref _alternatorTechnologies, value); }
		}


		//public ObservableCollection<IAlternatorDeclarationInputData> Alternators
		//{
		//	get { return _alternators; }
		//	set { SetProperty(ref _alternators, value); }
		//}

		public ConsumerTechnology DoorDriveTechnology
		{
			get { return _doorDriveTechnology; }
			set { SetProperty(ref _doorDriveTechnology, value); }
		}

		public BusHVACSystemConfiguration SystemConfiguration
		{
			get { return _systemConfiguration; }
			set { SetProperty(ref _systemConfiguration, value); }
		}

		public ACCompressorType CompressorTypeDriver
		{
			get { return _compressorTypeDriver; }
			set { SetProperty(ref _compressorTypeDriver, value); }
		}

		public ACCompressorType CompressorTypePassenger
		{
			get { return _compressorTypePassenger; }
			set { SetProperty(ref _compressorTypePassenger, value); }
		}

		public Watt AuxHeaterPower
		{
			get { return _auxHeaterPower; }
			set { SetProperty(ref _auxHeaterPower, value); }
		}

		public bool DoubleGlasing
		{
			get { return _doubleGlasing; }
			set { SetProperty(ref _doubleGlasing, value); }
		}

		public bool HeatPump
		{
			get { return _heatPump; }
			set { SetProperty(ref _heatPump, value); }
		}

		public bool AdjustableAuxiliaryHeater
		{
			get { return _adjustableAuxiliaryHeater; }
			set { SetProperty(ref _adjustableAuxiliaryHeater, value); }
		}

		public bool SeparateAirDistributionDucts
		{
			get { return _separateAirDistributionDucts; }
			set { SetProperty(ref _separateAirDistributionDucts, value); }
		}

		public AllowedEntry<BusHVACSystemConfiguration>[] AllowedSystemConfigurations { get; private set; }
		public AllowedEntry<ACCompressorType>[] AllowedDriverACCompressorTypes { get; private set; }
		public AllowedEntry<ACCompressorType>[] AllowedPassengerACCompressorTypes { get; private set; }
		public AllowedEntry<ConsumerTechnology>[] AllowedConsumerTechnologies { get; private set; }

		#endregion

		protected override void InputDataChanged()
		{
			var inputData = JobViewModel.InputDataProvider as IDeclarationInputDataProvider;

			//SetValues(inputData.JobInputData.Vehicle.Components.);

			////ToDo
			//JobViewModel.InputDataProvider.Switch()
			//			.If<IDeclarationInputDataProvider>(d => SetValues(d.JobInputData.Vehicle.Components.AuxiliaryInputData()))
			//			.If<IEngineeringInputDataProvider>(e => SetValues(e.JobInputData.Vehicle.Components.AuxiliaryInputData()));

			if (inputData?.JobInputData?.Vehicle?.Components?.BusAuxiliaries != null) {
				SetValues(inputData.JobInputData.Vehicle.Components.BusAuxiliaries);
				_busAuxiliaries = inputData.JobInputData.Vehicle.Components.BusAuxiliaries;
			}

			ConnectAxleViewModel();
		}

		private void ConnectAxleViewModel()
		{
			var axlesVm = ParentViewModel.GetComponentViewModel(Component.Axles);

			if (axlesVm == null)
			{
				return;
			}

			axlesViewModel = axlesVm as IAxlesViewModel;
			(axlesViewModel as AxlesViewModel).PropertyChanged += UpdateSteeringPump;
			DoUpdateSteeringPumpTechnologies();
		}

		private void UpdateSteeringPump(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "NumSteeredAxles")
			{
				DoUpdateSteeringPumpTechnologies();
			}
		}

		private void DoUpdateSteeringPumpTechnologies()
		{
			if (axlesViewModel == null)
			{
				return;
			}

			while (_steeringPumpTechnologies.Count > axlesViewModel.NumSteeredAxles)
			{
				_steeringPumpTechnologies.RemoveAt(_steeringPumpTechnologies.Count - 1);
			}
			while (_steeringPumpTechnologies.Count < axlesViewModel.NumSteeredAxles)
			{
				_steeringPumpTechnologies.Add(new SteeringPumpEntry(_steeringPumpTechnologies.Count + 1, AllowedSteeringPumpTechnologies.First().Label));
			}
		}

		private void SetValues(IAuxiliariesEngineeringInputData aux)
		{
			throw new NotImplementedException();
		}

		private void SetValues(IAuxiliariesDeclarationInputData aux)
		{
			FanTechnology = aux.Auxiliaries.First(x => x.Type == AuxiliaryType.Fan).Technology.FirstOrDefault();
			ElectricSystemTechnology = aux.Auxiliaries.First(x => x.Type == AuxiliaryType.ElectricSystem).Technology.FirstOrDefault();
			HVACTechnology = aux.Auxiliaries.First(x => x.Type == AuxiliaryType.HVAC).Technology.FirstOrDefault();
			PneumaticSystemTechnology = aux.Auxiliaries.First(x => x.Type == AuxiliaryType.PneumaticSystem).Technology.FirstOrDefault();
			SteeringPumpTechnologies.Clear();
			foreach (var tech in aux.Auxiliaries.First(x => x.Type == AuxiliaryType.SteeringPump).Technology)
			{
				SteeringPumpTechnologies.Add(new SteeringPumpEntry(SteeringPumpTechnologies.Count + 1, tech));
			}
		}

		private void SetValues(IBusAuxiliariesDeclarationData busAux)
		{
			if (!busAux.ElectricSupply.Alternators.IsNullOrEmpty()) {
				AlternatorTechnologies = new ObservableCollection<string>();
				
				for (int i = 0; i < busAux.ElectricSupply.Alternators.Count; i++) {
					AlternatorTechnologies.Add(busAux.ElectricSupply.Alternators[i].Technology);
				}
			}

			DayrunninglightsLED = busAux.ElectricConsumers.DayrunninglightsLED;
			HeadlightsLED = busAux.ElectricConsumers.HeadlightsLED;
			PositionlightsLED = busAux.ElectricConsumers.PositionlightsLED;
			BrakelightsLED = busAux.ElectricConsumers.BrakelightsLED;
			InteriorLightsLED = busAux.ElectricConsumers.InteriorLightsLED;
			
			DoorDriveTechnology = busAux.PneumaticConsumers.DoorDriveTechnology;

			SystemConfiguration = busAux.HVACAux.SystemConfiguration;
			CompressorTypeDriver = busAux.HVACAux.CompressorTypeDriver;
			CompressorTypePassenger = busAux.HVACAux.CompressorTypePassenger;
			AuxHeaterPower = busAux.HVACAux.AuxHeaterPower;
			DoubleGlasing = busAux.HVACAux.DoubleGlasing;
			HeatPump = busAux.HVACAux.HeatPump;
			AdjustableAuxiliaryHeater = busAux.HVACAux.AdjustableAuxiliaryHeater;
			SeparateAirDistributionDucts = busAux.HVACAux.SeparateAirDistributionDucts;

			SetAllowedValues();
		}

		private void SetAllowedValues()
		{
			AllowedSystemConfigurations = Enum.GetValues(typeof(BusHVACSystemConfiguration)).Cast<BusHVACSystemConfiguration>()
				.Select(sc => AllowedEntry.Create(sc, sc.GetLabel())).ToArray();

			AllowedDriverACCompressorTypes = Enum.GetValues(typeof(ACCompressorType)).Cast<ACCompressorType>()
				.Select(acc => AllowedEntry.Create(acc, acc.GetLabel())).ToArray();

			AllowedPassengerACCompressorTypes = AllowedDriverACCompressorTypes;

			AllowedConsumerTechnologies = Enum.GetValues(typeof(ConsumerTechnology)).Cast<ConsumerTechnology>()
				.Select(sc => AllowedEntry.Create(sc, sc.GetLabel())).ToArray();
		}

		public override bool AnyDataChanges()
		{
			if(_busAuxiliaries == null)
				return base.AnyDataChanges();

			bool changed;

			if (!_busAuxiliaries.ElectricSupply.Alternators.IsNullOrEmpty()) {
				//ToDo
				//Changed Event?!
			}


			changed = _busAuxiliaries.ElectricConsumers.DayrunninglightsLED != DayrunninglightsLED ||
					_busAuxiliaries.ElectricConsumers.HeadlightsLED != HeadlightsLED ||
					_busAuxiliaries.ElectricConsumers.PositionlightsLED != PositionlightsLED ||
					_busAuxiliaries.ElectricConsumers.BrakelightsLED != BrakelightsLED ||
					_busAuxiliaries.ElectricConsumers.InteriorLightsLED != InteriorLightsLED ||
					_busAuxiliaries.PneumaticConsumers.DoorDriveTechnology != DoorDriveTechnology ||
					_busAuxiliaries.HVACAux.SystemConfiguration != SystemConfiguration ||
					_busAuxiliaries.HVACAux.CompressorTypeDriver != CompressorTypeDriver ||
					_busAuxiliaries.HVACAux.CompressorTypePassenger != CompressorTypePassenger ||
					_busAuxiliaries.HVACAux.AuxHeaterPower != AuxHeaterPower ||
					_busAuxiliaries.HVACAux.DoubleGlasing != DoubleGlasing ||
					_busAuxiliaries.HVACAux.HeatPump != HeatPump ||
					_busAuxiliaries.HVACAux.AdjustableAuxiliaryHeater != AdjustableAuxiliaryHeater ||
					_busAuxiliaries.HVACAux.SeparateAirDistributionDucts != SeparateAirDistributionDucts;

			return changed;
		}
	}
}