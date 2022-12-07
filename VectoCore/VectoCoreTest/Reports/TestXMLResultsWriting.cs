using System;
using Ninject;
using NUnit.Framework;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.CustomerInformationFile.CustomerInformationFile_0_9.ResultWriter;

namespace TUGraz.VectoCore.Tests.Reports;

[TestFixture]
public class TestXMLResultsWriting
{
	private StandardKernel _kernel;

	private IResultsWriterFactory _reportResultsFactory;
	//private IXMLInputDataReader _xmlReader;

	[OneTimeSetUp]
	public void OneTimeSetup()
	{
		_kernel = new StandardKernel(new VectoNinjectModule());
		//_xmlReader = _kernel.Get<IXMLInputDataReader>();
		_reportResultsFactory = _kernel.Get<IResultsWriterFactory>();
	}

	[TestCase(VehicleCategory.RigidTruck, VectoSimulationJobType.ConventionalVehicle, false, true, typeof(ExemptedResultsWriter))]
	public void TestReportResultInstance(VehicleCategory vehicleCategory, VectoSimulationJobType jobType, bool ovc,
		bool exempted, Type expectedResultWriterType)
	{
		var resultsWriter = _reportResultsFactory.GetCIFResultsWriter(vehicleCategory.GetVehicleType(), jobType, ovc, exempted);

		Assert.AreEqual(expectedResultWriterType, resultsWriter.GetType());
	}
}