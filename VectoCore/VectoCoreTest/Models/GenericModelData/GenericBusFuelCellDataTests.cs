using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
