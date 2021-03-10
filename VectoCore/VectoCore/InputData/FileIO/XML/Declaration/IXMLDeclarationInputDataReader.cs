using System.Collections.Generic;
using System.Xml;
using TUGraz.VectoCommon.InputData;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration
{
	public interface IXMLDeclarationInputDataReader
	{
		IDeclarationJobInputData JobData { get; }
	}

	public interface IXMLDeclarationPrimaryVehicleBusInputDataReader : IXMLDeclarationInputDataReader
	{
		IResultsInputData ResultsInputData { get; }

		DigestData GetDigestData(XmlNode xmlNode);

		IApplicationInformation ApplicationInformation { get; }
	}

	public interface IXMLDeclarationMultistageVehicleBusInputDataReader
	{
		IDeclarationMultistageJobInputData JobData { get; }
	}

	public interface IXMLMultistageJobReader
	{

	}
}
