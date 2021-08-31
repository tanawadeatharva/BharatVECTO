using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using Castle.Core.Internal;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider
{
    public class XMLElectricMotorDeclarationInputDataProvider : AbstractCommonComponentType, IXMLElectricMotorDeclarationInputData, IElectricMotorVoltageLevel
	{
		public static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V2101_JOBS;
		public const string XSD_TYPE = "ElectricMachineSystemMeasuredDataDeclarationType";
		public static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		private IList<IElectricMotorVoltageLevel> _voltageLevels;

		public XMLElectricMotorDeclarationInputDataProvider(
			XmlNode componentNode, string sourceFile) : base(componentNode, sourceFile)
		{
			SourceType = DataSourceType.XMLEmbedded;
		}

		#region Implementation of IElectricMotorDeclarationInputData

		public virtual ElectricMachineType ElectricMachineType => 
			ElectricMachineTypeHelper.Parse(GetString(XMLNames.ElectricMachine_ElectricMachineType));
		
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

		public virtual Volt TestVoltageOverload =>
			GetDouble(XMLNames.ElectricMachine_TestVoltageOverload).SI<Volt>();

		public virtual bool DcDcConverterIncluded => GetBool(XMLNames.ElectricMachine_DcDcConverterIncluded);

		public virtual string IHPCType => GetString(XMLNames.ElectricMachine_IHPCType);

		public virtual IList<IElectricMotorVoltageLevel> VoltageLevels =>
			_voltageLevels ?? (_voltageLevels = GetVoltageLevels());

		public TableData DragCurve =>
			ReadTableData(XMLNames.DragCurve, XMLNames.DragCurve_Entry, new Dictionary<string, string> 
			{
				{XMLNames.DragCurve_OutShaftSpeed, XMLNames.DragCurve_OutShaftSpeed},
				{XMLNames.DragCurve_DragTorque, XMLNames.DragCurve_DragTorque}
			});

		public virtual double OverloadRecoveryFactor { get; }

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
			
			foreach (XmlNode voltageLevelNode in voltageLevelNodes) {
				voltageLevels.Add(new XMLElectricMotorDeclarationInputDataProvider(voltageLevelNode, null));
			}

			return voltageLevels;
		}


		#region Overrides of AbstractXMLResource

		protected override XNamespace SchemaNamespace => NAMESPACE_URI;
		protected override DataSourceType SourceType { get; }

		#endregion
	}
}
