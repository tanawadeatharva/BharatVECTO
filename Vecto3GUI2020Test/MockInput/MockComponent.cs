using System;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
using Moq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider;
using TUGraz.VectoCore.Utils;

namespace Vecto3GUI2020Test.MockInput;

internal static class MockComponent
{
	public static IVehicleDeclarationInputData AddAirdragComponent(this IVehicleDeclarationInputData mocked, XNamespace version)
	{
		Mock.Get(mocked.Components).SetupGet(c => c.AirdragInputData).Returns((IAirdragDeclarationInputData)GetAirdragComponentData(version));
		return mocked;
	}

	public static IAirdragDeclarationInputData GetAirdragComponentData(XNamespace version)
	{
		var mock = new Mock<IAirdragDeclarationInputData>();
		mock.SetupGet(a => a.Manufacturer).Returns("Manufacturer");
		mock.SetupGet(a => a.Model).Returns("Model");
		mock.SetupGet(a => a.Date).Returns(DateTime.Today);
		mock.SetupGet(a => a.AppVersion).Returns("APPVERSION");
		mock.SetupGet(a => a.SavedInDeclarationMode).Returns(true);
		mock.SetupGet(a => a.AirDragArea).Returns(6.66.SI<SquareMeter>());
		mock.SetupGet(a => a.AirDragArea_0).Returns(7.77.SI<SquareMeter>());
		mock.SetupGet(a => a.TransferredAirDragArea).Returns(8.88.SI<SquareMeter>());

		if (version == XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V24)
		{
			mock.SetupGet(a => a.DataSource).Returns(
				MockInput.GetMockDataSource(XMLDeclarationAirdragDataProviderV24.XSD_TYPE, version));
		}
		else
		{
			mock.SetupGet(a => a.DataSource).Returns(
				MockInput.GetMockDataSource(XMLDeclarationAirdragDataProviderV10.XSD_TYPE, version));
		}


		//mock.SetupGet(a => a.DataSource.SourceVersion).Returns(version.GetVersionFromNamespaceUri);
		//mock.SetupGet(a => a.DataSource.Type).Returns(XMLDeclarationAirdragDataProviderV20.XSD_TYPE);
		//mock.SetupGet(a => a.DataSource.TypeVersion).Returns(version.NamespaceName);
		return mock.Object;
	}

	public static IVehicleDeclarationInputData SetAirdragVersion(this IVehicleDeclarationInputData mocked, XNamespace version, [CallerMemberName] string creatingMethod = "mocked")
	{
		if (mocked.Components.AirdragInputData == null)
		{
			throw new VectoException("Airdrag not mocked");
		};

		var airdrag = Mock.Get(mocked.Components.AirdragInputData);
		airdrag.SetupGet((a) => a.DataSource).Returns(new DataSource()
		{
			SourceFile = creatingMethod,
			SourceType = DataSourceType.XMLFile,
			Type = XMLNames.AirDrag_Data_Type_Attr,
			SourceVersion = version.GetVersionFromNamespaceUri(),
		});


		return mocked;
	}
}