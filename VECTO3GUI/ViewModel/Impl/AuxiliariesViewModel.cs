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
	public class AuxiliariesViewModel : AbstractComponentViewModel, IAuxiliariesViewModel
	{
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

		public AllowedEntry<string>[] AllowedFanTechnologies
		{
			get { return DeclarationData.Fan.GetTechnologies().Select(t => AllowedEntry.Create(t, t)).ToArray(); }
		}

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

		public AllowedEntry<string>[] AllowedSteeringPumpTechnologies
		{
			get { return DeclarationData.SteeringPump.GetTechnologies().Select(t => AllowedEntry.Create(t, t)).ToArray(); }
		}

		public string ElectricSystemTechnology
		{
			get { return _electricSystemTechnology; }
			set { SetProperty(ref _electricSystemTechnology, value); }
		}

		public AllowedEntry<string>[] AllowedElectricSystemTechnologies
		{
			get { return DeclarationData.ElectricSystem.GetTechnologies().Select(t => AllowedEntry.Create(t, t)).ToArray(); }
		}

		public string HVACTechnology
		{
			get { return _hvacTechnology; }
			set { SetProperty(ref _hvacTechnology, value); }
		}

		public AllowedEntry<string>[] AllowedHVACTechnologies
		{
			get
			{
				return DeclarationData.HeatingVentilationAirConditioning.GetTechnologies()
					.Select(t => AllowedEntry.Create(t, t)).ToArray();
			}
		}

		#endregion


		#region Implementation of IBusAuxiliaries

		public ObservableCollection<string> AlternatorTechnologies
		{
			get { return _alternatorTechnologies; }
			set { SetProperty(ref _alternatorTechnologies, value); }
		}

		public bool DayrunninglightsLED
		{
			get { return _dayRunningLightsLED; }
			set
			{
				if (SetProperty(ref _dayRunningLightsLED, value))
				{
					var changed = true;
					if (_busAuxiliaries.ElectricConsumers != null)
						changed = _busAuxiliaries.ElectricConsumers.DayrunninglightsLED != value;
					SetChangedProperty(changed);
				}
			}
		}
		public bool HeadlightsLED
		{
			get { return _headlightyLED; }
			set
			{
				if (SetProperty(ref _headlightyLED, value))
				{
					var changed = true;
					if (_busAuxiliaries.ElectricConsumers != null)
						changed = _busAuxiliaries.ElectricConsumers.HeadlightsLED != value;
					SetChangedProperty(changed);
				}
			}
		}
		public bool PositionlightsLED
		{
			get { return _positionlightsLED; }
			set
			{
				if (SetProperty(ref _positionlightsLED, value))
				{
					var changed = true;
					if (_busAuxiliaries.ElectricConsumers != null)
						changed = _busAuxiliaries.ElectricConsumers.PositionlightsLED != value;
					SetChangedProperty(changed);
				}
			}
		}
		public bool BrakelightsLED
		{
			get { return _breaklightsLED; }
			set
			{
				if (SetProperty(ref _breaklightsLED, value))
				{
					var changed = true;
					if (_busAuxiliaries.ElectricConsumers != null)
						changed = _busAuxiliaries.ElectricConsumers.BrakelightsLED != value;
					SetChangedProperty(changed);
				}
			}
		}
		public bool InteriorLightsLED
		{
			get { return _interiorLightsLED; }
			set
			{
				if (SetProperty(ref _interiorLightsLED, value))
				{
					var changed = true;
					if (_busAuxiliaries.ElectricConsumers != null)
						changed = _busAuxiliaries.ElectricConsumers.InteriorLightsLED != value;
					SetChangedProperty(changed);
				}
			}
		}

		public ConsumerTechnology DoorDriveTechnology
		{
			get { return _doorDriveTechnology; }
			set
			{
				if (SetProperty(ref _doorDriveTechnology, value))
				{
					var changed = _busAuxiliaries.PneumaticConsumers != null
						? _busAuxiliaries.PneumaticConsumers.DoorDriveTechnology != value
						: value != default(ConsumerTechnology);
					SetChangedProperty(changed);
				}
			}
		}

		public BusHVACSystemConfiguration SystemConfiguration
		{
			get { return _systemConfiguration; }
			set
			{
				if (SetProperty(ref _systemConfiguration, value))
				{
					var changed = _busAuxiliaries.HVACAux != null
								? _busAuxiliaries.HVACAux.SystemConfiguration != value
								: value != default(BusHVACSystemConfiguration);
					SetChangedProperty(changed);
				}
			}
		}

		public ACCompressorType CompressorTypeDriver
		{
			get { return _compressorTypeDriver; }
			set
			{
				if (SetProperty(ref _compressorTypeDriver, value))
				{
					var changed = _busAuxiliaries.HVACAux != null
								? _busAuxiliaries.HVACAux.CompressorTypeDriver != value
								: value != default(ACCompressorType);
					SetChangedProperty(changed);
				}
			}
		}

		public ACCompressorType CompressorTypePassenger
		{
			get { return _compressorTypePassenger; }
			set
			{
				if (SetProperty(ref _compressorTypePassenger, value))
				{
					var changed = _busAuxiliaries.HVACAux != null
								? _busAuxiliaries.HVACAux.CompressorTypePassenger != value
								: value != default(ACCompressorType);
					SetChangedProperty(changed);
				}
			}
		}

		public Watt AuxHeaterPower
		{
			get { return _auxHeaterPower; }
			set
			{
				if (SetProperty(ref _auxHeaterPower, value))
				{
					var changed = _busAuxiliaries.HVACAux != null
								? _busAuxiliaries.HVACAux.AuxHeaterPower != value
								: value != default(Watt);
					SetChangedProperty(changed);
				}
			}
		}

		public bool DoubleGlasing
		{
			get { return _doubleGlasing; }
			set
			{
				if (SetProperty(ref _doubleGlasing, value))
				{
					var changed = true;
					if (_busAuxiliaries.HVACAux != null)
						changed = _busAuxiliaries.HVACAux.DoubleGlasing != value;
					SetChangedProperty(changed);
				}
			}
		}

		public bool HeatPump
		{
			get { return _heatPump; }
			set
			{
				if (SetProperty(ref _heatPump, value))
				{
					var changed = true;
					if (_busAuxiliaries.HVACAux != null)
						changed = _busAuxiliaries.HVACAux.HeatPump != value;
					SetChangedProperty(changed);
				}
			}
		}

		public bool AdjustableAuxiliaryHeater
		{
			get { return _adjustableAuxiliaryHeater; }
			set
			{
				if (SetProperty(ref _adjustableAuxiliaryHeater, value))
				{
					var changed = true;
					if (_busAuxiliaries.HVACAux != null)
						changed = _busAuxiliaries.HVACAux.AdjustableAuxiliaryHeater != value;
					SetChangedProperty(changed);
				}
			}
		}

		public bool SeparateAirDistributionDucts
		{
			get { return _separateAirDistributionDucts; }
			set
			{
				if (SetProperty(ref _separateAirDistributionDucts, value))
				{
					var changed = true;
					if (_busAuxiliaries.HVACAux != null)
						changed = _busAuxiliaries.HVACAux.SeparateAirDistributionDucts != value;
					SetChangedProperty(changed);
				}
			}
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

			if (inputData?.JobInputData?.Vehicle?.Components?.BusAuxiliaries != null)
			{
				_busAuxiliaries = inputData.JobInputData.Vehicle.Components.BusAuxiliaries;
				SetValues(_busAuxiliaries);

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
			if (!busAux.ElectricSupply.Alternators.IsNullOrEmpty())
			{
				AlternatorTechnologies = new ObservableCollection<string>();

				for (int i = 0; i < busAux.ElectricSupply.Alternators.Count; i++)
				{
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

		public override bool IsComponentDataChanged()
		{
			return _changedInput.Count > 0;
		}

		public override void ResetComponentData()
		{
			SetValues(_busAuxiliaries);
		}
	}
}