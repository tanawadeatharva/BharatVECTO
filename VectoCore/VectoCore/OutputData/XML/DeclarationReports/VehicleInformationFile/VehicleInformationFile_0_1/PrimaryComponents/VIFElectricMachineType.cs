using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportXMLTypeWriter;


namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.VehicleInformationFile.VehicleInformationFile_0_1.Components
{
	public class VIFElectricMachineType : VIFElectricMachineGENType, IXmlAxlePowertrainTypeWriter
    {
		public VIFElectricMachineType(IVIFReportFactory vifFactory) : base(vifFactory) { }

        public XElement GetElement(IAxlePowertrainDeclarationInputData axlePt)
        {
            return GetElement(axlePt.ElectricMotor);
        }

        public override XElement GetElement(IDeclarationInputDataProvider inputData)
        {
            var electricMachine = GetElectricMachine(inputData);
            return GetElement(electricMachine);
        }

        protected virtual ElectricMachineEntry<IElectricMotorDeclarationInputData> GetElectricMachine(IDeclarationInputDataProvider inputData)
        {
            var electricMachines = inputData.JobInputData.Vehicle.Components.ElectricMachines.Entries;

            return electricMachines.FirstOrDefault(x => x.Position != PowertrainPosition.GEN);
        }

		protected virtual string ElementLocalName => XMLNames.Component_ElectricMachine;

        private XElement GetElement(ElectricMachineEntry<IElectricMotorDeclarationInputData> electricMachine)
		{
			return (electricMachine != null) ? GetElectricMachineType(electricMachine) : null;
		}
		
		private XElement GetElectricMachineType(ElectricMachineEntry<IElectricMotorDeclarationInputData> electricMachineData)
		{
			return new XElement(_vif + ElementLocalName,
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

    public class VIFGeneratorType : VIFElectricMachineType
    {
        public VIFGeneratorType(IVIFReportFactory vifFactory) : base(vifFactory) { }

        protected override ElectricMachineEntry<IElectricMotorDeclarationInputData> GetElectricMachine(IDeclarationInputDataProvider inputData)
        {
            return inputData.JobInputData.Vehicle.Components.Generator;
        }

        protected override string ElementLocalName => XMLNames.Component_ElectricMachineGEN;
    }
}
