using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportXMLTypeWriter.Components
{
    internal class MRF_IEPCSpecificationsTypeWriter : AbstractMrfXmlType, IXmlTypeWriter, IXmlAxlePowertrainTypeWriter
    {
		public MRF_IEPCSpecificationsTypeWriter(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		public XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			return GetElement(inputData.JobInputData.Vehicle.Components.IEPC, inputData.JobInputData.Vehicle.Components.AxleGearInputData);
		}

        public XElement GetElement(IAxlePowertrainDeclarationInputData axlePt)
        {
            return GetElement(axlePt.IEPCInputData, axlePt.AxleGearInputData);
        }

        private XElement GetElement(IIEPCDeclarationInputData iepcData, IAxleGearInputData axleGearInputData)
		{
            var count = iepcData.DesignTypeWheelMotor && iepcData.NrOfDesignTypeWheelMotorMeasured == 1 ? 2 : 1;

            var iepcXElement = new XElement(_mrf + "IEPCSpecifications",
                new XElement(_mrf + XMLNames.Component_Model, iepcData.Model),
                new XElement(_mrf + XMLNames.Component_CertificationNumber, iepcData.CertificationNumber),
                new XElement(_mrf + XMLNames.DI_Signature_Reference_DigestValue, iepcData.DigestValue?.DigestValue ?? ""),
                new XElement(_mrf + XMLNames.Engine_RatedPower, iepcData.TotalRatedPowerCalculated.ConvertToKiloWatt().ToXMLFormat(0)));
            //new XElement(_mrf + "MaxContinuousPower",(iepcData.ContinuousTorque*iepcData.ContinuousTorqueSpeed).ToXMLFormat()),

            var voltageLevels = new XElement(_mrf + "VoltageLevels");
            iepcXElement.Add(voltageLevels);

            foreach (var electricMotorVoltageLevel in iepcData.VoltageLevels)
            {
                var voltageLevel = new XElement(_mrf + XMLNames.ElectricMachine_VoltageLevel,
                    electricMotorVoltageLevel.VoltageLevel == null
                        ? null
                        : new XAttribute("voltage", electricMotorVoltageLevel.VoltageLevel.ToXMLFormat(0)),
                    new XElement(_mrf + "MaxContinuousPower",
                        (electricMotorVoltageLevel.ContinuousTorque * electricMotorVoltageLevel.ContinuousTorqueSpeed * count)
                        .ConvertToKiloWatt().ToXMLFormat(0)));


                voltageLevels.Add(voltageLevel);
            }

            double gearBoxRatio = iepcData.Gears.OrderByDescending(g => g.GearNumber).First().Ratio;
            double axleGearRatio = axleGearInputData?.Ratio ?? 1;

            iepcXElement.Add(
                new XElement(_mrf + "NrOfGears", iepcData.Gears.Count),
                new XElement(_mrf + "LowestTotalTransmissionRatio", (gearBoxRatio * axleGearRatio).ToXMLFormat(3)),
                new XElement(_mrf + XMLNames.IEPC_DifferentialIncluded, iepcData.DifferentialIncluded),
                new XElement(_mrf + XMLNames.IEPC_DesignTypeWheelMotor, iepcData.DesignTypeWheelMotor),
                new XElement(_mrf + XMLNames.Component_CertificationMethod, iepcData.CertificationMethod)
            );

            return iepcXElement;
        }
	}
}
