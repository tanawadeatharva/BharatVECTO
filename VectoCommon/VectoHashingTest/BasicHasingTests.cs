/*
* This file is part of VECTO.
*
* Copyright © 2012-2019 European Union
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

using System.IO;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using TUGraz.VectoHashing.Impl;
using TUGraz.VectoHashing.Util;
using NUnit.Framework;

namespace VectoHashingTest
{
	[TestFixture]
	//[Parallelizable(ParallelScope.All)]
	public class BasicHasingTests
	{
		public const string SimpleXML = @"Testdata/XML/simple_document.xml";
		public const string ReferenceXMLEngine = @"Testdata/XML/Reference/vecto_engine-sample.xml";
		public const string ReferenceXMLVehicle = @"Testdata/XML/Reference/vecto_vehicle-sample_FULL.xml";

		public const string UnorderedXMLVehicle = @"Testdata/XML/Variations/vecto_vehicle-sample_FULL_Entry_Order.xml";

		public const string HashSimpleXML = "U2zic7KOnKw60rzh+KKQ1lwZL6NmXju+DXG7cYYmlxo=";

		public const string HashEngineXML = "cfPKB2LkHIbznFA9aQwCNfNLSj9V7qNnSskyOxaXB+o=";
		public const string HashVehicleXML = "k029AO90zxKbTybDrvUlCFszdynJot8S1Y+U5lVUG18=";

		public const string v3_MRF_Truck = @"TestData/XML/v3-validation/Class2_RigidTruck_DECL_SS.RSLT_MANUFACTURER.xml";
		public const string v3_MRF_Tractor = @"TestData/XML/v3-validation/Tractor_4x2_vehicle-class-5_Generic vehicle.RSLT_MANUFACTURER.xml";
		public const string v3_MRF_Dual = @"TestData/XML/v3-validation/vehicle_sampleSingleModeDualFuel.RSLT_MANUFACTURER.xml";
		
		public const string v3_CIF_Class9_AT = @"TestData/XML/v3-validation/Class_9_RigidTruck_AT_Decl.RSLT_CUSTOMER.xml";
		public const string v3_CIF_Class9 = @"TestData/XML/v3-validation/Class9_RigidTruck_DECL.RSLT_CUSTOMER.xml";
		public const string v3_CIF_Class2 = @"TestData/XML/v3-validation/Class2_RigidTruck_DECL.RSLT_CUSTOMER.xml";
		public const string v3_CIF_Class5 = @"TestData/XML/v3-validation/Class5_Tractor_DECL.RSLT_CUSTOMER.xml";

		public const string v3_Engine = @"TestData/XML/v3-validation/vecto_engine-input_SIGNED.xml";
		public const string v3_Gearbox = @"TestData/XML/v3-validation/vecto_gearbox-input_SIGNED.xml";
		public const string v3_Tyre = @"TestData/XML/v3-validation/Tyre_v25_SIGNED.xml";
		public const string v3_TorqueConverter = @"TestData/XML/v3-validation/TorqueConverter_SIGNED.xml";
		public const string v3_Retarder = @"TestData/XML/v3-validation/Retarder_SIGNED.xml";
		public const string v3_IEPC = @"TestData/XML/v3-validation/IEPC_1_SIGNED.xml";
		public const string v3_ElectricMachine = @"TestData/XML/v3-validation/ElectricMachineSystem_1_SIGNED.xml";
		public const string v3_Capacitor = @"TestData/XML/v3-validation/CapacitorSystem_1_SIGNED.xml";
		public const string v3_Battery = @"TestData/XML/v3-validation/BatterySystem_1_SIGNED.xml";
		public const string v3_Axlegear = @"TestData/XML/v3-validation/Axlegear_SIGNED.xml";
		public const string v3_Angledrive = @"TestData/XML/v3-validation/Angledrive_SIGNED.xml";
		public const string v3_ADC = @"TestData/XML/v3-validation/ADC_SIGNED.xml";
		public const string v3_FuelCell = @"TestData/XML/v3-validation/FuelCell_Measurement_v26_SIGNED.xml";

		public string[] Canonicalization;
		public string DigestAlgorithm;


		[OneTimeSetUp]
		public void RunBeforeAnyTests()
		{
			Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);
			Canonicalization = new[] { XMLHashProvider.VectoDsigTransform, XMLHashProvider.DsigExcC14NTransform };
			DigestAlgorithm = XMLHashProvider.DigestMethodSha256;
		}

		[
		TestCase("RESULT-abc1a6f2b9a14fe8b33f", v3_MRF_Truck, "dJDrxmpwMWdtoxTif1dHqfDE/5aThxeSbryyUsbJbys=", TestName="Hash_v3_MRF_Truck"),
		TestCase("RESULT-db205f616a0b4ccc9fed", v3_MRF_Tractor, "bnUj8nS/svHW4dS+ptebx8JFjtMMmO3Bd6qDc4U0v8s=", TestName="Hash_v3_MRF_Tractor"),
		TestCase("RESULT-4dcbdfb65f0e470bb63f", v3_MRF_Dual, "ctg1lfrWsEd6fxI5BLZ4x7fRHIeO5zmb6h5DbYfb9ug=", TestName="Hash_v3_MRF_Dual"),
		
		TestCase("COC-fdc3a394aa0744d49f50", v3_CIF_Class2, "v2YPLchB3BhHi1Zw+01q0SQHUVUv6oYiWhfeSVoytGE=", TestName="Hash_v3_CIF_class2"),
		TestCase("COC-b675dd32173c4b498801", v3_CIF_Class5, "ZrvLJcYAgk4/dLeR5/wF1KbjGHxnzcT/V1Sf+XRAetM=", TestName="Hash_v3_CIF_class5"),
		TestCase("COC-5adbddbf928a406c80bc", v3_CIF_Class9, "0tnWYf4SInKw0lmHoluuBzPwk2C54kniXueQNVQMfvI=", TestName="Hash_v3_CIF_class9"),
		TestCase("COC-5d390ef5b4f54d2caef4", v3_CIF_Class9_AT, "cn2oFs0hv3cL+J9+r1EPcp5MTpslNRvQpIMZbhKP+Qw=", TestName="Hash_v3_CIF_class9_AT"),

		TestCase("ENG-9dc7c7407ce34a27a331", v3_Engine, "PK9c2A8Po5xiRyoswqKH8mbUsGkTiwFc3kWC2wLECjE=", TestName="Hash_v3_Engine"),
		TestCase("GBX-d38de739fe184004a992", v3_Gearbox, "OucQrHvdsN/Dv91JcrjS1rwshfY8Az1FN3VYTT61GoU=", TestName="Hash_v3_Gearbox"),
		TestCase("TYR-123", v3_Tyre, "/17fLSjI4tp91UNHO0HmPOI16US3deEzh7rBYta3ZT8=", TestName="Hash_v3_Tyre"),
		TestCase("TC-asdf", v3_TorqueConverter, "eV5T56YGduPqat2tHf5yCAlX0Z8T7HdILmONLHSvOhQ=", TestName="Hash_v3_TorqueConverter"),
		TestCase("RET-asdf", v3_Retarder, "a986zDN0CIDibm2BRdOYBx8iZI3oy55+RyuP2T0sduo=", TestName="Hash_v3_Retarder"),
		TestCase("IEPC-asdf", v3_IEPC, "JcJ9ccahxohSIgBX4GUOaxDbRAlx+9dhnHOE1xWbPwk=", TestName="Hash_v3_IEPC"),
		TestCase("EM-asdf", v3_ElectricMachine, "lFQ3jySddrEjcTJDmYhUB9OzGRXP1t4YMmiOTvS1+iM=", TestName="Hash_v3_EM"),
		TestCase("CAP-asdf", v3_Capacitor, "0or1d+SsICgO9O+Q1U4ap/nnpYjcDvQmKW9V439wISo=", TestName="Hash_v3_Capacitor"),
		TestCase("BAT-asdf", v3_Battery, "G+8QNt2XEa1WyOotLvAbAQFqqMJRQNdAp0ClG9dVvkw=", TestName="Hash_v3_Battery"),
		TestCase("AXL-asdf", v3_Axlegear, "YmFY7xgB9vMOailhGDLaDqdBUUr0+z1W639012penoc=", TestName="Hash_v3_Axlegear"),
		TestCase("ANGL-asdf", v3_Angledrive, "43MVTl2L+7ymEoGrOc4yosrDG789RR0LoyYUg6z2e5c=", TestName="Hash_v3_Angledrive"),
		TestCase("ADC-123", v3_ADC, "bW4LoHHne1A1L+y2bGbJBa/rKQ7WrZS3hfLPRNpHHKI=", TestName="Hash_v3_ADC"),
        TestCase("FuelCell", v3_FuelCell, "vb0raVf5I8hQzQS3MQtUsPgeBfNSk7V2G8BZnvSOeCc=", TestName = "Hash_v3_FuelCell"),
		]
		public void HashXMLElement(string elementToHash, string xmlFile, string hashValue)
		{ 
			var doc = new XmlDocument();
			doc.Load(xmlFile);

			var hashed = XMLHashProvider.ComputeHash(doc, elementToHash, Canonicalization, DigestAlgorithm);
			
			var hash = GetHashValue(hashed, elementToHash);

			Assert.AreEqual(hashValue, hash);
		}

		[TestCase]
		public void HashSimpleXml()
		{
			var elementToHash = "elemID";
			var doc = new XmlDocument();
			doc.Load(SimpleXML);
			var hasher = new XMLHashProvider();
			var hashed = XMLHashProvider.ComputeHash(doc, elementToHash, Canonicalization, DigestAlgorithm);

			var hash = GetHashValue(hashed, elementToHash);
			WriteSignedXML(doc, "simple_document_hashed.xml");


			Assert.AreEqual(HashSimpleXML, hash);
		}

		[TestCase]
		public void HashReferenceEngineXML()
		{
			var elementToHash = "ENG-gooZah3D";
			var doc = new XmlDocument();
			doc.Load(ReferenceXMLEngine);
			var hasher = new XMLHashProvider();
			var hashed = XMLHashProvider.ComputeHash(doc, elementToHash, Canonicalization, DigestAlgorithm);

			var hash = GetHashValue(hashed, elementToHash);
			WriteSignedXML(doc, "reference_engine_hashed.xml");


			Assert.AreEqual(HashEngineXML, hash);
		}

		[TestCase]
		public void HashReferenceVehicleXML()
		{
			var elementToHash = "VEH-1234567890";
			var doc = new XmlDocument();
			doc.Load(ReferenceXMLVehicle);
			var hasher = new XMLHashProvider();
			var hashed = XMLHashProvider.ComputeHash(doc, elementToHash, Canonicalization, DigestAlgorithm);

			var hash = GetHashValue(hashed, elementToHash);
			WriteSignedXML(doc, "reference_vehicle_hashed.xml");


			Assert.AreEqual(HashVehicleXML, hash);
		}

		[TestCase]
		public void HashUnorderedVehicleXML()
		{
			var elementToHash = "VEH-1234567890";
			var doc = new XmlDocument();
			doc.Load(UnorderedXMLVehicle);
			var hasher = new XMLHashProvider();
			var hashed = XMLHashProvider.ComputeHash(doc, elementToHash, Canonicalization, DigestAlgorithm);

			var hash = GetHashValue(hashed, elementToHash);
			WriteSignedXML(doc, "reference_vehicle_hashed.xml");


			Assert.AreEqual(HashVehicleXML, hash);
		}

		private static string GetHashValue(XmlDocument hashed, string elementToHash)
		{
			var xdoc = hashed.ToXDocument();
			var hash = xdoc.XPathSelectElement("//*[@URI='#" + elementToHash + "']/*[local-name() = 'DigestValue']").Value;
			return hash;
		}

		private static void WriteSignedXML(XmlDocument doc, string filename)
		{
			var xmltw = new XmlTextWriter(filename, new UTF8Encoding(false));
			doc.WriteTo(xmltw);
			xmltw.Close();
		}
	}
}
