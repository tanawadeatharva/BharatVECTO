using System;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VECTO;

namespace VectoLegacyTests
{
	[TestClass]
	public class TorqueConverterTest
	{
		[ClassInitialize]
		public static void InitTests(TestContext ctx)
		{
			if (CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator == ".") {
				return;
			}
			try {
				Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
				Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
			} catch (Exception) {}
		}
	}
}