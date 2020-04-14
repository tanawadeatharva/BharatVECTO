using System;
using TUGraz.VectoCommon.InputData;

namespace TUGraz.VectoCore.OutputData.XML {
	public class XMLCustomerReportCompletedBus : XMLCustomerReport
	{
		internal void WriteResult(XMLDeclarationReport.ResultEntry genericResult, XMLDeclarationReport.ResultEntry specificResult, IResult primaryResult)
		{
			foreach (var entry in primaryResult.EnergyConsumption) {
				// TODO!
			}
		}

		public override void WriteResult(XMLDeclarationReport.ResultEntry resultEntry)
		{
			throw new NotSupportedException();
		}
	}
}