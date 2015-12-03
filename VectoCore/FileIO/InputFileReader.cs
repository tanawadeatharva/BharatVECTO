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
using TUGraz.VectoCore.Models;

namespace TUGraz.VectoCore.FileIO
{
	public class InputFileReader : LoggingObject
	{
		protected class VersionInfo
		{
			public bool SavedInDeclarationMode;
			public int Version;
		}

		protected InputFileReader() {}

		protected static VersionInfo GetFileVersion(string jsonStr)
		{
			var data = new { Header = new { FileVersion = -1 }, Body = new { SavedInDeclMode = false } };
			data = JsonConvert.DeserializeAnonymousType(jsonStr, data);
			return new VersionInfo { SavedInDeclarationMode = data.Body.SavedInDeclMode, Version = data.Header.FileVersion };
		}
	}
}