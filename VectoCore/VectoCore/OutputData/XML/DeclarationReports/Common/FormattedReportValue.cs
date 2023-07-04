using System;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.Common
{

	public class FormattedReportValue
	{
		public ConvertedSI Value { get; }

		protected Func<ConvertedSI, object[]> Formatter;

		public FormattedReportValue(ConvertedSI value, Func<ConvertedSI, object[]> formatter = null)
		{
			Value = value;
			Formatter = formatter ?? DefaultFormat;
		}

		public object[] GetElement()
		{
			return Formatter(Value);
		}

		protected object[] DefaultFormat(ConvertedSI value)
		{
			return Value.ValueAsUnit(4, 1);
		}

		public static object[] Format1Decimal(ConvertedSI value)
		{
			return value.ValueAsUnit(1);
		}

		public static object[] Format2Decimal(ConvertedSI value)
		{
			return value.ValueAsUnit(2);
		}
	}
}