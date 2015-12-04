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

using Newtonsoft.Json;
using TUGraz.VectoCore.FileIO.DeclarationFile;

namespace TUGraz.VectoCore.FileIO.EngineeringFile
{
	internal class EngineFileV3Engineering : EngineFileV3Declaration
	{
		[JsonProperty(Required = Required.Always)] public new DataBodyEng Body;

		public class DataBodyEng : DataBodyDecl
		{
			/// <summary>
			///     [kgm^2] Inertia including Flywheel
			///     Inertia for rotating parts including engine flywheel.
			///     In Declaration Mode the inertia is calculated automatically.
			/// </summary>
			[JsonProperty(Required = Required.Always)] public double Inertia;
		}
	}
}