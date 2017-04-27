using System;
using System.Collections.Generic;
using System.Xml;
using TUGraz.IVT.VectoXML;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Impl;
using TUGraz.VectoCore.Resources;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration
{
	public class XMLDeclarationGearboxDataProvider : AbstractDeclarationXMLComponentDataProvider,
		IGearboxDeclarationInputData
	{
		public XMLDeclarationGearboxDataProvider(XMLInputDataProvider xmlInputDataProvider) : base(xmlInputDataProvider)
		{
			XBasePath = Helper.Query(VehiclePath,
				XMLNames.Vehicle_Components,
				XMLNames.Component_Gearbox,
				XMLNames.ComponentDataWrapper);
		}

		public GearboxType Type
		{
			get {
				var value = GetElementValue(XMLNames.Gearbox_TransmissionType);
				switch (value) {
					case "MT":
						return GearboxType.MT;
					case "AMT":
						return GearboxType.AMT;
					case "AT - Serial":
						return GearboxType.ATSerial;
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
			return retVal;
		}
	}
}