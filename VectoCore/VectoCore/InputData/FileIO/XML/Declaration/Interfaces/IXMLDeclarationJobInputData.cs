using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Reader;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces
{
	public interface IXMLDeclarationJobInputData : IDeclarationJobInputData, IXMLResource
	{
		IXMLJobDataReader Reader { set; }

		IXMLDeclarationInputData InputData { get; }
	}

	public interface IXMLDeclarationMultistageJobInputData : IDeclarationMultistageJobInputData, IXMLResource
	{
		IXMLMultistageJobReader Reader { set; }

		IXMLMultistageBusInputDataProvider InputData { get; }
	}

	public interface IXMLPrimaryVehicleBusJobInputData : IDeclarationJobInputData, IXMLResource
	{
		IXMLJobDataReader Reader { set; }

		IXMLPrimaryVehicleBusInputData InputData { get; }
	}
}
