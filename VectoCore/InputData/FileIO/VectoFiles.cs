using System.IO;
using System.Runtime.Serialization;

namespace TUGraz.VectoCore.InputData.FileIO
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


	public abstract class VectoJobFile : VectoBaseFile, IJobInputData
	{
		private string _jobFile;

		[DataMember]
		internal string JobFile
		{
			get { return _jobFile; }
			set { _jobFile = Path.GetFileName(value); }
		}
	}

	public abstract class VectoVehicleFile : VectoBaseFile, IVehicleInputData {}

	public abstract class VectoGearboxFile : VectoBaseFile, IGearboxInputData, IAxleGearInputData {}

	public abstract class VectoEngineFile : VectoBaseFile, IEngineInputData {}

	public abstract class VectoAuxiliaryFile : VectoBaseFile, IAuxiliaryInputData {}
}