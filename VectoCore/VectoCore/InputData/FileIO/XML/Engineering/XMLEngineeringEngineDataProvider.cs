using System.Collections.Generic;
using System.Xml.XPath;
using TUGraz.IVT.VectoXML;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader;
using TUGraz.VectoCore.Resources;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Engineering
{
	public class XMLEngineeringEngineDataProvider : AbstractEngineeringXMLComponentDataProvider,
		IEngineEngineeringInputData
	{
		public XMLEngineeringEngineDataProvider(XMLEngineeringInputDataProvider xmlEngineeringJobInputDataProvider,
			XPathDocument engineDocument, string xmlBasePath, string fsBasePath)
			: base(xmlEngineeringJobInputDataProvider, engineDocument, xmlBasePath, fsBasePath) {}

		public CubicMeter Displacement
		{
			get { return GetDoubleElementValue(XMLNames.Engine_Displacement).SI().Cubic.Centi.Meter.Cast<CubicMeter>(); }
		}

		public PerSecond IdleSpeed
		{
			get { return GetDoubleElementValue(XMLNames.Engine_IdlingSpeed).RPMtoRad(); }
		}

		public double WHTCEngineering
		{
			get { return GetDoubleElementValue(XMLNames.Engine_WHTCEngineering); }
		}

		public double WHTCMotorway
		{
			get { throw new VectoException("Property not available in Engineering Mode"); }
		}

		public double WHTCRural
		{
			get { throw new VectoException("Property not available in Engineering Mode"); }
		}

		public double WHTCUrban
		{
			get { throw new VectoException("Property not available in Engineering Mode"); }
		}

		public double ColdHotBalancingFactor
		{
			get { throw new VectoException("Property not available in Engineering Mode"); }
		}

		public double CorrectionFactorRegPer
		{
			get {
				return 1;
				//GetDoubleElementValue(XMLNames.Engine_CorrectionFactor_RegPer); 
			}
		}

		public double CorrectionFactorNCV
		{
			get { return 1; //GetDoubleElementValue(XMLNames.Engine_CorrecionFactor_NCV); 
			}
		}

		public FuelType FuelType
		{
			get { return FuelType.DieselCI; //GetElementValue(XMLNames.Engine_FuelType).ParseEnum<FuelType>();
			}
		}

		public TableData FuelConsumptionMap
		{
			get {
				if (!ElementExists(Helper.Query(XMLNames.Engine_FuelConsumptionMap, XMLNames.Engine_FuelConsumptionMap_Entry))) {
					return ReadCSVResourceFile(XMLNames.Engine_FuelConsumptionMap);
				}
				return ReadTableData(AttributeMappings.FuelConsumptionMapMapping,
					Helper.Query(XMLNames.Engine_FuelConsumptionMap, XMLNames.Engine_FuelConsumptionMap_Entry));
			}
		}

		public TableData FullLoadCurve
		{
			get {
				if (!ElementExists(Helper.Query(XMLNames.Engine_FullLoadAndDragCurve, XMLNames.Engine_FuelConsumptionMap_Entry))) {
					return ReadCSVResourceFile(XMLNames.Engine_FullLoadAndDragCurve);
				}

				return ReadTableData(AttributeMappings.EngineFullLoadCurveMapping,
					Helper.Query(XMLNames.Engine_FullLoadAndDragCurve, XMLNames.Engine_FuelConsumptionMap_Entry));
			}
		}

		public Watt RatedPowerDeclared
		{
			get { return null; //GetDoubleElementValue(XMLNames.Engine_RatedPower).SI<Watt>(); 
			}
		}

		public PerSecond RatedSpeedDeclared
		{
			get { return null; //GetDoubleElementValue(XMLNames.Engine_RatedSpeed).RPMtoRad(); 
			}
		}

		public NewtonMeter MaxTorqueDeclared
		{
			get { return null; //GetDoubleElementValue(XMLNames.Engine_MaxTorque).SI<NewtonMeter>(); 
			}
		}

		public KilogramSquareMeter Inertia
		{
			get { return GetDoubleElementValue(XMLNames.Engine_Inertia).SI<KilogramSquareMeter>(); }
		}
	}
}