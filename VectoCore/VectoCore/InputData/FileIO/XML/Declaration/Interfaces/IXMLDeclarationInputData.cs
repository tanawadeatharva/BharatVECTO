using System.Collections.Generic;
using System.Xml;
using TUGraz.VectoCommon.InputData;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces
{
	public interface IXMLDeclarationInputData : IDeclarationInputDataProvider, IXMLResource
	{
		IXMLDeclarationInputDataReader Reader { set; }
	}



	public interface IXMLPrimaryVehicleBusInputData : IPrimaryVehicleInformationInputDataProvider, IXMLResource
	{
		IXMLDeclarationPrimaryVehicleBusInputDataReader Reader { set; }

		XmlNode ResultsNode { get; }

		XmlNode ApplicationInformationNode { get; }
	}

	public interface IXMLMultistageInputDataProvider : IMultistageBusInputDataProvider, IXMLResource
	{
		IXMLDeclarationMultistageVehicleInputDataReader Reader { set; }

		
	}
}
