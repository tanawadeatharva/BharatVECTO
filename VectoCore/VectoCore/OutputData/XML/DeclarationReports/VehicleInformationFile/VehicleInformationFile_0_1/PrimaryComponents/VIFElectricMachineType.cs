using System.Collections.Generic;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;


namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.VehicleInformationFile.VehicleInformationFile_0_1.Components
{
	public class VIFElectricMachineType : VIFElectricMachineGENType
	{
		public VIFElectricMachineType(IVIFReportFactory vifFactory) : base(vifFactory) { }

		#region Implementation of IXmlTypeWriter

		public override XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			var electricMachineEntries = inputData.JobInputData.Vehicle.Components.ElectricMachines.Entries;
			if (electricMachineEntries == null || electricMachineEntries.Count == 0)
				return null;

			foreach (var electricMachine in electricMachineEntries) {
				if (electricMachine.Position != PowertrainPosition.GEN)
					return GetElectricMachineType(electricMachine);
			}

			return null;
		}

		#endregion
		
		private XElement GetElectricMachineType(ElectricMachineEntry<IElectricMotorDeclarationInputData> electricMachineData)
		{
			return new XElement(_vif + XMLNames.Component_ElectricMachine,
				new XElement(_vif + XMLNames.ElectricMachine_PowertrainPosition,
					electricMachineData.Position.ToXmlFormat()),
				new XElement(_vif + XMLNames.ElectricMachine_Count, electricMachineData.Count),
				_vifFactory.GetElectricMachineSystemType().GetElement(electricMachineData.ElectricMachine),
				GetADC(electricMachineData.ADC),
				Get25GearRatio(electricMachineData.RatioPerGear)
			);
		}

		private XElement Get25GearRatio(double[] gearRatios)
		{
			if (gearRatios == null)
				return null;

			var results = new List<XElement>();
			for (int i = 0; i < gearRatios.Length; i++) {
				results.Add(new XElement(_vif + XMLNames.Gear_Ratio, new XAttribute(XMLNames.ElectricMachine_P2_5GearRatios_Gear_Attr, i + 1),
					gearRatios[i].ToXMLFormat(3)));
			}

			return new XElement(_vif + XMLNames.ElectricMachine_P2_5GearRatios, results);
		}
	}
}
