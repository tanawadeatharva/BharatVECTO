using System;
using System.Collections.Generic;
using System.Xml;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCore.Tests.Utils {
	public class MockDeclarationVehicleInputData : IVehicleDeclarationInputData, IVehicleComponentsDeclaration
	{
		#region Implementation of IComponentInputData

		public DataSource DataSource { get; }
		public bool SavedInDeclarationMode { get; }
		public string Manufacturer { get; }
		public string Model { get; }
		public DateTime Date { get; }
		public string AppVersion { get { return "Mock-Class"; } }
		public CertificationMethod CertificationMethod { get; }
		public string CertificationNumber { get; }
		public DigestData DigestValue { get; }

		#endregion

		#region Implementation of IVehicleDeclarationInputData

		public string Identifier { get; }
		public bool ExemptedVehicle { get; }
		public string VIN { get; }
		public LegislativeClass LegislativeClass { get; }
		public VehicleCategory VehicleCategory { get; }
		public AxleConfiguration AxleConfiguration { get; }
		public Kilogram CurbMassChassis { get; }
		public Kilogram GrossVehicleMassRating { get; }
		public IList<ITorqueLimitInputData> TorqueLimits { get { return new List<ITorqueLimitInputData>(); } }
		public string ManufacturerAddress { get; }
		public PerSecond EngineIdleSpeed { get; }
		public bool VocationalVehicle { get; }
		public bool SleeperCab { get; }
		public TankSystem? TankSystem { get; }
		public IAdvancedDriverAssistantSystemDeclarationInputData ADAS { get; }
		public bool ZeroEmissionVehicle { get; }
		public bool HybridElectricHDV { get; }
		public bool DualFuelVehicle { get; }
		public Watt MaxNetPower1 { get; }
		public Watt MaxNetPower2 { get; }
		public RegistrationClass RegisteredClass { get; set; }
		public int NuberOfPassengersUpperDeck { get; set; }
		public int NumberOfPassengersLowerDeck { get; set; }
		public VehicleCode VehicleCode { get; set; }
		public FloorType FloorType { get; }
		public bool Articulated { get; }
		public Meter Height { get; set; }
		public Meter Length { get; set; }
		public Meter Width { get; set; }
		public Meter EntranceHeight { get; }
		public IVehicleComponentsDeclaration Components { get { return this; } }
		public XmlNode XMLSource { get; }

		#endregion

		#region Implementation of IVehicleComponentsDeclaration

		public IAirdragDeclarationInputData AirdragInputData { get; set; }
		public IGearboxDeclarationInputData GearboxInputData { get; set; }
		public ITorqueConverterDeclarationInputData TorqueConverterInputData { get; set; }
		public IAxleGearInputData AxleGearInputData { get; set; }
		public IAngledriveInputData AngledriveInputData { get; set; }
		public IEngineDeclarationInputData EngineInputData { get; set; }
		public IAuxiliariesDeclarationInputData AuxiliaryInputData { get; set; }
		public IRetarderInputData RetarderInputData { get; set; }
		public IPTOTransmissionInputData PTOTransmissionInputData { get; set; }
		public IAxlesDeclarationInputData AxleWheels { get; set; }
		public IBusAuxiliariesDeclarationData BusAuxiliaries { get; set; }

		#endregion
	}

	public class MockEngineeringVehicleInputData : IVehicleEngineeringInputData, IVehicleComponentsEngineering
	{
		private IAdvancedDriverAssistantSystemDeclarationInputData _adas;
		private IVehicleComponentsDeclaration _components;

		#region Implementation of IComponentInputData

		public DataSource DataSource { get; }
		public bool SavedInDeclarationMode { get; }
		public string Manufacturer { get; }
		public string Model { get; }
		public DateTime Date { get; }
		public string AppVersion { get { return "Mock-Class"; } }
		public CertificationMethod CertificationMethod { get; }
		public string CertificationNumber { get; }
		public DigestData DigestValue { get; }

		#endregion

		#region Implementation of IVehicleEngineeringInputData

		public Kilogram CurbMassExtra { get; }
		public Kilogram Loading { get; }
		public Meter DynamicTyreRadius { get; }
		public bool Articulated { get; }
		public Meter Height { get; }
		public Meter Length { get; set; }
		public Meter Width { get; set; }
		public Meter EntranceHeight { get; }
		public Watt MaxNetPower2 { get; }
		public RegistrationClass RegisteredClass { get; set; }
		public int NuberOfPassengersUpperDeck { get; set; }
		public int NumberOfPassengersLowerDeck { get; set; }
		public VehicleCode VehicleCode { get; set; }
		public FloorType FloorType { get; }

		IVehicleComponentsDeclaration IVehicleDeclarationInputData.Components
		{
			get { return _components; }
		}

		public XmlNode XMLSource { get; }

		public IVehicleComponentsEngineering Components { get { return this; } }
		public string Identifier { get; }
		public bool ExemptedVehicle { get; }
		public string VIN { get; }
		public LegislativeClass LegislativeClass { get; }
		public VehicleCategory VehicleCategory { get; }
		public AxleConfiguration AxleConfiguration { get; }
		public Kilogram CurbMassChassis { get; }
		public Kilogram GrossVehicleMassRating { get; }
		public IList<ITorqueLimitInputData> TorqueLimits { get { return new List<ITorqueLimitInputData>(); } }
		public string ManufacturerAddress { get; }
		public PerSecond EngineIdleSpeed { get; }
		public bool VocationalVehicle { get; }
		public bool SleeperCab { get; }
		public TankSystem? TankSystem { get; }

		IAdvancedDriverAssistantSystemDeclarationInputData IVehicleDeclarationInputData.ADAS
		{
			get { return _adas; }
		}

		public bool ZeroEmissionVehicle { get; }
		public bool HybridElectricHDV { get; }
		public bool DualFuelVehicle { get; }
		public Watt MaxNetPower1 { get; }
		public IAdvancedDriverAssistantSystemsEngineering ADAS { get; }

		#endregion

		#region Implementation of IVehicleComponentsEngineering

		public IAirdragEngineeringInputData AirdragInputData { get; set; }
		public IGearboxEngineeringInputData GearboxInputData { get; set; }
		public ITorqueConverterEngineeringInputData TorqueConverterInputData { get; set; }
		public IAxleGearInputData AxleGearInputData { get; set; }
		public IAngledriveInputData AngledriveInputData { get; set; }
		public IEngineEngineeringInputData EngineInputData { get; set; }
		public IAuxiliariesEngineeringInputData AuxiliaryInputData { get; set; }
		public IRetarderInputData RetarderInputData { get; set; }
		public IPTOTransmissionInputData PTOTransmissionInputData { get; set; }
		public IAxlesEngineeringInputData AxleWheels { get; set; }

		#endregion
	}
}