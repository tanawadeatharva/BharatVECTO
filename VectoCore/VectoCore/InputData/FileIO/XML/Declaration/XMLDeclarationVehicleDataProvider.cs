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
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Impl;
using TUGraz.VectoCore.Models.Declaration;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration
{
	public class XMLDeclarationVehicleDataProvider : AbstractDeclarationXMLComponentDataProvider,
		IVehicleDeclarationInputData, IPTOTransmissionInputData
	{
		private readonly IAuxiliariesDeclarationInputData XMLAuxiliaryData;

		public XMLDeclarationVehicleDataProvider(XMLDeclarationInputDataProvider xmlInputDataProvider)
			: base(xmlInputDataProvider)
		{
			XBasePath = VehiclePath;
			AirdragInputData = new XMLDeclarationAirdragDataProvider(xmlInputDataProvider);
			AxleGearInputData = new XMLDeclarationAxlegearDataProvider(xmlInputDataProvider);
			AngledriveInputData = new XMLDeclarationAngledriveDataProvider(xmlInputDataProvider);
			EngineInputData = new XMLDeclarationEngineDataProvider(xmlInputDataProvider);
			GearboxInputData = new XMLDeclarationGearboxDataProvider(xmlInputDataProvider);
			TorqueConverterInputData = new XMLDeclarationTorqueConverterDataProvider(xmlInputDataProvider);
			RetarderInputData = new XMLDeclarationRetarderDataProvider(xmlInputDataProvider);
			XMLAuxiliaryData = new XMLDeclarationAuxiliaryDataProvider(xmlInputDataProvider);
		}

		public string VIN
		{
			get { return GetElementValue(XMLNames.Vehicle_VIN); }
		}

		public LegislativeClass LegislativeClass
		{
			get { return GetElementValue(XMLNames.Vehicle_LegislativeClass).ParseEnum<LegislativeClass>(); }
		}

		public VehicleCategory VehicleCategory
		{
			get { return GetElementValue(XMLNames.Vehicle_VehicleCategory).ParseEnum<VehicleCategory>(); }
		}

		public Kilogram CurbMassChassis
		{
			get { return GetDoubleElementValue(XMLNames.Vehicle_CurbMassChassis).SI<Kilogram>(); }
		}


		public Kilogram GrossVehicleMassRating
		{
			get { return GetDoubleElementValue(XMLNames.Vehicle_GrossVehicleMass).SI<Kilogram>(); }
		}

		public IList<ITorqueLimitInputData> TorqueLimits
		{
			get {
				var retVal = new List<ITorqueLimitInputData>();
				var limits =
					Navigator.Select(Helper.Query(VehiclePath, XMLNames.Vehicle_TorqueLimits, XMLNames.Vehicle_TorqueLimits_Entry),
						Manager);
				while (limits.MoveNext()) {
					retVal.Add(new TorqueLimitInputData() {
						Gear = limits.Current.GetAttribute(XMLNames.Vehicle_TorqueLimits_Entry_Gear_Attr, "").ToInt(),
						MaxTorque =
							limits.Current.GetAttribute(XMLNames.Vehicle_TorqueLimits_Entry_MaxTorque_Attr, "").ToDouble().SI<NewtonMeter>()
					});
				}
				return retVal;
			}
		}

		public AxleConfiguration AxleConfiguration
		{
			get { return AxleConfigurationHelper.Parse(GetElementValue(XMLNames.Vehicle_AxleConfiguration)); }
		}

		public IList<IAxleDeclarationInputData> Axles
		{
			get {
				var axles = Navigator.Select(Helper.Query(VehiclePath, XMLNames.Vehicle_Components, XMLNames.Component_AxleWheels,
					XMLNames.ComponentDataWrapper, XMLNames.AxleWheels_Axles, XMLNames.AxleWheels_Axles_Axle), Manager);

				var retVal = new IAxleDeclarationInputData[axles.Count];
				while (axles.MoveNext()) {
					var axleNumber = axles.Current.GetAttribute(XMLNames.AxleWheels_Axles_Axle_AxleNumber_Attr, "").ToInt();
					if (axleNumber < 1 || axleNumber > retVal.Length) {
						throw new VectoException("Axle #{0} exceeds axle count", axleNumber);
					}
					if (retVal[axleNumber - 1] != null) {
						throw new VectoException("Axle #{0} defined multiple times!", axleNumber);
					}
					var axleType = axles.Current.SelectSingleNode(Helper.NSPrefix(XMLNames.AxleWheels_Axles_Axle_AxleType), Manager);
					var twinTyres = axles.Current.SelectSingleNode(Helper.NSPrefix(XMLNames.AxleWheels_Axles_Axle_TwinTyres), Manager);
					var steered = axles.Current.SelectSingleNode(Helper.NSPrefix(XMLNames.AxleWheels_Axles_Axle_Steered), Manager);
					var tyre =
						axles.Current.SelectSingleNode(Helper.Query(XMLNames.AxleWheels_Axles_Axle_Tyre, XMLNames.ComponentDataWrapper),
							Manager);
					if (tyre == null) {
						throw new VectoException("Axle #{0} contains no tyre definition", axleNumber);
					}
					var dimension = tyre.SelectSingleNode(Helper.NSPrefix(XMLNames.AxleWheels_Axles_Axle_Dimension), Manager);
					var rollResistance = tyre.SelectSingleNode(Helper.NSPrefix(XMLNames.AxleWheels_Axles_Axle_RRCDeclared), Manager);
					var tyreTestLoad = tyre.SelectSingleNode(Helper.NSPrefix(XMLNames.AxleWheels_Axles_Axle_FzISO), Manager);
					var certirficationNumber = tyre.SelectSingleNode(Helper.NSPrefix(XMLNames.Component_CertificationNumber), Manager);
					var digestValue = tyre.SelectSingleNode(Helper.Query("..//*[local-name()='DigestValue']"), Manager);
					retVal[axleNumber - 1] = new AxleInputData {
						AxleType = axleType == null ? AxleType.VehicleNonDriven : axleType.Value.ParseEnum<AxleType>(),
						TwinTyres = twinTyres != null && XmlConvert.ToBoolean(twinTyres.Value),
						Steered = steered != null && XmlConvert.ToBoolean(steered.Value),
						Tyre = new TyreInputData() { 
							TyreTestLoad = tyreTestLoad == null ? null : tyreTestLoad.Value.ToDouble().SI<Newton>(),
							RollResistanceCoefficient = rollResistance == null ? double.NaN : rollResistance.Value.ToDouble(),
							Dimension = dimension == null ? null : dimension.Value,
							CertificationNumber = certirficationNumber == null ? null : certirficationNumber.Value,
							CertificationMethod = CertificationMethod.Measured,
							DigestValue = digestValue == null ? "" : digestValue.Value
						}
					};
				}
				return retVal;
			}
		}

		public string ManufacturerAddress
		{
			get { return GetElementValue(XMLNames.Component_ManufacturerAddress); }
		}

		public PerSecond EngineIdleSpeed
		{
			get { return GetDoubleElementValue(XMLNames.Vehicle_IdlingSpeed).RPMtoRad(); }
		}

		public double RetarderRatio
		{
			get { return GetDoubleElementValue(XMLNames.Vehicle_RetarderRatio); }
		}

		public RetarderType RetarderType
		{
			get {
				var value = GetElementValue(XMLNames.Vehicle_RetarderType); //.ParseEnum<RetarderType>(); 
				switch (value) {
					case "None":
						return RetarderType.None;
					case "Losses included in Gearbox":
						return RetarderType.LossesIncludedInTransmission;
					case "Engine Retarder":
						return RetarderType.EngineRetarder;
					case "Transmission Input Retarder":
						return RetarderType.TransmissionInputRetarder;
					case "Transmission Output Retarder":
						return RetarderType.TransmissionOutputRetarder;
				}
				throw new ArgumentOutOfRangeException("RetarderType", value);
			}
		}

		public AngledriveType AngulargearType
		{
			get { return GetElementValue(XMLNames.Vehicle_AngledriveType).ParseEnum<AngledriveType>(); }
		}

		public string PTOTransmissionType
		{
			get {
				var shaftGearWheels = GetElementValue(Helper.Query(XMLNames.Vehicle_PTO, XMLNames.Vehicle_PTO_ShaftsGearWheels));
				if ("none".Equals(shaftGearWheels, StringComparison.InvariantCultureIgnoreCase)) {
					return "None";
				}
				if ("only one engaged gearwheel above oil level".Equals(shaftGearWheels, StringComparison.CurrentCultureIgnoreCase)) {
					return "only one engaged gearwheel above oil level";
				}
				var otherElements = GetElementValue(Helper.Query(XMLNames.Vehicle_PTO, XMLNames.Vehicle_PTO_OtherElements));
				var ptoTech = string.Format("{0} - {1}", shaftGearWheels, otherElements);
				if (DeclarationData.PTOTransmission.GetTechnologies().Contains(ptoTech)) {
					return ptoTech;
				}
				throw new VectoException("PTO Technology {0} invalid!", ptoTech);
			}
		}

		public TableData PTOLossMap
		{
			get { return null; }
		}

		public TableData PTOCycle
		{
			get { return null; }
		}

		public IAirdragDeclarationInputData AirdragInputData { get; private set; }

		public IGearboxDeclarationInputData GearboxInputData { get; private set; }

		public ITorqueConverterDeclarationInputData TorqueConverterInputData { get; private set; }

		public IAxleGearInputData AxleGearInputData { get; private set; }

		public IAngledriveInputData AngledriveInputData { get; private set; }

		public IEngineDeclarationInputData EngineInputData { get; private set; }

		public IAuxiliariesDeclarationInputData AuxiliaryInputData()
		{
			return XMLAuxiliaryData;
		}

		public IRetarderInputData RetarderInputData { get; private set; }

		public IPTOTransmissionInputData PTOTransmissionInputData
		{
			get { return this; }
		}
	}
}
