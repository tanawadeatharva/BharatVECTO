using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Reader;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider
{
	public class XMLDeclarationGearboxDataProviderV10 : AbstractCommonComponentType, IXMLGearboxDeclarationInputData
	{
		public static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V10;

		public const string XSD_TYPE = "GearboxDataDeclarationType";

		public static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);


		protected ITorqueConverterDeclarationInputData _torqueConverter;
		protected IList<ITransmissionInputData> _gears;
		protected IXMLDeclarationVehicleData _vehicle;

		public XMLDeclarationGearboxDataProviderV10(
			IXMLDeclarationVehicleData vehicle, XmlNode componentNode, string sourceFile) :
			base(componentNode, sourceFile)
		{
			SourceType = DataSourceType.XMLEmbedded;
			_vehicle = vehicle;
		}


		#region Overrides of AbstractXMLResource

		protected override XNamespace SchemaNamespace
		{
			get { return NAMESPACE_URI; }
		}

		protected override DataSourceType SourceType { get; }

		#endregion

		#region Implementation of IGearboxDeclarationInputData

		public virtual GearboxType Type
		{
			get {
				var value = GetString(XMLNames.Gearbox_TransmissionType);
				switch (value) {
					case "MT":
					case "SMT": return GearboxType.MT;
					case "AMT": return GearboxType.AMT;
					case "APT-S":
					case "AT - Serial": return GearboxType.ATSerial;
					case "APT-P":
					case "AT - PowerSplit": return GearboxType.ATPowerSplit;
				}

				throw new ArgumentOutOfRangeException("GearboxType", value);
			}
		}

		public virtual IList<ITransmissionInputData> Gears
		{
			get {
				if (_gears != null) {
					return _gears;
				}

				_gears = new List<ITransmissionInputData>();

				var gearNodes = GetNodes(new[] { XMLNames.Gearbox_Gears, XMLNames.Gearbox_Gears_Gear });
				if (gearNodes != null) {
					foreach (XmlNode gearNode in gearNodes) {
						_gears.Add(Reader.CreateGear(gearNode));
					}
				}

				return _gears;
			}
		}

		public virtual bool DifferentialIncluded { get { return false; } }

		public virtual double AxlegearRatio { get { return double.NaN; } }

		//public virtual ITorqueConverterDeclarationInputData TorqueConverter
		//{
		//	get { return _vehicle.Components.TorqueConverterInputData; }
		//}

		#endregion

		#region Implementation of IXMLGearboxDeclarationInputData

		public IXMLGearboxReader Reader { protected get; set; }

		#endregion
	}

	// ---------------------------------------------------------------------------------------

	public class XMLDeclarationGearboxDataProviderV20 : XMLDeclarationGearboxDataProviderV10
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V20;

		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		public XMLDeclarationGearboxDataProviderV20(
			IXMLDeclarationVehicleData vehicle, XmlNode componentNode, string sourceFile) : base(
			vehicle, componentNode, sourceFile) { }

		protected override XNamespace SchemaNamespace
		{
			get { return NAMESPACE_URI; }
		}
	}

	// ---------------------------------------------------------------------------------------

	public class XMLDeclarationGearboxDataProviderV26 : XMLDeclarationGearboxDataProviderV10
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V26;

		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		public XMLDeclarationGearboxDataProviderV26(
			IXMLDeclarationVehicleData vehicle, XmlNode componentNode, string sourceFile) : base(
			vehicle, componentNode, sourceFile)
		{ }

		#region Overrides of XMLDeclarationGearboxDataProviderV10

		public override bool DifferentialIncluded { get { return GetBool(XMLNames.Gearbox_DifferentialIncluded); } }

		public override double AxlegearRatio { get { return DifferentialIncluded ? GetDouble(XMLNames.Gearbox_AxlegearRatio) : double.NaN; } }

		#endregion
		
		protected override XNamespace SchemaNamespace
		{
			get { return NAMESPACE_URI; }
		}
	}

	// ---------------------------------------------------------------------------------------

	public class XMLDeclarationPrimaryVehicleBusGearboxDataProviderV01 : XMLDeclarationGearboxDataProviderV10
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.DECLARATION_PRIMARY_BUS_VEHICLE_URI_V01;

		public new const string XSD_TYPE = "TransmissionDataPIFType";

		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		public XMLDeclarationPrimaryVehicleBusGearboxDataProviderV01(IXMLDeclarationVehicleData vehicle, XmlNode componentNode, string sourceFile) 
			: base(vehicle, componentNode, sourceFile) { }

		protected override XNamespace SchemaNamespace
		{
			get { return NAMESPACE_URI; }
		}
	}
}
