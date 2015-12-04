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

using System.Collections.Generic;
using Newtonsoft.Json;
using TUGraz.VectoCore.FileIO.DeclarationFile;

namespace TUGraz.VectoCore.FileIO.EngineeringFile
{
	internal class VehicleFileV7Engineering : VehicleFileV7Declaration
	{
		[JsonProperty(Required = Required.Always)] public new DataBodyEng Body;


		internal class DataBodyEng : DataBodyDecl
		{
			[JsonProperty] public double CurbWeightExtra;

			[JsonProperty] public double Loading;


			[JsonProperty("rdyn")] public double DynamicTyreRadius;


			[JsonProperty("CdCorrMode")] public string CrossWindCorrectionModeStr;

			[JsonProperty("CdCorrFile")] public string CrossWindCorrectionFile;


			//[JsonProperty(Required = Required.Always)] public new AxleConfigData AxleConfig;


			//public new class AxleConfigData
			//{
			//	[JsonProperty("Type", Required = Required.Always)] public string TypeStr;
			//	[JsonProperty(Required = Required.Always)] public IList<AxleDataEng> Axles;
			//}

			//public class AxleDataEng : AxleDataDecl
			//{
			//	[JsonProperty] public double Inertia;
			//	[JsonProperty] public double AxleWeightShare;
			//}
		}
	}
}

//}