using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using TUGraz.IVT.VectoXML;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.XML.Common;
using TUGraz.VectoCore.InputData.FileIO.XML.Engineering.Interfaces;
using TUGraz.VectoCore.InputData.Impl;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Engineering.DataProvider
{
	internal class XMLEngineeringAuxiliariesDataProviderV07 : AbstractXMLType, IXMLAuxiliairesData
	{
		public static readonly XNamespace NAMESPACE_URI = XMLDefinitions.ENGINEERING_DEFINITONS_NAMESPACE_V07;

		public const string XSD_TYPE = "AuxiliariesDataEngineeringType";

		public static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI, XSD_TYPE);


		public XMLEngineeringAuxiliariesDataProviderV07(
			IXMLEngineeringVehicleData vehicle, XmlNode componentNode, string sourceFile) : base(componentNode) { }

		public IXMLAuxiliaryReader Reader { protected get; set; }

		#region Implementation of IAuxiliariesEngineeringInputData

		public virtual IAuxiliaryEngineeringInputData Auxiliaries
		{
			get;
		}

		public IBusAuxiliariesEngineeringData BusAuxiliariesData
		{
			get
			{
				// TODO: MQ 20210211 - implement...
				return null;
			}
		}

		public Watt ElectricAuxPower { get; }

		#endregion
	}

	internal class XMLEngineeringAuxiliariesDataProviderV10 : XMLEngineeringAuxiliariesDataProviderV07
	{
		public new const string NAMESPACE_URI = XMLDefinitions.ENGINEERING_DEFINITONS_NAMESPACE_V10;

		public new const string XSD_TYPE = "AuxiliariesDataEngineeringType";

		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI, XSD_TYPE);


		public XMLEngineeringAuxiliariesDataProviderV10(
			IXMLEngineeringVehicleData vehicle, XmlNode componentNode, string sourceFile) : base(
			vehicle, componentNode, sourceFile) { }
	}

	internal class XMLAuxiliaryEngineeringDataV07 : AbstractXMLType, IXMLAuxiliaryData
	{
		public const string NAMESPACE_URI = XMLDefinitions.ENGINEERING_DEFINITONS_NAMESPACE_V07;

		public const string XSD_TYPE = "AuxiliaryEntryEngineeringType";

		public static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI, XSD_TYPE);



		protected string BasePath;

		public XMLAuxiliaryEngineeringDataV07(XmlNode node, string basePath) : base(node)
		{
			BasePath = basePath;

		}

		protected virtual XNamespace SchemaNamespace {  get { return NAMESPACE_URI; } }

		#region Implementation of IAuxiliaryEngineeringInputData


		public virtual Watt ConstantPowerDemand
		{
			get;
		}

		public Watt PowerDemandICEOffDriving { get; }
		public Watt PowerDemandICEOffStandstill { get; }
		public Watt ElectricPowerDemand { get; }


		#endregion
	}

	internal class XMLAuxiliaryEngineeringDataV10 : XMLAuxiliaryEngineeringDataV07
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.ENGINEERING_DEFINITONS_NAMESPACE_V10;

		public new const string XSD_TYPE = "AuxiliaryEntryEngineeringType";

		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI, XSD_TYPE);


		public XMLAuxiliaryEngineeringDataV10(XmlNode node, string basePath) : base(node, basePath) { }

		protected override XNamespace SchemaNamespace { get { return NAMESPACE_URI; } }
	}
}
