using System.Xml;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider
{
	public class XMLDeclarationTyreDataProviderV10 : AbstractCommonComponentType, IXMLTyreDeclarationInputData
	{
		public const string NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V10;

		protected string _dimension;
		protected double? _rrc;
		protected Newton _fzIso;

		public XMLDeclarationTyreDataProviderV10(
			IXMLDeclarationVehicleData vehicle, XmlNode componentNode, string sourceFile) :
			base(componentNode, sourceFile)
		{
			SourceType = DataSourceType.XMLFile;
		}

		#region Overrides of AbstractXMLResource

		protected override string SchemaNamespace
		{
			get { return NAMESPACE_URI; }
		}

		protected override DataSourceType SourceType { get; }

		#endregion

		#region Implementation of ITyreDeclarationInputData

		public virtual string Dimension
		{
			get { return _dimension ?? (_dimension = GetString(XMLNames.AxleWheels_Axles_Axle_Dimension)); }
		}

		public virtual double RollResistanceCoefficient
		{
			get { return _rrc ?? (_rrc = GetDouble(XMLNames.AxleWheels_Axles_Axle_RRCDeclared)).Value; }
		}

		public virtual Newton TyreTestLoad
		{
			get { return _fzIso ?? (_fzIso = GetDouble(XMLNames.AxleWheels_Axles_Axle_FzISO).SI<Newton>()); }
		}

		#endregion
	}

	// ---------------------------------------------------------------------------------------

	public class XMLDeclarationTyreDataProviderV20 : XMLDeclarationTyreDataProviderV10
	{
		public new const string NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V20;

		public XMLDeclarationTyreDataProviderV20(IXMLDeclarationVehicleData vehicle, XmlNode componentNode, string sourceFile)
			: base(vehicle, componentNode, sourceFile) { }

		protected override string SchemaNamespace
		{
			get { return NAMESPACE_URI; }
		}
	}

	// ---------------------------------------------------------------------------------------

	public class XMLDeclarationTyreDataProviderV22 : XMLDeclarationTyreDataProviderV10
	{
		public new const string NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V22;

		public XMLDeclarationTyreDataProviderV22(IXMLDeclarationVehicleData vehicle, XmlNode componentNode, string sourceFile)
			: base(vehicle, componentNode, sourceFile) { }

		protected override string SchemaNamespace
		{
			get { return NAMESPACE_URI; }
		}
	}
}
