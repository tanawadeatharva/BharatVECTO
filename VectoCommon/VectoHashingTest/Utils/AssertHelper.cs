using System;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace VectoHashingTest.Utils
{
	public static class AssertHelper
	{
		/// <summary>
		/// Assert an expected Exception.
		/// </summary>
		[DebuggerHidden]
		public static void Exception<T>(this Action func, string message = null) where T : Exception
		{
			try {
				func();
				Assert.Fail("Expected Exception {0}, but no exception occured.", typeof(T));
			} catch (T ex) {
				if (message != null) {
					Assert.AreEqual(message, ex.Message);
				}
			}
		}
	}
}