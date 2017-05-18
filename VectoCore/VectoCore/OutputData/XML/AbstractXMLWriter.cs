using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Resources;

namespace TUGraz.IVT.VectoXML.Writer
{
	public abstract class AbstractXMLWriter
	{
		//protected const string SchemaLocationBaseUrl = "http://markus.quaritsch.at/VECTO/";
		protected const string SchemaLocationBaseUrl = "https://webgate.ec.europa.eu/CITnet/svn/VECTO/trunk/Share/XML/XSD/";
		protected const string SchemaVersion = "0.8";

		protected XNamespace tns;
		protected XNamespace rootNamespace;
		protected XNamespace di;

		protected const string Creator = "TU Graz, IVT-EM XML Exporter";
		protected readonly string Vendor;

		protected readonly string BasePath;


		protected AbstractXMLWriter(string basePath, string vendor)
		{
			BasePath = basePath;
			Vendor = vendor;

			di = "http://www.w3.org/2000/09/xmldsig#";
		}

		protected XElement CreateTorqueLimits(IVehicleDeclarationInputData vehicle)
		{
			return vehicle.TorqueLimits.Count == 0
				? null
				: new XElement(tns + XMLNames.Vehicle_TorqueLimits,
					vehicle.TorqueLimits
						.OrderBy(x => x.Gear)
						.Select(entry => new XElement(tns + XMLNames.Vehicle_TorqueLimits_Entry,
							new XAttribute(XMLNames.Vehicle_TorqueLimits_Entry_Gear_Attr, entry.Gear),
							new XAttribute(XMLNames.Vehicle_TorqueLimits_Entry_MaxTorque_Attr, entry.MaxTorque.ToXMLFormat(0))
							)
						)
					);
		}

		protected object[] EmbedDataTable(DataTable table, Dictionary<string, string> mapping, string tagName = "Entry",
			Dictionary<string, uint> precision = null)
		{
			return (from DataRow row in table.Rows
				select
					new XElement(tns + tagName,
						table.Columns.Cast<DataColumn>()
							.Where(c => mapping.ContainsKey(c.ColumnName))
							.Select(c => {
								var p = precision != null && precision.ContainsKey(c.ColumnName) ? precision[c.ColumnName] : 2;
								return new XAttribute(mapping[c.ColumnName], row.Field<string>(c).ToDouble().ToXMLFormat(p));
							})))
				.Cast<object>().ToArray();
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
	}
}