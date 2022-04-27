using System;
using System.Collections.Generic;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.CustomerInformationFile.CustomerInformationFile_0_9
{
	public abstract class AbstractCustomerReport : IXMLCustomerReport
    {
		protected readonly ICustomerInformationFileFactory _cifFactory;
		protected XNamespace xsi = XNamespace.Get("http://www.w3.org/2001/XMLSchema-instance");
		protected XElement Vehicle { get; set; }

		protected AbstractCustomerReport(ICustomerInformationFileFactory cifFactory)
		{
			_cifFactory = cifFactory;
		}


		public abstract void InitializeVehicleData(IDeclarationInputDataProvider inputData);

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
