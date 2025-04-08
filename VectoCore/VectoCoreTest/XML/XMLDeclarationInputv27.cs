using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Ninject;
using NUnit.Framework;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Tests.XML
{
    [TestFixture]
    public class XMLDeclarationInputv27
    {
        protected IXMLInputDataReader xmlInputReader;
        private IKernel _kernel;

        private const string BASE_DIR = @"TestData/XML/XMLReaderDeclaration/SchemaVersion2.7";
        private const string LORRIES = $"{BASE_DIR}/Lorries/";
        private const string PRIMARYBUSES = $"{BASE_DIR}/PrimaryBuses/";
        private const string COMPLETEDBUSES = $"{BASE_DIR}/CompletedBuses/";

        [OneTimeSetUp]
        public void RunBeforeAnyTests()
        {
            Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);

            _kernel = new StandardKernel(new VectoNinjectModule());
            xmlInputReader = _kernel.Get<IXMLInputDataReader>();
        }

        [
            TestCase(@"Conventional_HeavyLorry.xml", LORRIES, TestName = "v27_Conventional_HeavyLorry"),
            TestCase(@"Exempted_HeavyLorry.xml", LORRIES, TestName = "v27_Exempted_HeavyLorry"),
            TestCase(@"SHEV_IEPC_HeavyLorry.xml", LORRIES, TestName = "v27_SHEV_IEPC_HeavyLorry"),
            TestCase(@"SHEV_S2_HeavyLorry.xml", LORRIES, TestName = "v27_SHEV_S2_HeavyLorry"),
            TestCase(@"SHEV_S3_HeavyLorry.xml", LORRIES, TestName = "v27_SHEV_S3_HeavyLorry"),
            TestCase(@"SHEV_S4_HeavyLorry.xml", LORRIES, TestName = "v27_SHEV_S4_HeavyLorry"),
            TestCase(@"PEV_E2_HeavyLorry.xml", LORRIES, TestName = "v27_PEV_E2_HeavyLorry"),
            TestCase(@"PEV_E3_HeavyLorry.xml", LORRIES, TestName = "v27_PEV_E3_HeavyLorry"),
            TestCase(@"PEV_E4_HeavyLorry.xml", LORRIES, TestName = "v27_PEV_E4_HeavyLorry"),
            TestCase(@"PEV_E4_stdEM_HeavyLorry.xml", LORRIES, TestName = "v27_PEV_E4_stdEM_HeavyLorry"),
            TestCase(@"PEV_IEPC_HeavyLorry.xml", LORRIES, TestName = "v27_PEV_IEPC_HeavyLorry"),
            TestCase(@"PEV_IEPC_multiCurve_HeavyLorry.xml", LORRIES, TestName = "v27_PEV_IEPC_multiCurve_HeavyLorry"),
            TestCase(@"PEV_IEPC_stdValues_HeavyLorry.xml", LORRIES, TestName = "v27_PEV_IEPC_stdValues_HeavyLorry"),
            TestCase(@"H2_ICE_HeavyLorry.xml", LORRIES, TestName = "v27_H2_ICE_HeavyLorry"),
            TestCase(@"HEV_P2_HeavyLorry.xml", LORRIES, TestName = "v27_HEV_P2_HeavyLorry"),
            TestCase(@"HEV_P2_supercap_HeavyLorry.xml", LORRIES, TestName = "v27_HEV_P2_supercap_HeavyLorry"),
            TestCase(@"HEV_IHPC_HeavyLorry.xml", LORRIES, TestName = "v27_HEV_IHPC_HeavyLorry"),
            TestCase(@"FCHV_IEPC_HeavyLorry.xml", LORRIES, TestName = "v27_FCHV_IEPC_HeavyLorry"),
            TestCase(@"FCHV_IEPC_2xFC_HeavyLorry.xml", LORRIES, TestName = "v27_FCHV_IEPC_2xFC_HeavyLorry"),
            TestCase(@"FCHV_F2_HeavyLorry.xml", LORRIES, TestName = "v27_FCHV_F2_HeavyLorry"),
            TestCase(@"FCHV_F3_HeavyLorry.xml", LORRIES, TestName = "v27_FCHV_F3_HeavyLorry"),
            TestCase(@"FCHV_F4_HeavyLorry.xml", LORRIES, TestName = "v27_FCHV_F4_HeavyLorry"),
            TestCase(@"Multiple_PEV_E2_IEPC_HeavyLorry.xml", LORRIES, TestName = "v27_Multiple_PEV_E2_IEPC_HeavyLorry"),
            TestCase(@"Multiple_PEV_E3_E4_HeavyLorry.xml", LORRIES, TestName = "v27_Multiple_PEV_E3_E4_HeavyLorry"),
            TestCase(@"Multiple_FCHV_F2_IEPC_HeavyLorry.xml", LORRIES, TestName = "v27_Multiple_FCHV_F2_IEPC_HeavyLorry"),
            TestCase(@"Multiple_FCHV_F3_F4_HeavyLorry.xml", LORRIES, TestName = "v27_Multiple_FCHV_F3_F4_HeavyLorry"),
            TestCase(@"Multiple_SHEV_S2_IEPC_HeavyLorry.xml", LORRIES, TestName = "v27_Multiple_SHEV_S2_IEPC_HeavyLorry"),
            TestCase(@"Multiple_SHEV_S3_S4_HeavyLorry.xml", LORRIES, TestName = "v27_Multiple_SHEV_S3_S4_HeavyLorry"),

            TestCase(@"Conventional_MediumLorry.xml", LORRIES, TestName = "v27_Conventional_MediumLorry"),
            TestCase(@"Exempted_MediumLorry.xml", LORRIES, TestName = "v27_Exempted_MediumLorry"),
            TestCase(@"HEV_P2_MediumLorry.xml", LORRIES, TestName = "v27_HEV_P2_MediumLorry"),
            TestCase(@"SHEV_S2_MediumLorry.xml", LORRIES, TestName = "v27_SHEV_S2_MediumLorry"),
            TestCase(@"SHEV_S3_MediumLorry.xml", LORRIES, TestName = "v27_SHEV_S3_MediumLorry"),
            TestCase(@"SHEV_S4_MediumLorry.xml", LORRIES, TestName = "v27_SHEV_S4_MediumLorry"),
            TestCase(@"SHEV_IEPC_MediumLorry.xml", LORRIES, TestName = "v27_SHEV_IEPC_MediumLorry"),
            TestCase(@"PEV_IEPC_MediumLorry.xml", LORRIES, TestName = "v27_PEV_IEPC_MediumLorry"),
            TestCase(@"PEV_E2_MediumLorry.xml", LORRIES, TestName = "v27_PEV_E2_MediumLorry"),
            TestCase(@"FCHV_IEPC_MediumLorry.xml", LORRIES, TestName = "v27_FCHV_IEPC_MediumLorry"),
            TestCase(@"FCHV_F2_MediumLorry.xml", LORRIES, TestName = "v27_FCHV_F2_MediumLorry"),

            TestCase(@"Conventional_PrimaryBus.xml", PRIMARYBUSES, TestName = "v27_Conventional_PrimaryBus"),
            TestCase(@"Exempted_PrimaryBus.xml", PRIMARYBUSES, TestName = "v27_Exempted_PrimaryBus"),
            TestCase(@"SHEV_IEPC_PrimaryBus.xml", PRIMARYBUSES, TestName = "v27_SHEV_IEPC_PrimaryBus"),
            TestCase(@"PEV_IEPC_PrimaryBus.xml", PRIMARYBUSES, TestName = "v27_PEV_IEPC_PrimaryBus"),
            TestCase(@"PEV_E2_PrimaryBus.xml", PRIMARYBUSES, TestName = "v27_PEV_E2_PrimaryBus"),
            TestCase(@"PEV_E3_PrimaryBus.xml", PRIMARYBUSES, TestName = "v27_PEV_E3_PrimaryBus"),
            TestCase(@"PEV_E4_PrimaryBus.xml", PRIMARYBUSES, TestName = "v27_PEV_E4_PrimaryBus"),
            TestCase(@"HEV_IHPC_PrimaryBus.xml", PRIMARYBUSES, TestName = "v27_HEV_IHPC_PrimaryBus"),
            TestCase(@"HEV_P2_PrimaryBus.xml", PRIMARYBUSES, TestName = "v27_HEV_P2_PrimaryBus"),
            TestCase(@"SHEV_S2_PrimaryBus.xml", PRIMARYBUSES, TestName = "v27_SHEV_S2_PrimaryBus"),
            TestCase(@"SHEV_S3_PrimaryBus.xml", PRIMARYBUSES, TestName = "v27_SHEV_S3_PrimaryBus"),
            TestCase(@"SHEV_S4_PrimaryBus.xml", PRIMARYBUSES, TestName = "v27_SHEV_S4_PrimaryBus"),
            TestCase(@"FCHV_F2_PrimaryBus.xml", PRIMARYBUSES, TestName = "v27_FCHV_F2_PrimaryBus"),
            TestCase(@"FCHV_F3_PrimaryBus.xml", PRIMARYBUSES, TestName = "v27_FCHV_F3_PrimaryBus"),
            TestCase(@"FCHV_F4_PrimaryBus.xml", PRIMARYBUSES, TestName = "v27_FCHV_F4_PrimaryBus"),
            TestCase(@"FCHV_IEPC_PrimaryBus.xml", PRIMARYBUSES, TestName = "v27_FCHV_IEPC_PrimaryBus"),
            TestCase(@"Multiple_PEV_E2_IEPC_PrimaryBus.xml", PRIMARYBUSES, TestName = "v27_Multiple_PEV_E2_IEPC_PrimaryBus"),
            TestCase(@"Multiple_PEV_E3_E4_PrimaryBus.xml", PRIMARYBUSES, TestName = "v27_Multiple_PEV_E3_E4_PrimaryBus"),
            TestCase(@"Multiple_FCHV_F2_IEPC_PrimaryBus.xml", PRIMARYBUSES, TestName = "v27_Multiple_FCHV_F2_IEPC_PrimaryBus"),
            TestCase(@"Multiple_FCHV_F3_F4_PrimaryBus.xml", PRIMARYBUSES, TestName = "v27_Multiple_FCHV_F3_F4_PrimaryBus"),
            TestCase(@"Multiple_SHEV_S2_IEPC_PrimaryBus.xml", PRIMARYBUSES, TestName = "v27_Multiple_SHEV_S2_IEPC_PrimaryBus"),
            TestCase(@"Multiple_SHEV_S3_S4_PrimaryBus.xml", PRIMARYBUSES, TestName = "v27_Multiple_SHEV_S3_S4_PrimaryBus"),

            TestCase(@"Conventional_CompletedBus.xml", COMPLETEDBUSES, TestName = "v27_Conventional_CompletedBus"),
            TestCase(@"Exempted_CompletedBus.xml", COMPLETEDBUSES, TestName = "v27_Exempted_CompletedBus"),
            TestCase(@"HEV_CompletedBus.xml", COMPLETEDBUSES, TestName = "v27_HEV_CompletedBus"),
            TestCase(@"PEV_CompletedBus.xml", COMPLETEDBUSES, TestName = "v27_PEV_CompletedBus"),
            TestCase(@"FCHV_CompletedBus.xml", COMPLETEDBUSES, TestName = "v27_FCHV_CompletedBus"),
            TestCase(@"Multiple_SHEV_CompletedBus.xml", COMPLETEDBUSES, TestName = "v27_Multiple_SHEV_CompletedBus"),
            TestCase(@"Multiple_PEV_CompletedBus.xml", COMPLETEDBUSES, TestName = "v27_Multiple_PEV_CompletedBus"),
            TestCase(@"Multiple_FCHV_CompletedBus.xml", COMPLETEDBUSES, TestName = "v27_Multiple_FCHV_CompletedBus"),
        ]
        public void TestVehicleInput(string jobfile, string testDir)
        {
            var filename = Path.Combine(testDir, jobfile);

            var inputData = XmlReader.Create(filename);
            var xmlDoc = new XmlDocument();
            xmlDoc.Load(inputData);

            if (xmlDoc.DocumentElement == null)
            {
                throw new VectoException("empty xml document!");
            }

            var documentType = XMLHelper.GetDocumentTypeFromRootElement(xmlDoc.DocumentElement.LocalName);
            if (documentType == null)
            {
                throw new VectoException("unknown xml file! {0}", xmlDoc.DocumentElement.LocalName);
            }

            var isValid = new XMLValidator(xmlDoc, null, XMLValidator.CallBackExceptionOnError).ValidateXML(documentType.Value);

            Assert.IsTrue(isValid);
        }
    }
}
