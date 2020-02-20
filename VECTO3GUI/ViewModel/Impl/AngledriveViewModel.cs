using System;
using System.Collections.ObjectModel;
using System.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Utils;
using VECTO3.Util;
using VECTO3.ViewModel.Adapter.Declaration;
using VECTO3.ViewModel.Interfaces;

namespace VECTO3.ViewModel.Impl {
	public class AngledriveViewModel : AbstractComponentViewModel, IAngledriveViewModel
	{
		private CertificationMethod _certificationMethod;
		private readonly ObservableCollection<GearLossMapEntry> _lossMap = new ObservableCollection<GearLossMapEntry>();
		private double _ratio;

		#region Implementation of IAngledriveViewModel

		public IAngledriveInputData ModelData { get { return AdapterFactory.AngledriveDeclarationAdapter(this); } }

		public CertificationMethod CertificationMethod
		{
			get { return _certificationMethod; }
			set { SetProperty(ref _certificationMethod, value); }
		}

		public AllowedEntry<CertificationMethod>[] AllowedCertificationMethods
		{
			get
			{
				return null;
				//ToDo
				//return DeclarationData.AngledriveCertificationMethods.Select(x => AllowedEntry.Create(x, x.GetLabel())).ToArray();
			}
		}

		public double Ratio
		{
			get { return _ratio; }
			set { SetProperty(ref _ratio, value); }
		}

		public ObservableCollection<GearLossMapEntry> LossMap
		{
			get { return _lossMap; }
		}

		public AngledriveType AngledriveType
		{
			get { return (ParentViewModel as IVehicleViewModel).AngledriveType; }
			set { (ParentViewModel as IVehicleViewModel).AngledriveType = value; }
		}

		#endregion

		protected override void InputDataChanged()
		{
			JobViewModel.InputDataProvider.Switch()
						.If<IDeclarationInputDataProvider>(d => SetValues(d.JobInputData.Vehicle.Components.AngledriveInputData))
						.If<IEngineeringInputDataProvider>(e => SetValues(e.JobInputData.Vehicle.Components.AngledriveInputData));
		}

		private void SetValues(IAngledriveInputData angledrive)
		{
			Model = angledrive.Model;
			Manufacturer = angledrive.Manufacturer;
			CertificationNumber = angledrive.CertificationNumber;
			CertificationMethod = angledrive.CertificationMethod;
			//ToDo
			//Date = DateTime.Parse(angledrive.Date);
			Ratio = angledrive.Ratio;
			//ToDo
			//var lossMap = TransmissionLossMapReader.Create(angledrive.LossMap, angledrive.Ratio, "Angledirve").Entries.OrderBy(x => x.InputSpeed).ThenBy(x => x.InputTorque);
			//foreach (var entry in lossMap) {
			//	LossMap.Add(new GearLossMapEntry(entry));
			//}
		}
	}
}