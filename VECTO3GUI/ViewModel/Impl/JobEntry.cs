using System;
using Newtonsoft.Json;

namespace VECTO3GUI.ViewModel.Impl
{
	public enum JobType
	{
		SingleBusJob,
		CompletedBusJob,
		Unknown
	}

	public static class JobTypeHelper
	{
		private const string SingleBusJobLabel = "Single Bus Job";
		private const string CompletedBusJobLabel = "Completed Bus Job";


		public static string GetLabel(this JobType jobType)
		{
			switch (jobType)
			{
				case JobType.SingleBusJob:
					return SingleBusJobLabel;
				case JobType.CompletedBusJob:
					return CompletedBusJobLabel;
				default:
					return string.Empty;
			}
		}

		public static JobType Parse(this string jobTypeName)
		{
			if (SingleBusJobLabel == jobTypeName)
				return JobType.SingleBusJob;
			if (CompletedBusJobLabel == jobTypeName)
				return JobType.CompletedBusJob;
			return JobType.Unknown;
		}
	}

	[JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
	public class JobEntry : ObservableObject
	{
		private JobType _jobType;
		private string _jobTypeName;
		private bool _selected;
		private string _jobEntryFilePath;
		private string _firstFilePath;
		private string _secondFilePath;

		[JsonIgnore]
		public int Sorting;

		[JsonIgnore]
		public JobType JobType
		{
			get { return _jobType;}
			set
			{
				_jobType = value;
				_jobTypeName = _jobType.GetLabel();
			}
		}

		public string JobTypeName
		{
			get { return _jobTypeName; }
			set
			{
				_jobTypeName = value;
				_jobType = _jobTypeName.Parse();
			}
		}

		[JsonIgnore]
		public bool Selected
		{
			get { return _selected; }
			set { SetProperty(ref _selected, value); }
		}

		public string JobEntryFilePath
		{
			get { return _jobEntryFilePath; }
			set { SetProperty(ref _jobEntryFilePath, value); }
		}
		public string FirstFilePath
		{
			get { return _firstFilePath; }
			set { SetProperty(ref _firstFilePath, value); }
		}

		public string SecondFilePath
		{
			get { return _secondFilePath; }
			set { SetProperty(ref _secondFilePath, value); }
		}
	}
}