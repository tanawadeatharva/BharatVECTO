using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace TUGraz.VectoCore.InputData.FileIO.JSON
{
    public class JSONFuelCellSystem : JSONFile
    {
		public JSONFuelCellSystem(JObject data, string filename, bool tolerateMissing = false) : base(data, filename,
			tolerateMissing)
		{

		}




	}
}
