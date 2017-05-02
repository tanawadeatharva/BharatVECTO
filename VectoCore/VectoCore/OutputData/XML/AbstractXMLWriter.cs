using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.Resources;

namespace TUGraz.IVT.VectoXML.Writer
{
	public abstract class AbstractXMLWriter
	{
		//protected const string SchemaLocationBaseUrl = "http://markus.quaritsch.at/VECTO/";
		protected const string SchemaLocationBaseUrl = "http://www.ivt.tugraz.at/VECTO/";
		protected const string SchemaVersion = "0.6";

		protected XNamespace tns;
		protected XNamespace rootNamespace;

		private const string Creator = "TU Graz, IVT-EM XML Exporter";
		protected readonly string Vendor;

		protected readonly string BasePath;


		protected AbstractXMLWriter(string basePath, string vendor)
		{
			BasePath = basePath;
			Vendor = vendor;
		}

		protected XElement[] GetDefaultComponentElements(string typeId, string makeAndModel)
		{
			return new[] {
				new XElement(tns + XMLNames.Component_Manufacturer, String.Format("{0,-5}", Vendor)),
				new XElement(tns + XMLNames.Component_Creator, String.Format("{0,-10}", Creator)),
				new XElement(tns + XMLNames.Component_Date, XmlConvert.ToString(DateTime.Now, XmlDateTimeSerializationMode.Utc)),
				new XElement(tns + XMLNames.Component_Model, String.Format("{0,-10}", makeAndModel)),
				new XElement(tns + XMLNames.Component_CertificationNumber, String.Format("{0,-10}", typeId)),
			};
		}

		protected object[] EmbedDataTable(DataTable table, Dictionary<string, string> mapping, string tagName = "Entry")
		{
			return (from DataRow row in table.Rows
				select
					new XElement(tns + tagName,
						table.Columns.Cast<DataColumn>()
							.Where(c => mapping.ContainsKey(c.ColumnName))
							.Select(c => new XAttribute(mapping[c.ColumnName], row[c])))).Cast<object>().ToArray();
		}

		protected string GetVehicleCategoryXML(VehicleCategory vehicleCategory)
		{
			switch (vehicleCategory) {
				case VehicleCategory.Coach:
				case VehicleCategory.Tractor:
					return vehicleCategory.ToString();
				case VehicleCategory.CityBus:
					return "City Bus";
				case VehicleCategory.InterurbanBus:
					return "Interurban Bus";
				case VehicleCategory.RigidTruck:
					return "Rigid Truck";

				default:
					throw new ArgumentOutOfRangeException("vehicleCategory", vehicleCategory, null);
			}
		}

		protected string GetCorrectionModeXML(CrossWindCorrectionMode mode)
		{
			switch (mode) {
				case CrossWindCorrectionMode.NoCorrection:
					return "No Correction";
				case CrossWindCorrectionMode.SpeedDependentCorrectionFactor:
					return "Speed Dependent Correction Factor";
				case CrossWindCorrectionMode.VAirBetaLookupTable:
					return "VAir Beta Lookup Table";
				case CrossWindCorrectionMode.DeclarationModeCorrection:
					return "Declaration Mode Correction";
				default:
					throw new ArgumentOutOfRangeException("CrosswindCorrection", mode, null);
			}
		}

		protected string GetRetarterTypeXML(RetarderType type)
		{
			switch (type) {
				case RetarderType.None:
					return "None";
				case RetarderType.TransmissionInputRetarder:
					return "Transmission Input Retarder";
				case RetarderType.TransmissionOutputRetarder:
					return "Transmission Output Retarder";
				case RetarderType.EngineRetarder:
					return "Engine Retarder";
				case RetarderType.LossesIncludedInTransmission:
					return "Losses included in Gearbox";
				default:
					throw new ArgumentOutOfRangeException("RetarderType", type, null);
			}
		}

		protected string GearboxtypeToXML(GearboxType type)
		{
			switch (type) {
				case GearboxType.MT:
				case GearboxType.AMT:
				case GearboxType.DrivingCycle:
					return type.ToString();
				case GearboxType.ATSerial:
					return "AT - Serial";
				case GearboxType.ATPowerSplit:
					return "AT - PowerSplit";
				default:
					throw new ArgumentOutOfRangeException("type", type, null);
			}
		}
	}
}