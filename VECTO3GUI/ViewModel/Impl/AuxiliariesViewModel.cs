using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Ninject;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Utils;
using VECTO3.Util;
using VECTO3.ViewModel.Adapter;
using VECTO3.ViewModel.Adapter.Declaration;
using VECTO3.ViewModel.Interfaces;
using Component = VECTO3.Util.Component;

namespace VECTO3.ViewModel.Impl {
	public class AuxiliariesViewModel : AbstractViewModel, IAuxiliariesViewModel
	{
		[Inject] public IAdapterFactory AdapterFactory { set; protected get; }

		private string _pneumaticSystemTechnology;
		private string _fanTechnology;
		private string _electricSystemTechnology;
		private string _hvacTechnology;
		private readonly ObservableCollection<SteeringPumpEntry> _steeringPumpTechnologies = new ObservableCollection<SteeringPumpEntry>();
		private IAxlesViewModel axlesViewModel;

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
			get {
				if (axlesViewModel == null) {
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

		#endregion

		protected override void InputDataChanged()
		{
			//ToDo
			//JobViewModel.InputDataProvider.Switch()
			//			.If<IDeclarationInputDataProvider>(d => SetValues(d.JobInputData.Vehicle.Components.AuxiliaryInputData()))
			//			.If<IEngineeringInputDataProvider>(e => SetValues(e.JobInputData.Vehicle.Components.AuxiliaryInputData()));
			ConnectAxleViewModel();
		}

		private void ConnectAxleViewModel()
		{
			var axlesVm = ParentViewModel.GetComponentViewModel(Component.Axles);
			
			if (axlesVm == null) {
				return;
			}

			axlesViewModel = axlesVm as IAxlesViewModel;
			(axlesViewModel as AxlesViewModel).PropertyChanged += UpdateSteeringPump;
			DoUpdateSteeringPumpTechnologies();
		}

		private void UpdateSteeringPump(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "NumSteeredAxles") {
				DoUpdateSteeringPumpTechnologies();
			}
		}

		private void DoUpdateSteeringPumpTechnologies()
		{
			if (axlesViewModel == null) {
				return;
			}

			while (_steeringPumpTechnologies.Count > axlesViewModel.NumSteeredAxles) {
				_steeringPumpTechnologies.RemoveAt(_steeringPumpTechnologies.Count - 1);
			}
			while (_steeringPumpTechnologies.Count < axlesViewModel.NumSteeredAxles) {
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
			foreach (var tech in aux.Auxiliaries.First(x => x.Type == AuxiliaryType.SteeringPump).Technology) {
				SteeringPumpTechnologies.Add(new SteeringPumpEntry(SteeringPumpTechnologies.Count + 1, tech));
			}
		}
	}
}