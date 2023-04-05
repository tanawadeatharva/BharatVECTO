using System;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.Utils;
using VECTO3GUI2020.Helper;
using VECTO3GUI2020.ViewModel.Implementation.JobEdit.Vehicle.Components;
using VECTO3GUI2020.ViewModel.MultiStage.Implementation;

namespace VECTO3GUI2020.Util.XML.Components
{
    public abstract class XMLAirDragWriter : IXMLComponentWriter
    {
        protected IAirdragDeclarationInputData _inputData;
		protected abstract XNamespace DefaultNamespace { get; }
		protected XElement _xElement;
		protected string _uri = null;

        protected XMLAirDragWriter()
        {
           
            if (_uri == null) {
				_uri = "AirdragComponent" + XMLHelper.GetGUID();
			}
        }

        public XElement GetElement()
        {
            
            if (_xElement == null)
            {

                Initialize();
                CreateDataElements();
                var signatureElemnet = this.CreateSignatureElement(XMLNamespaces.V20, _uri, _inputData.DigestValue);
                _xElement.Add(signatureElemnet);
            }

            return _xElement;
        }

		public IXMLComponentWriter Init(IComponentInputData inputData)
		{
			_inputData = inputData as IAirdragDeclarationInputData;
			_uri = inputData.DigestValue?.Reference?.Replace("#", "");

			return this;
		}

		public XElement GetElement(XNamespace wrapperNamespace)
        {
            throw new NotImplementedException();
        }

        protected abstract void Initialize();
        protected abstract void CreateDataElements();


    }

    public class XMLAirDragWriter_v2_0 : XMLAirDragWriter
    {
		public XMLAirDragWriter_v2_0() : base() { }
        protected override void CreateDataElements()
        {
            var dataElement = new XElement(DefaultNamespace + XMLNames.ComponentDataWrapper);
            _xElement.Add(dataElement);

            dataElement.Add(new XAttribute(XMLNames.Component_ID_Attr, _uri), new XAttribute(XMLNamespaces.Xsi + XMLNames.Component_Type_Attr, XMLNames.AirDrag_Data_Type_Attr), new XAttribute("xmlns", DefaultNamespace));

            dataElement.Add(new XElement(DefaultNamespace + XMLNames.Component_Manufacturer, _inputData.Manufacturer));
            dataElement.Add(new XElement(DefaultNamespace + XMLNames.Component_Model, _inputData.Model));
            dataElement.Add(new XElement(DefaultNamespace + XMLNames.Component_CertificationNumber, _inputData.CertificationNumber));
            dataElement.Add(new XElement(DefaultNamespace + XMLNames.Component_Date, _inputData.Date));
            dataElement.Add(new XElement(DefaultNamespace + XMLNames.Component_AppVersion, _inputData.AppVersion));
            dataElement.Add(new XElement(DefaultNamespace + XMLNames.AirDrag_CdxA_0, _inputData.AirDragArea_0.ToXMLFormat(2)));
            dataElement.Add(new XElement(DefaultNamespace + XMLNames.AirDrag_TransferredCDxA, _inputData.TransferredAirDragArea.ToXMLFormat(2)));
            dataElement.Add(new XElement(DefaultNamespace + XMLNames.AirDrag_DeclaredCdxA, _inputData.AirDragArea.ToXMLFormat(2)));


            dataElement.DescendantsAndSelf().Where(e => string.IsNullOrEmpty(e.Value)).Remove();
        }

		protected override XNamespace DefaultNamespace => XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V20;

		protected override void Initialize()
        {
			_xElement = new XElement(XMLNames.Component_AirDrag);
        }
    }

	public class XMLAirDragWriter_v2_4 : XMLAirDragWriter
	{
		protected override XNamespace DefaultNamespace => XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V24;
        public XMLAirDragWriter_v2_4() : base() { }
		protected override void CreateDataElements()
		{
			var dataElement = new XElement(XMLNamespaces.V20 + XMLNames.ComponentDataWrapper);
			_xElement.Add(dataElement);

			dataElement.Add(new XAttribute(XMLNames.Component_ID_Attr, "std"), new XAttribute(XMLNamespaces.Xsi + XMLNames.Component_Type_Attr,
                "AirDragModifiedUseStandardValueType"), new XAttribute("xmlns", DefaultNamespace));

			//dataElement.Add(new XElement(DefaultNamespace + XMLNames.Component_Manufacturer, _inputData.Manufacturer));
			//dataElement.Add(new XElement(DefaultNamespace + XMLNames.Component_Model, _inputData.Model));
			//dataElement.Add(new XElement(DefaultNamespace + XMLNames.Component_CertificationNumber, _inputData.CertificationNumber));
			//dataElement.Add(new XElement(DefaultNamespace + XMLNames.Component_Date, _inputData.Date));
			//dataElement.Add(new XElement(DefaultNamespace + XMLNames.Component_AppVersion, _inputData.AppVersion));
			//dataElement.Add(new XElement(DefaultNamespace + XMLNames.AirDrag_CdxA_0, _inputData.AirDragArea_0.ToXMLFormat(2)));
			//dataElement.Add(new XElement(DefaultNamespace + XMLNames.AirDrag_TransferredCDxA, _inputData.TransferredAirDragArea.ToXMLFormat(2)));
			//dataElement.Add(new XElement(DefaultNamespace + XMLNames.AirDrag_DeclaredCdxA, _inputData.AirDragArea.ToXMLFormat(2)));


			//dataElement.DescendantsAndSelf().Where(e => string.IsNullOrEmpty(e.Value)).Remove();
		}

		protected override void Initialize()
		{
			_xElement = new XElement(XMLNames.Component_AirDrag);
		}
	}

	public class XMLAirDragWriter_v1_0 : XMLAirDragWriter
	{
		protected override XNamespace DefaultNamespace => XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V10;
        public XMLAirDragWriter_v1_0() : base() { }
		protected override void CreateDataElements()
		{
			var dataElement = new XElement(XMLNamespaces.V20 + XMLNames.ComponentDataWrapper);
			_xElement.Add(dataElement);

			dataElement.Add(new XAttribute(XMLNames.Component_ID_Attr, _uri), new XAttribute(XMLNamespaces.Xsi + XMLNames.Component_Type_Attr, XMLNames.AirDrag_Data_Type_Attr), new XAttribute("xmlns", DefaultNamespace));

			dataElement.Add(new XElement(DefaultNamespace + XMLNames.Component_Manufacturer, _inputData.Manufacturer));
			dataElement.Add(new XElement(DefaultNamespace + XMLNames.Component_Model, _inputData.Model));
			dataElement.Add(new XElement(DefaultNamespace + XMLNames.Component_CertificationNumber, _inputData.CertificationNumber));
			dataElement.Add(new XElement(DefaultNamespace + XMLNames.Component_Date, _inputData.Date));
			dataElement.Add(new XElement(DefaultNamespace + XMLNames.Component_AppVersion, _inputData.AppVersion));
			dataElement.Add(new XElement(DefaultNamespace + XMLNames.AirDrag_CdxA_0, _inputData.AirDragArea_0.ToXMLFormat(2)));
			dataElement.Add(new XElement(DefaultNamespace + XMLNames.AirDrag_TransferredCDxA, _inputData.TransferredAirDragArea.ToXMLFormat(2)));
			dataElement.Add(new XElement(DefaultNamespace + XMLNames.AirDrag_DeclaredCdxA, _inputData.AirDragArea.ToXMLFormat(2)));


			dataElement.DescendantsAndSelf().Where(e => string.IsNullOrEmpty(e.Value)).Remove();
		}

		protected override void Initialize()
		{
			_xElement = new XElement(XMLNames.Component_AirDrag);
		}
	}

}
