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

using System.Xml.XPath;
using TUGraz.IVT.VectoXML;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;

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