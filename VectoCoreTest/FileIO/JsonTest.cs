using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using TUGraz.VectoCore.FileIO;

namespace TUGraz.VectoCore.Tests.FileIO
{
	[TestClass]
	public class JsonTest
	{
		private const string jsonExpected = @"{
  ""CreatedBy"": ""Michael Krisper"",
  ""Date"": ""2015-11-17T11:49:03Z"",
  ""AppVersion"": ""3.0.1.320"",
  ""FileVersion"": 7
}";

		private const string jsonExpected2 = @"{
  ""CreatedBy"": ""Michael Krisper"",
  ""Date"": ""2015-01-07T11:49:03Z"",
  ""AppVersion"": ""3.0.1.320"",
  ""FileVersion"": 7
}";


		[TestMethod]
		public void TestJsonHeaderEquality()
		{
			var h1 = new JsonDataHeader {
				AppVersion = "MyVecto3",
				CreatedBy = "UnitTest",
				Date = new DateTime(1970, 1, 1),
				FileVersion = 3
			};
			var h2 = new JsonDataHeader {
				AppVersion = "MyVecto3",
				CreatedBy = "UnitTest",
				Date = new DateTime(1970, 1, 1),
				FileVersion = 3
			};
			Assert.AreEqual(h1, h1);
			Assert.AreEqual(h1, h2);
			Assert.AreNotEqual(h1, null);
			Assert.AreNotEqual(h1, "hello world");
		}

		[TestMethod]
		public void Test_Json_DateFormat_German()
		{
			var json = @"{
  ""CreatedBy"": ""Michael Krisper"",
  ""Date"": ""17.11.2015 11:49:03"",
  ""AppVersion"": ""3.0.1.320"",
  ""FileVersion"": 7
}";
			var header = JsonConvert.DeserializeObject<JsonDataHeader>(json);

			Assert.AreEqual("3.0.1.320", header.AppVersion);
			Assert.AreEqual(7u, header.FileVersion);
			Assert.AreEqual("Michael Krisper", header.CreatedBy);
			Assert.AreEqual(new DateTime(2015, 11, 17, 11, 49, 3, DateTimeKind.Utc), header.Date);

			var jsonCompare = JsonConvert.SerializeObject(header, Formatting.Indented);
			Assert.AreEqual(jsonExpected, jsonCompare);
		}

		[TestMethod]
		public void Test_Json_DateFormat_German2()
		{
			var json = @"{
  ""CreatedBy"": ""Michael Krisper"",
  ""Date"": ""7.1.2015 11:49:03"",
  ""AppVersion"": ""3.0.1.320"",
  ""FileVersion"": 7
}";
			var header = JsonConvert.DeserializeObject<JsonDataHeader>(json);

			Assert.AreEqual("3.0.1.320", header.AppVersion);
			Assert.AreEqual(7u, header.FileVersion);
			Assert.AreEqual("Michael Krisper", header.CreatedBy);
			Assert.AreEqual(new DateTime(2015, 1, 7, 11, 49, 3, DateTimeKind.Utc), header.Date);

			var jsonCompare = JsonConvert.SerializeObject(header, Formatting.Indented);
			Assert.AreEqual(jsonExpected2, jsonCompare);
		}

		[TestMethod]
		public void Test_Json_DateFormat_English()
		{
			var json = @"{
  ""CreatedBy"": ""Michael Krisper"",
  ""Date"": ""11/17/2015 11:49:03 AM"",
  ""AppVersion"": ""3.0.1.320"",
  ""FileVersion"": 7
}";
			var header = JsonConvert.DeserializeObject<JsonDataHeader>(json);

			Assert.AreEqual("3.0.1.320", header.AppVersion);
			Assert.AreEqual(7u, header.FileVersion);
			Assert.AreEqual("Michael Krisper", header.CreatedBy);
			Assert.AreEqual(new DateTime(2015, 11, 17, 11, 49, 3, DateTimeKind.Utc), header.Date);

			var jsonCompare = JsonConvert.SerializeObject(header, Formatting.Indented);
			Assert.AreEqual(jsonExpected, jsonCompare);
		}

		[TestMethod]
		public void Test_Json_DateFormat_English2()
		{
			var json = @"{
  ""CreatedBy"": ""Michael Krisper"",
  ""Date"": ""1/7/2015 11:49:03 AM"",
  ""AppVersion"": ""3.0.1.320"",
  ""FileVersion"": 7
}";
			var header = JsonConvert.DeserializeObject<JsonDataHeader>(json);

			Assert.AreEqual("3.0.1.320", header.AppVersion);
			Assert.AreEqual(7u, header.FileVersion);
			Assert.AreEqual("Michael Krisper", header.CreatedBy);
			Assert.AreEqual(new DateTime(2015, 1, 7, 11, 49, 3, DateTimeKind.Utc), header.Date);

			var jsonCompare = JsonConvert.SerializeObject(header, Formatting.Indented);
			Assert.AreEqual(jsonExpected2, jsonCompare);
		}


		[TestMethod]
		public void Test_Json_DateFormat_ISO8601()
		{
			var json = @"{
  ""CreatedBy"": ""Michael Krisper"",
  ""Date"": ""2015-11-17T11:49:03Z"",
  ""AppVersion"": ""3.0.1.320"",
  ""FileVersion"": 7
}";
			var header = JsonConvert.DeserializeObject<JsonDataHeader>(json);

			Assert.AreEqual("3.0.1.320", header.AppVersion);
			Assert.AreEqual(7u, header.FileVersion);
			Assert.AreEqual("Michael Krisper", header.CreatedBy);
			Assert.AreEqual(new DateTime(2015, 11, 17, 11, 49, 3, DateTimeKind.Utc), header.Date);

			var jsonCompare = JsonConvert.SerializeObject(header, Formatting.Indented);
			Assert.AreEqual(json, jsonCompare);
		}

		[TestMethod]
		public void Test_Json_DateFormat_ISO8601_CET()
		{
			var json = @"{
  ""CreatedBy"": ""Michael Krisper"",
  ""Date"": ""2015-11-17T11:49:03+01:00"",
  ""AppVersion"": ""3.0.1.320"",
  ""FileVersion"": 7
}";
			var header = JsonConvert.DeserializeObject<JsonDataHeader>(json);

			Assert.AreEqual("3.0.1.320", header.AppVersion);
			Assert.AreEqual(7u, header.FileVersion);
			Assert.AreEqual("Michael Krisper", header.CreatedBy);
			Assert.AreEqual(new DateTime(2015, 11, 17, 11, 49, 3, DateTimeKind.Utc), header.Date);

			var jsonCompare = JsonConvert.SerializeObject(header, Formatting.Indented);
			Assert.AreEqual(json, jsonCompare);
		}
	}
}