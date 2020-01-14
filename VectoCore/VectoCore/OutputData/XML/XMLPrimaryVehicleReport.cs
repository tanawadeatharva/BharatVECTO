using System;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.Models.Simulation.Data;

namespace TUGraz.VectoCore.OutputData.XML
{
	public class XMLPrimaryVehicleReport : XMLManufacturerReport
	{

		protected XNamespace RootNS = "urn:tugraz:ivt:VectoAPI:PrimaryVehicleInformation";
		protected XNamespace PifNS = "urn:tugraz:ivt:VectoAPI:DeclarationOutput:PrimaryBusInformation:HeavyBus:v0.1"; 
		protected XNamespace di = "http://www.w3.org/2000/09/xmldsig#";

		public XMLPrimaryVehicleReport()
		{
			
		}

		public override XDocument Report { get { return null; } }

		public override void GenerateReport()
		{
			throw new InvalidOperationException();
		}

		public void GenerateReport(XElement mrfHash)
		{
			
		}
	}
}
