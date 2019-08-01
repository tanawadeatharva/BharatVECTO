using System.Collections.Generic;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCore.Tests.Utils {
	public class MockVehicleInputData : IVehicleDeclarationInputData, IVehicleComponentsDeclaration
	{
		#region Implementation of IComponentInputData

		public DataSource DataSource { get; }
		public bool SavedInDeclarationMode { get; }
		public string Manufacturer { get; }
		public string Model { get; }
		public string Date { get; }
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
		public IVehicleComponentsDeclaration Components { get { return this; } }

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

		#endregion
	}
}