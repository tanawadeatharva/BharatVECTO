using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Ninject;
using Ninject.Parameters;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration;
using TUGraz.VectoCore.InputData.FileIO.XML.Engineering;
using TUGraz.VectoCore.Utils;
using VECTO3.Util;
using VECTO3.ViewModel.Interfaces;

namespace VECTO3.ViewModel.Impl
{
	public class JoblistViewModel : ObservableObject, IJoblistViewModel
	{
		protected readonly ObservableCollection<JobEntry> _jobs = new ObservableCollection<JobEntry>();


		public JoblistViewModel()
		{
			AddJobEntry(@"~\..\..\..\..\Generic Vehicles\Declaration Mode\Class5_Tractor_4x2\Class5_Tractor_DECL.xml");
			AddJobEntry("DummyEntry");
			AddJobEntry(@"~\..\..\..\..\Generic Vehicles\Declaration Mode\Class5_Tractor_4x2\Class5_Tractor_ENG.vecto");
			AddJobEntry(@"~\..\..\..\..\Generic Vehicles\Engineering Mode\EngineOnly\EngineOnly.vecto");
		}

		private void AddJobEntry(string jobFile)
		{
			_jobs.Add(new JobEntry() {
				Filename = jobFile,
				Selected = false,
				Sorting = _jobs.Count
			});
		}

		public ObservableCollection<JobEntry> Jobs
		{
			get { return _jobs; }
		}

		public ICommand AddJob { get { return new RelayCommand(() => {}, () => false); } }


		public ICommand RemoveJob { get { return new RelayCommand<object>(DoRemoveJob, CanRemoveJob);} }

		private void DoRemoveJob(object selected)
		{
			var list = (IList)selected;
			var selectedEntries = list.Cast<JobEntry>().ToArray();

			foreach (var entry in selectedEntries) {
				_jobs.Remove(entry);
			}
		}

		private bool CanRemoveJob(object selected)
		{
			var list = (IList)selected;
			return list.Count > 0;
		}

		public ICommand MoveJobUp { get { return new RelayCommand(() => { }, () => false); } }


		public ICommand MoveJobDown { get { return new RelayCommand(() => { }, () => false); } }


		public ICommand StartSimulation { get { return new RelayCommand(DoStartSimulation, CanStartSimulation); } }

		private void DoStartSimulation()
		{
			
		}
		private bool CanStartSimulation()
		{
			return false;
		}

		public ICommand EditJob { get { return new RelayCommand<object>(DoEditJob, CanEditJob);} }

		public ICommand JobEntrySetActive { get {return new RelayCommand<object>(DoJobEntrySetActive);} }

		private void DoJobEntrySetActive(object obj)
		{
			var jobEntry = (JobEntry)((ListViewItem)obj).Content;
			jobEntry.Selected = !jobEntry.Selected;
		}

		private void DoEditJob(object selected)
		{
			var entry = (JobEntry)selected;
			if (entry == null) {
				return;
			}

			try {
				var jobEditView = ReadJob(entry.Filename); //Kernel.Get<IJobEditViewModel>();

				var wnd = new Window { Content = jobEditView };
				wnd.Show();
			} catch (Exception e) {
				MessageBox.Show(
					"Failed to read selected job: " + Environment.NewLine + Environment.NewLine + e.Message, "Failed reading Job",
					MessageBoxButton.OK);
			}
		}

		private IJobEditViewModel ReadJob(string jobFile)
		{
			IInputDataProvider inputData = null;
			var ext = Path.GetExtension(jobFile);
			switch (ext) {
				case Constants.FileExtensions.VectoJobFile:
					inputData = JSONInputDataFactory.ReadJsonJob(jobFile);
					break;
				case Constants.FileExtensions.VectoXMLDeclarationFile:
				//ToDo
				//case Constants.FileExtensions.VectoXMLJobFile:
				//inputData = Kernel.Get<IXMLInputDataReader>().CreateDeclaration(jobFile);
				//break;
				default:
					throw new UnsupportedFileVersionException(jobFile);
			}

			var retVal = CreateJobEditViewModel(inputData);

			if (retVal == null) {
				throw new Exception("Unsupported job type");
			}
			return retVal;
		}

		private IJobEditViewModel CreateJobEditViewModel(IInputDataProvider inputData)
		{
			IJobEditViewModel retVal = null;
			if (inputData is JSONInputDataV2) {
				var jsoninputData = inputData as JSONInputDataV2;
				if (jsoninputData.SavedInDeclarationMode) {
					retVal = new DeclarationJobViewModel(Kernel, jsoninputData);
				} else {
					if (jsoninputData.EngineOnlyMode) {
						retVal = new EngineOnlyJobViewModel(Kernel, jsoninputData);
					} else {
						// TODO!
					}
				}
			}
			//ToDo
			//if (inputData is XMLDeclarationInputDataProvider) {
			//	var declInput = inputData as IDeclarationInputDataProvider;
			//	retVal = new DeclarationJobViewModel(Kernel, declInput);
			//}
			return retVal;
		}

		private bool CanEditJob(object selected)
		{
			var list = (IList)selected;
			return list.Count == 1;
		}
	}
}
