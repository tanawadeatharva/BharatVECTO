using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCore.OutputData.XML.Engineering.Interfaces {
	public interface IXMLEngineeringComponentWriter
	{
		IXMLEngineeringWriter Writer { set; }

		XAttribute GetXMLTypeAttribute();

		object[] WriteXML(IComponentInputData inputData);

		object[] WriteXML(IDriverModelData inputData);

		object[] WriteXML(IEngineeringInputDataProvider inputData);

		object[] WriteXML(ITransmissionInputData inputData, int gearNumber);
		
		object[] WriteXML(IAxleEngineeringInputData axle, int idx, Meter dynamicTyreRadius);
		object[] WriteXML(IAuxiliaryEngineeringInputData inputData);

		object[] WriteXML(IAdvancedDriverAssistantSystemsEngineering inputData);
	}


	public interface IXMLEngineeringDriverDataWriter : IXMLEngineeringComponentWriter { }

	public interface IXMLLookaheadDataWriter : IXMLEngineeringComponentWriter { }

	public interface IXMLOverspeedDataWriter : IXMLEngineeringComponentWriter { }

	public interface IXMLAccelerationDataWriter : IXMLEngineeringComponentWriter { }

	public interface IXMLGearshiftDataWriter : IXMLEngineeringComponentWriter { }

	public interface IXMLVehicleDataWriter : IXMLEngineeringComponentWriter { }

	public interface IXMLEngineeringGearWriter : IXMLEngineeringComponentWriter { }

	public interface IXMLEngineeringTorqueconverterWriter : IXMLEngineeringComponentWriter { }

	public interface IXMLEngineeringAxlegearWriter :IXMLEngineeringComponentWriter { }

	public interface IXMLEngineeringRetarderWriter : IXMLEngineeringComponentWriter { }

	public interface IXMLEngineeringAirdragWriter : IXMLEngineeringComponentWriter { }

	public interface IXMLEngineeringAxlesWriter : IXMLEngineeringComponentWriter { }

	public interface IXMLEngineeringAxleWriter : IXMLEngineeringComponentWriter { }

	public interface IXmlEngineeringTyreWriter : IXMLEngineeringComponentWriter { }

	public interface IXMLEngineeringAngledriveWriter : IXMLEngineeringComponentWriter { }

	public interface IXMLAuxiliariesWriter : IXMLEngineeringComponentWriter { }

	public interface IXMLAuxiliaryWriter : IXMLEngineeringComponentWriter { }

	public interface IXMLEngineeringADASWriter : IXMLEngineeringComponentWriter { }



}