using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Ninject;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.OutputData.XML;
using TUGraz.VectoCore.Utils;
using VECTO3.Util;
using VECTO3.ViewModel.Adapter.Declaration;
using VECTO3.ViewModel.Interfaces;
using Component = VECTO3.Util.Component;

namespace VECTO3.ViewModel.Impl
{
	public class DeclarationJobViewModel : AbstractJobViewModel, IJobEditViewModel
	{
		private IDeclarationInputDataProvider _inputData;

		
		public DeclarationJobViewModel(IKernel kernel, IDeclarationInputDataProvider inputData)
		{
			Kernel = kernel;
			InputDataProvider = inputData;
			JobViewModel = this;
			CreateComponentModel(Component.Vehicle);
			CreateComponentModel(Component.Cycle);
			CurrentComponent = GetComponentViewModel(Component.Vehicle);
		}

		#region Implementation of IJobEditViewModel

		public string JobFile
		{
			get { return _inputData.JobInputData.JobName; }
		}

		
		#endregion

		public IDeclarationInputDataProvider ModelData
		{
			get { return new DeclarationJobAdapter(this); }
		}


		protected override void DoSaveJob()
		{
			var writer = new XMLDeclarationWriter("TEST");
			var tmp = writer.GenerateVectoJob(ModelData);
		}

		
		public IInputDataProvider InputDataProvider
		{
			get { return _inputData; }
			set {
				value.Switch()
					.If<IDeclarationInputDataProvider>(
						d => {
							SetProperty(ref IsDeclarationMode, true);
							SetProperty(ref _inputData, d);
						}
					);
			}
		}

		
	}
}
