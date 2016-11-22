using System.Data;
using System.IO;
using System.Text;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Impl;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData
{
	public static class AuxiliaryFileHelper
	{
		public static void FillAuxiliaryDataInputData(AuxiliaryDataInputData auxData, string auxFile)
		{
			try {
				var stream = new StreamReader(auxFile);
				stream.ReadLine(); // skip header "Transmission ration to engine rpm [-]"
				auxData.TransmissionRatio = stream.ReadLine().IndulgentParse();
				stream.ReadLine(); // skip header "Efficiency to engine [-]"
				auxData.EfficiencyToEngine = stream.ReadLine().IndulgentParse();
				stream.ReadLine(); // skip header "Efficiency auxiliary to supply [-]"
				auxData.EfficiencyToSupply = stream.ReadLine().IndulgentParse();

				var table = VectoCSVFile.ReadStream(new MemoryStream(Encoding.UTF8.GetBytes(stream.ReadToEnd())), source: auxFile);
				foreach (DataRow row in table.Rows) {
					if (AuxiliaryDataReader.HeaderIsValid(table.Columns)) {
						row[AuxiliaryDataReader.Fields.MechPower] =
							row.ParseDouble(AuxiliaryDataReader.Fields.MechPower).SI().Kilo.Watt.Value();
						row[AuxiliaryDataReader.Fields.SupplyPower] =
							row.ParseDouble(AuxiliaryDataReader.Fields.SupplyPower).SI().Kilo.Watt.Value();
					} else {
						row[1] = row.ParseDouble(1).SI().Kilo.Watt.Value();
						row[2] = row.ParseDouble(2).SI().Kilo.Watt.Value();
					}
				}
				auxData.DemandMap = table;
			} catch (FileNotFoundException e) {
				throw new VectoException("Auxiliary file not found: " + auxFile, e);
			}
		}
	}
}