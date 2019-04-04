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
		protected XmlNode JobNode;
		protected IVehicleDeclarationInputData _vehicle;

		[Inject]
		public IDeclarationInjectFactory Factory { protected get; set; }

		public XMLJobDataReaderV10(IXMLDeclarationJobInputData jobData, XmlNode jobNode, bool verifyXML) : base(
			jobData, jobNode, verifyXML)
		{
			JobNode = jobNode;
			JobData = jobData;
		}

		#region Implementation of IXMLJobDataReader

		public virtual IVehicleDeclarationInputData CreateVehicle
		{
			get { return _vehicle ?? (_vehicle = CreateComponent(XMLNames.Component_Vehicle, VehicleCreator)); }
		}

		#endregion

		protected virtual IVehicleDeclarationInputData VehicleCreator(string version, XmlNode vehicleNode, string sourceFile)
		{
			var vehicle = Factory.CreateVehicleData(version, JobData, vehicleNode, sourceFile);
			vehicle.ComponentReader = Factory.CreateComponentReader(version, vehicle, vehicleNode, VerifyXML);
			vehicle.ADASReader = Factory.CreateADASReader(version, vehicle, vehicleNode, VerifyXML);
			vehicle.PTOReader = Factory.CreatePTOReader(version, vehicle, vehicleNode, VerifyXML);
			return vehicle;
		}
	}

	// ---------------------------------------------------------------------------------------

	public class XMLJobDataReaderV20 : XMLJobDataReaderV10
	{
		public new const string NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V20;

		public XMLJobDataReaderV20(IXMLDeclarationJobInputData jobData, XmlNode jobNode, bool verifyXML) : base(
			jobData, jobNode, verifyXML) { }
	}
}
