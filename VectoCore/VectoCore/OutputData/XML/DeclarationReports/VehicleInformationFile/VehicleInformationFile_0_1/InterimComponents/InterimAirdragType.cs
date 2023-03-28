using System;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Hashing;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.VehicleInformationFile.VehicleInformationFile_0_1.InterimComponents
{
	public class AirdragInterimType : IReportMultistepCompletedBusTypeWriter
	{
		protected XNamespace xsi = XNamespace.Get("http://www.w3.org/2001/XMLSchema-instance");

		protected XNamespace di = "http://www.w3.org/2000/09/xmldsig#";
		protected XNamespace v10 = "urn:tugraz:ivt:VectoAPI:DeclarationDefinitions:v1.0";
		protected XNamespace v20 = "urn:tugraz:ivt:VectoAPI:DeclarationDefinitions:v2.0";
		protected XNamespace v23 = "urn:tugraz:ivt:VectoAPI:DeclarationDefinitions:v2.3";
		protected XNamespace v24 = "urn:tugraz:ivt:VectoAPI:DeclarationDefinitions:v2.4";

		public AirdragInterimType() {}

		#region Implementation of IReportMultistepCompletedBusTypeWriter

		public XElement GetElement(IMultistageVIFInputData inputData)
		{
			var airdrag = inputData.VehicleInputData.Components?.AirdragInputData;
			if (airdrag != null && inputData.VehicleInputData.AirdragModifiedMultistep != false) {
				return GetAirdragElement(airdrag);
			}

			return inputData.VehicleInputData.AirdragModifiedMultistep == true
				? GetBusAirdragUseStandardValues()
				: null;
		}

		#endregion

		protected XElement GetAirdragElement(XMLDeclarationAirdragDataProviderV10 airdrag)
		{
			var retVal = new XElement(v24 + XMLNames.Component_AirDrag);
			var tmp = XElement.Load(airdrag.XMLSource.CreateNavigator().ReadSubtree());
			retVal.Add(tmp.Elements());
			return retVal;
		}

		protected XElement GetAirdragElement(XMLDeclarationAirdragDataProviderV20 airdrag)
		{
			var retVal = new XElement(v24 + XMLNames.Component_AirDrag//, 
				//new XAttribute("xmlns", v20.NamespaceName)
				);
			var tmp = XElement.Load(airdrag.XMLSource.CreateNavigator().ReadSubtree());
			retVal.Add(tmp.Elements());
			return retVal;
		}

		protected XElement GetAirdragElement(XMLDeclarationAirdragDataProviderV24 airdrag)
		{
			var retVal = new XElement(v24 + XMLNames.Component_AirDrag);
			var tmp = XElement.Load(airdrag.XMLSource.CreateNavigator().ReadSubtree());
			retVal.Add(tmp.Elements());
			return retVal;
		}

		protected XElement GetAirdragElement(IAirdragDeclarationInputData inputData, string version)
		{
			var retVal = new XElement(v24 + XMLNames.Component_AirDrag);
				
			var tmp = XElement.Load(inputData.XMLSource.ParentNode.CreateNavigator().ReadSubtree());
			var dataElement = tmp.Elements().First(e => e.Name.LocalName == XMLNames.ComponentDataWrapper);
			dataElement.Name =
				v20 + XMLNames.ComponentDataWrapper;
			dataElement.Add(new XAttribute("xmlns", inputData.DataSource.TypeVersion));

			var signatureElement = tmp.Elements().First(e => e.Name.LocalName == XMLNames.DI_Signature);
			signatureElement.Name = v20 + XMLNames.DI_Signature;
			

            retVal.Add(tmp.Elements());
			return retVal;
        }

        protected XElement GetAirdragElement(IAirdragDeclarationInputData airdrag)
        {
			switch (airdrag) {
				case XMLDeclarationAirdragDataProviderV20 av20:
					return GetAirdragElement(av20);
				case XMLDeclarationAirdragDataProviderV24 av24:
					return GetAirdragElement(av24);
				case XMLDeclarationAirdragDataProviderV10 av10:
					return GetAirdragElement(av10);
			}


			var sourceVersion = airdrag.DataSource.SourceVersion;
			if (sourceVersion.IsOneOf(v10.GetVersionFromNamespaceUri(), v20.GetVersionFromNamespaceUri(),
					v24.GetVersionFromNamespaceUri())) {
				return GetAirdragElement(airdrag, "");
            }

	

            throw new VectoException(
                $"Specific implementation for Airdrag Data (Interim Stage) missing {airdrag.GetType().Name}");

            //	//if (airdrag == null)
            //	//	return null;

            //	//var dataAndSig =
            //	//	airdrag.XMLSource.SelectSingleNode(XMLHelper.QueryLocalName(XMLNames.Component_AirDrag));
            //	//var dataXmlType = dataAndSig.FirstChild.SchemaInfo.SchemaAttribute.QualifiedName;

            //	//return null;
            //	//XmlNode airdragNode = null;
            //	//if (airdrag is AbstractCommonComponentType)
            //	//    airdragNode = (airdrag as AbstractCommonComponentType).XMLSource;
            //	//else {
            //	//    var type = airdrag.GetType();
            //	//    var property = type.GetProperty(nameof(AbstractCommonComponentType.XMLSource));
            //	//    if (property != null)
            //	//        airdragNode = (XmlNode)property.GetValue(airdrag, null);
            //	//}

            //	//if (airdragNode == null)
            //	//    return null;

            //	//var dataNode = airdragNode.SelectSingleNode(".//*[local-name()='Data']");
            //	//var signatureNode = airdragNode.SelectSingleNode(".//*[local-name()='Signature']");

            //	//if (dataNode == null)
            //	//    throw new VectoException("Data node of airdrag component is null!");
            //	//if (signatureNode == null)
            //	//    throw new VectoException("Signature node of the given airdrag component is null!");

            //	//return dataNode.NamespaceURI == v10.NamespaceName
            //	//    ? GetAirdragXMLElementV1(dataNode, signatureNode)
            //	//    : GetAirdragXMLElementV2(dataNode, signatureNode);
        }

        protected XElement GetBusAirdragUseStandardValues()
		{
			var id = $"{VectoComponents.Airdrag.HashIdPrefix()}{XMLHelper.GetGUID()}";

			return new XElement(v24 + XMLNames.Component_AirDrag,
				new XElement(v20 + XMLNames.ComponentDataWrapper,
					new XAttribute(xsi + XMLNames.Attr_Type, "AirDragModifiedUseStandardValueType"),
					new XAttribute(XMLNames.Component_ID_Attr, id)
				),
				new XElement(v20 + XMLNames.DI_Signature, XMLHelper.CreateDummySig(di)));
		}
	}
}