using System.Collections.Generic;
using System.Xml;
using System.Xml.XPath;
using TUGraz.IVT.VectoXML;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Impl;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Engineering
{
	public class XMLEngineeringGearboxDataProvider : AbstractEngineeringXMLComponentDataProvider,
		IGearboxEngineeringInputData
	{
		public XMLEngineeringGearboxDataProvider(XMLEngineeringInputDataProvider xmlEngineeringJobInputDataProvider,
			XPathDocument gbxDocument, string xmlBasePath, string fsBasePath)
			: base(xmlEngineeringJobInputDataProvider, gbxDocument, xmlBasePath, fsBasePath) {}

		public GearboxType Type
		{
			get { return GetElementValue(XMLNames.Gearbox_TransmissionType).ParseEnum<GearboxType>(); }
		}


		public KilogramSquareMeter Inertia
		{
			get { return GetDoubleElementValue(XMLNames.Gearbox_Inertia).SI<KilogramSquareMeter>(); }
		}

		public Second TractionInterruption
		{
			get { return GetDoubleElementValue(XMLNames.Gearbox_TractionInterruption).SI<Second>(); }
		}

		public IList<ITransmissionInputData> Gears
		{
			get
			{
				var retVal = new List<ITransmissionInputData>();
				var gears = Navigator.Select(Helper.Query(XBasePath, XMLNames.Gearbox_Gears, XMLNames.Gearbox_Gears_Gear), Manager);
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
			if (
				ElementExists(Helper.Query(gearPath, XMLNames.Gearbox_Gear_TorqueLossMap, XMLNames.Gearbox_Gear_TorqueLossMap_Entry))) {
				retVal.LossMap = ReadTableData(AttributeMappings.TransmissionLossmapMapping,
					Helper.Query(gearPath, XMLNames.Gearbox_Gear_TorqueLossMap, XMLNames.Gearbox_Gear_TorqueLossMap_Entry));
			} else {
				retVal.LossMap = ReadCSVResourceFile(Helper.Query(gearPath, XMLNames.Gearbox_Gear_TorqueLossMap));
			}

			if (
				ElementExists(Helper.Query(gearPath, XMLNames.Gearbox_Gears_Gear_ShiftPolygon,
					XMLNames.Gearbox_Gears_Gear_ShiftPolygon_Entry))) {
				retVal.ShiftPolygon = ReadTableData(AttributeMappings.ShiftPolygonMapping,
					Helper.Query(gearPath, XMLNames.Gearbox_Gears_Gear_ShiftPolygon, XMLNames.Gearbox_Gears_Gear_ShiftPolygon_Entry));
			}
			if (
				ElementExists(Helper.Query(gearPath, XMLNames.Gearbox_Gears_Gear_ShiftPolygon, ExtCsvResourceTag))) {
				retVal.ShiftPolygon = ReadCSVResourceFile(Helper.Query(gearPath, XMLNames.Gearbox_Gears_Gear_ShiftPolygon));
			}

			retVal.MaxTorque = ElementExists(Helper.Query(gearPath, XMLNames.Gearbox_Gears_MaxTorque))
				? GetDoubleElementValue(Helper.Query(gearPath, XMLNames.Gearbox_Gears_MaxTorque)).SI<NewtonMeter>()
				: null;
			return retVal;
		}

		public Second MinTimeBetweenGearshift
		{
			get { return InputData.XMLEngineeringJobData.MinTimeBetweenGearshift; }
		}

		public double TorqueReserve
		{
			get { return InputData.XMLEngineeringJobData.TorqueReserve; }
		}

		public MeterPerSecond StartSpeed
		{
			get { return InputData.XMLEngineeringJobData.StartSpeed; }
		}

		public MeterPerSquareSecond StartAcceleration
		{
			get { return InputData.XMLEngineeringJobData.StartAcceleration; }
		}

		public double StartTorqueReserve
		{
			get { return InputData.XMLEngineeringJobData.StartTorqueReserve; }
		}


		ITorqueConverterDeclarationInputData IGearboxDeclarationInputData.TorqueConverter
		{
			get { return TorqueConverter; }
		}

		public ITorqueConverterEngineeringInputData TorqueConverter
		{
			get
			{
				return new XMLEngineeringTorqueConverterDataProvider(InputData, XMLDocument,
					Helper.Query(XBasePath, "parent::*", XMLNames.Component_TorqueConverter, XMLNames.ComponentDataWrapper), FSBasePath);
			}
		}

		public Second DownshiftAfterUpshiftDelay
		{
			get { return InputData.XMLEngineeringJobData.DownshiftAfterUpshiftDelay; }
		}

		public Second UpshiftAfterDownshiftDelay
		{
			get { return InputData.XMLEngineeringJobData.UpshiftAfterDownshiftDelay; }
		}

		public MeterPerSquareSecond UpshiftMinAcceleration
		{
			get { return InputData.XMLEngineeringJobData.UpshiftMinAcceleration; }
		}

		public Second PowershiftShiftTime
		{
			get { return InputData.XMLEngineeringJobData.PowershiftShiftTime; }
		}

	}
}