using System;

namespace TUGraz.VectoCommon.Models
{
	public enum AxleLineType
	{
		SingleReductionAxle,
		SinglePortalAxle,
		HubReductionAxle,
		SingleReductionTandemAxle,
		HubReductionTandemAxle
	}

	public static class AxleLineTypeHelper
	{
		public static string ToXMLFormat(this AxleLineType type)
		{
			switch (type) {
				case AxleLineType.SingleReductionAxle:
					return "Single reduction axle";
				case AxleLineType.SinglePortalAxle:
					return "Single portal axle";
				case AxleLineType.HubReductionAxle:
					return "Hub reduction axle";
				case AxleLineType.SingleReductionTandemAxle:
					return "Single reduction tandem axle";
				case AxleLineType.HubReductionTandemAxle:
					return "Hub reduction tandem axle";
				default:
					throw new ArgumentOutOfRangeException("AxleLineType", type, null);
			}
		}
	}
}