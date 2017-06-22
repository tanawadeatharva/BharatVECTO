/*
* This file is part of VECTO.
*
* Copyright © 2012-2016 European Union
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

using TUGraz.IVT.VectoXML;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration
{
	public class XMLDeclarationEngineDataProvider : AbstractDeclarationXMLComponentDataProvider,
		IEngineDeclarationInputData
	{
		public XMLDeclarationEngineDataProvider(XMLDeclarationInputDataProvider xmlInputDataProvider)
			: base(xmlInputDataProvider)
		{
			XBasePath = Helper.Query(VehiclePath,
				XMLNames.Vehicle_Components,
				XMLNames.Component_Engine,
				XMLNames.ComponentDataWrapper);
		}

		public CubicMeter Displacement
		{
			get { return GetDoubleElementValue(XMLNames.Engine_Displacement).SI().Cubic.Centi.Meter.Cast<CubicMeter>(); }
		}

		public PerSecond IdleSpeed
		{
			get { return GetDoubleElementValue(XMLNames.Engine_IdlingSpeed).RPMtoRad(); }
		}

		public FuelType FuelType
		{
			get { return GetElementValue(XMLNames.Engine_FuelType).ParseEnum<FuelType>(); }
		}

		public TableData FuelConsumptionMap
		{
			get {
				return ReadTableData(AttributeMappings.FuelConsumptionMapMapping,
					Helper.Query(XMLNames.Engine_FuelConsumptionMap, XMLNames.Engine_FuelConsumptionMap_Entry));
			}
		}

		public TableData FullLoadCurve
		{
			get {
				return ReadTableData(AttributeMappings.EngineFullLoadCurveMapping,
					Helper.Query(XMLNames.Engine_FullLoadAndDragCurve, XMLNames.Engine_FullLoadCurve_Entry));
			}
		}

		public Watt RatedPowerDeclared
		{
			get { return GetDoubleElementValue(XMLNames.Engine_RatedPower).SI<Watt>(); }
		}

		public PerSecond RatedSpeedDeclared
		{
			get { return GetDoubleElementValue(XMLNames.Engine_RatedSpeed).RPMtoRad(); }
		}

		public NewtonMeter MaxTorqueDeclared
		{
			get { return GetDoubleElementValue(XMLNames.Engine_MaxTorque).SI<NewtonMeter>(); }
		}

		public double WHTCMotorway
		{
			get { return GetDoubleElementValue(XMLNames.Engine_WHTCMotorway); }
		}

		public double WHTCRural
		{
			get { return GetDoubleElementValue(XMLNames.Engine_WHTCRural); }
		}

		public double WHTCUrban
		{
			get { return GetDoubleElementValue(XMLNames.Engine_WHTCUrban); }
		}

		public double ColdHotBalancingFactor
		{
			get { return GetDoubleElementValue(XMLNames.Engine_ColdHotBalancingFactor); }
		}

		public double CorrectionFactorRegPer
		{
			get { return GetDoubleElementValue(XMLNames.Engine_CorrectionFactor_RegPer); }
		}

		public double CorrectionFactorNCV
		{
			get { return GetDoubleElementValue(XMLNames.Engine_CorrecionFactor_NCV); }
		}
	}
}