using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.CustomerInformationFile.CustomerInformationFile_0_9
{
	public abstract class AbstractCustomerReport : IXMLCustomerReport
    {
		private readonly ICustomerInformationFileFactory _cifFactory;

		protected XElement Vehicle { get; private set; }

		protected AbstractCustomerReport(ICustomerInformationFileFactory cifFactory)
		{
			_cifFactory = cifFactory;
		}


		public abstract void InitializeVehicleData(IVehicleDeclarationInputData inputData);

		#region Implementation of IXMLCustomerReport

		public void Initialize(VectoRunData modelData, List<List<FuelData.Entry>> fuelModes)
		{
			throw new NotImplementedException();
		}

		public XDocument Report { get; protected set; }
		public void WriteResult(XMLDeclarationReport.ResultEntry resultValue)
		{
			throw new NotImplementedException();
		}

		public void GenerateReport(XElement resultSignature)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}
