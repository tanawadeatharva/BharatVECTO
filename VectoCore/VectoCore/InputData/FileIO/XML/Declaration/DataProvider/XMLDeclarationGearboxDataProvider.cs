using System;
using System.Collections.Generic;
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
	public class XMLDeclarationGearboxDataProviderV10 : AbstractCommonComponentType, IXMLGearboxDeclarationInputData
	{
		public const string NAMESPACE_URI = XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V10;

		protected ITorqueConverterDeclarationInputData _torqueConverter;
		private IList<ITransmissionInputData> _gears;

		public XMLDeclarationGearboxDataProviderV10(
			IXMLDeclarationVehicleData vehicle, XmlNode componentNode, string sourceFile) :
			base(componentNode, sourceFile) { }

		#region Overrides of AbstractXMLResource

		protected override string SchemaNamespace
		{
			get { return NAMESPACE_URI; }
		}

		protected override DataSourceType SourceType
		{
			get { return DataSourceType.XMLFile; }
		}

		#endregion

		#region Implementation of IGearboxDeclarationInputData

		public GearboxType Type
		{
			get {
				var value = GetString(XMLNames.Gearbox_TransmissionType);
				switch (value) {
					case "MT":
					case "SMT":
						return GearboxType.MT;
					case "AMT":
						return GearboxType.AMT;
					case "APT-S":
					case "AT - Serial":
						return GearboxType.ATSerial;
					case "APT-P":
					case "AT - PowerSplit":
						return GearboxType.ATPowerSplit;
				}
				throw new ArgumentOutOfRangeException("GearboxType", value);
			}
		}

		public IList<ITransmissionInputData> Gears
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

		public ITorqueConverterDeclarationInputData TorqueConverter { get {
			return _torqueConverter ?? (_torqueConverter = Reader.TorqueConverterInputData);
		} }

		#endregion

		#region Implementation of IXMLGearboxDeclarationInputData

		public IXMLComponentReader Reader { protected get; set; }

		#endregion
	}
}
