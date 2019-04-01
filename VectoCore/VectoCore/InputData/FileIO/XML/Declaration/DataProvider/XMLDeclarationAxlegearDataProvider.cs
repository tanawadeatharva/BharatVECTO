using System.Xml;
using TUGraz.IVT.VectoXML;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider
{
	public class XMLDeclarationAxlegearDataProviderV10 : AbstractCommonComponentType, IXMLAxleGearInputData
	{
		public const string NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V10;

		public XMLDeclarationAxlegearDataProviderV10(IXMLDeclarationVehicleData vehicle, XmlNode componentNode, string sourceFile) :
			base(componentNode, sourceFile)
		{
			SourceType = DataSourceType.XMLFile;
		}

		#region Implementation of IAxleGearInputData

		public double Ratio
		{
			get { return GetDouble(XMLNames.Axlegear_Ratio); }
		}

		public TableData LossMap
		{
			get {
				return ReadTableData(
					XMLNames.Axlegear_TorqueLossMap, XMLNames.Axlegear_TorqueLossMap_Entry,
					AttributeMappings.TransmissionLossmapMapping);
			}
		}

		public double Efficiency
		{
			get { throw new VectoException("Efficiency not supported in Declaration Mode!"); }
		}

		public AxleLineType LineType
		{
			get {
				var value = GetString(XMLNames.Axlegear_LineType);
				return value.ParseEnum<AxleLineType>();
			}
		}
		#endregion

		#region Overrides of AbstractXMLResource

		protected override string SchemaNamespace
		{
			get { return NAMESPACE_URI; }
		}

		protected override DataSourceType SourceType { get; }

		#endregion
	}
}
