using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using NLog.LayoutRenderers.Wrappers;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportXMLTypeWriter.Components
{
	public interface IMrfAirdragType
	{
		XElement GetXmlType(IAirdragDeclarationInputData inputData);


	}
	public class MRFAirdragType : AbstractMrfXmlType, IMrfAirdragType
	{

		public MRFAirdragType(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Implementation of IMrfAirdragType

		public XElement GetXmlType(IAirdragDeclarationInputData inputData)
		{
			if (inputData == null || inputData.AirDragArea == null) {
				return null;
			}
			return new XElement(_mrf + XMLNames.Component_AirDrag,
				new XElement(_mrf + XMLNames.Component_Model, inputData.Model),
				new XElement(_mrf + XMLNames.Component_CertificationMethod, inputData.CertificationMethod),
				inputData.CertificationNumber.IsNullOrEmpty()
					? null
					: new XElement(_mrf + XMLNames.Component_CertificationNumber, inputData.CertificationNumber),
				new XElement(_mrf + "CdxA", inputData.AirDragArea.ToXMLFormat(2)),
				new XElement(_mrf + XMLNames.DI_Signature_Reference_DigestValue, inputData.DigestValue?.DigestValue ?? ""));
		}

		#endregion
	}


}
