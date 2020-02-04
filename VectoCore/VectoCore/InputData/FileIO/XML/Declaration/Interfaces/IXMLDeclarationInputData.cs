using System.Xml;
using TUGraz.VectoCommon.InputData;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces
{
	public interface IXMLDeclarationInputData : IDeclarationInputDataProvider, IXMLResource
	{
		IXMLDeclarationInputDataReader Reader { set; }
	}



	public interface IXMLPrimaryVehicleBusInputData : IPrimaryVehicleInputDataProvider, IXMLResource
	{
		IXMLDeclarationPrimaryVehicleBusInputDataReader Reader { set; }

		XmlNode ResultsNode { get; }

		XmlNode ApplicationInformationNode { get; }
	}
}
