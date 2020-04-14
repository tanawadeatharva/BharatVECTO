using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Utils;
using VECTO3GUI.ViewModel.Interfaces;

namespace VECTO3GUI.ViewModel.Impl
{
	public class AirdragViewModel : AbstractComponentViewModel, IAirdragViewModel
	{
		private string _manufacturer;
		private string _model;
		private string _certificationNumber;
		private DateTime? _date;
		private SquareMeter _declaredCdxA;
		private bool _useStandardValues;
		private string _appVersion;
		
		private IAirdragDeclarationInputData _airdragData;

		#region Implementation of IAirdragViewModel

		public IAirdragDeclarationInputData ModelData {
			get { return AdapterFactory.AirdragDeclarationAdapter(this); }
		}

		public bool UseMeasuredValues
		{
			get { return _useStandardValues; }
			set { SetProperty(ref _useStandardValues, value); }
		}

		public SquareMeter CdxA_0 { get; set; }
		public SquareMeter TransferredCdxA { get; set; }

		public string Manufacturer
		{
			get { return _manufacturer; }
			set
			{
				if (SetProperty(ref _manufacturer, value)) {
					var changed = _airdragData != null
								? _airdragData.Manufacturer != value
								: value != default(string);
					SetChangedProperty(changed);
				}
			}
		}

		public string Model
		{
			get { return _model; }
			set
			{
				if (SetProperty(ref _model, value)) {
					var changed = _airdragData != null
								? _airdragData.Model != value
								: value != default(string);
					SetChangedProperty(changed);
				}
			}
		}

		public string CertificationNumber
		{
			get { return _certificationNumber; }
			set
			{
				if (SetProperty(ref _certificationNumber, value)) {
					var changed = _airdragData != null
								? _airdragData.CertificationNumber != value
								: value != default(string);
					SetChangedProperty(changed);
				}
			}
		}

		public DateTime? Date
		{ 
			get { return _date; }
			set
			{
				if (SetProperty(ref _date, value)) {
					var changed = _airdragData != null
							? _airdragData.Date != value
							: value != default(DateTime?);
					SetChangedProperty(changed);
				}
			}
		}

		public SquareMeter DeclaredCdxA
		{
			get { return _declaredCdxA; }
			set
			{
				if (SetProperty(ref _declaredCdxA, value)) {
					var changed = _airdragData.AirDragArea != null
								? _airdragData.AirDragArea != value
								: value != default(SquareMeter);
					if (changed)
						SetAirdragArea();
					SetChangedProperty(changed);
				}
			}
		}

		public string AppVersion
		{
			get { return _appVersion; }
			set
			{
				if (SetProperty(ref _appVersion, value)) {
					var changed = _airdragData.AppVersion != null
						? _airdragData.AppVersion != value
						: value != default(string);
					SetChangedProperty(changed);
				}
			}
		}


		#endregion

		protected override void InputDataChanged()
		{
			var inputData = JobViewModel.InputDataProvider as IDeclarationInputDataProvider;
			_airdragData = inputData?.JobInputData.Vehicle.Components.AirdragInputData;
			SetAirdragValues(_airdragData);
		}

		private void SetAirdragValues(IAirdragDeclarationInputData airdrag)
		{
			UseMeasuredValues = airdrag != null;
			if (airdrag == null) {
				return;
			}
			
			Model = airdrag.Model;
			Manufacturer = airdrag.Manufacturer;
			CertificationNumber = airdrag.CertificationNumber;
			Date = airdrag.Date;
			AppVersion = airdrag.AppVersion;
			SetAirdragArea();
		}

		private void SetAirdragArea()
		{
			DeclaredCdxA = _airdragData.AirDragArea;
			CdxA_0 = DeclaredCdxA;
			TransferredCdxA = DeclaredCdxA;
		}

		
		public override bool AnyDataChanges()
		{
			return _changedInput.Count > 0;
		}
	}
}
