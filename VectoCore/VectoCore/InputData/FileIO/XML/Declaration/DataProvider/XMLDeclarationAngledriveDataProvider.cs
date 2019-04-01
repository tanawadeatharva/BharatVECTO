using System.Collections.Generic;
using System.Xml;
using TUGraz.IVT.VectoXML;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider
{
	public class XMLDeclarationAngledriveDataProviderV10 : AbstractCommonComponentType, IXMLAngledriveInputData
	{
		public const string NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V10;

		protected IXMLDeclarationVehicleData Vehicle;


		public XMLDeclarationAngledriveDataProviderV10(IXMLDeclarationVehicleData vehicle, XmlNode componentNode, string sourceFile) :
			base(componentNode, sourceFile)
		{
			SourceType = DataSourceType.XMLFile;
			Vehicle = vehicle;
		}

		#region Implementation of IAngledriveInputData

		public AngledriveType Type
		{
			get { return Vehicle.AngledriveType; }
		}

		public double Ratio
		{
			get { return GetDouble(XMLNames.AngleDrive_Ratio); }
		}

		public TableData LossMap
		{
			get {
				return ReadTableData(
					XMLNames.AngleDrive_TorqueLossMap, XMLNames.Angledrive_LossMap_Entry,
					AttributeMappings.TransmissionLossmapMapping);
			}
		}

		public double Efficiency
		{
			get { throw new VectoException("Efficiency not supported in Declaration Mode!"); }
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
