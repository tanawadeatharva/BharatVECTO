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

using System.IO;
using System.Runtime.Serialization;

namespace TUGraz.VectoCore.FileIO
{
	public abstract class VectoBaseFile
	{
		protected string basePath;

		[DataMember]
		internal string BasePath
		{
			get { return basePath; }
			set { basePath = Path.GetDirectoryName(Path.GetFullPath(value)); }
		}
	}


	public abstract class VectoJobFile : VectoBaseFile
	{
		private string _jobFile;

		[DataMember]
		internal string JobFile
		{
			get { return _jobFile; }
			set { _jobFile = Path.GetFileName(value); }
		}
	}

	public abstract class VectoVehicleFile : VectoBaseFile {}

	public abstract class VectoGearboxFile : VectoBaseFile {}

	public abstract class VectoEngineFile : VectoBaseFile {}

	public abstract class VectoAuxiliaryFile : VectoBaseFile {}
}