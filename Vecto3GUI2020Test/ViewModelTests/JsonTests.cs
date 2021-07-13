using NUnit.Framework;
using VECTO3GUI2020.Model.Multistage;
using TUGraz.VectoCore;

namespace Vecto3GUI2020Test.ViewModelTests
{
	[TestFixture]
	public class JsonTests
	{
		[Test]
		public void NamingTest()
		{
            //Membernames have to match JsonKeys for correct serialization/desirialization
			Assert.AreEqual(JsonKeys.JsonHeader, nameof(JSONJob.Header));
			Assert.AreEqual(JsonKeys.JsonHeader_FileVersion, nameof(JSONJob.Header.FileVersion));

			Assert.AreEqual(JsonKeys.JsonBody, nameof(JSONJob.Body));
            Assert.AreEqual(JsonKeys.PrimaryVehicle, nameof(JSONJob.Body.PrimaryVehicle));
            Assert.AreEqual(JsonKeys.InterimStage, nameof(JSONJob.Body.InterimStage));
		}
	}
}