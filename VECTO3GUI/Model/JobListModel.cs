using System;
using System.Collections.Generic;
using System.IO;
using Castle.Core.Internal;
using VECTO3GUI.Helper;
using VECTO3GUI.ViewModel.Impl;

namespace VECTO3GUI.Model
{

	public class JobListEntry
	{
		public bool IsSelected { get; set; }
		public string JobFilePath { get; set; }
	}

	public class JobListModel
	{
		private const string ConfigFolderName = "Config";
		private const string JobListFileName = "JobList.txt";

		private string _jobListFilePath = Path.Combine(".", ConfigFolderName, JobListFileName);

		public List<JobListEntry> JobList { get; set; }

		public JobListModel()
		{
			SetConfigFolder();
			LoadJobList();
		}

		private void SetConfigFolder()
		{
			if (!Directory.Exists($"./{ConfigFolderName}")) {
				Directory.CreateDirectory($"{ConfigFolderName}");
			}
		}

		private void LoadJobList()
		{
			var jobList = new List<JobListEntry>();
			if (File.Exists(_jobListFilePath))
				jobList = SerializeHelper.DeserializeToObject<List<JobListEntry>>(_jobListFilePath);
			JobList = jobList;
		}


		public void SaveJobList(IList<JobEntry> jobEntries)
		{
			SetJobList(jobEntries);
			SerializeHelper.SerializeToFile(_jobListFilePath, JobList);
		}
		
		private void SetJobList(IList<JobEntry> jobEntries)
		{
			if (jobEntries == null)
				return;

			JobList = new List<JobListEntry>();

			for (int i = 0; i < jobEntries.Count; i++) {
				JobList.Add
				(
					new JobListEntry
					{
						IsSelected = jobEntries[i].Selected,
						JobFilePath = jobEntries[i].JobEntryFilePath
					}
				);
			}
		}

		public IList<JobEntry> GetJobEntries()
		{
			var jobEntries = new List<JobEntry>();

			if (JobList.IsNullOrEmpty())
				return jobEntries;

			for (int i = 0; i < JobList.Count; i++) {
				var jobEntry = SerializeHelper.DeserializeToObject<JobEntry>(JobList[i].JobFilePath);
				if(jobEntry != null)
					jobEntries.Add(jobEntry);
			}

			return jobEntries;
		}
	}
}
