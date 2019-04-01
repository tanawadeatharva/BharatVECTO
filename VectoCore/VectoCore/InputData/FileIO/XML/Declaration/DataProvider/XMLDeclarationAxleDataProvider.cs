using System.Xml;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Reader;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider
{
	public class XMLDeclarationAxleDataProviderV10 : AbstractCommonComponentType, IXMLAxleDeclarationInputData
	{
		public const string NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V10;

		private ITyreDeclarationInputData _tyre;
		private bool? _twinTyre;
		private AxleType? _axleType;

		public XMLDeclarationAxleDataProviderV10(IXMLDeclarationVehicleData vehicle, XmlNode componentNode, string sourceFile) : base(componentNode, sourceFile)
		{
			SourceType = DataSourceType.XMLFile;
		}

		#region Implementation of IAxleDeclarationInputData

		public bool TwinTyres
		{
			get {
				return _twinTyre ?? (_twinTyre = XmlConvert.ToBoolean(GetString(XMLNames.AxleWheels_Axles_Axle_TwinTyres))).Value;
			}
		}

		public AxleType AxleType
		{
			get {
				return _axleType ?? (_axleType = GetString(XMLNames.AxleWheels_Axles_Axle_AxleType).ParseEnum<AxleType>()).Value;
			}
		}

		public ITyreDeclarationInputData Tyre
		{
			get { return _tyre ?? (_tyre = Reader.Tyre); }
		}

		#endregion


		#region Implementation of IXMLAxleDeclarationInputData

		public IXMLComponentReader Reader { protected get; set; }

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
