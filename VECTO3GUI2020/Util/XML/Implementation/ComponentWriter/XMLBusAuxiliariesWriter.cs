using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;

namespace VECTO3GUI2020.Util.XML.Implementation.ComponentWriter
{
	public abstract class XMLBusAuxiliariesWriter
	{
		private readonly IBusAuxiliariesDeclarationData _inputData;
		private XElement _xElement;

		protected XMLBusAuxiliariesWriter(IBusAuxiliariesDeclarationData inputData)
		{
			_inputData = inputData;
		}

		public XElement GetElement()
		{
			if (_xElement == null)
			{
				Initialize();
				CreateElements();
			}

			return _xElement;
		}
		public abstract void Initialize();

		public abstract void CreateElements();
	}



	public class XMLBusAuxiliariesWriterMultistage : XMLBusAuxiliariesWriter
	{
		private XNamespace _defaultNamespace;
		public XMLBusAuxiliariesWriterMultistage(IBusAuxiliariesDeclarationData inputData) : base(inputData) { }

		#region Overrides of XMLBusAuxiliariesWriter

		public override void Initialize()
		{
			_defaultNamespace = XMLNamespaces.V23;
		}

		public override void CreateElements()
		{
			throw new System.NotImplementedException();
		}

		#endregion
	}
}