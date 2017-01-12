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