using System.Collections.Generic;
using System.Data;
using System.Xml;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;


namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.VehicleInformationFile.VehicleInformationFile_0_1.Components
{
	public interface IXmlElectricMachineSystemType
	{
		XElement GetElement(IElectricMotorDeclarationInputData em);
	}


	public class XmlElectricMachineSystemMeasuredType : AbstractVIFXmlType, IXmlElectricMachineSystemType
	{
		public string XmlTypeAttributeName { get; set; }

		public XmlElectricMachineSystemMeasuredType(IVIFReportFactory vifFactory) : base(vifFactory)
		{
			XmlTypeAttributeName = "ElectricMachineSystemMeasuredDataDeclarationType";
		}

		#region Implementation of IVIFElectricMachineSystemType

		public virtual XElement GetElement(IElectricMotorDeclarationInputData em)
		{
			return new XElement(_vif + XMLNames.ElectricMachineSystem,
				new XElement(_vif + XMLNames.ComponentDataWrapper,
					new XAttribute(_xsi + "type", $"vif:{XmlTypeAttributeName}"),
					new XAttribute("id", em.DigestValue.Reference),
					GetElectricMachineSystemCommon(em),
					new XElement(_vif + XMLNames.Component_CertificationMethod, em.CertificationMethod.ToXMLFormat()),
					GetElectricMachineSystemPowerRange(em),
					new XElement(_vif + XMLNames.ElectricMachine_DcDcConverterIncluded, em.DcDcConverterIncluded),
					new XElement(_vif + XMLNames.ElectricMachine_IHPCType, em.IHPCType),
					GetVoltageLevels(em.VoltageLevels),
					GetDragCurve(em.DragCurve),
					GetConditioning(em.Conditioning)
				),
				GetSignature(em.DigestValue)
				);
		}
		
		#endregion

		protected virtual List<XElement> GetElectricMachineSystemCommon(IElectricMotorDeclarationInputData em)
		{
			return new List<XElement> {
				new XElement(_vif + XMLNames.Component_Manufacturer, em.Manufacturer),
				new XElement(_vif + XMLNames.Component_Model, em.Model),
				new XElement(_vif + XMLNames.Component_CertificationNumber, em.CertificationNumber),
				new XElement(_vif + XMLNames.Component_Date,
					XmlConvert.ToString(em.Date, XmlDateTimeSerializationMode.Utc)),
				new XElement(_vif + XMLNames.Component_AppVersion, em.AppVersion),
				new XElement(_vif + XMLNames.ElectricMachine_ElectricMachineType, em.ElectricMachineType.ToString()),
			};
		}

		protected virtual List<XElement> GetElectricMachineSystemPowerRange(IElectricMotorDeclarationInputData em)
		{
			return new List<XElement> {
				new XElement(_vif + XMLNames.ElectricMachine_R85RatedPower, em.R85RatedPower.ToXMLFormat()),
				new XElement(_vif + XMLNames.ElectricMachine_RotationalInertia, em.Inertia.ToXMLFormat(2))
			};
		}



		protected virtual List<XElement> GetVoltageLevels(IList<IElectricMotorVoltageLevel> voltageLevels)
		{
			var result = new List<XElement>();

			foreach (var voltageLevel in voltageLevels) {

				var entry = new XElement(_vif + XMLNames.ElectricMachine_VoltageLevel,
					new XElement(_vif + XMLNames.VoltageLevel_Voltage, voltageLevel.VoltageLevel.ToXMLFormat()),
					new XElement(_vif + XMLNames.ElectricMachine_ContinuousTorque, voltageLevel.ContinuousTorque.ToXMLFormat(2)),
					new XElement(_vif + XMLNames.ElectricMachine_TestSpeedContinuousTorque, voltageLevel.ContinuousTorqueSpeed.ToXMLFormat(2)),
					new XElement(_vif + XMLNames.ElectricMachine_OverloadTorque, voltageLevel.OverloadTorque.ToXMLFormat(2)),
					new XElement(_vif + XMLNames.ElectricMachine_TestSpeedOverloadTorque, voltageLevel.OverloadTestSpeed.ToXMLFormat(2)),
					new XElement(_vif + XMLNames.ElectricMachine_OverloadDuration, voltageLevel.OverloadTime.ToXMLFormat(2)),
					GetMaxTorqueCurve(voltageLevel.FullLoadCurve),
					GetPowerMap(voltageLevel.PowerMap)

				);
				result.Add(entry);
			}
			
			return result;
		}


		protected virtual XElement GetMaxTorqueCurve(DataTable maxTorqueData)
		{
			var maxTorqueCurveEntries = new List<XElement>();

			for (int r = 0; r < maxTorqueData.Rows.Count; r++)
			{
				var row = maxTorqueData.Rows[r];
				var outShaftSpeed = row[XMLNames.MaxTorqueCurve_OutShaftSpeed];
				var maxTorque = row[XMLNames.MaxTorqueCurve_MaxTorque];
				var minTorque = row[XMLNames.MaxTorqueCurve_MinTorque];

				var element = new XElement(_vif + XMLNames.MaxTorqueCurve_Entry,
					new XAttribute(XMLNames.MaxTorqueCurve_OutShaftSpeed, outShaftSpeed),
					new XAttribute(XMLNames.MaxTorqueCurve_MaxTorque, maxTorque),
					new XAttribute(XMLNames.MaxTorqueCurve_MinTorque, minTorque)
				);

				maxTorqueCurveEntries.Add(element);
			}

			return new XElement(XMLNames.MaxTorqueCurve, maxTorqueCurveEntries);
		}

		protected virtual List<XElement> GetPowerMap(IList<IElectricMotorPowerMap> powerMapData)
		{
			var powerMaps = new List<XElement>();

			foreach (var powerMapEntry in powerMapData)
			{

				var entries = new List<XElement>();
				for (int r = 0; r < powerMapEntry.PowerMap.Rows.Count; r++)
				{
					var outShaftSpeed = powerMapEntry.PowerMap.Rows[r][XMLNames.PowerMap_OutShaftSpeed].ToString().ToDouble();
					var torque = powerMapEntry.PowerMap.Rows[r][XMLNames.PowerMap_Torque].ToString().ToDouble();
					var electricPower = powerMapEntry.PowerMap.Rows[r][XMLNames.PowerMap_ElectricPower].ToString().ToDouble();

					var entry = new XElement(_vif + XMLNames.PowerMap_Entry,
						new XAttribute(XMLNames.PowerMap_OutShaftSpeed, outShaftSpeed.ToXMLFormat(2)),
						new XAttribute(XMLNames.PowerMap_Torque, torque.ToXMLFormat(2)),
						new XAttribute(XMLNames.PowerMap_ElectricPower, electricPower.ToXMLFormat(2)));
					entries.Add(entry);
				}

				var powerEntry = new XElement(_vif + XMLNames.PowerMap,
					new XAttribute(XMLNames.Gear_GearNumber_Attr, powerMapEntry.Gear),
					entries);

				powerMaps.Add(powerEntry);
			}

			return powerMaps;
		}

		protected virtual XElement GetDragCurve(TableData dragCurve)
		{
			var entries = new List<XElement>();
			foreach (DataRow rowEntry in dragCurve.Rows) 
			{
				var outShaftSpeed = rowEntry[XMLNames.DragCurve_OutShaftSpeed].ToString().ToDouble();
				var dragTorque = rowEntry[XMLNames.DragCurve_DragTorque].ToString().ToDouble();

				var entry = new XElement(_vif + XMLNames.DragCurve_Entry,
					new XAttribute(XMLNames.DragCurve_OutShaftSpeed, outShaftSpeed.ToXMLFormat(2)),
					new XAttribute(XMLNames.DragCurve_DragTorque, dragTorque));

				entries.Add(entry);
			}

			return new XElement(_vif + XMLNames.DragCurve, entries);
		}


		protected virtual XElement GetConditioning(DataTable iepcData)
		{
			if (iepcData == null)
				return null;

			var entries = new List<XElement>();
			for (int r = 0; r < iepcData.Rows.Count; r++)
			{

				var coolantTempInLet = iepcData.Rows[r][XMLNames.Conditioning_CoolantTempInlet];
				var coolingPower = iepcData.Rows[r][XMLNames.Conditioning_CoolingPower];

				entries.Add(new XElement(_vif + XMLNames.Conditioning_Entry,
					new XAttribute(XMLNames.Conditioning_CoolantTempInlet, coolantTempInLet),
					new XAttribute(XMLNames.Conditioning_CoolingPower, coolingPower)));
			}

			return new XElement(_vif + XMLNames.Conditioning, entries);
		}
	}


	public class XmlElectricMachineSystemIHPCMeasuredType :  XmlElectricMachineSystemMeasuredType
	{
		public XmlElectricMachineSystemIHPCMeasuredType(IVIFReportFactory vifFactory) : base(vifFactory)
		{
			XmlTypeAttributeName = "ElectricMachineSystemIHPCMeasuredDataDeclarationType";
		}
	}


	public class XmlElectricMachineSystemStandardValueType : XmlElectricMachineSystemMeasuredType
	{
		public XmlElectricMachineSystemStandardValueType(IVIFReportFactory vifFactory) : base(vifFactory)
		{
			XmlTypeAttributeName = "ElectricMachineSystemStandardValuesDataDeclarationType";
		}
		
		#region Overrides of XmlElectricMachineSystemMeasuredType

		public override XElement GetElement(IElectricMotorDeclarationInputData em)
		{
			return new XElement(_vif + XMLNames.ElectricMachineSystem,
				new XElement(_vif + XMLNames.ComponentDataWrapper,
					new XAttribute(_xsi + "type", $"vif:{XmlTypeAttributeName}"),
					new XAttribute("id", em.DigestValue.Reference),
					GetElectricMachineSystemCommon(em),
					new XElement(_vif + XMLNames.Component_CertificationMethod, em.CertificationMethod.ToXMLFormat()),
					GetElectricMachineSystemPowerRange(em),
					new XElement(_vif + XMLNames.ElectricMachine_DcDcConverterIncluded, em.DcDcConverterIncluded),
					new XElement(_vif + XMLNames.ElectricMachine_IHPCType, em.IHPCType),
					GetVoltageLevels(em.VoltageLevels),
					GetDragCurve(em.DragCurve)
				),
				GetSignature(em.DigestValue)
			);
		}

		#endregion

		#region Overrides of XmlElectricMachineSystemMeasuredType

		protected override List<XElement> GetVoltageLevels(IList<IElectricMotorVoltageLevel> voltageLevels)
		{
			var result = new List<XElement>();

			foreach (var voltageLevel in voltageLevels)
			{

				var entry = new XElement(_vif + XMLNames.ElectricMachine_VoltageLevel,
					new XElement(_vif + XMLNames.ElectricMachine_ContinuousTorque, voltageLevel.ContinuousTorque.ToXMLFormat(2)),
					new XElement(_vif + XMLNames.ElectricMachine_TestSpeedContinuousTorque, voltageLevel.ContinuousTorqueSpeed.ToXMLFormat(2)),
					new XElement(_vif + XMLNames.ElectricMachine_OverloadTorque, voltageLevel.OverloadTorque.ToXMLFormat(2)),
					new XElement(_vif + XMLNames.ElectricMachine_TestSpeedOverloadTorque, voltageLevel.OverloadTestSpeed.ToXMLFormat(2)),
					new XElement(_vif + XMLNames.ElectricMachine_OverloadDuration, voltageLevel.OverloadTime.ToXMLFormat(2)),
					GetMaxTorqueCurve(voltageLevel.FullLoadCurve),
					GetPowerMap(voltageLevel.PowerMap)

				);
				result.Add(entry);
			}

			return result;
		}

		#endregion
	}
}
