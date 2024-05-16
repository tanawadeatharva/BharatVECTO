using Moq;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Models.Connector.Ports;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.Tests.Utils;
using TUGraz.VectoCore.Utils;
using Assert = NUnit.Framework.Assert;

namespace TUGraz.Vecto.UnitTests.TestCases.ComponentReader;

public class RetarderDataReaderTests
{
	private const double Tolerance = 0.0001;

    [TestCase]
    public void RetarderDataSorting()
    {
        var retarderEntries = new[] {
            "100,10.02",
            "0,10",
            "200,10.08",
            "500,10.5",
            "300,10.18",
            "400,10.32",
        };
        var retarderTbl =
            VectoCSVFile.ReadStream(InputDataHelper.InputDataAsStream("Retarder Speed [rpm],Loss Torque [Nm]",
                retarderEntries));
        var vehicle = new VehicleContainer(ExecutionMode.Engineering);
        var retarderData = RetarderLossMapReader.Create(retarderTbl);
        var retarder = new Retarder(vehicle, retarderData, 2.0);

		var mockPort = new Mock<ITnOutPort>();
		NewtonMeter tqRequest = null;
		PerSecond rpmRequest = null;
		mockPort.Setup(p => p.Request(It.IsAny<Second>(), It.IsAny<Second>(), It.IsAny<NewtonMeter>(),
			It.IsAny<PerSecond>(), It.Is<bool>(b => !b))).Returns((Second _, Second _, NewtonMeter tq, PerSecond rpm, bool _) => {
			tqRequest = tq;
			rpmRequest = rpm;
			return new ResponseSuccess(this);
		});

        retarder.InPort().Connect(mockPort.Object);
        var outPort = retarder.OutPort();

        var absTime = 0.SI<Second>();
        var dt = 0.SI<Second>();

		var cardanSpeed = 125.RPMtoRad();
		var cardanTorque = 100.SI<NewtonMeter>();
		var expectedRetarderLoss = 20.26.SI<NewtonMeter>();
        // --------
        outPort.Initialize(cardanTorque, cardanSpeed);
        outPort.Request(absTime, dt, cardanTorque, cardanSpeed, false);

		mockPort.Verify(p => p.Request(It.IsAny<Second>(), It.IsAny<Second>(), It.IsAny<NewtonMeter>(), It.IsAny<PerSecond>(), false), Times.Once);
		Assert.IsNotNull(rpmRequest);
		Assert.IsNotNull(tqRequest);
		Assert.AreEqual(cardanSpeed.AsRPM, rpmRequest.AsRPM, Tolerance);
		Assert.AreEqual((cardanTorque + expectedRetarderLoss).Value(), tqRequest.Value(), Tolerance);

        //Assert.AreEqual(125.RPMtoRad().Value(), nextRequest.AngularVelocity.Value(), Delta);
        //Assert.AreEqual(100 + 20.26, nextRequest.Torque.Value(), Delta);
    }

}