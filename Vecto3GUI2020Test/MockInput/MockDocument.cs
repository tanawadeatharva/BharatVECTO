using Moq;
using System;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider.v24;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Reader.Impl;
using TUGraz.VectoCore.Utils;

namespace Vecto3GUI2020Test.MockInput;

public static class MockDocument
{
	public static IDeclarationJobInputData GetStepInput(string type, XNamespace version)
    {
        var mock = new Mock<IDeclarationJobInputData>();
        var mocked = mock.Object;
        mocked.AddVehicle(type, version);
        return mock.Object;
    }

    public static IDeclarationInputDataProvider GetDeclarationJob()
    {
        var mock = new Mock<IDeclarationInputDataProvider>(MockBehavior.Strict);
        mock.SetupGet(i => i.DataSource).Returns(MockInput.GetMockDataSource("VectoInputDeclaration",
            XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V20));
		mock.SetupGet(i => i.JobInputData).Returns(GetDeclarationJobInputData());
        return mock.Object;
    }



	public static IMultistepBusInputDataProvider GetMultistepInput(XNamespace stepInputVersion, string stepInputType, int nrOfStages)
	{
		
		var mock = new Mock<IMultistepBusInputDataProvider>(MockBehavior.Strict);
		mock.SetupGet(m => m.JobInputData).Returns(GetMultistageJobInputData(version:stepInputVersion, type:stepInputType, nrOfStages:nrOfStages));
		mock.SetupGet(m => m.DataSource).Returns(MockInput.GetMockDataSource(XMLDeclarationInputDataProviderMultistageV01.XSD_TYPE, XMLDefinitions.DECLARATION_MULTISTAGE_BUS_VEHICLE_NAMESPACE_VO1));
        


        return mock.Object;
	}

	private static IDeclarationMultistageJobInputData GetMultistageJobInputData(XNamespace version, string type,
		int nrOfStages = 3)
	{
		var mock = new Mock<IDeclarationMultistageJobInputData>();
		var mocked = mock.Object;
		mock.SetupGet(m => m.PrimaryVehicle).Returns(MockVehicle.GetPrimaryVehicleInformation(XMLDeclarationMultistage_Conventional_PrimaryVehicleBusJobInputDataProviderV01.XSD_TYPE));
		mock.SetupGet(m => m.ManufacturingStages).Returns(MockVehicle.GetManufacturingStages(nrOfStages:nrOfStages, type, version));


        //Evaluated later, in case the caller alters some values
		mock.SetupGet(m => m.ConsolidateManufacturingStage).Returns(() =>
			new ConsolidateManufacturingStages(mocked.PrimaryVehicle, mocked.ManufacturingStages));


		return mock.Object;
	}

	public static IDeclarationInputDataProvider AddStepInput(this IDeclarationInputDataProvider mocked, string type, XNamespace version)
    {
        var mock = Mock.Get(mocked);
        mock.SetupGet(i => i.JobInputData).Returns(GetStepInput(type, version));
        return mocked;
    }

	public static IDeclarationJobInputData AddPrimaryVehicle(this IDeclarationJobInputData mocked, string primaryBusType, XNamespace primaryBusVersion)
	{
		var mock = Mock.Get(mocked);

		mock.Setup(m => m.Vehicle).Returns(MockVehicle.GetPrimaryVehicle(primaryBusType));
		return mocked;
	}

    public static IDeclarationJobInputData AddVehicle(this IDeclarationJobInputData declJob, string type, XNamespace version)
    {
        var mock = Mock.Get(declJob);
        mock.SetupGet(j => j.Vehicle).Returns(MockVehicle.GetMockVehicle(type, version));
        return declJob;
    }

	public static IDeclarationJobInputData GetDeclarationJobInputData()
    {
        var mock = new Mock<IDeclarationJobInputData>();
		return mock.Object;
    }

    public static IDeclarationInputDataProvider GetDeclarationInputDataProvider(
        out Mock<IDeclarationInputDataProvider> mock, VectoSimulationJobType jobType, XNamespace nameSpace)
    {
        mock = new Mock<IDeclarationInputDataProvider>();
        mock.SetupGet(d => d.JobInputData)
			.Returns(GetDeclarationJobInputData().AddPrimaryVehicle(XMLDeclarationConventionalPrimaryBusVehicleDataProviderV24.XSD_TYPE, XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V24));
        mock.SetupGet(d => d.PrimaryVehicleData).Returns(MockVehicle.GetPrimaryVehicleInformation(XMLDeclarationMultistage_Conventional_PrimaryVehicleBusJobInputDataProviderV01.XSD_TYPE));
        mock.SetupGet(d => d.DataSource).Returns(new DataSource()
        {
            SourceFile = "Mock",
            SourceType = DataSourceType.XMLFile,
            SourceVersion = nameSpace.GetVersionFromNamespaceUri(),
        });






        return mock.Object;
    }

	public static IMultistageVIFInputData GetMultistepVIFInputData(XNamespace vifVersion, string vifType, int stages, string stepType, XNamespace stepVersion)
	{
		var mock = new Mock<IMultistageVIFInputData>();
		mock.SetupGet(m => m.MultistageJobInputData).Returns(GetMultistepInput(vifVersion, vifType, stages));
		mock.SetupGet(m => m.VehicleInputData).Returns(GetStepInput(stepType, stepVersion).Vehicle);
		mock.SetupGet(m => m.DataSource.SourceFile).Returns("mocked.vecto");


		return mock.Object;
	}

	public static IMultistagePrimaryAndStageInputDataProvider GetPrimaryAndStepInput(string primaryType,
		XNamespace stepVehicleVersion, string stepVehicleType, XNamespace primaryBusVersion)
	{
		var mock = new Mock<IMultistagePrimaryAndStageInputDataProvider>();
		var declJob = GetDeclarationJob();
		declJob.JobInputData.AddPrimaryVehicle(primaryType, primaryBusVersion: primaryBusVersion);
		mock.SetupGet(m => m.PrimaryVehicle).Returns(declJob);
		mock.SetupGet(m => m.StageInputData).Returns(MockVehicle.GetMockVehicle(stepVehicleType, stepVehicleVersion));
		return mock.Object;
	}
}