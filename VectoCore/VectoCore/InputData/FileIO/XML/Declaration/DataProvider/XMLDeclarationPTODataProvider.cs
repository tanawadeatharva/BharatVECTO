using System;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.InputData.FileIO.XML.Common;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider
{
	public class XMLDeclarationPTODataProviderV10 : AbstractXMLType, IXMLPTOTransmissionInputData
	{
		public static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V10;

		public const string XSD_TYPE = "PTOType";

		public static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		public XMLDeclarationPTODataProviderV10(
			IXMLDeclarationVehicleData vehicle, XmlNode componentNode, string sourceFile) : base(componentNode) { }


		#region Implementation of IPTOTransmissionInputData

		public virtual string PTOTransmissionType
		{
			get {
				var shaftGearWheels = GetString(XMLNames.Vehicle_PTO_ShaftsGearWheels);
				if ("none".Equals(shaftGearWheels, StringComparison.InvariantCultureIgnoreCase)) {
					return "None";
				}
				if ("only one engaged gearwheel above oil level".Equals(
					shaftGearWheels, StringComparison.CurrentCultureIgnoreCase)) {
					return "only one engaged gearwheel above oil level";
				}

				var otherElements = GetString(XMLNames.Vehicle_PTO_OtherElements);
				var ptoTech = string.Format("{0} - {1}", shaftGearWheels, otherElements);
				if (DeclarationData.PTOTransmission.GetTechnologies().Contains(ptoTech)) {
					return ptoTech;
				}

				throw new VectoException("PTO Technology {0} invalid!", ptoTech);
			}
		}

		public virtual TableData PTOLossMap
		{
			get { return null; }
		}

		public virtual TableData PTOCycle
		{
			get { return null; }
		}

		#endregion
	}

	// ---------------------------------------------------------------------------------------

	public class XMLDeclarationPTODataProviderV20 : XMLDeclarationPTODataProviderV10
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V20;

		public new const string XSD_TYPE = "PTOType";

		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		
		public XMLDeclarationPTODataProviderV20(IXMLDeclarationVehicleData vehicle, XmlNode componentNode, string sourceFile)
			: base(vehicle, componentNode, sourceFile) { }
	}
}
