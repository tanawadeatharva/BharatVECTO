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
	public class TorqueConverterViewModel : AbstractComponentViewModel, ITorqueConverterViewModel
	{
		private readonly ObservableCollection<TorqueConverterCharacteristics> _characteristics = new ObservableCollection<TorqueConverterCharacteristics>();
		private CertificationMethod _certificationMethod;

		#region Implementation of IEditComponentTorqueConverterViewModel

		public ITorqueConverterDeclarationInputData ModelData { get { return AdapterFactory.TorqueConverterDeclarationAdapter(this); } }

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
				//return DeclarationData.TorqueConverterCertificationMethods.Select(x => AllowedEntry.Create(x, x.GetLabel())).ToArray();
			}
		}

		public ObservableCollection<TorqueConverterCharacteristics> Characteristics
		{
			get { return _characteristics; }
		}

		#endregion

		protected override void InputDataChanged()
		{
			JobViewModel.InputDataProvider.Switch()
						.If<IDeclarationInputDataProvider>(d => SetValues(d.JobInputData.Vehicle.Components.TorqueConverterInputData))
						.If<IEngineeringInputDataProvider>(e => SetValues(e.JobInputData.Vehicle.Components.TorqueConverterInputData));
		}

		private void SetValues(ITorqueConverterDeclarationInputData tc)
		{
			Model = tc.Model;
			Manufacturer = tc.Manufacturer;
			CertificationNumber = tc.CertificationNumber;
			CertificationMethod = tc.CertificationMethod;
			//ToDo
			//Date = DateTime.Parse(tc.Date);

			var ratio = 1; // TODO!

			//ToDo
			//var lossMap = TorqueConverterDataReader.Create(tc.TCData,
			//												DeclarationData.TorqueConverter.ReferenceRPM, DeclarationData.TorqueConverter.MaxInputSpeed,
			//												ExecutionMode.Declaration, ratio,
			//												DeclarationData.TorqueConverter.CLUpshiftMinAcceleration, DeclarationData.TorqueConverter.CCUpshiftMinAcceleration).Entries.OrderBy(x => x.SpeedRatio);
			//foreach (var entry in lossMap) {
			//	Characteristics.Add(new TorqueConverterCharacteristics(entry));
			//}
		}
	}
}