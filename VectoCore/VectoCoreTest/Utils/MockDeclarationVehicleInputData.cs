using System;
using System.Collections.Generic;
using System.Xml;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCore.Tests.Utils
{
	public class MockDeclarationVehicleInputData : IVehicleDeclarationInputData, IVehicleComponentsDeclaration
	{
		#region Implementation of IComponentInputData

		public DataSource DataSource { get; }
		public bool SavedInDeclarationMode { get; }
		public string Manufacturer { get; }
		public string Model { get; }
		public DateTime Date { get; }
		public string AppVersion => "Mock-Class";
		public CertificationMethod CertificationMethod { get; }
		public string CertificationNumber { get; }
		public DigestData DigestValue { get; }

		#endregion

		#region Implementation of IVehicleDeclarationInputData

		public string Identifier { get; }
		public bool ExemptedVehicle { get; }
		public string VIN { get; }
		public LegislativeClass? LegislativeClass { get; }
		public VehicleCategory VehicleCategory { get; }
		public AxleConfiguration AxleConfiguration { get; }
		public Kilogram CurbMassChassis { get; }
		public Kilogram GrossVehicleMassRating { get; }
		public IList<ITorqueLimitInputData> TorqueLimits => new List<ITorqueLimitInputData>();
		public string ManufacturerAddress { get; }
		public string SimulationToolLicenseNumber { get; }
        public string VehicleMonitoringData { get; }
        public Kilogram H2StorageUsableCapacity { get; }
        public HydrogenStorageTechnology? HydrogenStorageTechnology { get; }
        public bool BatteryOnlyMode { get; }
        public DynamicChargingTechnology DynamicChargingTechnology { get; }
        public PerSecond EngineIdleSpeed { get; }
		public bool VocationalVehicle { get; }
		public bool? SleeperCab { get; }
		public bool? AirdragModifiedMultistep { get; }
		public TankSystem? TankSystem { get; }
		public IAdvancedDriverAssistantSystemDeclarationInputData ADAS { get; }
		public IVehicleInMotionChargingDeclaration InMotionCharging { get; }
		public bool ZeroEmissionVehicle { get; }
		public bool HybridElectricHDV { get; }
		public bool DualFuelVehicle { get; }
		public Watt MaxNetPower1 { get; }
		public string ExemptedTechnology { get; }
		public RegistrationClass? RegisteredClass { get; set; }
		public int? NumberPassengerSeatsUpperDeck { get; set; }
		public int? NumberPassengerSeatsLowerDeck { get; set; }
		public int? NumberPassengersStandingLowerDeck { get; set; }
		public int? NumberPassengersStandingUpperDeck { get; set; }
		public CubicMeter CargoVolume { get; }
		public VehicleCode? VehicleCode { get; set; }
		public bool? LowEntry { get; }
		public bool Articulated { get; }
		public Meter Height { get; set; }
		public Meter Length { get; set; }
		public Meter Width { get; set; }
		public Meter EntranceHeight { get; }
		public ConsumerTechnology? DoorDriveTechnology { get; }
		public VehicleDeclarationType VehicleDeclarationType { get; }
		public IDictionary<EMPlacement, IList<Tuple<Volt, TableData>>> ElectricMotorTorqueLimits { get; }
		public TableData BoostingLimitations { get; }
		public IVehicleComponentsDeclaration Components => this;
		public XmlNode XMLSource { get; }
		public string VehicleTypeApprovalNumber { get; }
		public ArchitectureID ArchitectureID { get; }
		public ArchitectureID ArchitectureIDPwt2 {  get; }

        public bool OVC { get; }
		public Watt MaxChargingPower { get; }
		public VectoSimulationJobType VehicleType { get; }

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
		public IElectricStorageSystemDeclarationInputData ElectricStorage { get; set; }
		public IElectricMachinesDeclarationInputData ElectricMachines { get; set; }
		public IIEPCDeclarationInputData IEPC { get; set; }
		public IFuelCellSystemDeclarationInputData FuelCellSystem { get; set; }
        public IList<IAxlePowertrainDeclarationInputData> AxlePowertrainInputData { get; set; }
        public ElectricMachineEntry<IElectricMotorDeclarationInputData> Generator { get; set; }
		
		#endregion
    }

	public class MockEngineeringVehicleInputData : IVehicleEngineeringInputData, IVehicleComponentsEngineering
	{
		#region Implementation of IComponentInputData

		public DataSource DataSource { get; }
		public bool SavedInDeclarationMode { get; }
		public string Manufacturer { get; }
		public string Model { get; }
		public DateTime Date { get; }
		public string AppVersion => "Mock-Class";
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
		public TableData ElectricMotorTorqueLimits { get; }
		public TableData BoostingLimitations { get; set; }
		public Meter Length { get; set; }
		public Meter Width { get; set; }
		public Meter EntranceHeight { get; }
		public ConsumerTechnology? DoorDriveTechnology { get; }
		public VehicleDeclarationType VehicleDeclarationType { get; }

		public string ExemptedTechnology { get; }
		public RegistrationClass? RegisteredClass { get; set; }
		public int? NumberPassengerSeatsUpperDeck { get; set; }
		public int? NumberPassengerSeatsLowerDeck { get; set; }
		public int? NumberPassengersStandingLowerDeck { get; set; }
		public int? NumberPassengersStandingUpperDeck { get; set; }
		public CubicMeter CargoVolume { get; }
		public VehicleCode? VehicleCode { get; set; }
		public bool? LowEntry { get; }

		IVehicleComponentsDeclaration IVehicleDeclarationInputData.Components => null;

		public XmlNode XMLSource { get; }
		public string VehicleTypeApprovalNumber { get; }
		public ArchitectureID ArchitectureID { get; }
		public ArchitectureID ArchitectureIDPwt2 { get; }

        public bool OVC { get; }
		public Watt MaxChargingPower { get; }

		public IVehicleComponentsEngineering Components => this;
		public string Identifier { get; }
		public bool ExemptedVehicle { get; }
		public string VIN { get; }
		public LegislativeClass? LegislativeClass { get; }
		public VehicleCategory VehicleCategory { get; }
		public AxleConfiguration AxleConfiguration { get; }
		public Kilogram CurbMassChassis { get; }
		public Kilogram GrossVehicleMassRating { get; }
		public IList<ITorqueLimitInputData> TorqueLimits => new List<ITorqueLimitInputData>();
		public string ManufacturerAddress { get; }
		public PerSecond EngineIdleSpeed { get; }
		public bool VocationalVehicle { get; }
		public bool? SleeperCab { get; }
		public bool? AirdragModifiedMultistep { get; }
		public TankSystem? TankSystem { get; }
		public string SimulationToolLicenseNumber { get; }
        public string VehicleMonitoringData { get; }
        public Kilogram H2StorageUsableCapacity { get; }
        public HydrogenStorageTechnology? HydrogenStorageTechnology { get; }
        public bool BatteryOnlyMode { get; }
        public DynamicChargingTechnology DynamicChargingTechnology { get; }
        IAdvancedDriverAssistantSystemDeclarationInputData IVehicleDeclarationInputData.ADAS => null;
		IVehicleInMotionChargingDeclaration IVehicleDeclarationInputData.InMotionCharging => InMotionCharging;

		public double InitialSOC { get; }
		public IVehicleInMotionChargingEngineering InMotionCharging { get; }
		public VectoSimulationJobType VehicleType { get; }
		public GearshiftPosition PTO_DriveGear { get; }
		public PerSecond PTO_DriveEngineSpeed { get; }

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
		public IElectricStorageSystemEngineeringInputData ElectricStorage { get; set; }
		public IElectricMachinesEngineeringInputData ElectricMachines { get; set; }
		public IIEPCEngineeringInputData IEPCEngineeringInputData { get; }

		public IFuelCellSystemEngineeringInputData FuelCellSystemInputData { get; }

		public IList<IAxlePowertrainEngineeringInputData> AxlePowertrainEngineeringInputData { get; }

		IDictionary<EMPlacement, IList<Tuple<Volt, TableData>>> IVehicleDeclarationInputData.ElectricMotorTorqueLimits => throw new NotImplementedException();

		#endregion
	}
}