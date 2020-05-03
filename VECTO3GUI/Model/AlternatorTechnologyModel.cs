using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading.Tasks;
using VECTO3GUI.ViewModel.Impl;

namespace VECTO3GUI.Model
{
	public enum AlternatorTechnology
	{
		Empty,
		Default
	}
	
	public static class AlternatorTechnologyHelper 
	{
		public static string GetLabel(this AlternatorTechnology technology)
		{
			switch (technology) {
				case AlternatorTechnology.Default:
					return nameof(AlternatorTechnology.Default).ToLower();
				default:
					return string.Empty;
			}
		}

		public static AlternatorTechnology Parse(string technologyName)
		{
			switch (technologyName.ToLower()) {
				case "default":
					return AlternatorTechnology.Default;
				default:
					return AlternatorTechnology.Empty;
			}
		}
	}
	
	public class AlternatorTechnologyModel : ObservableObject
	{
		public AlternatorTechnology _alternatorTechnology;

		public AlternatorTechnology AlternatorTechnology
		{
			get { return _alternatorTechnology; }
			set { SetProperty(ref _alternatorTechnology, value); }
		}
	}
}
