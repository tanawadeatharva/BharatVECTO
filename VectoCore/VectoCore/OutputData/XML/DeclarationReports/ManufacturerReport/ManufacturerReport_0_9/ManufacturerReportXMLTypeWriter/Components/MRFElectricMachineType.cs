using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportXMLTypeWriter.Components
{
    internal class ElectricMachineTypeWriter : AbstractMrfXmlType, IXmlTypeWriter, IXmlAxlePowertrainTypeWriter
    {
		public ElectricMachineTypeWriter(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		protected virtual ElectricMachineEntry<IElectricMotorDeclarationInputData> GetElectricMachine(IDeclarationInputDataProvider inputData)
		{
            var electricMachines = inputData.JobInputData.Vehicle.Components.ElectricMachines.Entries;

			return electricMachines.FirstOrDefault(x => x.Position != PowertrainPosition.GEN);
        }

        public XElement GetElement(IAxlePowertrainDeclarationInputData axlePt)
		{
            return GetElement(axlePt.ElectricMotor);
        }

        public XElement GetElement(IDeclarationInputDataProvider inputData)
        {
            var electricMachine = GetElectricMachine(inputData);
            return GetElement(electricMachine);
        }

        private XElement GetElement(ElectricMachineEntry<IElectricMotorDeclarationInputData> electricMachine)
		{
            var isGenerator = electricMachine.Position == PowertrainPosition.GEN;

            var electricMachineElement = new XElement(
                _mrf + (isGenerator ? "ElectricMachineGen" : "ElectricMachine"),
                new XElement(_mrf + XMLNames.ElectricMachine_Position, electricMachine.Position.ToXmlFormat()),
                new XElement(_mrf + "CountAtPosition", electricMachine.Count));

            var electricMachineSystem = new XElement(_mrf + XMLNames.ElectricMachineSystem);

            electricMachineSystem.Add(new XElement(_mrf + XMLNames.Component_Model, electricMachine.ElectricMachine.Model),
                new XElement(_mrf + XMLNames.Component_CertificationNumber, electricMachine.ElectricMachine.CertificationNumber),
                new XElement(_mrf + XMLNames.DI_Signature_Reference_DigestValue, electricMachine.ElectricMachine.DigestValue?.DigestValue ?? ""),
                new XElement(_mrf + XMLNames.ElectricMachine_ElectricMachineType, electricMachine.ElectricMachine.ElectricMachineType),
                new XElement(_mrf + XMLNames.Component_CertificationMethod, electricMachine.ElectricMachine.CertificationMethod),
                new XElement(_mrf + "RatedPower", electricMachine.ElectricMachine.R85RatedPower.ConvertToKiloWatt().ToXMLFormat(0)));

            var voltageLevels = new XElement(_mrf + "VoltageLevels");
            electricMachineSystem.Add(voltageLevels);

            foreach (var electricMotorVoltageLevel in electricMachine.ElectricMachine.VoltageLevels)
            {
                var voltageLevel = new XElement(_mrf + XMLNames.ElectricMachine_VoltageLevel,
                    electricMachine.ElectricMachine.VoltageLevels.Count > 1
                        ? new XAttribute("voltage", electricMotorVoltageLevel.VoltageLevel.ToXMLFormat(0))
                        : null,
                    new XElement(_mrf + "MaxContinuousPower",
                        (electricMotorVoltageLevel.ContinuousTorque *
                        electricMotorVoltageLevel.ContinuousTorqueSpeed).ConvertToKiloWatt().ToXMLFormat(0)));

                voltageLevels.Add(voltageLevel);
            }

            electricMachineElement.Add(electricMachineSystem);
            if (electricMachine.ADC != null)
            {
                var adc = electricMachine.ADC;
                electricMachineElement.Add(new XElement(_mrf + XMLNames.Component_ADC,
                    new XElement(_mrf + XMLNames.Component_Model, adc.Model),
                    new XElement(_mrf + XMLNames.Component_CertificationNumber, adc.CertificationNumber),
                    new XElement(_mrf + XMLNames.DI_Signature_Reference_DigestValue, adc.DigestValue?.DigestValue ?? ""),
                    new XElement(_mrf + XMLNames.Component_CertificationMethod, adc.CertificationMethod.ToXMLFormat()),
                    new XElement(_mrf + XMLNames.AngleDrive_Ratio, adc.Ratio.ToXMLFormat(3)))
                    );
            }

            return electricMachineElement;
        }
	}

	internal class MRFElectricMachineGenTypeWriter : ElectricMachineTypeWriter
	{
        public MRFElectricMachineGenTypeWriter(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

        protected override ElectricMachineEntry<IElectricMotorDeclarationInputData> GetElectricMachine(IDeclarationInputDataProvider inputData)
        {
            var electricMachines = inputData.JobInputData.Vehicle.Components.ElectricMachines.Entries;

            return electricMachines.FirstOrDefault(x => x.Position == PowertrainPosition.GEN);
        }
    }

    internal class MRFGeneratorTypeWriter : ElectricMachineTypeWriter
    {
        public MRFGeneratorTypeWriter(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

        protected override ElectricMachineEntry<IElectricMotorDeclarationInputData> GetElectricMachine(IDeclarationInputDataProvider inputData)
        {
            return inputData.JobInputData.Vehicle.Components.Generator;
        }
    }
}
