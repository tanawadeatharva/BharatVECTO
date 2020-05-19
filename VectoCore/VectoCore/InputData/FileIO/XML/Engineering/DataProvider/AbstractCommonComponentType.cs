using System;
using System.Xml;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.Configuration;
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

		public DateTime Date
		{
			get { return XmlConvert.ToDateTime(GetString(XMLNames.Component_Date), XmlDateTimeSerializationMode.Utc); }
		}

		public virtual string AppVersion
		{
			get { return GetString(XMLNames.Component_AppVersion); }
		}

		public virtual CertificationMethod CertificationMethod
		{
			get { return CertificationMethod.NotCertified; }
		}

		public virtual string CertificationNumber
		{
			get { return Constants.NOT_AVailABLE; }
		}

		public virtual DigestData DigestValue
		{
			get { return null; }
		}

		public virtual XmlNode XMLSource { get { return BaseNode; } }
	}
}
