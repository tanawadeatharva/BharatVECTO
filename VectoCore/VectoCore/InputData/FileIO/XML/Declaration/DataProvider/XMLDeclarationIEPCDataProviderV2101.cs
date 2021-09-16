using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using Castle.Core.Internal;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider
{
    public class XMLDeclarationIEPCDataProviderV2101 : AbstractCommonComponentType, IXMLIEPCInputData, IElectricMotorVoltageLevel, IGearEntry
    {
		public static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V2101_JOBS;
		public const string XSD_TYPE = "IEPCMeasuredDataDeclarationType";
		public static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		private IXMLDeclarationVehicleData _vehicle;
		private IList<IElectricMotorVoltageLevel> _voltageLevels;
		private IList<IGearEntry> _gears;

		public XMLDeclarationIEPCDataProviderV2101(IXMLDeclarationVehicleData vehicle, XmlNode componentNode, string sourceFile) 
			: base(componentNode, sourceFile)
		{
			_vehicle = vehicle;
			SourceType = DataSourceType.XMLEmbedded;
		}

		#region Overrides of AbstractCommonComponentType

		public override CertificationMethod CertificationMethod
		{
			get
			{
				var certMethod = GetString(XMLNames.Component_Gearbox_CertificationMethod, required: false) ??
								GetString(XMLNames.Component_CertificationMethod, required: false);
				
				if(certMethod != null && certMethod == "Measured for complete component")
					return CertificationMethod.Measured;

				return certMethod != null
					? EnumHelper.ParseEnum<CertificationMethod>(certMethod)
					: CertificationMethod.Measured;
			}
		} 

		#endregion


		#region Implementation of IPowerRatingInputData

		public virtual ElectricMachineType ElectricMachineType => 
			GetString(XMLNames.ElectricMachine_ElectricMachineType).ParseEnum<ElectricMachineType>();

		public virtual Watt R85RatedPower =>
			GetDouble(XMLNames.ElectricMachine_R85RatedPower).SI<Watt>();

		public virtual KilogramSquareMeter Inertia =>
			GetDouble(XMLNames.ElectricMachine_RotationalInertia).SI<KilogramSquareMeter>();

		public virtual NewtonMeter ContinuousTorque =>
			GetDouble(XMLNames.ElectricMachine_ContinuousTorque).SI<NewtonMeter>();

		public virtual PerSecond ContinuousTorqueSpeed =>
			GetDouble(XMLNames.ElectricMachine_TestSpeedContinuousTorque).SI<PerSecond>();

		public virtual NewtonMeter OverloadTorque =>
			GetDouble(XMLNames.ElectricMachine_OverloadTorque).SI<NewtonMeter>();

		public virtual PerSecond OverloadTestSpeed =>
			GetDouble(XMLNames.ElectricMachine_TestSpeedOverloadTorque).SI<PerSecond>();

		public virtual Second OverloadTime =>
			GetDouble(XMLNames.ElectricMachine_OverloadDuration).SI<Second>();

		#endregion

		#region Implementation of IIEPCDeclarationInputData

		public virtual Volt TestVoltageOverload =>
			GetDouble(XMLNames.ElectricMachine_TestVoltageOverload).SI<Volt>();

		public virtual bool DifferentialIncluded => GetBool(XMLNames.IEPC_DifferentialIncluded);

		public virtual bool DesignTypeWheelMotor => GetBool(XMLNames.IEPC_DesignTypeWheelMotor);

		public virtual int? NrOfDesignTypeWheelMotorMeasured => ElementExists(XMLNames.IEPC_NrOfDesignTypeWheelMotorMeasured) ? 
			(int?) Convert.ToInt32(GetString(XMLNames.IEPC_NrOfDesignTypeWheelMotorMeasured)) : null;

		public IList<IGearEntry> Gears =>
			_gears ?? (_gears = GetGearEntries());


		public virtual IList<IElectricMotorVoltageLevel> VoltageLevels =>
			_voltageLevels ?? (_voltageLevels = GetVoltageLevels());

		public TableData DragCurve =>
			ReadTableData(XMLNames.DragCurve, XMLNames.DragCurve_Entry, new Dictionary<string, string>
			{
				{XMLNames.DragCurve_OutShaftSpeed, XMLNames.DragCurve_OutShaftSpeed},
				{XMLNames.DragCurve_DragTorque, XMLNames.DragCurve_DragTorque}
			});

		#endregion

		#region Implementation of IElectricMotorVoltageLevel

		public Volt VoltageLevel => GetDouble(XMLNames.VoltageLevel_Voltage).SI<Volt>();

		public TableData FullLoadCurve => ReadTableData(XMLNames.MaxTorqueCurve, XMLNames.MaxTorqueCurve_Entry, new Dictionary<string, string> {
			{XMLNames.MaxTorqueCurve_OutShaftSpeed, XMLNames.MaxTorqueCurve_OutShaftSpeed},
			{XMLNames.MaxTorqueCurve_MaxTorque, XMLNames.MaxTorqueCurve_MaxTorque},
			{XMLNames.MaxTorqueCurve_MinTorque, XMLNames.MaxTorqueCurve_MinTorque}
		});

		public TableData EfficiencyMap => ReadTableData(XMLNames.PowerMap, XMLNames.PowerMap_Entry, new Dictionary<string, string> {
			{ XMLNames.PowerMap_OutShaftSpeed, XMLNames.PowerMap_OutShaftSpeed },
			{ XMLNames.PowerMap_Torque, XMLNames.PowerMap_Torque },
			{ XMLNames.PowerMap_ElectricPower, XMLNames.PowerMap_ElectricPower }
		});
		
		#endregion

		private IList<IElectricMotorVoltageLevel> GetVoltageLevels()
		{
			var voltageLevelNodes = GetNodes(XMLNames.ElectricMachine_VoltageLevel);
			if (voltageLevelNodes.IsNullOrEmpty())
				return null;

			var voltageLevels = new List<IElectricMotorVoltageLevel>();

			foreach (XmlNode voltageLevelNode in voltageLevelNodes)
			{
				voltageLevels.Add(new XMLDeclarationIEPCDataProviderV2101(null, voltageLevelNode, null));
			}

			return voltageLevels;
		}

		#region Implementation of IGearEntry

		public int GearNumber => Convert.ToInt32(GetAttribute(BaseNode, XMLNames.Gear_GearNumber_Attr));

		public double Ratio => GetDouble(XMLNames.Gear_Ratio);

		public NewtonMeter MaxOutputShaftTorque => ElementExists(XMLNames.Gear_MaxOutputShaftTorque)
			? GetDouble(XMLNames.Gear_MaxOutputShaftTorque).SI<NewtonMeter>()
			: null;

		public PerSecond MaxOutputShaftSpeed => ElementExists(XMLNames.Gear_MaxOutputShaftSpeed)
			? GetDouble(XMLNames.Gear_MaxOutputShaftSpeed).SI<PerSecond>()
			: null;

		#endregion
		
		private IList<IGearEntry> GetGearEntries()
		{
			var gearNodes = GetNodes(XMLNames.Gear_EntryName);
			if (gearNodes.IsNullOrEmpty())
				return null;
			
			var gears = new List<IGearEntry>();
			foreach (XmlNode gearNode in gearNodes) {
				gears.Add(new XMLDeclarationIEPCDataProviderV2101(null, gearNode, null));
			}

			return gears;
		}
		
		#region Overrides of AbstractXMLResource

		protected override XNamespace SchemaNamespace => NAMESPACE_URI;
		protected override DataSourceType SourceType { get; }

		#endregion



	}
}
