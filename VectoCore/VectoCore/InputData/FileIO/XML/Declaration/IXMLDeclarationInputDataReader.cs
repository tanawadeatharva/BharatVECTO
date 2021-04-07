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

	public interface IXMLDeclarationMultistageVehicleInputDataReader
	{
		IDeclarationMultistageJobInputData JobData { get; }
	}

	public interface IXMLMultistageJobReader
	{
		IPrimaryVehicleInformationInputDataProvider PrimaryVehicle { get; }

		IList<IManufacturingStageInputData> ManufacturingStages { get; }

		IManufacturingStageInputData ConsolidateManufacturingStage { get; }

		VectoSimulationJobType JobType { get; }

		bool InputComplete { get; }
	}

	public interface IXMLMultistageReader 
	{
		IVehicleDeclarationInputData Vehicle { get; }

		IApplicationInformation ApplicationInformation { get; }

		DigestData GetDigestData(XmlNode xmlNode);
	}
}
