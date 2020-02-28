using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUGraz.VectoCommon.InputData;

namespace VECTO3GUI.Model
{

	public class AlternatorDeclarationInputData : IAlternatorDeclarationInputData
	{
		public string Technology { get; set; }
		public double Ratio { get; set; }
	}

}
