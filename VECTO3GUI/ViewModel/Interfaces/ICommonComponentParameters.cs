using System;

namespace VECTO3.ViewModel.Interfaces {
	public interface ICommonComponentParameters
	{
		string Manufacturer { get; set; }
		string Model { get; set; }
		string CertificationNumber { get; set; }
		DateTime? Date { get; set; }
	}
}