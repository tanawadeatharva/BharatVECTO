using NUnit.Framework;
using TUGraz.VectoCommon.InputData;

namespace TUGraz.VectoCore.Tests.Models;


[TestFixture]
public class SimulationJobTypeTest
{



	/// <summary>
	/// The only purpose of this test is to make sure we don't miss a simulation job type in the switch statements of the helper class
	/// </summary>
	/// <param name="jobType"></param>
	[Test]
	
	public void JobTypeTest([Values]VectoSimulationJobType jobType)
	{
		Assert.DoesNotThrow(() => jobType.HasEngine());
		Assert.DoesNotThrow(() => jobType.GetPowertrainArchitectureType());

	}
}