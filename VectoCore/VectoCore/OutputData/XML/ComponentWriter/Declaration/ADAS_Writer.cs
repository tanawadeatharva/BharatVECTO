using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider;
using TUGraz.VectoCore.OutputData.XML.GroupWriter.Declaration;

namespace TUGraz.VectoCore.OutputData.XML.ComponentWriter
{
	public interface IDeclarationAdasWriter
	{
		XElement[] GetComponentElements(IAdvancedDriverAssistantSystemDeclarationInputData adas);

		XElement GetComponent(IAdvancedDriverAssistantSystemDeclarationInputData adas);
	}

	public abstract class AdasWriter : ComponentWriter, IDeclarationAdasWriter
	{
		protected abstract string AdasType { get; }

		protected AdasWriter(XNamespace writerNamespace) : base(writerNamespace)
		{
		}

		#region Implementation of IDeclarationAdasWriter

		public abstract XElement[] GetComponentElements(IAdvancedDriverAssistantSystemDeclarationInputData adas);

		public XElement GetComponent(IAdvancedDriverAssistantSystemDeclarationInputData adas)
		{
			return new XElement(_writerNamespace + XMLNames.Vehicle_ADAS,
				new XAttribute(XMLDeclarationNamespaces.Xsi + XMLNames.Attr_Type, AdasType),
				GetComponentElements(adas));
		}

        #endregion
    }


    public class AdasConventionalWriter : AdasWriter
    {
		public AdasConventionalWriter(XNamespace writerNamespace) : base(writerNamespace) { }

		#region Implementation of IADASWriter

		protected override string AdasType => GroupNames.ADAS_Conventional_Type;

        public override XElement[] GetComponentElements(IAdvancedDriverAssistantSystemDeclarationInputData adas)
		{
			var elements = new List<XElement>();
			
			elements.Add(new XElement(_writerNamespace + XMLNames.Vehicle_ADAS_EngineStopStart, adas.EngineStopStart));
			elements.Add(new XElement(_writerNamespace + XMLNames.Vehicle_ADAS_EcoRollWithoutEngineStop, adas.EcoRollWithOutEngineStop()));
			elements.Add(new XElement(_writerNamespace + XMLNames.Vehicle_ADAS_EcoRollWithEngineStopStart, adas.EcoRollWithEngineStop()));
			elements.Add(new XElement(_writerNamespace + XMLNames.Vehicle_ADAS_PCC,adas.PredictiveCruiseControl.ToXMLFormat()));
			if (adas.ATEcoRollReleaseLockupClutch != null) {
				elements.Add(new XElement(_writerNamespace + XMLNames.Vehicle_ADAS_ATEcoRollReleaseLockupClutch, adas.ATEcoRollReleaseLockupClutch));
			}

			return elements.ToArray();
		}



		#endregion
	}

	public class AdasHEVWriter : AdasWriter
	{
		protected override string AdasType => GroupNames.ADAS_HEV_Type;
        public AdasHEVWriter(XNamespace writerNamespace) : base(writerNamespace) { }

		#region Implementation of IADASWriter

		public override XElement[] GetComponentElements(IAdvancedDriverAssistantSystemDeclarationInputData adas)
		{
			var elements = new List<XElement>();

			elements.Add(new XElement(_writerNamespace + XMLNames.Vehicle_ADAS_EngineStopStart, adas.EngineStopStart));
			elements.Add(new XElement(_writerNamespace + XMLNames.Vehicle_ADAS_PCC, adas.PredictiveCruiseControl.ToXMLFormat()));

			return elements.ToArray();
		}

		#endregion
	}

	public class AdasPEVWriter : AdasWriter
	{
		protected override string AdasType => GroupNames.ADAS_PEV_Type;
		public AdasPEVWriter(XNamespace writerNamespace) : base(writerNamespace) { }
			
		#region Implementation of IADASWriter

		public override XElement[] GetComponentElements(IAdvancedDriverAssistantSystemDeclarationInputData adas)
		{
			var elements = new List<XElement>();
			elements.Add(new XElement(_writerNamespace + XMLNames.Vehicle_ADAS_PCC, adas.PredictiveCruiseControl.ToXMLFormat()));
			return elements.ToArray();
		}

		#endregion
	}

	public class AdasIEPCWriter : AdasWriter
	{
		protected override string AdasType => GroupNames.ADAS_IEPC_Type;
		public AdasIEPCWriter(XNamespace writerNamespace) : base(writerNamespace) { }

		#region Implementation of IADASWriter

		public override XElement[] GetComponentElements(IAdvancedDriverAssistantSystemDeclarationInputData adas)
		{
			var elements = new List<XElement>();
			elements.Add(new XElement(_writerNamespace + XMLNames.Vehicle_ADAS_PCC, adas.PredictiveCruiseControl.ToXMLFormat()));
			return elements.ToArray();
		}

		#endregion
	}

}
