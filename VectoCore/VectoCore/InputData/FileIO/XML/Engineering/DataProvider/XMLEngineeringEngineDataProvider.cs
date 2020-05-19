/*
* This file is part of VECTO.
*
* Copyright © 2012-2017 European Union
*
* Developed by Graz University of Technology,
*              Institute of Internal Combustion Engines and Thermodynamics,
*              Institute of Technical Informatics
*
* VECTO is licensed under the EUPL, Version 1.1 or - as soon they will be approved
* by the European Commission - subsequent versions of the EUPL (the "Licence");
* You may not use VECTO except in compliance with the Licence.
* You may obtain a copy of the Licence at:
*
* https://joinup.ec.europa.eu/community/eupl/og_page/eupl
*
* Unless required by applicable law or agreed to in writing, VECTO
* distributed under the Licence is distributed on an "AS IS" basis,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the Licence for the specific language governing permissions and
* limitations under the Licence.
*
* Authors:
*   Stefan Hausberger, hausberger@ivt.tugraz.at, IVT, Graz University of Technology
*   Christian Kreiner, christian.kreiner@tugraz.at, ITI, Graz University of Technology
*   Michael Krisper, michael.krisper@tugraz.at, ITI, Graz University of Technology
*   Raphael Luz, luz@ivt.tugraz.at, IVT, Graz University of Technology
*   Markus Quaritsch, markus.quaritsch@tugraz.at, IVT, Graz University of Technology
*   Martin Rexeis, rexeis@ivt.tugraz.at, IVT, Graz University of Technology
*/

using System;
using System.Collections.Generic;
using System.Linq;
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
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Engineering.DataProvider
{
	internal class XMLEngineeringEngineDataProviderV07 : AbstractEngineeringXMLComponentDataProvider,
		IXMLEngineData, IEngineModeEngineeringInputData, IEngineFuelEngineeringInputData
	{
		public static readonly XNamespace NAMESPACE_URI = XMLDefinitions.ENGINEERING_DEFINITONS_NAMESPACE_V07;

		public const string XSD_TYPE = "EngineDataEngineeringType";

		public static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI.NamespaceName, XSD_TYPE);

		protected IList<IEngineModeEngineeringInputData> _modes;

		public XMLEngineeringEngineDataProviderV07(
			IXMLEngineeringVehicleData vehicle,
			XmlNode vehicleNode, string fsBasePath)
			: base(vehicle, vehicleNode, fsBasePath)
		{
			SourceType = (vehicle as IXMLResource).DataSource.SourceFile == fsBasePath
				? DataSourceType.XMLEmbedded
				: DataSourceType.XMLFile;
		}


		public XMLEngineeringEngineDataProviderV07(XmlNode node, string sourceFile) : base(null, node, sourceFile) { }

		public virtual CubicMeter Displacement
		{
			get { return GetDouble(XMLNames.Engine_Displacement).SI(Unit.SI.Cubic.Centi.Meter).Cast<CubicMeter>(); }
		}

		public virtual PerSecond IdleSpeed
		{
			get { return GetDouble(XMLNames.Engine_IdlingSpeed).RPMtoRad(); }
		}

		public virtual double WHTCEngineering
		{
			get { return GetDouble(XMLNames.Engine_WHTCEngineering); }
		}

		IList<IEngineModeEngineeringInputData> IEngineEngineeringInputData.EngineModes
		{
			get { return _modes ?? (_modes = ReadEngineModes()); }
		}

		public virtual Second EngineStartTime
		{
			get { return null; }
		}

		public virtual double WHTCMotorway
		{
			get { throw new VectoException("Property not available in Engineering Mode"); }
		}

		public virtual double WHTCRural
		{
			get { throw new VectoException("Property not available in Engineering Mode"); }
		}

		public virtual double WHTCUrban
		{
			get { throw new VectoException("Property not available in Engineering Mode"); }
		}

		public virtual double ColdHotBalancingFactor
		{
			get { throw new VectoException("Property not available in Engineering Mode"); }
		}

		public virtual double CorrectionFactorRegPer
		{
			get {
				return 1;

				//GetDoubleElementValue(XMLNames.Engine_CorrectionFactor_RegPer); 
			}
		}

		public virtual double CorrectionFactorNCV
		{
			get { return 1; }
		}

		public virtual FuelType FuelType
		{
			get {
				return FuelType.DieselCI; //GetElementValue(XMLNames.Engine_FuelType).ParseEnum<FuelType>();
			}
		}

		public virtual TableData FuelConsumptionMap
		{
			get {
				return XMLHelper.ReadEntriesOrResource(
					BaseNode, DataSource.SourcePath, XMLNames.Engine_FuelConsumptionMap, XMLNames.Engine_FuelConsumptionMap_Entry,
					AttributeMappings.FuelConsumptionMapMapping);
			}
		}

		public virtual TableData FullLoadCurve
		{
			get {
				return XMLHelper.ReadEntriesOrResource(
					BaseNode, DataSource.SourcePath, XMLNames.Engine_FullLoadAndDragCurve, XMLNames.Engine_FullLoadCurve_Entry,
					AttributeMappings.EngineFullLoadCurveMapping);
			}
		}

		IList<IEngineFuelEngineeringInputData> IEngineModeEngineeringInputData.Fuels
		{
			get { return new[] { this }.Cast<IEngineFuelEngineeringInputData>().ToList(); }
		}

		public virtual IList<IEngineFuelDelcarationInputData> Fuels
		{
			get { return new[] { this }.Cast<IEngineFuelDelcarationInputData>().ToList(); }
		}

		public virtual IWHRData WasteHeatRecoveryDataElectrical
		{
			get { return null; }
		}


		public virtual IWHRData WasteHeatRecoveryDataMechanical
		{
			get { return null; }
		}


		public virtual Watt RatedPowerDeclared
		{
			get {
				return null; //GetDoubleElementValue(XMLNames.Engine_RatedPower).SI<Watt>(); 
			}
		}

		public virtual PerSecond RatedSpeedDeclared
		{
			get {
				return null; //GetDoubleElementValue(XMLNames.Engine_RatedSpeed).RPMtoRad(); 
			}
		}

		public virtual NewtonMeter MaxTorqueDeclared
		{
			get {
				return null; //GetDoubleElementValue(XMLNames.Engine_MaxTorque).SI<NewtonMeter>(); 
			}
		}

		public virtual IList<IEngineModeDeclarationInputData> EngineModes
		{
			get { return (_modes ?? (_modes = ReadEngineModes())).Cast<IEngineModeDeclarationInputData>().ToList(); }
		}

		protected virtual IList<IEngineModeEngineeringInputData> ReadEngineModes()
		{
			return new IEngineModeEngineeringInputData[] { this };
		}

		public virtual WHRType WHRType
		{
			get { return WHRType.None; }
		}

		public virtual KilogramSquareMeter Inertia
		{
			get { return GetString(XMLNames.Engine_Inertia, required: false)?.ToDouble().SI<KilogramSquareMeter>(); }
		}

		#region Overrides of AbstractXMLResource

		protected override XNamespace SchemaNamespace
		{
			get { return NAMESPACE_URI; }
		}

		protected override DataSourceType SourceType { get; }

		#endregion
	}

	internal class XMLEngineeringEngineDataProviderV10 : XMLEngineeringEngineDataProviderV07
	{
		public new const string NAMESPACE_URI = XMLDefinitions.ENGINEERING_DEFINITONS_NAMESPACE_V10;

		//public new const string XSD_TYPE = "EngineDataEngineeringType";

		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI, XSD_TYPE);

		public XMLEngineeringEngineDataProviderV10(
			XMLEngineeringVehicleDataProviderV07 vehicle, XmlNode vehicleNode, string fsBasePath) : base(
			vehicle, vehicleNode, fsBasePath) { }

		// engine-only constructor
		public XMLEngineeringEngineDataProviderV10(XmlNode node, string sourceFile) : base(node, sourceFile) { }

		public override double WHTCEngineering
		{
			get { return GetDouble(XMLNames.Engine_FCCorrection, 1.0); }
		}

		#region Overrides of XMLEngineeringEngineDataProviderV07

		protected override XNamespace SchemaNamespace
		{
			get { return NAMESPACE_URI; }
		}

		#endregion
	}

	// this class is just for testing reading derived XML datatypes
	internal class XMLEngineeringEngineDataProviderV10TEST : XMLEngineeringEngineDataProviderV10
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.ENGINEERING_DEFINITONS_NAMESPACE_V10_TEST;

		public new const string XSD_TYPE = "EngineDataEngineeringType";

		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI, XSD_TYPE);

		public XMLEngineeringEngineDataProviderV10TEST(
			XMLEngineeringVehicleDataProviderV07 vehicle, XmlNode vehicleNode, string fsBasePath) : base(
			vehicle, vehicleNode, fsBasePath) { }

		// engine-only constructor
		public XMLEngineeringEngineDataProviderV10TEST(XmlNode node, string sourceFile) : base(node, sourceFile) { }

		#region Overrides of XMLEngineeringEngineDataProviderV07

		public override PerSecond RatedSpeedDeclared
		{
			get { return GetString(XMLNames.Engine_RatedSpeed).Replace("rpm", "").ToDouble(0).RPMtoRad(); }
		}

		protected override XNamespace SchemaNamespace
		{
			get { return NAMESPACE_URI; }
		}

		#endregion
	}

	internal class XMLEngineeringEngineDataProviderV11 : XMLEngineeringEngineDataProviderV10
	{
		public new static readonly XNamespace NAMESPACE_URI = XMLDefinitions.ENGINEERING_DEFINITONS_NAMESPACE_V11;

		public new const string XSD_TYPE = "EngineDataEngineeringType";

		public new static readonly string QUALIFIED_XSD_TYPE = XMLHelper.CombineNamespace(NAMESPACE_URI, XSD_TYPE);

		
		public XMLEngineeringEngineDataProviderV11(
			XMLEngineeringVehicleDataProviderV07 vehicle, XmlNode vehicleNode, string fsBasePath) : base(
			vehicle, vehicleNode, fsBasePath) { }

		public XMLEngineeringEngineDataProviderV11(XmlNode node, string sourceFile) : base(node, sourceFile) { }

		#region Overrides of XMLEngineeringEngineDataProviderV07

		protected override IList<IEngineModeEngineeringInputData> ReadEngineModes()
		{
			return GetNodes(XMLNames.Engine_FuelModes)
							.Cast<XmlNode>().Select(x => new XMLDualFuelEngineMode(x, DataSource)).Cast<IEngineModeEngineeringInputData>().ToList();
			
		}

		public override WHRType WHRType
		{
			get {
				var retVal = WHRType.None;
				if (XmlConvert.ToBoolean(GetString("MechanicalOutputICE"))) {
					retVal |= WHRType.MechanicalOutputICE;
				}
				if (XmlConvert.ToBoolean(GetString("MechanicalOutputDrivetrain"))) {
					retVal |= WHRType.MechanicalOutputDrivetrain;
				}
				if (XmlConvert.ToBoolean(GetString("ElectricalOutput"))) {
					retVal |= WHRType.ElectricalOutput;
				}

				return retVal;
			}
		}

		#endregion


		internal class XMLDualFuelEngineMode : AbstractXMLType, IEngineModeEngineeringInputData
		{
			protected IList<IEngineFuelEngineeringInputData> _fuels;
			protected DataSource Source;
			protected IWHRData WHRData;

			public XMLDualFuelEngineMode(XmlNode xmlNode, DataSource source) : base(xmlNode)
			{
				Source = source;
			}

			#region Implementation of IEngineModeDeclarationInputData

			public PerSecond IdleSpeed {
				get { return GetDouble(XMLNames.Engine_IdlingSpeed).RPMtoRad(); }
			}
			public TableData FullLoadCurve {
				get {
					return XMLHelper.ReadEntriesOrResource(
						BaseNode, Source.SourcePath, XMLNames.Engine_FullLoadAndDragCurve, XMLNames.Engine_FullLoadCurve_Entry,
						AttributeMappings.EngineFullLoadCurveMapping);
				}
			}

			IList<IEngineFuelEngineeringInputData> IEngineModeEngineeringInputData.Fuels
			{
				get { return _fuels ?? (_fuels = ReadFuels()); }
			}

			

			IList<IEngineFuelDelcarationInputData> IEngineModeDeclarationInputData.Fuels
			{
				get { return (_fuels ?? (_fuels = ReadFuels())).Cast<IEngineFuelDelcarationInputData>().ToList(); }
			}

			public virtual IWHRData WasteHeatRecoveryDataElectrical
			{
				get { return WHRData ?? (WHRData = ReadWHRData("Electrical")); }
			}

			public virtual IWHRData WasteHeatRecoveryDataMechanical
			{
				get { return WHRData ?? (WHRData = ReadWHRData("Mechanical")); }
			}

			#endregion

			protected virtual IList<IEngineFuelEngineeringInputData> ReadFuels()
			{
				return GetNodes(XMLNames.Engine_FuelModes_Fuel).Cast<XmlNode>().Select(x => new XMLEngineFuel(x, Source)).Cast<IEngineFuelEngineeringInputData>().ToList();
			}

			protected virtual IWHRData ReadWHRData(string typeNode)
			{
				var fuelNode = GetNodes(XMLNames.Engine_FuelModes_Fuel).Cast<XmlNode>().First();
				var whrNode = GetNode(new [] { "WasteHeatRecovery", typeNode}, fuelNode);
				return new XMLEngineeringWHRData(whrNode, Source);
			}
		}

		internal class XMLEngineFuel : AbstractXMLType, IEngineFuelEngineeringInputData
		{
			protected DataSource Source;

			public XMLEngineFuel(XmlNode xmlNode, DataSource source) : base(xmlNode)
			{
				Source = source;
			}

			#region Implementation of IEngineFuelDelcarationInputData

			public virtual FuelType FuelType
			{
				get {
					var value = GetString(XMLNames.Engine_FuelType);
					if ("LPG".Equals(value, StringComparison.InvariantCultureIgnoreCase)) {
						return FuelType.LPGPI;
					}
					if ("NG".Equals(value, StringComparison.InvariantCultureIgnoreCase)) {
						return FuelType.NGPI;
					}

					return value.ParseEnum<FuelType>();
				}
			}

			public virtual double WHTCMotorway { get { return 1; } }

			public virtual double WHTCRural { get { return 1; } }

			public virtual double WHTCUrban { get { return 1; } }

			public virtual double ColdHotBalancingFactor { get { return 1; } }

			public virtual double CorrectionFactorRegPer { get { return 1; } }

			public virtual TableData FuelConsumptionMap {
				get {
					return XMLHelper.ReadEntriesOrResource(
						BaseNode, Source.SourcePath, XMLNames.Engine_FuelConsumptionMap, XMLNames.Engine_FuelConsumptionMap_Entry,
						AttributeMappings.FuelConsumptionMapMapping);
				}
			}

			#endregion

			#region Implementation of IEngineFuelEngineeringInputData

			public double WHTCEngineering { get { return GetDouble(XMLNames.Engine_FCCorrection); } }

			#endregion
		}

		internal class XMLEngineeringWHRData : AbstractXMLType, IWHRData
		{
			protected DataSource Source;

			public XMLEngineeringWHRData(XmlNode baseNode, DataSource source) : base (baseNode)
			{
				Source = source;
			}

			
			#region Implementation of IWHRData

			public virtual double UrbanCorrectionFactor { get { return 1; } }
			public virtual double RuralCorrectionFactor { get { return 1; } }
			public virtual double MotorwayCorrectionFactor { get { return 1; } }
			public virtual double BFColdHot { get { return 1; } }
			public virtual double CFRegPer { get { return 1; } }
			public virtual double EngineeringCorrectionFactor { get { return GetDouble(XMLNames.Engine_WHRCorrectionFactor); } }

			public virtual TableData GeneratedPower
			{
				get {
					return XMLHelper.ReadEntriesOrResource(
						BaseNode, Source.SourcePath, XMLNames.Engine_WHRMap, XMLNames.Engine_WHRMap_Entry,
						AttributeMappings.WHRPowerMapMapping);
				}
			}

			#endregion
		}


		#region Overrides of XMLEngineeringEngineDataProviderV07

		protected override XNamespace SchemaNamespace
		{
			get { return NAMESPACE_URI; }
		}

		#endregion
	}
}
