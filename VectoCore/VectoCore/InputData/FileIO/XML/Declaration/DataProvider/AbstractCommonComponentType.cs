using System.Collections.Generic;
using System.Xml;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider {
	public abstract class AbstractCommonComponentType : AbstractXMLResource
	{
		protected AbstractCommonComponentType(XmlNode node, string source) : base(node, source) { }

		public bool SavedInDeclarationMode
		{
			get { return true; }
		}

		public string Manufacturer
		{
			get { return GetString(XMLNames.Component_Manufacturer); }
		}

		public string Model
		{
			get { return GetString(XMLNames.Component_Model); }
		}

		public string Date
		{
			get { return GetString(XMLNames.Component_Date); }
		}

		public virtual CertificationMethod CertificationMethod
		{
			get {
				var certMethod = GetString(XMLNames.Component_Gearbox_CertificationMethod, required:false) ?? GetString(XMLNames.Component_CertificationMethod, required:false);
				return certMethod != null ? EnumHelper.ParseEnum<CertificationMethod>(certMethod) : CertificationMethod.Measured;
			}
		}

		protected TableData ReadTableData(string baseElement, string entryElement, Dictionary<string, string> mapping)
		{
			var entries = BaseNode.SelectNodes(
				XMLHelper.QueryLocalName(baseElement, entryElement));
			if (entries != null && entries.Count > 0) {
				return XMLHelper.ReadTableData(mapping, entries);
			}

			return null;
		}

		public virtual string CertificationNumber
		{
			get { return GetString(XMLNames.Component_CertificationNumber); }
		}

		public virtual DigestData DigestValue
		{
			get { return new DigestData(GetNode(XMLNames.DI_Signature, required:false)); }
		}
	}
}