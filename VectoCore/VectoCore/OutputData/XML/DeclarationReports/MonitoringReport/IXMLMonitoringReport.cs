using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TUGraz.VectoCore.Models.Simulation.Data;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.MonitoringReport
{
    public interface IXMLMonitoringReport
    {
        void Initialize(VectoRunData modelData);

		XDocument Report { get; }

		void GenerateReport();
    }
}
