using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VECTO3GUI.ViewModel.Impl
{
	public class MessageEntry : ObservableObject
	{
		private string _message;
		private DateTime _time;
		private string _source;

		public string Message
		{
			get { return _message; }
			set { SetProperty(ref _message, value); }
		}

		public DateTime Time
		{
			get { return _time; }
			set { SetProperty(ref _time, value); }
		}
		public string Source
		{
			get { return _source; }
			set { SetProperty(ref _source, value); }
		}
	}
}
