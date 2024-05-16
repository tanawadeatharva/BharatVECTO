using Moq;
using NUnit.Framework;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents;
using TUGraz.VectoCore.Tests.Utils;
using TUGraz.VectoCore.Utils;
using Assert = NUnit.Framework.Assert;

namespace TUGraz.Vecto.UnitTests.TestCases.DataAdapter.Declaration;

public class RetarderDataAdapterTests
{
	// TODO: MQ 20240510: mismatch of allowed retarder positions in declaration mode and engineering mode!
	[TestCase(RetarderType.AxlegearInputRetarder, ArchitectureID.E2, true),
	TestCase(RetarderType.AxlegearInputRetarder, ArchitectureID.E3, false),
	TestCase(RetarderType.AxlegearInputRetarder, ArchitectureID.E4, true),
	TestCase(RetarderType.AxlegearInputRetarder, ArchitectureID.P2, true),
	TestCase(RetarderType.AxlegearInputRetarder, ArchitectureID.P3, true),
	TestCase(RetarderType.AxlegearInputRetarder, ArchitectureID.UNKNOWN, false),

	TestCase(RetarderType.EngineRetarder, ArchitectureID.E2, true),
	TestCase(RetarderType.EngineRetarder, ArchitectureID.E3, true),
	TestCase(RetarderType.EngineRetarder, ArchitectureID.E4, true),
	TestCase(RetarderType.EngineRetarder, ArchitectureID.P2, false),
	TestCase(RetarderType.EngineRetarder, ArchitectureID.P3, false),
	TestCase(RetarderType.EngineRetarder, ArchitectureID.UNKNOWN, false),

	TestCase(RetarderType.LossesIncludedInTransmission, ArchitectureID.E2, false),
	TestCase(RetarderType.LossesIncludedInTransmission, ArchitectureID.E3, true),
	TestCase(RetarderType.LossesIncludedInTransmission, ArchitectureID.E4, true),
	TestCase(RetarderType.LossesIncludedInTransmission, ArchitectureID.P2, false),
	TestCase(RetarderType.LossesIncludedInTransmission, ArchitectureID.P3, false),
	TestCase(RetarderType.LossesIncludedInTransmission, ArchitectureID.UNKNOWN, false),

	TestCase(RetarderType.None, ArchitectureID.E2, false),
	TestCase(RetarderType.None, ArchitectureID.E3, false),
	TestCase(RetarderType.None, ArchitectureID.E4, false),
	TestCase(RetarderType.None, ArchitectureID.P2, false),
	TestCase(RetarderType.None, ArchitectureID.P3, false),
	TestCase(RetarderType.None, ArchitectureID.UNKNOWN, false),

	TestCase(RetarderType.TransmissionInputRetarder, ArchitectureID.E2, false),
	TestCase(RetarderType.TransmissionInputRetarder, ArchitectureID.E3, true),
	TestCase(RetarderType.TransmissionInputRetarder, ArchitectureID.E4, true),
	TestCase(RetarderType.TransmissionInputRetarder, ArchitectureID.P2, false),
	TestCase(RetarderType.TransmissionInputRetarder, ArchitectureID.P3, false),
	TestCase(RetarderType.TransmissionInputRetarder, ArchitectureID.UNKNOWN, false),

	TestCase(RetarderType.TransmissionOutputRetarder, ArchitectureID.E2, false),
	TestCase(RetarderType.TransmissionOutputRetarder, ArchitectureID.E3, true),
	TestCase(RetarderType.TransmissionOutputRetarder, ArchitectureID.E4, true),
	TestCase(RetarderType.TransmissionOutputRetarder, ArchitectureID.P2, false),
	TestCase(RetarderType.TransmissionOutputRetarder, ArchitectureID.P3, false),
	TestCase(RetarderType.TransmissionOutputRetarder, ArchitectureID.UNKNOWN, false),
    ]
    public void CreateRetarderDataTest(RetarderType type, ArchitectureID arch, bool throwsExeption)
    {
        var adapter = new RetarderDataAdapter();
        var lossMap = VectoCSVFile.ReadStream(InputDataHelper.InputDataAsStream("Retarder Speed [rpm],Loss Torque [Nm]",
            new[] { "0, 10", "1000, 12", "2000, 18", "2300, 20.58" }));

		var inputData = new Mock<IRetarderInputData>();
		inputData.Setup(r => r.Type).Returns(type);
		if (type.IsDedicatedComponent()) {
			inputData.Setup(r => r.LossMap).Returns(lossMap);
		}

		if (throwsExeption) {
			Assert.Throws<VectoException>(() => adapter.CreateRetarderData(inputData.Object, arch, null));
		} else {
			var retarderData = adapter.CreateRetarderData(inputData.Object, arch, null);
			Assert.NotNull(retarderData);
		}

	}
}