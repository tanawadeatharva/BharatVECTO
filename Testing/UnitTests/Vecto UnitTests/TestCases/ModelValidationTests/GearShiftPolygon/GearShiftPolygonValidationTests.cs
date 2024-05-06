using NUnit.Framework;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Tests.Utils;
using TUGraz.VectoCore.Utils;
using Assert = NUnit.Framework.Assert;

namespace TUGraz.Vecto.UnitTests.TestCases.ModelValidationTests.GearShiftPolygon;

public class GearShiftPolygonValidationTests
{
	/// <summary>
	/// VECTO-517 Shiftpolygon is considered invalid
	/// </summary>
	[TestCase]
	public void ShiftCurve_ShiftPolygon_Validation_Test()
	{
		var vgbs = new[] {
			"-50,685,1537",
			"550,685,1537",
			"678,763,1537",
			"1080,1008,2092",
			"1200,1081,2092",
			"1200,1081,2092",
			"3000,1081,2092"
		};

		var shiftPolygon =
			ShiftPolygonReader.Create(
				VectoCSVFile.ReadStream(
					InputDataHelper.InputDataAsStream("engine torque,downshift rpm [rpm],upshift rpm [rpm]	", vgbs)));

		var results = shiftPolygon.Validate(ExecutionMode.Engineering, VectoSimulationJobType.ConventionalVehicle, null, GearboxType.MT, false);
		Assert.IsFalse(results.Any(), results.Select(r => r.ErrorMessage).Join("\n"));
	}
}