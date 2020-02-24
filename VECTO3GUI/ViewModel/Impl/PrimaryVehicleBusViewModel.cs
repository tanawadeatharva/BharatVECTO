using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ninject;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider;
using TUGraz.VectoCore.Utils;
using VECTO3GUI.ViewModel.Adapter;
using VECTO3GUI.ViewModel.Impl;
using VECTO3GUI.ViewModel.Interfaces;

namespace VECTO3GUI.ViewModel.Impl
{
	public class PrimaryVehicleBusViewModel : AbstractViewModel, IPrimaryVehicleBusViewModel
	{

		#region Members

		private string _manufacturer;

		#endregion

		#region Properties
		#endregion



		public PrimaryVehicleBusViewModel()
		{
			
		}


		[Inject]
		public IAdapterFactory AdapterFactory { set; protected get; }


		#region Methods IPrimaryVehicleBusViewModel
		
		public IVehicleDeclarationInputData Vehicle { get; set; }
		public DigestData ResultDataHash { get; set; }
		public IResultsInputData ResultsInputData { get; set; }
		public IApplicationInformation ApplicationInformation { get; set; }
		public DigestData ManufacturerHash { get; set; }


		public string Manufacturer
		{
			get { return _manufacturer;}
			set { SetProperty(ref _manufacturer, value); }
		}
		public string ManufacturerAddress { get; set; }
		public string Model { get; set; }
		public string VIN { get; set; }
		public DateTime Date { get; set; }
		public VehicleCategory VehicleCategory { get; set; }
		public AxleConfiguration AxleConfiguration { get; set; }
		public bool Articulated { get; set; }
		public Kilogram TechnicalPermissibleMaximumLadenMass { get; set; }
		public PerSecond IdlingSpeed { get; set; }
		public RetarderType RetarderType { get; set; }
		public double RetarderRatio { get; set; }
		public AngledriveType AngledriveType { get; set; }
		public bool ZeroEmissionVehicle { get; set; }
		public IAdvancedDriverAssistantSystemDeclarationInputData ADAS { get; set; }
		public IList<ITorqueLimitInputData> TorqueLimits { get; set; }


		#endregion

		private void SetVehicleData(IVehicleDeclarationInputData vehicle)
		{
			Manufacturer = vehicle.Manufacturer;
			Model = vehicle.Model;
			ManufacturerAddress = vehicle.ManufacturerAddress;
			VIN = vehicle.VIN;
			Date = vehicle.Date;
			VehicleCategory = vehicle.VehicleCategory;
			AxleConfiguration = vehicle.AxleConfiguration;
			Articulated = vehicle.Articulated;
			TechnicalPermissibleMaximumLadenMass = vehicle.GrossVehicleMassRating;
			IdlingSpeed = vehicle.EngineIdleSpeed;

			RetarderType = ((XMLDeclarationPrimaryVehicleBusDataProviderV01)vehicle).RetarderType;
			RetarderRatio = ((XMLDeclarationPrimaryVehicleBusDataProviderV01)vehicle).RetarderRatio;
			AngledriveType = ((XMLDeclarationPrimaryVehicleBusDataProviderV01)vehicle).AngledriveType;

			ZeroEmissionVehicle = vehicle.ZeroEmissionVehicle;
			ADAS = vehicle.ADAS;
			TorqueLimits = vehicle.TorqueLimits;
		}

		
		protected override void InputDataChanged()
		{
			var inputData = JobViewModel.InputDataProvider as IPrimaryVehicleInputDataProvider;
			SetVehicleData(inputData?.Vehicle);
		}


	}
}
