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
using System.Xml;
using TUGraz.IVT.VectoXML;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Impl;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration
{
	public class XMLDeclarationGearboxDataProvider : AbstractDeclarationXMLComponentDataProvider,
		IGearboxDeclarationInputData
	{
		public XMLDeclarationGearboxDataProvider(XMLDeclarationInputDataProvider xmlInputDataProvider)
			: base(xmlInputDataProvider)
		{
			XBasePath = Helper.Query(VehiclePath,
				XMLNames.Vehicle_Components,
				XMLNames.Component_Gearbox,
				XMLNames.ComponentDataWrapper);
		}

		public new CertificationMethod CertificationMethod
		{
			get { return GetElementValue(XMLNames.Component_Gearbox_CertificationMethod).ParseEnum<CertificationMethod>(); }
		}

		public GearboxType Type
		{
			get {
				var value = GetElementValue(XMLNames.Gearbox_TransmissionType);
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
				var retVal = new List<ITransmissionInputData>();
				var gears = Navigator.Select(
					Helper.Query(XBasePath, XMLNames.Gearbox_Gears, XMLNames.Gearbox_Gears_Gear),
					Manager);
				while (gears.MoveNext()) {
					var gear = gears.Current.GetAttribute(XMLNames.Gearbox_Gear_GearNumber_Attr, "");
					retVal.Add(ReadGear(gear));
				}
				return retVal;
			}
		}

		public ITorqueConverterDeclarationInputData TorqueConverter
		{
			get {
				return new XMLDeclarationTorqueConverterDataProvider(InputData);
			}
		}

		protected ITransmissionInputData ReadGear(string gearNr)
		{
			var retVal = new TransmissionInputData();
			var gearPath = Helper.Query(XMLNames.Gearbox_Gears,
				Helper.QueryConstraint(XMLNames.Gearbox_Gears_Gear, XMLNames.Gearbox_Gear_GearNumber_Attr, gearNr));
			retVal.Ratio = GetDoubleElementValue(Helper.Query(gearPath, XMLNames.Gearbox_Gear_Ratio));
			retVal.Gear = XmlConvert.ToUInt16(gearNr);
			retVal.LossMap = ReadTableData(AttributeMappings.TransmissionLossmapMapping,
				Helper.Query(gearPath, XMLNames.Gearbox_Gear_TorqueLossMap, XMLNames.Gearbox_Gear_TorqueLossMap_Entry));

			if (ElementExists(Helper.Query(gearPath, XMLNames.Gearbox_Gears_MaxTorque))) {
				retVal.MaxTorque = GetDoubleElementValue(Helper.Query(gearPath, XMLNames.Gearbox_Gears_MaxTorque)).SI<NewtonMeter>();
			}
			if (ElementExists(Helper.Query(gearPath, XMLNames.Gearbox_Gear_MaxSpeed))) {
				retVal.MaxInputSpeed = GetDoubleElementValue(Helper.Query(gearPath, XMLNames.Gearbox_Gear_MaxSpeed)).RPMtoRad();
			}
			return retVal;
		}
	}
}
