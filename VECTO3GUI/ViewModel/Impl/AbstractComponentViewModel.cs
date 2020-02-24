using System;
using Ninject;
using VECTO3GUI.ViewModel.Adapter;

namespace VECTO3GUI.ViewModel.Impl {
	public abstract class AbstractComponentViewModel : AbstractViewModel
	{
		private string _manufacturer;
		private string _model;
		private string _certificationNumber;
		private DateTime? _date;

		[Inject] public IAdapterFactory AdapterFactory { set; protected get; }


		#region Implementation of ICommonComponentParameters

		public string Manufacturer
		{
			get { return _manufacturer; }
			set { _manufacturer = value; }
		}

		public string Model
		{
			get { return _model; }
			set { _model = value; }
		}

		public string CertificationNumber
		{
			get { return _certificationNumber; }
			set { _certificationNumber = value; }
		}

		public DateTime? Date
		{
			get { return _date; }
			set { _date = value; }
		}

		#endregion
	}
}