/*
* This file is part of VECTO.
*
* Copyright © 2012-2016 European Union
*
* Developed by Graz University of Technology,
*              Institute of Internal Combustion Engines and Thermodynamics,
*              Institute of Technical Informatics
*
* VECTO is licensed under the EUPL, Version 1.1 or - as soon they will be approved
* by the European Commission - subsequent versions of the EUPL (the "Licence");
* You may not use VECTO except in compliance with the Licence.
* You may obtain a copy of the Licence at:
*
* https://joinup.ec.europa.eu/community/eupl/og_page/eupl
*
* Unless required by applicable law or agreed to in writing, VECTO
* distributed under the Licence is distributed on an "AS IS" basis,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the Licence for the specific language governing permissions and
* limitations under the Licence.
*
* Authors:
*   Stefan Hausberger, hausberger@ivt.tugraz.at, IVT, Graz University of Technology
*   Christian Kreiner, christian.kreiner@tugraz.at, ITI, Graz University of Technology
*   Michael Krisper, michael.krisper@tugraz.at, ITI, Graz University of Technology
*   Raphael Luz, luz@ivt.tugraz.at, IVT, Graz University of Technology
*   Markus Quaritsch, markus.quaritsch@tugraz.at, IVT, Graz University of Technology
*   Martin Rexeis, rexeis@ivt.tugraz.at, IVT, Graz University of Technology
*/

using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TUGraz.VectoCore.Exceptions;
using TUGraz.VectoCore.InputData;
using TUGraz.VectoCore.InputData.FileIO;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.Tests.Utils;

namespace TUGraz.VectoCore.Tests.FileIO
{
	[TestClass]
	public class JsonTest
	{
		private const string TestJobFile = @"Testdata\Jobs\40t_Long_Haul_Truck.vecto";

		[TestMethod]
		public void ReadJobTest()
		{
			var job = JSONInputDataFactory.ReadJsonJob(TestJobFile);

			Assert.IsNotNull(job);
//			AssertHelper.Exception<InvalidFileFormatException>(() => );
		}

		[TestMethod]
		public void NoEngineFileTest()
		{
			var json = (JObject)JToken.ReadFrom(new JsonTextReader(File.OpenText(TestJobFile)));
			((JObject)json["Body"]).Property("EngineFile").Remove();

			AssertHelper.Exception<VectoException>(() => new JSONInputDataV2(json, TestJobFile), "Failed to read Engine file.");
		}

		[TestMethod]
		public void NoGearboxFileTest()
		{
			var json = (JObject)JToken.ReadFrom(new JsonTextReader(File.OpenText(TestJobFile)));
			((JObject)json["Body"]).Property("GearboxFile").Remove();

			AssertHelper.Exception<VectoException>(() => new JSONInputDataV2(json, TestJobFile), "Failed to read Gearbox file.");
		}

		[TestMethod]
		public void NoVehicleFileTest()
		{
			var json = (JObject)JToken.ReadFrom(new JsonTextReader(File.OpenText(TestJobFile)));
			((JObject)json["Body"]).Property("VehicleFile").Remove();

			AssertHelper.Exception<VectoException>(() => new JSONInputDataV2(json, TestJobFile), "Failed to read Vehicle file.");
		}

		[TestMethod]
		public void NoCyclesTest()
		{
			var json = (JObject)JToken.ReadFrom(new JsonTextReader(File.OpenText(TestJobFile)));
			((JObject)json["Body"]).Property("Cycles").Remove();

			AssertHelper.Exception<InvalidFileFormatException>(() => {
				var tmp = new JSONInputDataV2(json, TestJobFile).Cycles;
			}, "Key Cycles not found");
		}

		[TestMethod]
		public void NoAuxTest()
		{
			var json = (JObject)JToken.ReadFrom(new JsonTextReader(File.OpenText(TestJobFile)));
			((JObject)json["Body"]).Property("Aux").Remove();

			// MK,2016-01-20: Changed for PWheel: aux entry may be missing, and that is ok.
			var tmp = new JSONInputDataV2(json, TestJobFile).Auxiliaries;
			Assert.IsTrue(tmp.Count == 0);
		}

		[TestMethod]
		public void NoDriverAccCurveTest()
		{
			var json = (JObject)JToken.ReadFrom(new JsonTextReader(File.OpenText(TestJobFile)));
			((JObject)json["Body"]).Property("VACC").Remove();

			AssertHelper.Exception<VectoException>(() => {
				var tmp = new JSONInputDataV2(json, TestJobFile).DriverInputData.AccelerationCurve;
			}, "AccelerationCurve (VACC) required");
		}


		[TestMethod]
		public void UseDeclarationDriverAccCurveTest()
		{
			var json = (JObject)JToken.ReadFrom(new JsonTextReader(File.OpenText(TestJobFile)));
			json["Body"]["VACC"] = "Truck";

			var tmp = new JSONInputDataV2(json, TestJobFile).DriverInputData.AccelerationCurve;
			Assert.IsNotNull(tmp);
		}

		[TestMethod]
		public void NoLookaheadCoastingTest()
		{
			var json = (JObject)JToken.ReadFrom(new JsonTextReader(File.OpenText(TestJobFile)));
			((JObject)json["Body"]).Property("LAC").Remove();

			AssertHelper.Exception<VectoException>(() => {
				var tmp = new JSONInputDataV2(json, TestJobFile).DriverInputData.Lookahead;
			}, "Key LAC not found");
		}

		[TestMethod]
		public void NoOverspeedEcoRollTest()
		{
			var json = (JObject)JToken.ReadFrom(new JsonTextReader(File.OpenText(TestJobFile)));
			((JObject)json["Body"]).Property("OverSpeedEcoRoll").Remove();

			AssertHelper.Exception<VectoException>(() => {
				var tmp = new JSONInputDataV2(json, TestJobFile).DriverInputData.OverSpeedEcoRoll;
			}, "Key OverSpeedEcoRoll not found");
		}
	}


//	[TestClass]
//	public class JsonTest
//	{
//		private const string jsonExpected = @"{
//  ""CreatedBy"": ""Michael Krisper"",
//  ""Date"": ""2015-11-17T11:49:03Z"",
//  ""AppVersion"": ""3.0.1.320"",
//  ""FileVersion"": 7
//}";

//		private const string jsonExpected2 = @"{
//  ""CreatedBy"": ""Michael Krisper"",
//  ""Date"": ""2015-01-07T11:49:03Z"",
//  ""AppVersion"": ""3.0.1.320"",
//  ""FileVersion"": 7
//}";


//		[TestMethod]
//		public void TestJsonHeaderEquality()
//		{
//			var h1 = new JsonDataHeader {
//				AppVersion = "MyVecto3",
//				CreatedBy = "UnitTest",
//				Date = new DateTime(1970, 1, 1),
//				FileVersion = 3
//			};
//			var h2 = new JsonDataHeader {
//				AppVersion = "MyVecto3",
//				CreatedBy = "UnitTest",
//				Date = new DateTime(1970, 1, 1),
//				FileVersion = 3
//			};
//			Assert.AreEqual(h1, h1);
//			Assert.AreEqual(h1, h2);
//			Assert.AreNotEqual(h1, null);
//			Assert.AreNotEqual(h1, "hello world");
//		}

//		[TestMethod]
//		public void Test_Json_DateFormat_German()
//		{
//			var json = @"{
//  ""CreatedBy"": ""Michael Krisper"",
//  ""Date"": ""17.11.2015 11:49:03"",
//  ""AppVersion"": ""3.0.1.320"",
//  ""FileVersion"": 7
//}";
//			var header = JsonConvert.DeserializeObject<JsonDataHeader>(json);

//			Assert.AreEqual("3.0.1.320", header.AppVersion);
//			Assert.AreEqual(7u, header.FileVersion);
//			Assert.AreEqual("Michael Krisper", header.CreatedBy);
//			Assert.AreEqual(new DateTime(2015, 11, 17, 11, 49, 3, DateTimeKind.Utc), header.Date);

//			var jsonCompare = JsonConvert.SerializeObject(header, Formatting.Indented);
//			Assert.AreEqual(jsonExpected, jsonCompare);
//		}

//		[TestMethod]
//		public void Test_Json_DateFormat_German2()
//		{
//			var json = @"{
//  ""CreatedBy"": ""Michael Krisper"",
//  ""Date"": ""7.1.2015 11:49:03"",
//  ""AppVersion"": ""3.0.1.320"",
//  ""FileVersion"": 7
//}";
//			var header = JsonConvert.DeserializeObject<JsonDataHeader>(json);

//			Assert.AreEqual("3.0.1.320", header.AppVersion);
//			Assert.AreEqual(7u, header.FileVersion);
//			Assert.AreEqual("Michael Krisper", header.CreatedBy);
//			Assert.AreEqual(new DateTime(2015, 1, 7, 11, 49, 3, DateTimeKind.Utc), header.Date);

//			var jsonCompare = JsonConvert.SerializeObject(header, Formatting.Indented);
//			Assert.AreEqual(jsonExpected2, jsonCompare);
//		}

//		[TestMethod]
//		public void Test_Json_DateFormat_English()
//		{
//			var json = @"{
//  ""CreatedBy"": ""Michael Krisper"",
//  ""Date"": ""11/17/2015 11:49:03 AM"",
//  ""AppVersion"": ""3.0.1.320"",
//  ""FileVersion"": 7
//}";
//			var header = JsonConvert.DeserializeObject<JsonDataHeader>(json);

//			Assert.AreEqual("3.0.1.320", header.AppVersion);
//			Assert.AreEqual(7u, header.FileVersion);
//			Assert.AreEqual("Michael Krisper", header.CreatedBy);
//			Assert.AreEqual(new DateTime(2015, 11, 17, 11, 49, 3, DateTimeKind.Utc), header.Date);

//			var jsonCompare = JsonConvert.SerializeObject(header, Formatting.Indented);
//			Assert.AreEqual(jsonExpected, jsonCompare);
//		}

//		[TestMethod]
//		public void Test_Json_DateFormat_English2()
//		{
//			var json = @"{
//  ""CreatedBy"": ""Michael Krisper"",
//  ""Date"": ""1/7/2015 11:49:03 AM"",
//  ""AppVersion"": ""3.0.1.320"",
//  ""FileVersion"": 7
//}";
//			var header = JsonConvert.DeserializeObject<JsonDataHeader>(json);

//			Assert.AreEqual("3.0.1.320", header.AppVersion);
//			Assert.AreEqual(7u, header.FileVersion);
//			Assert.AreEqual("Michael Krisper", header.CreatedBy);
//			Assert.AreEqual(new DateTime(2015, 1, 7, 11, 49, 3, DateTimeKind.Utc), header.Date);

//			var jsonCompare = JsonConvert.SerializeObject(header, Formatting.Indented);
//			Assert.AreEqual(jsonExpected2, jsonCompare);
//		}


//		[TestMethod]
//		public void Test_Json_DateFormat_ISO8601()
//		{
//			var json = @"{
//  ""CreatedBy"": ""Michael Krisper"",
//  ""Date"": ""2015-11-17T11:49:03Z"",
//  ""AppVersion"": ""3.0.1.320"",
//  ""FileVersion"": 7
//}";
//			var header = JsonConvert.DeserializeObject<JsonDataHeader>(json);

//			Assert.AreEqual("3.0.1.320", header.AppVersion);
//			Assert.AreEqual(7u, header.FileVersion);
//			Assert.AreEqual("Michael Krisper", header.CreatedBy);
//			Assert.AreEqual(new DateTime(2015, 11, 17, 11, 49, 3, DateTimeKind.Utc), header.Date);

//			var jsonCompare = JsonConvert.SerializeObject(header, Formatting.Indented);
//			Assert.AreEqual(json, jsonCompare);
//		}

//		[TestMethod]
//		public void Test_Json_DateFormat_ISO8601_CET()
//		{
//			var json = @"{
//  ""CreatedBy"": ""Michael Krisper"",
//  ""Date"": ""2015-11-17T11:49:03+01:00"",
//  ""AppVersion"": ""3.0.1.320"",
//  ""FileVersion"": 7
//}";
//			var header = JsonConvert.DeserializeObject<JsonDataHeader>(json);

//			Assert.AreEqual("3.0.1.320", header.AppVersion);
//			Assert.AreEqual(7u, header.FileVersion);
//			Assert.AreEqual("Michael Krisper", header.CreatedBy);
//			Assert.AreEqual(new DateTime(2015, 11, 17, 11, 49, 3, DateTimeKind.Utc), header.Date);

//			var jsonCompare = JsonConvert.SerializeObject(header, Formatting.Indented);
//			Assert.AreEqual(json, jsonCompare);
//		}
//	}
}