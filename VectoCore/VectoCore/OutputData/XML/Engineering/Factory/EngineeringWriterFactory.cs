using System;
using System.Globalization;
using System.Linq;
using Ninject;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.OutputData.XML.Engineering.Interfaces;

namespace TUGraz.VectoCore.OutputData.XML.Engineering.Factory
{
	public interface IEngineeringWriterFactory
	{
		IXMLEngineeringJobWriter CreateJobWriter(string version, IXMLEngineeringWriter writer, IEngineeringInputDataProvider inputData);

		IXMLEngineeringComponentWriter GetWriter<T>(T inputData, IXMLEngineeringWriter writer, DataSource source) where T : class;

		IXMLEngineeringComponentWriter GetWriter<T>(T inputData, IXMLEngineeringWriter writer) where T : class, IComponentInputData;

		//IXMLEngineeringComponentWriter GetWriter(IComponentInputData inputData, IXMLEngineeringWriter xmlEngineeringWriter);

		//IXMLEngineeringComponentWriter GetWriter(object inputData, IXMLEngineeringWriter xmlEngineeringWriter, DataSource source);

		//IXMLEngineeringComponentWriter CreateComponentsWriter(string version, IXMLEngineeringWriter writer);
	}

	public class EngineeringWriterFactory : IEngineeringWriterFactory
	{
		[Inject]
		public IKernel Kernel { protected get; set; }

		public IXMLEngineeringJobWriter CreateJobWriter(
			string version, IXMLEngineeringWriter writer, IEngineeringInputDataProvider inputData)
		{
			var jobWriter = Kernel.Get<IXMLEngineeringJobWriter>(version);
			jobWriter.Writer = writer;
			jobWriter.InputData = inputData;
			return jobWriter;
		}

		public virtual IXMLEngineeringComponentWriter GetWriter<T>(
			T inputData, IXMLEngineeringWriter writer, DataSource source) where T : class 
		{
			return DoGetWriter(typeof(T), writer, source);
		}

		public virtual IXMLEngineeringComponentWriter GetWriter<T>(
			T inputData, IXMLEngineeringWriter writer) where T : class, IComponentInputData
		{
			return DoGetWriter(typeof(T), writer, inputData.DataSource);
		}

		protected virtual IXMLEngineeringComponentWriter DoGetWriter(Type inputDataType, IXMLEngineeringWriter xmlEngineeringWriter, DataSource source)
		{
			Type writerType = null;
			if (inputDataType.IsInterface) {
				writerType = XMLWriterMapping.GetWriterType(inputDataType);
			} else { 
				foreach (var type in inputDataType.GetInterfaces()) {
					writerType = XMLWriterMapping.GetWriterType(type);
					if (writerType != null) {
						break;
					}
				}
			}

			if (writerType == null) {
				throw new VectoException("no writer defined for {0}", inputDataType.FullName);
			}
			 
			try {
				var version = source.SourceVersion;
				if (!source.SourceType.IsXMLFormat()) {
					var bindings = Kernel.GetBindings(writerType).ToArray();
					if (bindings.Any()) {
						version = bindings.MaxBy(b => b.Metadata.Name.ToDouble()).Metadata.Name;
					}
				}
				return GetEngineeringWriter(inputDataType, version, writerType, xmlEngineeringWriter);
			} catch (Exception) {
				var bindings = Kernel.GetBindings(writerType).ToArray();
				if (bindings.Any()) {
					var mostRecent = bindings.MaxBy(b => {
						double retVal;
						var success = double.TryParse(b.Metadata.Name, NumberStyles.Float, CultureInfo.InvariantCulture, out retVal);
						return success ? retVal : -1;
					}).Metadata.Name;
					return GetEngineeringWriter(inputDataType, mostRecent, writerType, xmlEngineeringWriter);
				}

			}
			throw new VectoException("No binding found for {0} ({2}) version {1}", inputDataType.FullName, source.SourceVersion, writerType.FullName);
		}

		private IXMLEngineeringComponentWriter GetEngineeringWriter(Type inputDataType, string version, Type writerType, IXMLEngineeringWriter xmlEngineeringWriter)
		{
			var retVal = Kernel.Get(writerType, version) as IXMLEngineeringComponentWriter;
			if (retVal == null) {
				throw new VectoException("Writer for type {0} is not an IXMLEngineeringWriter!", inputDataType.FullName);
			}

			retVal.Writer = xmlEngineeringWriter;
			return retVal;
		}
	}
}
