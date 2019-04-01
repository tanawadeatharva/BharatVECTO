using System.Xml;
using Ninject;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Factory;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Reader.Impl
{
	public class XMLJobDataReaderV10 : AbstractComponentReader, IXMLJobDataReader
	{
		public const string NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V10;

		protected IXMLDeclarationJobInputData JobData;
		private XmlNode JobNode;
		private IVehicleDeclarationInputData _vehicle;

		[Inject]
		public IDeclarationInjectFactory Factory { protected get; set; }

		public XMLJobDataReaderV10(IXMLDeclarationJobInputData jobData, XmlNode jobNode, bool verifyXML) : base (jobData, jobNode, verifyXML)
		{
			JobNode = jobNode;
			JobData = jobData;
		}

		#region Implementation of IXMLJobDataReader

		public IVehicleDeclarationInputData CreateVehicle
		{
			get { return _vehicle ?? (_vehicle = CreateComponent(XMLNames.Component_Vehicle, VehicleCreator)); }
		}


		#endregion

		private IVehicleDeclarationInputData VehicleCreator(string version, XmlNode vehicleNode, string sourceFile)
		{
			var vehicle = Factory.CreateVehicleData(version, JobData, vehicleNode, sourceFile);
			vehicle.ComponentReader = Factory.CreateComponentReader(version, vehicle, vehicleNode, VerifyXML);
			vehicle.ADASReader = Factory.CreateADASReader(version, vehicle, vehicleNode, VerifyXML);
			vehicle.PTOReader = Factory.CreatePTOReader(version, vehicle, vehicleNode, VerifyXML);
			return vehicle;
		}

	}
}
