using System.Collections.Generic;
using TUGraz.VectoCore.Models.Simulation.Data;

namespace TUGraz.VectoCore.InputData
{
	public interface IVectoRunDataFactory
	{
		//void SetJobFile(string fileName);

		IEnumerable<VectoRunData> NextRun();

		//bool IsEngineOnly { get; }
		//	void SetJobJson(string jsonData, string basePath);
	}

}