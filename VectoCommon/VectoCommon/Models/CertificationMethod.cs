using System;

namespace TUGraz.VectoCommon.Models
{
	public enum CertificationMethod
	{
		StandardValues,
		Measured,
		NotCertified
	}

	public static class CertificationMethodHelper
	{
		public static string ToXMLFormat(this CertificationMethod method)
		{
			switch (method) {
				case CertificationMethod.StandardValues:
					return "Standard values";
				case CertificationMethod.Measured:
					return "Measured";
				case CertificationMethod.NotCertified:
					return "NOT CERTIFIED";
				default:
					throw new ArgumentOutOfRangeException("CertificationMethod", method, null);
			}
		}
	}
}