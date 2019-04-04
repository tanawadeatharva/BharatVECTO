using System.Collections.Generic;
using System.Xml;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider
{
	public class XMLAuxiliaryDeclarationDataProviderV10 : AbstractXMLType, IXMLAuxiliaryDeclarationInputData
	{
		public const string NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V10;

		protected AuxiliaryType? _type;
		private IList<string> _technology;

		public XMLAuxiliaryDeclarationDataProviderV10(XmlNode auxNode, IXMLDeclarationVehicleData vehicle) : base(auxNode) { }

		#region Implementation of IAuxiliaryDeclarationInputData

		public virtual AuxiliaryType Type
		{
			get {
				return _type ?? (_type = BaseNode.LocalName.ParseEnum<AuxiliaryType>()).Value;
			}
		}

		public virtual IList<string> Technology
		{
			get {
				if (_technology != null) {
					return _technology;
				}

				_technology = new List<string>();
				var techNodes = GetNodes(XMLNames.Auxiliaries_Auxiliary_Technology);
				foreach (XmlNode techNode in techNodes) {
					_technology.Add(techNode.InnerText);
				}

				return _technology;
			}
		}

		#endregion
	}
}
