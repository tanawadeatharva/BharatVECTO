using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ninject;
using VECTO3GUI.ViewModel.Impl;
using VECTO3GUI.Views;

namespace VECTO3GUI.Helper
{

	public static class OutputWindowHelper
	{
		public static OutputWindow CreateOutputWindow(IKernel kernel, object viewModel,
			double width = default(double), double height= default(double))
		{
			var window =  new OutputWindow {
				DataContext = new OutputWindowViewModel(kernel, viewModel)
			};

			if (Math.Abs(width - default(double)) > 0 )
				window.Width = width;
			if (Math.Abs(height - default(double)) > 0)
				window.Height = height;

			return window;
		}


		public static OutputWindow CreateOutputWindow(IKernel kernel, object viewModel,
			string windowName, double width = default(double), double height = default(double))
		{
			var window = new OutputWindow
			{
				DataContext = new OutputWindowViewModel(kernel, viewModel, windowName)
			};

			if (Math.Abs(width - default(double)) > 0)
				window.Width = width;
			if (Math.Abs(height - default(double)) > 0)
				window.Height = height;

			return window;
		}

	}
}
