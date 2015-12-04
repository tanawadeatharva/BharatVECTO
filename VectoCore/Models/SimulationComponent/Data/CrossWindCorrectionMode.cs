/*
* Copyright 2015 European Union
*
* Licensed under the EUPL (the "Licence");
* You may not use this work except in compliance with the Licence.
* You may obtain a copy of the Licence at:
*
* http://ec.europa.eu/idabc/eupl5
*
* Unless required by applicable law or agreed to in writing, software 
* distributed under the Licence is distributed on an "AS IS" basis,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the Licence for the specific language governing permissions and 
* limitations under the Licence.
*/

using System;
using NLog;

namespace TUGraz.VectoCore.Models.SimulationComponent.Data
{
	public enum CrossWindCorrectionMode
	{
		NoCorrection,
		SpeedDependentCorrectionFactor,
		VAirBetaLookupTable,
		DeclarationModeCorrection
	}

	public static class CrossWindCorrectionModeHelper
	{
		public static CrossWindCorrectionMode Parse(string correctionMode)
		{
			if (correctionMode.Equals("CdofVEng", StringComparison.OrdinalIgnoreCase)) {
				return CrossWindCorrectionMode.SpeedDependentCorrectionFactor;
			}
			if (correctionMode.Equals("CdofVdecl", StringComparison.OrdinalIgnoreCase)) {
				return CrossWindCorrectionMode.DeclarationModeCorrection;
			}
			if (correctionMode.Equals("CdofBeta", StringComparison.OrdinalIgnoreCase)) {
				return CrossWindCorrectionMode.VAirBetaLookupTable;
			}
			if (correctionMode.Equals("Off", StringComparison.OrdinalIgnoreCase)) {
				return CrossWindCorrectionMode.NoCorrection;
			}
			LogManager.GetLogger(typeof(CrossWindCorrectionModeHelper).ToString())
				.Warn("Invalid Crosswind correction Mode given. Ignoring Crosswind Correction!");
			return CrossWindCorrectionMode.NoCorrection;
		}
	}
}