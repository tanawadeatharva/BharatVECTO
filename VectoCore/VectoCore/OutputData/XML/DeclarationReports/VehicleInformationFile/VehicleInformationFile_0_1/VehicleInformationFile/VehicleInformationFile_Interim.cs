using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using TUGraz.VectoCommon.Hashing;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Utils;
using TUGraz.VectoHashing;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.VehicleInformationFile.VehicleInformationFile_0_1.VIFReport
{
	public abstract class AbstractVehicleInformationFileCompleted : IXMLMultistepIntermediateReport
	{
		protected XNamespace VIF = XMLDefinitions.VEHICLE_INTERIM_FILE_TARGET_VERSION;

		protected XNamespace xsi = XNamespace.Get("http://www.w3.org/2001/XMLSchema-instance");
		protected XNamespace di = "http://www.w3.org/2000/09/xmldsig#";

		protected IPrimaryVehicleInformationInputDataProvider _primaryVehicleInputData;
		protected IList<IManufacturingStageInputData> _manufacturingStageInputData;
		protected IManufacturingStageInputData _consolidatedInputData;
		protected IVehicleDeclarationInputData _vehicleInputData;

		protected XElement _primaryVehicle;
		protected List<XElement> _manufacturingStages;
		protected List<XAttribute> _namespaceAttributes;
		
		protected readonly IVIFReportInterimFactory _vifFactory;
		protected IMultistageVIFInputData _inputData;

		protected AbstractVehicleInformationFileCompleted(IVIFReportInterimFactory vifReportFactory)
		{
			_manufacturingStages = new List<XElement>();
			_namespaceAttributes = new List<XAttribute>();

			_vifFactory = vifReportFactory;
		}

		#region Implementation of IXMLVehicleInformationFile


		public void Initialize(VectoRunData modelData)
		{
			//InitializeVehicleData(modelData.MultistageVifInputData);

			_primaryVehicleInputData = modelData.MultistageVifInputData.MultistageJobInputData.JobInputData.PrimaryVehicle;
			_manufacturingStageInputData = modelData.MultistageVifInputData.MultistageJobInputData.JobInputData.ManufacturingStages;
			_consolidatedInputData = modelData.MultistageVifInputData.MultistageJobInputData.JobInputData.ConsolidateManufacturingStage;
			_vehicleInputData = modelData.MultistageVifInputData.VehicleInputData;

			_inputData = modelData.MultistageVifInputData;

			SetInputXMLData(_primaryVehicleInputData.Vehicle.XMLSource);
		}

		private void SetInputXMLData(XmlNode primeVehicleNode)
		{
			var nodes = GetDocumentNodes(primeVehicleNode);
			var documentNode = GetDocumentNode(nodes);
			SetXmlNamespaceAttributes(nodes);

			var xDocument = XElement.Parse(documentNode.InnerXml);
			foreach (var xElement in xDocument.Descendants()) {
				if (xElement.Name.LocalName == XMLNames.Bus_PrimaryVehicle)
					_primaryVehicle = xElement;
				else if (xElement.Name.LocalName == XMLNames.ManufacturingStep)
					_manufacturingStages.Add(xElement);
			}

			if (_manufacturingStages.Count == 0)
				_manufacturingStages = null;
		}

		private List<XmlNode> GetDocumentNodes(XmlNode primeVehicleNode)
		{
			var nodes = new List<XmlNode>();
			NodeParentSearch(primeVehicleNode, nodes);
			return nodes;
		}

		private XmlNode GetDocumentNode(List<XmlNode> nodes)
		{
			if (nodes == null || nodes.Count == 0)
				return null;

			foreach (var node in nodes) {
				if (node.NodeType == XmlNodeType.Document)
					return node;
			}
			return null;
		}

		private void SetXmlNamespaceAttributes(List<XmlNode> nodes)
		{
			if (nodes == null || nodes.Count == 0)
				return;
			XmlAttributeCollection namespaceAttributes = null;
			foreach (var node in nodes) {
				if (node.LocalName == XMLNames.VectoOutputMultistep) {
					namespaceAttributes = node.Attributes;
					break;
				}
			}

			if (namespaceAttributes == null || namespaceAttributes.Count == 0)
				return;

			foreach (XmlAttribute attribute in namespaceAttributes) {
				if (string.IsNullOrEmpty(attribute.Prefix))
					_namespaceAttributes.Add(new XAttribute(attribute.LocalName, attribute.Value));
				else if (attribute.Prefix == "xmlns")
					_namespaceAttributes.Add(new XAttribute(XNamespace.Xmlns + attribute.LocalName, attribute.Value));
				else if (attribute.Prefix == "xsi")
					_namespaceAttributes.Add(new XAttribute(xsi + attribute.LocalName, attribute.Value));
			}
		}

		private void NodeParentSearch(XmlNode currentNode, List<XmlNode> nodes)
		{
			if (currentNode?.ParentNode == null || nodes == null)
				return;

			nodes.Add(currentNode.ParentNode);
			NodeParentSearch(currentNode.ParentNode, nodes);
		}

		public XDocument Report { get; protected set; }
		
		public void GenerateReport()
		{
			var retVal = new XDocument();
			retVal.Add(
				new XElement(VIF + XMLNames.VectoOutputMultistep,
					_namespaceAttributes,
					_primaryVehicle,
					_manufacturingStages,
					GenerateInputManufacturingStage()
				)
			);
			Report = retVal;
		}

		#endregion

		protected abstract XElement GenerateInputManufacturingStage();
	}

	// ----------------------------------

	internal abstract class VehicleInformationFile_InterimStep : AbstractVehicleInformationFileCompleted
	{
		protected VehicleInformationFile_InterimStep(IVIFReportInterimFactory vifFactory) : base(vifFactory) { }

		protected override XElement GenerateInputManufacturingStage()
		{
			var multistageId = $"{VectoComponents.VectoManufacturingStep.HashIdPrefix()}{GetGUID()}";
			var vehicleId = $"{VectoComponents.Vehicle.HashIdPrefix()}{GetGUID()}";

			var stage = new XElement(VIF + XMLNames.ManufacturingStep,
				new XAttribute(XMLNames.ManufacturingStep_StepCount, GetStageNumber()),
				new XAttribute(XNamespace.Xmlns + "xsi", xsi),
				new XElement(VIF + XMLNames.Report_DataWrap,
					new XAttribute(xsi + XMLNames.Attr_Type, "BusManufacturingStepDataType"),
					new XAttribute(XMLNames.Component_ID_Attr, multistageId),
					GetHashPreviousStepElement(),
					GetVehicleElement(),
					XMLHelper.GetApplicationInfo(VIF)));

			var sigXElement = GetSignatureElement(stage);
			stage.LastNode.Parent.Add(sigXElement);
			return stage;
		}

		protected abstract XElement GetVehicleElement();

		private XElement GetHashPreviousStepElement()
		{
			DigestData digitData;
			if (_manufacturingStageInputData == null || _manufacturingStageInputData.Count == 0) {
				digitData = _primaryVehicleInputData.VehicleSignatureHash;
			} else {
				digitData = _manufacturingStageInputData.Last().Signature;
			}

			return new XElement(VIF + "HashPreviousStep",
				digitData.ToXML(di));
		}

		private XElement GetSignatureElement(XElement stage)
		{
			var stream = new MemoryStream();
			var w = new XmlTextWriter(stream, Encoding.UTF8);
			stage.WriteTo(w);
			w.Flush();
			stream.Seek(0, SeekOrigin.Begin);

			return new XElement(VIF + XMLNames.DI_Signature,
				VectoHash.Load(stream).ComputeXmlHash
					(VectoHash.DefaultCanonicalizationMethod, VectoHash.DefaultDigestMethod)
			);
		}

		private int GetStageNumber()
		{
			if (_manufacturingStageInputData == null || _manufacturingStageInputData.Count == 0)
				return 2;

			return _manufacturingStageInputData.Last().StepCount + 1;
		}

		protected string GetGUID()
		{
			return Guid.NewGuid().ToString("n").Substring(0, 20);
		}
	}

	// ----------------------------------

	internal class Conventional_CompletedBus_VIF : VehicleInformationFile_InterimStep
	{
		public Conventional_CompletedBus_VIF(IVIFReportInterimFactory vifFactory) : base(vifFactory) { }


		#region Overrides of VehicleInformationFile_InterimStep

		protected override XElement GetVehicleElement()
		{
            return _vifFactory.GetConventionalVehicleType().GetElement(_inputData);
		}

		#endregion
	}

	// ----------------------------------

	internal class HEV_CompletedBus_VIF : VehicleInformationFile_InterimStep
	{
		public HEV_CompletedBus_VIF(IVIFReportInterimFactory vifFactory) : base(vifFactory) { }


		#region Overrides of VehicleInformationFile_InterimStep

		protected override XElement GetVehicleElement()
		{
			return _vifFactory.GetHEVVehicleType().GetElement(_inputData);
		}

		#endregion
	}

	// ----------------------------------

	internal class PEV_CompletedBus_VIF : VehicleInformationFile_InterimStep
	{
		public PEV_CompletedBus_VIF(IVIFReportInterimFactory vifFactory) : base(vifFactory) { }


		#region Overrides of VehicleInformationFile_InterimStep

		protected override XElement GetVehicleElement()
		{
			return _vifFactory.GetPEVVehicleType().GetElement(_inputData);
		}

		#endregion
	}

	// ----------------------------------

	internal class FCHV_CompletedBus_VIF : VehicleInformationFile_InterimStep
	{
        public FCHV_CompletedBus_VIF(IVIFReportInterimFactory vifFactory) : base(vifFactory) { }

        protected override XElement GetVehicleElement()
        {
            return _vifFactory.Get_FCHV_VehicleType().GetElement(_inputData);
        }
    }

    // ----------------------------------

    internal class Exempted_CompletedBus_VIF : VehicleInformationFile_InterimStep
	{
		public Exempted_CompletedBus_VIF(IVIFReportInterimFactory vifFactory) : base(vifFactory) { }


		#region Overrides of VehicleInformationFile_InterimStep

		protected override XElement GetVehicleElement()
		{
			return _vifFactory.GetExemptedVehicleType().GetElement(_inputData);
		}

		#endregion
	}
}