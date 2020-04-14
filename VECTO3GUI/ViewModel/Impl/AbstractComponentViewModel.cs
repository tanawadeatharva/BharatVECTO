using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Ninject;
using VECTO3GUI.ViewModel.Adapter;

namespace VECTO3GUI.ViewModel.Impl
{
	public abstract class AbstractComponentViewModel : AbstractViewModel
	{
		protected HashSet<string> _changedInput = new HashSet<string>();

		[Inject] public IAdapterFactory AdapterFactory { set; protected get; }

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