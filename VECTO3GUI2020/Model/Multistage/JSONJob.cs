using System;
using Newtonsoft.Json;
using VECTO3GUI2020.ViewModel.Implementation.Common;

namespace VECTO3GUI2020.Model.Multistage
{
	public class JSONJob : ObservableObject
	{
		private JSONJobHeader _jobHeader;

		public JSONJobHeader JobHeader
		{
			get => _jobHeader;
			set => SetProperty(ref _jobHeader, value);
		}

		private JSONJobBody _jobBody;

		private JSONJobBody JobBody
		{
			get => _jobBody;
			set => SetProperty(ref _jobBody, value);
		}
	}



	public class JSONJobHeader : ObservableObject
	{
		public class JobHeader : ObservableObject
		{
			public const int PrimaryAndInterimVersion = 10;
			//public const int CompletedBusFileVersion = 7;

			private string _createdBy;
			private DateTime _dateTime;
			private string _appVersion;
			private int _fileVersion;


			public string CreatedBy
			{
				get { return _createdBy; }
				set { SetProperty(ref _createdBy, value); }
			}

			public DateTime Date
			{
				get { return _dateTime; }
				set { SetProperty(ref _dateTime, value); }
			}

			public string AppVersion
			{
				get { return _appVersion; }
				set { SetProperty(ref _appVersion, value); }
			}

			public int FileVersion
			{
				get { return _fileVersion; }
				set
				{
					SetProperty(ref _fileVersion, value);
					//JobType = JobTypeHelper.GetJobTypeByFileVersion(_fileVersion);
				}
			}
		}
	}


	public class JSONJobBody : ObservableObject
	{
		private string _interimVehicle;
		private string _primaryVehicle;

		public string InterimVehicle
		{
			get { return _interimVehicle; }
			set { SetProperty(ref _interimVehicle, value); }
		}

		public string PrimaryVehicle
		{
			get { return _primaryVehicle; }
			set { SetProperty(ref _primaryVehicle, value); }
		}
	}
}