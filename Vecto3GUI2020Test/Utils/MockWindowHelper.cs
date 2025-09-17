using System.Collections.Generic;
using NUnit.Framework;
using VECTO3GUI2020.Helper;

namespace Vecto3GUI2020Test.Utils
{
    public class MockWindowHelper : IWindowHelper
    {
		#region Implementation of IWindowHelper

		public List<object> Windows = new List<object>();

		public void ShowWindow(object viewModel)
		{
			TestContext.WriteLine($"Opened {viewModel.GetType()}");
			lock (Windows) {
				Windows.Add(viewModel);
            }
		}
		#endregion
	}
}
