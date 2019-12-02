using System.Xml;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.InputData.FileIO.XML.Common;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Engineering.DataProvider
{
	
	public abstract class AbstractCommonComponentType : AbstractXMLResource
	{
		public AbstractCommonComponentType(XmlNode node, string source) : base(node, source) { }

		
		public bool SavedInDeclarationMode
		{
			get { return false; }
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
			get { return CertificationMethod.NotCertified; }
		}

		public virtual string CertificationNumber
		{
			get { return "N.A."; }
		}

		public virtual DigestData DigestValue
		{
			get { return null; }
		}
	}
}
