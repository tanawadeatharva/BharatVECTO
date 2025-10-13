using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportXMLTypeWriter.Components
{
    internal class MRFFuelCellSystemType : AbstractMrfXmlType, IXmlTypeWriter
    {
        public MRFFuelCellSystemType(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

        public XElement GetElement(IDeclarationInputDataProvider inputData)
        {
            var result = new XElement(_mrf + "FuelCellSystem");

            foreach (var module in inputData.JobInputData.Vehicle.Components.FuelCellSystem.FuelCellModules)
            {
                var fc = new XElement(_mrf + "FuelCellModule",
                    new XElement(_mrf + XMLNames.Component_Model, module.FuelCell.Model),
                    new XElement(_mrf + XMLNames.Component_CertificationNumber, module.FuelCell.CertificationNumber),
                    new XElement(_mrf + XMLNames.DI_Signature_Reference_DigestValue, module.FuelCell.DigestValue?.DigestValue ?? ""),
                    new XElement(_mrf + XMLNames.Component_CertificationMethod, module.FuelCell.CertificationMethod.ToXMLFormat()),
                    new XElement(_mrf + "FCSRatedPower", (int)module.FuelCell.FCSRatedPower.Value()),
                    new XElement(_mrf + "Count", module.Count)
                );

                result.Add(fc);
            }

            return result;
        }
    }
}
