using System.Text;
using NUnit.Framework;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.Tests.Utils;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace TUGraz.Vecto.UnitTests.TestCases.ComponentReader;

public class PWheelCycleReaderTests
{
    [TestCase]
    public void Pwheel_ReadCycle_Test()
    {

		var cycleFile = InputDataHelper.InputDataAsStream("<t>,<Pwheel>,<gear>,<n>,<Padd>", new[] {
			"1,89,2,1748,1.300",
			"2,120,2,1400,0.4"
		});
        var drivingCycle = DrivingCycleDataReader.ReadFromStream(cycleFile, CycleType.PWheel, "", false);

        Assert.AreEqual(CycleType.PWheel, drivingCycle.CycleType);
	}
}