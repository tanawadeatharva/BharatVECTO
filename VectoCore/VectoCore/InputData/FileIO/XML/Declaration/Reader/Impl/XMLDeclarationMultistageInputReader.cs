using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Castle.Core.Internal;
using Ninject;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Factory;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using TUGraz.VectoCore.OutputData.XML;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Reader.Impl
{
	public class XMLDeclarationMultistageInputReaderV01 : AbstractComponentReader, IXMLDeclarationMultistageVehicleInputDataReader
	{

		public static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_MULTISTAGE_BUS_VEHICLE_NAMESPACE_VO1;

		public const string XSD_TYPE = "VectoOutputMultistageType";

		public static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		[Inject]
		public IDeclarationInjectFactory Factory { protected get; set; }

		protected IDeclarationMultistageJobInputData _jobData;

		protected IXMLMultistageInputDataProvider InputData;

		protected XmlNode JobNode;

		public XMLDeclarationMultistageInputReaderV01(IXMLMultistageInputDataProvider inputData, XmlNode baseNode)
			: base(inputData, baseNode)
		{
			JobNode = baseNode;
			InputData = inputData;
		}

		public IDeclarationMultistageJobInputData JobData
		{
			get { return _jobData ?? (_jobData = CreateComponent(XMLNames.VectoOutputMultistage, JobCreator)); }
		}

		protected virtual IDeclarationMultistageJobInputData JobCreator(string version, XmlNode node, string arg3)
		{
			var job = Factory.CreateMultiStageJobData(version, BaseNode, InputData, (InputData as IXMLResource).DataSource.SourceFile);
			job.Reader = Factory.CreateMultistageJobReader(version, job, JobNode);
			return job;

		}
	}

	// ---------------------------------------------------------------------------------------

	public class XMLMultistageJobReaderV01 : AbstractComponentReader, IXMLMultistageJobReader
	{
		public static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_MULTISTAGE_BUS_VEHICLE_NAMESPACE_VO1;

		public const string XSD_TYPE = "VectoOutputMultistageType";

		public static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		protected IXMLDeclarationMultistageJobInputData InputData;
		protected IPrimaryVehicleInformationInputDataProvider _primaryVehicle;
		protected IList<IManufacturingStageInputData> _manufacturingStages;
		protected ConsolidateManufacturingStages _consolidateManufacturingStages;

		private XmlNodeList _manufacturingNodeStages;

		[Inject]
		public IDeclarationInjectFactory Factory { protected get; set; }



		public XMLMultistageJobReaderV01(IXMLDeclarationMultistageJobInputData inputData, XmlNode baseNode)
			: base(inputData, baseNode)
		{
			InputData = inputData;
			SetManufacturingStageNodes();
		}

		public IPrimaryVehicleInformationInputDataProvider PrimaryVehicle
		{
			get { return _primaryVehicle ?? (_primaryVehicle = CreateComponent(XMLNames.Bus_PrimaryVehicle, PrimaryVehicleCreator)); }
		}

		protected IPrimaryVehicleInformationInputDataProvider PrimaryVehicleCreator(string version, XmlNode node,
			string arg3)
		{
			var primaryVehicle = Factory.CreatePrimaryMultistageVehicleData(version, node, arg3);
			primaryVehicle.Reader = Factory.CreatePrimaryVehicleBusInputReader(version, primaryVehicle, node.FirstChild);
			return primaryVehicle;
		}

		public IList<IManufacturingStageInputData> ManufacturingStages
		{
			get
			{
				if (_manufacturingNodeStages.IsNullOrEmpty())
					return null;

				return _manufacturingStages ?? (_manufacturingStages = ManufacturingStagesCreator());
			}
		}

		private IList<IManufacturingStageInputData> ManufacturingStagesCreator()
		{
			var stages = new List<IManufacturingStageInputData>();

			foreach (XmlNode manufacturingNodeStage in _manufacturingNodeStages)
			{
				var version = XMLHelper.GetXsdType(manufacturingNodeStage?.SchemaInfo.SchemaType);
				stages.Add(ManufacturingStageCreator(version, manufacturingNodeStage));
			}
			return stages;
		}

		protected IManufacturingStageInputData ManufacturingStageCreator(string version, XmlNode node)
		{
			var stage = Factory.CreateMultistageData(version, node, null);
			stage.Reader = Factory.CreateMultistageDataReader(version, stage, node);
			return stage;
		}

		public IManufacturingStageInputData ConsolidateManufacturingStage
		{
			get
			{
				if (ManufacturingStages.IsNullOrEmpty())
					return null;

				return _consolidateManufacturingStages ??
						(_consolidateManufacturingStages = GetConsolidateManufacturingStage());
			}
		}

		public VectoSimulationJobType JobType
		{
			get { return InputData.JobType; }
		}

		public bool InputComplete
		{
			get
			{
				if (ManufacturingStages.IsNullOrEmpty())
					return false;

				if (_consolidateManufacturingStages == null)
					_consolidateManufacturingStages = GetConsolidateManufacturingStage();

				return _consolidateManufacturingStages.IsInputDataComplete(JobType);
			}
		}

		private ConsolidateManufacturingStages GetConsolidateManufacturingStage()
		{
			return new ConsolidateManufacturingStages(PrimaryVehicle, ManufacturingStages.Reverse());
		}


		private void SetManufacturingStageNodes()
		{
			_manufacturingNodeStages = BaseNode.SelectNodes(XMLHelper.QueryLocalName(XMLNames.ManufacturingStage));
		}
	}

	// ---------------------------------------------------------------------------------------

	public class XMLMultistageEntryReaderV01 : AbstractComponentReader, IXMLMultistageReader
	{
		public static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_MULTISTAGE_BUS_VEHICLE_NAMESPACE_VO1;

		public const string XSD_TYPE = "ManufacturingStageType";

		public static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);


		protected readonly XmlNode JobNode;
		protected readonly IXMLMultistageEntryInputDataProvider _multistageData;
		protected IApplicationInformation _applicationInformation;
		protected IVehicleDeclarationInputData _vehicle;

		[Inject]
		public IDeclarationInjectFactory Factory { protected get; set; }

		public XMLMultistageEntryReaderV01(IXMLMultistageEntryInputDataProvider multistageData, XmlNode node) : base(
			multistageData, node)
		{
			JobNode = node;
			_multistageData = multistageData;
		}

		public IVehicleDeclarationInputData Vehicle
		{
			get { return _vehicle ?? (_vehicle = CreateComponent(XMLNames.Tag_Vehicle, VehicleCreator)); }
		}

		private IVehicleDeclarationInputData VehicleCreator(string version, XmlNode node, string arg3)
		{
			var vehicle = Factory.CreateVehicleData(version, null, node, arg3);

			if (vehicle.ComponentNode != null)
				vehicle.ComponentReader = GetReader(vehicle, vehicle.ComponentNode, Factory.CreateComponentReader);

			if (vehicle.ADASNode != null)
				vehicle.ADASReader = GetReader(vehicle, vehicle.ADASNode, Factory.CreateADASReader);

			return vehicle;
		}

		public IApplicationInformation ApplicationInformation
		{
			get
			{
				return _applicationInformation ??
						(_applicationInformation = CreateComponent(XMLNames.Tag_ApplicationInformation, ApplicationCreator));
			}
		}

		protected IApplicationInformation ApplicationCreator(string version, XmlNode node, string agr3)
		{
			return Factory.CreateApplicationInformationReader(version, node);
		}

		public DigestData GetDigestData(XmlNode xmlNode)
		{
			return xmlNode == null ? null : new DigestData(xmlNode);
		}
	}

	// ---------------------------------------------------------------------------------------


	public class XMLMultistagePrimaryVehicleReaderV01 : AbstractComponentReader, IXMLDeclarationPrimaryVehicleBusInputDataReader
	{

		public static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_MULTISTAGE_BUS_VEHICLE_NAMESPACE_VO1;

		public const string XSD_TYPE = "PrimaryVehicleDataType";

		public static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		protected XmlNode JobNode;
		protected IDeclarationJobInputData _jobData;
		protected IXMLPrimaryVehicleBusInputData _primaryInputData;
		protected IApplicationInformation _applicationInformation;
		protected IResultsInputData _resultsInputData;

		[Inject]
		public IDeclarationInjectFactory Factory { protected get; set; }


		public XMLMultistagePrimaryVehicleReaderV01(IXMLPrimaryVehicleBusInputData inputData, XmlNode baseNode) : base(inputData, baseNode)
		{
			JobNode = baseNode;
			_primaryInputData = inputData;
		}

		public virtual IDeclarationJobInputData JobData
		{
			get
			{
				return _jobData ?? (_jobData = CreateComponent(XMLNames.Tag_Vehicle, JobCreator));
			}
		}


		protected IDeclarationJobInputData JobCreator(string version, XmlNode node, string arg3)
		{
			var job = Factory.CreatePrimaryVehicleJobData(version, BaseNode, _primaryInputData,
				(_primaryInputData as IXMLResource).DataSource.SourceFile);
			job.Reader = Factory.CreatePrimaryVehicleJobReader(version, job, JobNode);
			return job;
		}

		public IResultsInputData ResultsInputData
		{
			get
			{
				return _resultsInputData ??
					   (_resultsInputData = CreateComponent(XMLNames.Report_Results, ResultsInputDataCreator));
			}
		}

		protected IResultsInputData ResultsInputDataCreator(string version, XmlNode node, string arg3)
		{
			return Factory.CreateResultsInputDataReader(version, node);
		}

		public DigestData GetDigestData(XmlNode xmlNode)
		{
			return xmlNode == null ? null : new DigestData(xmlNode);
		}

		protected IApplicationInformation ApplicationCreator(string version, XmlNode node, string agr3)
		{
			return Factory.CreateApplicationInformationReader(version, node);
		}

		public IApplicationInformation ApplicationInformation
		{
			get
			{
				return _applicationInformation ??
					  (_applicationInformation = CreateComponent(XMLNames.Tag_ApplicationInformation, ApplicationCreator));
			}
		}
	}

	// ---------------------------------------------------------------------------------------


	#region  Generate Consolidated Multistage InputData
	
	public abstract class ConsolidatedDataBase 
	{
		protected readonly IEnumerable<IManufacturingStageInputData> _manufacturingStages;
		protected string InvalidEntry { get; private set; }

		public ConsolidatedDataBase(IEnumerable<IManufacturingStageInputData> manufacturingStages)
		{
			_manufacturingStages = manufacturingStages;
		}

		protected T GetPropertyValue<T>(object obj, string name)
		{
			var propertyValue = GetPropertyValue(obj, name);
			if (propertyValue == null)
				return default;

			return (T)propertyValue;
		}

		protected object GetPropertyValue(object obj, string name)
		{
			foreach (var part in name.Split('.'))
			{
				if (obj == null) { return null; }

				var type = obj.GetType();
				var info = type.GetProperty(part);
				if (info == null) { return null; }

				obj = info.GetValue(obj, null);
			}
			return obj;
		}

		public abstract bool IsInputDataComplete(VectoSimulationJobType jobType);
		
		public abstract string GetInvalidEntry();

		protected bool InputComplete<T>(T value, string variableName)
		{
			if (value != null)
				return true;

			InvalidEntry  = variableName; 
			return false;
		}
	}

	// ---------------------------------------------------------------------------------------

	public class ConsolidateManufacturingStages : ConsolidatedDataBase, IManufacturingStageInputData
	{
		private ConsolidatedVehicleData _consolidatedVehicleData;
		private IPrimaryVehicleInformationInputDataProvider _primaryVehicle;
		
		public ConsolidateManufacturingStages(IPrimaryVehicleInformationInputDataProvider primaryVehicle, 
			IEnumerable<IManufacturingStageInputData> manufacturingStages) : base(manufacturingStages)
		{
			_primaryVehicle = primaryVehicle;
		}

		public DigestData HashPreviousStage
		{
			get { return _manufacturingStages.First().HashPreviousStage; }
		}

		public int StageCount
		{
			get { return _manufacturingStages.First().StageCount; }
		}

		public IVehicleDeclarationInputData Vehicle
		{
			get { return GetConsolidatedVehicleData(); }
		}

		public IApplicationInformation ApplicationInformation
		{
			get { return _manufacturingStages.First().ApplicationInformation; }
		}

		public DigestData Signature
		{
			get { return _manufacturingStages.First().Signature; }
		}

		public override bool IsInputDataComplete(VectoSimulationJobType jobType)
		{
			return GetConsolidatedVehicleData().IsInputDataComplete(jobType);
		}

		public override string GetInvalidEntry()
		{
			return _consolidatedVehicleData.GetInvalidEntry();
		}

		private ConsolidatedVehicleData GetConsolidatedVehicleData()
		{
			return _consolidatedVehicleData ??
					(_consolidatedVehicleData = new ConsolidatedVehicleData(_primaryVehicle, _manufacturingStages));
		}

	}

	// ---------------------------------------------------------------------------------------

	public class ConsolidatedVehicleData : ConsolidatedDataBase, IVehicleDeclarationInputData
	{
		private ConsolidatedADASData _consolidatedADAS;
		private ConsolidatedComponentData _consolidatedComponents;
		private readonly IPrimaryVehicleInformationInputDataProvider _primaryVehicle;

		public ConsolidatedVehicleData(IPrimaryVehicleInformationInputDataProvider primaryVehicle,
			IEnumerable<IManufacturingStageInputData> manufacturingStages) : base(manufacturingStages)
		{
			_primaryVehicle = primaryVehicle;
		}

		#region ManufacturingStage mandatory properties

		public string Manufacturer
		{
			get { return _manufacturingStages.First().Vehicle.Manufacturer; }
		}

		public string ManufacturerAddress
		{
			get { return _manufacturingStages.First().Vehicle.ManufacturerAddress; }
		}
		
		public DateTime Date
		{
			get { return _manufacturingStages.First().Vehicle.Date; }
		}

		public string VIN
		{
			get { return _manufacturingStages.First().Vehicle.VIN; }
		}

		public string LegislativeCategory
		{
			get { return null; }
		}

		public VehicleDeclarationType VehicleDeclarationType
		{
			get { return _manufacturingStages.First().Vehicle.VehicleDeclarationType; }
		}

		#endregion

		#region ManufacturingStage optional properties

		public string Model
		{
			get { return GetVehiclePropertyValue<string>(nameof(Model)); }
		}

		public LegislativeClass? LegislativeClass
		{
			get { return GetVehiclePropertyValue<LegislativeClass?>(nameof(LegislativeClass)); }
		}

		public Kilogram CurbMassChassis
		{
			get { return GetVehiclePropertyValue<Kilogram>(nameof(CurbMassChassis)); }
		}

		public Kilogram GrossVehicleMassRating
		{
			get
			{
				return GetVehiclePropertyValue<Kilogram>(nameof(GrossVehicleMassRating));
			}
		}

		public bool? AirdragModifiedMultistage
		{
			get
			{
				return GetVehiclePropertyValue<bool?>(nameof(AirdragModifiedMultistage));
			}
		}

		public TankSystem? TankSystem
		{
			get { return GetVehiclePropertyValue<TankSystem?>(nameof(TankSystem)); }
		}

		public RegistrationClass? RegisteredClass
		{
			get { return GetVehiclePropertyValue<RegistrationClass?>(nameof(RegisteredClass)); }
		}


		public int? NumberOfPassengersUpperDeck
		{
			get { return GetVehiclePropertyValue<int?>(nameof(NumberOfPassengersUpperDeck)); }
		}

		public int? NumberOfPassengersLowerDeck
		{
			get { return GetVehiclePropertyValue<int?>(nameof(NumberOfPassengersLowerDeck)); }
		}

		public int? NumberOfPassengersStandingLowerDeck
		{
			get { return GetVehiclePropertyValue<int?>(nameof(NumberOfPassengersStandingLowerDeck)); }
		}

		public int? NumberOfPassengersStandingUpperDeck
		{
			get { return GetVehiclePropertyValue<int?>(nameof(NumberOfPassengersStandingUpperDeck)); }
		}

		public VehicleCode? VehicleCode
		{
			get { return GetVehiclePropertyValue<VehicleCode?>(nameof(VehicleCode)); }
		}

		public bool? LowEntry
		{
			get { return GetVehiclePropertyValue<bool?>(nameof(LowEntry)); }
		}

		public Meter Height
		{
			get { return GetVehiclePropertyValue<Meter>(nameof(Height)); }
		}

		public Meter Length
		{
			get { return GetVehiclePropertyValue<Meter>(nameof(Length)); }
		}

		public Meter Width
		{
			get { return GetVehiclePropertyValue<Meter>(nameof(Width)); }
		}

		public Meter EntranceHeight
		{
			get { return GetVehiclePropertyValue<Meter>(nameof(EntranceHeight)); }
		}

		public ConsumerTechnology? DoorDriveTechnology
		{
			get { return GetVehiclePropertyValue<ConsumerTechnology?>(nameof(DoorDriveTechnology)); }

		}

		public IAdvancedDriverAssistantSystemDeclarationInputData ADAS
		{
			get { return GetADAS(); }
		}

		private IAdvancedDriverAssistantSystemDeclarationInputData GetADAS()
		{
			if (GetVehiclePropertyValue<IAdvancedDriverAssistantSystemDeclarationInputData>(nameof(ADAS)) == null)
				return null;

			return _consolidatedADAS
					?? (_consolidatedADAS = new ConsolidatedADASData(_manufacturingStages));
		}


		public IVehicleComponentsDeclaration Components
		{
			get { return GetComponents(); }
		}

		private IVehicleComponentsDeclaration GetComponents()
		{
			if (GetVehiclePropertyValue<IVehicleComponentsDeclaration>(nameof(Components)) == null)
				return null;

			return _consolidatedComponents
					?? (_consolidatedComponents = new ConsolidatedComponentData(_manufacturingStages));
		}

		#endregion

		#region Non set IVehicleDeclarationInputData interface properties

		public DataSource DataSource { get; }
		public bool SavedInDeclarationMode { get; }
		public string AppVersion { get; }
		public CertificationMethod CertificationMethod { get; }
		public string CertificationNumber { get; }
		public DigestData DigestValue { get; }
		public string Identifier { get; }
		public bool ExemptedVehicle { get; }
		public VehicleCategory VehicleCategory { get; }
		public AxleConfiguration AxleConfiguration { get; }
		public IList<ITorqueLimitInputData> TorqueLimits { get; }

		public PerSecond EngineIdleSpeed { get; }
		public bool VocationalVehicle { get; }
		public bool SleeperCab { get; }
		public bool ZeroEmissionVehicle { get; }
		public bool HybridElectricHDV { get; }
		public bool DualFuelVehicle { get; }
		public Watt MaxNetPower1 { get; }
		public Watt MaxNetPower2 { get; }
		public CubicMeter CargoVolume { get; }
		public bool Articulated { get; }

		public XmlNode XMLSource { get; }

		#endregion

		private T GetVehiclePropertyValue<T>(string propertyName)
		{
			foreach (var manufacturingStage in _manufacturingStages) {
				var value = GetPropertyValue<T>(manufacturingStage.Vehicle, propertyName);
				if (value != null)
					return value;
			}
			return default;
		}

		private bool PrimaryEngineWithNGTankSystem()
		{
			var enginePrimaryEngine = _primaryVehicle?.Vehicle?.Components?.EngineInputData;
			if (enginePrimaryEngine == null)
				return false;

			foreach (var engineMode in enginePrimaryEngine.EngineModes)
			{
				foreach (var fuel in engineMode.Fuels)
				{
					if (fuel.FuelType == FuelType.NGPI || fuel.FuelType == FuelType.NGCI)
						return true;
				}
			}

			return false;
		}
		

		private bool IsTankSystemValid()
		{
			if (PrimaryEngineWithNGTankSystem())
				return TankSystem != null;
			return true;
		}

		private bool IsAirdragEntriesValid()
		{
			var checkAirdragModified = false;
			var validAirdragEntries = true;

			var stages = _manufacturingStages.Reverse();
			foreach (var manufacturingStage in stages) {
				if (manufacturingStage.Vehicle?.Components?.AirdragInputData != null && !checkAirdragModified ) {
					checkAirdragModified = true;
					continue;
				}
				if (checkAirdragModified && manufacturingStage.Vehicle?.AirdragModifiedMultistage == null) 
					validAirdragEntries = false;
			}

			return validAirdragEntries;
		}

		public override bool IsInputDataComplete(VectoSimulationJobType jobType)
		{
			GetADAS();
			GetComponents();
			
			return  InputComplete(Model, nameof(Model)) 
					&& InputComplete(LegislativeClass, nameof(LegislativeClass)) 
					&& InputComplete(CurbMassChassis, nameof(CurbMassChassis)) 
					&& InputComplete(GrossVehicleMassRating, nameof(GrossVehicleMassRating))
					&& InputComplete(IsAirdragEntriesValid(), nameof(IsAirdragEntriesValid)) 
					&& InputComplete(IsTankSystemValid(), nameof(IsTankSystemValid))
					&& InputComplete(RegisteredClass, nameof(RegisteredClass))
					&& InputComplete(NumberOfPassengersLowerDeck, nameof(NumberOfPassengersLowerDeck))
					&& InputComplete(NumberOfPassengersUpperDeck, nameof(NumberOfPassengersUpperDeck))
					&& InputComplete(NumberOfPassengersStandingLowerDeck, nameof(NumberOfPassengersStandingLowerDeck))
					&& InputComplete(NumberOfPassengersStandingUpperDeck, nameof(NumberOfPassengersStandingUpperDeck))
					&& InputComplete(VehicleCode, nameof(VehicleCode))
					&& InputComplete(LowEntry, nameof(LowEntry)) && InputComplete(Height, nameof(Height)) 
					&& InputComplete(Length, nameof(Length)) && InputComplete(Width, nameof(Width)) 
					&& InputComplete(EntranceHeight, nameof(EntranceHeight))  
					&& InputComplete(DoorDriveTechnology, nameof(DoorDriveTechnology)) 
					&& InputComplete(_consolidatedADAS, nameof(_consolidatedADAS))
					&& _consolidatedADAS.IsInputDataComplete(jobType)
					&& InputComplete(_consolidatedComponents, nameof(_consolidatedComponents))
					&& _consolidatedComponents.IsInputDataComplete(jobType);
		}

		public override string GetInvalidEntry()
		{
			if (InvalidEntry != null)
				return InvalidEntry;
			
			if (_consolidatedADAS?.GetInvalidEntry() != null)
				return _consolidatedADAS.GetInvalidEntry();

			if (_consolidatedComponents?.GetInvalidEntry() != null)
				return _consolidatedComponents.GetInvalidEntry(); 

			return null;
		}
	}

	// ---------------------------------------------------------------------------------------

	public class ConsolidatedADASData : ConsolidatedDataBase, IAdvancedDriverAssistantSystemDeclarationInputData
	{
		public ConsolidatedADASData(IEnumerable<IManufacturingStageInputData> manufacturingStages)
			: base(manufacturingStages) { }

		public bool EngineStopStart
		{
			get { return GetADASPropertyValue<bool>(nameof(EngineStopStart)); }
		}

		public EcoRollType EcoRoll
		{
			get { return GetADASPropertyValue<EcoRollType>(nameof(EcoRoll)); }
		}

		public PredictiveCruiseControlType PredictiveCruiseControl
		{
			get { return GetADASPropertyValue<PredictiveCruiseControlType>(nameof(PredictiveCruiseControl)); }
		}

		public bool? ATEcoRollReleaseLockupClutch
		{
			get { return GetADASPropertyValue<bool?>(nameof(ATEcoRollReleaseLockupClutch)); }
		}

		public XmlNode XMLSource { get; }


		private T GetADASPropertyValue<T>(string propertyName)
		{
			foreach (var manufacturingStage in _manufacturingStages) {
				var adas = manufacturingStage.Vehicle.ADAS;
				if (adas == null)
					continue;
				var value = GetPropertyValue<T>(adas, propertyName);
				if (value != null)
					return value;
			}

			return default;
		}

		public override bool IsInputDataComplete(VectoSimulationJobType jobType)
		{
			return InputComplete(ATEcoRollReleaseLockupClutch, nameof(ATEcoRollReleaseLockupClutch));
		}

		public override string GetInvalidEntry()
		{
			return InvalidEntry;
		}
	}

	// ---------------------------------------------------------------------------------------

	public class ConsolidatedComponentData : ConsolidatedDataBase, IVehicleComponentsDeclaration
	{
		private ConsolidatedAirdragData _consolidateAirdragData;
		private ConsolidatedBusAuxiliariesData _consolidateBusAuxiliariesData;

		public ConsolidatedComponentData(IEnumerable<IManufacturingStageInputData> manufacturingStages)
			: base(manufacturingStages) { }


		public IAirdragDeclarationInputData AirdragInputData
		{
			get { return GetAirdragInputData(); }
		}

		private IAirdragDeclarationInputData GetAirdragInputData()
		{
			if (GetComponentPropertyValue<IAirdragDeclarationInputData>(nameof(AirdragInputData)) == null)
				return null;

			return _consolidateAirdragData ??
					(_consolidateAirdragData = new ConsolidatedAirdragData(_manufacturingStages));
		}

		public IGearboxDeclarationInputData GearboxInputData
		{
			get { return null; }
		}
		public ITorqueConverterDeclarationInputData TorqueConverterInputData
		{
			get { return null; }
		}
		public IAxleGearInputData AxleGearInputData
		{
			get { return null; }
		}
		public IAngledriveInputData AngledriveInputData
		{
			get { return null; }
		}
		public IEngineDeclarationInputData EngineInputData
		{
			get { return null; }
		}
		public IAuxiliariesDeclarationInputData AuxiliaryInputData
		{
			get { return null; }
		}
		public IRetarderInputData RetarderInputData
		{
			get { return null; }
		}
		public IPTOTransmissionInputData PTOTransmissionInputData
		{
			get { return null; }
		}
		public IAxlesDeclarationInputData AxleWheels
		{
			get { return null; }
		}
		
		public IBusAuxiliariesDeclarationData BusAuxiliaries
		{
			get { return GetBusAuxiliaries(); }
		}

		private IBusAuxiliariesDeclarationData GetBusAuxiliaries()
		{
			if (GetComponentPropertyValue<IBusAuxiliariesDeclarationData>(nameof(BusAuxiliaries)) == null)
				return null;

			return _consolidateBusAuxiliariesData ??
					(_consolidateBusAuxiliariesData = new ConsolidatedBusAuxiliariesData(_manufacturingStages));
		}


		public IElectricStorageDeclarationInputData ElectricStorage
		{
			get { return null; }
		}
		public IElectricMachinesDeclarationInputData ElectricMachines
		{
			get { return null; }
		}

		private T GetComponentPropertyValue<T>(string propertyName)
		{

			foreach (var manufacturingStage in _manufacturingStages) {
				var component = manufacturingStage.Vehicle?.Components;
				if (component == null)
					continue;
				var value = GetPropertyValue<T>(component, propertyName);
				if (value != null)
					return value;
			}

			return default;
		}


		public override bool IsInputDataComplete(VectoSimulationJobType jobType)
		{
			GetAirdragInputData();
			GetBusAuxiliaries();

			return InputComplete(_consolidateBusAuxiliariesData, nameof(_consolidateBusAuxiliariesData)) 
					&& _consolidateBusAuxiliariesData.IsInputDataComplete(jobType);
		}

		public override string GetInvalidEntry()
		{
			if (InvalidEntry != null)
				return InvalidEntry;
			
			if (_consolidateAirdragData?.GetInvalidEntry() != null)
				return _consolidateAirdragData.GetInvalidEntry();

			if (_consolidateBusAuxiliariesData?.GetInvalidEntry() != null)
				return _consolidateBusAuxiliariesData.GetInvalidEntry(); 

			return InvalidEntry;
		}
	}

	// ---------------------------------------------------------------------------------------

	public class ConsolidatedAirdragData : ConsolidatedDataBase, IAirdragDeclarationInputData
	{
		public IAirdragDeclarationInputData AirdragEntry { private set; get; }

		public ConsolidatedAirdragData(IEnumerable<IManufacturingStageInputData> manufacturingStages)
			: base(manufacturingStages)
		{
			SetLastValidAirdragEntry();
		}

		public string Manufacturer
		{
			get { return AirdragEntry?.Manufacturer; }
		}

		public string Model
		{
			get { return AirdragEntry?.Model; }
		}

		public DateTime Date
		{
			get { return AirdragEntry.Date; }
		}

		public string AppVersion
		{
			get { return AirdragEntry?.AppVersion; }
		}
		public CertificationMethod CertificationMethod
		{
			get { return AirdragEntry.CertificationMethod; }
		}
		public string CertificationNumber
		{
			get { return AirdragEntry?.CertificationNumber; }
		}
		public DigestData DigestValue
		{
			get { return AirdragEntry?.DigestValue; }
		}
		public SquareMeter AirDragArea
		{
			get { return AirdragEntry?.AirDragArea; }
		}

		public SquareMeter TransferredAirDragArea
		{
			get { return AirdragEntry?.TransferredAirDragArea; }
		}

		public SquareMeter AirDragArea_0
		{
			get { return AirdragEntry.AirDragArea_0; }
		}

		public DataSource DataSource
		{
			get { return AirdragEntry?.DataSource; }
		}
		public bool SavedInDeclarationMode { get; }

		private void SetLastValidAirdragEntry()
		{
			foreach (var manufacturingStage in _manufacturingStages) {
				var airdragData = manufacturingStage.Vehicle?.Components?.AirdragInputData;
				if (airdragData == null)
					continue;

				var value = airdragData.AirDragArea;
				if (value == null)
					continue;

				AirdragEntry = airdragData;
				return;
			}
		}

		public override bool IsInputDataComplete(VectoSimulationJobType jobType)
		{
			return InputComplete(AirdragEntry, nameof(AirdragEntry));
		}

		public override string GetInvalidEntry()
		{
			return InvalidEntry;
		}
	}

	// ---------------------------------------------------------------------------------------

	public class ConsolidatedBusAuxiliariesData : ConsolidatedDataBase, IBusAuxiliariesDeclarationData
	{
		private ConsolidateElectricConsumerData _consolidateElectricConsumerData;
		private ConsolidatedHVACBusAuxiliariesData _consolidatedHVACBusAuxiliariesData;
		private XmlNode _xmlNode;


		public ConsolidatedBusAuxiliariesData(IEnumerable<IManufacturingStageInputData> manufacturingStages)
			: base(manufacturingStages) { }

		public XmlNode XMLSource
		{
			get { return _xmlNode ?? (_xmlNode = GetBusAuxXMLSource()); }
		}
		public string FanTechnology
		{
			get { return null; }
		}
		public IList<string> SteeringPumpTechnology
		{
			get { return null; }
		}
		public IElectricSupplyDeclarationData ElectricSupply
		{
			get { return null; }
		}

		public IElectricConsumersDeclarationData ElectricConsumers
		{
			get { return GetElectricConsumers(); }
		}

		private IElectricConsumersDeclarationData GetElectricConsumers()
		{
			if (GetBusAuxPropertyValue<IElectricConsumersDeclarationData>(nameof(ElectricConsumers)) == null)
				return null;

			return _consolidateElectricConsumerData ??
					(_consolidateElectricConsumerData = new ConsolidateElectricConsumerData(_manufacturingStages));
		}

		public IPneumaticSupplyDeclarationData PneumaticSupply
		{
			get { return null; }
		}

		public IPneumaticConsumersDeclarationData PneumaticConsumers
		{
			get { return null; }
		}

		public IHVACBusAuxiliariesDeclarationData HVACAux
		{
			get { return GetHVACAux(); }
		}

		private IHVACBusAuxiliariesDeclarationData GetHVACAux()
		{
			if (GetBusAuxPropertyValue<IHVACBusAuxiliariesDeclarationData>(nameof(HVACAux)) == null)
				return null;

			return _consolidatedHVACBusAuxiliariesData ??
					(_consolidatedHVACBusAuxiliariesData = new ConsolidatedHVACBusAuxiliariesData(_manufacturingStages));
		}

		private T GetBusAuxPropertyValue<T>(string propertyName)
		{
			foreach (var manufacturingStage in _manufacturingStages) {
				var busAux = manufacturingStage.Vehicle?.Components?.BusAuxiliaries;
				if (busAux == null)
					continue;
				var value = GetPropertyValue<T>(busAux, propertyName);
				if (value != null)
					return value;
			}

			return default;
		}
		

		public override bool IsInputDataComplete(VectoSimulationJobType jobType)
		{
			GetElectricConsumers();
			GetHVACAux();

			return InputComplete(_consolidateElectricConsumerData, nameof(_consolidateElectricConsumerData)) 
					&& _consolidateElectricConsumerData.IsInputDataComplete(jobType)
					&& InputComplete(_consolidatedHVACBusAuxiliariesData, nameof(_consolidatedHVACBusAuxiliariesData))
					&& _consolidatedHVACBusAuxiliariesData.IsInputDataComplete(jobType);
		}

		public override string GetInvalidEntry()
		{
			if (InvalidEntry != null)
				return InvalidEntry;

			if (_consolidateElectricConsumerData?.GetInvalidEntry() != null)
				return _consolidateElectricConsumerData.GetInvalidEntry(); 
			if (_consolidatedHVACBusAuxiliariesData?.GetInvalidEntry() != null)
				return _consolidatedHVACBusAuxiliariesData.GetInvalidEntry(); 
			
			return null;
		}


		private XmlNode GetBusAuxXMLSource()
		{
			var multistageBusReport = new XMLMultistageBusReport();
			var auxElement = multistageBusReport.GetBusAuxiliaries(this);
			
			if (auxElement == null)
				return null;
			
			using (var xmlReader = auxElement.CreateReader())
			{
				var xmlDoc = new XmlDocument();
				xmlDoc.Load(xmlReader);
				return xmlDoc.FirstChild;
			}
		}

	}

	// ---------------------------------------------------------------------------------------

	public class ConsolidateElectricConsumerData : ConsolidatedDataBase, IElectricConsumersDeclarationData
	{
		public ConsolidateElectricConsumerData(IEnumerable<IManufacturingStageInputData> manufacturingStages)
			: base(manufacturingStages) { }

		public bool? InteriorLightsLED
		{
			get { return GetElectricConsumerPropertyValue<bool?>(nameof(InteriorLightsLED)); }
		}

		public bool? DayrunninglightsLED
		{
			get { return GetElectricConsumerPropertyValue<bool?>(nameof(DayrunninglightsLED)); }
		}

		public bool? PositionlightsLED
		{
			get { return GetElectricConsumerPropertyValue<bool?>(nameof(PositionlightsLED)); }
		}

		public bool? HeadlightsLED
		{
			get { return GetElectricConsumerPropertyValue<bool?>(nameof(HeadlightsLED)); }
		}

		public bool? BrakelightsLED
		{
			get { return GetElectricConsumerPropertyValue<bool?>(nameof(BrakelightsLED)); }
		}


		private T GetElectricConsumerPropertyValue<T>(string propertyName)
		{
			foreach (var manufacturingStage in _manufacturingStages) {
				var electricConsumer = manufacturingStage.Vehicle?.Components?.BusAuxiliaries?.ElectricConsumers;
				if (electricConsumer == null)
					continue;
				var value = GetPropertyValue<T>(electricConsumer, propertyName);
				if (value != null)
					return value;
			}
			return default;
		}

		public override bool IsInputDataComplete(VectoSimulationJobType jobType)
		{
			return InputComplete(InteriorLightsLED, nameof(InteriorLightsLED))
				&& InputComplete(DayrunninglightsLED, nameof(DayrunninglightsLED))
				&& InputComplete(PositionlightsLED, nameof(PositionlightsLED))
				&& InputComplete(HeadlightsLED, nameof(HeadlightsLED))
				&& InputComplete(BrakelightsLED, nameof(BrakelightsLED));
		}

		public override string GetInvalidEntry()
		{
			return InvalidEntry;
		}
	}

	// ---------------------------------------------------------------------------------------
	public class ConsolidatedHVACBusAuxiliariesData : ConsolidatedDataBase, IHVACBusAuxiliariesDeclarationData
	{
		public ConsolidatedHVACBusAuxiliariesData(IEnumerable<IManufacturingStageInputData> manufacturingStages)
			: base(manufacturingStages) { }

		public BusHVACSystemConfiguration? SystemConfiguration
		{
			get
			{
				return GetHVACBusAuxPropertyValue<BusHVACSystemConfiguration?>(nameof(SystemConfiguration));
			}
		}

		public HeatPumpType? HeatPumpTypeDriverCompartment
		{
			get
			{
				return GetHVACBusAuxPropertyValue<HeatPumpType?>(nameof(HeatPumpTypeDriverCompartment));
			}
		}

		public HeatPumpMode? HeatPumpModeDriverCompartment
		{
			get
			{
				return GetHVACBusAuxPropertyValue<HeatPumpMode?>(nameof(HeatPumpModeDriverCompartment));
			}
		}

		public IList<Tuple<HeatPumpType, HeatPumpMode>> HeatPumpPassengerCompartments
		{
			get
			{
				return GetHVACBusAuxPropertyValue<IList<Tuple<HeatPumpType, HeatPumpMode>>>(
					nameof(HeatPumpPassengerCompartments));
			}
		}

		public Watt AuxHeaterPower
		{
			get
			{
				return GetHVACBusAuxPropertyValue<Watt>(nameof(AuxHeaterPower));
			}
		}

		public bool? DoubleGlazing
		{
			get
			{
				return GetHVACBusAuxPropertyValue<bool?>(nameof(DoubleGlazing));
			}
		}

		public bool? AdjustableAuxiliaryHeater
		{
			get
			{
				return GetHVACBusAuxPropertyValue<bool?>(nameof(AdjustableAuxiliaryHeater));
			}
		}

		public bool? SeparateAirDistributionDucts
		{
			get
			{
				return GetHVACBusAuxPropertyValue<bool?>(nameof(SeparateAirDistributionDucts));
			}
		}

		public bool? WaterElectricHeater
		{
			get
			{
				return GetHVACBusAuxPropertyValue<bool?>(nameof(WaterElectricHeater));
			}
		}

		public bool? AirElectricHeater
		{
			get
			{
				return GetHVACBusAuxPropertyValue<bool?>(nameof(AirElectricHeater));
			}
		}

		public bool? OtherHeatingTechnology
		{
			get
			{
				return GetHVACBusAuxPropertyValue<bool?>(nameof(OtherHeatingTechnology));
			}
		}

		public bool? AdjustableCoolantThermostat { get; }

		public bool EngineWasteGasHeatExchanger { get; }


		private T GetHVACBusAuxPropertyValue<T>(string propertyName)
		{
			foreach (var manufacturingStage in _manufacturingStages) {
				var havacAux = manufacturingStage.Vehicle?.Components?.BusAuxiliaries?.HVACAux;
				if (havacAux == null)
					continue;
				var value = GetPropertyValue<T>(havacAux, propertyName);
				if (value != null)
					return value;
			}

			return default;
		}
		
		private bool IsCorrectSystemConfiguration()
		{
			return SystemConfiguration != null && SystemConfiguration != BusHVACSystemConfiguration.Unknown;
		}
		
		private bool RequiredParametersForJobType(VectoSimulationJobType jobType)
		{
			switch (jobType) {
				case VectoSimulationJobType.ConventionalVehicle:
					return true;
				case VectoSimulationJobType.ParallelHybridVehicle:
				case VectoSimulationJobType.SerialHybridVehicle:
				case VectoSimulationJobType.BatteryElectricVehicle:
					return WaterElectricHeater != null && AirElectricHeater != null && OtherHeatingTechnology != null;
				default:
					return false;
			}
		}
		
		public override bool IsInputDataComplete(VectoSimulationJobType jobType)
		{
			return InputComplete(IsCorrectSystemConfiguration(), nameof(IsCorrectSystemConfiguration))
					&& InputComplete(AuxHeaterPower, nameof(AuxHeaterPower))
					&& InputComplete(DoubleGlazing, nameof(DoubleGlazing))
					&& InputComplete(AdjustableAuxiliaryHeater, nameof(AdjustableAuxiliaryHeater))
					&& InputComplete(SeparateAirDistributionDucts, nameof(SeparateAirDistributionDucts))
					&& InputComplete(RequiredParametersForJobType(jobType), nameof(RequiredParametersForJobType));
		}

		public override string GetInvalidEntry()
		{
			return InvalidEntry;
		}
	}

	#endregion
}