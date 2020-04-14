using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Ninject;
using VECTO3GUI.ViewModel.Adapter;

namespace VECTO3GUI.ViewModel.Impl
{
	public abstract class AbstractComponentViewModel : AbstractViewModel
	{
		private string _manufacturer;
		private string _model;
		private string _certificationNumber;
		private DateTime? _date;

		protected HashSet<string> _changedInput = new HashSet<string>();

		[Inject] public IAdapterFactory AdapterFactory { set; protected get; }


		#region Implementation of ICommonComponentParameters

		public virtual string Manufacturer
		{
			get { return _manufacturer; }
			set { SetProperty(ref _manufacturer, value); }
		}

		public virtual string Model
		{
			get { return _model; }
			set { SetProperty(ref _model, value); }
		}

		public virtual string CertificationNumber
		{
			get { return _certificationNumber; }
			set { SetProperty(ref _certificationNumber, value); }
		}

		public virtual DateTime? Date
		{
			get { return _date; }
			set { SetProperty(ref _date, value); }
		}

		#endregion


		protected void SetChangedProperty(bool changed, [CallerMemberName] string propertyName = "")
		{
			if (!changed) {
				if (_changedInput.Contains(propertyName))
					_changedInput.Remove(propertyName);
			}
			else {
				if (!_changedInput.Contains(propertyName))
					_changedInput.Add(propertyName);
			}
		}

		public override bool AnyDataChanges()
		{
			return _changedInput.Count > 0;
		}
	}
}