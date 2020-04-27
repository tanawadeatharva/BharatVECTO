using System;
using System.Security.RightsManagement;
using System.Windows.Forms;

namespace VECTO3GUI.ViewModel.Impl {

	public enum JobType
	{
		SingleBusJob,
		CompletedBusJob,
		Unknown
	}

	public static class JobTypeHelper
	{
		public static string GetLabel(this JobType jobType)
		{
			switch (jobType) {
				case JobType.SingleBusJob:
					return "Single Bus Job";
				case JobType.CompletedBusJob:
					return "Completed Bus Job";
				default:
					return string.Empty;
			}
		}

		public static JobType Parse(this string jobTypeName)
		{
			if (JobType.SingleBusJob.GetLabel() == jobTypeName)
				return JobType.SingleBusJob;
			if (JobType.CompletedBusJob.GetLabel() == jobTypeName)
				return JobType.CompletedBusJob;
			return JobType.Unknown;
		}
	}
	
	public class JobEntry : ObservableObject
	{
		private bool _selected;
		private string _filename;

		public int Sorting;

		public bool Selected
		{
			get { return _selected; }
			set { SetProperty(ref _selected, value); }
		}

		public string Filename
		{
			get { return _filename; }
			set { SetProperty(ref _filename, value); }
		}
	}
}