using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Ninject;
using NUnit.Framework;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.InputData.FileIO.XML;

namespace TUGraz.VectoCore.Tests.XML
{
	public class XMLMultistageBusDataTest
	{
		protected IXMLInputDataReader xmlInputReader;
		private IKernel _kernel;

		const string VIF =
			@"TestData\XML\XMLReaderDeclaration\SchemaVersionMultistage.0.1\vecto_multistage_primary_vehicle_stage_2_full.xml";

		[OneTimeSetUp]
		public void RunBeforeAnyTests()
		{
			Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);

			_kernel = new StandardKernel(new VectoNinjectModule());
			xmlInputReader = _kernel.Get<IXMLInputDataReader>();
		}


		[TestCase]
		public void TestVehicleMultistageBustInput()
		{
			var reader = XmlReader.Create(VIF);
			var inputDataProvider = xmlInputReader.Create(reader) as IMultistageBusInputDataProvider;
			var vehicle = inputDataProvider.PrimaryVehicle.ApplicationInformation;


		}
	}
}
