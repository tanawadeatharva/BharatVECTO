using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using TUGraz.VectoCore.Models.Connector.Ports;
using TUGraz.VectoCore.OutputData.XML.Engineering.Interfaces;
using TUGraz.VectoCore.OutputData.XML.Engineering.Writer;
using VECTO3GUI2020.Util.XML.Implementation.ComponentWriter;

namespace VECTO3GUI2020.Util.XML.Interfaces
{
    public interface IXMLWriterFactory
	{
		IXMLDeclarationJobWriter CreateJobWriter(IDeclarationJobInputData inputData);
		IXMLVehicleWriter CreateVehicleWriter(IVehicleDeclarationInputData inputData);
		IXMLComponentWriter CreateComponentWriter(IComponentInputData inputData);
		IXMLComponentWriter CreateComponentWriter(IAuxiliariesDeclarationInputData inputData);
		IXMLComponentWriter CreateComponentWriter(IAxlesDeclarationInputData inputData);
		IXMLComponentWriter CreateComponentWriter(IAxleDeclarationInputData inputData);
		IXMLComponentWriter CreateComponentWriter(IAirdragDeclarationInputData inputData);

		IXMLComponentWriter CreateComponentWriter(IPTOTransmissionInputData inputData);
		IXMLComponentsWriter CreateComponentsWriter(IVehicleComponentsDeclaration inputData);

		IXMLBusAuxiliariesWriter CreateBuxAuxiliariesWriter(IBusAuxiliariesDeclarationData inputData);
	}

	public interface IXMLWriterFactoryInternal
	{
		TWriter CreateWriter<TWriter, TData>(DataSource source, TData inputData);
	}

	public class XMLWriterFactory : IXMLWriterFactory
	{
		private IXMLWriterFactoryInternal _internalFactory;

		private TWriter CreateWriter<TWriter, TData>(DataSource source, TData inputData)
		{
			return _internalFactory.CreateWriter<TWriter, TData>(source, inputData);
		}


		#region Implementation of IXMLWriterFactory

		public IXMLDeclarationJobWriter CreateJobWriter(IDeclarationJobInputData inputData)
		{
			throw new System.NotImplementedException();
		}

		public IXMLVehicleWriter CreateVehicleWriter(IVehicleDeclarationInputData inputData)
		{
			return _internalFactory.CreateWriter<IXMLVehicleWriter, IVehicleDeclarationInputData>(inputData.DataSource, inputData);
			//throw new System.NotImplementedException();
		}

		public IXMLComponentWriter CreateComponentWriter(IComponentInputData inputData)
		{
			throw new System.NotImplementedException();
		}

		public IXMLComponentWriter CreateComponentWriter(IAuxiliariesDeclarationInputData inputData)
		{
			throw new System.NotImplementedException();
		}

		public IXMLComponentWriter CreateComponentWriter(IAxlesDeclarationInputData inputData)
		{
			throw new System.NotImplementedException();
		}

		public IXMLComponentWriter CreateComponentWriter(IAxleDeclarationInputData inputData)
		{
			throw new System.NotImplementedException();
		}

		public IXMLComponentWriter CreateComponentWriter(IAirdragDeclarationInputData inputData)
		{
			throw new System.NotImplementedException();
		}

		public IXMLComponentWriter CreateComponentWriter(IPTOTransmissionInputData inputData)
		{
			throw new System.NotImplementedException();
		}

		public IXMLComponentsWriter CreateComponentsWriter(IVehicleComponentsDeclaration inputData)
		{
			throw new System.NotImplementedException();
		}

		public IXMLBusAuxiliariesWriter CreateBuxAuxiliariesWriter(IBusAuxiliariesDeclarationData inputData)
		{
			throw new System.NotImplementedException();
		}

		#endregion
	}
}
