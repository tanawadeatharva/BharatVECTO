using NUnit.Framework;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.GenericModelData;

namespace TUGraz.VectoCore.Tests.Models.GenericModelData
{
    [TestFixture]
    public class GenericBusFuelCellDataTests
    {
        [Test]
        public void ParseFCTable()
        {
            var result = GenericBusFuelCellData.CreateFuelCellPowerOutputMap(100.SI<Watt>());

            Assert.IsNotNull(result);
        }
    }
}
