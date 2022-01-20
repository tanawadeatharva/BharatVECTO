using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Castle.Core.Internal;
using NLog.LayoutRenderers.Wrappers;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportXMLTypeWriter.Components
{
    public class MRFAirdragType : AbstractMrfXmlType, IMrfXmlType
	{
		#region Implementation of IMrfXmlType

		public XElement GetXmlType(IDeclarationInputDataProvider inputData)
		{
			
			var airdragData = inputData.JobInputData.Vehicle.Components.AirdragInputData;
			return new XElement(_mrf + XMLNames.Component_AirDrag,
				new XElement(_mrf + XMLNames.Component_Model, airdragData.Model),
				new XElement(_mrf + XMLNames.Component_CertificationMethod, airdragData.CertificationMethod),
				airdragData.CertificationNumber.IsNullOrEmpty()
					? null
					: new XElement(_mrf + XMLNames.Component_CertificationNumber, airdragData.CertificationNumber),
				new XElement(_mrf + "CdxA", airdragData.AirDragArea.ToXMLFormat(2)),
				new XElement(_mrf + XMLNames.DI_Signature_Reference_DigestValue, airdragData.DigestValue.DigestValue));
		}

		#endregion

		public MRFAirdragType(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }
	}
}
